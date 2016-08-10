using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Hosting;
using Owin;

namespace CSMarkdown.Hosting
{
    public class WebApp
    {
        private IDisposable m_webApp;

        public WebApp()
        {

        }

        public void Start(StartOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            m_webApp = Microsoft.Owin.Hosting.WebApp.Start("http://localhost/csmarkdown/", (builder) => builder.Use(OnRequest));
        }

        private async Task OnRequest(IOwinContext context, Func<Task> next)
        {
            await next();
        }
    }
}
