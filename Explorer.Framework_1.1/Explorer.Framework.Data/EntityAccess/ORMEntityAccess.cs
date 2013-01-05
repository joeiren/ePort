using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Data;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Data.OleDb;

using Explorer.Framework.Core.ExtendAttribute;
using Explorer.Framework.Data.DataAccess;
using Explorer.Framework.Data.ORM;

namespace Explorer.Framework.Data.EntityAccess
{
    /// <summary>
    /// 实体类存取器, 通过 DataEntity 绑定来操作数据存取
    /// </summary>
    [Serializable]
    public class ORMEntityAccess<T> : EntityAccess<T> where T : new()
    {
        #region 私有变量
        private bool _isStaticUpdate = true;
        #endregion

        internal ORMEntityAccess(IDataAccess dataAccess, EntityAdapter entityAdapter)
            : base(dataAccess, entityAdapter)
        {
            Type baseType = typeof(T);
            while (baseType.BaseType != null)
            {
                if (baseType.Equals(typeof(ORMEntity)))
                {
                    this._isStaticUpdate = false;
                    break;
                }
                baseType = baseType.BaseType;
            }
        }

        /// <summary>
        /// 根据主键来读取当前数据实体类
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public override bool Load(ref T t)
        {
            if (t == null)
                throw new ArgumentNullException("t");
            Type classType = t.GetType();
            IDbCommand dbCommand = this.GetCacheLoadCommand();

            foreach (IDataParameter dataParameter in dbCommand.Parameters)
            {
                string name = dataParameter.ParameterName.Substring(1);
                PropertyInfo propertyInfo = classType.GetProperty(name);
                dataParameter.Value = propertyInfo.GetValue(t, null);
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
                t = DataConverter.ToORMEntity<T>(dataSet.Tables[0].Rows[0]);
                if (t is ORMEntity)
                {
                    ORMEntity dataEntity = t as ORMEntity;
                    dataEntity.ClearChangedProperties();
                }
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
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
            if (t == null)
                throw new ArgumentNullException("t");
            IDbCommand dbCommand = this.DataAccess.DbConnection.CreateCommand();
            dbCommand.CommandType = CommandType.Text;

            Type classType = typeof(T);

            StringBuilder sbSqlText = new StringBuilder("SELECT COUNT(*) FROM ");
            sbSqlText.Append(this.EntityAdapter.TableName);

            StringBuilder sbWhere = new StringBuilder();
            foreach (PropertyInfo propertyInfo in classType.GetProperties())
            {
                object value = propertyInfo.GetValue(t, null);
                if (this.EntityAdapter.KeyMap.ContainsKey(propertyInfo.Name) && value != null)
                {
                    if (mode == ExistsMode.And)
                    {
                        sbWhere.Append(" AND ");
                    }
                    else
                    {
                        sbWhere.Append(" OR ");
                    }
                    sbWhere.Append(this.EntityAdapter.KeyMap[propertyInfo.Name]);
                    sbWhere.Append("=" + this.DataAccess.ParameterToken);
                    sbWhere.Append(propertyInfo.Name);
                    IDataParameter dataParameter = dbCommand.CreateParameter();
                    dataParameter.ParameterName = this.DataAccess.ParameterToken + propertyInfo.Name;
                    dataParameter.DbType = this.EntityAdapter.KeyDbType[propertyInfo.Name];
                    dataParameter.Value = value;
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
            IDbCommand dbCommand = this.DataAccess.DbConnection.CreateCommand();
            dbCommand.CommandType = CommandType.Text;

            Type classType = typeof(T);

            StringBuilder sbSqlText = new StringBuilder("SELECT * FROM ");
            sbSqlText.Append(this.EntityAdapter.TableName);

            StringBuilder sbWhere = new StringBuilder();
            foreach (PropertyInfo propertyInfo in classType.GetProperties())
            {
                object value = propertyInfo.GetValue(t, null);
                if (this.EntityAdapter.KeyMap.ContainsKey(propertyInfo.Name) && value != null)
                {
                    if (mode == ExistsMode.And)
                    {
                        sbWhere.Append(" AND ");
                    }
                    else
                    {
                        sbWhere.Append(" OR ");
                    }
                    sbWhere.Append(this.EntityAdapter.KeyMap[propertyInfo.Name]);
                    sbWhere.Append("=" + this.DataAccess.ParameterToken);
                    sbWhere.Append(propertyInfo.Name);
                    IDataParameter dataParameter = dbCommand.CreateParameter();
                    dataParameter.ParameterName = this.DataAccess.ParameterToken + propertyInfo.Name;
                    dataParameter.DbType = this.EntityAdapter.KeyDbType[propertyInfo.Name];
                    dataParameter.Value = value;
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
            if (t == null)
                throw new ArgumentNullException("t");
            IDbCommand dbCommand = this.GetCacheSaveCommand();
            foreach (IDataParameter dataParameter in dbCommand.Parameters)
            {
                PropertyInfo p = t.GetType().GetProperty(dataParameter.ParameterName.Substring(1));
                object value = p.GetValue(t, null);
                dataParameter.Value = value == null ? DBNull.Value : value;
            }
            if (this.DataAccess.DbTransaction != null)
            {
                dbCommand.Transaction = this.DataAccess.DbTransaction;
            }
            if (t is ORMEntity)
            {
                ORMEntity dataEntity = t as ORMEntity;
                dataEntity.ClearChangedProperties();
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
            if (t == null)
            {
                throw new ArgumentNullException("t");
            }
            IDbCommand dbCommand = null;
            if (this._isStaticUpdate)
            {
                dbCommand = this.GetCacheUpdateCommand();
            }
            else
            {
                ORMEntity dataEntity = t as ORMEntity;
                List<string> list = dataEntity.GetChangedProperties();
                if (list.Count <= 0)
                {
                    return 0;
                }

                StringBuilder sbSqlText = new StringBuilder("UPDATE " + this.EntityAdapter.TableName + " SET ");
                StringBuilder sbColumns = new StringBuilder();
                StringBuilder sbWhere = new StringBuilder();

                dbCommand = this.DataAccess.DbConnection.CreateCommand();
                string[] nameArray = null;
                if (!this.EntityAdapter.KeyMap["$Id"].Equals(""))
                {
                    nameArray = this.EntityAdapter.KeyMap["$Id"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string name in nameArray)
                    {
                        list.Remove(name);
                        sbWhere.Append(" AND ");
                        sbWhere.Append(this.EntityAdapter.KeyMap[name]);
                        sbWhere.Append("=" + this.DataAccess.ParameterToken);
                        sbWhere.Append(name);
                        IDataParameter dataParameter = dbCommand.CreateParameter();
                        dataParameter.ParameterName = this.DataAccess.ParameterToken + name;
                        dataParameter.DbType = this.EntityAdapter.KeyDbType[name];
                        dbCommand.Parameters.Add(dataParameter);
                    }
                }
                else
                {
                    nameArray = this.EntityAdapter.KeyMap["$Pk"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string name in nameArray)
                    {
                        list.Remove(name);
                        sbWhere.Append(" AND ");
                        sbWhere.Append(this.EntityAdapter.KeyMap[name]);
                        sbWhere.Append("=" + this.DataAccess.ParameterToken);
                        sbWhere.Append(name);
                        IDataParameter dataParameter = dbCommand.CreateParameter();
                        dataParameter.ParameterName = this.DataAccess.ParameterToken + name;
                        dataParameter.DbType = this.EntityAdapter.KeyDbType[name];
                        dbCommand.Parameters.Add(dataParameter);
                    }
                }
                foreach (string name in list)
                {
                    sbColumns.Append(",");
                    sbColumns.Append(this.EntityAdapter.KeyMap[name]);
                    sbColumns.Append("=" + this.DataAccess.ParameterToken);
                    sbColumns.Append(name);
                    IDataParameter dataParameter = dbCommand.CreateParameter();
                    dataParameter.ParameterName = this.DataAccess.ParameterToken + name;
                    dataParameter.DbType = this.EntityAdapter.KeyDbType[name];
                    dbCommand.Parameters.Add(dataParameter);
                }

                sbSqlText.Append(sbColumns.ToString().Substring(1));
                sbSqlText.Append(" WHERE ");
                sbSqlText.Append(sbWhere.ToString().Substring(4));
                dbCommand.CommandText = sbSqlText.ToString();
                dbCommand.CommandType = CommandType.Text;
            }

            PropertyInfo propertyInfo = null;
            foreach (IDataParameter dataParameter in dbCommand.Parameters)
            {
                propertyInfo = t.GetType().GetProperty(dataParameter.ParameterName.Substring(1));
                object value = propertyInfo.GetValue(t, null);
                dataParameter.Value = value == null ? DBNull.Value : value;
            }
            if (t is ORMEntity)
            {
                ORMEntity dataEntity = t as ORMEntity;
                dataEntity.ClearChangedProperties();
            }

            if (dbCommand != null && this.DataAccess.DbTransaction != null)
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
            if (t == null)
                throw new ArgumentNullException("t");
            IDbCommand dbCommand = this.GetCacheDeleteCommand();

            PropertyInfo propertyInfo = null;
            foreach (IDataParameter dataParameter in dbCommand.Parameters)
            {
                propertyInfo = t.GetType().GetProperty(dataParameter.ParameterName.Substring(1));
                object value = propertyInfo.GetValue(t, null);
                dataParameter.Value = value == null ? DBNull.Value : value;
            }
            if (this.DataAccess.DbTransaction != null)
            {
                dbCommand.Transaction = this.DataAccess.DbTransaction;
            }
            return dbCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="whereString"></param>
        /// <param name="orderString"></param>
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
            return DataConverter.ToORMEntityList<T>(dataSet, this.EntityAdapter.TableName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="whereString"></param>
        /// <param name="orderString"></param>
        /// <param name="pageStart"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [Obsolete]
        public override DataSet QueryDataSetByPage(string whereString, string orderString, int pageStart, int pageSize)
        {
            Type classType = typeof(T);
            ORMTableAttribute tableAttritube = (ORMTableAttribute)classType.GetCustomAttributes(typeof(ORMTableAttribute), false)[0];

            IDbCommand dbCommand = this.EntityAdapter.CreateDataSetPage(this.DataAccess, whereString, orderString, pageStart, pageSize);
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
        /// <param name="pageStart"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [Obsolete]
        public override List<T> QueryListByPage(string whereString, string orderString, int pageStart, int pageSize)
        {
            DataSet dataSet = QueryDataSetByPage(whereString, orderString, pageStart, pageSize);
            return DataConverter.ToORMEntityList<T>(dataSet, this.EntityAdapter.TableName);
        }

    }
}