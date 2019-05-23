using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;

namespace BLL
{
    public interface ISmrzServiceCtrl
    {
        void newCertificateAuthentication(int employeeId);
        string KeyLogin(string keyCode, string socialCreditCode);
        List<SmrzServiceCtrl.CerificateInfo> GetCertificateListByIDNumber(string iDNumber);
        SmrzServiceCtrl.CerificateInfo GetCertificateByCertificateNo(string certificateNo);
        SmrzServiceCtrl.AGRYEnterpriseInfo GetAGRYEnterpriseInfo(string keyCode);

        void DelayCertificateAuthentication(int certificateId);
        void ChangeCertificateStatus(int certificateId, string status);
    }
}
