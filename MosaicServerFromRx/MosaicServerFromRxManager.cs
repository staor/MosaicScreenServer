
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using RxNS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utils;

namespace MosaicServerFromRx
{
    public class MosaicServerFromRxManager
    {
        //MosaicServerFromRx_Event Event;
        //MosaicServerFromRx ServerReceive;
        Utils_UdpMulti UdpMulti = null;
        public string MultiIp_Rx { get; set; } = "235.235.100.100";
        public int MultiPort_Rx { get; set; } = 4000;
        public static readonly int CheckRxInterval = 3200; // 单位为ms   启动后等待Rx同步在线信息完成后 ，再检查有无设置Rx为主 需比3s大一些  默认Rx组播间隔是3s更新一次  
        public Action<RxInfo> Action_UpdateRxInfoOnline { get; set; }  //上线通知，1需更新Online  Name参数  
        public  Action<RxInfo> Action_UpdateRxInfoWin { get; set; }  //上线后 重发当前大屏窗口模式

        //public Action<RxInfo> ActionRxInfoOffline { get; set; }  //离线通知
        public RxInfo RxInfoMaster { get; set; }  //当前是主的RxInfo对象
        public MosaicServerFromRxManager()
        {
            //Event = new MosaicServerFromRx_Event();
            //ServerReceive = new MosaicServerFromRx();
            UdpMulti = new Utils_UdpMulti();
        }
        public bool InitialServerFromRx()
        {
            bool isOk = true;

            //ServerReceive.RcvMulticastServerMsgEvent += ManagerJson;
            //ServerReceive.Initial(MultiIp_Rx,MultiPort_Rx);
            UdpMulti.FuncUdpMultiMsg += ManagerJson;
            UdpMulti.InitialJoinGroup(MultiIp_Rx, MultiPort_Rx);

            InitialCheckRxIsMaster();

            return isOk;
        }
        public bool Load()
        {
            bool isOk = true;

            return isOk;
        }
        public void InitialCheckRxIsMaster()
        {
            Task t = Task.Run(() =>
            {
                //Thread.Sleep(CheckRxInterval/2); //错开一般时间
                Thread.Sleep(CheckRxInterval); //错开一般时间
                //MessageString = "HS-KEY16-setkey-16-onkeydown";
                hsServer.ShowDebug("开始循环检查和设置Rx上下线状态及主从状态。。。。。");
                try
                {
                    while (true)
                    {
                        Thread.Sleep(CheckRxInterval);
                        List<string> listIdRx = new List<string>();
                        listIdRx.AddRange(RxInfo.DicRxInfo.Keys);
                        foreach (var item in listIdRx)
                        {
                            RxInfo rxInfo = RxInfo.DicRxInfo[item];
                            if (!rxInfo.CurrentOnline)  //开始离线
                            {
                                if (rxInfo.IsOnline)
                                {
                                    rxInfo.IsOnline = false;
                                    rxInfo.Online = "n";
                                    if (rxInfo.IsMaster)  //若为主的掉线 需重新在列表中设置一个新主Rx
                                    {
                                        rxInfo.IsMaster = false;
                                        if (RxInfoMaster != null && RxInfoMaster == rxInfo)
                                        {
                                            RxInfoMaster = null;
                                        }
                                        //hsServer.ShowDebug("当前主Rx设备下线："+rxInfo.Id +"需设置一个主Rx");
                                        //for (int i = 0; i < listIdRx.Count; i++)
                                        //{
                                        //    RxInfo tempRx = RxInfo.DicRxInfo[listIdRx[i]];
                                        //    if (tempRx.IsOnline&&tempRx.CurrentOnline)
                                        //    {
                                        //        RxInfoMaster = tempRx;
                                        //        tempRx.IsMaster = true;
                                        //        SetRxRoleAsync(tempRx);
                                        //    }
                                        //}
                                        //if (RxInfoMaster==null)
                                        //{
                                        //    hsServer.ShowDebug("InitialCheckRxIsMaster-当前没有找到可设置为主的RxInfo");
                                        //}
                                    }
                                    Action_UpdateRxInfoOnline?.Invoke(rxInfo);
                                    hsServer.ShowDebug("触发Rx离线通知：" + rxInfo.Id + "-" + (rxInfo.IsMaster ? ":MASTER" : ":SLAVE"));
                                }
                            }
                            else
                            {
                                rxInfo.CurrentOnline = false;
                            }
                        }
                        if (RxInfoMaster == null)
                        {
                            for (int i = 0; i < listIdRx.Count; i++)
                            {
                                RxInfo tempRx = RxInfo.DicRxInfo[listIdRx[i]];
                                if (tempRx.IsOnline)
                                {
                                    RxInfoMaster = tempRx;
                                    tempRx.IsMaster = true;
                                    //SetRxRoleAsync(tempRx); //由实时Rx组播信息触发来执行设主从
                                    break;
                                }
                            }
                            if (RxInfoMaster == null)
                            {
                                hsServer.ShowDebug("InitialCheckRxIsMaster-当前没有找到可设置为主的RxInfo");
                            }
                            else
                            {
                                hsServer.ShowDebug("变动后的当前主Rx的Id:" + RxInfoMaster.Id);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    hsServer.ShowDebug("InitialCheckRxIsMaster-异常：" + ex.Message);
                    //throw;  //不抛出异常 但有在调试记录显示
                    //MessageBox.Show("连接失败：" + ip + ":" + port + "\n" + instruct);
                }
            });
        }


        string ManagerJson(string json)
        {
            string rcv = null;
            JObject obj = null;
            //DataContractJsonSerializer serializer = new DataContractJsonSerializer(;
            try
            {
                if (json.TrimStart(' ').StartsWith("{"))
                {
                    obj = JObject.Parse(json);
                }
                else
                {
                    return rcv;
                }
                //if (json[0]!='{')
                //{

                //}
            }
            catch (Exception)
            {
                //string rcvToClient = "GetJsonDic方法异常：" + ex.Message;
                //hsServer.ShowDebug(rcvToClient);   //因可能有定时2s发送结构体的包，注释是以防一直有这个提示
                return rcv;
            }


            //JToken jToken;
            //obj.TryGetValue("", StringComparison.CurrentCulture, out jToken);
            var header = obj["device"];
            if (header == null)
            {
                hsServer.ShowDebug("当前Json格式中没有：device ");
                return rcv;
            }
            UpdateDevice_Rx(json);
            return rcv;
        }

        public void UpdateDevice_Rx(string json)
        {
            bool isAddRx = false;
            try
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(Json_Rec_Device));
                    Json_Rec_Device rcvJs = (Json_Rec_Device)deseralizer.ReadObject(ms);// //反序列化ReadObject
                    var device = rcvJs.device;

                    RxInfo rxInfo = null;
                    if (RxInfo.DicRxInfo.ContainsKey(device.id))
                    {
                        rxInfo = RxInfo.DicRxInfo[device.id];
                        //rxInfo.Ip4 = device.ip;   
                    }
                    else
                    {
                        rxInfo = new RxInfo(device.id, device.ip, int.Parse(device.tcp_port));
                        isAddRx = true;
                        hsServer.ShowDebug("UpdateDevice_Rx - 新增RxInfo项 Id：" + rxInfo.Id + "-" + rxInfo.Ip4 + ":" + rxInfo.Port4);
                    }
                    rxInfo.Port4 = int.Parse(device.tcp_port);
                    bool isDevTyeChanged = false;
                    if (rxInfo.DevType!=device.dev_type)
                    {
                        rxInfo.DevType = device.dev_type;
                        isDevTyeChanged = true;
                    }
                    if (rxInfo.Name!=device.name)
                    {
                        rxInfo.Name = device.name;
                        isDevTyeChanged = true;
                    }
                    if (rxInfo.Ip4!=device.ip)  //实际1个Ip更新后Id关联变化的
                    {
                        rxInfo.Ip4 = device.ip;
                        isDevTyeChanged = true;
                    }
                    
                    rxInfo.Version = device.version;
                    if (device.dev_type == "RX_SLAVE")  //  /*目前枚举表示：0 - Tx，1 - RX_SLAVE 2 - RX_MASTER*/   RX_SLAVE 与  RX_MASTER整个列表中只需1个主Rx，且掉线后需重新设置另一为主Rx
                    {
                        if (rxInfo.IsMaster)
                        {
                            SetRxRole2Async(rxInfo, true);  //触发由从设为主
                        }
                        else
                        {
                            rxInfo.IsMaster = false;
                            
                        }
                    }
                    else if (device.dev_type == "RX_MASTER")
                    {
                        rxInfo.IsMaster = true;

                        if (RxInfoMaster == null)
                        {
                            RxInfoMaster = rxInfo;
                        }
                        else
                        {
                            if (RxInfoMaster != rxInfo) //多个主Rx，以此最新一个Rx为准
                            {
                                if (!RxInfoMaster.IsMaster) //变成非主le
                                {
                                    RxInfoMaster = rxInfo;
                                }
                                else
                                {
                                    RxInfo oldMaster = RxInfoMaster;
                                    oldMaster.IsMaster = false;
                                    RxInfoMaster = rxInfo;
                                    SetRxRole2Async(oldMaster, false);
                                    
                                    hsServer.ShowDebug("UpdateDevice_Rx-发现原主Master Id:" + RxInfoMaster.Id + "新主Master的Rx.Id：" + rxInfo.Id + " 将原前者者设置为Slave");
                                }
                            }
                            //else
                            //{
                            //    rxInfo.IsMaster = true;
                            //}
                        }

                    }
                    rxInfo.CurrentOnline = true;
                    bool oldIsOnline = rxInfo.IsOnline;
                    rxInfo.IsOnline = true;
                    rxInfo.Online = "y";
                    if (isAddRx || !oldIsOnline)  //新加 或 再次上线，发出通知
                    {
                        isDevTyeChanged = false;
                        
                        Action_UpdateRxInfoOnline?.Invoke(rxInfo);
                        Action_UpdateRxInfoWin?.Invoke(rxInfo);

                        hsServer.ShowDebug("触发Rx上线通知：" + rxInfo.Id + "-" + (rxInfo.IsMaster ? ":MASTER" : ":SLAVE"));
                    }
                    if (isDevTyeChanged)
                    {
                        Action_UpdateRxInfoOnline?.Invoke(rxInfo); //触发主从更改通知
                    }
                }
            }
            catch (Exception ex)
            {
                hsServer.ShowDebug("UpdateDevice_Rx-异常：" + ex.Message);
            }

        }

