using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Explorer.Framework.Business;
using Explorer.Framework.Logger;
using Explorer.Framework.Data.DataAccess;

namespace Explorer.Framework.BLLExample
{
    public class BLOExample : BaseBusinessLogic
    {
        #region IBusinesslogic 成员
        private ILogger _logger = LoggerFactory.Instance.GetLogger(typeof(BLOExample));

        public override DataSet Process(DataSet dataSet)
        {
            IDataAccess da = DataAccessFactory.Instance.GetSlotDataAccess();

            DataSet result = da.QueryDataSet("SELECT * FORM tableName");


            return result;
        }

        #endregion
    }
}
