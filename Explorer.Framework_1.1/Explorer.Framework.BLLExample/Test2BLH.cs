using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Explorer.Framework.Business;
using Explorer.Framework.Logger;
using Explorer.Framework.Data;
using Explorer.Framework.Data.XRM;
using Explorer.Framework.Data.DataAccess;
using Explorer.Framework.Data.EntityAccess;

namespace Explorer.Framework.BLLExample
{
    /// <summary>
    /// 
    /// </summary>
    public class Test2BLH : BaseBusinessLogic
    {
        /// <summary>
        /// 重写 DataProcess 方法后, 数据库就已经按照 BusinessKey 里传递的 DbName 打开了
        /// 如果有在配置文件里指定了事务模式, 则事务也自动处理好了
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        public override DataSet DataProcess(DataSet dataSet)
        {
             //XRMEntity entity = DataConverter.ToXRMEntity<XRMEntity>(dataSet.Tables[""]

            XRMEntity entity = EntityAdapterManager.Instance.CreateXRMEntityByFullName("Entity.Gps.TXServer.OrderAgent");

            IDataAccess da = DataAccessFactory.Instance.GetDataAccess("Gps");

            EntityAccess<XRMEntity> ea = da.GetEntityAccess<XRMEntity>(entity.EntityAdapter);

            /// 测试 Load 方法
            entity.Items.Add("OrderAgentId", 7);
            
            ea.Load(ref entity);

            DataConverter.FillDataSet(entity, ref dataSet);

            return dataSet;
        }
    }
}
