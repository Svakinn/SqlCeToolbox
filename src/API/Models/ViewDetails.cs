using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ErikEJ.SqlCeScripting
{
    /// <summary>
    /// Class to return vie details to be used for modeling/code generation
    /// </summary>
    public class ViewDetails
    {
        public ViewDetails()
        {
            Columns = new List<Column>();
            //PrimaryKeys = new List<PrimaryKey>();
           // Indexes = new List<Index>();
           // RefTables = new List<TabDetails>();
            //RefViews = new List<ViewDetails>();
        }
        public string Schema;
        public string ViewName;
        public string Definition;
        //public int DiveLevel = 0;   //Experimental in case we recurivly use this structure

        public List<Column> Columns;
        //public List<PrimaryKey> PrimaryKeys; //experimental
        //public List<Index> Indexes; //experimental
        //public List<TabDetails> RefTables; //experimental - option to link view to tables it is composed of
        //public List<ViewDetails> RefViews; //experimental - option to link view to views it is composed of

    }
}
