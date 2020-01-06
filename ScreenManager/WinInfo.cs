

using RxNS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Utils;

namespace ScreenManagerNS
{
    public enum WinShowType
    {
        WinGrid = 0, //单元格子屏 ==0默认值
        WinResolution = 1,//像素屏
        WinRoam = 2 //漫游窗口
    }
    public class HsMosaicWinInfo
    {
        public string Tag { get; set; }
        public int IdWin { get; set; }
        public string Name { get; set; }
    
        public string IdTx { get; set; }
        public string IdRx { get; set; }
        public int ZIndex { get; set; }

        public int IdScreen { get; set; }
        public int IdScene { get; set; }
        public Rect Position { get => new Rect(X, Y, Width, Height); }
        public int Chn { get; set; }
        public int SubChn { get; set; }

        public int Row { get; set; }
        public int Column { get; set; }
        public int RowSpan { get; set; }
        public int ColumnSpan { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsEnable { get; set; }

        public Rect ImageDefaultResolution { get; set; } = new Rect(0,0,1920,1080); //默认设置开窗播放的信号源画面分辨率为1920x1080
        public WinShowType WinShowType { get; set; }
        public List<RxInfo> RxInfo { get; } = new List<RxInfo>();//记录窗口位于哪些关联的单元Rx
       
    }
}
