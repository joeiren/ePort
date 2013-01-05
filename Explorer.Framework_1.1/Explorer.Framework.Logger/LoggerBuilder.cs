using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Explorer.Framework.Core;
using Explorer.Framework.Core.Base;

namespace Explorer.Framework.Logger
{
    internal class LoggerBuilder : BaseBuilder<LoggerBuilder, ILogger>
    {

        public override ILogger CreateInstance()
        {
            return CreateInstance(null);
        }

        public override ILogger CreateInstance(string name)
        {
            ILogger logger = null;

            // 如果 name 为空则是取默认的
            if (string.IsNullOrEmpty(name))
            {
                logger = new TextLogger("Logger", null);
            }
            else
            {
                DataSet dataSet = AppContext.Instance.GetAppContextSet();
                if (dataSet != null)
                {
                    DataTable tableLoggerFactory = dataSet.Tables["Logger"];
                    if (tableLoggerFactory != null)
                    {
                        DataRow[] rows = tableLoggerFactory.Select("Name='" + name + "'");
                        if (rows.Length > 0)
                        {
                            switch (rows[0]["Type"].ToString().ToUpper())
                            {
                                case "TEXT":
                                    logger = new TextLogger(name, rows[0]);
                                    break;
                                case "TXT":
                                    logger = new TextLogger(name, rows[0]);
                                    break;
                                case "XML":
                                    //logger = new XmlLogger(name, rows[0]);
                                    string path = this.GetType().Module.FullyQualifiedName;
                                    path = path.Substring(0, path.LastIndexOf("\\") + 1) + "Explorer.Framework.Logger.Xml.dll";
                                    //E:\Svn项目\Explorer.Framework_1.1\Explorer.Framework.Test\bin\Debug\Explorer.Framework.Logger.dll
                                    System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFile(path);
                                    Type tp = assembly.GetType("Explorer.Framework.Logger.XmlLogger");
                                    logger = (ILogger)Activator.CreateInstance(tp, new object[] { name, rows[0] });
                                    break;
                                case "DB":
                                    logger = new DbLogger(name, rows[0]);
                                    break;
                                case "DATABASE":
                                    logger = new DbLogger(name, rows[0]);
                                    break;
                                case "PROXY":
                                    logger = new ProxyLogger(name, rows[0]);
                                    break;
                            }
                        }
                        else
                        {
                            logger = new TextLogger(name, null);
                        }
                    }
                    else
                    {
                        logger = new TextLogger(name, null);
                    }
                }
                else
                {
                    logger = new TextLogger(name, null);
                }
            }
            return logger;
        }
    }
}
