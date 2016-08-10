using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMarkdown.Rendering
{
    public class CSMarkdownRenderOptions
    {
        public CSMarkdownRenderOptions()
        {
            Output = RenderOutput.Html;
        }

        public RenderOutput Output { get; set; }
    }
}
