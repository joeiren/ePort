using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Explorer.Framework.Data.DataAccess;

namespace Explorer.Framework.Data.DataProvider
{
    /// <summary>
    /// 
    /// </summary>
    public class DataProvider : IDataProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDataAccess GetDataAccess()
        {
            return DataAccessFactory.Instance.GetDataAccess();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IDataAccess GetDataAccess(string name)
        {
            return DataAccessFactory.Instance.GetDataAccess(name);
        }

    }
}
