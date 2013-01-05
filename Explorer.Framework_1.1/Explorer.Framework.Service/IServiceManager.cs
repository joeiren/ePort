using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Explorer.Framework.Service
{
    /// <summary>
    /// 
    /// </summary>
    public interface IServiceManager
    {
        /// <summary>
        /// 启动一个指定服务
        /// </summary>
        /// <param name="item">服务项</param>
        void StartService(IServiceItem item);

        /// <summary>
        /// 启动一个指定服务
        /// </summary>
        /// <param name="nameSpace">命名空间</param>
        /// <param name="name">名称</param>
        void StartService(string nameSpace, string name);

        /// <summary>
        /// 停止一个指定服务
        /// </summary>
        /// <param name="item">服务项</param>
        void StopService(IServiceItem item);

        /// <summary>
        /// 停止一个指定服务
        /// </summary>
        /// <param name="nameSpace">命名空间</param>
        /// <param name="name">名称</param>
        void StopService(string nameSpace, string name);

        /// <summary>
        /// 启动所有服务
        /// </summary>
        /// <returns></returns>
        void StartServiceAll();

        /// <summary>
        /// 停止所有服务
        /// </summary>
        /// <returns></returns>
        void StopServiceAll();
    }
}
