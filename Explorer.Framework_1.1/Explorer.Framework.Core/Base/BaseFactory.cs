using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Explorer.Framework.Core.Base
{
    /// <summary>
    /// 基本工厂, 带缓冲
    /// </summary>
    /// <typeparam name="T">工厂单实例类型</typeparam>
    /// <typeparam name="M">工厂提供的类型</typeparam>
    public abstract class BaseFactory<T, M> : BaseSingletonInstance<T> where T : BaseFactory<T, M>, new()
    {
        /// <summary>
        /// 释放
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }

    }
    
}
