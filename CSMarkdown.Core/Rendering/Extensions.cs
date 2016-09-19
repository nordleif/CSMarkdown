using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CommonMark.Syntax;
using HtmlAgilityPack;
using CSMarkdown.Scripting;
using System.Data;

namespace CSMarkdown.Rendering
{
    internal static class Extensions
    {
        public static HtmlDocument Clone(this HtmlDocument document)
        {
            var html = document.DocumentNode.OuterHtml;
            document = new HtmlDocument();
            document.LoadHtml(html);
            return document;
        }

        public static HtmlDocument Flatten(this HtmlDocument document)
        {
            document = document.Clone();

            // link
            var nodes = document.DocumentNode.Descendants("link").ToArray();
            foreach (var node in nodes)
            {
                var rel = node.Attributes["rel"]?.Value;
                if (!string.IsNullOrWhiteSpace(rel) && !rel.Equals("stylesheet", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                var href = node.Attributes["href"]?.Value;
                if (string.IsNullOrWhiteSpace(href))
                    continue;

                var data = Assembly.GetExecutingAssembly().ReadResourceString(href);
                var newNode = HtmlNode.CreateNode($"<style>{data}</style>");
                node.ParentNode.ReplaceChild(newNode, node);
            }

            // script
            nodes = document.DocumentNode.Descendants("script").ToArray();
            foreach (var node in nodes)
            {
                var src = node.Attributes["src"]?.Value;
                if (string.IsNullOrWhiteSpace(src))
                    continue;

                var data = Assembly.GetExecutingAssembly().ReadResourceString(src);
                var newNode = HtmlNode.CreateNode($"<script>{data}</script>");
                node.ParentNode.ReplaceChild(newNode, node);
            }

            // img
            nodes = document.DocumentNode.Descendants("img").ToArray();
            foreach (var node in nodes)
            {
                var src = node.Attributes["src"]?.Value;
                if (string.IsNullOrWhiteSpace(src))
                    continue;

                var bytes = Assembly.GetExecutingAssembly().ReadResourceBytes(src);
                var base64 = Convert.ToBase64String(bytes);
                node.Attributes["src"].Value = $"data:image/png;base64,{base64}";

            }

            return document;
        }

        public static void LoadHtmlFromResource(this HtmlDocument document, string resourceName)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (string.IsNullOrWhiteSpace(resourceName))
                throw new ArgumentNullException(nameof(resourceName));

            var html = Assembly.GetExecutingAssembly().ReadResourceString(resourceName);

            document.LoadHtml(html);
        }

        public static dynamic ParseParameters(this YamlOptions options, Dictionary<string, string> values)
        {
            if (values == null)
                values = new Dictionary<string, string>();

            var expandoObject = new ExpandoObject();
            var parameterNames = options.ReadKeys("params");
            foreach (var parameterName in parameterNames)
            {
                if (string.IsNullOrWhiteSpace(parameterName))
                    continue;

                var type = options.ReadValue<string>($"params.{parameterName}.type", string.Empty);
                var defaultValue = options.ReadValue<string>($"params.{parameterName}.value", string.Empty);

                var propertyType = typeof(string);
                if (type.Equals("bool", StringComparison.InvariantCultureIgnoreCase))
                    propertyType = typeof(bool);
                else if (type.Equals("date", StringComparison.InvariantCultureIgnoreCase))
                    propertyType = typeof(DateTime);
                else if (type.Equals("datetime", StringComparison.InvariantCultureIgnoreCase))
                    propertyType = typeof(DateTime);
                else if (type.Equals("double", StringComparison.InvariantCultureIgnoreCase))
                    propertyType = typeof(double);
                else if (type.Equals("int", StringComparison.InvariantCultureIgnoreCase))
                    propertyType = typeof(int);

                object parameterValue = null;
                if (!string.IsNullOrEmpty(defaultValue))
                {
                    if (propertyType == typeof(DateTime) && (defaultValue.ToLower().Contains('x') || defaultValue.Contains('u')))
                        defaultValue = DateTimeNotation(defaultValue);

                    parameterValue = Convert.ChangeType(defaultValue, propertyType);
                }
                else
                    parameterValue = Activator.CreateInstance(propertyType);

                var value = values.FirstOrDefault(kvp => kvp.Key.Equals(parameterName, StringComparison.InvariantCultureIgnoreCase)).Value;
                if (!string.IsNullOrEmpty(value))
                {
                    if (propertyType == typeof(DateTime) && (defaultValue.ToLower().Contains('x') || defaultValue.Contains('u')))
                        defaultValue = DateTimeNotation(defaultValue);

                    parameterValue = Convert.ChangeType(value, propertyType);
                }

                if (parameterValue == null)
                    throw new ParameterMissingException($"The '{parameterName}' parameter is missing a value.");

                ((IDictionary<string, object>)expandoObject).Add(parameterName, parameterValue);
            }

            return expandoObject;
        }

        public static string ReadResourceString(this Assembly assembly, string resourceName)
        {
            using (var stream = assembly.GetManifestResourceStream($"CSMarkdown.Rendering.www.{resourceName}"))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static byte[] ReadResourceBytes(this Assembly assembly, string resourceName)
        {
            byte[] bytes = null;
            using (var resourceStream = assembly.GetManifestResourceStream($"CSMarkdown.Rendering.www.{resourceName}"))
            {
                using (var memoryStream = new MemoryStream())
                {
                    resourceStream.CopyTo(memoryStream);
                    bytes = memoryStream.ToArray();
                }
            }
            return bytes;
        }

        public static void ThrowOrWriteError(this ScriptContextBase scriptContext, Exception ex)
        {
            if (ex == null)
                throw new ArgumentNullException(nameof(ex));

            if (!scriptContext.Options.ReadValue<bool>("error", false))
                throw ex;

            var aggregateException = ex as AggregateException;
            if (aggregateException != null)
            {
                foreach (var innerException in aggregateException.InnerExceptions)
                    scriptContext.ThrowOrWriteError(innerException);
            }
            else
            {
                scriptContext.CurrentNode.AppendChild(HtmlNode.CreateNode($"<pre><code class=\"error\">## {ex.Message}</code></pre>"));
            }

        }

        private static string DateTimeNotation(string dateParam)
        {
            string dateTimeString = "";

            DateTime localNow = DateTime.Now.ToLocalTime();
            DateTime utcNow = DateTime.Now.ToUniversalTime();
            DateTime defaultDate = new DateTime();

            char[] plusAndMinus = { '+', '-' };
            char[] allowedLetters = { 'x', 'u' };
            char previousChar = new char();
            if (dateParam.Contains(allowedLetters[0]) && dateParam.Contains(allowedLetters[1]))
            {
                throw new Exception("Date parameter can't consist of both local time and UTC time.");
            }
            StringBuilder paramBuilder = new StringBuilder();
            foreach (var cha in dateParam.ToLower())
            {
                bool notFirstChar = paramBuilder.Length - 1 >= 0 ? true : false;

                if ((cha >= '0' && cha <= '9') || (plusAndMinus.Contains(cha) || allowedLetters.Contains(cha)))
                {
                    if (notFirstChar && (plusAndMinus.Contains(cha) && plusAndMinus.Contains(previousChar)))
                    {
                        throw new Exception("Invalid input format. \'" + previousChar + "\' and \'" + cha + "\' can't be used in succession of each other");
                    }
                    else if (notFirstChar && (plusAndMinus.Contains(previousChar) && allowedLetters.Contains(cha)))
                        paramBuilder.Remove(paramBuilder.Length - 1, 1).Append("." + cha);
                    else if (notFirstChar && ((previousChar >= '0' && previousChar <= '9') && allowedLetters.Contains(cha)))
                        paramBuilder.Append("." + cha);
                    else if (notFirstChar && (allowedLetters.Contains(previousChar) && (cha >= '0' && cha <= '9')))
                        paramBuilder.Append("." + cha);
                    else
                        paramBuilder.Append(cha);
                }
                else
                    paramBuilder.Append('.');

                previousChar = cha;
            }
            dateParam = paramBuilder.ToString().ToLower();
            string[] dateParamArr = dateParam.Split('.');

            List<DateTimeComponents> dtComponents = Enum.GetValues(typeof(DateTimeComponents)).Cast<DateTimeComponents>().ToList();

            if (dateParamArr.Length > dtComponents.Count)
                throw new Exception("Too many date parameters received, can't set date - Will use default if any is provided");

            string calcExpression = "";
            for (int i = 0; i < dateParamArr.Length; i++)
            {
                for (int j = 0; j < dateParamArr[i].Length; j++)
                {
                    if (allowedLetters.Contains(dateParamArr[i][j]))
                    {
                        if (dateParamArr[i][j] == 'x')
                            calcExpression += dtComponents[i].GetDateTimeComponent(localNow);

                        else if (dateParamArr[i][j] == 'u')
                            calcExpression += dtComponents[i].GetDateTimeComponent(utcNow);
                    }
                    else
                        calcExpression += dateParamArr[i][j];
                }
                string seperator = "";

                if (i > 0 && i < 3)
                    seperator = "-";
                else if (i == 3)
                    seperator = " ";
                else if (i > 3)
                    seperator = ":";

                if (calcExpression != "")
                    dateTimeString += seperator + Evaluate(calcExpression).ToString("00");
                else if (calcExpression == "" && dateTimeString == "")
                    dateTimeString += seperator + Evaluate(dtComponents[i].GetDateTimeComponent(defaultDate).ToString()).ToString("0000");
                else if (calcExpression == "")
                    dateTimeString += seperator + Evaluate(dtComponents[i].GetDateTimeComponent(defaultDate).ToString()).ToString("00");

                calcExpression = "";
            }

            return dateTimeString;
        }

        public static double Evaluate(string expression)
        {
            DataTable table = new DataTable();
            table.Columns.Add("expression", typeof(string), expression);
            DataRow row = table.NewRow();
            table.Rows.Add(row);
            return double.Parse((string)row["expression"]);
        }

    }
}
