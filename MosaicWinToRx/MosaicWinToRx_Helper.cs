
using LoggerHelper;

using RxNS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace MosaicWinToRx
{
    public static class MosaicWinToRx_Helper
    {

        public static string ServerToSynchroSendMultiIp = "234.234.0.1";
        public static int ServerToSynchroSendMultiPort = 32110;
        private static IPEndPoint multicastServer = null;
        private static UdpClient ServerToSendMultiUdpClient = null;
        private static object lockObj = new object();
        public static void Load(string multiIp,int mulitPort)
        {
            ServerToSynchroSendMultiIp = multiIp;
            ServerToSynchroSendMultiPort = mulitPort;         
        }

        public static void ServerToRxSendMulticastMsg(string msg)  //往组播里发送数据
        {
            Task.Run(() =>
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
                            ServerToSendMultiUdpClient.JoinMulticastGroup(IPAddress.Parse(ServerToSynchroSendMultiIp));

                        }

                        byte[] bytes = Encoding.UTF8.GetBytes(msg);
                        ServerToSendMultiUdpClient.Send(bytes, bytes.Length, multicastServer);
                        //ShowDebug("发送组播消息："+ChatMultiIp+":"+ChatSendMultiPort+"-"+msg);
                        //client.Close(); //一直存在 一直需要发送信息
                        Logger.Info("服务端》Rx端发送组播同步消息：" + ServerToSynchroSendMultiIp + ":" + ServerToSynchroSendMultiPort + "\r\n" + msg);

                    }
                    catch (Exception ex)
                    {
                        Logger.Info("服务端》Rx端  UdpClient.Send组播异常--" + ServerToSynchroSendMultiIp + ":" + ServerToSynchroSendMultiPort + "\r\n" + ex.Message);
                        if (ServerToSendMultiUdpClient != null)
                        {
                            ServerToSendMultiUdpClient.DropMulticastGroup(IPAddress.Parse(ServerToSynchroSendMultiIp));
                            ServerToSendMultiUdpClient.Close();
                            ServerToSendMultiUdpClient = null;
                        }
                        multicastServer = null;
                    }
                }
            });
            
        }
        public static string SendRxBind(RxInfo info,byte[] data)
        {
            string rcv = "";
            try
            {
                TcpClient client = new TcpClient(info.Ip4, info.Port4);
                client.ReceiveTimeout = 1000;
                NetworkStream stream = client.GetStream();
                stream.Write(data, 0, data.Length);
                Byte[] buffer = new byte[1024*2];
                Int32 bytes = stream.Read(buffer, 0, buffer.Length);
                byte[] rcvBuffer = new byte[bytes];
                Buffer.BlockCopy(buffer, 0, rcvBuffer, 0, bytes);
                stream.Close();
                client.Close();
                using (var ms = new MemoryStream(rcvBuffer))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(Json_Send_Rx_Bind));
                    Json_Send_Rx_Bind rcvJs = (Json_Send_Rx_Bind)deseralizer.ReadObject(ms);// //反序列化ReadObject
                    if (rcvJs.end.Err_code != "0")
                    {
                        rcv = rcvJs.end.Err_str;
                    }
                }
                Logger.Info($"SendRxBind-已发送IP/Port:{info.Ip4}:{info.Port4}");
                return rcv;
            }
            catch (Exception ex)
            {
                Logger.Error("SendRxBind-异常(RxId)："+info.Id+"-"+info.Ip4+":"+info.Port4+" || "+ex.Message);
                rcv = info.Id+"：Rx重设绑定连接失败 ";
            }
            return rcv;
        }
    }
}
