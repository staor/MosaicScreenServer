using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace FromTxRxServer
{
    class MosaicScreenToTxRxServer_Json
    {
    }
    class LIJsonFunction
    {
        [DataContract]
        public class SendGetHsTxConfigJson
        {
            [DataMember(Order = 0)]
            public string json_header { get; set; }

            [DataMember(Order = 1)]
            public SendGetHsTxConfigToIdNameJsonFormat format { get; set; }

        }
        [DataContract]
        public class SendGetHsTxConfigToIdNameJsonFormat
        {
            [DataMember] //Order值表示数据排序规则，默认即可
            public string id { get; set; }
            [DataMember]
            public string name { get; set; }

            [DataMember]
            public string online { get; set; }
            [DataMember]
            public string resolution { get; set; }
            [DataMember]
            public string rate { get; set; }
           
            [DataMember]
            public string usb { get; set; }

            [DataMember]
            public string ip { get; set; }
            [DataMember]
            public string port { get; set; }
            [DataMember]
            public string devType { get; set; }
            [DataMember]
            public string version { get; set; }

        }
        [DataContract]
        public class SendGetHsTxConfigJsonFormat
        {
            [DataMember(Order = 0)]
            public string online { get; set; }

            [DataMember(Order = 1)]
            public string port { get; set; }
            [DataMember(Order = 2)]
            public string area { get; set; }
            [DataMember(Order = 3)]
            public string name { get; set; }
            [DataMember(Order = 4)]
            public string id { get; set; }
            [DataMember(Order = 5)]
            public string ip { get; set; }
            [DataMember(Order = 6)]
            public string devType { get; set; }
            [DataMember(Order = 7)]
            public string audio { get; set; }
            [DataMember(Order = 8)]
            public string analogVol { get; set; }
            [DataMember(Order = 9)]
            public string usb { get; set; }
            [DataMember(Order = 10)]
            public string km { get; set; }
            [DataMember(Order = 11)]
            public string ir { get; set; }
            [DataMember(Order = 12)]
            public string serial { get; set; }
            [DataMember(Order = 13)]
            public string hdcp { get; set; }
            [DataMember(Order = 14)]
            public string sendPic { get; set; }
            [DataMember(Order = 15)]
            public string baudRate { get; set; }
            [DataMember(Order = 16)]
            public string resolution { get; set; }
            [DataMember(Order = 17)]
            public string rate { get; set; }
            [DataMember(Order = 18)]
            public string system { get; set; }
            [DataMember(Order = 19)]
            public string encode { get; set; }
            [DataMember(Order = 20)]
            public string fiber { get; set; }
            [DataMember(Order = 21)]
            public string version { get; set; }

        }
        [DataContract]
        public class RecGetHsTxConfigJson
        {
            [DataMember(Order = 0)]
            public string json_header { get; set; }

            [DataMember(Order = 1)]
            public RecGetHsTxConfigJsonFormat format { get; set; }

        }
        [DataContract]
        public class RecGetHsTxConfigJsonFormat
        {
            [DataMember(Order = 0)]
            public RecGetHsTxConfigToIdNameJsonElement[] parameter { get; set; }
        }
        [DataContract]       
        public class RecGetHsTxConfigToIdNameJsonElement
        {
            [DataMember(Order = 0)]
            public string name { get; set; }
            [DataMember(Order = 1)]
            public string id { get; set; }
            [DataMember(Order = 2)]
            public string online { get; set; }
            [DataMember(Order = 3)]
            public string resolution { get; set; }
            [DataMember(Order = 4)]
            public string usb { get; set; }

            [DataMember(Order = 5)]
            public string ip { get; set; }
            [DataMember(Order = 6)]
            public string device_id { get; set; } //旧版本
            [DataMember(Order = 7)]
            public string device_name { get; set; } //旧版本
            [DataMember(Order = 0)]
            public string devType { get; set; }
            [DataMember(Order = 0)]
            public string version { get; set; }
            [DataMember(Order = 0)]
            public string rate { get; set; }  //码率
        }


        
    }
    
}
