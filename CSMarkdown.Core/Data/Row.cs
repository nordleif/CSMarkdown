using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMarkdown.Data
{
    public class Row
    {
        private Table m_table;

        internal Row(Table table, int index)
        {
            Table = table;
            Index = index;
        }
        
        public int Index { get; protected set; }

        public Table Table { get; protected set; }
    }
}
