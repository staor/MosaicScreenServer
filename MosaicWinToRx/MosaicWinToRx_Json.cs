using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MosaicWinToRx
{
   
    [DataContract]
    public class Json_Rec_Hs_Rx_End
    {
        [DataMember]
        public string Operator { get; set; } = "MosaicServer";
        [DataMember]
        public long Token
        {
            get
            {
                return DateTime.Now.Ticks;
            }
            set { }
        }
        [DataMember]
        public string Err_code { get; set; } = "0"; // 0 表示成功 非0表示错误码
        [DataMember]
        public string Err_str { get; set; } // 成功为空  错误为错误
    }

    #region 大屏绑定--------    
    [DataContract]
    public class Json_Send_Rx_Bind
    {
        [DataMember(Order = 0)]
        public string head { get; set; } = "rx-bind";
        [DataMember(Order = 0)]
        public Json_Send_Rx_Bind_Body body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Rx_End end { get; set; } = new Json_Rec_Hs_Rx_End();
    }
    [DataContract]
    public class Json_Send_Rx_Bind_Body
    {
        [DataMember]
        public string idWall { get; set; }
        [DataMember]
        public string rows { get; set; }
        [DataMember]
        public string columns { get; set; }
        [DataMember]
        public string pixW { get; set; }
        [DataMember]
        public string pixH { get; set; }
        [DataMember]
        public string gapW { get; set; }
        [DataMember]
        public string gapH { get; set; }
        [DataMember]
        public List<string> rxId { get; set; }
    }
    #endregion
    #region 大屏操作--------    
    //大屏开 移动 切换 图层
    [DataContract]
    public class Json_Send_Win_Modify
    {
        [DataMember(Order = 0)]
        public string head { get; set; } = "win-modify";
        [DataMember(Order = 0)]
        public Json_Send_Win_Modify_Body body { get; set; }
        //[DataMember(Order = 0)]
        //public Json_Rec_Hs_Cmd_End cmd_end { get; set; } = new Json_Rec_Hs_Cmd_End();
    }
    [DataContract]
    public class Json_Send_Win_Modify_Body
    {
        [DataMember]
        public string idWall { get; set; }
        [DataMember]
        public List<Json_Send_Win_Modify_Body_Win> wins { get; set; }
    }
    [DataContract]
    public class Json_Send_Win_Modify_Body_Win
    {
        [DataMember]
        public string idWin { get; set; }
        [DataMember]
        public string x { get; set; }
        [DataMember]
        public string y { get; set; }
        [DataMember]
        public string w { get; set; }
        [DataMember]
        public string h { get; set; }
        [DataMember]
        public string txIp { get; set; }
        [DataMember]
        public string txPort { get; set; }
        [DataMember]
        public string lay { get; set; }
    }
    //大屏关窗
    [DataContract]
    public class Json_Send_Win_Close
    {
        [DataMember(Order = 0)]
        public string head { get; set; } = "win-close";
        [DataMember(Order = 0)]
        public Json_Send_Win_Close_Body body { get; set; }
        //[DataMember(Order = 0)]
        //public Json_Rec_Hs_Cmd_End cmd_end { get; set; } = new Json_Rec_Hs_Cmd_End();
    }
    [DataContract]
    public class Json_Send_Win_Close_Body
    {
        [DataMember]
        public string idWall { get; set; }
        [DataMember]
        public string idWin { get; set; }
    }
    //大屏清空
    [DataContract]
    public class Json_Send_Wall_Clear
    {
        [DataMember(Order = 0)]
        public string head { get; set; } = "wall-clear";
        [DataMember(Order = 0)]
        public Json_Send_Wall_Clear_Body body { get; set; }
        //[DataMember(Order = 0)]
        //public Json_Rec_Hs_Cmd_End cmd_end { get; set; } = new Json_Rec_Hs_Cmd_End();
    }
    [DataContract]
    public class Json_Send_Wall_Clear_Body
    {
        [DataMember]
        public string idWall { get; set; } 
    }
    #endregion
}
