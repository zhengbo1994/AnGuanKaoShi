using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Library.baseFn;
using Model;
using BLL;
using System.IO;

namespace SafetyProfessionalAssessmentSystem.Controllers
{
    [AuthorizeFilter]
    public class StartExamController : BaseController
    {
        //
        // GET: /StartExam/

        public ActionResult Index()
        {
            return View();
        }
        #region  获取考试信息
        public JsonResult GetExamInfo()
        {
            ExamInfoResult result = new ExamInfoResult();
            result.resultMessage = new ResultMessage()
            {
                IsSuccess = true
            };
            try
            {
                Sys_Account account = base.LoginAccount as Sys_Account;
                int employeeId = account.UserId;
                IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
                IEnterpriseCtrl enterpriseCtrl = new EnterpriseCtrl(account);
                IExaminationPointCtrl trainingInstitutionCtrl = new ExaminationPointCtrl(account);

                Biz_Employee employee = workFlowCtrl.GetEmployeeInfoById(employeeId);
                Biz_Enterprise enterprise = enterpriseCtrl.GetEnterpriseById(employee.EnterpriseId);

                Biz_ExamPlanRecord examPlan = workFlowCtrl.GetFirstExamPlanByEmployeeId(employeeId);
                Biz_ExaminationRoom examRoom = workFlowCtrl.GetExamRoom(examPlan.Id, employeeId);
                Biz_EmployeeForExamPlanRecord employeeForExamPlanRecord = workFlowCtrl.GetEmployeeForExamPlanRecordByEmployeeIdList(new List<int>() { employeeId }).FirstOrDefault();
                Biz_ExaminationPoint trainingInstitution = trainingInstitutionCtrl.GetExaminationPointById(examRoom.ExaminationPointId);
                //写入返回值
                result.EmployeeName = employee.EmployeeName;
                result.ExamRegistrationNumber = employeeForExamPlanRecord.ExamRegistrationNumber.IsNull() ? "" : employeeForExamPlanRecord.ExamRegistrationNumber + "(座位号:" + employeeForExamPlanRecord.ExamSeatNumber + ")";
                result.IDNumber = employee.IDNumber;
                result.ExamType = employee.ExamType;
                result.Industry = employee.Industry;
                result.Sex = employee.Sex;
                result.ExamCoreExamId = examPlan.ExamCoreExamId;
                result.ExamRoomName = examRoom.ExamRoomName;
                result.ExamDateTimeBegin = examPlan.ExamDateTimeBegin.ToString("yyyy-MM-dd HH:mm");
                result.ExamDateTimeEnd = examPlan.ExamDateTimeEnd.ToString("yyyy-MM-dd HH:mm");

                result.AExamDateTimeBegin = examPlan.ExamDateTimeBegin.ToString("yyyy-MM-dd HH:mm");
                result.AExamDateTimeEnd = examPlan.AExamDateTimeEnd.ToString("yyyy-MM-dd HH:mm");
                result.BExamDateTimeBegin = examPlan.BExamDateTimeBegin.ToString("yyyy-MM-dd HH:mm");
                result.BExamDateTimeEnd = examPlan.ExamDateTimeEnd.ToString("yyyy-MM-dd HH:mm");

                result.EnterpriseName = enterprise.EnterpriseName;
                result.TrainingInstitutionName = trainingInstitution.InstitutionName;
                result.TrainingInstitutionAddress = trainingInstitution.Address;
                //通过当前时间 判断是哪一场考试 只控制开始时间 默认开始时间是考试计划的开始时间
                DateTime examDateTimeBegin = examPlan.ExamDateTimeBegin;
                if (DateTime.Now >= examPlan.AExamDateTimeEnd)
                {
                    examDateTimeBegin = examPlan.BExamDateTimeBegin;
                }
                result.ExamDateTimeBegin = examDateTimeBegin.ToString("yyyy-MM-dd HH:mm");
            }
            catch (Exception ex)
            {
                result.resultMessage.IsSuccess = false;
                result.resultMessage.ErrorMessage = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public class ExamInfoResult
        {
            public string EmployeeName { get; set; }
            public string ExamRegistrationNumber { get; set; }
            public string IDNumber { get; set; }
            public string ExamType { get; set; }
            public string Industry { get; set; }
            public string Sex { get; set; }
            public int ExamCoreExamId { get; set; }
            public string AExamDateTimeBegin { get; set; }
            public string AExamDateTimeEnd { get; set; }
            public string BExamDateTimeBegin { get; set; }
            public string BExamDateTimeEnd { get; set; }
            public string ExamDateTimeBegin { get; set; }
            public string ExamDateTimeEnd { get; set; }
            public string ExamRoomName { get; set; }
            public string EnterpriseName { get; set; }
            public string TrainingInstitutionName { get; set; }
            public string TrainingInstitutionAddress { get; set; }
            public ResultMessage resultMessage { get; set; }
        }
        #endregion

        #region 获取注册考生的信息
        public JsonResult GetExamTakerInfo(int examCoreExamId, string paperType)
        {
            List<string> paperTypeList = new List<string>() { "SafetyKnowledgeExam", "ManagementAbilityExam" };
            if (!paperTypeList.Contains(paperType))
            {
                throw new Exception("试卷类别错误");
            }
            RegisterExamTakerParam examTakerParam = new RegisterExamTakerParam();
            examTakerParam.resultMessage = new ResultMessage();
            try
            {
                Sys_Account account = base.LoginAccount as Sys_Account;
                IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
                Biz_ExamPlanRecord examplan = workFlowCtrl.GetExamPlanRecordByExamCoreExamId(examCoreExamId, account.UserId);
                Biz_Employee employee = workFlowCtrl.GetEmployeeInfoById(account.UserId);
                Biz_PaperForExamType paperForExamType = workFlowCtrl.GetPaper(account.UserId, paperType);
                // Biz_PaperForExamType paperForExamType = workFlowCtrl.GetPaperIdForStartExam(examplan.Id, account.UserId);

                //考试核心站点地址
                string examCoreUrl = AppFn.GetAppSettingsValue("ExamCoreUrl");
                //开始答题页面
                string startExamUrl = examCoreUrl + AppFn.GetAppSettingsValue("StrarExamUrl");
                //注册考生地址
                string registerExamTakerUrl = examCoreUrl + AppFn.GetAppSettingsValue("RegisterExamTakerUrl");

                //赋值
                examTakerParam.examId = examplan.Id;
                examTakerParam.examCoreExamId = examplan.ExamCoreExamId;
                examTakerParam.examTakerName = employee.EmployeeName;
                examTakerParam.idNumber = employee.IDNumber;
                examTakerParam.paperId = paperForExamType.PaperId;
                examTakerParam.startExamUrl = startExamUrl;
                examTakerParam.registerExamTakerUrl = registerExamTakerUrl;

                if (paperType == "SafetyKnowledgeExam" && !(examplan.ExamDateTimeBegin <= DateTime.Now && DateTime.Now <= examplan.AExamDateTimeEnd))
                {
                    throw new Exception("不在安全知识考试时间段内");
                }
                if (paperType == "ManagementAbilityExam" && !(examplan.BExamDateTimeBegin <= DateTime.Now && DateTime.Now <= examplan.ExamDateTimeEnd))
                {
                    throw new Exception("不在管理能力考试时间段内");
                }


                //DateTime examDateTimeBegin = examplan.ExamDateTimeBegin;
                //DateTime examDateTimeEnd = examplan.ExamDateTimeEnd;

                //if (examplan.ExamDateTimeBegin <= DateTime.Now && DateTime.Now <= examplan.AExamDateTimeEnd)
                //{
                //    examDateTimeBegin = examplan.ExamDateTimeBegin;
                //    examDateTimeEnd = examplan.AExamDateTimeEnd;
                //}
                //else if (examplan.BExamDateTimeBegin <= DateTime.Now && DateTime.Now <= examplan.ExamDateTimeEnd)
                //{
                //    examDateTimeBegin = examplan.BExamDateTimeBegin;
                //    examDateTimeEnd = examplan.ExamDateTimeEnd;
                //}
                //else
                //{
                //    throw new Exception("不在考试时间段内");
                //}
                //examTakerParam.examDateTimeBegin = examDateTimeBegin.ToString("yyyy-MM-dd HH:mm");
                //examTakerParam.examDateTimeEnd = examDateTimeEnd.ToString("yyyy-MM-dd HH:mm");
                examTakerParam.resultMessage.IsSuccess = true;
            }
            catch (Exception ex)
            {
                examTakerParam.resultMessage.IsSuccess = false;
                examTakerParam.resultMessage.ErrorMessage = ex.Message;
            }
            return Json(examTakerParam, JsonRequestBehavior.AllowGet);
        }
        public class RegisterExamTakerParam
        {
            public int? examId { get; set; }
            //考试核心ExamId
            public int? examCoreExamId { get; set; }
            //考生名称
            public string examTakerName { get; set; }
            //身份证号
            public string idNumber { get; set; }
            //试卷Id
            public int? paperId { get; set; }
            //考试跳转Url
            public string startExamUrl { get; set; }
            public string registerExamTakerUrl { get; set; }
            public string examDateTimeBegin { get; set; }
            public string examDateTimeEnd { get; set; }
            public ResultMessage resultMessage { get; set; }
        }

        #endregion

        //#region 获取考试结果
        //public JsonResult GetExamResult(int examCoreExamId)
        //{
        //    ExamResult examResult = new ExamResult()
        //    {
        //        resultMessage = new ResultMessage()
        //        {
        //            IsSuccess = false
        //        }
        //    };
        //    try
        //    {
        //        Sys_Account account = base.LoginAccount as Sys_Account;
        //        IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
        //        Biz_EmployeeExamResultRecord employeeExamResultRecord = workFlowCtrl.GetEmployeeExamResultRecord(examCoreExamId, account.UserId);
        //        if (employeeExamResultRecord.IsNull())
        //        {
        //            throw new Exception("无考试结果");
        //        }
        //        examResult.SafetyKnowledgeExamScore = employeeExamResultRecord.SafetyKnowledgeExamScore.IsNull() ? "" : employeeExamResultRecord.SafetyKnowledgeExamScore.ToString();
        //        examResult.ManagementAbilityExamScore = employeeExamResultRecord.ManagementAbilityExamScore.IsNull() ? "" : employeeExamResultRecord.ManagementAbilityExamScore.ToString();
        //        examResult.FieldExamResult = employeeExamResultRecord.FieldExamResult.IsNull() ? "" : employeeExamResultRecord.FieldExamResult == true ? "合格" : "不合格";
        //        examResult.resultMessage.IsSuccess = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        examResult.resultMessage.IsSuccess = false;
        //        examResult.resultMessage.ErrorMessage = ex.Message;
        //    }
        //    return Json(examResult, JsonRequestBehavior.AllowGet);
        //}
        //public class ExamResult
        //{
        //    public string SafetyKnowledgeExamScore { get; set; }
        //    public string ManagementAbilityExamScore { get; set; }
        //    public string FieldExamResult { get; set; }
        //    public ResultMessage resultMessage { get; set; }
        //}
        //#endregion

        #region 保存考试照片
        public void SavePhoto(string imgBase64)
        {
            FileStream fs = null;
            MemoryStream ms = null;
            try
            {

                byte[] arrFileData = Convert.FromBase64String(imgBase64);
                ms = new MemoryStream(arrFileData);

                string rootFolder = @"D:\tmp\";
                string fileName = string.Format("{0}{1}.png"
                    , DateTime.Now.ToString("yyyyMMdd")
                    , Guid.NewGuid().ToString().Replace("-", string.Empty));//文件名

                string relativePath = string.Format(@"\{0}\{1}"
                    , DateTime.Now.ToString("yyyyMMdd")
                    , fileName);//数据库相对路径
                string absolutePath = string.Format(@"{0}\{1}"
                    , rootFolder.TrimEnd('\\')
                    , relativePath.TrimStart('\\'));//文件绝对路径
                FileInfo file = new FileInfo(absolutePath);
                if (!file.Directory.Exists)
                {
                    file.Directory.Create();
                }
                else if (file.Exists)
                {
                    file.Attributes = FileAttributes.Normal;
                    file.Delete();
                }
                fs = new FileStream(absolutePath, FileMode.CreateNew);
                ms.CopyToStream(fs);
                fs.Flush();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (null != fs) fs.Close();
                if (null != ms) ms.Close();
            }
        }

        #endregion
        [JsonException]
        public JsonResult GetCurrentDateTime()
        {
            return Json(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}
