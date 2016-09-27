using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMarkdown.DataTable
{
    class DataTable
    {
        private List<Cell> m_cells = new List<Cell>();
        public DataTable()
        {

        }
        public ColumnArray Columns { get; }
        public RowArray Rows { get; }


        public void AddColumn(string name, Func<Row, object> func)
        {
            throw new NotImplementedException();
        }

        public void AddColumnAfter(int afterIndex, string name, Func<Row, object> func)
        {
            throw new NotImplementedException();
        }

        public void AddColumnBefore(int beforeColumnIndex, string name, Func<Row, object> func)
        {
            throw new NotImplementedException();
        }

        public void AddRow(Func<Column, object> func)
        {
            throw new NotImplementedException();
        }

        public void AddRowAfter(int afterIndex, Func<Column, object> func)
        {
            throw new NotImplementedException();
        }

        public void AddRowBefore(int beforeIndex, Func<Column, object> func)
        {
            throw new NotImplementedException();
        }
    }
}
