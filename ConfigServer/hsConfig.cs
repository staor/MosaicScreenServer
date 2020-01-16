using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigServer
{
    class hsConfig
    {
        public static string serverFixedIp = "169.254.0.253";
        public static string serverIp = "169.254.0.253";
        public static int serverPort = 6000;

        public static string mainTitle = "";

        public static bool isDebug = true;

        public static int packageTs = 500;//图片流的一帧设定的默认数据包的数量

        public static int buffSize = 1024;//连接服务器socket缓冲区接受的大小
        public static int serverReceiveTimeOut = 500;
        public static int serverSendTimeOut = 500;
        public static int serverConnectTimeOut = 500;//连接一个终结点的超时时间

        //public static string PJ_Ip = "192.168.0.221";
        //public static int PJ_Port = 5999;
        //public static int pjRcvOutTime = 200;

        public static int playersCount = 10;//双流Tx的播放器默认数量

        public static bool IsManualSwitch = false; //是否根据手动操作进行信号切换或模式切换，后台若服务器同步也会进行信号切换或模式切换
        public static bool IsPjModeGroupSwitch = false; //切换拼接模式时是否进行拼接模式的组切组切信号源指令发送，默认不是,服务器已内部切换了。

        public static string ipAudio = "192.168.0.220";  //音频管理的ip /port
        public static int portAudio = 6000;

        public static string ipEnvironment = "192.168.0.160";  //环境（继电器控制的灯光）管理的ip /port
        public static int portEnvironment = 4800;

        #region hs拼接参数
        public static string IpHsPj = "169.254.1.252";
        public static int PortHsPj = 5000;
        #endregion

        #region 拼接处理器参数
        public static int ReceiveTimeOutMosaicRoam = 200;  //100ms在操作快了有几率进行接收不到回码，
        public static string strOpenWinLimit = "小提示：小屏中窗口最多叠加4层！";
        public static string strRoamWinLimitSize = "";

        public static int ReceiveTimeOutFromRx4 = 150;  //100ms在操作快了有几率进行接收不到回码，

        public static string IpPj_screen1 = "192.168.0.121";  //默认拼接处理器ip，最新的由xml文件中HsSplitScreen参数获取
        public static int PortPj_screen1 = 5000;


        public static int pjRcvOutTime = 200;
        #endregion

        public static string TipPsw;

        //<!--设置vlc的是否处于一直运行状态,直至软件关闭-->
        public static bool KeepVlcRunning;
        ////是否查询TxRx所有设备的中等数量参数,true为查询返回其他参数devType version
        //public static bool HasTxRxMiddleParameter = false;

        public static List<string> listScreen1x2_Left;
        public static List<string> listScreen1x2_Right;

        #region 对接Rx解码4路盒子的指令 是否显示调试信息
        public static bool IsShowRx4Debug = false;
        /// <summary>
        /// 窗口坐标Rect的磁贴单元屏Rect的比率。  0表示不磁贴  正常取值范围0 或0.1~0.3  其他默认都是0；为解决单元屏Rx开窗小格子的最小宽度为128  最小高度为72 的限制而来
        /// </summary>
        public static double WinRectRate = 0;
        #endregion

        public static int Port_Tcp_MosaicServer = 5000; //本服务器侦听的客户端连接的Tcp端口

        public static string MultiIp_SynchroMosaicServer = "234.234.0.3";
        public static int MultiPort_SynchroMosaicServer = 32108;

        public static string MultiIp_SynchroTxServer = "234.234.1.3";
        public static int MultiPort_SynchroTxServer = 32008;

        public static string MultiIp_SynchroRx = "235.235.100.100";
        public static int MultiPort_SynchroRx = 4000;

        //<!--拼接服务端与Rx端通信方式版本 2：-->
        public static string Version_MosaicServerToRx = "2";
        public static string SendRx_MultiIp = "234.234.0.1";  //V2.0 Rx端接收开窗组播Ip
        public static int SendRx_MultiPort = 32110;           //V2.0 Rx端接收开窗组播Port

        public static int TxOnlineToResolution = 3;  //Tx上线延时查询resolution时间
        public static double MirrorWinRectRate = 0.1;  //镜像屏窗口磁贴，附加若窗口小于最小尺寸，放大

    }
}
