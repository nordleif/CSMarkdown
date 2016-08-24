using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMarkdown.Scripting
{
    public class BarLegend : BaseLegend
    {
        public BarLegend()
        {
            Values = new Dictionary<string, string>();
        }

        private string type = "bar";

        public string Type
        {
            get
            {
                return type;
            }
        }
    }
}
