using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMarkdown.Rendering
{
    public class PdfRendererOptions
    {
        public PdfRendererOptions()
        {

        }

        public int MarginBottom { get; set; } = 10;

        public int MarginLeft { get; set; } = 0;

        public int MarginRight { get; set; } = 0;

        public int MarginTop { get; set; } = 10;
    }
}
