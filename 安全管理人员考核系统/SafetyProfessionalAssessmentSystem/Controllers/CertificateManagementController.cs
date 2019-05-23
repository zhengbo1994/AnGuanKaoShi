using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
using BLL;
using Library.baseFn;
using System.IO;
using System.Transactions;
using CLSLibrary;


namespace SafetyProfessionalAssessmentSystem.Controllers
{
    [AuthorizeFilter]
    public class CertificateManagementController : BaseController
    {
        //
        // GET: /CertificateManagement/    

        public ActionResult Index()
        {
            return View();
        }

        #region 获取证书信息
        public class GetCertificateListForJqGridParam
        {
            //证书编号
            public string certificateNo { get; set; }
            //证书状态
            public string certificateStatus { get; set; }
            //持证人姓名
            public string employeeName { get; set; }
            public string idNumber { get; set; }
            //企业名称
            public string enterpriseName { get; set; }
            public string city { get; set; }
            public int page { get; set; }
            public int rows { get; set; }
        }
        public JsonResult GetCertificateListForJqGrid(GetCertificateListForJqGridParam param)
        {
            int totalCount = 0;
            Sys_Account account = base.LoginAccount as Sys_Account;
            ICertificateCtrl certificateManagementCtrl = new CertificateCtrl(account);
            // List<string> cityList = LoginDataPermissionDetailList.Select(p => p.DetailName).ToList();
            List<string> cityList = new List<string>();
            if (!param.city.IsNull())
            {
                cityList.Add(param.city);
            }
            List<Biz_Certificate> certificateList = certificateManagementCtrl.GetCertificateList(param.employeeName, param.idNumber,
                param.certificateNo, param.certificateStatus, param.enterpriseName, param.page, param.rows, ref totalCount, cityList);
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
                certificateStatus = (e.Invalid || e.EndCertificateDate < DateTime.Now) ? "无效的" : "有效的",
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
            //证书状态
            public string certificateStatus { get; set; }
            //证书有效期
            public string endCertificateDate { get; set; }
        }

        #endregion

        #region 获取证书操作子表信息
        public JsonResult GetCertificateListForSubJqGrid(string certificateNo)
        {
            List<GetCertificateListForSubJqGridResult> GetCertificateListForSubJqGridResultList = new List<GetCertificateListForSubJqGridResult>();
            int totalCount = 0;

            Sys_Account account = base.LoginAccount as Sys_Account;
            ICertificateCtrl certificateManagementCtrl = new CertificateCtrl(account);
            IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
            List<Biz_CertificateOperationRecord> certificateOperationRecordList = certificateManagementCtrl.GetCertificateOperationRecordList(certificateNo);
            totalCount = certificateOperationRecordList.Count();
            GetCertificateListForSubJqGridResultList = certificateOperationRecordList.Select(p => new GetCertificateListForSubJqGridResult()
            {
                id = p.Id,
                certificateId = p.CertificateId,
                operateType = p.OperateType,
                remark = p.Remark,
                createUserName = workFlowCtrl.GetUserName(p.CreateById),
                createDate = p.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
            }).ToList();
            JqGridData dataResult = ConvertIListToJqGridData(1, 100, totalCount, GetCertificateListForSubJqGridResultList);
            return Json(dataResult);
        }
        public class GetCertificateListForSubJqGridResult
        {
            public int id { get; set; }
            public int certificateId { get; set; }
            public string changeBeforeStatus { get; set; }
            public string changeToStatus { get; set; }
            public string operateType { get; set; }
            public string remark { get; set; }
            public string createUserName { get; set; }
            public string createDate { get; set; }
        }
        #endregion

        #region 证书吊销
        public class CertificateDeactiveParam
        {
            public int certificateId { get; set; }
            public string remark { get; set; }
        }
        public JsonResult CertificateDeactive(CertificateDeactiveParam param)
        {
            ResultMessage resultMessage = new ResultMessage();
            try
            {
                Sys_Account account = base.LoginAccount as Sys_Account;
                ICertificateCtrl certificateManagementCtrl = new CertificateCtrl(account);
                certificateManagementCtrl.ChangeCertificateStatus(param.certificateId, param.remark, "吊销");
            }
            catch (Exception ex)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = ex.Message;
            }
            return Json(resultMessage);
        }
        #endregion

