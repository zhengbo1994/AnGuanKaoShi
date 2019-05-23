using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL;
using Model;
using Library.baseFn;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Data;
using ClassLibrary;

namespace SafetyProfessionalAssessmentSystem.Controllers
{
    public class LoginController : BaseController
    {
        const string loginValidateCodeKey = "LoginValidateCode";
        const string certificateSearchValidateCodeKey = "CertificateSearchValidateCode";
        const string examResultSearchValidateCodeKey = "ExamResultSearchValidateCode";
        //
        // GET: /Login/
        public ActionResult Index()
        {
            return View();
        }

        #region 验证用户登录是否正确
        private bool CheckValidateCode(string validateCode, string validateCodeKey)
        {
            if (validateCode.IsNull())
            {
                return false;
            }
            string key = GetValidateCode(validateCodeKey);
            return validateCode.ToLower() == key.ToLower();
        }
        private string GetValidateCode(string validateCodeKey)
        {
            string code = null;
            try
            {
                code = Session[validateCodeKey].ToString();
            }
            catch (Exception ex)
            {
                code = "";
            }
            return code;
        }
        public JsonResult UserLogin(string loginname, string password, string validateCode)
        {
            ResultMessage msg = new ResultMessage();
            try
            {
                if (!this.CheckValidateCode(validateCode, loginValidateCodeKey))
                {
                    throw new Exception("验证码错误");
                }
                IAccountCtrl accountCtrl = new AccountCtrl(null);
                bool loginFlag = accountCtrl.ValidAccount(loginname, password);
                if (loginFlag)
                {
                    Sys_Account account = accountCtrl.GetAccountByAccountName(loginname);
                    LoginAccount = account;
                    msg.IsSuccess = true;
                    msg.ErrorMessage = account.RoleList.SingleOrDefault().RoleType;
                }
                else
                {
                    msg.IsSuccess = false;
                    msg.ErrorMessage = "登录失败：用户名或密码错误！";
                }
            }
            catch (Exception ex)
            {
                msg.IsSuccess = false;
                msg.ErrorMessage = ex.Message;
            }

            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 企业注册
        public JsonResult RegisterEnterprise(RegisterEnterpriseParam param)
        {

            ResultMessage resultMessage = new ResultMessage();
            try
            {

                IEnterpriseCtrl enterpriseCtrl = new EnterpriseCtrl(null);
                enterpriseCtrl.RegisterEnterprise(param.socialCreditCode, param.enterpriseName, param.city, param.area);
                resultMessage.IsSuccess = true;

            }
            catch (Exception ex)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = ex.Message;
            }
            return Json(resultMessage);
        }
        public class RegisterEnterpriseParam
        {
            public string socialCreditCode { get; set; }
            public string enterpriseName { get; set; }
            public string city { get; set; }
            public string area { get; set; }
        }
        #endregion

        #region 根据USBKEY的序列号获取信息中心企业信息
        public JsonResult GetAGRYEnterprise(string passwordKey)
        {
            GetAGRYEnterpriseResult result = new GetAGRYEnterpriseResult();
            try
            {
                Sys_Account account = base.LoginAccount as Sys_Account;
                ISmrzServiceCtrl smrzServiceCtrl = new SmrzServiceCtrl(account);
                SmrzServiceCtrl.AGRYEnterpriseInfo agryEnterprise = smrzServiceCtrl.GetAGRYEnterpriseInfo(passwordKey);
                if (agryEnterprise == null)
                {
                    throw new Exception("在建委信息中心未找到此加密锁对应的企业信息");
                }
                if (agryEnterprise.zhuxiao == "0")//0表示注销1表示正常
                {
                    throw new Exception("此加密锁已经注销,请联系建委信息中心");
                }
                result.socialCreditCode = agryEnterprise.zzjgdm3;
                result.enterpriseName = agryEnterprise.qymc;

            }
            catch (Exception ex)
            {
                result.resultMessage.IsSuccess = false;
                result.resultMessage.ErrorMessage = ex.Message;
            }
            return Json(result);
        }
        public class GetAGRYEnterpriseResult
        {
            public GetAGRYEnterpriseResult()
            {
                this.resultMessage = new ResultMessage();
            }
            public ResultMessage resultMessage { get; set; }
            public string socialCreditCode { get; set; }
            public string enterpriseName { get; set; }
        }

