using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;

using Explorer.Framework.Data;
using Explorer.Framework.Data.DataAccess;

namespace Explorer.Framework.Business.BusinessProxy
{
    /// <summary>
    /// 业务逻辑代理,本地文件带代理
    /// </summary>
    public class LocalBusinessProxy : BaseBusinessProxy
    {
        internal LocalBusinessProxy(DataRow row)
            : base(row)
        {
        }

        public override DataSet Process(DataSet dataSet)
        {
            DataSet result;
            BusinessKey businessKey = BusinessKey.GetKey(dataSet, false);
            // 如果没有指定反射的具体类, 则由配置文件来指定
            if (string.IsNullOrEmpty(businessKey.Provider))
            {
                businessKey.Provider = this.Provider;
            }
            // 如果没有指定数据库, 则由配置文件来指定
            if (string.IsNullOrEmpty(businessKey.DbName))
            {
                businessKey.DbName = this.DbName;
            }
            // 如果没有指定数据库事务模式或是模式为默认, 则由配置文件来指定
            if (string.IsNullOrEmpty(businessKey.TransactionMode) || "default".Equals(businessKey.TransactionMode.ToLower()))
            {
                businessKey.TransactionMode = this.TransactionMode;
            }
            BusinessKey.SetKey(ref dataSet, businessKey);
            
            BusinessAssembly ba = BusinessAssemblyFactory.Instance.GetBusinessAssembly(this.BusinessAssemblyName, this.Version);

            BaseBusinessLogic logic = (BaseBusinessLogic)ba.CreateInstance(businessKey.Provider);

            result = logic.Process(dataSet);

            return result;
        }
    }
}
