using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReverseEngineer20.ReverseEngineer
{
    public class TypeMapper
    {
        public TypeMapper()
        {
             dict = new Dictionary<string, string>   {
                    { "varchar", "string"},
                    { "decimal", "decimal" },
                    { "float", "double"},
                    { "int", "int"},
                    { "bigint", "long"  },
                    { "datetime", "DateTime" },
                    { "numeric", "decimal" },
                    { "bit", "bool" },
                    { "real", "float" },
                    { "text", "string"},
                    { "binary", "byte[]"},
                    { "char", "char" },
                    { "date", "DateTime" },
                    { "datetime2", "DateTime" },
                    { "datetimeoffset", "DateTimeOffset" },
                    { "image", "byte[]"},
                    { "money", "decimal" },
                    { "nchar", "char" },
                    { "ntext", "string" },
                    { "nvarchar","string"},
                    { "smalldatetime","DateTime" },
                    { "smallint","short"},
                    { "smallmoney", "decimal" },
                    { "sql_variant", "byte[]" },
                    { "sysname", "string" },
                    { "time","TimeSpan" },
                    { "timestamp","byte[]" },
                    { "tinyint", "byte" },
                    { "uniqueidentifier", "Guid" },
                    { "varbinary","byte[]"},
                    { "xml", "string" },
                };
        }
        private Dictionary<string, string> dict;

        public string mapType(string dbType)
        {
            dbType = dbType.ToLower().Trim();
            var ret = "";
            if (!dict.TryGetValue(dbType, out ret))
                ret = "string";
            return ret;
        }
    }
}
