using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLineParser.Arguments;

namespace CSMarkdown.App
{
    internal class ActivationOptions
    {
        [SwitchArgument('w', "webapp", true, Description = "Starts the web app")]
        public bool WebApp { get; set; }

    }
}
