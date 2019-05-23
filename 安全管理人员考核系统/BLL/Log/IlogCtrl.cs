using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL
{
    public interface ILogCtrl
    {
        void WriteErrorLog(Exception ex);
        void WriteErrorLog(string msg);
        void WriteInfoLog(string msg);
    }
}
