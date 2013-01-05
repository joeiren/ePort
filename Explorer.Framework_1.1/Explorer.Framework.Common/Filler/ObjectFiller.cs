using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Explorer.Framework.Common.Filler
{
    public class ObjectFiller
    {

        /// <summary>
        /// 把 key=value|key=value 这种字符串, 填到对象属性里
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <returns></returns>
        public static T FillPropertyByString<T>(string content) where T : new()
        {
            Type classType = typeof(T);
            T t = new T();
            string[] values = content.Split('=','|');
            for (int i = 0; i < values.Length; i+=2)
            {
                PropertyInfo propertyInfo = classType.GetProperty(values[i]);
                string value = values[i + 1];
                if (!string.IsNullOrEmpty(value))
                {
                    if (value is String && string.IsNullOrEmpty(value.ToString()))
                    {
                        propertyInfo.SetValue(t, null, null);
                    }
                    else
                    {
                        try
                        {
                            Type targetType;
                            if (propertyInfo.PropertyType.Name.StartsWith("Nullable"))
                            {
                                targetType = propertyInfo.PropertyType.GetGenericArguments()[0];
                            }
                            else
                            {
                                targetType = propertyInfo.PropertyType;
                            }
                            propertyInfo.SetValue(t, Convert.ChangeType(value, targetType), null);
                        }
                        catch { }
                    }
                }

            }
            return t;
        }

        /// <summary>
        /// 把 key=value|key=value 这种字符串, 填到对象属性里
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="content"></param>
        public static void FillPropertyByString(ref object entity, string content)
        {
            Type classType = entity.GetType();
            string[] values = content.Split('=', '|');
            for (int i = 0; i < values.Length; i += 2)
            {
                PropertyInfo propertyInfo = classType.GetProperty(values[i]);
                string value = values[i + 1];
                if (!string.IsNullOrEmpty(value))
                {
                    if (value is String && string.IsNullOrEmpty(value.ToString()))
                    {
                        propertyInfo.SetValue(entity, null, null);
                    }
                    else
                    {
                        try
                        {
                            Type targetType;
                            if (propertyInfo.PropertyType.Name.StartsWith("Nullable"))
                            {
                                targetType = propertyInfo.PropertyType.GetGenericArguments()[0];
                            }
                            else
                            {
                                targetType = propertyInfo.PropertyType;
                            }
                            propertyInfo.SetValue(entity, Convert.ChangeType(value, targetType), null);
                        }
                        catch { }
                    }
                }

            }           
        }

        /// <summary>
        /// 把对象属性生成 key=value|key=value 这种字符串
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string GetStringByProperty(object entity)
        {
            StringBuilder sb = new StringBuilder();
            Type classType = entity.GetType();
            PropertyInfo[] PropertyInfos = classType.GetProperties();
            foreach (PropertyInfo PropertyInfo in PropertyInfos)
            { 
                string name = PropertyInfo.Name;
                object value = PropertyInfo.GetValue(entity, null);
                sb.Append(name);
                sb.Append("=");
                sb.Append(Convert.ChangeType(value, typeof(String)));
                sb.Append("|");
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
    }
}
