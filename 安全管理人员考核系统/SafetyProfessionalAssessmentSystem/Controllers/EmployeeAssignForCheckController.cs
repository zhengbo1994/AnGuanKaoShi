using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;

using Model;
using BLL;
using Library.baseFn;


namespace SafetyProfessionalAssessmentSystem.Controllers
{
    public class EmployeeAssignForCheckController : BaseController
    {
        //
        // GET: /EmployeeAssignForCheck/

        public ActionResult Index()
        {
            return View();
        }
        #region 根据查询条件获取jqgrid人员信息
        public JsonResult GetEmployeeAssignListForJqgrid(GetEmployeeAssignListForJqgridParam param)
        {
            List<EmployeeAssignListResult> employeeAssignListResult = new List<EmployeeAssignListResult>();
            int totalCount = 0;

            Sys_Account account = LoginAccount as Sys_Account;
            List<String> cityList = LoginDataPermissionDetailList.Select(p => p.DetailName).ToList();

            IWorkFlowCtrl workCtrl = new WorkFlowCtrl(account);

            IEnterpriseCtrl entrpriseCtrl = new EnterpriseCtrl(account);
            ITrainingInstitutionCtrl trainingInstitutionCtrl = new TrainingInstitutionCtrl(account);



            //分配人员信息
            List<Biz_Employee> employeeAssignList = workCtrl.GetEmployeeAssignList(param.InstitutionName, param.EmployeeName,
                param.EnterpriseName, param.ExamType, param.AssignStatus, param.AssignDateBegin, param.AssignDateEnd, param.page, param.rows, param.sidx, param.sord, ref totalCount, cityList);
            //人员已分配记录
            List<Biz_EmployeeAssignForCheckRecord> employeeAssignRecoderList = workCtrl.GetEmployeeAssignForCheckRecordByEmployeeIdList(employeeAssignList.Select(p => p.Id).ToList());

            //相关培训机构信息
            List<Biz_TrainingInstitution> trainingInstitutionList = trainingInstitutionCtrl.GetTrainingInstitutionByIdList(employeeAssignRecoderList.Select(p => p.TrainingInstitutionId).ToList());
            //相关企业信息
            List<Biz_Enterprise> enterpriseList = entrpriseCtrl.GetEnterpriseInfoByIdList(employeeAssignList.Select(p => p.EnterpriseId).ToList());


            try
            {
                employeeAssignListResult = employeeAssignList.GroupJoin(employeeAssignRecoderList, a => a.Id, b => b.EmployeeId, (a, b) => new EmployeeAssignListResult()
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
                    InstitutionId = (b.Count() == 0 ? -1 : b.Max(o => o.TrainingInstitutionId)),
                    AssignStatus = b.Count() == 0 ? "未分配" : "已分配",
                    AssignDate = b.Count() == 0 ? "" : b.Max().CreateDate.ToString("yyyy-MM-dd")
                }).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }

            //取考核点信息
            employeeAssignListResult = employeeAssignListResult.GroupJoin(trainingInstitutionList, a => a.InstitutionId, b => b.Id, (a, b) => new EmployeeAssignListResult()
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
                AssignStatus = a.AssignStatus,
                AssignDate = a.AssignDate,
                InstitutionName = b.Max(o => o.InstitutionName)
            }).ToList();
            //取企业信息
            employeeAssignListResult = employeeAssignListResult.Join(enterpriseList, a => a.EnterpriseId, b => b.Id, (a, b) => new EmployeeAssignListResult()
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
                InstitutionName = a.InstitutionName,
                EnterpriseName = b.EnterpriseName,
                AssignStatus = a.AssignStatus,
                AssignDate = a.AssignDate
            }).ToList();

            JqGridData dataResult = ConvertIListToJqGridData(param.page, param.rows, totalCount, employeeAssignListResult);

            return Json(dataResult, JsonRequestBehavior.AllowGet);
        }

        public class GetEmployeeAssignListForJqgridParam
        {
            public int EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public string InstitutionName { get; set; }
            public string EnterpriseName { get; set; }
            public string ExamType { get; set; }
            public string AssignStatus { get; set; }
            public string AssignDateBegin { get; set; }
            public string AssignDateEnd { get; set; }
            public int page { get; set; }
            public int rows { get; set; }
            public string sidx { get; set; }//排序字段名称
            public string sord { get; set; }//排序方式asc desc

        }

        public class EmployeeAssignListResult
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
            public string InstitutionName { get; set; }

            // 职位
            public string Job { get; set; }
            // 技术职称
            public string Title4Technical { get; set; }

            // 报考科目
            public string ExamType { get; set; }
            //报考行业
            public string Industry { get; set; }
            // 分配状态
            public string AssignStatus { get; set; }
            // 分配状态
            public string AssignDate { get; set; }

        }
        #endregion

        //自动分配
        public JsonResult EmployeeAutoAssignForCheck(int PersonCount, int TrainingInstitutionId)
        {
            ResultMessage resultMessage = new ResultMessage();
            try
            {
                Sys_Account account = LoginAccount as Sys_Account;
                IWorkFlowCtrl workflowCtrl = new WorkFlowCtrl(account);
                List<string> cityList = base.LoginDataPermissionDetailList.Select(p => p.DetailName).ToList();
                workflowCtrl.AssignEmployeeListForCheck(PersonCount, TrainingInstitutionId, cityList);
                resultMessage.IsSuccess = true;
            }
            catch (Exception e)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = e.Message;
            }
            return Json(resultMessage, JsonRequestBehavior.AllowGet);

        }


        //手动分配人员
        public JsonResult EmployeeManualAssignForCheck(List<int> employeeIdList, int TrainingInstitutionId)
        {
            ResultMessage resultMessage = new ResultMessage();
            try
            {
                Sys_Account account = LoginAccount as Sys_Account;
                IWorkFlowCtrl workflowCtrl = new WorkFlowCtrl(account);
                workflowCtrl.AssignEmployeeListForCheck(employeeIdList, TrainingInstitutionId);
                resultMessage.IsSuccess = true;
            }
            catch (Exception e)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = e.Message;
            }
            return Json(resultMessage, JsonRequestBehavior.AllowGet);

        }

        #region 获取考核点List
        public JsonResult GetTrainingInstitutionList()
        {
            Sys_Account account = LoginAccount as Sys_Account;
            List<String> cityList = LoginDataPermissionDetailList.Select(p => p.DetailName).ToList();
            ITrainingInstitutionCtrl trainingInstitutionCtrl = new TrainingInstitutionCtrl(account);
            List<TrainingInstitutionListResult> trainingInstitutionListResultList = trainingInstitutionCtrl.GetTrainingInstitutionListByCityList(cityList).Select(P => new TrainingInstitutionListResult()
            {
                TrainingInstitutionId = P.Id,
                TrainingInstitutionName = P.InstitutionName
            }).ToList();
            return Json(trainingInstitutionListResultList, JsonRequestBehavior.AllowGet);
        }
        public class TrainingInstitutionListResult
        {
            public int TrainingInstitutionId { get; set; }
            public string TrainingInstitutionName { get; set; }
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
    }
}
