using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace AVShinin.Serializer.Objects
{
    public partial class ObjectSerializer
    {
        private bool IsNullable(Type type)
        {
            if (!type.GetTypeInfo().IsGenericType) return false;
            var g = type.GetGenericTypeDefinition();
            return (g.Equals(typeof(Nullable<>)));
        }
        private Type UnderlyingTypeOf(Type type)
        {
            if (!type.GetTypeInfo().IsGenericType) return type;
            return type.GetTypeInfo().GenericTypeArguments[0];
        }

        private object Deserialize(Type type, SerializeObject value)
        {
            if (value.IsNull) return null;
            else if (IsNullable(type)) type = UnderlyingTypeOf(type);

            if (type == typeof(SerializeObject)) return new SerializeObject(value);
            else if (type == typeof(SerializeObjectDictionary)) return value.AsDictionary;
            else if (type == typeof(SerializeObjectArray)) return value.AsArray;

            else if (type == typeof(UInt64)) return unchecked((UInt64)((Int64)value.Value));
            else if (type.GetTypeInfo().IsEnum) return Enum.Parse(type, value.AsString);
            else if (value.IsArray)
            {
                if (type == typeof(object)) return this.DeserializeArray(typeof(object), value.AsArray);
                if (type.IsArray) return this.DeserializeArray(type.GetElementType(), value.AsArray);
                else return this.DeserializeList(type, value.AsArray);
            }
            else if (value.IsDictionary)
            {
                SerializeObject typeField;
                var dict = value.AsDictionary;

                if (dict.Value.TryGetValue("_type", out typeField))
                {
                    type = Type.GetType(typeField.AsString);
                    if (type == null) throw new Exception($"Тип {typeField.AsString} не найден!");
                }
                else if (type == typeof(object)) type = typeof(Dictionary<string, object>);

                var o = Activator.CreateInstance(type);

                if (o is IDictionary && type.GetTypeInfo().IsGenericType)
                {
                    var k = type.GetTypeInfo().GenericTypeArguments[0];
                    var t = type.GetTypeInfo().GenericTypeArguments[1];
                    this.DeserializeDictionary(k, t, (IDictionary)o, value.AsDictionary);
                }
                else
                {
                    this.DeserializeObject(type, o, dict);
                }

                return o;
            }
            return value.Value;
        }

        private object DeserializeArray(Type type, SerializeObjectArray array)
        {
            var arr = Array.CreateInstance(type, array.Count);
            var idx = 0;

            foreach (var item in array)
            {
                arr.SetValue(this.Deserialize(type, item), idx++);
            }

            return arr;
        }

        private object DeserializeList(Type type, SerializeObjectArray value)
        {
            var itemType = GetListItemType(type);
            var enumerable = (IEnumerable)Activator.CreateInstance(type);
            var list = enumerable as IList;

            if (list != null) foreach (SerializeObject item in value) list.Add(Deserialize(itemType, item));
            else
            {
                var addMethod = type.GetRuntimeMethod("Add", new Type[1] { itemType });
                foreach (SerializeObject item in value) addMethod.Invoke(enumerable, new[] { Deserialize(itemType, item) });
            }
            return enumerable;
        }

        private void DeserializeDictionary(Type K, Type T, IDictionary dict, SerializeObjectDictionary value)
        {
            foreach (var key in value.Keys)
            {
                var k = K.GetTypeInfo().IsEnum ? Enum.Parse(K, key) : Convert.ChangeType(key, K);
                var v = this.Deserialize(T, value[key]);

                dict.Add(k, v);
            }
        }

        private void DeserializeObject(Type type, object obj, SerializeObjectDictionary value)
        {
            foreach (var member in type.GetRuntimeFields())
            {
                var val = value[member.Name];

                if (!val.IsNull)
                {
                    member.SetValue(obj, this.Deserialize(member.FieldType, val));
                }
            }
        }
    }
}
