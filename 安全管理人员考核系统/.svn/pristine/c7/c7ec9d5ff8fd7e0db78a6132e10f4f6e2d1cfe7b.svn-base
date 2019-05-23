using BLL;
using Library;
using Library.LogFn;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SafetyProfessionalAssessmentSystem.WCF
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Sys_AccountService”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Sys_AccountService.svc 或 Sys_AccountService.svc.cs，然后开始调试。
    public class Sys_AccountService : ISys_AccountService
    {

        public void DoWork()
        {
        }
        #region 验证用户登录 正确返回账户对象  错误返回空
        public AccountInfo UserLogin(string loginname, string password)
        {
            LogHelper loghelper = new LogHelper();
            AccountInfo accountInfo = null;
            try
            {
                loghelper.WriteInfo(this.GetType(),"用户登录成功[UserLogin] 参数{loginname:" + loginname + ",password:" + password + "}");
                IAccountCtrl accountCtrl = new AccountCtrl(null);
                IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(null);
                bool loginFlag = accountCtrl.ValidAccount(loginname, password);

                if (loginFlag)
                {
                    Sys_Account account = accountCtrl.GetAccountByAccountName(loginname);
                    accountInfo = new AccountInfo()
                    {
                        AccountName = account.AccountName,
                        Password = account.Password,
                        Sex = account.Sex,
                        Age = account.Age,
                        Tel = account.Tel,
                        Address = account.Address,
                        UserId=account.UserId,
                        UserName = workFlowCtrl.GetUserName(account.Id)
                    };
                }
                else
                {
                    accountInfo = null;
                }

            }
            catch (Exception ex)
            {
                loghelper.WriteLog(this.GetType(),ex);
                accountInfo = null;
            }
            return accountInfo;
        }
        #endregion


        public string GetUserName(int accountId)
        {
            LogHelper loghelper = new LogHelper();
            string userName = "";
            try
            {
                loghelper.WriteInfo(this.GetType(), "获取用户名 参数{accountId:" + accountId + "}");
                IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(null);
                userName = workFlowCtrl.GetUserName(accountId);

            }
            catch (Exception ex)
            {
                loghelper.WriteLog(this.GetType(), ex);
                userName = "呵呵,用户名出错";
            }
            return userName;
        }

    }

}
