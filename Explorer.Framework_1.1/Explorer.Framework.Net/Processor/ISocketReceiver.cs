using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Explorer.Framework.Net.Processor
{
    public delegate void DataReceiveHandler(object sender, DataReceiveEventArgs e);

    public class DataReceiveEventArgs : EventArgs
    {
        public DataReceiveEventArgs(byte[] data)
        {
            this.Data = data;
        }

        public byte[] Data { get; set; }
    }

    public interface ISocketReceiver
    {
        event DataReceiveHandler DataReceive;
    }
}
