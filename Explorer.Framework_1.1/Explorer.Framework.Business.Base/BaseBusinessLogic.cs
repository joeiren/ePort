using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Explorer.Framework.Data;
using Explorer.Framework.Data.DataAccess;

namespace Explorer.Framework.Business
{
    /// <summary>
    /// 业务逻辑接口
    /// </summary>
    public abstract class BaseBusinessLogic : MarshalByRefObject
    {
        /// <summary>
        /// 核心逻辑处理接口, 实现该接口后数据库需要自己去通过 BusinessKey.DbName 获取, 并需要自己控制数据库 Close, Commit, Rollback 等
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        public virtual DataSet Process(DataSet dataSet)
        {
            bool isTrans = false;
            IDbTransaction dbTrans;

            DataSet result = new DataSet();
            BusinessKey businessKey = BusinessKey.GetKey(dataSet, false);

            if ("TRUE".Equals(businessKey.TransactionMode.ToUpper()))
            {
                isTrans = true;
            }

            IDataAccess da = DataAccessFactory.Instance.GetDataAccess(businessKey.DbName);
            if (da != null && isTrans)
            {
                dbTrans = da.BeginTransaction();
            }
            try
            {
                result = DataProcess(dataSet);
                if (da != null && isTrans)
                {
                    da.Commit();
                }
            }
            catch (Exception ex)
            {
                if (da != null && isTrans)
                {
                    da.Rollback();
                }
                throw ex;
            }
            finally
            {
                if (da != null)
                {
                    da.Close();
                }
            }

            return result;
        }


        /// <summary>
        /// 数据逻辑处理接口, 实现该接口后数据库事务将会自动管理,不用在程序里去 Close, Commit, Rollback 等, 在没有实现 Process 时必须实现该接口
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        public virtual DataSet DataProcess(DataSet dataSet)
        {
            return dataSet;
        }
    }
}
