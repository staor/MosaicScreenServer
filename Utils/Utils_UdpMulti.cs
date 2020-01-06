using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Utils
{
    public class Utils_UdpMulti
    {
        public UdpClient ServerMultiUdpClient = null;
        public   Func<string,string> FuncUdpMultiMsg;  //volatile
        public Encoding encoding = Encoding.UTF8;
        public  CancellationTokenSource cts = new CancellationTokenSource();
        private string Ip { get; set; }
        private int Port { get; set; }

        public void InitialJoinGroup(string ip, int port)
        {
            Ip = ip;
            Port = port;

            Task task=Task.Factory.StartNew(() =>
            {
                ServerUdpMulticastListener(ip, port);
            }/*, TaskCreationOptions.LongRunning */);
            //task.Start(); 
        }
        public void Stop()
        {
            cts.Cancel();             
            FuncUdpMultiMsg = null;
            if (ServerMultiUdpClient!=null)
            {
                ServerMultiUdpClient.DropMulticastGroup(IPAddress.Parse(Ip));
                ServerMultiUdpClient.Close();
            }
            hsServer.ShowDebug("停止组播获取/发送：" + Ip + ":" + Port);
            
        }

        public void ServerUdpMulticastListener(string multiIp, int multiPort)
        {
            //ParameterizedThreadStart p = new ParameterizedThreadStart(ReceivThreadEventHandler);
            //if (!IsChating)
            //{
            //    IsChating = true;
            //}
            UdpClient client = null;  //断网后马上接入网线，会继续接收组播数据，也不会报异常。

            try
            {
                if (client == null)
                {
                    client = new UdpClient();
                    client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    client.Client.Bind(new IPEndPoint(IPAddress.Any, multiPort));

                    ServerMultiUdpClient = client;
                    client.JoinMulticastGroup(IPAddress.Parse(multiIp));
                    //client.BeginReceive(new AsyncCallback(UdpRcvMultiMsgAsyncBack), client);
                    hsServer.ShowDebug("加入Rx4服务器同步信息组播频道：" + multiIp + ":" + multiPort);
                    UdpRcvMultiMsgAsyncBackAsync(client);
                }
                //hsServer.ShowDebug("停止组播获取/发送：" + multiIp + ":" + multiPort);
                //client.DropMulticastGroup(IPAddress.Parse(multiIp));
                //client.Close();
            }
            catch (Exception ex)
            {
                hsServer.ShowDebug("Utils_UdpMulti-加入组播异常：" + ex.Message);
                if (client != null)
                {
                    client.Close();
                    client = null;
                }
            }

            //Thread th = new Thread(ReceivThreadEventHandler);
            //th.IsBackground = true;
            //th.Start();
        }
       
        private void UdpRcvMultiMsgAsyncBackAsync(UdpClient client)
        {

            try
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] bytes = client.Receive(ref endPoint);
                if (cts.IsCancellationRequested)
                {
                    return;
                }
                Task.Run(() =>
                {
                    UdpRcvMultiMsgAsyncBackAsync(client);
                },cts.Token);
                string msg = encoding.GetString(bytes);  //HS的json格式为utf-8
                string rcv=  FuncUdpMultiMsg?.Invoke(msg);   //返回值可以
                if (!string.IsNullOrEmpty(rcv))
                {
                    client.Send(encoding.GetBytes(rcv), rcv.Length);
                }                
            }
            catch (Exception ex)
            {
                //if (client != null)
                //{
                //    client.Close();
                //}
                hsServer.ShowDebug("Utils_UdpMulti-Udp组播侦听异常" + ex.Message);
                //return;
                //throw;
            }
        }
        
    }
}
