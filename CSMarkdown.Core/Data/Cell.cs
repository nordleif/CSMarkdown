using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMarkdown.Data
{
    public class Cell
    {
        internal Cell(Table table, Row row, Column column, object value)
        {
            Table = table;
            Row = row;
            Column = column;
            Value = value;
        }

        public Column Column { get; protected set; }

        public Row Row { get; protected set; }

        public Table Table { get; protected set; }

        public object Value { get; protected set; }
    }
}
