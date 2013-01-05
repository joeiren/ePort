using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace Explorer.Framework.Logger
{
    public interface ILogger
    { 
        /// <summary>
        /// 
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// 日志记录的级别 
        /// DEBUG,INFO,WARN,ERROR
        /// </summary>
        string Level { get; set; }
        /// <summary>
        /// 输出位置
        /// </summary>
        string Position { get; set; }
        /// <summary>
        /// 输出模板
        /// </summary>
        string Template { get; set; }
        /// <summary>
        /// 是否记录 Debug 级
        /// </summary>
        bool IsDebugEnabled { get; }
        /// <summary>
        /// 是否记录 Info 级
        /// </summary>
        bool IsInfoEnabled { get; }
        /// <summary>
        /// 是否记录 Warn 级
        /// </summary>
        bool IsWarnEnabled { get; }
        /// <summary>
        /// 是否记录 Error 级
        /// </summary>
        bool IsErroEnabled { get; }
        /// <summary>
        /// 是否输出控制台
        /// </summary>
        bool IsOutputConsole { get; set; }
        /// <summary>
        /// 指定被记录类的 Type
        /// </summary>
        Type ClassType { get; set; }


        void Debug(string message);
        void Debug(string message, Exception exception);
        void Info(string message);
        void Info(string message, Exception exception);
        void Warn(string message);
        void Warn(string message, Exception exception);
        void Error(string message);
        void Error(string message, Exception exception);
        void LogWrite(string level, string message, Exception exception);
    }

  
}
