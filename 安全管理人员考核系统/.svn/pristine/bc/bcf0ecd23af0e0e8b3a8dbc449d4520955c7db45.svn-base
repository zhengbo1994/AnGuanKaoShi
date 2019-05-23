using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Library.baseFn;
using CLSLibrary;
using System.Drawing;
using System.IO;

namespace SafetyProfessionalAssessmentSystem.Controllers
{
    [AuthorizeFilter]
    public class RP_PrintCertificateController : BaseController
    {
        //
        // GET: /继续教育打印证书/

        public ActionResult Index()
        {
            return View();
        }

        #region   获取证书列表
        public class GetEmployeeListForJqgridParam
        {
            public string EmployeeName { get; set; }
            public string IDNumber { get; set; }
            public string EnterpriseName { get; set; }
            public string ExamType { get; set; }
            public string Industry { get; set; }
            public int page { get; set; }
            public int rows { get; set; }
            public string sidx { get; set; }//排序字段名称
            public string sord { get; set; }//排序方式asc desc

        }
        public JsonResult GetEmployeeListForJqgrid(GetEmployeeListForJqgridParam param)
        {
            int totalCount = 0;
            Sys_Account account = LoginAccount as Sys_Account;
            IRP_EmployeeCtrl rpEmployeeCtrl = new RP_EmployeeCtrl(account);
            List<string> cityList = base.LoginDataPermissionDetailList.Select(p => p.DetailName).ToList();
            List<Biz_Certificate> certificateList = rpEmployeeCtrl.GetEmployeeListInPrintCertificate(param.EmployeeName, param.IDNumber, param.ExamType, param.Industry, param.EnterpriseName, cityList, param.page, param.rows, ref totalCount);
            List<GetEmployeeListForJqgridResult> CertificateInfoList = certificateList.Select(o => new GetEmployeeListForJqgridResult()
                                                       {
                                                           CertificateId = o.Id,
                                                           EmployeeName = o.EmployeeName,
                                                           Sex = o.Sex,
                                                           Birthday = o.Birthday,
                                                           IDNumber = o.IDNumber,
                                                           EnterpriseName = o.EnterpriseName,
                                                           Job = o.Job,
                                                           Title4Technical = o.Title4Technical,
                                                           Industry = o.Industry,
                                                           ExamType = o.ExamType,
                                                           CertificateNo = o.CertificateNo,
                                                           StartCertificateDate = o.StartCertificateDate.ToString("yyyy-MM-dd"),
                                                           EndCertificateDate = o.EndCertificateDate.ToString("yyyy-MM-dd")
                                                       }).ToList();
            JqGridData dataResult = ConvertIListToJqGridData(param.page, param.rows, totalCount, CertificateInfoList);
            return Json(dataResult, JsonRequestBehavior.AllowGet);
        }
        public class GetEmployeeListForJqgridResult
        {
            public int CertificateId { get; set; }
            public string EmployeeName { get; set; }
            public string Sex { get; set; }
            public string Birthday { get; set; }
            public string IDNumber { get; set; }
            public string EnterpriseName { get; set; }
            public string Job { get; set; }
            public string Title4Technical { get; set; }
            public string ExamPlanNumber { get; set; }
            public string TrainingInstutionName { get; set; }
            public string Industry { get; set; }
            public string ExamType { get; set; }
            public string CertificateNo { get; set; }
            public string StartCertificateDate { get; set; }
            public string EndCertificateDate { get; set; }
            public string PrintStatus { get; set; }

        }
        #endregion

        #region  获取证书信息
        public JsonResult GetCertificateInfo(int certificateId)
        {
            CertificateInfo certificate = new CertificateInfo()
            {
                resultMessage = new ResultMessage() { IsSuccess = true }
            };
            try
            {

                Sys_Account account = base.LoginAccount as Sys_Account;
                ICertificateCtrl certificateCtrl = new CertificateCtrl(account);
                Biz_Certificate cert = certificateCtrl.GetCertificateById(certificateId);
                certificate.CertificateId = cert.Id;
                certificate.EmployeeName = cert.EmployeeName;
                certificate.Sex = cert.Sex;
                certificate.Birthday = cert.Birthday;
                certificate.IDNumber = cert.IDNumber;
                certificate.EnterpriseName = cert.EnterpriseName;
                certificate.Job = cert.Job;
                certificate.Title4Technical = cert.Title4Technical;
                certificate.CertificateNo = cert.CertificateNo;
                certificate.StartCertificateDate_Year = cert.StartCertificateDate.Year.ToString();
                certificate.StartCertificateDate_Month = cert.StartCertificateDate.Month.ToString();
                certificate.StartCertificateDate_Day = cert.StartCertificateDate.Day.ToString();

            }
            catch (Exception ex)
            {
                certificate.resultMessage.IsSuccess = false;
                certificate.resultMessage.ErrorMessage = ex.Message;
            }
            return Json(certificate);
        }
        public class CertificateInfo
        {
            public ResultMessage resultMessage { get; set; }
            public int CertificateId { get; set; }
            public string EmployeeName { get; set; }
            public string Sex { get; set; }
            public string Birthday { get; set; }
            public string IDNumber { get; set; }
            public string EnterpriseName { get; set; }
            public string Job { get; set; }
            public string Title4Technical { get; set; }
            public string CertificateNo { get; set; }
            public string StartCertificateDate_Year { get; set; }
            public string StartCertificateDate_Month { get; set; }
            public string StartCertificateDate_Day { get; set; }
        }
        #endregion

        #region 登记照片
        public string GetPhoto(int certificateId)
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
                resultFilePath = fileRootDirectory + "/error.jpg";
            }
            string strbase64 = AppFn.ImgToBase64String(resultFilePath);
            return strbase64;
        }
        #endregion



        #region 证书二维码
        public string GetCertificateQRCode(string certificateNo)
        {
            string resultFilePath = "";
            string fileRootDirectory = base.getFileRootDirectory();
            try
            {
                QRCodeHelper qrcoder = new QRCodeHelper();

                string FileFolder = fileRootDirectory + "\\证书二维码(可删除)";
                //二维码内容
                string localtionUrl = System.Configuration.ConfigurationManager.AppSettings["LocaltionUrl"];
                string certificateVerifyUrl = System.Configuration.ConfigurationManager.AppSettings["CertificateVerifyUrl"];
                certificateNo = HttpUtility.UrlEncode(certificateNo);//encodeURIComponent编码 解码用decodeURIComponent
                string qrcodeStr = localtionUrl + string.Format(certificateVerifyUrl, certificateNo);
                resultFilePath = qrcoder.GetQRCODEByString(qrcodeStr, FileFolder, 128);
            }
            catch (Exception ex)
            {
                resultFilePath = fileRootDirectory + "/error.jpg";
            }
            string strbase64 = AppFn.ImgToBase64String(resultFilePath);
            return strbase64;
        }
        #endregion

        #region 保存打印记录
        public JsonResult SavePrintCertificateRecord(string certificateNo)
        {
            ResultMessage resultMessage = new ResultMessage();
            try
            {
                Sys_Account account = base.LoginAccount as Sys_Account;

                Biz_RP_CertificatePrintRecord pintRecord = new Biz_RP_CertificatePrintRecord()
                {
                    CertificateNo = certificateNo,
                    PrintType = "继续教育证书打印"
                };
                IRP_EmployeeCtrl rpEmployeeCtrl = new RP_EmployeeCtrl(account);
                rpEmployeeCtrl.SaveRPCertificatePrintRecord(pintRecord);
            }
            catch (Exception ex)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = ex.Message;
            }
            return Json(resultMessage);
        }
        #endregion

    }
}
