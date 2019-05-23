using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLSLibrary;

namespace PrintApp.Common
{
    public class CustomConfig
    {
        private static ConfigHelper _configHelper = null;
        private static object _locker = new object();
        public ConfigHelper Config
        {
            get
            {
                if (null == _configHelper)
                {
                    lock (_locker)
                    {
                        if (null == _configHelper)
                        {
                            _configHelper = new ConfigHelper("/CustomConfig.xml");
                        }
                    }
                }
                return _configHelper;
            }
        }
    }
}
