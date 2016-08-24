using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMarkdown.Scripting
{
    public class ChartOptions : BaseChartOptions
    {
        public ChartOptions()
        {
            m_listOfLegends = new List<BaseLegend>();
            m_xAxisType = "";
        }
        private List<string> m_XAxisLabels;
        private string m_LabelColumn;
        private List<BaseLegend> m_listOfLegends;
        private string m_xAxisType;
        private string m_xDataName = "";
        private string m_xAxisDateFormat;
        private string m_showLabels = "true";
        private string m_labelThreshold = ".05";
        private string m_labelType = "key";
        private string m_donutRatio = "0.35";
        public List<string> XAxisLabels
        {
            get
            {
                return m_XAxisLabels;
            }
            set
            {
                m_XAxisLabels = value;
            }
        }
        public string LabelColumn
        {
            get
            {
                return m_LabelColumn;
            }
            set
            {
                m_LabelColumn = value;
            }
        }

        public List<BaseLegend> ListOfLegends
        {
            get
            {
                return m_listOfLegends;
            }
            set
            {
                m_listOfLegends = value;
            }
        }

        public string XAxisType
        {
            get
            {
                return m_xAxisType;
            }

            set
            {
                m_xAxisType = value;
            }
        }

        public string XDataName
        {
            get
            {
                return m_xDataName;
            }

            set
            {
                m_xDataName = value;
            }
        }

        public string XAxisDateFormat
        {
            get
            {
                return m_xAxisDateFormat;
            }

            set
            {
                m_xAxisDateFormat = value;
            }
        }

        public string ShowLabels
        {
            get
            {
                return m_showLabels;
            }
            set
            {
                m_showLabels = value;
            }
        }

        public string LabelThreshold
        {
            get
            {
                return m_labelThreshold;
            }
            set
            {
                m_labelThreshold = value;
            }
        }

        public string LabelType
        {
            get
            {
                return m_labelType;
            }
            set
            {
                m_labelType = value;
            }
        }

        public string DonutRatio
        {
            get
            {
                return m_donutRatio;
            }
            set
            {
                m_donutRatio = value;
            }
        }



    }
}
