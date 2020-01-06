using ScreenManagerNS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Extensions
{
    static class ScreenExtension
    {
        public static ScreenViewModel GetScreen(this HsScreenInfo info)
        {
            ScreenViewModel screen = new ScreenViewModel()
            {
                IdScreen = info.IdScreen,
                BindMasterId = info.BindMasterId,
                Name = info.Name,
                Rows = info.Rows,
                Columns = info.Columns,
                StartX = info.StartX,
                StartY = info.StartY,
                WallPixW = info.WallPixW,
                WallPixH = info.WallPixH,
                UnitWidth = info.UnitWidth,
                UnitHeight = info.UnitHeight,
                GapWidth = info.GapWidth,
                GapHeight = info.GapHeight,
                IsMirror = info.IsMirror
            };
            foreach (var item in info.Rxid)
            {
                screen.Rxid.Add(item);
            }
            foreach (var item in info.Scenes)
            {
                screen.Scenes.Add(item);
            }
            foreach (var item in info.PrePlans)
            {
                screen.PrePlans.Add(item);
            }
            foreach (var item in info.CurrentWins)
            {
                screen.CurrentWins.Add(item);
            }
            return screen; 
        }
        //public static PrePlan GetPrePlan(this PrePlanInfo info)
        //{
        //    PrePlan pp = new PrePlan()
        //    {
        //        IdScreen = info.IdScreen, 
        //        Name = info.Name
                
        //    };             
        //    return pp;
        //}
    }
}
