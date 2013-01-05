using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Security;

namespace Explorer.Framework.Net
{
    /// <summary>
    /// 传输编码转换器
    /// </summary>
    public class BaseEncoding
    {
        /// <summary>
        /// 7转8
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ToBase728(byte[] data)
        {            
            MemoryStream ms = new MemoryStream();

            for (int i = 0; i < data.Length - 1; i++)
            {
                int j = i % 8;
                int offset = j + 1;
                if (offset == 8)
                {
                    continue; // 算出了这个字节要偏移8位就不要了, 因为并到上个字节里了
                }
                byte value0 = (byte)(data[i] << offset); // 高位
                byte value1 = (byte)(data[i + 1] << 1);  // 低位去掉头
                byte value2 = (byte)(value1 >> (8 - offset)); // 低位偏移
                byte value = (byte)(value0 ^ value2);
                ms.WriteByte(value);
            }
            return ms.ToArray();
        }

        /// <summary>
        /// 8转7
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ToBase827(byte[] data)
        {
            MemoryStream ms = new MemoryStream();
            for (int i = 0; i < data.Length; i++)
            {
                int offset1 = i % 7;
                int offset0 = 8 - offset1;
                if (offset0 == 8)
                {
                    ms.WriteByte((byte)(data[i] >> 1));
                }
                else
                {
                    byte value0 = (byte)(data[i - 1] << offset0);
                    byte value1 = (byte)(data[i] >> offset1);
                    byte value2 = (byte)(value0 ^ value1);
                    byte value = (byte)(value2 >> 1);
                    ms.WriteByte(value);
                    if (offset1 == 6)
                    {
                        ms.WriteByte((byte)((byte)(data[i] << 1) >> 1));
                    }
                }
            }
            return ms.ToArray();
        }
    }
}
