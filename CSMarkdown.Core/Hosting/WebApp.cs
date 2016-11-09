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

        private async Task OnRequest(IOwinContext context, Func<Task> next)
        {
            //Mads Nørgaard
            var requestedPath = context.Request.Path.ToString();
            Console.WriteLine(requestedPath);


            /*if (context.Request.QueryString.ToString() != null) //Check for query string
            {
                var param = context.Request.QueryString.ToString();
                param = param.Remove(0, 1); //Remove &
                var paramSegments = param.Split(new char[] { '&' });

                var parameters = new Dictionary<string, string>();
                foreach (var p in paramSegments)
                {
                    var paramss = p.Split('=');
                    parameters.Add(paramss[0], paramss[1]);
                }

            }*/

            var pathSegments = requestedPath.Split(new string[] {"/"}, StringSplitOptions.RemoveEmptyEntries);
            var firstSegment = pathSegments.FirstOrDefault();

            if (firstSegment != null)
            {
                //Mads Nørgaard
                if(firstSegment.Equals("render"))
                {
                    var markdownPath = Path.Combine(m_options.WorkingDirectory, $"{pathSegments[1]}.smd");
                    if (File.Exists(markdownPath))
                    {
                        var text = File.ReadAllText(markdownPath);
                        var renderer = new CSMarkdownRenderer();
                        var result = renderer.Render(text, new CSMarkdownRenderOptions { Output = RenderOutput.Html, FlattenHtml = true });

                        context.Response.ContentType = "text/html";
                        await context.Response.WriteAsync(result);
                    }
                }
                
                //Nicholai Axelgaard
                else if(firstSegment.Equals("getReports"))
                {
                    //var markdownPath = Path.Combine(m_options.WorkingDirectory, $"{pathSegments[1]}.smd");
                    string[] reportsArray = Directory.GetFiles(m_options.WorkingDirectory, "*.smd", SearchOption.AllDirectories);
                    for (int i = 0; i < reportsArray.Length; i++)
                    {
                        reportsArray[i] = reportsArray[i].Replace(m_options.WorkingDirectory, "");
                        reportsArray[i] = reportsArray[i].Replace("\\", "/");
                    }
                    Reports reports = new Reports();

                    foreach (var reportPath in reportsArray)
                    {
                        reports.AddToCollection(reportPath);
                    }

                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    settings.Converters.Add(new Converter());
                    settings.Formatting = Formatting.Indented;

                    string json = JsonConvert.SerializeObject(reports, settings);

                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(json);
                }  
            }
            
            await next();
        }



    }
}
