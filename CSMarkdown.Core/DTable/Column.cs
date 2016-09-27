using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMarkdown.DataTable
{
    class Column
    {
        internal Column(DTable table, string name, int Index)
        {

        }

        public DTable Table { get; set; }
        public string Name { get; set; }
        public int Index { get; set; }
    }
}
