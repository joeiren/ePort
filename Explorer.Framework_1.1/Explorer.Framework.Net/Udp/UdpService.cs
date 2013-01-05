using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Net;
using System.Net.Sockets;

using Explorer.Framework.Core.Base;
using Explorer.Framework.Logger;
using Explorer.Framework.Net;
using Explorer.Framework.Net.Processor;
using Explorer.Framework.Net.Protocol;

namespace Explorer.Framework.Net.Udp
{
    /// <summary>
    /// UDP 服务类
    /// </summary>
    public class UdpService : BaseSingletonInstance<UdpService>, ISocketSender, ISocketReceiver
    {        
        public event DataReceiveHandler DataReceive;

        protected ILogger _logger;
        protected IPEndPoint _localEP;
        protected UdpClient _udpClient;
        protected Dictionary<string, IPEndPoint> _endPointMap;

        protected bool _isActivity;
        protected Thread _thread;
        protected int[] _keyIndex;
        protected byte[] _cacheData;

        /// <summary>
        /// 发送的字节统计
        /// </summary>
        public long SendBytesCount { get; private set; }

        /// <summary>
        /// 接收的字节统计
        /// </summary>
        public long ReceiveBytesCount { get; private set; }

        /// <summary>
        /// 接收缓冲区
        /// </summary>
        public int ReceiveBufferSize 
        {
            get { return this._receiveBufferSize; }
            set 
            {                
                this._receiveBufferSize = value;
                if (this._udpClient != null)
                {
                    this._udpClient.Client.ReceiveBufferSize = this._receiveBufferSize;   
                }
            }
        }protected int _receiveBufferSize;

        /// <summary>
        /// 
        /// </summary>
        public bool Blocking
        {
            get { return this._blocking; }
            set
            {
                this._blocking = value;
                if (this._udpClient != null)
                {
                    this._udpClient.Client.Blocking = this._blocking;
                }
            }
        }protected bool _blocking;

        /// <summary>
        /// 最后一次发送的时间
        /// </summary>
        public DateTime LastSendTime { get; private set; }

        /// <summary>
        /// 最后一次接收的时间
        /// </summary>
        public DateTime LaseReceiveTime { get; private set; }

        /// <summary>
        /// 数据帧头关键字
        /// </summary>
        public byte FrameDataBegin { get; set; }

        /// <summary>
        /// 数据帧尾关键字
        /// </summary>
        public byte FrameDataEnd { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Socket Client
        {
            get { return this._udpClient.Client; }
        }

        public override void Initialize()
        {
            this._logger = LoggerFactory.Instance.GetLogger("UdpService", typeof(UdpService), @"${Path}\Socket\Udp.${Date}.txt");

            this._endPointMap = new Dictionary<string, IPEndPoint>();
            
            base.Initialize();
        }

        public List<IPEndPoint> GetIPEndPoints()
        {
            List<IPEndPoint> list = new List<IPEndPoint>();
            lock (this._endPointMap)
            {
                IPEndPoint[] eps = (IPEndPoint[])this._endPointMap.Values.ToArray<IPEndPoint>();
                list.AddRange(eps);
            }
            return list;
        }

        public void SetKeyIndex(int[] keyIndex)
        {
            this._keyIndex = keyIndex;
        }

        public void Ready()
        {
            if (this._udpClient != null)
            {
                this._udpClient.Close();
            }
            this._cacheData = new byte[0];
            this.SendBytesCount = 0;
            this.ReceiveBytesCount = 0;
            this._localEP = new IPEndPoint(IPAddress.Parse(this.IpAddress), this.Port);
            this._udpClient = new UdpClient(this._localEP);

            if (this._blocking)
            {
                this._udpClient.Client.Blocking = this._blocking;
            }

            if (this._receiveBufferSize == 0)
            {
                this._udpClient.Client.ReceiveBufferSize = 10240;
            }
            else
            {
                this._udpClient.Client.ReceiveBufferSize = this.ReceiveBufferSize;
            }
            

            this._logger.Debug("UdpService is ready");
        }

        public override void Dispose()
        {
            if (this._udpClient != null)
            {
                this._udpClient.Close();
            }
            base.Dispose();
        }

        public void AddClient(string ipAddress, int port)
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            if (!this._endPointMap.ContainsKey(ep.ToString()))
            {
                this._endPointMap.Add(ep.ToString(), ep);
                this._logger.Debug("UdpService add remote " + ep.ToString());
            }
        }

        public void Send(byte[] data)
        {
            foreach (IPEndPoint ep in this._endPointMap.Values)
            {
                this._udpClient.Send(data, data.Length, ep);
                this._logger.Debug("UdpService send to " + ep.ToString() + ":" + HexConverter.ToHexString(data, "00"));
            }
            this.SendBytesCount += data.Length;
            this.LastSendTime = DateTime.Now;
        }

        public void Send(byte[] data, IPEndPoint ep)
        {
            Send(data, ep, false);
        }

