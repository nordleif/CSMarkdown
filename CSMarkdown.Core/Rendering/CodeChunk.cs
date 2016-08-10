using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace CSMarkdown.Rendering
{
    internal class CodeChunk
    {
        #region Static Members

        public static CodeChunk Parse(HtmlNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            var options = node.Attributes.FirstOrDefault(a => a.Name.Equals("options", StringComparison.InvariantCultureIgnoreCase))?.Value;
            var code = node.InnerHtml.Trim(' ', '\r', '\n');
            var bytes = Convert.FromBase64String(code);
            code = Encoding.UTF8.GetString(bytes);

            var codeChunk = new CodeChunk();
            codeChunk.Code = code;
            codeChunk.Inline = options != null && options.Equals("inline", StringComparison.InvariantCultureIgnoreCase);
            codeChunk.Options = CodeChunkOptions.Parse(options);
            codeChunk.Node = node;
            
            return codeChunk;
        }
        
        #endregion

        private CodeChunk()
        {

        }

        public string Code { get; protected set; }

        public bool Inline { get; protected set; }

        public HtmlNode Node { get; protected set; } 

        public CodeChunkOptions Options { get; protected set; }

        public override string ToString()
        {
            return Code;
        }
    }
}
