using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Explorer.Framework.Net.Processor
{
    public interface ISocketProcessor
    {
        byte[] ProcessorKey { get; set; }

        /// <summary>
        /// 服务器地址
        /// </summary>
        IPEndPoint ServerIPEndPoint { get; set; }

        /// <summary>
        /// 客户端地址
        /// </summary>
        IPEndPoint ClientIPEndPoint { get; set; }

        ISocketSender SocketSender { get; set; }

        ISocketReceiver SocketReceiver { get; set; }

        object Process(byte[] data);
    }
}
