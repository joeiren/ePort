using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.Threading;

namespace Explorer.Framework.Logger
{
    /// <summary>
    /// 日志类的基类
    /// </summary>
    public abstract class BaseLogger : ILogger, IDisposable
    {
        #region ILogger 成员
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 输出位置
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// 输出模板
        /// </summary>
        public string Template { get; set; }

        /// <summary>
        /// 日志记录的级别 
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// 是否输出控制台
        /// </summary>
        public bool IsOutputConsole { get; set; }

        /// <summary>
        /// 指定被记录类的 Type
        /// </summary>
        public Type ClassType { get; set; }

        

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name"></param>
        public BaseLogger(string name, DataRow rowLogger)
        {
            this.IsOutputConsole = false;
            this.Name = name;
            if (rowLogger != null)
            {
                if (rowLogger.Table.Columns.Contains("OutputConsole"))
                {
                    this.IsOutputConsole = bool.Parse(rowLogger["OutputConsole"].ToString());
                }

                if (rowLogger.Table.Columns.Contains("Type"))
                {
                    this.Type = rowLogger["Type"].ToString();
                }
                else
                {
                    this.Type = "Text";
                }

                DataRow[] rows = rowLogger.GetChildRows("Logger_Property");
                foreach (DataRow rowProperty in rows)
                {
                    PropertyInfo propertyInfo = this.GetType().GetProperty(rowProperty["Name"].ToString());
                    if (propertyInfo != null)
                    {
                        object value ;                       
                        if (rowProperty["Name"].ToString().ToLower().Equals("databasetype"))
                        {
                            value = Enum.Parse(propertyInfo.PropertyType, rowProperty["Property_Text"].ToString());
                        }
                        else
                        {
                            value = Convert.ChangeType(rowProperty["Property_Text"], propertyInfo.PropertyType);
                            
                        }
                        propertyInfo.SetValue(this, value, null);                    
                    }
                }
            }
        }

        /// <summary>
        /// 是否记录 Debug 级
        /// </summary>
        public bool IsDebugEnabled
        {
            get
            {
                if (this.Level.ToUpper().IndexOf("DEBUG") > -1)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 是否记录 Info 级
        /// </summary>
        public bool IsInfoEnabled
        {
            get
            {
                if (this.Level.ToUpper().IndexOf("INFO") > -1)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 是否记录 Warn 级
        /// </summary>
        public bool IsWarnEnabled
        {
            get
            {
                if (this.Level.ToUpper().IndexOf("WARN") > -1)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 是否记录 Error 级
        /// </summary>
        public bool IsErroEnabled
        {
            get
            {
                if (this.Level.ToUpper().IndexOf("ERROR") > -1)
                {
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Debug(string message)
        {
            LogWrite("DEBUG", message, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Debug(string message, Exception exception)
        {
            LogWrite("DEBUG", message, exception);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Info(string message)
        {
            LogWrite("INFO", message, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Info(string message, Exception exception)
        {
            LogWrite("INFO", message, exception);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Warn(string message)
        {
            LogWrite("WARN", message, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Warn(string message, Exception exception)
        {
            LogWrite("WARN", message, exception);
        }
        /// <summary>
        /// /
        /// </summary>
        /// <param name="message"></param>
        public void Error(string message)
        {
            LogWrite("ERROR", message, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Error(string message, Exception exception)
        {
            LogWrite("ERROR", message, exception);
        }

        #endregion

        /// <summary>
        /// 将日志实际输出
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public abstract void LogWrite(string level, string message, Exception exception);

        /// <summary>
        /// 将日志写入具体载体
        /// </summary>
        /// <param name="message"></param>
        protected abstract void LogOutput(object input);

        /// <summary>
        /// 常用变量替换
        /// </summary>
        /// <param name="inputText"></param>
        /// <returns></returns>
        protected string ConvertVar(string inputText)
        {
            string outputText = inputText;
            outputText = outputText.Replace("${Guid}", System.Guid.NewGuid().ToString());
            outputText = outputText.Replace("${Name}", this.Name);
            outputText = outputText.Replace("${NewLine}", "\r\n");
            outputText = outputText.Replace("${Path}", AppDomain.CurrentDomain.BaseDirectory);
            outputText = outputText.Replace("${WebRoot}", LoggerFactory.Instance.WebRoot);
            outputText = outputText.Replace("${DateTime}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            outputText = outputText.Replace("${Date}", DateTime.Now.ToString("yyyy-MM-dd"));
            outputText = outputText.Replace("${Time}", DateTime.Now.ToString("HH:mm:ss"));
            outputText = outputText.Replace("${ClassName}", this.ClassType.FullName);
            outputText = outputText.Replace("${TypeName}", this.ClassType.FullName);
            outputText = outputText.Replace("${ThreadName}", Thread.CurrentThread.Name);
            return outputText;
        }

        public virtual void Dispose()
        {            
        }
    }
}
