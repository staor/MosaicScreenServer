using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenManagerNS
{
    public class SceneInfo
    {
        public int IdScene { get; set; }
        public string Name { get; set; }
        public int IdScreen { get; set; }
        public int WidthScreen { get; set; }
        public int HeightScreen { get; set; }
        public List<HsMosaicWinInfo> ListWins { get; set; } = new List<HsMosaicWinInfo>();
    }
}
