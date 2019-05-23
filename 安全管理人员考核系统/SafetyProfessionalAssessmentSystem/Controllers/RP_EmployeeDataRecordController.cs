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
    public class RP_EmployeeDataCheckController : BaseController
    {
        //
        // GET: /继续教育人员资料审核/

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
            public string CheckStatus { get; set; }
            public int page { get; set; }
            public int rows { get; set; }
            public string sidx { get; set; }//排序字段名称
            public string sord { get; set; }//排序方式asc desc
        }
        public JsonResult GetEmployeeListForJqgrid(GetEmployeeListForJqgridParam param)
        {
            int totalCount = 0;
            Sys_Account account = LoginAccount as Sys_Account;
            IRP_EmployeeCtrl rpEmployeeCtrl = new RP_EmployeeCtrl(account);
            IEnterpriseCtrl enterpriseCtrl = new EnterpriseCtrl(account);
            List<Biz_RP_EmployeeRegistration> employeeList = rpEmployeeCtrl.GetEmployeeListForDataCheck(param.EnterpriseName, param.EmployeeName, param.IDNumber,
                param.ExamType, param.Industry, param.CheckStatus, param.page, param.rows, ref totalCount);
            List<Biz_Enterprise> enterpriseList = enterpriseCtrl.GetEnterpriseInfoByIdList(employeeList.Select(p => p.EnterpriseId).ToList());
            List<Biz_RP_EmployeeDataCheckedRecord> dataCheckedRecordList = rpEmployeeCtrl.GetEmployeeDataCheckedRecordListByRPEmployeeIdList(employeeList.Select(p => p.Id).ToList());
            List<GetEmployeeListForJqgridJsonResult> employeeListJsonResult = employeeList.Join(enterpriseList, a => a.EnterpriseId, b => b.Id, (a, b) => new { a, b })
             .GroupJoin(dataCheckedRecordList, p => p.a.Id, q => q.EmployeeId, (p, q) => new { p, q })
             .Select(o => new GetEmployeeListForJqgridJsonResult()
             {
                 Id = o.p.a.Id,
                 EmployeeName = o.p.a.EmployeeName,
                 Sex = o.p.a.Sex,
                 Age = o.p.a.Birthday.IsNull() ? "" : (DateTime.Now.Year - o.p.a.Birthday.ConvertToDateTime().Year).ToString(),
                 IDNumber = o.p.a.IDNumber,
                 Job = o.p.a.Job,
                 Title4Technical = o.p.a.Title4Technical,
                 ExamType = o.p.a.ExamType,
                 Industry = o.p.a.Industry,
                 City = o.p.a.City,
                 EnterpriseName = o.p.b.EnterpriseName,
                 CheckStatus = o.q.Count() == 0 ? "未审核" : o.q.First().PassStatus ? "审核通过" : "审核不通过",
                 CheckDate = o.q.Count() == 0 ? "" : o.q.First().CreateDate.ConvertToDateString()
             }).ToList();
            JqGridData dataResult = ConvertIListToJqGridData(param.page, param.rows, totalCount, employeeListJsonResult);
            return Json(dataResult, JsonRequestBehavior.AllowGet);
        }
        public class GetEmployeeListForJqgridJsonResult
        {
            public int Id { get; set; }
            public string EmployeeName { get; set; }
            public string Sex { get; set; }
            public string Age { get; set; }
            public string IDNumber { get; set; }
            public string EnterpriseName { get; set; }
            public string Job { get; set; }
            public string Title4Technical { get; set; }
            public string ExamType { get; set; }
            public string Industry { get; set; }
            public string City { get; set; }
            public string CheckStatus { get; set; }
            public string CheckDate { get; set; }

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
        public JsonResult GetStudyByVideoRecordList(int employeeId)
        {
            GetStudyByVideoRecordListResult result = new GetStudyByVideoRecordListResult()
            {
                resultMessage = new ResultMessage()
            };
            try
            {
                Sys_Account account = base.LoginAccount as Sys_Account;
                IEmployeeCtrl employeeCtrl = new EmployeeCtrl(account);
                List<Biz_StudyByVideoRecoder> studyByVideoRecoderList = employeeCtrl.GetStudyByVideoRecoderListByEmployeeId(employeeId);
                result.data = studyByVideoRecoderList.OrderBy(p => p.CreateDateTime).Select(p => new GetStudyByVideoRecordListResultItem()
                {
                    videoName = p.VideoName,
                    studyHours = p.Studyhours.ToString(),
                    studyDateTimeStart = p.CreateDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    studyDateTimeEnd = p.UpdateDateTime.ToString("yyyy-MM-dd HH:mm:ss")
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
            public string studyHours { get; set; }
            public string studyDateTimeStart { get; set; }
            public string studyDateTimeEnd { get; set; }
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
    }
}
