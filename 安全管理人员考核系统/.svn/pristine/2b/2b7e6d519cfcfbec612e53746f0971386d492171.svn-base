using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Library.baseFn;

namespace SafetyProfessionalAssessmentSystem.Controllers
{
    [LogException]
    public class CertificateVerifyController : BaseController
    {
        //
        // GET: /CertificateVerify/

        public ActionResult Index(string certificateNo)
        {
            certificateNo = HttpUtility.UrlDecode(certificateNo);
            ICertificateCtrl certificateCtrl = new CertificateCtrl(null);
            Biz_Certificate certificate = certificateCtrl.GetCertificateByNo(certificateNo);
            ViewBag.certificate = certificate;
            return View();
        }
        #region 登记照片
        public FileResult GetPhoto(int certificateId)
        
        {
            string resultFilePath = "";
            string fileRootDirectory = base.getFileRootDirectory();
            try
            {
                Sys_Account account = base.LoginAccount as Sys_Account;
                IEmployeeCtrl employeeCtrl = new EmployeeCtrl(account);
                Biz_EmployeeFile employeeFile = employeeCtrl.GetEmployeeFile(certificateId);
                if (employeeFile.IsNull())
                {
                    throw new Exception("未找到登记照信息");
                }
                resultFilePath = fileRootDirectory + employeeFile.FilePath;
                if (!System.IO.File.Exists(resultFilePath))
                {
                    throw new Exception("照片文件不存在");
                }
            }
            catch (Exception ex)
            {
                resultFilePath = fileRootDirectory + "\\error.jpg";
            }

            return File(resultFilePath, "image/jpeg");
        }
        #endregion

    }
}
