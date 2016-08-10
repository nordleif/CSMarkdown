using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;
using CSMarkdown.Rendering;
using CSMarkdown.Hosting;

namespace CSMarkdown.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var renderArgument = new SwitchArgument('r', "render", "", true);
            var webAppArgument = new SwitchArgument('w', "webapp", false);

            try
            {
                var parser = new CommandLineParser.CommandLineParser();
                parser.Arguments.Add(renderArgument);
                parser.ParseCommandLine(args);
    
            }
            catch (CommandLineException ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (renderArgument.Value)
            {
                var renderer = new CSMarkdownRenderer();
                var text = renderer.Render("# Header 1");
            }
            else if (webAppArgument.Value)
            {
                var wepApp = new WebApp();
                wepApp.Start(new StartOptions());
                
                Console.ReadLine();
            }
        }
    }
}
