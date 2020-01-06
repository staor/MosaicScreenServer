
using AbstractInterFace;
using ConfigServer;
using LoggerHelper;
using Rx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Tx;
using Utils;


namespace MosaicServerToRx
{
    public delegate void ShowDebugTextDelegate(string arg);
    class MosaicServerToRx : IMosaicServerToRx
    {
        List<Win_Info> ListRoamWines { get; set; } = new List<Win_Info>();
        List<ScreenSub> ListScreenSub { get; set; } = new List<ScreenSub>();
        public List<RxInfo> ListRxInfo { get; set; } = new List<RxInfo>();
        //public List<TxInfo> ListTxInfo { get; set; } = new List<TxInfo>();
        public int Rows { get; set; }
        public int Columns { get; set; }
        public int LimitWin { get; set; } = 4;
        public Rect RectUnit { get; set; }
        public ShowDebugTextDelegate ShowDebugTextEvent { get; set; }

        public MosaicServerToRx(int columns, int rows, int unitW,int unitH, List<RxInfo> rxInfos)
        {
            foreach (var item in rxInfos)
            {
                ListRxInfo.Add(item);
            }
            //ListRxInfo = rxInfos;
            Columns = columns;
            Rows = rows;
            RectUnit = new Rect(0, 0, unitW, unitH);
            Load();

        }
        //public MosaicServerToRx(HsScreenInfo screenInfo)
        //{
        //    ListRoamWines = new List<Win_Info>();
        //}
        public bool Load()
        {
            
            bool isOk = true;
            if (Rows * Columns != ListRxInfo.Count)
            {
                Logger.CWshow("MosaicRoam中行列数与Rxes数量不一致(可能为Rx未绑定)：" + Rows + " x " + Columns + " != " + ListRxInfo.Count);
                return false;
            }
            if (Rows<1|Columns<1| RectUnit.Width<=0|RectUnit.Height<=0)
            {
                Logger.CWshow("MosaicRoam中行列/长宽数值<1：");
                return false;
            }
            int sumRoamWin = Rows * Columns * LimitWin;
            for (int i = 0; i < sumRoamWin; i++)
            {
                Win_Info rw = new Win_Info();
                rw.IdWin = i + 1;
                rw.IsEnable = false;
                ListRoamWines.Add(rw);
            }
            for (int i = 0; i < ListRxInfo.Count; i++)
            {
                RxInfo rxId = ListRxInfo[i];
                ScreenSub ss = new ScreenSub();
                ss.Rx4 = rxId;
                rxId.ActionOnline += ss.UpdateRxInfoWins;
                ss.Index = i;
                ss.Column = i % Columns;
                ss.Row = i / Columns;
                ss.RowSpan = 1;
                ss.ColumnSpan = 1;
                ss.RectScreenSub = new Rect(ss.Column * RectUnit.Width, ss.Row * RectUnit.Height, RectUnit.Width, RectUnit.Height);// 此方框相对物理大屏分辨率而言..........     
                ss.RectDefaultSize = RectUnit;
                for (int m = 0; m < LimitWin; m++)
                {
                    GridSub gs = new GridSub();
                    gs.Index = m;
                    string strNo = "tx1";
                    if (m == 1)
                    {
                        strNo = "tx2";
                    }
                    else if (m == 2)
                    {
                        strNo = "tx3";
                    }
                    else if (m == 3)
                    {
                        strNo = "tx4";
                    }
                    gs.tx = strNo;
                    gs.ParentScreenSub = ss;

                    ss.ListGridSub.Add(gs);
                }
                ListScreenSub.Add(ss);
            }
            return isOk;
        }

       


