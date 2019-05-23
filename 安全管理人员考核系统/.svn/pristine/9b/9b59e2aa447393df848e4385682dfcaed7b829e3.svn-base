using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Model;
using BLL;

namespace SafetyProfessionalAssessmentSystem.Controllers
{
    [AuthorizeFilter]
    public class ExamManageController : BaseController
    {
        //
        // GET: /考试列表/

        public ActionResult Index()
        {
            return View();
        }
        #region 获取考试计划列表
        public JsonResult GetExamInfoInExamingForJqgrid(ExamInfoPram param)
        {
            Sys_Account account = base.LoginAccount as Sys_Account;
            IExamManageCtrl examMangeCtrl = new ExamManageCtrl(account);
            List<string> cityList = base.LoginDataPermissionDetailList.Select(p => p.DetailName).ToList();
            List<Biz_ExamPlanRecord> examPlanList = examMangeCtrl.GetExamPlanRecordListInExaming(cityList);
            List<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordList = examMangeCtrl.GetEmployeeForExamPlanRecordListByExamPlanIdList(examPlanList.Select(p => p.Id).ToList());
            List<Biz_ExaminationRoom> examRoomList = examMangeCtrl.GetExamRoomByIdList(employeeForExamPlanRecordList.Select(p => p.ExamRoomId).ToList());
            List<Biz_ExaminationPoint> trainingInstitution = examMangeCtrl.GetTrainingInstitutionByRoomIdList(examRoomList.Select(p => p.Id).ToList());


            List<ExamInfoResult> examInfoResultList = employeeForExamPlanRecordList.GroupBy(p => new { p.ExamRoomId, p.ExamPlanRecordId }).Select(a => new
            {
                a.Key.ExamPlanRecordId,
                a.Key.ExamRoomId,
                employeeCount = a.Count()
            })
            .Join(examRoomList, b => b.ExamRoomId, c => c.Id, (b, c) => new { b, c })
            .Join(examPlanList, d => d.b.ExamPlanRecordId, e => e.Id, (d, e) => new { d, e })
            .Join(trainingInstitution, f => f.d.c.ExaminationPointId, g => g.Id, (f, g) => new ExamInfoResult()
            {
                ExamPlanId = f.e.Id,
                ExamCoreExamId = f.e.ExamCoreExamId,
                ExamPlanNumber = f.e.ExamPlanNumber,
                ExamDateTimeBegin = f.e.ExamDateTimeBegin.ToString("yyyy-MM-dd HH:mm:ss"),
                ExamDateTimeEnd = f.e.ExamDateTimeEnd.ToString("yyyy-MM-dd HH:mm:ss"),
                TrainingInstitutionId = g.Id,
                TrainingInstitutionName = g.InstitutionName,
                ExamRoomId = f.d.c.Id,
                ExamRoomName = f.d.c.ExamRoomName,
                ExamEmployeeCnt = f.d.b.employeeCount
            }
            ).ToList();
            int totalCount = examInfoResultList.Count();
            JqGridData dataResult = ConvertIListToJqGridData(param.page, param.rows, totalCount, examInfoResultList);
            return Json(dataResult, JsonRequestBehavior.AllowGet);
        }
        public class ExamInfoPram
        {
            public int page { get; set; }
            public int rows { get; set; }
        }
        public class ExamInfoResult
        {
            public int ExamPlanId { get; set; }
            public int ExamCoreExamId { get; set; }
            public string ExamPlanNumber { get; set; }
            public string ExamDateTimeBegin { get; set; }
            public string ExamDateTimeEnd { get; set; }
            public int TrainingInstitutionId { get; set; }
            public string TrainingInstitutionName { get; set; }
            public int ExamRoomId { get; set; }
            public string ExamRoomName { get; set; }
            public int? ExamEmployeeCnt { get; set; }
        }
        #endregion
    }
}
