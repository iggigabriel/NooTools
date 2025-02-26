using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Noo.Tools
{
    public class SfloatJsonConverter : JsonConverter<Sfloat>
    {
        public override Sfloat ReadJson(JsonReader reader, Type objectType, Sfloat existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value is long longVal)
            {
                return new Sfloat((int)longVal);
            }
            else if (reader.Value is int intVal)
            {
                return new Sfloat(intVal);
            }
            else
            {
                return default;
            }
        }

        public override void WriteJson(JsonWriter writer, Sfloat value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Raw);
        }
    }
}
