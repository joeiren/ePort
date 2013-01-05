using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Explorer.Framework.Data.DataAccess
{
    /// <summary>
    /// Oracle.Net 数据访问对象
    /// </summary>
    public class OracleDataAccess : BaseDataAccess
    {
        internal OracleDataAccess()
            : base()
        { }

        internal OracleDataAccess(IDbConnection dbConnection)
            : base(dbConnection)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IDbDataAdapter CreateDataAdapter()
        {
            return new System.Data.OracleClient.OracleDataAdapter();
        }

        /// <summary>
        /// 
        /// </summary>
        public override string ParameterToken
        {
            get { return ":"; }
        }
    }

    /// <summary>
    /// Oracle.Odac 数据访问对象
    /// </summary>
    public class OdacDataAccess : BaseDataAccess
    { 
            internal OdacDataAccess()
            : base()
        { }

            internal OdacDataAccess(IDbConnection dbConnection)
            : base(dbConnection)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IDbDataAdapter CreateDataAdapter()
        {
            Type classType = this.DbConnection.GetType();
            IDbDataAdapter dbAdapter = (IDbDataAdapter)classType.Assembly.CreateInstance("Oracle.DataAccess.Client.OracleDataAdapter");
            return dbAdapter;
        }

        /// <summary>
        /// 
        /// </summary>
        public override string ParameterToken
        {
            get { return ":"; }
        }
    }
}
