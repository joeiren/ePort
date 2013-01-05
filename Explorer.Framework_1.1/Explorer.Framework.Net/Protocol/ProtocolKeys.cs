using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Explorer.Framework.Core.ExtendAttribute;

namespace Explorer.Framework.Net.Protocol
{
    /// <summary>
    /// 协议关键字
    /// </summary>
    public abstract class ProtocolKeys
    {
        /// <summary>
        /// 通过协议 Key 的 Remark 获取当前协议的名称, 如果 Remark 不存在就返回 "";
        /// </summary>
        /// <param name="protocolKey"></param>
        /// <returns></returns>
        public static string GetProtocolName(Type classType, byte[] protocolKey)
        {
            string name = "";
            FieldInfo[] fieldInfos = classType.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (FieldInfo fieldInfo in fieldInfos)
            {

                if (fieldInfo.FieldType.ToString().Equals("System.Byte[]"))
                {
                    byte[] value = (byte[])fieldInfo.GetValue(null);
                    if (ProtocolKeys.BuildKey(value) == ProtocolKeys.BuildKey(protocolKey))
                    {
                        RemarkAttribute[] remarks = (RemarkAttribute[])fieldInfo.GetCustomAttributes(typeof(RemarkAttribute), true);
                        if (remarks != null && remarks.Length > 0)
                        {
                            name = remarks[0].Name;
                        }
                    }
                }
            }
            return name;
        }

        /// <summary>
        /// 生成一个协议 Key
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static long BuildKey(byte[] values)
        {
            long _key = 0L;
            for (int i = 0; i < values.Length; i++)
            {
                _key |= (long)(values[i] & 0xFF) << (i * 8);
            }
            return _key;
        }

        /// <summary>
        /// 生成一个协议 Key
        /// </summary>
        /// <param name="values"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static long BuildKey(byte[] values, int index, int length)
        {
            long _key = 0L;
            for (int i = index; i < length; i++)
            {
                _key |= (long)(values[i] & 0xFF) << (i * 8);
            }
            return _key;
        }

        /// <summary>
        /// 协议一个协议 Key
        /// </summary>
        /// <param name="values"></param>
        /// <param name="data"></param>
        public static void ParseKey(long values, ref byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(values >> (i * 8));
            }
        }
    }
}
