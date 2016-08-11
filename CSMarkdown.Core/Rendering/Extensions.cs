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

                var client = new WebClient();
                var data = client.DownloadString(href);
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

                var client = new WebClient();
                var data = client.DownloadString(src);
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
                
                byte[] bytes = null;
                var assembly = Assembly.GetExecutingAssembly();
                using (var resourceStream = assembly.GetManifestResourceStream($"CSMarkdown.Rendering.www.{src}"))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        resourceStream.CopyTo(memoryStream);
                        bytes = memoryStream.ToArray();
                    }
                }
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

            var html = string.Empty;
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream($"CSMarkdown.Rendering.www.{resourceName}"))
            {
                using (var reader = new StreamReader(stream))
                {
                    html = reader.ReadToEnd();
                }
            }

            document.LoadHtml(html);
        }
    }
}
