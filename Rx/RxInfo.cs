using System;
using System.Collections.Generic;
using System.Text;
using Utils;

namespace RxNS
{
    public class RxInfo
    {
        public static Dictionary<string, RxInfo> DicRxInfo = new Dictionary<string, RxInfo>();
        public Action<RxInfo> ActionOnline { get; set; } 
        public RxInfo(string id, string ip4, int port4)
        {
            if (!DicRxInfo.ContainsKey(id))
            {
                DicRxInfo.Add(id, this);
            }
            Id = id;
            Ip4 = ip4;
            Port4 = port4;
        }
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string DevType { get; set; } = "";
        public int WallID { get; set; } = -1;
        public int Row { get; set; } 
        public int Column { get; set; } 
        public string Ip4 { get; set; }
        public int Port4 { get; set; }

        public bool IsMaster { get; set; }
        public string Version { get; set; }
        public string Online { get; set; } = "n";
        bool isOnline;
        public bool IsOnline
        {
            get => isOnline;
            set
            {
                if (value!=isOnline)
                {
                    isOnline = value;
                    if (isOnline)
                    {
                        ActionOnline?.BeginInvoke(this, null, null); //上线触发
                    }
                }
            }
        } //检查Rx组播中是否在线 临时使用
        public bool IsBinded { get; set; }  //是否绑定到了大屏
        public bool CurrentOnline { get; set; }  //当前是否在线，定时刷新

        public int SumSubWins { get; set; }  //统计当前的有的小格子窗口数量，

        /// <summary>
        /// 相对在物理大屏上的坐标长宽
        /// </summary>
        public Rect RectScreenSub { get; set; }//相对大屏坐标系统中 的像素位置
    }
}
