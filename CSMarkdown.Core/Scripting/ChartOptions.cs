using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMarkdown.Scripting
{
    public class ChartOptions : BaseChartStyle
    {
        public ChartOptions()
        {
            m_legends = new List<BaseLegend>();
            m_xAxisType = "";
        }
        private List<string> m_XAxisLabels;
        private string m_LabelColumn;
        private List<BaseLegend> m_legends;
        private string m_xAxisType;
        private string m_xDataName = "";
        private string m_xAxisDateFormat;
        private string m_showLabels = "false";
        private string m_labelThreshold = ".05";
        private string m_labelType = "key";
        private string m_donutRatio = "0.35";
        private bool m_showMaxMin = false;
        private int m_rotateLabels = 45;
        private bool m_showAllTicks = true;
        private int m_maxAmountOfTicks = 29;

        private string m_YAxisLabel;
        private string m_XAxisLabel;
        private string m_YAxisLabel2;

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

        public List<BaseLegend> Legends
        {
            get
            {
                return m_legends;
            }
            set
            {
                m_legends = value;
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

        public bool ShowMaxMin
        {
            get
            {
                return m_showMaxMin;
            }

            set
            {
                m_showMaxMin = value;
            }
        }

        public int RotateLabels
        {
            get
            {
                return m_rotateLabels;
            }
            set
            {
                m_rotateLabels = value;
            }
        }

        public string YAxisLabel
        {
            get { return m_YAxisLabel; }
            set { m_YAxisLabel = value; }
        }
        

        public string YAxisLabel2
        {
            get { return m_YAxisLabel2; }
            set { m_YAxisLabel2 = value; }
        }

        public string XAxisLabel
        {
            get { return m_XAxisLabel; }
            set { m_XAxisLabel = value; }
        }

        public bool ShowAllTicks
        {
            get
            {
                return m_showAllTicks;
            }

            set
            {
                m_showAllTicks = value;
            }
        }

        public int MaxAmountOfTicks
        {
            get
            {
                return m_maxAmountOfTicks;
            }

            set
            {
                m_maxAmountOfTicks = value;
            }
        }
    }
}
