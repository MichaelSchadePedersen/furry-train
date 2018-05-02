using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace Toolpack.Shared.BusinessLogic
{
    public static class Extensions
    {
        
        public static dynamic ToDynamic(this object value, String apiUrl)
        {
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(value.GetType()))
            {
                expando.Add(property.Name, property.GetValue(value));
            }
            expando.Add("Link", apiUrl);
            return expando as ExpandoObject;
        }

        public static List<dynamic> ToDynamicList<T>(this IEnumerable<T> objects, String apiUrl, String identity = "Id")
        {
            return ToDynamicList(objects.ToList(), apiUrl, identity);
        }

        public static List<dynamic> ToDynamicList<T>(this List<T> objects, String apiUrl, String identity = "Id")
        {
            var results = new List<dynamic>();
            foreach (var obj in objects)
            {
                IDictionary<string, object> expando = new ExpandoObject();
                foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(obj.GetType()))
                {
                    expando.Add(property.Name, property.GetValue(obj));
                }

                Object identifier = null;
                var ok = expando.TryGetValue(identity, out identifier);

                if (identifier != null)
                    expando.Add("Link", apiUrl + "/" + identifier);
                else
                    expando.Add("Link", apiUrl);
                results.Add(expando);
            }
            return results;
        }

        public static void RemoveNullValues(this object obj)
        {
            var properties = obj.GetType().GetProperties().ToList();
            foreach (var p in properties)
            {
                if (p.PropertyType.FullName == "System.String" && p.GetValue(obj) == null)
                    p.SetValue(obj, "");
                if (p.PropertyType.FullName == "System.DateTime" && (p.GetValue(obj) == null || Convert.ToDateTime(p.GetValue(obj)) == DateTime.MinValue))
                    p.SetValue(obj, new DateTime(1900, 1, 1));
                if (p.PropertyType.FullName == "System.Int32" && p.GetValue(obj) == null)
                    p.SetValue(obj, 0);
            }
        }

        public static Int32 ToSafeInt32(this String s, Int32 defaultValue = 0)
        {
            if (String.IsNullOrEmpty(s))
                return defaultValue;
            Int32 output = 0;
            Int32.TryParse(s, out output);
            return output;
        }

        

        public static List<List<T>> SpliceList<T>(this List<T> objects, Int32 maxItems)
        {
            var results = new List<List<T>>();
            var temporaryObject = new Queue<T>(objects);

            var subList = new List<T>();
            while (temporaryObject.Count > 0)
            {
                subList.Add(temporaryObject.Dequeue());
                if (subList.Count >= maxItems || temporaryObject.Count == 0)
                {
                    results.Add(subList);
                    subList = new List<T>();
                }
            }

            return results;
        }

        public static decimal ToDecimal(this double d)
        {
            return Convert.ToDecimal(d);
        }

        public static List<PropertyInfo> GetDbSetProperties(this DbContext context)
        {
            var dbSetProperties = new List<PropertyInfo>();
            var properties = context.GetType().GetProperties();

            foreach (var property in properties)
            {
                var setType = property.PropertyType;

                //#if EF5 || EF6
                var isDbSet = setType.IsGenericType && (typeof(IDbSet<>).IsAssignableFrom(setType.GetGenericTypeDefinition()) || setType.GetInterface(typeof(IDbSet<>).FullName) != null);
                //#elif EF7
                //var isDbSet = setType.IsGenericType && (typeof (DbSet<>).IsAssignableFrom(setType.GetGenericTypeDefinition()));
                //#endif

                if (isDbSet)
                {
                    dbSetProperties.Add(property);
                }
            }

            return dbSetProperties;

        }
    }

}
