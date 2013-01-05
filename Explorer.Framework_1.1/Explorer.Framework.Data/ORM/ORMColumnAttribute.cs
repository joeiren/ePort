using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Explorer.Framework.Data.ORM
{
    /// <summary>
    /// ORM Column 特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class ORMColumnAttribute : Attribute
    {
        /// <summary>
        /// 是否为自动填充, 由数据库自动填充的
        /// </summary>
        public bool IsAutoFill
        { get; set; }

        /// <summary>
        /// 是否为标识列
        /// </summary>
        public bool IsIdentity
        { get; set; }

        /// <summary>
        /// 是否为主键
        /// </summary>
        public bool IsPrimaryKey
        { get; set; }

        /// <summary>
        /// 字段名称
        /// </summary>
        public string Name
        { get; set; }

        /// <summary>
        /// 字段描述
        /// </summary>
        public string Description
        { get; set; }


        /// <summary>
        /// 字段对应的 DbType
        /// </summary>
        public DbType DbType
        { get; set; }

        /// <summary>
        /// 字段的长度
        /// </summary>
        public int Size
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ORMColumnAttribute()
        {             
        }
    }
}
