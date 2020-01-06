
using AbstractInterFace;
using LoggerHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace TxNS
{
    public class Tx : AConfig
    {
        private string _text;
        private static Dictionary<string, Tx> _dicTx;

        TxInfo _txInfo;
        public TxInfo TxInfo
        {
            get => _txInfo;
        }
        public string Text  //额外补充自定义名称，若没有自定义这为服务器里设置的Tx名称
        {
            get
            {
                if (_text == null)
                {
                    return Name;
                }
                else
                {
                    return _text;
                }
            }
            set
            {
                if (value != _text)
                {
                    _text = value;
                    //NotifyPropertyChanged("Text");
                }
            }
        }


        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value)
                {
                    return;
                }
                _isSelected = value;
                //NotifyPropertyChanged(nameof(IsSelected));
            }
        }
        //private static Tx _selectedTx;
        public static Tx SelectedTx { get; set; }
        //{
        //    get => _selectedTx;
        //    set
        //    {
        //        if (_selectedTx == value)
        //        {
        //            return;
        //        }
        //        _selectedTx = value;
        //    }
        //}


        public string MultiIp { get; set; }
        public int MultiPort { get; set; }
        public bool IsTx2 { get; set; }
        public string VideoIp { get; set; }
        public int VideoPort { get; set; }
        public static readonly string PreIp = "225.225.100."; 
        public static int PrePort = 32004;
        public string Stream { get; set; }  //对应双流Tx的rtsp地址
        public string Rate { get; set; }  //码率
        public string EncodeFormat { get; set; } = "H264"; //编码格式
        public static Dictionary<string, Tx> DicTx
        {
            get
            {
                if (_dicTx == null)
                {
                    _dicTx = new Dictionary<string, Tx>();
                }
                return _dicTx;
            }
        }
        public Tx()
        {
            Id = "0000";
            Name = "";
        }

        public Tx(TxInfo info)
        {
            _txInfo = info;
            Id = info.Id;
            Name = info.Name;
            //未完成
            if (!Tx.DicTx.ContainsKey(info.Id))
            {                
                Tx.DicTx.Add(info.Id, this);
            }
        }
        /// <summary>
        /// 根据关键字id值，加入Tx.DicTx中
        /// </summary>
        /// <param name="tx"></param>
        public Tx(string tx)
        {
            Id = "0000";
            Name = "";
            if (tx != null && tx != "")
            {
                Id = tx;
                int ip;
                if (!int.TryParse(tx, out ip))
                {
                    Logger.Error("Txid不能转化为整数,请检查！");
                }
                else
                {
                    if (ip > 300)  //嵌入式设定规则：4K分布式的Tx的id>300 例子0304 其ip为169.254.200.4  组播ip为225.225.200.4；port:32004
                    {
                        MultiIp = "225.225.200." + (ip - 300);
                    }
                    else
                    {
                        MultiIp = PreIp + ip;  //ip：169.254.105.11  对应id：5011  105截取5  11》011 组成 对应组播地址：225.225.100.011：32004
                    }
                }
                MultiPort = PrePort;
                if (!DicTx.ContainsKey(tx))
                {
                    DicTx.Add(tx, this);
                }
                else
                {
                    Logger.Error("初始化Tx有重复id：" + tx);
                    //return DicTx[tx];
                }
            }
            else
            {
                Id = "0000";
                Logger.Error("初始化Tx的id参数无效：" + tx);

            }
        }

    }
}
