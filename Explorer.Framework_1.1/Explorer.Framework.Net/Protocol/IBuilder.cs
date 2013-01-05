using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Explorer.Framework.Net.Protocol
{
    /// <summary>
    /// 协议构建器
    /// </summary>
    public interface IBuilder
    {
        /// <summary>
        /// 构建本层协议
        /// </summary>
        /// <returns>返回本层构建后的数据bytes</returns>
        byte[] Build();

        /// <summary>
        /// 构建本层协议, 并把下层协议传入的数据也构建在内
        /// </summary>
        /// <param name="dataContent">下层协议的数据Bytes</param>
        /// <returns>返回本层构建后的数据bytes</returns>
        byte[] Build(byte[] dataContent);
    }
}
