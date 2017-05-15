using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AVShinin.Serializer.Utils.Extensions
{
    public static class MemberInfoExtension
    {
        public static object GetValue(this MemberInfo member, object obj)
        {
            var field = member as FieldInfo;
            if (field != null)
                return field.GetValue(obj);
            var property = member as PropertyInfo;
            if (property != null)
                return property.GetValue(obj);

            throw new NotSupportedException($"The type '{member.GetType()}' is not supported.");
        }

        public static void SetValue(this MemberInfo member, object obj, object value)
        {
            var field = member as FieldInfo;
            if (field != null)
                field.SetValue(obj, value);
            var property = member as PropertyInfo;
            if (property != null)
                property.SetValue(obj, value);

            throw new NotSupportedException($"The type '{member.GetType()}' is not supported.");
        }
    }
}
