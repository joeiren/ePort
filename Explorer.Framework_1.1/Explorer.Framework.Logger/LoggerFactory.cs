using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;

using Explorer.Framework.Core;
using Explorer.Framework.Core.Base;

namespace Explorer.Framework.Logger
{
    public class LoggerFactory : BaseFactory<LoggerFactory, ILogger>
    {        
        /// <summary>
        /// 
        /// </summary>
        public string WebRoot { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DefaultName { get; set; }

        public override void Initialize()
        {
            // 如果已经初始化过后, 还手工调用初始化, 则视为重初始化
            if (this.IsInitialize)
            {
                // 重初始化系统环境配置
                AppContext.Instance.Initialize();
                
                // 重建缓冲区的数据

            }
            this.DefaultName = "Logger";
            DataSet dataSet = AppContext.Instance.GetAppContextSet();
            if (dataSet != null)
            {
                DataTable table = dataSet.Tables["LoggerContext"];
                if (table != null)
                {
                    if (table.Rows[0]["Default"] != null)
                    {
                        this.DefaultName = table.Rows[0]["Default"].ToString();
                    }
                }
            }
            base.Initialize();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classType"></param>
        /// <returns></returns>
        public ILogger GetLogger(Type classType)
        {
            return GetLogger(DefaultName, classType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classType"></param>
        /// <param name="rebuilder"></param>
        /// <returns></returns>
        public ILogger GetLogger(Type classType, bool reBuilder)
        {
            return GetLogger(DefaultName, classType, null, reBuilder);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="classType"></param>
        /// <returns></returns>
        public ILogger GetLogger(string name, Type classType)
        {
            return GetLogger(name, classType, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="classType"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public ILogger GetLogger(string name, Type classType, string position)
        {
            return GetLogger(name, classType, position, false);
        }

        public ILogger GetLogger(string name, Type classType, string position, bool reBuilder)
        {
            ILogger logger = null;
            string fullName = name + "@" + classType.FullName;
            lock (LoggerManager.Instance.Items)
            {
                if (name != null && LoggerManager.Instance.Items.ContainsKey(fullName) && !reBuilder)
                {
                    logger = LoggerManager.Instance.Items[fullName];
                }
                else
                {
                    if (LoggerManager.Instance.Items.ContainsKey(fullName))
                    {
                        LoggerManager.Instance.Items.Remove(fullName);
                    }
                    logger = LoggerBuilder.Instance.CreateInstance(name);
                    LoggerManager.Instance.Items.Add(fullName, logger);
                }
                if (!string.IsNullOrEmpty(position))
                {
                    logger.Position = position;
                }
                logger.ClassType = classType;
            }
            return logger;
        }
    }
}
