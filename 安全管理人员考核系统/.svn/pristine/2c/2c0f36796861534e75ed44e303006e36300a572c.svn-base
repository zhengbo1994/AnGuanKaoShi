using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL;
using Model;
using Library.baseFn;
using CLSLibrary;

namespace SafetyProfessionalAssessmentSystem.Controllers
{
    public class CertificatePrintOnlineController : BaseController
    {
        //
        // GET: /CertificatePrintOnline/

        public ActionResult Index()
        {
            return View();
        }

        #region  获取表格显示信息
        public class GetCertificateListForJqGridParam
        {
            //证书编号
            public string certificateNo { get; set; }

            //持证人姓名
            public string employeeName { get; set; }
            public string idNumber { get; set; }
            //企业名称
            public string enterpriseName { get; set; }
            public int page { get; set; }
            public int rows { get; set; }
        }

        public JsonResult GetCertificateListForJqGrid(GetCertificateListForJqGridParam param)
        {
            const string CERTIFICATESTATUS_ACTIVE = "有效的";

            int totalCount = 0;
            Sys_Account account = base.LoginAccount as Sys_Account;
            ICertificateCtrl certificateManagementCtrl = new CertificateCtrl(account);
            List<string> cityList = LoginDataPermissionDetailList.Select(p => p.DetailName).ToList();
            List<Biz_Certificate> certificateList = certificateManagementCtrl.GetCertificateList(param.employeeName, param.idNumber,
                param.certificateNo, CERTIFICATESTATUS_ACTIVE, param.enterpriseName, param.page, param.rows, ref totalCount, cityList);
            List<GetCertificateManagementListForJqGridResult> certificateManagementListJsonResult = certificateList.Select(e => new GetCertificateManagementListForJqGridResult()
            {

                certificateId = e.Id,
                employeeName = e.EmployeeName,
                sex = e.Sex,
                birthday = e.Birthday,
                iDNumber = e.IDNumber,
                enterpriseName = e.EnterpriseName,
                job = e.Job,
                title4Technical = e.Title4Technical,
                certificateNo = e.CertificateNo,
                examType = e.ExamType,
                endCertificateDate = e.EndCertificateDate.ToString("yyyy-MM-dd")
            }).ToList();
            JqGridData dataResult = ConvertIListToJqGridData(param.page, param.rows, totalCount, certificateManagementListJsonResult);
            return Json(dataResult);
        }

        public class GetCertificateManagementListForJqGridResult
        {
            public int certificateId { get; set; }
            //持证人姓名
            public string employeeName { get; set; }
            //性别
            public string sex { get; set; }
            public string birthday { get; set; }
            //身份证号
            public string iDNumber { get; set; }
            //企业名称
            public string enterpriseName { get; set; }
            public string job { get; set; }
            public string title4Technical { get; set; }
            //证书编号
            public string certificateNo { get; set; }
            //证书类别
            public string examType { get; set; }

            //证书有效期
            public string endCertificateDate { get; set; }
        }
        #endregion

        #region 根据id获取证书信息

        public class GetCertificateInfoParam
        {
            //证书id
            public int certificateId { get; set; }

        }
        public JsonResult GetCertificateInfo(GetCertificateInfoParam param)
        {

            Sys_Account account = base.LoginAccount as Sys_Account;

            ICertificateCtrl certificateCtrl = new CertificateCtrl(account);

            Biz_Certificate certificate = certificateCtrl.GetCertificateById(param.certificateId);

            GetCertificateInfoResult result = new GetCertificateInfoResult()
            {

                EmployeeName = certificate.EmployeeName,
                Sex = certificate.Sex,
                Birthday = certificate.Birthday,
                IDNumber = certificate.IDNumber,
                EnterpriseName = certificate.EnterpriseName,
                Job = certificate.Job,
                Title4Technical = certificate.Title4Technical,
                CertificateNo = certificate.CertificateNo,
                StartCertificateDate = certificate.StartCertificateDate.ToString("yyyy年MM月dd日"),
                EndCertificateDate = certificate.EndCertificateDate.ToString("yyyy年MM月dd日")
            };

            return Json(result);
        }

        public class GetCertificateInfoResult
        {
            public string EmployeeName { get; set; }
            public string Sex { get; set; }
            public string Birthday { get; set; }
            public string IDNumber { get; set; }
            public string EnterpriseName { get; set; }
            public string Job { get; set; }
            public string Title4Technical { get; set; }
            public string CertificateNo { get; set; }
            public string StartCertificateDate { get; set; }
            public string EndCertificateDate { get; set; }
        }

        #endregion

        #region 登记照片
        public class GetPhotoParam
        {
            //证书id
            public int certificateId { get; set; }

        }
        public string GetPhoto(GetPhotoParam param)
        {
            string resultFilePath = "";
            string fileRootDirectory = base.getFileRootDirectory();
            try
            {
                Sys_Account account = base.LoginAccount as Sys_Account;

                ICertificateCtrl certificateCtrl = new CertificateCtrl(account);

                Biz_Certificate certificate = certificateCtrl.GetCertificateById(param.certificateId);

                if (certificate.PhotoPath.IsNull())
                {
                    throw new Exception("照片文件不存在");
                }

                resultFilePath = fileRootDirectory + certificate.PhotoPath;
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

    }
}
