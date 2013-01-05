using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;

namespace Explorer.Framework.Data.DataAccess
{
    /// <summary>
    /// OleDb 数据访问对象
    /// </summary>
    public class OleDbDataAccess : BaseDataAccess
    {
        internal OleDbDataAccess()
            : base()
        { }

        internal OleDbDataAccess(IDbConnection dbConnection)
            : base(dbConnection)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IDbDataAdapter CreateDataAdapter()
        {
            return new OleDbDataAdapter();
        }

        /// <summary>
        /// 
        /// </summary>
        public override string ParameterToken
        {
            get { throw new NotImplementedException(); }
        }
    }
}
