
using Rx;
using System;
using System.Collections.Generic;
using System.Text;
using Tx;

namespace AbstractInterFace
{
    public interface IMosaicServerToRx
    {
        bool SetRxesBind(List<RxInfo> rxinfos);
        bool Win_Open(int id, int x, int y, int w, int h, TxInfo txInfo);
        bool Win_Move(int id, int x, int y, int w, int h);
        bool Win_Lay(int id, int zIndex);
        bool Win_Swi(int id, TxInfo txInfo);
        bool Win_Close(int id);
        bool Win_Clear();
    }
}
