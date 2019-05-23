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



namespace SafetyProfessionalAssessmentSystem.Controllers
{
    [AuthorizeFilter]
    public class ExaminationResultsAuditController : BaseController
    {
        //
        // GET: /考试结果审核/

        public ActionResult Index()
        {
            return View();
        }
        #region 获取科目列表
        public JsonResult GetEmployeeExamTypeList()
        {
            Sys_Account account = LoginAccount as Sys_Account;
            IWorkFlowCtrl employeeCtrl = new WorkFlowCtrl(account);
            List<GetEmployeeExamTypeListResult> employeeExamTypeList = employeeCtrl.GetEmployeeSubjectList().Select(p => new GetEmployeeExamTypeListResult()
            {
                ItemText = p.ItemText,
                ItemValue = p.ItemValue
            }).ToList();
            return Json(employeeExamTypeList, JsonRequestBehavior.AllowGet);
        }
        public class GetEmployeeExamTypeListResult
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

        #region 获取考试计划记录


        public JsonResult GetExamPlanRecoderListForJqgrid(GetExamPlanRecoderListForJqgridParam param)
        {
            int totalCount = 0;
            Sys_Account account = LoginAccount as Sys_Account;
            List<String> cityList = LoginDataPermissionDetailList.Select(p => p.DetailName).ToList();
            IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
            List<Biz_ExamPlanRecord> examPlanList = workFlowCtrl.GetExamPlanRecordInCheckExamResult(param.ExamPlanNumber,
                param.EmployeeName, param.IDNumber, param.Industry, param.ExamType, param.CheckStatus, param.page, param.rows, ref totalCount, cityList);
            List<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordList = workFlowCtrl.GetEmployeeForExamPlanRecordByExamPlanIdList(examPlanList.Select(e => e.Id).ToList()).ToList();
            List<Biz_EmployeeExamResultRecord> employeeExamResultRecordList = workFlowCtrl.GetEmployeeExamResultRecordByExamPlanIdList(examPlanList.Select(p => p.Id).ToList(), true);

            List<ExamPlanRecoderListResult> examPlanRecoderListResultList = examPlanList
                .GroupJoin(employeeForExamPlanRecordList, a => a.Id, b => b.ExamPlanRecordId, (a, b) => new { a, b })
                .GroupJoin(employeeExamResultRecordList, o => o.a.Id, c => c.ExamPlanRecordId, (o, c) => new { o.a, o.b, examPlanRecoderCount = c.Count() })
                .Select(o =>
            new ExamPlanRecoderListResult()
            {
                ExamPlanId = o.a.Id,
                ExamPlanNumber = o.a.ExamPlanNumber,
                ExamDatetimeBegin = o.a.ExamDateTimeBegin.ToString("yyyy-MM-dd HH:mm"),
                ExamDatetimeEnd = o.a.ExamDateTimeEnd.ToString("yyyy-MM-dd HH:mm"),
                ExamCount = (o.examPlanRecoderCount.ToString() + "/" + o.b.Count().ToString())
            }).ToList();
            JqGridData dataResult = ConvertIListToJqGridData(param.page, param.rows, totalCount, examPlanRecoderListResultList);
            return Json(dataResult, JsonRequestBehavior.AllowGet);
        }
        public class GetExamPlanRecoderListForJqgridParam
        {
            public string ExamPlanNumber { get; set; }
            public string EmployeeName { get; set; }
            public string IDNumber { get; set; }
            public string Industry { get; set; }
            public string ExamType { get; set; }
            public string CheckStatus { get; set; }
            public int page { get; set; }
            public int rows { get; set; }
        }

        public class ExamPlanRecoderListResult
        {
            public int ExamPlanId { get; set; }
            public string ExamPlanNumber { get; set; }

            public string ExamDatetimeBegin { get; set; }
            public string ExamDatetimeEnd { get; set; }
            public string ExamCount { get; set; }

        }
        #endregion