        public async Task<bool> SetRxRoleAsync(RxInfo rxInfo, bool isMaster)
        {
            bool isOk = true;
            Json_Send_Set_RxRole_Body body = null;
            if (isMaster)
            {
                body = new Json_Send_Set_RxRole_Body { dev_role = "MASTER" }; // 设置rx角色  MASTER or SLAVE
                hsServer.ShowDebug("SetRxRoleAsync-设置指令Rx的 为主Master- Id：" + rxInfo.Id);
            }
            else
            {
                body = new Json_Send_Set_RxRole_Body { dev_role = "SLAVE" };
                hsServer.ShowDebug("SetRxRoleAsync-设置指令Rx的 为从Slave- Id：" + rxInfo.Id);
            }
            Json_Send_Set_RxRole js = new Json_Send_Set_RxRole() { set_role = body };
            var jsonString = hsServer.stringify(js);
            try
            {
                string temp = await hsServer.Call_TCP_string_Async(rxInfo.Ip4, rxInfo.Port4, 0, jsonString, Encoding.UTF8);
                if (temp == "")
                {
                    hsServer.ShowDebug("返回结果： SetRxRoleAsync-字符串为空字符串（可能为接收异常）！" + "\r\n   ------------" + rxInfo.Ip4 + ":" + rxInfo.Port4 + jsonString);
                    isOk = false;
                    return isOk;
                }
            }
            catch (Exception ex)
            {
                isOk = false;
                hsServer.ShowDebug("SetRxRoleAsync-异常：" + ex.Message + "\r\n   ------------" + rxInfo.Ip4 + ":" + rxInfo.Port4 + jsonString);
            }
            return isOk;
        }
        public async Task<bool> SetRxRole2Async(RxInfo rxInfo, bool isMaster)
        {
            bool isOk = true;
            Json_Send_Set_RxRole2_Body body = null;
            if (isMaster)
            {
                body = new Json_Send_Set_RxRole2_Body { dev_role = "MASTER" }; // 设置rx角色  MASTER or SLAVE
                hsServer.ShowDebug("SetRxRole2Async-设置指令Rx的 为主Master- Id：" + rxInfo.Id);
            }
            else
            {
                body = new Json_Send_Set_RxRole2_Body { dev_role = "SLAVE" };
                hsServer.ShowDebug("SetRxRole2Async-设置指令Rx的 为从Slave- Id：" + rxInfo.Id);
            }
            Json_Send_Set_RxRole2 js = new Json_Send_Set_RxRole2() {  body = body };
            var jsonString = hsServer.stringify(js);
            try
            {
                string temp = await hsServer.Call_TCP_string_Async(rxInfo.Ip4, rxInfo.Port4, 0, jsonString, Encoding.UTF8);
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(temp)))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(Json_Send_Set_RxRole2));
                    Json_Send_Set_RxRole2 rcvJs = (Json_Send_Set_RxRole2)deseralizer.ReadObject(ms);// //反序列化ReadObject

                    if (rcvJs.cmd_end.Err_code == "0")
                    {
                        isOk = true;
                    }
                    else
                    {
                        isOk = false;
                        hsServer.ShowDebug("返回结果： SetRxRole2Async-不成功：" + rcvJs.cmd_end.Err_str);
                    }
                }
                //if (temp == "")
                //{
                //    hsServer.ShowDebug("返回结果： SetRxRole2Async-字符串为空字符串（可能为接收异常）！" + "\r\n   ------------" + rxInfo.Ip4 + ":" + rxInfo.Port4 + jsonString);
                //    isOk = false;
                //    return isOk;
                //}
            }
            catch (Exception ex)
            {
                isOk = false;
                hsServer.ShowDebug("SetRxRole2Async-异常：" + ex.Message + "\r\n   ------------" + rxInfo.Ip4 + ":" + rxInfo.Port4 + jsonString);
            }
            return isOk;
        }

    }
}
