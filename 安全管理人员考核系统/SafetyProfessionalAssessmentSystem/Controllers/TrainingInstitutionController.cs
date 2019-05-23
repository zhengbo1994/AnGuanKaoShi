using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
using BLL;


namespace SafetyProfessionalAssessmentSystem.Controllers
{
    [AuthorizeFilter]
    public class TrainingInstitutionController : BaseController
    {
        //
        // GET: /TrainingInstitution/
        public ActionResult Index()
        {
            return View();
        }
        #region 获取培训机构信息
        public JsonResult GetTrainInstitutionListForJqGrid(GetTrainInstitutionListForJpGridParam param)
        {
            int totalCount = 0;
            Sys_Account account = LoginAccount as Sys_Account;
            ITrainingInstitutionCtrl traininginstitutionCtrl = new TrainingInstitutionCtrl(account);
            List<string> loginCityList = base.LoginDataPermissionDetailList.Select(p => p.DetailName).ToList();
            List<Biz_TrainingInstitution> traininginstitutionList = traininginstitutionCtrl.GetTrainingInstitutionList(param.InstitutionName,
                param.SocialCreditCode, param.City, param.Area, param.page, param.rows, ref totalCount, loginCityList);
            List<GetTrainingInstitutionListForJqGridJsonResult> traininginstitutionListJsonResult = traininginstitutionList.Select(e => new GetTrainingInstitutionListForJqGridJsonResult()
            {
                Id = e.Id,
                InstitutionName = e.InstitutionName,
                SocialCreditCode = e.SocialCreditCode,
                LegalRepresentative = e.LegalRepresentative,
                LegalRepresentativeNumber = e.LegalRepresentativeNumber,
                ContactPerson = e.ContactPerson,
                ContactNumber = e.ContactNumber,
                City = e.City,
                Area = e.Area,
                Address = e.Address
            }).ToList();
            JqGridData dataResult = ConvertIListToJqGridData(param.page, param.rows, totalCount, traininginstitutionListJsonResult);
            return Json(dataResult, JsonRequestBehavior.AllowGet);
        }
        public class GetTrainInstitutionListForJpGridParam
        {

            //考核点名称
            public string InstitutionName { get; set; }
            //社会信用代码
            public string SocialCreditCode { get; set; }
            //考核城市
            public string City { get; set; }
            //考核区域
            public string Area { get; set; }
            public int page { get; set; }
            public int rows { get; set; }

        }
        public class GetTrainingInstitutionListForJqGridJsonResult
        {
            public int Id { get; set; }
            //考核点名称
            public string InstitutionName { get; set; }
            //社会信用代码
            public string SocialCreditCode { get; set; }
            //法定代表人
            public string LegalRepresentative { get; set; }
            //法定代表人电话
            public string LegalRepresentativeNumber { get; set; }
            //联系人
            public string ContactPerson { get; set; }
            //联系人电话
            public string ContactNumber { get; set; }
            //考核城市
            public string City { get; set; }
            //考核区域
            public string Area { get; set; }
            //地址
            public string Address { get; set; }

        }
        #endregion

        #region 获取考场信息

        //public JsonResult GetExaminationRoomListForJqGrid(GetExamRoomParam param)
        //{
        //    int totalCount = 0;
        //    List<ExamRoomResult> examRoomListJsonResult = new List<ExamRoomResult>();
        //    try
        //    {
        //        Sys_Account account = LoginAccount as Sys_Account;
        //        IExaminationPointCtrl traininginstitutionCtrl = new ExaminationPointCtrl(account);
        //        List<Biz_ExaminationRoom> examRoomList = traininginstitutionCtrl.GetExaminationRoomListByExaminationPointId(param.TrainInstitutionId, true);
        //        examRoomListJsonResult = examRoomList.Select(e => new ExamRoomResult()
        //       {
        //           Id = e.Id,
        //           ExamRoomName = e.ExamRoomName,
        //           PersonCount = e.PersonCount,
        //           Remark = e.Remark

        //       }).ToList();
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    JqGridData dataResult = ConvertIListToJqGridData(param.page, param.rows, totalCount, examRoomListJsonResult);
        //    return Json(dataResult, JsonRequestBehavior.AllowGet);
        //}
        //public class GetExamRoomParam
        //{
        //    public int TrainInstitutionId { get; set; }
        //    public int rows { get; set; }
        //    public int page { get; set; }
        //}
        //public class ExamRoomResult
        //{
        //    public int Id { get; set; }
        //    //考场名称
        //    public string ExamRoomName { get; set; }
        //    //核定人数
        //    public int PersonCount { get; set; }
        //    public string Remark { get; set; }

        //}

        #endregion

