using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Library.baseFn;

namespace SafetyProfessionalAssessmentSystem.Controllers
{
    public class ViewStudyRecordController : BaseController
    {
        //
        // GET: /ViewStudyRecord/

        public ActionResult Index()
        {
            return View();
        }

        #region 根据查询条件获取jqgrid人员信息
        public class GetEmployeeListForJqgridParam
        {
            public string EnterpriseName { get; set; }
            public string EmployeeName { get; set; }
            public string ExamType { get; set; }
            public string Industry { get; set; }
            public string City { get; set; }
            public string Area { get; set; }
            public string IDNumber { get; set; }
            public int page { get; set; }
            public int rows { get; set; }
            public string sidx { get; set; }//排序字段名称
            public string sord { get; set; }//排序方式asc desc
        }

        public JsonResult GetEmployeeListForJqgrid(GetEmployeeListForJqgridParam param)
        {
            int totalCount = 0;
            Sys_Account account = LoginAccount as Sys_Account;
            List<string> cityList = base.LoginDataPermissionDetailList.Select(p => p.DetailName).ToList();
            IRP_EmployeeCtrl rpEmployeeCtrl = new RP_EmployeeCtrl(account);
            IEmployeeCtrl employeeCtrl = new EmployeeCtrl(account);
            IEnterpriseCtrl enterpriseCtrl = new EnterpriseCtrl(account);
            IExaminationPointCtrl trainingInstitutionCtrl = new ExaminationPointCtrl(account);

            List<Biz_RP_EmployeeRegistration> rpemployeeList = rpEmployeeCtrl.GetEmployeeList_ViewStudyRecord(param.EnterpriseName, param.City, param.Area, param.EmployeeName, param.IDNumber,
                param.ExamType, param.Industry, param.page, param.rows, ref totalCount, cityList);
            List<Biz_Enterprise> enterpriseList = enterpriseCtrl.GetEnterpriseInfoByIdList(rpemployeeList.Select(p => p.EnterpriseId).ToList());
            List<Biz_ExaminationPoint> trainingInstitution = trainingInstitutionCtrl.GetExaminationPointByIdList(rpemployeeList.Select(p => p.TrainingInstitutionId).ToList());
            List<string> idNumberList = rpemployeeList.Select(p => p.IDNumber).ToList();
            // List<RP_EmployeeCtrl.OnlineStudyRecord> onlineStudyRecordList = rpEmployeeCtrl.GetOnlineStudyRecord(idNumberList);
            List<Biz_StudyByVideoComplete> studyByVideoCompleteList = employeeCtrl.GetStudyByVideoCompleteList(idNumberList);
            // List<RP_EmployeeCtrl.OnlineExercise> onlineExerciseList = rpEmployeeCtrl.GetOnlineExerciseRecord(idNumberList);
            List<GetEmployeeListForJqgridJsonResult> employeeListJsonResult =
                rpemployeeList.Join(enterpriseList, a => a.EnterpriseId, b => b.Id, (a, b) => new { a, b })
                           .GroupJoin(trainingInstitution, o => o.a.TrainingInstitutionId, c => c.Id, (o, c) => new { o.a, o.b, c = c.FirstOrDefault() })
                           .GroupJoin(studyByVideoCompleteList, o => o.a.IDNumber, d => d.IDNumber, (o, d) => new { o.a, o.b, o.c, TotalPeriods = d.Count() })
                //.GroupJoin(onlineExerciseList, o => o.a.IDNumber, e => e.IDNumber, (o, e) => new { o.a, o.b, o.c, o.d, e = e.FirstOrDefault() })
                           .Select(o => new GetEmployeeListForJqgridJsonResult()
                           {
                               EmployeeId = o.a.Id,
                               EmployeeName = o.a.EmployeeName,
                               Sex = o.a.Sex,
                               Age = o.a.Birthday.IsNull() ? "" : (DateTime.Now.Year - o.a.Birthday.ConvertToDateTime().Year).ToString(),
                               IDNumber = o.a.IDNumber,
                               Job = o.a.Job,
                               Title4Technical = o.a.Title4Technical,
                               ExamType = o.a.ExamType,
                               ConstructorCertificateNo = o.a.ConstructorCertificateNo,
                               Industry = o.a.Industry,
                               City = o.a.City,
                               EnterpriseName = o.b.EnterpriseName,
                               EnterpriseCity = o.b.City,
                               EnterpriseArea = o.b.Area,
                               TrainingInstitutionName = o.c.IsNull() ? "" : o.c.InstitutionName,
                               TrainingType = !o.a.IsTraining ? "内训" : o.a.TrainingType,
                               TotalPeriods = o.TotalPeriods.ToString(),
                               // MaxScore = o.e.IsNull() ? "无" : o.e.MaxScore.ToString()
                           }).ToList();

            JqGridData dataResult = ConvertIListToJqGridData(param.page, param.rows, totalCount, employeeListJsonResult);
            return Json(dataResult, JsonRequestBehavior.AllowGet);
        }
        public class GetEmployeeListForJqgridJsonResult
        {
            public int EmployeeId { get; set; }
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
            public string ConstructorCertificateNo { get; set; }
            //报考行业
            public string Industry { get; set; }
            //企业城市
            public string EnterpriseCity { get; set; }
            //企业区域
            public string EnterpriseArea { get; set; }
            // 报考城市
            public string City { get; set; }
            public string TrainingType { get; set; }
            public string TrainingInstitutionName { get; set; }
            public string TotalPeriods { get; set; }
            public string MaxScore { get; set; }

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

        #region 审核人员
        public class CheckEmployeeListParam
        {
            public List<int> employeeIdList { get; set; }
            public bool passFlag { get; set; }
            public string checkedMark;
        }
        public JsonResult CheckEmployeeList(string strData)
        {
            ResultMessage resultMessage = new ResultMessage();
            try
            {
                CheckEmployeeListParam para = strData.JSONStringToObj<CheckEmployeeListParam>();
                Sys_Account account = base.LoginAccount as Sys_Account;
                IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
                workFlowCtrl.CheckEmployeeList(para.employeeIdList, para.passFlag, para.checkedMark);
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
                List<Biz_StudyByVideoRecoder> studyByVideoRecoderList = employeeCtrl.GetStudyByVideoRecoderListByIDNumber(IdNumber);
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
