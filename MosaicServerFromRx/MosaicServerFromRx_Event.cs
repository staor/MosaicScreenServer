
using System;
using System.Collections.Generic;
using System.Text;

namespace MosaicServerFromRx
{
    public class MosaicServerFromRx_Event
    {
        //从组播中获取RxInfo列表
        public Action<Json_Rec_Hs_Cmd_End, RxInfo> Event_NewMosaicWall;
    }
}
