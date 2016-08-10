using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonMark;
using CommonMark.Formatters;
using CommonMark.Syntax;

namespace CSMarkdown.Rendering
{
    internal class CustomHtmlFormatter : HtmlFormatter
    {
        public CustomHtmlFormatter(TextWriter target, CommonMarkSettings settings)
            : base(target, settings)
        {

        }

        protected override void WriteBlock(Block block, bool isOpening, bool isClosing, out bool ignoreChildNodes)
        {
            switch (block.Tag)
            {
                case BlockTag.FencedCode:
                    var info = block?.FencedCodeData?.Info?.Trim();
                    if (!string.IsNullOrWhiteSpace(info) && info.StartsWith("{s", StringComparison.InvariantCultureIgnoreCase) && info.EndsWith("}", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var html = $"<code lang=\"cs\" options=\"{info}\">{block.StringContent}</code>";
                        Write(html);
                        ignoreChildNodes = true;
                        return;
                    }
                    break;
            }

            base.WriteBlock(block, isOpening, isClosing, out ignoreChildNodes);
        }

        protected override void WriteInline(Inline inline, bool isOpening, bool isClosing, out bool ignoreChildNodes)
        {
            base.WriteInline(inline, isOpening, isClosing, out ignoreChildNodes);
        }
    }
}
