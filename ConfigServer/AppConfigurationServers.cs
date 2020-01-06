using LoggerHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using Utils;

namespace ConfigServer
{
    class AppConfigurationServers
    {
        public static IConfiguration Configuration { get; set; }
        static AppConfigurationServers()
        {
            //ReloadOnChange = true 当appsettings.json被修改时重新加载 
            Configuration = new ConfigurationBuilder()  //.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json") //你配置的文件
                .Add(new JsonConfigurationSource
                {
                    Path = "appsettings.json", ReloadOnChange = true
                })
                .AddJsonFile("appsettings.json", true,reloadOnChange:true)
                .Build()
                
                ;

            //LoadAppConfig();


        }
        public string GetConnectionString(string key=null)
        {
            try
            {
                return string.IsNullOrEmpty(key) ? Configuration.GetConnectionString("default") : Configuration.GetConnectionString(key);
            }
            catch (Exception ex)
            {
                Logger.Error("GetValueString-" + ex.Message);
            }
            return "";
        }
        public string GetValueString(string key)   //AppConfigurtaionServices.Configuration["Appsettings:SystemName"]; 二级节点   //得到 PDF .NET CORE
        {
            try
            {                
                return string.IsNullOrEmpty(key) ? "":Configuration[key];
            }
            catch (Exception ex)
            {
                Logger.Error("GetValueString-" + ex.Message);
            }
            return "";
        }
        public IConfigurationSection GetSectionString(string key)
        {
            try
            {
                return  Configuration.GetSection(key);                
            }
            catch (Exception ex)
            {
                Logger.Error("GetValueString-" + ex.Message);
            }
            return null;
        }

        public static void LoadAppConfig()
        {
            try
            {
                var manager = ConfigurationManager.AppSettings;
                string hasWinDebug = ConfigurationManager.AppSettings["hasWinDebug"];  //配置文件修改是否显示调试内容.没有调试内容肯能会提高些性能.
                if (hasWinDebug == "1")
                {
                    hsConfig.isDebug = true;
                }
                else
                {
                    hsConfig.isDebug = false;
                }
               
                string IsShowRx4Debug = ConfigurationManager.AppSettings["IsShowRx4Debug"];  //配置文件修改是否显示调试内容.没有调试内容肯能会提高些性能.
                if (IsShowRx4Debug == "1")
                {
                    hsConfig.IsShowRx4Debug = true;
                }
                else
                {
                    hsConfig.IsShowRx4Debug = false;
                }

                hsConfig.Port_Tcp_MosaicServer = int.Parse(ConfigurationManager.AppSettings["Port_Tcp_MosaicServer"]);


                hsConfig.serverIp = ConfigurationManager.AppSettings["serverIp"];
                hsConfig.serverPort = int.Parse(ConfigurationManager.AppSettings["serverPort"]);

                hsConfig.MultiIp_SynchroTxServer = ConfigurationManager.AppSettings["MultiIp_SynchroTxServer"];
                hsConfig.MultiPort_SynchroTxServer = int.Parse(ConfigurationManager.AppSettings["MultiPort_SynchroTxServer"]);

                hsConfig.MultiIp_SynchroMosaicServer = ConfigurationManager.AppSettings["MultiIp_SynchroMosaicServer"];
                hsConfig.MultiPort_SynchroMosaicServer = int.Parse(ConfigurationManager.AppSettings["MultiPort_SynchroMosaicServer"]);

                hsConfig.MultiIp_SynchroRx = ConfigurationManager.AppSettings["MultiIp_SynchroRx"];
                hsConfig.MultiPort_SynchroRx = int.Parse(ConfigurationManager.AppSettings["MultiPort_SynchroRx"]);

                //OneInstanceHelper.ChatMultiIp = ConfigurationManager.AppSettings["MultiIp_SynchroServerState"];
                //OneInstanceHelper.ChatRcvMultiPort = int.Parse(ConfigurationManager.AppSettings["MultiPort_SynchroServerState"]);

                hsConfig.Version_MosaicServerToRx = ConfigurationManager.AppSettings["Version_MosaicServerToRx"];

                hsConfig.SendRx_MultiIp = ConfigurationManager.AppSettings["SendRx_MultiIp"];
                hsConfig.SendRx_MultiPort = int.Parse(ConfigurationManager.AppSettings["SendRx_MultiPort"]);

                hsConfig.WinRectRate = double.Parse(ConfigurationManager.AppSettings["WinRectRate"]);
            }
            catch (Exception ex)
            {
                hsServer.ShowDebug("Window_Loaded- config读取配置文件异常：" + ex.Message);
            }
        }
    }
}
