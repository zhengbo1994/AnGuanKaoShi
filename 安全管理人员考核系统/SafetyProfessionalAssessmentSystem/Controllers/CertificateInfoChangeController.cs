using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL;
using Model;
using Library.baseFn;
using System.Globalization;

namespace SafetyProfessionalAssessmentSystem.Controllers
{
    [AuthorizeFilter]
    public class CertificateInfoChangeController : BaseController
    {
        //
        // GET: /证书信息变更/

        public ActionResult Index()
        {
            return View();
        }
        #region 获取证书变更记录
        public class GetCertificateInfoChangeListForJqGridParam
        {
            //证书编号
            public string certificateNo { get; set; }
            //证书状态
            public string certificateChangeStatus { get; set; }
            //持证人姓名
            public string employeeName { get; set; }
            //企业名称
            public string enterpriseName { get; set; }
            public int page { get; set; }
            public int rows { get; set; }
        }
        public JsonResult GetCertificateInfoChangeListForJqGrid(GetCertificateInfoChangeListForJqGridParam param)
        {
            Sys_Account account = LoginAccount as Sys_Account;
            ICertificateCtrl certificateManagementCtrl = new CertificateCtrl(account);
            int totalCount = 0;
            List<GetCertificateInfoChangeListForJqGridResult> resultList = certificateManagementCtrl.GetCertificateChangList(param.employeeName, param.certificateNo, param.certificateChangeStatus, param.enterpriseName, param.page, param.rows, ref totalCount)
                .Select(p => new GetCertificateInfoChangeListForJqGridResult()
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
                    checkStatus = p.PassFlag == true ? "审核通过" : p.PassFlag == false ? "审核不通过" : p.SubmitStatus == true ? "已提交" : "未提交",
                    submitDate = p.SubmitDate.IsNull() ? "" : Convert.ToDateTime(p.SubmitDate).ToString("yyyy-MM-dd"),
                    remark=p.Remark
                }).ToList();
            JqGridData dataResult = ConvertIListToJqGridData(param.page, param.rows, totalCount, resultList);
            return Json(dataResult);
        }
        public class GetCertificateInfoChangeListForJqGridResult
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

