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
    public class ExamInvigilateController : BaseController
    {
        //
        // GET: /监考/

        public ActionResult Index()
        {
            return View();
        }
        #region 获取考生信息
        public JsonResult GetEmployeeInfo(int examPlanId, int examRoomId)
        {
            EmployeeInfoResult result = new EmployeeInfoResult()
            {
                EmployeeInfoList = new List<EmployeeInfo>(),
                ResultMsg = new ResultMessage()
                {
                    IsSuccess = false
                }
            };

            try
            {
                //请求考试核心 考生信息Url
                //考试核心站点地址
                string examCoreUrl = System.Web.Configuration.WebConfigurationManager.AppSettings["ExamCoreUrl"];

                result.GetExamInvigilateInfoUrl = examCoreUrl + System.Web.Configuration.WebConfigurationManager.AppSettings["GetExamInvigilateInfoUrl"];
                result.RemoveBindingUrl = examCoreUrl + System.Web.Configuration.WebConfigurationManager.AppSettings["RemoveBindingUrl"];
                Sys_Account account = base.LoginAccount as Sys_Account;
                IExamManageCtrl examManageCtrl = new ExamManageCtrl(account);
                List<Biz_Employee> employeeList = examManageCtrl.GetEmployeeList(examPlanId, examRoomId);
                List<EmployeeInfo> employeeInfoList = employeeList.Select(p => new EmployeeInfo()
                {
                    EmployeeName = p.EmployeeName,
                    IDNumber = p.IDNumber,
                    PaperTypeList = examManageCtrl.GetPaperForExamTypeList(p.Id).Select(q => new PaperType()
                    {
                        PaperId = q.PaperId,
                        PaperTypeName = (q.PaperType == "SafetyKnowledgeExam" ? "安全知识考核" : q.PaperType == "ManagementAbilityExam" ? "管理能力考核" : "")
                    }).ToList()
                }).ToList();
                result.EmployeeInfoList = employeeInfoList;

                result.ResultMsg.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.ResultMsg.IsSuccess = false;
                result.ResultMsg.ErrorMessage = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public class EmployeeInfoResult
        {
            public List<EmployeeInfo> EmployeeInfoList { get; set; }
            public string GetExamInvigilateInfoUrl { get; set; }
            public string RemoveBindingUrl { get; set; }
            public ResultMessage ResultMsg { get; set; }
        }
        public class EmployeeInfo
        {
            public string EmployeeName { get; set; }
            public string IDNumber { get; set; }
            public List<PaperType> PaperTypeList { get; set; }
        }
        public class PaperType
        {
            public int PaperId { get; set; }
            public string PaperTypeName { get; set; }
        }
        #endregion

    }
}
