
using AbstractInterFace;
using ConfigServer;
using Extensions;
using FromTxRxServer;
using MosaicServerFrom;
using MosaicServerFromRx;
using MosaicServerToStore;
using MosaicWinToRxNS;

using RxNS;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TxNS;
using Utils;

namespace ScreenManagerNS
{
    public enum ScreenToServerType
    {
        MosaicServer = 0,
        MosaicClient = 1,
        MosaicClientAndServer = 2
    }
    public class ScreenManager
    {

        SortedList<int, ScreenViewModel> Screens { get; set; } = new SortedList<int, ScreenViewModel>();
        List<RxInfo> ListRxInfo { get; set; } = new List<RxInfo>();
        List<TxInfo> ListTxInfo { get; set; } = new List<TxInfo>();

        //bool IsActivedRxOnlineResume { get; set; }  //此变量控制开关 是否在启动时候禁止Rx上线时触发重复发送恢复当前大屏模式


        //public string IP_Server { get; set; } = "127.0.0.1";
        //public int Port_Server { get; set; } = 5000;

        //public string MultiIP_Synchro { get; set; } = "234.234.0.3";
        //public int MultiPort_Synchro { get; set; } = 32108;

        //public string MultiIP_FromTxRxSynchro { get; set; } = "234.234.1.3";
        //public int MultiPort_FromTxRxSynchro { get; set; } = 32008;


        //public IMosaicServerToRx MosaicServerToRx { get; set; }
        //public IMosaicOperator MosaicOperator { get; set; }
        public IMosaicServerToStore MosaicServerToStore { get; set; }
        //public IMosaicServerToRx_Factory MosaicServerToRx_Factory { get; set; }
        public IMosaicWinToRx MosaicWinToRx { get; set; }

        MosaicServerFromManager MosaicServerFromManager { get; set; }

        MosaicServerFromRxManager MosaicServerFromRxManager { get; set; }

        //public MosaicServerEvent Event { get; set; }

        public ScreenToServerType ScreenToServerType { get; set; } = ScreenToServerType.MosaicServer;
        public ScreenManager()
        {

        }

