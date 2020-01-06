

using RxNS;
using ScreenManagerNS;
using System;
using System.Collections.Generic;
using System.Text;
using TxNS;

namespace MosaicServerFrom
{
    public class MosaicServerEvent   //:IMosaicServerEvent
    {
        #region MyRegion

        
        //大屏相关-------------------------------
        public Action<Json_Rec_Hs_Cmd_End, HsScreenInfo> Event_NewMosaicWall;
        public Action<Json_Rec_Hs_Cmd_End, int> Event_CancelMosaicWall;
        public Action<Json_Rec_Hs_Cmd_End, int, string> Event_ModifyMosaicWallName;
        public Action<Json_Rec_Hs_Cmd_End, List<HsScreenInfo>> Event_GetMosaicWall;
        public Action<Json_Rec_Hs_Cmd_End, int,int> Event_SetMirrorBind;
        public Action<Json_Rec_Hs_Cmd_End, HsScreenInfo> Event_SetRxesBind;
        public Action<Json_Rec_Hs_Cmd_End, HsScreenInfo> Event_SetScreenClip;
        //窗口相关-------------------------------
        public Action<Json_Rec_Hs_Cmd_End, HsMosaicWinInfo> Event_OpenMosaicWin;
        public Action<Json_Rec_Hs_Cmd_End, int,int> Event_CloseMosaicWin;
        public Action<Json_Rec_Hs_Cmd_End, int> Event_CloseAllMosaicWin;
        public Action<Json_Rec_Hs_Cmd_End, HsMosaicWinInfo> Event_MoveMosaicWin;
        public Action<Json_Rec_Hs_Cmd_End, HsMosaicWinInfo> Event_LayMosaicWin;
        public Action<Json_Rec_Hs_Cmd_End, HsMosaicWinInfo> Event_SwiMosaicWin;

        public Action<Json_Rec_Hs_Cmd_End, HsMosaicWinInfo> Event_ModifyMosaicWin;
        

        //场景相关-------------------------------
        public Action<Json_Rec_Hs_Cmd_End, int, string> Event_CallScene;
        public Action<Json_Rec_Hs_Cmd_End, SceneInfo> Event_SaveScene;
        public Action<Json_Rec_Hs_Cmd_End, int, string> Event_DeleteScene;
        /// <summary>
        /// 返回 结果  大屏id  原名称 新名称
        /// </summary>
        public Action<Json_Rec_Hs_Cmd_End, int,string, string> Event_ModifySceneName;

        //预案相关-------------------------------
        public Action<Json_Rec_Hs_Cmd_End, PrePlanInfo> Event_CreatePrePlan;
        public Action<Json_Rec_Hs_Cmd_End, PrePlanInfo> Event_ModifyPrePlan;
        public Action<Json_Rec_Hs_Cmd_End, int, string> Event_DeletePrePlan;
        /// <summary>
        /// 返回 结果  大屏id  原名称 新名称
        /// </summary>
        public Action<Json_Rec_Hs_Cmd_End, int, string, string> Event_ModifyPrePlanName;
        //获取所有预案信息
        public Action<Json_Rec_Hs_Cmd_End, List<HsScreenInfo>> Event_GetPrePlans;

        //其他--------------
        public Action<Json_Rec_Hs_Cmd_End, List<HsScreenInfo>> Event_GetScenes;
        public Action<Json_Rec_Hs_Cmd_End, List<HsScreenInfo>> Event_GetCurrentWins;
        //bool SaveScene(string name, int idSecene);
        //public abstract bool SwitchSignal();
        //public abstract List<RoamWin> GetCurrentWall();
        //获取Tx Rx 列表------------
        public Action<Json_Rec_Hs_Cmd_End, List<RxInfo>> Event_GetRxes;
        public Action<Json_Rec_Hs_Cmd_End, List<TxInfo>> Event_GetTxes;

        //Action<Json_Rec_Hs_Cmd_End, HsScreenInfo> IMosaicServerEvent.Event_NewMosaicWall { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //Action<Json_Rec_Hs_Cmd_End, int> IMosaicServerEvent.Event_CancelMosaicWall { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //Action<Json_Rec_Hs_Cmd_End, int, string> IMosaicServerEvent.Event_ModifyMosaicWallName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //Action<Json_Rec_Hs_Cmd_End, List<int>, List<HsScreenInfo>> IMosaicServerEvent.Event_GetMosaicWall { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //Action<Json_Rec_Hs_Cmd_End, HsMosaicWinInfo> IMosaicServerEvent.Event_OpenMosaicWin { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //Action<Json_Rec_Hs_Cmd_End, int, int> IMosaicServerEvent.Event_CloseMosaicWin { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //Action<Json_Rec_Hs_Cmd_End, int> IMosaicServerEvent.Event_CloseAllMosaicWin { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //Action<Json_Rec_Hs_Cmd_End, HsMosaicWinInfo> IMosaicServerEvent.Event_MoveMosaicWin { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //Action<Json_Rec_Hs_Cmd_End, HsMosaicWinInfo> IMosaicServerEvent.Event_LayMosaicWin { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //Action<Json_Rec_Hs_Cmd_End, HsMosaicWinInfo> IMosaicServerEvent.Event_SwiMosaicWin { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //Action<Json_Rec_Hs_Cmd_End, HsMosaicWinInfo> IMosaicServerEvent.Event_ModifyMosaicWin { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //Action<Json_Rec_Hs_Cmd_End, int, string> IMosaicServerEvent.Event_CallScene { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //Action<Json_Rec_Hs_Cmd_End, SceneInfo> IMosaicServerEvent.Event_SaveScene { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //Action<Json_Rec_Hs_Cmd_End, int, string> IMosaicServerEvent.Event_DeleteScene { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //Action<Json_Rec_Hs_Cmd_End, int, string, string> IMosaicServerEvent.Event_ModifySceneName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //Action<Json_Rec_Hs_Cmd_End, List<HsScreenInfo>> IMosaicServerEvent.Event_GetScenes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //Action<Json_Rec_Hs_Cmd_End, List<HsScreenInfo>> IMosaicServerEvent.Event_GetCurrentWins { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //Action<Json_Rec_Hs_Cmd_End, List<RxInfo>> IMosaicServerEvent.Event_GetRxes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //Action<Json_Rec_Hs_Cmd_End, List<TxInfo>> IMosaicServerEvent.Event_GetTxes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        #endregion
    }
}
