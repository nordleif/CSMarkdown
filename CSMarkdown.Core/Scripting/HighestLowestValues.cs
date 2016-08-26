using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMarkdown.Scripting
{
    class HighestLowestValues
    {
        private double m_highestValue;
        private double m_lowestValue;
        private double m_highestLowestDifference;
        private int m_columnIndex;
        private int m_leftOrRightYAxis;
        private int m_indexInOriginalList;

        public double HighestValue
        {
            get
            {
                return m_highestValue;
            }

            set
            {
                m_highestValue = value;
            }
        }

        public double LowestValue
        {
            get
            {
                return m_lowestValue;
            }

            set
            {
                m_lowestValue = value;
            }
        }

        public double HighestLowestDifference
        {
            get
            {
                return m_highestLowestDifference;
            }

            set
            {
                m_highestLowestDifference = value;
            }
        }

        public int ColumnIndex
        {
            get
            {
                return m_columnIndex;
            }

            set
            {
                m_columnIndex = value;
            }
        }

        public int LeftOrRightYAxis
        {
            get
            {
                return m_leftOrRightYAxis;
            }

            set
            {
                m_leftOrRightYAxis = value;
            }
        }

        public int IndexInOriginalList
        {
            get
            {
                return m_indexInOriginalList;
            }

            set
            {
                m_indexInOriginalList = value;
            }
        }
    }
}
