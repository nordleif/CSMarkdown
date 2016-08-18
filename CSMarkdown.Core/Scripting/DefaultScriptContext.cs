using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace CSMarkdown.Scripting
{
    public class DefaultScriptContext
        : ScriptContextBase
    {
        public DefaultScriptContext()
        {

        }

        public DataTable ReadSql(string query, string connectionString)
        {
            var dataTable = new DataTable();
            var dataAdapter = new SqlDataAdapter(query, connectionString);
            
            var parameters = (IDictionary<string, object>)p;
            foreach (var parameter in parameters)
                dataAdapter.SelectCommand.Parameters.AddWithValue(parameter.Key, parameter.Value != null ? parameter.Value : DBNull.Value);
            
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

        public void RenderChart(DataTable data, object options)
        {
            // TODO:
            // Nye smd eksempler + unit test
            // Gamle unit test må ikke overskrives
            //  1. Vis graf
            //  2. Vis værdier ved mouse over
            //  3. Vis graf med 2 legends
            //  4. Vis forskellige chart types -> line, bar etc
            //  5. x akse som dato
            //  6. x akse som tal
            //
            //  
            //  a. max/min værdier på x og y
            //  b. flere legends :-)
        }
    }
}
