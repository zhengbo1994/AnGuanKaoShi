using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
using BLL;
using Library.baseFn;

namespace SafetyProfessionalAssessmentSystem.Controllers
{
    [AuthorizeFilter]
    public class ExamPlanInformationController : BaseController
    {
        //
        // GET: /考试计划查询/

        public ActionResult Index()
        {
            return View();
        }
        #region 获取考试计划记录
        public JsonResult GetExamPlanRecoderListForJqgrid(GetExamPlanRecoderListForJqgridParam param)
        {
            int totalCount = 0;
            Sys_Account account = LoginAccount as Sys_Account;
            List<String> cityList = LoginDataPermissionDetailList.Select(p => p.DetailName).ToList();
            IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
            IStatisticalReportCtrl statisticalReportCtrl = new StatisticalReportCtrl(account);
            IEmployeeCtrl employeeCtrl = new EmployeeCtrl(account);
            //考试计划
            List<Biz_ExamPlanRecord> examPlanList = statisticalReportCtrl.GetExamPlanRecord(param.ExamPlanNumber,
                param.ExamDatetimeBegin, param.ExamDatetimeEnd, param.TrainingInstutionName, param.EmployeeName, param.IDNumber, param.ExamStatus, param.page, param.rows, ref totalCount, cityList);
            //考试计划分配表
            List<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordList = workFlowCtrl.GetEmployeeForExamPlanRecordByExamPlanIdList(examPlanList.Select(e => e.Id).ToList()).ToList();
            //考试结果表
            List<Biz_EmployeeExamResultRecord> employeeExamResultRecordList = employeeCtrl.GetEmployeeExamResultRecordListByEmployeeId(employeeForExamPlanRecordList.Select(p => p.EmployeeId).ToList());

            List<ExamPlanRecoderListResult> examPlanRecoderListResultList = examPlanList.GroupJoin(employeeForExamPlanRecordList, a => a.Id, b => b.ExamPlanRecordId, (a, b) => new { a, b })
                .GroupJoin(employeeExamResultRecordList, o => o.a.Id, c => c.ExamPlanRecordId, (o, c) => new { o.a, o.b, c })
                .Select(o => new ExamPlanRecoderListResult()
            {
                ExamPlanId = o.a.Id,
                ExamPlanNumber = o.a.ExamPlanNumber,
                ExamDatetimeBegin = o.a.ExamDateTimeBegin.ToString("yyyy-MM-dd HH:mm"),
                ExamDatetimeEnd = o.a.ExamDateTimeEnd.ToString("yyyy-MM-dd HH:mm"),
                ExamTotalCount = o.b.Count(),//分配考试总人数
                ExamCount = o.c.Count(),//参加考试的人数
                QualifiedCount = o.c.Sum(p => (p.FinalExamResult == "合格" ? 1 : 0)),
                ExamStatus = !o.a.SubmitStatus ? "未提交" : (DateTime.Now < o.a.ExamDateTimeBegin) ? "待考试" : (o.a.ExamDateTimeEnd < DateTime.Now) ? "已结束" : (o.a.ExamDateTimeBegin <= DateTime.Now && DateTime.Now <= o.a.ExamDateTimeEnd) ? "正在考试" : "未知状态"
            }).ToList();
            JqGridData dataResult = ConvertIListToJqGridData(param.page, param.rows, totalCount, examPlanRecoderListResultList);
            return Json(dataResult, JsonRequestBehavior.AllowGet);
        }
        public class GetExamPlanRecoderListForJqgridParam
        {
            public string ExamPlanNumber { get; set; }
            public string ExamDatetimeBegin { get; set; }
            public string ExamDatetimeEnd { get; set; }
            public string TrainingInstutionName { get; set; }
            public string EmployeeName { get; set; }
            public string IDNumber { get; set; }
            public string ExamStatus { get; set; }
            public int page { get; set; }
            public int rows { get; set; }
        }

        public class ExamPlanRecoderListResult
        {
            public int ExamPlanId { get; set; }
            public string ExamPlanNumber { get; set; }
            public string ExamDatetimeBegin { get; set; }
            public string ExamDatetimeEnd { get; set; }
            /// <summary>
            /// 考试分配总人数
            /// </summary>
            public int ExamTotalCount { get; set; }
            /// <summary>
            /// 实际参加考试的总人数
            /// </summary>
            public int ExamCount { get; set; }
            /// <summary>
            /// 未参加考试的总人数
            /// </summary>
            public int NoExamCount
            {
                get
                {
                    return ExamTotalCount - ExamCount;
                }
                set
                {
                    this.NoExamCount = value;
                }
            }
            /// <summary>
            /// 合格人数
            /// </summary>
            public int QualifiedCount { get; set; }
            /// <summary>
            ///不合格人数
            /// </summary>
            public int NotQualifiedCount
            {
                get
                {
                    return ExamCount - QualifiedCount;
                }
                set
                {
                    this.NotQualifiedCount = value;
                }
            }
            //考试状态
            public string ExamStatus { get; set; }

        }
        #endregion