        #region 接口--------------
        public bool Win_Clear()
        {
            bool isOk = true;
            foreach (var item in ListRoamWines)
            {
                item.IsEnable = false;
                //item.Visibility = Visibility.Hidden;
            }
            foreach (var item in ListScreenSub)
            {
                ServerScreenSubHelper.HS_CloseAllPlayWin(item.Rx4.Ip4, item.Rx4.Port4);
                foreach (var gridSub in item.ListGridSub)
                {
                    gridSub.IsEnable = false;
                }
            }
            return isOk;
        }
        public bool Win_Close(int id)
        {
            bool isOk = true;
            if (id < 1 | id > ListRoamWines.Count)
            {
                Logger.CWshow($"Win_Close -窗口Id:{id} 不在范围内：1 - {ListRoamWines.Count}");
                return false;
            }
            Win_Info rw = ListRoamWines[id - 1];
            if (rw.IsEnable)
            {
                rw.IsEnable = false;
                foreach (var item in ListRoamWines)  //RoamWin在关闭窗口时，大于此的优先级都减1；以便区别及统计唯一的层次
                {
                    if (item.IsEnable && item.ZIndex > rw.ZIndex)
                    {
                        item.ZIndex--;
                    }
                }
                foreach (var gs in rw.ListGridSub)
                {
                    ScreenSub ss = gs.ParentScreenSub;
                    gs.IsEnable = false;

                    foreach (var gsOther in ss.ListGridSub)
                    {                         
                        if (gsOther.IsEnable && gsOther.ZIndex < gs.ZIndex)
                        {
                            gsOther.ZIndex++; //不需要发送切换叠层数指令，Rx4自动设置后续高层数下降
                        }
                    }
                    ServerScreenSubHelper.HS_OpenOrClose_PlayWin(gs, ss.Rx4.Ip4, ss.Rx4.Port4);
                }
            }
            else
            {
                Logger.CWshow("当前窗口已关闭。。。-"+ id);
            }
            ShowWinInfo();
            return isOk;
        }
        public bool Win_Lay(int id, int zIndex)  //<1 表示最底层 >当前最大成 则为最上层 
        {
            bool isOk = true;
            //RoamWin rw = ListRoamWines.Find(f => f.Id == idWin);
            if (id < 1 && id > ListRoamWines.Count)
            {
                Logger.CWshow("CloseWin中窗口Id超出列表索引范围，返回失败-1：" + id);
                return false;
            }
            Win_Info realWin = ListRoamWines[id - 1];
            if (realWin.IsEnable)
            {
                if (realWin.ZIndex == zIndex) //若层数一样，则忽略  //开窗时候是否设置rw.ZIndex！=0？？
                {
                    Logger.CWshow("MosaicRoam.ModifyWinZIndex中切换的zIndex相同，将忽略处理：" + zIndex);
                    return true;
                }
                int currentMaxLay = ListRoamWines.Count(c => c.IsEnable);
                if (zIndex >= currentMaxLay)  //默认为最上层
                {
                    foreach (var gs in realWin.ListGridSub)
                    {
                        if (gs.ZIndex == LimitWin - 1)
                        {
                            Logger.CWshow("当前小框已经为最上层Proirity：" + gs.tx + ":" + gs.ZIndex);
                        }
                        else
                        {
                            foreach (var gsOther in gs.ParentScreenSub.ListGridSub)
                            {
                                if (gsOther.IsEnable && gsOther.ZIndex > gs.ZIndex)
                                {
                                    gsOther.ZIndex--;
                                }
                            }

                            gs.ZIndex = LimitWin - 1;
                            ServerScreenSubHelper.HS_ModifyZIndex_PlayWin(gs, gs.ParentScreenSub.Rx4.Ip4, gs.ParentScreenSub.Rx4.Port4);
                        }
                    }
                    foreach (var item in ListRoamWines)
                    {
                        if (item.IsEnable&&item.ZIndex>realWin.ZIndex)
                        {
                            item.ZIndex--;
                        }
                    }
                    realWin.ZIndex = currentMaxLay;
                }
                else if (zIndex <= 1)
                {
                    foreach (var gs in realWin.ListGridSub)
                    {
                        //ShowGridSub(gs.ParentScreenSub);
                        int lowerLay = gs.ZIndex;  //从最上层往上确认哪层 //改为从当前层确认，以防本来在底层，错误的设置为3
                        foreach (var gsOther in gs.ParentScreenSub.ListGridSub)
                        {
                            if (gsOther.IsEnable && gsOther.ZIndex < gs.ZIndex && gsOther.tx != gs.tx)
                            {
                                if (lowerLay>gsOther.ZIndex)
                                {
                                    lowerLay = gsOther.ZIndex;
                                }
                                gsOther.ZIndex++;
                            }
                        }

                        gs.ZIndex = lowerLay;
                        ServerScreenSubHelper.HS_ModifyZIndex_PlayWin(gs, gs.ParentScreenSub.Rx4.Ip4, gs.ParentScreenSub.Rx4.Port4);
                        //ShowGridSub(gs.ParentScreenSub);
                    }

                    foreach (var item in ListRoamWines)
                    {
                        if (item.IsEnable && item.ZIndex < realWin.ZIndex)
                        {
                            item.ZIndex++;
                        }
                    }
                    realWin.ZIndex = 1;
                }
                else 
                {
                    if (realWin.ZIndex < zIndex)  //往上移
                    {
                        foreach (var gs in realWin.ListGridSub)
                        {
                            int newLay = gs.ZIndex;
                            foreach (var gsOther in gs.ParentScreenSub.ListGridSub)
                            {
                                if (gsOther.IsEnable && gsOther.Win.ZIndex > realWin.ZIndex && gsOther.Win.ZIndex <= zIndex)
                                {
                                    if (newLay < gsOther.ZIndex)
                                    {
                                        newLay = gsOther.ZIndex;
                                    }
                                    gsOther.ZIndex--;
                                }
                            }
                            gs.ZIndex = newLay;
                            ServerScreenSubHelper.HS_ModifyZIndex_PlayWin(gs, gs.ParentScreenSub.Rx4.Ip4, gs.ParentScreenSub.Rx4.Port4);
                        }
                        foreach (var item in ListRoamWines)
                        {
                            if (item.IsEnable && item.ZIndex > realWin.ZIndex && item.ZIndex <= zIndex)
                            {
                                item.ZIndex--;
                            }
                        }
                    }
                    else if (realWin.ZIndex > zIndex) //往下移
                    {
                        foreach (var gs in realWin.ListGridSub)
                        {
                            int newLay = gs.ZIndex;
                            foreach (var gsOther in gs.ParentScreenSub.ListGridSub)
                            {
                                if (gsOther.IsEnable && gsOther.Win.ZIndex < realWin.ZIndex && gsOther.Win.ZIndex >= zIndex)
                                {
                                    if (newLay > gsOther.ZIndex)
                                    {
                                        newLay = gsOther.ZIndex;
                                    }
                                    gsOther.ZIndex++;
                                }
                            }
                            gs.ZIndex = newLay;
                            ServerScreenSubHelper.HS_ModifyZIndex_PlayWin(gs, gs.ParentScreenSub.Rx4.Ip4, gs.ParentScreenSub.Rx4.Port4);
                        }
                        foreach (var item in ListRoamWines)
                        {
                            if (item.IsEnable && item.ZIndex < realWin.ZIndex && item.ZIndex >= zIndex)
                            {
                                item.ZIndex++;
                            }
                        }
                    }
                    realWin.ZIndex = zIndex;
                }
            }
            else
            {
                Logger.CWshow("MosaicRoam.ModifyWinZIndex中方法的窗口状态时关闭的Id：" + id);
            }
            ShowWinInfo();
            return isOk;
        }
        public bool Win_Move(int id, int x, int y, int w, int h)
        {
            bool isOk = true;
            if (id < 1 | id > ListRoamWines.Count)
            {
                Logger.CWshow($"Win_Move -窗口Id:{id} 不在范围内：1 - {ListRoamWines.Count}");
                return false;
            }

            Win_Info realWin = ListRoamWines[id - 1];
            Rect newRect = new Rect(x, y, w, h);
            if (realWin.IsEnable)
            {
                Win_Info tempWin = new Win_Info();  //此处使用相同坐标进行试探可否开窗
                tempWin.IdWin = realWin.IdWin;
                tempWin.TxInfo = realWin.TxInfo;
                if (realWin.Position == newRect)
                {
                    Logger.CWshow("MoveWin方法中的新旧位置相同，将过滤不处理："+newRect);
                    return true;
                }
                tempWin.X = x;
                tempWin.Y = y;
                tempWin.W = w;
                tempWin.H = h;
                tempWin.ZIndex = realWin.ZIndex;
                //foreach (var item in rw.ListGridSub) //临时设置可以关闭掉
                //{
                //    item.IsEnable = false;
                //}

                List<KeyValuePair<int, GridSub>> listKVP = GetKeyValueGridSub(tempWin, ListScreenSub);
                if (listKVP.Count == 0)
                {
                    //foreach (var item in realWin.ListGridSub)  //若发现目的窗口位置不能新建，返回设置恢复原状土
                    //{
                    //    //item.IsEnable = true;
                    //    item.IsReplace = false;  //在这里还原可能设置有的IsReplace=true的情况。
                    //}
                    //realWin.X = tempWin.X;
                    //realWin.Y = tempWin.Y;
                    //realWin.W = tempWin.W;
                    //realWin.H = tempWin.H;
                    isOk = false;
                    Logger.CWshow("MoveWin当前操作窗口时，单元屏拼接窗口格子的重叠数量超过上限4或其长宽数过小.将不能移动！");
                    return isOk;
                }
                else  //移动成功 ，设置为新位置
                {
                    realWin.X = tempWin.X;
                    realWin.Y = tempWin.Y;
                    realWin.W = tempWin.W;
                    realWin.H = tempWin.H;

                    foreach (var oldGS in realWin.ListGridSub)
                    {
                        if (!oldGS.IsReplace)  //不是分类为移动窗口的，需要进行关闭隐藏  
                        {
                            oldGS.IsEnable = false;

                            ServerScreenSubHelper.HS_OpenOrClose_PlayWin(oldGS, oldGS.ParentScreenSub.Rx4.Ip4, oldGS.ParentScreenSub.Rx4.Port4);
                            foreach (var item in oldGS.ParentScreenSub.ListGridSub)  //若移出小屏，则小屏中的其他格子（小于当前层的格子）的层次+1
                            {
                                if (item.IsEnable && item.Win.ZIndex < realWin.ZIndex)
                                {
                                    item.ZIndex++;
                                }
                            }
                        }
                        else
                        {
                            oldGS.IsReplace = false;
                        }
                    }
                    realWin.ListGridSub.Clear();

                    foreach (var kvp in listKVP)
                    {
                        ScreenSub ss = kvp.Value.ParentScreenSub;
                        GridSub newGridSub = kvp.Value;  //newGridSub为新建的一个小格子，与大屏里面的格子没有关系，除了值有部分一样

                        if (!newGridSub.IsReplace)  //表示新进来指定层次的小格子的时候  
                        {
                            //ShowGridSub(ss);
                            foreach (var gsOther in ss.ListGridSub)
                            {
                                if (gsOther.IsEnable && gsOther.tx != newGridSub.tx)  //表示已活动的且不是同一个小格子  
                                {
                                    if (newGridSub.Win.ZIndex < gsOther.Win.ZIndex)  //原大窗口124  移来3  原小格子123  移来需变成2   //newGridSub.Win为复制真实Win的层次值
                                    {
                                        if (newGridSub.ZIndex >= gsOther.ZIndex)
                                        {
                                            newGridSub.ZIndex = gsOther.ZIndex - 1;
                                            if (newGridSub.ZIndex < 0)
                                            {
                                                newGridSub.ZIndex = 0;
                                                Logger.CWshow("MoveWin-减小格子层次<0?：比较对方小格子层次-" + gsOther.ZIndex);
                                            }
                                        }
                                    }
                                    else if (newGridSub.Win.ZIndex > gsOther.Win.ZIndex) //大窗口层次比新来的层次低，则对应小格子层次都-1
                                    {
                                        if (newGridSub.ZIndex<gsOther.ZIndex)
                                        {
                                            newGridSub.ZIndex = gsOther.ZIndex;
                                        }
                                        gsOther.ZIndex--;
                                    }
                                    else
                                    {
                                        Logger.CWshow("MoveWin-有大窗口层次一样的？：" + newGridSub.Win.ZIndex);
                                    }
                                }
                            }
                            
                        }
                        
                        GridSub oldGS = ss.ListGridSub[kvp.Key];
                        oldGS.CurrentTx = newGridSub.CurrentTx;
                        oldGS.IdWin = newGridSub.IdWin;
                        oldGS.IsEnable = true;
                        oldGS.IsReplace = false;
                        oldGS.RectWin = newGridSub.RectWin;
                        oldGS.RectCrop = newGridSub.RectCrop;
                        oldGS.ZIndex = newGridSub.ZIndex;
                        oldGS.Win = realWin;  //需要为真实的窗口引用

                        realWin.ListGridSub.Add(oldGS);
                        ServerScreenSubHelper.HS_OpenOrClose_PlayWin(oldGS, ss.Rx4.Ip4, ss.Rx4.Port4);

                        //ShowGridSub(oldGS.ParentScreenSub);
                    }


                }               
            }
            else
            {
                isOk = false;
                Logger.CWshow("当前窗口已经关闭：" + id);
            }
            ShowWinInfo();
            return isOk;
        }
        public bool Win_Open(int id, int x, int y, int w, int h, TxInfo txInfo)
        {
            bool isOk = false;
            if (id<1|id>ListRoamWines.Count)
            {
                Logger.CWshow($"Win_Open -窗口Id:{id} 不在范围内：1 - {ListRoamWines.Count}");
                return false;
            }
            
            Win_Info realWin = ListRoamWines[id - 1];
            TxInfo oldTx = realWin.TxInfo;

            if (realWin.IsEnable)
            {
                Logger.CWshow("Win_Open -在新建已有的窗口，默认转为执行为移动此一样Id的窗口");
                //return false;
            }
            else
            {
                realWin.IsEnable = true;
            }
            Win_Info tempInfo = new Win_Info()
            {
                IdWin = realWin.IdWin,
                ZIndex = ListRoamWines.Count(c => c.IsEnable),
                TxInfo = txInfo,
                X = x,
                Y = y,
                W = w,
                H = h
            };

            
            //rw.ZIndex = zIndex ;  //层数为自然层，最大为
            isOk = CanNewWin(tempInfo); //逻辑开窗成功。。。
            if (isOk)
            {
                realWin.X = x;
                realWin.Y = y;
                realWin.W = w;
                realWin.H = h;
                realWin.ZIndex = tempInfo.ZIndex;
                realWin.TxInfo = txInfo;
            }
            else
            {
                realWin.IsEnable = false;
            }
            ShowWinInfo();
            return isOk;
        }

