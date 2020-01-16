
using ConfigServer;
 
using RxNS;
using ScreenManagerNS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TxNS;
using Utils;

namespace ScreenManagerNS
{
    public delegate void ShowDebugTextDelegate(string arg);
    public class ScreenViewModel :HsScreenInfo
    {
        public ShowDebugTextDelegate ShowDebugTextEvent { get; set; }
        #region Varivable
      
        public List<HsMosaicWinInfo> ListRoamWines { get; private set; } = new List<HsMosaicWinInfo>();
       
        public Dictionary<string, TxInfo> DicTxInfo = new Dictionary<string, TxInfo>();

        public static ScreenViewModel SelectedScreenViewModel { get; set; }


     
        private List<RxInfo> _screenRxInfos;
        public List<RxInfo> ScreenRxInfos
        {
            get
            {
                if (_screenRxInfos == null)
                {
                    _screenRxInfos = new List<RxInfo>();
                }
                return _screenRxInfos;
            }
            private set
            {
                if (value == _screenRxInfos)
                    return;
                _screenRxInfos = value;
                //OnPropertyChanged(nameof(ScreenRxInfos));
            }
        }

        public Rect ReferTotalPixels
        {
            get => new Rect(0, 0, Columns * UnitWidth, Rows * UnitHeight);
        }


        private Rect _rectCanvasActualSize;
        public Rect RectCanvasActualSize
        {
            get { return _rectCanvasActualSize; }
            set
            {
                if (value != _rectCanvasActualSize)
                {
                    _rectCanvasActualSize = value;
                    //OnPropertyChanged(nameof(RectCanvasActualSize));
                }
            }
        }

        public bool IsPrearrange { get; set; } 
   
        public WinShowType WinShowType
        {
            get; set;
        } = WinShowType.WinRoam;
        int _limitWin = 4;
        /// <summary>
        /// 单个Rx4开窗的实际上限数量，默认为4个
        /// </summary>
        public int LimitWin
        {
            get { return _limitWin; }
            set
            {
                if (value != _limitWin)
                {
                    _limitWin = value;
                    //OnPropertyChanged(nameof(LimitWin));
                }
            }
        }
        
        public string Ip { get; set; }
        public int Port { get; set; }
        

        public List<ScreenViewModel> ListMirrors { get; set; } = new List<ScreenViewModel>();
        #endregion
        public ScreenViewModel()
        {
            
        }



        #region Methods
        public bool Initial()
        {
            bool isOk = true;
            //if (Rows * Columns != Rxid.Count)
            //{
            //    hsServer.ShowDebug("MosaicRoam中行列数与Rxes数量不一致(将忽略此拼接墙)：" + Rows + " x " + Columns + " != " + Rxid.Count);
            //    return false;
            //}

            for (int i = 0; i < Rxid.Count; i++)
            {
                RxInfo rxInfo = null; 
                if (RxInfo.DicRxInfo.ContainsKey(Rxid[i]))
                {
                    rxInfo = RxInfo.DicRxInfo[Rxid[i]];
                }
                else
                {
                    rxInfo = new RxInfo(Rxid[i], "0.0.0.0", 0);
                }
                ScreenRxInfos.Add(rxInfo);
                 
                rxInfo.IsBinded = true;
                rxInfo.WallID = IdScreen;
                rxInfo.Row = i % Columns;
                rxInfo.Column = i / Columns;
                rxInfo.RectScreenSub= new Rect(rxInfo.Column * UnitWidth, rxInfo.Row*UnitHeight, UnitWidth, UnitHeight);// 此方框相对物理大屏分辨率而言..........     
                 

                if (!IsMirror) //镜像屏暂时不设置窗口数据，由主屏动态设定
                {
                    for (int j = 0; j < LimitWin; j++)
                    {
                        HsMosaicWinInfo winInfo = new HsMosaicWinInfo();
                        winInfo.IdScreen = IdScreen;
                        winInfo.IdWin = i * LimitWin + j + 1;  //从1 开始

                        winInfo.Chn = winInfo.IdWin;
                        winInfo.IdScreen = IdScreen; //默认都是拼接处理器墙的第一组屏幕
                        winInfo.IsEnable = false;
                        
                        //winInfo.WinShowType = WinShowType;
                        //winInfo.Row = i % Columns;
                        //winInfo.Column = i / Columns;
                        //winInfo.RowSpan = 1;
                        //winInfo.ColumnSpan = 1;

                        ListRoamWines.Add(winInfo);
                    }
                }
            }
            if (!IsMirror)
            {
                SceneLayoutAndPlay(CurrentWins);
                
            }
            
            return isOk;
        }

