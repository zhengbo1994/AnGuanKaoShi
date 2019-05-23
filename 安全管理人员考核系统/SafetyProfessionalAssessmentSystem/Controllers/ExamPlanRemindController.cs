using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using BLL;
using Model;
using Library.baseFn;

namespace SafetyProfessionalAssessmentSystem.Controllers
{
    [AuthorizeFilter]
    public class ExamPlanRemindController : BaseController
    {
        //
        // GET: /考试计划总览/

        public ActionResult Index()
        {
            return View();
        }

        #region 获取考试计划列表
        public JsonResult GetExamPlanListForRemind()
        {
            IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(LoginAccount as Sys_Account);

            List<Biz_ExamPlanRecord> examPlanRecordList = workFlowCtrl.GetExamPlanListForRemind();

            List<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecord = workFlowCtrl.GetEmployeeForExamPlanRecordByExamPlanIdList(examPlanRecordList.Select(p => p.Id).ToList());

            List<GetExamPlanListForRemindResult> getExamPlanListForRemindResult = examPlanRecordList.Select(p => new GetExamPlanListForRemindResult
            {
                Id = p.Id.ToString(),
                Title = " | " + p.ExamPlanNumber + " | 共" + employeeForExamPlanRecord.Where(q=>q.ExamPlanRecordId==p.Id).Count()+ "人",
                Start = p.ExamDateTimeBegin.ToString("yyyy-MM-dd HH:mm"),
                End = p.ExamDateTimeEnd.ToString("yyyy-MM-dd HH:mm")
            }).ToList();

            return Json(getExamPlanListForRemindResult, JsonRequestBehavior.AllowGet);
        }
        public class GetExamPlanListForRemindResult
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string Start { get; set; }
            public string End { get; set; }
        }
        #endregion

        #region 根据考试计划ID获取考试明细
        public JsonResult GetExamPlanInfoForRemind(int examPlanId)
        {
            Sys_Account account = LoginAccount as Sys_Account;

            IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);

            IExaminationPointCtrl trainingInstitutionCtrl = new ExaminationPointCtrl(account);

            Biz_ExamPlanRecord examPlanRecord = workFlowCtrl.GetExamPlanRecordById(examPlanId);

            List<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordList = workFlowCtrl.GetEmployeeForExamPlanRecordByExamPlanId(examPlanRecord.Id);

            List<Biz_ExaminationRoom> examinationRoomList = trainingInstitutionCtrl
                .GetExaminationRoomByIdList(employeeForExamPlanRecordList.Select(p => p.ExamRoomId).ToList());

            List<Biz_ExaminationPoint> trainingInstitutionList = trainingInstitutionCtrl
                .GetExaminationPointByIdList(examinationRoomList.Select(p => p.ExaminationPointId).Distinct().ToList());

            string ExamRoomString = "";

            foreach (Biz_ExaminationPoint trainingInstitution in trainingInstitutionList)
            {
                List<Biz_ExaminationRoom> examinationRoomListForTrainingInstitution = examinationRoomList.Where(p => p.ExaminationPointId == trainingInstitution.Id).ToList();

                foreach (Biz_ExaminationRoom examinationRoom in examinationRoomListForTrainingInstitution)
                {
                    ExamRoomString += String.Join(" | ", trainingInstitution.InstitutionName + " - " + examinationRoom.ExamRoomName);
                }
            }

            GetExamPlanInfoForRemindResult getExamPlanInfoForRemindResult = new GetExamPlanInfoForRemindResult()
            {
                Id = examPlanRecord.Id.ToString(),
                ExamPlanNumber = examPlanRecord.ExamPlanNumber,
                EmployeeCount = employeeForExamPlanRecordList.Count,
                ExamPlanRoom = ExamRoomString,
                SubmitStatus = examPlanRecord.SubmitStatus ? "已提交" : "未提交",
                StartTime = examPlanRecord.ExamDateTimeBegin.ToString("yyyy-MM-dd HH:mm"),
                EndTime = examPlanRecord.ExamDateTimeEnd.ToString("yyyy-MM-dd HH:mm"),
            };




            //List<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecord = workFlowCtrl.GetEmployeeForExamPlanRecordByExamPlanIdList(examPlanRecordList.Select(p => p.Id).ToList());



            return Json(getExamPlanInfoForRemindResult, JsonRequestBehavior.AllowGet);
        }
        public class GetExamPlanInfoForRemindResult
        {
            public string Id { get; set; }
            public string ExamPlanNumber { get; set; }
            public int EmployeeCount { get; set; }
            public string ExamPlanRoom { get; set; }
            public string SubmitStatus { get; set; }
            public string StartTime { get; set; }
            public string EndTime { get; set; }
        }
        #endregion
    }
}
