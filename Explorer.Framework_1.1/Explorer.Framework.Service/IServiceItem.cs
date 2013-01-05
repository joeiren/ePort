using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Explorer.Framework.Service
{
    /// <summary>
    /// 服务项接口
    /// </summary>
    public interface IServiceItem
    {
        /// <summary>
        /// 服务空间, 用法和命名空间一样
        /// </summary>
        string ServiceSpace { get; set; }

        /// <summary>
        /// 服务名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 是否是自动服务
        /// </summary>
        bool IsAutoService { get; set; }

        /// <summary>
        /// 服务状态
        /// </summary>
        ServiceState State { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        void Initialize(Dictionary<string, string> param);

        /// <summary>
        /// 开始服务
        /// </summary>
        void Start();

        /// <summary>
        /// 暂停服务
        /// </summary>
        void Pause();

        /// <summary>
        /// 停止服务
        /// </summary>
        void Stop();
    }

    /// <summary>
    /// 服务状态
    /// </summary>
    public enum ServiceState 
    { 
        /// <summary>
        /// 已经安装
        /// </summary>
        Install,
        /// <summary>
        /// 错误
        /// </summary>
        Error,
        /// <summary>
        /// 准备就绪
        /// </summary>
        Ready,
        /// <summary>
        /// 运行中
        /// </summary>
        Run,
        /// <summary>
        /// 暂停
        /// </summary>
        Pause,
        /// <summary>
        /// 停止
        /// </summary>
        Stop        
    }
}
