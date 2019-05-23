using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL;
using Model;
using Library.baseFn;

namespace SafetyProfessionalAssessmentTrainSystem.Controllers
{
    public class DelayReCordController : BaseController
    {
        //
        // GET: /DelayReCord/

        public ActionResult Index()
        {
            return View();
        }

        #region 根据查询条件获取jqgrid人员信息
        public class GetEmployeeListForJqgridParam
        {
            public string employeeName { get; set; }
            public string iDNumber { get; set; }
            public string certificateNo { get; set; }
            public string enterpriseName { get; set; }
            public string workFlowStatus { get; set; }
            public string trainingInstitutionName { get; set; }
            public int page { get; set; }
            public int rows { get; set; }
            public string sidx { get; set; }//排序字段名称
            public string sord { get; set; }//排序方式asc desc

        }

        public JsonResult GetCertificateListForJqgrid(GetEmployeeListForJqgridParam param)
        {
            const int DATAPERMISSIONHEAD_CITY = 1;

            try
            {
                int totalCount = 0;
                Sys_Account account = LoginAccount as Sys_Account;
                ICertificateDelay_XX_WorkFlowCtrl certificateDelayWorkFlowCtrl = new CertificateDelay_XX_WorkFlowCtrl(account);
                ICertificateCtrl certificateCtrl = new CertificateCtrl(account);

                ITrainingInstitutionCtrl trainingInstitutionCtrl = new TrainingInstitutionCtrl(account);

                List<string> cityList = LoginDataPermissionDetailList.Where(p => p.HeadId == DATAPERMISSIONHEAD_CITY).Select(p => p.DetailName).ToList();

                List<Biz_XX_CertificateDelayApplyRecord> certificateDelayApplyRecordList = certificateDelayWorkFlowCtrl.GetAllCertificateDelayRecordList(param.employeeName, param.iDNumber, param.certificateNo, param.enterpriseName, cityList, param.page, param.rows, ref totalCount);

                List<int> certificateIdList = certificateDelayApplyRecordList.Select(p => p.CertificateId).ToList();

                List<Biz_Certificate> certificateList = certificateCtrl.GetCertificateByIdList(certificateIdList);

                List<CertificateDelay_XX_WorkFlowCtrl.WorkFlowStatus> workFlowStatusList = certificateDelayWorkFlowCtrl.GetCurrentWorkFlowStatusByCertificateIdList(certificateIdList);

                List<int> trainingInstitutionIdList = certificateDelayApplyRecordList.Select(p => p.TrainingInstitutionId).ToList();
                List<Biz_TrainingInstitution> trainingInstitutionList = trainingInstitutionCtrl.GetTrainingInstitutionByIdList(trainingInstitutionIdList);

                List<GetEmployeeListForJqgridJsonResult> employeeListJsonResult = certificateDelayApplyRecordList
                    .Join(certificateList, b => b.CertificateId, a => a.Id, (b, a) => new { a, b })
                    .Join(workFlowStatusList, o => o.a.Id, c => c.certificateId, (o, c) => new { o.a, o.b, c })
                    .Join(trainingInstitutionList, o => (o.b.IsNull() ? 0 : o.b.TrainingInstitutionId), d => d.Id, (o, d) => new { o.a, o.b, o.c, d })
                    .Select(o => new GetEmployeeListForJqgridJsonResult()
                    {
                        employeeId = o.b.IsNull() ? 0 : o.b.Id,
                        certificateId = o.a.Id,
                        certificateNo = o.a.CertificateNo,
                        employeeName = o.a.EmployeeName,
                        sex = o.a.Sex,
                        age = o.a.Birthday.IsNull() ? "" : (DateTime.Now.Year - o.a.Birthday.ConvertToDateTime().Year).ToString(),
                        iDNumber = o.a.IDNumber,
                        job = o.a.Job,
                        title4Technical = o.a.Title4Technical,
                        examType = o.a.ExamType,
                        industry = o.a.Industry,
                        startCertificateDate = o.a.StartCertificateDate.ConvertToDateString(),
                        endCertificateDate = o.a.EndCertificateDate.ConvertToDateString(),
                        trainingInstitutionName = o.d == null ? "" : o.d.InstitutionName,
                        currentStatus = o.c.IsNull() ? "" : o.c.WorkFlowStatusTag,
                        operationDate = o.c.IsNull() ? "" : o.c.CreateDate.ConvertToDateString()
                    }).ToList();
                JqGridData dataResult = ConvertIListToJqGridData(param.page, param.rows, totalCount, employeeListJsonResult);
                return Json(dataResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(base.GetEmptyJqGridResult(), JsonRequestBehavior.AllowGet);
            }

        }
        public class GetEmployeeListForJqgridJsonResult
        {
            public int certificateId { get; set; }
            public int employeeId { get; set; }
            public string employeeName { get; set; }
            public string sex { get; set; }
            public string age { get; set; }
            public string iDNumber { get; set; }
            //public string enterpriseName { get; set; }
            public string job { get; set; }
            public string title4Technical { get; set; }
            public string examType { get; set; }
            public string industry { get; set; }
            public string certificateNo { get; set; }
            public string startCertificateDate { get; set; }
            public string endCertificateDate { get; set; }
            public string trainingInstitutionName { get; set; }
            public string currentStatus { get; set; }
            public string operationDate { get; set; }

        }

        #endregion

        #region 人员流程记录
        public JsonResult GetWorkFlow(int certificateId)
        {
            try
            {
                List<WorkFlowRecord> workFlowRecordList = new List<WorkFlowRecord>();

                Sys_Account account = base.LoginAccount as Sys_Account;
                IAccountCtrl accountCtrl = new AccountCtrl(account);
                ICertificateDelay_XX_WorkFlowCtrl certificateDelay_XX_WorkFlowCtrl = new CertificateDelay_XX_WorkFlowCtrl(account);
                ICertificateCtrl certificateCtrl = new CertificateCtrl(account);
                //证书延期申请
                {
                    List<Biz_XX_CertificateDelayApplyRecord> certificateDelayApplyRecordList = certificateDelay_XX_WorkFlowCtrl.GetCertificateDelayApplyRecordListByCertificateIdList(new List<int>() { certificateId });
                    List<WorkFlowRecord> tmpWorkFlowRecordList = certificateDelayApplyRecordList.Select(p => new WorkFlowRecord()
                      {
                          Operation = "证书延期申请",
                          Remark = "",
                          OperaDateTime = p.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                          OperaUserName = accountCtrl.GetUserName(p.CreateById)
                      }).ToList();
                    workFlowRecordList.AddRange(tmpWorkFlowRecordList);
                }
                //证书资料审核
                {
                    List<Biz_XX_CertificateDelayDataCheckedRecord> certificateDelayDataCheckedRecordList = certificateDelay_XX_WorkFlowCtrl.GetCertificateDelayDataCheckedRecordList(new List<int>() { certificateId });
                    List<WorkFlowRecord> tmpWorkFlowRecordList = certificateDelayDataCheckedRecordList.Select(p => new WorkFlowRecord()
                    {
                        Operation = p.PassStatus ? "证书延期资格审核通过" : "证书延期资格审核不通过",
                        Remark = p.CheckedMark,
                        OperaDateTime = p.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        OperaUserName = accountCtrl.GetUserName(p.CreateById)
                    }).ToList();
                    workFlowRecordList.AddRange(tmpWorkFlowRecordList);
                }
                //证书延期确认
                {
                    List<Biz_CertificateDelayConfirmRecord> certificateDelayConfirmRecordList = certificateCtrl.GetCertificateDelayConfirmRecordListByCertificateIdList(new List<int>() { certificateId });
                    List<WorkFlowRecord> tmpWorkFlowRecordList = certificateDelayConfirmRecordList.Select(p => new WorkFlowRecord()
                      {
                          Operation = p.PassStatus ? "证书延期确认通过" : "证书延期确认不通过",
                          Remark = p.CheckedMark,
                          OperaDateTime = p.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                          OperaUserName = accountCtrl.GetUserName(p.CreateById)
                      }).ToList();
                    workFlowRecordList.AddRange(tmpWorkFlowRecordList);
                }
                workFlowRecordList = workFlowRecordList.OrderBy(p => p.OperaDateTime).ToList();
                int totalCount = workFlowRecordList.Count();
                int page = 1;
                int rows = 100;
                JqGridData dataResult = ConvertIListToJqGridData(page, rows, totalCount, workFlowRecordList);
                return Json(dataResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return base.GetEmptyJqGridResult();
            }

        }
        public class WorkFlowRecord
        {
            public string Operation { get; set; }
            public string Remark { get; set; }
            public string OperaDateTime { get; set; }
            public string OperaUserName { get; set; }
        }
        #endregion
    }
}
