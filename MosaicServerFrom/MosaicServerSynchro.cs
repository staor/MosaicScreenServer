
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Utils;

namespace MosaicServerFrom
{
    public class MosaicServerSynchro
    {
        //服务器发送同步消息到组播中
        public static string ServerToSynchroSendMultiIp = "234.234.1.3";
        public static int ServerToSynchroSendMultiPort = 32108;
        private static IPEndPoint multicastServer = null;
        public static UdpClient ServerToSendMultiUdpClient = null;
        private static object lockObj = new object();  

        public MosaicServerSynchro()
        {

        }
        public MosaicServerSynchro(string ip,int port)
        {
            ServerToSynchroSendMultiIp = ip;
            ServerToSynchroSendMultiPort = port;
        }

        public void ServerToSynchroSendMulticastMsg(string msg)  //往组播里发送数据
        {
            lock (lockObj)
            {
                try
                {
                    //IPAddress groupAddr = IPAddress.Parse(LocalIP);
                    //IPEndPoint groupEP = new IPEndPoint(groupAddr, ChatSendMultiPort);
                    if (multicastServer == null)
                    {
                        multicastServer = new IPEndPoint(IPAddress.Parse(ServerToSynchroSendMultiIp), ServerToSynchroSendMultiPort); //发送端口6665 与接收端口尽量不一致，因接收端口是一直侦听
                    }
                    //client = new UdpClient(groupEP);
                    if (ServerToSendMultiUdpClient == null)
                    {
                        ServerToSendMultiUdpClient = new UdpClient();
                        ServerToSendMultiUdpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);//需要？
                        ServerToSendMultiUdpClient.JoinMulticastGroup(IPAddress.Parse(ServerToSynchroSendMultiIp));
                        //ServerToSendMultiUdpClient.EnableBroadcast = true;  //待研  发送后关闭退出？ 或退出组播
                    }

                    byte[] bytes = Encoding.UTF8.GetBytes(msg);
                    ServerToSendMultiUdpClient.Send(bytes, bytes.Length, multicastServer);
                    //ShowDebug("发送组播消息："+ChatMultiIp+":"+ChatSendMultiPort+"-"+msg);
                    //client.Close(); //一直存在 一直需要发送信息
                    hsServer.ShowDebug("服务器发送组播同步消息：" + ServerToSynchroSendMultiIp + ":" + ServerToSynchroSendMultiPort + "\r\n" + msg);

                }
                catch (Exception ex)
                {
                    hsServer.ShowDebug("UdpClient.Send组播异常--" + ServerToSynchroSendMultiIp + ":" + ServerToSynchroSendMultiPort + "\r\n" + ex.Message);
                    if (ServerToSendMultiUdpClient != null)
                    {
                        ServerToSendMultiUdpClient.DropMulticastGroup(IPAddress.Parse(ServerToSynchroSendMultiIp));
                        ServerToSendMultiUdpClient.Close();
                        ServerToSendMultiUdpClient = null;
                    }
                    multicastServer = null;
                }
            }
        }
    }
}
