using AbstractInterFace;
using LoggerHelper;
 
using ScreenManagerNS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TxNS;
using Utils;
using Newtonsoft.Json;
using RxNS;

namespace MosaicWinToRx
{
    public class MosaicWinToRx : IMosaicWinToRx
    {         
        public void Load(string mulitIp,int multiPort)
        {
            MosaicWinToRx_Helper.Load(mulitIp, multiPort);
        }
        public void WallClear(int idWall)
        { 
            Json_Send_Wall_Clear js = new Json_Send_Wall_Clear() { body = new Json_Send_Wall_Clear_Body { idWall = idWall.ToString() } };
            
            string jsonString = hsServer.stringify(js);
            MosaicWinToRx_Helper.ServerToRxSendMulticastMsg(jsonString);
        }

        public void WinClose(int idWall,int idWin)
        {
            Json_Send_Win_Close js = new Json_Send_Win_Close() { body = new Json_Send_Win_Close_Body { idWall = idWall.ToString(),idWin=idWin.ToString() } };
            string jsonString = hsServer.stringify(js);
            MosaicWinToRx_Helper.ServerToRxSendMulticastMsg(jsonString);
        }

        public void WinModify(int idWall, List<HsMosaicWinInfo> infos, List<TxInfo> txInfos)
        {
            if (infos==null|| txInfos ==null||infos.Count!=txInfos.Count)
            {
                Logger.CWshow("WinModify-参数不正确！");
                return;
            }
            if (infos.Count==0)
            {
                return;
            }
            List<Json_Send_Win_Modify_Body_Win> wins = new List<Json_Send_Win_Modify_Body_Win>();
            for (int i = 0; i < infos.Count; i++)
            {
                HsMosaicWinInfo item = infos[i];
                TxInfo txInfo = txInfos[i];
                Json_Send_Win_Modify_Body_Win win = new Json_Send_Win_Modify_Body_Win()
                {
                    idWin = item.IdWin.ToString(),
                    x = item.X.ToString(),
                    y = item.Y.ToString(),
                    w = item.Width.ToString(),
                    h = item.Height.ToString(),
                    lay = item.ZIndex.ToString(),
                    txIp=txInfo.Udp_addr,
                    txPort=txInfo.Udp_port.ToString()
                };
                wins.Add(win);
            }
            Json_Send_Win_Modify_Body body = new Json_Send_Win_Modify_Body() { idWall=idWall.ToString(), wins = wins };
            Json_Send_Win_Modify js = new Json_Send_Win_Modify() { body = body };
            string jsonString = hsServer.stringify(js);
            MosaicWinToRx_Helper.ServerToRxSendMulticastMsg(jsonString);
        }
         

        public string ReBindRx(List<RxInfo> oldRxInfo, List<RxInfo> newRxInfo, HsScreenInfo newWall)
        {
            if (newRxInfo==null||oldRxInfo==null || newWall ==null)
            {
                return null;
            }
            string rcv = "";
            StringBuilder sb = new StringBuilder();
            List<RxInfo> toSendRxInfos = newRxInfo.Union(oldRxInfo).ToList();
            List<string> rxes = new List<string>();
            foreach (var item in newRxInfo)
            {
                rxes.Add(item.Id);
            }
            Json_Send_Rx_Bind_Body body = new Json_Send_Rx_Bind_Body()
            {
                idWall = newWall.IdScreen.ToString(),
                rows = newWall.Rows.ToString(),
                columns = newWall.Columns.ToString(),
                pixW = newWall.UnitWidth.ToString(),
                pixH = newWall.UnitHeight.ToString(),
                gapW = newWall.GapWidth.ToString(),
                gapH = newWall.GapHeight.ToString(),
                rxId = rxes
            };            
            Json_Send_Rx_Bind js = new Json_Send_Rx_Bind() { body = body };
            try
            {
                string jsonString = hsServer.stringify(js);
                Logger.CWshow("ReBindRx-发送消息："+jsonString);
                byte[] sendData = Encoding.UTF8.GetBytes(jsonString);
                List<Task> tasks = new List<Task>();
            
                foreach (var item in toSendRxInfos)
                {
                    RxInfo info = item;
                    Task t=Task.Factory.StartNew(() =>
                    {
                       string result= MosaicWinToRx_Helper.SendRxBind(info, sendData);
                        sb.Append(result);
                    });
                    tasks.Add(t);
                }
                bool isOk=Task.WaitAll(tasks.ToArray(),1500);
                rcv = sb.ToString();
                if (!isOk)
                {
                    Logger.CWshow("ReBindRx-连接接收超时1500ms ：" + newWall.Name);
                }
                else
                {
                    Logger.CWshow($"ReBindRx-返回 ： {newWall.IdScreen}-{newWall.Name}  | {rcv}");
                }

            }
            catch (Exception ex)
            {                 
                Logger.CWshow("ReBindRx-返回： 异常：" + ex.Message + "|" + newWall.Name);
            }
            return rcv;
        }

        

