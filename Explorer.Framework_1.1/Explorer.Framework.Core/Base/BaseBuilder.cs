using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Explorer.Framework.Core.Base
{
    /// <summary>
    /// 基本构建器
    /// </summary>
    /// <typeparam name="T">构建器单实例类型</typeparam>
    /// <typeparam name="M">被构建的对象类型</typeparam>
    public abstract class BaseBuilder<T, M> : BaseSingletonInstance<T> where T : BaseBuilder<T, M> ,new()
    {

        /// <summary>
        /// 创建默认的被构建对象
        /// </summary>
        /// <returns></returns>
        public virtual M CreateInstance()
        {
            return CreateInstance("");
        }


        /// <summary>
        /// 创建指定的被构建对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public abstract M CreateInstance(string name);

    }
}
