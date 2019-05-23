using System;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;

using Model;
using BLL;
using System.Web;
using Library.baseFn;


namespace SafetyProfessionalAssessmentSystem.Controllers
{
    public class EmployeeRegistrationController : BaseController
    {
        //
        // GET: /EmployeeRegistration/

        public ActionResult Index()
        {
            return View();
        }
        #region 根据查询条件获取jqgrid人员信息
        public JsonResult GetEmployeeListForJqgrid(GetEmployeeListForJqgridParam param)
        {
            int totalCount = 0;
            Sys_Account account = LoginAccount as Sys_Account;
            IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
            List<Biz_Employee> employeeList = workFlowCtrl.GetEmployeeList(param.EmployeeName, param.IDNumber,
                param.EnterpriseID, param.ExamType, param.Industry, param.WorkFlowStatus, param.page, param.rows, ref totalCount);

            List<WorkFlowStatus> WorkFlowStatusList = workFlowCtrl.GetCurrentWorkFlowStatusByEmployeeIdList(employeeList.Select(p => p.Id).ToList());

            List<GetEmployeeListForJqgridJsonResult> employeeListJsonResult = employeeList.Join(WorkFlowStatusList, p => p.Id, q => q.employeeId, (p, q) => new { p, q }).Select(o => new GetEmployeeListForJqgridJsonResult()
            {
                Id = o.p.Id,
                EmployeeName = o.p.EmployeeName,
                Sex = o.p.Sex,
                Age = o.p.Birthday.IsNull() ? "" : (DateTime.Now.Year - o.p.Birthday.ConvertToDateTime().Year).ToString(),
                IDNumber = o.p.IDNumber,
                Job = o.p.Job,
                Title4Technical = o.p.Title4Technical,
                ExamType = o.p.ExamType,
                Industry = o.p.Industry,
                City = o.p.City,
                CurrentStatus = o.q.WorkFlowStatusTag,
                OperationDate = o.q.CreateDate.ToString("yyyy-MM-dd HH:mm")
            }).OrderByDescending(p=>p.OperationDate).ToList();

            JqGridData dataResult = ConvertIListToJqGridData(param.page, param.rows, totalCount, employeeListJsonResult);
            return Json(dataResult, JsonRequestBehavior.AllowGet);
        }

        public class GetEmployeeListForJqgridParam
        {
            public string EmployeeName { get; set; }
            public string IDNumber { get; set; }
            public int? EnterpriseID { get; set; }
            public string ExamType { get; set; }
            public string Industry { get; set; }
            public string WorkFlowStatus { get; set; }
            public int page { get; set; }
            public int rows { get; set; }
            public string sidx { get; set; }//排序字段名称
            public string sord { get; set; }//排序方式asc desc

        }

        public class GetEmployeeListForJqgridJsonResult
        {
            public int Id { get; set; }
            /// <summary>
            /// 人员名称
            /// </summary>
            public string EmployeeName { get; set; }
            /// <summary>
            /// 性别
            /// </summary>
            public string Sex { get; set; }
            //年龄
            public string Age { get; set; }

            /// <summary>
            /// 身份证号
            /// </summary>
            public string IDNumber { get; set; }
            /// <summary>
            /// 所属单位
            /// </summary>
            public string EnterpriseName { get; set; }
            /// <summary>
            /// 职位
            /// </summary>
            public string Job { get; set; }
            /// <summary>
            /// 技术职称
            /// </summary>
            public string Title4Technical { get; set; }
            /// <summary>
            /// 报考科目
            /// </summary>
            public string ExamType { get; set; }
            //报考行业
            public string Industry { get; set; }
            /// <summary>
            /// 报考城市
            /// </summary>
            public string City { get; set; }

            /// <summary>
            /// 当前流程
            /// </summary>
            public string CurrentStatus { get; set; }
            public string OperationDate { get; set; }// 操作日期

        }
        #endregion

        #region 取证人员报名
        public JsonResult RegistrationEmployee(EmployeeRegistrationParam employeeParam)
        {
            ResultMessage resultMessage = new ResultMessage();
            try
            {
                Sys_Account account = LoginAccount as Sys_Account;
                //保存人员信息
                Biz_Employee employee = new Biz_Employee()
                {
                    Id = employeeParam.Id,
                    EmployeeName = employeeParam.EmployeeName,
                    Sex = employeeParam.Sex,
                    Birthday = Convert.ToDateTime(employeeParam.Birthday),
                    IDNumber = employeeParam.IDNumber,
                    Job = employeeParam.Job,
                    Title4Technical = employeeParam.Title4Technical,
                    City = employeeParam.City,
                    ExamType = employeeParam.ExamType,
                    Industry = employeeParam.Industry,
                    SubmitStatus = false,
                    OperationStatus = false,
                    CreateById = account.Id,
                    CreateDate = DateTime.Now,
                    EnterpriseId = account.UserId,
                    Remark = employeeParam.Remark
                };
                //保存人员附件

                //写入流程表
                IWorkFlowCtrl workflowCtrl = new WorkFlowCtrl(account);
                workflowCtrl.RegisterEmployee(employee);
                resultMessage.IsSuccess = true;
            }
            catch (Exception e)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = e.Message;
            }
            return Json(resultMessage, JsonRequestBehavior.AllowGet);
        }

        public class EmployeeRegistrationParam
        {
            // ID主键
            public int Id { get; set; }
            //人员名称
            public string EmployeeName { get; set; }
            //性别
            public string Sex { get; set; }
            // 出生日期
            public string Birthday { get; set; }
            //身份证号
            public string IDNumber { get; set; }
            //职位
            public string Job { get; set; }
            // 技术职称
            public string Title4Technical { get; set; }
            //报考城市
            public string City { get; set; }
            //报考科目
            public string ExamType { get; set; }
            public string Industry { get; set; }
            //备注
            public string Remark { get; set; }
            //身份证附件
            public HttpPostedFileBase IDCard { get; set; }
            //登记照
            public HttpPostedFileBase RegistrationPhoto { get; set; }

        }
        #endregion

