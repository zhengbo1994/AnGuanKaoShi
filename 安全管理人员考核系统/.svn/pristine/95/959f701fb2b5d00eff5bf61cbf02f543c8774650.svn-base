using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SafetyProfessionalAssessmentSystem.Controllers
{
    public class CamerViewController : BaseController
    {
        //
        // GET: /CamerView/
        [AuthorizeFilter]
        public ActionResult Index()
        {
            return View();
        }
        #region 获取考场NVR数据
        public JsonResult GetExamRoomNVRInfo(int examRoomId)
        {
            GetExamRoomNVRInfoResult result = new GetExamRoomNVRInfoResult() { resultMessage = new ResultMessage() };
            try
            {
                IExaminationPointCtrl trainingInstitutionCtrl = new ExaminationPointCtrl(null);
                Biz_ExaminationRoom examRoom = trainingInstitutionCtrl.GetExaminationRoomById(examRoomId);
                Biz_ExaminationPoint trainingInstitution = trainingInstitutionCtrl.GetExaminationPointById(examRoom.ExaminationPointId);
                Biz_ExamRoomNVR examRoomNVR = trainingInstitutionCtrl.GetExamRoomNVRByExamRoomId(examRoomId);
                if (examRoomNVR == null)
                {
                    throw new Exception("没有配置NVR");
                }
                result.TrainingInstitutionName = trainingInstitution.InstitutionName;
                result.ExamRoomName = examRoom.ExamRoomName;
                result.IPAddress = examRoomNVR.IPAddress;
                result.Protocol = examRoomNVR.Protocol;
                result.Port = examRoomNVR.Port;
                result.Username = examRoomNVR.Username;
                result.Password = examRoomNVR.Password;
            }
            catch (Exception ex)
            {
                result.resultMessage.IsSuccess = false;
                result.resultMessage.ErrorMessage = ex.Message;

            }
            return Json(result);
        }
        public class GetExamRoomNVRInfoResult
        {
            public ResultMessage resultMessage { get; set; }
            public string TrainingInstitutionName { get; set; }
            public string ExamRoomName { get; set; }
            public string IPAddress { get; set; }
            public int Protocol { get; set; }
            public int Port { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
        }
        #endregion
    }
}
