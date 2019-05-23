using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Library;
using System.Configuration;

namespace BLL
{
    public class LogCtrl : ILogCtrl
    {
        private LogHelper _logError = null;
        private LogHelper _logInfo = null;
        public LogCtrl()
        {
            _logError = new LogHelper(ConfigurationManager.AppSettings["LogFolderPath_Error"]);
            _logInfo = new LogHelper(ConfigurationManager.AppSettings["LogFolderPath_Info"]);
        }

        public void WriteErrorLog(Exception ex)
        {
            _logError.WriteLog(ex);
        }

        public void WriteErrorLog(string msg)
        {
            _logError.WriteLog(msg);
        }

        public void WriteInfoLog(string msg)
        {
            _logInfo.WriteLog(msg);
        }
    }
}
