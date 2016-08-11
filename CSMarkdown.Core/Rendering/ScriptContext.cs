using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace CSMarkdown.Rendering
{
    public class ScriptContext 
    {
        public ScriptContext()
        {

        }

        public HtmlNode CurrentNode { get; set; }
        
        public CodeChunkOptions Options { get; set; }

        public DataTable ReadSql(string query, string connectionString)
        {
            var dataTable = new DataTable();
            var dataAdapter = new SqlDataAdapter(query, connectionString);
            dataAdapter.Fill(dataTable);

            return dataTable;
        }
        
        public void RenderTable(DataTable data)
        {
            if (data == null)
                return;

            var tableNode = HtmlNode.CreateNode("<table>");
            tableNode.Attributes.Add("class", "table");

            var headNode = tableNode.AppendChild(HtmlNode.CreateNode("<thead>"));
            var bodyNode = tableNode.AppendChild(HtmlNode.CreateNode("<tbody>"));

            foreach (DataColumn column in data.Columns)
                headNode.AppendChild(HtmlNode.CreateNode($"<th>{column.ColumnName.Trim()}"));

            foreach (DataRow row in data.Rows)
            {
                var rowNode = bodyNode.AppendChild(HtmlNode.CreateNode("<tr>"));
                foreach (DataColumn column in data.Columns)
                    rowNode.AppendChild(HtmlNode.CreateNode($"<td data-label=\"{column.ColumnName.Trim()}\">{row[column]}"));
            }

            CurrentNode.ChildNodes.Add(tableNode);
        }

        public void WriteMessage(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            if (Options.ReadValue<bool>("message", true))
                CurrentNode.AppendChild(HtmlNode.CreateNode($"<pre><code class=\"message\">## {text}</code></pre>"));

        }

        public void WriteWarning(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            if (Options.ReadValue<bool>("warning", false))
                CurrentNode.AppendChild(HtmlNode.CreateNode($"<pre><code class=\"warning\">## {text}</code></pre>"));
        }
    }
}
