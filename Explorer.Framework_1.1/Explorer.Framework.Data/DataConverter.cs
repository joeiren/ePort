using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;

using Explorer.Framework.Data.ORM;
using Explorer.Framework.Data.XRM;
using Explorer.Framework.Data.EntityAccess;

namespace Explorer.Framework.Data
{
    /// <summary>
    /// 数据对象转换器
    /// </summary>
    public class DataConverter
    {
        /// <summary>
        /// 将一个 DataRow 转换成一个 DataEntity
        /// </summary>
        /// <typeparam name="T">转出的泛型对象</typeparam>
        /// <param name="row">需要转换的 DataRow 对象</param>
        /// <returns></returns>
        public static T ToORMEntity<T>(DataRow row) where T : new()
        {
            Type type = typeof(T);
            T t = ToORMEntity<T>(row, IsRM(type, row.Table.TableName));
            return t;
        }


        /// <summary>
        /// 将一个 Hashtable 转换成一个 DataEntity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashtable"></param>
        /// <returns></returns>
        public static T ToORMEntity<T>(Hashtable hashtable) where T : new()
        {
            T t = new T();
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                object value = hashtable[propertyInfo.Name];
                if (value != null)
                {
                    propertyInfo.SetValue(t, Convert.ChangeType(value, propertyInfo.PropertyType), null);
                }
            }
            return t;
        }

        /// <summary>
        /// 将一个 IDataReader 的当前行转换成一个 DataEntity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        public static T ToORMEntity<T>(IDataReader dataReader) where T : new()
        {
            T t = new T();
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                ORMColumnAttribute[] columnAttributes = (ORMColumnAttribute[])propertyInfo.GetCustomAttributes(typeof(ORMColumnAttribute), false);
                if (columnAttributes.Length > 0)
                {
                    object value = dataReader[columnAttributes[0].Name];
                    if (value != null && value != System.DBNull.Value)
                    {
                        Type propertyType;
                        if (propertyInfo.PropertyType.Name.StartsWith("Nullable"))
                        {
                            propertyType = propertyInfo.PropertyType.GetGenericArguments()[0];
                        }
                        else
                        {
                            propertyType = propertyInfo.PropertyType;
                        }
                        propertyInfo.SetValue(t, Convert.ChangeType(value, propertyType), null);

                        //propertyInfo.SetValue(t, Convert.ChangeType(value, propertyInfo.PropertyType), null);
                    }
                }
                else
                {
                    object value = dataReader[propertyInfo.Name];
                    if (value != null && value != System.DBNull.Value)
                    {
                        Type propertyType;
                        if (propertyInfo.PropertyType.Name.StartsWith("Nullable"))
                        {
                            propertyType = propertyInfo.PropertyType.GetGenericArguments()[0];
                        }
                        else
                        {
                            propertyType = propertyInfo.PropertyType;
                        }
                        propertyInfo.SetValue(t, Convert.ChangeType(value, propertyType), null);
                        //propertyInfo.SetValue(t, Convert.ChangeType(value, propertyInfo.PropertyType), null);
                    }
                }
            }
            return t;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        public static List<T> ToORMEntityList<T>(IDataReader dataReader) where T : new()
        {
            List<T> list = new List<T>();
            while (dataReader.Read())
            {
                T t = ToORMEntity<T>(dataReader);
                list.Add(t);
            }
            return list;
        }


        /// <summary>
        /// 将一个 DataTable 里的数据项转成 DataEntity 的集合
        /// </summary>
        /// <typeparam name="T">转出的泛型对象</typeparam>
        /// <param name="table">需要转换的 DataTable 对象</param>
        /// <returns></returns>
        public static List<T> ToORMEntityList<T>(DataTable table) where T : new()
        {
            List<T> list = new List<T>();
            bool isORM = IsRM(typeof(T), table.TableName);
            foreach (DataRow row in table.Rows)
            {
                T t = ToORMEntity<T>(row, isORM);
                list.Add(t);
            }
            return list;
        }