        #region 获取考试计划记录子表  人员信息
        public class GetEmployeeForExamPlanRecoderListParam
        {
            public string ExamPlanNumber { get; set; }
            public string TrainingInstutionName { get; set; }
            public string EmployeeName { get; set; }
            public string IDNumber { get; set; }
            public string SubmitStatus { get; set; }
            public int page { get; set; }
            public int rows { get; set; }
        }
        public JsonResult GetEmployeeForExamPlanRecoderListForJqgrid(GetEmployeeForExamPlanRecoderListParam param)
        {
            try
            {
                List<GetEmployeeForExamPlanRecoderListResultItem> getEmployeeForExamPlanRecoderListResultItemList = new List<GetEmployeeForExamPlanRecoderListResultItem>();
                int totalCount = 0;

                Sys_Account account = LoginAccount as Sys_Account;
                List<String> cityList = LoginDataPermissionDetailList.Select(p => p.DetailName).ToList();
                IWorkFlowCtrl workCtrl = new WorkFlowCtrl(account);
                IEnterpriseCtrl enterpriseCtrl = new EnterpriseCtrl(account);
                IEmployeeCtrl employeeCtrl = new EmployeeCtrl(account);
                IExaminationPointCtrl trainingInstitutionCtrl = new ExaminationPointCtrl(account);
                List<Biz_Employee> employeeList = workCtrl.GetEmployeeForExamPlanRecord(param.ExamPlanNumber, param.TrainingInstutionName, param.EmployeeName, param.IDNumber, param.SubmitStatus, param.page, param.rows, ref totalCount, cityList);
                List<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordList = workCtrl.GetEmployeeForExamPlanRecordByEmployeeIdList(employeeList.Select(e => e.Id).ToList()).ToList();
                List<Biz_ExaminationRoom> examroomList = trainingInstitutionCtrl.GetExaminationRoomByIdList(employeeForExamPlanRecordList.Select(p => p.ExamRoomId).ToList());
                List<Biz_ExaminationPoint> trainingInstitutionList = trainingInstitutionCtrl.GetExaminationPointByIdList(examroomList.Select(p => p.ExaminationPointId).ToList());
                List<Biz_Enterprise> enterpriseList = enterpriseCtrl.GetEnterpriseInfoByIdList(employeeList.Select(p => p.EnterpriseId).ToList());
                List<Biz_EmployeeExamResultRecord> employeeExamResultRecordList = employeeCtrl.GetEmployeeExamResultRecordListByEmployeeId(employeeList.Select(p => p.Id).ToList());

                getEmployeeForExamPlanRecoderListResultItemList =
                employeeList.Join(employeeForExamPlanRecordList, a => a.Id, b => b.EmployeeId, (a, b) => new { a, b })
                .GroupJoin(examroomList, o => o.b.ExamRoomId, c => c.Id, (o, c) => new { o.a, o.b, c })
               .GroupJoin(trainingInstitutionList, o => o.c.FirstOrDefault().ExaminationPointId, d => d.Id, (o, d) => new { o.a, o.b, o.c, d })
               .GroupJoin(employeeExamResultRecordList, o => o.a.Id, f => f.EmployeeId, (o, f) => new { o.a, o.b, o.c, o.d, f })
                .Join(enterpriseList, o => o.a.EnterpriseId, e => e.Id, (o, e) => new { o.a, o.b, c = o.c.FirstOrDefault(), d = o.d.FirstOrDefault(), f = o.f.FirstOrDefault(), e })
                .Select(o => new GetEmployeeForExamPlanRecoderListResultItem()
                {
                    EmployeeId = o.a.Id,
                    ExamRoomName = !o.c.IsNull() ? o.c.ExamRoomName : "考场不存在",
                    TrainingInstitutionName = !o.d.IsNull() ? o.d.InstitutionName : "",
                    EmployeeName = o.a.EmployeeName,
                    Sex = o.a.Sex,
                    Age = (DateTime.Now.Year - o.a.Birthday.Year),
                    IDNumber = o.a.IDNumber,
                    Job = o.a.Job,
                    Title4Technical = o.a.Title4Technical,
                    Industry = o.a.Industry,
                    ExamType = o.a.ExamType,
                    EnterpriseName = o.e.EnterpriseName,
                    SafetyKnowledgeExamScore = (o.f.IsNull() || o.f.SafetyKnowledgeExamScore.IsNull()) ? "" : o.f.SafetyKnowledgeExamScore.ToString(),
                    ManagementAbilityExamScore = (o.f.IsNull() || o.f.ManagementAbilityExamScore.IsNull()) ? "" : o.f.ManagementAbilityExamScore.ToString(),
                    ActualOperationExamResult = (o.f.IsNull() || o.f.ActualOperationExamResult.IsNull()) ? "" : o.f.ActualOperationExamResult,
                    FinalExamResult = (o.f.IsNull() || o.f.IsNull()) ? "" : o.f.FinalExamResult,
                    ExamRegistrationNumber = o.b.ExamRegistrationNumber,
                    ExamSeatNumber = o.b.ExamSeatNumber.ToString()
                }).ToList();
                JqGridData dataResult = ConvertIListToJqGridData(param.page, param.rows, totalCount, getEmployeeForExamPlanRecoderListResultItemList);
                return Json(dataResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ResultMessage resultMessage = new ResultMessage() { IsSuccess = false, ErrorMessage = ex.Message };
                return Json(resultMessage, JsonRequestBehavior.AllowGet);
            }
        }



        public class GetEmployeeForExamPlanRecoderListResultItem
        {
            public int EmployeeId { get; set; }
            //public int TrainingInstitutionId { get; set; }
            public string TrainingInstitutionName { get; set; }
            // public int ExamRoomId { get; set; }
            public string ExamRoomName { get; set; }
            //public int EnterpriseId { get; set; }
            public string EnterpriseName { get; set; }
            public string EmployeeName { get; set; }
            public string Sex { get; set; }
            public int Age { get; set; }
            public string IDNumber { get; set; }
            public string Job { get; set; }
            public string Title4Technical { get; set; }
            public string Industry { get; set; }
            public string ExamType { get; set; }
            public string SafetyKnowledgeExamScore { get; set; }
            public string ManagementAbilityExamScore { get; set; }
            public string ActualOperationExamResult { get; set; }
            public string FinalExamResult { get; set; }
            public string ExamRegistrationNumber { get; set; }
            public string ExamSeatNumber { get; set; }

        }
        #endregion


    }
}
