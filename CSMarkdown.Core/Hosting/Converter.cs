using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMarkdown.Hosting
{
    //Nicholai Axelgaard
    class Converter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Reports));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Reports reports = (Reports)value;
            JObject obj = new JObject();
            if (reports.Files.Count > 0)
            {
                JArray files = new JArray();
                foreach (string name in reports.Files)
                {
                    files.Add(new JValue(name));
                }
                obj.Add("report_smd_files", files);
            }
            foreach (var pair in reports.Folders)
            {
                obj.Add(pair.Key, JToken.FromObject(pair.Value, serializer));
            }
            obj.WriteTo(writer);
        }
    }
}
