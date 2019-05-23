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
    public class RP_EmployeeDataCheckController : BaseController
    {
        //
        // GET: /继续教育人员资料审核/

        public ActionResult Index()
        {
            return View();
        }
        #region 根据查询条件获取jqgrid人员信息
        public class GetEmployeeListForJqgridParam
        {
            public string enterpriseName { get; set; }
            public string employeeName { get; set; }
            public bool? photograph { get; set; }
            public string iDNumber { get; set; }
            public string examType { get; set; }
            public string industry { get; set; }
            public string checkStatus { get; set; }
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
            IEnterpriseCtrl enterpriseCtrl = new EnterpriseCtrl(account);
            List<Biz_RP_EmployeeRegistration> employeeList = rpEmployeeCtrl.GetEmployeeListForDataCheck(param.enterpriseName, param.employeeName, param.iDNumber,
                param.examType, param.industry, param.checkStatus,param.photograph, param.page, param.rows, ref totalCount);
            List<Biz_Enterprise> enterpriseList = enterpriseCtrl.GetEnterpriseInfoByIdList(employeeList.Select(p => p.EnterpriseId).ToList());
            List<Biz_RP_EmployeeDataCheckedRecord> dataCheckedRecordList = rpEmployeeCtrl.GetEmployeeDataCheckedRecordListByRPEmployeeIdList(employeeList.Select(p => p.Id).ToList());
            List<Biz_RP_EmployeeFile> RPEmployeePhotoList = rpEmployeeCtrl.GetRPEmployeePhotoListByRPEmployeeIdList(employeeList.Select(p => p.Id).ToList());
            List<GetEmployeeListForJqgridJsonResult> employeeListJsonResult = employeeList.Join(enterpriseList, a => a.EnterpriseId, b => b.Id, (a, b) => new { a, b })
             .GroupJoin(dataCheckedRecordList, o => o.a.Id, c => c.EmployeeId, (o, c) => new { o.a, o.b, c })
             .GroupJoin(RPEmployeePhotoList, o => o.a.Id, d => d.RPEmployeeId, (o, d) => new { o.a, o.b, o.c, d })
             .Select(o => new GetEmployeeListForJqgridJsonResult()
             {
                 Id = o.a.Id,
                 EmployeeName = o.a.EmployeeName,
                 Sex = o.a.Sex,
                 Age = o.a.Birthday.IsNull() ? "" : (DateTime.Now.Year - o.a.Birthday.ConvertToDateTime().Year).ToString(),
                 IDNumber = o.a.IDNumber,
                 OldCertificateNo = o.a.OldCertificateNo,
                 Job = o.a.Job,
                 Title4Technical = o.a.Title4Technical,
                 ExamType = o.a.ExamType,
                 Industry = o.a.Industry,
                 City = o.a.City,
                 EnterpriseName = o.b.EnterpriseName,
                 CheckStatus = o.c.Count() == 0 ? "未审核" : o.c.First().PassStatus ? "审核通过" : "审核不通过",
                 CheckDate = o.c.Count() == 0 ? "" : o.c.First().CreateDate.ConvertToDateString(),
                 PhotoStatus = o.d.Count() == 0 ? "未拍照" : "已拍照"
             }).ToList();
            JqGridData dataResult = ConvertIListToJqGridData(param.page, param.rows, totalCount, employeeListJsonResult);
            return Json(dataResult, JsonRequestBehavior.AllowGet);
        }
        public class GetEmployeeListForJqgridJsonResult
        {
            public int Id { get; set; }
            public string EmployeeName { get; set; }
            public string Sex { get; set; }
            public string Age { get; set; }
            public string IDNumber { get; set; }
            public string OldCertificateNo { get; set; }
            public string EnterpriseName { get; set; }
            public string Job { get; set; }
            public string Title4Technical { get; set; }
            public string ExamType { get; set; }
            public string Industry { get; set; }
            public string City { get; set; }
            public string CheckStatus { get; set; }
            public string CheckDate { get; set; }
            public string PhotoStatus { get; set; }
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
        public class CheckEmployeeListParam
        {
            public List<int> rpEmployeeIdList { get; set; }
            public bool inValidityDate { get; set; }
            public bool annualSafetyTraining { get; set; }
            public bool notBadBehavior { get; set; }
            public bool trainingWith24Hours { get; set; }
            public bool delayConditions { get; set; }
            public bool passStatus { get; set; }
            public string checkedMark { get; set; }

        }
        public JsonResult CheckEmployeeList(string strData)
        {
            ResultMessage resultMessage = new ResultMessage();
            try
            {
                CheckEmployeeListParam param = strData.JSONStringToObj<CheckEmployeeListParam>();
                Sys_Account account = base.LoginAccount as Sys_Account;
                IRP_WorkFlowCtrl workFlowCtrl = new RP_WorkFlowCtrl(account);
                workFlowCtrl.CheckEmployeeData(param.inValidityDate, param.annualSafetyTraining, param.notBadBehavior, param.trainingWith24Hours, param.delayConditions, param.passStatus, param.checkedMark, param.rpEmployeeIdList);
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
            public int rpEmployeeId { get; set; }
            public string fileInBase64 { get; set; }
        }
        public JsonResult SaveEmployeePhoto(SaveEmployeePhotoParam param)
        {
            FileStream fs = null;
            MemoryStream ms = null;
            try
            {
                Sys_Account account = base.LoginAccount as Sys_Account;
                IRP_EmployeeCtrl rpEmployeeCtrl = new RP_EmployeeCtrl(account);
                Biz_RP_EmployeeRegistration rpEmployee = rpEmployeeCtrl.GetRP_EmployeeRegistrationById(param.rpEmployeeId);

                byte[] arrFileData = Convert.FromBase64String(param.fileInBase64);
                ms = new MemoryStream(arrFileData);

                string rootFolder = base.getFileRootDirectory();
                string fileName = string.Format("{0}_{1}.png", rpEmployee.IDNumber, param.rpEmployeeId);//文件名

                string relativePath = string.Format(@"\{0}\{1}\{2}", "继续教育文件", DateTime.Now.ToString("yyyy-MM-dd"), fileName);//数据库相对路径
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
                rpEmployeeCtrl.SaveRPEmployeeFile(param.rpEmployeeId, "登记照", fileName, relativePath);
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

        #region 获取登记照
        public JsonResult GetRPEmployeePhoto(int rpEmployeeId)
        {
            try
            {
                Sys_Account account = base.LoginAccount as Sys_Account;
                IRP_EmployeeCtrl rpEmployeeCtrl = new RP_EmployeeCtrl(account);
                Biz_RP_EmployeeFile rpEmployeePhoto = rpEmployeeCtrl.GetRPEmployeePhoto(rpEmployeeId);
                if (rpEmployeePhoto == null)
                {
                    throw new Exception("未上传照片");
                }
                string rootFolder = base.getFileRootDirectory();
                string absolutePath = string.Format(@"{0}\{1}", rootFolder.TrimEnd('\\'), rpEmployeePhoto.FilePath.TrimStart('\\'));//文件绝对路径
                if (!System.IO.File.Exists(absolutePath))
                {
                    throw new Exception("照片文件丢失");
                }
                string base64str = AppFn.ImgToBase64String(absolutePath);
                ResultMessage result = new ResultMessage() { IsSuccess = true, ErrorMessage = base64str };
                return Json(result);
            }
            catch (Exception ex)
            {
                ResultMessage result = new ResultMessage() { IsSuccess = false, ErrorMessage = ex.Message };
                return Json(result);
            }
        }
        #endregion

        public JsonResult GetCertificateDelayData(int rpEmployeeId)
        {
            try
            {
                Sys_Account account = base.LoginAccount as Sys_Account;
                IRP_EmployeeCtrl rpEmployeeCtrl = new RP_EmployeeCtrl(account);
                ICertificateCtrl certificateCtrl = new CertificateCtrl(account);
                IEnterpriseCtrl enterpriseCtrl = new EnterpriseCtrl(account);
                Biz_RP_EmployeeRegistration rpEmployee = rpEmployeeCtrl.GetRP_EmployeeRegistrationById(rpEmployeeId);
                Biz_RP_EmployeeDataCheckedRecord rpEmployeeDataCheckedRecord = rpEmployeeCtrl.GetRP_EmployeeDataCheckedRecordByRPEmployeeId(rpEmployeeId);
                Biz_Certificate certificate = certificateCtrl.GetCertificateByNo(rpEmployee.OldCertificateNo);
                Biz_Enterprise enterprise = enterpriseCtrl.GetEnterpriseById(rpEmployee.EnterpriseId);
                GetCertificateDelayDataResult result = new GetCertificateDelayDataResult()
                {
                    certificateNo = certificate.CertificateNo,
                    employeeName = rpEmployee.EmployeeName,
                    sex = rpEmployee.Sex,
                    iDNumber = rpEmployee.IDNumber,
                    birthday = rpEmployee.Birthday,
                    enterpriseName = enterprise.EnterpriseName,
                    job = rpEmployee.Job,
                    title4Technical = rpEmployee.Title4Technical,
                    examType = rpEmployee.ExamType,
                    certificateStartDate = certificate.StartCertificateDate.ConvertToDateString()
                };
                if (!rpEmployeeDataCheckedRecord.IsNull())
                {
                    result.inValidityDate = rpEmployeeDataCheckedRecord.InValidityDate;
                    result.annualSafetyTraining = rpEmployeeDataCheckedRecord.AnnualSafetyTraining;
                    result.notBadBehavior = rpEmployeeDataCheckedRecord.NotBadBehavior;
                    result.trainingWith24Hours = rpEmployeeDataCheckedRecord.TrainingWith24Hours;
                    result.delayConditions = rpEmployeeDataCheckedRecord.DelayConditions;
                    result.checkedMark = rpEmployeeDataCheckedRecord.CheckedMark;
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
            //public string industry { get; set; }
            public string examType { get; set; }
            public string certificateStartDate { get; set; }
            //public string certificateEndDate { get; set; }
            public bool? inValidityDate { get; set; }
            public bool? annualSafetyTraining { get; set; }
            public bool? notBadBehavior { get; set; }
            public bool? trainingWith24Hours { get; set; }
            public bool? delayConditions { get; set; }
            public string checkedMark { get; set; }
        }

    }
}
