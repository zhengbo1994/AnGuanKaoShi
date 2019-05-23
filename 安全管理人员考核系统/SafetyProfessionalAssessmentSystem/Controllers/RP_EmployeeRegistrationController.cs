using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Model;
using BLL;
using Library.baseFn;
using System.Data;
using System.Transactions;

namespace SafetyProfessionalAssessmentSystem.Controllers
{
    [AuthorizeFilter]
    public class RP_EmployeeRegistrationController : BaseController
    {
        //
        // GET: /RP_EmployeeRegistration/
        public ActionResult Index()
        {
            return View();
        }
        #region 根据查询条件获取jqgrid人员信息
        public class GetEmployeeListForJqgridParam
        {
            public string employeeName { get; set; }
            public string iDNumber { get; set; }
            public string examType { get; set; }
            public string industry { get; set; }
            public string trainingType { get; set; }
            public string workFlowStatus { get; set; }
            public string trainingInstitutionName { get; set; }
            public int page { get; set; }
            public int rows { get; set; }
            public string sidx { get; set; }//排序字段名称
            public string sord { get; set; }//排序方式asc desc

        }

        public JsonResult GetCertificateListForJqgrid(GetEmployeeListForJqgridParam param)
        {
            try
            {
                int totalCount = 0;
                Sys_Account account = LoginAccount as Sys_Account;
                IRP_EmployeeCtrl rp_employeeCtrl = new RP_EmployeeCtrl(account);
                IRP_WorkFlowCtrl rpWorkFlowCtrl = new RP_WorkFlowCtrl(account);
                ICertificateCtrl certificateCtrl = new CertificateCtrl(account);
                IEnterpriseCtrl enterpriseCtrl = new EnterpriseCtrl(account);

                Biz_Enterprise enterprise = enterpriseCtrl.GetEnterpriseById(account.UserId);
                string enterpriseName = enterprise.EnterpriseName;
                List<Biz_Certificate> certificateList = certificateCtrl.GetCertificateList(param.employeeName, param.iDNumber, null, "有效的", enterpriseName, param.page, param.rows, ref totalCount);
                List<string> certificateNoList = certificateList.Select(p => p.CertificateNo).ToList();
                List<RP_WorkFlowCtrl.WorkFlowStatus> workFlowStatusList = rpWorkFlowCtrl.GetCurrentWorkFlowStatusByCertificateNoList(certificateNoList);
                List<int> rpEmployeeIdList = workFlowStatusList.Select(p => p.RPEmployeeId).ToList();
                List<Biz_RP_EmployeeRegistration> rpEmployeeList = rp_employeeCtrl.GetRP_EmployeeRegistrationByIdList(rpEmployeeIdList);


                List<GetEmployeeListForJqgridJsonResult> employeeListJsonResult = certificateList
                    .GroupJoin(rpEmployeeList, a => a.CertificateNo, b => b.OldCertificateNo, (a, b) => new { a,b=b.FirstOrDefault()})
                    .GroupJoin(workFlowStatusList, o => (o.b.IsNull() ? 0 : o.b.Id), c => c.RPEmployeeId, (o, c) => new { o.a, o.b, c = c.FirstOrDefault() })
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
            public string city { get; set; }
            public string certificateNo { get; set; }
            public string currentStatus { get; set; }
            public string operationDate { get; set; }

        }

        #endregion

        #region 根据证书编号获取证书信息
        public JsonResult GetCertificateInfo(string certificateNo)
        {
            try
            {
                Sys_Account account = base.LoginAccount as Sys_Account;
                ICertificateCtrl certificateCtrl = new CertificateCtrl(account);
                GetCertificateInfoResult result = null;
                Biz_Certificate certificate = certificateCtrl.GetCertificateByNo(certificateNo);
                if (certificate.IsNull())//本地找不到 然后就去建委信息中心找
                {

                    ISmrzServiceCtrl smrzServiceCtrl = new SmrzServiceCtrl(account);
                    SmrzServiceCtrl.CerificateInfo certificateInfo = smrzServiceCtrl.GetCertificateByCertificateNo(certificateNo);
                    if (certificateInfo != null)//找到以后存下来
                    {
                        result = new GetCertificateInfoResult()
                        {
                            EmployeeName = certificateInfo.name,
                            Sex = certificateInfo.sex,
                            Birthday = certificateInfo.born,
                            IDNumber = certificateInfo.cardno,
                            EnterpriseName = certificateInfo.qymc,
                            Job = certificateInfo.zhiwu,
                            Title4Technical = certificateInfo.zhicheng,
                            CertificateNo = certificateInfo.zsbh,
                            StartCertificateDate = certificateInfo.yxqkssj.ToString("yyyy-MM-dd"),
                            EndCertificateDate = certificateInfo.yxqjssj.ToString("yyyy-MM-dd")
                        };
                    }
                }
                else
                {
                    result = new GetCertificateInfoResult()
                    {
                        EmployeeName = certificate.EmployeeName,
                        Sex = certificate.Sex,
                        Birthday = certificate.Birthday,
                        IDNumber = certificate.IDNumber,
                        EnterpriseName = certificate.EnterpriseName,
                        Job = certificate.Job,
                        Title4Technical = certificate.Title4Technical,
                        CertificateNo = certificate.CertificateNo,
                        StartCertificateDate = certificate.StartCertificateDate.ToString("yyyy-MM-dd"),
                        EndCertificateDate = certificate.EndCertificateDate.ToString("yyyy-MM-dd")
                    };
                }
                if (certificate.IsNull())
                {
                    throw new Exception("此证书不存在");
                }
                if (certificate.EndCertificateDate.ConvertToDateTime() < DateTime.Now)
                {
                    throw new Exception("证书超过有效期");
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                ResultMessage result = new ResultMessage()
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
                return Json(result);
            }
        }
        public class GetCertificateInfoResult
        {
            public string EmployeeName { get; set; }
            public string Sex { get; set; }
            public string Birthday { get; set; }
            public string IDNumber { get; set; }
            public string EnterpriseName { get; set; }
            public string Job { get; set; }
            public string Title4Technical { get; set; }
            public string CertificateNo { get; set; }
            public string ExamType { get; set; }
            public string Industry { get; set; }
            public string StartCertificateDate { get; set; }
            public string EndCertificateDate { get; set; }
        }
        #endregion

        #region 取证人员报名
        public class EmployeeRegistrationParam
        {
            // ID主键
            public int Id { get; set; }
            public int TrainingInstitutionId { get; set; }
            //人员名称
            public string EmployeeName { get; set; }
            //性别
            public string Sex { get; set; }
            // 出生日期
            public string Birthday { get; set; }
            //身份证号
            public string IDNumber { get; set; }
            //职位
            public string Job { get; set; }
            // 技术职称
            public string Title4Technical { get; set; }
            //报考城市
            public string City { get; set; }
            //报考科目
            public string ExamType { get; set; }
            public string Industry { get; set; }
            //备注
            public string Remark { get; set; }
            public string OldCertificateNo { get; set; }
            public bool PrintCertificate { get; set; }
            public string ConstructorCertificateNo { get; set; }

        }
        public JsonResult RegistrationEmployee(EmployeeRegistrationParam employeeParam)
        {
            ResultMessage resultMessage = new ResultMessage();
            try
            {
                Sys_Account account = LoginAccount as Sys_Account;
                //保存人员信息
                Biz_RP_EmployeeRegistration rp_employee = new Biz_RP_EmployeeRegistration()
                {
                    Id = employeeParam.Id,
                    TrainingInstitutionId = employeeParam.TrainingInstitutionId,
                    EmployeeName = employeeParam.EmployeeName,
                    Sex = employeeParam.Sex,
                    Birthday = employeeParam.Birthday,
                    IDNumber = employeeParam.IDNumber.Trim(),
                    Job = employeeParam.Job,
                    Title4Technical = employeeParam.Title4Technical,
                    City = employeeParam.City,
                    ExamType = employeeParam.ExamType,
                    Industry = employeeParam.Industry,
                    SubmitStatus = false,
                    OperationStatus = false,
                    CreateById = account.Id,
                    CreateDate = DateTime.Now,
                    EnterpriseId = account.UserId,
                    PrintCertificate = employeeParam.PrintCertificate,
                    Remark = employeeParam.Remark,
                    OldCertificateNo = employeeParam.OldCertificateNo
                };

                //写入流程表
                IRP_WorkFlowCtrl workflowCtrl = new RP_WorkFlowCtrl(account);
                // VerifyCertificateNo(rp_employee);
                workflowCtrl.RegisterRPEmployee(rp_employee);
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

        #region 删除报名信息
        public JsonResult DeleteEmployeeById(int employeeId)
        {
            ResultMessage resultMessage = new ResultMessage();
            try
            {
                Sys_Account account = LoginAccount as Sys_Account;
                IRP_EmployeeCtrl rp_EmployeeCtrl = new RP_EmployeeCtrl(account);
                rp_EmployeeCtrl.DeleteRP_EmployeeRegistrationById(employeeId);
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

        #region 获取报名人员信息
        public JsonResult GetEmployeeById(int id)
        {
            Sys_Account account = LoginAccount as Sys_Account;
            IRP_EmployeeCtrl rp_EmployeeCtrl = new RP_EmployeeCtrl(account);
            Biz_RP_EmployeeRegistration rpEmployee = rp_EmployeeCtrl.GetRP_EmployeeRegistrationById(id);
            EmployeeRegistrationParam dataResult = new EmployeeRegistrationParam()
            {
                Id = rpEmployee.Id,
                TrainingInstitutionId = rpEmployee.TrainingInstitutionId,
                EmployeeName = rpEmployee.EmployeeName,
                Sex = rpEmployee.Sex,
                Birthday = rpEmployee.Birthday,
                IDNumber = rpEmployee.IDNumber,
                Job = rpEmployee.Job,
                Title4Technical = rpEmployee.Title4Technical,
                City = rpEmployee.City,
                ExamType = rpEmployee.ExamType,
                Industry = rpEmployee.Industry,
                PrintCertificate = rpEmployee.PrintCertificate,
                Remark = rpEmployee.Remark,
                OldCertificateNo = rpEmployee.OldCertificateNo
            };
            return Json(dataResult, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 取证人员提交
        public JsonResult SubmitEmployee(string strEmployeeIdList)
        {
            List<int> employeeIdList = AppFn.JSONStringToObj<List<int>>(strEmployeeIdList);

            ResultMessage resultMessage = new ResultMessage();
            try
            {
                Sys_Account account = LoginAccount as Sys_Account;
                IRP_WorkFlowCtrl workflowCtrl = new RP_WorkFlowCtrl(account);
                workflowCtrl.SubmitRPEmployeeRegistrationRecord(employeeIdList);
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

        #region 更新报名人员信息
        public JsonResult UpdateRegistrationEmployee(EmployeeRegistrationParam employeeParam)
        {
            ResultMessage resultMessage = new ResultMessage();
            try
            {
                Sys_Account account = LoginAccount as Sys_Account;
                IRP_EmployeeCtrl rpEmployeeCtrl = new RP_EmployeeCtrl(account);
                Biz_RP_EmployeeRegistration oldRPEmployee = new Biz_RP_EmployeeRegistration()
                {
                    Id = employeeParam.Id,
                    TrainingInstitutionId = employeeParam.TrainingInstitutionId,
                    EmployeeName = employeeParam.EmployeeName,
                    Sex = employeeParam.Sex,
                    Birthday = employeeParam.Birthday,
                    IDNumber = employeeParam.IDNumber,
                    Job = employeeParam.Job,
                    Title4Technical = employeeParam.Title4Technical,
                    City = employeeParam.City,
                    ExamType = employeeParam.ExamType,
                    Industry = employeeParam.Industry,
                    PrintCertificate = employeeParam.PrintCertificate,
                    Remark = employeeParam.Remark,
                    OldCertificateNo = employeeParam.OldCertificateNo
                };
                rpEmployeeCtrl.UpdateEmployeeRegistrationInfo(oldRPEmployee);
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

        #region  获取企业所在城市
        public JsonResult GetEnterpriseCity()
        {
            Sys_Account account = base.LoginAccount as Sys_Account;
            IEnterpriseCtrl enterpriseCtrl = new EnterpriseCtrl(account);
            string city = enterpriseCtrl.GetEnterpriseById(account.UserId).City;
            return Json(city, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region 获取企业名称
        public string GetEnterpriseName()
        {
            return LoginUserName;
        }
        #endregion

        #region 获取培训机构列表
        public JsonResult GetTrainingInstitutionList()
        {
            Sys_Account account = base.LoginAccount as Sys_Account;
            ITrainingInstitutionCtrl trainingInstitutionCtrl = new TrainingInstitutionCtrl(account);
            IEnterpriseCtrl enterpriseCtrl = new EnterpriseCtrl(account);
            string city = enterpriseCtrl.GetEnterpriseById(account.UserId).City;
            List<string> cityList = new List<string>() { city };
            List<GetTrainingInstitutionListResult> trainingInstitutionList = trainingInstitutionCtrl.GetTrainingInstitutionListByCityList(cityList).Select(p => new GetTrainingInstitutionListResult()
            {
                ItemText = p.InstitutionName,
                ItemValue = p.Id.ToString()
            }).ToList();
            return Json(trainingInstitutionList, JsonRequestBehavior.DenyGet);
        }
        public class GetTrainingInstitutionListResult
        {
            public string ItemText { get; set; }
            public string ItemValue { get; set; }
        }
        #endregion

        #region 证书延期提交
        public class SubmitParam
        {
            public int trainingInstitutionId { get; set; }
            public List<int> certificateIdList { get; set; }
            public string submitRemark { get; set; }
        }
        public JsonResult Submit(string strParam)
        {
            Sys_Account account = base.LoginAccount as Sys_Account;
            ResultMessage result = new ResultMessage() { IsSuccess = true };

            TransactionScope scope = CreateTransaction();

            try
            {
                SubmitParam param = strParam.JSONStringToObj<SubmitParam>();
                IEnterpriseCtrl enterpriseCtrl = new EnterpriseCtrl(account);
                Biz_Enterprise enterprise = enterpriseCtrl.GetEnterpriseById(account.UserId);

                ICertificateCtrl certificateCtrl = new CertificateCtrl(account);
                IRP_WorkFlowCtrl workFlowCtrl = new RP_WorkFlowCtrl(account);
                List<Biz_Certificate> certificateList = certificateCtrl.GetCertificateByIdList(param.certificateIdList);
                List<int> rpEmployyIdList = new List<int>();
                for (int i = 0; i < certificateList.Count; i++)
                {
                    Biz_Certificate certificate = certificateList[i];
                    Biz_RP_EmployeeRegistration reEmployee = new Biz_RP_EmployeeRegistration()
                    {
                        EmployeeName = certificate.EmployeeName,
                        Sex = certificate.Sex,
                        Birthday = certificate.Birthday,
                        IDNumber = certificate.IDNumber,
                        EnterpriseId = enterprise.Id,
                        Job = certificate.Job,
                        Title4Technical = certificate.Title4Technical,
                        ExamType = certificate.ExamType,
                        Industry = certificate.Industry,
                        StartCertificateDate = certificate.StartCertificateDate,
                        EndCertificateDate = certificate.EndCertificateDate,
                        OldCertificateNo = certificate.CertificateNo,
                        TrainingInstitutionId = param.trainingInstitutionId,
                        City = enterprise.City,
                        Remark = param.submitRemark,
                        PrintCertificate = true,
                        CreateById = account.Id,
                        CreateDate = DateTime.Now,
                        SummitById = account.Id,
                        SubmitStatus = false,
                        SummitDate = DateTime.Now,
                        OperationStatus = false,
                        Invalid = false
                    };
                    workFlowCtrl.RegisterRPEmployee(reEmployee);
                    rpEmployyIdList.Add(reEmployee.Id);
                }
                workFlowCtrl.SubmitRPEmployeeRegistrationRecord(rpEmployyIdList);
                TransactionCommit(scope);
            }
            catch (Exception ex)
            {
                TransactionRollback(scope);
                result = new ResultMessage() { IsSuccess = false, ErrorMessage = ex.Message };
            }
            return Json(result);
        }
        #endregion
    }
}
