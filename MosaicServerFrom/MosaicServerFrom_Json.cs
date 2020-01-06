using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MosaicServerFrom
{
    class MosaicServerFrom_Json
    {
    }

    #region Hs-拼接漫游协议的Json格式---------------
    [DataContract]
    public class Json_Rec_Hs_Cmd_End
    {
        [DataMember(Order = 0)]
        public string Operator { get; set; } = "pc-role";
        [DataMember(Order = 0)]
        public long Token
        {
            get
            {
                return DateTime.Now.Ticks;
            }
            set { }
        }
        [DataMember(Order = 0)]
        public string Err_code { get; set; } = "0"; // 0 表示成功 非0表示错误码
        [DataMember(Order = 0)]
        public string Err_str { get; set; } // 成功为空  错误为错误
    }
    //[DataContract]
    //public class Json_Rec_Hs_Cmd_End
    //{
    //    [DataMember(Order = 0)]
    //    public string Operator { get; set; }  // 操作者	iphone-role/web-role	....
    //    [DataMember(Order = 0)]
    //    public long Token { get; set; } // 会话		当前系统时间ms
    //    [DataMember(Order = 0)]
    //    public string Err_code { get; set; } = "0"; // 0 表示成功 非0表示错误码
    //    [DataMember(Order = 0)]
    //    public string Err_str { get; set; } // 成功为空  错误为错误
    //}

    //返回空的Json结果  客户端接收服务器发送的Json
    [DataContract]
    public class Json_Rec_Hs_Result
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; }
        [DataMember(Order = 0)]
        public string cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; }
    }

    //服务器发送给客户端的返回空Body结果的Json
    //[DataContract]
    //public class Json_Rec_Hs_Result
    //{
    //    [DataMember(Order = 0)]
    //    public string cmd_header { get; set; }
    //    [DataMember(Order = 0)]
    //    public string cmd_body { get; set; }
    //    [DataMember(Order = 0)]
    //    public Json_Rec_Hs_Cmd_End cmd_end { get; set; }
    //}

    #region 大屏相关------------------
    //创建-----------
    [DataContract]
    public class Json_Send_Hs_Wall_Create
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; } = "hscmd-video-wall-create";
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_Create_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; } = new Json_Rec_Hs_Cmd_End();
    }
    [DataContract]
    public class Json_Send_Hs_Wall_Create_Body
    {
        [DataMember(Order = 0)]
        public string WallID { get; set; }
        [DataMember(Order = 0)]
        public string WallName { get; set; }
        [DataMember(Order = 0)]
        public string Row { get; set; }
        [DataMember(Order = 0)]
        public string Column { get; set; }
        [DataMember(Order = 0)]
        public string StartX { get; set; }
        [DataMember(Order = 0)]
        public string StartY { get; set; }
        [DataMember(Order = 0)]
        public string WallPixW { get; set; }
        [DataMember(Order = 0)]
        public string WallPixH { get; set; }
        //[DataMember(Order = 0)]  
        //public string VSyncMode { get; set; } = "inside"; //"inside",		// outside 同步模式  V2.0协议版本取消
        [DataMember(Order = 0)]
        public List<string> RxIDList { get; set; }
        [DataMember(Order = 0)]
        public string IsMirror { get; set; }  //null或“”表示为不是镜像屏 而是主屏   "y" 表示为镜像屏 
        [DataMember(Order = 0)]
        public string BindMasterId { get; set; }  //默认为0 表示没有绑定到主屏id

        [DataMember(Order = 0)]
        public string UnitW { get; set; } = "1920";
        [DataMember(Order = 0)]
        public string UnitH { get; set; } = "1080";
        [DataMember(Order = 0)]
        public string GapW { get; set; } = "0";
        [DataMember(Order = 0)]
        public string GapH { get; set; } = "0";
    }
    [DataContract]
    public class Json_Rec_Hs_Wall_Create
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Wall_Create_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; }
    }
    [DataContract]
    public class Json_Rec_Hs_Wall_Create_Body
    {
        [DataMember(Order = 0)]
        public string WallName { get; set; }
        [DataMember(Order = 0)]
        public string WallID { get; set; }
    }
    //查询大屏参数
    [DataContract]
    public class Json_Send_Hs_Wall_Get
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; } = "hscmd-video-wall-get";
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_Get_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; } = new Json_Rec_Hs_Cmd_End();
    }
    [DataContract]
    public class Json_Send_Hs_Wall_Get_Body
    {
        [DataMember(Order = 0)]
        public List<string> Walls { get; set; }
    }
    [DataContract]
    public class Json_Rec_Hs_Wall_Get
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Wall_Get_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; }
    }
    [DataContract]
    public class Json_Rec_Hs_Wall_Get_Body
    {
        [DataMember(Order = 0)]
        public List<Json_Send_Hs_Wall_Create_Body> Walls { get; set; }
    }
    [DataContract]
    public class Json_Rec_Hs_Wall_Get_Format_Wall  //V2.0取消，改为Json_Send_Hs_Wall_Create_Body
    {
        [DataMember(Order = 0)]
        public string WallName { get; set; }
        [DataMember(Order = 0)]
        public string WallID { get; set; }
        [DataMember(Order = 0)]
        public string WallRow { get; set; }
        [DataMember(Order = 0)]
        public string WallCol { get; set; }
        [DataMember(Order = 0)]
        public string WallStartX { get; set; }
        [DataMember(Order = 0)]
        public string WallStartY { get; set; }
        [DataMember(Order = 0)]
        public string WallWidth { get; set; }
        [DataMember(Order = 0)]
        public string WallHeight { get; set; }
        //[DataMember(Order = 0)]
        //public string VSyncMode { get; set; } = "inside"; //"inside",		// outside 同步模式 
        [DataMember(Order = 0)]
        public List<string> RxID { get; set; }
        [DataMember(Order = 0)]
        public string IsMirror { get; set; }
        [DataMember(Order = 0)]
        public string BindMasterId { get; set; }
    }
    //删除大屏-----------
    [DataContract]
    public class Json_Send_Hs_Wall_Delete
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; } = "hscmd-video-wall-delete";
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_Delete_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; } = new Json_Rec_Hs_Cmd_End();
    }
    [DataContract]
    public class Json_Send_Hs_Wall_Delete_Body
    {
        [DataMember(Order = 0)]
        public string WallID { get; set; }
    }
    [DataContract]
    public class Json_Rec_Hs_Wall_Delete
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; }
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_Delete_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; } = new Json_Rec_Hs_Cmd_End();
    }
    //修改大屏名称-----------
    [DataContract]
    public class Json_Send_Hs_Wall_Modify_Name
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; } = "hscmd-video-wall-modify";
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_Modify_Name_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; } = new Json_Rec_Hs_Cmd_End();
    }
    [DataContract]
    public class Json_Send_Hs_Wall_Modify_Name_Body
    {
        [DataMember(Order = 0)]
        public string WallName { get; set; }
        [DataMember(Order = 0)]
        public string WallID { get; set; }
    }
    [DataContract]
    public class Json_Rec_Hs_Wall_Modify_Name
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; }
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_Modify_Name_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; } = new Json_Rec_Hs_Cmd_End();
    }

    //大屏镜像绑定---------------
    [DataContract]
    public class Json_Send_Hs_Wall_Mirror_Bind
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; } = "hscmd-video-wall-mirror-bind";
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_Mirror_Bind_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; } = new Json_Rec_Hs_Cmd_End();
    }
    [DataContract]
    public class Json_Send_Hs_Wall_Mirror_Bind_Body
    {
        [DataMember(Order = 0)]
        public string MirrorID { get; set; }
        [DataMember(Order = 0)]
        public string BindMasterId { get; set; }
    }
    //大屏Rx绑定-------------
    [DataContract]
    public class Json_Send_Hs_Wall_Rxes_Bind
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; } = "hscmd-video-wall-rxes-bind";
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_Rxes_Bind_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; } = new Json_Rec_Hs_Cmd_End();
    }
    [DataContract]
    public class Json_Send_Hs_Wall_Rxes_Bind_Body
    {
        [DataMember(Order = 0)]
        public string WallID { get; set; }
        [DataMember(Order = 0)]
        public List<string> RxIDList { get; set; }
    }

    //大屏裁剪---------------
    [DataContract]
    public class Json_Send_Hs_Wall_Clip_Set
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; } = "hscmd-video-wall-clip-set";
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_Clip_Set_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; } = new Json_Rec_Hs_Cmd_End();
    }
    [DataContract]
    public class Json_Send_Hs_Wall_Clip_Set_Body
    {
        [DataMember(Order = 0)]
        public string WallID { get; set; }
        [DataMember(Order = 0)]
        public string StartX { get; set; }
        [DataMember(Order = 0)]
        public string StartY { get; set; }
        [DataMember(Order = 0)]
        public string WallPixW { get; set; }
        [DataMember(Order = 0)]
        public string WallPixH { get; set; }
    }
    #endregion

    #region 窗口相关------------
    //改动窗口的消息进行同步时候发送的类----------
    [DataContract]
    public class Json_SynchroTo_Hs_Wall_Modify_Win
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; } = "hscmd-video-wall-win-modify";
        [DataMember(Order = 0)]
        public Json_SynchroTo_Hs_Wall_Modify_Win_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; } = new Json_Rec_Hs_Cmd_End();
    }
    [DataContract]
    public class Json_SynchroTo_Hs_Wall_Modify_Win_Body
    {
        [DataMember(Order = 0)]
        public string WallID { get; set; }
        [DataMember(Order = 0)]
        public string WinID { get; set; }

        [DataMember(Order = 0)]
        public string WinPosX { get; set; }
        [DataMember(Order = 0)]
        public string WinPosY { get; set; }
        [DataMember(Order = 0)]
        public string WinDisplayWidth { get; set; }
        [DataMember(Order = 0)]
        public string WinDisplayHeight { get; set; }
        [DataMember(Order = 0)]
        public string TxID { get; set; }
        [DataMember(Order = 0)]
        public string WinLay { get; set; }
    }
    //------------开窗----------
    [DataContract]
    public class Json_Send_Hs_Wall_Open_Win
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; } = "hscmd-video-wall-win-add";
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_Open_Win_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; } = new Json_Rec_Hs_Cmd_End();
    }
    [DataContract]
    public class Json_Send_Hs_Wall_Open_Win_Body
    {
        [DataMember(Order = 0)]
        public string WallID { get; set; }
        [DataMember(Order = 0)]
        public string WinID { get; set; }
        [DataMember(Order = 0)]
        public string WinLay { get; set; }
        [DataMember(Order = 0)]
        public string WinPosX { get; set; }
        [DataMember(Order = 0)]
        public string WinPosY { get; set; }
        [DataMember(Order = 0)]
        public string WinDisplayWidth { get; set; }
        [DataMember(Order = 0)]
        public string WinDisplayHeight { get; set; }
        [DataMember(Order = 0)]
        public string TxID { get; set; }
    }
    [DataContract]
    public class Json_Rec_Hs_Wall_Open_Win
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Wall_Open_Win_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; }
    }
    [DataContract]
    public class Json_Rec_Hs_Wall_Open_Win_Body
    {
        [DataMember(Order = 0)]
        public string WinID { get; set; }
        [DataMember(Order = 0)]
        public string WinLay { get; set; }
    }
    //----------关闭窗口------------
    [DataContract]
    public class Json_Send_Hs_Wall_Close_Win
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; } = "hscmd-video-wall-win-delete";
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_Close_Win_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; } = new Json_Rec_Hs_Cmd_End();
    }
    [DataContract]
    public class Json_Send_Hs_Wall_Close_Win_Body
    {
        [DataMember(Order = 0)]
        public string WallID { get; set; }
        [DataMember(Order = 0)]
        public string WinID { get; set; }
    }
    [DataContract]
    public class Json_Rec_Hs_Wall_Close_Win
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; }
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_Close_Win_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; }
    }
    //-----------------改动窗口位置大小-----
    [DataContract]
    public class Json_Send_Hs_Wall_Size_Win
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; } = "hscmd-video-wall-win-move";
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_Size_Win_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; } = new Json_Rec_Hs_Cmd_End();
    }
    [DataContract]
    public class Json_Send_Hs_Wall_Size_Win_Body
    {
        [DataMember(Order = 0)]
        public string WallID { get; set; }
        [DataMember(Order = 0)]
        public string WinID { get; set; }
        [DataMember(Order = 0)]
        public string WinPosX { get; set; }
        [DataMember(Order = 0)]
        public string WinPosY { get; set; }
        [DataMember(Order = 0)]
        public string WinDisplayWidth { get; set; }
        [DataMember(Order = 0)]
        public string WinDisplayHeight { get; set; }
    }
    [DataContract]
    public class Json_Rec_Hs_Wall_Size_Win
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; }
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_Size_Win_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; }
    }
    //-----------------切换窗口信号源-----
    [DataContract]
    public class Json_Send_Hs_Wall_Swi_Win
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; } = "hscmd-video-wall-win-swi";
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_Swi_Win_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; } = new Json_Rec_Hs_Cmd_End();
    }
    [DataContract]
    public class Json_Send_Hs_Wall_Swi_Win_Body
    {
        [DataMember(Order = 0)]
        public string WallID { get; set; }
        [DataMember(Order = 0)]
        public string WinID { get; set; }
        [DataMember(Order = 0)]
        public string TxID { get; set; }
    }
    [DataContract]
    public class Json_Rec_Hs_Wall_Swi_Win
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; }
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_Swi_Win_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; }
    }
    //-----------------修改窗口层次-----
    [DataContract]
    public class Json_Send_Hs_Wall_ZIndex_Win
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; } = "hscmd-video-wall-win-lay";
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_ZIndex_Win_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; } = new Json_Rec_Hs_Cmd_End();
    }
    [DataContract]
    public class Json_Send_Hs_Wall_ZIndex_Win_Body
    {
        [DataMember(Order = 0)]
        public string WallID { get; set; }
        [DataMember(Order = 0)]
        public string WinID { get; set; }
        [DataMember(Order = 0)]
        public string WinLay { get; set; }
    }
    [DataContract]
    public class Json_Rec_Hs_Wall_ZIndex_Win
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; }
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_ZIndex_Win_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; }
    }
    //-----------------修改窗口层次-----
    [DataContract]
    public class Json_Send_Hs_Wall_Clear_Win
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; } = "hscmd-video-wall-win-delete-all";
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_Clear_Win_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; } = new Json_Rec_Hs_Cmd_End();
    }
    [DataContract]
    public class Json_Send_Hs_Wall_Clear_Win_Body
    {
        [DataMember(Order = 0)]
        public string WallID { get; set; }
    }
    [DataContract]
    public class Json_Rec_Hs_Wall_Clear_Win
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; }
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_Clear_Win_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; }
    }
    //--------服务器版接收全部的Json对象类
    [DataContract]
    public class Json_Send_Hs_Wall_Modify_Win
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; } = "hscmd-video-wall-win-modify";
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_Modify_Win_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; } = new Json_Rec_Hs_Cmd_End();
    }
    [DataContract]
    public class Json_Send_Hs_Wall_Modify_Win_Body
    {
        [DataMember(Order = 0)]
        public string WallID { get; set; }
        [DataMember(Order = 0)]
        public string WinID { get; set; }
        [DataMember(Order = 0)]
        public string WinPosX { get; set; }
        [DataMember(Order = 0)]
        public string WinPosY { get; set; }
        [DataMember(Order = 0)]
        public string WinDisplayWidth { get; set; }
        [DataMember(Order = 0)]
        public string WinDisplayHeight { get; set; }
        [DataMember(Order = 0)]
        public string TxID { get; set; }
        [DataMember(Order = 0)]
        public string WinLay { get; set; }
    }
    [DataContract]
    public class Json_Rec_Hs_Wall_Modify_Win
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; }
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_Modify_Win_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; }
    }

    #endregion

    #region 场景相关-----------------
    //----------------获取指定大屏（多个）下的当前窗口信息（第一次回显适用）
    [DataContract]
    public class Json_Send_Hs_Wall_Current_Scene
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; } = "hscmd-video-wall-win-get";
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_Current_Scene_Win_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; } = new Json_Rec_Hs_Cmd_End();
    }
    [DataContract]
    public class Json_Send_Hs_Wall_Current_Scene_Win_Body
    {
        [DataMember(Order = 0)]
        public List<string> Walls { get; set; }
    }
    [DataContract]
    public class Json_Rec_Hs_Wall_Current_Scene
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Wall_Current_Scene_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; }
    }
    [DataContract]
    public class Json_Rec_Hs_Wall_Current_Scene_Body
    {
        [DataMember(Order = 0)]
        public List<Json_Rec_Hs_Wall_Current_Scene_Body_Wall> Walls { get; set; }
    }
    [DataContract]
    public class Json_Rec_Hs_Wall_Current_Scene_Body_Wall
    {
        [DataMember(Order = 0)]
        public string WallID { get; set; }
        [DataMember(Order = 0)]
        public string WallName { get; set; }
        [DataMember(Order = 0)]
        public string Row { get; set; }
        [DataMember(Order = 0)]
        public string Column { get; set; }
        [DataMember(Order = 0)]
        public string StartX { get; set; }
        [DataMember(Order = 0)]
        public string StartY { get; set; }
        [DataMember(Order = 0)]
        public string WallPixW { get; set; }
        [DataMember(Order = 0)]
        public string WallPixH { get; set; }
        [DataMember(Order = 0)]
        public List<Json_Rec_Hs_Wall_Current_Scene_Body_Wall_Windows> Windows { get; set; }
    }
    [DataContract]
    public class Json_Rec_Hs_Wall_Current_Scene_Body_Wall_Windows
    {
        [DataMember(Order = 0)]
        public string WinID { get; set; }
        [DataMember(Order = 0)]
        public string WinLay { get; set; }
        [DataMember(Order = 0)]
        public string PosX { get; set; }
        [DataMember(Order = 0)]
        public string PosY { get; set; }
        [DataMember(Order = 0)]
        public string WinW { get; set; }
        [DataMember(Order = 0)]
        public string WinH { get; set; }
        [DataMember(Order = 0)]
        public string TxID { get; set; }
    }
    //----------------获取指定大屏（多个）下的所有场景+窗口信息 
    [DataContract]
    public class Json_Send_Hs_Wall_Get_Scene
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; } = "hscmd-video-wall-scene-get";
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_Get_Scene_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; } = new Json_Rec_Hs_Cmd_End();
    }
    [DataContract]
    public class Json_Send_Hs_Wall_Get_Scene_Body
    {
        [DataMember(Order = 0)]
        public List<string> Walls { get; set; }
    }
    [DataContract]
    public class Json_Rec_Hs_Wall_Get_Scene
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Wall_Get_Scene_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; }
    }
    [DataContract]
    public class Json_Rec_Hs_Wall_Get_Scene_Body
    {
        [DataMember(Order = 0)]
        public List<Json_Rec_Hs_Wall_Get_Scene_Body_Wall> Walls { get; set; }
    }
    [DataContract]
    public class Json_Rec_Hs_Wall_Get_Scene_Body_Wall
    {
        [DataMember(Order = 0)]
        public string WallID { get; set; }
        [DataMember(Order = 0)]
        public string WallName { get; set; }
        [DataMember(Order = 0)]
        public string WallRow { get; set; }
        [DataMember(Order = 0)]
        public string WallCol { get; set; }
        [DataMember(Order = 0)]
        public string WallStartX { get; set; }
        [DataMember(Order = 0)]
        public string WallStartY { get; set; }
        [DataMember(Order = 0)]
        public string WallPixW { get; set; }
        [DataMember(Order = 0)]
        public string WallPixH { get; set; }
        [DataMember(Order = 0)]
        public List<Json_Rec_Hs_Wall_Get_Scene_Body_Wall_Scene> Scene { get; set; }
    }
    [DataContract]
    public class Json_Rec_Hs_Wall_Get_Scene_Body_Wall_Scene
    {
        [DataMember(Order = 0)]
        public string SceneName { get; set; }
        [DataMember(Order = 0)]
        public List<Json_Rec_Hs_Wall_Current_Scene_Body_Wall_Windows> Windows { get; set; }
    }
    //-------------查询指定大屏的场景名称（指定大屏id为空，表示查询所有大屏的场景名字）
    [DataContract]
    public class Json_Send_Hs_Wall_Get_SceneName
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; } = "hscmd-video-wall-scene-modify";
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_Get_SceneName_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; } = new Json_Rec_Hs_Cmd_End();
    }
    [DataContract]
    public class Json_Send_Hs_Wall_Get_SceneName_Body
    {
        [DataMember(Order = 0)]
        public List<string> WallID { get; set; }
    }
    [DataContract]
    public class Json_Rec_Hs_Wall_Get_SceneName
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Wall_Get_SceneName_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; } = new Json_Rec_Hs_Cmd_End();
    }
    [DataContract]
    public class Json_Rec_Hs_Wall_Get_SceneName_Body
    {
        [DataMember(Order = 0)]
        public List<Json_Rec_Hs_Wall_Get_SceneName_Body_Wall> wall { get; set; }
    }
    [DataContract]
    public class Json_Rec_Hs_Wall_Get_SceneName_Body_Wall
    {
        [DataMember(Order = 0)]
        public string WallID { get; set; }
        [DataMember(Order = 0)]
        public List<string> SceneName { get; set; }
    }
    //-------------保存指定大屏墙的当前场景
    [DataContract]
    public class Json_Send_Hs_Wall_Save_Scene
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; } = "hscmd-video-wall-scene-save";
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_Save_Scene_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; } = new Json_Rec_Hs_Cmd_End();
    }
    [DataContract]
    public class Json_Send_Hs_Wall_Save_Scene_Body
    {
        [DataMember(Order = 0)]
        public string WallID { get; set; }
        [DataMember(Order = 0)]
        public string SceneName { get; set; }
    }
    [DataContract]
    public class Json_Rec_Hs_Wall_Save_Scene
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Wall_Save_Scene_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; }
    }
    [DataContract]
    public class Json_Rec_Hs_Wall_Save_Scene_Body
    {
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_Save_Scene_Body_Scene Scene { get; set; }
    }
    [DataContract]
    public class Json_Send_Hs_Wall_Save_Scene_Body_Scene
    {
        [DataMember(Order = 0)]
        public string WallID { get; set; }
        [DataMember(Order = 0)]
        public string SceneName { get; set; }
        [DataMember(Order = 0)]
        public List<Json_Rec_Hs_Wall_Current_Scene_Body_Wall_Windows> Windows { get; set; }
    }

    //-------------删除指定大屏墙的当前场景
    [DataContract]
    public class Json_Send_Hs_Wall_Delete_Scene
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; } = "hscmd-video-wall-scene-delete";
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_Delete_Scene_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; } = new Json_Rec_Hs_Cmd_End();
    }
    [DataContract]
    public class Json_Send_Hs_Wall_Delete_Scene_Body
    {
        [DataMember(Order = 0)]
        public string WallID { get; set; }
        [DataMember(Order = 0)]
        public string SceneName { get; set; }
    }
    [DataContract]
    public class Json_Rec_Hs_Wall_Delete_Scene
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; }
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_Delete_Scene_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; }
    }
    //-------------修改指定大屏墙的场景名字
    [DataContract]
    public class Json_Send_Hs_Wall_Modify_SceneName
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; } = "hscmd-video-wall-scene-modify";
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_Modify_SceneName_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; } = new Json_Rec_Hs_Cmd_End();
    }
    [DataContract]
    public class Json_Send_Hs_Wall_Modify_SceneName_Body
    {
        [DataMember(Order = 0)]
        public string WallID { get; set; }
        [DataMember(Order = 0)]
        public string SceneName { get; set; }
        [DataMember(Order = 0)]
        public string NewName { get; set; }
    }
    [DataContract]
    public class Json_Rec_Hs_Wall_Modify_SceneName_Body
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; }
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_Modify_SceneName_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; } = new Json_Rec_Hs_Cmd_End();
    }
    //-------------调用指定大屏墙的指定场景
    [DataContract]
    public class Json_Send_Hs_Wall_Call_Scene
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; } = "hscmd-video-wall-scene-call";
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_Call_Scene_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; } = new Json_Rec_Hs_Cmd_End();
    }
    [DataContract]
    public class Json_Send_Hs_Wall_Call_Scene_Body
    {
        [DataMember(Order = 0)]
        public string WallID { get; set; }
        [DataMember(Order = 0)]
        public string SceneName { get; set; }
    }
    [DataContract]
    public class Json_Rec_Hs_Wall_Call_Scene
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; }
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_Call_Scene_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; }
    }
    #endregion

    #region 预案相关--------------
    
    //----------------获取指定大屏（多个）下的所有预案+信息 
    [DataContract]
    public class Json_Send_Hs_Wall_Get_PrePlan
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; } = "hscmd-video-wall-preplan-get";
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_Get_PrePlan_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; } = new Json_Rec_Hs_Cmd_End();
    }
    [DataContract]
    public class Json_Send_Hs_Wall_Get_PrePlan_Body
    {
        [DataMember(Order = 0)]
        public List<string> Walls { get; set; }
    }
    [DataContract]
    public class Json_Rec_Hs_Wall_Get_PrePlan
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Wall_Get_PrePlan_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; }
    }
    [DataContract]
    public class Json_Rec_Hs_Wall_Get_PrePlan_Body
    {
        [DataMember(Order = 0)]
        public List<Json_Rec_Hs_Wall_Get_PrePlan_Body_Wall> Walls { get; set; }
    }
    [DataContract]
    public class Json_Rec_Hs_Wall_Get_PrePlan_Body_Wall
    {
        [DataMember(Order = 0)]
        public string WallID { get; set; }
        [DataMember(Order = 0)]
        public List<Json_Rec_Hs_Wall_Get_PrePlan_Body_Wall_PrePlan> PrePlans { get; set; }
    }
    [DataContract]
    public class Json_Rec_Hs_Wall_Get_PrePlan_Body_Wall_PrePlan
    {
        [DataMember(Order = 0)]
        public string PrePlanName { get; set; }
        [DataMember(Order = 0)]
        public string SwiInterval { get; set; }
        [DataMember(Order = 0)]
        public List<string> SceneNames { get; set; }
    }
    
    
    //-------------新建指定大屏墙的预案
    [DataContract]
    public class Json_Send_Hs_Wall_Create_PrePlan
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; } = "hscmd-video-wall-preplan-create";
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_Create_PrePlan_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; } = new Json_Rec_Hs_Cmd_End();
    }
    [DataContract]
    public class Json_Send_Hs_Wall_Create_PrePlan_Body
    {
        [DataMember(Order = 0)]
        public string WallID { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Wall_Get_PrePlan_Body_Wall_PrePlan PrePlan { get; set; }
    }

    //--------------------修改预案的参数
    [DataContract]
    public class Json_Send_Hs_Wall_Modify_PrePlan_Parameter
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; } = "hscmd-video-wall-preplan-modify";
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_Create_PrePlan_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; } = new Json_Rec_Hs_Cmd_End();
    }

    //-------------删除指定大屏墙的当前场景
    [DataContract]
    public class Json_Send_Hs_Wall_Delete_PrePlan
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; } = "hscmd-video-wall-preplan-delete";
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_Delete_PrePlan_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; } = new Json_Rec_Hs_Cmd_End();
    }
    [DataContract]
    public class Json_Send_Hs_Wall_Delete_PrePlan_Body
    {
        [DataMember(Order = 0)]
        public string WallID { get; set; }
        [DataMember(Order = 0)]
        public string PrePlanName { get; set; }
    }
    
    //-------------修改指定大屏墙的场景名字
    [DataContract]
    public class Json_Send_Hs_Wall_Modify_PrePlan_Name
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; } = "hscmd-video-wall-preplan-modify-name";
        [DataMember(Order = 0)]
        public Json_Send_Hs_Wall_Modify_PrePlanName_Body cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; } = new Json_Rec_Hs_Cmd_End();
    }
    [DataContract]
    public class Json_Send_Hs_Wall_Modify_PrePlanName_Body
    {
        [DataMember(Order = 0)]
        public string WallID { get; set; }
        [DataMember(Order = 0)]
        public string PrePlanName { get; set; }
        [DataMember(Order = 0)]
        public string NewName { get; set; }
    }
    
    #endregion

    #region Tx Rx列表相关-------------
    //Rx相关---------------
    [DataContract]
    public class Json_Send_Hs_ListRx_Get
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; } = "hscmd-video-wall-get-rx-list";
        [DataMember(Order = 0)]
        public string cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; } = new Json_Rec_Hs_Cmd_End();
    }
    [DataContract]
    public class Json_Rec_Hs_ListRx_Get
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_ListRx_Get_List cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; }
    }
    [DataContract]
    public class Json_Rec_Hs_ListRx_Get_List
    {
        [DataMember(Order = 0)]
        public List<Json_Rx_Property> RxIdList { get; set; }
    }
    [DataContract]
    public class Json_Rec_Hs_ListRx_Get_List_Rx
    {
        [DataMember(Order = 0)]
        public string ID { get; set; }
        [DataMember(Order = 0)]
        public string Ip { get; set; }
        [DataMember(Order = 0)]
        public string TcpPort { get; set; }
        [DataMember(Order = 0)]
        public string Name { get; set; }
        [DataMember(Order = 0)]
        public string WallID { get; set; }
        [DataMember(Order = 0)]
        public string Row { get; set; }
        [DataMember(Order = 0)]
        public string Column { get; set; }
    }
    //Tx列表相关--------------
    //Rx相关---------------
    [DataContract]
    public class Json_Send_Hs_ListTx_Get
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; } = "hscmd-video-wall-get-tx-list";
        [DataMember(Order = 0)]
        public string cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; } = new Json_Rec_Hs_Cmd_End();
    }
    [DataContract]
    public class Json_Rec_Hs_ListTx_Get
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_ListTx_Get_List cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; }
    }
    [DataContract]
    public class Json_Rec_Hs_ListTx_Get_List
    {
        [DataMember(Order = 0)]
        public List<Json_Tx_Property> TxIdList { get; set; }
    }
    [DataContract]
    public class Json_Rec_Hs_ListTx_Get_List_Tx
    {
        [DataMember(Order = 0)]
        public string ID { get; set; }
        [DataMember(Order = 0)]
        public string Name { get; set; }
        [DataMember(Order = 0)]
        public string Udp_addr { get; set; }
        [DataMember(Order = 0)]
        public string Udp_port { get; set; }
        [DataMember(Order = 0)]
        public string Ts_addr { get; set; }
        [DataMember(Order = 0)]
        public string Ts_port { get; set; }
        [DataMember(Order = 0)]
        public string Stream { get; set; }
    }
    #endregion

    #region Tx Rx 属性


    //Json:Rx属性-----------------------
    [DataContract]
    public class Json_Rx
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; } = "hscmd-video-rxInfo-config";
        [DataMember(Order = 0)]
        public Json_Rx_Property cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; }

    }
    [DataContract]
    public class Json_Rx_Property
    {
        [DataMember]
        public string ID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string DevType { get; set; }
        [DataMember]
        public string Ip { get; set; }
        [DataMember]
        public string TcpPort { get; set; }
        [DataMember]
        public string WallID { get; set; }
        [DataMember]
        public string Row { get; set; }
        [DataMember]
        public string Column { get; set; }
        [DataMember]
        public string Online { get; set; }
        [DataMember]
        public string IsBinded { get; set; }
        [DataMember]
        public string Version { get; set; }
    }
    //Json:Tx属性-----------------------
    [DataContract]
    public class Json_Tx
    {
        [DataMember(Order = 0)]
        public string cmd_header { get; set; } = "hscmd-video-txInfo-config";
        [DataMember(Order = 0)]
        public Json_Tx_Property cmd_body { get; set; }
        [DataMember(Order = 0)]
        public Json_Rec_Hs_Cmd_End cmd_end { get; set; }

    }
    [DataContract]
    public class Json_Tx_Property
    {
        [DataMember]
        public string ID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string DevType { get; set; }
        [DataMember]
        public string Ip { get; set; }
        [DataMember]
        public string TcpPort { get; set; }
        [DataMember]
        public string Udp_addr { get; set; }
        [DataMember]
        public string Udp_port { get; set; }
        [DataMember]
        public string Ts_addr { get; set; }
        [DataMember]
        public string Ts_port { get; set; }
        [DataMember]
        public string Stream { get; set; }
        [DataMember]
        public string Online { get; set; }
        [DataMember]
        public string Resolution { get; set; }
        [DataMember]
        public string Rate { get; set; }
        [DataMember]
        public string EncodeFormat { get; set; }
        [DataMember]
        public string Version { get; set; }
    }
    #endregion

    #endregion
}
