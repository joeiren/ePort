using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.Data.SqlClient;
using System.Data.OracleClient;
using System.Data.OleDb;

using Explorer.Framework.Data.DataAccess;
using Explorer.Framework.Data.XRM;

namespace Explorer.Framework.Data.EntityAccess
{
    /// <summary>
    /// 
    /// </summary>
    public class XRMEntityAccess<T> : EntityAccess<T> where T : new()
    {
        
        internal XRMEntityAccess(IDataAccess dataAccess, EntityAdapter entityAdapter)
            : base(dataAccess, entityAdapter)
        {
            
        }


        /// <summary>
        /// 根据主键来读取当前数据实体类
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public override bool Load(ref T t)
        {
            XRMEntity entity = t as XRMEntity;
            
            this.EntityAdapter = entity.EntityAdapter;

            IDbCommand dbCommand = GetCacheLoadCommand();

            foreach (IDataParameter dataParameter in dbCommand.Parameters)
            {
                object value = entity.Items.GetValue(dataParameter.ParameterName.Substring(1));
                dataParameter.Value = value == null ? DBNull.Value : value;
            }
            if (this.DataAccess.DbTransaction != null)
            {
                dbCommand.Transaction = this.DataAccess.DbTransaction;
            }
            DataSet dataSet = new DataSet();
            IDataAdapter dataAdapter = this.DataAccess.CreateDataAdapter(dbCommand);
            try
            {
                dataAdapter.Fill(dataSet);
                dataSet.Tables[0].TableName = this.EntityAdapter.TableName;
                t = DataConverter.ToXRMEntity<T>(dataSet.Tables[0].Rows[0], this.EntityAdapter);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 跟据属性值来判断当前实体类是否存在, 如果返回值大于 1 则表示有多条数据满足该条件
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public override int Exists(T t)
        {
            return Exists(t, ExistsMode.And);
        }

        /// <summary>
        /// 跟据属性值来判断当前实体类是否存在, 如果返回值大于 1 则表示有多条数据满足该条件
        /// 并且通过 ExistsMode.And 或是 ExistsMode.Or 决定检查条件
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public override int Exists(T t, ExistsMode mode)
        {
            XRMEntity entity = t as XRMEntity;

            this.EntityAdapter = entity.EntityAdapter;

            IDbCommand dbCommand = this.DataAccess.DbConnection.CreateCommand();
            dbCommand.CommandType = CommandType.Text;

            StringBuilder sbSqlText = new StringBuilder("SELECT COUNT(*) FROM ");
            sbSqlText.Append(this.EntityAdapter.TableName);

            StringBuilder sbWhere = new StringBuilder();
            foreach (KeyValuePair<string, object> kvp in entity.Items)
            {
                if (this.EntityAdapter.KeyMap.ContainsKey(kvp.Key) && kvp.Value != null)
                {
                    if (mode == ExistsMode.And)
                    {
                        sbWhere.Append(" AND ");
                    }
                    else
                    {
                        sbWhere.Append(" OR ");
                    }
                    sbWhere.Append(this.EntityAdapter.KeyMap[kvp.Key]);
                    sbWhere.Append("=" + this.DataAccess.ParameterToken);
                    sbWhere.Append(kvp.Key);
                    IDataParameter dataParameter = dbCommand.CreateParameter();
                    dataParameter.ParameterName = this.DataAccess.ParameterToken + kvp.Key;
                    dataParameter.DbType = this.EntityAdapter.KeyDbType[kvp.Key];
                    dataParameter.Value = kvp.Value;
                    dbCommand.Parameters.Add(dataParameter);
                }
            }

            if (sbWhere.Length > 0)
            {
                sbSqlText.Append(" WHERE ");
                sbSqlText.Append(sbWhere.ToString().Substring(4));
            }

            dbCommand.CommandText = sbSqlText.ToString();
            if (this.DataAccess.DbTransaction != null)
            {
                dbCommand.Transaction = this.DataAccess.DbTransaction;
            }
            return (int)dbCommand.ExecuteScalar();
        }

        /// <summary>
        /// 跟据属性值来判断当前实体类是否存在, 并返回找到的所有实体类, 
        /// 默认使用 ExistsMode.And 判断
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public override List<T> Select(T t)
        {
            return Select(t, ExistsMode.And);
        }

        /// <summary>
        /// 跟据属性值来判断当前实体类是否存在, 并返回找到的所有实体类
        /// 并且通过 ExistsMode.And 或是 ExistsMode.Or 决定检查条件
        /// </summary>
        /// <param name="t"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public override List<T> Select(T t, ExistsMode mode)
        {
            if (t == null)
                throw new ArgumentNullException("t");

            XRMEntity entity = t as XRMEntity;

            this.EntityAdapter = entity.EntityAdapter;

            IDbCommand dbCommand = this.DataAccess.DbConnection.CreateCommand();
            dbCommand.CommandType = CommandType.Text;

            StringBuilder sbSqlText = new StringBuilder("SELECT * FROM ");
            sbSqlText.Append(this.EntityAdapter.TableName);

            StringBuilder sbWhere = new StringBuilder();
            foreach (KeyValuePair<string, object> kvp in entity.Items)
            {
                if (this.EntityAdapter.KeyMap.ContainsKey(kvp.Key) && kvp.Value != null)
                {
                    if (mode == ExistsMode.And)
                    {
                        sbWhere.Append(" AND ");
                    }
                    else
                    {
                        sbWhere.Append(" OR ");
                    }
                    sbWhere.Append(this.EntityAdapter.KeyMap[kvp.Key]);
                    sbWhere.Append("=" + this.DataAccess.ParameterToken);
                    sbWhere.Append(kvp.Key);
                    IDataParameter dataParameter = dbCommand.CreateParameter();
                    dataParameter.ParameterName = this.DataAccess.ParameterToken + kvp.Key;
                    dataParameter.DbType = this.EntityAdapter.KeyDbType[kvp.Key];
                    dataParameter.Value = kvp.Value;
                    dbCommand.Parameters.Add(dataParameter);
                }
            }

            if (sbWhere.Length > 0)
            {
                sbSqlText.Append(" WHERE ");
                sbSqlText.Append(sbWhere.ToString().Substring(4));
            }

            dbCommand.CommandText = sbSqlText.ToString();
            if (this.DataAccess.DbTransaction != null)
            {
                dbCommand.Transaction = this.DataAccess.DbTransaction;
            }

            IDataAdapter dataAdapter = this.DataAccess.CreateDataAdapter(dbCommand);

            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet);
            dataSet.Tables[0].TableName = this.EntityAdapter.TableName;
            return DataConverter.ToORMEntityList<T>(dataSet, this.EntityAdapter.TableName);
        }

        /// <summary>
        /// 实现对数据的保存操作
        /// </summary>
        /// <param name="t">数据源，即被操作实体类对象（对应数据库中的某一张表）</param>
        /// <returns></returns>
        public override int Save(T t)
        {
            XRMEntity entity = t as XRMEntity;

            this.EntityAdapter = entity.EntityAdapter;

            IDbCommand dbCommand = GetCacheSaveCommand();

            foreach (IDataParameter dataParameter in dbCommand.Parameters)
            {
                object value = entity.Items.GetValue(dataParameter.ParameterName.Substring(1));
                dataParameter.Value = value == null ? DBNull.Value : value;
            }
            if (this.DataAccess.DbTransaction != null)
            {
                dbCommand.Transaction = this.DataAccess.DbTransaction;
            }
            return dbCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// 实现对数据的更新操作
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public override int Update(T t)
        {
            XRMEntity entity = t as XRMEntity;

            this.EntityAdapter = entity.EntityAdapter;

            IDbCommand dbCommand = GetCacheUpdateCommand();

            foreach (IDataParameter dataParameter in dbCommand.Parameters)
            {
                object value = entity.Items.GetValue(dataParameter.ParameterName.Substring(1));
                dataParameter.Value = value == null ? DBNull.Value : value;
            }
            if (this.DataAccess.DbTransaction != null)
            {
                dbCommand.Transaction = this.DataAccess.DbTransaction;
            }
            return dbCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// 实现对数据的删除操作
        /// </summary>
        /// <param name="t">数据源，即被操作的实体类对象（对应数据库中的某一张表）</param>
        /// <returns>被影响的数据行数</returns>
        public override int Delete(T t)
        {
            XRMEntity entity = t as XRMEntity;

            this.EntityAdapter = entity.EntityAdapter;

            IDbCommand dbCommand = GetCacheDeleteCommand();

            foreach (IDataParameter dataParameter in dbCommand.Parameters)
            {
                object value = entity.Items[dataParameter.ParameterName.Substring(1)];
                dataParameter.Value = value == null ? DBNull.Value : value;
            }

            if (this.DataAccess.DbTransaction != null)
            {
                dbCommand.Transaction = this.DataAccess.DbTransaction;
            }
            return dbCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// 查询操作
        /// </summary>
        /// <param name="whereString">查询条件子句</param>
        /// <param name="orderString">排序条件子句</param>
        /// <returns></returns>
        [Obsolete]
        public override DataSet QueryDataSet(string whereString, string orderString)
        {
            IDbCommand dbCommand = this.DataAccess.DbConnection.CreateCommand();

            StringBuilder sbSqlText = new StringBuilder("SELECT * FROM " + this.EntityAdapter.TableName);
            if (!string.IsNullOrEmpty(whereString))
            {
                whereString = " WHERE " + whereString;
                sbSqlText.Append(whereString);
            }

            if (!string.IsNullOrEmpty(orderString))
            {
                orderString = " ORDER BY " + orderString;
                sbSqlText.Append(orderString);
            }

            dbCommand.CommandText = sbSqlText.ToString();
            dbCommand.CommandType = CommandType.Text;

            if (this.DataAccess.DbTransaction != null)
            {
                dbCommand.Transaction = this.DataAccess.DbTransaction;
            }

            IDataAdapter dataAdapter = this.DataAccess.CreateDataAdapter(dbCommand);

            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet);
            dataSet.Tables[0].TableName = this.EntityAdapter.TableName;
            return dataSet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="whereString"></param>
        /// <param name="orderString"></param>
        /// <returns></returns>
        [Obsolete]
        public override List<T> QueryList(string whereString, string orderString)
        {
            DataSet dataSet = QueryDataSet(whereString, orderString);
            return DataConverter.ToXRMEntityList<T>(dataSet, this.EntityAdapter, this.EntityAdapter.TableName);
        }

        /// <summary>
        ///查询操作 
        /// </summary>
        /// <param name="whereString">查询条件子句</param>
        /// <param name="orderString">排序条件子句</param>
        /// <param name="pageStart">定义从第几条数据开始查询</param>
        /// <param name="pageSize">定义查询出的记录数</param>
        /// <returns></returns>
        [Obsolete]
        public override DataSet QueryDataSetByPage(string whereString, string orderString, int pageStart, int pageSize)
        {
            IDbCommand dbCommand = this.EntityAdapter.CreateDataSetPage(this.DataAccess,whereString,orderString, pageStart, pageSize);
            if (this.DataAccess.DbTransaction != null)
            {
                dbCommand.Transaction = this.DataAccess.DbTransaction;
            }
            IDataAdapter dataAdapter = this.DataAccess.CreateDataAdapter(dbCommand);

            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet);
            dataSet.Tables[0].TableName = this.EntityAdapter.TableName;
            return dataSet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="whereString"></param>
        /// <param name="orderString"></param>
        /// <param name="start"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [Obsolete]
        public override List<T> QueryListByPage(string whereString, string orderString, int start, int size)
        {
            DataSet dataSet = QueryDataSetByPage(whereString, orderString, start, size);
            return DataConverter.ToXRMEntityList<T>(dataSet, this.EntityAdapter, this.EntityAdapter.TableName);
        }
    }
}