        #region 获取考试计划记录子表  人员信息
        public JsonResult GetEmployeeListInExamResultCheckForJqgrid(GetEmployeeForExamPlanRecoderListParam param)
        {
            List<EmployeeListInExamResultCheck> getEmployeeForExamPlanRecoderListResultItemList = new List<EmployeeListInExamResultCheck>();
            int totalCount = 0;

            Sys_Account account = LoginAccount as Sys_Account;
            List<String> cityList = LoginDataPermissionDetailList.Select(p => p.DetailName).ToList();
            IWorkFlowCtrl workCtrl = new WorkFlowCtrl(account);
            IEmployeeCtrl employeeCtrl = new EmployeeCtrl(account);

            List<Biz_Employee> employeeList = workCtrl.GetEmployeeListInExamResultCheck(param.ExamPlanId, param.EmployeeName, param.IDNumber, param.Industry, param.ExamType, param.CheckStatus, param.page, param.rows, ref totalCount);
            List<Biz_EmployeeExamResultRecord> employeeExamResultRecordList = employeeCtrl.GetEmployeeExamResultRecordListByEmployeeId(employeeList.Select(p => p.Id).ToList());
            List<Biz_EmployeeExamResultCheckedRecord> employeeExamResultCheckedRecord = workCtrl.GetEmployeeExamResultCheckedRecordListByEmployeeIdList(employeeList.Select(p => p.Id).ToList(), null);
            getEmployeeForExamPlanRecoderListResultItemList = employeeList.GroupJoin(employeeExamResultRecordList, a => a.Id, b => b.EmployeeId, (a, b) => new { a, b = b.FirstOrDefault() })
                .GroupJoin(employeeExamResultCheckedRecord, o => o.a.Id, c => c.EmployeeId, (o, c) => new { o.a, o.b, c = c.FirstOrDefault() }).Select(o => new EmployeeListInExamResultCheck()
            {
                EmployeeId = o.a.Id,
                EmployeeName = o.a.EmployeeName,
                Sex = o.a.Sex,
                Age = DateTime.Now.Year - o.a.Birthday.Year,
                IDNumber = o.a.IDNumber,
                Industry = o.a.Industry,
                ExamType = o.a.ExamType,
                SafetyKnowledgeExamResult = o.b == null ? "" : o.b.SafetyKnowledgeExamResult,
                SafetyKnowledgeExamScore = (o.b == null || o.b.SafetyKnowledgeExamScore == null) ? "" : o.b.SafetyKnowledgeExamScore.ToString(),
                ManagementAbilityExamResult = o.b == null ? "" : o.b.ManagementAbilityExamResult,
                ManagementAbilityExamScore = (o.b == null || o.b.ManagementAbilityExamScore == null) ? "" : o.b.ManagementAbilityExamScore.ToString(),
                FieldExamResult = o.b == null ? "" : o.b.ActualOperationExamResult,
                FinalExamResult = o.b == null ? "" : o.b.FinalExamResult,
                CheckDate = o.c == null ? "" : o.c.CreateDate.ToString("yyyy-MM-dd HH:mm"),
                CheckStatus = o.c == null ? "" : o.c.CheckedStatus ? "审核通过" : "审核不通过",
            }).ToList();
            JqGridData dataResult = ConvertIListToJqGridData(param.page, param.rows, totalCount, getEmployeeForExamPlanRecoderListResultItemList);
            return Json(dataResult, JsonRequestBehavior.AllowGet);
        }

        public class GetEmployeeForExamPlanRecoderListParam
        {
            public int ExamPlanId { get; set; }
            public string EmployeeName { get; set; }
            public string IDNumber { get; set; }
            public string Industry { get; set; }
            public string ExamType { get; set; }
            public string CheckStatus { get; set; }
            public int page { get; set; }
            public int rows { get; set; }
        }

        public class EmployeeListInExamResultCheck
        {
            public int EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public string Sex { get; set; }
            public int Age { get; set; }
            public string IDNumber { get; set; }
            public string Industry { get; set; }
            public string ExamType { get; set; }
            public string SafetyKnowledgeExamResult { get; set; }
            public string SafetyKnowledgeExamScore { get; set; }
            public string ManagementAbilityExamResult { get; set; }
            public string ManagementAbilityExamScore { get; set; }
            public string FieldExamResult { get; set; }
            public string FinalExamResult { get; set; }
            public string CheckDate { get; set; }
            public string CheckStatus { get; set; }
        }
        #endregion


