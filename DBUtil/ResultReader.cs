using System;
using System.Data.Odbc;

namespace SFDemo
{
    internal class ResultReader
    {
        internal static T ToDomainObject<T>(Type modelType, OdbcDataReader reader) where T : new()
        {
            var item = new T();
            for (var i = 0; i < reader.FieldCount; i++)
                SetPropertyValue(modelType, reader.GetName(i), reader.GetValue(i), item);
            return item;
        }

        private static void SetPropertyValue<T>(Type modelType, string name, object value, T item) where T : new()
        {
            if (value is DBNull) return;

            var normalizedPropertyName = NormalizePropertyName(name);
            var p = modelType.GetProperty(normalizedPropertyName);
            if (p == null) return;

            var t = (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        ? p.PropertyType.GetGenericArguments()[0]
                        : p.PropertyType;
            p.SetValue(item, value is IConvertible ? Convert.ChangeType(value, t) : value, null);
        }

        internal static String NormalizePropertyName(string name)
        {
            var chars = name.ToCharArray();
            var len = name.Length;

            for (var i = 0; i < len; i++)
            {
                var c = chars[i];
                if (!(c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c == '_' || (c >= '0' && c <= '9' && i > 0)))
                    chars[i] = '_';
            }

            return new string(chars);
        }
    }
}
