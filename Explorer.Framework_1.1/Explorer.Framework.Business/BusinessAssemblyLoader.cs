using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Channels.Tcp;
using System.Collections;

namespace Explorer.Framework.Business
{
    /// <summary>
    /// 
    /// </summary>
    public class BusinessAssemblyLoader : MarshalByRefObject
    {
        /// <summary>
        /// 装载本地对象
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public object CreateInstance(string filename, string className)
        {
            object obj = null;
            Assembly assembly = Assembly.LoadFile(filename);
            obj = assembly.CreateInstance(className, false);
            return obj;
        }

        /// <summary>
        /// 注册 Remoting 服务对象
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="className"></param>
        /// <param name="port"></param>
        /// <param name="serviceName"></param>
        /// <param name="remoteMode"></param>
        /// <returns></returns>
        public void RegisterRemoting(string filename, string className, string serviceName, int port, string remoteMode)
        {
            IChannel channel = null;
            foreach (IChannel c in ChannelServices.RegisteredChannels)
            {
                if (c.ChannelName.Equals(port.ToString()))
                {
                    channel = c;
                }
            }
            if (null == channel)
            {
                IDictionary properties = new Hashtable();
                properties["name"] = port.ToString();
                properties["port"] = port;

                if (remoteMode.ToUpper().EndsWith("HTTP"))
                {
                    channel = new HttpChannel(properties, new SoapClientFormatterSinkProvider(), new SoapServerFormatterSinkProvider());
                }
                else
                {
                    channel = new TcpChannel(properties, new BinaryClientFormatterSinkProvider(), new BinaryServerFormatterSinkProvider());
                }
                ChannelServices.RegisterChannel(channel, false);
                string str = channel.ChannelName;
            }
            Assembly assembly = Assembly.LoadFile(filename);
            Type type = assembly.GetType(className);
            RemotingConfiguration.RegisterWellKnownServiceType(type, serviceName, WellKnownObjectMode.SingleCall);
        }

    }
}
