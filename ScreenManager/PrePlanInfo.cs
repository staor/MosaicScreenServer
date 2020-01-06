using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenManagerNS
{
    public class PrePlanInfo
    {
        public int IdScreen { get; set; }
        public string Name { get; set; }
        public int SwiInterval { get; set; }
        public List<string> SceneNames { get; set; } = new List<string>();
    }
}
