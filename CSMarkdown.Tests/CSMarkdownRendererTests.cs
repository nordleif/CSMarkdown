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
        [TestCase("markdown_read_excel_csv.smd", RenderOutput.Pdf)]
        [TestCase("markdown_syntax_test.smd", RenderOutput.Html)]

        [TestCase("markdown_display_error.smd", RenderOutput.Html)]
        [TestCase("markdown_display_message.smd", RenderOutput.Html)]
        [TestCase("markdown_display_warning.smd", RenderOutput.Html)]
        [TestCase("markdown_multiple_value_without_legends.smd", RenderOutput.Html)]
        [TestCase("markdown_multiple_value_with_legends_defined.smd", RenderOutput.Html)]
        [TestCase ("markdown_2_legends_using_readTags.smd", RenderOutput.Html)]

        [TestCase("markdown_yaml_params - Copy.smd", RenderOutput.Html)]
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

        [TestCase("-r -s markdown_2_legends.smd -n Hubba -i ../../Documents -o C:/CSMarkdownTempFiles-DeletableFolder -t html", TestName = "App: new name of output file")]
        [TestCase("--render --smdfile markdown_2_legends.smd --newfilename LongName --inputpath ../../Documents --outputpath C:/CSMarkdownTempFiles-DeletableFolder -t html", TestName = "App: using long name")]
        [TestCase("--render --smdfile markdown_2_legends.smd --inputpath ../../Documents --outputpath C:/CSMarkdownTempFiles-DeletableFolder -t html", TestName = "App: no name given")]
        [TestCase("-r -s markdown_2_legends.smd -n NoOutputDefined -i ../../Documents -o C:/CSMarkdownTempFiles-DeletableFolder", TestName = "App: no output defined")]
        [TestCase("-r -s markdown_2_legends.smd -n \"new name of output with whitespaces\" -i ../../Documents -o C:/CSMarkdownTempFiles-DeletableFolder -t html", TestName = "App: new name of output file with whitespaces")]
        [TestCase("-r -s markdown_yaml_params.smd -n FileMadeUsingParams -i ../../Documents -o C:/CSMarkdownTempFiles-DeletableFolder -t html -p \"from = 2015-12-29, to= 2016-12-29, tag = foo, boo\"", TestName = "App: using params")]
        [TestCase("-r -s markdown_2_legends.smd -n HubbaPDF -i ../../Documents -o C:/CSMarkdownTempFiles-DeletableFolder -t pdf", TestName = "App: new name and pdf as output")]
        [TestCase("-r -s markdown_2_legends.smd -n Markdown2LegendsUsingTheApp -i ../../Documents -t pdf", TestName = "App: no output path defined - Places file where .smd file is")]
        [TestCase("/?", TestName = "App: get /? for CSMarkdown.exe")]
        [Test]
        public void MarkdownAppTest(string args)
        {
            string outputFolder = @"C:\CSMarkdownTempFiles-DeletableFolder";
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
            string exeFilePath = AppDomain.CurrentDomain.BaseDirectory;
            exeFilePath = exeFilePath.Remove(exeFilePath.Length - 27) + "CSMarkdown.App\\bin\\Debug\\CSMarkdown.exe";
            Process process = new Process();
            process.StartInfo.FileName = exeFilePath;
            process.StartInfo.Arguments = args;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            StreamReader errorReader = process.StandardError;
            string errorOutput = errorReader.ReadToEnd();


            //Writes the output, which normally would have been shown in the Console
            StreamReader outputReader = process.StandardOutput;
            string consoleOutput = outputReader.ReadToEnd();
            string consoleOutputPath = Path.Combine(outputFolder, "CSMarkdownOutput.txt");
            string currentContent = String.Empty;
            if (File.Exists(consoleOutputPath))
                currentContent = File.ReadAllText(consoleOutputPath);
            File.WriteAllText(consoleOutputPath, DateTime.Now + " - " + TestContext.CurrentContext.Test.Name + "\r\n" + consoleOutput + "\r\n" + currentContent);
            

            process.WaitForExit();
            process.Close();

            if (!string.IsNullOrEmpty(errorOutput))
            {
                if (errorOutput != "Qt: Could not initialize OLE (error 80010106)\r\n")
                    throw new Exception(errorOutput);
            }
        }

        //[Test]
        //public void GetCSMarkdownAppArguementOptions()
        //{
        //    string outputPath = "C:/temp/CSMarkdownArguementOptions.txt";

        //    string exeFilePath = AppDomain.CurrentDomain.BaseDirectory;
        //    exeFilePath = exeFilePath.Remove(exeFilePath.Length - 27) + "CSMarkdown.App\\bin\\Debug\\CSMarkdown.exe";
        //    Process process = new Process();
        //    process.StartInfo.FileName = exeFilePath;
        //    process.StartInfo.Arguments = "/?";
        //    process.StartInfo.UseShellExecute = false;
        //    process.StartInfo.RedirectStandardError = true;
        //    process.StartInfo.RedirectStandardOutput = true;
        //    process.Start();

        //    StreamReader errorReader = process.StandardError;
        //    string errorOutput = errorReader.ReadToEnd();
        //    StreamReader outputReader = process.StandardOutput;
        //    string output = outputReader.ReadToEnd();

        //    StreamWriter writer = new StreamWriter(outputPath);
        //    if (!string.IsNullOrWhiteSpace(errorOutput))
        //    {
        //        writer.WriteLine(errorOutput);
        //        writer.WriteLine();
        //    }
        //    if (!string.IsNullOrWhiteSpace(output))
        //    writer.WriteLine(output);
        //    writer.Close();

        //    process.WaitForExit();
        //    process.Close();

        //    Process.Start(outputPath);
        //}
    }
}
