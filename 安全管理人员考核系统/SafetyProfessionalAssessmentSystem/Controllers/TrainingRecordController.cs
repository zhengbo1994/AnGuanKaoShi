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
    public class TrainingRecordController : BaseController
    {
        //
        // GET: /培训记录管理/

        public ActionResult Index()
        {
            return View();
        }
        #region 根据查询条件获取jqgrid人员信息
        public class GetEmployeeListForJqgridParam
        {
            public string EnterpriseName { get; set; }
            public string EmployeeName { get; set; }
            public string IDNumber { get; set; }
            public string ExamType { get; set; }
            public string Industry { get; set; }
            public string TrainingType { get; set; }
            public string TrainingStatus { get; set; }
            public int page { get; set; }
            public int rows { get; set; }
            public string sidx { get; set; }//排序字段名称
            public string sord { get; set; }//排序方式asc desc
        }
        public JsonResult GetEmployeeListForJqgrid(GetEmployeeListForJqgridParam param)
        {
            int totalCount = 0;
            Sys_Account account = LoginAccount as Sys_Account;
            IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
            IEnterpriseCtrl enterpriseCtrl = new EnterpriseCtrl(account);
            IEmployeeCtrl rpEmployeeCtrl = new EmployeeCtrl(account);

            List<Biz_Employee> employeeList = workFlowCtrl.GetEmployeeList_TrainingRecord(param.EnterpriseName, param.EmployeeName, param.IDNumber,
                param.TrainingType, param.ExamType, param.Industry, param.TrainingStatus, param.page, param.rows, ref totalCount);
            List<Biz_TrainingRecord> trainingRecord = workFlowCtrl.GetTrainingRecordListByEmployeeIdList(employeeList.Select(p => p.Id).ToList());
            List<Biz_Enterprise> enterpriseList = enterpriseCtrl.GetEnterpriseInfoByIdList(employeeList.Select(p => p.EnterpriseId).ToList());

            List<EmployeeCtrl.StudyRecord> studyRecordList = rpEmployeeCtrl.GetOneYearGetOnlineStudyRecord(employeeList.Select(p => p.IDNumber).ToList());

            List<GetEmployeeListForJqgridJsonResult> employeeListJsonResult = employeeList.Join(enterpriseList, a => a.EnterpriseId, b => b.Id, (a, b) => new { a, b })
             .GroupJoin(trainingRecord, o => o.a.Id, c => c.EmployeeId, (o, c) => new { o.a, o.b, c = c.FirstOrDefault() })
             .Join(studyRecordList, o => o.a.IDNumber, d => d.IDNumber, (o, d) => new { o.a, o.b, o.c, d })
             .Select(o => new GetEmployeeListForJqgridJsonResult()
             {
                 Id = o.a.Id,
                 EmployeeName = o.a.EmployeeName,
                 Sex = o.a.Sex,
                 Age = o.a.Birthday.IsNull() ? "" : (DateTime.Now.Year - o.a.Birthday.ConvertToDateTime().Year).ToString(),
                 IDNumber = o.a.IDNumber,
                 Job = o.a.Job,
                 Title4Technical = o.a.Title4Technical,
                 ExamType = o.a.ExamType,
                 Industry = o.a.Industry,
                 TrainingType = o.a.TrainingType,
                 City = o.a.City,
                 EnterpriseName = o.b.EnterpriseName,
                 TrainingStatus = o.c == null ? (o.a.TrainingType == "线上培训" ? "" : "未审核") : o.c.PassStatus ? "审核通过" : "审核不通过",
                 TotalHours = o.a.TrainingType == "线上培训" ? (o.d.TotalHours.ToString()) : (o.c == null ? "0" : o.c.StudyTime),
                 OnlineExerciseMaxCore = o.a.TrainingType == "线上培训" ? (o.d.OnlineExerciseMaxCore.ToString()) : (o.c == null ? "0" : o.c.AbilityTestResult),
                 SubmitDate = (o.c != null && !o.c.SubmitDate.IsNull()) ? o.c.SubmitDate.ConvertToDateString() : ""
             }).ToList();

            JqGridData dataResult = ConvertIListToJqGridData(param.page, param.rows, totalCount, employeeListJsonResult);
            return Json(dataResult, JsonRequestBehavior.AllowGet);
        }
        public class GetEmployeeListForJqgridJsonResult
        {
            public int Id { get; set; }
            // 人员名称
            public string EmployeeName { get; set; }

            // 性别
            public string Sex { get; set; }
            //年龄
            public string Age { get; set; }

            //身份证号
            public string IDNumber { get; set; }

            // 所属单位
            public string EnterpriseName { get; set; }
            // 职位
            public string Job { get; set; }
            // 技术职称
            public string Title4Technical { get; set; }

            // 报考科目
            public string ExamType { get; set; }
            //报考行业
            public string Industry { get; set; }
            public string TrainingType { get; set; }
            // 报考城市
            public string City { get; set; }
            // 培训状态
            public string TrainingStatus { get; set; }
            //培训日期
            public string SubmitDate { get; set; }
            public string TotalHours { get; set; }
            public string OnlineExerciseMaxCore { get; set; }

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

        #region 获取培训信息
        public JsonResult GetTrainingRecord(int employeeId)
        {
            Sys_Account account = LoginAccount as Sys_Account;
            IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
            Biz_TrainingRecord trainingRecord = workFlowCtrl.GetTrainingRecord(employeeId);
            TrainingRecordResult result = new TrainingRecordResult()
            {
                Id = trainingRecord.Id,
                EmployeeId = trainingRecord.EmployeeId,
                StudyTime = trainingRecord.StudyTime,
                PracticeTime = trainingRecord.PracticeTime,
                AbilityTestResult = trainingRecord.AbilityTestResult,
                Remark = trainingRecord.Remark
            };
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        public class TrainingRecordResult
        {

            public int Id { get; set; }
            public int EmployeeId { get; set; }
            public string StudyTime { get; set; }
            public string PracticeTime { get; set; }
            public string AbilityTestResult { get; set; }
            public string Remark { get; set; }
        }
        #endregion

        #region 保存培训记录
        public JsonResult SaveTrainingRecord(TrainingRecordParam param)
        {
            ResultMessage resultMessage = new ResultMessage();
            try
            {
                Sys_Account account = base.LoginAccount as Sys_Account;
                IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
                Biz_TrainingRecord trainingRecord = new Biz_TrainingRecord()
                {
                    Id = param.Id,
                    EmployeeId = param.EmployeeId,
                    StudyTime = param.StudyTime,
                    PracticeTime = param.PracticeTime,
                    AbilityTestResult = param.AbilityTestResult,
                    Remark = param.Remark
                };
                workFlowCtrl.SaveTrainingRecord(trainingRecord);
                resultMessage.IsSuccess = true;
            }
            catch (Exception ex)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = ex.Message;
            }
            return Json(resultMessage, JsonRequestBehavior.DenyGet);
        }
        public class TrainingRecordParam
        {

            public int Id { get; set; }
            public int EmployeeId { get; set; }
            public string StudyTime { get; set; }
            public string PracticeTime { get; set; }
            public string AbilityTestResult { get; set; }
            public string Remark { get; set; }
        }
        #endregion

        #region 提交培训记录
        public JsonResult SubmitTrainRecord(string strData)
        {
            ResultMessage resultMessage = new ResultMessage();
            try
            {
                SubmitTrainRecordParam param = strData.JSONStringToObj<SubmitTrainRecordParam>();
                Sys_Account account = base.LoginAccount as Sys_Account;
                IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
                workFlowCtrl.SubmitTrainingRecord(param.employeeIdList);
                resultMessage.IsSuccess = true;
            }
            catch (Exception ex)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = ex.Message;
            }
            return Json(resultMessage, JsonRequestBehavior.DenyGet);
        }
        public class SubmitTrainRecordParam
        {
            public List<int> employeeIdList { get; set; }
        }
        #endregion

        #region 获取视频学习记录
        public JsonResult GetStudyByVideoRecordList(string IdNumber)
        {
            GetStudyByVideoRecordListResult result = new GetStudyByVideoRecordListResult()
            {
                resultMessage = new ResultMessage()
            };
            try
            {
                Sys_Account account = base.LoginAccount as Sys_Account;
                IEmployeeCtrl employeeCtrl = new EmployeeCtrl(account);
                List<Biz_StudyByVideoComplete> studyByVideoRecoderList = employeeCtrl.GetStudyByVideoCompleteList(IdNumber);
                result.data = studyByVideoRecoderList.OrderBy(p => p.CreateDate).Select(p => new GetStudyByVideoRecordListResultItem()
                {
                    videoName = p.VideoName,
                    completeDate = p.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
                }).ToList();
            }
            catch (Exception ex)
            {
                result.resultMessage.IsSuccess = false;
                result.resultMessage.ErrorMessage = ex.Message;
            }
            return Json(result);
        }
        public class GetStudyByVideoRecordListResultItem
        {
            public string videoName { get; set; }
            public string completeDate { get; set; }
        }
        public class GetStudyByVideoRecordListResult
        {
            public ResultMessage resultMessage { get; set; }
            public List<GetStudyByVideoRecordListResultItem> data { get; set; }
        }
        #endregion

        #region 获取在线练习记录
        public JsonResult GetOnlineExerciseRecord(string iDNumber)
        {
            GetOnlineExerciseRecordResult result = new GetOnlineExerciseRecordResult()
            {
                resultMessage = new ResultMessage()
            };
            try
            {
                Sys_Account account = base.LoginAccount as Sys_Account;
                IEmployeeCtrl employeeCtrl = new EmployeeCtrl(account);
                List<Biz_OnlineExerciseRecord> onlineExerciseRecordList = employeeCtrl.GetOnlineExerciseRecordListByIDNumber(iDNumber);

                GetOnlineExerciseRecordResult newGetOnlineExerciseRecordResult = onlineExerciseRecordList.GroupBy(p => p.IDNumber).Select(group =>
                   new GetOnlineExerciseRecordResult()
                   {
                       startDateTime = group.Min(p => p.StartDateTime).ToString("yyyy-MM-dd HH:mm:ss"),
                       endDateTime = group.Max(p => p.EndDateTime).ToString("yyyy-MM-dd HH:mm:ss"),
                       onlineExerciseCount = group.Count().ToString(),
                       maxScore = group.Max(p => p.Score).ToString()
                   }).FirstOrDefault();
                if (newGetOnlineExerciseRecordResult.IsNull())
                {
                    throw new Exception("无在线练习记录");
                }
                result = newGetOnlineExerciseRecordResult;
                result.resultMessage = new ResultMessage();
            }
            catch (Exception ex)
            {
                result.resultMessage.IsSuccess = false;
                result.resultMessage.ErrorMessage = ex.Message;
            }
            return Json(result);
        }
        public class GetOnlineExerciseRecordResult
        {
            public ResultMessage resultMessage { get; set; }
            public string startDateTime { get; set; }
            public string endDateTime { get; set; }
            public string onlineExerciseCount { get; set; }
            public string maxScore { get; set; }
        }
        #endregion

        #region 审核培训记录
        public class CheckTrainingRecordParam
        {
            public List<int> EmployeeIdList { get; set; }
            public string StudyTime { get; set; }
            public string PracticeTime { get; set; }
            public string AbilityTestResult { get; set; }
            public string Remark { get; set; }
            public bool PassFlag { get; set; }
        }
        public JsonResult CheckTrainingRecord(string strParam)
        {
            ResultMessage resultMessage = new ResultMessage();
            try
            {
                CheckTrainingRecordParam param = strParam.JSONStringToObj<CheckTrainingRecordParam>();
                Sys_Account account = base.LoginAccount as Sys_Account;
                IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
                workFlowCtrl.CheckTrainingRecord(param.EmployeeIdList, param.StudyTime, param.PracticeTime, param.AbilityTestResult, param.Remark, param.PassFlag);
                resultMessage.IsSuccess = true;
            }
            catch (Exception ex)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = ex.Message;
            }
            return Json(resultMessage);
        }

        #endregion
    }
}
