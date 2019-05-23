using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace Model
{
    public static class SessionModel
    {
        /// <summary>
        /// 登录用户Account
        /// </summary>
        public static Sys_Account Account
        {
            get
            {
                return HttpContext.Current.Session["Account"] as Sys_Account;
            }
        }

        public static string GetAccountInfo(string fieldName)
        {
            string result = string.Empty;
            if (Account != null)
            {
                PropertyInfo arrProps = Account.GetType().GetProperties().FirstOrDefault(x => x.Name == fieldName);
                if (null != arrProps)
                {
                    result = arrProps.GetValue(Account, null).ToString();
                }
            }
            return result;
        }
    }

}
