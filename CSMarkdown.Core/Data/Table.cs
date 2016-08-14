using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMarkdown.Data
{
    public class Table
    {
        #region Static Members

        public Table Create(DataTable dataTable)
        {
            if (dataTable == null)
                throw new ArgumentNullException(nameof(dataTable));

            var table = new Table();

            var columns = new Dictionary<int, Column>();
            for (var columnIndex = 0; columnIndex < dataTable.Columns.Count; columnIndex++)
                columns.Add(columnIndex, new Column(table, columnIndex, dataTable.Columns[columnIndex].ColumnName));
            
            for(var rowIndex = 0; rowIndex < dataTable.Rows.Count; rowIndex ++)
            {
                var row = new Row(table, rowIndex);
                for (var columnIndex = 0; columnIndex < dataTable.Columns.Count; columnIndex++)
                {
                    var value = dataTable.Rows[rowIndex][columnIndex];
                    if (value == DBNull.Value)
                        value = null;
                    
                    table.m_cells.Add(new Cell(table, row, columns[columnIndex], value));
                }
            }
            
            return table;
        }
        
        #endregion

        private List<Cell> m_cells = new List<Cell>();

        public Table()
        {
            
        }

        public Cell[] Cells
        {
            get { return m_cells.ToArray(); }
        }

        public Column[] Columns
        {
            get { return m_cells.Select(c => c.Column).Distinct().OrderBy(c => c.Index).ToArray(); }
        }

        public Row[] Rows
        {
            get { return m_cells.Select(c => c.Row).Distinct().OrderBy(r => r.Index).ToArray(); }
        }

        public Cell this[Column column, Row row]
        {
            get { return m_cells.FirstOrDefault(c => c.Column == column && c.Row == row); }
        }

        public Cell this[int columnIndex, int rowIndex]
        {
            get { return m_cells.FirstOrDefault(c => c.Column.Index == columnIndex && c.Row.Index == rowIndex); }
        }
        
        public Table InsertColumnAfter(Column afterColumn, string name, Func<Table, Row, object> func)
        {
            return InsertColumnAfter(afterColumn.Index, name, func);
        }

        public Table InsertColumnAfter(string afterColumnName, string name, Func<Table, Row, object> func)
        {
            var column = m_cells.Select(c => c.Column).FirstOrDefault(c => c.Name.Equals(afterColumnName, StringComparison.InvariantCultureIgnoreCase));
            if (column == null)
                column = Columns.Last();
            return InsertColumnAfter(column.Index, name, func);
        }
        
        public Table InsertColumnAfter(int afterIndex, string name, Func<Table, Row, object> func)
        {
            throw new NotImplementedException();
        }

        public Table InsertColumnBefore(Column beforeColumn, string name, Func<Table, Row, object> func)
        {
            return InsertColumnAfter(beforeColumn.Index, name, func);
        }

        public Table InsertColumnBefore(string beforeColumnName, string name, Func<Table, Row, object> func)
        {
            var column = m_cells.Select(c => c.Column).FirstOrDefault(c => c.Name.Equals(beforeColumnName, StringComparison.InvariantCultureIgnoreCase));
            if (column == null)
                column = Columns.Last();
            return InsertColumnAfter(column.Index, name, func);
        }

        public Table InsertColumnBefore(int beforeColumnIndex, string name, Func<Table, Row, object> func)
        {
            throw new NotImplementedException();
        }
        
        public Table Clone()
        {
            var newTable = new Table();
            var newColumns = m_cells.Select(c => c.Column).Distinct().OrderBy(c => c.Index).Select(c => new Column(newTable, c.Index, c.Name)).ToDictionary(c => c.Index);
            var newRows = m_cells.Select(c => c.Row).Distinct().OrderBy(r => r.Index).Select(r => new Row(newTable, r.Index)).ToDictionary(r => r.Index);
            newTable.m_cells = m_cells.Select(c => new Cell(newTable, newRows[c.Row.Index], newColumns[c.Column.Index], c.Value)).ToList();
            return newTable;
        }
    }
}
