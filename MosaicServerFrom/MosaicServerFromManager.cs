
using LoggerHelper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using RxNS;
using ScreenManagerNS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TxNS;
using Utils;

namespace MosaicServerFrom
{
    public enum MosaicServerJsonType
    {
        //hscmd-video-wall-create,

    }
    public class MosaicServerFromManager
    {
        public MosaicServerEvent Event;
        //MosaicServerReceiveJson ServerReceive;
        Utils_TcpListener Listener = null;
        //ScreenManager ScreenManager;
        MosaicServerSynchro ServerSynchro;

        public Action<string> Event_SynchroServerJson;
        public MosaicServerFromManager()
        {            
            Event = new MosaicServerEvent();
            //ServerReceive = new MosaicServerReceiveJson();
            Listener = new Utils_TcpListener();


        }
        public bool InitialServerFrom(int port)
        {
            bool isOk = true;
            //ServerReceive.Event_FromJson += ManagerJson;
            //isOk = ServerReceive.Initial(port);
            Listener.Event_GetStringFromTcp += ManagerJson;
            Listener.TcpListenerServer(port);
            return isOk;
        }
        public bool InitialServerSynchro(string ip, int port)
        {
            bool isOk = true;
            ServerSynchro = new  MosaicServerSynchro(ip, port);
            Event_SynchroServerJson += ServerSynchro.ServerToSynchroSendMulticastMsg;
            return isOk;
        }

        string ManagerJson(string json)
        {
            string rcvToClient = "";
            JObject obj = null;
            //DataContractJsonSerializer serializer = new DataContractJsonSerializer(;
            try
            {
                obj = JObject.Parse(json);
            }
            catch (Exception ex)
            {
                rcvToClient = "Erro Json ：" + ex.Message;
                Logger.Error(rcvToClient);
                return rcvToClient;
            }


            //JToken jToken;
            //obj.TryGetValue("", StringComparison.CurrentCulture, out jToken);
            var header = obj["cmd_header"];
            string strIns = "";
            if (header != null)
            {
                strIns = header.ToString();
            }

            //switch (strIns)
            //{
            //    case "";
            //        break;
            //    default:
            //        break;
            //}
            switch (strIns)
            {
                case "hscmd-video-wall-create":
                    rcvToClient = HsWallCreate(json);
                    break;
                case "hscmd-video-wall-delete":
                    rcvToClient = HsWallDelete(json); 
                    break;
                case "hscmd-video-wall-modify":
                    rcvToClient = HsWallModifyName(json);
                    break;
                case "hscmd-video-wall-get":
                    rcvToClient = HsWallGet(json);
                    break;
                case "hscmd-video-wall-mirror-bind":
                    rcvToClient = HsWallMirrorBind(json);
                    break;
                case "hscmd-video-wall-rxes-bind":
                    rcvToClient = HsWallRxesBind(json);
                    break;
                case "hscmd-video-wall-clip-set":
                    rcvToClient = HsWallClipSet(json);
                    break;

                //窗口相关

                case "hscmd-video-wall-win-add":
                    rcvToClient = HsWallWinAdd(json);
                    break;
                //case "hscmd-video-wall-win-modify":
                //    rcvToClient = HsWallWinAdd(json);
                //    break;
                case "hscmd-video-wall-win-move":
                    rcvToClient = HsWallWinMove(json);
                    break;
                case "hscmd-video-wall-win-swi":
                    rcvToClient = HsWallWinSwi(json);
                    break;
                case "hscmd-video-wall-win-lay":
                    rcvToClient = HsWallWinLay(json);
                    break;
                case "hscmd-video-wall-win-delete":
                    rcvToClient = HsWallWinClose(json);
                    break;
                case "hscmd-video-wall-win-delete-all":
                    rcvToClient = HsWallWinReMoveAll(json);
                    break;

                case "hscmd-video-wall-scene-call":
                    rcvToClient = HsWallSceneCall(json);
                    break;
                case "hscmd-video-wall-scene-save":
                    rcvToClient = HsWallSceneSave(json);
                    break;
                case "hscmd-video-wall-scene-delete":
                    rcvToClient = HsWallSceneDelete(json);
                    break;
                case "hscmd-video-wall-scene-modify":
                    rcvToClient = HsWallSceneModifyName(json);
                    break;

                #region 预案相关
                case "hscmd-video-wall-preplan-get":
                    rcvToClient = HsWallPrePlansGet(json);
                    break;
                case "hscmd-video-wall-preplan-create":
                    rcvToClient = HsWallPrePlanCreate(json);
                    break;
                case "hscmd-video-wall-preplan-delete":
                    rcvToClient = HsWallPrePlanDelete(json);
                    break;
                case "hscmd-video-wall-preplan-modify-name":
                    rcvToClient = HsWallPrePlanModifyName(json);
                    break;
                case "hscmd-video-wall-preplan-modify":
                    rcvToClient = HsWallPrePlanModify(json);
                    break;
                //case "hscmd-video-wall-preplan-modify":   //查询单个预案  /场景等 单个预案列表/场景列表等
                //    rcvToClient = HsWallSceneModifyName(json);
                //    break;
                #endregion


                case "hscmd-video-wall-win-get":
                    rcvToClient = HsWallSceneCurrent(json);
                    break;
                case "hscmd-video-wall-scene-get":
                    rcvToClient = HsWallSceneGet(json);
                    break;


                case "hscmd-video-wall-get-rx-list":
                    rcvToClient = HsWallListRxGet(json);
                    break;
                case "hscmd-video-wall-get-tx-list":
                    rcvToClient = HsWallListTxGet(json);
                    break;
                default:
                    rcvToClient = "协议暂不支持。。。："+strIns;
                    break;
            }
            //if (strIns == "hscmd-video-wall-create")
            //{
            //    rcvToClient = HsWallCreate(json);
            //}
            //else if (strIns == "hscmd-video-wall-get")
            //{
            //    rcvToClient = HsWallGet(json);
            //}
            //else if (strIns == "hscmd-video-wall-win-add")
            //{
            //    rcvToClient = HsWallWinAdd(json);
            //}
            //else if (strIns == "hscmd-video-wall-win-modify")
            //{
            //    rcvToClient = HsWallWinModify(json);
            //} 
            //else if (strIns == "hscmd-video-wall-win-move")
            //{
            //    rcvToClient = HsWallWinMove(json);
            //}
            //else if (strIns == "hscmd-video-wall-win-swi")
            //{
            //    rcvToClient = HsWallWinSwi(json);
            //}
            //else if (strIns == "hscmd-video-wall-win-lay")
            //{
            //    rcvToClient = HsWallWinLay(json);
            //}
            //else if (strIns == "hscmd-video-wall-win-delete")
            //{
            //    rcvToClient = HsWallWinReMove(json);
            //}
            //else if (strIns == "hscmd-video-wall-win-clear")
            //{
            //    rcvToClient = HsWallWinReMoveAll(json);
            //}
            //else if (strIns == "hscmd-video-wall-scene-call")
            //{
            //    rcvToClient = HsWallSceneCall(json);
            //}
            //else if (strIns == "hscmd-video-wall-scene-save")
            //{
            //    rcvToClient = HsWallSceneSave(json);
            //}
            //else if (strIns == "hscmd-video-wall-scene-delete")
            //{
            //    rcvToClient = HsWallSceneDelete(json);
            //}
            //else if (strIns == "hscmd-video-wall-scene-current")
            //{
            //    rcvToClient = HsWallSceneCurrent(json);
            //}
            //else if (strIns == "hscmd-video-wall-scene-get")
            //{
            //    rcvToClient = HsWallSceneGet(json);
            //}
            //else if (strIns == "hscmd-video-wall-get-rx-list")
            //{
            //    rcvToClient = HsWallListRxGet(json);
            //}
            //else if (strIns == "hscmd-video-wall-get-tx-list")
            //{
            //    rcvToClient = HsWallListTxGet(json);
            //}
            //else  //指令未设定
            //{
            //    rcvToClient = "协议暂不支持。。。";
            //}
            return rcvToClient;
        }

