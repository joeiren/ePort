using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.OracleClient;
//using Oracle.DataAccess.Client;

using Explorer.Framework.Core;
using Explorer.Framework.Core.Base;
using Explorer.Framework.Core.ExtendAttribute;

namespace Explorer.Framework.Data.DataAccess
{
    /// <summary>
    /// 
    /// </summary>
    public enum DataBaseType
    {
        /// <summary>
        /// SqlServer
        /// </summary>
        [Remark("Sql", "System.Data.SqlClient.SqlConnection")]
        Sql,
        /// <summary>
        /// Oracle
        /// </summary>
        [Remark("Oracle", "System.Data.OracleClient.OracleConnection")]
        Oracle,
        /// <summary>
        /// Oracle ODAC
        /// </summary>
        [Remark("Odac", "Oracle.DataAccess.Client.OracleConnection")]
        Odac,
        /// <summary>
        /// OleDb
        /// </summary>
        [Remark("OleDb", "System.Data.OleDb.OleDbConnection")]
        OleDb,
        /// <summary>
        /// SQLite
        /// </summary>
        [Remark("SQLite", "System.Data.SQLite.SQLiteConnection")]
        SQLite
    }

    /// <summary>
    /// 
    /// </summary>
    public class DataAccessBuilder : BaseBuilder<DataAccessBuilder, IDataAccess>
    {
        /// <summary>
        /// 跟据配置文件创建指定名称数据访问对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override IDataAccess CreateInstance(string name)
        {
            IDataAccess da = null;
            DataSet dataSet = AppContext.Instance.GetAppContextSet();
            DataTable table = dataSet.Tables["DataSource"];
            if (table != null)
            {
                DataRow[] rows = table.Select("Name='" + name + "'");
                if (rows.Length > 0)
                {
                    if (rows[0]["Mode"].ToString().ToUpper().IndexOf("LOCAL") > -1)
                    {
                        DataRow[] childRows = rows[0].GetChildRows("DataSource_Property");
                        string connectionString = "";
                        foreach (DataRow row in childRows)
                        {
                            if (row["Name"].ToString().ToUpper().Equals("CONNECTIONSTRING"))
                            {
                                connectionString = row["Property_Text"].ToString();
                            }
                        }                        
                        string dbType = "Sql";
                        if (rows[0].Table.Columns.Contains("DataBase"))
                        {
                            dbType = rows[0]["DataBase"].ToString();
                        }
                        else if (rows[0].Table.Columns.Contains("DataBaseType"))
                        {
                            dbType = rows[0]["DataBaseType"].ToString();
                        }
                        da = CreateInstance(rows[0]["Name"].ToString(), (DataBaseType)Enum.Parse(typeof(DataBaseType), dbType), connectionString);
                    }
                    if (rows[0].Table.Columns.Contains("File"))
                    {
                        // 特殊 SQL 连接用到的文件名
                        //Assembly.LoadFile( rows[0]["File"].ToString());

                    }
                    if (rows[0]["Mode"].ToString().ToUpper().IndexOf("SERVICE") > -1)
                    {
                        // 将上面的这个 DataAccess 注册到 Remoting 服务里
                    }
                    if (rows[0]["Mode"].ToString().ToUpper().IndexOf("REMOTING") > -1)
                    { 
                        // 从 Remoting 里取出一个 DataAccess
                    }
                }
            }
            return da;
        }

        /// <summary>
        /// 创建数据访问对象
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dataBaseType"></param>
        /// <param name="connectionString"></param>
        public IDataAccess CreateInstance(string name, DataBaseType dataBaseType, string connectionString)
        {
            IDbConnection conn = null;
            IDataAccess da = null;

            RemarkAttribute remark = (RemarkAttribute)Attribute.GetCustomAttribute(dataBaseType.GetType().GetField(dataBaseType.ToString()), typeof(RemarkAttribute));
            
            string className = remark.Description;

            Type classType = null;
            Assembly[] assemblyArray = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblyArray)
            {
                classType = assembly.GetType(className);
                if (classType != null)
                {
                    conn = (IDbConnection)classType.Assembly.CreateInstance(className);
                    break;
                }
            }
            if (conn == null) 
            {
                if (className.Equals("System.Data.OracleClient.OracleConnection"))
                {
                    string[] fOracle = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "System.Data.OracleClient*", SearchOption.AllDirectories);
                    if (fOracle.Length == 0)
                    {
                        throw new ArgumentException("System File Not Found System.Data.OracleClient.dll");
                    }
                    Assembly assembly = Assembly.LoadFile(fOracle[0]);
                    conn = (IDbConnection)assembly.CreateInstance(className);
                }
                else if (className.Equals("Oracle.DataAccess.Client.OracleConnection"))
                {
                    string[] fOdac = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "Oracle.DataAccess*", SearchOption.AllDirectories);
                    string[] fOra = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "OraOps11w*", SearchOption.AllDirectories);
                    if (fOdac.Length == 0) 
                    {
                        throw new ArgumentException("System File Not Found Oracle.DataAccess.dll");
                    }
                    if (fOra.Length == 0)
                    {
                        throw new ArgumentException("System File Not Found OraOps11w.dll");
                    }
                    Assembly assembly = Assembly.LoadFile(fOdac[0]);
                    conn = (IDbConnection)assembly.CreateInstance(className);
                }
                else if (className.Equals("System.Data.SQLite.SQLiteConnection"))
                {
                    string[] fSQLite = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "System.Data.SQLite*", SearchOption.AllDirectories);
                    if (fSQLite.Length == 0)
                    {
                        throw new ArgumentException("System File Not Found System.Data.SQLite.dll");
                    }
                    Assembly assembly = Assembly.LoadFile(fSQLite[0]);
                    conn = (IDbConnection)assembly.CreateInstance(className);
                }
            }
            conn.ConnectionString = connectionString;

            switch (dataBaseType)
            {
                case DataBaseType.Sql:
                    da = new SqlDataAccess(conn);
                    break;
                case DataBaseType.OleDb:
                    da = new OleDbDataAccess(conn);
                    break;
                case DataBaseType.Oracle:
                    da = new OracleDataAccess(conn);
                    break;
                case DataBaseType.Odac:                    
                    da = new OdacDataAccess(conn);
                    break;
                case DataBaseType.SQLite:
                    da = new OdacDataAccess(conn);
                    break;
            }
            da.Name = name;
            da.DataBaseType = dataBaseType;
            conn.Open();
            return da;
        }
    }
}