        #region 新增考核点
        public JsonResult InsertTrainingInstitution(InsertTrainingInstitutionParam param)
        {
            ResultMessage resultMessage = new ResultMessage();
            try
            {
                Sys_Account account = LoginAccount as Sys_Account;
                ITrainingInstitutionCtrl traininginstitutionCtrl = new TrainingInstitutionCtrl(account);
                Biz_TrainingInstitution traininginstitution = new Biz_TrainingInstitution()
                {
                    Id = param.Id,
                    InstitutionName = param.InstitutionName,
                    SocialCreditCode = param.SocialCreditCode,
                    LegalRepresentative = param.LegalRepresentative,
                    LegalRepresentativeNumber = param.LegalRepresentativeNumber,
                    ContactPerson = param.ContactPerson,
                    ContactNumber = param.ContactNumber,
                    City = param.City,
                    Area = param.Area,
                    Email = param.Email,
                    Address = param.Address

                };
                traininginstitutionCtrl.InsertTrainingInstitution(traininginstitution);
                resultMessage.IsSuccess = true;
            }
            catch (Exception e)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = e.Message;
            };
            return Json(resultMessage, JsonRequestBehavior.AllowGet);
        }
        public class InsertTrainingInstitutionParam
        {
            public int Id { get; set; }
            //考核点名称
            public string InstitutionName { get; set; }
            //社会信用代码
            public string SocialCreditCode { get; set; }
            //法定代表人
            public string LegalRepresentative { get; set; }
            //法定代表人电话
            public string LegalRepresentativeNumber { get; set; }
            //联系人
            public string ContactPerson { get; set; }
            //联系人电话
            public string ContactNumber { get; set; }
            //考核城市
            public string City { get; set; }
            //考核区域
            public string Area { get; set; }
            //邮箱
            public string Email { get; set; }
            //地址
            public string Address { get; set; }
        }
        #endregion

        #region 根据Id获取考核点信息
        public JsonResult GetTrainingInstitutionById(int trainingInstitutionId)
        {
            Sys_Account account = LoginAccount as Sys_Account;
            ITrainingInstitutionCtrl trainingInstitutionCtrl = new TrainingInstitutionCtrl(account);
            Biz_TrainingInstitution trainingInstitution = trainingInstitutionCtrl.GetTrainingInstitutionById(trainingInstitutionId);
            TrainingInstitutionResult trainingInstitutionResult = new TrainingInstitutionResult()
            {
                Id = trainingInstitution.Id,
                InstitutionName = trainingInstitution.InstitutionName,
                SocialCreditCode = trainingInstitution.SocialCreditCode,
                City = trainingInstitution.City,
                Area = trainingInstitution.Area,
                LegalRepresentative = trainingInstitution.LegalRepresentative,
                LegalRepresentativeNumber = trainingInstitution.LegalRepresentativeNumber,
                ContactPerson = trainingInstitution.ContactPerson,
                ContactNumber = trainingInstitution.ContactNumber,
                Email = trainingInstitution.Email,
                Address = trainingInstitution.Address
            };
            return Json(trainingInstitutionResult, JsonRequestBehavior.AllowGet);

        }
        public class TrainingInstitutionResult
        {
            public int Id { get; set; }
            //企业名称
            public string InstitutionName { get; set; }
            //社会信用代码
            public string SocialCreditCode { get; set; }
            //城市
            public string City { get; set; }
            //区域
            public string Area { get; set; }
            //法定代表人
            public string LegalRepresentative { get; set; }
            //法定代表人电话
            public string LegalRepresentativeNumber { get; set; }
            //联系人
            public string ContactPerson { get; set; }
            //联系人电话
            public string ContactNumber { get; set; }
            public string Email { get; set; }
            public string Address { get; set; }
        }
        #endregion

        #region 修改考核点的信息
        public JsonResult UpdateTrainingInstitution(UpdateTrainingInstitutionParam param)
        {
            ResultMessage resultMessage = new ResultMessage();
            try
            {
                Sys_Account account = LoginAccount as Sys_Account;
                ITrainingInstitutionCtrl traininginstitutionCtrl = new TrainingInstitutionCtrl(account);
                Biz_TrainingInstitution traininginstitution = new Biz_TrainingInstitution()
                {
                    Id = param.Id,
                    InstitutionName = param.InstitutionName,
                    SocialCreditCode = param.SocialCreditCode,
                    LegalRepresentative = param.LegalRepresentative,
                    LegalRepresentativeNumber = param.LegalRepresentativeNumber,
                    ContactPerson = param.ContactPerson,
                    ContactNumber = param.ContactNumber,
                    City = param.City,
                    Area = param.Area,
                    Email = param.Email,
                    Address = param.Address
                };
                traininginstitutionCtrl.UpdateTrainingInstitution(traininginstitution);
                resultMessage.IsSuccess = true;
            }
            catch (Exception e)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = e.Message;
            };
            return Json(resultMessage, JsonRequestBehavior.AllowGet);
        }

        public class UpdateTrainingInstitutionParam
        {

            public int Id { get; set; }
            //考核点名称
            public string InstitutionName { get; set; }
            //社会信用代码
            public string SocialCreditCode { get; set; }
            //法定代表人
            public string LegalRepresentative { get; set; }
            //法定代表人电话
            public string LegalRepresentativeNumber { get; set; }
            //联系人
            public string ContactPerson { get; set; }
            //联系人电话
            public string ContactNumber { get; set; }
            //考核城市
            public string City { get; set; }
            //考核区域
            public string Area { get; set; }
            //邮箱
            public string Email { get; set; }
            //地址
            public string Address { get; set; }
        }
        #endregion

        #region 删除考核点信息
        public JsonResult DeleteTrainingInstitutionById(int trainingInstitutionId)
        {
            ResultMessage resultMessage = new ResultMessage();
            try
            {
                Sys_Account account = LoginAccount as Sys_Account;
                ITrainingInstitutionCtrl traininginstitutionCtrl = new TrainingInstitutionCtrl(account);
                traininginstitutionCtrl.DeleteTrainingInstitutionById(trainingInstitutionId);
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
    }
}
