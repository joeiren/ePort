using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;

namespace Explorer.Framework.Logger
{
    /// <summary>
    /// 
    /// </summary>
    public class ProxyLogger : BaseLogger
    {
        #region ProxyLogger 成员
        /// <summary>
        /// 
        /// </summary>
        public string Proxy { get; set; }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="row"></param>
        public ProxyLogger(string name, DataRow row)
            : base(name, row)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public override void LogWrite(string level, string message, Exception exception)
        {
            string[] proxys = this.Proxy.Split(',');
            foreach (string proxy in proxys)
            {
                ILogger Logger = LoggerFactory.Instance.GetLogger(proxy, this.ClassType);
                Logger.LogWrite(level, message, exception);                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        protected override void LogOutput(object input)
        {
        }
    }
}
