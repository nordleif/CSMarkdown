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
        [SwitchArgument('w', "webapp", false, Description = "Starts the web app")]
        public bool WebApp { get; set; }

        [SwitchArgument('r', "render", false, Description = "Render report")]
        public bool Render { get; set; }

        [SwitchArgument('f', "flattenhtml", true, Description = "Use when you don't want the html to be flattened. Default is true")]
        public bool FalttenHtml { get; set; }

        [ValueArgument(typeof(string), 's', LongName = "smdfile", Description = "Name of the .smd file to use")]
        public string SmdFileName { get; set; }

        [ValueArgument(typeof(string), 'n', LongName = "newfilename", Description = "Name of the output file. If name consists of white spaces, use \"[name]\" around the name")]
        public string NewFileName { get; set; }

        [ValueArgument(typeof(string), 'i', LongName = "inputpath", Description = "Holds the path of the smd file to use")]
        public string InputPath { get; set; }

        [ValueArgument(typeof(string), 'o', LongName = "outputpath", Description = "Holds the path of where to put the output file")]
        public string OutputPath { get; set; }

        [ValueArgument(typeof(string), 't', LongName = "outputtype", Description = "Define which type the output document should be")]
        public string OutputType { get; set; }

        [ValueArgument(typeof(string), 'p', LongName = "parameters", Description = "Holds the parameters for the report. If the argument with parameters contains whitespaces, then use \"[parameters]\" to encapsulate", Example = "from=2016-04-01, to=2016-01-03, tag=NameTag")]
        public string Parameters { get; set; }
    }
}
