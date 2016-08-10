using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMarkdown.Rendering
{
    internal class CodeChunkOptions
    {
        #region Static Members

        public static CodeChunkOptions Parse(string text)
        {
            var dictionary = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

            if (text.StartsWith("{s", StringComparison.InvariantCultureIgnoreCase))
            {
                text = text.Substring(2);
                text = text.TrimEnd('}');

                var items = text.Split(',');
                foreach(var item in items)
                {
                    var keyValue = item.Split('=');
                    var key = keyValue[0].Trim();
                    object value = keyValue.Length > 1 ? keyValue[1] : null;

                    if (((string)value).Equals("true", StringComparison.InvariantCultureIgnoreCase))
                        value = true;
                    else if (((string)value).Equals("false", StringComparison.InvariantCultureIgnoreCase))
                        value = false;

                    if (!dictionary.ContainsKey(key))
                        dictionary.Add(key, value);
                }
            }
            
            return new CodeChunkOptions { m_dictionary = dictionary };
        }

        #endregion

        private Dictionary<string, object> m_dictionary;

        private CodeChunkOptions()
        {

        }

        public object ReadValue(string key, object defaultValue)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (m_dictionary.ContainsKey(key))
                return m_dictionary[key];

            return defaultValue;
        }

        public T ReadValue<T>(string key, T defaultValue)
        {
            try
            {
                return (T)Convert.ChangeType(ReadValue(key, (object)defaultValue), typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