        public void SceneLayoutAndPlay(List<HsMosaicWinInfo>  list)
        {
            Dictionary<int, HsMosaicWinInfo> dic = new Dictionary<int, HsMosaicWinInfo>();
            foreach (var item in list)
            {
                if (!dic.ContainsKey(item.IdWin))
                {
                    dic.Add(item.IdWin,item);
                }
            }
            foreach (var item in ScreenRxInfos)
            {
                item.SumSubWins = 0;
            }
            foreach (var win in ListRoamWines)
            {
                if (dic.ContainsKey(win.IdWin))
                {
                    HsMosaicWinInfo item = dic[win.IdWin];
                    win.IdTx = item.IdTx;
                    win.X = item.X;
                    win.Y = item.Y;
                    win.Width = item.Width;
                    win.Height = item.Height;
                    win.ZIndex = item.ZIndex;
                    //win.IsEnable = item.IsEnable;
                    if (CanNewWin(win,new Rect(win.X,win.Y,win.Width,win.Height)))
                    {
                        win.IsEnable = true;
                    }
                    else
                    {
                        win.IsEnable = false;
                        hsServer.ShowDebug("SceneLayoutAndPlay-窗口无法开窗：" + win.IdWin + ":" + win.X + "," + win.Y + "," + win.Width + "," + win.Height);
                    }                    
                }
                else
                {
                    win.IsEnable = false;
                }
            }
        }

        public void UpdateInfoToViewModel()
        {
            //更新大屏当前窗口信息
            //Dictionary<int, HsMosaicWinInfo> dicIdToWinFromScene = new Dictionary<int, HsMosaicWinInfo>();
            //foreach (var item in CurrentWins)
            //{
            //    if (!dicIdToWinFromScene.ContainsKey(item.IdWin))
            //    {
            //        dicIdToWinFromScene.Add(item.IdWin, item);
            //    }
            //    else
            //    {
            //        hsServer.ShowDebug("UpdateInfoToViewModel-当前场景中的Id有重复(将忽略）：" + item.IdWin);
            //    }
            //}
            
            //更新大屏场景信息
            //foreach (var item in Scenes)
            //{
            //    SceneViewModel scene = new SceneViewModel(item)
            //    {
            //        ParentScreen = this,
            //        IdScreen = IdScreen,
            //        WidthScreen = WallWidth,
            //        HeightScreen = WallHeight
            //    };
            //    scene.SetWinInfoToWinViewModels();
            //    Scenes.Add(scene);
            //}

        }

        public void UpdateCurrentViewModelWinToInfo()
        {
            CurrentWins.Clear();
            foreach (var item in ListRoamWines)
            {
                if (item.IsEnable)
                {
                    HsMosaicWinInfo winInfo = new HsMosaicWinInfo()
                    {
                        IdWin = item.IdWin,
                        ZIndex = item.ZIndex,
                        IdTx = item.IdTx,
                        X = (int)item.Position.X,
                        Y = (int)item.Position.Y,
                        Width = (int)item.Position.Width,
                        Height = (int)item.Position.Height
                    };
                    CurrentWins.Add(winInfo);
                }
            }
        }


        //public void UpdateCurrentViewModelWinToInfo()
        //{
        //    ScreenInfo.CurrentWins.Clear();
        //    foreach (var item in ListRoamWines)
        //    {
        //        if (item.Visible== Visibility.Visible)
        //        {
        //            HsMosaicWinInfo winInfo = new HsMosaicWinInfo()
        //            {
        //                IdWin = item.IdWin,
        //                ZIndex = item.ZIndex,
        //                IdTx = item.CurrentRx.IdTx,
        //                X = (int)item.Position.X,
        //                Y = (int)item.Position.Y,
        //                Width = (int)item.Position.Width,
        //                Height = (int)item.Position.Height
        //            };
        //            ScreenInfo.CurrentWins.Add(winInfo);
        //        }
        //    }
        //}

        //public void UpdateCurrentWinToRxServer()
        //{
        //    Task t = Task.Run(() =>
        //    {
        //        Thread.Sleep(3200); //错开一般时间
        //        // await Task.Delay(3200);
        //        if (MosaicServerToRx != null)
        //        {
        //            MosaicServerToRx.Win_Clear();
        //        }
        //        else
        //        {
        //            return;
        //        }
        //        Thread.Sleep(20);  //等待清空指令已发送，保证优先Rx接收到
        //        //for (int i = 0; i < 1000; i++)  //等待清空指令已发送，保证优先Rx接收到
        //        //{

