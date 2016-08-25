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

        private int m_numberOfLegends = 1;
        private int m_chartCounter = 0;
        private string m_chartXAxisType;
        private int m_yDataColumnIndex = 1;

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
            tableNode.Attributes.Add("class", "responsive");

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

        /*public void RenderTable(DataTable data)
        {
            if (data == null)
                return;
            var tableNode = HtmlNode.CreateNode("<table>");
            tableNode.Attributes.Add("class","responsive");

            var rowHeaderNode = HtmlNode.CreateNode("<tr>");
            foreach (DataColumn column in data.Columns)
            {
                rowHeaderNode.AppendChild(HtmlNode.CreateNode($"<th>{column.ColumnName.Trim()}"));
                
            }
            tableNode.AppendChild(rowHeaderNode);

            HtmlNode rowNode;
            foreach (DataRow row in data.Rows)
            {
                rowNode = HtmlNode.CreateNode("<tr>");
                foreach (DataColumn column in data.Columns)
                {
                    rowNode.AppendChild(HtmlNode.CreateNode($"<td>{row[column]}"));
                }
                tableNode.AppendChild(rowNode);
            }
            CurrentNode.ChildNodes.Add(tableNode);

        }*/

        public void RenderChart(DataTable data, ChartOptions options = null)
        {

            if (data == null || data.Rows.Count == 0)
            {
                data = new DataTable();
                data = FillEmptyDataTable();
            }

            if (options == null)
            {
                options = CreateDefaultChartOptions(data);
            }

            else if (options.Legends.Count == 0)
            {
                data = CreateEmptyChartLegend();
                options.Legends = CreateUndefinedLegends(data, options.Legends);
            }

            foreach (var legendType in options.Legends)
            {
                legendType.Values = MakeValuesDictionary(data, options.XDataName, legendType.YDataName, options, legendType);
            }

            if (options.XAxisType == "")
            {
                options.XAxisType = m_chartXAxisType;
            }

            string datasetString = "";

            foreach (var legend in options.Legends)
            {
                datasetString = CreateDataSet(legend, datasetString, options);
            }



            var divNode = HtmlNode.CreateNode("<div>");
            divNode.Attributes.Add("id", "chart" + m_chartCounter);
            CurrentNode.ChildNodes.Add(divNode);

            var svgNode = HtmlNode.CreateNode("<svg>");
            divNode.ChildNodes.Add(svgNode);

            var scriptNode = HtmlNode.CreateNode("<script>");

            if (options.ChartModelType == "pie" || options.ChartModelType == "donut")
                scriptNode.InnerHtml = datasetString + MakeNVAddPieAndDonutChart(options);
            else
                scriptNode.InnerHtml = datasetString + MakeNVAddGraphFunction(options);
            CurrentNode.ChildNodes.Add(scriptNode);


            m_chartCounter++;
            m_numberOfLegends = 1;
            m_yDataColumnIndex = 1;
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

        private DataTable CreateEmptyChartLegend()
        {
            DataTable data = new DataTable();
            DataColumn column = new DataColumn();
            DataRow row;

            column = new DataColumn();
            column.DataType = Type.GetType("System.Int32");
            column.ColumnName = "x";
            data.Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.Int32");
            column.ColumnName = "y";
            data.Columns.Add(column);

            for (int i = 0; i < 10; i++)
            {
                row = data.NewRow();
                row["x"] = i;
                row["y"] = 0;
                data.Rows.Add(row);
            }

            return data;
        }

        private List<BaseLegend> CreateUndefinedLegends(DataTable data, List<BaseLegend> listOfLegends)
        {
            List<BaseLegend> defaultListOfLegends = listOfLegends;
            int columnsCount = data.Columns.Count;

            for (int i = 1; i < columnsCount; i++)
            {
                defaultListOfLegends.Add(new LineLegend() { Key = data.Columns[i].ColumnName });
            }

            return defaultListOfLegends;
        }

        private DataTable FillEmptyDataTable()
        {
            DataTable newDataTable = new DataTable();
            DataColumn column;
            DataRow row;

            column = new DataColumn();
            column.DataType = typeof(DateTime);
            column.ColumnName = "data";
            newDataTable.Columns.Add(column);

            column = new DataColumn();
            column.DataType = typeof(int);
            column.ColumnName = "No Values Received";
            newDataTable.Columns.Add(column);

            //DateTime date;
            for (int i = 1; i < 6; i++)
            {
                row = newDataTable.NewRow();
                row[0] = new DateTime(1970, 01, i, 0, 0, 0);
                row[1] = 0;
                newDataTable.Rows.Add(row);
            }
            WriteWarning("No data were defined for creating chart");
            return newDataTable;
        }

        private ChartOptions CreateDefaultChartOptions(DataTable data)
        {
            ChartOptions defaultChartOptions = new ChartOptions();
            int columnCounter;
            if (data.Columns.Count > 3)
                columnCounter = 3;

            else
                columnCounter = data.Columns.Count;

            for (int i = 1; i < columnCounter; i++)
            {
                defaultChartOptions.Legends.Add(new LineLegend() { Key = data.Columns[i].ColumnName });
            }

            return defaultChartOptions;
        }

        private string CreateDataSet(dynamic lineLegend, string dataset, ChartOptions options)
        {
            if (m_numberOfLegends == 1)
            {
                dataset = "\nvar dataset" + m_chartCounter + " = [";
            }
            else if (m_numberOfLegends > 1)
            {
                dataset = dataset.Remove(dataset.Length - 1);
                dataset += ", ";
            }
            if (lineLegend.Type != "pie" && lineLegend.Type != "donut")
            {
                dataset += "{\"key\": \"";
                dataset += lineLegend.Key;
                dataset += "\", \"values\": [";
            }

            int i = 0;
            foreach (var dataCombination in lineLegend.Values)
            {
                if (options.XAxisType == "string" && lineLegend.Type != "pie")
                    dataset += "{" + dataCombination.Key + "," + dataCombination.Value + ", label: \"" + options.XAxisLabels.ElementAt(i++) + "\"},";
                else
                    dataset += "{" + dataCombination.Key + "," + dataCombination.Value + "},";

            }
            dataset = dataset.Remove(dataset.Length - 1);
            if (lineLegend.Type != "pie" && lineLegend.Type != "donut")
                dataset += "], \"type\": \"" + lineLegend.Type + "\", \"yAxis\": " + m_numberOfLegends.ToString() + "}]";
            else
                dataset += "]";
            m_numberOfLegends++;

            if (lineLegend.Type == "pie" || lineLegend.Type == "donut")
            {
                options.ChartModelType = lineLegend.Type;
            }

            return dataset;
        }

        private Dictionary<string, string> MakeValuesDictionary(DataTable dTable, string xDataColumnName, string yDataColumnName, ChartOptions options, dynamic legend)
        {
            int xDataColumnIndex = 0;
            if (xDataColumnName != "")
                xDataColumnIndex = dTable.Columns.IndexOf(xDataColumnName);

            bool yDataColumnNameWasNotDefined = true;
            if (yDataColumnName != "")
            {
                m_yDataColumnIndex = dTable.Columns.IndexOf(yDataColumnName);
                yDataColumnNameWasNotDefined = false;
            }

            List<string> labels = new List<string>();
            Dictionary<string, string> dicOfValues = new Dictionary<string, string>();
            DateTime date = new DateTime();
            string xValue;
            var epoch = new DateTime(1970, 01, 01, 0, 0, 0);
            for (int i = 0; i < dTable.Rows.Count; i++)
            {
                if (legend.Type == "pie" || legend.Type == "donut")
                {
                    xValue = "\"" + dTable.Rows[i].ItemArray[xDataColumnIndex].ToString() + "\"";
                    dicOfValues.Add("\"x\":" + xValue, "\"y\":" + dTable.Rows[i].ItemArray[m_yDataColumnIndex].ToString().Replace(',', '.'));
                }
                else if (dTable.Rows[i].ItemArray[xDataColumnIndex].GetType() == typeof(string) || options.XAxisType.ToLower() == "string")
                {
                    xValue = (i + 1).ToString();

                    int LabelColumn = 0;
                    if (options.LabelColumn != null)
                        LabelColumn = dTable.Columns.IndexOf(options.LabelColumn);
                    //options.XAxisLabels.Add(Convert.ToString(dTable.Rows[i].ItemArray[LabelColumn]));
                    labels.Add(dTable.Rows[i].ItemArray[LabelColumn].ToString());
                    dicOfValues.Add("\"x\":" + xValue, "\"y\":" + dTable.Rows[i].ItemArray[m_yDataColumnIndex].ToString().Replace(',', '.'));
                    m_chartXAxisType = "string";
                }

                else if (dTable.Rows[i].ItemArray[xDataColumnIndex].GetType() == typeof(DateTime) || options.XAxisType.ToLower() == "date")
                {

                    date = Convert.ToDateTime(dTable.Rows[i].ItemArray[xDataColumnIndex]).ToUniversalTime();
                    long ticker = date.Ticks - epoch.Ticks;
                    xValue = ticker.ToString().Remove(ticker.ToString().Length - 4);
                    dicOfValues.Add("\"x\":" + xValue, "\"y\":" + dTable.Rows[i].ItemArray[m_yDataColumnIndex].ToString().Replace(',', '.'));
                    m_chartXAxisType = "date";
                }
                else
                {

                    xValue = dTable.Rows[i].ItemArray[xDataColumnIndex].ToString().Replace(',', '.');
                    dicOfValues.Add("\"x\":" + xValue, "\"y\":" + dTable.Rows[i].ItemArray[m_yDataColumnIndex].ToString().Replace(',', '.'));
                    m_chartXAxisType = "int";
                }

            }
            options.XAxisLabels = labels;

            if (yDataColumnNameWasNotDefined)
                m_yDataColumnIndex++;

            return dicOfValues;
        }


        private string MakeNVAddPieAndDonutChart(ChartOptions options)
        {
            string addGraphFunction = ";\n nv.addGraph(function() {var chart = nv.models.pieChart()\n"
                + ".x(function(d) {return d.x})\n"
                + ".y(function(d) {return d.y})\n"
                + ".showLabels(" + options.ShowLabels + ")";
            if (options.ChartModelType == "donut")
            {
                addGraphFunction += ".labelThreshold(" + options.LabelThreshold + ")\n"
                    + ".labelType(\"" + options.LabelType + "\")\n"
                    + ".donut(true)\n"
                    + ".donutRatio(" + options.DonutRatio + ");\n";
            }
            else
                addGraphFunction += ";\n";

            addGraphFunction += "d3.select('#chart" + m_chartCounter + " svg')\n"
                + ".datum(dataset" + m_chartCounter + ")\n"
                + ".transition().duration(350)\n"
                + ".call(chart);\n"
                + "return chart;\n});";
            return addGraphFunction;

        }

        private string CreatePieData(dynamic lineLegend, string dataset)
        {
            if (m_numberOfLegends == 1)
            {
                dataset = "\nvar dataset" + m_chartCounter + " = [";
            }

            foreach (var dataCombination in lineLegend.Values)
            {
                dataset += "{" + dataCombination.Key + "," + dataCombination.Value + "},";

            }
            dataset = dataset.Remove(dataset.Length - 1);
            dataset += "]";

            m_numberOfLegends++;
            return dataset;
        }

        private string MakeNVAddGraphFunction(ChartOptions options)
        {
            string addGraphFunction = ";\nnv.addGraph( function() { var chart = nv.models." + options.ChartModelType
                + ".margin({top: " + options.MarginTop + ", right: " + options.MarginRight + ", bottom: "
                + options.MarginBottom + ", left: " + options.MarginLeft + "})\n" //.height(450)
                + ".useInteractiveGuideline(true)\n"
                + ".color(d3.scale.category10().range());\n";

            //HVIS DATATABLE ER TOMT, VIL DEN FEJLE VED NEDSTÅENDE MED xAxisType, DA DEN IKKE ER BLEVET SAT TIL NOGET
            //TIDLIGERE I PROGRAMMET. HVILKET VIL SIGE AT DEN PT ER SAT TIL null

            if (options.XAxisType.ToLower() == "int")
            {
                addGraphFunction += "chart.xAxis.tickFormat(d3.format(',r')).staggerLabels(true);\n";
            }
            else if (options.XAxisType.ToLower() == "string")
            {
                //Til denne skal der laves ændringer andre steder først.
                //Hvis man skal bruge en string til at definere x axen, skal en datatable gerne bestå af 3 columns.
                //Der skal så i MakeLineValuesDictionary tjekkes om der er 3 columns, og hvis der er, skal den lave
                //en value mere i dictionariet, hvor den laver en værdi der hedder label:, så det kommer til at hede
                //{"y": [værdi], "x": [værdi], "label": [string værdi]}.
                // -Kig evt på dette link: http://channagayan.blogspot.dk/2014/08/adding-string-values-to-nvd3-line-chart.html

                addGraphFunction += "chart.xAxis.axisLabel('Date').tickFormat(function(d) {var label = dataset" + m_chartCounter + "[0].values[--d].label; return label}).staggerLabels(true);\n";
            }
            else if (options.XAxisType.ToLower() == "date")
            {
                if (options.XAxisDateFormat != null)
                {
                    CustomDefinedDateConverter dateConverter = new CustomDefinedDateConverter();
                    string newDateFormat = dateConverter.ConvertDateFormat(options.XAxisDateFormat);
                    addGraphFunction += "chart.xAxis.tickFormat(function(d) {return d3.time.format('" + newDateFormat + "')(new Date(d))}).staggerLabels(true);\n";
                }
                else
                    addGraphFunction += "chart.xAxis.tickFormat(function(d) {return d3.time.format('%d.%m.%Y - %H:%M:%S')(new Date(d))}).staggerLabels(true);\n";
            }

            addGraphFunction += "chart.yAxis1.tickFormat(d3.format(',.0f'));\n";

            if (options.Legends.Count > 1)
            {
                addGraphFunction += "chart.yAxis2.tickFormat(d3.format(',.1f'));\n";
            }
            int legendCounter = 1;
            foreach (var legend in options.Legends)
            {
                if (legend.MinValue.HasValue && legend.MaxValue.HasValue)
                {
                    addGraphFunction += "chart.yDomain" + legendCounter + "([" + legend.MinValue + ", " + legend.MaxValue + "]);\n";
                }
                else if (legend.MinValue.HasValue && !legend.MaxValue.HasValue)
                {
                    double highestValue = 0;
                    foreach (var value in legend.Values)
                    {
                        double currentValue = Convert.ToDouble(value.Value.Remove(0, 4).Replace(".", ","));
                        if (highestValue < currentValue)
                            highestValue = currentValue;
                    }
                    addGraphFunction += "chart.yDomain" + legendCounter + "([" + legend.MinValue + ", " + Convert.ToString(highestValue).Replace(",", ".") + "]);\n";
                }
                else if (!legend.MinValue.HasValue && legend.MaxValue.HasValue)
                {
                    double lowestValue = 0;
                    foreach (var value in legend.Values)
                    {
                        double currentValue = Convert.ToDouble(value.Value.Remove(0, 4).Replace(".", ","));
                        if (lowestValue > currentValue)
                            lowestValue = currentValue;
                    }
                    addGraphFunction += "chart.yDomain" + legendCounter + "([" + Convert.ToString(lowestValue).Replace(",", ".") + ", " + legend.MaxValue + "]);\n";
                }
            }
            addGraphFunction += "d3.select('#chart" + m_chartCounter + " svg')\n"
                + ".datum(dataset" + m_chartCounter + ")\n"
                + ".transition().duration(500).call(chart); nv.utils.windowResize(chart.update);\n"
                + "return chart;\n"
                + "});";
            return addGraphFunction;
        }
    }
}
