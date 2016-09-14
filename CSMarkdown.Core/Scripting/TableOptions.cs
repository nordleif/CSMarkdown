using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMarkdown.Scripting
{
    public class TableOptions
    {
        private List<GroupedColumn> _GroupedColumns = new List<GroupedColumn>();
        public string Responsive { get; set; }
        public string Style { get; set; }
        public bool RotateColumns { get; set; }
        public List<GroupedColumn> GroupedColumns
        {
            get { return _GroupedColumns;}
        } 

        public TableOptions()
        {
            Responsive = "bo";
            Style = "table-bordered";
            RotateColumns = false;
        }

        public void GroupedHeaders(string name, params string[] headers)
        {
            GroupedColumn group = new GroupedColumn(name, headers);
            _GroupedColumns.Add(group);
        }

    }

    public class GroupedColumn
    {
        public string Name { get; set; }
        public string[] Headers { get; set; }
        public bool Fit { get; set; } = false;

        public GroupedColumn(string name, string[] headers)
        {
            Name = name;
            Headers = headers;
        }
    }
}