        public bool Win_Swi(int id, TxInfo txInfo)
        {
            bool isOk = true;
            TxInfo currentTx = txInfo;
            if (id < 1 | id > ListRoamWines.Count)
            {
                Logger.CWshow($"Win_Swi -窗口Id:{id} 不在范围内：1 - {ListRoamWines.Count}");
                return false;
            }
            Win_Info rw = ListRoamWines[id - 1];
            rw.TxInfo = currentTx;
            foreach (var item in rw.ListGridSub)
            {
                item.CurrentTx = currentTx;
                ServerScreenSubHelper.HS_ModifyTx_PlayWin(item, item.ParentScreenSub.Rx4.Ip4, item.ParentScreenSub.Rx4.Port4);
            }

            return isOk;
        }

        

        #endregion
        private void ShowWinInfo()
        {
            return;
            StringBuilder sb = new StringBuilder();
            string splite1 = "--";
            string splite2 = " | ";
            foreach (var item in ListRoamWines)
            {
               
                sb.Append(item.IdWin);
                sb.Append(splite1);
                sb.Append(item.IsEnable);
                sb.Append(splite1);
                sb.Append(item.ZIndex);
                sb.Append(splite2);
            }
            Logger.CWshow("RxInfo图层信息(idWin-IsEnable-Win.Zindex："+sb.ToString());
        }
        private void ShowGridSub(ScreenSub sub)
        {
            if (sub == null||sub.ListGridSub==null)
            {
                Logger.CWshow("ShowGridSub-参数ScreenSub为空！");
                return;
            }
            StringBuilder sb = new StringBuilder();
            string split = "-";
            string split2 = ":";
            foreach (var item in sub.ListGridSub)
            {
                sb.Append(item.tx);
                sb.Append(split);
                sb.Append(item.ZIndex);
                sb.Append(split);
                if (item.Win!=null)
                {
                    sb.Append(item.Win.IdWin);
                    sb.Append(split2);
                    sb.Append(item.Win.ZIndex);
                }
                sb.Append(split);
            }
            Logger.CWshow("ShowGridSub-显示ScreenSub.Id(tx-gs.Zindex-id:"+sub.Rx4.Ip4+"--"+sb.ToString());
        }
        

