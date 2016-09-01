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
        [TestCase("markdown_2_legends.smd", RenderOutput.Html)]
        [TestCase("markdown_2_legends.smd", RenderOutput.Pdf)]
        [TestCase("markdown_2_legends_1_type.smd", RenderOutput.Html)]
        [TestCase("markdown_2_legends_1_type.smd", RenderOutput.Pdf)]
        [TestCase("markdown_2_legends_2_types.smd", RenderOutput.Html)]
        [TestCase("markdown_2_legends_2_types.smd", RenderOutput.Pdf)]
        [TestCase("markdown_custom_dateformat.smd", RenderOutput.Html)]
        [TestCase("markdown_custom_dateformat.smd", RenderOutput.Pdf)]
        [TestCase("markdown_inline_code.smd", RenderOutput.Html)]
        [TestCase("markdown_inline_code.smd", RenderOutput.Pdf)]
        [TestCase("markdown_legend.smd", RenderOutput.Html)]
        [TestCase("markdown_legend.smd", RenderOutput.Pdf)]
        [TestCase("markdown_min_and_max_values.smd", RenderOutput.Html)]
        [TestCase("markdown_min_and_max_values.smd", RenderOutput.Pdf)]
        [TestCase("markdown_min_value.smd", RenderOutput.Html)]
        [TestCase("markdown_min_value.smd", RenderOutput.Pdf)]
        [TestCase("markdown_no_legends.smd", RenderOutput.Html)]
        [TestCase("markdown_no_legends.smd", RenderOutput.Pdf)]
        [TestCase("markdown_null_data.smd", RenderOutput.Html)]
        [TestCase("markdown_null_data.smd", RenderOutput.Pdf)]
        [TestCase("markdown_pie_and_donut_chart.smd", RenderOutput.Html)]
        [TestCase("markdown_pie_and_donut_chart.smd", RenderOutput.Pdf)]
        [TestCase("markdown_render_10_charts.smd", RenderOutput.Html)]
        [TestCase("markdown_render_10_charts.smd", RenderOutput.Pdf)]
        [TestCase("markdown_xaxis_as_string.smd", RenderOutput.Html)]
        [TestCase("markdown_xaxis_as_string.smd", RenderOutput.Pdf)]
        [TestCase("markdown_yaml_params.smd", RenderOutput.Html)]
        [TestCase("markdown_yaml_params.smd", RenderOutput.Pdf)]
        [TestCase("markdown_table.smd", RenderOutput.Html)]
        [TestCase("markdown_table.smd", RenderOutput.Pdf)]
        [TestCase("markdown_read_excel_csv.smd", RenderOutput.Html)]

        [TestCase("markdown_display_error.smd", RenderOutput.Html)]
        [TestCase("markdown_display_message.smd", RenderOutput.Html)]
        [TestCase("markdown_display_warning.smd", RenderOutput.Html)]
        [TestCase("markdown_multiple_value_without_legends.smd", RenderOutput.Html)]
        [TestCase("markdown_multiple_value_with_legends_defined.smd", RenderOutput.Html)]
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
            var path = Path.Combine(@"../../Documents\\markdown_error_throw.smd");
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
            var path = Path.Combine(@"../../Documents\\markdown_error_display.smd");
            //var path = Path.Combine("D:\\Source\\GitHub\\CSMarkdown\\CSMarkdown.Tests\\Documents\\markdown_error_display.smd");
            var text = File.ReadAllText(path);

            var renderer = new CSMarkdownRenderer();
            var result = renderer.Render(text, new CSMarkdownRenderOptions { Output = RenderOutput.Html });

            Assert.NotNull(result);
        }

        [Test]
        public void CSMarkdownRenderer_Parameters()
        {
            var path = Path.Combine(@"../../Documents\\markdown_parameters.smd");
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
