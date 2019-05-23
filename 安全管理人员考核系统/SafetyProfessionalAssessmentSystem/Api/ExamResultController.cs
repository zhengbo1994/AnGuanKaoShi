using BLL;
using Library.LogFn;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SafetyProfessionalAssessmentSystem.Controllers
{
    public class ExamResultController : BaseController
    {

        //
        // GET: /SetExamResult/

        public ActionResult Index()
        {
            return View();
        }
        public void SetExamResult(string value)
        {
            LogHelper loghelper = new LogHelper();

            try
            {
                loghelper.WriteInfo(this.GetType(), "设置考试成绩[SetExamResult] 参数{value:" + value + "}");
                JavaScriptSerializer jss = new JavaScriptSerializer();
                List<ExamResult> examResultList = jss.Deserialize<List<ExamResult>>(value);
                Sys_Account account = base.LoginAccount as Sys_Account;
                IWorkFlowCtrl workFlow = new WorkFlowCtrl(account);
                foreach (ExamResult examResult in examResultList)
                {
                    workFlow.SaveExamResult(examResult.ExamCoreExamId, examResult.IDNumber, examResult.PaperId, examResult.ExamPassedFlag, examResult.Score);
                }
            }
            catch (Exception ex)
            {
                loghelper.WriteLog(this.GetType(), ex);
                throw ex;
            }
        }
        public class ExamResult
        {
            public int ExamCoreExamId { get; set; }
            public string IDNumber { get; set; }
            public int PaperId { get; set; }
            public bool ExamPassedFlag { get; set; }
            public double Score { get; set; }
        }

    }
}
