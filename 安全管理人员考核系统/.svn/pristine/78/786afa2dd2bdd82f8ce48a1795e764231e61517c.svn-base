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
    [AuthorizeFilter]
    public class EmployeeInformationController : BaseController
    {
        //
        // GET: /人员信息查询/

        public ActionResult Index()
        {
            return View();
        }

        #region 根据查询条件获取jqgrid人员信息
        public JsonResult GetEmployeeListForJqgrid(GetEmployeeListForJqgridParam param)
        {
            int totalCount = 0;
            Sys_Account account = LoginAccount as Sys_Account;
            IStatisticalReportCtrl statisticalReportCtrl = new StatisticalReportCtrl(account);
            IEnterpriseCtrl enterpriseCtrl = new EnterpriseCtrl(account);
            IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
            IEmployeeCtrl employeeCtrl = new EmployeeCtrl(account);
            List<string> cityList = base.LoginDataPermissionDetailList.Select(p => p.DetailName).ToList();
            List<Biz_Employee> employeeList = statisticalReportCtrl.GetEmployeeList(param.EmployeeName, param.IDNumber,
                param.EnterpriseName, param.ExamType, param.Industry, param.WorkFlowStatus, param.page, param.rows, ref totalCount, cityList);


            List<Biz_Enterprise> enterprise = enterpriseCtrl.GetEnterpriseInfoByIdList(employeeList.Select(p => p.EnterpriseId).ToList());
            List<WorkFlowStatus> workFlowStatusList = workFlowCtrl.GetCurrentWorkFlowStatusByEmployeeIdList(employeeList.Select(p => p.Id).ToList());
            List<Biz_EmployeeExamResultRecord> employeeExamResultRecordList = employeeCtrl.GetEmployeeExamResultRecordListByEmployeeId(employeeList.Select(p => p.Id).ToList());


            List<GetEmployeeListForJqgridJsonResult> employeeListJsonResult = employeeList
                .Join(workFlowStatusList, p => p.Id, q => q.employeeId, (p, q) => new { p, q })
                .Join(enterprise, o => o.p.EnterpriseId, b => b.Id, (o, b) => new { o.p, o.q, b })
                .GroupJoin(employeeExamResultRecordList, o => o.p.Id, c => c.EmployeeId, (o, c) => new { o.p, o.q, o.b, c })
             .Select(o => new GetEmployeeListForJqgridJsonResult()
            {
                EmployeeId = o.p.Id,
                EmployeeName = o.p.EmployeeName,
                Sex = o.p.Sex,
                Age = o.p.Birthday.IsNull() ? "" : (DateTime.Now.Year - o.p.Birthday.ConvertToDateTime().Year).ToString(),
                IDNumber = o.p.IDNumber,
                Job = o.p.Job,
                Title4Technical = o.p.Title4Technical,
                ExamType = o.p.ExamType,
                Industry = o.p.Industry,
                SafetyKnowledgeExamScore = o.c.Max(d => d.SafetyKnowledgeExamScore) == null ? "" : o.c.Max(d => d.SafetyKnowledgeExamScore).ToString(),
                ManagementAbilityExamScore = o.c.Max(d => d.ManagementAbilityExamScore) == null ? "" : o.c.Max(d => d.ManagementAbilityExamScore).ToString(),
                SafetyKnowledgeExamResult = o.c.Max(d => d.SafetyKnowledgeExamResult),
                ManagementAbilityExamResult = o.c.Max(d => d.ManagementAbilityExamResult),
                ActualOperationExamResult = o.c.Max(d => d.ActualOperationExamResult),
                FinalExamResult = o.c.Max(d => d.FinalExamResult),
                LocaltionCity = o.b.LocaltionCity,
                CurrentStatus = o.q.WorkFlowStatusTag,
                OperationDate = o.q.CreateDate.ToString("yyyy-MM-dd HH:mm"),
                EnterpriseName = o.b.EnterpriseName
            }).ToList();
            JqGridData dataResult = ConvertIListToJqGridData(param.page, param.rows, totalCount, employeeListJsonResult);
            return Json(dataResult, JsonRequestBehavior.AllowGet);
        }

        public class GetEmployeeListForJqgridParam
        {
            public string EmployeeName { get; set; }
            public string IDNumber { get; set; }
            public string EnterpriseName { get; set; }
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
            public int EmployeeId { get; set; }
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
            //安全知识分数
            public string SafetyKnowledgeExamScore { get; set; }
            //管理能力分数
            public string ManagementAbilityExamScore { get; set; }
            //安全知识结果
            public string SafetyKnowledgeExamResult { get; set; }
            //管理能力结果
            public string ManagementAbilityExamResult { get; set; }
            //实操结果
            public string ActualOperationExamResult { get; set; }
            //考核结果
            public string FinalExamResult { get; set; }
            /// <summary>
            /// 报考城市
            /// </summary>
            public string LocaltionCity { get; set; }

            /// <summary>
            /// 当前流程
            /// </summary>
            public string CurrentStatus { get; set; }
            public string OperationDate { get; set; }// 操作日期

        }
        #endregion

        #region 人员流程记录
        public JsonResult GetWorkFlow(int employeeId)
        {
            List<WorkFlowRecord> workFlowRecordList = new List<WorkFlowRecord>();

            try
            {
                workFlowRecordList = GetWorkFlowRecordList(employeeId);
            }
            catch (Exception ex)
            {

            }
            int totalCount = workFlowRecordList.Count();
            int page = 1;
            int rows = 10;
            JqGridData dataResult = ConvertIListToJqGridData(page, rows, totalCount, workFlowRecordList);
            return Json(dataResult, JsonRequestBehavior.AllowGet);
        }
        public class WorkFlowRecord
        {
            public string Operation { get; set; }
            public string Remark { get; set; }
            public string OperaDateTime { get; set; }
            public string OperaUserName { get; set; }
        }

        List<WorkFlowRecord> GetWorkFlowRecordList(int employeeId)
        {
            Sys_Account account = base.LoginAccount as Sys_Account;
            List<WorkFlowRecord> workFlowRecordList = new List<WorkFlowRecord>();
            IStatisticalReportCtrl statisticalReportCtrl = new StatisticalReportCtrl(account);
            Biz_Employee employee = statisticalReportCtrl.GetEmployeeById(employeeId);
            if (!employee.IsNull())
            {
                WorkFlowRecord register = new WorkFlowRecord()
                {
                    OperaDateTime = employee.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    OperaUserName = statisticalReportCtrl.GetUserName(employee.CreateById),
                    Operation = "报名",
                    Remark = ""
                };
                workFlowRecordList.Add(register);
                if (employee.SubmitStatus == true)
                {
                    WorkFlowRecord registerSubmit = new WorkFlowRecord()
                    {
                        OperaDateTime = Convert.ToDateTime(employee.SummitDate).ToString("yyyy-MM-dd HH:mm:ss"),
                        OperaUserName = statisticalReportCtrl.GetUserName(Convert.ToInt32(employee.SummitById)),
                        Operation = "提交报名信息",
                        Remark = ""
                    };
                    workFlowRecordList.Add(registerSubmit);
                }
            }
            else
            {
                return workFlowRecordList;
            }
            Biz_TrainingRecord trainingRecord = statisticalReportCtrl.GetTrainingRecord(employeeId);

            if (!trainingRecord.IsNull())
            {
                WorkFlowRecord training = new WorkFlowRecord()
                {
                    OperaDateTime = trainingRecord.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    OperaUserName = statisticalReportCtrl.GetUserName(trainingRecord.CreateById),
                    Operation = trainingRecord.PassStatus ? "培训通过" : "培训不通过",
                    Remark = trainingRecord.Remark
                };
                workFlowRecordList.Add(training);

            }

          

            Biz_EmployeeCheckedRecord employeeCheckedRecord = statisticalReportCtrl.GetEmployeeCheckedRecordByEmployeeId(employeeId);
            if (!employeeCheckedRecord.IsNull())
            {
                WorkFlowRecord workFlowRecord = new WorkFlowRecord()
                {
                    OperaDateTime = employeeCheckedRecord.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    OperaUserName = statisticalReportCtrl.GetUserName(employeeCheckedRecord.CreateById),
                    Operation = employeeCheckedRecord.PassStatus ? "报名确认通过" : "报名确认未通过",
                    Remark = employeeCheckedRecord.CheckedMark
                };
                workFlowRecordList.Add(workFlowRecord);
            }
            else
            {
                return workFlowRecordList;
            }

            Biz_EmployeeForExamPlanRecord employeeForExamPlanRecord = statisticalReportCtrl.GetEmployeeForExamPlanRecordByEmployeeId(employeeId);
            if (!employeeForExamPlanRecord.IsNull())
            {

                WorkFlowRecord workFlowRecord = new WorkFlowRecord()
                {
                    OperaDateTime = employeeForExamPlanRecord.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    OperaUserName = statisticalReportCtrl.GetUserName(employeeForExamPlanRecord.CreateById),
                    Operation = "制定考试计划",
                    Remark = ""
                };
                workFlowRecordList.Add(workFlowRecord);
                Biz_ExamPlanRecord examPlan = statisticalReportCtrl.GetExamPlanRecordByExamPlanId(employeeForExamPlanRecord.ExamPlanRecordId);
                if (examPlan.SubmitStatus == true)
                {
                    WorkFlowRecord examPlanWorkFlowRecord = new WorkFlowRecord()
                    {
                        OperaDateTime = Convert.ToDateTime(examPlan.SubmitDate).ToString("yyyy-MM-dd HH:mm:ss"),
                        OperaUserName = statisticalReportCtrl.GetUserName(Convert.ToInt32(examPlan.SubmitById)),
                        Operation = "提交考试计划",
                        Remark = "准考证号【" + employeeForExamPlanRecord.ExamRegistrationNumber + "】  考试计划流水号【" + examPlan.ExamPlanNumber + "】考试开始时间【" + examPlan.ExamDateTimeBegin.ToString("yyyy-MM-dd HH:mm:ss") + "--" + examPlan.ExamDateTimeEnd.ToString("yyyy-MM-dd HH:mm:ss") + "】"
                    };
                    workFlowRecordList.Add(examPlanWorkFlowRecord);
                }
            }
            else
            {
                return workFlowRecordList;
            }

            Biz_EmployeeExamResultRecord employeeExamResultRecord = statisticalReportCtrl.GetEmployeeExamResultRecordByEmployeeId(employeeId);
            if (!employeeExamResultRecord.IsNull())
            {
                string safetyKnowledgeExamScore = employeeExamResultRecord.SafetyKnowledgeExamScore.IsNull() ? "" : employeeExamResultRecord.SafetyKnowledgeExamScore.ToString();
                string managementAbilityExamScore = employeeExamResultRecord.ManagementAbilityExamScore.IsNull() ? "" : employeeExamResultRecord.ManagementAbilityExamScore.ToString();
                string fieldExamResult = employeeExamResultRecord.FieldExamResult.IsNull() ? "" : employeeExamResultRecord.FieldExamResult == true ? "合格" : "不合格";
                string remark = "";
                if (!safetyKnowledgeExamScore.IsNull())
                {
                    remark += "  安全知识考核成绩【" + safetyKnowledgeExamScore + "】";
                }
                if (!managementAbilityExamScore.IsNull())
                {
                    remark += "   管理能力考核成绩【" + managementAbilityExamScore + "】";
                }
                if (!fieldExamResult.IsNull())
                {
                    remark += "   实操考核成绩【" + fieldExamResult + "】";
                }
                WorkFlowRecord workFlowRecord = new WorkFlowRecord()
                {
                    OperaDateTime = employeeExamResultRecord.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    OperaUserName = statisticalReportCtrl.GetUserName(employeeExamResultRecord.CreateById),
                    Operation = "完成考试",
                    Remark = ""
                };
                workFlowRecordList.Add(workFlowRecord);
                if (employeeExamResultRecord.SummitStatus)
                {
                    WorkFlowRecord SubmitExamResultWorkFlowRecord = new WorkFlowRecord()
                    {
                        OperaDateTime = Convert.ToDateTime(employeeExamResultRecord.SummitDate).ToString("yyyy-MM-dd HH:mm:ss"),
                        OperaUserName = statisticalReportCtrl.GetUserName(Convert.ToInt32(employeeExamResultRecord.SummitById)),
                        Operation = "提交考试结果",
                        Remark = remark
                    };
                }
            }
            else
            {
                return workFlowRecordList;
            }

            Biz_EmployeeExamResultCheckedRecord employeeExamResultCheckedRecord = statisticalReportCtrl.GetEmployeeExamResultCheckedRecordByEmployeeId(employeeId);
            if (!employeeExamResultCheckedRecord.IsNull())
            {
                WorkFlowRecord workFlowRecord = new WorkFlowRecord()
                {
                    OperaDateTime = employeeExamResultCheckedRecord.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    OperaUserName = statisticalReportCtrl.GetUserName(employeeExamResultCheckedRecord.CreateById),
                    Operation = employeeExamResultCheckedRecord.CheckedStatus ? "审核成绩通过" : "审核成绩未通过",
                    Remark = employeeExamResultCheckedRecord.CheckedMark
                };
                workFlowRecordList.Add(workFlowRecord);
            }
            else
            {
                return workFlowRecordList;
            }

            Biz_EmployeeCertificateIssuanceRecord employeeCertificateIssuanceRecord = statisticalReportCtrl.GetEmployeeCertificateIssuanceRecordByEmployeeId(employeeId);
            if (!employeeCertificateIssuanceRecord.IsNull())
            {
                WorkFlowRecord workFlowRecord = new WorkFlowRecord()
                {
                    OperaDateTime = employeeCertificateIssuanceRecord.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    OperaUserName = statisticalReportCtrl.GetUserName(employeeCertificateIssuanceRecord.CreateById),
                    Operation = "发放证书",
                    Remark = employeeCertificateIssuanceRecord.Remark
                };
                workFlowRecordList.Add(workFlowRecord);
            }
            else
            {
                return workFlowRecordList;
            }

            return workFlowRecordList;

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
    }
}
