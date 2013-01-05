 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Explorer.Framework.Core;
using Explorer.Framework.Data;

namespace Explorer.Framework.Business
{    
    /// <summary>
    /// 业务委托
    /// </summary>
    public class BusinessDelegate
    {
        /// <summary>
        /// 业务逻辑处理
        /// </summary>
        /// <param name="name">业务关键字</param>
        /// <param name="dataSet">业务数据</param>
        /// <returns></returns>
        public static DataSet Process(string name, DataSet dataSet)
        {
            return Process(name, TransactionMode.Default, dataSet);
        }

        /// <summary>
        /// 业务逻辑处理
        /// </summary>
        /// <param name="name">业务关键字</param>
        /// <param name="mode">数据事务模式</param>
        /// <param name="dataSet">业务数据</param>
        /// <returns></returns>
        public static DataSet Process(string name, TransactionMode mode, DataSet dataSet)
        {
            return Process(name, mode, string.Empty, dataSet);
        }

        /// <summary>
        /// 业务逻辑处理
        /// </summary>
        /// <param name="name">业务关键字</param>
        /// <param name="mode">数据事务模式</param>
        /// <param name="dbName">数据源名称</param>
        /// <param name="dataSet">业务数据</param>
        /// <returns></returns>
        public static DataSet Process(string name, TransactionMode mode, string dbName, DataSet dataSet)
        {
            BusinessKey businessKey = BusinessKey.CreateKey(name, mode, dbName);

            return Process(businessKey, dataSet);
        }

        /// <summary>
        /// 业务逻辑处理
        /// </summary>
        /// <param name="businessKey">业务控制对象</param>
        /// <param name="dataSet">业务数据</param>
        /// <returns></returns>
        public static DataSet Process(BusinessKey businessKey, DataSet dataSet)
        {
            if (dataSet.Tables.Contains("BusinessKey"))
            {
                dataSet.Tables.Remove("BusinessKey");
            }
            BusinessKey.SetKey(ref dataSet, businessKey);
            DataSet result = null;
            BaseBusinessProxy proxy = BusinessProxyFactory.Instance.GetBusinessProxy(businessKey.Name, businessKey.Version);
            if (proxy != null)
            {
                result = proxy.Process(dataSet);
            }
            else
            {
                throw new Exception("无法找到 " + businessKey.Name + "@" + businessKey.Version + " 所指定的业务功能");
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Destory()
        {
            BusinessAssemblyManager.Instance.Dispose();      
        }
    }
}
