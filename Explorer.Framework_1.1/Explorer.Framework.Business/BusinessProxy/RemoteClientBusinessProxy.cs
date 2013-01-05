using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Channels.Tcp;

using Explorer.Framework.Data;
using Explorer.Framework.Data.DataAccess;

namespace Explorer.Framework.Business.BusinessProxy
{
    /// <summary>
    /// 业务逻辑代理,远程文件代理
    /// </summary>f
    public class RemoteClientBusinessProxy : BaseBusinessProxy
    {
        internal RemoteClientBusinessProxy(DataRow row)
            : base(row)
        {

        }

        public override DataSet Process(DataSet dataSet)
        {
            //DataSet result = new DataSet();
            //BusinessAssembly ba = BusinessAssemblyFactory.Instance.GetAssembly(this.BusinessAssemblyName);
            //ba.CreateInstance(this.ClassName,this.Key);
            //result = logic.Process(dataSet);
            //ChannelServices.UnregisterChannel(channel);
            return null;
        }

    }
}