        #region 加载组件------------
        public void LoadAsync()
        {
            if (ScreenToServerType == ScreenToServerType.MosaicServer)
            {
                if (MosaicServerFromRxManager == null)  //底层与Rx列表管理维护模块
                {
                    MosaicServerFromRxManager = new MosaicServerFromRxManager();
                    MosaicServerFromRxManager.MultiIp_Rx = hsConfig.MultiIp_SynchroRx;
                    MosaicServerFromRxManager.MultiPort_Rx = hsConfig.MultiPort_SynchroRx;

                    MosaicServerFromRxManager.InitialServerFromRx();
                }
                if (MosaicServerToStore == null)  //储存数据模块
                {
                    MosaicServerToStore = new MosaicServerToXml();
                    MosaicServerToStore.Load();
                }
                if (MosaicWinToRx == null)
                {
                    MosaicWinToRx ToRx = new MosaicWinToRx();
                    ToRx.Load(hsConfig.SendRx_MultiIp, hsConfig.SendRx_MultiPort);
                    MosaicWinToRx = ToRx;
                }
                if (MosaicServerFromManager == null)  //与上层客户端操作交互模块
                {
                    MosaicServerFromManager = new MosaicServerFromManager();
                    MosaicServerFromManager.InitialServerFrom(hsConfig.Port_Tcp_MosaicServer);
                    MosaicServerFromManager.InitialServerSynchro(hsConfig.MultiIp_SynchroMosaicServer, hsConfig.MultiPort_SynchroMosaicServer);
                    Load_MosaicServerEvent(MosaicServerFromManager.Event);

                    //MosaicServerFromRxManager.Action_UpdateRxInfoOnline += MosaicServerFromManager.UpdateRxStateToSynchro;
                }
                //ListTxInfo = await TxRxServer.GetAllTxInfoAsync();
                //测试用------------ -
                //TxInfo txInfo = new TxInfo("1003");
                //txInfo.Name = "模拟用";
                //txInfo.Ip = "169.254.100.3";
                //txInfo.TcpPort = 6000;
                //txInfo.Version = "模拟版本";
                //txInfo.Online = "模拟y";
                //txInfo.Resolution = "模拟1920x1080";
                //txInfo.Rate = "2000";
                ////txInfo.EncodeFormat = item.EncodeFormat;
                //txInfo.Ts_addr = "192.168.1.3";
                //txInfo.Ts_port = 32222;
                //txInfo.Udp_addr = "228.228.100.3";
                //txInfo.Udp_port = 20100;
                //txInfo.Stream = @"H:\testingVideo\猫和老鼠02.rm";

                ////ListTxInfo.Add(txInfo);

                //txInfo = new TxInfo("1004");
                //txInfo.Name = "模拟用2";
                //txInfo.Ip = "169.254.100.4";
                //txInfo.TcpPort = 6000;
                //txInfo.Version = "模拟版本";
                //txInfo.Online = "模拟y";
                //txInfo.Resolution = "模拟1920x1080";
                //txInfo.Rate = "2000";
                ////txInfo.EncodeFormat = item.EncodeFormat;
                //txInfo.Ts_addr = "192.168.1.4";
                //txInfo.Ts_port = 32222;
                //txInfo.Udp_addr = "228.228.100.4";
                //txInfo.Udp_port = 20100;
                //txInfo.Stream = @"H:\testingVideo\猫和老鼠04.rm";

                //ListTxInfo.Add(txInfo);

                TxRxServer.LoadSynchroFromTxRxServer(hsConfig.MultiIp_SynchroTxServer, hsConfig.MultiPort_SynchroTxServer);
                TxRxServer.ActionTxInfoOnline += MosaicServerFromManager.UpdateTxStateToSynchro;

                
                TxRxServer.ActionTxInfoAdd += MosaicServerFromManager.UpdateTxStateToSynchro;
                TxRxServer.TimerUpdateAddTxInfo();


                var listScreenInfo = MosaicServerToStore.GetAllScreenInfo();
                foreach (var item in listScreenInfo)
                {
                    ScreenViewModel screen =item.GetScreen();

                    bool isOk = screen.Initial();
                    if (isOk)
                    {
                        Screens.Add(screen.IdScreen, screen); 
                        //screen.UpdateInfoToViewModel();
                        //screen.UpdateInfoToPrePlan();
                        //screen.UpdateCurrentWinToRxServer();

                        //if (MosaicWinToRx!=null)
                        //{
                        //    MosaicWinToRx.BindRx(rxInfos, item);
                        //}                        
                    }
                }
                Thread.Sleep(MosaicServerFromRxManager.CheckRxInterval);
                //await Task.Delay(MosaicServerFromRxManager.CheckRxInterval);
                //服务端启动更新大屏绑定消息：
                if (MosaicWinToRx != null)
                {
                    foreach (var item in Screens)
                    {
                        MosaicWinToRx.BindRx(item.Value.ScreenRxInfos, item.Value);
                    }
                    
                }
                
                if (MosaicServerFromRxManager != null)
                {
                    MosaicServerFromRxManager.Action_UpdateRxInfoOnline += MosaicServerFromManager.UpdateRxStateToSynchro;
                    //MosaicServerFromRxManager.Action_UpdateRxInfoWin += UpdateScreenWinsForRxInfoOnline;  //此处会引起重复发送当前大屏模式
                    //刚开始运行时同步显示Rx绑定情况------
                    foreach (var item in RxInfo.DicRxInfo)
                    {
                        MosaicServerFromManager.UpdateRxStateToSynchro(item.Value);
                    }
                }

                //设置镜像屏绑定主屏
                for (int i = 0; i < Screens.Values.Count; i++)
                {
                    if (Screens.Values[i].IsMirror)
                    {
                        if (Screens.Values[i].BindMasterId>0)
                        {
                            int matserId = Screens.Values[i].BindMasterId;
                            ScreenViewModel masterScreen = null;
                            Screens.TryGetValue(matserId, out masterScreen);
                            if (masterScreen!=null)
                            {
                                masterScreen.ListMirrors.Add(Screens.Values[i]);
                            }
                        }
                    }
                }
                //IsActivedRxOnlineResume = true;
                foreach (var item in Screens)  //设置主屏 镜像屏恢复当前窗口
                {
                    ScreenViewModel screen = item.Value;
                    //Multi_MirrorAndMasterToWallClear(screen);
                    Multi_ResumeCurrentWinsAndMirrors(screen);
                    hsServer.ShowDebug("设置主屏 镜像屏恢复当前窗口："+item.Value.IdScreen+"::"+item.Value.Name);
                }
                if (MosaicServerFromManager!=null)
                {
                    MosaicServerFromRxManager.Action_UpdateRxInfoWin += UpdateScreenWinsForRxInfoOnline;
                }
            }
        }
        public void Load_MosaicServerEvent(MosaicServerEvent Event)
        {
            Event.Event_NewMosaicWall += Event_NewMosaicWall;
            Event.Event_CancelMosaicWall += Event_CancelMosaicWall;
            Event.Event_ModifyMosaicWallName += Event_ModifyMosaicWallName;
            Event.Event_GetMosaicWall += Event_GetMosaicWall;
            Event.Event_SetMirrorBind += Event_SetMirrorBind;
            Event.Event_SetRxesBind += Event_SetRxes;
            Event.Event_SetScreenClip += Event_SetScreenClip;

            Event.Event_OpenMosaicWin += Event_OpenMosaicWin;
            Event.Event_CloseMosaicWin += Event_CloseMosaicWin;
            Event.Event_CloseAllMosaicWin += Event_CloseAllMosaicWin;
            Event.Event_MoveMosaicWin += Event_MoveMosaicWin;
            Event.Event_LayMosaicWin += Event_LayMosaicWin;
            Event.Event_SwiMosaicWin += Event_SwiMosaicWin;

            Event.Event_CallScene += Event_CallScene;
            Event.Event_SaveScene += Event_SaveScene;
            Event.Event_DeleteScene += Event_DeleteScene;
            Event.Event_ModifySceneName += Event_ModifySceneName;

            Event.Event_CreatePrePlan += Event_CreatePrePlan;
            Event.Event_ModifyPrePlan += Event_ModifyPrePlan;
            Event.Event_DeletePrePlan += Event_DeletePrePlan;
            Event.Event_ModifyPrePlanName += Event_ModifyPrePlanName;
            Event.Event_GetPrePlans += Event_GetPrePlans;

            Event.Event_GetCurrentWins += Event_GetCurrentWins;
            Event.Event_GetScenes += Event_GetScenes;

            Event.Event_GetRxes += Event_GetRxes;
            Event.Event_GetTxes += Event_GetTxes;
        }

        public void SaveScreenCurrentWinsToStore()
        {
            if (MosaicServerToStore != null)
            {
                foreach (var item in Screens)
                {
                    ScreenViewModel screen = item.Value;
                    screen.CurrentWins.Clear();
                    foreach (var win in screen.ListRoamWines)
                    {
                        if (win.IsEnable)
                        {
                            screen.CurrentWins.Add(win);
                        }
                    }
                    MosaicServerToStore.SaveCurrentWins(screen);
                }
            }
        }

