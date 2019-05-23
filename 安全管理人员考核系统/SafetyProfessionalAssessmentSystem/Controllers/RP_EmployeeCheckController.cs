using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Model;
using BLL;
using Library.baseFn;
using System.IO;

namespace SafetyProfessionalAssessmentSystem.Controllers
{
    [AuthorizeFilter]
    public class RP_EmployeeCheckController : BaseController
    {
        //
        // GET: /人员成绩审核/

        public ActionResult Index()
        {
            return View();
        }

        #region 根据查询条件获取jqgrid人员信息
        public class GetEmployeeListForJqgridParam
        {
            public string EmployeeName { get; set; }
            public string EnterpriseName { get; set; }
            public string TrainingInstitutionName { get; set; }
            public string IdNumber { get; set; }
            public string OldCertificateNo { get; set; }
            public string ExamType { get; set; }
            public string CheckStatus { get; set; }
            public string CheckDateBegin { get; set; }
            public string CheckDateEnd { get; set; }

            public int page { get; set; }
            public int rows { get; set; }
            public string sidx { get; set; }//排序字段名称
            public string sord { get; set; }//排序方式asc desc
        }
        public JsonResult GetEmployeeListForJqgrid(GetEmployeeListForJqgridParam param)
        {
            int totalCount = 0;
            Sys_Account account = LoginAccount as Sys_Account;
            List<string> cityList = base.LoginDataPermissionDetailList.Select(p => p.DetailName).ToList();
            IEnterpriseCtrl enterpriseCtrl = new EnterpriseCtrl(account);
            IRP_EmployeeCtrl rpEmployeeCtrl = new RP_EmployeeCtrl(account);
            ITrainingInstitutionCtrl trainingInstitutionCtrl = new TrainingInstitutionCtrl(account);
            IAccountCtrl accountCtrl = new AccountCtrl(account);

            List<Biz_RP_EmployeeRegistration> employeeList = rpEmployeeCtrl.GetEmployeeListForViewStudyRecordCheck(param.EmployeeName, param.EnterpriseName, param.TrainingInstitutionName, param.IdNumber,
               param.OldCertificateNo, param.ExamType, param.CheckStatus, param.CheckDateBegin, param.CheckDateEnd, param.page, param.rows, ref totalCount, cityList);

            List<Biz_RP_EmployeeCheckedRecord> employeeCheckedRecordList = rpEmployeeCtrl.GetEmployeeCheckedRecordByEmployeeIdList(employeeList.Select(p => p.Id).ToList());
            List<Biz_Enterprise> enterpriseList = enterpriseCtrl.GetEnterpriseInfoByIdList(employeeList.Select(p => p.EnterpriseId).ToList());
            List<Biz_TrainingInstitution> trainingInstitution = trainingInstitutionCtrl.GetTrainingInstitutionByIdList(employeeList.Select(p => p.TrainingInstitutionId).ToList());
            List<RP_EmployeeCtrl.StudyRecord> studyRecordList = rpEmployeeCtrl.GetOneYearGetOnlineStudyRecord(employeeList.Select(p => p.IDNumber).ToList());

            List<GetEmployeeListForJqgridJsonResult> employeeListJsonResult =
                employeeList.Join(enterpriseList, a => a.EnterpriseId, b => b.Id, (a, b) => new { a, b })
                           .GroupJoin(trainingInstitution, o => o.a.TrainingInstitutionId, c => c.Id, (o, c) => new { o.a, o.b, c = c.FirstOrDefault() })
                           .GroupJoin(employeeCheckedRecordList, o => o.a.Id, d => d.EmployeeId, (o, d) => new { o.a, o.b, o.c, d = d.OrderByDescending(t => t.CreateDate).FirstOrDefault() })
            .Join(studyRecordList, o => o.a.IDNumber, e => e.IDNumber, (o, e) => new { o.a, o.b, o.c, o.d, e })
            .Select(o => new GetEmployeeListForJqgridJsonResult()
                         {
                             EmployeeId = o.a.Id,
                             EmployeeName = o.a.EmployeeName,
                             Sex = o.a.Sex,
                             IDNumber = o.a.IDNumber,
                             City = o.a.City,
                             TrainingInstitutionName = o.c.IsNull() ? "" : o.c.InstitutionName,
                             EnterpriseName = o.b.EnterpriseName,
                             ExamType = o.a.ExamType,
                             OldCertificateNo = o.a.OldCertificateNo,
                             TotalHours = o.e.TotalHours.ToString(),
                             SimulatedExamMaxCore = o.e.SimulatedExamMaxCore.ToString(),
                             CheckStatus = o.d.IsNull() ? "未审核" : o.d.PassStatus ? "审核通过" : "审核不通过",
                             CheckDate = o.d.IsNull() ? "" : o.d.CreateDate.ToString("yyyy-MM-dd"),
                             CheckUserName = o.d.IsNull() ? "" : accountCtrl.GetUserName(o.d.CreateById)
                         }).ToList();

            JqGridData dataResult = ConvertIListToJqGridData(param.page, param.rows, totalCount, employeeListJsonResult);
            return Json(dataResult, JsonRequestBehavior.AllowGet);
        }



        public class GetEmployeeListForJqgridJsonResult
        {
            public int EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public string Sex { get; set; }
            public string IDNumber { get; set; }
            public string City { get; set; }
            public string TrainingInstitutionName { get; set; }
            public string EnterpriseName { get; set; }
            public string ExamType { get; set; }
            public string OldCertificateNo { get; set; }
            public string TotalHours { get; set; }
            public string SimulatedExamMaxCore { get; set; }
            public string CheckStatus { get; set; }
            public string CheckDate { get; set; }
            public string CheckUserName { get; set; }

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

        #region 审核人员
        public class CheckEmployeeListParam
        {
            public List<int> employeeIdList { get; set; }
            public bool passFlag { get; set; }
            public string checkedMark;
        }
        public JsonResult CheckEmployeeList(string strData)
        {
            ResultMessage resultMessage = new ResultMessage();
            try
            {
                CheckEmployeeListParam para = strData.JSONStringToObj<CheckEmployeeListParam>();
                Sys_Account account = base.LoginAccount as Sys_Account;
                IRP_WorkFlowCtrl workFlowCtrl = new RP_WorkFlowCtrl(account);
                workFlowCtrl.CheckRPEmployee(para.employeeIdList, para.passFlag, para.checkedMark);
                resultMessage.IsSuccess = true;
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
