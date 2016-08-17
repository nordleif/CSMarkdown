﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSMarkdown.Scripting;

namespace CSMarkdown.Rendering
{
    public class CSMarkdownRenderOptions
    {
        public CSMarkdownRenderOptions()
        {
            FlattenHtml = true;
            Output = RenderOutput.Html;
        }

        public bool FlattenHtml { get; set; }

        public ScriptContextBase ScriptContext { get; set; }

        public RenderOutput Output { get; set; }
    }
}
