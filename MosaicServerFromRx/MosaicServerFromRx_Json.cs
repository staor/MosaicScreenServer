using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MosaicServerFromRx
{
    class MosaicServerFromRx_Json
    {
        
    }

    [DataContract]
    public class Json_Rec_Device
    {
        [DataMember(Order = 0)]
        public Json_Rec_Device_Body device { get; set; }
    }
    [DataContract]
    public class Json_Rec_Device_Body
    {
        [DataMember(Order = 0)]
        public string id { get; set; }
        [DataMember(Order = 0)]
        public string name { get; set; }
        [DataMember(Order = 0)]
        public string ip { get; set; }
        [DataMember(Order = 0)]
        public string tcp_port { get; set; }
        [DataMember(Order = 0)]
        public string dev_type { get; set; }   //目前枚举表示：0-Tx，1-RX_SLAVE 2-RX_MASTER   整个列表中只需1个主Rx，且掉线后需重新设置另一为主Rx
        [DataMember(Order = 0)]
        public string version { get; set; }
    }

    [DataContract]
    public class Json_Send_Set_RxRole
    {
        [DataMember(Order = 0)]
        public Json_Send_Set_RxRole_Body set_role { get; set; }
    }
    [DataContract]
    public class Json_Send_Set_RxRole_Body
    {
        [DataMember(Order = 0)]
        public string dev_role { get; set; }   //// 设置rx角色  MASTER or SLAVE
    }


    #region 2019.12.06 更新协议：
    [DataContract]
    public class Json_Send_Set_RxRole2
    {
        [DataMember]
        public string head { get; set; } = "rx-set-role";
        [DataMember]
        public Json_Send_Set_RxRole2_Body body { get; set; }
        [DataMember]
        public Json_Rec_Set_RxRole2_End cmd_end { get; set; } = new Json_Rec_Set_RxRole2_End();
    }
    [DataContract]
    public class Json_Send_Set_RxRole2_Body
    {
        [DataMember(Order = 0)]
        public string dev_role { get; set; }   //// 设置rx角色  MASTER or SLAVE
    }

    [DataContract]
    public class Json_Rec_Set_RxRole2_End
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
    #endregion



}
