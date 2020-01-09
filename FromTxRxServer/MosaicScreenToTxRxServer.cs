
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TxNS;
using Utils;

namespace FromTxRxServer
{
    
    public class TxRxServer
    {
        public static bool IsAllTxRcvOk = true;
        private static bool IsAllTxOffLineToSynchro = false;  //是否第一次上下线发送了所有同步通知

        public static bool IsNeedToReconnect = false;  //是否需要重连
        public static bool IsConnected = false;//当前的连接是否正常
        public static bool IsSendAllTxInfoOff = false; //是否发出了所有TxInfo掉线通知
        public static bool IsSendAllTxInfoOnline = false; //是否发出了所有上线更新TxInfo的通知

        private static AutoResetEvent autoEvent = new AutoResetEvent(false);
        private static Timer TimerUpdateTxInfo { get; set; }
        public static Action<TxInfo> ActionTxInfoOnline { get; set; }
        public static Action<TxInfo> ActionTxInfoAdd { get; set; }
        public static void LoadSynchroFromTxRxServer(string ip,int port)
        {
            MosaicScreenToTxRxServer_Helper.RcvMulticastServerMsgEvent += MulticastServerRcvMsg;  //都加入组播，侦探消息
            MosaicScreenToTxRxServer_Helper.ServerUdpMulticastListener(ip, port); //加入TxRx服务器同步组播里

        }


