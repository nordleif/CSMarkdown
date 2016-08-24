using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMarkdown.Scripting
{
    public class BaseLegend
    {
        private string m_key;
        private Dictionary<string, string> m_values;
        private int? m_minValue;
        private int? m_maxValue;
        private string m_YDataName = "";

        public string Key
        {
            get
            {
                return m_key;
            }

            set
            {
                m_key = value;
            }
        }

        public Dictionary<string, string> Values
        {
            get
            {
                return m_values;
            }

            set
            {
                m_values = value;
            }
        }

        public int? MinValue
        {
            get
            {
                return m_minValue;
            }

            set
            {
                m_minValue = value;
            }
        }

        public int? MaxValue
        {
            get
            {
                return m_maxValue;
            }

            set
            {
                m_maxValue = value;
            }
        }

        public string YDataName
        {
            get
            {
                return m_YDataName;
            }

            set
            {
                m_YDataName = value;
            }
        }
    }
}
