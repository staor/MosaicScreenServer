using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ScreenManagerNS
{
    public class HsScreenInfo
    {
        public string Name { get; set; } = "";
        public int Rows { get; set; } = 1;
        public int Columns { get; set; } = 1;
        public int IdScreen { get; set; }
        public int UnitWidth { get; set; } = 1920;
        public int UnitHeight { get; set; } = 1080;
        public int GapWidth { get; set; } = 0;
        public int GapHeight { get; set; } = 0;
        public int StartX { get; set; }
        public int StartY { get; set; }
        public int WallPixW { get; set; }
        public int WallPixH { get; set; }
        //public Rect TotalPixels
        //{
        //    get; set;
        //}
        public string Tag { get; set; }
        public List<string> Rxid { get; } = new List<string>();
        public List<HsMosaicWinInfo> CurrentWins { get; set; } = new List<HsMosaicWinInfo>();
        public List<SceneInfo> Scenes { get; set; } = new List<SceneInfo>();
        public List<PrePlanInfo> PrePlans { get; set; } = new List<PrePlanInfo>();
        public bool IsMirror { get; set; }  //是否作为镜像屏
        public int BindMasterId { get; set; }  //绑定的主屏Id
    }
}
