using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Model;
using BLL;
using Library.baseFn;
using System.Data;
using System.Transactions;

namespace SafetyProfessionalAssessmentSystem.Controllers
{
    [AuthorizeFilter]
    public class CertificateDelayApplyController : BaseController
    {
        //
        // GET: /RP_EmployeeRegistration/
        public ActionResult Index()
        {
            return View();
        }

        #region 根据查询条件获取jqgrid人员信息
        public class GetEmployeeListForJqgridParam
        {
            public string employeeName { get; set; }
            public string iDNumber { get; set; }
            public string examType { get; set; }
            public string industry { get; set; }
            public string certificateNo { get; set; }
            public string trainingType { get; set; }
            public string workFlowStatus { get; set; }
            public string trainingInstitutionName { get; set; }
            public int page { get; set; }
            public int rows { get; set; }
            public string sidx { get; set; }//排序字段名称
            public string sord { get; set; }//排序方式asc desc

        }

        public JsonResult GetCertificateListForJqgrid(GetEmployeeListForJqgridParam param)
        {
            try
            {
                int totalCount = 0;
                Sys_Account account = LoginAccount as Sys_Account;
                ICertificateDelay_WorkFlowCtrl certificateDelayWorkFlowCtrl = new CertificateDelay_WorkFlowCtrl(account);
                ICertificateCtrl certificateCtrl = new CertificateCtrl(account);
                ITrainingInstitutionCtrl trainingInstitutionCtrl = new TrainingInstitutionCtrl(account);

                List<Biz_Certificate> certificateList = certificateCtrl.GetCertificateListForDelayApply(param.employeeName, param.iDNumber, param.certificateNo, param.trainingInstitutionName, param.workFlowStatus, param.page, param.rows, ref totalCount);
                List<int> certificateIdList = certificateList.Select(p => p.Id).ToList();
                List<CertificateDelay_WorkFlowCtrl.WorkFlowStatus> workFlowStatusList = certificateDelayWorkFlowCtrl.GetCurrentWorkFlowStatusByCertificateIdList(certificateIdList);
                certificateIdList = certificateList.Select(p => p.Id).ToList();
                List<Biz_CertificateDelayApplyRecord> certificateDelayApplyRecordList = certificateDelayWorkFlowCtrl.GetCertificateDelayApplyRecordListByCertificateIdList(certificateIdList);
                List<int> trainingInstitutionIdList = certificateDelayApplyRecordList.Select(p => p.TrainingInstitutionId).ToList();
                List<Biz_TrainingInstitution> trainingInstitutionList = trainingInstitutionCtrl.GetTrainingInstitutionByIdList(trainingInstitutionIdList);

                List<GetEmployeeListForJqgridJsonResult> employeeListJsonResult = certificateList
                    .GroupJoin(certificateDelayApplyRecordList, a => a.Id, b => b.CertificateId, (a, b) => new { a, b = b.FirstOrDefault() })
                    .GroupJoin(workFlowStatusList, o => o.a.Id, c => c.certificateId, (o, c) => new { o.a, o.b, c = c.FirstOrDefault() })
                    .GroupJoin(trainingInstitutionList, o => (o.b.IsNull() ? 0 : o.b.TrainingInstitutionId), d => d.Id, (o, d) => new { o.a, o.b, o.c, d = d.FirstOrDefault() })
                    .Select(o => new GetEmployeeListForJqgridJsonResult()
                 {
                     employeeId = o.b.IsNull() ? 0 : o.b.Id,
                     certificateId = o.a.Id,
                     certificateNo = o.a.CertificateNo,
                     employeeName = o.a.EmployeeName,
                     sex = o.a.Sex,
                     age = o.a.Birthday.IsNull() ? "" : (DateTime.Now.Year - o.a.Birthday.ConvertToDateTime().Year).ToString(),
                     iDNumber = o.a.IDNumber,
                     job = o.a.Job,
                     title4Technical = o.a.Title4Technical,
                     examType = o.a.ExamType,
                     industry = o.a.Industry,
                     startCertificateDate=o.a.StartCertificateDate.ConvertToDateString(),
                     endCertificateDate=o.a.EndCertificateDate.ConvertToDateString(),
                     trainingInstitutionName = o.d == null ? "" : o.d.InstitutionName,
                     currentStatus = o.c.IsNull() ? "" : o.c.WorkFlowStatusTag,
                     operationDate = o.c.IsNull() ? "" : o.c.CreateDate.ConvertToDateString()
                 }).ToList();
                JqGridData dataResult = ConvertIListToJqGridData(param.page, param.rows, totalCount, employeeListJsonResult);
                return Json(dataResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(base.GetEmptyJqGridResult(), JsonRequestBehavior.AllowGet);
            }

        }
        public class GetEmployeeListForJqgridJsonResult
        {
            public int certificateId { get; set; }
            public int employeeId { get; set; }
            public string employeeName { get; set; }
            public string sex { get; set; }
            public string age { get; set; }
            public string iDNumber { get; set; }
            //public string enterpriseName { get; set; }
            public string job { get; set; }
            public string title4Technical { get; set; }
            public string examType { get; set; }
            public string industry { get; set; }
            public string certificateNo { get; set; }
            public string startCertificateDate { get; set; }
            public string endCertificateDate { get; set; }
            public string trainingInstitutionName { get; set; }
            public string currentStatus { get; set; }
            public string operationDate { get; set; }

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

        #region 获取培训机构列表
        public JsonResult GetTrainingInstitutionList()
        {
            Sys_Account account = base.LoginAccount as Sys_Account;
            ITrainingInstitutionCtrl trainingInstitutionCtrl = new TrainingInstitutionCtrl(account);
            IEnterpriseCtrl enterpriseCtrl = new EnterpriseCtrl(account);
            string city = enterpriseCtrl.GetEnterpriseById(account.UserId).City;
            List<string> cityList = new List<string>() { city };
            List<GetTrainingInstitutionListResult> trainingInstitutionList = trainingInstitutionCtrl.GetTrainingInstitutionListByCityList(cityList).Select(p => new GetTrainingInstitutionListResult()
            {
                ItemText = p.InstitutionName,
                ItemValue = p.Id.ToString()
            }).ToList();
            return Json(trainingInstitutionList, JsonRequestBehavior.DenyGet);
        }
        public class GetTrainingInstitutionListResult
        {
            public string ItemText { get; set; }
            public string ItemValue { get; set; }
        }
        #endregion

        #region 证书延期提交
        public class SubmitParam
        {
            public int trainingInstitutionId { get; set; }
            public List<int> certificateIdList { get; set; }
            public string submitRemark { get; set; }
        }
        public JsonResult Submit(string strParam)
        {
            Sys_Account account = base.LoginAccount as Sys_Account;
            ResultMessage result = new ResultMessage() { IsSuccess = true };
            TransactionScope scope = CreateTransaction();
            try
            {
                SubmitParam param = strParam.JSONStringToObj<SubmitParam>();
                ICertificateDelay_WorkFlowCtrl certificateDelayWorkFlowCtrl = new CertificateDelay_WorkFlowCtrl(account);
                for (int i = 0; i < param.certificateIdList.Count; i++)
                {
                    certificateDelayWorkFlowCtrl.CertificateDelayApply(param.trainingInstitutionId, param.certificateIdList[i], param.submitRemark);
                }
                certificateDelayWorkFlowCtrl.SubmitCertificateDelayApply(param.certificateIdList);
                TransactionCommit(scope);
            }
            catch (Exception ex)
            {
                TransactionRollback(scope);
                result = new ResultMessage() { IsSuccess = false, ErrorMessage = ex.Message };
            }
            return Json(result);
        }
        #endregion
    }
}
