using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMarkdown.Scripting
{
    public class PieLegend : BaseLegend
    {
        public PieLegend()
        {
            Values = new Dictionary<string, string>();
        }

        private string m_Type = "pie";

        public string Type
        {
            private set
            {
                m_Type = value;
            }
            get
            {
                return m_Type;
            }
        }
        public bool IsDonut
        {
            get
            {
                if (Type == "donut")
                    return true;
                return false;
            }
            set
            {
                if (value)
                    Type = "donut";
                else
                    Type = "pie";
            }
        }
    }
}
