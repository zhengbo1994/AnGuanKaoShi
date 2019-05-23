﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
using BLL;

namespace SafetyProfessionalAssessmentTrainSystem.Controllers
{
    [AuthorizeFilter]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        #region 获取菜单信息
        public JsonResult GetMenuList()
        {
            //List<Sys_Page> leafPageList = LoginPageList.OrderBy(p => p.Sequence).ToList();
            //IPageCtrl pageCtrl = new PageCtrl(LoginAccount as Sys_Account);
            //List<Sys_Page> totalPageList = pageCtrl.GetPageListByLeafPage(leafPageList);


            List<Sys_Page> totalPageList = new List<Sys_Page>();
            totalPageList.Add(new Sys_Page() { Id = 1, PageName = "证书延期", Url = null, Icon = "fa-laptop", ParentId = 0, Sequence = 1 });
            totalPageList.Add(new Sys_Page() { Id = 2, PageName = "延期申报", Url = "/CertificateDelayApply	", Icon = "", ParentId = 1, Sequence = 2 });
            totalPageList.Add(new Sys_Page() { Id = 3, PageName = "延期资格审核", Url = "/CertificateDelayDataCheck", Icon = "", ParentId = 1, Sequence = 3 });
            totalPageList.Add(new Sys_Page() { Id = 3, PageName = "延期记录查询", Url = "/DelayReCord", Icon = "", ParentId = 1, Sequence = 4 });

            if (LoginRoleList.Where(p => p.RoleType == RoleCtrl.RoleType.Enterprise).Count() > 0)
            {
                List<string> pageList = new List<string>() { "证书延期", "延期申报", "延期记录查询" };
                totalPageList = totalPageList.Where(p => pageList.Contains(p.PageName)).ToList();
            }
            else if (LoginRoleList.Where(p => p.RoleType == RoleCtrl.RoleType.TrainingInstitution).Count() > 0)
            {
                List<string> pageList = new List<string>() { "证书延期", "延期资格审核", "延期记录查询" };
                totalPageList = totalPageList.Where(p => pageList.Contains(p.PageName)).ToList();
            }
            else if (LoginRoleList.Where(p => p.RoleType == RoleCtrl.RoleType.Manager).Count() > 0)
            {
                List<string> pageList = new List<string>() { "证书延期","延期记录查询" };
                totalPageList = totalPageList.Where(p => pageList.Contains(p.PageName)).ToList();
            }
            else
            {
                totalPageList = totalPageList.Where(p => 1 == 2).ToList();
            }

            List<GetMenuListResult> menuList = totalPageList.Select(p => new GetMenuListResult()
            {
                Id = p.Id,
                Title = p.PageName,
                Icon = p.Icon,
                Url = p.Url,
                ParentId = p.ParentId
            }).ToList();
            return Json(menuList, JsonRequestBehavior.AllowGet);
        }

        public class GetMenuListResult
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Icon { get; set; }
            public string Url { get; set; }
            public int ParentId { get; set; }

        }



        #endregion
        //获取登录用户名
        public JsonResult GetUserName()
        {
            return Json(base.LoginUserName, JsonRequestBehavior.AllowGet);
        }
        //修改密码
        public JsonResult ChangePwd(string oldPwd, string newPwd)
        {
            ResultMessage resultMessage = new ResultMessage();
            resultMessage.IsSuccess = true;
            try
            {
                Sys_Account account = LoginAccount as Sys_Account;
                IAccountCtrl accountCtrl = new AccountCtrl(account);
                accountCtrl.ChangePassword(oldPwd, newPwd);
                LogOut();
            }
            catch (Exception ex)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = ex.Message;
            }
            return Json(resultMessage, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LogOut()
        {
            ResultMessage resultMessage = new ResultMessage();
            resultMessage.IsSuccess = true;
            try
            {
                LoginAccount = null;
                Session.Clear();
            }
            catch (Exception ex)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = ex.Message;
            }
            return Json(resultMessage, JsonRequestBehavior.AllowGet);
        }
    }
}
