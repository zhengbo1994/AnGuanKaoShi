using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Model;
using BLL;
using Library.baseFn;
using System.Transactions;

namespace SafetyProfessionalAssessmentSystem.Controllers
{
    [AuthorizeFilter]
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
            ITrainingInstitutionCtrl trainingInstitutionCtrl = new TrainingInstitutionCtrl(account);
            List<Biz_Employee> employeeList = workFlowCtrl.GetEmployeeList(param.EmployeeName, param.IDNumber,
                param.TrainingType, param.ExamType, param.Industry, param.WorkFlowStatus, param.page, param.rows, ref totalCount);
            List<Biz_TrainingInstitution> trainingInstitutionList = trainingInstitutionCtrl.GetTrainingInstitutionByIdList(employeeList.Select(p => p.TrainingInstitutionId).ToList());

            List<WorkFlowStatus> WorkFlowStatusList = workFlowCtrl.GetCurrentWorkFlowStatusByEmployeeIdList(employeeList.Select(p => p.Id).ToList());

            List<GetEmployeeListForJqgridJsonResult> employeeListJsonResult = employeeList.Join(WorkFlowStatusList, p => p.Id, q => q.employeeId, (p, q) => new { p, q })
                .GroupJoin(trainingInstitutionList, a => a.p.TrainingInstitutionId, b => b.Id, (a, b) => new
                {
                    a,
                    TrainingInstitutionName = b.Count() == 0 ? "" : b.Max(o => o.InstitutionName)
                })
             .Select(o => new GetEmployeeListForJqgridJsonResult()
            {
                Id = o.a.p.Id,
                EmployeeName = o.a.p.EmployeeName,
                Sex = o.a.p.Sex,
                Age = o.a.p.Birthday.IsNull() ? "" : (DateTime.Now.Year - o.a.p.Birthday.ConvertToDateTime().Year).ToString(),
                IDNumber = o.a.p.IDNumber,
                Job = o.a.p.Job,
                Title4Technical = o.a.p.Title4Technical,
                ExamType = o.a.p.ExamType,
                Industry = o.a.p.Industry,
                TrainingType = o.a.p.IsTraining ? o.a.p.TrainingType : "内训",
                City = o.a.p.City,
                TrainingInstitutionName = o.TrainingInstitutionName,
                ConstructorCertificateNo = o.a.p.ConstructorCertificateNo,
                CurrentStatus = o.a.q.WorkFlowStatusTag,
                OperationDate = o.a.q.CreateDate.ToString("yyyy-MM-dd HH:mm")

            }).ToList();

