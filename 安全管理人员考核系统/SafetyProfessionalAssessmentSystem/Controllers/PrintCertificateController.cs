﻿using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Library.baseFn;
using CLSLibrary;
using System.Drawing;
using System.IO;

namespace SafetyProfessionalAssessmentSystem.Controllers
{
    [AuthorizeFilter]
    public class PrintCertificateController : BaseController
    {
        //
        // GET: /打印证书/

        public ActionResult Index()
        {
            return View();
        }

        #region   获取证书列表
        public class GetEmployeeListForJqgridParam
        {
            public string EmployeeName { get; set; }
            public string IDNumber { get; set; }
            public string EnterpriseName { get; set; }
            public string ExamPlanNumber { get; set; }
            public string TraningInstutionName { get; set; }
            public string ExamType { get; set; }
            public bool? IsPrinted { get; set; }
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
            ICertificateCtrl certificateCtrl = new CertificateCtrl(account);
            IEnterpriseCtrl enterpriseCtrl = new EnterpriseCtrl(account);
            IExaminationPointCtrl examinationPointCtrl = new ExaminationPointCtrl(account);

            List<Biz_Certificate> certificateList = workFlowCtrl.GetEmployeeListInPrintCertificate(param.EmployeeName, param.IDNumber, param.ExamType, null, param.ExamPlanNumber, param.EnterpriseName, param.TraningInstutionName, param.IsPrinted, param.page, param.rows, ref totalCount);
            List<int> certificateIdList = certificateList.Select(p => p.Id).ToList();
            List<Biz_RelEmployeeCertificate> relEmployeeCertificateList = certificateCtrl.GetRelEmployeeCertificateListByCertificateIdList(certificateIdList);
            List<int> employeeIdList = relEmployeeCertificateList.Select(p => p.EmployeeId).ToList();
            List<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordList = workFlowCtrl.GetEmployeeForExamPlanRecordByEmployeeIdList(employeeIdList);
            List<Biz_ExamPlanRecord> examPlanRecordList = workFlowCtrl.GetExamPlanRecordByIdList(employeeForExamPlanRecordList.Select(p => p.ExamPlanRecordId).ToList());
            List<Biz_ExaminationRoom> examRoomList = examinationPointCtrl.GetExaminationRoomByIdList(employeeForExamPlanRecordList.Select(p => p.ExamRoomId).ToList());
            List<Biz_ExaminationPoint> trainingInstitutionList = examinationPointCtrl.GetExaminationPointByIdList(examRoomList.Select(p => p.ExaminationPointId).ToList());
            List<Biz_CertificatePrintRecord> certificatePrintRecordList = workFlowCtrl.GetCertificatePrintRecordList(certificateList.Select(p => p.CertificateNo).ToList());
            List<GetEmployeeListForJqgridResult> CertificateInfoList = certificateList.Join(relEmployeeCertificateList, a => a.Id, a1 => a1.CertificateId, (a, a1) => new { a,a1})
                                                       .Join(employeeForExamPlanRecordList, o => o.a1.EmployeeId, b => b.EmployeeId, (o, b) => new { o.a, b })
                                                       .Join(examPlanRecordList, o => o.b.ExamPlanRecordId, c => c.Id, (o, c) => new { o.a, o.b, c })
                                                       .Join(examRoomList, o => o.b.ExamRoomId, d => d.Id, (o, d) => new { o.a, o.b, o.c, d })
                                                       .Join(trainingInstitutionList, o => o.d.ExaminationPointId, e => e.Id, (o, e) => new { o.a, o.b, o.c, o.d, e })
                                                       .GroupJoin(certificatePrintRecordList, o => o.a.CertificateNo, f => f.CertificateNo, (o, f) => new { o.a, o.b, o.c, o.d, o.e, f })
                                                       .Select(o => new GetEmployeeListForJqgridResult()
                                                       {
                                                           EmployeeId = o.b.EmployeeId,
                                                           EmployeeName = o.a.EmployeeName,
                                                           Sex = o.a.Sex,
                                                           Birthday = o.a.Birthday,
                                                           IDNumber = o.a.IDNumber,
                                                           EnterpriseName = o.a.EnterpriseName,
                                                           Job = o.a.Job,
                                                           Title4Technical = o.a.Title4Technical,
                                                           ExamPlanNumber = o.c.ExamPlanNumber,
                                                           TrainingInstutionName = o.e.InstitutionName,
                                                           Industry = o.a.Industry,
                                                           ExamType = o.a.ExamType,
                                                           CertificateNo = o.a.CertificateNo,
                                                           StartCertificateDate = o.a.StartCertificateDate.ToString("yyyy-MM-dd"),
                                                           EndCertificateDate = o.a.EndCertificateDate.ToString("yyyy-MM-dd"),
                                                           PrintStatus = o.f.Count() > 0 ? "已打印" : "未打印"
                                                       }).ToList();

