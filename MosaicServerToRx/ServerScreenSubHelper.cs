
using ConfigServer;
using LoggerHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Utils;

namespace MosaicServerToRx
{
    
    class ServerScreenSubHelper
    {
        public static bool IsPrevent { get; set; } 

        public static async Task<bool> HS_OpenOrClose_PlayWin(GridSub grid, string ip, int port)
        {
            if (IsPrevent)
            {
                return true;
            }
            bool isOk = true;
            HsOpenRoamWin_tx_single_win win = new HsOpenRoamWin_tx_single_win();
            HsOpenRoamWin_Udp udp = new HsOpenRoamWin_Udp() { udp_addr = grid.CurrentTx.Udp_addr, udp_port = grid.CurrentTx.Udp_port.ToString() };
            HsOpenRoamWin_tx_single single = null;
            if (grid.IsEnable)  //true为开窗
            {
                win.dev_id = grid.Dev_id.ToString();
                win.win_x = grid.RectWin.X.ToString();
                win.win_y = grid.RectWin.Y.ToString();
                win.win_w = grid.RectWin.Width.ToString();
                win.win_h = grid.RectWin.Height.ToString();
                win.crop_x = grid.RectCrop.X.ToString();
                win.crop_y = grid.RectCrop.Y.ToString();
                win.crop_w = grid.RectCrop.Width.ToString();
                win.crop_h = grid.RectCrop.Height.ToString();

                single = new HsOpenRoamWin_tx_single() { tx = grid.tx, operation = "1", priority = grid.ZIndex.ToString(), udp = udp, win = win };
            }
            else  //false为关窗
            {
                win.dev_id = grid.Dev_id.ToString();
                win.win_x = grid.RectWin.X.ToString();
                win.win_y = grid.RectWin.Y.ToString();
                win.win_w = grid.RectWin.Width.ToString();
                win.win_h = grid.RectWin.Height.ToString();
                win.crop_x = grid.RectCrop.X.ToString();
                win.crop_y = grid.RectCrop.Y.ToString();
                win.crop_w = grid.RectCrop.Width.ToString();
                win.crop_h = grid.RectCrop.Height.ToString();

                single = new HsOpenRoamWin_tx_single() { tx = grid.tx, operation = "0", priority = grid.ZIndex.ToString(), udp = udp, win = win };
            }

            HsOpenRoamWin js = new HsOpenRoamWin() { tx_single = single };

            //DataContractJsonSerializer djs = new DataContractJsonSerializer(typeof(HsOpenRoamWin));
            //MemoryStream msObj = new MemoryStream();
            //djs.WriteObject(msObj, js);
            //msObj.Position = 0;
            //StreamReader sr = new StreamReader(msObj, Encoding.UTF8);
            //string json = sr.ReadToEnd();
            //sr.Close();
            //msObj.Close();
            var jsonString = GetStringFromJson(js, Encoding.UTF8);
            try
            {
                string temp = await hsServer.Call_TCP_string_Async(ip, port, 0, jsonString, Encoding.UTF8);
                if (temp == "")
                {
                    Logger.CWshow("返回结果： HS_OpenOrClose_PlayWin字符串为空字符串（可能为接收异常）！" + "\r\n   ------------" + ip + ":" + port + jsonString);
                    isOk = false;
                    return isOk;
                }
                if (hsConfig.IsShowRx4Debug)
                {
                    Logger.CWshow("HS_OpenOrClose_PlayWin-Info："+ ip + ":" + port + jsonString + "\r\n--返回："+ temp);
                }
                Logger.CWshow("");
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(temp)))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(HsCloseAllRoamWin_Operate_Return_Json));
                    HsCloseAllRoamWin_Operate_Return_Json rcv = (HsCloseAllRoamWin_Operate_Return_Json)deseralizer.ReadObject(ms);
                    if (rcv.husion_3536vdecs_demo_cmd_return.result.code != 0)
                    {
                        isOk = false;
                    }
                };
            }
            catch (Exception ex)
            {
                isOk = false;
                Logger.CWshow("HS_OpenOrClose_PlayWin异常：" + ex.Message + "\r\n   ------------" + ip + ":" + port + jsonString);
            }
            return isOk;
        }
        public static async Task<bool> HS_ModifyTx_PlayWin(GridSub grid, string ip, int port)
        {
            if (IsPrevent)
            {
                return true;
            }
            bool isOk = true;
            HsOpenRoamWin_tx_single_win win = new HsOpenRoamWin_tx_single_win();
            win.dev_id = grid.Dev_id.ToString();
            win.win_x = grid.RectWin.X.ToString();
            win.win_y = grid.RectWin.Y.ToString();
            win.win_w = grid.RectWin.Width.ToString();
            win.win_h = grid.RectWin.Height.ToString();
            win.crop_x = grid.RectCrop.X.ToString();
            win.crop_y = grid.RectCrop.Y.ToString();
            win.crop_w = grid.RectCrop.Width.ToString();
            win.crop_h = grid.RectCrop.Height.ToString();

            HsOpenRoamWin_Udp udp = new HsOpenRoamWin_Udp() { udp_addr = grid.CurrentTx.Udp_addr, udp_port = grid.CurrentTx.Udp_port.ToString() };
            HsOpenRoamWin_tx_single single = new HsOpenRoamWin_tx_single() { tx = grid.tx, operation = "1", priority = grid.ZIndex.ToString(), udp = udp, win = win };
            HsOpenRoamWin js = new HsOpenRoamWin() { tx_single = single };

            //DataContractJsonSerializer djs = new DataContractJsonSerializer(typeof(HsOpenRoamWin));
            //MemoryStream msObj = new MemoryStream();
            //djs.WriteObject(msObj, js);
            //msObj.Position = 0;
            //StreamReader sr = new StreamReader(msObj, Encoding.UTF8);
            //string json = sr.ReadToEnd();
            //sr.Close();
            //msObj.Close();
            var jsonString = GetStringFromJson(js, Encoding.UTF8);
            try
            {
                string temp = await hsServer.Call_TCP_string_Async(ip, port, 0, jsonString, Encoding.UTF8);
                if (temp == "")
                {
                    Logger.CWshow("返回结果： HS_OpenOrClose_PlayWin字符串为空字符串（可能为接收异常）！" + "\r\n   ------------" + ip + ":" + port + jsonString);
                    isOk = false;
                }
                if (hsConfig.IsShowRx4Debug)
                {
                    Logger.CWshow("HS_ModifyTx_PlayWin-Info：" + ip + ":" + port + jsonString + "\r\n--返回：" + temp);
                }
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(temp)))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(HsOpenRoamWin_Rcv));
                    HsOpenRoamWin_Rcv rcv = (HsOpenRoamWin_Rcv)deseralizer.ReadObject(ms);
                    if (rcv.rx.err_code != 0)
                    {
                        isOk = false;
                        return isOk;
                    }
                };
            }
            catch (Exception ex)
            {
                isOk = false;
                Logger.CWshow("HS_OpenOrClose_PlayWin异常：" + ex.Message + "\r\n   ------------" + ip + ":" + port + jsonString);
            }
            return isOk;
        }
        public static async Task<bool> HS_ModifyZIndex_PlayWin(GridSub grid, string ip, int port)
        {
            if (IsPrevent)
            {
                return true;
            }
            bool isOk = true;
            HsOpenRoamWin_tx_single_win win = new HsOpenRoamWin_tx_single_win();
            win.dev_id = grid.Dev_id.ToString();
            win.win_x = grid.RectWin.X.ToString();
            win.win_y = grid.RectWin.Y.ToString();
            win.win_w = grid.RectWin.Width.ToString();
            win.win_h = grid.RectWin.Height.ToString();
            win.crop_x = grid.RectCrop.X.ToString();
            win.crop_y = grid.RectCrop.Y.ToString();
            win.crop_w = grid.RectCrop.Width.ToString();
            win.crop_h = grid.RectCrop.Height.ToString();

            HsOpenRoamWin_Udp udp = new HsOpenRoamWin_Udp() { udp_addr = grid.CurrentTx.Udp_addr, udp_port = grid.CurrentTx.Udp_port.ToString() };
            HsOpenRoamWin_tx_single single = new HsOpenRoamWin_tx_single() { tx = grid.tx, operation = "1", priority = grid.ZIndex.ToString(), udp = udp, win = win };
            HsOpenRoamWin js = new HsOpenRoamWin() { tx_single = single };

            //DataContractJsonSerializer djs = new DataContractJsonSerializer(typeof(HsOpenRoamWin));
            //MemoryStream msObj = new MemoryStream();
            //djs.WriteObject(msObj, js);
            //msObj.Position = 0;
            //StreamReader sr = new StreamReader(msObj, Encoding.UTF8);
            //string json = sr.ReadToEnd();
            //sr.Close();
            //msObj.Close();
            var jsonString = GetStringFromJson(js, Encoding.UTF8);
            try
            {
                string temp = await hsServer.Call_TCP_string_Async(ip, port, 0, jsonString, Encoding.UTF8);
                if (temp == "")
                {
                    Logger.CWshow("返回结果： HS_OpenOrClose_PlayWin字符串为空字符串（可能为接收异常）！" + "\r\n   ------------" + ip + ":" + port + jsonString);
                    isOk = false;
                    return isOk;
                }
                if (hsConfig.IsShowRx4Debug)
                {
                    Logger.CWshow("HS_ModifyZIndex_PlayWin-Info：" + ip + ":" + port + jsonString + "\r\n--返回：" + temp);
                }
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(temp)))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(HsOpenRoamWin_Rcv));
                    HsOpenRoamWin_Rcv rcv = (HsOpenRoamWin_Rcv)deseralizer.ReadObject(ms);
                    if (rcv.rx.err_code != 0)
                    {
                        isOk = false;
                    }
                };
            }
            catch (Exception ex)
            {
                isOk = false;
                Logger.CWshow("HS_OpenOrClose_PlayWin异常：" + ex.Message + "\r\n   ------------" + ip + ":" + port + jsonString);
            }
            return isOk;
        }
        public static async Task<bool> HS_CloseAllPlayWin(string ip, int port)
        {
            if (IsPrevent)
            {
                return true;
            }
            bool isOk = true;
            HsCloseAllRoamWin_Operate o = new HsCloseAllRoamWin_Operate { clear = "1" };
            HsCloseAllRoamWin js = new HsCloseAllRoamWin() { operate = o };
            var jsonString = GetStringFromJson(js, Encoding.UTF8);
            try
            {
                string temp = await hsServer.Call_TCP_string_Async(ip, port, 0, jsonString, Encoding.UTF8);
                if (temp == "")
                {
                    Logger.CWshow("返回结果： HS_OpenOrClose_PlayWin字符串为空字符串（可能为接收异常）！" + "\r\n   ------------" + ip + ":" + port + jsonString);
                    isOk = false;
                    return isOk;
                }
                if (hsConfig.IsShowRx4Debug)
                {
                    Logger.CWshow("HS_CloseAllPlayWin-Info：" + ip + ":" + port + jsonString + "\r\n--返回：" + temp);
                }
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(temp)))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(HsCloseAllRoamWin_Operate_Return_Json));
                    HsCloseAllRoamWin_Operate_Return_Json rcv = (HsCloseAllRoamWin_Operate_Return_Json)deseralizer.ReadObject(ms);
                    if (rcv.husion_3536vdecs_demo_cmd_return.result.code != 0)
                    {
                        isOk = false;
                    }
                };
            }
            catch (Exception ex)
            {
                isOk = false;
                Logger.CWshow("HS_CloseAllPlayWin异常：" + ex.Message + "\r\n   ------------" + ip + ":" + port + jsonString);
            }
            return isOk;
        }



        public static string GetStringFromJson(object jsonObject, Encoding encoding)
        {
            using (var ms = new MemoryStream())
            {
                new DataContractJsonSerializer(jsonObject.GetType()).WriteObject(ms, jsonObject);
                return encoding.GetString(ms.ToArray());
            }
        }





        #region Json类----------------
        /*---------------------2019.4.12 临时北京800盒子开窗漫游Demo

            RX设备控制指令json格式说明
            如 ip地址：192.168.1.200  接收指令的tcp端口 22224  tcp使用短连接

            {
                "operate":{
                    "clear":"1"     // 清空该rx屏幕
                }
            }

            
        {
            "tx_single": {
                "tx": "tx1",
                "operation":"0",        // 0-移除 1-其它
                "priority": "", // 叠加优先级0-3 值越大 优先级越高
                "win": {
                    "dev_id": "", // 设备id 这里默认都是0
                    "win_x": "", // 窗口放置在屏幕中的位置
                    "win_y": "",
                    "win_w": "",
                    "win_h": "",
                    "crop_x": "", // 窗口内 裁剪位置 
                    "crop_y": "",
                    "crop_w": "", // 裁剪窗口的大小
                    "crop_h": ""                
                }
            }
        }
        2019.9.× 统一返回：
        {	
	        "husion_3536vdecs_demo_cmd_return":	
	        {		
		        "result":	
		        {			
			        "err_str":	"",			
			        "code":	0		
		        }	
	        }
        }
        ---------------------*/
        #region 清空Rx4所有窗口------------
        [DataContract]
        public class HsCloseAllRoamWin
        {
            [DataMember]
            public HsCloseAllRoamWin_Operate operate { get; set; }
        }
        [DataContract]
        public class HsCloseAllRoamWin_Operate
        {
            [DataMember]
            public string clear { get; set; }
        }
        //返回--------------
        [DataContract]
        public class HsCloseAllRoamWin_Operate_Return_Json
        {
            [DataMember]
            public HsCloseAllRoamWin_Operate_Return husion_3536vdecs_demo_cmd_return { get; set; }
        }
        [DataContract]
        public class HsCloseAllRoamWin_Operate_Return
        {
            [DataMember]
            public HsCloseAllRoamWin_Operate_Result result { get; set; }
        }
        [DataContract]
        public class HsCloseAllRoamWin_Operate_Result
        {
            [DataMember]
            public string err_str { get; set; }
            [DataMember]
            public int code { get; set; }
        }
        #endregion

        [DataContract]
        public class HsOpenRoamWin
        {
            [DataMember]
            public HsOpenRoamWin_tx_single tx_single { get; set; }
        }
        [DataContract]
        public class HsOpenRoamWin_tx_single
        {
            [DataMember]
            public string tx { get; set; }
            [DataMember]
            public string operation { get; set; }
            [DataMember]
            public HsOpenRoamWin_Udp udp { get; set; }
            [DataMember]
            public string priority { get; set; }
            [DataMember]
            public HsOpenRoamWin_tx_single_win win { get; set; }
        }
        [DataContract]
        public class HsOpenRoamWin_Udp
        {
            [DataMember]
            public string udp_addr { get; set; }
            [DataMember]
            public string udp_port { get; set; }
        }
        [DataContract]
        public class HsOpenRoamWin_tx_single_win
        {
            [DataMember]
            public string dev_id { get; set; }
            [DataMember]
            public string win_x { get; set; }
            [DataMember]
            public string win_y { get; set; }
            [DataMember]
            public string win_w { get; set; }
            [DataMember]
            public string win_h { get; set; }
            [DataMember]
            public string crop_x { get; set; }
            [DataMember]
            public string crop_y { get; set; }
            [DataMember]
            public string crop_w { get; set; }
            [DataMember]
            public string crop_h { get; set; }

        }
        [DataContract]
        public class HsOpenRoamWin_Rcv
        {
            [DataMember]
            public HsOpenRoamWin_Rcv_rx rx { get; set; }
        }
        [DataContract]
        public class HsOpenRoamWin_Rcv_rx
        {
            [DataMember]
            public int err_code { get; set; }
            [DataMember]
            public string tokens { get; set; }
        }
        #endregion
    }
}