        public string UnBindRx(List<RxInfo> oldRxInfo, HsScreenInfo newWall)
        {
            if (oldRxInfo == null || newWall == null||oldRxInfo.Count==0)
            {
                return null;
            }
            string rcv = "";
            StringBuilder sb = new StringBuilder();
            List<RxInfo> toSendRxInfos = oldRxInfo;
            Json_Send_Rx_Bind_Body body = new Json_Send_Rx_Bind_Body()
            {
                idWall = newWall.IdScreen.ToString(),
                rows = newWall.Rows.ToString(),
                columns = newWall.Columns.ToString(),
                pixW = newWall.UnitWidth.ToString(),
                pixH = newWall.UnitHeight.ToString(),
                gapW = newWall.GapWidth.ToString(),
                gapH = newWall.GapHeight.ToString(),
                rxId = new List<string>()
            };
            Json_Send_Rx_Bind js = new Json_Send_Rx_Bind() { body = body };
            try
            {
                string jsonString = hsServer.stringify(js);
                Logger.CWshow("UnBindRx-发送消息：" + jsonString);
                byte[] sendData = Encoding.UTF8.GetBytes(jsonString);
                List<Task> tasks = new List<Task>();

                foreach (var item in toSendRxInfos)
                {
                    RxInfo info = item;
                    Task t = Task.Factory.StartNew(() =>
                    {
                        string result = MosaicWinToRx_Helper.SendRxBind(info, sendData);
                        sb.Append(result);
                    });
                    tasks.Add(t);
                }
                bool isOk = Task.WaitAll(tasks.ToArray(), 1500);
                rcv = sb.ToString();
                if (!isOk)
                {
                    Logger.CWshow("UnBindRx-连接接收超时1500ms ：" + newWall.Name);
                }
                else
                {
                    Logger.CWshow($"UnBindRx-返回 ： {newWall.IdScreen}-{newWall.Name}  | {rcv}");
                }
            }
            catch (Exception ex)
            {
                Logger.CWshow("UnBindRx-返回： 异常：" + ex.Message + "|" + newWall.IdScreen);
            }
            return rcv;
        }