        public void UpdateScreenWinsForRxInfoOnline(RxInfo info)
        {

            ScreenViewModel screen = null;
            if (info.WallID>0)
            {
                Screens.TryGetValue(info.WallID, out screen);
                if (screen!=null)
                {

                    MosaicWinToRx?.BindOneRxInfo(screen.ScreenRxInfos, info, screen);
                    
                    if (!screen.IsMirror)
                    {
                        Multi_ResumeCurrentWinsAndMirrors(screen, false);
                    }
                    else
                    {
                        UpdateMirrorWinsFromMasterWall(screen);
                    }
                }
            }
        }
        /// <summary>
        /// 主屏根据当前的大屏模式 更新到Rx。若有关联的镜像屏，同步更新
        /// </summary>
        /// <param name="screen">主屏</param>
        /// <param name="isSynchroMirrors">是否更新到镜像屏 ，默认true</param>
        private void Multi_ResumeCurrentWinsAndMirrors(ScreenViewModel screen ,bool isSynchroMirrors=true)
        {
            //hsServer.ShowDebug("Debug-----------Multi_ResumeCurrentWinsAndMirrors");
            if (screen == null || screen.IsMirror )  //镜像屏不处理，由主屏附带处理
            {
                return;
            }

            if (screen.Rxid.Count == 0)  //未绑定的大屏无需处理
            {
                return;
            }
            //if (!IsActivedRxOnlineResume)
            //{
            //    return;
            //}

            if (MosaicWinToRx!=null)
            {
                MosaicWinToRx.WallClear(screen.IdScreen);
                if (isSynchroMirrors)
                {
                    for (int i = 0; i < screen.ListMirrors.Count; i++)
                    {
                        ScreenViewModel mirror = screen.ListMirrors[i];
                        MosaicWinToRx.WallClear(mirror.IdScreen);
                    }
                }
            }
            List<HsMosaicWinInfo> winInfos = new List<HsMosaicWinInfo>();
            List<TxInfo> txInfos = new List<TxInfo>();

            List<List<HsMosaicWinInfo>> listMirrorWins = new List<List<HsMosaicWinInfo>>();
            foreach (var item in screen.ListMirrors)
            {
                listMirrorWins.Add(new List<HsMosaicWinInfo>());
            }

            foreach (var item in screen.ListRoamWines)
            {
                if (item.IsEnable )
                {
                    if (TxInfo.DicTxInfo.ContainsKey(item.IdTx))
                    {
                        TxInfo tx = TxInfo.DicTxInfo[item.IdTx];
                        txInfos.Add(tx);
                        winInfos.Add(item);

                        if (isSynchroMirrors)
                        {
                            for (int i = 0; i < screen.ListMirrors.Count; i++)
                            {
                                ScreenViewModel mirror = screen.ListMirrors[i];
                                Rect rectMirrorWin = Rect.ConverterRect0ToRect1(item.Position, screen.ReferTotalPixels, mirror.ReferTotalPixels);
                                HsMosaicWinInfo mirrorWin = new HsMosaicWinInfo();
                                mirrorWin.X = (int)rectMirrorWin.X;
                                mirrorWin.Y = (int)rectMirrorWin.Y;
                                mirrorWin.Width = (int)rectMirrorWin.Width;
                                mirrorWin.Height = (int)rectMirrorWin.Height;
                                mirrorWin.IdWin = item.IdWin;
                                mirrorWin.ZIndex = item.ZIndex;
                                listMirrorWins[i].Add(mirrorWin);
                            }
                        }                        
                    }
                    else
                    {
                        hsServer.ShowDebug("当前信号源TxInfo不在线：" + item.IdTx);
                    }
                }
            }            
             
            if (MosaicWinToRx != null)
            {                
                if (winInfos.Count > 0)
                {
                    Thread.Sleep(15);  //确保场景调用时，新开窗口指令在清屏指令之后，后续协议可以考虑附加一个指示，适用解决这个问题。
                    MosaicWinToRx.WinModify(screen.IdScreen, winInfos, txInfos);
                    if (isSynchroMirrors)  //同步镜像屏的当前窗口
                    {                        
                        for (int i = 0; i < screen.ListMirrors.Count; i++)
                        {
                            ScreenViewModel mirror = screen.ListMirrors[i];
                            MosaicWinToRx.WinModify(mirror.IdScreen, listMirrorWins[i], txInfos);
                        }
                    }
                }               
            }
        }
        /// <summary>
        /// 更新镜像屏（若绑定有Rx和关联主屏），则更新同步主屏的当前模式
        /// </summary>
        /// <param name="screen">镜像屏</param>
        private void UpdateMirrorWinsFromMasterWall(ScreenViewModel screen)
        {
             
            if (screen == null || !screen.IsMirror||screen.BindMasterId<1)  
            {
                return;
            }
            if (screen.Rxid.Count==0)  //未绑定的大屏无需处理
            {
                return;
            }
            ScreenViewModel masterWall = null;
            Screens.TryGetValue(screen.BindMasterId, out masterWall);
            if (masterWall==null)
            {
                return;
            }
            if (MosaicWinToRx != null)
            {
                MosaicWinToRx.WallClear(screen.IdScreen);

                List<HsMosaicWinInfo> winInfos = new List<HsMosaicWinInfo>();
                List<TxInfo> txInfos = new List<TxInfo>();

                foreach (var item in masterWall.ListRoamWines)
                {
                    if (item.IsEnable)
                    {
                        if (TxInfo.DicTxInfo.ContainsKey(item.IdTx))
                        {
                            TxInfo tx = TxInfo.DicTxInfo[item.IdTx];
                            

                            Rect rectMirrorWin = Rect.ConverterRect0ToRect1(item.Position, masterWall.ReferTotalPixels, screen.ReferTotalPixels);
                            HsMosaicWinInfo mirrorWin = new HsMosaicWinInfo();
                            mirrorWin.X = (int)rectMirrorWin.X;
                            mirrorWin.Y = (int)rectMirrorWin.Y;
                            mirrorWin.Width = (int)rectMirrorWin.Width;
                            mirrorWin.Height = (int)rectMirrorWin.Height;
                            mirrorWin.IdWin = item.IdWin;
                            mirrorWin.ZIndex = item.ZIndex;
                            winInfos.Add(mirrorWin);
                            txInfos.Add(tx);
                        }
                        else
                        {
                            hsServer.ShowDebug("当前信号源TxInfo不在线：" + item.IdTx);
                        }
                    }
                }

                if (winInfos.Count > 0)  //同步镜像屏的当前窗口
                {
                    MosaicWinToRx.WinModify(screen.IdScreen, winInfos, txInfos);
                }
            }
        }
        /// <summary>
        /// 更新单个窗口的在主屏上的变动，且同步更新镜像屏
        /// </summary>
        /// <param name="screen"></param>
        /// <param name="idWin"></param>
        private void UpdateWinFromMasterWinOrToMirror(ScreenViewModel screen, int idWin)
        {
            if (screen == null || screen.IsMirror)  //镜像屏不处理，由主屏附带处理
            {
                return;
            }

            if (idWin<1||idWin>screen.ListRoamWines.Count)
            {
                return;
            }
            HsMosaicWinInfo Win = screen.ListRoamWines[idWin - 1];
            if (!Win.IsEnable) //同步镜像屏  关闭窗口
            {
                if (MosaicWinToRx != null)
                {
                    MosaicWinToRx.WinClose(screen.IdScreen, idWin);
                    for (int i = 0; i < screen.ListMirrors.Count; i++)
                    {
                        ScreenViewModel mirror = screen.ListMirrors[i];
                        MosaicWinToRx.WinClose(mirror.IdScreen, idWin);
                    }
                }

                return;
            }

            List<HsMosaicWinInfo> winInfos = new List<HsMosaicWinInfo>();
            List<TxInfo> txInfos = new List<TxInfo>();

            List<List<HsMosaicWinInfo>> listMirrorWins = new List<List<HsMosaicWinInfo>>();
            foreach (var item in screen.ListMirrors)
            {
                listMirrorWins.Add(new List<HsMosaicWinInfo>());
            }

            
            if (TxInfo.DicTxInfo.ContainsKey(Win.IdTx))
            {
                TxInfo tx = TxInfo.DicTxInfo[Win.IdTx];
                txInfos.Add(tx);
                winInfos.Add(Win);

                for (int i = 0; i < screen.ListMirrors.Count; i++)
                {
                    ScreenViewModel mirror = screen.ListMirrors[i];
                    Rect rectMirrorWin = Rect.ConverterRect0ToRect1(Win.Position, screen.ReferTotalPixels, mirror.ReferTotalPixels);
                    Rect mirroNewWin= mirror.RectifyPostion(rectMirrorWin, hsConfig.WinRectRate);
                    if (mirroNewWin.IsEmpty)
                    {
                        mirroNewWin = rectMirrorWin;
                    }

                    HsMosaicWinInfo mirrorWin = new HsMosaicWinInfo();
                    mirrorWin.X = (int)mirroNewWin.X;
                    mirrorWin.Y = (int)mirroNewWin.Y;
                    mirrorWin.Width = (int)mirroNewWin.Width;
                    mirrorWin.Height = (int)mirroNewWin.Height;
                    mirrorWin.IdWin = Win.IdWin;
                    mirrorWin.ZIndex = Win.ZIndex;
                    listMirrorWins[i].Add(mirrorWin);
                }
            }
            else
            {
                hsServer.ShowDebug("当前信号源TxInfo不在线：" + Win.IdTx);
                return;
            }               

            if (winInfos.Count > 0)  //同步镜像屏的当前窗口
            {
                 
                if (MosaicWinToRx!=null)
                {
                    MosaicWinToRx.WinModify(screen.IdScreen, winInfos, txInfos);
                    for (int i = 0; i < screen.ListMirrors.Count; i++)
                    {
                        ScreenViewModel mirror = screen.ListMirrors[i];
                        MosaicWinToRx.WinModify(mirror.IdScreen, listMirrorWins[i], txInfos);
                    }
                }                
            }             
        }
        private void Multi_MirrorAndMasterToWallClear(ScreenViewModel screen)
        {
            if (screen == null || screen.IsMirror||screen.Rxid.Count==0)  //镜像屏不处理，由主屏附带处理
            {
                return;
            }
            if (MosaicWinToRx != null)
            {
                MosaicWinToRx.WallClear(screen.IdScreen);
                for (int i = 0; i < screen.ListMirrors.Count; i++)
                {
                    ScreenViewModel mirror = screen.ListMirrors[i];
                    MosaicWinToRx.WallClear(mirror.IdScreen);
                }
            }
        }
        #endregion

