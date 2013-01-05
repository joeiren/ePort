using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using System.IO;
using System.Threading;

using Explorer.Framework.Core.Base;
using Explorer.Framework.Core.Initialize;

namespace Explorer.Framework.Core
{
    /// <summary>
    /// 系统环境
    /// </summary>
    public class AppContext : BaseSingletonInstance<AppContext>, IInitialize
    {
        private string _configFilename;
        private Dictionary<string, DataSet> _configSetMap;
        private Configuration _configuration;
        
        /// <summary>
        /// 初始化
        /// </summary>
        public override void Initialize()
        {
            this._configSetMap = new Dictionary<string, DataSet>();
            this._configFilename = AppDomain.CurrentDomain.BaseDirectory + "AppContext.xml";
            if (File.Exists(this._configFilename))
            {
                Initialize(this._configFilename);
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="configFilename"></param>
        public void Initialize(string configFilename)
        {
            DataSet configSet = new DataSet();
            configSet.ReadXml(configFilename);
            string filename = Path.GetFileName(configFilename);
            if (this._configSetMap.ContainsKey(filename))
            {
                this._configSetMap.Remove(filename);
            }
            this._configSetMap.Add(filename, configSet);


            this._configuration = new Configuration();            
            DataTable table = configSet.Tables["AppSettings"];
            if (table != null)
            {
                foreach (DataRow rowAppSetting in table.Rows)
                {
                    this.Configuration.Add(rowAppSetting["Name"].ToString(), new AppSettings());
                    DataRow[] rowsAdd = rowAppSetting.GetChildRows("AppSettings_Add");
                    foreach (DataRow rowAdd in rowsAdd)
                    {
                        this.Configuration[rowAppSetting["Name"].ToString()].Add(rowAdd["Key"].ToString(), rowAdd["Value"].ToString());
                    }
                }
            }

            this.IsInitialize = true;
        }

        /// <summary>
        /// 自定义配置
        /// </summary>
        public Configuration Configuration
        {
            get { return this._configuration; }
        }
    

        /// <summary>
        /// 获取 AppContext.xml 文件的数据集
        /// </summary>
        /// <returns></returns>
        public DataSet GetAppContextSet()
        {
            if (this._configSetMap.Count > 0)
            {
                return this._configSetMap["AppContext.xml"];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取指定配置文件的数据集
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DataSet GetConfigSet(string name)
        {
            return this._configSetMap[name];
        }

        /// <summary>
        /// 环境变量转换
        /// </summary>
        /// <param name="inputText"></param>
        /// <returns></returns>
        public static string ConvertEnvironmentVariable(string inputText)
        {
            inputText = inputText.Replace("${Path}", AppDomain.CurrentDomain.BaseDirectory);            
            inputText = inputText.Replace("${DateTime}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            inputText = inputText.Replace("${Date}", DateTime.Now.ToString("yyyy-MM-dd"));
            inputText = inputText.Replace("${Time}", DateTime.Now.ToString("HH:mm:ss"));
            inputText = inputText.Replace("${ThreadName}", Thread.CurrentThread.Name);
            return inputText;
        }
    }

    /// <summary>
    /// 自定义配置
    /// </summary>
    public class Configuration : IDictionary<string, AppSettings>
    {
        private Dictionary<string, AppSettings> _appSettingsMap;

        internal Configuration()
        {
            this._appSettingsMap = new Dictionary<string, AppSettings>();
        }

        #region IDictionary<string,AppSettings> 成员

        /// <summary>
        /// 添加一个 AppSettings
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, AppSettings value)
        {
            this._appSettingsMap.Add(key, value);
        }

        /// <summary>
        /// 检查一个 AppSettings 是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            return this._appSettingsMap.ContainsKey(key);
        }

        /// <summary>
        /// 获取所有的 Keys
        /// </summary>
        public ICollection<string> Keys
        {
            get { return this._appSettingsMap.Keys; }
        }

        /// <summary>
        /// 移除一个 Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            return this._appSettingsMap.Remove(key);
        }

        /// <summary>
        /// 尝试获取一个 Value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(string key, out AppSettings value)
        {
            return this.TryGetValue(key, out value);
        }

        /// <summary>
        /// 所有值的集合
        /// </summary>
        public ICollection<AppSettings> Values
        {
            get { return this._appSettingsMap.Values; }
        }

        /// <summary>
        /// AppSettings 索引器
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public AppSettings this[string key]
        {
            get
            {
                return this._appSettingsMap[key];
            }
            set
            {
                this._appSettingsMap[key] = value;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<string,AppSettings>> 成员

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(KeyValuePair<string, AppSettings> item)
        {
            ((ICollection<KeyValuePair<string, AppSettings>>)this._appSettingsMap).Add(item);
        }
        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            ((ICollection<KeyValuePair<string, AppSettings>>)this._appSettingsMap).Clear();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<string, AppSettings> item)
        {
            return ((ICollection<KeyValuePair<string, AppSettings>>)this._appSettingsMap).Contains(item);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(KeyValuePair<string, AppSettings>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, AppSettings>>)this._appSettingsMap).CopyTo(array, arrayIndex);
        }
        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get { return ((ICollection<KeyValuePair<string, AppSettings>>)this._appSettingsMap).Count; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly
        {
            get { return ((ICollection<KeyValuePair<string, AppSettings>>)this._appSettingsMap).IsReadOnly; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<string, AppSettings> item)
        {
            return ((ICollection<KeyValuePair<string, AppSettings>>)this._appSettingsMap).Remove(item);
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,AppSettings>> 成员
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, AppSettings>> GetEnumerator()
        {
            return this._appSettingsMap.GetEnumerator();
        }

        #endregion

        #region IEnumerable 成员

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this._appSettingsMap.GetEnumerator();
        }

        #endregion
    }

    /// <summary>
    /// 自定义配置集
    /// </summary>
    public class AppSettings : System.Collections.Specialized.NameValueCollection
    {
        internal AppSettings()
            : base()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        public string Name
        { get; set; }
    }
}
