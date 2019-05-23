using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using BLL;
using Library.baseFn;
using Model;
using System.IO;

namespace SafetyProfessionalAssessmentSystem.Controllers
{
    [AuthorizeFilter]
    public class MakeExaminationPlanController : BaseController
    {
        //
        // GET: /MakeExaminationPaln/

        public ActionResult Index()
        {
            return View();
        }
        //获取待安排考试计划的人员数量
        public int GetNotInExamPlanCount()
        {
            int NotInExamPlanEmployeeCnt = 0;
            try
            {
                Sys_Account account = LoginAccount as Sys_Account;
                IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);

                NotInExamPlanEmployeeCnt = workFlowCtrl.GetNotInExamPlanEmployeeCountByCityList(base.LoginDataPermissionDetailList.Select(p => p.DetailName).ToList());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return NotInExamPlanEmployeeCnt;

        }

        #region 获取考核点List
        public JsonResult GetTrainingInstitutionListByCiyList()
        {
            Sys_Account account = LoginAccount as Sys_Account;
            IExaminationPointCtrl trainingInstitutionCtrl = new ExaminationPointCtrl(account);
            List<Biz_ExaminationPoint> trainingInstitutionList = trainingInstitutionCtrl.GetExaminationPointListByCityList(base.LoginDataPermissionDetailList.Select(p => p.DetailName).ToList());
            List<TrainingInstitutionListResult> trainingInstitutionListResult = trainingInstitutionList.Select(p => new TrainingInstitutionListResult()
            {
                TrainingInstitutionId = p.Id,
                TrainingInstitutionName = p.InstitutionName
            }).ToList();
            return Json(trainingInstitutionListResult, JsonRequestBehavior.DenyGet);
        }
        public JsonResult GetTrainingInstitutionList()
        {
            Sys_Account account = LoginAccount as Sys_Account;
            IExaminationPointCtrl trainingInstitutionCtrl = new ExaminationPointCtrl(account);
            List<Biz_ExaminationPoint> trainingInstitutionList = trainingInstitutionCtrl.GetExaminationPointList();
            List<TrainingInstitutionListResult> trainingInstitutionListResult = trainingInstitutionList.Select(p => new TrainingInstitutionListResult()
            {
                TrainingInstitutionId = p.Id,
                TrainingInstitutionName = p.InstitutionName
            }).ToList();
            return Json(trainingInstitutionListResult, JsonRequestBehavior.DenyGet);
        }
        public class TrainingInstitutionListResult
        {
            public int TrainingInstitutionId { get; set; }
            public string TrainingInstitutionName { get; set; }
        }
        #endregion

        #region  制定考试计划 自动分配考生
        public JsonResult MakeExamPlanAndAutoAssign(MakeExamPlanAndAutoAssignParam param)
        {
            ResultMessage resultMessage = new ResultMessage();
            resultMessage.IsSuccess = true;
            try
            {
                Sys_Account account = LoginAccount as Sys_Account;
                IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
                string cityName = base.LoginDataPermissionDetailList.SingleOrDefault().DetailName;
                Biz_ExamPlanRecord examPlan = new Biz_ExamPlanRecord()
                {
                    ExamDateTimeBegin = param.ExamDateTimeBegin,
                    //AExamDateTimeEnd = param.AExamDateTimeEnd,
                    // BExamDateTimeBegin = param.BExamDateTimeBegin,
                    // ExamDateTimeEnd = param.ExamDateTimeEnd
                };
                workFlowCtrl.MakeExamPlanAndAutoAssign(examPlan, param.TrainingInstitutionId, param.AutoAssignCount, cityName);
            }
            catch (Exception ex)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = ex.Message;
            }
            return Json(resultMessage, JsonRequestBehavior.DenyGet);
        }
        public class MakeExamPlanAndAutoAssignParam
        {
            public int TrainingInstitutionId { get; set; }
            public DateTime ExamDateTimeBegin { get; set; }
            public DateTime AExamDateTimeEnd { get; set; }
            public DateTime BExamDateTimeBegin { get; set; }
            public DateTime ExamDateTimeEnd { get; set; }
            public int AutoAssignCount { get; set; }
        }
        #endregion

