using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Explorer.Framework.Net.Protocol
{
    /// <summary>
    /// 协议解析器
    /// </summary>
    public interface IParser
    {
        /// <summary>
        /// 解析方法, 解析本层协议内容, 返回下层协议数据bytes
        /// </summary>
        /// <param name="dataContent"></param>
        /// <returns></returns>
        byte[] Parse(byte[] dataContent);
    }
}
