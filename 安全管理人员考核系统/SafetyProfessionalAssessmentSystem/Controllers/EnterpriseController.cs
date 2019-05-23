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
    [AuthorizeFilter]
    public class EnterpriseController : BaseController
    {
        //
        // GET: /Enterprise/
        private Sys_Account account;

        public ActionResult Index()
        {
            return View();
        }

        #region 获取企业信息
        public JsonResult GetEnterpriseListForJqgrid(GetEnterpriseListForJqgridParam param)
        {
            int totalCount = 0;
            Sys_Account account = LoginAccount as Sys_Account;
            IEnterpriseCtrl enterpriseCtrl = new EnterpriseCtrl(account);
            List<string> cityList = base.LoginDataPermissionDetailList.Select(p => p.DetailName).ToList();
            List<Biz_Enterprise> enterpriseList = enterpriseCtrl.GetEnterpriseList(param.EnterpriseName, param.SocialCreditCode,
                param.City, param.Area, param.page, param.rows, ref totalCount, cityList);
            List<GetEnterpriseListForJqgridJsonResult> enterpriseListJsonResult = enterpriseList.Select(e => new GetEnterpriseListForJqgridJsonResult()
            {
                Id = e.Id,
                EnterpriseName = e.EnterpriseName,
                SocialCreditCode = e.SocialCreditCode,
                City = e.City,
                Area = e.Area,
                EnterpriseAddress = e.EnterpriseAddress,
                LegalRepresentative = e.LegalRepresentative,
                LegalRepresentativeNumber = e.LegalRepresentativeNumber,
                ContactPerson = e.ContactPerson,
                ContactNumber = e.ContactNumber,
                LocaltionCity = e.LocaltionCity,
                EnterpriseType=e.EnterpriseType
            }).ToList();
            JqGridData dataResult = ConvertIListToJqGridData(param.page, param.rows, totalCount, enterpriseListJsonResult);
            return Json(dataResult, JsonRequestBehavior.AllowGet);
        }
        public class GetEnterpriseListForJqgridParam
        {
            //企业名称
            public string EnterpriseName { get; set; }

            //社会信用代码
            public string SocialCreditCode { get; set; }

            //城市
            public string City { get; set; }

            //区域
            public string Area { get; set; }

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
            public int page { get; set; }
            public int rows { get; set; }
            public string sidx { get; set; }//排序字段名称
            public string sord { get; set; }//排序方式asc desc

        }
        public class GetEnterpriseListForJqgridJsonResult
        {
            public int Id { get; set; }
            public string EnterpriseType { get; set; }
            //企业名称
            public string EnterpriseName { get; set; }
            //社会信用代码
            public string SocialCreditCode { get; set; }
            //城市
            public string City { get; set; }
            public string LocaltionCity { get; set; }
            //区域
            public string Area { get; set; }

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

        }
        #endregion

        #region 插入企业信息
        public JsonResult InsertEnterprise(string strData)
        {
            ResultMessage resultMessage = new ResultMessage();
            try
            {
                InsertEnterpriseParam param = strData.JSONStringToObj<InsertEnterpriseParam>();
                Sys_Account account = LoginAccount as Sys_Account;
                IEnterpriseCtrl enterpriseCtrl = new EnterpriseCtrl(account);
                Biz_Enterprise enterprise = new Biz_Enterprise()
                {

                    EnterpriseName = param.EnterpriseName,
                    SocialCreditCode = param.SocialCreditCode,
                    City = param.City,
                    Area = param.Area,
                    EnterpriseAddress = param.EnterpriseAddress,
                    LegalRepresentative = param.LegalRepresentative,
                    LegalRepresentativeNumber = param.LegalRepresentativeNumber,
                    ContactPerson = param.ContactPerson,
                    ContactNumber = param.ContactNumber,
                    CreateById = account.Id,
                    CreateDate = DateTime.Now

                };

                enterpriseCtrl.InsertEnterprise(enterprise);

                resultMessage.IsSuccess = true;
            }
            catch (Exception e)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = e.Message;
            }
            return Json(resultMessage, JsonRequestBehavior.AllowGet);

        }

        public class InsertEnterpriseParam
        {
            //企业名称
            public string EnterpriseName { get; set; }

            //社会信用代码
            public string SocialCreditCode { get; set; }

            //城市
            public string City { get; set; }

            //区域
            public string Area { get; set; }

            //企业地址
            public string EnterpriseAdress { get; set; }

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


            public string EnterpriseAddress { get; set; }
        }
        #endregion

        #region        根据ID 获取企业信息
        public JsonResult GetEnterpriseById(int enterpriseId)
        {
            Sys_Account account = LoginAccount as Sys_Account;
            IEnterpriseCtrl enterpriseCtrl = new EnterpriseCtrl(account);
            Biz_Enterprise enterprise = enterpriseCtrl.GetEnterpriseById(enterpriseId);
            EnterpriseResult enterpriseResult = new EnterpriseResult()
            {
                Id = enterprise.Id,
                EnterpriseName = enterprise.EnterpriseName,
                SocialCreditCode = enterprise.SocialCreditCode,
                City = enterprise.City,
                Area = enterprise.Area,
                EnterpriseAddress = enterprise.EnterpriseAddress,
                LegalRepresentative = enterprise.LegalRepresentative,
                LegalRepresentativeNumber = enterprise.LegalRepresentativeNumber,
                ContactPerson = enterprise.ContactPerson,
                ContactNumber = enterprise.ContactNumber,
            };
            return Json(enterpriseResult, JsonRequestBehavior.AllowGet);

        }
        public class EnterpriseResult
        {
            public int Id { get; set; }
            //企业名称
            public string EnterpriseName { get; set; }

            //社会信用代码
            public string SocialCreditCode { get; set; }

            //城市
            public string City { get; set; }

            //区域
            public string Area { get; set; }

            //企业地址
            public string EnterpriseAdress { get; set; }

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


            public string EnterpriseAddress { get; set; }
        }
        #endregion
        #region 修改企业信息
        public JsonResult UpdateEnterprise(string strData)
        {
            ResultMessage resultMessage = new ResultMessage();
            try
            {
                UpdateEnterpriseParam param = strData.JSONStringToObj<UpdateEnterpriseParam>();
                Sys_Account account = LoginAccount as Sys_Account;
                IEnterpriseCtrl enterpriseCtrl = new EnterpriseCtrl(account);
                Biz_Enterprise enterprise = new Biz_Enterprise()
                {
                    Id=param.Id,
                    EnterpriseName = param.EnterpriseName,
                    SocialCreditCode = param.SocialCreditCode,
                    City = param.City,
                    Area = param.Area,
                    EnterpriseAddress = param.EnterpriseAddress,
                    LegalRepresentative = param.LegalRepresentative,
                    LegalRepresentativeNumber = param.LegalRepresentativeNumber,
                    ContactPerson = param.ContactPerson,
                    ContactNumber = param.ContactNumber,
                    CreateById = account.Id,
                    CreateDate = DateTime.Now
                };
                enterpriseCtrl.UpdateEnterprise(enterprise);

                resultMessage.IsSuccess = true;
            }
            catch (Exception e)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = e.Message;
            }
            return Json(resultMessage, JsonRequestBehavior.AllowGet);
        }

        public class UpdateEnterpriseParam
        {
            public int Id { get; set; }
            public string EnterpriseName { get; set; }

            public string SocialCreditCode { get; set; }

            public string City { get; set; }

            public string Area { get; set; }

            public string EnterpriseAddress { get; set; }

            public string LegalRepresentative { get; set; }

            public string LegalRepresentativeNumber { get; set; }

            public string ContactPerson { get; set; }

            public string ContactNumber { get; set; }
        }
        #endregion

        #region //删除企业信息
        public JsonResult DeleteEnterprise(DeleteEnterpriseParam param)
        {
            ResultMessage resultMessage = new ResultMessage();
            try
            {
                Sys_Account account = LoginAccount as Sys_Account;
                IEnterpriseCtrl enterpriseCtrl = new EnterpriseCtrl(account);

                enterpriseCtrl.DeleteEnterprise(param.EnterpriseId);
                resultMessage.IsSuccess = true;
            }
            catch (Exception e)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = e.Message;
            }
            return Json(resultMessage, JsonRequestBehavior.AllowGet);

        }

        public class DeleteEnterpriseParam
        {
            public int EnterpriseId { get; set; }
        }
        #endregion
    }
}


