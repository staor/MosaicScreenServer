
using ScreenManagerNS;
using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractInterFace

{
    public interface IMosaicServerToStore
    {
        void Load();
        List<HsScreenInfo> GetAllScreenInfo();
        //大屏相关
        void InsertScreen(HsScreenInfo screenInfo);
        void DeleteScreen(HsScreenInfo screenInfo);
        void ModifyWallName(int idScreen, string newName);
        void ModifyWallBindMasterId(int idScreen, int newMasterId);
        void ModifyWallBindRxes(int idScreen, List<string> rxes);
        void ModifyWallClipScreen(int idScreen, int x,int y,int w,int h);
        //场景相关-------
        void InsertScene(SceneInfo sceneInfo);
        void DeleteScene(SceneInfo sceneInfo);
        void ModifySceneName(int idScreen, string oldName, string newName);
        //预案相关-------
        void InsertPrePlan(PrePlanInfo info);
        void DeletePrePlan(int idScreen,string name);

        void ModifyPrePlanName(int idScreen,string oldName, string newName);
        // 当前大屏窗口信息
        void SaveCurrentWins(List<HsScreenInfo> screenInfos);
        void GetCurrentWins(HsScreenInfo screenInfo);
    }
}
