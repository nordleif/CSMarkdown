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
            var param = context.Request.QueryString.ToString();
            if(param != null)
            {
                param = param.Remove(0, 1); //Remove &
                var paramSegments = param.Split(new char[] { '&' });

                var parameters = new Dictionary<string, string>();
                foreach (var p in paramSegments)
                {
                    var paramss = p.Split('=');
                    parameters.Add(paramss[0], paramss[1]);
                }
            }
            Console.WriteLine(requestedPath);


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
                
            }
            

            await next();
        }


        /*private async Task OnRequest(IOwinContext context, Func<Task> next)
        {

            // Urls:
            // -----
            //
            // /render/<smdname>?rc:output=html?intparam=1&boolparam=true
            // /render/reports
            //
            //  /reports/<smdname >


            Console.WriteLine(context.Request.Path);

            var requestPath = context.Request.Path.ToUriComponent(); // context.Request.Uri.LocalPath;
            var requestUri = new Uri(requestPath, UriKind.Relative);
            
            var segments = requestPath.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            var firstSegment = segments.FirstOrDefault();
            
            if (firstSegment != null)
            {
                if (firstSegment.Equals("render", StringComparison.InvariantCultureIgnoreCase) && segments.Length == 2)
                {
                    // Render
                    var markdownPath = Path.Combine(m_options.WorkingDirectory, $"{segments[1]}.smd");
                    if (File.Exists(markdownPath))
                    {
                        var text = File.ReadAllText(markdownPath);
                        var renderer = new CSMarkdownRenderer();
                        var responseText = renderer.Render(text, new CSMarkdownRenderOptions { Output = RenderOutput.Html, FlattenHtml = true });

                        context.Response.ContentType = "text/html";
                        await context.Response.WriteAsync(responseText);
                    }
                }
                else
                {
                    // Read file 
                    var fileUri = new Uri($"file://{m_options.WorkingDirectory}{requestPath}");
                    var filePath = fileUri.LocalPath;
                    if (File.Exists(filePath))
                    {
                        // Read file
                        var responseText = File.ReadAllText(filePath);
                        context.Response.ContentType = "text/html";
                        await context.Response.WriteAsync(responseText);
                    }
                    else
                    {
                        // read resource
                    }
                }
            }
            
            await next();
        }*/
    }
}
