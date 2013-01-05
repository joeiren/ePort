using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Explorer.Framework.Net.Protocol
{
    /// <summary>
    /// 协议帧定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ProtocolFrame : Attribute
    {
        /// <summary>
        /// 对应的名称
        /// </summary>
        public string Name
        { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        { get; set; }

        /// <summary>
        /// 对应的长度
        /// </summary>
        public string Length
        { get; set; }
    }
}
