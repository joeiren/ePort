using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading;
using System.Net;

using Explorer.Framework.Core;
using Explorer.Framework.Core.Base;
using Explorer.Framework.Core.ExtendAttribute;

namespace Explorer.Framework.Data.DataAccess
{


    /// <summary>
    /// 
    /// </summary>
    public interface IDataAccessFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IDataAccess GetDataAccess();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IDataAccess GetDataAccess(string name);        
    }

    /// <summary>
    /// 数据存取工厂
    /// </summary>
    public class DataAccessFactory : BaseFactory<DataAccessFactory, IDataAccess>, IDataAccessFactory
    {
        /// <summary>
        /// 
        /// </summary>
        public string DefaultName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public override void Initialize()
        {
            // 如果已经初始化过后, 还手工调用初始化, 则视为重初始化
            if (this.IsInitialize)
            {
                // 重初始化系统环境配置
                AppContext.Instance.Initialize();     
            }
            // 重建数据
            this.DefaultName = "Default";
            DataSet dataSet = AppContext.Instance.GetAppContextSet();
            if (dataSet != null)
            {
                DataTable table = dataSet.Tables["DataSourceContext"];
                if (table != null)
                {
                    if (table.Rows[0]["Default"] != null)
                    {
                        this.DefaultName = table.Rows[0]["Default"].ToString();
                    }
                }
            }
            base.Initialize();
        }

        /// <summary>
        /// 获取指定数据存取器, 并将该数据存取器缓存到线程槽
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IDataAccess GetDataAccess(string name)
        {
            return GetDataAccess(name, true);
        }

        /// <summary>
        /// 获取指定数据存取器, 并指定是否将数据存取器缓存到线程槽
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isCacheSlot"></param>
        /// <returns></returns>
        public IDataAccess GetDataAccess(string name, bool isCacheSlot)
        {
            IDataAccess da = DataAccessBuilder.Instance.CreateInstance(name);
            if (isCacheSlot)
            {
                Thread.SetData(Thread.GetNamedDataSlot("DataAccess"), da);
            }
            da.Open();
            return da;

        }

        /// <summary>
        /// 获取指定线程槽里的数据存取器
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IDataAccess GetSlotDataAccess(string name)
        {
            IDataAccess da = null;
            da = Thread.GetData(Thread.GetNamedDataSlot("DataAccess")) as IDataAccess;
            if (da == null)
            {
                da = GetDataAccess(name);
            }
            da.Open();
            return da;
        }

        /// <summary>
        /// 获取默认的数据存取器, 如果线程槽里无缓存则拿出默认的数据存取器并缓存到线程槽
        /// </summary>
        /// <returns></returns>
        public IDataAccess GetDataAccess() 
        {
            return GetDataAccess(this.DefaultName);
        }

        /// <summary>
        /// 从默认线程槽里拿出缓存的数据存取器
        /// </summary>
        /// <returns></returns>
        public IDataAccess GetSlotDataAccess()
        {
            return GetSlotDataAccess(this.DefaultName);
        }

        /// <summary>
        /// 创建一个数据库存取器
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="dataBaseType">数据库类型</param>
        /// <param name="connectionString">数据库链接字符串</param>
        /// <returns></returns>
        public IDataAccess CreateDataAccess(string name, DataBaseType dataBaseType, string connectionString)
        {
            return DataAccessBuilder.Instance.CreateInstance(name, dataBaseType, connectionString);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Dispose()
        {
            // 释放实体类数据存取
            //ORMEntityAccessFactory.Instance.Dispose();            
            base.Dispose();
        }       
    }
}