        public string BindRx(List<RxInfo> rxInfos, HsScreenInfo newWall)
        {
            if (rxInfos == null || newWall == null)
            {
                return null;
            }
            string rcv = "";
            StringBuilder sb = new StringBuilder();
            List<RxInfo> toSendRxInfos = rxInfos;
            List<string> rxes = new List<string>();
            foreach (var item in rxInfos)
            {
                rxes.Add(item.Id);
            }
            Json_Send_Rx_Bind_Body body = new Json_Send_Rx_Bind_Body()
            {
                idWall = newWall.IdScreen.ToString(),
                rows = newWall.Rows.ToString(),
                columns = newWall.Columns.ToString(),
                pixW = newWall.UnitWidth.ToString(),
                pixH = newWall.UnitHeight.ToString(),
                gapW = newWall.GapWidth.ToString(),
                gapH = newWall.GapHeight.ToString(),
                rxId = rxes
            };
            Json_Send_Rx_Bind js = new Json_Send_Rx_Bind() { body = body };
            try
            {
                string jsonString = hsServer.stringify(js);
                Logger.CWshow("BindRx-发送消息：" + jsonString);
                byte[] sendData = Encoding.UTF8.GetBytes(jsonString);
                List<Task> tasks = new List<Task>();

                foreach (var item in toSendRxInfos)
                {
                    RxInfo info = item;
                    Task t = Task.Factory.StartNew(() =>
                    {
                        string result = MosaicWinToRx_Helper.SendRxBind(info, sendData);
                        sb.Append(result);
                    });
                    tasks.Add(t);
                }
                bool isOk = Task.WaitAll(tasks.ToArray(), 1500);
                rcv = sb.ToString();
                if (!isOk)
                {
                    Logger.CWshow("BindRx-连接接收超时1500ms ：" + newWall.Name);
                }
                else
                {
                    Logger.CWshow($"BindRx-返回 ： {newWall.IdScreen}-{newWall.Name}  | {rcv}");
                }

            }
            catch (Exception ex)
            {
                Logger.CWshow("BindRx-返回： 异常：" + ex.Message + "|" + newWall.IdScreen+"-"+newWall.Name);
            }
            return rcv;
        }
        public string BindOneRxInfo(List<RxInfo> rxInfos,RxInfo info, HsScreenInfo newWall)
        {
            if (rxInfos == null||info==null || newWall == null)
            {
                return null;
            }
            string rcv = "";
            StringBuilder sb = new StringBuilder();
            List<RxInfo> toSendRxInfos = rxInfos;
            List<string> rxes = new List<string>();
            foreach (var item in rxInfos)
            {
                rxes.Add(item.Id);
            }
            Json_Send_Rx_Bind_Body body = new Json_Send_Rx_Bind_Body()
            {
                idWall = newWall.IdScreen.ToString(),
                rows = newWall.Rows.ToString(),
                columns = newWall.Columns.ToString(),
                pixW = newWall.UnitWidth.ToString(),
                pixH = newWall.UnitHeight.ToString(),
                gapW = newWall.GapWidth.ToString(),
                gapH = newWall.GapHeight.ToString(),
                rxId = rxes
            };
            Json_Send_Rx_Bind js = new Json_Send_Rx_Bind() { body = body };
            try
            {
                string jsonString = hsServer.stringify(js);
                Logger.CWshow("BindOneRxInfo-发送消息："+info.Ip4+":"+info.Port4+"--" + jsonString);
                byte[] sendData = Encoding.UTF8.GetBytes(jsonString);
                List<Task> tasks = new List<Task>();
                                
                 
                Task t = Task.Factory.StartNew(() =>
                {
                    string result = MosaicWinToRx_Helper.SendRxBind(info, sendData);
                    sb.Append(result);
                    //Thread.Sleep(100);  //让先更新绑定后在恢复当前窗口
                });
                tasks.Add(t);
                
                bool isOk = Task.WaitAll(tasks.ToArray(), 1500);
                rcv = sb.ToString();
                if (!isOk)
                {
                    Logger.CWshow("BindOneRxInfo-连接接收超时1500ms ：" + newWall.Name);
                }
                else
                {
                    Logger.CWshow($"BindOneRxInfo-返回 ： {newWall.IdScreen}-{newWall.Name}  | {rcv}");
                }

            }
            catch (Exception ex)
            {
                Logger.CWshow("BindOneRxInfo-返回： 异常：" + ex.Message + "|" + newWall.IdScreen + "-" + newWall.Name);
            }
            return rcv;
        }
    }
}
