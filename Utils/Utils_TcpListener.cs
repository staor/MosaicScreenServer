using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LoggerHelper;

namespace Utils
{
    public class Utils_TcpListener
    {
         
        public CancellationTokenSource cts = new CancellationTokenSource();
        public int BuffRcv = 1024;
        public TcpListener TcpListenerClient = null;
        /// <summary>
        /// 默认Utf8
        /// </summary>
        public Encoding Encoding = Encoding.UTF8; 
        public Func<string, string> Event_GetStringFromTcp; 
        private int Port { get; set; }
        public void TcpListenerServer(int serverPort)
        {
            Port = serverPort;
            Task task = Task.Factory.StartNew(() =>
            {
                if(TcpListenerClient != null)
                {
                    TcpListenerClient.Stop();
                }
                try
                {
                    //TcpListenerClient = new TcpListener(IPAddress.Parse(serverIp), serverPort); 127.0.0.1 是本地的回环的ip，也就不接受外界来的ip
                    TcpListenerClient = new TcpListener(IPAddress.Any, serverPort);  //
                    TcpListenerClient.Start();
                    Logger.Info("TcpListenerServer-开始侦听Tcp本机port：" + serverPort);
                    TcpRcvMultiMsgAsyncBackAsync(TcpListenerClient);                     
                }
                catch (SocketException e)
                {
                    TcpListenerClient.Stop();
                     
                    Logger.Error("TcpListenerServer-开始侦听Tcp本机ip/port：" + serverPort + "异常：" + e.Message);
                }
            }/*, TaskCreationOptions.LongRunning */);
            //task.Start();            
        }
        private void TcpRcvMultiMsgAsyncBackAsync(TcpListener listener)
        {
            try
            {
                TcpClient client = listener.AcceptTcpClient();                 
                if (cts.IsCancellationRequested)
                {
                    return;
                }
                Task.Run(() =>
                {
                    TcpRcvMultiMsgAsyncBackAsync(listener);
                }, cts.Token);
                NetworkStream stream = client.GetStream();
                List<byte> data = new List<byte>();
                byte[] bytes = new byte[BuffRcv];
                // Loop to receive all the data sent by the client.
                try
                {
                    do
                    {
                        int i = stream.Read(bytes, 0, bytes.Length);
                        if (i < BuffRcv)
                        {
                            byte[] temp = new byte[i];
                            Buffer.BlockCopy(bytes, 0, temp, 0, i);
                            data.AddRange(temp);
                        }
                        else
                        {
                            data.AddRange(bytes);
                        }
                        bytes = new byte[BuffRcv];
                    }
                    while (stream.DataAvailable);
                    string rcvClient = Encoding.GetString(data.ToArray());
                    Logger.Debug("Server收到客户端消息：" + client.Client.RemoteEndPoint + "|--" + rcvClient);
                    string rcv = Event_GetStringFromTcp?.Invoke(rcvClient);

                    // Send back a response.
                    if (!string.IsNullOrEmpty(rcv))
                    {
                        if (stream.CanWrite)
                        {
                            byte[] msg = Encoding.GetBytes(rcv);
                            stream.Write(msg, 0, msg.Length);   //没有加判断时候，调试报错？？？ Length 连接已经关闭？  
                            Logger.Debug("返回客户端消息：" + client.Client.RemoteEndPoint + "|--" + rcv);
                        }                        
                    }     
                }
                catch (Exception ex)
                {
                    string ipPort = "";
                    if (client != null)
                    {
                        ipPort = client.ToString();
                    }
                    Logger.Error("接收的Tcp连接出现异常（可能在服务器返回数据时连接断开了)：" + ipPort + "\r\n" + ex.Message);
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
            catch (Exception ex)
            {
                //if (client != null)
                //{
                //    client.Close();
                //}
                hsServer.ShowDebug("TcpRcvMultiMsgAsyncBackAsync-TcpListener侦听异常" + ex.Message);
                //return;
                //throw;
            }
        }

    }
    //public class Utils_TcpListener2
    //{
    //    public int BuffRcv = 1024;
    //    public TcpListener TcpListenerClient = null;
    //    /// <summary>
    //    /// 默认Utf8
    //    /// </summary>
    //    public Encoding Encoding = Encoding.UTF8;
    //    public Func<string, string> Event_GetStringFromTcp;

    //    public bool Initial(int port)
    //    {
    //        bool isOk = true;
    //        Thread threadListener = new Thread(new ThreadStart(delegate
    //        {
    //            TcpListenerServer(port);
    //        }));
    //        threadListener.IsBackground = true;
    //        threadListener.Start();
    //        return isOk;
    //    }

    //    public void TcpListenerServer(int serverPort)
    //    {
    //        if (TcpListenerClient != null)
    //        {
    //            TcpListenerClient.Stop();
    //        }
    //        try
    //        {
    //            //TcpListenerClient = new TcpListener(IPAddress.Parse(serverIp), serverPort); 127.0.0.1 是本地的回环的ip，也就不接受外界来的ip
    //            TcpListenerClient = new TcpListener(IPAddress.Any, serverPort);  //
    //            TcpListenerClient.Start();
    //            Logger.Info("TcpListenerServer-开始侦听Tcp本机port：" + serverPort);

    //            TcpListenerClient.BeginAcceptTcpClient(DoAcceptTcpClientCallback, TcpListenerClient);
    //        }
    //        catch (SocketException e)
    //        {
    //            TcpListenerClient.Stop();
    //            Logger.Error("TcpListenerServer-开始侦听Tcp本机ip/port：" + serverPort + "异常：" + e.Message);
    //        }
    //    }

    //    private void DoAcceptTcpClientCallback(IAsyncResult ar)
    //    {
    //        TcpListener listener = (TcpListener)ar.AsyncState;
    //        TcpClient client = listener.EndAcceptTcpClient(ar);
    //        listener.BeginAcceptTcpClient(DoAcceptTcpClientCallback, listener);
    //        NetworkStream stream = client.GetStream();
    //        List<byte> data = new List<byte>();
    //        byte[] bytes = new byte[BuffRcv];
    //        // Loop to receive all the data sent by the client.
    //        try
    //        {
    //            do
    //            {
    //                int i = stream.Read(bytes, 0, bytes.Length);
    //                if (i < BuffRcv)
    //                {
    //                    byte[] temp = new byte[i];
    //                    Buffer.BlockCopy(bytes, 0, temp, 0, i);
    //                    data.AddRange(temp);
    //                }
    //                else
    //                {
    //                    data.AddRange(bytes);
    //                }
    //                bytes = new byte[BuffRcv];
    //            }
    //            while (stream.DataAvailable);
    //            string rcvClient = Encoding.GetString(data.ToArray());
    //            Logger.Debug("Server收到客户端消息：" + client.Client.RemoteEndPoint + "|--" + rcvClient);
    //            string rcv = Event_GetStringFromTcp?.Invoke(rcvClient);
    //            byte[] msg = Encoding.GetBytes(rcv);

    //            // Send back a response.
    //            if (stream.CanWrite)
    //            {
    //                stream.Write(msg, 0, msg.Length);   //没有加判断时候，调试报错？？？ Length 连接已经关闭？  
    //            }
    //            Logger.Debug("返回客户端消息：" + client.Client.RemoteEndPoint + "|--" + rcv);
    //        }
    //        catch (Exception ex)
    //        {
    //            string ipPort = "";
    //            if (client != null)
    //            {
    //                ipPort = client.ToString();
    //            }
    //            Logger.Error("接收的Tcp连接出现异常（可能在服务器返回数据时连接断开了)：" + ipPort + "\r\n" + ex.Message);
    //        }
    //        finally
    //        {
    //            stream.Close();
    //            // Shutdown and end connection
    //            //client.SendTimeout = 3000;
    //            //client.ReceiveTimeout = 3000;
    //            client.Close();  //Tcp/Upd测试工具
    //        }
    //    }
    //}
}
