using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSMarkdown.Rendering;
using CSMarkdown.Hosting;
using System.IO;
using System.Diagnostics;

namespace CSMarkdown.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new ActivationOptions();

            var parser = new CommandLineParser.CommandLineParser();
            parser.ShowUsageHeader = "CSMarkdown";
            parser.ExtractArgumentAttributes(options);
            parser.ParseCommandLine(args);


            if (options.WebApp)
                WebApp(options);
            else if (options.Render)
                Render(options);

            Environment.Exit(0);
        }

        static void Render(ActivationOptions options)
        {
            if (options.InputPath == null)
                options.InputPath = @"../../Documents/";

            else if (options.InputPath.Contains(options.SmdFileName))
                options.SmdFileName = "";

            if (options.OutputPath == null)
                options.OutputPath = options.InputPath;

            if (options.OutputType == null)
                options.OutputType = "html";

            else if (!options.OutputType.Contains("html") && !options.OutputType.Contains("pdf"))
                options.OutputType = "html";

            Dictionary<string, string> parameters = new Dictionary<string, string>();

            if (options.Parameters != null)
            {
                options.Parameters = options.Parameters.Replace("\"", "");
                int padsBetweenCommas = 0;
                bool isKey = true;
                string key = "", value = "";
                for (int i = 0; i < options.Parameters.Length + 1; i++)
                {
                    if ((isKey && !string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value)) || i >= options.Parameters.Length)
                    {
                        if (value[0] == ' ')
                        {
                            value = value.Remove(0, 1);
                            i--;
                        }
                        else if(value[value.Length-1] == ' ')
                        {
                            value = value.Remove(value.Length-1, 1);
                            i--;
                        }
                        else
                        {
                            parameters.Add(key.Trim(), value);
                            key = "";
                            value = "";
                            padsBetweenCommas = 0;
                        }
                    }
                    else if (isKey)
                    {
                        if (options.Parameters[i] == '=')
                            isKey = false;

                        else
                            key += options.Parameters[i];


                    }
                    else if (!isKey)
                    {
                        if (options.Parameters[i] == '=')
                        {
                            isKey = true;
                            i -= padsBetweenCommas + 1;
                            if (value.Length > padsBetweenCommas)
                                value = value.Remove(value.Length - padsBetweenCommas);
                        }
                        else if (options.Parameters[i] == ',')
                        {
                            padsBetweenCommas = 1;
                            value += options.Parameters[i];
                        }
                        else
                        {
                            value += options.Parameters[i];
                            padsBetweenCommas++;
                        }
                    }
                }
                //if (!isKey && !string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
                //{
                //    parameters.Add(key.Trim(), value);
                //}
            }

            var path = Path.Combine(options.InputPath, options.SmdFileName);
            var text = File.ReadAllText(path);

            var renderer = new CSMarkdownRenderer();

            var renderOutput = new RenderOutput();
            if (options.OutputType.Contains("pdf"))
            {
                renderOutput = RenderOutput.Pdf;
            }
            else
                renderOutput = RenderOutput.Html;

            var result = renderer.Render(text, new CSMarkdownRenderOptions { Output = renderOutput, FlattenHtml = options.FalttenHtml, Parameters = parameters });

            if (options.NewFileName == null)
                path = options.OutputPath + "\\" + options.SmdFileName.Replace(Path.GetExtension(options.SmdFileName), renderOutput == RenderOutput.Html ? ".html" : ".pdf");
            else
                path = options.OutputPath + "\\" + options.NewFileName + (renderOutput == RenderOutput.Html ? ".html" : ".pdf");

            File.WriteAllBytes(path, result);
        }

        static void WebApp(ActivationOptions options)
        {
            var webApp = new WebApp();
            webApp.Start(new StartOptions { WorkingDirectory = @"C:\Users\Nicholai\Desktop\CSMarkdownGit\CSMarkdown.Tests\Documents" });
            //webApp.Start(new StartOptions { WorkingDirectory = @"D:\Source\GitHub\CSMarkdown\CSMarkdown.Tests\Documents" });

            Console.WriteLine("WebApp started. Press enter to quit.");
            Console.ReadLine();
        }
    }
}
