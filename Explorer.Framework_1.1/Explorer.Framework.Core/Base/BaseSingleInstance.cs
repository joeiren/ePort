using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Explorer.Framework.Core.Initialize;

namespace Explorer.Framework.Core.Base
{
    /// <summary>
    /// 基本 Singletion 模式实列基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseSingletonInstance<T> : MarshalByRefObject, IInitialize, IDisposable where T : BaseSingletonInstance<T>, new()
    {        
        private static T _instance = null;
        private static readonly object _locker = new object();

        /// <summary>
        /// 是否已经初始化
        /// </summary>
        public bool IsInitialize { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Initialize() 
        {
            this.IsInitialize = true;
        }
        
        /// <summary>
        /// 返回当前实列
        /// </summary>
        public static T Instance
        {
            get
            {
                lock (_locker)
                {
                    if (_instance == null)
                    {
                        _instance = new T();                        
                    }
                    if (_instance.IsInitialize != true)
                    {
                        _instance.Initialize();
                    }
                    return _instance;
                }                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Dispose()
        {
            BaseSingletonInstance<T>._instance = null;
            System.GC.Collect();
        }
    }
}
