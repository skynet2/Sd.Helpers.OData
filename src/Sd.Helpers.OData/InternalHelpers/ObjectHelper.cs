using System;
using System.Collections.Generic;
using System.Text;

namespace Sd.Helpers.OData.InternalHelpers
{
    internal class ObjectHelper
    {
        internal static object GetPropValue(object item, string propName)
        {
            if (item == null)
                return null;

            var prop = item.GetType().GetProperty(propName);

            return prop != null ? prop.GetValue(item) : null;
        }

        internal static void SetPropValue(object item, string propName, object value)
        {
            var propertyInfo = item.GetType().GetProperty(propName);

            if (propertyInfo != null)
                propertyInfo.SetValue(item, ChangeType(value, propertyInfo.PropertyType), null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ChangeType(object value, Type type)
        {
            switch (value)
            {
                case null when type.IsGenericType:
                    return Activator.CreateInstance(type);
                case null:
                    return null;
                default:
                    break;
            }

            if (type == value.GetType()) return value;
            if (type.IsEnum)
            {
                return value is string ? Enum.Parse(type, (string)value) : Enum.ToObject(type, value);
            }

            if (!type.IsInterface && type.IsGenericType)
            {
                var innerType = type.GetGenericArguments()[0];
                var innerValue = ChangeType(value, innerType);
                return Activator.CreateInstance(type, innerValue);
            }

            switch (value)
            {
                case string _ when type == typeof(Guid):
                    return new Guid(value as string ?? throw new InvalidOperationException());
                case string _ when type == typeof(Version):
                    return new Version(value as string ?? throw new InvalidOperationException());
            }

            return !(value is IConvertible) ? value : Convert.ChangeType(value, type);
        }
    }
}
