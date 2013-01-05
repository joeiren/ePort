using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Explorer.Framework.Net
{
    /// <summary>
    /// 16进制转换器
    /// </summary>
    public class HexConverter
    {
        /// <summary>
        /// 把 bytes 转成 2进制文本
        /// </summary>
        /// <param name="content">bytes</param>
        /// <returns>2进制文本</returns>
        public static string ToBitString(byte[] content)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte value in content)
            {
                string text = Convert.ToString(value, 2);
                text = text.PadLeft(8, '0');
                sb.Append(text);
                sb.Append(" ");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 把 bytes 转成 16进制文本, 并且按照 0x00 格式化
        /// </summary>
        /// <param name="content">bytes</param>
        /// <returns>16进制文本</returns>
        public static string ToHexString(byte[] content)
        {
            return ToHexString(content, "0x00");
        }
        /// <summary>
        /// 把 bytes 转成 16进制文本, 并且按照 0x00 或是 00 格式化
        /// </summary>
        /// <param name="content">0x00或00格式</param>
        /// <returns>16进制文本</returns>
        public static string ToHexString(byte[] content, string format)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < content.LongLength; i++)
            {
                string text = System.Convert.ToString(content[i], 16);
                if (format.Equals("0x00"))
                {
                    if (text.Length == 1)
                    {
                        text = "0x0" + text;
                    }
                    else
                    {
                        text = "0x" + text;
                    }
                }
                else
                {
                    if (text.Length == 1)
                    {
                        text = "0" + text;
                    }
                }
                sb.Append(text);
                if (i < content.LongLength - 1)
                {
                    sb.Append(" ");
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 把 0x00 或是 00 这样的 16进制文本, 转回成 bytes 
        /// </summary>
        /// <param name="source">16进是文本</param>
        /// <returns>bytes</returns>
        public static byte[] ReadHexString(string source)
        {
            source = source.Replace(" ", ",");
            byte[] outputData;
            string[] values = source.Split(',');
            outputData = new byte[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                outputData[i] = System.Convert.ToByte(values[i], 16);
            }
            return outputData;
        }

        /// <summary>
        /// 将$??{?,?}里的变量值取出, 并分析转换为 bytes, 支持 $HEX{?}, $BIN{?}, $INT{?}, $STR{?} 这几种变量类型
        /// </summary>
        /// <param name="source">要转换的变量</param>
        /// <returns>返回Bytes</returns>
        public static byte[] ReadVariable(string source)
        {
            source = source.Replace(" ", ",");
            byte[] outputData;
            string temp;
            if (source.StartsWith("$HEX{"))
            {
                temp = source.Substring(5, source.Length - 6);
                string[] values = temp.Split(',');
                outputData = new byte[values.Length];
                for (int i = 0; i < values.Length; i++)
                {
                    outputData[i] = System.Convert.ToByte(values[i], 16);
                }
            }
            else if (source.StartsWith("$BIN{"))
            {
                temp = source.Substring(5, source.Length - 6);
                string[] values = temp.Split(',');
                outputData = new byte[values.Length];
                for (int i = 0; i < values.Length; i++)
                {
                    outputData[i] = System.Convert.ToByte(values[i], 2);
                }
            }
            else if (source.StartsWith("$INT{"))
            {
                temp = source.Substring(5, source.Length - 6);
                string[] values = temp.Split(',');
                outputData = new byte[values.Length];
                for (int i = 0; i < values.Length; i++)
                {
                    outputData[i] = (byte)System.Convert.ToInt32(values[i]);
                }
            }
            else if (source.StartsWith("$STR{"))
            {
                temp = source.Substring(5, source.Length - 6);
                outputData = System.Text.Encoding.Default.GetBytes(temp);
            }
            else
            {
                outputData = System.Text.Encoding.Default.GetBytes(source);
            }
            return outputData;
        }

        /// <summary>
        /// 将$??{?,?}里的变量值取出, 并分析转换为 byte[], 支持 $HEX{?}, $BIN{?}, $INT{?}, $STR{?} 这几种变量类型
        /// </summary>
        /// <param name="source">要转换的变量</param>
        /// <param name="index">返回 index 位置的数据</param>
        /// <returns>返回byte[index]</returns>
        public static byte ReadVariable(string source, int index)
        {
            return ReadVariable(source)[index];
        }
    }
}
