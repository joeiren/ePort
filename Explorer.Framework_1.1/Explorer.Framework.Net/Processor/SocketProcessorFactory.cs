using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Explorer.Framework.Core.Base;

namespace Explorer.Framework.Net.Processor
{
    /// <summary>
    /// Socket 数据处理器, 用于反射处理数据协议
    /// </summary>
    public class SocketProcessorFactory : BaseFactory<SocketProcessorFactory, ISocketProcessor>
    {
        private Dictionary<long, Type> _processorMap;

        public override void Initialize()
        {
            this._processorMap = new Dictionary<long, Type>();
            base.Initialize();
        }

        /// <summary>
        /// 添加一个处理器
        /// </summary>
        /// <param name="key">处理器关键字</param>
        /// <param name="processorClassType">处理器类型</param>
        public void Add(byte[] key, Type processorClassType)
        {
            long _key = SocketProcessorFactory.BuildKey(key);
            Add(_key, processorClassType);
        }

        /// <summary>
        /// 添加一个处理器
        /// </summary>
        /// <param name="key">处理器关键字</param>
        /// <param name="processorClassType">处理器类型</param>
        public void Add(long key, Type processorClassType)
        {            
            if (!this._processorMap.ContainsKey(key))
            {
                this._processorMap.Add(key, processorClassType);
            }
        }

        /// <summary>
        /// 删除一个处理器
        /// </summary>
        /// <param name="key">处理器关键字</param>
        /// <returns></returns>
        public bool Remove(byte[] key)
        {
            long _key = SocketProcessorFactory.BuildKey(key);
            return Remove(_key);
        }

        /// <summary>
        /// 删除一个处理器
        /// </summary>
        /// <param name="key">处理器关键字</param>
        /// <returns></returns>
        public bool Remove(long key)
        {
            if (this._processorMap.ContainsKey(key))
            {
                return this._processorMap.Remove(key);
            }
            return true;
        }

        /// <summary>
        /// 检查一下处理器是否存在
        /// </summary>
        /// <param name="key">处理器关键字</param>
        /// <returns></returns>
        public bool Contains(byte[] key)
        {
            long _key = BuildKey(key);
            return Contains(_key);
        }

        /// <summary>
        /// 检查一下处理器是否存在
        /// </summary>
        /// <param name="key">处理器关键字</param>
        /// <returns></returns>
        public bool Contains(long key)
        {
            return this._processorMap.ContainsKey(key);
        }

        /// <summary>
        /// 获取一个处理器类型
        /// </summary>
        /// <param name="key">处理器关键字</param>
        /// <returns></returns>
        public Type Get(byte[] key)
        { 
            long _key = BuildKey(key);
            return Get(_key);
        }

        /// <summary>
        /// 获取一个处理器类型
        /// </summary>
        /// <param name="key">处理器关键字</param>
        /// <returns></returns>
        public Type Get(long key)
        {
            if (this._processorMap.ContainsKey(key))
            {
                return this._processorMap[key];
            }
            return null;
        }

        /// <summary>
        /// 创建一个处理器
        /// </summary>
        /// <param name="key">处理器关键字</param>
        /// <returns></returns>
        public ISocketProcessor CreateProcessor(byte[] key)
        {
            long _key = BuildKey(key);
            return CreateProcessor(_key);
        }

        /// <summary>
        /// 创建一个处理器
        /// </summary>
        /// <param name="key">处理器关键字</param>
        /// <returns></returns>
        public ISocketProcessor CreateProcessor(long key)
        {
            Type classType = Get(key);
            return classType.Assembly.CreateInstance(classType.FullName) as ISocketProcessor;
        }

        /// <summary>
        /// 创建一个处理器
        /// </summary>
        /// <param name="KeyIndex">处理器关键字索引</param>
        /// <param name="frameData">数据帧</param>
        /// <returns></returns>
        public ISocketProcessor CreateProcessor(int[] KeyIndex, byte[] frameData)
        {
            byte[] b_ProtocolKey = new byte[KeyIndex.Length];
            for (int i = 0; i < KeyIndex.Length; i++)
            {
                b_ProtocolKey[i] = frameData[KeyIndex[i]];
            }
            long l_ProtocolKey = BuildKey(b_ProtocolKey);

            return CreateProcessor(l_ProtocolKey);
        }

        /// <summary>
        /// 生成一个协议 Key
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static long BuildKey(byte[] values)
        {
            long _key = 0L;
            for (int i = 0; i < values.Length; i++)
            {
                _key |= (long)(values[i] & 0xFF) << (i * 8);
            }
            return _key;
        }
    }
}
