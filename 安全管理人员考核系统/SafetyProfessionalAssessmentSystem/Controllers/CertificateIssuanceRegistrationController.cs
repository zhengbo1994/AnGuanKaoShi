using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SafetyProfessionalAssessmentSystem.Controllers
{
    [AuthorizeFilter]
    public class CertificateIssuanceRegistrationController : BaseController
    {
        //
        // GET: /证书发放登记/

        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetExamPlanInCertificateIssuanceForJqgrid(GetExamPlanInCertificateIssuanceForJqgridParam param)
        {
            try
            {
                Sys_Account account = base.LoginAccount as Sys_Account;
                IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
                IExaminationPointCtrl trainingInstitutionCtrl = new ExaminationPointCtrl(account);
                int totalCount = 0;
                List<string> cityList = base.LoginDataPermissionDetailList.Select(p => p.DetailName).ToList();
                List<Biz_ExamPlanRecord> examplanList = workFlowCtrl.getExamPlanListInCertificateIssuance(param.examPlanNumber, param.trainingInsititutionName, param.issuanceStatus, param.issuanceDateTimeBegin, param.issuanceDateTimeEnd, param.page, param.rows, ref totalCount, cityList);
                List<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordList = workFlowCtrl.GetEmployeeForExamPlanRecordByExamPlanIdList(examplanList.Select(p => p.Id).ToList());
                List<Biz_ExaminationRoom> examRoomList = trainingInstitutionCtrl.GetExaminationRoomByIdList(employeeForExamPlanRecordList.Select(p => p.ExamRoomId).ToList());
                List<Biz_ExaminationPoint> trainingInstitutionList = trainingInstitutionCtrl.GetExaminationPointByIdList(examRoomList.Select(p => p.ExaminationPointId).ToList());
                //需要发证的人
                List<Biz_Employee> employeeList = workFlowCtrl.GetEmployeeListInCertificateIssuance(employeeForExamPlanRecordList.Select(p => p.EmployeeId).ToList());

                //发放记录
                List<Biz_EmployeeCertificateIssuanceRecord> employeeCertificateIssuanceRecordList = workFlowCtrl.GetEmployeeCertificateIssuanceRecordList(employeeForExamPlanRecordList.Select(p => p.EmployeeId).ToList());
                List<GetExamPlanInCertificateIssuanceForJqgridResult> GetExamPlanInCertificateIssuanceForJqgridResultList =
                              examplanList.Join(employeeForExamPlanRecordList, a => a.Id, b => b.ExamPlanRecordId, (a, b) => new { a, b })
                              .Join(examRoomList, c => c.b.ExamRoomId, d => d.Id, (c, d) => new { c, d })
                              .Join(trainingInstitutionList, e => e.d.ExaminationPointId, f => f.Id, (e, f) => new { e, f })
                              .Join(employeeList, g => g.e.c.b.EmployeeId, h => h.Id, (g, h) => new { g, h })
                              .GroupBy(p => new { p.g.e.c.a, p.g.f })
                              .Select(p => new { p.Key, CertificateTotal = p.Count() })
                              .GroupJoin(employeeCertificateIssuanceRecordList, i => new { TrainingInstitutionId = i.Key.f.Id, ExamPlanId = i.Key.a.Id }, j => new { TrainingInstitutionId = j.TrainingInstitutionId, ExamPlanId = j.ExamPlanRecordId }, (i, j) => new
                              {
                                  i,
                                  IssuancedCount = j.Count(),
                                  LastIssuanceDateTime = j.Count() == 0 ? "" : j.FirstOrDefault().CreateDate.ToString("yyyy-MM-dd"),
                                  Remark = j.Count() == 0 ? "" : j.FirstOrDefault().Remark

                              })
                              .Select(p => new GetExamPlanInCertificateIssuanceForJqgridResult()
                            {
                                ExamPlanId = p.i.Key.a.Id,
                                ExamPlanNumber = p.i.Key.a.ExamPlanNumber,
                                TrainingInsititutionId = p.i.Key.f.Id,
                                TrainingInsititutionName = p.i.Key.f.InstitutionName,
                                IssuanceStatus = p.i.CertificateTotal.ToString(),
                                IssuanceDateTime = p.LastIssuanceDateTime,
                                Remark = p.Remark
                            }).ToList();
                JqGridData dataResult = ConvertIListToJqGridData(param.page, param.rows, totalCount, GetExamPlanInCertificateIssuanceForJqgridResultList);
                return Json(dataResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public class GetExamPlanInCertificateIssuanceForJqgridParam
        {
            public string examPlanNumber { get; set; }
            public string trainingInsititutionName { get; set; }
            public string issuanceStatus { get; set; }
            public string issuanceDateTimeBegin { get; set; }
            public string issuanceDateTimeEnd { get; set; }
            public int page { get; set; }
            public int rows { get; set; }
        }
        public class GetExamPlanInCertificateIssuanceForJqgridResult
        {
            public int ExamPlanId { get; set; }
            public string ExamPlanNumber { get; set; }
            public int TrainingInsititutionId { get; set; }
            public string TrainingInsititutionName { get; set; }
            public string IssuanceStatus { get; set; }
            public string Remark { get; set; }
            public string IssuanceDateTime { get; set; }
        }
        //根据计划和学校发放
        public JsonResult IssuanceByExamPlan(int examPlanId, int trainingInsititutionId, string remark)
        {
            ResultMessage resultMessage = new ResultMessage();
            try
            {
                Sys_Account account = base.LoginAccount as Sys_Account;
                IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
                workFlowCtrl.IssuanceByExamPlan(examPlanId, trainingInsititutionId, remark);
                resultMessage.IsSuccess = true;
            }
            catch (Exception ex)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = ex.Message;
            }
            return Json(resultMessage);
        }
        //单个人员发放
        public JsonResult IssuanceByEmployee(int employeeId, int examPlanId, int trainingInsititutionId, string remark)
        {
            ResultMessage resultMessage = new ResultMessage();
            try
            {
                Sys_Account account = base.LoginAccount as Sys_Account;
                IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
                workFlowCtrl.IssuanceByEmployee(employeeId, examPlanId, trainingInsititutionId, remark);
                resultMessage.IsSuccess = true;
            }
            catch (Exception ex)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = ex.Message;
            }
            return Json(resultMessage);
        }
    }
}
