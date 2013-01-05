using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Explorer.Framework.Data.EntityAccess;

namespace Explorer.Framework.Data.DataAccess
{
    /// <summary>
    /// 数据存取器对象, 负责完成低级的 SQL 操作接口, 并能获取实体类对数据操作的实体存取器对象
    /// </summary>
    public interface IDataAccess : IDisposable
    {
        /// <summary>
        /// 当前数据存取器的注册名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 当前数据存取器的数据库类型
        /// </summary>
        DataBaseType DataBaseType { get; set; }

        /// <summary>
        /// 当前数据存取器的连接对象
        /// </summary>
        IDbConnection DbConnection { get; }

        /// <summary>
        /// 当前数据存取器的事务对象
        /// </summary>
        IDbTransaction DbTransaction { get; }

        /// <summary>
        /// 当前数据存取器的参数分隔符
        /// </summary>
        string ParameterToken { get; }

        /// <summary>
        /// 为前数据存取器打开一个事务
        /// </summary>
        /// <returns></returns>
        IDbTransaction BeginTransaction();

        /// <summary>
        /// 提交当前数据存取器的事务
        /// </summary>
        void Commit();

        /// <summary>
        /// 回滚当前数据存取器的事务
        /// </summary>
        void Rollback();

        /// <summary>
        /// 打开当前数据存取器, 当从工厂里取回一个数据存取器时它默认是打开的
        /// </summary>
        void Open();

        /// <summary>
        /// 关闭当前数据存取器
        /// </summary>
        void Close();

        /// <summary>
        /// 当前数据存取器的连接状态
        /// </summary>
        ConnectionState State { get; }

        /// <summary>
        /// 创建时间
        /// </summary>
        long CreateTimeTicks { get; }

        /// <summary>
        /// 通过当前数据存取器获取一个实体存取器
        /// </summary>
        /// <typeparam name="T">被操作的实体类</typeparam>
        /// <returns></returns>
        EntityAccess<T> GetEntityAccess<T>() where T : new();

        /// <summary>
        /// 通过当前数据存取器获取一个实体存取器
        /// </summary>
        /// <typeparam name="T">被操作的实体类</typeparam>
        /// <param name="entityAdapter">被操作的实体类适配器</param>
        /// <returns></returns>
        EntityAccess<T> GetEntityAccess<T>(EntityAdapter entityAdapter) where T : new();

        /// <summary>
        /// 通过当前数据存取器获取一个实体存取器
        /// </summary>
        /// <typeparam name="T">被操作的实体类</typeparam>
        /// <param name="fullName">被操作的实体类全名称</param>
        /// <returns></returns>
        EntityAccess<T> GetEntityAccess<T>(string fullName) where T : new();

        /// <summary>
        /// 查询 SQL 句子, 并将数据装载成对象返回
        /// </summary>
        /// <typeparam name="T">返回对象泛型</typeparam>
        /// <param name="textCommand">SQL句子</param>
        /// <returns></returns>
        List<T> QueryList<T>(string textCommand) where T : new();

        /// <summary>
        /// 查询 SQL 句子, 并将数据装载成对象返回
        /// </summary>
        /// <typeparam name="T">返回对象泛型</typeparam>
        /// <param name="dbCommand">SQL命令</param>
        /// <returns></returns>
        List<T> QueryList<T>(IDbCommand dbCommand) where T : new();

        /// <summary>
        /// 查询 SQL 句子, 返回第一行一列
        /// </summary>
        /// <param name="textCommand">SQL句子</param>
        /// <returns></returns>
        object QueryScalar(string textCommand);

        /// <summary>
        /// 查询 SQL 命令, 返回第一行一列
        /// </summary>
        /// <param name="dbCommand">SQL命令</param>
        /// <returns></returns>
        object QueryScalar(IDbCommand dbCommand);

        /// <summary>
        /// 查询 SQL 句子, 返回数据读取对象
        /// </summary>
        /// <param name="textCommand">SQL句子</param>
        /// <returns></returns>
        IDataReader QueryDataReader(string textCommand);

