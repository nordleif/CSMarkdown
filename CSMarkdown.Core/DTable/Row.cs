using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMarkdown.DTable
{
    class Row
    {
        internal Row(Table table, int Index)
        {

        }

        public Table Table { get; set; }
        public int Index { get; set; }
    }
}
