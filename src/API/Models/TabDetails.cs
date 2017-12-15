using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ErikEJ.SqlCeScripting
{
    /// <summary>
    /// Class to return table details to be used for modeling/code generation
    /// </summary>
    public class TabDetails
    {
        public TabDetails()
        {
            Columns = new List<Column>();
            PrimaryKeys = new List<PrimaryKey>();
            FkConstraints = new List<Constraint>();
            Indexes = new List<Index>();
        }
        public string Schema;
        public string TableName;
        //public int DiveLevel = 0;   //Experimental in case we recurivly use this structure

        public List<Column> Columns;
        public List<PrimaryKey> PrimaryKeys;
        public List<Constraint> FkConstraints;
        public List<Index> Indexes;
    }
}
