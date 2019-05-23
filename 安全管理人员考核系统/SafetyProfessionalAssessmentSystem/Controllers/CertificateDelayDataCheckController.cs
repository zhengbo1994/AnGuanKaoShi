using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using BLL;
using Model;
using Library.baseFn;
using System.IO;

namespace SafetyProfessionalAssessmentSystem.Controllers
{
    [AuthorizeFilter]
    public class CertificateDelayDataCheckController : BaseController
    {
        //
        // GET: /证书延期资料审核/

        public ActionResult Index()
        {
            return View();
        }
        #region 根据查询条件获取jqgrid人员信息
        public class GetEmployeeListForJqgridParam
        {
            public string employeeName { get; set; }
            public string iDNumber { get; set; }
            public string enterpriseName { get; set; }
            public bool? photograph { get; set; }
            public string checkStatus { get; set; }
            public string examType { get; set; }
            public string industry { get; set; }
            public int page { get; set; }
            public int rows { get; set; }
            public string sidx { get; set; }//排序字段名称
            public string sord { get; set; }//排序方式asc desc
        }
        public JsonResult GetCertificateListForJqgrid(GetEmployeeListForJqgridParam param)
        {
            int totalCount = 0;
            Sys_Account account = LoginAccount as Sys_Account;

            ICertificateCtrl certificateCtrl = new CertificateCtrl(account);
            List<Biz_Certificate> certificateIdList = certificateCtrl.GetCertificateListForDelayDataCheck(param.employeeName, param.iDNumber, param.enterpriseName, param.photograph, param.checkStatus, param.page, param.rows, ref totalCount);
            List<Biz_CertificateDelayFile> certificateDelayFileList = certificateCtrl.GetCertificateDelayPhotoList(certificateIdList.Select(p => p.Id).ToList());
            List<Biz_CertificateAuthentication> certificateDelayAuthenticationList = certificateCtrl.GetCertificateDelayAuthenticationList(certificateIdList.Select(p => p.Id).ToList());
            List<Biz_CertificateDelayDataCheckedRecord> certificateDelayDataCheckedRecordList = certificateCtrl.GetCertificateDelayDataCheckedRecordList(certificateIdList.Select(p => p.Id).ToList());

            List<GetCertificateListForJqgridResult> resultList = certificateIdList.GroupJoin(certificateDelayFileList, a => a.Id, b => b.CertificateId, (a, b) => new { a, b = b.FirstOrDefault() })
                .GroupJoin(certificateDelayDataCheckedRecordList, o => o.a.Id, c => c.CertificateId, (o, c) => new { o.a, o.b, c = c.FirstOrDefault() })
                .GroupJoin(certificateDelayAuthenticationList, o => o.a.Id, d => d.CertificateId, (o, d) => new { o.a, o.b, o.c, d = d.FirstOrDefault() })
                .Select(o => new GetCertificateListForJqgridResult()
                {
                    certificateId = o.a.Id,
                    employeeName = o.a.EmployeeName,
                    sex = o.a.Sex,
                    birthday = o.a.Birthday,
                    iDNumber = o.a.IDNumber,
                    enterpriseName = o.a.EnterpriseName,
                    job = o.a.Job,
                    title4Technical = o.a.Title4Technical,
                    certificateNo = o.a.CertificateNo,
                    examType = o.a.ExamType,
                    industry = o.a.Industry,
                    startCertificateDate = o.a.StartCertificateDate.ConvertToDateString(),
                    photoStatus = !o.b.IsNull() ? true : false,
                    authenticationStatus = !o.d.IsNull() ? true : false,
                    checkStatus = o.c.IsNull() ? "未审核" : (o.c.PassStatus ? "审核通过" : "审核不通过"),
                    checkDate = o.c.IsNull() ? "" : o.c.CreateDate.ConvertToDateString(),
                }).ToList();
            JqGridData dataResult = ConvertIListToJqGridData(param.page, param.rows, totalCount, resultList);
            return Json(dataResult, JsonRequestBehavior.AllowGet);
        }
        public class GetCertificateListForJqgridResult
        {
            public int certificateId { get; set; }
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
            public bool photoStatus { get; set; }
            public bool authenticationStatus { get; set; }
            public string checkStatus { get; set; }
            public string checkDate { get; set; }

        }
        #endregion

        #region 获取科目列表
        public JsonResult GetEmployeeSubjectList()
        {
            Sys_Account account = LoginAccount as Sys_Account;
            IWorkFlowCtrl employeeCtrl = new WorkFlowCtrl(account);
            List<GetEmployeeSubjectListResult> employeeSubjectList = employeeCtrl.GetEmployeeSubjectList().Select(p => new GetEmployeeSubjectListResult()
            {
                ItemText = p.ItemText,
                ItemValue = p.ItemValue
            }).ToList();
            return Json(employeeSubjectList, JsonRequestBehavior.AllowGet);
        }
        public class GetEmployeeSubjectListResult
        {
            public string ItemText { get; set; }
            public string ItemValue { get; set; }
        }
        #endregion

        #region 获取行业列表
        public JsonResult GetEmployeeIndustryList()
        {
            Sys_Account account = LoginAccount as Sys_Account;
            IWorkFlowCtrl employeeCtrl = new WorkFlowCtrl(account);
            List<GetEmployeeIndustryListResult> employeeIndustryList = employeeCtrl.GetEmployeeIndustryList().Select(p => new GetEmployeeIndustryListResult()
            {
                ItemText = p.ItemText,
                ItemValue = p.ItemValue
            }).ToList();
            return Json(employeeIndustryList, JsonRequestBehavior.AllowGet);
        }
        public class GetEmployeeIndustryListResult
        {
            public string ItemText { get; set; }
            public string ItemValue { get; set; }
        }
        #endregion

