using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMarkdown.DataTable
{
    class DTable
    {
        private List<Cell> m_cells = new List<Cell>();
        public DTable()
        {

        }

        public Columns Columns { get; }
        public Rows Rows { get; }

        public Cell this[Column column, Row row]
        {
            get { return null; }
        }

        public Cell this[int columnIndex, int rowIndex]
        {
            get { return null; }
        }

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

        public void AddColumnAfter(Column afterColumn, string name, Func<Row, object> func)
        {
            AddColumnAfter(afterColumn.Index, name, func);
        }

        public void AddColumnBefore(Column beforeColumn, string name, Func<Row, object> func)
        {
            AddColumnBefore(beforeColumn.Index, name, func);
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

        public void AddRowAfter(Row afterRow, Func<Column, object> func)
        {
            AddRowAfter(afterRow.Index, func);
        }

        public void AddRowBefore(Row beforeRow, Func<Column, object> func)
        {
            AddRowBefore(beforeRow.Index, func);
        }

        public void Create(System.Data.DataTable datatable)
        {
            throw new NotImplementedException();
        }
    }
}
