using Hammergo.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DamServiceV3.Helper
{
    public class AppParamConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(AppParam).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader,
            Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject item = JObject.Load(reader);

          

            if (item["Val"] !=null)
            {
                return item.ToObject< ConstantParam>();
            }
            else if (item["CalculateValues"] != null)
            {
                return item.ToObject< CalculateParam>();
            }else
            {
                return item.ToObject<MessureParam>();
            }
        }

        public override void WriteJson(JsonWriter writer,
            object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}