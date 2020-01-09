
using ConfigServer;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using TxNS;
using Utils;

namespace FromTxRxServer
{
    public delegate void RcvMulticastMsgDelegate(string msg);
    class MosaicScreenToTxRxServer_Helper
    {
        public static string StrHeartBeat = "heartbeat";
        public static async Task<List<Tx>> GetHsTxConfig(List<string> txNames, Dictionary<string, Tx> dic = null)
        {
            List<Tx> temp = new List<Tx>();
            StringBuilder sb1 = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            StringBuilder txes = new StringBuilder();
            if (txNames.Count > 0)
            {
                foreach (var item in txNames)
                {
                    txes.Append(item + ",");
                }
                txes.Remove(txes.Length - 1, 1);
            }
            else
            {
                txes.Append("all");
            }

            LIJsonFunction.SendGetHsTxConfigToIdNameJsonFormat f = new LIJsonFunction.SendGetHsTxConfigToIdNameJsonFormat
            { id = "y", name = "y", ip = "y", port = "y", online = "y", resolution = "y", usb = "y", devType = "y", version = "y", rate = "y" };
            LIJsonFunction.SendGetHsTxConfigJson sj = new LIJsonFunction.SendGetHsTxConfigJson { json_header = "hscmd-get-hs-tx-config-" + txes, format = f };
            var jsonString = stringify(sj);
            try
            {
                byte[] rev = await hsServer.Get_TCP_string_UTF8_Async(hsConfig.serverIp, hsConfig.serverPort, 0, jsonString);
                //hsServer.ShowDebug("GetHsTxConfig-接受查询Tx返回数量：" + rev.Length + "|" + Encoding.UTF8.GetString(rev));
                using (var ms = new MemoryStream(rev))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(LIJsonFunction.RecGetHsTxConfigJson));
                    LIJsonFunction.RecGetHsTxConfigJson model = (LIJsonFunction.RecGetHsTxConfigJson)deseralizer.ReadObject(ms);// //反序列化ReadObject
                    LIJsonFunction.RecGetHsTxConfigToIdNameJsonElement[] parameter = model.format.parameter;

                    Dictionary<string, LIJsonFunction.RecGetHsTxConfigToIdNameJsonElement> dicTemp = parameter.ToDictionary(p => p.id);
                    if (txNames.Count > 0)  //自定义指定查询Tx
                    {
                        for (int i = 0; i < txNames.Count; i++)
                        {
                            if (dicTemp.ContainsKey(txNames[i]))
                            {

                                LIJsonFunction.RecGetHsTxConfigToIdNameJsonElement jsonTx = dicTemp[txNames[i]];
                                Tx tx = null;
                                if (Tx.DicTx.ContainsKey(jsonTx.id))
                                {
                                    tx = Tx.DicTx[jsonTx.id];
                                }
                                else
                                {
                                    tx = new Tx(jsonTx.id);
                                }
                                tx.Name = jsonTx.name;
                                if (jsonTx.ip != null)
                                {
                                    tx.Ip = jsonTx.ip;
                                }
                                else
                                {
                                    tx.Ip = "";
                                }

                                if (jsonTx.online != null)
                                {
                                    tx.Online = jsonTx.online;
                                }
                                else
                                {
                                    tx.Online = "";
                                }
                                if (jsonTx.usb != null)
                                {
                                    tx.Usb = jsonTx.usb;
                                }
                                else
                                {
                                    tx.Usb = "";
                                }
                                if (jsonTx.resolution != null)
                                {
                                    tx.Resolution = jsonTx.resolution;
                                }
                                else
                                {
                                    tx.Resolution = "";
                                }
                                if (jsonTx.rate != null)
                                {
                                    tx.Rate = jsonTx.rate;
                                }
                                else
                                {
                                    tx.Rate = "";
                                }
                                if (jsonTx.devType != null)
                                {
                                    tx.DevType = jsonTx.devType;
                                }
                                else
                                {
                                    tx.DevType = "";
                                }
                                if (jsonTx.version != null)
                                {
                                    tx.Version = jsonTx.version;
                                }
                                else
                                {
                                    tx.Version = "";
                                }
                                temp.Add(tx);
                                //if (!dic.ContainsKey(txNames[i]))
                                //{
                                //    dic.Add(tx.Id, tx);
                                //}
                            }
                            else
                            {
                                Tx tx = new Tx();
                                tx.Id = txNames[i];
                                tx.Online = "";
                                tx.Resolution = "";
                                tx.Rate = "";
                                tx.Usb = "";
                                tx.Name = "无";
                                tx.Version = "";
                                tx.DevType = "";
                                temp.Add(tx);
                                //temp.Add(null);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < parameter.Length; i++)
                        {
                            LIJsonFunction.RecGetHsTxConfigToIdNameJsonElement jsonTx = parameter[i];
                            Tx tx = null;
                            if (Tx.DicTx.ContainsKey(jsonTx.id))
                            {
                                tx = Tx.DicTx[jsonTx.id];
                            }
                            else
                            {
                                tx = new Tx(jsonTx.id);
                            }
                            tx.Name = jsonTx.name;
                            if (jsonTx.ip != null)
                            {
                                tx.Ip = jsonTx.ip;
                            }
                            else
                            {
                                tx.Ip = "";
                            }
                            if (jsonTx.online != null)
                            {
                                tx.Online = jsonTx.online;
                            }
                            else
                            {
                                tx.Online = "";
                            }
                            if (jsonTx.usb != null)
                            {
                                tx.Usb = jsonTx.usb;
                            }
                            else
                            {
                                tx.Usb = "";
                            }
                            if (jsonTx.resolution != null)
                            {
                                tx.Resolution = jsonTx.resolution;
                            }
                            else
                            {
                                tx.Resolution = "";
                            }
                            if (jsonTx.rate != null)
                            {
                                tx.Rate = jsonTx.rate;
                            }
                            else
                            {
                                tx.Rate = "";
                            }
                            if (jsonTx.version != null)
                            {
                                tx.Version = jsonTx.version;
                            }
                            else
                            {
                                tx.Version = "";
                            }
                            if (jsonTx.devType != null)
                            {
                                tx.DevType = jsonTx.devType;
                            }
                            else
                            {
                                tx.DevType = "";
                            }

                            temp.Add(tx);
                            if (dic != null)
                            {
                                if (!dic.ContainsKey(txNames[i]))
                                {
                                    dic.Add(tx.Id, tx);
                                }
                            }
                        }
                    }

                }

                foreach (string item in txNames)
                {
                    sb1.Append(item + "-");
                }
                foreach (Tx item in temp)
                {
                    if (item == null)
                    {
                        sb2.Append("-");
                        continue;
                    }
                    sb2.Append(item.Id + "-");
                }
                //hsServer.ShowDebug("查询Tx.id" + sb1 + " 有返回数量：" + temp.Count + "\r\n" + sb2);
            }
            catch (Exception ex)
            {
                hsServer.ShowDebug("查询Tx.id" + sb1 + " 异常。返回：\r\n" + ex.Message);
                //throw;
            }
            return temp;
        }