        //        //}
        //        foreach (var item in ListRoamWines)
        //        {
        //            HsMosaicWinInfo rw = item.RoamWinInfo;
        //            if (item.Visible == Visibility.Visible)
        //            {
        //                if (TxInfo.DicTxInfo.ContainsKey(rw.IdTx))
        //                {
        //                    TxInfo txInfo = TxInfo.DicTxInfo[rw.IdTx];
        //                    MosaicServerToRx.Win_Open(rw.IdWin, rw.X, rw.Y, rw.Width, rw.Height, txInfo);
        //                    Thread.Sleep(20);  //保证发送时候的顺序
        //                }
        //                else
        //                {
        //                    hsServer.ShowDebug("UpdateCurrentWinToRx-当前Tx字典中未包含id：" + rw.IdWin + "-" + rw.IdTx);
        //                }
        //            }
        //        }
        //    });
        //}


        /// <summary>
        /// 只判断和更新ScreenViewModel的数据，和MosaicServerToRx的数据更新
        /// </summary>
        /// <param name="rxes"></param>
        /// <returns></returns>
        public bool SetRxesBind(List<string> rxes)
        {
            bool isOk = true;
            List<RxInfo> tempRxes = new List<RxInfo>();
            List<RxInfo> NewRxes = new List<RxInfo>();
            List<RxInfo> OldRxes = new List<RxInfo>();
            bool isBindingOk = true;
            if (rxes.Count!=Rows*Columns)
            {
                return false;
            }
            foreach (var item in ScreenRxInfos)
            {
                OldRxes.Add(item);
            }
            for (int i = 0; i < rxes.Count; i++)
            {
                RxInfo rxInfo = null;
                //RxInfo rxInfo = ListRxInfo.Find(f => f.Id == item);
                if (RxInfo.DicRxInfo.ContainsKey(rxes[i]))
                {
                    rxInfo = RxInfo.DicRxInfo[rxes[i]];
                }
                if (rxInfo != null)
                {
                    if (rxInfo.WallID == -1)
                    {
                        rxInfo.WallID = IdScreen;
                        rxInfo.IsBinded = true;
                        rxInfo.Row = i / Columns;
                        rxInfo.Column = i % Columns;
                        NewRxes.Add(rxInfo);
                        tempRxes.Add(rxInfo);
                    }
                    else if (rxInfo.WallID == IdScreen)
                    {
                        tempRxes.Add(rxInfo);
                        OldRxes.Remove(rxInfo);
                    }
                    else
                    {
                        isBindingOk = false;
                        break;
                    }
                }
                else
                {
                    isBindingOk = false;
                    break;
                }
            }
            if (!isBindingOk)
            {
                foreach (var item in NewRxes)
                {
                    item.WallID = -1;
                    item.IsBinded = false;
                }
                return false;
            }


            //检查简单逻辑判断---
            if (Rows * Columns != tempRxes.Count)
            {
                foreach (var item in NewRxes)
                {
                    item.WallID = -1;
                    item.IsBinded = false;
                }
                return false;
            }

            
            ScreenRxInfos.Clear();
            Rxid.Clear();
            foreach (var item in tempRxes)
            {
                ScreenRxInfos.Add(item);
                Rxid.Add(item.Id);
            }
            foreach (var item in OldRxes)  //还原解绑掉的RxInfo值
            {
                item.WallID = -1;
                item.Column = 0;
                item.Row = 0;
                item.IsBinded = false;
            }            
            return isOk;
        }
        public bool SetScreenClip(int x,int y,int w,int h)
        {
            bool isOk = true;
            if (StartX==x&&StartY==y&&WallPixW==w&&WallPixH==h)
            {
                return isOk;
            }
            Rect rectRefer = ReferTotalPixels;
            if (x<0||x>=rectRefer.Width||x+w> rectRefer.Width||y<0||y>=rectRefer.Height||y+h>rectRefer.Height)
            {
                return false;
            }
            StartX = x;
            StartY = y;
            WallPixW = w;
            WallPixH = h;
              //由客户端进行清空当前大屏
            return isOk;
        }