        #endregion

        #region 加密锁登录
        public JsonResult USBKEYLogin(string usbKeySn)
        {
            Sys_Account loginaccount = base.LoginAccount as Sys_Account;
            ISmrzServiceCtrl smrzServiceCtrl = new SmrzServiceCtrl(loginaccount);
            try
            {
                SmrzServiceCtrl.AGRYEnterpriseInfo agryEnterprise = smrzServiceCtrl.GetAGRYEnterpriseInfo(usbKeySn);
                if (agryEnterprise == null)
                {
                    throw new Exception("加密锁无效,请联系建委信息中心");
                }
                if (agryEnterprise.zhuxiao == "0")
                {
                    throw new Exception("加密锁已注销,请联系建委信息中心");
                }
                IAccountCtrl accountCtrl = new AccountCtrl(null);
                Sys_Account account = accountCtrl.GetAccountByAccountName(agryEnterprise.zzjgdm3);
                if (account == null)
                {
                    throw new Exception("加密锁未在本系统注册,请先注册后再登录");
                }
                string result = smrzServiceCtrl.KeyLogin(usbKeySn, agryEnterprise.zzjgdm3);

                if (result.ToLower() != "true")
                {
                    throw new Exception(string.Format("加密锁验证失败{0}", result));
                }
                string ValidateCodes = this.GetValidateCode(loginValidateCodeKey);
                return this.UserLogin(account.AccountName, account.Password, ValidateCodes);
            }
            catch (Exception ex)
            {
                ResultMessage resultMessage = new ResultMessage()
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
                return Json(resultMessage);
            }

        }
        #endregion


        #region 获取发布的新闻列表

