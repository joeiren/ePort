using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Channels.Tcp;

using System.Data;
using Explorer.Framework.Data;

namespace Explorer.Framework.Business
{
    /// <summary>
    /// 业务逻辑代理器的基类
    /// </summary>
    public abstract class BaseBusinessProxy
    {
        /// <summary>
        /// 引用到的 BusinessAssembly 名称
        /// </summary>
        public string BusinessAssemblyName { get; set; }

        /// <summary>
        /// 业务名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 业务版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 调用模式
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        /// 数据源名称
        /// </summary>
        public string DbName { get; set; }

        /// <summary>
        /// 事务模式
        /// </summary>
        public string TransactionMode { get; set; }

        /// <summary>
        /// 反射的 ClassName
        /// </summary>
        public string Provider { get; set; }

        public BaseBusinessProxy(DataRow row)
        {            
            this.Name = row["Name"].ToString();

            this.Version = "";
            if (row.GetParentRow("Bla_Blh").Table.Columns.Contains("Version"))
            {
                this.Version = row.GetParentRow("Bla_Blh")["Version"].ToString();
            }
            this.DbName = "";
            if (row.Table.Columns.Contains("DbName"))
            {
                this.DbName = row["DbName"].ToString();
            }
            this.TransactionMode = "false";
            if (row.Table.Columns.Contains("TransactionMode"))
            {
                this.TransactionMode = row["TransactionMode"].ToString();
            }
            if (row.Table.Columns.Contains("Provider"))
            {
                this.Provider = row["Provider"].ToString();
            }
            try
            {
                this.BusinessAssemblyName = row.GetParentRow("Bla_Blh")["Name"].ToString();
            }
            catch { }
        }

        public abstract DataSet Process(DataSet dataSet);


    }




}
