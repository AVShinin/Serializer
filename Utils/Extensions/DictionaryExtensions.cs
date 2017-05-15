using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVShinin.Serializer.Utils.Extensions
{
    public static class DictionaryExtensions
    {
        public static T GetOrDefault<K, T>(this IDictionary<K, T> dict, K key, T defaultValue = default(T))
        {
            T result;

            if (dict.TryGetValue(key, out result))
            {
                return result;
            }

            return defaultValue;
        }
    }
}
