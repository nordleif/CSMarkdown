using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using CSMarkdown.Rendering;

namespace CSMarkdown.Tests
{
    [TestFixture]
    public class CSMarkdownRendererTests
    {
        [TestCase("markdown_001.smd", RenderOutput.Html)]
        [TestCase("markdown_001.smd", RenderOutput.Pdf)]
        [TestCase("markdown_002.smd", RenderOutput.Html)]
        [TestCase("markdown_002.smd", RenderOutput.Pdf)]
        [TestCase("markdown_003.smd", RenderOutput.Html)]
        [TestCase("markdown_003.smd", RenderOutput.Pdf)]
        [TestCase("markdown_004.smd", RenderOutput.Html)]
        [TestCase("markdown_004.smd", RenderOutput.Pdf)]
        [TestCase("markdown_005.smd", RenderOutput.Html)]
        [TestCase("markdown_006.smd", RenderOutput.Html)]

        [TestCase("markdown_display_error.smd", RenderOutput.Html)]
        [TestCase("markdown_display_message.smd", RenderOutput.Html)]
        [TestCase("markdown_display_warning.smd", RenderOutput.Html)]

        [Test]
        public void CSMarkdownRenderer_Render(string fileName, RenderOutput output)
        {

            var path = Path.Combine("D:\\Source\\GitHub\\CSMarkdown\\CSMarkdown.Tests\\Documents", fileName);
            var text = File.ReadAllText(path);

            var renderer = new CSMarkdownRenderer();
            var result = renderer.Render(text, new CSMarkdownRenderOptions { Output = output });
            
            path = path.Replace(Path.GetExtension(path), output == RenderOutput.Html ? ".html" : ".pdf");
            File.WriteAllBytes(path, result);

            Process.Start(path);
        }
    }
}
