using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMarkdown.Data
{
    public class Column
    {
        internal Column(Table table, int index, string name)
        {
            Table = table;
            Index = index;
            Name = name;
        }

        public string Name { get; set; }

        public int Index { get; protected set; }

        public Table Table { get; protected set; }
    }
}
