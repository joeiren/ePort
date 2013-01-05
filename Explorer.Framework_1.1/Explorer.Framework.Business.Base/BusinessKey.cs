using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.Security;
using System.Security.Cryptography;

namespace Explorer.Framework.Business
{
    public enum TransactionMode
    {
        /// <summary>
        /// 默认模式,采用配置文件定义
        /// </summary>
        Default,
        /// <summary>
        /// 简单模式,无数据库事务提交
        /// </summary>
        Simple,
        /// <summary>
        /// 事务模式,数据库事务提交
        /// </summary>
        Transaction
    }

    public class BusinessKey
    {
        /// <summary>
        /// 跨网络访问时用户帐号
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 跨网络访问时用户认证签名 MD5(Username + Password + DateTime[yyyyMMddHHmmss])
        /// </summary>
        public string Sign { get; set; }

        /// <summary>
        /// 跨网络访问时认证时间
        /// </summary>
        public string DateTime { get; set; }

        /// <summary>
        /// 业务逻辑名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 业务逻辑简单值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 引用数据访问名
        /// </summary>
        public string DbName { get; set; }

        /// <summary>
        /// 事务模式
        /// </summary>
        public string TransactionMode { get; set; }
        
        /// <summary>
        /// 服务提供者 [ClassName 或是 Url]
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// 业务逻辑里出错的捕获信息
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// 业务逻辑上下文消息
        /// </summary>
        public string ContentMessage { get; set; }

        /// <summary>
        /// 当业务逻辑为远程服务时采用的版本号
        /// </summary>
        public string Version { get; set; }


        public BusinessKey()
        {
            this.Version = "";
            this.Sign = "";
            this.DbName = "";
        }

        /// <summary>
        /// 创建立一个业务逻辑键
        /// </summary>
        /// <param name="key"></param>
        /// <param name="mode"></param>
        /// <param name="dbName"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static BusinessKey CreateKey(string name, TransactionMode mode, string dbName, string provider)
        {
            BusinessKey businessKey = new BusinessKey();
            businessKey.Name = name;
            businessKey.TransactionMode = mode.ToString();
            businessKey.DbName = dbName;
            businessKey.Provider = provider;
            return businessKey;
        }

        /// <summary>
        /// 创建立一个业务逻辑键
        /// </summary>
        /// <param name="key"></param>
        /// <param name="mode"></param>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public static BusinessKey CreateKey(string name, TransactionMode mode, string dbName)
        {
            BusinessKey businessKey = new BusinessKey();
            businessKey.Name = name;
            businessKey.TransactionMode = mode.ToString();
            businessKey.DbName = dbName;
            return businessKey;
        }

        /// <summary>
        /// 创建立一个业务逻辑键
        /// </summary>
        /// <param name="key"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static BusinessKey CreateKey(string name, TransactionMode mode)
        {
            BusinessKey businessKey = new BusinessKey();
            businessKey.Name = name;
            businessKey.TransactionMode = mode.ToString();
            return businessKey;
        }

        /// <summary>
        /// 把一个业务逻辑键设置到 DataSet 里
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="businessKey"></param>
        /// <returns></returns>
        public static DataSet SetKey(ref DataSet dataSet, BusinessKey businessKey)
        {
            Type type = businessKey.GetType();
            string tableName = type.Name;
            DataTable table;
            if (dataSet.Tables.Contains("BusinessKey"))
            {               
                table = dataSet.Tables["BusinessKey"];
                table.Clear();
            }
            else
            {
                table = dataSet.Tables[type.Name];
                if (table == null)
                {
                    table = new DataTable(tableName);
                    dataSet.Tables.Add(table);
                    foreach (PropertyInfo pInfo in type.GetProperties())
                    {
                        if (pInfo.PropertyType.Name.StartsWith("Nullable"))
                        {
                            table.Columns.Add(pInfo.Name, pInfo.PropertyType.GetGenericArguments()[0]);
                        }
                        else
                        {
                            table.Columns.Add(pInfo.Name, pInfo.PropertyType);
                        }
                    }
                }
            }
            DataRow row = table.NewRow();
            foreach (PropertyInfo pInfo in type.GetProperties())
            {
                string columnName = pInfo.Name;
                if (table.Columns.Contains(columnName))
                {
                    row[columnName] = pInfo.GetValue(businessKey, null);
                }
            }
            table.Rows.Add(row);

            return dataSet;
        }

        /// <summary>
        /// 从 DataSet 里获取一个业务逻辑键
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        public static BusinessKey GetKey(DataSet dataSet)
        {
            return GetKey(dataSet, false);
        }

        /// <summary>
        /// 从 DataSet 里获取一个业务逻辑键, 并决定是否要在 DataSet 删除他
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="isRemove"></param>
        /// <returns></returns>
        public static BusinessKey GetKey(DataSet dataSet, bool isRemove)
        {
            BusinessKey businessKey = new BusinessKey();
            DataTable table = dataSet.Tables["BusinessKey"];
            try
            {                
                DataRow row = table.Rows[0];
                Type classType = typeof(BusinessKey);
                foreach (PropertyInfo propertyInfo in classType.GetProperties())
                {
                    object value = row[propertyInfo.Name];
                    if (value != null && value != System.DBNull.Value)
                    {
                        propertyInfo.SetValue(businessKey, value, null);
                    }
                }
                if (isRemove)
                {
                    dataSet.Tables.Remove("BusinessKey");
                }
            }
            catch
            {
                return null;
            }
            return businessKey;
        }

        /// <summary>
        /// 通过帐号密码和标记时间来计算签名
        /// </summary>
        /// <param name="username">认证帐号</param>
        /// <param name="password">认证密码</param>
        /// <param name="datetime">认证时间(yyyyMMddHHmmss)</param>
        /// <returns></returns>
        public static string BuildSign(string username, string password, string datetime)
        {
            // 创建一个MD5CryptoServiceProvider对象的新实例。

            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            // 将输入的字符串转换为一个字节数组并计算哈希值。
            byte[] InBytes = Encoding.Default.GetBytes(username + password + datetime);
            byte[] OutBytes = md5.ComputeHash(InBytes);
            string OutString = "";

            // 遍历字节数组，将每一个字节转换为十六进制字符串后，追加到StringBuilder实例的结尾
            for (int i = 0; i < OutBytes.Length; i++)
            {
                OutString += OutBytes[i].ToString("x2");
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符
            }

            return OutString;
        }


    }
}
