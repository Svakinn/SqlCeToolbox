using ErikEJ.SqlCeScripting;
using System;
using System.Collections.Generic;
using System.IO;

namespace ReverseEngineer20.ReverseEngineer
{
    /// <summary>
    /// Add contexts for the view into the ContextClass File.
    /// NOTE: if the existing context file has been edited, the inject may fail
    /// For example we require OnModelCreating method to be present
    /// </summary>
    public class ViewContextClassBuilder : FileBuilder
    {
        public ViewContextClassBuilder(string fileName, List<ViewDetails> views,   bool onlyFluentApi) : base(fileName)
        {
            this.viewDetails = views;
            this.onlyFluentApi = onlyFluentApi;
        }


        private bool onlyFluentApi;
        private List<ViewDetails> viewDetails;

        public void Build()
        {
            string currentFile = File.ReadAllText(this.fileName);
            var lines = File.ReadAllLines(this.fileName);
            bool classBeginFound = false;
            bool classBeginReady = false;
            bool classInsertDone = false;
            bool ignoreBeginFound = false;
            bool ignoreBeginReady = false;
            bool ignoreInsertDone = false;
            bool ignoreNeedToClose = false;
            foreach (var line in lines)
            {
                if (!classBeginFound)
                {
                    classBeginFound = line.LastIndexOf(" : DbContext") > -1;
                    wln(line);
                }
                else if (!classInsertDone && !classBeginReady)
                {
                    wln(line);
                    classBeginReady = line.LastIndexOf("{") > -1;
                }
                else if (!classInsertDone && classBeginReady)
                {
                    //Here we inject our views
                    AddTab();
                    AddTab();
                    foreach (var view in viewDetails)
                    {
                        //Note: the ViewName has been converted to the class name
                        var chkStr = "public virtual DbSet<"+view.ViewName+">";
                        if (currentFile.LastIndexOf(chkStr) == -1)
                        {
                            var mName = Inflector.Inflector.Pluralize(view.ViewName);
                            wl("public virtual DbSet<" + view.ViewName + "> "+mName+ " { get; set; }");
                        }
                    }
                    wln(line);
                    classInsertDone = true;
                }
                else
                {
                    //Next phase is to inject Ignore statement for the view in OnModelCreating
                    if (!ignoreBeginFound)
                    {
                        ignoreBeginFound = line.LastIndexOf(" OnModelCreating(ModelBuilder") > -1;
                        wln(line);
                    }
                    else if (!ignoreInsertDone && !ignoreBeginReady)
                    {
                        //Handling the special case of the OnModelCreating is empty (left this way when modelGenerator generates no tables)
                        ignoreBeginReady = line.LastIndexOf("{") > -1;
                        if (ignoreBeginReady && line.LastIndexOf("{}") > -1)
                        {
                            RemTab();
                            wl("{");
                            AddTab();
                            ignoreNeedToClose = true;
                        }
                        else
                            wln(line);
                    }
                    else if (!ignoreInsertDone && ignoreBeginReady)
                    {
                        foreach (var view in viewDetails)
                        {
                            //Note: the ViewName has been converted to the class name
                            //var chkStr = "modelBuilder.Ignore<" + view.ViewName + ">()";
                            var chkStr = "modelBuilder.Entity<"+view.ViewName+">";
                            if (currentFile.LastIndexOf(chkStr) == -1)
                            {
                                //wl("if (IsMigration)");
                                //AddTab();
                                //wl("modelBuilder.Ignore<" + view.ViewName + ">();");
                                //RemTab();
                                wl("modelBuilder.Entity<" + view.ViewName + ">(); ");
                            }
                        }
                        wln(line);
                        ignoreInsertDone = true;
                        if (ignoreNeedToClose)
                        {
                            RemTab();
                            wl("}");
                            AddTab();
                        }
                    }
                    else
                        wln(line);
                }
            }
            Save();
        }
    }
}