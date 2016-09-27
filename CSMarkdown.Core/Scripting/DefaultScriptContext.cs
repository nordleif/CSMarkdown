using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ClosedXML.Excel;
using System.IO;
using CSMarkdown.Rendering;

namespace CSMarkdown.Scripting
{
    public class DefaultScriptContext
        : ScriptContextBase
    {

        private int m_numberOfLegends = 1;
        private int m_chartCounter = 0;
        //private string m_chartXAxisType;
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

        private DataTable ExtractTagTable(string query, string connectionString)
        {
            var dataTable = new DataTable();
            var dataAdapter = new SqlDataAdapter(query, connectionString);

            dataAdapter.Fill(dataTable);

            return dataTable;
        }

        public DataTable ReadTags(string connectionString, Interval interval, DateTime from, DateTime to, params string[] tags)
        {
            var dTable = new DataTable();
            var intervalForQuery = "";
            var formatString = "";

            if (interval == Interval.Data)
            {
                intervalForQuery = "data";
                //Hvordan skal det håndteres ved data?.
                //Hvis der skal laves en ny metode til data, skal understående CreateBaseDataTable's blive hvor de er
                //Men kan samme metode bruges, kan CreateBaseDataTable naturligvis blot skrives en gang og sættes udenfor if statementsne
            }
            else if (interval == Interval.Hour)
            {
                intervalForQuery = "hourdata";
                formatString = "yyyy-MM-dd HH";
                dTable = CreateBaseDataTable(interval, from, to, tags);
            }
            else if (interval == Interval.Day)
            {
                intervalForQuery = "daydata";
                formatString = "yyyy-MM-dd";
                dTable = CreateBaseDataTable(interval, from, to, tags);
            }
            else if (interval == Interval.Month)
            {
                intervalForQuery = "monthdata";
                formatString = "yyyy-MM";
                dTable = CreateBaseDataTable(interval, from, to, tags);
            }

            var fromDate = from.Year + "-" + from.Month.ToString("00") + "-" + from.Day.ToString("00") + " " + from.Hour.ToString("00") + ":" + from.Minute.ToString("00") + ":" + from.Second.ToString("00");
            var toDate = to.Year + "-" + to.Month.ToString("00") + "-" + to.Day.ToString("00") + " " + to.Hour.ToString("00") + ":" + to.Minute.ToString("00") + ":" + to.Second.ToString("00");

            //////////////////////////////////

            for (int i = 0; i < tags.Length; i++)
            {
                var extractedTable = new DataTable();
                extractedTable = ExtractTagTable("select d.local_time, d.value from rpt_" + intervalForQuery + " d inner join pnt p on d.pnt_no = p.pnt_no where p.pnt_name = '" + tags[i] + "' and local_time >= '" + fromDate + "' and local_time < '" + toDate + "'", connectionString);

                foreach (DataRow extractedRow in extractedTable.Rows)
                {
                    for (int j = 0; j < dTable.Rows.Count; j++)
                    {
                        if (Convert.ToDateTime(extractedRow.ItemArray[0]).ToString(formatString) == Convert.ToDateTime(dTable.Rows[j].ItemArray[0]).ToString(formatString))
                        {
                            dTable.Rows[j][dTable.Columns.IndexOf(tags[i])] = Convert.ToDouble(extractedRow.ItemArray[1]);

                            j = dTable.Rows.Count;
                        }
                    }
                }
            }

            return dTable;
        }

        private DataTable CreateBaseDataTable(Interval interval, DateTime from, DateTime to, string[] tags)
        {
            DataTable dTable = new DataTable();
            DataColumn column = new DataColumn();
            DataRow row;
            //column.ColumnName = interval.ToString();
            column.ColumnName = "local_time";
            column.DataType = typeof(DateTime);
            dTable.Columns.Add(column);
            for (int i = 0; i < tags.Length; i++)
            {
                column = new DataColumn();
                column.ColumnName = tags[i];
                column.DataType = typeof(double);
                dTable.Columns.Add(column);
            }
            while (from < to)
            {
                row = dTable.NewRow();
                row[0] = from;
                //for (int i = 1; i < dTable.Columns.Count; i++)
                //{
                //    row[i] = DBNull.Value;
                //}
                dTable.Rows.Add(row);
                if (interval == Interval.Hour)
                    from = from.AddHours(1);

                else if (interval == Interval.Day)
                    from = from.AddDays(1);

                else if (interval == Interval.Month)
                    from = from.AddMonths(1);
            }

            return dTable;
        }

        public DataTable ReadCsv(string path)
        {
            var p = Path.Combine(path);
            if (Path.GetExtension(p) != ".csv")
                throw new Exception();


            DataTable dataTable = new DataTable();
            DataColumn column;
            using (StreamReader reader = new StreamReader(path))
            {
                string[] splitLine = reader.ReadLine().Split(',');
                foreach (var head in splitLine)
                {
                    column = new DataColumn();
                    column.ColumnName = head;
                    column.DataType = typeof(string);
                    dataTable.Columns.Add(column);
                }

                while (!reader.EndOfStream)
                {
                    DataRow row;
                    string[] line = reader.ReadLine().Split(',');
                    row = dataTable.NewRow();

                    for (int i = 0; i < line.Length; i++)
                        row[i] = line[i];

                    dataTable.Rows.Add(row);
                }
            }

            return dataTable;
        }

        public DataTable ReadExcel(string path, int sheet = 1)
        {
            var p = Path.Combine(path);
            if (Path.GetExtension(p) != ".xls" && Path.GetExtension(p) != ".xlsx")
                throw new Exception();

            DataTable dataTable = new DataTable();
            using (XLWorkbook workbook = new XLWorkbook(path))
            {
                IXLWorksheet worksheet = workbook.Worksheet(sheet);

                bool firstRow = true;
                foreach (IXLRow row in worksheet.Rows())
                {
                    if (firstRow)
                    {
                        foreach (IXLCell cell in row.Cells())
                        {
                            dataTable.Columns.Add(cell.Value.ToString());
                        }
                        firstRow = false;
                    }
                    else
                    {
                        dataTable.Rows.Add();
                        int i = 0;
                        foreach (IXLCell cell in row.Cells())
                        {
                            dataTable.Rows[dataTable.Rows.Count - 1][i] = cell.Value.ToString();
                            i++;
                        }
                    }
                }
            }
            return dataTable;
        }


        public void RenderTable(DataTable data, TableOptions options = null)
        {
            //data.SetColumnsOrder("","");

            if (data == null)
                return;

            if (options == null)
                options = new TableOptions();

            var divNode = HtmlNode.CreateNode("<div>");

            if (options.Responsive == "table-responsive")
                divNode.Attributes.Add("class", options.Responsive);

            var tableNode = HtmlNode.CreateNode("<table>");

            if (options.Responsive != "table-responsive")
                tableNode.Attributes.Add("class", "table " + options.Style + " " + options.Responsive + " table-header-rotated");
            else
                tableNode.Attributes.Add("class", "table " + options.Style);

            tableNode.Attributes.Add("align", "center");

            var headNode = tableNode.AppendChild(HtmlNode.CreateNode("<thead>"));
            var bodyNode = tableNode.AppendChild(HtmlNode.CreateNode("<tbody>"));

            if (options.GroupedColumns.Count > 0)
            {
                int emptySpan = 0;
                var rowNode = HtmlNode.CreateNode("<tr>");
                foreach (DataColumn column in data.Columns)
                {
                    bool fit = false;
                    foreach (var group in options.GroupedColumns)
                    {
                        if (!group.Fit && group.Headers.Contains(column.ColumnName))
                        {
                            if (emptySpan > 0)
                            {
                                rowNode.AppendChild(HtmlNode.CreateNode($"<th colspan=\"" + emptySpan + "\"><div><span>"));
                                emptySpan = 0;
                            }
                            rowNode.AppendChild(HtmlNode.CreateNode($"<th class=\"groupheader\" colspan=\"" + group.Headers.Length + "\"><div><span>" + group.Name));
                            emptySpan -= group.Headers.Length - 1;
                            group.Fit = fit = true;
                        }

                    }
                    if (!fit)
                    {
                        emptySpan++;
                    }
                }
                headNode.AppendChild(rowNode);
            }

            var tableRowNode = HtmlNode.CreateNode("<tr>");
            foreach (DataColumn column in data.Columns)
            {
                if (options.RotateColumns)
                {
                    int height = column.ColumnName.Length;
                    headNode.AppendChild(HtmlNode.CreateNode($"<th class=\"rotate-90\" nowrap style=\"height: {height*8}px;\"><div><span>{column.ColumnName.Trim()}"));
                }
                else
                    headNode.AppendChild(HtmlNode.CreateNode($"<th><div><span>{column.ColumnName.Trim()}"));
            }
            headNode.AppendChild(tableRowNode);

            foreach (DataRow row in data.Rows)
            {
                
                var rowNode = bodyNode.AppendChild(HtmlNode.CreateNode("<tr>"));
                foreach (DataColumn column in data.Columns)
                    rowNode.AppendChild(HtmlNode.CreateNode($"<td data-label=\"{column.ColumnName.Trim()}\">{row[column]}"));
            }

            divNode.ChildNodes.Add(tableNode);
            CurrentNode.ChildNodes.Add(divNode);
        }

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
                data = FillEmptyDataTable();
                options.Legends = CreateUndefinedLegends(data, options.Legends);
            }