        public Rect RectifyPostion(Rect position,double rate=0, int uniteW=1920,int uniteH=1080)
        {
            Rect newP = position;
            Rect totalPixels = new Rect(StartX,StartY,WallPixW,WallPixH);
            if (position.X<StartX||position.Y<StartY||position.Right> totalPixels.Right||position.Bottom> totalPixels.Bottom)
            {
                return Rect.Empty;
            }
            if (rate==0||rate<0.1||rate>0.3)
            {             
                return newP; 
            }          

            int wOff = (int)(uniteW * rate);
            int hOff = (int)(uniteH * rate);
            int x1Remainder = (int)position.X % uniteW;
            if (x1Remainder<wOff)
            {
                if (newP.X-x1Remainder<=StartX)
                {
                    newP.X = StartX;
                    newP.Width = (int)position .Right- StartX;
                }
                else
                {
                    newP.X = newP.X - x1Remainder;
                    newP.Width += x1Remainder;
                }                
            }
            else if (uniteW-x1Remainder< wOff)
            {
                newP.X += uniteW - x1Remainder;
                if (position.Right % uniteW < wOff)
                {
                    newP.Width -= x1Remainder;
                }
            }

            int x2Remainder = (int)newP.Right % uniteW;
            if (x2Remainder < wOff)
            {
                newP.Width -= x2Remainder;
            }
            else if (uniteW - x2Remainder <= wOff)
            {
                newP.Width = newP.Right + uniteW - x2Remainder > totalPixels.Right ? totalPixels.Right - newP.X : newP.Width + uniteW - x2Remainder;
            }
            if (newP.Width==0)
            {
                return Rect.Empty;
            }

            int y1Remainder = (int)position.Y % uniteH;
            if (y1Remainder < hOff)
            {
                if (newP.Y - y1Remainder <= StartY)
                {
                    newP.Y = StartY;
                    newP.Height = (int)position.Bottom - StartY;
                }
                else
                {
                    newP.Y = newP.Y - y1Remainder;
                    newP.Height += y1Remainder;
                }                              
            }
            else if (uniteH - y1Remainder < hOff)
            {
                newP.Y += uniteH - y1Remainder;
                if (position.Bottom % uniteH < hOff)
                {
                    newP.Height -= y1Remainder;
                }
            }

            int y2Remainder = (int)newP.Bottom % uniteH;
            if (y2Remainder < wOff)
            {
                newP.Height -= y2Remainder;
            }
            else if (uniteH - y2Remainder <= wOff)
            {
                newP.Height = newP.Bottom + uniteH - y2Remainder > totalPixels.Bottom ? totalPixels.Bottom - newP.Y : newP.Height + uniteH - y2Remainder;
            }
            if (newP.Height == 0)
            {
                return Rect.Empty;
            }

            return newP;
        }

        /// <summary>
        /// 镜像屏附加的窗口磁贴及缩放最小窗口放大功能
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rate"></param>
        /// <param name="uniteW"></param>
        /// <param name="uniteH"></param>
        /// <returns></returns>
        public Rect RectifyPostionToMirror(Rect position, double rate = 0, int uniteW = 1920, int uniteH = 1080)
        {
            Rect newP = position;
            Rect totalPixels = new Rect(StartX, StartY, WallPixW, WallPixH); 
            if (position.X < StartX || position.Y < StartY || position.Right > totalPixels.Right || position.Bottom > totalPixels.Bottom)
            {
                return Rect.Empty;
            }
            if (rate == 0 || rate < 0.1 || rate > 0.3)
            {
                return newP;
            }

            int wOff = (int)(uniteW * rate);
            int hOff = (int)(uniteH * rate);
            int x1Remainder = (int)position.X % uniteW;
            if (x1Remainder < wOff)
            {
                if (newP.X - x1Remainder <= StartX)
                {
                    newP.X = StartX;
                    newP.Width = (int)position.Right - StartX;
                }
                else
                {
                    newP.X = newP.X - x1Remainder;
                    newP.Width += x1Remainder;
                }
            }
            else if (uniteW - x1Remainder < wOff)
            {
                newP.X += uniteW - x1Remainder;
                if (position.Right % uniteW < wOff)
                {
                    newP.Width -= x1Remainder;
                }
            }

            int x2Remainder = (int)newP.Right % uniteW;
            if (x2Remainder < wOff)
            {
                newP.Width -= x2Remainder;
            }
            else if (uniteW - x2Remainder <= wOff)
            {
                newP.Width = newP.Right + uniteW - x2Remainder > totalPixels.Right ? totalPixels.Right - newP.X : newP.Width + uniteW - x2Remainder;
            }
            if (newP.Width == 0)
            {
                return Rect.Empty;
            }

            int y1Remainder = (int)position.Y % uniteH;
            if (y1Remainder < hOff)
            {
                if (newP.Y - y1Remainder <= StartY)
                {
                    newP.Y = StartY;
                    newP.Height = (int)position.Bottom - StartY;
                }
                else
                {
                    newP.Y = newP.Y - y1Remainder;
                    newP.Height += y1Remainder;
                }
            }
            else if (uniteH - y1Remainder < hOff)
            {
                newP.Y += uniteH - y1Remainder;
                if (position.Bottom % uniteH < hOff)
                {
                    newP.Height -= y1Remainder;
                }
            }

            int y2Remainder = (int)newP.Bottom % uniteH;
            if (y2Remainder < wOff)
            {
                newP.Height -= y2Remainder;
            }
            else if (uniteH - y2Remainder <= wOff)
            {
                newP.Height = newP.Bottom + uniteH - y2Remainder > totalPixels.Bottom ? totalPixels.Bottom - newP.Y : newP.Height + uniteH - y2Remainder;
            }
            if (newP.Height == 0)
            {
                return Rect.Empty;
            }

            return newP;
        }

