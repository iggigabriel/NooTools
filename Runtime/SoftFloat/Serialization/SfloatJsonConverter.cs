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
                return Sfloat.FromInt((int)longVal);
            }
            else if (reader.Value is int intVal)
            {
                return Sfloat.FromInt(intVal);
            }
            else if (reader.Value is double doubleVal)
            {
                var dec = new decimal(doubleVal * 100d);
                return Sfloat.Ratio100((int)dec);
            }
            else
            {
                return default;
            }
        }

        public override void WriteJson(JsonWriter writer, Sfloat value, JsonSerializer serializer)
        {
            var integer = Sfloat.RoundToInt(value);
            var fract = Sfloat.RoundToInt((value - integer) * Sfloat.FromInt(100));
            var dec = integer + fract / 100m;
            writer.WriteValue(dec);
        }
    }
}
