using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using System.Threading;

using Explorer.Framework.Data.EntityAccess;
using Explorer.Framework.Data.ORM;

namespace Explorer.Framework.Data.DataAccess
{
    /// <summary>
    /// 数据访问对象
    /// </summary>
    [Serializable]
    public abstract class BaseDataAccess : MarshalByRefObject, IDataAccess
    {
        internal BaseDataAccess()
        {
            this.CreateTimeTicks = DateTime.Now.Ticks;
        }

        internal BaseDataAccess(IDbConnection dbConnection)
        {
            this.DbConnection = dbConnection;
            this.CreateTimeTicks = DateTime.Now.Ticks;
        }

        /// <summary>
        /// 连接名称
        /// </summary>
        public string Name
        { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DataBaseType DataBaseType
        { get; set; }

        /// <summary>
        /// 当前连接
        /// </summary>
        public IDbConnection DbConnection
        { get; private set; }

        /// <summary>
        /// 当前事务
        /// </summary>
        public IDbTransaction DbTransaction
        { get; set; }

        /// <summary>
        /// 参数分隔符
        /// </summary>
        public abstract string ParameterToken { get; }

        /// <summary>
        /// 建立一个事务
        /// </summary>
        /// <returns></returns>
        public IDbTransaction BeginTransaction()
        {
            this.DbTransaction = DbConnection.BeginTransaction();
            return this.DbTransaction;
        }

        /// <summary>
        /// 提交当前事务
        /// </summary>
        public void Commit()
        {
            if (this.DbTransaction != null)
            {
                this.DbTransaction.Commit();
            }
        }

        /// <summary>
        /// 回滚当前事务
        /// </summary>
        public void Rollback()
        {
            if (this.DbTransaction != null)
            {
                this.DbTransaction.Rollback();
            }
        }

        /// <summary>
        /// 打开当前连接
        /// </summary>
        public void Open()
        {
            if (DbConnection.State == ConnectionState.Closed || DbConnection.State == ConnectionState.Broken)
            {
                DbConnection.Open();
            }
        }

        /// <summary>
        /// 关闭当前连接
        /// </summary>
        public void Close()
        {
            this.DbConnection.Close();
        }

        /// <summary>
        /// 当前连接状态
        /// </summary>
        public ConnectionState State
        { get { return this.DbConnection.State; } }

        /// <summary>
        /// 创建时间戳
        /// </summary>
        public long CreateTimeTicks { get; private set; }

        /// <summary>
        /// 获取实体访问器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public EntityAccess<T> GetEntityAccess<T>() where T : new()
        {
            return EntityAccessFactory.Instance.CreateEntityAccess<T>(this);
        }

        /// <summary>
        /// 获取实体访问器
        /// </summary>
        /// <typeparam name="T">被操作的类</typeparam>
        /// <param name="entityAdapter">被操作的适配器</param>
        /// <returns></returns>
        public EntityAccess<T> GetEntityAccess<T>(EntityAdapter entityAdapter) where T : new()
        {
            return EntityAccessFactory.Instance.CreateEntityAccess<T>(this, entityAdapter);
        }

        /// <summary>
        /// 获取实体访问器
        /// </summary>
        /// <typeparam name="T">被操作的类</typeparam>
        /// <param name="fullName">被操作的类全名</param>
        /// <returns></returns>
        public EntityAccess<T> GetEntityAccess<T>(string fullName) where T : new()
        {
            EntityAdapter entityAdapter = EntityAdapterManager.Instance.GetAdapterByFullName(fullName);
            return EntityAccessFactory.Instance.CreateEntityAccess<T>(this, entityAdapter);
        }

        #region IDataAccess 成员
        /// <summary>
        /// 查询 SQL 句子, 并将数据装载成对象返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="textCommand"></param>
        /// <returns></returns>
        public List<T> QueryList<T>(string textCommand) where T : new()
        {
            IDbCommand dbCommand = this.DbConnection.CreateCommand();
            dbCommand.CommandText = textCommand;
            dbCommand.CommandType = CommandType.Text;
            return QueryList<T>(dbCommand);
        }

        /// <summary>
        /// 查询 SQL 句子, 并将数据装载成对象返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="textCommand"></param>
        /// <returns></returns>
        public List<T> QueryList<T>(IDbCommand dbCommand) where T : new()
        {
            IDataAdapter dataAdapter = null;
            dbCommand.Connection = this.DbConnection;
            dbCommand.Transaction = this.DbTransaction;
            dataAdapter = this.CreateDataAdapter(dbCommand);
            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet);
            DataTable table = dataSet.Tables[0];
            List<T> list = DataConverter.ToORMEntityList<T>(dataSet.Tables[0], true);
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textCommand"></param>
        /// <returns></returns>
        public virtual object QueryScalar(string textCommand)
        {
            IDbCommand dbCommand = this.DbConnection.CreateCommand();
            dbCommand.CommandText = textCommand;
            dbCommand.CommandType = CommandType.Text;
            return QueryScalar(dbCommand);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbCommand"></param>
        /// <returns></returns>
        public virtual object QueryScalar(IDbCommand dbCommand)
        {
            dbCommand.Connection = this.DbConnection;
            dbCommand.Transaction = this.DbTransaction;
            return dbCommand.ExecuteScalar();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textCommand"></param>
        /// <returns></returns>
        public virtual IDataReader QueryDataReader(string textCommand)
        {
            IDbCommand dbCommand = this.DbConnection.CreateCommand();
            dbCommand.CommandText = textCommand;
            dbCommand.CommandType = CommandType.Text;
            return QueryDataReader(dbCommand);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbCommand"></param>
        /// <returns></returns>
        public virtual IDataReader QueryDataReader(IDbCommand dbCommand)
        {
            dbCommand.Connection = this.DbConnection;
            dbCommand.Transaction = this.DbTransaction;
            return dbCommand.ExecuteReader();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textCommand"></param>
        /// <returns></returns>
        public virtual DataSet QueryDataSet(string textCommand)
        {
            IDbCommand dbCmd = this.DbConnection.CreateCommand();
            dbCmd.CommandText = textCommand;
            dbCmd.CommandType = CommandType.Text;
            return QueryDataSet(dbCmd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textCommand"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public virtual DataSet QueryDataSet(string textCommand, string tableName)
        {
            IDbCommand dbCommand = this.DbConnection.CreateCommand();
            dbCommand.CommandText = textCommand;
            dbCommand.CommandType = CommandType.Text;
            return QueryDataSet(dbCommand, tableName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="textCommand"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public virtual DataSet QueryFillDataSet(ref DataSet dataSet, string textCommand, string tableName)
        {
            IDbCommand dbCommand = this.DbConnection.CreateCommand();
            dbCommand.CommandText = textCommand;
            dbCommand.CommandType = CommandType.Text;
            return QueryFillDataSet(ref dataSet, dbCommand, tableName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbCommand"></param>
        /// <returns></returns>
        public virtual DataSet QueryDataSet(IDbCommand dbCommand)
        {
            string tableName = "";
            int posb = dbCommand.CommandText.ToUpper().IndexOf("FROM");
            int pose = dbCommand.CommandText.ToUpper().IndexOf(" WHERE", posb);
            if (pose == -1)
            {
                pose = dbCommand.CommandText.ToUpper().IndexOf(" GO", posb);
            }
            if (pose == -1)
            {
                pose = dbCommand.CommandText.ToUpper().IndexOf(" INNER JOIN", posb);
            }
            if (pose == -1)
            {
                pose = dbCommand.CommandText.ToUpper().IndexOf(" RIGHT", posb);
            }
            if (pose == -1)
            {
                pose = dbCommand.CommandText.ToUpper().IndexOf(" LEFT", posb);
            }
            if (pose == -1)
            {
                pose = dbCommand.CommandText.ToUpper().IndexOf(" FULL", posb);
            }
            if (pose == -1)
            {
                tableName = dbCommand.CommandText.Substring(posb + 4).Trim();
            }
            else
            {
                tableName = dbCommand.CommandText.Substring(posb + 4, pose - posb - 4).Trim();
            }
            posb = tableName.LastIndexOf(".");
            if (posb > -1)
            {
                tableName = tableName.Substring(posb + 1);
                tableName = tableName.Replace("[", "");
                tableName = tableName.Replace("]", "");
            }
            if (!dbCommand.Connection.GetType().FullName.Equals(this.DbConnection.GetType().FullName))
            {
                throw new DataException(this.DbConnection.GetType().FullName + " 不能执行 " + dbCommand.Connection.GetType().FullName);
            }
            return QueryDataSet(dbCommand, tableName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbCommand"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public virtual DataSet QueryDataSet(IDbCommand dbCommand, string tableName)
        {
            IDataAdapter dataAdapter = null;
            dbCommand.Connection = this.DbConnection;
            dbCommand.Transaction = this.DbTransaction;
            dataAdapter = this.CreateDataAdapter(dbCommand);
            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet);
            dataSet.Tables[0].TableName = tableName;
            return dataSet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="dbCommand"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public virtual DataSet QueryFillDataSet(ref DataSet dataSet, IDbCommand dbCommand, string tableName)
        {
            List<string> tableNameList = new List<string>();
            foreach (DataTable table in dataSet.Tables)
            {
                tableNameList.Add(table.TableName);
            }            
            IDataAdapter dataAdapter = null;
            dbCommand.Connection = this.DbConnection;
            dbCommand.Transaction = this.DbTransaction;
            dataAdapter = this.CreateDataAdapter(dbCommand);
            dataAdapter.Fill(dataSet);
            foreach (DataTable table in dataSet.Tables)
            {
                if (!tableNameList.Contains(table.TableName))
                {
                    table.TableName = tableName;
                }
            } 
            return dataSet;        
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textCommand"></param>
        /// <returns></returns>
        public virtual int Execute(string textCommand)
        {
            IDbCommand dbCommmand = this.DbConnection.CreateCommand();
            dbCommmand.Transaction = this.DbTransaction;
            dbCommmand.CommandText = textCommand;
            return dbCommmand.ExecuteNonQuery();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbCommand"></param>
        /// <returns></returns>
        public virtual int Execute(IDbCommand dbCommand)
        {
            dbCommand.Connection = this.DbConnection;
            dbCommand.Transaction = this.DbTransaction;
            return dbCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual IDbCommand CreateCommand()
        {
            return this.DbConnection.CreateCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlText"></param>
        /// <returns></returns>
        public virtual IDbCommand CreateCommand(string sqlText)
        {
            return  CreateCommand(sqlText, CommandType.Text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlText"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public virtual IDbCommand CreateCommand(string sqlText, CommandType commandType)
        {
            IDbCommand dbCommand = this.DbConnection.CreateCommand();
            dbCommand.CommandText = sqlText;
            dbCommand.CommandType = commandType;
            return dbCommand;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract IDbDataAdapter CreateDataAdapter();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectCommand"></param>
        /// <returns></returns>
        public virtual IDbDataAdapter CreateDataAdapter(IDbCommand selectCommand)
        {
            IDbDataAdapter adapter = this.CreateDataAdapter();
            if (CheckCommand(selectCommand, true))
            {
                adapter.SelectCommand = selectCommand;
            }
            return adapter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectCommand"></param>
        /// <param name="insertCommand"></param>
        /// <param name="updateCommand"></param>
        /// <param name="deleteCommand"></param>
        /// <returns></returns>
        public virtual IDbDataAdapter CreateDataAdapter(IDbCommand selectCommand, IDbCommand insertCommand, IDbCommand updateCommand, IDbCommand deleteCommand)
        {
            IDbDataAdapter adapter = this.CreateDataAdapter();
            if (CheckCommand(selectCommand, true))
            {
                adapter.SelectCommand = selectCommand;
            }
            if (CheckCommand(insertCommand, true))
            {
                adapter.InsertCommand = insertCommand;
            }
            if (CheckCommand(updateCommand, true))
            {
                adapter.UpdateCommand = updateCommand;
            }
            if (CheckCommand(deleteCommand, true))
            {
                adapter.DeleteCommand = deleteCommand;
            }

            return adapter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="couldNull"></param>
        private bool CheckCommand(IDbCommand command, bool couldNull)
        {
            if (command != null)
            {
                if ((command.Connection != null && command.Connection.GetType() != this.DbConnection.GetType())
                    || (!command.GetType().Namespace.Equals(this.DbConnection.GetType().Namespace)))
                {
                    throw new DataException("命令与DataAccess实例不匹配！");
                }
            }
            else
            {
                if (!couldNull)
                {
                    throw new DataException("IDbCommand不能为空！");
                }
            }
            return true;
        }

        #endregion

        #region IDisposable 成员

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.DbConnection.Close();
            this.DbConnection.Dispose();
            this.DbConnection = null;
            this.DbTransaction = null;
        }

        #endregion

    }



}
