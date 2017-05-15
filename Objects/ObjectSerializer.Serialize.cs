using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace AVShinin.Serializer.Objects
{
    public partial class ObjectSerializer
    {
        private SerializeObject Serialize(Type type, object obj, int depth)
        {
            if (++depth > MAX_DEPTH) throw new Exception("Превышено максимальное количество вложений!");

            if (obj == null) return SerializeObject.Null;

            if (obj is SerializeObject) return new SerializeObject((SerializeObject)obj);
            else if (obj is String) return new SerializeObject((String)obj);
            else if (obj is Int32) return new SerializeObject((Int32)obj);
            else if (obj is Int64) return new SerializeObject((Int64)obj);
            else if (obj is Double) return new SerializeObject((Double)obj);
            else if (obj is Decimal) return new SerializeObject((Decimal)obj);
            else if (obj is Byte[]) return new SerializeObject((Byte[])obj);
            else if (obj is Guid) return new SerializeObject((Guid)obj);
            else if (obj is Boolean) return new SerializeObject((Boolean)obj);
            else if (obj is DateTime) return new SerializeObject((DateTime)obj);
            else if (obj is Int16 || obj is UInt16 || obj is Byte || obj is SByte) return new SerializeObject(Convert.ToInt32(obj));
            else if (obj is UInt32) return new SerializeObject(Convert.ToInt64(obj));
            else if (obj is UInt64)
            {
                var ulng = ((UInt64)obj);
                var lng = unchecked((Int64)ulng);

                return new SerializeObject(lng);
            }
            else if (obj is Single) return new SerializeObject(Convert.ToDouble(obj));
            else if (obj is Char || obj is Enum) return new SerializeObject(obj.ToString());
            else if (obj is IDictionary)
            {
                if (type == typeof(object))
                {
                    type = obj.GetType();
                }
                var itemType = type.GetTypeInfo().GenericTypeArguments[1];
                return this.SerializeDictionary(itemType, obj as IDictionary, depth);
            }
            else if (obj is IEnumerable) return this.SerializeArray(GetListItemType(obj.GetType()), obj as IEnumerable, depth);
            else return this.SerializeSObject(type, obj, depth);
        }

        private SerializeObjectArray SerializeArray(Type type, IEnumerable array, int depth)
        {
            var arr = new SerializeObjectArray();

            foreach (var item in array)
            {
                arr.Add(this.Serialize(type, item, depth));
            }

            return arr;
        }

        private SerializeObjectDictionary SerializeDictionary(Type type, IDictionary dict, int depth)
        {
            var o = new SerializeObjectDictionary();

            foreach (var key in dict.Keys)
            {
                var value = dict[key];

                o.Value[key.ToString()] = this.Serialize(type, value, depth);
            }

            return o;
        }

        private SerializeObjectDictionary SerializeSObject(Type type, object obj, int depth)
        {
            var o = new SerializeObjectDictionary();
            var t = obj.GetType();
            var dict = o.Value;

            if (type != t)
            {
                dict["_type"] = new SerializeObject(t.FullName + ", " + t.GetTypeInfo().Assembly.GetName().Name);
            }

            foreach (var member in t.GetRuntimeFields().Where(x => x.GetValue(obj) != null))
            {
                var value = member.GetValue(obj);
                if (value == null) continue;

                dict[member.Name] = this.Serialize(member.FieldType, value, depth);
            }

            return o;
        }
    }
}
