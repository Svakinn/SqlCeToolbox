using ErikEJ.SqlCeScripting;
using System;

namespace ReverseEngineer20.ReverseEngineer
{
    /// <summary>
    /// Build EntytyType for view and save to disk
    /// </summary>
    public class ViewBuilder : FileBuilder
    {
        public ViewBuilder(string fileName, ViewDetails viewDetail, string className, string nameSpace, bool onlyFluentApi) : base(fileName)
        {
            this.viewDetail = viewDetail;
            this.className = className;
            this.nameSpace = nameSpace;
            this.onlyFluentApi = onlyFluentApi;
            this.typeMapper = new TypeMapper();
        }

        private ViewDetails viewDetail;
        private string className;
        private string nameSpace;
        private bool onlyFluentApi;
        private TypeMapper typeMapper;

        public void Build()
        {
            wl("//Note: we cannot determine key fields for views, do some guesswork at best. You must thererore review the class and set the approprate key fields.");
            wl("//In case the view does not have unique keys, you must set keys on all the fields below.");
            if (!onlyFluentApi)
                wl("//Below we did set a key field on the first view column that is not nullable.");
            wl("");
            wl("using System;");
            wl("using System.ComponentModel.DataAnnotations;");
            wl("using System.ComponentModel.DataAnnotations.Schema;");
            wl("");
            wl("namespace " + nameSpace);
            wl("{");
            AddTab();
            wl("[Table(\""+ viewDetail.ViewName+ "\", Schema = \""+viewDetail.Schema+"\")]");
            wl("public partial class " + className);
            wl("{");
            AddTab();
            var keyFound = false;
            foreach (var column in viewDetail.Columns)
            {
                if (!onlyFluentApi && !keyFound && column.IsNullable != YesNoOption.YES)
                {
                    wl("[Key]");
                    keyFound = true;
                }
                var mapTpe = typeMapper.mapType(column.DataType);
                if (mapTpe == "string" && column.CharacterMaxLength > 0)
                    wl("[StringLength(" + Convert.ToString(column.CharacterMaxLength) + ")]");
                var colName = column.ColumnName.Replace(" ", "");
                colName = colName.Substring(0, 1).ToUpper() + colName.Substring(1);
                if (colName != column.ColumnName)
                    wl(" [Column(\"" + column.ColumnName + "\")]");
                //Perhaps this is not needed for views..
                //if ( (mapTpe != "string" && mapTpe != "int" && mapTpe != "long" ))
                //  wl("[Column(\" TypeName = " + column.DataType+"\")]"); 
                wl("public "+mapTpe+(column.IsNullable == YesNoOption.YES && mapTpe != "string" ? "?" : "")+" "+ colName+" { get; set; }");
            }
            RemTab();
            wl("}");
            RemTab();
            wl("}");
            Save();
        }
    }
}