        #region MosaicServerEvent 事件方法接口--------
        //------------------Tx/Rx-----------
        public void Event_GetRxes(Json_Rec_Hs_Cmd_End end, List<RxInfo> rxInfo)
        {
            if (rxInfo == null)
            {
                end.Err_code = "1";
                end.Err_str = "列表参数为null";
                return;
            }
            //foreach (var item in ListRxInfo)
            //{
            //    rxInfo.Add(item);
            //}
            foreach (var item in RxInfo.DicRxInfo)
            {
                rxInfo.Add(item.Value);
            }
        }
        public void Event_GetTxes(Json_Rec_Hs_Cmd_End end, List<TxInfo> txInfo)
        {
            if (txInfo == null)
            {
                end.Err_code = "1";
                end.Err_str = "列表参数为null";
                return;
            }
            //foreach (var item in ListTxInfo)
            //{
            //    txInfo.Add(item);
            //}
            foreach (var item in TxInfo.DicTxInfo)
            {
                txInfo.Add(item.Value);
            }
        }

        //-------------------大屏事件响应
        public void Event_NewMosaicWall(Json_Rec_Hs_Cmd_End end, HsScreenInfo screenInfo)
        {
            if (screenInfo == null)
            {
                end.Err_code = "1";
                end.Err_str = "参数为null";
                return;
            }
             
            foreach (var item in Screens)
            {
                if (item.Value.Name == screenInfo.Name)
                {
                    end.Err_code = "1";
                    end.Err_str = "请勿创建重复的大屏名称！";
                    return;
                }
            }
            int newIdScreen = 1;
            //Screens.Sort (delegate (ScreenViewModel x, ScreenViewModel y)
            //{
            //    return x.IdScreen.CompareTo(y.IdScreen);
            //});
            for (int i = 1; i <= Screens.Keys.Count; i++)
            {
                if (Screens.Keys[i - 1] != i)
                {
                    break;
                }
                newIdScreen++;
            }
            //检查简单逻辑判断---
            List<RxInfo> tempRxes = new List<RxInfo>();
            ScreenViewModel newScreen = screenInfo.GetScreen();
             
            if (screenInfo.Rxid.Count!=0)   //绑定RxId的创建大屏
            {
                if (screenInfo.Rows * screenInfo.Columns != screenInfo.Rxid.Count)
                {
                    end.Err_code = "1";
                    end.Err_str = "行×列值与绑定的RxId数量不一致！";
                    return;
                }

                bool isBindingOk = true;
                foreach (var item in screenInfo.Rxid)
                {
                    RxInfo rxInfo = null;
                    //RxInfo rxInfo = ListRxInfo.Find(f => f.Id == item);
                    if (RxInfo.DicRxInfo.ContainsKey(item))
                    {
                        rxInfo = RxInfo.DicRxInfo[item];
                    }
                    if (rxInfo != null && rxInfo.WallID==-1)
                    {
                        rxInfo.IsBinded = true;
                        rxInfo.WallID = newIdScreen;
                        tempRxes.Add(rxInfo);
                    }
                    else
                    {
                        isBindingOk = false;
                        end.Err_code = "1";
                        end.Err_str = "错误：使用已绑定/不存在的RxId：" + item;
                        break;
                    }
                }
                if (!isBindingOk)
                {
                    foreach (var item in tempRxes)
                    {
                        item.IsBinded = false;
                        item.WallID = -1;
                    }
                    return;
                }               
                
                bool isOk = newScreen.Initial();
                if (!isOk)
                {
                    end.Err_code = "1";
                    end.Err_str = "大屏初始化错误";
                    return;
                }
                MosaicWinToRx?.BindRx(newScreen.ScreenRxInfos, screenInfo);               
            }
            else  //未绑定Rx的大屏
            {

            }
            Screens.Add(newScreen.IdScreen, newScreen);
            if (MosaicServerToStore != null)
            {
                MosaicServerToStore.InsertScreen(newScreen);
            }
        }
        public void Event_CancelMosaicWall(Json_Rec_Hs_Cmd_End end, int idScreen)
        {

            ScreenViewModel screen = null;
            if (Screens.ContainsKey(idScreen))
            {
                screen = Screens[idScreen];
            }
            if (screen != null)
            {
                 
                foreach (var item in screen.ScreenRxInfos)
                {
                    item.WallID = -1;
                    item.ActionOnline = null;
                    item.Row = 0;
                    item.Column = 0;
                    item.IsBinded = false;
                }
                screen.Event_CloseOrHiddenAllWin_ToRx();
                Screens.Remove(screen.IdScreen);

                if (screen.IsMirror)
                {
                    if (screen.BindMasterId > 0)
                    {
                        ScreenViewModel masterScreen = null;
                        Screens.TryGetValue(screen.BindMasterId, out masterScreen);
                        if (masterScreen != null)
                        {
                            masterScreen.ListMirrors.Remove(screen);
                        }
                    }
                }
                else
                {
                    foreach (var item in screen.ListMirrors)  //同步删除镜像屏的画面 同时设置绑定主屏Id=0
                    {
                        item.BindMasterId = 0;
                        item.Event_CloseOrHiddenAllWin_ToRx();
                        
                        if (MosaicServerToStore != null)
                        {
                            MosaicServerToStore.ModifyWallBindMasterId(item.IdScreen, 0);
                        }
                        if (MosaicWinToRx != null)
                        {
                            MosaicWinToRx.WallClear(item.IdScreen);
                        }
                    }
                }
                                

                if (MosaicServerToStore != null)
                {
                    MosaicServerToStore.DeleteScreen(screen);
                }

                if (MosaicWinToRx!=null)
                {
                    MosaicWinToRx.UnBindRx(screen.ScreenRxInfos, screen);
                }
            }
            else
            {
                end.Err_code = "1";
                end.Err_str = "当前未找到大屏Id:" + idScreen;
            }
        }
        public void Event_ModifyMosaicWallName(Json_Rec_Hs_Cmd_End end, int idScreen, string newName)
        {
            bool isOK = true;
            foreach (var item in Screens.Values)
            {
                if (item.Name == newName)
                {
                    isOK = false;
                    break;
                }
            }
            if (!isOK)
            {
                end.Err_code = "1";
                end.Err_str = "已有重复大屏名称:" + newName;
                return;
            }
            if (Screens.ContainsKey(idScreen))
            {
                ScreenViewModel screen = Screens[idScreen];
                screen.Name = newName;

                if (MosaicServerToStore != null)
                {
                    MosaicServerToStore.ModifyWallName(screen.IdScreen, newName);
                }
            }
            else
            {
                end.Err_code = "1";
                end.Err_str = "当前未找到大屏Id:" + idScreen;
            }
        }
        public void Event_GetMosaicWall(Json_Rec_Hs_Cmd_End end, List<HsScreenInfo> listScreenInfo)
        {
            foreach (var item in listScreenInfo)
            {
                ScreenViewModel screen = null;
                Screens.TryGetValue(item.IdScreen, out screen);
                if (screen != null)
                {
                    listScreenInfo.Add(screen);
                }
                else
                {
                    end.Err_str += item + " null ";
                }
            }
            if (listScreenInfo.Count == 0)
            {
                foreach (var item in Screens.Values)
                {
                    listScreenInfo.Add(item);
                }
            }
        }
        public void Event_SetMirrorBind(Json_Rec_Hs_Cmd_End end, int idMirror, int idMaster)
        {
            ScreenViewModel screen = null;
            Screens.TryGetValue(idMirror, out screen);

            ScreenViewModel screenMaster = null;
            Screens.TryGetValue(idMaster, out screenMaster);

            if (idMaster>0)  //取消绑定
            {
                if (screenMaster==null)
                {
                    end.Err_code = "1";
                    end.Err_str = idMaster + " is not exist.";
                    return;
                }
            }

            if (screen != null  )
            {
                if (screen.IsMirror)
                {
                    if (screen.BindMasterId==idMaster)
                    {
                        return;
                    }
                    if (screen.BindMasterId!=0)  //原来有绑定的主屏
                    {
                        ScreenViewModel oldMaster = null;
                        Screens.TryGetValue(screen.BindMasterId, out oldMaster);
                        if (oldMaster!=null)
                        {
                            oldMaster.ListMirrors.Remove(screen);
                        }
                    }
                    screen.BindMasterId = idMaster;

                    if (idMaster>0)
                    {
                        if (!screenMaster.ListMirrors.Contains(screen))
                        {
                            screenMaster.ListMirrors.Add(screen);
                        }
                    }
                    
                    if (MosaicServerToStore != null)
                    {
                        MosaicServerToStore.ModifyWallBindMasterId(screen.IdScreen,idMaster);
                    }

                    if (MosaicWinToRx!=null)
                    {
                        UpdateMirrorWinsFromMasterWall(screen);
                    } 
                }
                else
                {
                    end.Err_code = "1";
                    end.Err_str = idMirror+" : not mirror screen.";
                    return;
                }
            }
            else
            {
                end.Err_code = "1";
                end.Err_str = idMirror +   " is not exist.";
                return;
            }
        }
        public void Event_SetRxes(Json_Rec_Hs_Cmd_End end, HsScreenInfo screenInfo)
        {
            if (screenInfo == null)
            {
                end.Err_code = "1";
                end.Err_str = "参数为null";
                return;
            }
            ScreenViewModel screen = null;
            Screens.TryGetValue(screenInfo.IdScreen, out screen);
            if (screen != null)
            {
                bool isOk = true;

                bool isNewBind = false;
                if (screen.Rxid.Count==0)  //表示从无绑定到新绑定。。。
                {
                    isNewBind = true;
                }
                List<RxInfo> oldRxinfos = new List<RxInfo>();
                foreach (var item in screen.ScreenRxInfos)
                {
                    oldRxinfos.Add(item);
                }

                isOk = screen.SetRxesBind(screenInfo.Rxid);  //只更新判断screen.ScreenRxes, screen.rxInfo  Rxid
                if (isOk)
                {
                    if (isNewBind)
                    {
                        MosaicWinToRx?.BindRx(screen.ScreenRxInfos, screen);
                        bool isInitialOk = screen.Initial();
                    }
                    else
                    {
                        MosaicWinToRx?.ReBindRx(oldRxinfos, screen.ScreenRxInfos, screen);
                    }

                }
                if (!isOk)
                {
                    end.Err_code = "1";
                    end.Err_str = "error pramaters";
                }
                else
                {
                    MosaicServerToStore?.ModifyWallBindRxes(screen.IdScreen, screenInfo.Rxid);
                }
            }
            else
            {
                end.Err_code = "1";
                end.Err_str = "未找到对应大屏Id："+screenInfo.IdScreen;
                return;
            }
        }
        public void Event_SetScreenClip(Json_Rec_Hs_Cmd_End end, HsScreenInfo screenInfo)
        {
            if (screenInfo == null)
            {
                end.Err_code = "1";
                end.Err_str = "参数为null";
                return;
            }
            ScreenViewModel screen = null;
            Screens.TryGetValue(screenInfo.IdScreen, out screen);
            if (screen != null)
            {
                bool isOk = true;
                isOk = screen.SetScreenClip(screenInfo.StartX,screenInfo.StartY,screenInfo.WallPixW,screenInfo.WallPixH);
                if (!isOk)
                {
                    end.Err_code = "1";
                    end.Err_str = "错误：尺寸超出范围！";
                }
                else
                {
                    MosaicServerToStore?.ModifyWallClipScreen(screen.IdScreen, screenInfo.StartX, screenInfo.StartY, screenInfo.WallPixW, screenInfo.WallPixH);
                }
            }
            else
            {
                end.Err_code = "1";
                end.Err_str = "未找到对应大屏Id：" + screenInfo.IdScreen;
                return;
            }
        }