            JqGridData dataResult = ConvertIListToJqGridData(param.page, param.rows, totalCount, employeeListJsonResult);
            return Json(dataResult, JsonRequestBehavior.AllowGet);
        }

        public class GetEmployeeListForJqgridParam
        {
            public string EmployeeName { get; set; }
            public string IDNumber { get; set; }
            public string ExamType { get; set; }
            public string Industry { get; set; }
            public string TrainingType { get; set; }
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
            public string TrainingType { get; set; }
            /// <summary>
            /// 报考城市
            /// </summary>
            public string City { get; set; }
            public string TrainingInstitutionName { get; set; }
            public string ConstructorCertificateNo { get; set; }
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
                    IDNumber = employeeParam.IDNumber.Trim().Replace('x', 'X').Replace('ｘ', 'X').Replace('Ｘ', 'X'),//将小写x转大写 全角转半角
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
                    IsTraining = employeeParam.IsTraining,
                    TrainingType = employeeParam.TrainingType,
                    PrintCertificate = employeeParam.PrintCertificate,
                    TrainingInstitutionId = employeeParam.TrainingInstitutionId,
                    Remark = employeeParam.Remark,
                    ConstructorCertificateNo = employeeParam.ConstructorCertificateNo
                };

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
            public bool IsTraining { get; set; }
            //培训类型(线上/线下)
            public string TrainingType { get; set; }

            public int TrainingInstitutionId { get; set; }
            //备注
            public string Remark { get; set; }
            //身份证附件
            public HttpPostedFileBase IDCard { get; set; }
            //登记照
            public HttpPostedFileBase RegistrationPhoto { get; set; }
            public bool PrintCertificate { get; set; }
            public string ConstructorCertificateNo { get; set; }

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


        #endregion

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
                IsTraining = emplyeeInfo.IsTraining,
                TrainingType = emplyeeInfo.TrainingType,
                PrintCertificate = emplyeeInfo.PrintCertificate,
                TrainingInstitutionId = emplyeeInfo.TrainingInstitutionId,
                ConstructorCertificateNo = emplyeeInfo.ConstructorCertificateNo,
                Remark = emplyeeInfo.Remark
            };
            return Json(dataResult, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 取证人员提交
        public JsonResult SubmitEmployee(string strEmployeeIdList)
        {
            List<int> employeeIdList = AppFn.JSONStringToObj<List<int>>(strEmployeeIdList);

            ResultMessage resultMessage = new ResultMessage();
            TransactionScope scope = base.CreateTransaction();
            try
            {
                Sys_Account account = LoginAccount as Sys_Account;
                IWorkFlowCtrl workflowCtrl = new WorkFlowCtrl(account);
                workflowCtrl.SummitEmployee(employeeIdList);
                resultMessage.IsSuccess = true;
                base.TransactionCommit(scope);
            }
            catch (Exception e)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = e.Message;
                base.TransactionRollback(scope);
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
                Biz_Employee oldEmployee = new Biz_Employee()
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
                    IsTraining = employeeParam.IsTraining,
                    TrainingType = employeeParam.TrainingType,
                    PrintCertificate = employeeParam.PrintCertificate,
                    TrainingInstitutionId = employeeParam.TrainingInstitutionId,
                    ConstructorCertificateNo = employeeParam.ConstructorCertificateNo,
                    Remark = employeeParam.Remark
                };
                workflowCtrl.UpdateEmployeeInfo(oldEmployee);
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

        #region 获取培训机构列表
        public JsonResult GetTrainingInstitutionList(string city)
        {
            Sys_Account account = base.LoginAccount as Sys_Account;
            ITrainingInstitutionCtrl trainingInstitutionCtrl = new TrainingInstitutionCtrl(account);
            List<string> cityList = new List<string>();
            if (!city.IsNull())
            {
                cityList.Add(city);
            }
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

        #region  获取企业所在城市
        public JsonResult GetEnterpriseCity()
        {
            Sys_Account account = base.LoginAccount as Sys_Account;
            IEnterpriseCtrl enterpriseCtrl = new EnterpriseCtrl(account);
            string city = enterpriseCtrl.GetEnterpriseById(account.UserId).City;
            return Json(city, JsonRequestBehavior.DenyGet);
        }
        #endregion

        //#region 根据取证人员ID获取取证人员信息

        //public JsonResult GetEmployeeInfoById(int employeeId)
        //{
        //    Sys_Account account = LoginAccount as Sys_Account;
        //    IWorkFlowCtrl employeeCtrl = new WorkFlowCtrl(account);
        //    Biz_Employee employee = employeeCtrl.GetEmployeeInfoById(employeeId);
        //    GetEmployeeInfoByIdResult employeeInfo = new GetEmployeeInfoByIdResult()
        //    {
        //        Id = employee.Id,
        //        EmployeeName = employee.EmployeeName,
        //        Sex = employee.Sex,
        //        Birthday = employee.Birthday.ToString("yyyy-MM-dd"),
        //        IDNumber = employee.IDNumber,
        //        Job = employee.Job,
        //        Title4Technical = employee.Title4Technical,
        //        City = employee.City,
        //        ExamType = employee.ExamType,
        //        Industry = employee.Industry,
        //        IsTraining = employee.IsTraining,
        //        TrainingType = employee.TrainingType,
        //        TrainingInstitutionId = employee.TrainingInstitutionId,
        //        Remark = employee.Remark
        //    };
        //    return Json(employeeInfo, JsonRequestBehavior.AllowGet);
        //}
        //public class GetEmployeeInfoByIdResult
        //{
        //    // ID主键
        //    public int Id { get; set; }
        //    //人员名称
        //    public string EmployeeName { get; set; }
        //    //性别
        //    public string Sex { get; set; }
        //    // 出生日期
        //    public string Birthday { get; set; }
        //    //身份证号
        //    public string IDNumber { get; set; }
        //    //职位
        //    public string Job { get; set; }
        //    // 技术职称
        //    public string Title4Technical { get; set; }
        //    //报考城市
        //    public string City { get; set; }
        //    //报考科目
        //    public string ExamType { get; set; }
        //    //报考行业
        //    public string Industry { get; set; }
        //    public bool IsTraining { get; set; }
        //    public string TrainingType { get; set; }
        //    public int TrainingInstitutionId { get; set; }
        //    //备注
        //    public string Remark { get; set; }
        //    //身份证附件
        //    public HttpPostedFileBase IDCard { get; set; }
        //    //登记照
        //    public HttpPostedFileBase RegistrationPhoto { get; set; }
        //}

        //#endregion

        #region 获取企业名称
        public string GetEnterpriseName()
        {
            return LoginUserName;
        }
        #endregion


        #region 生成准考证
        public ResultMessage CreateAdmissionticket(int employeeId)
        {
            ResultMessage result = new ResultMessage();
            try
            {
                Sys_Account account = base.LoginAccount as Sys_Account;
                //int employeeId = account.UserId;
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
                result.IsSuccess = false;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        public ActionResult ShowAdmissionticket(int employeeId)
        {
            ResultMessage result = CreateAdmissionticket(employeeId);
            if (result.IsSuccess)
            {
                string fileName = result.ErrorMessage.Substring(result.ErrorMessage.LastIndexOf('\\') + 1);
                return File(AppFn.GetFileBytes(result.ErrorMessage), "application/pdf");
            }
            else
            {
                // throw new Exception(result.ErrorMessage);
                ViewBag.ErrorMessage = result.ErrorMessage;
                return View("~/Views/ErrorView.cshtml");
            }
        }

        public FileResult DownloadAdmissionticket(int employeeId)
        {
            ResultMessage result = CreateAdmissionticket(employeeId);
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
