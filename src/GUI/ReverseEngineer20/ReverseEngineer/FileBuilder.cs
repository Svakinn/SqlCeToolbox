using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReverseEngineer20.ReverseEngineer
{
    /// <summary>
    /// Support to write code to file using stringbuilder
    /// </summary>
    public class FileBuilder
    {
        public FileBuilder(string fileName)
        {
            _sb = new StringBuilder();
            this.fileName = fileName;
        }
        public string fileName;

        public const string tab = "\t";
        public int tabCnt = 0;
        private string currTab = "";
        public StringBuilder _sb;

        
        /// <summary>
        /// Increase tab indent
        /// </summary>
        public void AddTab()
        {
            currTab += tab;
        }

        /// <summary>
        /// Decrease tab indent
        /// </summary>
        public void RemTab()
        {
            currTab = currTab.Substring(1);
        }

        /// <summary>
        /// write to string without end line and tab indent
        /// </summary>
        /// <param name="str"></param>
        public void w(string str)
        {
            _sb.Append(str);
        }

        /// <summary>
        /// Write tab indent only (withiout end line)
        /// </summary>
        public void wTab()
        {
            _sb.Append(currTab);
        }

        /// <summary>
        /// Write line with tab indent
        /// </summary>
        /// <param name="str"></param>
        public void wl(string str)
        {
            _sb.Append(currTab);
            _sb.Append(str);
            _sb.Append("\r\n");
        }

        /// <summary>
        /// Write line without tab indent
        /// </summary>
        /// <param name="str"></param>
        public void wln(string str)
        {
            _sb.Append(str);
            _sb.Append("\r\n");
        }

        /// <summary>
        /// Save the file to disk
        /// </summary>
        public void Save()
        {
            File.WriteAllText(fileName, _sb.ToString(), Encoding.UTF8);
        }

    }

}
