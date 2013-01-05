using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Explorer.Framework.Net.Protocol
{
    /// <summary>
    /// 协议字段特性, 描述协议字段作用, 在协议帧里的起点位置, 长度等信息的
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class ProtocolField : Attribute
    {
        /// <summary>
        /// 对应的名称
        /// </summary>
        public string Name
        { get; set; }

        /// <summary>
        /// 对应的起点
        /// </summary>
        public string Index
        { get; set; }

        /// <summary>
        /// 对应的长度
        /// </summary>
        public string Length
        { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue
        { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        { get; set; }

        /// <summary>
        /// 属性
        /// </summary>
        public Type PropertyType
        { get; set; }

        public ProtocolField()
        { }
    }

}
