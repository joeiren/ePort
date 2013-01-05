using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Explorer.Framework.Net.Processor
{
    public interface ISocketSender
    {
        void Send(byte[] data);

        void Send(byte[] data, IPEndPoint ep);
    }


}
