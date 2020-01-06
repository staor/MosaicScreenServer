

using Hs_MosaicServer.ViewModels;
using Rx;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using Tx;
using Windows.Foundation;

namespace MosaicServerToRx
{
   
    /// <summary>
    /// 一个Rx4中的可开窗+裁剪的小窗口
    /// </summary>
    public class GridSub 
    {
        int _index;
        //string _tx;
        TxInfo _currentTx;
        bool _isEnable;
        int _dev_id;
        Rect _rectWin;
        Rect _rectCrop;
        int _zIndex;
        int _idWin;


        //public int IdScreenSub { get; set; }  //归属子屏的ID索引
        public int Index //位于ScreenSub的对应窗口位置索引，非列表索引 ，
        {
            get { return _index; }
            set
            {
                if (value != _index)
                {
                    _index = value;
                    //NotifyPropertyChanged(nameof(Index));
                }
            }
        }
        public string tx { get; set; } //对应Rx4的第几个开窗的序号tx1~tx2，临时值 。目前4个，对应第几个组播播放地址列表的序号，从0开始
        public TxInfo CurrentTx //小窗口对应的视频源
        {
            get { return _currentTx; }
            set
            {
                if (value != _currentTx)
                {
                    _currentTx = value;
                    //NotifyPropertyChanged(nameof(CurrentTx));
                }
            }
        }
        public ScreenSub ParentScreenSub { get; set; } //储存父引用，方便获取父类的Rx4.ip/port
        public bool IsEnable  //= true; //默认是可用的
        {
            get { return _isEnable; }
            set
            {
                if (value != _isEnable)
                {
                    _isEnable = value;
                    //NotifyPropertyChanged(nameof(IsEnable));
                }
            }
        }
        public int Dev_id { get; set; }
        public Rect RectWin
        {
            get { return _rectWin; }
            set
            {
                if (value != _rectWin)
                {
                    _rectWin = value;
                    //NotifyPropertyChanged(nameof(RectWin));
                }
            }
        }
        public Rect RectCrop
        {
            get { return _rectCrop; }
            set
            {
                if (value != _rectCrop)
                {
                    _rectCrop = value;
                    //NotifyPropertyChanged(nameof(RectCrop));
                }
            }
        }
        public int ZIndex
        {
            get { return _zIndex; }
            set
            {
                if (value != _zIndex)
                {
                    _zIndex = value;
                    //NotifyPropertyChanged(nameof(ZIndex));
                }
            }
        }
        public int IdWin //储存窗口Id
        {
            get { return _idWin; }
            set
            {
                if (value != _idWin)
                {
                    _idWin = value;
                    //NotifyPropertyChanged(nameof(IdWin));
                }
            }
        }
        public bool IsReplace { get; set; } //临时标识此对象在同一个窗口移动时是否将被取代
        public Win_Info Win { get; set; } //临时标识此对象在同一个窗口移动时是否将被取代
    }
    /// <summary>
    /// 子屏格子的Rx4对象的类
    /// </summary>
    public class ScreenSub
    {
        List<GridSub> _listGridSub;
        TxInfo _currentTx;
        public int Index { get; set; }  //归属子屏下的小窗口列表的索引的ID
        public int Row { get; set; }
        public int Column { get; set; }
        public int RowSpan { get; set; }
        public int ColumnSpan { get; set; }
        /// <summary>
        /// 相对在物理大屏上的坐标长宽
        /// </summary>
        public Rect RectScreenSub { get; set; }
        //本身的长宽尺寸（0，0，w,h）
        public Rect RectDefaultSize { get; set; }
        /// <summary>
        /// 此Rx当前所有的窗口列表,目前只支持叠加4个字窗口WinSub，其字窗口对应Tx需要按列表顺序过来，
        /// </summary>
        public List<GridSub> ListGridSub
        {
            get
            {
                if (_listGridSub == null)
                {
                    _listGridSub = new List<GridSub>();
                }
                return _listGridSub;
            }
        }
        public RxInfo Rx4 { get; set; }
        public TxInfo CurrentTx
        {
            get
            {
                return _currentTx;
            }
            set
            {
                if (_currentTx != value)
                {
                    _currentTx = value;
                }
            }
        }

        public void UpdateRxInfoWins(RxInfo rxInfo)
        {
            if (rxInfo == null)
            {
                return;
            }
            if (rxInfo.IsOnline)
            {                
                foreach (var item in ListGridSub)
                {
                    //if (item.IsEnable)  //Rx盒子不重启 时上线会保留原窗口，因此需要发送4条指令
                    //{
                        Thread.Sleep(20);
                        ServerScreenSubHelper.HS_OpenOrClose_PlayWin(item, rxInfo.Ip4, rxInfo.Port4);
                    //}
                }
            }
        }

    }

}