        #region 审核批量人员
        public class CheckByCertificateIdListParam
        {
            public List<int> certificateIdList { get; set; }
            public bool inValidityDate { get; set; }
            public bool annualSafetyTraining { get; set; }
            public bool notBadBehavior { get; set; }
            public bool trainingWith24Hours { get; set; }
            //public bool delayConditions { get; set; }
            public bool passStatus { get; set; }
            public string checkedMark { get; set; }

        }
        public JsonResult CheckByCertificateIdList(string strData)
        {
            ResultMessage resultMessage = new ResultMessage();
            try
            {
                CheckByCertificateIdListParam param = strData.JSONStringToObj<CheckByCertificateIdListParam>();
                Sys_Account account = base.LoginAccount as Sys_Account;
                ICertificateDelay_WorkFlowCtrl certificateDelay_WorkFlowCtrl = new CertificateDelay_WorkFlowCtrl(account);
                certificateDelay_WorkFlowCtrl.CheckCertificateDelayData(param.inValidityDate, param.annualSafetyTraining, param.notBadBehavior, param.trainingWith24Hours, param.passStatus, param.checkedMark, param.certificateIdList);
                resultMessage.IsSuccess = true;
            }
            catch (Exception ex)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = ex.Message;
            }
            return Json(resultMessage, JsonRequestBehavior.DenyGet);
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

        #region 上传取证人员的实名认证信息
        public class UploadCertInfoParam
        {
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
                Biz_Certificate certificate = certificateCtrl.GetCertificateForAuthentication(param.CertNumber);
                if (certificate == null)
                {
                    throw new Exception("没有需要实名认证的证书延期记录");
                }
                Biz_CertificateAuthentication certificateDelayAuthentication = new Biz_CertificateAuthentication()
                {
                    CertificateId = certificate.Id,
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
                resultMessage.ErrorMessage = certificate.Id.ToString();
            }
            catch (Exception ex)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = ex.Message;
            }
            return Json(resultMessage);
        }
        #endregion

        public JsonResult GetCertificateDelayData(int certificateId)
        {
            try
            {
                Sys_Account account = base.LoginAccount as Sys_Account;
                ICertificateCtrl certificateCtrl = new CertificateCtrl(account);
                Biz_Certificate certificate = certificateCtrl.GetCertificateById(certificateId);
                Biz_CertificateDelayDataCheckedRecord certificateDelayDataCheckedRecord = certificateCtrl.GetCertificateDelayDataCheckedRecordList(new List<int>() { certificateId }).SingleOrDefault();
                GetCertificateDelayDataResult result = new GetCertificateDelayDataResult()
                {
                    certificateNo = certificate.CertificateNo,
                    employeeName = certificate.EmployeeName,
                    sex = certificate.Sex,
                    iDNumber = certificate.IDNumber,
                    birthday = certificate.Birthday,
                    enterpriseName = certificate.EnterpriseName,
                    job = certificate.Job,
                    title4Technical = certificate.Title4Technical,
                    examType = certificate.ExamType,
                    industry = certificate.Industry,
                    certificateStartDate = certificate.StartCertificateDate.ConvertToDateString(),
                    certificateEndDate = certificate.EndCertificateDate.ConvertToDateString()
                };
                if (!certificateDelayDataCheckedRecord.IsNull())
                {
                    result.inValidityDate = certificateDelayDataCheckedRecord.InValidityDate;
                    result.annualSafetyTraining = certificateDelayDataCheckedRecord.AnnualSafetyTraining;
                    result.notBadBehavior = certificateDelayDataCheckedRecord.NotBadBehavior;
                    result.trainingWith24Hours = certificateDelayDataCheckedRecord.TrainingWith24Hours;
                    result.delayConditions = certificateDelayDataCheckedRecord.DelayConditions;
                    result.checkedMark = certificateDelayDataCheckedRecord.CheckedMark;
                }
                IEmployeeCtrl employeeCtrl = new EmployeeCtrl(account);
                EmployeeCtrl.StudyRecord studyRecord = employeeCtrl.GetOneYearGetOnlineStudyRecord(new List<string>() { certificate.IDNumber }).SingleOrDefault();
                if (studyRecord != null && studyRecord.TotalHours > 24)
                {
                    result.trainingWith24Hours = true;
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                ResultMessage resultMsg = new ResultMessage() { IsSuccess = false, ErrorMessage = ex.Message };
                return Json(resultMsg);
            }
        }
        public class GetCertificateDelayDataResult
        {
            public int employeeDataCheckedRecordId { get; set; }
            public string certificateNo { get; set; }
            public string employeeName { get; set; }
            public string sex { get; set; }
            public string iDNumber { get; set; }
            public string birthday { get; set; }
            public string enterpriseName { get; set; }
            public string job { get; set; }
            public string title4Technical { get; set; }
            public string industry { get; set; }
            public string examType { get; set; }
            public string certificateStartDate { get; set; }
            public string certificateEndDate { get; set; }
            public bool? inValidityDate { get; set; }
            public bool? annualSafetyTraining { get; set; }
            public bool? notBadBehavior { get; set; }
            public bool? trainingWith24Hours { get; set; }
            public bool? delayConditions { get; set; }
            public string checkedMark { get; set; }
        }

    }
}
