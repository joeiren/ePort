using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Explorer.Framework.Business;
using Explorer.Framework.Logger;
using Explorer.Framework.Data.DataAccess;
using Explorer.Framework.Data;

namespace Explorer.Framework.BLLExample
{
    public class Test1BLH : BaseBusinessLogic
    {
        private ILogger _logger;

        /// <summary>
        /// 重写 Process 方法后, 所有数据库都需要自己打开和关闭
        /// 数据库事务也需要自己控制处理
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        public override DataSet Process(DataSet dataSet)
        {
            BusinessKey businessKey = BusinessKey.GetKey(dataSet);
            this._logger = LoggerFactory.Instance.GetLogger(businessKey.Name, typeof(Test1BLH));

            ///
            ///
            using (IDataAccess da = DataAccessFactory.Instance.GetDataAccess(businessKey.DbName))
            {
                da.BeginTransaction();
                try
                {
                    da.Execute("INSERT INTO () VALUES ()");
                    da.Commit();
                }
                catch (Exception)
                {
                    da.Rollback();
                }
                finally
                {
                    da.Close();
                }

            }
            
            return dataSet;
        }

    }
}