        #region 证书恢复
        public class CertificateRecoverParam
        {
            public int certificateId { get; set; }
            public string remark { get; set; }
        }
        public JsonResult CertificateRecover(CertificateRecoverParam param)
        {
            ResultMessage resultMessage = new ResultMessage();
            try
            {
                Sys_Account account = base.LoginAccount as Sys_Account;
                ICertificateCtrl certificateManagementCtrl = new CertificateCtrl(account);
                certificateManagementCtrl.ChangeCertificateStatus(param.certificateId, param.remark, "恢复");
            }
            catch (Exception ex)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = ex.Message;
            }
            return Json(resultMessage);
        }
        #endregion

        #region 根据Id获取证书信息
        public JsonResult GetCertificateManagementById(int certificateId)
        {
            Sys_Account account = LoginAccount as Sys_Account;
            ICertificateCtrl certificateManagementCtrl = new CertificateCtrl(account);
            Biz_Certificate certificate = certificateManagementCtrl.GetCertificateById(certificateId);
            CertificateResult certificateResult = new CertificateResult()
            {
                Id = certificate.Id,
                //CertificateType = certificate.CertificateType,
                //CertificateNumber = certificate.CertificateNumber,
                //CertificateStatus = certificate.CertificateStatus,
                //HolderOfTheCertificate = certificate.HolderOfTheCertificate,
                //IDNumber = certificate.IDNumber,
                //ContactWay = certificate.ContactWay,
                //EnterpriseName = certificate.EnterpriseName
            };
            return Json(certificateResult, JsonRequestBehavior.AllowGet);

        }
        public class CertificateResult
        {
            public int Id { get; set; }
            //证书类别
            public string CertificateType { get; set; }
            //证书编号
            public string CertificateNumber { get; set; }
            //证书状态
            public string CertificateStatus { get; set; }
            //证书有效期
            public string ValidityOfTheCertificate { get; set; }
            //持证人姓名
            public string employeeName { get; set; }
            //身份证号
            public string IDNumber { get; set; }
            //联系方式
            public string ContactWay { get; set; }
            //企业名称
            public string EnterpriseName { get; set; }
        }
        #endregion

        #region 获取企业List
        public JsonResult GetEnterpriseList()
        {
            Sys_Account account = LoginAccount as Sys_Account;
            IEnterpriseCtrl enterpriseCtrl = new EnterpriseCtrl(account);
            List<string> cityList = base.LoginDataPermissionDetailList.Select(p => p.DetailName).ToList();
            List<Biz_Enterprise> enterpriseList = enterpriseCtrl.GetEnterpriseListByCityList(cityList);
            List<EnterpriseResult> enterpriseNameList = enterpriseList.Select(p => new EnterpriseResult() { enterpriseName = p.EnterpriseName, socialCreditCode = p.SocialCreditCode }).ToList();
            return Json(enterpriseNameList);
        }
        public class EnterpriseResult
        {
            public string enterpriseName { get; set; }
            public string socialCreditCode { get; set; }
        }
        #endregion


        #region 上传实名认证信息
        public class UploadCertInfoParam
        {
            public int CertificateId { get; set; }
            public string PartyName { get; set; }
            public string Gender { get; set; }
            public string Nation { get; set; }
            public string BornDay { get; set; }
            public string CertAddress { get; set; }
            public string CertNumber { get; set; }
            public string CertOrg { get; set; }
            public string EffDate { get; set; }
            public string ExpDate { get; set; }
            public string PictureBase64 { get; set; }
            public string SamId { get; set; }
        }
        public JsonResult UploadCertInfo(UploadCertInfoParam param)
        {
            ResultMessage resultMessage = new ResultMessage() { IsSuccess = true };
            Sys_Account account = LoginAccount as Sys_Account;
            ICertificateCtrl certificateCtrl = new CertificateCtrl(account);
            try
            {
                certificateCtrl.VerifyChangeEnterprise(param.CertificateId, param.CertNumber);
                Biz_CertificateAuthentication certificateDelayAuthentication = new Biz_CertificateAuthentication()
                {
                    CertificateId = param.CertificateId,
                    Nation = param.Nation,
                    BornDay = param.BornDay,
                    CertAddress = param.CertAddress,
                    CertNumber = param.CertNumber,
                    CertOrg = param.CertOrg,
                    EffDate = param.EffDate,
                    ExpDate = param.ExpDate,
                    Gender = param.Gender,
                    PictureBase64 = param.PictureBase64,
                    PartyName = param.PartyName,
                    SamId = param.SamId
                };
                certificateCtrl.AddCertificateDelayAuthentication(certificateDelayAuthentication);
            }
            catch (Exception ex)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = ex.Message;
            }
            return Json(resultMessage);
        }
        #endregion

        #region 获取证书信息
        public JsonResult GetCertificateInfo(int certificateId)
        {
            try
            {
                Sys_Account account = base.LoginAccount as Sys_Account;
                ICertificateCtrl certificateCtrl = new CertificateCtrl(account);
                Biz_Certificate certificate = certificateCtrl.GetCertificateById(certificateId);
                GetCertificateInfoResult result = new GetCertificateInfoResult()
                {

                    employeeName = certificate.EmployeeName,
                    sex = certificate.Sex,
                    birthday = certificate.Birthday,
                    iDNumber = certificate.IDNumber,
                    enterpriseName = certificate.EnterpriseName,
                    job = certificate.Job,
                    title4Technical = certificate.Title4Technical,
                    certificateNo = certificate.CertificateNo,
                    examType = certificate.ExamType,
                    industry = certificate.Industry,
                    startCertificateDate = certificate.StartCertificateDate.ConvertToDateString(),
                    endCertificateDate = certificate.EndCertificateDate.ConvertToDateString(),
                };

                Biz_CertificateDelayFile certificateDelayFile = certificateCtrl.GetCertificateDelayPhoto(certificateId);
                if (certificateDelayFile != null)
                {
                    string rootFolder = base.getFileRootDirectory();
                    string absolutePath = string.Format(@"{0}\{1}", rootFolder.TrimEnd('\\'), certificateDelayFile.FilePath.TrimStart('\\'));//文件绝对路径
                    if (System.IO.File.Exists(absolutePath))
                    {
                        string base64str = AppFn.ImgToBase64String(absolutePath);
                        result.photoBase64 = base64str;
                    }
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                ResultMessage result = new ResultMessage() { IsSuccess = false, ErrorMessage = ex.Message };
                return Json(result);
            }
        }
        public class GetCertificateInfoResult
        {
            public string employeeName { get; set; }
            public string sex { get; set; }
            public string birthday { get; set; }
            public string iDNumber { get; set; }
            public string enterpriseName { get; set; }
            public string job { get; set; }
            public string title4Technical { get; set; }
            public string certificateNo { get; set; }
            public string examType { get; set; }
            public string industry { get; set; }
            public string startCertificateDate { get; set; }
            public string endCertificateDate { get; set; }
            public string photoBase64 { get; set; }
        }
        #endregion

        #region 保存登记照
        public class SaveEmployeePhotoParam
        {
            public int certificateId { get; set; }
            public string fileInBase64 { get; set; }
        }
        public JsonResult SavePhoto(SaveEmployeePhotoParam param)
        {
            FileStream fs = null;
            MemoryStream ms = null;
            try
            {
                Sys_Account account = base.LoginAccount as Sys_Account;
                ICertificateCtrl certificateCtrl = new CertificateCtrl(account);

                Biz_Certificate certificate = certificateCtrl.GetCertificateById(param.certificateId);

                byte[] arrFileData = Convert.FromBase64String(param.fileInBase64);
                ms = new MemoryStream(arrFileData);

                string rootFolder = base.getFileRootDirectory();
                string fileName = string.Format("{0}_{1}.png", certificate.IDNumber, param.certificateId);//文件名

                string relativePath = string.Format(@"\{0}\{1}\{2}", "证书延期文件", DateTime.Now.ToString("yyyy-MM-dd"), fileName);//数据库相对路径
                string absolutePath = string.Format(@"{0}\{1}", rootFolder.TrimEnd('\\'), relativePath.TrimStart('\\'));//文件绝对路径
                FileInfo file = new FileInfo(absolutePath);
                if (!file.Directory.Exists)
                {
                    file.Directory.Create();
                }
                else if (file.Exists)
                {
                    file.Attributes = FileAttributes.Normal;
                    file.Delete();
                }
                fs = new FileStream(absolutePath, FileMode.CreateNew);
                ms.CopyToStream(fs);
                fs.Flush();
                certificateCtrl.SaveCertificateDelayFile(param.certificateId, "登记照", fileName, relativePath);
                ResultMessage result = new ResultMessage() { IsSuccess = true };
                return Json(result);
            }
            catch (Exception ex)
            {
                ResultMessage result = new ResultMessage() { IsSuccess = false, ErrorMessage = ex.Message };
                return Json(result);
            }
            finally
            {
                if (null != fs) fs.Close();
                if (null != ms) ms.Close();
            }
        }
        #endregion

        #region 证书企业变更
        public class EnterpriseChangeParam
        {
            public int certificateId { get; set; }
            public string examType { get; set; }
            public string enterpriseName { get; set; }
        }
        public JsonResult EnterpriseChange(EnterpriseChangeParam param)
        {
            ResultMessage resultMessage = new ResultMessage() { IsSuccess = true };
            Sys_Account account = LoginAccount as Sys_Account;
            ICertificateCtrl certificateCtrl = new CertificateCtrl(account);

            TransactionScope scope = CreateTransaction();
            try
            {
                certificateCtrl.ChangeEnterprise(param.certificateId, param.examType, param.enterpriseName);
                base.TransactionCommit(scope);
            }
            catch (Exception ex)
            {
                base.TransactionRollback(scope);
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = ex.Message;
            }
            return Json(resultMessage);
        }
        #endregion

        #region  获取证书信息
        public JsonResult GetCertificateData(int certificateId)
        {

            try
            {
                Sys_Account account = base.LoginAccount as Sys_Account;
                ICertificateCtrl certifcateCtrl = new CertificateCtrl(account);
                Biz_Certificate cert = certifcateCtrl.GetCertificateById(certificateId);

                GetCertificateDataResult result = new GetCertificateDataResult();
                result.EmployeeName = cert.EmployeeName;
                result.Sex = cert.Sex;
                result.Birthday = cert.Birthday;
                result.IDNumber = cert.IDNumber;
                result.EnterpriseName = cert.EnterpriseName;
                result.Job = cert.Job;
                result.Title4Technical = cert.Title4Technical;
                result.CertificateNo = cert.CertificateNo;
                result.StartCertificateDate_Year = cert.StartCertificateDate.Year.ToString();
                result.StartCertificateDate_Month = cert.StartCertificateDate.Month.ToString();
                result.StartCertificateDate_Day = cert.StartCertificateDate.Day.ToString();

                string fileRootDirectory = base.getFileRootDirectory();
                string photoPath = fileRootDirectory + cert.PhotoPath;
                if (!System.IO.File.Exists(photoPath))
                {
                    throw new Exception("照片文件不存在");
                }
                result.PhotoBase64 = AppFn.ImgToBase64String(photoPath);

                {
                    string certificateNo = cert.CertificateNo;
                    string FileFolder = fileRootDirectory + "\\证书二维码(可删除)";
                    //二维码内容
                    string localtionUrl = System.Configuration.ConfigurationManager.AppSettings["LocaltionUrl"];
                    string certificateVerifyUrl = System.Configuration.ConfigurationManager.AppSettings["CertificateVerifyUrl"];
                    certificateNo = HttpUtility.UrlEncode(certificateNo);//encodeURIComponent编码 解码用decodeURIComponent
                    string qrcodeStr = localtionUrl + string.Format(certificateVerifyUrl, certificateNo);
                    QRCodeHelper qrcoder = new QRCodeHelper();
                    string QRCodeFilePath = qrcoder.GetQRCODEByString(qrcodeStr, FileFolder, 128);
                    result.QRCodeBase64 = AppFn.ImgToBase64String(QRCodeFilePath);
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                ResultMessage resultMessage = new ResultMessage() { IsSuccess = false, ErrorMessage = ex.Message };
                return Json(resultMessage);
            }

        }
        public class GetCertificateDataResult
        {
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
            public string QRCodeBase64 { get; set; }
            public string PhotoBase64 { get; set; }
        }
        #endregion

        #region 保存打印记录
        public JsonResult SavePrintCertificateRecord(string certificateNo)
        {
            ResultMessage resultMessage = new ResultMessage();
            try
            {
                Sys_Account account = base.LoginAccount as Sys_Account;
                IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
                Biz_CertificatePrintRecord pintRecord = new Biz_CertificatePrintRecord()
                {
                    CertificateNo = certificateNo,
                    PrintType = "新证打印"
                };
                workFlowCtrl.SaveCertificatePrintRecord(pintRecord);
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
