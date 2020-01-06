using AbstractInterFace;
using ScreenManagerNS;
using System;
using System.Collections.Generic;
using System.Text;

namespace MosaicServerToStore
{
    class MosaicServerToXml : IMosaicServerToStore
    {
        public static List<HsScreenInfo> _listMosaicScreenInfo;
        public static Dictionary<string, HsScreenInfo> _dicNameToMosaicScreenInfo;
        public static Dictionary<int, HsScreenInfo> _dicIdToMosaicScreenInfo;
        public static List<HsScreenInfo> ListMosaicScreenInfo
        {
            get
            {
                if (_listMosaicScreenInfo == null)
                {
                    _listMosaicScreenInfo = new List<HsScreenInfo>();
                }
                return _listMosaicScreenInfo;
            }
        }
        public static Dictionary<string, HsScreenInfo> DicNameToMosaicScreenInfo
        {
            get
            {
                if (_dicNameToMosaicScreenInfo == null)
                {
                    _dicNameToMosaicScreenInfo = new Dictionary<string, HsScreenInfo>();
                }
                return _dicNameToMosaicScreenInfo;
            }
        }
        public static Dictionary<int, HsScreenInfo> DicIdToMosaicScreenInfo
        {
            get
            {
                if (_dicIdToMosaicScreenInfo == null)
                {
                    _dicIdToMosaicScreenInfo = new Dictionary<int, HsScreenInfo>();
                }
                return _dicIdToMosaicScreenInfo;
            }
        }

      
        public void Load()
        {
            //hsServer.GetXmlTxInfo("HS/Txes/Tx"); //结合查询服务器的xmlHS中的TxInfo

            MosaicServerToXml_Helper.GetXmlMosaicScreenInfo("HS/ScreenInfo", DicNameToMosaicScreenInfo, DicIdToMosaicScreenInfo);  //返回列表中有可能有重复的Id /Name 
            foreach (var item in DicIdToMosaicScreenInfo)
            {
                ListMosaicScreenInfo.Add(item.Value);
            }
        }



        #region 接口实现------------      
        //大屏相关