        #region V2.0 协议内部处理方法-----

        /// <summary>
        /// 验证是否可以新开或移动一个窗口
        /// </summary>
        /// <param name="info">当前要新开的窗口，包含IdWin和新坐标</param>

        /// <param name="IsSure">是否确认执行，默认若可以则执行后续改变</param>
        /// <returns></returns>
        public bool CanOpenWin(HsMosaicWinInfo info, bool IsSure = true)
        {
            bool isOk = false;
            return isOk;
        }
        //关闭窗口更新 每个单元屏记录小格子信息
        public void CloseWinToGridSub(HsMosaicWinInfo info)
        {

        }
        #endregion

        private bool CanNewWin(HsMosaicWinInfo info,Rect newPosition, bool isOpen = true)
        {
            bool isOk = true;
            List<RxInfo> list = new List<RxInfo>();
            foreach (var item in ScreenRxInfos)
            {
                Rect rectGrid = Rect.Intersect(item.RectScreenSub, newPosition);
                if (rectGrid.Width>0&&rectGrid.Height>0)
                {
                    if (rectGrid.Width < 128 || rectGrid.Height < 72)
                    {
                        hsServer.ShowDebug("有Rx4底格子的窗口叠加列表中小窗口最小尺寸不符合要求：（限制为w>=128&&h>=72）当前：" + rectGrid);
                        return false;
                    }
                    if (item.SumSubWins>=LimitWin)
                    {
                        return false;
                    }
                    
                    list.Add(item);
                }                
            }
            if (isOpen)
            {
                info.RxInfo.Clear();
                foreach (var item in list)
                {
                    item.SumSubWins++;
                    info.RxInfo.Add(item);
                }
            }
            return isOk;
        }
        private bool CanMoveWin(HsMosaicWinInfo info, Rect newPosition, bool isMove = true)
        {
            bool isOk = true;
            
            List<RxInfo> list = new List<RxInfo>();
            foreach (var item in ScreenRxInfos)
            {
                Rect rectGrid = Rect.Intersect(item.RectScreenSub, newPosition);
                if (rectGrid.Width > 0 && rectGrid.Height > 0)
                {
                    if (rectGrid.Width < 128 || rectGrid.Height < 72)
                    {
                        hsServer.ShowDebug("有Rx4底格子的窗口叠加列表中小窗口最小尺寸不符合要求：（限制为w>=128&&h>=72）当前：" + rectGrid);
                        return false;
                    }
                    if (item.SumSubWins >= LimitWin&&!info.RxInfo.Contains(item))
                    {
                        return false;
                    }

                    list.Add(item);
                }
            }
            if (isMove)
            {
                IEnumerable<RxInfo> temp1 = info.RxInfo.Except(list); //, new RxInfoComparer()
                foreach (var item in temp1)
                {
                    item.SumSubWins--;
                }                
                IEnumerable<RxInfo> temp2 = list.Except( info.RxInfo); //, new RxInfoComparer()
                foreach (var item in temp2)
                {
                    item.SumSubWins++;
                }
                info.RxInfo.Clear();
                foreach (var item in list)
                {
                    info.RxInfo.Add(item);
                }
            }
            return isOk;
        }

        #region 拼接处理方法相关-------------
        //---------大屏相关

        //------------窗口相关


        public bool Event_OpenWin_ToRx(HsMosaicWinInfo rw)
        {
            bool isOk = false;
            int id = rw.IdWin;
            //if (rw.IdWin<1|rw.IdWin>ListRoamWines.Count)  //若超出Id范围，默认为无Id新开窗
            //{
                //return false;  //严格判断有服务端返回Id
                foreach (var item in ListRoamWines)
                {
                    if (!item.IsEnable)
                    {
                        rw.IdWin = item.IdWin;
                        id = item.IdWin;
                        break;
                    }
                }
            //}
            HsMosaicWinInfo Win = ListRoamWines[id - 1];
            //进行修复坐标，以防出现小格子长宽限制问题
            Rect newP=RectifyPostion(rw.Position, hsConfig.WinRectRate);
            if (newP.IsEmpty)
            {
                return false;
            }
            else
            {
                rw.X = (int)newP.X;
                rw.Y = (int)newP.Y;
                rw.Width = (int)newP.Width;
                rw.Height = (int)newP.Height;
            }
            Win.X = rw.X;
            Win.Y = rw.Y;
            Win.Width = rw.Width;
            Win.Height = rw.Height;

            isOk = CanNewWin(Win,Win.Position);
             
            if (isOk)
            {                 
              
                Win.IsEnable = true;

                Win.IdTx = rw.IdTx;
                rw.IdWin = Win.IdWin;
                rw.ZIndex = ListRoamWines.Count(c => c.IsEnable);  //默认开窗在最上层。。。
                Win.ZIndex = rw.ZIndex;
            }
            else
            {
                hsServer.ShowDebug("OpenWin_ToRx-开窗失败！");
            }
            
            return isOk;
        }


