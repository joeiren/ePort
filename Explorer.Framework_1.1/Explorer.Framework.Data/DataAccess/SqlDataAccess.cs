using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Explorer.Framework.Data.DataAccess
{
    /// <summary>
    /// 
    /// </summary>
    public class SqlDataAccess : BaseDataAccess
    {
        internal SqlDataAccess()
            : base()
        { }

        internal SqlDataAccess(IDbConnection dbConnection)
            : base(dbConnection)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IDbDataAdapter CreateDataAdapter()
        {
            return new SqlDataAdapter();
        }        

        /// <summary>
        /// 
        /// </summary>
        public override string ParameterToken
        {
            get { return "@"; }
        }
    }
}