        public void Send(byte[] data, IPEndPoint ep, bool isSaveEP)
        {
            if (isSaveEP)
            {
                this._endPointMap.Add(ep.ToString(), ep);
                this._logger.Debug("UdpService send to " + ep.ToString() + ":" + HexConverter.ToHexString(data, "00"));
            }
            this._udpClient.Send(data, data.Length, ep);
            this.SendBytesCount += data.Length;
            this.LastSendTime = DateTime.Now;
        }

        public void StartReceive()
        {
            this._isActivity = true;

            this._thread = new Thread(new ThreadStart(Receive));
            this._thread.IsBackground = false;
            this._thread.Name = "UdpReceiver";
            this._thread.Start();
        }

        public void StopReceive()
        {
            this._isActivity = false;
        }

        public virtual void Receive()
        {
            while (this._isActivity)
            {
                try
                {
                    IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

                    Byte[] dataPacket = this._udpClient.Receive(ref remoteEP);

                    this.ReceiveBytesCount += dataPacket.Length;
                    this.LaseReceiveTime = DateTime.Now;

                    this._logger.Debug("UdpService receive from " + remoteEP.ToString() + ":" + HexConverter.ToHexString(dataPacket, "00"));

                    if (!this._endPointMap.ContainsKey(remoteEP.ToString()))
                    {
                        this._endPointMap.Add(remoteEP.ToString(), remoteEP);
                    }

                    OnDataReceive(new DataReceiveEventArgs(dataPacket));

                    Parser(dataPacket, remoteEP);
                }
                catch (Exception ex)
                {
                    this._logger.Error("UdpService receive error ", ex);
                }
            }
        }

        /// <summary>
        /// 解析数据, 默认方法里会用到 SplitData 方法来按照 FrameDataBegin 和 FrameDataEnd 来查找当前帧数据
        /// </summary>
        /// <param name="dataPacket"></param>
        public virtual void Parser(byte[] dataPacket, IPEndPoint ep)
        {           
            /// 重新拆分和拼装数据
            List<byte[]> frameDataList = new List<byte[]>();
            
            SplitData(frameDataList, dataPacket);

            foreach (byte[] frameData in frameDataList)
            {
                if (frameData.Length > this._keyIndex.Length)
                {
                    byte[] b_ProtocolKey = new byte[this._keyIndex.Length];

                    for (int i = 0; i < this._keyIndex.Length; i++)
                    {
                        b_ProtocolKey[i] = frameData[this._keyIndex[i]];
                    }

                    long l_ProtocolKey = SocketProcessorFactory.BuildKey(b_ProtocolKey);
                    if (SocketProcessorFactory.Instance.Contains(l_ProtocolKey))
                    {
                        ISocketProcessor processor = SocketProcessorFactory.Instance.CreateProcessor(l_ProtocolKey);
                        processor.ServerIPEndPoint = new IPEndPoint(IPAddress.Parse(this.IpAddress), this.Port);
                        processor.ClientIPEndPoint = ep;
                        processor.ProcessorKey = b_ProtocolKey;
                        processor.Process(frameData);
                        processor.SocketReceiver = this;
                        processor.SocketReceiver = this;
                    }
                }
            }
        }

        /// <summary>
        /// 数据拆分器
        /// </summary>
        /// <param name="frameList"></param>
        /// <param name="dataPacket"></param>
        public virtual void SplitData(List<byte[]> frameList, byte[] dataPacket)
        {
            byte[] tempData = new byte[this._cacheData.Length + dataPacket.Length];

            // 把缓存的填到 tempData 里
            Array.Copy(this._cacheData, tempData, this._cacheData.Length);
            // 把新收到的 dataPacket 填到 tempData 里
            Array.Copy(dataPacket, 0, tempData, this._cacheData.Length, dataPacket.Length);

            
            int pStart = -1;
            
            for (int i = 0; i < tempData.Length; i++)
            {
                // 找到头关键字
                if (tempData[i] == this.FrameDataBegin)
                {
                    pStart = i;
                    int pEnd = -1;

                    for (int j = i + 1; j < tempData.Length; j++)
                    {
                        i = j;
                        if (tempData[j] == this.FrameDataEnd)
                        {
                            pEnd = j;
                            break;
                        }
                    }

                    if (pStart > -1 && pEnd > pStart)
                    {
                        byte[] outputData = new byte[pEnd - pStart + 1];
                        Array.Copy(tempData, pStart, outputData, 0, outputData.Length);
                        frameList.Add(outputData);

                        this._cacheData = new byte[tempData.Length - pStart - outputData.Length];
                        if (this._cacheData.Length > 0)
                        {
                            Array.Copy(tempData, pStart + outputData.Length, this._cacheData, 0, this._cacheData.Length);
                            SplitData(frameList, new byte[] { });
                        }                        
                    }
                    else if (pEnd == -1)
                    {
                        this._cacheData = tempData;
                    }                                       
                }                
            }
        }

        private void OnDataReceive(DataReceiveEventArgs e)
        {
            if (this.DataReceive != null)
            {
                this.DataReceive(this, e);
            }
        }

    }
}