        public bool Event_CloseWin_ToRx(int id)
        {
            bool isOk = true;
            if (id < 1 || id > ListRoamWines.Count)
            {
                return false;
            }
            HsMosaicWinInfo Win = ListRoamWines[id - 1];
            Win.IsEnable = false;
            foreach (var item in Win.RxInfo)
            {
                item.SumSubWins--;
            }
            Win.RxInfo.Clear();
            foreach (var item in ListRoamWines)
            {
                if (item.IsEnable && item.ZIndex > Win.ZIndex)
                {
                    item.ZIndex--;
                }
            }
            return isOk;
        }

        public bool Event_CloseOrHiddenAllWin_ToRx()
        {
            bool isOk = true;
            foreach (var item in ListRoamWines)
            {
                item.IsEnable = false;                
                item.RxInfo.Clear();
            }
            foreach (var item in ScreenRxInfos)
            {
                item.SumSubWins = 0;                
            }
            return isOk;
        }


        

        public bool Event_MoveWin_ToRx(HsMosaicWinInfo rw)
        {
            bool isOk = false;
            int id = rw.IdWin;
            if (rw.IdWin < 1 | rw.IdWin > ListRoamWines.Count)  //若超出Id范围，默认为无Id新开窗
            {
                return false;
            }
            HsMosaicWinInfo Win = ListRoamWines[id - 1];
            if (!Win.IsEnable)  //不显示的窗口返回false
            {
                return false;
            }

            //进行修复坐标，以防出现小格子长宽限制问题
            Rect newP = RectifyPostion(rw.Position, hsConfig.WinRectRate);
            if (newP.IsEmpty)
            {
                return false;
            }
            else
            {
                rw.X = (int)newP.X;
                rw.Y = (int)newP.Y;
                rw.Width = (int)newP.Width;
                rw.Height = (int)newP.Height;
            }

            isOk= CanMoveWin(Win, newP);
            if (isOk)
            {
                Win.X = rw.X;
                Win.Y = rw.Y;
                Win.Width = rw.Width;
                Win.Height = rw.Height;
                 
                rw.ZIndex = Win.ZIndex;
                rw.IdTx = Win.IdTx;
            }
            else
            {
                hsServer.ShowDebug("MoveWin_ToRx-移窗失败！");
            }
            return isOk;            
        }


        public bool Event_ModifyTxWin_ToRx(HsMosaicWinInfo rw)
        {
            bool isOk = false;
             
            if (rw.IdWin < 1 | rw.IdWin > ListRoamWines.Count)  //若超出Id范围，默认为无Id新开窗
            {
                return false;
            }
            HsMosaicWinInfo Win = ListRoamWines[rw.IdWin - 1];
            if (!Win.IsEnable)  //不显示的窗口返回false
            {
                return false;
            }
            
            rw.ZIndex = Win.ZIndex;
            rw.X = Win.X;
            rw.Y = Win.Y;
            rw.Width = Win.Width;
            rw.Height = Win.Height;

            return isOk;
        }