            if (options.Legends.Count > 2 && options.ChartModelType == "multiChart()")
            {
                if (options.XDataName == "")
                {
                    options.XDataName = data.Columns[0].ColumnName;
                }
                options.Legends = OrganizingOrderOfLegends(options, data);
            }
            else if (options.Legends.Count == 2)
            {
                if (options.Legends[0].LeftOrRightYAxis == null || options.Legends[0].LeftOrRightYAxis.ToLower() != "left" || options.Legends[0].LeftOrRightYAxis.ToLower() != "right")
                    options.Legends[0].LeftOrRightYAxis = "left";

                if (options.Legends[1].LeftOrRightYAxis == null || options.Legends[1].LeftOrRightYAxis.ToLower() != "left" || options.Legends[1].LeftOrRightYAxis.ToLower() != "right")
                    options.Legends[1].LeftOrRightYAxis = "right";
            }
            foreach (var legendType in options.Legends)
            {
                legendType.Values = MakeValuesDictionary(data, options.XDataName, legendType.YDataName, options, legendType);
            }

            /*if (options.XAxisType == "")
            {
                options.XAxisType = m_chartXAxisType;
            }*/

            string datasetString = "";

            foreach (var legend in options.Legends)
            {
                datasetString = CreateDataSet(legend, datasetString, options);
            }



