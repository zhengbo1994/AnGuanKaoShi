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
    public class DataAuditController : BaseController
    {
        //
        // GET: /DataAudit/

        public ActionResult Index()
        {
            return View();
        }
        #region 根据查询条件获取jqgrid人员信息
        public JsonResult GetEmployeeDataAduitListForJqgrid(QueryParam param)
        {
            List<EmployeeDataAduitResult> employeeDataAduitListResult = new List<EmployeeDataAduitResult>();
            int totalCount = 0;

            Sys_Account account = LoginAccount as Sys_Account;

            try
            {
                IWorkFlowCtrl workCtrl = new WorkFlowCtrl(account);
                IEnterpriseCtrl entrpriseCtrl = new EnterpriseCtrl(account);
                ITrainingInstitutionCtrl trainingInstitutionCtrl = new TrainingInstitutionCtrl(account);



                //资料审核人员信息
                List<Biz_Employee> employeeDataAduitList = workCtrl.GetEmployeeDataAduitList(param.EmployeeName,
                    param.EnterpriseName, param.Industry, param.ExamType, param.AuditDateBegin, param.AuditDateEnd, param.AuditStatus, param.page, param.rows, param.sidx, param.sord, ref totalCount);
                //人员已分配记录
                List<Biz_EmployeeCheckedRecord> employeeDataAduitRecordList = workCtrl.GetEmployeeCheckedRecordByEmployeeIdList(employeeDataAduitList.Select(p => p.Id).ToList());

                //相关培训机构信息
                IAccountCtrl accountCtrl = new AccountCtrl();
                List<int> accountIdList = employeeDataAduitRecordList.Select(p => p.CreateById).ToList();
                List<int> trainingInstitutionIdList = accountCtrl.GetAccountByAccountIdList(accountIdList).Select(p => p.UserId).ToList();
                List<Biz_TrainingInstitution> trainingInstitutionList = trainingInstitutionCtrl.GetTrainingInstitutionByIdList(trainingInstitutionIdList);
                //相关企业信息
                List<Biz_Enterprise> enterpriseList = entrpriseCtrl.GetEnterpriseInfoByIdList(employeeDataAduitList.Select(p => p.EnterpriseId).ToList());

                employeeDataAduitListResult = employeeDataAduitList.GroupJoin(employeeDataAduitRecordList, a => a.Id, b => b.EmployeeId, (a, b) => new EmployeeDataAduitResult()
                {
                    EmployeeId = a.Id,
                    EmployeeName = a.EmployeeName,
                    Sex = a.Sex,
                    Age = (DateTime.Now.Year - a.Birthday.Year).ToString(),
                    IDNumber = a.IDNumber,
                    Job = a.Job,
                    Title4Technical = a.Title4Technical,
                    ExamType = a.ExamType,
                    Industry = a.Industry,
                    EnterpriseId = a.EnterpriseId,
                   
                    InstitutionId = (b.Count() == 0 ? -1 : b.Max(o => accountCtrl.GetAccountByAccountId(o.CreateById).UserId)),
                    AuditStatus = (b.Count()==0?"未审核":b.Max(o=>o.PassStatus?"审核通过":"审核未通过")),
                    AuditDate=(b.Count()==0?"":b.Max(o=>o.CreateDate.ToString("yyyy-MM-dd")))
                }).ToList();


                //取考核点信息
                employeeDataAduitListResult = employeeDataAduitListResult.GroupJoin(trainingInstitutionList, a => a.InstitutionId, b => b.Id, (a, b) => new EmployeeDataAduitResult()
                {
                    EmployeeId = a.EmployeeId,
                    EmployeeName = a.EmployeeName,
                    Sex = a.Sex,
                    Age = a.Age,
                    IDNumber = a.IDNumber,
                    Job = a.Job,
                    Title4Technical = a.Title4Technical,
                    ExamType = a.ExamType,
                    Industry = a.Industry,
                    EnterpriseId = a.EnterpriseId,
                    InstitutionId = a.InstitutionId,
                    AuditUnit = b.Max(o => o.InstitutionName),
                    AuditStatus=a.AuditStatus,
                    AuditDate=a.AuditDate
                }).ToList();
                //取企业信息
                employeeDataAduitListResult = employeeDataAduitListResult.Join(enterpriseList, a => a.EnterpriseId, b => b.Id, (a, b) => new EmployeeDataAduitResult()
                {
                    EmployeeId = a.EmployeeId,
                    EmployeeName = a.EmployeeName,
                    Sex = a.Sex,
                    Age = a.Age,
                    IDNumber = a.IDNumber,
                    Job = a.Job,
                    Title4Technical = a.Title4Technical,
                    ExamType = a.ExamType,
                    Industry = a.Industry,
                    EnterpriseId = a.EnterpriseId,
                    InstitutionId = a.InstitutionId,
                    AuditUnit = a.AuditUnit,
                    EnterpriseName = b.EnterpriseName,
                    AuditStatus = a.AuditStatus,
                    AuditDate=a.AuditDate
                }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            JqGridData dataResult = ConvertIListToJqGridData(param.page, param.rows, totalCount, employeeDataAduitListResult);

            return Json(dataResult, JsonRequestBehavior.AllowGet);
        }

        public class QueryParam
        {
            public string EmployeeName { get; set; }
            public string EnterpriseName { get; set; }
            public string Industry { get; set; }
            public string ExamType { get; set; }
            public string AuditStatus { get; set; }
            public string AuditDateBegin { get; set; }
            public string AuditDateEnd { get; set; }
            public int page { get; set; }
            public int rows { get; set; }
            public string sidx { get; set; }//排序字段名称
            public string sord { get; set; }//排序方式asc desc

        }

        public class EmployeeDataAduitResult
        {
            public int EmployeeId { get; set; }
            // 人员名称
            public string EmployeeName { get; set; }
            // 性别
            public string Sex { get; set; }
            //年龄
            public string Age { get; set; }
            // 身份证号
            public string IDNumber { get; set; }

            //企业ID
            public int EnterpriseId { get; set; }
            // 所属单位
            public string EnterpriseName { get; set; }
            public int InstitutionId { get; set; }
            //考核点名称
            public string AuditUnit { get; set; }
            // 职位
            public string Job { get; set; }
            // 技术职称
            public string Title4Technical { get; set; }
            // 报考科目
            public string ExamType { get; set; }
            //报考行业
            public string Industry { get; set; }

            public string AuditStatus { get; set; }
            public string AuditDate { get; set; }

        }
        #endregion

        #region 获取科目列表
        public JsonResult GetEmployeeExamTypeList()
        {
            Sys_Account account = LoginAccount as Sys_Account;
            IWorkFlowCtrl employeeCtrl = new WorkFlowCtrl(account);
            List<GetEmployeeExamTypeListResult> employeeExamTypeList = employeeCtrl.GetEmployeeSubjectList().Select(p => new GetEmployeeExamTypeListResult()
            {
                ItemText = p.ItemText,
                ItemValue = p.ItemValue
            }).ToList();
            return Json(employeeExamTypeList, JsonRequestBehavior.AllowGet);
        }
        public class GetEmployeeExamTypeListResult
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

        #region 审核
        public JsonResult EmployeeDataCheck(CheckMultiParam checkParam)
        {
            ResultMessage resultMessage = new ResultMessage();
            resultMessage.IsSuccess = true;
            try
            {
                Sys_Account account = LoginAccount as Sys_Account;
                IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
                workFlowCtrl.CheckEmployeeList(checkParam.EmployeeIdList, checkParam.PassFlag, checkParam.CheckedMark);
            }
            catch (Exception ex)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = ex.Message;
            }
            return Json(resultMessage, JsonRequestBehavior.DenyGet);
        }
        public class CheckMultiParam
        {
            public List<int> EmployeeIdList { get; set; }
            public bool PassFlag { get; set; }
            public string CheckedMark { get; set; }
        }
        #endregion

        #region 获取报名信息
        public JsonResult GetEmployeeById(int employeeId)
        {
            Sys_Account account = LoginAccount as Sys_Account;
            IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
            Biz_Employee employee = workFlowCtrl.GetEmployeeInfoById(employeeId);
            IEnterpriseCtrl enterpriseCtrl = new EnterpriseCtrl(account);
            EmployeeInfoResult employeeResult = new EmployeeInfoResult()
            {
                EmployeeId = employee.Id,
                EmployeeName = employee.EmployeeName,
                Sex = employee.Sex,
                IDNumber = employee.IDNumber,
                Birthday = employee.Birthday.ToString("yyyy-MM-dd"),
                Job = employee.Job,
                Title4Technical = employee.Title4Technical,
                Industry = employee.Industry,
                ExamType = employee.ExamType,
                EnterpriseName = enterpriseCtrl.GetEnterpriseById(employee.EnterpriseId).EnterpriseName
            };
            return Json(employeeResult, JsonRequestBehavior.DenyGet);
        }
        public class EmployeeInfoResult
        {
            public int EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public string Sex { get; set; }
            public string IDNumber { get; set; }

            public string Birthday { get; set; }
            public string Job { get; set; }
            public string Title4Technical { get; set; }
            public string Industry { get; set; }
            public string ExamType { get; set; }
            public string EnterpriseName { get; set; }
        }
        #endregion
    }
}
