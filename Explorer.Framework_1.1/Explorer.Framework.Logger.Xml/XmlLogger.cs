using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.Xml;
using System.IO;

namespace Explorer.Framework.Logger
{
    /// <summary>
    /// 将日志存入XML
    /// </summary>
    public class XmlLogger : BaseLogger
    {
        private string _realLogFilename = String.Empty;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="rowLogger"></param>
        public XmlLogger(string name, DataRow rowLogger)
            : base(name, rowLogger)
        {
            if (rowLogger != null)
            {
                DataRow[] rows = rowLogger.GetChildRows("Logger_Property");
                foreach (DataRow rowProperty in rows)
                {
                    PropertyInfo propertyInfo = this.GetType().GetProperty(rowProperty["Name"].ToString());
                    if (propertyInfo != null)
                    {
                        object value = Convert.ChangeType(rowProperty["Property_Text"], propertyInfo.PropertyType);
                        propertyInfo.SetValue(this, value, null);
                    }
                }
            }
        }

        private string GetRealLogFilename()
        {
            if (_realLogFilename == String.Empty)
            {
                string outputFile = this.Position;
                outputFile = this.ConvertVar(outputFile);
                return outputFile;
            }
            else
            {
                return _realLogFilename;
            }
        }

        protected override void Format(string level, string message, Exception exception)
        {

            if (this.Level.ToUpper().IndexOf(level.ToUpper()) < 0)
            {
                return;
            }

            string outputText =this.Template;
            outputText = this.ConvertVar(outputText);
            outputText = outputText.Replace("${Level}", level);
            outputText = outputText.Replace("${Message}", message);
            if (exception != null)
            {
                outputText = outputText.Replace("${Exception}", exception.ToString());
            }
            else
            {
                outputText = outputText.Replace("${Exception}", "");
            }
            outputText = outputText.Replace(@"\", "/");
            //直接把json数据转成XmlDocument
            XmlDocument xmlDoc = (XmlDocument)Newtonsoft.Json.JsonConvert.DeserializeXmlNode(outputText);
            
            LogWrite(xmlDoc);
            
        }

        protected override void LogWrite(object input)
        {
            string path = GetRealLogFilename();
           
            XmlDocument xdoc = ((XmlDocument)input);
            
            if (!File.Exists(path))
            {
                //当文件不村在的时候就创建，然后补上根节点
                Directory.CreateDirectory(Path.GetDirectoryName(path));                
                File.AppendAllText(path, "<logXml>\r\n" + xdoc.OuterXml + "\r\n</logXml>");
                File.AppendAllText(path, "\r\n");

            }
            else
            {
                //当文件存在时就 偏移11位（</logXml>\r\n）追加到xml文件中，然后后面加上 </logXml>\r\n
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Write, FileShare.None))
                {
                    fs.Seek(-11, SeekOrigin.End);
                    string OuterXml = xdoc.OuterXml + "\r\n</logXml>\r\n";
                    fs.Write(Encoding.GetEncoding("UTF-8").GetBytes(OuterXml), 0, Encoding.GetEncoding("UTF-8").GetByteCount(OuterXml));
                }
            }
         
           
        }
    }
}