        public class GetPublishedNews_Param
        {
            //当前页码
            public int current_page { get; set; }
            //每页包含题目数量
            public int page_size { get; set; }
        }
        public class GetPublishedNews_Result
        {
            //当前页码
            public int current_page { get; set; }
            //每页包含题目数量
            public int page_size { get; set; }
            //总页数
            public int max_page { get; set; }
            public List<GetPublishedNews_News> data { get; set; }
        }
        public class GetPublishedNews_News
        {
            public string Id { get; set; }
            public string NewsTitle { get; set; }
            public string PublishDate { get; set; }
        }
        public JsonResult GetPublishedNews(GetPublishedNews_Param para)
        {
            try
            {
                INewsCtrl news = new NewsCtrl();
                PagedNewsList newsList = news.GetPagedPublishedNewsList(para.current_page, para.page_size);
                GetPublishedNews_Result result = new GetPublishedNews_Result();
                if (null != newsList.Data && newsList.Data.Count > 0)
                {
                    result.current_page = para.current_page;
                    result.page_size = para.page_size;
                    result.max_page = newsList.TotalPage;
                    result.data = newsList.Data.Select(x => new GetPublishedNews_News()
                    {
                        Id = x.Id,
                        NewsTitle = x.NewsTitle,
                        PublishDate = x.PublishDate.ToString("yyyy-MM-dd")
                    }).ToList();
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        #region 证书查询
        public class CertificateSearchParam
        {
            public string examType { get; set; }
            public string employeeName { get; set; }
            public string iDNumber { get; set; }
            public string validateCode { get; set; }
        }
        public JsonResult CertificateSearch(CertificateSearchParam param)
        {
            try
            {
                //if (!this.CheckValidateCode(param.validateCode, certificateSearchValidateCodeKey))
                //{
                //    throw new Exception("验证码错误");
                //}
                Sys_Account account = base.LoginAccount as Sys_Account;
                ICertificateCtrl certificateCtrl = new CertificateCtrl(account);
                Biz_Certificate certificate = certificateCtrl.CertificateSearch(param.examType, param.employeeName, param.iDNumber);
                if (certificate == null)
                {
                    throw new Exception("证书不存在或已经注销");
                }

                CertificateSearchResult result = new CertificateSearchResult()
                {
                    employeeName = certificate.EmployeeName,
                    sex = certificate.Sex,
                    birthday = certificate.Birthday,
                    iDNumber = certificate.IDNumber,
                    enterpriseName = certificate.EnterpriseName,
                    job = certificate.Job,
                    title4Technical = certificate.Title4Technical,
                    certificateNo = certificate.CertificateNo,
                    examType = certificate.ExamType,
                    industry = certificate.Industry,
                    startCertificateDate = certificate.StartCertificateDate.ConvertToDateString(),
                    endCertificateDate = certificate.EndCertificateDate.ConvertToDateString()
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                ResultMessage resultMessage = new ResultMessage()
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
                return Json(resultMessage);
            }

        }
        public class CertificateSearchResult
        {
            public string employeeName { get; set; }
            public string sex { get; set; }
            public string birthday { get; set; }
            public string iDNumber { get; set; }
            public string enterpriseName { get; set; }
            public string job { get; set; }
            public string title4Technical { get; set; }
            public string certificateNo { get; set; }
            public string examType { get; set; }
            public string industry { get; set; }
            public string startCertificateDate { get; set; }
            public string endCertificateDate { get; set; }
        }
        #endregion

        #region 成绩查询
        public class ExamResultSearchParam
        {
            public string iDNumber { get; set; }
        }
        public JsonResult ExamResultSearch(ExamResultSearchParam param)
        {
            ResultMessage resultMessage = new ResultMessage() { IsSuccess = true };
            try
            {
              
                Sys_Account account = base.LoginAccount as Sys_Account;
                //IStatisticalReportCtrl statisticalReportCtrl = new StatisticalReportCtrl(account);
                //Biz_EmployeeExamResultRecord examresult = statisticalReportCtrl.ExamResultSearch(param.iDNumber);
                //if (examresult == null)
                //{
                //    throw new Exception("考试成绩不存在");
                //}
                //Biz_ExamPlanRecord examplan = statisticalReportCtrl.GetExamPlanRecordByExamPlanId(examresult.ExamPlanRecordId);
                //string msg = string.Format("考试计划流水号：{0}\r\n", examplan.ExamPlanNumber);
                //IEmployeeCtrl employeeCtrl = new EmployeeCtrl(account);

                //string msg2 = employeeCtrl.GetEmployeeExamResultRecordByIdNumber(param.iDNumber);
                //if (examresult.SafetyKnowledgeExamScore != null)
                //{
                //    msg += string.Format("安全知识考核成绩：{0}\r\n", examresult.SafetyKnowledgeExamScore);
                //}
                //if (examresult.ManagementAbilityExamScore != null)
                //{
                //    msg += string.Format("管理能力考核成绩：{0}\r\n", examresult.ManagementAbilityExamScore);
                //}
                //if (examresult.FieldExamResult != null)
                //{
                //    msg += string.Format("实操考核成绩：{0}\r\n", examresult.FieldExamResult);
                //}
                //if (examresult.FinalExamResult != null)
                //{
                //    msg += string.Format("最终考核成绩：{0}\r\n", examresult.FinalExamResult);
                //}
                //resultMessage.ErrorMessage = msg;
                //resultMessage.ErrorMessage = msg + msg2;
                IEmployeeCtrl employeeCtrl = new EmployeeCtrl(account);
                IWorkFlowCtrl workFlowCtrl=new WorkFlowCtrl(account);
                Biz_EmployeeExamResultRecord employeeExamResultRecord = employeeCtrl.GetPublishEmployeeExamResultRecordByIdNumber(param.iDNumber);
                if (employeeExamResultRecord.IsNull())
                {
                    throw new Exception("暂无考试成绩,或身份证号码输入错误");
                }
                Biz_ExamPlanRecord examPlanRecord = workFlowCtrl.GetExamPlanRecordById(employeeExamResultRecord.ExamPlanRecordId);

                string msg = string.Format("考试计划流水号：{0}\r\n", examPlanRecord.ExamPlanNumber);
                if (!employeeExamResultRecord.SafetyKnowledgeExamResult.IsNull())
                {
                    msg += string.Format("安全知识考核成绩：{0}\r\n", employeeExamResultRecord.SafetyKnowledgeExamResult);
                }
                if (!employeeExamResultRecord.ManagementAbilityExamResult.IsNull())
                {
                    msg += string.Format("管理能力考核成绩：{0}\r\n", employeeExamResultRecord.ManagementAbilityExamResult);
                }
                if (!employeeExamResultRecord.ActualOperationExamResult.IsNull())
                {
                    msg += string.Format("实操考核成绩：{0}\r\n", employeeExamResultRecord.ActualOperationExamResult);
                }
                msg += string.Format("最终考核成绩：{0}\r\n", employeeExamResultRecord.FinalExamResult);
                resultMessage.ErrorMessage = msg;
            }
            catch (Exception ex)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = ex.Message;
            }

            return Json(resultMessage);
        }
        #endregion

        #region 考试计划公示
        public class GetPublishedExamPlanList_Param
        {
            //当前页码
            public int current_page { get; set; }
            //每页包含题目数量
            public int page_size { get; set; }
        }

        public JsonResult GetPublishedExamPlanList(GetPublishedExamPlanList_Param para)
        {
            JsonResult result = new JsonResult();
            try
            {
                Sys_Account account = LoginAccount as Sys_Account;
                IExamManageCtrl examC = new ExamManageCtrl(account);
                List<Biz_ExamPlanRecord> examPlanList = examC.ExamPlanPublicity();
                List<GetPublishedExamPlanResult> publishedExamResultList = examPlanList.Select(p => new GetPublishedExamPlanResult()
                {
                    id = p.Id,
                    examPlanNumber = p.ExamPlanNumber,
                    examDateTimeBegin = p.ExamDateTimeBegin.ToString("yyyy-MM-dd HH:mm")
                }).ToList();
                return Json(publishedExamResultList);
            }
            catch (Exception ex)
            {
                ResultMessage err = new ResultMessage();
                err.IsSuccess = false;
                err.ErrorMessage = ex.Message;
                result.Data = err;
            }
            return result;
        }
        public class GetPublishedExamPlanResult
        {
            public int id { get; set; }
            public string examPlanNumber { get; set; }
            public string examDateTimeBegin { get; set; }
        }
        #endregion

        #region 考试结果公式
        public class GetPublishedExamResultList_Param
        {
            //当前页码
            public int current_page { get; set; }
            //每页包含题目数量
            public int page_size { get; set; }
        }

        public JsonResult GetPublishedExamResultList(GetPublishedExamResultList_Param para)
        {
            try
            {
                Sys_Account account = LoginAccount as Sys_Account;
                IExamManageCtrl examC = new ExamManageCtrl(account);
                List<Biz_ExamPlanRecord> examPlanList = examC.GetPublishedExamResultList();
                List<GetPublishedExamResult> publishedExamResultList = examPlanList.Select(p => new GetPublishedExamResult()
                {
                    id = p.Id,
                    examPlanNumber = p.ExamPlanNumber,
                    examDateTimeBegin = p.ExamDateTimeBegin.ToString("yyyy-MM-dd HH:mm")
                }).ToList();
                return Json(publishedExamResultList);
            }
            catch (Exception ex)
            {
                ResultMessage err = new ResultMessage();
                err.IsSuccess = false;
                err.ErrorMessage = ex.Message;
                return Json(err);
            }

        }
        public class GetPublishedExamResult
        {
            public int id { get; set; }
            public string examPlanNumber { get; set; }
            public string examDateTimeBegin { get; set; }
        }

        #endregion


        private byte[] GenValidateCode(string validateCodeKey)
        {
            ValidateCodeImage v = new ValidateCodeImage();
            v.FontFamilies = new string[] { "微软雅黑", "Arial", "Lucida Wide" };
            string codes = v.CreateValidateCodes(4);
            Session[validateCodeKey] = codes;
            using (System.IO.MemoryStream ms = v.CreateValidateCodesImageStream(codes))
            {
                return ms.ToArray();
            }
        }

        #region 获取登录验证码
        public FileResult GetValidateCodeImage()
        {
            try
            {
                byte[] fileContent = GenValidateCode(loginValidateCodeKey);
                return File(fileContent, "image/png");
            }
            catch { return null; }
        }
        #endregion

        #region 获取证书查询验证码
        public FileResult GetCertificateSearchValidateCodeImage()
        {
            try
            {
                byte[] fileContent = GenValidateCode(certificateSearchValidateCodeKey);
                return File(fileContent, "image/png");
            }
            catch { return null; }
        }
        #endregion

        #region 获取考试结果查询验证码
        public FileResult GetExamResultSearchValidateCodeImage()
        {
            try
            {
                byte[] fileContent = GenValidateCode(examResultSearchValidateCodeKey);
                return File(fileContent, "image/png");
            }
            catch { return null; }
        }
        #endregion
    }
}
