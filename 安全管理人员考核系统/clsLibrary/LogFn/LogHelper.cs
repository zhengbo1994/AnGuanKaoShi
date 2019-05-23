using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.IO;
using Library.baseFn;

namespace Library.LogFn
{
    public class LogHelper
    {
      


        public LogHelper()
        {

            var path = AppDomain.CurrentDomain.BaseDirectory + @"\log4net_config.xml";
            log4net.Config.XmlConfigurator.Configure(new FileInfo(path));

        }

        public void WriteLog(Type type, Exception ex)
        {
            ILog log = LogManager.GetLogger(type);
            log.Error("Error", ex);
        }

        public void WriteLog(Type type, string msg)
        {
           
            ILog log = LogManager.GetLogger(type);
            log.Error(msg);
        }

        public void WriteInfo(Type type, string msg)
        {
          
            ILog log = LogManager.GetLogger(type);
            log.Info(msg);
        }
    }
}