        public bool Event_LayMosaicWin_ToRx(HsMosaicWinInfo rw)
        {
            bool isOk = false;
            if (rw.IdWin < 1 | rw.IdWin > ListRoamWines.Count)  //若超出Id范围，返回错误
            {
                return false;
            }
            HsMosaicWinInfo Win = ListRoamWines[rw.IdWin - 1];
            if (!Win.IsEnable)  //不显示的窗口返回false
            {
                return false;
            }

            if (Win.ZIndex == rw.ZIndex)
            {
                rw.IdTx = Win.IdTx;
                rw.X = Win.X;
                rw.Y = Win.Y;
                rw.Width = Win.Width;
                rw.Height = Win.Height;
                hsServer.ShowDebug("当前窗口图层与要切换的图层一致，将忽略处理");
                return true;
            }
            int realZIndex = rw.ZIndex;

            if (realZIndex == int.MaxValue)
            {
                realZIndex = ListRoamWines.Count(f => f.IsEnable);

            }
            if (realZIndex < 1)
            {
                realZIndex = 1;
            }


            int oldZIdenx = Win.ZIndex;
            hsServer.ShowDebug("当前窗口需切换到的层次值切换 Id：" + rw.IdWin + ":" + oldZIdenx + " >> " + realZIndex);


            if (Win.ZIndex != realZIndex)
            {
                if (Win.ZIndex < realZIndex)   //层次往上移 1 2 3 4 5 6   例子 2》5
                {
                    foreach (var item in ListRoamWines)
                    {
                        if (!item.IsEnable)
                        {
                            continue;
                        }
                        if (item.ZIndex > Win.ZIndex && item.ZIndex <= realZIndex)
                        {
                            item.ZIndex--;
                        }
                    }
                }
                else if (Win.ZIndex > realZIndex) //层次往下移 1 2 3 4 5 6   例子 5》2
                {
                    foreach (var item in ListRoamWines)
                    {
                        if (!item.IsEnable)
                        {
                            continue;
                        }
                        if (item.ZIndex < Win.ZIndex && item.ZIndex >= realZIndex)
                        {
                            item.ZIndex++;
                        }
                    }
                }
                hsServer.ShowDebug("SynchroHsWallWinModify-同步进行层次切换 Id：" + rw.IdWin + ":" + oldZIdenx + ">>" + realZIndex);
                Win.ZIndex = realZIndex;
                
                rw.X = Win.X;
                rw.Y = Win.Y;
                rw.Width = Win.Width;
                rw.Height = Win.Height;
                rw.IdTx = Win.IdTx;
                rw.ZIndex = realZIndex;
            }
            foreach (var item in ListRoamWines)
            {
                if (item.IsEnable)
                {
                    hsServer.ShowDebug("窗口Id--图层最新信息：" + item.IdWin + " -- " + item.ZIndex);
                }
            }
            return isOk;
        }

        //-------------场景相关
       
        public  void Event_GetCurrentWins(HsScreenInfo screenInfo)
        {
            if (WinShowType == WinShowType.WinGrid)
            {

            }
            else if (WinShowType == WinShowType.WinResolution)
            {

            }
            else if (WinShowType == WinShowType.WinRoam)
            {
                screenInfo.IdScreen = IdScreen;
                screenInfo.Name = Name;
                screenInfo.StartX = StartX;
                screenInfo.StartY = StartY;
                screenInfo.WallPixW = WallPixW;
                screenInfo.WallPixH = WallPixH;
                screenInfo.Rows = Rows;
                screenInfo.Columns = Columns;
                screenInfo.UnitWidth = UnitWidth;
                screenInfo.UnitHeight = UnitHeight;
                foreach (var item in ListRoamWines)
                {
                    if (item.IsEnable)
                    {
                        screenInfo.CurrentWins.Add(item);
                    }
                }
            }
        }
        
        public void Event_GetScenes(HsScreenInfo screenInfo)
        {
            if (WinShowType == WinShowType.WinGrid)
            {

            }
            else if (WinShowType == WinShowType.WinResolution)
            {

            }
            else if (WinShowType == WinShowType.WinRoam)
            {
                foreach (var item in Scenes)
                {
                    screenInfo.Scenes.Add(item);
                }
            }
        }
        
        public bool Event_CallScene(string viewMode)
        {
            if (string.IsNullOrEmpty(viewMode))
            {
                hsServer.ShowDebug("CallScene-拼接模式参数为nullOrEmpty");
                return false;
            }
            bool isOk = true;
            SceneInfo scene = null;
            foreach (var item in Scenes)
            {
                if (item.Name == viewMode)
                {
                    scene = item;
                    break;
                }
            }
            if (scene != null)
            {                
                SceneLayoutAndPlay(scene.ListWins);
            }
            else
            {
                isOk = false;
                hsServer.ShowDebug("CallScene方法中大屏没有包含场景名称：" + viewMode);
            }
            return isOk;
        }

       
        public bool Event_SaveScene(SceneInfo sceneInfo)
        {
            bool isOk = true;
            string sceneName = sceneInfo.Name;

            if (string.IsNullOrEmpty(sceneName))
            {
                hsServer.ShowDebug("CallViewMode拼接模式参数为nullOrEmpty");
                return false;
            }
            foreach (var item in Scenes)
            {
                if (item.Name==sceneName)
                {
                    hsServer.ShowDebug("Event_SaveScene-有重名");
                    return false;
                }
            }

            #region  改为由服务器返回统一的数据来保持同一            
            SceneInfo newScene = new SceneInfo();
            //newScene.ParentScreen = this;
            newScene.IdScreen = IdScreen;
            newScene.Name = sceneName;
            newScene.WidthScreen = WallPixW;
            newScene.HeightScreen = WallPixH;
            foreach (var item in ListRoamWines)
            {
                if (item.IsEnable)
                {
                    HsMosaicWinInfo rw = new HsMosaicWinInfo()
                    {
                        IdWin = item.IdWin,
                        IdTx = item.IdTx,
                        ZIndex = item.ZIndex,
                        X = item.X,
                        Y = item.Y,
                        Width = item.Width,
                        Height = item.Height,
                        IsEnable = item.IsEnable
                    };
                    newScene.ListWins.Add(rw); 
                }
            }
            
            Scenes.Add(newScene);
            

            sceneInfo.Name = newScene.Name;
            sceneInfo.IdScreen = newScene.IdScreen;
            sceneInfo.WidthScreen = newScene.WidthScreen;
            sceneInfo.HeightScreen = newScene.HeightScreen;
            sceneInfo.ListWins = newScene.ListWins;
            sceneInfo.IdScene = newScene.IdScene; 
            
            return isOk;
        }
       
