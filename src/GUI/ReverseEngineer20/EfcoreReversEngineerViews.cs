using EntityFrameworkCore.Scaffolding.Handlebars;
using ErikEJ.SqlCeScripting;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.Extensions.DependencyInjection;
using ReverseEngineer20.ReverseEngineer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;  //ss
using System.Text;
using System.Text.RegularExpressions;

namespace ReverseEngineer20
{
    /// <summary>
    /// Sine Efcore design does not have proper poco generator for views, we do it by hand
    /// </summary>
    public class EfcoreReversEngineerViews
    {
        public EfCoreReverseEngineerResult GenerateFiles(ReverseEngineerOptions reverseEngineerOptions, List<ViewDetails> viewDetails, string nameSpace)
        {
            var errors = new List<string>();
            var warnings = new List<string>();
            var reporter = new OperationReporter(
                new OperationReportHandler(
                    m => errors.Add(m),
                    m => warnings.Add(m)));


            ReverseEngineerFiles filePaths = new ReverseEngineerFiles();
            var calcPath = reverseEngineerOptions.ProjectPath;
            if (!string.IsNullOrEmpty(reverseEngineerOptions.OutputPath))
                calcPath = Path.Combine(calcPath, reverseEngineerOptions.OutputPath);
            filePaths.ContextFile = Path.Combine(calcPath, reverseEngineerOptions.ContextClassName + ".cs");
            //Create a file for each view:
            foreach (var viewInfo in viewDetails) {
                try
                {
                    var className = GenerateClassName(viewInfo.ViewName);
                    var fileName = className + ".cs";
                    fileName = Path.Combine(calcPath, fileName);
                    var path = Path.GetDirectoryName(fileName);
                    if (!String.IsNullOrEmpty(path))
                        Directory.CreateDirectory(path);
                    filePaths.EntityTypeFiles.Add(fileName);
                    ViewBuilder vb = new ViewBuilder(fileName, viewInfo, className, nameSpace, reverseEngineerOptions.UseFluentApiOnly);
                    vb.Build();
                }
                catch (Exception e)
                {
                    errors.Add("Error for view " + viewInfo.ViewName + ": " + e.Message);
                }
            }

            //In case we do not have context file.. we must make it
            if (!File.Exists(filePaths.ContextFile))
            {
                var emptyBuilder = new EmtyContextClassBuilder(filePaths.ContextFile, reverseEngineerOptions.ContextClassName, nameSpace, reverseEngineerOptions.UseFluentApiOnly);
                emptyBuilder.Build();
            }
            //Process the context-file to add our views. Pass in calculated context name instead of view name
            var classNameList = new List<ViewDetails>();
            foreach (var w in viewDetails)
            {
                classNameList.Add(new ViewDetails { Columns = w.Columns, Definition = w.Definition, Schema = w.Schema, ViewName = this.GenerateClassName(w.ViewName) });
            }
            var ctBuilder = new ViewContextClassBuilder(filePaths.ContextFile, classNameList, reverseEngineerOptions.UseFluentApiOnly);
            ctBuilder.Build();

            if (!reverseEngineerOptions.IncludeConnectionString)
            {
                PostProcessContext(filePaths.ContextFile);
            }

            var result = new EfCoreReverseEngineerResult
            {
                EntityErrors = errors,
                EntityWarnings = warnings,
                EntityTypeFilePaths = filePaths.EntityTypeFiles,
                ContextFilePath = filePaths.ContextFile,
            };

            return result;
        }

        private void PostProcessContext(string contextFile)
        {
            var finalLines = new List<string>();
            var lines = File.ReadAllLines(contextFile);

            foreach (var line in lines)
            {
                if (line.Trim().StartsWith("#warning To protect"))
                    continue;

                if (line.Trim().StartsWith("optionsBuilder.Use"))
                    continue;

                finalLines.Add(line);
            }
            File.WriteAllLines(contextFile, finalLines, Encoding.UTF8);
        }


        public string GenerateClassName(string value)
        {
            var className = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value);
            var isValid = System.CodeDom.Compiler.CodeDomProvider.CreateProvider("C#").IsValidIdentifier(className);

            if (!isValid)
            {
                // File name contains invalid chars, remove them
                var regex = new Regex(@"[^\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Nd}\p{Nl}\p{Mn}\p{Mc}\p{Cf}\p{Pc}\p{Lm}]", RegexOptions.None, TimeSpan.FromSeconds(5));
                className = regex.Replace(className, "");

                // Class name doesn't begin with a letter, insert an underscore
                if (!char.IsLetter(className, 0))
                {
                    className = className.Insert(0, "_");
                }
            }

            return className.Replace(" ", string.Empty);
        }

    }
}
