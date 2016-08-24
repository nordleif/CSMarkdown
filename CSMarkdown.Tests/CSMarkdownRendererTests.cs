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
        [TestCase("markdown_007.smd", RenderOutput.Html)]
        [TestCase("markdown_008.smd", RenderOutput.Html)]
        [TestCase("markdown_009.smd", RenderOutput.Html)]
        [TestCase("markdown_010.smd", RenderOutput.Html)]
        [TestCase("markdown_011.smd", RenderOutput.Html)]
        [TestCase("markdown_012.smd", RenderOutput.Html)]
        [TestCase("markdown_013.smd", RenderOutput.Html)]
        [TestCase("markdown_014.smd", RenderOutput.Html)]
        [TestCase("markdown_015.smd", RenderOutput.Html)]
        [TestCase("markdown_016.smd", RenderOutput.Html)]
        [TestCase("markdown_017.smd", RenderOutput.Html)]

        [TestCase("markdown_display_error.smd", RenderOutput.Html)]
        [TestCase("markdown_display_message.smd", RenderOutput.Html)]
        [TestCase("markdown_display_warning.smd", RenderOutput.Html)]

        [Test]
        public void CSMarkdownRenderer_Render(string fileName, RenderOutput output)
        {
            var path = Path.Combine(@"../../Documents", fileName);
            //var path = Path.Combine("D:\\Source\\GitHub\\CSMarkdown\\CSMarkdown.Tests\\Documents", fileName);
            var text = File.ReadAllText(path);

            var renderer = new CSMarkdownRenderer();
            var result = renderer.Render(text, new CSMarkdownRenderOptions { Output = output });
            
            path = path.Replace(Path.GetExtension(path), output == RenderOutput.Html ? ".html" : ".pdf");
            File.WriteAllBytes(path, result);

            Process.Start(path);
        }

        [Test]
        public void CSMarkdownRenderer_Error_Throw()
        {
            var path = Path.Combine(@"../../Documents/markdown_error_throw.smd");
            //var path = Path.Combine("D:\\Source\\GitHub\\CSMarkdown\\CSMarkdown.Tests\\Documents\\markdown_error_throw.smd");
            var text = File.ReadAllText(path);

            Assert.Catch<Exception>(() =>
            {
                var renderer = new CSMarkdownRenderer();
                var result = renderer.Render(text, new CSMarkdownRenderOptions { Output = RenderOutput.Html });
            });
        }

        [Test]
        public void CSMarkdownRenderer_Error_Display()
        {
            var path = Path.Combine(@"../../Documents/markdown_error_display.smd");
            //var path = Path.Combine("D:\\Source\\GitHub\\CSMarkdown\\CSMarkdown.Tests\\Documents\\markdown_error_display.smd");
            var text = File.ReadAllText(path);

            var renderer = new CSMarkdownRenderer();
            var result = renderer.Render(text, new CSMarkdownRenderOptions { Output = RenderOutput.Html });

            Assert.NotNull(result);
        }

        [Test]
        public void CSMarkdownRenderer_Parameters()
        {
            var path = Path.Combine(@"../../Documents/markdown_parameters.smd");
            //var path = Path.Combine("D:\\Source\\GitHub\\CSMarkdown\\CSMarkdown.Tests\\Documents\\markdown_parameters.smd");
            var text = File.ReadAllText(path);

            var renderer = new CSMarkdownRenderer();
            var result = renderer.Render(text, new CSMarkdownRenderOptions { Output = RenderOutput.Html });

            path = path.Replace(Path.GetExtension(path), ".html");
            File.WriteAllBytes(path, result);

            Process.Start(path);
        }
    }
}
