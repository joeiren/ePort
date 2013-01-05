using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;

using Explorer.Framework.Core.ExtendAttribute;

namespace Explorer.Framework.Common.Helper
{
    public class EnumHelper
    {
        /// <summary>
        /// 获取指定枚举类型下的所有枚举值
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static List<T> GetEnumValueList<T>(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                return null;
            }
            List<T> list = new List<T>();
            
            FieldInfo[] fields = enumType.GetFields();
            foreach (FieldInfo field in fields)
            {
                if (!field.IsSpecialName)
                {
                    T enumValue = (T)Enum.Parse(enumType, field.Name);
                    list.Add(enumValue);
                }
            }
            return list;
        }

        /// <summary>
        /// 获取指定枚举类型下的所有枚举名称(RemarkAttribute.Name)
        /// 当前枚举类型必须冠以 RemarkAttribute 特性
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static List<string> GetEnumNameList(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                return null;
            }
            List<string> list = new List<string>();
            FieldInfo[] fields = enumType.GetFields();
            foreach (FieldInfo field in fields)
            {
                if (!field.IsSpecialName)
                {
                    string enumName = GetEnumNameByValue((Enum)Enum.Parse(enumType, field.GetRawConstantValue().ToString().Trim()));
                    list.Add(enumName);
                }
            }
            return list;
        }

        /// <summary>
        /// 获取指定枚举类型下的所有枚举描述(RemarkAttribute.Description)
        /// 当前枚举类型必须冠以 RemarkAttribute 特性
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static List<string> GetEnumDescriptionList(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                return null;
            }
            List<string> list = new List<string>();
            FieldInfo[] fields = enumType.GetFields();
            foreach (FieldInfo field in fields)
            {
                if (!field.IsSpecialName)
                {
                    string enumName = GetEnumDescriptionByValue((Enum)Enum.Parse(enumType, field.GetRawConstantValue().ToString().Trim()));
                    list.Add(enumName);
                }
            }
            return list;
        }

        /// <summary>
        /// 获取指定枚举值的 RemarkAttribute 对象
        /// </summary>
        /// <param name="enumClass"></param>
        /// <returns></returns>
        public static RemarkAttribute GetEnumValueRemark(Enum enumValue)
        {
            return (RemarkAttribute)Attribute.GetCustomAttribute(enumValue.GetType().GetField(enumValue.ToString()), typeof(RemarkAttribute));
        }

        /// <summary>
        /// 获取指定枚举值的 RemarkAttribute.Name 
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetEnumNameByValue(Enum enumValue)
        {
            RemarkAttribute remarkAttribute = GetEnumValueRemark(enumValue);
            if (remarkAttribute != null)
            {
                return remarkAttribute.Name;
            }
            return "";
        }

        /// <summary>
        /// 获取指定枚举值的 RemarkAttribute.Description 
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetEnumDescriptionByValue(Enum enumValue)
        {
            RemarkAttribute remarkAttribute = GetEnumValueRemark(enumValue);
            if (remarkAttribute != null)
            {
                return remarkAttribute.Description;
            }
            return "";
        }

        /// <summary>
        /// 获取指定枚举值的 RemarkAttribute.Name 
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetEnumNameByValue(Type enumType, string enumValue)
        {
            if (!enumType.IsEnum)
            {
                return null;
            }
            FieldInfo[] fields = enumType.GetFields();
            foreach (FieldInfo field in fields)
            {
                if (!field.IsSpecialName && (field.GetRawConstantValue().ToString().Trim() == enumValue.Trim()))
                {
                    return GetEnumNameByValue((Enum)Enum.Parse(enumType, field.Name));
                }
            }
            return "";
        }

        /// <summary>
        /// 获取指定枚举值的 RemarkAttribute.Description 
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetEnumDescriptionByValue(Type enumType, string enumValue)
        {
            if (!enumType.IsEnum)
            {
                return null;
            }
            FieldInfo[] fields = enumType.GetFields();
            foreach (FieldInfo field in fields)
            {
                if (!field.IsSpecialName && (field.GetRawConstantValue().ToString().Trim() == enumValue.Trim()))
                {
                    return GetEnumDescriptionByValue((Enum)Enum.Parse(enumType, field.Name));
                }
            }
            return "";
        }
    }
}