        public bool CanNewWin(Win_Info rw, bool isOpen = true)
        {
            bool isOk = true;
            List<KeyValuePair<int, GridSub>> list = GetKeyValueGridSub(rw, ListScreenSub);
            if (list.Count == 0)  //无法新开窗
            {
                //rw.IsEnable = false;  //无需对此值更改，以防这是对已有窗口进行移窗。
                isOk = false;
                Logger.CWshow("CanNewWin中目标位置无法创建新窗口：" + rw.IdWin + "|x0=" + rw.X + " y0=" + rw.Y + " w=" + rw.W + " h=" + rw.H);
            }
            if (isOk && isOpen)//有空的位置可以放置GridSub，且正式发开窗指令
            {
                Win_Info realWin = ListRoamWines[rw.IdWin - 1];
                realWin.IsEnable = true;
                realWin.ListGridSub.Clear();

                //rw.IsEnable = true;                
                //rw.ListGridSub.Clear();
                foreach (var kvp in list)
                {
                    ScreenSub ss = kvp.Value.ParentScreenSub;
                    GridSub newGridSub = kvp.Value;                    

                    //GridSub temp = ss.ListGridSub.Find(f => f.IsEnable && f.ZIndex == LimitWin - 1);
                    //if (temp != null)   //若有最上层的活动小框，则新开窗促使之前的小框 的层次全部-1
                    //{
                    //    foreach (var gsOther in ss.ListGridSub)
                    //    {
                    //        if (gsOther.IsEnable)
                    //        {
                    //            gsOther.ZIndex--;
                    //        }
                    //    }
                    //}
                    //ss.ListGridSub[kvp.Key] = kvp.Value; //目前按照 索引排序来确定被取代的GridSub
                    GridSub realGS = ss.ListGridSub[kvp.Key];  //真正的原始GridSub
                    if (!newGridSub.IsReplace)  //新建的
                    {
                        foreach (var gsOther in ss.ListGridSub)
                        {
                            if (gsOther.IsEnable && gsOther != realGS)
                            {
                                gsOther.ZIndex--;
                            }
                        }
                    }
                    else
                    {
                        foreach (var gsOther in ss.ListGridSub)
                        {
                            if (gsOther.IsEnable&&gsOther.ZIndex>realGS.ZIndex)
                            {
                                gsOther.ZIndex--;
                            }
                        }
                    }
                    realGS.ZIndex = LimitWin - 1;  //默认不管之前在哪一层，都移至最上层


                    realGS.CurrentTx = newGridSub.CurrentTx;
                    realGS.IdWin = newGridSub.IdWin;
                    realGS.IsEnable = true;
                    realGS.IsReplace = false;
                    realGS.RectWin = newGridSub.RectWin;
                    realGS.RectCrop = newGridSub.RectCrop;
                    realGS.Win = realWin;

                    realWin.ListGridSub.Add(realGS);
                    ServerScreenSubHelper.HS_OpenOrClose_PlayWin(realGS, ss.Rx4.Ip4, ss.Rx4.Port4);

                }
            }
            return isOk;
        }
        private List<KeyValuePair<int, GridSub>> GetKeyValueGridSub(Win_Info rw, List<ScreenSub> grids)
        {
            List<KeyValuePair<int, GridSub>> list = new List<KeyValuePair<int, GridSub>>();
            Rect rectWinCurrent = rw.Position;  //预防多线程改值
            foreach (var ss in grids)
            {
                Rect rectWin = Rect.Intersect(rectWinCurrent, ss.RectScreenSub); //两个矩形的交集；如果不存在任何交集，则为 Rect.Empty。  msdn描述错误，实际则为with<=0;
                if (rectWin.Width > 0 && rectWin.Height > 0)  //裁剪小方框的最新尺寸为宽>=128 高>=72 
                {
                    if (rectWin.Width < 128 | rectWin.Height < 72)
                    {
                        ShowDebugTextEvent?.BeginInvoke(hsConfig.strRoamWinLimitSize, null, null);
                        Logger.CWshow("有Rx4底格子的窗口叠加列表中小窗口最小尺寸不符合要求：（限制为w>=128&&h>=72）当前：" + rectWin);
                        return new List<KeyValuePair<int, GridSub>>();
                    }
                    GridSub gs = new GridSub();
                    GridSub oldGS = null;
                    bool isNewZIndex = true;  //表示小格子是否新开
                    foreach (var item in ss.ListGridSub)
                    {
                        if (item.IdWin == rw.IdWin && item.IsEnable)  //此为将要移动的在此处的新的坐标小框，优先处理
                        {
                            oldGS = item;
                            isNewZIndex = false;
                            break;
                        }
                        if (oldGS == null && !item.IsEnable)  //获取第一个有空闲的。
                        {
                            //item.IsEnable = true;
                            oldGS = item;
                        }
                    }
                    //GridSub oldGS= ss.ListGridSub.Find(f => !f.IsEnable);
                    if (oldGS == null)  //实际上对原始GS的值不变动
                    {
                        foreach (var itemKVP in list)  //还原之前被标识的恢复。
                        {
                            if (itemKVP.Value.IsReplace)
                            {
                                itemKVP.Value.IsReplace = false;
                            }
                        }
                        Logger.CWshow("有Rx4底格子的窗口叠加列表中无空闲的窗口（画面数量超过限制：" + LimitWin + "  --位于底格子索引：" + ss.Index);
                        ShowDebugTextEvent?.BeginInvoke(hsConfig.strOpenWinLimit, null, null);
                        //Application.Current.Dispatcher.BeginInvoke( System.Windows.Threading.DispatcherPriority.Normal,new ShowDebugTextDelegate(ShowDebugText),hsConfig.strOpenWinLimit);
                        return new List<KeyValuePair<int, GridSub>>();
                    }
                    //gs.Dev_id = rw.Id;
                    gs.ParentScreenSub = ss;
                    gs.CurrentTx = rw.TxInfo;
                    gs.IsEnable = true;
                    if (isNewZIndex)
                    {
                        gs.ZIndex = LimitWin - 1; // 叠加优先级0-3 值越大 优先级越高  //默认开窗/时为最上层
                        oldGS.IsReplace = false; //以防移走再移回
                        gs.IsReplace = false;
                    }
                    else
                    {
                        gs.ZIndex = oldGS.ZIndex; //若为移动缩放，则还是原来的层次
                        oldGS.IsReplace = true;  //标识此对象将被取代   //注意在最终未成功移动新位置后，设置原本要取代的标识要在上一层还原成IsReplace=false，否则影响后续的判断
                        gs.IsReplace = true;
                    }
                    gs.Index = oldGS.Index;
                    gs.tx = oldGS.tx;
                    gs.IdWin = rw.IdWin;
                    gs.Win = rw;

                    KeyValuePair<int, GridSub> kvp = new KeyValuePair<int, GridSub>(gs.Index, gs);

                    if (Convert.ToBoolean(rectWin.Width % 2))  //目前需要设置长宽值为偶数
                    {
                        rectWin.Width++;
                    }
                    if (Convert.ToBoolean(rectWin.Height % 2))
                    {
                        rectWin.Height++;
                    }
                    rectWin.X -= ss.RectScreenSub.X;
                    rectWin.Y -= ss.RectScreenSub.Y;
                    gs.RectWin = rectWin;
                    Rect temp = Rect.Intersect(ss.RectScreenSub, rectWinCurrent);
                    Rect temp2 = Rect.ConverterRect0ToRect1(temp, rectWinCurrent, rw.ImageDefaultResolution);  //有小数

                    if (Convert.ToBoolean((int)temp2.Width % 2))  //目前需要设置长宽值为偶数
                    {
                        temp2.Width++;
                    }
                    if (Convert.ToBoolean((int)temp2.Height % 2))
                    {
                        temp2.Height++;
                    }
                    gs.RectCrop = new Rect((int)temp2.X, (int)temp2.Y, (int)temp2.Width, (int)temp2.Height);

                    list.Add(kvp);
                }
                //else
                //{
                //    break;
                //}                
            }
            //if (list.Count == 0)  //优化为在所有小屏都开满窗口数特殊情况下，再次开窗的调用提示文本信息
            //{
            //    ShowDebugTextEvent?.BeginInvoke(hsConfig.strOpenWinLimit, null, null);
            //}
            return list;
        }

        public bool Initial()
        {
            return Load();
        }

        public bool SetRxesBind(List<RxInfo> rxinfos)
        {
            bool isOk = true;
            if (rxinfos.Count!=Rows*Columns)
            {
                return false;
            }
            ListRxInfo.Clear();
            for (int i = 0; i < rxinfos.Count; i++)
            {
                if (i<ListScreenSub.Count)
                {
                    if (ListScreenSub[i].Rx4!=null)
                    {
                        ListScreenSub[i].Rx4.ActionOnline -= ListScreenSub[i].UpdateRxInfoWins;
                    }
                    ListScreenSub[i].Rx4 = rxinfos[i];
                    rxinfos[i].ActionOnline += ListScreenSub[i].UpdateRxInfoWins;
                    ListRxInfo.Add(rxinfos[i]);
                }
            }
            return isOk;
        }
    }
}
