
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utils;

namespace MosaicServerFromRx
{
    public class MosaicServerFromRx
    {
        public static UdpClient ServerMultiUdpClient = null;
        public Action<string> RcvMulticastServerMsgEvent;
        public bool Initial(string ip, int port)
        {
            bool isOk = true;
            Thread threadListener = new Thread(new ThreadStart(delegate
            {
                ServerUdpMulticastListener(ip, port);
            }));
            threadListener.IsBackground = true;
            threadListener.Start();
            return isOk;
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
            }
            catch (Exception ex)
            {
                hsServer.ShowDebug("加入组播异常,将重加组，UdpMulticastServerListener：" + ex.Message);
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
        private  void UdpRcvMultiMsgAsyncBack(IAsyncResult state)
        {
            UdpClient udpClient = null;
            try
            {
                udpClient = (UdpClient)state.AsyncState;
                IPEndPoint endPoint = (IPEndPoint)udpClient.Client.LocalEndPoint;

                byte[] bytes = udpClient.EndReceive(state, ref endPoint);
                string msg = Encoding.UTF8.GetString(bytes);  //HS的json格式为utf-8
                RcvMulticastServerMsgEvent?.BeginInvoke(msg, null, null);   //此处报错 ，提示操作平台不支持？  
                //RcvMulticastServerMsgEvent?.Invoke(msg);      //会引入目标线程任务异步排队执行，
                //App.Current.Dispatcher.Invoke(RcvMulticastServerMsgEvent, msg);
                //ShowDebug("接受组播消息：" + msg);
                //Console.WriteLine(value);
                //// 在这里使用异步委托来处理接收到的数组，并再次启动接收
                var ar = udpClient.BeginReceive(new AsyncCallback(UdpRcvMultiMsgAsyncBack), udpClient);
            }
            catch (Exception ex)
            {
                if (udpClient != null)
                {
                    udpClient.Close();
                }
                hsServer.ShowDebug("UdpRcvMultiMsgAsyncBack-Udp组播侦听异常退出" + ex.Message);
                return;
                //throw;
            }
        }

        private void UdpRcvMultiMsgAsyncBackAsync(UdpClient client)
        {

            try
            {
                IPEndPoint endPoint = (IPEndPoint)client.Client.RemoteEndPoint;
                byte[] bytes = client.Receive(ref endPoint);
                Task.Run(() =>
                {
                    UdpRcvMultiMsgAsyncBackAsync(client);
                });                
                string msg = Encoding.UTF8.GetString(bytes);  //HS的json格式为utf-8
                RcvMulticastServerMsgEvent?.Invoke(msg);   //此处报错 ，提示操作平台不支持？  
                 
            }
            catch (Exception ex)
            {
                //if (client != null)
                //{
                //    client.Close();
                //}
                hsServer.ShowDebug("UdpRcvMultiMsgAsyncBack-Udp组播侦听异常退出" + ex.Message);
                //return;
                //throw;
            }
        }
    }
}