        #region 根据考核点获取考场
        public JsonResult GetExamRoomByTrainingInstitutionId(int trainingInstitutionId)
        {
            List<ExamRoomResult> examRoomResultList = null;
            try
            {
                Sys_Account account = LoginAccount as Sys_Account;
                IExaminationPointCtrl trainingInstitutionCtrl = new ExaminationPointCtrl(account);
                List<Biz_ExaminationRoom> examRoomList = trainingInstitutionCtrl.GetExaminationRoomListByExaminationPointId(trainingInstitutionId, true);
                examRoomResultList = examRoomList.Select(p => new ExamRoomResult()
                {
                    ExamRoomId = p.Id,
                    ExamRoomName = p.ExamRoomName + "核定人数【" + p.PersonCount + "】"
                }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(examRoomResultList, JsonRequestBehavior.DenyGet);
        }
        public class ExamRoomResult
        {
            public int ExamRoomId { get; set; }
            public string ExamRoomName { get; set; }
        }
        #endregion

        #region 获取待分配考生的计划列表
        public JsonResult GetMakePlanEmployeeListForJqgrid(GetMakePlanEmployeeListForJqgridParam param)
        {
            try
            {
                List<MakePlanEmployeeListResult> MakePlanEmployeeListResultList = new List<MakePlanEmployeeListResult>();
                int totalCount = 0;

                Sys_Account account = LoginAccount as Sys_Account;
                List<String> cityList = LoginDataPermissionDetailList.Select(p => p.DetailName).ToList();
                IWorkFlowCtrl workCtrl = new WorkFlowCtrl(account);
                IEnterpriseCtrl enterpriseCtrl = new EnterpriseCtrl(account);
                List<Biz_Employee> employeeList = workCtrl.GetMakeExamPlanEmployeeList(param.ExamPlanNumber, param.TrainingInstitutionId, param.ExamRoomId, param.ConditionStr, param.page, param.rows, ref totalCount, cityList);
                List<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordList = workCtrl.GetEmployeeForExamPlanRecordByEmployeeIdList(employeeList.Select(e => e.Id).ToList()).ToList();
                List<Biz_Enterprise> enterpriseList = enterpriseCtrl.GetEnterpriseInfoByIdList(employeeList.Select(p => p.EnterpriseId).ToList());

                MakePlanEmployeeListResultList = employeeList.Join(enterpriseList, a => a.EnterpriseId, b => b.Id, (a, b) => new MakePlanEmployeeListResult()
                {
                    EmployeeId = a.Id,
                    EnterpriseName = b.EnterpriseName,
                    EmployeeName = a.EmployeeName,
                    Sex = a.Sex,
                    Age = (DateTime.Now.Year - a.Birthday.Year),
                    IDNumber = a.IDNumber,
                    Industry = a.Industry,
                    ExamType = a.ExamType,
                    Job = a.Job,
                    Title4Technical = a.Title4Technical
                }).ToList();
                MakePlanEmployeeListResultList = MakePlanEmployeeListResultList.GroupJoin(employeeForExamPlanRecordList, a => a.EmployeeId, b => b.EmployeeId, (a, b) => new MakePlanEmployeeListResult()
                {
                    EmployeeId = a.EmployeeId,
                    EnterpriseName = a.EnterpriseName,
                    EmployeeName = a.EmployeeName,
                    Sex = a.Sex,
                    Age = a.Age,
                    IDNumber = a.IDNumber,
                    Industry = a.Industry,
                    ExamType = a.ExamType,
                    Job = a.Job,
                    Title4Technical = a.Title4Technical,
                    MakePlanStatus = b.Count() == 0 ? "未安排" : "已安排"
                }).ToList();
                MakePlanEmployeeListResultList = MakePlanEmployeeListResultList.OrderBy(p => p.EnterpriseName).ToList();
                JqGridData dataResult = ConvertIListToJqGridData(param.page, param.rows, totalCount, MakePlanEmployeeListResultList);
                return Json(dataResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ResultMessage resultMsg = new ResultMessage() { IsSuccess = false, ErrorMessage = ex.Message };
                return Json(resultMsg, JsonRequestBehavior.AllowGet);
            }
        }

        public class GetMakePlanEmployeeListForJqgridParam
        {
            public string ExamPlanNumber { get; set; }
            public int TrainingInstitutionId { get; set; }
            public int ExamRoomId { get; set; }
            public string ConditionStr { get; set; }
            public int page { get; set; }
            public int rows { get; set; }
        }

        public class MakePlanEmployeeListResult
        {
            public int EmployeeId { get; set; }
            public string EnterpriseName { get; set; }
            public string EmployeeName { get; set; }
            public string Sex { get; set; }
            public int Age { get; set; }
            public string IDNumber { get; set; }
            public string Industry { get; set; }
            public string ExamType { get; set; }
            public string Job { get; set; }
            public string Title4Technical { get; set; }
            public string MakePlanStatus { get; set; }

        }
        #endregion

        #region 获取考试计划记录

        public JsonResult GetExamPlanRecoderListForJqgrid(GetExamPlanRecoderListForJqgridParam param)
        {
            int totalCount = 0;
            Sys_Account account = LoginAccount as Sys_Account;
            List<String> cityList = LoginDataPermissionDetailList.Select(p => p.DetailName).ToList();
            IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
            List<Biz_ExamPlanRecord> examPlanList = workFlowCtrl.GetExamPlanRecord(param.ExamPlanNumber,
                param.ExamDatetimeBegin, param.ExamDatetimeEnd, param.TrainingInstutionName, param.EmployeeName, param.IDNumber, param.SubmitStatus, param.page, param.rows, ref totalCount, cityList);
            List<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordList = workFlowCtrl.GetEmployeeForExamPlanRecordByExamPlanIdList(examPlanList.Select(e => e.Id).ToList()).ToList();

            List<ExamPlanRecoderListResult> examPlanRecoderListResultList = examPlanList.GroupJoin(employeeForExamPlanRecordList, a => a.Id, b => b.ExamPlanRecordId, (a, b) => new ExamPlanRecoderListResult()
            {
                ExamPlanId = a.Id,
                ExamPlanNumber = a.ExamPlanNumber,
                ExamDatetimeBegin = a.ExamDateTimeBegin.ToString("yyyy-MM-dd HH:mm"),
                ExamDatetimeEnd = a.ExamDateTimeEnd.ToString("yyyy-MM-dd HH:mm"),
                ExamCount = b.Count(),
                SubmitStatus = a.SubmitStatus ? "已提交" : "未提交"
            }).ToList();

            JqGridData dataResult = ConvertIListToJqGridData(param.page, param.rows, totalCount, examPlanRecoderListResultList);
            return Json(dataResult, JsonRequestBehavior.AllowGet);
        }
        public class GetExamPlanRecoderListForJqgridParam
        {
            public string ExamPlanNumber { get; set; }
            public string ExamDatetimeBegin { get; set; }
            public string ExamDatetimeEnd { get; set; }
            public string TrainingInstutionName { get; set; }
            public string EmployeeName { get; set; }
            public string IDNumber { get; set; }
            public string SubmitStatus { get; set; }
            public int page { get; set; }
            public int rows { get; set; }
        }

        public class ExamPlanRecoderListResult
        {
            public int ExamPlanId { get; set; }
            public string ExamPlanNumber { get; set; }
            // public string TrainingInstutionName { get; set; }
            public string ExamDatetimeBegin { get; set; }
            public string ExamDatetimeEnd { get; set; }
            //ublic string ExamAddress { get; set; }
            public int ExamCount { get; set; }
            public string SubmitStatus { get; set; }

        }
        #endregion

        #region 获取考试计划记录子表  人员信息
        public JsonResult GetEmployeeForExamPlanRecoderListForJqgrid(GetEmployeeForExamPlanRecoderListParam param)
        {
            List<GetEmployeeForExamPlanRecoderListResultItem> getEmployeeForExamPlanRecoderListResultItemList = new List<GetEmployeeForExamPlanRecoderListResultItem>();
            int totalCount = 0;

            Sys_Account account = LoginAccount as Sys_Account;
            // List<String> cityList = LoginDataPermissionDetailList.Select(p => p.DetailName).ToList();
            IWorkFlowCtrl workCtrl = new WorkFlowCtrl(account);
            IEnterpriseCtrl enterpriseCtrl = new EnterpriseCtrl(account);
            IExaminationPointCtrl trainingInstitutionCtrl = new ExaminationPointCtrl(account);
            List<Biz_Employee> employeeList = workCtrl.GetEmployeeForExamPlanRecord(param.ExamPlanNumber, param.TrainingInstutionName, param.EmployeeName, param.IDNumber, param.SubmitStatus, param.page, param.rows, ref totalCount, null);
            List<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordList = workCtrl.GetEmployeeForExamPlanRecordByEmployeeIdList(employeeList.Select(e => e.Id).ToList()).ToList();
            List<Biz_ExaminationRoom> examroomList = trainingInstitutionCtrl.GetExaminationRoomByIdList(employeeForExamPlanRecordList.Select(p => p.ExamRoomId).ToList());
            List<Biz_ExaminationPoint> trainingInstitutionList = trainingInstitutionCtrl.GetExaminationPointByIdList(examroomList.Select(p => p.ExaminationPointId).ToList());
            List<Biz_Enterprise> enterpriseList = enterpriseCtrl.GetEnterpriseInfoByIdList(employeeList.Select(p => p.EnterpriseId).ToList());

            getEmployeeForExamPlanRecoderListResultItemList = employeeList.Join(employeeForExamPlanRecordList, a => a.Id, b => b.EmployeeId, (a, b) => new GetEmployeeForExamPlanRecoderListResultItem()
            {
                EmployeeId = a.Id,
                ExamRoomId = b.ExamRoomId,
                EmployeeName = a.EmployeeName,
                Sex = a.Sex,
                Age = (DateTime.Now.Year - a.Birthday.Year),
                IDNumber = a.IDNumber,
                Job = a.Job,
                Title4Technical = a.Title4Technical,
                Industry = a.Industry,
                ExamType = a.ExamType,
                ExamSeatNumber = b.ExamSeatNumber,
                EnterpriseId = a.EnterpriseId
            }).ToList();
            getEmployeeForExamPlanRecoderListResultItemList = getEmployeeForExamPlanRecoderListResultItemList.GroupJoin(examroomList, a => a.ExamRoomId, b => b.Id, (a, b) => new GetEmployeeForExamPlanRecoderListResultItem()
            {
                EmployeeId = a.EmployeeId,
                ExamRoomId = a.ExamRoomId,
                ExamRoomName = b.Count() == 0 ? "考场不存在" : b.Max(o => o.ExamRoomName),
                TrainingInstitutionId = b.Count() == 0 ? 0 : b.Max(o => o.ExaminationPointId),
                EmployeeName = a.EmployeeName,
                Sex = a.Sex,
                Age = a.Age,
                IDNumber = a.IDNumber,
                Job = a.Job,
                Title4Technical = a.Title4Technical,
                Industry = a.Industry,
                ExamType = a.ExamType,
                ExamSeatNumber = a.ExamSeatNumber,
                EnterpriseId = a.EnterpriseId
            }).ToList();
            getEmployeeForExamPlanRecoderListResultItemList = getEmployeeForExamPlanRecoderListResultItemList.GroupJoin(trainingInstitutionList, a => a.TrainingInstitutionId, b => b.Id, (a, b) => new GetEmployeeForExamPlanRecoderListResultItem()
            {
                EmployeeId = a.EmployeeId,
                ExamRoomId = a.ExamRoomId,
                ExamRoomName = a.ExamRoomName,
                TrainingInstitutionId = a.TrainingInstitutionId,
                TrainingInstitutionName = b.Count() == 0 ? "" : b.Max(o => o.InstitutionName),
                EmployeeName = a.EmployeeName,
                Sex = a.Sex,
                Age = a.Age,
                IDNumber = a.IDNumber,
                Job = a.Job,
                Title4Technical = a.Title4Technical,
                Industry = a.Industry,
                ExamType = a.ExamType,
                ExamSeatNumber = a.ExamSeatNumber,
                EnterpriseId = a.EnterpriseId
            }).ToList();
            getEmployeeForExamPlanRecoderListResultItemList = getEmployeeForExamPlanRecoderListResultItemList.Join(enterpriseList, a => a.EnterpriseId, b => b.Id, (a, b) => new GetEmployeeForExamPlanRecoderListResultItem()
            {
                EmployeeId = a.EmployeeId,
                ExamRoomId = a.ExamRoomId,
                ExamRoomName = a.ExamRoomName,
                TrainingInstitutionId = a.TrainingInstitutionId,
                TrainingInstitutionName = a.TrainingInstitutionName,
                EmployeeName = a.EmployeeName,
                Sex = a.Sex,
                Age = a.Age,
                IDNumber = a.IDNumber,
                Job = a.Job,
                Title4Technical = a.Title4Technical,
                Industry = a.Industry,
                ExamType = a.ExamType,
                EnterpriseId = a.EnterpriseId,
                ExamSeatNumber = a.ExamSeatNumber,
                EnterpriseName = b.EnterpriseName
            }).ToList();
            JqGridData dataResult = ConvertIListToJqGridData(param.page, param.rows, totalCount, getEmployeeForExamPlanRecoderListResultItemList);
            return Json(dataResult, JsonRequestBehavior.AllowGet);
        }

        public class GetEmployeeForExamPlanRecoderListParam
        {
            public string ExamPlanNumber { get; set; }
            public string TrainingInstutionName { get; set; }
            public string EmployeeName { get; set; }
            public string IDNumber { get; set; }
            public string SubmitStatus { get; set; }
            public int page { get; set; }
            public int rows { get; set; }
        }

        public class GetEmployeeForExamPlanRecoderListResultItem
        {
            public int EmployeeId { get; set; }
            public int TrainingInstitutionId { get; set; }
            public string TrainingInstitutionName { get; set; }
            public int ExamRoomId { get; set; }
            public string ExamRoomName { get; set; }
            public int EnterpriseId { get; set; }
            public string EnterpriseName { get; set; }
            public string EmployeeName { get; set; }
            public string Sex { get; set; }
            public int Age { get; set; }
            public string IDNumber { get; set; }
            public string Job { get; set; }
            public string Title4Technical { get; set; }
            public string Industry { get; set; }
            public string ExamType { get; set; }
            public int ExamSeatNumber { get; set; }

        }
        #endregion

        #region  制定考试计划 手动分配考生
        public JsonResult MakeExamPlanAndManualAssign(string strData)
        {
            ResultMessage resultMessage = new ResultMessage();
            resultMessage.IsSuccess = true;
            try
            {
                MakeExamPlanAndAutoManualParam param = strData.JSONStringToObj<MakeExamPlanAndAutoManualParam>();
                Sys_Account account = LoginAccount as Sys_Account;
                IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
                Biz_ExamPlanRecord examPlan = new Biz_ExamPlanRecord()
                {
                    ExamPlanNumber = param.ExamPlanNumber,
                    ExamDateTimeBegin = param.ExamDatetimeBegin,
                    //AExamDateTimeEnd = param.AExamDatetimeEnd,
                    //BExamDateTimeBegin = param.BExamDatetimeBegin,
                    // ExamDateTimeEnd = param.ExamDatetimeEnd
                };
                workFlowCtrl.MakeExamPlanAndManualAssign(examPlan, param.ExamRoomId, param.EmployeeIdList);
            }
            catch (Exception ex)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = ex.Message;
            }
            return Json(resultMessage, JsonRequestBehavior.DenyGet);
        }
        public class MakeExamPlanAndAutoManualParam
        {
            public string ExamPlanNumber { get; set; }
            public int ExamRoomId { get; set; }
            public DateTime ExamDatetimeBegin { get; set; }
            //public DateTime AExamDatetimeEnd { get; set; }
            //public DateTime BExamDatetimeBegin { get; set; }
            //public DateTime ExamDatetimeEnd { get; set; }
            public List<int> EmployeeIdList { get; set; }
        }
        #endregion

        #region 获取流水号
        public string GetExamPlanNumber()
        {
            string examPlanNumber = "";
            try
            {
                Sys_Account account = LoginAccount as Sys_Account;
                string cityName = LoginDataPermissionDetailList.SingleOrDefault().DetailName;
                IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
                examPlanNumber = workFlowCtrl.GetExamPlanNumber(cityName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return examPlanNumber;
        }
        #endregion


        #region  获取考试计划
        public JsonResult GetExamPlanInfo(int ExamPlanId)
        {
            GetExamPlanInfoResult result = new GetExamPlanInfoResult();
            try
            {
                Sys_Account account = LoginAccount as Sys_Account;
                IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
                Biz_ExamPlanRecord examPlan = workFlowCtrl.GetExamPlanRecordById(ExamPlanId);
                result.ExamPlanNumber = examPlan.ExamPlanNumber;
                result.ExamDateTimeBegin = examPlan.ExamDateTimeBegin.ToString("yyyy-MM-dd HH:mm");
                //result.AExamDateTimeEnd = examPlan.AExamDateTimeEnd.ToString("yyyy-MM-dd HH:mm");
                //result.BExamDateTimeBegin = examPlan.BExamDateTimeBegin.ToString("yyyy-MM-dd HH:mm");
                //result.ExamDateTimeEnd = examPlan.ExamDateTimeEnd.ToString("yyyy-MM-dd HH:mm");

                Biz_ExaminationRoom examRoom = workFlowCtrl.GetExamRoomByExamPlanId(ExamPlanId).FirstOrDefault();
                if (examRoom != null)
                {
                    result.ExamRoomId = examRoom.Id;
                    IExaminationPointCtrl examinationPointCtrl = new ExaminationPointCtrl(account);
                    Biz_ExaminationPoint trainingInstitution = examinationPointCtrl.GetExaminationPointById(examRoom.ExaminationPointId);
                    result.TrainingInstitutionId = trainingInstitution.Id;
                }
                return Json(result);

            }
            catch (Exception ex)
            {
                ResultMessage resultMessage = new ResultMessage() { IsSuccess = false, ErrorMessage = ex.Message };
                return Json(resultMessage);
            }
        }
        public class GetExamPlanInfoResult
        {
            public string ExamPlanNumber { get; set; }
            public string ExamDateTimeBegin { get; set; }
            //public string AExamDateTimeEnd { get; set; }
            //public string BExamDateTimeBegin { get; set; }
            // public string ExamDateTimeEnd { get; set; }
            public int TrainingInstitutionId { get; set; }
            public int ExamRoomId { get; set; }
        }
        #endregion

        #region 获取考试计划信息 注册考试计划到考试核心
        public JsonResult GetPostExamData(int examPlanId)
        {
            PostExamParam resultParam = new PostExamParam()
            {
                resultMessage = new ResultMessage()
            };
            try
            {
                Sys_Account account = base.LoginAccount as Sys_Account;
                IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);

                //获取考试计划信息
                Biz_ExamPlanRecord examplan = workFlowCtrl.GetExamPlanRecordById(examPlanId);

                //考试核心站点地址
                string examCoreUrl = AppFn.GetAppSettingsValue("ExamCoreUrl");

                //注册考生地址
                string examRegisterUrl = examCoreUrl + AppFn.GetAppSettingsValue("ExamRegisterUrl");


                //本地站点地址
                string localtionUrl = AppFn.GetAppSettingsValue("LocaltionUrl");

                //考试成绩回写地址
                string callBack_SetExamResult = localtionUrl + AppFn.GetAppSettingsValue("CallBack_SetExamResultUrl");

                //考试完成 回调的页面
                string callBack_ReturnUrl = localtionUrl + AppFn.GetAppSettingsValue("CallBack_ReturnUrl");

                //获取不同考试类型对应的paperid
                List<Biz_PaperForExamType> PaperForExamTypeList = workFlowCtrl.GetPaperForExamTypeByExamPlanId(examPlanId);
                //List<Biz_PaperForExamType> PaperForExamTypeGroupList = PaperForExamTypeList.GroupBy(p => new { p.Industry, p.ExamType }).Select(t => t.Key).Select(p => new Biz_PaperForExamType() { Industry = p.Industry, ExamType = p.ExamType }).ToList();

                //获取考试计划的参考人员信息
                List<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordList = workFlowCtrl.GetEmployeeForExamPlanRecordByExamPlanId(examPlanId);

                List<Biz_Employee> employeeList = workFlowCtrl.GetEmployeeListByEmployeeIdList(employeeForExamPlanRecordList.Select(p => p.EmployeeId).ToList());

                List<ExamTakerInfo> examTakerInfoList = PaperForExamTypeList.Join(employeeList, p => new { p.Industry, p.ExamType }, q => new { q.Industry, q.ExamType }, (p, q) => new ExamTakerInfo() { IdNumber = q.IDNumber, PaperId = p.PaperId, Name = q.EmployeeName }).ToList();



                resultParam.TimeLimitFlag = true;
                resultParam.ExamBeginTime = examplan.ExamDateTimeBegin.ToString("yyyy-MM-dd HH:mm:ss");
                resultParam.ExamEndTime = examplan.ExamDateTimeEnd.ToString("yyyy-MM-dd HH:mm:ss");
                resultParam.ExamRegisterUrl = examRegisterUrl;
                resultParam.ExamResultUrl = callBack_SetExamResult;
                resultParam.ReturnUrl = callBack_ReturnUrl;
                resultParam.ExamTakerInfoList = examTakerInfoList;

                List<Paper> paperList = new List<Paper>();

                int examInterval = Convert.ToInt32(AppFn.GetAppSettingsValue("ExamInterval"));
                foreach (Biz_PaperForExamType item in PaperForExamTypeList)
                {
                    List<Biz_PaperForExamType> PaperForExamTypeTmpList = PaperForExamTypeList.Where(p => p.ExamType == item.ExamType).Where(p => p.Industry == item.Industry).OrderBy(p => p.SEQ).ToList();

                    //如果Biz_PaperForExamType表中查到的对应行业的试卷数量小于2，则说明系统配置错误
                    int paperCount = PaperForExamTypeTmpList.Count();
                    if (paperCount < 2)
                    {
                        throw new Exception("试卷数量不够");
                    }
                    //安全知识考试时间段
                    Paper paperA = new Paper()
                    {
                        PaperId = PaperForExamTypeTmpList[0].PaperId,
                        ExamBeginTime = examplan.ExamDateTimeBegin.ToString("yyyy-MM-dd HH:mm"),
                        ExamEndTime = examplan.ExamDateTimeBegin.AddMinutes(PaperForExamTypeTmpList[0].Duration).ToString("yyyy-MM-dd HH:mm")
                    };
                    paperList.Add(paperA);
                    //管理能力考试时间段
                    Paper paperB = new Paper()
                    {
                        PaperId = PaperForExamTypeTmpList[1].PaperId,
                        ExamBeginTime = examplan.ExamDateTimeBegin.AddMinutes(examInterval).ToString("yyyy-MM-dd HH:mm"),
                        ExamEndTime = examplan.ExamDateTimeEnd.ToString("yyyy-MM-dd HH:mm")
                    };
                    paperList.Add(paperB);
                }
                resultParam.PaperList = paperList.GroupBy(a => new { a.PaperId, a.ExamBeginTime, a.ExamEndTime }).Select(p => new Paper()
                {
                    PaperId = p.Key.PaperId,
                    ExamBeginTime = p.Key.ExamBeginTime,
                    ExamEndTime = p.Key.ExamEndTime
                }).ToList();
                resultParam.resultMessage.IsSuccess = true;
            }
            catch (Exception ex)
            {
                resultParam.resultMessage.IsSuccess = false;
                resultParam.resultMessage.ErrorMessage = ex.Message;
            }
            return Json(resultParam, JsonRequestBehavior.AllowGet);
        }
        public class Paper
        {
            public int PaperId { get; set; }
            public string ExamBeginTime { get; set; }
            //结束时间
            public string ExamEndTime { get; set; }
        }

        public class ExamTakerInfo
        {
            public string IdNumber { get; set; }
            public string Name { get; set; }
            public int PaperId { get; set; }

        }

        public class PostExamParam
        {
            //是否限时考试
            public bool TimeLimitFlag { get; set; }
            //开始时间
            public string ExamBeginTime { get; set; }
            //结束时间
            public string ExamEndTime { get; set; }
            //注册考试Url
            public string ExamRegisterUrl { get; set; }
            //考试结果写入Url
            public string ExamResultUrl { get; set; }
            //考试结束回调Url
            public string ReturnUrl { get; set; }
            public List<Paper> PaperList { get; set; }
            public List<ExamTakerInfo> ExamTakerInfoList { get; set; }
            public ResultMessage resultMessage { get; set; }
        }
        #endregion

        #region   提交考试计划
        public JsonResult SubmitExamPlan(int examPlanId, int examCoreExamId)
        {
            ResultMessage resultMessage = new ResultMessage();
            resultMessage.IsSuccess = true;
            try
            {
                Sys_Account account = LoginAccount as Sys_Account;
                IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
                workFlowCtrl.SubmitExamPlan(examPlanId, examCoreExamId);
            }
            catch (Exception ex)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = ex.Message;
            }
            return Json(resultMessage, JsonRequestBehavior.DenyGet);
        }
        #endregion


    }

}
