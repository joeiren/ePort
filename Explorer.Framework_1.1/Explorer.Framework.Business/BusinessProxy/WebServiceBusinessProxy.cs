using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Explorer.Framework.Business.BusinessProxy
{
    /// <summary>
    /// 业务逻辑代理,WebService模式代理
    /// </summary>
    public class WebServiceBusinessProxy : BaseBusinessProxy
    {
        public string Username { get; set; }
        public string Password { get; set; }

        internal WebServiceBusinessProxy(DataRow row)
            : base(row)
        { 
            
        }

        public override DataSet Process(DataSet dataSet)
        {
            return null;
        }
       
    }
}
