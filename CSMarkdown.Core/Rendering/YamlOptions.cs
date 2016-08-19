using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CSMarkdown.Rendering
{
    internal class YamlOptions
    {
        #region Static Members

        public static YamlOptions Parse(string yamlText)
        {
            Dictionary<object, object> dictionary = null;
            var deserializer = new Deserializer();
            using (var reader = new StringReader(yamlText))
            {
                var obj = deserializer.Deserialize(reader);
                dictionary = obj is Dictionary<object, object> ? (Dictionary<object, object>)obj : new Dictionary<object, object>();
            }
            return new YamlOptions { m_dictionary = dictionary };
        }

        #endregion

        private Dictionary<object, object> m_dictionary;

        private YamlOptions()
        {

        }

        public object ReadValue(string key, object defaultValue)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            object result = m_dictionary;
            var items = key.ToLowerInvariant().Split('.');
            foreach (var item in items)
            {
                var dictionary = result as Dictionary<object, object>;
                if (dictionary == null)
                    return defaultValue;

                if (!dictionary.ContainsKey(item))
                    return defaultValue;

                result = dictionary[item];
            }

            return result;
        }

        public string[] ReadKeys(string key)
        {
            var obj = ReadValue(key, null);
            var dictionary = obj as Dictionary<object, object>;
            if (dictionary != null)
                return dictionary.Keys.Select(k => (string)k).ToArray();
            return new string[0];
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