        #region 删除报名信息
        public JsonResult DeleteEmployeeById(int employeeId)
        {
            ResultMessage resultMessage = new ResultMessage();
            try
            {
                Sys_Account account = LoginAccount as Sys_Account;
                IWorkFlowCtrl workflowCtrl = new WorkFlowCtrl(account);
                workflowCtrl.DeleteEmployeeById(employeeId);
                resultMessage.IsSuccess = true;
            }
            catch (Exception e)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = e.Message;
            }
            return Json(resultMessage, JsonRequestBehavior.AllowGet);
        }
        #region 获取报名人员信息
        public JsonResult GetEmployeeById(int employeeId)
        {
            Sys_Account account = LoginAccount as Sys_Account;
            IWorkFlowCtrl workflowCtrl = new WorkFlowCtrl(account);
            Biz_Employee emplyeeInfo = workflowCtrl.GetEmployeeInfoById(employeeId);
            EmployeeRegistrationParam dataResult = new EmployeeRegistrationParam()
            {
                Id = emplyeeInfo.Id,
                EmployeeName = emplyeeInfo.EmployeeName,
                Sex = emplyeeInfo.Sex,
                Birthday = emplyeeInfo.Birthday.ToString("yyyy-MM-dd"),
                IDNumber = emplyeeInfo.IDNumber,
                Job = emplyeeInfo.Job,
                Title4Technical = emplyeeInfo.Title4Technical,
                City = emplyeeInfo.City,
                ExamType = emplyeeInfo.ExamType,
                Industry = emplyeeInfo.Industry,
                Remark = emplyeeInfo.Remark
            };
            return Json(dataResult, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #endregion

        #region 取证人员提交
        public JsonResult SubmitEmployee(string strEmployeeIdList)
        {
            List<int> employeeIdList = AppFn.JSONStringToObj<List<int>>(strEmployeeIdList);

            ResultMessage resultMessage = new ResultMessage();
            try
            {
                Sys_Account account = LoginAccount as Sys_Account;
                IWorkFlowCtrl workflowCtrl = new WorkFlowCtrl(account);
                workflowCtrl.SummitEmployee(employeeIdList);
                resultMessage.IsSuccess = true;
            }
            catch (Exception e)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = e.Message;
            }
            return Json(resultMessage, JsonRequestBehavior.AllowGet);

        }



        #endregion

        #region 更新报名人员信息
        public JsonResult UpdateRegistrationEmployee(EmployeeRegistrationParam employeeParam)
        {
            ResultMessage resultMessage = new ResultMessage();
            try
            {
                Sys_Account account = LoginAccount as Sys_Account;
                IWorkFlowCtrl workflowCtrl = new WorkFlowCtrl(account);

                Biz_Employee employee = new Biz_Employee()
                {
                    Id = employeeParam.Id,
                    EmployeeName = employeeParam.EmployeeName,
                    Sex = employeeParam.Sex,
                    Birthday = Convert.ToDateTime(employeeParam.Birthday),
                    IDNumber = employeeParam.IDNumber,
                    Job = employeeParam.Job,
                    Title4Technical = employeeParam.Title4Technical,
                    City = employeeParam.City,
                    ExamType = employeeParam.ExamType,
                    Industry = employeeParam.Industry,

                    Remark = employeeParam.Remark
                };

                workflowCtrl.UpdateEmployeeInfo(employee);
                resultMessage.IsSuccess = true;
            }
            catch (Exception e)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = e.Message;
            }
            return Json(resultMessage, JsonRequestBehavior.AllowGet);
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

        #region 根据取证人员ID获取取证人员信息

        public JsonResult GetEmployeeInfoById(int employeeId)
        {
            Sys_Account account = LoginAccount as Sys_Account;
            IWorkFlowCtrl employeeCtrl = new WorkFlowCtrl(account);
            Biz_Employee employee = employeeCtrl.GetEmployeeInfoById(employeeId);
            GetEmployeeInfoByIdResult employeeInfo = new GetEmployeeInfoByIdResult()
            {
                Id = employee.Id,
                EmployeeName = employee.EmployeeName,
                Sex = employee.Sex,
                Birthday = employee.Birthday.ToString("yyyy-MM-dd"),
                IDNumber = employee.IDNumber,
                Job = employee.Job,
                Title4Technical = employee.Title4Technical,
                City = employee.City,
                ExamType = employee.ExamType,
                Industry = employee.Industry,
                Remark = employee.Remark
            };
            return Json(employeeInfo, JsonRequestBehavior.AllowGet);
        }
        public class GetEmployeeInfoByIdResult
        {
            // ID主键
            public int Id { get; set; }
            //人员名称
            public string EmployeeName { get; set; }
            //性别
            public string Sex { get; set; }
            // 出生日期
            public string Birthday { get; set; }
            //身份证号
            public string IDNumber { get; set; }
            //职位
            public string Job { get; set; }
            // 技术职称
            public string Title4Technical { get; set; }
            //报考城市
            public string City { get; set; }
            //报考科目
            public string ExamType { get; set; }
            //报考行业
            public string Industry { get; set; }
            //备注
            public string Remark { get; set; }
            //身份证附件
            public HttpPostedFileBase IDCard { get; set; }
            //登记照
            public HttpPostedFileBase RegistrationPhoto { get; set; }
        }

        #endregion
        //获取所在企业的名称
        public string GetEnterpriseName()
        {
            return LoginUserName;
        }
    }
}
