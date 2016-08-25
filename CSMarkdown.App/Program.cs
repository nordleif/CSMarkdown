using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSMarkdown.Rendering;
using CSMarkdown.Hosting;

namespace CSMarkdown.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new ActivationOptions();
            options.WebApp = true;

            var parser = new CommandLineParser.CommandLineParser();
            parser.ShowUsageHeader = "CSMarkdown";
            parser.ExtractArgumentAttributes(options);
            parser.ParseCommandLine(args);

            if (options.WebApp)
                WebApp(options);
            
        }

        static void WebApp(ActivationOptions options)
        {
            var webApp = new WebApp();
            webApp.Start(new StartOptions { WorkingDirectory = @"../../Documents" });
            //webApp.Start(new StartOptions { WorkingDirectory = @"D:\Source\GitHub\CSMarkdown\CSMarkdown.Tests\Documents" });

            Console.WriteLine("WebApp started. Press enter to quit.");
            Console.ReadLine();
        }
    }
}
