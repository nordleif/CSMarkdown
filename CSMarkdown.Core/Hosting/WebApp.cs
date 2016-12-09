using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Hosting;
using Owin;
using CSMarkdown.Rendering;
using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace CSMarkdown.Hosting
{
    public class WebApp
    {
        private StartOptions m_options;
        private IDisposable m_webApp;

        public WebApp()
        {

        }

        public void Start(StartOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            m_options = options;
            m_webApp = Microsoft.Owin.Hosting.WebApp.Start("http://localhost/csmarkdown/", (builder) => builder.Use(OnRequest));
        }

        private string DecodeUrlString(string url)
        {
            string newUrl;
            while ((newUrl = Uri.UnescapeDataString(url)) != url)
                url = newUrl;
            return newUrl;
        }


        private async Task OnRequest(IOwinContext context, Func<Task> next)
        {
            context.Response.Headers.Add("Access-Control-Allow-Origin", new string[] { "*" });

            var requestedPath = context.Request.Path.ToString();
            Console.WriteLine(requestedPath);

            // Nicholai Axelgaard
            var incomingParameters = new Dictionary<string, string>();
            if (context.Request.QueryString.HasValue) //Check for query string
            {
                Console.WriteLine(context.Request.QueryString.ToString());
                var paramsString = context.Request.QueryString.ToString().Replace("?", "");
                foreach (var param in paramsString.Split('&'))
                {
                    incomingParameters.Add(param.ToString().Split('=')[0].Replace("%20", " ").Trim(), param.ToString().Split('=')[1].Replace("%20", " ").Trim());
                }
            }
            var pathSegments = requestedPath.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            var firstSegment = pathSegments.FirstOrDefault();

            if (firstSegment != null)
            {
                //Mads Nørgaard
                if (firstSegment.Equals("render"))
                {
                    var markdownPath = "";
                    if (incomingParameters.ContainsKey("path"))
                    {
                        markdownPath = Path.Combine(m_options.WorkingDirectory, $"{incomingParameters["path"] + pathSegments[1].Replace("%20", " ")}.smd");
                    }
                    else
                    {
                        markdownPath = Path.Combine(m_options.WorkingDirectory, $"{pathSegments[1]}.smd");
                        
                    }
                    Console.WriteLine("Markdown Path: " + markdownPath);

                    if (File.Exists(markdownPath))
                    {
                        var text = File.ReadAllText(markdownPath);
                        var renderer = new CSMarkdownRenderer();

                        byte[] result;
                        if (incomingParameters.ContainsKey("pdf"))
                        {
                            result = renderer.Render(text, new CSMarkdownRenderOptions { Output = RenderOutput.Pdf, FlattenHtml = true, Parameters = incomingParameters });
                            context.Response.ContentType = "application/pdf";
                        }
                        else
                        {
                            result = renderer.Render(text, new CSMarkdownRenderOptions { Output = RenderOutput.Html, FlattenHtml = true, Parameters = incomingParameters });
                            context.Response.ContentType = "text/html";
                        }

                        //context.Response.Headers.Add("Access-Control-Allow-Origin", new string[] { "http://localhost:51358" });
                        //context.Response.Headers.Add("Access-Control-Allow-Origin", new string[] { "*" });
                        await context.Response.WriteAsync(result);
                    }
                }


                //Nicholai Axelgaard
                else if (firstSegment.Equals("getReports"))
                {
                    //var markdownPath = Path.Combine(m_options.WorkingDirectory, $"{pathSegments[1]}.smd");
                    string[] reportsArray = Directory.GetFiles(m_options.WorkingDirectory, "*.smd", SearchOption.AllDirectories);
                    for (int i = 0; i < reportsArray.Length; i++)
                    {
                        reportsArray[i] = reportsArray[i].Replace(m_options.WorkingDirectory, "");
                        reportsArray[i] = reportsArray[i].Replace("\\", "/");
                    }
                    var rootFolders = m_options.WorkingDirectory.Split(new string[] { "/", "\\" }, StringSplitOptions.RemoveEmptyEntries);
                    string rootFolderName = rootFolders.Last().First().ToString().ToUpper() + rootFolders.Last().Substring(1);
                    Reports reports = new Reports(rootFolderName);

                    foreach (var reportPath in reportsArray)
                    {
                        reports.AddToCollection(reportPath);
                    }

                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    //settings.Converters.Add(new Converter());
                    settings.Formatting = Formatting.Indented;

                    string json = JsonConvert.SerializeObject(reports, settings);

                    //string origin = context.Response.Headers.Get("Origin");
                    //context.Response.Headers.Add("Access-Control-Allow-Origin", new string[] { "http://localhost:51358" });
                    //context.Response.Headers.Add("Access-Control-Allow-Origin", new string[] { "*" });

                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(json);
                }

                // Nicholai Axelgaard
                else if (firstSegment.Equals("params"))
                {
                    string markdownPath = "";
                    if (incomingParameters.ContainsKey("path"))
                    {
                        markdownPath = Path.Combine(m_options.WorkingDirectory, $"{incomingParameters["path"] + pathSegments[1].Replace("%20", " ")}.smd");
                    }
                    else
                    {
                        markdownPath = Path.Combine(m_options.WorkingDirectory, $"{pathSegments[1]}.smd");

                    }
                    Console.WriteLine("Markdown Path: " + markdownPath);
                    if (File.Exists(markdownPath))
                    {
                        var text = File.ReadAllText(markdownPath);

                        // Create context

                        // Parse YAML and markdown
                        var yamlText = string.Empty;
                        using (var reader = new StringReader(text))
                        {
                            using (StringWriter yamlWriter = new StringWriter(), markdownWriter = new StringWriter())
                            {
                                var isYamlSection = false;
                                var line = string.Empty;
                                while ((line = reader.ReadLine()) != null)
                                {
                                    if (line.TrimEnd().Equals("---", StringComparison.InvariantCultureIgnoreCase))
                                        isYamlSection = !isYamlSection;
                                    else if (isYamlSection)
                                        yamlWriter.WriteLine(line);
                                }
                                yamlText = yamlWriter.ToString();
                            }
                        }

                        Dictionary<object, object> dictionary = null;
                        var deserializer = new Deserializer();
                        using (var reader = new StringReader(yamlText))
                        {
                            var obj = deserializer.Deserialize(reader);
                            dictionary = obj is Dictionary<object, object> ? (Dictionary<object, object>)obj : new Dictionary<object, object>();
                        }

                        var parameters = new List<Parameter>();

                        foreach (var param in dictionary)
                        {
                            if (param.Key.ToString().Contains("params") && param.Value.GetType() == typeof(Dictionary<object, object>))
                            {
                                Parameter parameter;
                                foreach (var para in param.Value as Dictionary<object, object>)
                                {
                                    parameter = new Parameter();
                                    parameter.Key = para.Key.ToString();
                                    if (para.Value.GetType() == typeof(Dictionary<object, object>))
                                    {
                                        foreach (var par in para.Value as Dictionary<object, object>)
                                        {
                                            if (par.Key.ToString().Contains("type"))
                                                parameter.ParamType = par.Value.ToString();

                                            if (par.Key.ToString().Contains("value"))
                                                parameter.Value = par.Value.ToString();
                                        }
                                    }
                                    parameters.Add(parameter);
                                }
                            }
                        }

                        string json = JsonConvert.SerializeObject(parameters);
                        //context.Response.Headers.Add("Access-Control-Allow-Origin", new string[] { "http://localhost:51358" });
                        //context.Response.Headers.Add("Access-Control-Allow-Origin", new string[] { "*" });
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(json);
                    }
                }

                
            }
            await next();
        }
    }
}