        public void DeleteScreen(HsScreenInfo screenInfo)
        {
            MosaicServerToXml_Helper.DeleteHsXml(MosaicServerToXml_Helper.xmlPathMosaicScreenInfo, MosaicServerToXml_Helper.xmlNodeMosaicScreenInfo + "/ScreenInfo[@IdScreen='" + screenInfo.IdScreen + "']");
        }
        public void InsertScreen(HsScreenInfo screenInfo)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in screenInfo.Rxid)
            {
                sb.Append(item);
                sb.Append(" ");
            }
            MosaicServerToXml_Helper.InsertHsXml(MosaicServerToXml_Helper.xmlPathMosaicScreenInfo, MosaicServerToXml_Helper.xmlNodeMosaicScreenInfo, "ScreenInfo",  //hsServer.xmlNodePlanRxTx + "[@name='" + plan.Name + "']" 不需要
                    new List<string> { "IdScreen", "Name", "Rows", "Columns", "StartX", "StartY", "WallPixW", "WallPixH", "IsMirror", "BindMasterId", "UnitW", "UnitH", "GapW", "GapH", "Rxid" },
                    new List<string> { screenInfo.IdScreen.ToString(), screenInfo.Name, screenInfo.Rows.ToString(), screenInfo.Columns.ToString(),
                        screenInfo.StartX.ToString(),screenInfo.StartY.ToString(),screenInfo.WallPixW.ToString(),screenInfo.WallPixH.ToString(),screenInfo.IsMirror.ToString(),screenInfo.BindMasterId.ToString(),
                        screenInfo.UnitWidth.ToString(),screenInfo.UnitHeight.ToString(),screenInfo.GapWidth.ToString(),screenInfo.GapHeight.ToString(),sb.ToString() });
            MosaicServerToXml_Helper.InsertHsXml(MosaicServerToXml_Helper.xmlPathMosaicScreenInfo, MosaicServerToXml_Helper.xmlNodeMosaicScreenInfo + "/ScreenInfo[@IdScreen='" + screenInfo.IdScreen + "']", "CurrentWins",  //hsServer.xmlNodePlanRxTx + "[@name='" + plan.Name + "']" 不需要
        new List<string> { "Wins" }, new List<string> { "" });

        }
        public void ModifyWallName(int idScreen, string newName)
        {
            List<string> values = new List<string>() { newName };
            MosaicServerToXml_Helper.UpdateHsXml(MosaicServerToXml_Helper.xmlPathMosaicScreenInfo, MosaicServerToXml_Helper.xmlNodeMosaicScreenInfo + "/ScreenInfo[@IdScreen='" + idScreen + "']", new List<string> { "Name"}, values);
        }
        public void ModifyWallBindMasterId(int idScreen, int newMasterId)
        {
            List<string> values = new List<string>() { newMasterId.ToString() };
            MosaicServerToXml_Helper.UpdateHsXml(MosaicServerToXml_Helper.xmlPathMosaicScreenInfo, MosaicServerToXml_Helper.xmlNodeMosaicScreenInfo + "/ScreenInfo[@IdScreen='" + idScreen + "']", new List<string> { "BindMasterId" }, values);
        }
        public void ModifyWallBindRxes(int idScreen, List<string> rxes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in rxes)
            {
                sb.Append(item);
                sb.Append(" ");
            }
            
            MosaicServerToXml_Helper.UpdateHsXml(MosaicServerToXml_Helper.xmlPathMosaicScreenInfo, MosaicServerToXml_Helper.xmlNodeMosaicScreenInfo + "/ScreenInfo[@IdScreen='" + idScreen + "']", new List<string> { "Rxid" }, new List<string> { sb.ToString() });
        }
        public void ModifyWallClipScreen(int idScreen, int x,int y,int w,int h)
        {
             MosaicServerToXml_Helper.UpdateHsXml(MosaicServerToXml_Helper.xmlPathMosaicScreenInfo, MosaicServerToXml_Helper.xmlNodeMosaicScreenInfo + "/ScreenInfo[@IdScreen='" + idScreen + "']", 
                 new List<string> { "StartX", "StartY", "WallPixW", "WallPixH" }, new List<string> { x.ToString(),y.ToString(),w.ToString(),h.ToString() });
        }
        //场景相关-------

        public void ModifySceneName(int idScreen, string oldName, string newName)
        {
            List<string> values = new List<string>() { newName };
            MosaicServerToXml_Helper.UpdateHsXml(MosaicServerToXml_Helper.xmlPathMosaicScreenInfo, MosaicServerToXml_Helper.xmlNodeMosaicScreenInfo + "/ScreenInfo[@IdScreen='" + idScreen + "']" + "/Scene[@Name='" + oldName + "']", new List<string> { "Name" }, values);

        }       

        public void InsertScene(SceneInfo sceneInfo)
        {
            StringBuilder sb = new StringBuilder();
            string splite1 = ",";
            string splite2 = " ";
            foreach (var item in sceneInfo.ListWins)
            {
                sb.Append(item.IdWin);
                sb.Append(splite1);
                sb.Append(item.IdTx);
                sb.Append(splite1);
                sb.Append(item.ZIndex);
                sb.Append(splite1);
                sb.Append(item.X);
                sb.Append(splite1);
                sb.Append(item.Y);
                sb.Append(splite1);
                sb.Append(item.Width);
                sb.Append(splite1);
                sb.Append(item.Height);
                sb.Append(splite2);
                sb.AppendLine();
            }
            MosaicServerToXml_Helper.InsertHsXml(MosaicServerToXml_Helper.xmlPathMosaicScreenInfo, MosaicServerToXml_Helper.xmlNodeMosaicScreenInfo + "/ScreenInfo[@IdScreen='" +sceneInfo.IdScreen + "']", "Scene",  //hsServer.xmlNodePlanRxTx + "[@name='" + plan.Name + "']" 不需要
                    new List<string> { "Name", "ListWins"},
                    new List<string> { sceneInfo.Name, sb.ToString() });

        }
        public void DeleteScene(SceneInfo sceneInfo)
        {
            MosaicServerToXml_Helper.DeleteHsXml(MosaicServerToXml_Helper.xmlPathMosaicScreenInfo, MosaicServerToXml_Helper.xmlNodeMosaicScreenInfo + "/ScreenInfo[@IdScreen='" + sceneInfo.IdScreen + "']" + "/Scene[@Name='" + sceneInfo.Name + "']");

        }

        //预案相关---------
        public void InsertPrePlan(PrePlanInfo info)
        {
            //StringBuilder sb = new StringBuilder();
            //string splite1 = ",";

            //foreach (var item in info.SceneNames)
            //{
            //    sb.Append(item);
            //    sb.Append(splite1);

            //}
            List<string> innerTexts = new List<string>();
            foreach (var item in info.SceneNames)
            {
                innerTexts.Add(item);
            }

            MosaicServerToXml_Helper.AddChildXmlElements(MosaicServerToXml_Helper.xmlPathMosaicScreenInfo, MosaicServerToXml_Helper.xmlNodeMosaicScreenInfo + "/ScreenInfo[@IdScreen='" + info.IdScreen + "']", "PrePlan", "SceneName",
                    innerTexts,  
                    new List<string> { "Name", "SwiInterval" },
                    new List<string> { info.Name, info.SwiInterval.ToString()});
            //MosaicServerToXml_Helper.InsertHsXml(MosaicServerToXml_Helper.xmlPathMosaicScreenInfo, MosaicServerToXml_Helper.xmlNodeMosaicScreenInfo + "/ScreenInfo[@IdScreen='" + info.IdScreen + "']", "PrePlan",  //hsServer.xmlNodePlanRxTx + "[@name='" + plan.Name + "']" 不需要
            //        new List<string> { "Name","SwiInterval", "Scenes" },
            //        new List<string> { info.Name,info.SwiInterval.ToString(), sb.ToString() });

        }
        public void DeletePrePlan(int idScreen,string name)
        {
            MosaicServerToXml_Helper.DeleteHsXml(MosaicServerToXml_Helper.xmlPathMosaicScreenInfo, MosaicServerToXml_Helper.xmlNodeMosaicScreenInfo + "/ScreenInfo[@IdScreen='" + idScreen + "']" + "/PrePlan[@Name='" + name + "']");

        }
        //修改预案名称
        public void ModifyPrePlanName(int idScreen, string oldName, string newName)
        {
            List<string> values = new List<string>() { newName };
            MosaicServerToXml_Helper.UpdateHsXml(MosaicServerToXml_Helper.xmlPathMosaicScreenInfo, MosaicServerToXml_Helper.xmlNodeMosaicScreenInfo + "/ScreenInfo[@IdScreen='" + idScreen + "']" + "/PrePlan[@Name='" + oldName + "']", new List<string> { "Name" }, values);

        }
        
        // 当前大屏窗口信息
        public void GetCurrentWins(HsScreenInfo screenInfo)
        {
            if (screenInfo==null|screenInfo.CurrentWins==null)
            {
                return;
            }
            if (DicIdToMosaicScreenInfo.ContainsKey(screenInfo.IdScreen))
            {
                var list = DicIdToMosaicScreenInfo[screenInfo.IdScreen];
                foreach (var item in list.CurrentWins)
                {
                    HsMosaicWinInfo winInfo = new HsMosaicWinInfo()
                    {
                        IdWin = item.IdWin,
                        IdTx = item.IdTx,
                        ZIndex = item.ZIndex,
                        X = item.X,
                        Y = item.Y,
                        Width = item.Width,
                        Height = item.Height
                    };
                    screenInfo.CurrentWins.Add(winInfo);
                }
            }
        }
        public void SaveCurrentWins(HsScreenInfo screenInfo)
        {
            StringBuilder sb = new StringBuilder();
            string splite1 = ",";
            string splite2 = " ";
            foreach (var item in screenInfo.CurrentWins)
            {
                sb.Append(item.IdWin);
                sb.Append(splite1);
                sb.Append(item.IdTx);
                sb.Append(splite1);
                sb.Append(item.ZIndex);
                sb.Append(splite1);
                sb.Append(item.X);
                sb.Append(splite1);
                sb.Append(item.Y);
                sb.Append(splite1);
                sb.Append(item.Width);
                sb.Append(splite1);
                sb.Append(item.Height);
                sb.Append(splite2);
                sb.AppendLine();
            }
            MosaicServerToXml_Helper.UpdateHsXml(MosaicServerToXml_Helper.xmlPathMosaicScreenInfo, MosaicServerToXml_Helper.xmlNodeMosaicScreenInfo + "/ScreenInfo[@IdScreen='" + screenInfo.IdScreen + "']" + "/CurrentWins", new List<string> { "Wins" }, new List<string> { sb.ToString() });
            
        }

        public List<HsScreenInfo> GetAllScreenInfo()
        {
            return ListMosaicScreenInfo;
        }
        #endregion
    }
}