            public string remark { get; set; }
        }
        #endregion

        #region 根据证书编号获取 证书信息
        public JsonResult GetCertificateInfo(string certificateNo)
        {
            GetCertificateInfoResult Result = new GetCertificateInfoResult()
            {
                resultMessage = new ResultMessage()
            };
            try
            {
                Sys_Account account = base.LoginAccount as Sys_Account;
                ICertificateCtrl certificateManagementCtrl = new CertificateCtrl(account);
                Biz_Certificate certificate = certificateManagementCtrl.GetCertificateByNo(certificateNo);
                if (certificate == null)
                {
                    throw new Exception("未找到对应的证书信息");
                }
                GetCertificateInfoResultData resultData = new GetCertificateInfoResultData()
                {
                    employeeName = certificate.EmployeeName,
                    Sex = certificate.Sex,
                    birthday = certificate.Birthday,
                    iDNumber = certificate.IDNumber,
                    oldEnterpriseName = certificate.EnterpriseName,
                    job = certificate.Job,
                    title4Technical = certificate.Title4Technical,
                    endCertificateDate = certificate.EndCertificateDate.ToString("yyyy-MM-dd")
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
        public class GetCertificateInfoResultData
        {
            public string employeeName { get; set; }
            public string Sex { get; set; }
            public string birthday { get; set; }
            public string iDNumber { get; set; }
            public string oldEnterpriseName { get; set; }
            public string job { get; set; }
            public string title4Technical { get; set; }
            public string certificateStatus { get; set; }
            public string endCertificateDate { get; set; }
        }
        public class GetCertificateInfoResult
        {
            public ResultMessage resultMessage { get; set; }
            public GetCertificateInfoResultData data { get; set; }
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
                ICertificateCtrl certificateManagementCtrl = new CertificateCtrl(account);
                Biz_CertificateChangeRecord certificateChangeRecord = certificateManagementCtrl.GetCertificateChangeInfoById(certificateChangeId);
                if (certificateChangeRecord == null)
                {
                    throw new Exception("未找变更记录");
                }
                GetCertificateChangeInfoResultData resultData = new GetCertificateChangeInfoResultData()
                {
                    id = certificateChangeRecord.Id,
                    certificateNo=certificateChangeRecord.CertificateNo,
                    employeeName = certificateChangeRecord.EmployeeName,
                    sex = certificateChangeRecord.Sex,
                    birthday = certificateChangeRecord.Birthday,
                    iDNumber = certificateChangeRecord.IDNumber,
                    oldEnterpriseName = certificateChangeRecord.EnterpriseName,
                    job = certificateChangeRecord.Job,
                    title4Technical = certificateChangeRecord.Title4Technical,
                    endCertificateDate = certificateChangeRecord.EndCertificateDate.ToString("yyyy-MM-dd")
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
            public string  certificateNo { get; set; }
            public string sex { get; set; }
            public string birthday { get; set; }
            public string iDNumber { get; set; }
            public string oldEnterpriseName { get; set; }
            public string job { get; set; }
            public string title4Technical { get; set; }
            public string endCertificateDate { get; set; }
        }
        public class GetCertificateChangeInfoResult
        {
            public ResultMessage resultMessage { get; set; }
            public GetCertificateChangeInfoResultData data { get; set; }
        }
        #endregion

        #region 保存变更记录
        public class SaveCertificateChangeInfoParam
        {
            public int id { get; set; }
            public string certificateNo { get; set; }
            public string employeeName { get; set; }
            public string sex { get; set; }
            public string birthday { get; set; }
            public string iDNumber { get; set; }
            public string oldEnterpriseName { get; set; }
            public string enterpriseName { get; set; }
            public string job { get; set; }
            public string title4Technical { get; set; }
            public string endCertificateDate { get; set; }
        }
        public JsonResult SaveCertificateChangeInfo(SaveCertificateChangeInfoParam param)
        {
            ResultMessage resultMessage = new ResultMessage();
            try
            {
                Sys_Account account = base.LoginAccount as Sys_Account;
                ICertificateCtrl certificateManagementCtrl = new CertificateCtrl(account);
                Biz_CertificateChangeRecord certificateChangeRecord = new Biz_CertificateChangeRecord()
                {
                    Id = param.id,
                    CertificateNo=param.certificateNo,
                    EmployeeName = param.employeeName,
                    Sex = param.sex,
                    Birthday = param.birthday,
                    IDNumber = param.iDNumber,
                    OldEnterpriseName = param.oldEnterpriseName,
                    EnterpriseName = param.enterpriseName,
                    Job = param.job,
                    Title4Technical = param.title4Technical,
                    EndCertificateDate = DateTime.ParseExact(param.endCertificateDate, "yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.None)
                };
                if (param.id == 0)//新增
                {
                    certificateManagementCtrl.AddCertificateChangeRecord(certificateChangeRecord);
                }
                else
                {
                    certificateManagementCtrl.UpdateCertificateChangeRecord(certificateChangeRecord);
                }
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

        #region 提交变更记录
        public JsonResult SubmitCertificateChangeInfo(int certificateChangeId)
        {
            ResultMessage resultMessage = new ResultMessage();
            try
            {
                Sys_Account account = base.LoginAccount as Sys_Account;
                ICertificateCtrl certificateManagementCtrl = new CertificateCtrl(account);
                certificateManagementCtrl.SubmitCertificateChangeRecord(certificateChangeId);
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

        #region 获取企业名称
        public JsonResult GetEnterpriseName()
        {
            return Json(base.LoginUserName);
        }
        #endregion
    }
}
