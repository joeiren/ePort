using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Explorer.Framework.Core.Initialize
{
    /// <summary>
    /// 初始化接口
    /// </summary>
    public interface IInitialize
    {
        /// <summary>
        /// 是否已经初始化
        /// </summary>
        bool IsInitialize { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        void Initialize();
    }

    /// <summary>
    /// 文件模式初始化接口
    /// </summary>
    public interface IFileInitialize : IInitialize
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="filename">文件路径名称</param>
        void Initialize(string filename);
    }

    /// <summary>
    /// 远程资源模式初始化接口
    /// </summary>
    public interface IUriInitialize : IInitialize
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="uri">文件uri地址</param>
        void Initialize(Uri uri);
    }
}
