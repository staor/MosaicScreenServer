
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utils;

namespace ScreenManager
{
    public class PrePlan : PrePlanInfo
    {
        #region Property
        
        Task TaskTurning { get; set; }
        CancellationTokenSource TokenSource;
        Object lockObj = new object();

        bool _isDuring;
        public bool IsDuring
        {
            get => _isDuring;
            set
            {
                if (value != _isDuring)
                {
                    _isDuring = value;
                    //OnPropertyChanged(nameof(IsDuring));
                }
            }
        }
        //int _min;
        public int Min
        {
            get => (int)(SwiInterval / 60);
            set
            {
                int min = (int)(SwiInterval / 60);
                if (value != min)
                {
                    int sec = SwiInterval % 60;
                    SwiInterval = value * 60 + sec;
                    //OnPropertyChanged(nameof(Min));
                }
            }
        }
        //int _sec;
        public int Sec
        {
            get => SwiInterval % 60;
            set
            {
                int sec = SwiInterval % 60;
                if (value != sec)
                {
                    int min = SwiInterval / 60;
                    SwiInterval = min * 60 + value;
                    //OnPropertyChanged(nameof(Sec));
                }
            }
        }
        List<SceneInfo> _ocScenes = new List<SceneInfo>();
        public List<SceneInfo> OcScenes
        {
            get => _ocScenes;
            private set
            {
                if (value != _ocScenes)
                {
                    _ocScenes = value;
                    //OnPropertyChanged(nameof(OcScenes));
                }
            }
        }


        public ScreenViewModel ParentScreen { get; set; }
        #endregion
        
        #region Methods
        public void StartTurning()
        {
            if (Min == 0 && Sec == 0)
            {
                hsServer.ShowDebug($"当前预案 {Name} 的切换间隔时间为00:00,无法切换!");
                return;
            }
            //StopTurning();
            lock (lockObj)
            {
                if (TokenSource != null)
                {
                    TokenSource.Cancel();
                }
                TokenSource = new CancellationTokenSource();
            }
            var token = TokenSource.Token;
            Task task = Task.Run(() =>
            {
                List<SceneInfo> list = OcScenes;
                if (list.Count == 0)
                {
                    return;
                }
                int during = (Min * 60 + Sec) * 1000;
                int currentIndex = -1;
                while (!token.IsCancellationRequested)
                {
                    currentIndex++;
                    if (currentIndex >= OcScenes.Count)
                    {
                        currentIndex = 0;
                    }
                    ParentScreen.SceneLayoutAndPlay(OcScenes[currentIndex].ListWins); //实时的当前场景
                    Thread.Sleep(during);
                }
            }, token);
            IsDuring = true;
            hsServer.ShowDebug("已开始预案显示：" + Name);
        }
        public void StopTurning()
        {
            lock (lockObj)
            {
                if (TokenSource != null)
                {
                    TokenSource.Cancel();
                }
                TokenSource = null;
            }
            IsDuring = false;
            hsServer.ShowDebug("已停止预案显示：" + Name);
        }

        public void UpdateInfoToOcScenes()
        {
            OcScenes.Clear();
            List<SceneInfo> list = ParentScreen.Scenes;
            foreach (var item in SceneNames)
            {
                var temp = list.Find(f => f.Name == item);
                if (temp != null)
                {
                    OcScenes.Add(temp);
                }
                else
                {
                    hsServer.ShowDebug("当前大屏没有找到对应场景名称：" + item);
                }
            }
        }

        #endregion
        #region MyRegion

        #endregion
    }
}