        //------------------窗口事件响应
        public void Event_OpenMosaicWin(Json_Rec_Hs_Cmd_End end, HsMosaicWinInfo winInfo)
        {
            if (winInfo==null)
            {
                return;
            }
            ScreenViewModel screen = null   ;
            Screens.TryGetValue(winInfo.IdScreen, out screen);
            if (screen != null)
            {
                 
                bool isOk = screen.Event_OpenWin_ToRx(winInfo);
                if (!isOk)
                {
                    end.Err_code = "1";
                    end.Err_str = "parameter error!";
                    return;
                }
                else
                {
                    UpdateWinFromMasterWinOrToMirror(screen, winInfo.IdWin);
                }
                 
                //if (MosaicWinToRx!=null)
                //{
                //    //TxInfo txInfo = null;
                //    //if (TxInfo.DicTxInfo.ContainsKey(winInfo.IdTx))
                //    //{
                //    //    txInfo = TxInfo.DicTxInfo[winInfo.IdTx];
                //    //    MosaicWinToRx.WinModify(screen.IdScreen, new List<HsMosaicWinInfo> { winInfo }, new List<TxInfo> { txInfo });
                //    //}
                //    //else   //实际上不会出现这个
                //    //{
                //    //    txInfo = new TxInfo(winInfo.IdTx);
                //    //    //end.Err_code = "1";
                //    //    //end.Err_str = "txId error";
                //    //    //return;
                //    //}
                //}                
            }
            else
            {
                end.Err_code = "1";
                end.Err_str = "当前未找到大屏Id:" + winInfo.IdScreen;
            }
        }
        public void Event_CloseMosaicWin(Json_Rec_Hs_Cmd_End end, int idScreen,int idWin)
        {
            ScreenViewModel screen = null;
            Screens.TryGetValue(idScreen, out screen);
            if (screen != null)
            {                
                bool isOk = screen.Event_CloseWin_ToRx(idWin);
                if (!isOk)
                {
                    end.Err_code = "1";
                    end.Err_str = "parameter error!";
                }
                else
                {
                    UpdateWinFromMasterWinOrToMirror(screen, idWin);
                    //if (MosaicWinToRx != null)
                    //{
                    //    MosaicWinToRx.WinClose(idScreen, idWin);
                    //}
                }                            
            }
            else
            {
                end.Err_code = "1";
                end.Err_str = "当前未找到大屏Id:" + idScreen;
            }
        }
        public void Event_CloseAllMosaicWin(Json_Rec_Hs_Cmd_End end, int idScreen)
        {
            ScreenViewModel screen = null;
            Screens.TryGetValue(idScreen, out screen);
            if (screen != null)
            {
                bool isOk = screen.Event_CloseOrHiddenAllWin_ToRx();
                if (!isOk)
                {
                    end.Err_code = "1";
                    end.Err_str = "parameter error!";
                }
                else
                {
                    Multi_MirrorAndMasterToWallClear(screen);
                    //if (MosaicWinToRx != null)
                    //{
                    //    MosaicWinToRx.WallClear(idScreen);
                    //}
                }
            }
            else
            {
                end.Err_code = "1";
                end.Err_str = "当前未找到大屏Id:" + idScreen;
            }
        }
        public void Event_MoveMosaicWin(Json_Rec_Hs_Cmd_End end, HsMosaicWinInfo winInfo)
        {
            if (winInfo == null)
            {
                return;
            }
            ScreenViewModel screen = null;
            Screens.TryGetValue(winInfo.IdScreen, out screen);
            if (screen != null)
            {
                bool isOk = screen.Event_MoveWin_ToRx(winInfo);
                if (!isOk)
                {
                    end.Err_code = "1";
                    end.Err_str = "parameter error!";
                }
                else
                {
                    UpdateWinFromMasterWinOrToMirror(screen, winInfo.IdWin);
                    //if (MosaicWinToRx != null)
                    //{
                    //    if (TxInfo.DicTxInfo.ContainsKey(winInfo.IdTx))
                    //    {
                    //        TxInfo txInfo = TxInfo.DicTxInfo[winInfo.IdTx];
                    //        MosaicWinToRx.WinModify(winInfo.IdScreen, new List<HsMosaicWinInfo> { winInfo }, new List<TxInfo> { txInfo });
                    //    }
                    //}
                }
            }
            else
            {
                end.Err_code = "1";
                end.Err_str = "当前未找到大屏Id:" + winInfo.IdScreen;
            }
        }
        public void Event_LayMosaicWin(Json_Rec_Hs_Cmd_End end, HsMosaicWinInfo winInfo)
        {
            if (winInfo == null)
            {
                return;
            }
            ScreenViewModel screen = null;
            Screens.TryGetValue(winInfo.IdScreen, out screen);
            if (screen != null)
            {
                bool isOk = screen.Event_LayMosaicWin_ToRx(winInfo);
                if (!isOk)
                {
                    end.Err_code = "1";
                    end.Err_str = "parameter error!";
                }
                else
                {
                    UpdateWinFromMasterWinOrToMirror(screen, winInfo.IdWin);
                    //if (MosaicWinToRx != null)
                    //{
                    //    if (TxInfo.DicTxInfo.ContainsKey(winInfo.IdTx))
                    //    {
                    //        TxInfo txInfo = TxInfo.DicTxInfo[winInfo.IdTx];
                    //        MosaicWinToRx.WinModify(winInfo.IdScreen, new List<HsMosaicWinInfo> { winInfo }, new List<TxInfo> { txInfo });
                    //    }
                    //}
                }
            }
            else
            {
                end.Err_code = "1";
                end.Err_str = "当前未找到大屏Id:" + winInfo.IdScreen;
            }
        }
        public void Event_SwiMosaicWin(Json_Rec_Hs_Cmd_End end, HsMosaicWinInfo winInfo)
        {
            if (winInfo == null)
            {
                return;
            }
            ScreenViewModel screen = null;
            Screens.TryGetValue(winInfo.IdScreen, out screen);
            if (screen != null)
            {
                bool isOk = screen.Event_ModifyTxWin_ToRx(winInfo);
                if (!isOk)
                {
                    end.Err_code = "1";
                    end.Err_str = "parameter error!";
                }
                else
                {
                    UpdateWinFromMasterWinOrToMirror(screen, winInfo.IdWin);
                    //if (MosaicWinToRx != null)
                    //{

                    //    if (TxInfo.DicTxInfo.ContainsKey(winInfo.IdTx))
                    //    {
                    //        TxInfo txInfo = TxInfo.DicTxInfo[winInfo.IdTx];
                    //        MosaicWinToRx.WinModify(winInfo.IdScreen, new List<HsMosaicWinInfo> { winInfo }, new List<TxInfo> { txInfo });
                    //    }
                    //}
                }
            }
            else
            {
                end.Err_code = "1";
                end.Err_str = "当前未找到大屏Id:" + winInfo.IdScreen;
            }
        }

