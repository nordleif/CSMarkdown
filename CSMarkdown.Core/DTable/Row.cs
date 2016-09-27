using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMarkdown.DataTable
{
    class Row
    {
        internal Row(DTable table, int Index)
        {

        }

        public DTable Table { get; set; }
        public int Index { get; set; }

    }
}
