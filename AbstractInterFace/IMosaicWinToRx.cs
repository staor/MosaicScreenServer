

using RxNS;
using ScreenManagerNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxNS;

namespace AbstractInterFace
{
    public interface  IMosaicWinToRx
    {
        void WinModify(int idWall,List<HsMosaicWinInfo> infos,List<TxInfo> txInfos);
        void WinClose(int idWall,int idWin);
        void WallClear(int idWall);
        string BindRx(List<RxInfo> rxInfos, HsScreenInfo info);
        string UnBindRx(List<RxInfo> rxInfos,  HsScreenInfo info);
        string ReBindRx(List<RxInfo> oldRrxInfos, List<RxInfo> rxInfos, HsScreenInfo info);
        string BindOneRxInfo(List<RxInfo> rxInfos, RxInfo rxInfo, HsScreenInfo info);
    }
    //public class MosaicBindRxEvent
    //{
    //    //大屏相关-------------------------------
    //    public Action<Json_Rec_Hs_Cmd_End, HsScreenInfo, HsScreenInfo> Event_ReBindRx; //前者HsScreenInfo为原绑定的大屏，若无 则null，后者为新绑定的大屏
    //}


}
