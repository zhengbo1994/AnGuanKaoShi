using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SafetyProfessionalAssessmentSystem.Controllers
{
    [AuthorizeFilter]
    public class ExaminationRoomMaintenanceController : BaseController
    {
        //
        // GET: /ExaminationRoomMaintenance/

        public ActionResult Index()
        {
            return View();
        }
        #region 获取考场列表信息
        public JsonResult GetExaminationRoomListForJqGrid(GetExaminationRoomListForJpGridParam param)
        {
            int totalCount = 0;
            Sys_Account account = LoginAccount as Sys_Account;
            IExaminationPointCtrl traininginstitutionCtrl = new ExaminationPointCtrl(account);
            List<Biz_ExaminationRoom> examRoomList = traininginstitutionCtrl.GetExaminationRoomList(account.UserId, param.ExamRoomName,
                param.page, param.rows, ref totalCount);
            List<GetTrainingInstitutionListForJqGridJsonResult> examRoomListJsonResult = examRoomList.Select(e => new GetTrainingInstitutionListForJqGridJsonResult()
            {
                Id = e.Id,
                ExamRoomName = e.ExamRoomName,
                PersonCount = e.PersonCount,
                Enabled = e.Enabled ? "可用" : "不可用",
                CreatDate = e.CreatDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Remark = e.Remark

            }).ToList();
            JqGridData dataResult = ConvertIListToJqGridData(param.page, param.rows, totalCount, examRoomListJsonResult);
            return Json(dataResult, JsonRequestBehavior.AllowGet);
        }
        public class GetExaminationRoomListForJpGridParam
        {
            //考场名称
            public string ExamRoomName { get; set; }
            public int page { get; set; }
            public int rows { get; set; }
        }
        public class GetTrainingInstitutionListForJqGridJsonResult
        {
            public int Id { get; set; }
            //考场名称
            public string ExamRoomName { get; set; }
            //核定人数
            public int PersonCount { get; set; }
            //是否可用
            public string Enabled { get; set; }
            //创建日期
            public string CreatDate { get; set; }
            public string Remark { get; set; }

        }
        #endregion
        #region 新增考场
        public JsonResult InsertExamRoom(InsertExamRoomParam examRoomParam)
        {
            ResultMessage resultMessage = new ResultMessage();
            resultMessage.IsSuccess = true;
            try
            {
                Sys_Account account = LoginAccount as Sys_Account;
                IExaminationPointCtrl traininginstitutionCtrl = new ExaminationPointCtrl(account);
                Biz_ExaminationRoom examinationRoom = new Biz_ExaminationRoom()
                {
                    Id = examRoomParam.Id,
                    ExamRoomName = examRoomParam.ExamRoomName,
                    PersonCount = examRoomParam.PersonCount,
                    Enabled = examRoomParam.Enabled,
                    Remark = examRoomParam.Remark
                };
                traininginstitutionCtrl.InsertExaminationRoom(examinationRoom);

            }
            catch (Exception ex)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = ex.Message;
            }

            return Json(resultMessage, JsonRequestBehavior.AllowGet);
        }
        public class InsertExamRoomParam
        {
            public int Id { get; set; }
            public string ExamRoomName { get; set; }
            public int PersonCount { get; set; }
            public bool Enabled { get; set; }
            public string Remark { get; set; }
        }
        #endregion

        #region 修改考场
        public JsonResult UpdateExamRoom(UpdateExamRoomParam examRoomParam)
        {
            ResultMessage resultMessage = new ResultMessage();
            resultMessage.IsSuccess = true;
            try
            {
                Sys_Account account = LoginAccount as Sys_Account;
                IExaminationPointCtrl traininginstitutionCtrl = new ExaminationPointCtrl(account);
                Biz_ExaminationRoom examinationRoom = new Biz_ExaminationRoom()
                {
                    Id = examRoomParam.Id,
                    ExamRoomName = examRoomParam.ExamRoomName,
                    PersonCount = examRoomParam.PersonCount,
                    Enabled = examRoomParam.Enabled,
                    Remark = examRoomParam.Remark
                };
                traininginstitutionCtrl.UpdateExaminationRoom(examinationRoom);
            }
            catch (Exception ex)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = ex.Message;
            }

            return Json(resultMessage, JsonRequestBehavior.AllowGet);
        }
        public class UpdateExamRoomParam
        {
            public int Id { get; set; }
            public string ExamRoomName { get; set; }
            public int PersonCount { get; set; }
            public bool Enabled { get; set; }
            public string Remark { get; set; }

        }
        #endregion

        #region 删除考场
        public JsonResult DeleteExamRoomById(int examRoomId)
        {
            ResultMessage resultMessage = new ResultMessage();
            resultMessage.IsSuccess = true;
            try
            {
                Sys_Account account = LoginAccount as Sys_Account;
                IExaminationPointCtrl traininginstitutionCtrl = new ExaminationPointCtrl(account);
                traininginstitutionCtrl.DeleteExaminationRoomById(examRoomId);
            }
            catch (Exception ex)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = ex.Message;
            }

            return Json(resultMessage, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 根据Id获取考场信息
        public JsonResult GetExamRoomById(int examRoomId)
        {
            Sys_Account account = LoginAccount as Sys_Account;
            IExaminationPointCtrl traininginstitutionCtrl = new ExaminationPointCtrl(account);
            Biz_ExaminationRoom examinationRoom = traininginstitutionCtrl.GetExaminationRoomById(examRoomId);
            ResultExamRoomParam resultExamRoomParam = new ResultExamRoomParam()
            {
                Id = examinationRoom.Id,
                ExamRoomName = examinationRoom.ExamRoomName,
                PersonCount = examinationRoom.PersonCount,
                Enabled = examinationRoom.Enabled,
                Remark = examinationRoom.Remark
            };
            return Json(resultExamRoomParam, JsonRequestBehavior.AllowGet);
        }
        public class ResultExamRoomParam
        {
            public int Id { get; set; }
            public string ExamRoomName { get; set; }
            public int PersonCount { get; set; }
            public bool Enabled { get; set; }
            public string Remark { get; set; }
        }
        #endregion



    }
}