        #region 接收发送客户端Json处理方法------------------
        //大屏操作-------------------
        public string HsWallCreate(string json)
        {
            string rcv = "";
            object result = null;
            bool isOk = false;
            try
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(Json_Send_Hs_Wall_Create));
                    Json_Send_Hs_Wall_Create rcvJs = (Json_Send_Hs_Wall_Create)deseralizer.ReadObject(ms);// //反序列化ReadObject
                    result = rcvJs;
                    var body = rcvJs.cmd_body;
                    int rows = int.Parse(body.Row);
                    int columns = int.Parse(body.Column);
                    HsScreenInfo info = new HsScreenInfo();

                    info.Name = body.WallName;
                    info.Rows = rows;
                    info.Columns = columns;
                    if (!string.IsNullOrEmpty(body.UnitW))
                    {
                        info.UnitWidth = int.Parse(body.UnitW);
                    }
                    if (!string.IsNullOrEmpty(body.UnitH))
                    {
                        info.UnitHeight = int.Parse(body.UnitH);
                    }
                    if (!string.IsNullOrEmpty(body.GapH))
                    {
                        info.GapWidth = int.Parse(body.GapH);
                    }
                    if (!string.IsNullOrEmpty(body.GapH))
                    {
                        info.GapHeight = int.Parse(body.GapH);
                    }

                    if (!string.IsNullOrEmpty(body.StartX))
                    {
                        info.StartX = int.Parse(body.StartX);
                    }
                    if (!string.IsNullOrEmpty(body.StartX))
                    {
                        info.StartY = int.Parse(body.StartY);
                    }
                    if (!string.IsNullOrEmpty(body.StartX))
                    {
                        info.WallPixW = int.Parse(body.WallPixW);
                    }
                    if (!string.IsNullOrEmpty(body.StartX))
                    {
                        info.WallPixH = int.Parse(body.WallPixH);
                    }                                       

