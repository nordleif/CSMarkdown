using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace CSMarkdown.Rendering
{
    internal class RenderContext
    {
        public RenderContext()
        {

        }
        
        public CodeChunk[] CodeChunks { get; set; }

        public HtmlDocument HtmlDocument { get; set; }

        public string MarkdownText { get; set; }

        public CSMarkdownRenderOptions RenderOptions { get; set; }

        public YamlOptions YamlHeader { get; set; }
    }
}
