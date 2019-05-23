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
    public class EnterpriseDetailsController : BaseController
    {
        //
        // GET: /EnterpriseDetails/

        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetEnterpriseDetails()
        {
            Sys_Account account = LoginAccount as Sys_Account;
            IEnterpriseCtrl enterpriseCtrl = new EnterpriseCtrl(account);
            Biz_Enterprise enterprise = enterpriseCtrl.GetEnterpriseById(account.UserId);
            EnterpriseResult enterpriseResult = new EnterpriseResult()
            {
                EnterpriseId = enterprise.Id,
                EnterpriseName = enterprise.EnterpriseName,
                SocialCreditCode = enterprise.SocialCreditCode,
                City = enterprise.City,
                Area = enterprise.Area,
                LocaltionCity = enterprise.LocaltionCity,
                EnterpriseAddress = enterprise.EnterpriseAddress,
                LegalRepresentative = enterprise.LegalRepresentative,
                LegalRepresentativeNumber = enterprise.LegalRepresentativeNumber,
                ContactPerson = enterprise.ContactPerson,
                ContactNumber = enterprise.ContactNumber,
                Email = enterprise.Email
            };
            return Json(enterpriseResult, JsonRequestBehavior.AllowGet);
        }
        public class EnterpriseResult
        {
            public int EnterpriseId { get; set; }
            //企业名称
            public string EnterpriseName { get; set; }

            //社会信用代码
            public string SocialCreditCode { get; set; }

            //城市
            public string City { get; set; }

            //区域
            public string Area { get; set; }
            //所在城市
            public string LocaltionCity { get; set; }
            //企业地址
            public string EnterpriseAddress { get; set; }

            //法定代表人
            public string LegalRepresentative { get; set; }

            //法定代表人电话
            public string LegalRepresentativeNumber { get; set; }

            //联系人
            public string ContactPerson { get; set; }

            //联系人电话
            public string ContactNumber { get; set; }
            //邮箱
            public string Email { get; set; }
        }

        #region 修改企业信息
        public class UpdateEnterpriseParam
        {
            public string EnterpriseName { get; set; }
            public string LegalRepresentative { get; set; }
            public string LegalRepresentativeNumber { get; set; }
            public string ContactPerson { get; set; }
            public string ContactNumber { get; set; }
            public string City { get; set; }
            public string Area { get; set; }
            public string EnterpriseAddress { get; set; }
            public string Email { get; set; }
            //public string LocaltionCity { get; set; }
        }
        public JsonResult UpdateEnterprise(UpdateEnterpriseParam param)
        {
            ResultMessage resultMessage = new ResultMessage();
            try
            {
                Sys_Account account = LoginAccount as Sys_Account;
                IEnterpriseCtrl enterpriseCtrl = new EnterpriseCtrl(account);
                Biz_Enterprise oldEnterprise = enterpriseCtrl.GetEnterpriseById(account.UserId);
                oldEnterprise.EnterpriseName=param.EnterpriseName;
                oldEnterprise.LegalRepresentative = param.LegalRepresentative;
                oldEnterprise.LegalRepresentativeNumber = param.LegalRepresentativeNumber;
                oldEnterprise.ContactPerson = param.ContactPerson;
                oldEnterprise.ContactNumber = param.ContactNumber;
                oldEnterprise.City=param.City;
                oldEnterprise.Area=param.Area;
                oldEnterprise.EnterpriseAddress = param.EnterpriseAddress;
                oldEnterprise.Email = param.Email;
                enterpriseCtrl.UpdateEnterprise(oldEnterprise);
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

        #region 更新企业信息(通过一体化企业平台)

        public JsonResult RefreshEnterprise(int enterpriseId)
        {
            //var enterpriseId = Id;
            ResultMessage resultMessage = new ResultMessage();
            try
            {
                //Sys_Account account = base.LoginAccount as Sys_Account;
                //IEnterpriseCtrl enterpriseCtrl = new EnterpriseCtrl(account);
                //Biz_Enterprise enterprise = enterpriseCtrl.GetEnterpriseById(enterpriseId);
                //string SERVICEACCOUNTNAME = System.Web.Configuration.WebConfigurationManager.AppSettings["ServiceAccountname"];
                //string SERVICEPASSWORD = System.Web.Configuration.WebConfigurationManager.AppSettings["ServicePassword"];
                //System.Xml.XmlNode dataSetXmlNode = null;
                //try
                //{
                //    EnterpriseInfoWebService.AgryService webService = new EnterpriseInfoWebService.AgryService();
                //    dataSetXmlNode = webService.clsEnterprise(enterprise.SocialCreditCode, enterprise.EnterpriseType, SERVICEACCOUNTNAME, SERVICEPASSWORD);
                //    if (dataSetXmlNode == null)
                //    {
                //        throw new Exception("webservice返回值为空");
                //    }
                //}
                //catch (Exception ee)
                //{
                //    throw new Exception("webservice调用出错", ee);
                //}
                //enterpriseCtrl.RefreshEnterprise(enterprise.Id, enterprise.SocialCreditCode, enterprise.EnterpriseType, SERVICEACCOUNTNAME, SERVICEPASSWORD, dataSetXmlNode);
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
