using ConfigServer;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoggerHelper
{
    public class Logger
    {
        #region NLog
        static NLog.Logger logger = LogManager.GetCurrentClassLogger();
        public static void Info(object msg)
        {
            CWshow(msg);
            logger.Info(msg);
        }
        public static void Trace(object msg)
        {
            logger.Trace(msg);
        }
        public static void Debug(object msg)
        {
            CWshow(msg);
            logger.Debug(msg);
        }
        public static void Error(object msg)
        {
            CWshow(msg);
            logger.Error(msg);
        }

        public static void CWshow(object msg)
        {
            if (hsConfig.isDebug)
            {
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss:fff") + " ||" + msg);
            }
        }
        #endregion
    }
}