        //1：同步拼接屏层现模式  view::screenGroup::name   screenGroup ： 	大屏ID  name： 呈现模式名字  view::1::mode_1
        //1：信号切换同步         例子:swi::0001::vuasr::5001,5002,5003,5004   此时5001,5002,5003,5004为组合一个拼接框,当有其中一个rx切信号0001时,   另外若为单个切换0001->5002(非拼接框),只有swi::vuasr::0001::5002
        //swi::TX_ID::vuasr::RX_1,RX_2,RX_3,RX_4...  vuasr自由组合
        //swi::TX_ID::vuasr::all                   一切所有rx
        public static async void MulticastServerRcvMsg(string msg) //服务器的组播同步消息:udp 234.234.1.3 32008  同步状态/husion,所有设备
        {
            //hsServer.ShowDebug("UI获取TxRxServer组播消息：" + msg);  //测试环境常出现
            //App.Current.Dispatcher.BeginInvoke((Action)(() =>
            //{
                string[] msges = msg.Split("::".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (msges.Length < 1)
                {
                    return;
                }

            if (msges[0] == "offline") //offline::0002    0002：tx/rx的 id
            {
                
                if (msges.Length >= 2)
                {
                    string txId = msges[1];
                    if (TxInfo.DicTxInfo.ContainsKey(txId))
                    {
                        TxInfo txInfo = TxInfo.DicTxInfo[txId];
                        txInfo.Online = "n";
                        txInfo.Resolution = "";
                        ActionTxInfoOnline?.Invoke(txInfo);

                        hsServer.ShowDebug("UI获取TxRxServer组播消息：" + msg);
                    }
                }
            }
            else if (msges[0] == "online")  
            {

                //online::0002::1G-M::TX 
                if (msges.Length >= 4 && msges[3].Trim().ToUpper() == "TX")
                {
                    string txId = msges[1];
                    string online1 = DateTime.Now.ToString();
                    hsServer.ShowDebug("当前收到上线通知：" + online1 + "----" + msg);

                    await Task.Delay(3000);  //resolution不会立即更新
                    var item = await MosaicScreenToTxRxServer_Helper.GetHsTxConfigForOne(txId);
                    if (item == null)
                    {
                        hsServer.ShowDebug("GetHsTxConfigForOne-查询返回为空：" + txId);
                        return;
                    }
                    string online2 = DateTime.Now.ToString();
                    hsServer.ShowDebug("当前信号resolution查询结果时间：" + online2 + "----" + item.Resolution);

                    if (item.DevType.Contains("MULTIv4") | item.DevType.Contains("MULTIv5"))  //TX-KVM-MULTIv4-H-SL  :表示为双流Tx 设备类型 4代1080P TX-KVM-MULTIv5-H-SL  :表示为双流Tx 设备类型 5代
                    {
                        hsServer.ShowDebug("UI获取TxRxServer组播双流Tx上线消息：" + msg);

                        TxInfo txInfo = null;
                        if (TxInfo.DicTxInfo.ContainsKey(item.Id))
                        {
                            txInfo = TxInfo.DicTxInfo[item.Id];
                        }
                        else
                        {
                            txInfo = new TxInfo(item.Id);
                        }
                        txInfo.Name = item.Name;
                        txInfo.DevType = item.DevType;
                        txInfo.Ip = item.Ip;
                        txInfo.TcpPort = item.Port;
                        txInfo.Version = item.Version;
                        txInfo.Online = item.Online;
                        txInfo.Resolution = item.Resolution;
                        txInfo.Rate = item.Rate;
                        txInfo.EncodeFormat = item.EncodeFormat;
                        txInfo.Ts_addr = item.MultiIp;
                        txInfo.Ts_port = item.MultiPort;

                        string[] arrayIp = item.Ip.Split('.');
                        if (arrayIp.Length == 4)
                        {
                            txInfo.Udp_addr = "228.228." + arrayIp[2] + "." + arrayIp[3];  //169.254.100.1 >>228.228.100.1  默认端口20100
                            txInfo.Stream = @"rtsp://200.200." + arrayIp[2] + "." + arrayIp[3] + @":554/live/stream";            //rtsp://200.200.100.1:554/live/stream
                        }
                        else
                        {
                            hsServer.ShowDebug("Tx新增上线同步通知-Ip解析长度不为4？");
                        }
                        //hsServer.ShowDebug("UI获取TxRxServer组播消息：" + msg);
                        //hsServer.ShowDebug("Tx上线查询Resolution：" + item.Resolution);
                        ActionTxInfoOnline?.Invoke(txInfo);
                    }
                    else
                    {
                        hsServer.ShowDebug("查询双流Tx返回：" + txId + "为空");
                    }
                }
            }

            #region 其他同步---------


            //if (msges[0] == "view")
            //{
            //    if (msges.Length == 3)
            //    {
            //        try
            //        {
            //            string sg = msges[1];
            //            string tempCurrentViewMode = msges[2];


            //            if (hsServer.DicIdToSplitSystem.ContainsKey(sg))
            //            {
            //                SplitSystem ss = hsServer.DicIdToSplitSystem[sg];

            //                ss.SwitchViewMode(tempCurrentViewMode, ss.ScreenType);//--------同步切换idtx，但是并不播放//只要有切换模式指令，则重新设置模式布局和idtx
            //                //if (currentSplitSystem == ss)///----------------
            //                //{
            //                //    ss.PlayViewMode();
            //                //}   
            //                ss.PlayViewMode();
            //            }
            //            else
            //            {
            //                hsServer.ShowDebug("当前程序中没有储存同步的拼接墙系统id：" + sg);
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            hsServer.ShowDebug("同步组播数据异常:" + ex.Message);
            //            //throw;
            //        }
            //    }
            //}
            //else if (msges[0] == "swi")
            //{
            //    if (msges.Length == 4)
            //    {
            //        try
            //        {
            //            string swiType = msges[2];
            //            if (!swiType.Contains("v"))
            //            {
            //                return;
            //            }
            //            if (msges[3] == "all")
            //            {
            //                if (hsServer.streamPlayers != null)
            //                {
            //                    List<IPlayWin> tempCurrentPlayWins = new List<IPlayWin>();
            //                    foreach (var item in hsServer.streamPlayers.Players)  //设置所有预览卡为可使用空闲状态，实际有
            //                    {
            //                        foreach (var item2 in item.PlayWines)
            //                        {
            //                            tempCurrentPlayWins.Add(item2);
            //                        }
            //                        //tempCurrentPlayWins.AddRange(item.PlayWines);
            //                        item.PlayWines.Clear();
            //                        item.IdTx = "0000";
            //                    }
            //                    if (hsServer.streamPlayers.Players.Count > 1)  //把所有播放画面的窗口集中到第一个
            //                    {
            //                        hsServer.streamPlayers.Players[0].IdTx = msges[1];
            //                        hsServer.streamPlayers.Players[0].VP.Play();
            //                        foreach (var item in tempCurrentPlayWins)
            //                        {
            //                            item.IdTx = msges[1];
            //                            item.Player = hsServer.streamPlayers.Players[0];
            //                            hsServer.streamPlayers.Players[0].PlayWines.Add(item);
            //                        }
            //                        //hsServer.streamPlayers.Players[0].PlayWines.AddRange(tempCurrentPlayWins);

            //                    }
            //                    foreach (var item in Rx.DicRx.Values)
            //                    {
            //                        item.IdTx = msges[1];
            //                        //item.SwitchTx(msges[1], msges[2], false);
            //                    }
            //                }
            //                if (hsServer.tsPlayers != null)
            //                {
            //                    foreach (var item in Rx.DicRx.Values)
            //                    {
            //                        //item.IdTx = msges[1];
            //                        item.SwitchTx(msges[1], msges[2], false);
            //                    }
            //                }
            //                return;
            //            }
            //            else
            //            {
            //                string[] rxes = msges[3].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);    //非全部rx切换
            //                for (int i = 0; i < rxes.Length; i++)
            //                {
            //                    if (Rx.DicRx.ContainsKey(rxes[i]))   //更新维护列表
            //                    {
            //                        Rx rx = Rx.DicRx[rxes[i]];
            //                        rx.SwitchTx(msges[1], msges[2], false);
            //                    }
            //                    else
            //                    {
            //                        hsServer.ShowDebug("内存中Rx.DicRx没有存储此Rx:" + rxes[i]);
            //                    }
            //                }
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            hsServer.ShowDebug("同步组播数据切换tx--rx异常:" + ex.Message);
            //        }
            //    }
            //}

            //}));
            #endregion
        }

        public static async Task<List<TxInfo>> GetAllTxInfoAsync()
        {
            List<TxInfo> list = new List<TxInfo>();
            var ListTx = await MosaicScreenToTxRxServer_Helper.GetHsTxConfig(new List<string>());
            foreach (var item in ListTx)
            {
                if (item.DevType.Contains("MULTIv4") | item.DevType.Contains("MULTIv5"))  //TX-KVM-MULTIv4-H-SL  :表示为双流Tx 设备类型 4代1080P TX-KVM-MULTIv5-H-SL  :表示为双流Tx 设备类型 5代
                {
                    TxInfo txInfo = null;
                    if (TxInfo.DicTxInfo.ContainsKey(item.Id))
                    {
                        txInfo = TxInfo.DicTxInfo[item.Id];
                    }
                    else
                    {
                        txInfo = new TxInfo(item.Id);
                    }
                    txInfo.Name = item.Name;
                    txInfo.DevType = item.DevType;
                    txInfo.Ip = item.Ip;
                    txInfo.TcpPort = item.Port;
                    txInfo.Version = item.Version;
                    txInfo.Online = item.Online;
                    txInfo.Resolution = item.Resolution;
                    txInfo.Rate = item.Rate;
                    txInfo.EncodeFormat = item.EncodeFormat;
                    txInfo.Ts_addr = item.MultiIp;
                    txInfo.Ts_port = item.MultiPort;
                    
                    string[] arrayIp = item.Ip.Split('.');
                    if (arrayIp.Length == 4)
                    {
                        txInfo.Udp_addr = "228.228." + arrayIp[2] + "." + arrayIp[3];  //169.254.100.1 >>228.228.100.1  默认端口20100
                        txInfo.Stream = @"rtsp://200.200." + arrayIp[2] + "." + arrayIp[3] + @":554/live/stream";            //rtsp://200.200.100.1:554/live/stream
                    }
                    else
                    {
                        hsServer.ShowDebug("GetAllTxInfoAsync-Ip解析长度不为4？");
                    }
                    list.Add(txInfo);
                    
                }
            }
            hsServer.ShowDebug("GetAllTxInfoAsync-双流Tx数量：" + list.Count);
            return list;
        }

        public static void TimerUpdateAddTxInfo()
        {
            if (TimerUpdateTxInfo==null)
            {
                TimerUpdateTxInfo = new Timer(new TimerCallback(UpdateTxInfoAsync_FromHeartBeat), autoEvent, 0, 3000);
            }
        }
        private static void UpdateAddTxInfoAsync(object stateInfo)
        {
            AutoResetEvent auto = (AutoResetEvent)stateInfo;
            List<TxInfo> newListTxInfo = new List<TxInfo>();
            QueryNewTxInfoAsync(newListTxInfo, auto);
            //auto.WaitOne(3000, false);
            //TimerUpdateTxInfo.Change(0, 1000);
            //hsServer.ShowDebug("改变查询Tx间隔："+1000);

            auto.WaitOne(2000, true);
            if (newListTxInfo.Count==0)
            {
                if (IsAllTxRcvOk) //正常查询没有新增Tx
                {
                    if (IsAllTxOffLineToSynchro)  //突然正常上线查询有Tx，但无新增Tx
                    {
                        IsAllTxOffLineToSynchro = false;
                        foreach (var item in TxInfo.DicTxInfo)
                        {
                            //if (!string.IsNullOrEmpty(item.Value.Online)&&item.Value.Online!="n")  //正常在线的发出上线提示。
                            //{
                                newListTxInfo.Add(item.Value);
                            //}
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else  //连接不上TxRx服务器
                {
                    if (!IsAllTxOffLineToSynchro)
                    {
                        IsAllTxOffLineToSynchro = true;
                        foreach (var item in TxInfo.DicTxInfo)
                        {
                            item.Value.Online = "n";
                            newListTxInfo.Add(item.Value);
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else
            {
                if (IsAllTxRcvOk) //正常查询，显示有有新增Tx
                {
                    if (IsAllTxOffLineToSynchro)  //突然正常上线查询有Tx，但有新增Tx
                    {
                        IsAllTxOffLineToSynchro = false;
                        newListTxInfo.Clear();
                        foreach (var item in TxInfo.DicTxInfo)
                        {
                            if (!string.IsNullOrEmpty(item.Value.Online) && item.Value.Online != "n")  //正常在线的发出上线提示。
                            {
                                newListTxInfo.Add(item.Value);
                            }
                        }
                    }
                    else  //继续往后处理。。。
                    {
                        //return;
                    }
                }
            }
            StringBuilder sb = new StringBuilder();
            string split = "-";
            foreach (var item in newListTxInfo)
            {
                sb.Append(item.Id);
                sb.Append(split);
                ActionTxInfoAdd?.BeginInvoke(item, null, null);
            }            
            hsServer.ShowDebug("UpdateAddTxInfoAsync-双流Tx数量：" + sb.ToString());          
        }
        private static async Task  QueryNewTxInfoAsync(List<TxInfo> list,AutoResetEvent auto)
        {
            var ListTx = await MosaicScreenToTxRxServer_Helper.GetHsTxConfig(new List<string>());
            if (ListTx.Count==0)
            {
                IsAllTxRcvOk = false;
            }
            else
            {
                IsAllTxRcvOk = true;
                //IsAllTxOffLineToSynchro = false;
            }
            foreach (var item in ListTx)
            {
                if (item.DevType.Contains("MULTIv4") | item.DevType.Contains("MULTIv5"))  //TX-KVM-MULTIv4-H-SL  :表示为双流Tx 设备类型 4代1080P TX-KVM-MULTIv5-H-SL  :表示为双流Tx 设备类型 5代
                {
                    TxInfo txInfo = null;
                    bool isNewAdd = false;
                    if (TxInfo.DicTxInfo.ContainsKey(item.Id))
                    {
                        txInfo = TxInfo.DicTxInfo[item.Id];
                        //continue;  //不需提示新增TxInfo
                    }
                    else
                    {
                        isNewAdd = true;
                        txInfo = new TxInfo(item.Id);
                    }
                    txInfo.Name = item.Name;
                    txInfo.DevType = item.DevType;
                    txInfo.Ip = item.Ip;
                    txInfo.TcpPort = item.Port;
                    txInfo.Version = item.Version;
                    txInfo.Online = item.Online;
                    txInfo.Resolution = item.Resolution;
                    txInfo.Rate = item.Rate;
                    txInfo.EncodeFormat = item.Rate;
                    txInfo.Ts_addr = item.MultiIp;
                    txInfo.Ts_port = item.MultiPort;

                    string[] arrayIp = item.Ip.Split('.');
                    if (arrayIp.Length == 4)
                    {
                        txInfo.Udp_addr = "228.228." + arrayIp[2] + "." + arrayIp[3];  //169.254.100.1 >>228.228.100.1  默认端口20100
                        txInfo.Stream = @"rtsp://200.200." + arrayIp[2] + "." + arrayIp[3] + @":554/live/stream";            //rtsp://200.200.100.1:554/live/stream
                    }
                    else
                    {
                        hsServer.ShowDebug("GetAllTxInfoAsync-Ip解析长度不为4？");
                    }
                    if (isNewAdd)
                    {
                        list.Add(txInfo);
                    }
                }
            }
            if (list.Count!=0)
            {
                hsServer.ShowDebug("QueryNewTxInfoAsync-双流Tx数量：" + list.Count);
            }
            auto.Set();
        }
        private static async Task QueryAllTxToInfoAsync(List<TxInfo> list)
        {
            var ListTx = await MosaicScreenToTxRxServer_Helper.GetHsTxConfig(new List<string>());
            //hsServer.ShowDebug("GetHsTxConfig-查询Tx数量：" + ListTx.Count +"||"+ DateTime.Now.ToString("HH:mm:ss:fff"));

            foreach (var item in ListTx)
            {
                if (item.DevType.Contains("MULTIv4") | item.DevType.Contains("MULTIv5"))  //TX-KVM-MULTIv4-H-SL  :表示为双流Tx 设备类型 4代1080P TX-KVM-MULTIv5-H-SL  :表示为双流Tx 设备类型 5代
                {
                    TxInfo txInfo = null;
                    bool isNewAdd = false;
                    if (TxInfo.DicTxInfo.ContainsKey(item.Id))
                    {
                        txInfo = TxInfo.DicTxInfo[item.Id];
                        //continue;  //不需提示新增TxInfo
                    }
                    else
                    {
                        isNewAdd = true;
                        txInfo = new TxInfo(item.Id);
                    }
                    txInfo.Name = item.Name;
                    txInfo.DevType = item.DevType;
                    txInfo.Ip = item.Ip;
                    txInfo.TcpPort = item.Port;
                    txInfo.Version = item.Version;
                    txInfo.Online = item.Online;
                    txInfo.Resolution = item.Resolution;
                    txInfo.Rate = item.Rate;
                    txInfo.EncodeFormat = item.Rate;
                    txInfo.Ts_addr = item.MultiIp;
                    txInfo.Ts_port = item.MultiPort;

                    string[] arrayIp = item.Ip.Split('.');
                    if (arrayIp.Length == 4)
                    {
                        txInfo.Udp_addr = "228.228." + arrayIp[2] + "." + arrayIp[3];  //169.254.100.1 >>228.228.100.1  默认端口20100
                        txInfo.Stream = @"rtsp://200.200." + arrayIp[2] + "." + arrayIp[3] + @":554/live/stream";            //rtsp://200.200.100.1:554/live/stream
                    }
                    else
                    {
                        hsServer.ShowDebug("QueryAllTxToInfoAsync-Ip解析长度不为4？");
                    }
                    //if (isNewAdd)
                    //{
                        list.Add(txInfo);
                    //}
                }
            }
            
            hsServer.ShowDebug("QueryAllTxToInfoAsync-双流Tx数量：" + list.Count);
             
           
        }



        private static void UpdateTxInfoAsync_FromHeartBeat(object stateInfo)
        {
            AutoResetEvent auto = (AutoResetEvent)stateInfo;
            List<TxInfo> newListTxInfo = new List<TxInfo>();
            QueryTxInfoHeatBeatAsync(auto);
            //auto.WaitOne(3000, false);
            //TimerUpdateTxInfo.Change(0, 1000);
            //hsServer.ShowDebug("改变查询Tx间隔："+1000);

            bool isContinul= auto.WaitOne(2000, true);  //需要调试 是否 返回false还继续往下执行?
            if (!isContinul)
            {
                hsServer.ShowDebug("UpdateTxInfoAsync_FromHeartBeat  WaitOne等待2s 返回false");
                return;

            }
            //hsServer.ShowDebug("状态标识IsSendAllTxInfoOff：" + IsSendAllTxInfoOff.ToString() + "|| IsSendAllTxInfoOnline:" + IsSendAllTxInfoOnline.ToString());
            if (IsSendAllTxInfoOff)
            {
                IsSendAllTxInfoOff = false;
                foreach (var item in TxInfo.DicTxInfo)
                {
                    item.Value.Online = "n";
                    newListTxInfo.Add(item.Value);
                }
            }
            else if (IsSendAllTxInfoOnline)  //突然正常上线查询有Tx，但有新增Tx
            {
                IsSendAllTxInfoOnline = false;
                newListTxInfo.Clear();
                //hsServer.ShowDebug("重连和查询后开始TxInfo判断上线的时间：" + DateTime.Now.ToString("HH:mm:ss:fff"));
                foreach (var item in TxInfo.DicTxInfo)
                {
                    if (!string.IsNullOrEmpty(item.Value.Online) && item.Value.Online != "n")  //正常在线的发出上线提示。
                    {
                        newListTxInfo.Add(item.Value);
                    }
                }
                hsServer.ShowDebug("上线的双流TxInfo数量："+newListTxInfo.Count);
            }
            else  //继续往后处理。。。
            {
                //return;
            }
            if (newListTxInfo.Count==0)
            {
                return;
            }

            
            StringBuilder sb = new StringBuilder();
            string split = "-";
            foreach (var item in newListTxInfo)
            {
                sb.Append(item.Id);
                sb.Append(split);
                ActionTxInfoAdd?.BeginInvoke(item, null, null);
            }
            hsServer.ShowDebug("UpdateTxInfoAsync_FromHeartBeat-双流Tx数量：" + sb.ToString());
        }
        private static async Task QueryTxInfoHeatBeatAsync(AutoResetEvent auto)
        {

            bool isOk = await MosaicScreenToTxRxServer_Helper.SendHeartBeatAsync();
            if (isOk)
            {
                if (!IsConnected&&!IsSendAllTxInfoOnline)
                {
                    //hsServer.ShowDebug("重新连接服务端通信："+ DateTime.Now.ToString("HH:mm:ss:fff"));
                    IsSendAllTxInfoOnline = true;
                    
                    await QueryAllTxToInfoAsync(new List<TxInfo>()); //表示断线后 重连ok，并且是需要查询一次并转化Info的情况   
                    
                }
                IsConnected = true;
            }
            else
            {
                if (IsConnected&&!IsSendAllTxInfoOff)
                {
                    IsSendAllTxInfoOff = true;
                }
                IsConnected = false;
                //IsNeedToReconnect = true;
            }

            auto.Set();
        }
    }
}
