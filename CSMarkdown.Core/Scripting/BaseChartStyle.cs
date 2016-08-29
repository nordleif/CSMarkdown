using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMarkdown.Scripting
{
    public class BaseChartStyle
    {
        private string chartModelType = "multiChart()";
        private int m_marginTop = 75;
        private int m_marginBottom = 75;
        private int m_marginRight = 75;
        private int m_marginLeft = 75;

        public int MarginTop
        {
            get
            {
                return m_marginTop;
            }

            set
            {
                m_marginTop = value;
            }
        }

        public int MarginBottom
        {
            get
            {
                return m_marginBottom;
            }

            set
            {
                m_marginBottom = value;
            }
        }

        public int MarginRight
        {
            get
            {
                return m_marginRight;
            }

            set
            {
                m_marginRight = value;
            }
        }

        public int MarginLeft
        {
            get
            {
                return m_marginLeft;
            }

            set
            {
                m_marginLeft = value;
            }
        }

        public string ChartModelType
        {
            get
            {
                return chartModelType;
            }

            set
            {
                chartModelType = value;
            }
        }
    }
}
