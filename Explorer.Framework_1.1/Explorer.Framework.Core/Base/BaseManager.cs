using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Explorer.Framework.Core.Base
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="M"></typeparam>
    public abstract class BaseManager<T, M> : BaseSingletonInstance<T> where T : BaseManager<T, M>, new()
    {
        private Dictionary<string, M> _items = new Dictionary<string, M>();

        /// <summary>
        /// 管理对象池
        /// </summary>
        public Dictionary<string, M> Items
        {
            get { return _items; }
        }

        /// <summary>
        /// 释放
        /// </summary>
        public override void Dispose()
        {
            // 释放当前管理的所有对象
            if (typeof(M) is IDisposable)
            {
                lock (this._items)
                {
                    foreach (IDisposable item in this._items.Values)
                    {
                        item.Dispose();
                    }
                }
            }
            this._items.Clear();
            base.Dispose();
        }
    }
}
