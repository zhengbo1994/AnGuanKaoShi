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

namespace SafetyProfessionalAssessmentSystem.Controllers
{
    public class Login2Controller : BaseController
    {
        //
        // GET: /Login/
        public ActionResult Index()
        {
            return View();
        }

        #region 验证用户登录是否正确
        public ActionResult UserLogin(string loginname, string password)
        {
            ResultMessage msg = new ResultMessage();
            try
            {

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

        #region 获取建委信息中心企业信息
        public JsonResult GetAGRYEnterprise(string passwordKey)
        {
            GetAGRYEnterpriseResult result = new GetAGRYEnterpriseResult();
            try
            {

                ISmrzServiceCtrl smrzServiceCtrl = new SmrzServiceCtrl(null);
                try
                {
                    SmrzServiceCtrl.AGRYEnterpriseInfo agryEnterpriseInfo = smrzServiceCtrl.GetAGRYEnterpriseInfo(passwordKey);
                    if (agryEnterpriseInfo==null)
                    {
                        throw new Exception("webservice返回值为空");
                    }

                    if (agryEnterpriseInfo.zhuxiao == "0")//0表示注销1表示正常
                    {
                        throw new Exception("此加密锁已经注销,请联系建委信息中心");
                    }
                    result.socialCreditCode = agryEnterpriseInfo.zzjgdm3;
                    result.enterpriseName = agryEnterpriseInfo.qymc;
                }
                catch (Exception ee)
                {
                    throw new Exception("webservice调用出错", ee);
                }
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


        #region 加密锁 加密
        public string EncryptDog(string PassWord)
        {
            string DogWordKey = "cdgsabcd";
            return this.Encrypt(PassWord, DogWordKey);
        }
        private string Encrypt(string pToEncrypt, string sKey)
        {
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            byte[] bytes = Encoding.Default.GetBytes(pToEncrypt);
            provider.Key = Encoding.ASCII.GetBytes(sKey);
            provider.IV = Encoding.ASCII.GetBytes(sKey);
            MemoryStream stream = new MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, provider.CreateEncryptor(), CryptoStreamMode.Write);
            stream2.Write(bytes, 0, bytes.Length);
            stream2.FlushFinalBlock();
            StringBuilder builder = new StringBuilder();
            foreach (byte num in stream.ToArray())
            {
                builder.AppendFormat("{0:X2}", num);
            }
            builder.ToString();
            return builder.ToString();
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
                IStatisticalReportCtrl statisticalReportCtrl = new StatisticalReportCtrl(account);
                Biz_EmployeeExamResultRecord examresult = statisticalReportCtrl.ExamResultSearch(param.iDNumber);
                if (examresult == null)
                {
                    throw new Exception("考试成绩不存在");
                }
                Biz_ExamPlanRecord examplan = statisticalReportCtrl.GetExamPlanRecordByExamPlanId(examresult.ExamPlanRecordId);
                string msg = string.Format("考试计划流水号：{0}\r\n", examplan.ExamPlanNumber);
                if (examresult.SafetyKnowledgeExamScore != null)
                {
                    msg += string.Format("安全知识考核成绩：{0}\r\n", examresult.SafetyKnowledgeExamScore);
                }
                if (examresult.ManagementAbilityExamScore != null)
                {
                    msg += string.Format("管理能力考核成绩：{0}\r\n", examresult.ManagementAbilityExamScore);
                }
                if (examresult.FieldExamResult != null)
                {
                    msg += string.Format("实操考核成绩：{0}\r\n", examresult.FieldExamResult);
                }
                if (examresult.FinalExamResult != null)
                {
                    msg += string.Format("最终考核成绩：{0}\r\n", examresult.FinalExamResult);
                }
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

        #region 考试计划
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
                List<ExamPlanAbstractInfo> infoList = examC.GetPublishedExamPlanList(para.current_page, para.page_size).ToList();
                result.Data = infoList;
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
        #endregion

        #region 考试结果
        public class GetPublishedExamResultList_Param
        {
            //当前页码
            public int current_page { get; set; }
            //每页包含题目数量
            public int page_size { get; set; }
        }

        public JsonResult GetPublishedExamResultList(GetPublishedExamResultList_Param para)
        {
            JsonResult result = new JsonResult();
            try
            {
                Sys_Account account = LoginAccount as Sys_Account;
                IExamManageCtrl examC = new ExamManageCtrl(account);
                List<ExamResultAbstractInfo> infoList = examC.GetPublishedExamResultList(para.current_page, para.page_size).ToList();
                result.Data = infoList;
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
        #endregion
    }
}
