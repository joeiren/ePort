using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Explorer.Framework.Data.DataAccess;

namespace Explorer.Framework.Data.DataProvider
{
    interface IDataProvider
    {
        IDataAccess GetDataAccess();

        IDataAccess GetDataAccess(string name);
    }
}
