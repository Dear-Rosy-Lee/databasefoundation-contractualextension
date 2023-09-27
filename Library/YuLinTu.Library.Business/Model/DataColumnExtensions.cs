using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YuLinTu.Data;

namespace YuLinTu.Library.Business
{
    public static class DataColumnExtensions
    {
        public static List<DataColumn> GetDataColumns(this ObjectContext oc)
        {
            return oc.Columns.Select(x =>
            {
                DataColumn col = x.Value.DataColumn.ConvertTo<DataColumn>();
                col.PropertyName = x.Key;
                return col;
            }).ToList();
        }

        public static List<DataColumn> GetDataColumns(this Type type)
        {
            return ObjectContext.Create(type).GetDataColumns();
        }

        public static T ConvertTo<T>(this object source) where T : new()
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            T val2 = new T();
            Type tType = typeof(T);
            source.TraversalPropertiesInfo(delegate (PropertyInfo pi, object val, object t)
            {
                PropertyInfo property = tType.GetProperty(pi.Name);
                if (property == null)
                {
                    return true;
                }

                if (!property.CanWrite)
                {
                    return true;
                }

                if (pi.PropertyType != property.PropertyType)
                {
                    return true;
                }

                property.SetValue(t, val, null);
                return true;
            }, val2);
            return val2;
        }

        public static void Add<TKey, TValue>(this KeyValueList<TKey, TValue> list, TKey key, TValue value)
        {
            list.Add(new KeyValue<TKey, TValue>(key, value));
        }

        public static object ExecuteBySQL(this IDbContext dbContext, string sql)
        {
            var query = dbContext.CreateQuery();
            query.CommandContext.CommandText.Clear();
            query.CommandContext.CommandText.Append(sql);
            return query.Execute();
        }
    }
}