        //---------------场景事件响应
        public void Event_CallScene(Json_Rec_Hs_Cmd_End end, int idScreen ,string sceneName)
        {
            ScreenViewModel screen = null;
            Screens.TryGetValue(idScreen, out screen);
            if (screen != null)
            {
                bool isOk = screen.Event_CallScene(sceneName);
                if (!isOk)
                {
                    end.Err_code = "1";
                    end.Err_str = "parameter has erro, no servered！";
                }
                else
                {
                    if (MosaicWinToRx != null)
                    {
                        Multi_ResumeCurrentWinsAndMirrors(screen);
                    }
                }
            }
            else
            {
                end.Err_code = "1";
                end.Err_str = "当前未找到大屏Id:" + idScreen;
            }
        }
        public void Event_SaveScene(Json_Rec_Hs_Cmd_End end, SceneInfo sceneInfo)
        {
            ScreenViewModel screen = null;
            Screens.TryGetValue(sceneInfo.IdScreen, out screen);
            if (screen != null)
            {
                bool isOk = screen.Event_SaveScene(sceneInfo);
                if (!isOk)
                {
                    end.Err_code = "1";
                    end.Err_str = "parameter has erro, no servered！";
                }
                else
                {
                    if (MosaicServerToStore != null)
                    {
                        MosaicServerToStore.InsertScene(sceneInfo);
                    }
                }
            }
            else
            {
                end.Err_code = "1";
                end.Err_str = "当前未找到大屏Id:" + sceneInfo.IdScene;
            }
        }
        public void Event_DeleteScene(Json_Rec_Hs_Cmd_End end, int idScreen,string sceneName)
        {
            ScreenViewModel screen = null;
            Screens.TryGetValue(idScreen, out screen);
            if (screen != null)
            {
                SceneInfo info = null;
                foreach (var item in screen.Scenes)
                {
                    if (item.Name==sceneName)
                    {
                        info = item;
                        break;
                    }
                }
                bool isOk = screen.Event_DeleteScene(sceneName);
                if (!isOk)
                {
                    end.Err_code = "1";
                    end.Err_str = "parameter has erro, no servered！";
                }
                else
                {
                    if (MosaicServerToStore != null)
                    {
                        if (info!=null)
                        {
                            MosaicServerToStore.DeleteScene(info);
                        }
                    }
                }
            }
            else
            {
                end.Err_code = "1";
                end.Err_str = "当前未找到大屏Id:" + idScreen;
            }
        }
        public void Event_ModifySceneName(Json_Rec_Hs_Cmd_End end, int idScreen, string sceneName,string newName)
        {
            ScreenViewModel screen = null;
            Screens.TryGetValue(idScreen, out screen);
            if (screen != null)
            {
                SceneInfo info = null;
                foreach (var item in screen.Scenes)
                {
                    if (item.Name == sceneName)
                    {
                        info = item;
                        break;
                    }
                }
                bool isOk = screen.Event_ModifySceneName(sceneName, newName);
                if (!isOk)
                {
                    end.Err_code = "1";
                    end.Err_str = "parameter has erro, no servered！";
                }
                else
                {
                    if (MosaicServerToStore != null)
                    {
                        MosaicServerToStore.ModifySceneName(idScreen,sceneName,newName);
                    }
                }
            }
            else
            {
                end.Err_code = "1";
                end.Err_str = "当前未找到大屏Id:" + idScreen;
            }
        }
        public void Event_GetScenes(Json_Rec_Hs_Cmd_End end, List<HsScreenInfo> listScreenInfo)
        {
            foreach (var item in listScreenInfo)
            {
                ScreenViewModel screen = null;
                Screens.TryGetValue(item.IdScreen, out screen);
                if (screen != null)
                {
                    screen.Event_GetScenes(item);
                }
                else
                {
                    //end.Err_code = "1";
                    end.Err_str += "当前未找到大屏Id:" + item.IdScreen;
                }
            }
            if (listScreenInfo.Count==0)
            {
                foreach (var item in Screens.Values)
                {
                    if (item.IsMirror)
                    {
                        continue;
                    }
                    listScreenInfo.Add(item);
                }
            }
           
        }
        public void Event_GetCurrentWins(Json_Rec_Hs_Cmd_End end, List<HsScreenInfo> listScreenInfo)
        {
            foreach (var item in listScreenInfo)
            {
                ScreenViewModel screen = null;
                Screens.TryGetValue(item.IdScreen, out screen);
                if (screen != null)
                {
                    screen.Event_GetCurrentWins(item);
                }
                else
                {
                    //end.Err_code = "1";
                    end.Err_str += "当前未找到大屏Id:" + item.IdScreen;
                }
            }
            if (listScreenInfo.Count == 0)
            {
                foreach (var item in Screens.Values)
                {
                    HsScreenInfo info = new HsScreenInfo();
                    item.Event_GetCurrentWins(info);
                    listScreenInfo.Add(info);
                }
            }

        }

