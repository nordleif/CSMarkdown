using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CommonMark.Syntax;
using HtmlAgilityPack;
using CSMarkdown.Scripting;

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
            foreach(var node in nodes)
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
            foreach(var node in nodes)
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
            foreach(var node in nodes)
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
    }
}