        /// <summary>
        /// 将一个 DataTable 里的数据项转成 DataEntity 的集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="isORM"></param>
        /// <returns></returns>
        public static List<T> ToORMEntityList<T>(DataTable table, bool isORM) where T : new()
        {
            List<T> list = new List<T>();
            foreach (DataRow row in table.Rows)
            {
                T t = ToORMEntity<T>(row, isORM);
                list.Add(t);
            }
            return list;
        }


        /// <summary>
        /// 将一 DataSet 里指定 DataTable 的数据项转成 DataEntity 的集合
        /// </summary>
        /// <typeparam name="T">转出的泛型对象</typeparam>
        /// <param name="dataSet">需要转换的 DataSet 对象</param>
        /// <param name="tableName">需要转换的 DataTable 的 名称</param>
        /// <returns></returns>
        public static List<T> ToORMEntityList<T>(DataSet dataSet, string tableName) where T : new()
        {
            return ToORMEntityList<T>(dataSet, tableName, string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="entityAdapter"></param>
        /// <returns></returns>
        public static T ToXRMEntity<T>(DataRow row, EntityAdapter entityAdapter) where T : new()
        {
            return ToXRMEntity<T>(row, IsRM(entityAdapter.EntityName, entityAdapter.TableName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="isRM"></param>
        /// <returns></returns>
        public static T ToXRMEntity<T>(DataRow row, bool isRM) where T : new()
        {
            EntityAdapter entityAdapter;
            string entityName;
            if (isRM)
            {
                entityAdapter = EntityAdapterManager.Instance.GetAdapterByTableName(row.Table.TableName);
                if (entityAdapter != null)
                {
                    entityName = entityAdapter.EntityName;
                }
                else
                {
                    entityAdapter = EntityAdapterManager.Instance.GetAdapterByFullName(row.Table.TableName);
                    entityName = row.Table.TableName;
                    isRM = false;
                }
            }
            else
            {
                entityAdapter = EntityAdapterManager.Instance.GetAdapterByFullName(row.Table.TableName);
                entityName = row.Table.TableName;
            }

            return ToXRMEntity<T>(row, entityAdapter, isRM);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="entityAdapter"></param>
        /// <param name="isRM"></param>
        /// <returns></returns>
        public static T ToXRMEntity<T>(DataRow row, EntityAdapter entityAdapter, bool isRM) where T : new()
        {
            T t = new T();
            XRMEntity xrmEntity = t as XRMEntity;
            xrmEntity.EntityAdapter = entityAdapter;
            foreach (FieldAdapter field in entityAdapter.GetFields())
            {
                if (isRM)
                {
                    if (row.Table.Columns.Contains(field.ColumnName))
                    {
                        object value = row[field.ColumnName];
                        if (value != null || DBNull.Value != value)
                        {
                            xrmEntity.Items.Add(field.Name, value);
                        }
                    }
                }
                else
                {
                    if (row.Table.Columns.Contains(field.Name))
                    {
                        object value = row[field.Name];
                        if (value != null || DBNull.Value != value)
                        {
                            xrmEntity.Items.Add(field.Name, value);
                        }
                    }
                }

            }
            return t;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataSet"></param>
        /// <param name="entityAdapter"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static List<T> ToXRMEntityList<T>(DataSet dataSet, EntityAdapter entityAdapter, string tableName) where T : new() 
        {
            return ToXRMEntityList<T>(dataSet, tableName, entityAdapter, string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataSet"></param>
        /// <param name="tableName"></param>
        /// <param name="entityAdapter"></param>
        /// <param name="tableNamespace"></param>
        /// <returns></returns>
        public static List<T> ToXRMEntityList<T>(DataSet dataSet, string tableName, EntityAdapter entityAdapter, string tableNamespace) where T : new()
        {
            bool isTableExist = false;
            List<T> list = new List<T>();
            if (string.IsNullOrEmpty(tableNamespace))
            {
                isTableExist = dataSet.Tables.Contains(tableName);
            }
            else
            {
                isTableExist = dataSet.Tables.Contains(tableName, tableNamespace);
            }
            if (isTableExist)
            {
                for (int i = 0; i < dataSet.Tables[tableName].Rows.Count; i++)
                {
                    T xrmEntity = ToXRMEntity<T>(dataSet.Tables[tableName].Rows[i], entityAdapter, IsRM(entityAdapter.EntityName, entityAdapter.TableName));
                    list.Add(xrmEntity);
                }
            }
            return list;
        }


        /// <summary>
        /// 将一 DataSet 里指定 DataTable 的数据项转成 DataEntity 的集合
        /// </summary>
        /// <typeparam name="T">转出的泛型对象</typeparam>
        /// <param name="dataSet">需要转换的 DataSet 对象</param>
        /// <param name="tableName">需要转换的 DataTable 的 名称</param>
        /// <param name="tableNamespace">需要转换的 DataTable 的 命名空间</param>
        /// <returns></returns>
        public static List<T> ToORMEntityList<T>(DataSet dataSet, string tableName, string tableNamespace) where T : new()
        {
            bool isTableExist = false;
            bool isRM = IsRM(typeof(T), tableName);
            List<T> list = new List<T>();
            if (string.IsNullOrEmpty(tableNamespace))
            {
                isTableExist = dataSet.Tables.Contains(tableName);
            }
            else
            {
                isTableExist = dataSet.Tables.Contains(tableName, tableNamespace);
            }
            if (isTableExist)
            {
                foreach (DataRow row in dataSet.Tables[tableName].Rows)
                {
                    T t = ToORMEntity<T>(row, isRM);
                    list.Add(t);
                }
            }
            return list;
        }


        /// <summary>
        ///  将一个 DataEntity 填入一个新的 DataSet, 并指定 entity.getType().Name 为表名称
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static DataSet ToDataSet(object entity)
        {
            DataSet dataSet = new DataSet();
            FillDataSet(entity, ref dataSet);
            return dataSet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataSet ToDataSet<T>(IList<T> list)
        {
            return ToDataSet<T>(list, typeof(T).Name);
        }

        /// <summary>
        /// 将一个 DataEntity 填入一个新的 DataSet, 并指定填入的表名称
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static DataSet ToDataSet(object entity, string tableName)
        {
            DataSet dataSet = new DataSet();
            FillDataSet(entity, ref dataSet, tableName);
            return dataSet;
        }

        /// <summary>
        /// 将一组DataEntity对象列表 填入一个新的 DataSet, 并指定填入的表名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static DataSet ToDataSet<T>(IList<T> list, string tableName)
        {
            DataSet dataSet = new DataSet();
            FillDataSet<T>(list, ref dataSet, tableName);
            return dataSet;
        }


        /// <summary>
        /// 将 Hashtable 里的值存入指定表里
        /// </summary>
        /// <param name="hashtable"></param>
        /// <param name="dataSet"></param>
        /// <param name="tableName"></param>
        public static void FillDataSet(Hashtable hashtable, ref DataSet dataSet, string key)
        {
            DataTable table = dataSet.Tables["Hashtable"];
            if (table == null)
            {
                table = new DataTable("Hashtable");
                table.Columns.Add("Key");
                table.Columns.Add("Type");
                table.Columns.Add("Name");
                table.Columns.Add("Value");
                dataSet.Tables.Add(table);
            }
            
            foreach (string name in hashtable.Keys)
            {
                DataRow row = table.NewRow();
                row["Key"] = key;
                row["Type"] = hashtable[name].GetType();
                row["Name"] = name;
                row["Value"] = Convert.ChangeType(hashtable[name], typeof(String));
                table.Rows.Add(row);
            }            
        }

        /// <summary>
        /// 将 DataSet 里的值存入 Hashtable
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Hashtable ToHashtable(DataSet dataSet, string key)
        {
            Hashtable hashtable = new Hashtable();
            DataTable table = dataSet.Tables["Hashtable"];
            if (table != null)
            {
                DataRow[] rows = table.Select("Key='" + key + "'");
                foreach (DataRow row in rows)
                { 
                    object value = Convert.ChangeType(row["Value"], Type.GetType(row["Type"].ToString()));
                    hashtable.Add(row["Name"], value);                
                }
            }
            return hashtable;
        }


        /// <summary>
        /// 当 row 所属的表名和表名不一至时请将 isORM 置为 true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="isRM"></param>
        /// <returns></returns>
        public static T ToORMEntity<T>(DataRow row, bool isRM) where T : new()
        {
            T t = new T();
            Type type = typeof(T);

            PropertyInfo[] properties = type.GetProperties();
            if (isRM)
            {
                foreach (PropertyInfo propertyInfo in properties)
                {
                    object value = null;
                    ORMColumnAttribute[] columnAttributes = (ORMColumnAttribute[])propertyInfo.GetCustomAttributes(typeof(ORMColumnAttribute), false);
                    if (columnAttributes.Length > 0)
                    {
                        if (row.Table.Columns.Contains(columnAttributes[0].Name))
                        {
                            value = row[columnAttributes[0].Name];
                        }
                    }
                    else
                    {
                        if (row.Table.Columns.Contains(propertyInfo.Name))
                        {
                            value = row[propertyInfo.Name];
                        }
                    }
                    if (value != null && value != System.DBNull.Value)
                    {
                        Type propertyType;
                        if (propertyInfo.PropertyType.Name.StartsWith("Nullable"))
                        {
                            propertyType = propertyInfo.PropertyType.GetGenericArguments()[0];
                        }
                        else
                        {
                            propertyType = propertyInfo.PropertyType;
                        }
                        propertyInfo.SetValue(t, Convert.ChangeType(value, propertyType), null);
                    }
                }
            }
            else
            {
                foreach (PropertyInfo propertyInfo in properties)
                {
                    object value = row[propertyInfo.Name];
                    if (value != null && value != System.DBNull.Value)
                    {
                        Type propertyType;
                        if (propertyInfo.PropertyType.Name.StartsWith("Nullable"))
                        {
                            propertyType = propertyInfo.PropertyType.GetGenericArguments()[0];
                        }
                        else
                        {
                            propertyType = propertyInfo.PropertyType;
                        }
                        propertyInfo.SetValue(t, Convert.ChangeType(value, propertyType), null);
                    }
                }
            }
            return t;
        }

        /// <summary>
        /// 将一个 DataEntity 填入一个现有的 DataSet, 如果该 DataEntity 映射的 DataTable 已经存在, 则增加一条新记录, 否则创建一个 DataTable。
        /// </summary>
        /// <param name="sourceEntity"></param>
        /// <param name="targetDataSet"></param>
        public static void FillDataSet(object sourceEntity, ref DataSet targetDataSet)
        {
            FillDataSet(sourceEntity, ref targetDataSet, sourceEntity.GetType().Name);
        }


        /// <summary>
        /// 将一个 DataEntity 填入一个现有的 DataSet, 如果该 DataEntity 映射的 DataTable 已经存在, 则增加一条新记录, 否则创建一个 DataTable。
        /// </summary>
        /// <param name="targetDataSet"></param>
        /// <param name="sourceEntity"></param>
        /// <param name="tableName"></param>
        public static void FillDataSet(object sourceEntity, ref DataSet targetDataSet, string tableName)
        {
            if (sourceEntity.GetType().FullName.Equals("Explorer.Framework.Data.XRM.XRMEntity"))
            {
                XRMEntityFillDataSet(sourceEntity as XRMEntity, ref targetDataSet, tableName);
            }
            else
            {
                ORMEntityFillDataSet(sourceEntity as ORMEntity, ref targetDataSet, tableName);
            }
        }

        public static void XRMEntityFillDataSet(XRMEntity sourceEntity, ref DataSet targetDataSet, string tableName)
        {
            EntityAdapter entityAdapter = sourceEntity.EntityAdapter;
            bool isRM = IsRM(entityAdapter.EntityName, tableName);

            DataTable table = targetDataSet.Tables[tableName];
            if (table == null)
            {
                table = new DataTable(tableName);
                targetDataSet.Tables.Add(table);
                foreach (FieldAdapter fieldAdapter in entityAdapter.GetFields())
                {
                    if (isRM)
                    {
                        table.Columns.Add(fieldAdapter.ColumnName);
                    }
                    else
                    {
                        table.Columns.Add(fieldAdapter.Name);
                    }
                }
            }
            DataRow row = table.NewRow();
            foreach (FieldAdapter fieldAdapter in entityAdapter.GetFields())
            {
                if (isRM)
                {
                    object value = sourceEntity.Items[fieldAdapter.ColumnName];
                    if (value != null)
                    {
                        row[fieldAdapter.ColumnName] = value;
                    }
                }
                else
                {
                    object value = sourceEntity.Items[fieldAdapter.Name];
                    if (value != null)
                    {
                        row[fieldAdapter.Name] = value;
                    }
                }
            }
            table.Rows.Add(row);            
        }

        public static void ORMEntityFillDataSet(ORMEntity sourceEntity, ref DataSet targetDataSet, string tableName)
        {
            Type type = sourceEntity.GetType();
            bool isRM = IsRM(type, tableName);

            DataTable table = targetDataSet.Tables[tableName];
            if (table == null)
            {
                table = new DataTable(tableName);
                targetDataSet.Tables.Add(table);
                foreach (PropertyInfo pInfo in type.GetProperties())
                {
                    if (isRM)
                    {
                        ORMColumnAttribute ca = (ORMColumnAttribute)pInfo.GetCustomAttributes(typeof(ORMColumnAttribute), false)[0];
                        table.Columns.Add(ca.Name);
                    }
                    else
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
                if (isRM)
                {
                    ORMColumnAttribute ca = (ORMColumnAttribute)pInfo.GetCustomAttributes(typeof(ORMColumnAttribute), false)[0];
                    columnName = ca.Name;
                }
                if (table.Columns.Contains(columnName))
                {
                    object value = pInfo.GetValue(sourceEntity, null);
                    //  如果 value 不为空则加入 row 里面
                    if (value != null)
                    {
                        row[columnName] = value;
                    }
                }
            }
            table.Rows.Add(row);
        }
        
        /// <summary>
        /// 将一组DataEntity对象 填入一个现有的 DataSet, 如果该 DataEntity 映射的 DataTable 已经存在, 则增加一条新记录, 否则创建一个 DataTable。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceList"></param>
        /// <param name="targetDataSet"></param>
        public static void FillDataSet<T>(IList<T> sourceList, ref DataSet targetDataSet)
        {
            Type classType = typeof(T);
            if (classType.FullName.Equals("Explorer.Framework.Data.XRM.XRMEntity"))
            {
                XRMEntity entity = sourceList[0] as XRMEntity;
                EntityAdapter entityAdapter = entity.EntityAdapter;
                XRMEntityFillDataSet(sourceList, ref targetDataSet, entityAdapter.EntityName);
            }
            else
            {
                ORMEntityFillDataSet(sourceList, ref targetDataSet, typeof(T).Name);
            }

        }

        /// <summary>
        /// 将一组DataEntity对象 填入一个现有的 DataSet, 如果该 DataEntity 映射的 DataTable 已经存在, 则增加一条新记录, 否则创建一个 DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceList"></param>
        /// <param name="targetDataSet"></param>
        /// <param name="tableName"></param>
        public static void FillDataSet<T>(IList<T> sourceList, ref DataSet targetDataSet, string tableName)
        {
            Type classType = typeof(T); ;
            if (classType.FullName.Equals("Explorer.Framework.Data.XRM.XRMEntity"))
            {
                XRMEntityFillDataSet<T>(sourceList, ref targetDataSet, tableName);
            }
            else
            {
                ORMEntityFillDataSet<T>(sourceList, ref targetDataSet, tableName);
            }
        }

        public static void XRMEntityFillDataSet<T>(IList<T> sourceList, ref DataSet targetDataSet, string tableName)
        {            
            if (sourceList.Count == 0)
            {
                return;       
            }
            XRMEntity entity = sourceList[0] as XRMEntity;
            EntityAdapter entityAdapter = entity.EntityAdapter;

            bool isRM = IsRM(entityAdapter.EntityName, tableName);

            DataTable table = targetDataSet.Tables[entityAdapter.TableName];
            if (table == null)
            {
                table = new DataTable(tableName);
                targetDataSet.Tables.Add(table);
                foreach (FieldAdapter fieldAdapter in entityAdapter.GetFields())
                {
                    if (isRM)
                    {
                        table.Columns.Add(fieldAdapter.ColumnName);
                    }
                    else
                    {
                        table.Columns.Add(fieldAdapter.Name);
                    }
                }
            }
            foreach (T t in sourceList)
            {
                entity = t as XRMEntity;
                DataRow row = table.NewRow();
                foreach (FieldAdapter fieldAdapter in entityAdapter.GetFields())
                {
                    if (isRM)
                    {
                        object value = entity.Items[fieldAdapter.ColumnName];
                        if (value != null)
                        {
                            row[fieldAdapter.ColumnName] = value;
                        }
                    }
                    else
                    {
                        object value = entity.Items[fieldAdapter.Name];
                        if (value != null)
                        {
                            row[fieldAdapter.Name] = value;
                        }
                    }
                }
                table.Rows.Add(row);
            }
        }

        public static void ORMEntityFillDataSet<T>(IList<T> sourceList, ref DataSet targetDataSet, string tableName)
        {
            Type type = typeof(T);
            bool isORM = IsRM(type, tableName);

            DataTable table = targetDataSet.Tables[tableName];
            if (table == null)
            {
                table = new DataTable(tableName);
                targetDataSet.Tables.Add(table);
                foreach (PropertyInfo pInfo in type.GetProperties())
                {
                    if (isORM)
                    {
                        ORMColumnAttribute ca = (ORMColumnAttribute)pInfo.GetCustomAttributes(typeof(ORMColumnAttribute), false)[0];
                        table.Columns.Add(ca.Name);
                    }
                    else
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
            foreach (T t in sourceList)
            {
                DataRow row = table.NewRow();
                foreach (PropertyInfo pInfo in type.GetProperties())
                {
                    string columnName = pInfo.Name;
                    if (isORM)
                    {
                        ORMColumnAttribute ca = (ORMColumnAttribute)pInfo.GetCustomAttributes(typeof(ORMColumnAttribute), false)[0];
                        columnName = ca.Name;
                    }
                    if (table.Columns.Contains(columnName))
                    {
                        object value = pInfo.GetValue(t, null);
                        //  如果 value 不为空则加入 row 里面
                        if (value != null)
                        {
                            row[columnName] = value;
                        }
                    }
                }
                table.Rows.Add(row);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private static bool IsRM(Type type, string tableName)
        {
            if (!type.Name.EndsWith(tableName))
            {
                ORMTableAttribute[] tableAttribute = (ORMTableAttribute[])type.GetCustomAttributes(typeof(ORMTableAttribute), false);
                if (tableAttribute != null && tableAttribute.Length > 0 && tableAttribute[0].Name.Equals(tableName))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsRM(string entityName, string tableName)
        {
            if (!entityName.EndsWith(tableName))
            {
                return true;
            }
            return false;
        }    
    }
}