        public bool Event_ModifySceneName(string originName, string newName)
        {
            bool isOk = true;
            SceneInfo scene = null;
            foreach (var item in Scenes)
            {
                if (item.Name == originName)
                {
                    scene = item;
                    break;
                }
            }
            if (scene != null)
            {                
                scene.Name = newName;
            }
            else
            {
                isOk = false;
            }

            return isOk;
        }
      
        public bool Event_DeleteScene(string sceneName)
        {
            bool isOk = true;
            SceneInfo scene = null;
            foreach (var item in Scenes)
            {
                if (item.Name == sceneName)
                {
                    scene = item;
                    break;
                }
            }
            if (scene != null)
            {
                Scenes.Remove(scene); 
            }
            else
            {
                hsServer.ShowDebug("DeleteScene-当前大屏的场景列表无此场景名称：" + sceneName);
            }

            return isOk;
        }

        #region 预案相关-----------
       
        public bool Event_CreatePrePlan(PrePlanInfo info)
        {
            bool isOk = true;
            foreach (var item in PrePlans)
            {
                if (info.Name==item.Name)
                {
                    isOk = false;
                    return false;
                }
            }
            foreach (var item in info.SceneNames)
            {
                List<SceneInfo> list = Scenes.ToList();
                if (list.Find(f=>f.Name==item)==null)
                {
                    return false;  //没有对应场景名称的场景
                }
            }
            if (isOk)
            {                 
                PrePlans.Add(info);
            }
            return isOk;
        }
        public bool Event_ModifyPrePlan(PrePlanInfo info)
        {
            bool isOk = true;
            PrePlanInfo pp = null;
            foreach (var item in PrePlans)
            {
                if (info.Name == item.Name)
                {
                    pp = item;
                    break;
                }
            }
            if (pp==null)
            {
                hsServer.ShowDebug("Event_ModifyPrePlan-未找到对应预案名称："+info.Name);
                return false;
            }
            else
            {
                List<SceneInfo> list = Scenes.ToList();

                foreach (var item in info.SceneNames)
                {
                    if (list.Find(f => f.Name == item) == null)
                    {
                        return false;  //没有对应场景名称的场景
                    }
                }
                if (isOk)
                {
                    PrePlans.Remove(pp);
                    
                    PrePlans.Add(info);
                }                
            }         
            
            return isOk;
        }
        
        public bool Event_DeletePrePlan(string name)
        {
            bool isOk = true;
            PrePlanInfo pp = null;
            foreach (var item in PrePlans)
            {
                if (item.Name == name)
                {
                    pp = item;
                    break;
                }
            }
            PrePlans.Remove(pp);
            if (pp != null)
            {
                
            }
            else
            {
                hsServer.ShowDebug("DeletePrePlan-当前大屏的预案列表无此名称：" + name);
            }

            return isOk;
        }
       
        public bool Event_ModifyPrePlanName(string originName, string newName)
        {
            bool isOk = true;
            PrePlanInfo pp = null;
            foreach (var item in PrePlans)
            {
                if (item.Name == originName)
                {
                    pp = item;
                    break;
                }
            }
            if (pp != null)
            {
                pp.Name = newName;
            }
            else
            {
                isOk = false;
            }

            return isOk;
        }
        //获取当前大屏下的预案信息
        public void Event_GetPrePlan(HsScreenInfo screenInfo)
        {
            screenInfo.IdScreen = IdScreen;
            foreach (var item in PrePlans)
            {
                screenInfo.PrePlans.Add(item);
            }
        }
        #endregion
        #endregion

        

        #region MosaicServerFrom 事件相关调用方法-----------


        #endregion

        #endregion

        #endregion

    }
    public enum MoveMode
    {
        Default=0,
        OverLay=1,
        ResumeSize=2,
        Custom = 3   //自定义输入物理大屏坐标，而移动窗口位置
    }
}