                    if (body.IsMirror=="y")
                    {
                        info.IsMirror = true;
                    }
                    else
                    {
                        info.IsMirror = false;
                    }
                    int bindId=0;
                    if (!string.IsNullOrEmpty(body.BindMasterId))
                    {
                        int.TryParse(body.BindMasterId, out bindId);
                    }
                    info.BindMasterId = bindId;
                    foreach (var item in body.RxIDList)
                    {
                        info.Rxid.Add(item);
                    }
                    rcvJs.cmd_end.Err_code = "0";
                    Event?.Event_NewMosaicWall?.Invoke(rcvJs.cmd_end, info);
                    if (rcvJs.cmd_end.Err_code=="0")
                    {
                        isOk = true;
                        rcvJs.cmd_body.WallID= info.IdScreen.ToString();
                        //rcvJs.cmd_body.WallName = body.WallName;  
                    }
                }
            }
            catch (Exception ex)
            {
                hsServer.ShowDebug($"HsWallCreate-异常:{ex.Message}");
            }
            finally
            {
                rcv = hsServer.stringify(result);
                if (isOk)
                {
                    SynchroSendToJson(rcv);
                }
            }
            return rcv;
        }
        public string HsWallDelete(string json)
        {
            string rcv = "";
            object result = null;
            bool isOk = false;
            try
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(Json_Send_Hs_Wall_Delete));
                    Json_Send_Hs_Wall_Delete rcvJs = (Json_Send_Hs_Wall_Delete)deseralizer.ReadObject(ms);// //反序列化ReadObject
                    result = rcvJs;
                    var body = rcvJs.cmd_body;
                    int idScreen =int.Parse(body.WallID);
                    rcvJs.cmd_end.Err_code = "0";
                    Event?.Event_CancelMosaicWall?.Invoke(rcvJs.cmd_end, idScreen);
                    if (rcvJs.cmd_end.Err_code == "0")
                    {
                        isOk = true;
                    }                    
                }
            }
            catch (Exception ex)
            {
                hsServer.ShowDebug($"HsWallDelete-异常:{ex.Message}");
            }
            finally
            {
                rcv = hsServer.stringify(result);
                if (isOk)
                {
                    SynchroSendToJson(rcv);
                }
            }
            return rcv;
        }
        public string HsWallModifyName(string json)
        {
            string rcv = "";
            object result = null;
            bool isOk = false;
            try
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(Json_Send_Hs_Wall_Modify_Name));
                    Json_Send_Hs_Wall_Modify_Name rcvJs = (Json_Send_Hs_Wall_Modify_Name)deseralizer.ReadObject(ms);// //反序列化ReadObject
                    result = rcvJs;
                    var body =  rcvJs.cmd_body;
                    int idScreen = int.Parse(body.WallID);
                    string newName = body.WallName;
                    rcvJs.cmd_end.Err_code = "0";
                    Event?.Event_ModifyMosaicWallName?.Invoke(rcvJs.cmd_end, idScreen ,newName);
                    if (rcvJs.cmd_end.Err_code == "0")
                    {
                        isOk = true;
                    }
                }
            }
            catch (Exception ex)
            {
                hsServer.ShowDebug($"HsWallModifyName-异常:{ex.Message}");
            }
            finally
            {
                rcv = hsServer.stringify(result);
                if (isOk)
                {
                    SynchroSendToJson(rcv);
                }
            }
            return rcv;
        }
        public string HsWallGet(string json)
        {
            string rcv = "";
            Json_Rec_Hs_Wall_Get result = new Json_Rec_Hs_Wall_Get()
            {
                cmd_body = new Json_Rec_Hs_Wall_Get_Body() { Walls = new List<Json_Send_Hs_Wall_Create_Body>() },
                cmd_end = new Json_Rec_Hs_Cmd_End() { Err_code = "0", Err_str = "" }
            };
            try
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(Json_Send_Hs_Wall_Get));
                    Json_Send_Hs_Wall_Get rcvJs = (Json_Send_Hs_Wall_Get)deseralizer.ReadObject(ms);// //反序列化ReadObject
                    result.cmd_header = rcvJs.cmd_header;
                    var body = rcvJs.cmd_body;
                    List<HsScreenInfo> list = new List<HsScreenInfo>();

                    foreach (var item in body.Walls)
                    {
                        list.Add(new HsScreenInfo() { IdScreen = int.Parse(item) });
                    }
                    Event?.Event_GetMosaicWall?.Invoke(result.cmd_end, list);
                    if (result.cmd_end.Err_code == "0")
                    {
                        foreach (var item in list)
                        {
                            Json_Send_Hs_Wall_Create_Body wall = new Json_Send_Hs_Wall_Create_Body()
                            {
                                RxIDList = item.Rxid,
                                WallID = item.IdScreen.ToString(),
                                WallName = item.Name,

                                Row = item.Rows.ToString(),
                                Column = item.Columns.ToString(),
                                UnitW=item.UnitWidth.ToString(),
                                UnitH=item.UnitHeight.ToString(),
                                GapW=item.GapWidth.ToString(),
                                GapH=item.GapHeight.ToString(),

                                StartX = item.StartX.ToString(),
                                StartY = item.StartY.ToString(),
                                WallPixW = item.WallPixW.ToString(),
                                WallPixH = item.WallPixH.ToString(),

                                BindMasterId = item.BindMasterId.ToString()
                            };
                            if (item.IsMirror)
                            {
                                wall.IsMirror = "y";
                            }
                            else
                            {
                                wall.IsMirror = "n";
                            }
                            result.cmd_body.Walls.Add(wall);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                hsServer.ShowDebug("HsWallGet-异常：" + ex.Message);
                result.cmd_end.Err_code = "1";
                result.cmd_end.Err_str = "HsWallGet-异常：" + ex.Message;
            }
            finally
            {
                rcv = hsServer.stringify(result);
            }
            return rcv;
        }
        public string HsWallMirrorBind(string json)
        {
            string rcv = "";
            Json_Send_Hs_Wall_Mirror_Bind result = null;
            bool isOk = false;
            try
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(Json_Send_Hs_Wall_Mirror_Bind));
                    Json_Send_Hs_Wall_Mirror_Bind rcvJs = (Json_Send_Hs_Wall_Mirror_Bind)deseralizer.ReadObject(ms);// //反序列化ReadObject
                    result = rcvJs;
                    var body = rcvJs.cmd_body;
                    int idScreen = int.Parse(body.MirrorID);
                    int idMaster = int.Parse(body.BindMasterId);
                    rcvJs.cmd_end.Err_code = "0";
                    Event?.Event_SetMirrorBind?.Invoke(rcvJs.cmd_end, idScreen, idMaster);
                    if (rcvJs.cmd_end.Err_code == "0")
                    {
                        isOk = true;
                    }
                }
            }
            catch (Exception ex)
            {
                hsServer.ShowDebug($"HsWallMirrorBind-异常:{ex.Message}");
                if (result !=null)
                {
                    result.cmd_end.Err_code = "1";
                    result.cmd_end.Err_str = "errror：" + ex.Message;
                }

            }
            finally
            {
                rcv = hsServer.stringify(result);
                if (isOk)
                {
                    SynchroSendToJson(rcv);
                }
            }
            return rcv;
        }
        public string HsWallRxesBind(string json)
        {
            string rcv = "";
            object result = null;
            bool isOk = false;
            try
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(Json_Send_Hs_Wall_Rxes_Bind));
                    Json_Send_Hs_Wall_Rxes_Bind rcvJs = (Json_Send_Hs_Wall_Rxes_Bind)deseralizer.ReadObject(ms);// //反序列化ReadObject
                    result = rcvJs;
                    var body = rcvJs.cmd_body;
                    HsScreenInfo info = new HsScreenInfo();
                    info.IdScreen = int.Parse(body.WallID);
                    foreach (var item in body.RxIDList)
                    {
                        info.Rxid.Add(item);
                    }
                    rcvJs.cmd_end.Err_code = "0";
                    Event?.Event_SetRxesBind?.Invoke(rcvJs.cmd_end, info);
                    if (rcvJs.cmd_end.Err_code == "0")
                    {
                        isOk = true;
                    }
                }
            }
            catch (Exception ex)
            {
                hsServer.ShowDebug($"HsWallRxesBind-异常:{ex.Message}");
            }
            finally
            {
                rcv = hsServer.stringify(result);
                if (isOk)
                {
                    SynchroSendToJson(rcv);
                }
            }
            return rcv;
        }
        public string HsWallClipSet(string json)
        {
            string rcv = "";
            object result = null;
            bool isOk = false;
            try
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(Json_Send_Hs_Wall_Clip_Set));
                    Json_Send_Hs_Wall_Clip_Set rcvJs = (Json_Send_Hs_Wall_Clip_Set)deseralizer.ReadObject(ms);// //反序列化ReadObject
                    result = rcvJs;
                    var body = rcvJs.cmd_body;
                    HsScreenInfo info = new HsScreenInfo();
                    info.IdScreen = int.Parse(body.WallID);
                    info.StartX = int.Parse(body.StartX);
                    info.StartY = int.Parse(body.StartY);
                    info.WallPixW = int.Parse(body.WallPixW);
                    info.WallPixH = int.Parse(body.WallPixH);
                    rcvJs.cmd_end.Err_code = "0";
                    Event?.Event_SetScreenClip?.Invoke(rcvJs.cmd_end, info);
                    if (rcvJs.cmd_end.Err_code == "0")
                    {
                        isOk = true;
                    }
                }
            }
            catch (Exception ex)
            {
                hsServer.ShowDebug($"HsWallClipSet-异常:{ex.Message}");
            }
            finally
            {
                rcv = hsServer.stringify(result);
                if (isOk)
                {
                    SynchroSendToJson(rcv);
                }
            }
            return rcv;
        }



        //窗口操作------------------------
        public string HsWallWinAdd(string json)
        {
            string rcv = "";
            object result = null;
            bool isOk = false;
            try
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(Json_Send_Hs_Wall_Open_Win));
                    Json_Send_Hs_Wall_Open_Win rcvJs = (Json_Send_Hs_Wall_Open_Win)deseralizer.ReadObject(ms);// //反序列化ReadObject
                    result = rcvJs;
                    var body = rcvJs.cmd_body;
                    HsMosaicWinInfo win = new HsMosaicWinInfo();
                    win.IdScreen = int.Parse(body.WallID);
                    win.X = int.Parse(body.WinPosX);
                    win.Y = int.Parse(body.WinPosY);
                    win.Width = int.Parse(body.WinDisplayWidth);
                    win.Height = int.Parse(body.WinDisplayHeight);
                    win.IdTx = body.TxID;

                    rcvJs.cmd_end.Err_code = "0";
                    Event?.Event_OpenMosaicWin?.Invoke(rcvJs.cmd_end,win);
                    if (rcvJs.cmd_end.Err_code =="0")
                    {
                        isOk = true;
                        rcvJs.cmd_body.WinID = win.IdWin.ToString();
                        rcvJs.cmd_body.WinLay = win.ZIndex.ToString();

                        body.WinPosX = win.X.ToString();
                        body.WinPosY = win.Y.ToString();
                        body.WinDisplayWidth = win.Width.ToString();
                        body.WinDisplayHeight = win.Height.ToString();
                    }                    
                }
            }
            catch (Exception ex)
            {
                hsServer.ShowDebug("HsWallWinAdd-异常：" + ex.Message);
            }
            finally
            {
                rcv = hsServer.stringify(result);
                if (isOk)
                {
                    SynchroSendToJson(rcv);
                }
            }
            return rcv;
        }
        public string HsWallWinModify(string json)  //保留
        {
            string rcv = "";
            bool isOk = false;
            Json_Rec_Hs_Result result = new Json_Rec_Hs_Result() { cmd_end = new Json_Rec_Hs_Cmd_End() { Err_code = "0", Err_str = "" } };
            try
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(Json_Send_Hs_Wall_Modify_Win));
                    Json_Send_Hs_Wall_Modify_Win rcvJs = (Json_Send_Hs_Wall_Modify_Win)deseralizer.ReadObject(ms);// //反序列化ReadObject
                    result.cmd_header = rcvJs.cmd_header;
                    var body = rcvJs.cmd_body;

                    HsMosaicWinInfo win = new HsMosaicWinInfo();

                    win.X = int.Parse(body.WinPosX);
                    win.Y = int.Parse(body.WinPosY);
                    win.Width = int.Parse(body.WinDisplayWidth);
                    win.Height = int.Parse(body.WinDisplayHeight);
                    win.IdWin = int.Parse(body.WinID);
                    win.IdScreen = int.Parse(body.WallID);
                    win.ZIndex = int.Parse(body.WinLay);
                    win.IdTx = body.TxID;
                    Event?.Event_ModifyMosaicWin?.Invoke(result.cmd_end, win);

                    if (result.cmd_end.Err_code == "0")
                    {
                        Json_SynchroTo_Hs_Wall_Modify_Win_Body synchroJsbody = new Json_SynchroTo_Hs_Wall_Modify_Win_Body();
                        Json_SynchroTo_Hs_Wall_Modify_Win synchroJs = new Json_SynchroTo_Hs_Wall_Modify_Win() { cmd_body = synchroJsbody };
                        synchroJsbody.WallID = body.WallID;
                        synchroJsbody.WinID = body.WinID;
                        synchroJsbody.WinPosX = win.X.ToString();
                        synchroJsbody.WinPosY = win.Y.ToString();
                        synchroJsbody.WinDisplayWidth = win.Width.ToString();
                        synchroJsbody.WinDisplayHeight = win.Height.ToString();
                        synchroJsbody.TxID = win.IdTx;
                        synchroJsbody.WinLay = win.ZIndex.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                hsServer.ShowDebug("HsWallWinModify-异常：" + ex.Message);
                result.cmd_end.Err_code = "1";
                result.cmd_end.Err_str =ex.Message;
            }
            finally
            {
                rcv = hsServer.stringify(result);
            }
            return rcv;
        }
        public string HsWallWinMove(string json)
        {
            string rcv = "";
            object result = null;
            bool isOk = false;
            try
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(Json_Send_Hs_Wall_Modify_Win));
                    Json_Send_Hs_Wall_Modify_Win rcvJs = (Json_Send_Hs_Wall_Modify_Win)deseralizer.ReadObject(ms);// //反序列化ReadObject
                    result = rcvJs;
                    var body = rcvJs.cmd_body;

                    HsMosaicWinInfo win = new HsMosaicWinInfo();

                    win.IdWin = int.Parse(body.WinID);
                    win.IdScreen = int.Parse(body.WallID);
                    win.X = int.Parse(body.WinPosX);
                    win.Y = int.Parse(body.WinPosY);
                    win.Width = int.Parse(body.WinDisplayWidth);
                    win.Height = int.Parse(body.WinDisplayHeight);


                    rcvJs.cmd_end.Err_code = "0";
                    Event?.Event_MoveMosaicWin?.Invoke(rcvJs.cmd_end, win);
                    if (rcvJs.cmd_end.Err_code == "0")
                    {
                        body.WinPosX = win.X.ToString();
                        body.WinPosY = win.Y.ToString();
                        body.WinDisplayWidth = win.Width.ToString();
                        body.WinDisplayHeight = win.Height.ToString();
                        body.WinLay = win.ZIndex.ToString();
                        body.TxID = win.IdTx;
                        isOk = true;
                    }
                }
            }
            catch (Exception ex)
            {
                hsServer.ShowDebug("HsWallWinMove-异常：" + ex.Message);
            }
            finally
            {
                rcv = hsServer.stringify(result);
                if (isOk)
                {
                    SynchroSendToJson(rcv);
                }
            }
            return rcv;
        }
        public string HsWallWinLay(string json)
        {
            string rcv = "";
            object result = null;
            bool isOk = false;
            try
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(Json_Send_Hs_Wall_Modify_Win));
                    Json_Send_Hs_Wall_Modify_Win rcvJs = (Json_Send_Hs_Wall_Modify_Win)deseralizer.ReadObject(ms);// //反序列化ReadObject
                    result = rcvJs;
                    var body = rcvJs.cmd_body;

                    HsMosaicWinInfo win = new HsMosaicWinInfo();

                    win.IdWin = int.Parse(body.WinID);
                    win.IdScreen = int.Parse(body.WallID);
                    win.ZIndex = int.Parse(body.WinLay);

                    rcvJs.cmd_end.Err_code = "0";
                    Event?.Event_LayMosaicWin?.Invoke(rcvJs.cmd_end, win);
                    if (rcvJs.cmd_end.Err_code == "0")
                    {
                        isOk = true;
                        body.WinPosX = win.X.ToString();
                        body.WinPosY = win.Y.ToString();
                        body.WinDisplayWidth = win.Width.ToString();
                        body.WinDisplayHeight = win.Height.ToString();
                        body.TxID = win.IdTx;
                    }
                }
            }
            catch (Exception ex)
            {
                hsServer.ShowDebug("HsWallWinLay-异常：" + ex.Message);
            }
            finally
            {
                rcv = hsServer.stringify(result);
                if (isOk)
                {
                    SynchroSendToJson(rcv);
                }
            }
            return rcv;
        }
        public string HsWallWinSwi(string json)
        {
            string rcv = "";
            object result = null;
            bool isOk = false;
            try
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(Json_Send_Hs_Wall_Modify_Win));
                    Json_Send_Hs_Wall_Modify_Win rcvJs = (Json_Send_Hs_Wall_Modify_Win)deseralizer.ReadObject(ms);// //反序列化ReadObject
                    result = rcvJs;
                    var body = rcvJs.cmd_body;

                    HsMosaicWinInfo win = new HsMosaicWinInfo();

                    win.IdWin = int.Parse(body.WinID);
                    win.IdScreen = int.Parse(body.WallID);
                    win.IdTx = body.TxID;

                    rcvJs.cmd_end.Err_code = "0";
                    Event?.Event_SwiMosaicWin?.Invoke(rcvJs.cmd_end, win);
                    if (rcvJs.cmd_end.Err_code == "0")
                    {
                        isOk = true;
                        body.WinLay = win.ZIndex.ToString();
                        body.WinPosX = win.X.ToString();
                        body.WinPosY = win.Y.ToString();
                        body.WinDisplayWidth = win.Width.ToString();
                        body.WinDisplayHeight = win.Height.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                hsServer.ShowDebug("HsWallWinSwi-异常：" + ex.Message);
            }
            finally
            {
                rcv = hsServer.stringify(result);
                if (isOk)
                {
                    SynchroSendToJson(rcv);
                }
            }
            return rcv;
        }
        public string HsWallWinClose(string json)
        {
            string rcv = "";
            object result = null;
            bool isOk = false;
            try
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(Json_Send_Hs_Wall_Close_Win));
                    Json_Send_Hs_Wall_Close_Win rcvJs = (Json_Send_Hs_Wall_Close_Win)deseralizer.ReadObject(ms);// //反序列化ReadObject
                    result = rcvJs;
                    var body = rcvJs.cmd_body;
                    int idWin = int.Parse(body.WinID);
                    int idWall = int.Parse(body.WallID);

                    rcvJs.cmd_end.Err_code = "0";
                    Event?.Event_CloseMosaicWin?.Invoke(rcvJs.cmd_end, idWall, idWin);
                    if (rcvJs.cmd_end.Err_code == "0")
                    {
                        isOk = true;
                    }
                }
            }
            catch (Exception ex)
            {
                hsServer.ShowDebug("HsWallWinReMove-异常：" + ex.Message);
            }
            finally
            {
                rcv = hsServer.stringify(result);
                if (isOk)
                {
                    SynchroSendToJson(rcv);
                }
            }
            return rcv;
        }
        public string HsWallWinReMoveAll(string json)
        {
            string rcv = "";
            object result = null;
            bool isOk = false;
            try
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(Json_Send_Hs_Wall_Clear_Win));
                    Json_Send_Hs_Wall_Clear_Win rcvJs = (Json_Send_Hs_Wall_Clear_Win)deseralizer.ReadObject(ms);// //反序列化ReadObject
                    result = rcvJs;
                    var body = rcvJs.cmd_body;
                    int idWall = int.Parse(body.WallID);

                    rcvJs.cmd_end.Err_code = "0";
                    Event?.Event_CloseAllMosaicWin?.Invoke(rcvJs.cmd_end, idWall);
                    if (rcvJs.cmd_end.Err_code == "0")
                    {
                        isOk = true;
                    }
                }
            }
            catch (Exception ex)
            {
                hsServer.ShowDebug("HsWallWinReMove-异常：" + ex.Message);
            }
            finally
            {
                rcv = hsServer.stringify(result);
                if (isOk)
                {
                    SynchroSendToJson(rcv);
                }
            }
            return rcv;
        }

        //场景相关操作--------------------
        public string HsWallSceneSave(string json)
        {
            string rcv = "";
            Json_Rec_Hs_Wall_Save_Scene result = new Json_Rec_Hs_Wall_Save_Scene() { cmd_end = new Json_Rec_Hs_Cmd_End{ Err_code = "0", Err_str = "" } };
            try
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(Json_Send_Hs_Wall_Save_Scene));
                    Json_Send_Hs_Wall_Save_Scene rcvJs = (Json_Send_Hs_Wall_Save_Scene)deseralizer.ReadObject(ms);// //反序列化ReadObject
                    result.cmd_header = rcvJs.cmd_header;
                    result.cmd_end = rcvJs.cmd_end;

                    var body = rcvJs.cmd_body;

                    SceneInfo scene = new SceneInfo();
                    scene.IdScreen = int.Parse(body.WallID);
                    scene.Name = body.SceneName;

                    rcvJs.cmd_end.Err_code = "0";
                    
                    Event?.Event_SaveScene?.Invoke(result.cmd_end, scene);
                    if (result.cmd_end.Err_code == "0")
                    {
                        Json_Rec_Hs_Wall_Save_Scene_Body rcvbody = new Json_Rec_Hs_Wall_Save_Scene_Body();
                        result.cmd_body = rcvbody;
                        var rcvScene= rcvbody.Scene = new Json_Send_Hs_Wall_Save_Scene_Body_Scene();
                        rcvScene.WallID = scene.IdScreen.ToString();
                        rcvScene.SceneName = scene.Name;
                        var windows= rcvScene.Windows = new List<Json_Rec_Hs_Wall_Current_Scene_Body_Wall_Windows>();

                        foreach (var item in scene.ListWins)
                        {
                            Json_Rec_Hs_Wall_Current_Scene_Body_Wall_Windows win = new Json_Rec_Hs_Wall_Current_Scene_Body_Wall_Windows();
                            win.WinID = item.IdWin.ToString();
                            win.PosX = item.X.ToString();
                            win.PosY = item.Y.ToString();
                            win.WinW = item.Width.ToString();
                            win.WinH = item.Height.ToString();
                            win.WinLay = item.ZIndex.ToString();
                            win.TxID = item.IdTx;
                            windows.Add(win);
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                hsServer.ShowDebug("HsWallSceneSave-异常：" + ex.Message);
                result.cmd_end.Err_code = "1";
                result.cmd_end.Err_str = ex.Message;
            }
            finally
            {
                rcv = hsServer.stringify(result);
                if (result.cmd_end.Err_code=="0")
                {
                    SynchroSendToJson(rcv);
                }
            }
            return rcv;
        }
        public string HsWallSceneCall(string json)
        {
            string rcv = "";
            object result = null;
            bool isOk = false;
            try
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(Json_Send_Hs_Wall_Call_Scene));
                    Json_Send_Hs_Wall_Call_Scene rcvJs = (Json_Send_Hs_Wall_Call_Scene)deseralizer.ReadObject(ms);// //反序列化ReadObject
                    result = rcvJs;
                    var body = rcvJs.cmd_body;

                    int idWall = int.Parse(body.WallID);
                    string sceneName = body.SceneName;

                    rcvJs.cmd_end.Err_code = "0";
                    Event?.Event_CallScene?.Invoke(rcvJs.cmd_end, idWall, sceneName);
                    if (rcvJs.cmd_end.Err_code == "0")
                    {
                        isOk = true;
                    }
                }
            }
            catch (Exception ex)
            {
                hsServer.ShowDebug("HsWallSceneCall-异常：" + ex.Message);
            }
            finally
            {
                rcv = hsServer.stringify(result);
                if (isOk)
                {
                    SynchroSendToJson(rcv);
                }
            }
            return rcv;
        }
        public string HsWallSceneDelete(string json)
        {
            string rcv = "";
            object result = null;
            bool isOk = false;
            try
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(Json_Send_Hs_Wall_Delete_Scene));
                    Json_Send_Hs_Wall_Delete_Scene rcvJs = (Json_Send_Hs_Wall_Delete_Scene)deseralizer.ReadObject(ms);// //反序列化ReadObject
                    result = rcvJs;
                    var body = rcvJs.cmd_body;

                    int idWall = int.Parse(body.WallID);
                    string sceneName = body.SceneName;

                    rcvJs.cmd_end.Err_code = "0";
                    Event?.Event_DeleteScene?.Invoke(rcvJs.cmd_end, idWall, sceneName);
                    if (rcvJs.cmd_end.Err_code == "0")
                    {
                        isOk = true;
                    }
                }
            }
            catch (Exception ex)
            {
                hsServer.ShowDebug("HsWallSceneDelete-异常：" + ex.Message);
            }
            finally
            {
                rcv = hsServer.stringify(result);
                if (isOk)
                {
                    SynchroSendToJson(rcv);
                }
            }
            return rcv;
        }
        public string HsWallSceneModifyName(string json)
        {
            string rcv = "";
            object result = null;
            bool isOk = false;
            try
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(Json_Send_Hs_Wall_Modify_SceneName));
                    Json_Send_Hs_Wall_Modify_SceneName rcvJs = (Json_Send_Hs_Wall_Modify_SceneName)deseralizer.ReadObject(ms);// //反序列化ReadObject
                    result = rcvJs;
                    var body = rcvJs.cmd_body;

                    int idWall = int.Parse(body.WallID);
                    string sceneName = body.SceneName;
                    string newName = body.NewName;

                    rcvJs.cmd_end.Err_code = "0";
                    Event?.Event_ModifySceneName?.Invoke(rcvJs.cmd_end, idWall, sceneName, newName);
                    if (rcvJs.cmd_end.Err_code == "0")
                    {
                        isOk = true;
                    }
                }
            }
            catch (Exception ex)
            {
                hsServer.ShowDebug("HsWallSceneModifyName-异常：" + ex.Message);
            }
            finally
            {
                rcv = hsServer.stringify(result);
                if (isOk)
                {
                    SynchroSendToJson(rcv);
                }
            }
            return rcv;
        }
        public string HsWallSceneCurrent(string json)
        {
            string rcv = "";
            Json_Rec_Hs_Wall_Current_Scene result = new Json_Rec_Hs_Wall_Current_Scene()
            {
                cmd_body = new Json_Rec_Hs_Wall_Current_Scene_Body() { Walls = new List<Json_Rec_Hs_Wall_Current_Scene_Body_Wall>() },
                cmd_end = new Json_Rec_Hs_Cmd_End() { Err_code = "0", Err_str = "" }
            };
            try
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(Json_Send_Hs_Wall_Current_Scene));
                    Json_Send_Hs_Wall_Current_Scene rcvJs = (Json_Send_Hs_Wall_Current_Scene)deseralizer.ReadObject(ms);// //反序列化ReadObject
                    result.cmd_header = rcvJs.cmd_header;
                    var body = rcvJs.cmd_body;

                    List<HsScreenInfo> screens = new List<HsScreenInfo>();
                    foreach (var item in body.Walls)
                    {
                        HsScreenInfo screen = new HsScreenInfo();
                        screen.IdScreen = int.Parse(item);
                        screens.Add(screen);
                    }

                    Event?.Event_GetCurrentWins?.Invoke(result.cmd_end, screens);
                    if (result.cmd_end.Err_code == "0")
                    {
                        var walls = result.cmd_body.Walls = new List<Json_Rec_Hs_Wall_Current_Scene_Body_Wall>();

                        foreach (var screenInfo in screens)
                        {
                            Json_Rec_Hs_Wall_Current_Scene_Body_Wall wall = new Json_Rec_Hs_Wall_Current_Scene_Body_Wall()
                            {
                                WallID = screenInfo.IdScreen.ToString(),
                                WallName = screenInfo.Name,
                                Row = screenInfo.Rows.ToString(),
                                Column = screenInfo.Columns.ToString(),
                                StartX = screenInfo.StartX.ToString(),
                                StartY = screenInfo.StartY.ToString(),
                                WallPixW = screenInfo.WallPixW.ToString(),
                                WallPixH = screenInfo.WallPixH.ToString()
                            };
                            walls.Add(wall);

                            wall.Windows = new List<Json_Rec_Hs_Wall_Current_Scene_Body_Wall_Windows>();
                            foreach (var win in screenInfo.CurrentWins)
                            {
                                Json_Rec_Hs_Wall_Current_Scene_Body_Wall_Windows winJson = new Json_Rec_Hs_Wall_Current_Scene_Body_Wall_Windows()
                                {
                                    WinID = win.IdWin.ToString(),
                                    PosX = win.X.ToString(),
                                    PosY = win.Y.ToString(),
                                    WinW = win.Width.ToString(),
                                    WinH = win.Height.ToString(),
                                    WinLay = win.ZIndex.ToString(),
                                    TxID = win.IdTx
                                };
                                wall.Windows.Add(winJson);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                hsServer.ShowDebug($"{nameof(HsWallSceneCurrent)} -异常：" + ex.Message);
                result.cmd_end.Err_code = "1";
                result.cmd_end.Err_str = ex.Message;
            }
            finally
            {
                rcv = hsServer.stringify(result);
            }
            return rcv;
        }
        public string HsWallSceneGet(string json)
        {
            string rcv = "";
            Json_Rec_Hs_Wall_Get_Scene result = new Json_Rec_Hs_Wall_Get_Scene()
            {
                cmd_body = new Json_Rec_Hs_Wall_Get_Scene_Body() { Walls = new List<Json_Rec_Hs_Wall_Get_Scene_Body_Wall>() },
                cmd_end = new Json_Rec_Hs_Cmd_End() { Err_code = "0", Err_str = "" }
            };
            try
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(Json_Send_Hs_Wall_Get_Scene));
                    Json_Send_Hs_Wall_Get_Scene rcvJs = (Json_Send_Hs_Wall_Get_Scene)deseralizer.ReadObject(ms);// //反序列化ReadObject
                    result.cmd_header = rcvJs.cmd_header;
                    var body = rcvJs.cmd_body;

                    List<HsScreenInfo> screens = new List<HsScreenInfo>();
                    foreach (var item in body.Walls)
                    {
                        HsScreenInfo screen = new HsScreenInfo();
                        screen.IdScreen = int.Parse(item);
                        screens.Add(screen);
                    }

                    Event?.Event_GetScenes?.Invoke(result.cmd_end, screens);
                    if (result.cmd_end.Err_code == "0")
                    {
                        var walls = result.cmd_body.Walls ;

                        foreach (var screenInfo in screens)
                        {
                            Json_Rec_Hs_Wall_Get_Scene_Body_Wall wall = new Json_Rec_Hs_Wall_Get_Scene_Body_Wall()
                            {
                                WallID = screenInfo.IdScreen.ToString(),
                                WallName = screenInfo.Name,
                                WallRow = screenInfo.Rows.ToString(),
                                WallCol = screenInfo.Columns.ToString(),
                                WallStartX = screenInfo.StartX.ToString(),
                                WallStartY = screenInfo.StartY.ToString(),
                                WallPixW = screenInfo.WallPixW.ToString(),
                                WallPixH = screenInfo.WallPixH.ToString()
                            };
                            walls.Add(wall);
                            
                            wall.Scene = new List<Json_Rec_Hs_Wall_Get_Scene_Body_Wall_Scene>();
                            foreach (var sceneInfo in screenInfo.Scenes)
                            {
                                Json_Rec_Hs_Wall_Get_Scene_Body_Wall_Scene sceneJson = new Json_Rec_Hs_Wall_Get_Scene_Body_Wall_Scene()
                                {
                                    SceneName = sceneInfo.Name,
                                    Windows = new List<Json_Rec_Hs_Wall_Current_Scene_Body_Wall_Windows>()
                                };
                                wall.Scene.Add(sceneJson);
                                foreach (var win in sceneInfo.ListWins)
                                {
                                    Json_Rec_Hs_Wall_Current_Scene_Body_Wall_Windows winJson = new Json_Rec_Hs_Wall_Current_Scene_Body_Wall_Windows()
                                    {
                                        WinID = win.IdWin.ToString(),
                                        PosX = win.X.ToString(),
                                        PosY = win.Y.ToString(),
                                        WinW = win.Width.ToString(),
                                        WinH = win.Height.ToString(),
                                        WinLay = win.ZIndex.ToString(),
                                        TxID = win.IdTx
                                    };
                                    sceneJson.Windows.Add(winJson);
                                }                                
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                hsServer.ShowDebug($"{nameof(HsWallSceneCurrent)} -异常：" + ex.Message);
                result.cmd_end.Err_code = "1";
                result.cmd_end.Err_str =  ex.Message;
            }
            finally
            {
                rcv = hsServer.stringify(result);
            }
            return rcv;
        }

        //预案相关操作--------------------
        #region 预案相关---------
        public string HsWallPrePlanCreate(string json)
        {
            string rcv = "";
            object result = null;
            bool isOk = false;
            try
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(Json_Send_Hs_Wall_Create_PrePlan));
                    Json_Send_Hs_Wall_Create_PrePlan rcvJs = (Json_Send_Hs_Wall_Create_PrePlan)deseralizer.ReadObject(ms);// //反序列化ReadObject
                    result = rcvJs;
                    var body = rcvJs.cmd_body;

                    PrePlanInfo info = new PrePlanInfo();
                    info.IdScreen = int.Parse(body.WallID);
                    info.Name = body.PrePlan.PrePlanName;
                    info.SwiInterval =int.Parse(body.PrePlan.SwiInterval);
                    info.SceneNames = body.PrePlan.SceneNames;

                    Event?.Event_CreatePrePlan?.Invoke(rcvJs.cmd_end, info);
                    if (rcvJs.cmd_end.Err_code == "0")
                    {
                        isOk = true;
                    }

                }
            }
            catch (Exception ex)
            {
                hsServer.ShowDebug("HsWallPrePlanCreate-异常：" + ex.Message);
            }
            finally
            {
                rcv = hsServer.stringify(result);
                if (isOk)
                {
                    SynchroSendToJson(rcv);
                }
            }
            return rcv;
        }
        public string HsWallPrePlanModify(string json)
        {
            string rcv = "";
            object result = null;
            bool isOk = false;
            try
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(Json_Send_Hs_Wall_Create_PrePlan));
                    Json_Send_Hs_Wall_Create_PrePlan rcvJs = (Json_Send_Hs_Wall_Create_PrePlan)deseralizer.ReadObject(ms);// //反序列化ReadObject
                    result = rcvJs;
                    var body = rcvJs.cmd_body;

                    PrePlanInfo info = new PrePlanInfo();
                    info.IdScreen = int.Parse(body.WallID);
                    info.Name = body.PrePlan.PrePlanName;
                    info.SwiInterval = int.Parse(body.PrePlan.SwiInterval);
                    info.SceneNames = body.PrePlan.SceneNames;

                    Event?.Event_ModifyPrePlan?.Invoke(rcvJs.cmd_end, info);
                    if (rcvJs.cmd_end.Err_code == "0")
                    {
                        isOk = true;
                    }

                }
            }
            catch (Exception ex)
            {
                hsServer.ShowDebug("HsWallPrePlanCreate-异常：" + ex.Message);
            }
            finally
            {
                rcv = hsServer.stringify(result);
                if (isOk)
                {
                    SynchroSendToJson(rcv);
                }
            }
            return rcv;
        }
        public string HsWallPrePlanDelete(string json)
        {
            string rcv = "";
            object result = null;
            bool isOk = false;
            try
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(Json_Send_Hs_Wall_Delete_PrePlan));
                    Json_Send_Hs_Wall_Delete_PrePlan rcvJs = (Json_Send_Hs_Wall_Delete_PrePlan)deseralizer.ReadObject(ms);// //反序列化ReadObject
                    result = rcvJs;
                    var body = rcvJs.cmd_body;

                    int idWall = int.Parse(body.WallID);
                    string name = body.PrePlanName;

                    rcvJs.cmd_end.Err_code = "0";
                    Event?.Event_DeletePrePlan?.Invoke(rcvJs.cmd_end, idWall, name);
                    if (rcvJs.cmd_end.Err_code == "0")
                    {
                        isOk = true;
                    }
                }
            }
            catch (Exception ex)
            {
                hsServer.ShowDebug("HsWallPrePlanDelete-异常：" + ex.Message);
            }
            finally
            {
                rcv = hsServer.stringify(result);
                if (isOk)
                {
                    SynchroSendToJson(rcv);
                }
            }
            return rcv;
        }
        public string HsWallPrePlanModifyName(string json)
        {
            string rcv = "";
            object result = null;
            bool isOk = false;
            try
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(Json_Send_Hs_Wall_Modify_PrePlan_Name));
                    Json_Send_Hs_Wall_Modify_PrePlan_Name rcvJs = (Json_Send_Hs_Wall_Modify_PrePlan_Name)deseralizer.ReadObject(ms);// //反序列化ReadObject
                    result = rcvJs;
                    var body = rcvJs.cmd_body;

                    int idWall = int.Parse(body.WallID);
                    string name = body.PrePlanName;
                    string newName = body.NewName;

                    rcvJs.cmd_end.Err_code = "0";
                    Event?.Event_ModifyPrePlanName?.Invoke(rcvJs.cmd_end, idWall, name, newName);
                    if (rcvJs.cmd_end.Err_code == "0")
                    {
                        isOk = true;
                    }
                }
            }
            catch (Exception ex)
            {
                hsServer.ShowDebug("HsWallPrePlanModifyName-异常：" + ex.Message);
            }
            finally
            {
                rcv = hsServer.stringify(result);
                if (isOk)
                {
                    SynchroSendToJson(rcv);
                }
            }
            return rcv;
        }
        public string HsWallPrePlansGet(string json)
        {
            string rcv = "";
            Json_Rec_Hs_Wall_Get_PrePlan result = new Json_Rec_Hs_Wall_Get_PrePlan()
            {
                cmd_body = new Json_Rec_Hs_Wall_Get_PrePlan_Body() { Walls = new List<Json_Rec_Hs_Wall_Get_PrePlan_Body_Wall>() },
                cmd_end = new Json_Rec_Hs_Cmd_End() { Err_code = "0", Err_str = "" }
            };
            try
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(Json_Send_Hs_Wall_Get_PrePlan));
                    Json_Send_Hs_Wall_Get_PrePlan rcvJs = (Json_Send_Hs_Wall_Get_PrePlan)deseralizer.ReadObject(ms);// //反序列化ReadObject
                    result.cmd_header = rcvJs.cmd_header;
                    var body = rcvJs.cmd_body;

                    List<HsScreenInfo> screens = new List<HsScreenInfo>();
                    foreach (var item in body.Walls)
                    {
                        HsScreenInfo screen = new HsScreenInfo();
                        screen.IdScreen = int.Parse(item);
                        screens.Add(screen);
                    }

                    Event?.Event_GetPrePlans?.Invoke(result.cmd_end, screens);
                    if (result.cmd_end.Err_code == "0")
                    {
                        var walls = result.cmd_body.Walls;

                        foreach (var screenInfo in screens)
                        {
                            Json_Rec_Hs_Wall_Get_PrePlan_Body_Wall wall = new Json_Rec_Hs_Wall_Get_PrePlan_Body_Wall()
                            {
                                WallID = screenInfo.IdScreen.ToString()
                            };
                            walls.Add(wall);

                            wall.PrePlans = new  List<Json_Rec_Hs_Wall_Get_PrePlan_Body_Wall_PrePlan>();
                            foreach (var sceneInfo in screenInfo.PrePlans)
                            {
                                Json_Rec_Hs_Wall_Get_PrePlan_Body_Wall_PrePlan sceneJson = new Json_Rec_Hs_Wall_Get_PrePlan_Body_Wall_PrePlan()
                                {
                                    PrePlanName = sceneInfo.Name,
                                    SwiInterval=sceneInfo.SwiInterval.ToString(),
                                    SceneNames = sceneInfo.SceneNames
                                };
                                wall.PrePlans.Add(sceneJson);                                
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                hsServer.ShowDebug($"{nameof(HsWallPrePlansGet)} -异常：" + ex.Message);
                result.cmd_end.Err_code = "1";
                result.cmd_end.Err_str = ex.Message;
            }
            finally
            {
                rcv = hsServer.stringify(result);
            }
            return rcv;
        }
        #endregion
        //Tx/Rx列表相关操作---------------------
        public string HsWallListRxGet(string json)
        {
            string rcv = "";
            Json_Rec_Hs_ListRx_Get result = new Json_Rec_Hs_ListRx_Get() { cmd_end = new Json_Rec_Hs_Cmd_End() { Err_code = "0", Err_str = "" } };
            try
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(Json_Send_Hs_ListRx_Get));
                    Json_Send_Hs_ListRx_Get rcvJs = (Json_Send_Hs_ListRx_Get)deseralizer.ReadObject(ms);// //反序列化ReadObject
                    result.cmd_header = rcvJs.cmd_header;
                     

                    result.cmd_body = new Json_Rec_Hs_ListRx_Get_List() { RxIdList = new List<Json_Rx_Property>()};
                    var listRxJson = result.cmd_body.RxIdList;
                    List<RxInfo> listRx = new List<RxInfo>();
                    Event?.Event_GetRxes?.Invoke(result.cmd_end, listRx);
                    foreach (var item in listRx)
                    {
                        Json_Rx_Property rxJson = new Json_Rx_Property()
                        {
                            ID = item.Id,
                            Name = item.Name,  
                            DevType=item.DevType,
                            Ip=item.Ip4,
                            TcpPort=item.Port4.ToString(),
                            WallID = item.WallID.ToString(),
                            Row = item.Row.ToString(),
                            Column = item.Column.ToString(),
                            Online=item.Online,
                            IsBinded = item.IsBinded ? "y" : "n",
                            Version=item.Version
                        };
                        listRxJson.Add(rxJson);
                    }
                }
            }
            catch (Exception ex)
            {
                hsServer.ShowDebug($"{nameof(HsWallListRxGet)} -异常：" + ex.Message);
                result.cmd_end.Err_code = "1";
                result.cmd_end.Err_str = ex.Message;
            }
            finally
            {
                rcv = hsServer.stringify(result);
            }
            return rcv;
        }
        public string HsWallListTxGet(string json)
        {
            string rcv = "";
            Json_Rec_Hs_ListTx_Get result = new Json_Rec_Hs_ListTx_Get() { cmd_end = new Json_Rec_Hs_Cmd_End() { Err_code = "0", Err_str = "" } };
            try
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(Json_Send_Hs_ListTx_Get));
                    Json_Send_Hs_ListTx_Get rcvJs = (Json_Send_Hs_ListTx_Get)deseralizer.ReadObject(ms);// //反序列化ReadObject
                    result.cmd_header = rcvJs.cmd_header;
                    result.cmd_body = new Json_Rec_Hs_ListTx_Get_List() { TxIdList = new List<Json_Tx_Property>() };
                    var body = rcvJs.cmd_body;
                    var listTxJson = result.cmd_body.TxIdList;
                    List<TxInfo> listTx = new List<TxInfo>();
                    Event?.Event_GetTxes?.Invoke(result.cmd_end, listTx);
                    foreach (var item in listTx)
                    {
                        Json_Tx_Property txJson = new Json_Tx_Property()
                        {
                            ID = item.Id,
                            Name = item.Name,
                            DevType=item.DevType,
                            Ip=item.Ip,
                            TcpPort=item.TcpPort.ToString(),
                            Udp_addr = item.Udp_addr,
                            Udp_port=item.Udp_port.ToString(),
                            Ts_addr=item.Ts_addr,
                            Ts_port=item.Ts_port.ToString(),
                            Stream=item.Stream,
                            Online = item.Online ,                           
                            Resolution=item.Resolution,
                            Rate = item.Rate,
                            EncodeFormat =item.EncodeFormat,
                            Version = item.Version
                        };
                        listTxJson.Add(txJson);
                    }
                }
            }
            catch (Exception ex)
            {
                hsServer.ShowDebug($"{nameof(HsWallListTxGet)} -异常：" + ex.Message);
                result.cmd_end.Err_code = "1";
                result.cmd_end.Err_str = ex.Message;
            }
            finally
            {
                rcv = hsServer.stringify(result);
            }
            return rcv;
        }

        public void UpdateRxStateToSynchro(RxInfo rxInfo)
        {
            if (rxInfo==null)
            {
                return;
            }
            RxInfo rx = rxInfo;
            try
            {
                Json_Rx rxJson = new Json_Rx();
                rxJson.cmd_body = new Json_Rx_Property()
                {
                    ID = rx.Id.ToString(),
                    Name = rx.Name,
                    DevType=rx.DevType,
                    Ip = rx.Ip4,
                    TcpPort = rx.Port4.ToString(),
                    WallID = rx.WallID.ToString(),
                    Row = rx.Row.ToString(),
                    Column = rx.Column.ToString(),
                    Online = rx.Online,
                    IsBinded = rx.IsBinded ? "y" : "n",
                    Version = rx.Version
                };
                rxJson.cmd_end = new Json_Rec_Hs_Cmd_End();
                //string rcv = hsServer.stringify(rxJson);
                string rcv = hsServer.stringify(rxJson);
                SynchroSendToJson(rcv);
            }
            catch (Exception ex)
            {
                hsServer.ShowDebug("UpdateRxStateToSynchro-异常："+ex.Message);
            }            
        }
        public void UpdateTxStateToSynchro(TxInfo txInfo)
        {
            if (txInfo == null)
            {
                return;
            }
            TxInfo tx = txInfo;
            try
            {
                Json_Tx txJson = new Json_Tx();
                txJson.cmd_body = new Json_Tx_Property()
                {
                    ID = tx.Id.ToString(),
                    Name = tx.Name,
                    DevType=tx.DevType,
                    Ip = tx.Ip,
                    TcpPort = tx.TcpPort.ToString(),
                    Udp_addr = tx.Udp_addr,
                    Udp_port = tx.Udp_port.ToString(),
                    Ts_addr = tx.Ts_addr,
                    Ts_port = tx.Ts_port.ToString(),
                    Stream = tx.Stream,
                    Online = tx.Online,
                    Resolution=tx.Resolution,
                    Rate=tx.Rate,
                    EncodeFormat=tx.EncodeFormat,
                    Version = tx.Version
                };
                txJson.cmd_end = new Json_Rec_Hs_Cmd_End();
                string rcv = hsServer.stringify(txJson);
                SynchroSendToJson(rcv);
            }
            catch (Exception ex)
            {
                hsServer.ShowDebug("UpdateTxStateToSynchro-异常：" + ex.Message);
            }
        }

        #endregion

        #region 服务器同步处理方法-------------
        void SynchroSendToJson(string json)
        {
            //Event_SynchroServerJson?.Invoke(json);
            Task.Run(() =>
            {
                Event_SynchroServerJson?.Invoke(json);
            });
            
        }

        //void SynchroDeleteWall(string json)
        //{
        //    //string synchroJson = "";
            
        //}
        //void SynchroCreateWall(string json)
        //{
        //    //string synchroJson = "";
        //    Event_SynchroServerJson?.Invoke(json);
        //}
        //void SynchroModifyWallName(string json)
        //{
        //    //string synchroJson = "";
        //    //Event_SynchroServerJson?.BeginInvoke(json, null, null);  //.net core 3.0 平台不支持 
        //    Event_SynchroServerJson?.Invoke(json);
        //}

        //void SynchroWinOpen(string json)
        //{
        //    //string synchroJson = "";
        //    Event_SynchroServerJson?.Invoke(json);
        //}
        //void SynchroWinModify(string json)
        //{
        //    //string synchroJson = "";
        //    Event_SynchroServerJson?.Invoke(json);

        //}
        //void SynchroWinSize(string json)
        //{
        //    //string synchroJson = "";
        //    Event_SynchroServerJson?.Invoke(json);
        //}
        //void SynchroWinClose(string json)
        //{
        //    //string synchroJson = "";
        //    Event_SynchroServerJson?.Invoke(json);
        //}
        //void SynchroWinClear(string json)
        //{
        //    //string synchroJson = "";
        //    Event_SynchroServerJson?.Invoke(json);
        //}
        //void SynchroWinModifyIdTx(string json)
        //{
        //    //string synchroJson = "";
        //    Event_SynchroServerJson?.Invoke(json);
        //}
        //void SynchroWinModifyZIndex(string json)
        //{
        //    //string synchroJson = "";
        //    Event_SynchroServerJson?.Invoke(json);
        //}


        //void SynchroSaveScene(string json)
        //{
        //    //string synchroJson = "";
        //    Event_SynchroServerJson?.Invoke(json);
        //}
        //void SynchroCallScene(string json)
        //{
        //    //string synchroJson = "";
        //    Event_SynchroServerJson?.Invoke(json);
        //}
        //void SynchroDeleteScene(string json)
        //{
        //    //string synchroJson = "";
        //    Event_SynchroServerJson?.Invoke(json);
        //}
        //void SynchroModifySceneName(string json)
        //{
        //    //string synchroJson = "";
        //    Event_SynchroServerJson?.Invoke(json);
        //}


        //void SynchroCurrentScene(string json)
        //{
        //    //string synchroJson = "";
        //    Event_SynchroServerJson?.BeginInvoke(json, null, null);
        //}
        //void SynchroGetScenes(string json)
        //{
        //    //string synchroJson = "";
        //    Event_SynchroServerJson?.BeginInvoke(json, null, null);
        //}

        #endregion

       
    }
}
