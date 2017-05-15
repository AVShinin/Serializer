using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace AVShinin.Serializer.Objects
{
    public partial class ObjectSerializer
    {
        private static ObjectSerializer Instance = new ObjectSerializer();
        private const int MAX_DEPTH = 200;

        private Type GetListItemType(Type listType)
        {
            if (listType.IsArray) return listType.GetElementType();

            foreach (var i in listType.GetTypeInfo().ImplementedInterfaces)
            {
                if (i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    return i.GetTypeInfo().GenericTypeArguments[0];
                }
                else if (listType.GetTypeInfo().IsGenericType && i == typeof(IEnumerable))
                {
                    return listType.GetTypeInfo().GenericTypeArguments[0];
                }
            }

            return typeof(object);
        }

        private bool IsList(Type type)
        {
            if (type.IsArray) return true;

            foreach (var @interface in type.GetTypeInfo().ImplementedInterfaces)
            {
                if (@interface.GetTypeInfo().IsGenericType)
                {
                    if (@interface.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static SerializeObject Serialize<T>(T obj)
        {
            return Instance.Serialize(typeof(T), obj, 0);
        }

        public static T Deserialize<T>(SerializeObject obj)
        {
            return (T)Instance.Deserialize(typeof(T), obj);
        }
    }
}