        #region 获取考试结果
        public JsonResult GetExamResult(int employeeId)
        {
            GetExamResultItem getExamResultItem = new GetExamResultItem();
            try
            {
                Sys_Account account = LoginAccount as Sys_Account;
                IWorkFlowCtrl workFlow = new WorkFlowCtrl(account);
                IEmployeeCtrl employeeCtrl = new EmployeeCtrl(account);
                List<int> employeeIdList = new List<int> { employeeId };
                Biz_EmployeeExamResultRecord employeeExamResultRecord = employeeCtrl.GetEmployeeExamResultRecordListByEmployeeId(employeeIdList).FirstOrDefault();
                if (employeeExamResultRecord != null)
                {
                    getExamResultItem.EmployeeId = employeeExamResultRecord.EmployeeId;
                    getExamResultItem.SafetyKnowledgeExamResult = employeeExamResultRecord.SafetyKnowledgeExamResult;
                    getExamResultItem.ManagementAbilityExamResult = employeeExamResultRecord.ManagementAbilityExamResult;
                    getExamResultItem.SafetyKnowledgeExamScore = employeeExamResultRecord.SafetyKnowledgeExamScore.IsNull() ? "" : employeeExamResultRecord.SafetyKnowledgeExamScore.ToString();
                    getExamResultItem.ManagementAbilityExamScore = employeeExamResultRecord.ManagementAbilityExamScore.IsNull() ? "" : employeeExamResultRecord.ManagementAbilityExamScore.ToString();
                    getExamResultItem.FieldExamResult = employeeExamResultRecord.ActualOperationExamResult;
                    getExamResultItem.ImgFileList = workFlow.GetEmployeeExamResultRecordFileListByEmployeeExamResultRecordId(employeeExamResultRecord.Id).Select(p => new ImgFile()
                    {
                        Id = p.Id,
                        fileKey = p.FileKey
                    }).ToList();
                }
            }
            catch (Exception ex)
            {

            }
            return Json(getExamResultItem, JsonRequestBehavior.AllowGet);
        }
        public class GetExamResultItem
        {
            public int EmployeeId { get; set; }
            public int ExamPlanRecordId { get; set; }
            //安全知识考试结果
            public string SafetyKnowledgeExamResult { get; set; }

            //管理能力考试结果
            public string ManagementAbilityExamResult { get; set; }

            public string SafetyKnowledgeExamScore { get; set; }
            public string ManagementAbilityExamScore { get; set; }
            //实操考试结果
            public string FieldExamResult { get; set; }
            public List<ImgFile> ImgFileList { get; set; }

        }
        public class ImgFile
        {
            public string fileKey { get; set; }
            public int Id { get; set; }
        }
        #endregion

        #region 获取图片文件
        public FileResult GetExamResultFile(string imgId)
        {
            int employeeExamResultImgFileId = imgId.ConvertToInt32();
            Sys_Account account = LoginAccount as Sys_Account;
            IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
            Biz_EmployeeExamResultRecordFile employeeExamResultRecordFile = workFlowCtrl.GetEmployeeExamResultImgFileById(employeeExamResultImgFileId);
            string FilePath = GetAbsolutePath(employeeExamResultRecordFile.FilePath);
            return File(FilePath, "*");
        }
        protected string RootFolderPath
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["FileRootDirectory"];
            }
        }
        //通过相对路径返回文件绝对路径
        protected string GetAbsolutePath(string relativePath)
        {
            string absolutePath = string.Format("{0}\\{1}", RootFolderPath.TrimEnd('\\'), relativePath.TrimStart('\\'));
            FileInfo fileinfo = new FileInfo(absolutePath);
            if (!fileinfo.Directory.Exists)
            {
                fileinfo.Directory.Create();
            }
            //if (fileinfo.Exists)
            //{
            //    fileinfo.Delete();
            //}

            return absolutePath;
        }
        #endregion

        #region 根据考试计划审核考试结果
        public JsonResult CheckByExamPlan(CheckByExamPlanParam param)
        {
            ResultMessage resultMessage = new ResultMessage();
            resultMessage.IsSuccess = true;
            TransactionScope scope = base.CreateTransaction();
            try
            {
                Sys_Account account = LoginAccount as Sys_Account;
                IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
                workFlowCtrl.CheckByExamPlan(param.ExamPlanId, param.PassStatus, param.CheckedMark, param.StartCertificateDate);
                base.TransactionCommit(scope);
            }
            catch (Exception ex)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = ex.Message;
                base.TransactionRollback(scope);
            }
            return Json(resultMessage, JsonRequestBehavior.AllowGet);
        }
        public class CheckByExamPlanParam
        {
            public int ExamPlanId { get; set; }
            public bool PassStatus { get; set; }
            public string CheckedMark { get; set; }
            public string StartCertificateDate { get; set; }
        }
        #endregion

        #region 根据个人 审核考试结果
        public JsonResult CheckByEmployee(CheckByEmployeeParam param)
        {
            ResultMessage resultMessage = new ResultMessage();
            resultMessage.IsSuccess = true;
            TransactionScope scope = base.CreateTransaction();
            try
            {
                Sys_Account account = LoginAccount as Sys_Account;
                IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
                workFlowCtrl.CheckByEmployee(param.EmployeeId, param.PassStatus, param.CheckedMark, param.StartCertificateDate);
                base.TransactionCommit(scope);
            }
            catch (Exception ex)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = ex.Message;
                base.TransactionRollback(scope);
            }
            return Json(resultMessage, JsonRequestBehavior.AllowGet);
        }

        public class CheckByEmployeeParam
        {
            public int EmployeeId { get; set; }
            public bool PassStatus { get; set; }
            public string CheckedMark { get; set; }
            public string StartCertificateDate { get; set; }
        }
        #endregion
    }
}
