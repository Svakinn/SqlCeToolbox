using ErikEJ.SqlCeScripting;
using System;

namespace ReverseEngineer20.ReverseEngineer
{
    /// <summary>
    /// Build EntytyType for view and save to disk
    /// </summary>
    public class EmtyContextClassBuilder : FileBuilder
    {
        public EmtyContextClassBuilder(string fileName,  string className, string nameSpace, bool onlyFluentApi) : base(fileName)
        {
            this.className = className;
            this.nameSpace = nameSpace;
            this.onlyFluentApi = onlyFluentApi;
        }

        private string className;
        private string nameSpace;
        private bool onlyFluentApi;

        public void Build()
        {
            wl("using System;");
            wl("using Microsoft.EntityFrameworkCore;");
            wl("using Microsoft.EntityFrameworkCore.Metadata;");
            wl("");
            wl("namespace " + nameSpace);
            wl("{");
            AddTab();
            wl("public partial class "+className+" : DbContext");
            wl("{");
            AddTab();
            wl("");
            RemTab();
            wl("}");
            wl("protected override void OnModelCreating(ModelBuilder modelBuilder)");
            wl("{");
            AddTab();
            wl("");
            RemTab();
            wl("}");
            RemTab();
            wl("}");
            Save();
        }
    }
}