

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Tx;
using Windows.Foundation;

namespace MosaicServerToRx
{
    public class Win_Info
    {
        public int IdWin;
        public int X;
        public int Y;
        public int W;
        public int H;
        public int ZIndex;
        public List<GridSub> ListGridSub =new List<GridSub>();
        public TxInfo TxInfo;
        public bool IsEnable;
        public Rect Position
        {
            get=>new Rect(X, Y, W, H);
        }
        public Rect ImageDefaultResolution = new Rect(0, 0, 1920, 1080);
    }
}
