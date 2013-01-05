using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;

using Explorer.Framework.Data.DataAccess;
using Explorer.Framework.Data.ORM;
using Explorer.Framework.Data.XRM;

namespace Explorer.Framework.Data.EntityAccess
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class EntityAdapter : MarshalByRefObject, IDisposable
    {
        #region 私有变量
        /// <summary>
        /// 属性名称映射表
        /// </summary>
        private Dictionary<string, FieldAdapter> _mapPropteryName;

        /// <summary>
        /// 数据列名称映射表
        /// </summary>
        private Dictionary<string, FieldAdapter> _mapColumnName;

        /// <summary>
        /// 关键字列表
        /// </summary>
        private Dictionary<string, string> _keyMap;

        /// <summary>
        /// 关键字数据类型映射表
        /// </summary>
        private Dictionary<string, DbType> _keyDbType;
        #endregion

        #region 构造方法 & 初始化
        /// <summary>
        /// 
        /// </summary>
        internal EntityAdapter(Uri uri)
        {
            Initialize();
            DataSet dataSet = new DataSet();
            dataSet.ReadXml(uri.ToString());

            Intitalize(dataSet);
        }

        internal EntityAdapter(Stream stream)
        {
            Initialize();
            DataSet dataSet = new DataSet();
            dataSet.ReadXml(stream);

            Intitalize(dataSet);
        }

        internal EntityAdapter(Type classType)
        {
            Initialize();
            this.EntityName = classType.Name;
            this.FullName = classType.FullName;

            ORMTableAttribute[] tableAttributes = (ORMTableAttribute[])classType.GetCustomAttributes(typeof(ORMTableAttribute), false);
            if (tableAttributes != null && tableAttributes.Length > 0)
            {
                this.TableName = tableAttributes[0].Name;

                StringBuilder sbAf = new StringBuilder();
                StringBuilder sbId = new StringBuilder();
                StringBuilder sbPk = new StringBuilder();
                StringBuilder sbNames = new StringBuilder();
                FieldAdapter fieldAdapter;
                foreach (PropertyInfo propertyInfo in classType.GetProperties())
                {
                    fieldAdapter = new FieldAdapter();
                    ORMColumnAttribute[] columnAttributes = (ORMColumnAttribute[])propertyInfo.GetCustomAttributes(typeof(ORMColumnAttribute), false);
                    if (columnAttributes.Length == 0)
                    {
                        continue;
                    }
                    ORMColumnAttribute columnAttribute = columnAttributes[0];
                    if (columnAttribute.IsAutoFill)
                    {
                        fieldAdapter.IsAutoFill = true;
                        sbAf.Append(",");
                        sbAf.Append(propertyInfo.Name);
                    }
                    if (columnAttribute.IsIdentity)
                    {
                        fieldAdapter.IsIdentity = true;
                        sbId.Append(",");
                        sbId.Append(propertyInfo.Name);
                    }
                    if (columnAttribute.IsPrimaryKey)
                    {
                        fieldAdapter.IsPrimaryKey = true;
                        sbPk.Append(",");
                        sbPk.Append(propertyInfo.Name);
                    }
                    sbNames.Append(",");
                    sbNames.Append(propertyInfo.Name);

                    fieldAdapter.Name = propertyInfo.Name;
                    fieldAdapter.PropertyTypeName = propertyInfo.PropertyType.FullName;
                    fieldAdapter.PropertyType = propertyInfo.PropertyType;

                    fieldAdapter.ColumnName = columnAttribute.Name;
                    fieldAdapter.DbType = columnAttribute.DbType;
                    fieldAdapter.DbTypeName = columnAttribute.DbType.GetType().FullName;

                    this._keyMap.Add(propertyInfo.Name, columnAttribute.Name);
                    this._keyDbType.Add(propertyInfo.Name, columnAttribute.DbType);

                    this._mapColumnName.Add(fieldAdapter.ColumnName, fieldAdapter);
                    this._mapPropteryName.Add(fieldAdapter.Name, fieldAdapter);
                }
                this._keyMap.Add("$Af", sbAf.ToString()); // 找出所有 AutoFill(数据库自动填入项)
                this._keyMap.Add("$Id", sbId.ToString()); // 找出标记为 Id 的项
                this._keyMap.Add("$Pk", sbPk.ToString()); // 找出标记为 主键 的项
                this._keyMap.Add("$Names", sbNames.ToString()); //所有字段名
            }
        }

        private void Intitalize(DataSet dataSet)
        {
            DataTable table = dataSet.Tables["Entity"];
            this.TableName = table.Rows[0]["Table"].ToString();
            this.EntityName = table.Rows[0]["Name"].ToString();
            this.FullName = table.Rows[0]["NameSpace"].ToString() + "." + this.EntityName;

            StringBuilder sbAf = new StringBuilder();
            StringBuilder sbId = new StringBuilder();
            StringBuilder sbPk = new StringBuilder();
            StringBuilder sbNames = new StringBuilder();
            FieldAdapter fieldAdapter;

            table = dataSet.Tables["Proptery"];
            foreach (DataRow row in table.Rows)
            {
                fieldAdapter = new FieldAdapter();

                if (row.Table.Columns.Contains("IsAutoFill"))
                {
                    if ("true".Equals(row["IsAutoFill"].ToString().ToLower()))
                    {
                        fieldAdapter.IsAutoFill = true;
                        sbAf.Append(",");
                        sbAf.Append(row["Name"].ToString());
                    }
                }
                if (row.Table.Columns.Contains("IsIdentity"))
                {
                    if ("true".Equals(row["IsIdentity"].ToString().ToLower()))
                    {
                        fieldAdapter.IsIdentity = true;
                        sbId.Append(",");
                        sbId.Append(row["Name"].ToString());
                    }
                }
                if (row.Table.Columns.Contains("IsPrimaryKey"))
                {
                    if ("true".Equals(row["IsPrimaryKey"].ToString().ToLower()))
                    {
                        fieldAdapter.IsPrimaryKey = true;
                        sbPk.Append(",");
                        sbPk.Append(row["Name"].ToString());
                    }
                }
                sbNames.Append(",");
                sbNames.Append(row["Name"].ToString());

                fieldAdapter.Name = row["Name"].ToString();
                fieldAdapter.PropertyTypeName = row["PropteryType"].ToString();
                fieldAdapter.PropertyType = Type.GetType(fieldAdapter.PropertyTypeName);

                fieldAdapter.ColumnName = row["Column"].ToString();
                fieldAdapter.DbTypeName = row["DbType"].ToString();
                string dbTypeName = fieldAdapter.DbTypeName.Substring(fieldAdapter.DbTypeName.LastIndexOf('.') + 1);
                fieldAdapter.DbType = (DbType)Enum.Parse(typeof(DbType), dbTypeName);

                this._keyMap.Add(row["Name"].ToString(), row["Column"].ToString());
                this._keyDbType.Add(row["Name"].ToString(), fieldAdapter.DbType);

                this._mapColumnName.Add(fieldAdapter.ColumnName, fieldAdapter);
                this._mapPropteryName.Add(fieldAdapter.Name, fieldAdapter);
            }

            this._keyMap.Add("$Af", sbAf.ToString()); // 找出所有 AutoFill(数据库自动填入项)
            this._keyMap.Add("$Id", sbId.ToString()); // 找出标记为 Id 的项
            this._keyMap.Add("$Pk", sbPk.ToString()); // 找出标记为 主键 的项
            this._keyMap.Add("$Names", sbNames.ToString()); //所有字段名

            dataSet.Dispose();
        }

        private void Initialize()
        {
            _mapPropteryName = new Dictionary<string, FieldAdapter>();
            _mapColumnName = new Dictionary<string, FieldAdapter>();
            _keyMap = new Dictionary<string, string>();
            _keyDbType = new Dictionary<string, DbType>();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 数据表名称
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 实体名称
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// 完整名称
        /// </summary>
        public string FullName { get; set; }
        #endregion

        #region SQL命令创建方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataAccess"></param>
        /// <returns></returns>
        public IDbCommand CreateLoadByPk(IDataAccess dataAccess)
        {
            IDbCommand dbCommand;

            dbCommand = dataAccess.DbConnection.CreateCommand();
            dbCommand.CommandType = CommandType.Text;

            StringBuilder sbColumns = new StringBuilder();
            string[] nameArray = this._keyMap["$Pk"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string name in nameArray)
            {
                sbColumns.Append(" AND ");
                sbColumns.Append(this._keyMap[name]);
                sbColumns.Append("=" + dataAccess.ParameterToken);
                sbColumns.Append(name);
                IDataParameter dataParameter = dbCommand.CreateParameter();
                dataParameter.ParameterName = dataAccess.ParameterToken + name;
                dataParameter.DbType = this._keyDbType[name];
                dbCommand.Parameters.Add(dataParameter);
            }
            dbCommand.CommandText = String.Format("SELECT * FROM {0} WHERE {1}", this.TableName, sbColumns.ToString().Substring(4));
            sbColumns.Remove(0, sbColumns.Length);

            return dbCommand;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataAccess"></param>
        /// <returns></returns>
        public IDbCommand CreateLoadById(IDataAccess dataAccess)
        {
            IDbCommand dbCommand;

            dbCommand = dataAccess.DbConnection.CreateCommand();
            dbCommand.CommandType = CommandType.Text;

            StringBuilder sbColumns = new StringBuilder();
            string[] nameArray = this._keyMap["$Id"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string name in nameArray)
            {
                sbColumns.Append(" AND ");
                sbColumns.Append(this._keyMap[name]);
                sbColumns.Append("=" + dataAccess.ParameterToken);
                sbColumns.Append(name);
                IDataParameter dataParameter = dbCommand.CreateParameter();
                dataParameter.ParameterName = dataAccess.ParameterToken + name;
                dataParameter.DbType = this._keyDbType[name];
                dbCommand.Parameters.Add(dataParameter);
            }
            dbCommand.CommandText = String.Format("SELECT * FROM {0} WHERE {1}", this.TableName, sbColumns.ToString().Substring(4));
            sbColumns.Remove(0, sbColumns.Length);

            return dbCommand;
        }

        /// <summary>
        /// 创建 Insert 命令
        /// </summary>
        /// <param name="dataAccess"></param>
        /// <returns></returns>
        public IDbCommand CreateInsert(IDataAccess dataAccess)
        {
            IDbCommand dbCommand;

            dbCommand = dataAccess.DbConnection.CreateCommand();
            dbCommand.CommandType = CommandType.Text;

            string[] nameArray;
            string names = null;
            StringBuilder sbColumns = new StringBuilder();
            StringBuilder sbNames = new StringBuilder();

            nameArray = this._keyMap["$Af"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (nameArray.Length == 0)
            {
                names = this._keyMap["$Names"];
            }
            else
            {
                foreach (string name in nameArray)
                {
                    names = this._keyMap["$Names"].Replace("," + name, "");
                }
            }
            nameArray = names.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            sbColumns.Remove(0, sbColumns.Length);
            foreach (string name in nameArray)
            {
                sbColumns.Append(",");
                sbColumns.Append(this._keyMap[name]);
                IDataParameter dataParameter = dbCommand.CreateParameter();
                dataParameter.ParameterName = dataAccess.ParameterToken + name;
                dataParameter.DbType = this._keyDbType[name];
                dbCommand.Parameters.Add(dataParameter);
            }
            dbCommand.CommandText = String.Format("INSERT INTO {0} ({1}) VALUES ({2})", this.TableName, sbColumns.ToString().Substring(1), names.Replace(",", "," + dataAccess.ParameterToken).Substring(1));
            sbColumns.Remove(0, sbColumns.Length);

            return dbCommand;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataAccess"></param>
        /// <returns></returns>
        public IDbCommand CreateUpdateByPk(IDataAccess dataAccess)
        {
            IDbCommand dbCommand;

            dbCommand = dataAccess.DbConnection.CreateCommand();
            dbCommand.CommandType = CommandType.Text;

            string names = null;
            StringBuilder sbColumns = new StringBuilder();
            StringBuilder sbNames = new StringBuilder();
            string[] nameArray = this._keyMap["$Af"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string name in nameArray)
            {
                names = this._keyMap["$Names"].Replace("," + name, "");
            }
            nameArray = this._keyMap["$Pk"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string name in nameArray)
            {
                names = this._keyMap["$Names"].Replace("," + name, "");
            }
            nameArray = names.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            sbColumns.Remove(0, sbColumns.Length);
            foreach (string name in nameArray)
            {
                sbColumns.Append(",");
                sbColumns.Append(this._keyMap[name]);
                sbColumns.Append("=" + dataAccess.ParameterToken);
                sbColumns.Append(name);
                IDataParameter dataParameter = dbCommand.CreateParameter();
                dataParameter.ParameterName = dataAccess.ParameterToken + name;
                dataParameter.DbType = this._keyDbType[name];
                dbCommand.Parameters.Add(dataParameter);
            }
            names = this._keyMap["$Pk"];
            nameArray = names.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string name in nameArray)
            {
                sbNames.Append(" AND ");
                sbNames.Append(this._keyMap[name]);
                sbNames.Append("=" + dataAccess.ParameterToken);
                sbNames.Append(name);
                IDataParameter dataParameter = dbCommand.CreateParameter();
                dataParameter.ParameterName = dataAccess.ParameterToken + name;
                dataParameter.DbType = this._keyDbType[name];
                dbCommand.Parameters.Add(dataParameter);
            }
            dbCommand.CommandText = String.Format("UPDATE {0} SET {1} WHERE {2}", this.TableName, sbColumns.ToString().Substring(1), sbNames.ToString().Substring(4));
            sbColumns.Remove(0, sbColumns.Length);
            sbNames.Remove(0, sbNames.Length);

            return dbCommand;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataAccess"></param>
        /// <returns></returns>
        public IDbCommand CreateUpdateById(IDataAccess dataAccess)
        {
            IDbCommand dbCommand;

            dbCommand = dataAccess.DbConnection.CreateCommand();
            dbCommand.CommandType = CommandType.Text;

            StringBuilder sbColumns = new StringBuilder();
            StringBuilder sbNames = new StringBuilder();
            string names = null;
            string[] nameArray = this._keyMap["$Af"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string name in nameArray)
            {
                names = this._keyMap["$Names"].Replace("," + name, "");
            }
            nameArray = this._keyMap["$Id"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string name in nameArray)
            {
                names = this._keyMap["$Names"].Replace("," + name, "");
            }
            nameArray = names.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            sbColumns.Remove(0, sbColumns.Length);
            foreach (string name in nameArray)
            {
                sbColumns.Append(",");
                sbColumns.Append(this._keyMap[name]);
                sbColumns.Append("=" + dataAccess.ParameterToken);
                sbColumns.Append(name);
                IDataParameter dataParameter = dbCommand.CreateParameter();
                dataParameter.ParameterName = dataAccess.ParameterToken + name;
                dataParameter.DbType = this._keyDbType[name];
                dbCommand.Parameters.Add(dataParameter);
            }
            names = this._keyMap["$Id"];
            nameArray = names.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string name in nameArray)
            {
                sbNames.Append(" AND ");
                sbNames.Append(this._keyMap[name]);
                sbNames.Append("=" + dataAccess.ParameterToken);
                sbNames.Append(name);
                IDataParameter dataParameter = dbCommand.CreateParameter();
                dataParameter.ParameterName = dataAccess.ParameterToken + name;
                dataParameter.DbType = this._keyDbType[name];
                dbCommand.Parameters.Add(dataParameter);

            }
            dbCommand.CommandText = String.Format("UPDATE {0} SET {1} WHERE {2}", this.TableName, sbColumns.ToString().Substring(1), sbNames.ToString().Substring(4));
            sbColumns.Remove(0, sbColumns.Length);
            sbNames.Remove(0, sbNames.Length);

            return dbCommand;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataAccess"></param>
        /// <returns></returns>
        public IDbCommand CreateDeleteByPk(IDataAccess dataAccess)
        {
            IDbCommand dbCommand;

            dbCommand = dataAccess.DbConnection.CreateCommand();
            dbCommand.CommandType = CommandType.Text;

            StringBuilder sbColumns = new StringBuilder();
            string[] nameArray = this._keyMap["$Pk"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string name in nameArray)
            {
                sbColumns.Append(" AND ");
                sbColumns.Append(this._keyMap[name]);
                sbColumns.Append("=" + dataAccess.ParameterToken);
                sbColumns.Append(name);
                IDataParameter dataParameter = dbCommand.CreateParameter();
                dataParameter.ParameterName = dataAccess.ParameterToken + name;
                dataParameter.DbType = this._keyDbType[name];
                dbCommand.Parameters.Add(dataParameter);
            }
            dbCommand.CommandText = String.Format("DELETE FROM {0} WHERE {1}", this.TableName, sbColumns.ToString().Substring(4));
            sbColumns.Remove(0, sbColumns.Length);

            return dbCommand;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataAccess"></param>
        /// <returns></returns>
        public IDbCommand CreateDeleteById(IDataAccess dataAccess)
        {
            IDbCommand dbCommand;

            dbCommand = dataAccess.DbConnection.CreateCommand();
            dbCommand.CommandType = CommandType.Text;

            StringBuilder sbColumns = new StringBuilder();
            string[] nameArray = this._keyMap["$Id"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string name in nameArray)
            {
                sbColumns.Append(" AND ");
                sbColumns.Append(this._keyMap[name]);
                sbColumns.Append("=" + dataAccess.ParameterToken);
                sbColumns.Append(name);
                IDataParameter dataParameter = dbCommand.CreateParameter();
                dataParameter.ParameterName = dataAccess.ParameterToken + name;
                dataParameter.DbType = this._keyDbType[name];
                dbCommand.Parameters.Add(dataParameter);
            }
            dbCommand.CommandText = String.Format("DELETE FROM {0} WHERE {1}", this.TableName, sbColumns.ToString().Substring(4));
            sbColumns.Remove(0, sbColumns.Length);

            return dbCommand;
        }

        /// <summary>
        /// 按照分页查询数据
        /// </summary>
        /// <param name="dataAccess">数据访问器</param>
        /// <param name="whereString">查询条件</param>
        /// <param name="orderString">排序或分组条件</param>
        /// <param name="pageStart">查询页编号</param>
        /// <param name="pageSize">每页数据量</param>
        /// <returns></returns>
        public IDbCommand CreateDataSetPage(IDataAccess dataAccess, string whereString, string orderString, int pageStart, int pageSize)
        {
            string strOrderby = string.Empty;
            string strWhere = string.Empty;
            int index = -1;
            if (!string.IsNullOrEmpty(orderString))
            {
                index = orderString.ToUpper().IndexOf("ORDER BY");
                strOrderby = "ORDER BY " + (index == -1 ? orderString.Trim() : orderString.Substring(index + 9).Trim());
            }
            if (!string.IsNullOrEmpty(whereString))
            {
                index = whereString.ToUpper().IndexOf("WHERE");
                strWhere = "WHERE " + (index == -1 ? whereString.Trim() : whereString.Substring(index + 6).Trim());
            }

            IDbCommand dbCommand = dataAccess.CreateCommand();
            dbCommand.CommandType = CommandType.Text;
            string sqlText = null;

            long rowStart = pageStart < 1 ? 1 : (pageStart - 1) * pageSize + 1;
            long rowEnd = rowStart + pageSize;
            switch (dataAccess.DataBaseType)
            {
                case DataBaseType.Sql:
                    if (rowStart <= 1L)
                    {
                        //sqlText = string.Format("SELECT TOP {0} {1} FROM {2} WITH(NOLOCK){3} ORDER BY {4}",
                        sqlText = string.Format("SELECT TOP {0} {1} FROM {2} {3} {4}",
                         new object[] { rowEnd, "*", this.TableName, strWhere, strOrderby });
                    }
                    else
                    {
                        sqlText = string.Format("WITH temptable AS (SELECT ROW_NUMBER() OVER({0}) AS 'RowNo',{1} FROM {2} {3}) SELECT * FROM temptable WHERE RowNo BETWEEN {4} AND {5}",
                            new object[] { strOrderby, "*", this.TableName, strWhere, rowStart, rowEnd });
                    }
                    break;
                case DataBaseType.Oracle:
                case DataBaseType.Odac:
                    strWhere = "AND" + strWhere.Substring(5);
                    sqlText = string.Format("SELECT * FROM (SELECT a.*, ROWNUM rn FROM (SELECT * FROM {0} {1}) a WHERE ROWNUM <= {2} {3}) WHERE rn >= {4}",
                        new object[] { this.TableName, strOrderby, rowEnd, strWhere, rowStart });
                    break;
                case DataBaseType.OleDb:
                    throw new Exception("没有设定");
            }

            dbCommand.CommandText = sqlText;
            return dbCommand;
        }

        /// <summary>
        /// 按照分页查询数据
        /// </summary>
        /// <param name="dataAccess">数据访问器</param>
        /// <param name="dbTableName">数据表名称</param>
        /// <param name="whereString">查询条件</param>
        /// <param name="orderString">排序或分组条件</param>
        /// <param name="pageStart">查询页编号</param>
        /// <param name="pageSize">每页数据量</param>
        /// <returns></returns>
        public static IDbCommand CreateDataSetPage(IDataAccess dataAccess, string dbTableName, string whereString, string orderString, int pageStart, int pageSize)
        {
            string strOrderby = string.Empty;
            string strWhere = string.Empty;
            int index = -1;
            if (string.IsNullOrEmpty(orderString))
            {
                index = orderString.ToUpper().IndexOf("ORDER BY");
                strOrderby = "ORDER BY " + (index == -1 ? orderString.Trim() : orderString.Substring(index + 9).Trim());
            }
            if (string.IsNullOrEmpty(whereString))
            {
                index = whereString.ToUpper().IndexOf("WHERE");
                strWhere = "WHERE " + (index == -1 ? whereString.Trim() : whereString.Substring(index + 6).Trim());
            }

            IDbCommand dbCommand = dataAccess.CreateCommand();
            dbCommand.CommandType = CommandType.Text;
            string sqlText = null;

            long rowStart = pageStart < 1 ? 1 : (pageStart - 1) * pageSize + 1;
            long rowEnd = rowStart + pageSize;
            switch (dataAccess.DataBaseType)
            {
                case DataBaseType.Sql:
                    if (rowStart <= 1L)
                    {
                        //sqlText = string.Format("SELECT TOP {0} {1} FROM {2} WITH(NOLOCK){3} ORDER BY {4}",
                        sqlText = string.Format("SELECT TOP {0} {1} FROM {2} {3} {4}",
                         new object[] { rowEnd, "*", dbTableName, strWhere, strOrderby });
                    }
                    else
                    {
                        sqlText = string.Format("WITH temptable AS (SELECT ROW_NUMBER() OVER({0}) AS 'RowNo',{1} FROM {2} {3}) SELECT * FROM temptable WHERE RowNo BETWEEN {4} AND {5}",
                            new object[] { strOrderby, "*", dbTableName, strWhere, rowStart, rowEnd });
                    }
                    break;
                case DataBaseType.Oracle:
                case DataBaseType.Odac:
                    strWhere = "AND" + strWhere.Substring(5);
                    sqlText = string.Format("SELECT * FROM (SELECT a.*, ROWNUM rn FROM (SELECT * FROM {0} {1}) a WHERE ROWNUM <= {2} {3}) WHERE rn >= {4}",
                        new object[] { dbTableName, strOrderby, rowEnd, strWhere, rowStart });
                    break;
                case DataBaseType.OleDb:
                    throw new Exception("没有设定");
            }

            dbCommand.CommandText = sqlText;
            return dbCommand;
        }
        #endregion

        #region 基本的访问器
        /// <summary>
        /// 
        /// </summary>
        internal Dictionary<string, string> KeyMap
        {
            get
            {
                return this._keyMap;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal Dictionary<string, DbType> KeyDbType
        {
            get
            {
                return this._keyDbType;
            }
        }

        /// <summary>
        /// 通过属性名称拿出字段适配器
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public FieldAdapter GetFieldByPropteryName(string name)
        {
            if (this._mapPropteryName.ContainsKey(name))
            {
                return this._mapPropteryName[name];
            }
            return null;
        }

        /// <summary>
        /// 通过数据列名称拿出字段适配器
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public FieldAdapter GetFieldByColumnName(string name)
        {
            if (this._mapColumnName.ContainsKey(name))
            {
                return this._mapColumnName[name];
            }
            return null;
        }

        /// <summary>
        /// 获取所有的字段适配器
        /// </summary>
        /// <returns></returns>
        public FieldAdapter[] GetFields()
        {
            return (FieldAdapter[])this._mapColumnName.Values.ToArray();
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            _mapPropteryName.Clear();
            _mapColumnName.Clear();

            _keyMap.Clear();
            _keyDbType.Clear();

            this._mapPropteryName = null;
            this._mapColumnName = null;

            this._keyMap = null;
            this._keyDbType = null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class FieldAdapter : MarshalByRefObject
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PropertyTypeName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Type PropertyType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DbTypeName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DbType DbType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Size { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsPrimaryKey { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsIdentity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsAutoFill { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public FieldAdapter()
        {
            this.IsAutoFill = false;
            this.IsIdentity = false;
            this.IsPrimaryKey = false;
        }
    }
}
