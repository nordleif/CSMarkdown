using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMarkdown.Scripting
{
    public enum Interval
    {
        Data,
        Hour,
        Day,
        Month
    }

    internal enum DateTimeComponents
    {
        Year,
        Month,
        Day,
        Hour,
        Minute,
        Second
    }

    static class DateTimeComponentsExtension
    {
        public static int GetDateTimeComponent(this DateTimeComponents components, DateTime dateTime)
        {
            switch (components)
            {
                case DateTimeComponents.Year: return dateTime.Year;
                case DateTimeComponents.Month: return dateTime.Month;
                case DateTimeComponents.Day: return dateTime.Day;
                case DateTimeComponents.Hour: return dateTime.Hour;
                case DateTimeComponents.Minute: return dateTime.Minute;
                case DateTimeComponents.Second: return dateTime.Second;
                default: return 0;
            }
        }
    }
}
