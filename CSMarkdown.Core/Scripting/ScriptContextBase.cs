using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using CSMarkdown.Rendering;

namespace CSMarkdown.Scripting
{
    public class ScriptContextBase 
    {
        public ScriptContextBase()
        {

        }

        public HtmlNode CurrentNode { get; set; }

        public CodeChunkOptions Options { get; set; }

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
