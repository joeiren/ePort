using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Explorer.Framework.Data.ORM
{
    /// <summary>
    /// /
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ORMTableAttribute : Attribute
    {
        /// <summary>
        /// 对应的数据表名称
        /// </summary>
        public string Name 
        { get; set; }


        /// <summary>
        /// 该数据表是否只读
        /// </summary>
        public bool ReadOnly 
        { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public ORMTableAttribute() 
        { }
    }
}