            JqGridData dataResult = ConvertIListToJqGridData(param.page, param.rows, totalCount, CertificateInfoList);
            return Json(dataResult, JsonRequestBehavior.AllowGet);
        }
        public class GetEmployeeListForJqgridResult
        {
            public int EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public string Sex { get; set; }
            public string Birthday { get; set; }
            public string IDNumber { get; set; }
            public string EnterpriseName { get; set; }
            public string Job { get; set; }
            public string Title4Technical { get; set; }
            public string ExamPlanNumber { get; set; }
            public string TrainingInstutionName { get; set; }
            public string Industry { get; set; }
            public string ExamType { get; set; }
            public string CertificateNo { get; set; }
            public string StartCertificateDate { get; set; }
            public string EndCertificateDate { get; set; }
            public string PrintStatus { get; set; }

        }
        #endregion

        #region  获取证书信息
        public JsonResult GetCertificateInfo(int employeeId)
        {
            CertificateInfo certificate = new CertificateInfo()
            {
                resultMessage = new ResultMessage() { IsSuccess = true }
            };
            try
            {
                certificate.resultMessage.IsSuccess = true;
                Sys_Account account = base.LoginAccount as Sys_Account;
                IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
                Biz_Certificate cert = workFlowCtrl.GetCertificateByEmployeeId(employeeId);
                certificate.EmployeeName = cert.EmployeeName;
                certificate.Sex = cert.Sex;
                certificate.Birthday = cert.Birthday;
                certificate.IDNumber = cert.IDNumber;
                certificate.EnterpriseName = cert.EnterpriseName;
                certificate.Job = cert.Job;
                certificate.Title4Technical = cert.Title4Technical;
                certificate.CertificateNo = cert.CertificateNo;
                certificate.StartCertificateDate_Year = cert.StartCertificateDate.Year.ToString();
                certificate.StartCertificateDate_Month = cert.StartCertificateDate.Month.ToString();
                certificate.StartCertificateDate_Day = cert.StartCertificateDate.Day.ToString();

            }
            catch (Exception ex)
            {
                certificate.resultMessage.IsSuccess = false;
                certificate.resultMessage.ErrorMessage = ex.Message;
            }
            return Json(certificate);
        }
        public class CertificateInfo
        {
            public ResultMessage resultMessage { get; set; }
            public string EmployeeName { get; set; }
            public string Sex { get; set; }
            public string Birthday { get; set; }
            public string IDNumber { get; set; }
            public string EnterpriseName { get; set; }
            public string Job { get; set; }
            public string Title4Technical { get; set; }
            public string CertificateNo { get; set; }
            public string StartCertificateDate_Year { get; set; }
            public string StartCertificateDate_Month { get; set; }
            public string StartCertificateDate_Day { get; set; }
        }
        #endregion

        #region 登记照片
        public string GetPhoto(int employeeId)
        {
            string resultFilePath = "";
            string fileRootDirectory = base.getFileRootDirectory();
            try
            {
                Sys_Account account = base.LoginAccount as Sys_Account;
                IEmployeeCtrl employeeCtrl = new EmployeeCtrl(account);
                Biz_EmployeeFile employeeFile = employeeCtrl.GetEmployeeFile(employeeId, "进场照片");
                if (employeeFile.IsNull())
                {
                    throw new Exception("未找到登记照信息");
                }
                resultFilePath = fileRootDirectory + employeeFile.FilePath;
                if (!System.IO.File.Exists(resultFilePath))
                {
                    throw new Exception("照片文件不存在");
                }
            }
            catch (Exception ex)
            {
                resultFilePath = fileRootDirectory + "/error.jpg";
            }
            string strbase64 = AppFn.ImgToBase64String(resultFilePath);
            return strbase64;
        }
        #endregion



        #region 证书二维码
        public string GetCertificateQRCode(string certificateNo)
        {
            string resultFilePath = "";
            string fileRootDirectory = base.getFileRootDirectory();
            try
            {
                QRCodeHelper qrcoder = new QRCodeHelper();

                string FileFolder = fileRootDirectory + "\\证书二维码(可删除)";
                //二维码内容
                string localtionUrl = System.Configuration.ConfigurationManager.AppSettings["LocaltionUrl"];
                string certificateVerifyUrl = System.Configuration.ConfigurationManager.AppSettings["CertificateVerifyUrl"];
                certificateNo = HttpUtility.UrlEncode(certificateNo);//encodeURIComponent编码 解码用decodeURIComponent
                string qrcodeStr = localtionUrl + string.Format(certificateVerifyUrl, certificateNo);
                resultFilePath = qrcoder.GetQRCODEByString(qrcodeStr, FileFolder, 128);
            }
            catch (Exception ex)
            {
                resultFilePath = fileRootDirectory + "/error.jpg";
            }
            string strbase64 = AppFn.ImgToBase64String(resultFilePath);
            return strbase64;
        }
        #endregion

        #region 保存打印记录
        public JsonResult SavePrintCertificateRecord(string certificateNo)
        {
            ResultMessage resultMessage = new ResultMessage();
            try
            {
                Sys_Account account = base.LoginAccount as Sys_Account;
                IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
                Biz_CertificatePrintRecord pintRecord = new Biz_CertificatePrintRecord()
                {
                    CertificateNo = certificateNo,
                    PrintType = "新证打印"
                };
                workFlowCtrl.SaveCertificatePrintRecord(pintRecord);
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