        //---------------预案事件响应
        public void Event_CreatePrePlan(Json_Rec_Hs_Cmd_End end, PrePlanInfo info)
        {
            if (info == null)
            {
                end.Err_code = "1";
                end.Err_str = "参数为null";
                return;
            }
            ScreenViewModel screen = null;
            Screens.TryGetValue(info.IdScreen, out screen);
            if (screen != null)
            {
                bool isOk = screen.Event_CreatePrePlan(info);
                if (!isOk)
                {
                    end.Err_code = "1";
                    end.Err_str = "parameter has erro, no servered！";
                }
                else
                {
                    if (MosaicServerToStore != null)
                    {
                        MosaicServerToStore.InsertPrePlan(info);
                    }
                }
            }
            else
            {
                end.Err_code = "1";
                end.Err_str = "当前未找到大屏Id:" + info.IdScreen;
            }
        }
        public void Event_ModifyPrePlan(Json_Rec_Hs_Cmd_End end, PrePlanInfo info)
        {
            if (info == null)
            {
                end.Err_code = "1";
                end.Err_str = "参数为null";
                return;
            }
            ScreenViewModel screen = null;
            Screens.TryGetValue(info.IdScreen, out screen);
            if (screen != null)
            {
                bool isOk = screen.Event_ModifyPrePlan(info);
                if (!isOk)
                {
                    end.Err_code = "1";
                    end.Err_str = "parameter has erro, no servered！";
                }
                else
                {
                    if (MosaicServerToStore != null)
                    {
                        MosaicServerToStore.DeletePrePlan(info.IdScreen, info.Name);
                        MosaicServerToStore.InsertPrePlan(info);
                    }
                }
            }
            else
            {
                end.Err_code = "1";
                end.Err_str = "当前未找到大屏Id:" + info.IdScreen;
            }
        }
        public void Event_DeletePrePlan(Json_Rec_Hs_Cmd_End end, int idScreen, string name)
        {
            ScreenViewModel screen = null;
            Screens.TryGetValue(idScreen, out screen);
            if (screen != null)
            { 
                bool isOk = screen.Event_DeletePrePlan(name);
                if (!isOk)
                {
                    end.Err_code = "1";
                    end.Err_str = "parameter has erro, no servered！";
                }
                else
                {
                    MosaicServerToStore?.DeletePrePlan(idScreen, name);
                }
            }
            else
            {
                end.Err_code = "1";
                end.Err_str = "当前未找到大屏Id:" + idScreen;
            }
        }
        public void Event_ModifyPrePlanName(Json_Rec_Hs_Cmd_End end, int idScreen, string sceneName, string newName)
        {
            ScreenViewModel screen = null;
            Screens.TryGetValue(idScreen, out screen);
            if (screen != null)
            {                
                bool isOk = screen.Event_ModifyPrePlanName(sceneName, newName);
                if (!isOk)
                {
                    end.Err_code = "1";
                    end.Err_str = "parameter has erro, no servered！";
                }
                else
                {
                    if (MosaicServerToStore != null)
                    {
                        MosaicServerToStore.ModifyPrePlanName(idScreen, sceneName, newName);
                    }
                }
            }
            else
            {
                end.Err_code = "1";
                end.Err_str = "当前未找到大屏Id:" + idScreen;
            }
        }
        public void Event_GetPrePlans(Json_Rec_Hs_Cmd_End end, List<HsScreenInfo> listScreenInfo)
        {
            foreach (var item in listScreenInfo)
            {
                ScreenViewModel screen = null;
                Screens.TryGetValue(item.IdScreen, out screen);
                if (screen != null)
                {
                    if (screen.IsMirror)
                    {
                        return;
                    }
                    screen.Event_GetPrePlan(item);
                }
                else
                {
                    //end.Err_code = "1";
                    end.Err_str += "当前未找到大屏Id:" + item.IdScreen;
                }
            }
            if (listScreenInfo.Count == 0)
            {
                foreach (var item in Screens.Values)
                {
                    if (item.IsMirror)
                    {
                        continue;
                    }
                    HsScreenInfo info = new HsScreenInfo();
                    item.Event_GetPrePlan(info);
                    listScreenInfo.Add(info);
                }
            }

        }
        #endregion
    }
}
