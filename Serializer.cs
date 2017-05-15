using AVShinin.Serializer.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVShinin.Serializer
{
    public class Serializer
    {
        public static byte[] Serialize<T>(T value)
        {
            var writer = new ObjectWriter();
            return writer.Serialize((SerializeObjectDictionary)ObjectSerializer.Serialize(value));
        }

        public static T Deserialize<T>(byte[] value)
        {
            var reader = new ObjectReader();
            return ObjectSerializer.Deserialize<T>(reader.Deserialize(value));
        }
    }
}
