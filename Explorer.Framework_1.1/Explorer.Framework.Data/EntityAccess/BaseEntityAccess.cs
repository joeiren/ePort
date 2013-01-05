using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Explorer.Framework.Data.DataAccess;

namespace Explorer.Framework.Data.EntityAccess
{
    public enum ExistsMode
    {
        And, Or
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class EntityAccess<T> : MarshalByRefObject where T : new()
    {
        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public IDataAccess DataAccess { get; set; }

        /// <summary>
        /// 当前 Entity 的适配器
        /// </summary>
        public EntityAdapter EntityAdapter { get; set; }
        #endregion

        #region 私有变量
        /// <summary>
        /// 
        /// </summary>
        protected Dictionary<string, IDbCommand> _cacheCommandMap;
        #endregion

        internal EntityAccess(IDataAccess dataAccess)
        {
            this.DataAccess = dataAccess;

            this._cacheCommandMap = new Dictionary<string, IDbCommand>();
        }

        internal EntityAccess(IDataAccess dataAccess, EntityAdapter entityAdapter)
        {
            this.DataAccess = dataAccess;

            this.EntityAdapter = entityAdapter;

            this._cacheCommandMap = new Dictionary<string, IDbCommand>();
        }

        /// <summary>
        /// 根据主键来读取当前数据实体类
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public abstract bool Load(ref T t);

        /// <summary>
        /// 跟据属性值来判断当前实体类是否存在, 如果返回值大于 1 则表示有多条数据满足该条件
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public abstract int Exists(T t);

        /// <summary>
        /// 跟据属性值来判断当前实体类是否存在, 如果返回值大于 1 则表示有多条数据满足该条件
        /// 并且通过 ExistsMode.And 或是 ExistsMode.Or 决定检查条件
        /// </summary>
        /// <param name="t"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public abstract int Exists(T t, ExistsMode mode);

        /// <summary>
        /// 跟据属性值来判断当前实体类是否存在, 并返回找到的所有实体类, 
        /// 默认使用 ExistsMode.And 判断
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public abstract List<T> Select(T t);

        /// <summary>
        /// 跟据属性值来判断当前实体类是否存在, 并返回找到的所有实体类
        /// 并且通过 ExistsMode.And 或是 ExistsMode.Or 决定检查条件
        /// </summary>
        /// <param name="t"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public abstract List<T> Select(T t, ExistsMode mode);

        /// <summary>
        /// 实现对数据的保存操作
        /// </summary>
        /// <param name="t">数据源，即被操作实体类对象（对应数据库中的某一张表）</param>
        /// <returns></returns>
        public abstract int Save(T t);

        /// <summary>
        /// 实现对数据的更新操作
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public abstract int Update(T t);

        /// <summary>
        /// 只有包含 Id
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual int SaveOrUpdate(T t)
        {
            int count = Update(t);
            if (count == 0)
            {
                count = Save(t);
            }
            return count;
        }

        /// <summary>
        /// 实现对数据的删除操作
        /// </summary>
        /// <param name="t">数据源，即被操作的实体类对象（对应数据库中的某一张表）</param>
        /// <returns>被影响的数据行数</returns>
        public abstract int Delete(T t);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="whereString"></param>
        /// <param name="orderString"></param>
        /// <returns></returns>
        [Obsolete]
        public abstract DataSet QueryDataSet(string whereString, string orderString);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="whereString"></param>
        /// <param name="orderString"></param>
        /// <returns></returns>
        [Obsolete]
        public abstract List<T> QueryList(string whereString, string orderString);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="whereString"></param>
        /// <param name="orderString"></param>
        /// <param name="pageStart"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [Obsolete]
        public abstract DataSet QueryDataSetByPage(string whereString, string orderString, int pageStart, int pageSize);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="whereString"></param>
        /// <param name="orderString"></param>
        /// <param name="pageStart"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [Obsolete]
        public abstract List<T> QueryListByPage(string whereString, string orderString, int pageStart, int pageSize);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected IDbCommand GetCacheLoadCommand()
        {
            IDbCommand dbCommand = null;
            if (this._cacheCommandMap.ContainsKey("Load"))
            {
                dbCommand = this._cacheCommandMap["Load"];
            }
            else
            {
                if (!this.EntityAdapter.KeyMap["$Id"].Equals(""))
                {
                    dbCommand = this.EntityAdapter.CreateLoadById(this.DataAccess);
                }
                else
                {
                    dbCommand = this.EntityAdapter.CreateLoadByPk(this.DataAccess);
                }
                this._cacheCommandMap.Add("Load", dbCommand);
            }
            return dbCommand;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected IDbCommand GetCacheSaveCommand()
        {
            IDbCommand dbCommand = null;
            if (this._cacheCommandMap.ContainsKey("Save"))
            {
                dbCommand = this._cacheCommandMap["Save"];
            }
            else
            {
                dbCommand = this.EntityAdapter.CreateInsert(this.DataAccess);
                this._cacheCommandMap.Add("Save", dbCommand);
            }
            return dbCommand;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected IDbCommand GetCacheUpdateCommand()
        {
            IDbCommand dbCommand = null;
            if (this._cacheCommandMap.ContainsKey("Update"))
            {
                dbCommand = this._cacheCommandMap["Update"];
            }
            else
            {
                if (!this.EntityAdapter.KeyMap["$Id"].Equals(""))
                {
                    dbCommand = this.EntityAdapter.CreateUpdateById(this.DataAccess);
                }
                else
                {
                    dbCommand = this.EntityAdapter.CreateUpdateByPk(this.DataAccess);
                }
                this._cacheCommandMap.Add("Update", dbCommand);
            }
            return dbCommand;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected IDbCommand GetCacheDeleteCommand()
        {
            IDbCommand dbCommand = null;
            if (this._cacheCommandMap.ContainsKey("Delete"))
            {
                dbCommand = this._cacheCommandMap["Delete"];
            }
            else
            {
                if (!this.EntityAdapter.KeyMap["$Pk"].Equals(""))
                {
                    dbCommand = this.EntityAdapter.CreateDeleteByPk(this.DataAccess);
                }
                else if (!this.EntityAdapter.KeyMap["$Id"].Equals(""))
                {
                    dbCommand = this.EntityAdapter.CreateDeleteById(this.DataAccess);
                }
                this._cacheCommandMap.Add("Delete", dbCommand);
            }
            return dbCommand;
        }
    }
}
