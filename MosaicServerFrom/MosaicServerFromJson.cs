
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Utils;

namespace MosaicServerFrom
{
    class MosaicServerReceiveJson
    {
        public static int BuffRcv = 1024;
        public static TcpListener TcpListenerClient = null;
        public Func<string, string> Event_FromJson;



        public bool Initial(int port)
        {
            bool isOk = true;
            Thread threadListener = new Thread(new ThreadStart(delegate
            {
                TcpListenerServer(port);
            }));
            threadListener.IsBackground = true;
            threadListener.Start();
            return isOk;
        }

        public async void TcpListenerServer(int serverPort)
        { 
            if (TcpListenerClient != null)
            {
                TcpListenerClient.Stop();
            }
            try
            {
                //TcpListenerClient = new TcpListener(IPAddress.Parse(serverIp), serverPort); 127.0.0.1 是本地的回环的ip，也就不接受外界来的ip
                TcpListenerClient = new TcpListener(IPAddress.Any,serverPort);  //
                TcpListenerClient.Start();
                hsServer.ShowDebug("TcpListenerServer-开始侦听Tcp本机port：" +  serverPort);

                TcpListenerClient.BeginAcceptTcpClient(DoAcceptTcpClientCallback, TcpListenerClient);                
            }
            catch (SocketException e)
            {
                TcpListenerClient.Stop();
                Console.WriteLine("SocketException: {0}", e);
                hsServer.ShowDebug("TcpListenerServer-开始侦听Tcp本机ip/port：" +  serverPort +"异常："+e.Message);
            }
        }

        private void DoAcceptTcpClientCallback(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;
            TcpClient client = listener.EndAcceptTcpClient(ar);
            listener.BeginAcceptTcpClient(DoAcceptTcpClientCallback, listener);
            NetworkStream stream = client.GetStream();
            StringBuilder data = new StringBuilder();
            byte[] bytes = new byte[BuffRcv];
            // Loop to receive all the data sent by the client.
            try
            {
                do
                {
                    int i = stream.Read(bytes, 0, bytes.Length);
                    data.Append(System.Text.Encoding.UTF8.GetString(bytes, 0, i));
                    bytes = new byte[BuffRcv];
                }
                while (stream.DataAvailable);
                string rcvClient = data.ToString();
                hsServer.ShowDebug("Server收到客户端消息：" + client.Client.RemoteEndPoint + "|--" + rcvClient);
                string rcv = Event_FromJson?.Invoke(rcvClient);
                byte[] msg = System.Text.Encoding.UTF8.GetBytes(rcv);

                // Send back a response.
                if (stream.CanWrite)
                {
                    stream.Write(msg, 0, msg.Length);   //没有加判断时候，调试报错？？？ Length 连接已经关闭？  
                }
                hsServer.ShowDebug("返回客户端消息：" + client.Client.RemoteEndPoint + "|--" + rcv);
            }
            catch (Exception ex)
            {
                string ipPort = "";
                if (client != null)
                {
                    ipPort = client.ToString();
                }
                hsServer.ShowDebug("接收的Tcp连接出现异常（可能在服务器返回数据时连接断开了)：" + ipPort + "\r\n" + ex.Message);
            }
            finally
            {
                stream.Close();
                // Shutdown and end connection
                //client.SendTimeout = 3000;
                //client.ReceiveTimeout = 3000;
                client.Close();  //Tcp/Upd测试工具
            }
        }

         
    }
}