            var divNode = HtmlNode.CreateNode("<div>");
            divNode.Attributes.Add("id", "chart" + m_chartCounter);
            CurrentNode.ChildNodes.Add(divNode);

            var svgNode = HtmlNode.CreateNode("<svg>");
            //svgNode.Attributes.Add("width","100%");
            //svgNode.Attributes.Add("height", "auto");
            //svgNode.Attributes.Add("viewBox", "0 0 860 500");
            //svgNode.Attributes.Add("preserveAspectRatio", "xMinYMin meet");
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
        }

        private List<BaseLegend> OrganizingOrderOfLegends(ChartOptions options, DataTable data)
        {
            List<HighestLowestValues> valuesForSorting = new List<HighestLowestValues>();
            HighestLowestValues hlv;
            if (options.XDataName == "")
            {
                options.XDataName = data.Columns[0].ColumnName;
            }

            List<BaseLegend> unuseableLegends = new List<BaseLegend>();
            List<BaseLegend> useableLegends = new List<BaseLegend>();
            foreach (var legend in options.Legends)
            {
                if (legend.YDataName != "" && data.Columns.Contains(legend.YDataName))
                {
                    if (legend.Key == "")
                        legend.Key = legend.YDataName;

                    useableLegends.Add(legend);
                }
            }

            if (useableLegends.Count == 0 && options.Legends.Count != 0)
            {
                useableLegends = options.Legends;
                for (int i = 0, j = 0; i < useableLegends.Count + 1; i++)
                {
                    if (data.Columns[i].ColumnName != options.XDataName)
                    {
                        useableLegends[j].YDataName = data.Columns[i].ColumnName;

                        if (useableLegends[j].Key == null)
                            useableLegends[j].Key = data.Columns[i].ColumnName;
                        j++;
                    }
                }
            }

            foreach (var legend in useableLegends)
            {
                hlv = new HighestLowestValues();
                hlv.ColumnIndex = data.Columns.IndexOf(legend.YDataName);
                hlv.HighestValue = double.MinValue;
                hlv.LowestValue = double.MaxValue;
                int allNullValuesCounter = 0;
                for (int j = 0; j < data.Rows.Count; j++)
                {
                    if (data.Rows[j].ItemArray[hlv.ColumnIndex] != DBNull.Value)
                    {
                        if (Convert.ToDouble(data.Rows[j].ItemArray[hlv.ColumnIndex]) > hlv.HighestValue)
                            hlv.HighestValue = Convert.ToDouble(data.Rows[j].ItemArray[hlv.ColumnIndex]);

                        if (Convert.ToDouble(data.Rows[j].ItemArray[hlv.ColumnIndex]) < hlv.LowestValue)
                            hlv.LowestValue = Convert.ToDouble(data.Rows[j].ItemArray[hlv.ColumnIndex]);
                    }
                    else
                        allNullValuesCounter++;
                }


                for (int k = 0; k < useableLegends.Count; k++)
                {
                    if (useableLegends[k].YDataName == data.Columns[hlv.ColumnIndex].ColumnName)
                    {
                        hlv.IndexInOriginalList = k;
                        k = useableLegends.Count;
                    }
                }

                if (allNullValuesCounter != data.Rows.Count)
                {
                    valuesForSorting.Add(hlv);
                }
                else
                {
                    unuseableLegends.Add(useableLegends[hlv.IndexInOriginalList]);
                }
            }

            foreach (var unuseableLegend in unuseableLegends)
            {
                useableLegends.Remove(unuseableLegend);
            }

            foreach (var highestLowestCombination in valuesForSorting)
            {
                highestLowestCombination.HighestLowestDifference = highestLowestCombination.HighestValue - highestLowestCombination.LowestValue;
            }



            List<HighestLowestValues> valuesBySpand = new List<HighestLowestValues>();
            int currentValueItem = 0;
            while (currentValueItem < valuesForSorting.Count)
            {
                valuesBySpand.Add(new HighestLowestValues() { HighestLowestDifference = double.MinValue });
                foreach (var highestLowestCombination in valuesForSorting)
                {
                    if (highestLowestCombination.HighestLowestDifference >= valuesBySpand[currentValueItem].HighestLowestDifference &&
                        (highestLowestCombination.HighestValue >= valuesBySpand[currentValueItem].HighestValue - valuesBySpand[currentValueItem].HighestValue * 0.75 ||
                        highestLowestCombination.HighestValue <= valuesBySpand[currentValueItem].HighestValue - valuesBySpand[currentValueItem].HighestValue * 0.25) &&
                        !valuesBySpand.Contains(highestLowestCombination))
                    {
                        valuesBySpand[currentValueItem] = highestLowestCombination;
                    }
                }
                currentValueItem++;
            }


            HighestLowestValues highestValueItem = null;
            HighestLowestValues lowestValueItem = null;
            HighestLowestValues highestSpanItem = null;
            HighestLowestValues lowestSpanItem = null;

            foreach (var item in valuesBySpand)
            {
                if (highestValueItem == null || item.HighestValue > highestValueItem.HighestValue)
                    highestValueItem = item;
                if (lowestValueItem == null || item.LowestValue < lowestValueItem.LowestValue)
                    lowestValueItem = item;
                if (highestSpanItem == null || item.HighestLowestDifference > highestSpanItem.HighestLowestDifference)
                    highestSpanItem = item;
                if (lowestSpanItem == null || item.HighestLowestDifference < lowestSpanItem.HighestLowestDifference)
                    lowestSpanItem = item;
            }

            if (useableLegends.Count + unuseableLegends.Count == 2)
            {
                valuesBySpand[0].LeftOrRightYAxis = 1;
                valuesBySpand[1].LeftOrRightYAxis = 2;
            }

            else
            {
                if (highestValueItem == lowestValueItem)
                    foreach (var spanItem in valuesBySpand)
                    {
                        spanItem.LeftOrRightYAxis = 1;
                    }

                else
                {
                    foreach (var spanItem in valuesBySpand)
                    {
                        if (spanItem == highestValueItem)
                            spanItem.LeftOrRightYAxis = 1;
                        else if (spanItem == lowestValueItem)
                            spanItem.LeftOrRightYAxis = 2;
                    }
                    double y1HighestValue = highestValueItem.HighestValue;
                    double y1Lowestvalue = highestValueItem.LowestValue;
                    double y2HighestValue = lowestValueItem.HighestValue;
                    double y2LowestValue = lowestValueItem.LowestValue;

                    foreach (var spanItem in valuesBySpand)
                    {
                        if (spanItem.LeftOrRightYAxis == 1 || spanItem.LeftOrRightYAxis == 2)
                            continue;
                        if (spanItem.HighestValue > y1HighestValue && spanItem.LowestValue < y1Lowestvalue)
                        {
                            spanItem.LeftOrRightYAxis = 1;
                            y1HighestValue = spanItem.HighestValue;
                            y1Lowestvalue = spanItem.LowestValue;
                        }
                        else if (spanItem.HighestValue > y1HighestValue && spanItem.LowestValue > y1Lowestvalue)
                        {
                            spanItem.LeftOrRightYAxis = 1;
                            y1HighestValue = spanItem.HighestValue;
                        }
                        else if (spanItem.HighestValue > y1Lowestvalue && spanItem.LowestValue < y1Lowestvalue)
                        {
                            spanItem.LeftOrRightYAxis = 1;
                            y1Lowestvalue = spanItem.LowestValue;
                        }
                        else if (spanItem.HighestValue < y1HighestValue && spanItem.LowestValue > y1Lowestvalue)
                        {
                            spanItem.LeftOrRightYAxis = 1;
                        }
                        else if (spanItem.HighestValue < y1Lowestvalue && spanItem.LowestValue > y2HighestValue)
                        {
                            if (y1Lowestvalue - spanItem.HighestValue < spanItem.LowestValue - y2HighestValue)
                            {
                                spanItem.LeftOrRightYAxis = 1;
                                y1Lowestvalue = spanItem.LowestValue;
                            }
                            else
                            {
                                spanItem.LeftOrRightYAxis = 2;
                                y2HighestValue = spanItem.HighestValue;
                            }
                        }
                        else if (spanItem.HighestValue > y2HighestValue && spanItem.LowestValue < y2HighestValue)
                        {
                            spanItem.LeftOrRightYAxis = 2;
                            y2HighestValue = spanItem.HighestValue;
                        }
                        else if (spanItem.HighestValue < y2HighestValue && spanItem.LowestValue > y2LowestValue)
                        {
                            spanItem.LeftOrRightYAxis = 2;
                        }
                        else if (spanItem.HighestValue < y2HighestValue && spanItem.LowestValue < y2LowestValue)
                        {
                            spanItem.LeftOrRightYAxis = 2;
                            y2LowestValue = spanItem.LowestValue;
                        }
                    }
                }
            }

            List<BaseLegend> legendsBeforeReordering = new List<BaseLegend>();
            for (int i = 0; i < useableLegends.Count; i++)
            {
                legendsBeforeReordering.Add(useableLegends[valuesBySpand[i].IndexInOriginalList]);
                if (legendsBeforeReordering[i].LeftOrRightYAxis == null)
                {
                    if (valuesBySpand[i].LeftOrRightYAxis == 1)
                        legendsBeforeReordering[i].LeftOrRightYAxis = "left";

                    else /*if (valuesBySpand[i].LeftOrRightYAxis == 2)*/
                        legendsBeforeReordering[i].LeftOrRightYAxis = "right";
                }
            }
            for (int i = 0; i < unuseableLegends.Count; i++)
            {
                WriteWarning("No data was found for: " + unuseableLegends[i].Key);
                unuseableLegends[i].Key += " (no data found)";
                unuseableLegends[i].LeftOrRightYAxis = "right";
            }

            List<BaseLegend> legendsAfterReordering = new List<BaseLegend>();
            int leftThenRight = 1;
            while (leftThenRight < 3)
            {
                foreach (var legend in legendsBeforeReordering)
                {
                    if (leftThenRight == 1 && legend.LeftOrRightYAxis.ToLower() == "left")
                        legendsAfterReordering.Add(legend);
                    else if (leftThenRight == 2 && legend.LeftOrRightYAxis.ToLower() == "right")
                        legendsAfterReordering.Add(legend);
                }
                leftThenRight++;
            }
            foreach (var legend in unuseableLegends)
            {
                legendsAfterReordering.Add(legend);
            }
            return legendsAfterReordering;
        }

        private List<BaseLegend> CreateUndefinedLegends(DataTable data, List<BaseLegend> legends)
        {
            List<BaseLegend> defaultLegends = legends;
            int columnsCount = data.Columns.Count;

            for (int i = 1; i < columnsCount; i++)
            {
                defaultLegends.Add(new LineLegend() { Key = data.Columns[i].ColumnName });
            }

            return defaultLegends;
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
            //if (data.Columns.Count > 3)
            //    columnCounter = 3;

            //else
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
                if (options.XAxisType == "string")
                    dataset += "{" + dataCombination.Key + "," + dataCombination.Value + ", label: \"" + options.XAxisLabels.ElementAt(i++) + "\"},";
                else
                    dataset += "{" + dataCombination.Key + "," + dataCombination.Value + "},";

            }
            dataset = dataset.Remove(dataset.Length - 1);
            int leftOrRight;
            if (lineLegend.LeftOrRightYAxis == "right")
                leftOrRight = 2;

            else
                leftOrRight = 1;

            if (lineLegend.Type != "pie" && lineLegend.Type != "donut")
                dataset += "], \"type\": \"" + lineLegend.Type + "\", \"yAxis\": " + leftOrRight.ToString() + "}]";
            else
                dataset += "]";
            m_numberOfLegends++;

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
                    options.XAxisType = "";
                    options.ChartModelType = legend.Type;
                }
                else if (dTable.Rows[i].ItemArray[xDataColumnIndex].GetType() == typeof(string) || options.XAxisType.ToLower() == "string")
                {
                    xValue = (i + 1).ToString();

                    int LabelColumn = 0;
                    if (options.LabelColumn != null)
                        LabelColumn = dTable.Columns.IndexOf(options.LabelColumn);
                    labels.Add(dTable.Rows[i].ItemArray[LabelColumn].ToString());
                    //m_chartXAxisType = "string";
                    options.XAxisType = "string";
                }

                else if (dTable.Rows[i].ItemArray[xDataColumnIndex].GetType() == typeof(DateTime) || options.XAxisType.ToLower() == "date")
                {

                    date = Convert.ToDateTime(dTable.Rows[i].ItemArray[xDataColumnIndex]).ToUniversalTime();
                    long ticker = date.Ticks - epoch.Ticks;
                    xValue = ticker.ToString().Remove(ticker.ToString().Length - 4);
                    //m_chartXAxisType = "date";
                    options.XAxisType = "date";
                }
                else
                {
                    xValue = dTable.Rows[i].ItemArray[xDataColumnIndex].ToString().Replace(',', '.');
                    //m_chartXAxisType = "int";
                    options.XAxisType = "int";
                }

                if (String.IsNullOrWhiteSpace(dTable.Rows[i].ItemArray[m_yDataColumnIndex].ToString().Replace(',', '.')))
                    dicOfValues.Add("\"x\":" + xValue, "\"y\": null");

                else
                    dicOfValues.Add("\"x\":" + xValue, "\"y\":" + dTable.Rows[i].ItemArray[m_yDataColumnIndex].ToString().Replace(',', '.'));
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

                addGraphFunction += "chart.xAxis.axisLabel('').tickFormat(function(d) {var label = dataset" + m_chartCounter + "[0].values[--d].label; return label}).staggerLabels(true);\n";
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
            if (options.XAxisLabel != null)
            {
                addGraphFunction += "chart.xAxis.axisLabel('" + options.XAxisLabel + "');";
            }

            addGraphFunction += "chart.yAxis1.tickFormat(d3.format(',.0f'));\n";
            if (options.YAxisLabel != null)
            {
                addGraphFunction += "chart.yAxis1.axisLabel('" + options.YAxisLabel + "');";
            }

            if (options.Legends.Count > 1)
            {
                addGraphFunction += "chart.yAxis2.tickFormat(d3.format(',.1f'));\n";
                if (options.YAxisLabel2 != null)
                {
                    addGraphFunction += "chart.yAxis2.axisLabel('" + options.YAxisLabel2 + "');";
                }
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
            if (!options.ShowMaxMin && options.ShowAllTicks)
            {
                int amountOfTicks = options.Legends[0].Values.Count - 1;
                int steps = 1;
                string tickSteps = "1";
                if (options.Legends[0].Values.Count > options.MaxAmountOfTicks)
                {
                    while ((options.Legends[0].Values.Count / steps) > options.MaxAmountOfTicks)
                    {
                        steps++;
                    }
                    amountOfTicks = (amountOfTicks / steps);
                    tickSteps = steps.ToString();
                }
                addGraphFunction += "var ticker = new Array();\n";
                //addGraphFunction += "var ticker = new Array(dataset" + m_chartCounter + "[0].values.length);\n";
                if (options.XAxisType != "string")
                {
                    addGraphFunction += "for (var a = " + amountOfTicks.ToString() + ", b = 0; a > -1; a--, b += " + tickSteps + "){ticker[a] = dataset" + m_chartCounter.ToString() + "[0].values[b].x;}";
                    if (steps > 1)
                    {
                        addGraphFunction += "ticker[ticker.length] = dataset" + m_chartCounter.ToString() + "[0].values[dataset" + m_chartCounter.ToString() + "[0].values.length-1].x;";
                    }
                    addGraphFunction += "chart.xAxis.tickValues(ticker).rotateLabels(" + options.RotateLabels + ").showMaxMin(" + options.ShowMaxMin.ToString().ToLower() + ");";
                    //addGraphFunction += "for (var a = dataset" + m_chartCounter.ToString() + "[0].values.length-1; a > -1; a--){ticker[a] = dataset" + m_chartCounter.ToString() + "[0].values[a].x;}chart.xAxis.tickValues(ticker).rotateLabels(" + options.RotateLabels + ").showMaxMin(" + options.ShowMaxMin.ToString().ToLower() + ");";
                }
                else
                {
                    //Her skal der skrives hvordan den i if statementet skal være, i tilfælde af at den skal bruge labels, som man skal ved strings. Ved at man vælger den plads i datasettet, som labelsne har.
                }
            }
            else
                addGraphFunction += "chart.xAxis.rotateLabels(" + options.RotateLabels + ").showMaxMin(" + options.ShowMaxMin.ToString().ToLower() + ");\n";

            addGraphFunction += "d3.select('#chart" + m_chartCounter + " svg')\n"
                + ".datum(dataset" + m_chartCounter + ")\n"
                + ".transition().duration(0).call(chart); nv.utils.windowResize(chart.update);\n"
                + "return chart;\n"
                + "});";
            return addGraphFunction;
        }
    }
}
