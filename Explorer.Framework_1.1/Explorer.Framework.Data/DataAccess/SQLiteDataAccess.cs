using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Explorer.Framework.Data.DataAccess
{
    /// <summary>
    /// 
    /// </summary>
    public class SQLiteDataAccess : BaseDataAccess
    {
        internal SQLiteDataAccess()
            : base()
        { }

        internal SQLiteDataAccess(IDbConnection dbConnection)
            : base(dbConnection)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IDbDataAdapter CreateDataAdapter()
        {
            Type classType = this.DbConnection.GetType();
            IDbDataAdapter dbAdapter = (IDbDataAdapter)classType.Assembly.CreateInstance("System.Data.SQLite.SQLiteDataAdapter");
            return dbAdapter;
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