        public static async Task<Tx> GetHsTxConfigForOne(string txName)
        {

            Tx tx = null;
            StringBuilder sb1 = new StringBuilder();

            if (string.IsNullOrEmpty(txName))
            {
                return tx;
            }
            LIJsonFunction.SendGetHsTxConfigToIdNameJsonFormat f = new LIJsonFunction.SendGetHsTxConfigToIdNameJsonFormat
            { id = "y", name = "y", ip = "y", port = "y", online = "y", resolution = "y", usb = "y", devType = "y", version = "y", rate = "y" };
            LIJsonFunction.SendGetHsTxConfigJson sj = new LIJsonFunction.SendGetHsTxConfigJson { json_header = "hscmd-get-hs-tx-config-" + txName, format = f };
            var jsonString = stringify(sj);
            try
            {
                byte[] rev = await hsServer.Get_TCP_string_UTF8_Async(hsConfig.serverIp, hsConfig.serverPort, 0, jsonString);
                //hsServer.ShowDebug("GetHsTxConfig-接受查询Tx返回数量：" + rev.Length + "|" + Encoding.UTF8.GetString(rev));
                using (var ms = new MemoryStream(rev))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(LIJsonFunction.SendGetHsTxConfigJson));
                    LIJsonFunction.SendGetHsTxConfigJson model = (LIJsonFunction.SendGetHsTxConfigJson)deseralizer.ReadObject(ms);// //反序列化ReadObject                   
                    LIJsonFunction.SendGetHsTxConfigToIdNameJsonFormat jsonTx = model.format;

                    if (Tx.DicTx.ContainsKey(jsonTx.id))
                    {
                        tx = Tx.DicTx[jsonTx.id];
                    }
                    else
                    {
                        tx = new Tx(jsonTx.id);
                    }
                    tx.Name = jsonTx.name;
                    if (jsonTx.ip != null)
                    {
                        tx.Ip = jsonTx.ip;
                    }
                    else
                    {
                        tx.Ip = "";
                    }
                    if (jsonTx.online != null)
                    {
                        tx.Online = jsonTx.online;
                    }
                    else
                    {
                        tx.Online = "";
                    }
                    if (jsonTx.usb != null)
                    {
                        tx.Usb = jsonTx.usb;
                    }
                    else
                    {
                        tx.Usb = "";
                    }
                    if (jsonTx.resolution != null)
                    {
                        tx.Resolution = jsonTx.resolution;
                    }
                    else
                    {
                        tx.Resolution = "";
                    }
                    if (jsonTx.rate != null)
                    {
                        tx.Rate = jsonTx.rate;
                    }
                    else
                    {
                        tx.Rate = "";
                    }
                    if (jsonTx.version != null)
                    {
                        tx.Version = jsonTx.version;
                    }
                    else
                    {
                        tx.Version = "";
                    }
                    if (jsonTx.devType != null)
                    {
                        tx.DevType = jsonTx.devType;
                    }
                    else
                    {
                        tx.DevType = "";
                    }
                }
            }
            catch (Exception ex)
            {
                hsServer.ShowDebug("查询Tx.id" + sb1 + " 异常。返回：\r\n" + ex.Message);
                //throw;
            }
            return tx;
        }

        public static string stringify(object jsonObject)
        {
            using (var ms = new MemoryStream())
            {
                new DataContractJsonSerializer(jsonObject.GetType()).WriteObject(ms, jsonObject);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        public static event RcvMulticastMsgDelegate RcvMulticastServerMsgEvent;
        public static string ServerMultiIp = "234.234.1.3";
        public static int ServerRcvMultiPort = 32008;
        public static UdpClient ServerMultiUdpClient = null;

        public static void ServerUdpMulticastListener(string multiIp, int multiPort)
        {
            //ParameterizedThreadStart p = new ParameterizedThreadStart(ReceivThreadEventHandler);
            //if (!IsChating)
            //{
            //    IsChating = true;
            //}
            UdpClient client = null;

            try
            {
                if (client == null)
                {
                    client = new UdpClient();
                    client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    client.Client.Bind(new IPEndPoint(IPAddress.Any, multiPort));

                    client.JoinMulticastGroup(IPAddress.Parse(multiIp));
                    ServerMultiUdpClient = client;
                    client.BeginReceive(new AsyncCallback(UdpRcvMultiMsgAsyncBack), client);

                }
                hsServer.ShowDebug("加入TxRx服务器同步信息组播频道：" + multiIp + ":" + multiPort);
            }
            catch (Exception ex)
            {
                if (client != null)
                {
                    client.Close();
                }
                hsServer.ShowDebug("加入TxRx服务端组播异常UdpMulticastServerListener：" + multiIp + ":" + multiPort + " || " + ex.Message);
                //throw;
            }
            finally
            {
                //if (client != null)
                //{
                //    client.Close();
                //}
            }


            //Thread th = new Thread(ReceivThreadEventHandler);
            //th.IsBackground = true;
            //th.Start();
        }
        private static void UdpRcvMultiMsgAsyncBack(IAsyncResult state)
        {
            UdpClient udpClient = null;
            try
            {
                udpClient = (UdpClient)state.AsyncState;
                IPEndPoint endPoint = (IPEndPoint)udpClient.Client.LocalEndPoint;

                byte[] bytes = udpClient.EndReceive(state, ref endPoint);
                string msg = Encoding.UTF8.GetString(bytes);  //HS的json格式为utf-8
                RcvMulticastServerMsgEvent?.BeginInvoke(msg, null, null);
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

        private static int heartBeatNo = 0; //调试信息显示次数
        public static async Task<bool> SendHeartBeatAsync()
        {
            bool isOk = false;
            string rcv = await hsServer.Get_TCP_string_Async(hsConfig.serverIp, hsConfig.serverPort, 0, StrHeartBeat);
            if (rcv.Length > 0)
            {
                isOk = true;
            }
            return isOk;
        }
    }
    //class MosaicScreenToTxRxServer_Helper
    //{
    //    public static string StrHeartBeat = "heartbeat";
    //    public static async Task<List<TxInfo>> GetHsTxConfig(List<string> txNames, Dictionary<string, TxInfo> dic = null)
    //    {
    //        List<TxInfo> temp = new List<TxInfo>();
    //        StringBuilder sb1 = new StringBuilder();
    //        StringBuilder sb2 = new StringBuilder();
    //        StringBuilder txes = new StringBuilder();
    //        if (txNames.Count > 0)
    //        {
    //            foreach (var item in txNames)
    //            {
    //                txes.Append(item + ",");
    //            }
    //            txes.Remove(txes.Length - 1, 1);
    //        }
    //        else
    //        {
    //            txes.Append("all");
    //        }

    //        LIJsonFunction.SendGetHsTxConfigToIdNameJsonFormat f = new LIJsonFunction.SendGetHsTxConfigToIdNameJsonFormat
    //        { id = "y", name = "y", ip = "y",port="y", online = "y", resolution = "y", usb = "y", devType = "y", version = "y",rate="y" };
    //        LIJsonFunction.SendGetHsTxConfigJson sj = new LIJsonFunction.SendGetHsTxConfigJson { json_header = "hscmd-get-hs-tx-config-" + txes, format = f };
    //        var jsonString = stringify(sj);
    //        try
    //        {
    //            byte[] rev = await hsServer.Get_TCP_string_UTF8_Async(hsConfig.serverIp, hsConfig.serverPort, 0, jsonString);
    //            //hsServer.ShowDebug("GetHsTxConfig-接受查询TxInfo返回数量：" + rev.Length + "|" + Encoding.UTF8.GetString(rev));
    //            using (var ms = new MemoryStream(rev))
    //            {
    //                DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(LIJsonFunction.RecGetHsTxConfigJson));
    //                LIJsonFunction.RecGetHsTxConfigJson model = (LIJsonFunction.RecGetHsTxConfigJson)deseralizer.ReadObject(ms);// //反序列化ReadObject
    //                LIJsonFunction.RecGetHsTxConfigToIdNameJsonElement[] parameter = model.format.parameter;

    //                Dictionary<string, LIJsonFunction.RecGetHsTxConfigToIdNameJsonElement> dicTemp = parameter.ToDictionary(p => p.id);
    //                if (txNames.Count > 0)  //自定义指定查询TxInfo
    //                {
    //                    for (int i = 0; i < txNames.Count; i++)
    //                    {
    //                        if (dicTemp.ContainsKey(txNames[i]))
    //                        {

    //                            LIJsonFunction.RecGetHsTxConfigToIdNameJsonElement jsonTx = dicTemp[txNames[i]];
    //                            TxInfo tx = null;
    //                            if (TxInfo.DicTxInfo.ContainsKey(jsonTx.id))
    //                            {
    //                                tx = TxInfo.DicTxInfo[jsonTx.id];
    //                            }
    //                            else
    //                            {
    //                                tx = new TxInfo(jsonTx.id);
    //                            }
    //                            tx.Name = jsonTx.name;
    //                            if (jsonTx.ip != null)
    //                            {
    //                                tx.Ip = jsonTx.ip;
    //                            }
    //                            else
    //                            {
    //                                tx.Ip = "";
    //                            }

    //                            if (jsonTx.online != null)
    //                            {
    //                                tx.Online = jsonTx.online;
    //                            }
    //                            else
    //                            {
    //                                tx.Online = "";
    //                            }
    //                            if (jsonTx.usb != null)
    //                            {
    //                                tx.Usb = jsonTx.usb;
    //                            }
    //                            else
    //                            {
    //                                tx.Usb = "";
    //                            }
    //                            if (jsonTx.resolution != null)
    //                            {
    //                                tx.Resolution = jsonTx.resolution;
    //                            }
    //                            else
    //                            {
    //                                tx.Resolution = "";
    //                            }
    //                            if (jsonTx.rate != null)
    //                            {
    //                                tx.Rate = jsonTx.rate;
    //                            }
    //                            else
    //                            {
    //                                tx.Rate = "";
    //                            }
    //                            if (jsonTx.devType != null)
    //                            {
    //                                tx.DevType = jsonTx.devType;
    //                            }
    //                            else
    //                            {
    //                                tx.DevType = "";
    //                            }
    //                            if (jsonTx.version != null)
    //                            {
    //                                tx.Version = jsonTx.version;
    //                            }
    //                            else
    //                            {
    //                                tx.Version = "";
    //                            }
    //                            temp.Add(tx);
    //                            //if (!dic.ContainsKey(txNames[i]))
    //                            //{
    //                            //    dic.Add(tx.Id, tx);
    //                            //}
    //                        }
    //                        else
    //                        {
    //                            TxInfo tx = new TxInfo();
    //                            tx.Id = txNames[i];
    //                            tx.Online = "";
    //                            tx.Resolution = "";
    //                            tx.Rate = "";
    //                            tx.Usb = "";
    //                            tx.Name = "无";
    //                            tx.Version = "";
    //                            tx.DevType = "";
    //                            temp.Add(tx);
    //                            //temp.Add(null);
    //                        }
    //                    }
    //                }
    //                else
    //                {
    //                    for (int i = 0; i < parameter.Length; i++)
    //                    {
    //                        LIJsonFunction.RecGetHsTxConfigToIdNameJsonElement jsonTx = parameter[i];
    //                        TxInfo tx = null;
    //                        if (TxInfo.DicTxInfo.ContainsKey(jsonTx.id))
    //                        {
    //                            tx = TxInfo.DicTxInfo[jsonTx.id];
    //                        }
    //                        else
    //                        {
    //                            tx = new TxInfo(jsonTx.id);
    //                        }
    //                        tx.Name = jsonTx.name;
    //                        if (jsonTx.ip != null)
    //                        {
    //                            tx.Ip = jsonTx.ip;
    //                        }
    //                        else
    //                        {
    //                            tx.Ip = "";
    //                        }
    //                        if (jsonTx.online != null)
    //                        {
    //                            tx.Online = jsonTx.online;
    //                        }
    //                        else
    //                        {
    //                            tx.Online = "";
    //                        }
    //                        if (jsonTx.usb != null)
    //                        {
    //                            tx.Usb = jsonTx.usb;
    //                        }
    //                        else
    //                        {
    //                            tx.Usb = "";
    //                        }
    //                        if (jsonTx.resolution != null)
    //                        {
    //                            tx.Resolution = jsonTx.resolution;
    //                        }
    //                        else
    //                        {
    //                            tx.Resolution = "";
    //                        }
    //                        if (jsonTx.rate != null)
    //                        {
    //                            tx.Rate = jsonTx.rate;
    //                        }
    //                        else
    //                        {
    //                            tx.Rate = "";
    //                        }
    //                        if (jsonTx.version != null)
    //                        {
    //                            tx.Version = jsonTx.version;
    //                        }
    //                        else
    //                        {
    //                            tx.Version = "";
    //                        }
    //                        if (jsonTx.devType != null)
    //                        {
    //                            tx.DevType = jsonTx.devType;
    //                        }
    //                        else
    //                        {
    //                            tx.DevType = "";
    //                        }

    //                        temp.Add(tx);
    //                        if (dic != null)
    //                        {
    //                            if (!dic.ContainsKey(txNames[i]))
    //                            {
    //                                dic.Add(tx.Id, tx);
    //                            }
    //                        }
    //                    }
    //                }

    //            }

    //            foreach (string item in txNames)
    //            {
    //                sb1.Append(item + "-");
    //            }
    //            foreach (TxInfo item in temp)
    //            {
    //                if (item == null)
    //                {
    //                    sb2.Append("-");
    //                    continue;
    //                }
    //                sb2.Append(item.Id + "-");
    //            }
    //            //hsServer.ShowDebug("查询TxInfo.id" + sb1 + " 有返回数量：" + temp.Count + "\r\n" + sb2);
    //        }
    //        catch (Exception ex)
    //        {
    //            hsServer.ShowDebug("查询TxInfo.id" + sb1 + " 异常。返回：\r\n" + ex.Message);
    //            //throw;
    //        }
    //        return temp;
    //    }


    //    public static string stringify(object jsonObject)
    //    {
    //        using (var ms = new MemoryStream())
    //        {
    //            new DataContractJsonSerializer(jsonObject.GetType()).WriteObject(ms, jsonObject);
    //            return Encoding.UTF8.GetString(ms.ToArray());
    //        }
    //    }

    //    public static event RcvMulticastMsgDelegate RcvMulticastServerMsgEvent;
    //    public static string ServerMultiIp = "234.234.1.3";
    //    public static int ServerRcvMultiPort = 32008;
    //    public static UdpClient ServerMultiUdpClient = null;

    //    public static void ServerUdpMulticastListener(string multiIp, int multiPort)
    //    {
    //        //ParameterizedThreadStart p = new ParameterizedThreadStart(ReceivThreadEventHandler);
    //        //if (!IsChating)
    //        //{
    //        //    IsChating = true;
    //        //}
    //        UdpClient client = null;

    //            try
    //            {
    //                if (client == null)
    //                {
    //                    client = new UdpClient();
    //                    client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
    //                    client.Client.Bind(new IPEndPoint(IPAddress.Any, multiPort));

    //                    client.JoinMulticastGroup(IPAddress.Parse(multiIp));
    //                    ServerMultiUdpClient = client;
    //                    client.BeginReceive(new AsyncCallback(UdpRcvMultiMsgAsyncBack), client);

    //                }
    //                hsServer.ShowDebug("加入TxRx服务器同步信息组播频道：" + multiIp + ":" + multiPort);
    //            }
    //            catch (Exception ex)
    //            {
    //                if (client != null)
    //                {
    //                    client.Close();
    //                }
    //                hsServer.ShowDebug("加入TxRx服务端组播异常UdpMulticastServerListener：" + multiIp + ":" + multiPort+" || " + ex.Message);
    //                //throw;
    //            }
    //            finally
    //            {
    //                //if (client != null)
    //                //{
    //                //    client.Close();
    //                //}
    //            }


    //        //Thread th = new Thread(ReceivThreadEventHandler);
    //        //th.IsBackground = true;
    //        //th.Start();
    //    }
    //    private static void UdpRcvMultiMsgAsyncBack(IAsyncResult state)
    //    {
    //        UdpClient udpClient = null;
    //        try
    //        {
    //            udpClient = (UdpClient)state.AsyncState;
    //            IPEndPoint endPoint = (IPEndPoint)udpClient.Client.LocalEndPoint;

    //            byte[] bytes = udpClient.EndReceive(state, ref endPoint);
    //            string msg = Encoding.UTF8.GetString(bytes);  //HS的json格式为utf-8
    //            RcvMulticastServerMsgEvent?.BeginInvoke(msg, null, null);
    //            //App.Current.Dispatcher.Invoke(RcvMulticastServerMsgEvent, msg);
    //            //ShowDebug("接受组播消息：" + msg);
    //            //Console.WriteLine(value);
    //            //// 在这里使用异步委托来处理接收到的数组，并再次启动接收
    //            var ar = udpClient.BeginReceive(new AsyncCallback(UdpRcvMultiMsgAsyncBack), udpClient);
    //        }
    //        catch (Exception ex)
    //        {
    //            if (udpClient != null)
    //            {
    //                udpClient.Close();
    //            }
    //            hsServer.ShowDebug("UdpRcvMultiMsgAsyncBack-Udp组播侦听异常退出" + ex.Message);
    //            return;
    //            //throw;
    //        }
    //    }

    //    private static int heartBeatNo = 0; //调试信息显示次数
    //    public static async Task<bool> SendHeartBeatAsync()
    //    {
    //        bool isOk = false;
    //        string rcv = await hsServer.Get_TCP_string_Async(hsConfig.serverIp, hsConfig.serverPort, 0, StrHeartBeat);
    //        if (rcv.Length>0)
    //        {
    //            isOk = true;
    //        }
    //        return isOk;
    //    }
    //}
}
