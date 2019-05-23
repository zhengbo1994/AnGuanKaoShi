using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL;
using Library;
using Library.baseFn;
using Library.LogFn;
using Model;

namespace SafetyProfessionalAssessmentSystem.Controllers
{
    public class PrintAdmissionticketController : BaseController
    {
        //
        // GET: /PrintAdmissionticket/

        public ActionResult Index()
        {
            return ShowAdmissionticket();
        }

        #region 生成准考证
        public ResultMessage CreateAdmissionticket()
        {
           
            ResultMessage result = new ResultMessage();
            try
            {
                Sys_Account account = base.LoginAccount as Sys_Account;
                int employeeId = account.UserId;
                IExamManageCtrl emCtrl = new ExamManageCtrl(account);
                IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
                IEnterpriseCtrl enterpriseCtrl = new EnterpriseCtrl(account);
                IExaminationPointCtrl trainingInstitutionCtrl = new ExaminationPointCtrl(account);
                AdmissionticketInfo admissionticketInfo = emCtrl.GetAdmissionticketInfo(employeeId, workFlowCtrl, enterpriseCtrl, trainingInstitutionCtrl);

                string templatePath = Server.MapPath("/Templates/准考证打印模板.xls");
                string rootPath = getFileRootDirectory();
                string pdfFilePath = emCtrl.CreateNewAdmissionticketPDF(admissionticketInfo, templatePath, rootPath);
                result.IsSuccess = true;
                result.ErrorMessage = pdfFilePath;
            }
            catch (Exception ex)
            {
                LogHelper loghelper = new LogHelper();
                loghelper.WriteLog(this.GetType(), ex);
                result.IsSuccess = false;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        public FileResult ShowAdmissionticket()
        {
            ResultMessage result = CreateAdmissionticket();
            if (result.IsSuccess)
            {
                string fileName = result.ErrorMessage.Substring(result.ErrorMessage.LastIndexOf('\\') + 1);
                return File(AppFn.GetFileBytes(result.ErrorMessage), "application/pdf");
            }
            else
            {
                throw new Exception(result.ErrorMessage);
            }
        }

        public FileResult DownloadAdmissionticket()
        {
            ResultMessage result = CreateAdmissionticket();
            if (result.IsSuccess)
            {
                string fileName = result.ErrorMessage.Substring(result.ErrorMessage.LastIndexOf('\\') + 1);
                return File(AppFn.GetFileBytes(result.ErrorMessage), "*", fileName);
            }
            else
            {
                throw new Exception(result.ErrorMessage);
            }
        }
        #endregion
    
    }
}
