using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMarkdown.Scripting
{
    class CustomDefinedDateConverter
    {
        public string ConvertDateFormat(string definedDateFormat)
        {
            string convertedDateFormat = "";
            List<string> formatDefinitions = new List<string>();
            string specificFormatDefinition = definedDateFormat[0].ToString();
            for (int i = 1; i < definedDateFormat.Length; i++)
            {

                if (definedDateFormat[i - 1] != definedDateFormat[i])
                {
                    if (i == definedDateFormat.Length - 1)
                    {
                        formatDefinitions.Add(specificFormatDefinition);
                        specificFormatDefinition = definedDateFormat[i].ToString();
                        formatDefinitions.Add(specificFormatDefinition);
                    }
                    else
                        formatDefinitions.Add(specificFormatDefinition);
                    specificFormatDefinition = "";
                }

                else if (definedDateFormat[i - 1] == definedDateFormat[i] && i == definedDateFormat.Length - 1)
                {
                    specificFormatDefinition += definedDateFormat[i];
                    formatDefinitions.Add(specificFormatDefinition);
                }
                specificFormatDefinition += definedDateFormat[i];
            }

            foreach (var item in formatDefinitions)
            {
                switch (item)
                {
                    case "d": convertedDateFormat += "%e"; break;
                    case "dd": convertedDateFormat += "%d"; break;
                    case "ddd": convertedDateFormat += "%a"; break;
                    case "dddd": convertedDateFormat += "%A"; break;
                    case "h": convertedDateFormat += "%_I"; break;
                    case "hh": convertedDateFormat += "%I"; break;
                    case "H": convertedDateFormat += "%_H"; break;
                    case "HH": convertedDateFormat += "%H"; break;
                    case "K": convertedDateFormat += "%Z"; break;
                    case "m": convertedDateFormat += "%_M"; break;
                    case "mm": convertedDateFormat += "%M"; break;
                    case "M": convertedDateFormat += "%_m"; break;
                    case "MM": convertedDateFormat += "%m"; break;
                    case "MMM": convertedDateFormat += "%b"; break;
                    case "MMMM": convertedDateFormat += "%B"; break;
                    case "s": convertedDateFormat += "%_S"; break;
                    case "ss": convertedDateFormat += "%S"; break;
                    case "t": convertedDateFormat += "%p"; break;
                    case "tt": convertedDateFormat += "%p"; break;
                    case "y": convertedDateFormat += "%-y"; break;
                    case "yy": convertedDateFormat += "%y"; break;
                    case "yyy": convertedDateFormat += "%_Y"; break;
                    case "yyyy": convertedDateFormat += "%Y"; break;
                    case "yyyyy": convertedDateFormat += "%Y"; break;
                    case "z": convertedDateFormat += "%Z"; break;
                    case "zz": convertedDateFormat += "%Z"; break;
                    case "zzz": convertedDateFormat += "%Z"; break;
                    case "%": convertedDateFormat += "%%"; break;
                    default:
                        convertedDateFormat += item;
                        break;
                }
            }


            return convertedDateFormat;
        }
    }
}