        /// <summary>
        /// 查询 SQL 命令, 返回数据读取对象
        /// </summary>
        /// <param name="dbCommand">SQL命令</param>
        /// <returns></returns>
        IDataReader QueryDataReader(IDbCommand dbCommand);

        /// <summary>
        /// 查询 SQL 句子, 返回数据集对象
        /// </summary>
        /// <param name="textCommand">SQL句子</param>
        /// <returns></returns>
        DataSet QueryDataSet(string textCommand);

        /// <summary>
        /// 查询 SQL 句子, 返回指定表名称的数据集对象
        /// </summary>
        /// <param name="textCommand">SQL句子</param>
        /// <param name="tableName">要填入的表名称</param>
        /// <returns></returns>
        DataSet QueryDataSet(string textCommand, string tableName);

        /// <summary>
        /// 查询 SQL 句子, 并按照指定表名称填入指定 DataSet, 返回数据集对象
        /// </summary>
        /// <param name="dataSet">要填入的DataSet</param>
        /// <param name="textCommand">SQL句子</param>
        /// <param name="tableName">表名称</param>
        /// <returns></returns>
        DataSet QueryFillDataSet(ref DataSet dataSet, string textCommand, string tableName);

        /// <summary>
        /// 查询 SQL 命令, 返回数据集对象
        /// </summary>
        /// <param name="dbCommand">SQL命令</param>
        /// <returns></returns>
        DataSet QueryDataSet(IDbCommand dbCommand);

        /// <summary>
        /// 查询 SQL 命令, 返回指定表名称的数据集对象
        /// </summary>
        /// <param name="dbCommand">SQL命令</param>
        /// <param name="tableName">要填入的表名称</param>
        /// <returns></returns>
        DataSet QueryDataSet(IDbCommand dbCommand, string tableName);


        /// <summary>
        /// 查询 SQL 命令, 并按照指定表名称填入指定 DataSet, 返回数据集对象
        /// </summary>
        /// <param name="dataSet">要填入的DataSet</param>
        /// <param name="dbCommand">SQL句子</param>
        /// <param name="tableName">表名称</param>
        /// <returns></returns>
        DataSet QueryFillDataSet(ref DataSet dataSet, IDbCommand dbCommand, string tableName);

        /// <summary>
        /// 执行一个SQL 句子,并返回受影响的行数
        /// </summary>
        /// <param name="textCommand"></param>
        /// <returns></returns>
        int Execute(string textCommand);

        /// <summary>
        /// 执行一个SQL 命令,并返回受影响的行数
        /// </summary>
        /// <param name="dbCommand"></param>
        /// <returns></returns>
        int Execute(IDbCommand dbCommand);

        /// <summary>
        /// 创建一个空的SQL命令, 并使用当前数据连接
        /// </summary>
        /// <returns></returns>
        IDbCommand CreateCommand();

        /// <summary>
        /// 创建一个指定SQL句子的SQL命令, 并使用当前数据连接
        /// </summary>
        /// <param name="sqlText"></param>
        /// <returns></returns>
        IDbCommand CreateCommand(string sqlText);

        /// <summary>
        /// 创建一个指定SQL句子的SQL命令, 同时指定命令类型, 并使用当前数据连接
        /// </summary>
        /// <param name="sqlText"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        IDbCommand CreateCommand(string sqlText, CommandType commandType);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IDbDataAdapter CreateDataAdapter();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SelectCommand"></param>
        /// <returns></returns>
        IDbDataAdapter CreateDataAdapter(IDbCommand SelectCommand);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SelectCommand"></param>
        /// <param name="InsertCommand"></param>
        /// <param name="UpdateCommand"></param>
        /// <param name="DeleteCommand"></param>
        /// <returns></returns>
        IDbDataAdapter CreateDataAdapter(IDbCommand SelectCommand,
                                         IDbCommand InsertCommand,
                                         IDbCommand UpdateCommand,
                                         IDbCommand DeleteCommand);

    }
}
