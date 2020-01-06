using System;
using System.Collections.Generic;
using System.Text;

namespace TxNS

{
    public class TxInfo
    {
        public static Dictionary<string, TxInfo> DicTxInfo = new Dictionary<string, TxInfo>();
        public TxInfo(string id)
        {
            if (!DicTxInfo.ContainsKey(id))
            {
                DicTxInfo.Add(id, this);
            }
            Id = id;
        }
        public TxInfo(){}
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string DevType { get; set; } = "";
        public string Ip { get; set; }
        public string Usb { get; set; }
        public int TcpPort { get; set; }
        public string Udp_addr { get; set; }
        public int Udp_port { get; set; } = 20100;  //默认
        public string Ts_addr { get; set; } 
        public int Ts_port { get; set; }  //= 32008;  //默认
        public string Stream { get; set; }
        public string Version { get; set; }
        public string Online { get; set; }
        public string Resolution { get; set; }  
        public string Rate { get; set; }
        public string EncodeFormat { get; set; } = "H264";

    }
}
