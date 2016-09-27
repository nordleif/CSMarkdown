using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMarkdown.DTable
{
    class Column
    {
        internal Column(Table table, string name, int Index)
        {

        }

        public Table Table { get; set; }
        public string Name { get; set; }
        public int Index { get; set; }
    }
}
