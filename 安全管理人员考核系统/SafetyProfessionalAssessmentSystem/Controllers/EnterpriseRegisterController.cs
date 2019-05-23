using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SafetyProfessionalAssessmentSystem.Controllers
{
    public class EnterpriseRegisterController : BaseController
    {
        //
        // GET: /企业注册/

        public ActionResult Index()
        {
            return View();
        }
        #region 企业注册
        public JsonResult RegisterEnterprise(RegisterEnterpriseParam param)
        {
            ResultMessage resultMessage = new ResultMessage();
            try
            {
                const string SERVICEACCOUNTNAME = "AgryServiceAccount001";
                const string SERVICEPASSWORD = "AgryServicePassword001";
                System.Xml.XmlNode dataSetXmlNode = null;
                try
                {
                    EnterpriseInfoWebService.AgryService webService = new EnterpriseInfoWebService.AgryService();
                    dataSetXmlNode = webService.clsEnterprise(param.SocialCreditCode.Trim(), param.EnterpriseType.Trim(), SERVICEACCOUNTNAME, SERVICEPASSWORD);
                    if (dataSetXmlNode == null)
                    {
                        throw new Exception("webservice返回值为空");
                    }
                }
                catch (Exception ee)
                {
                    throw new Exception("webservice调用出错", ee);
                }
                IEnterpriseCtrl enterpriseCtrl = new EnterpriseCtrl(null);
                enterpriseCtrl.RegisterEnterprise(param.SocialCreditCode, param.EnterpriseType, SERVICEACCOUNTNAME, SERVICEPASSWORD, dataSetXmlNode,param.);
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
            public string SocialCreditCode { get; set; }
            public string EnterpriseType { get; set; }
        }
        #endregion

    }
}
