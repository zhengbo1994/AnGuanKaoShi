using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SafetyProfessionalAssessmentSystem.WCF
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“ISys_AccountService”。
    [ServiceContract]
    public interface ISys_AccountService
    {
        [OperationContract]
        void DoWork();
        [OperationContract]
        AccountInfo UserLogin(string loginname, string password);
        [OperationContract]
        string GetUserName(int accountId);
    }
}
