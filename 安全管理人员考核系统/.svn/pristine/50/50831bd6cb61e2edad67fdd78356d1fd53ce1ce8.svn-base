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
    public class ExamPlanDetailViewController : BaseController
    {
        //
        // GET: /ExamPlanDetailView/

        public ActionResult Index(int examPlanId)
        {
            ViewBag.ExamPlanId = examPlanId;
            return View();
        }

        #region 获取考试计划详情
        public JsonResult GetExamPlanDetail(int examPlanId)
        {
            JsonResult result = new JsonResult();
            try
            {
                Sys_Account account = LoginAccount as Sys_Account;
                IExamManageCtrl examC = new ExamManageCtrl(account);
                ExamPlanDetails resultData = examC.GetExamPlanDetailsById(examPlanId, 8);
                result.Data = resultData;
            }
            catch (Exception ex)
            {
                ResultMessage err = new ResultMessage();
                err.IsSuccess = false;
                err.ErrorMessage = ex.Message;
                result.Data = err;
            }
            return result;
        }
        #endregion
    }
}
