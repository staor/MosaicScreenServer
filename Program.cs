using System;
using System.Threading;
using System.Threading.Tasks;
using ConfigServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
     
using ScreenManagerNS;
namespace MosaicScreenServer
{
    class Program
    {
        static ScreenManager ScreenManager = null;
        static void Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            Console.WriteLine("Hello World!");

            AppConfigurationServers.LoadAppConfig();

             
            ScreenManager = new ScreenManager();
             ScreenManager.LoadAsync();

            //var bgtask = Task.Run(() => { Run(cts.Token); });

            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                Console.WriteLine($"{DateTime.Now} 后台测试服务，准备进行资源清理！");

                cts.Cancel();    //设置IsCancellationRequested=true，让TestService今早结束 
                //bgtask.Wait();   //等待 testService 结束执行

                ScreenManager?.SaveScreenCurrentWinsToStore();
                Console.WriteLine($"{DateTime.Now} 恭喜，服务程序已正常退出！");
                Environment.Exit(0);
            };

            Console.ReadLine();
        }

        public static void Run(CancellationToken token)
        {

            if (ScreenManager==null)
            {
                ScreenManager = new ScreenManager();
                Task.Run(() =>
                {
                    ScreenManager.LoadAsync();
                });
                
            }
            
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                } 
            }
            ScreenManager?.SaveScreenCurrentWinsToStore();
        }

        

       
    }
}
