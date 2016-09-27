using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMarkdown.DTable
{
    class Cell
    {
        internal Cell(Table table, Column column, Row row, object value)
        {

        }

        public Column Column { get; set; }
        public Row Row { get; set; }
        public Table Table { get; set; }
        public object Value { get; set; }

        public string BackgroundColor { get; set; }
        public string FontColor { get; set; }
        public string FontStyle { get; set; }
    }
}
