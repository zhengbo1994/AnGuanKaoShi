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
    public class CertificateInfoChangeCheckController : BaseController
    {
        //
        // GET: /证书信息变更审核/

        public ActionResult Index()
        {
            return View();
        }

        #region 获取证书变更记录
        public class GetCertificateInfoChangeCheckListForJqGridParam
        {
            //证书编号
            public string certificateNo { get; set; }
            //证书状态
            public string checkStatus { get; set; }
            //持证人姓名
            public string employeeName { get; set; }
            //企业名称
            public string enterpriseName { get; set; }
            public int page { get; set; }
            public int rows { get; set; }
        }
        public JsonResult GetCertificateInfoChangeCheckListForJqGrid(GetCertificateInfoChangeCheckListForJqGridParam param)
        {
            Sys_Account account = LoginAccount as Sys_Account;
            ICertificateManagementCtrl certificateManagementCtrl = new CertificateManagementCtrl(account);
            int totalCount = 0;
            List<string> cityList= base.LoginDataPermissionDetailList.Select(p=>p.DetailName).ToList();
            List<GetCertificateInfoChangeCheckListForJqGridResult> resultList = certificateManagementCtrl.GetCertificateChangCheckList(param.employeeName, param.certificateNo, param.checkStatus, param.enterpriseName, param.page, param.rows, ref totalCount, cityList)
                .Select(p => new GetCertificateInfoChangeCheckListForJqGridResult()
                {
                    certificateChangeId = p.Id,
                    employeeName = p.EmployeeName,
                    sex = p.Sex,
                    birthday = p.Birthday,
                    iDNumber = p.IDNumber,
                    oldEnterpriseName = p.OldEnterpriseName,
                    job = p.Job,
                    title4Technical = p.Title4Technical,
                    certificateNo = p.CertificateNo,
                    examType = p.ExamType,
                    checkStatus = p.PassFlag == true ? "审核通过" : p.PassFlag == false ? "审核不通过" : "未审核",
                    submitDate = p.SubmitDate.IsNull() ? "" : Convert.ToDateTime(p.SubmitDate).ToString("yyyy-MM-dd"),
                    checkDate = p.CheckDate.IsNull() ? "" : Convert.ToDateTime(p.CheckDate).ToString("yyyy-MM-dd"),
                    remark=p.Remark
                }).ToList();
            JqGridData dataResult = ConvertIListToJqGridData(param.page, param.rows, totalCount, resultList);
            return Json(dataResult);
        }
        public class GetCertificateInfoChangeCheckListForJqGridResult
        {
            public int certificateChangeId { get; set; }
            //持证人姓名
            public string employeeName { get; set; }
            //性别
            public string sex { get; set; }
            public string birthday { get; set; }
            //身份证号
            public string iDNumber { get; set; }
            //企业名称
            public string oldEnterpriseName { get; set; }
            public string job { get; set; }
            public string title4Technical { get; set; }
            //证书编号
            public string certificateNo { get; set; }
            //证书类别
            public string examType { get; set; }
            //证书变更申请状态
            public string checkStatus { get; set; }
            public string submitDate { get; set; }
            public string checkDate { get; set; }
            public string  remark { get; set; }

        }
        #endregion


        #region 根据Id 获取变更记录信息
        public JsonResult GetCertificateChangeInfo(int certificateChangeId)
        {
            GetCertificateChangeInfoResult Result = new GetCertificateChangeInfoResult()
            {
                resultMessage = new ResultMessage()
            };
            try
            {
                Sys_Account account = base.LoginAccount as Sys_Account;
                ICertificateManagementCtrl certificateManagementCtrl = new CertificateManagementCtrl(account);
                IWorkFlowCtrl workFlowCtrl= new WorkFlowCtrl(account);
                Biz_CertificateChangeRecord certificateChangeRecord = certificateManagementCtrl.GetCertificateChangeInfoById(certificateChangeId);
                if (certificateChangeRecord == null)
                {
                    throw new Exception("未找变更记录");
                }
                GetCertificateChangeInfoResultData resultData = new GetCertificateChangeInfoResultData()
                {
                    id = certificateChangeRecord.Id,
                    certificateNo = certificateChangeRecord.CertificateNo,
                    employeeName = certificateChangeRecord.EmployeeName,
                    sex = certificateChangeRecord.Sex,
                    birthday = certificateChangeRecord.Birthday,
                    iDNumber = certificateChangeRecord.IDNumber,
                    oldEnterpriseName = certificateChangeRecord.EnterpriseName,
                    job = certificateChangeRecord.Job,
                    title4Technical = certificateChangeRecord.Title4Technical,
                    endCertificateDate = certificateChangeRecord.EndCertificateDate.ToString("yyyy-MM-dd"),
                    enterpriseName = workFlowCtrl.GetUserName(certificateChangeRecord.CreateById),
                    remark = certificateChangeRecord.Remark
                };
                Result.resultMessage.IsSuccess = true;
                Result.data = resultData;
            }
            catch (Exception ex)
            {
                Result.resultMessage.IsSuccess = false;
                Result.resultMessage.ErrorMessage = ex.Message;
            }
            return Json(Result);

        }
        public class GetCertificateChangeInfoResultData
        {
            public int id { get; set; }
            public string employeeName { get; set; }
            public string certificateNo { get; set; }
            public string sex { get; set; }
            public string birthday { get; set; }
            public string iDNumber { get; set; }
            public string oldEnterpriseName { get; set; }
            public string enterpriseName { get; set; }
            public string job { get; set; }
            public string title4Technical { get; set; }
            public string endCertificateDate { get; set; }
            public string remark { get; set; }
        }
        public class GetCertificateChangeInfoResult
        {
            public ResultMessage resultMessage { get; set; }
            public GetCertificateChangeInfoResultData data { get; set; }
        }
        #endregion


        #region 审核变更记录
        public class CheckCertificateChangeInfoParam
        {
            public int id { get; set; }
            public bool passFlag { get; set; }
            public string remark { get; set; }
        }
        public JsonResult CheckCertificateChangeInfo(CheckCertificateChangeInfoParam param)
        {
            ResultMessage resultMessage = new ResultMessage();
            try
            {
                Sys_Account account = base.LoginAccount as Sys_Account;
                ICertificateManagementCtrl certificateManagementCtrl = new CertificateManagementCtrl(account);
                certificateManagementCtrl.CheckedCertificateChangeRecord(param.id, param.remark, param.passFlag);
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
