using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMarkdown.Scripting
{
    public class LineLegend : BaseLegend
    {
        private string m_type = "line";

        public LineLegend()
        {
            Values = new Dictionary<string, string>();
        }

        public string Type
        {
            get
            {
                return m_type;
            }
        }

        
    }
}
