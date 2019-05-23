using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Library.baseFn;

namespace SafetyProfessionalAssessmentSystem.Controllers
{
    [AuthorizeFilter]
    public class NewsManageController : BaseController
    {
        //
        // GET: /NewsManage/

        public ActionResult Index()
        {
            return View();
        }

        private Sys_Account Account
        {
            get
            {
                return LoginAccount as Sys_Account;
            }
        }

        #region 获取新闻列表
        public class GetAllNews_SearchParam
        {
            public string NewsTitle { get; set; }
            public string PublishDate_From { get; set; }
            public string PublishDate_To { get; set; }
        }

        public class GetAllNews_News
        {
            public string Id { get; set; }
            public string NewsTitle { get; set; }
            public string NewsAbstract { get; set; }
            public string PublishDate { get; set; }
        }

        public JsonResult GetAllNews(string strData)
        {
            try
            {
                GetAllNews_SearchParam para = strData.JSONStringToObj<GetAllNews_SearchParam>();
                INewsCtrl news = new NewsCtrl();
                List<NewsWithNoContent> newsList = news.GetAllNews(para.NewsTitle, para.PublishDate_From, para.PublishDate_To);
                List<GetAllNews_News> result = new List<GetAllNews_News>();
                if (newsList.Count > 0)
                {
                    result = newsList.Select(x => new GetAllNews_News()
                    {
                        Id = x.Id,
                        NewsTitle = x.NewsTitle,
                        NewsAbstract = x.NewsAbstract,
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

        #region 获取新闻详情
        public class GetNewsDetail_News
        {
            public string Id { get; set; }
            public string NewsTitle { get; set; }
            public string NewsContent { get; set; }
            public string PublishDate { get; set; }
        }

        public JsonResult GetNewsDetail(string newsId)
        {
            try
            {
                INewsCtrl newsCtrl = new NewsCtrl();
                Biz_News news = newsCtrl.GetNewsById(newsId);
                GetNewsDetail_News result = new GetNewsDetail_News()
                {
                    Id = news.Id,
                    NewsTitle = news.NewsTitle,
                    NewsContent = news.NewsContent,
                    PublishDate = news.PublishDate.ToString("yyyy-MM-dd")
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        #region 新增新闻
        public class InsertNews_News
        {
            public string NewsTitle { get; set; }
            public string NewsAbstract { get; set; }
            public string NewsContent { get; set; }
            public string PublishDate { get; set; }
        }
        public JsonResult InsertNews(string strData)
        {
            ResultMessage result = new ResultMessage();
            try
            {
                InsertNews_News para = strData.JSONStringToObj<InsertNews_News>();

                Biz_News news = new Biz_News();
                news.NewsTitle = para.NewsTitle;
                news.NewsAbstract = para.NewsAbstract;
                news.NewsContent = para.NewsContent;
                news.PublishDate = para.PublishDate.ConvertToDateTime();
                news.CreateBy = Account.Id;

                INewsCtrl newsCtrl = new NewsCtrl();
                newsCtrl.InsertNews(new List<Biz_News>() { news });
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = ex.Message;
            }
            return Json(result);
        }
        #endregion

        #region 修改新闻
        public class UpdateNews_News
        {
            public string Id { get; set; }
            public string NewsTitle { get; set; }
            public string NewsAbstract { get; set; }
            public string NewsContent { get; set; }
            public string PublishDate { get; set; }
        }
        public JsonResult UpdateNews(string strData)
        {
            ResultMessage result = new ResultMessage();
            try
            {
                UpdateNews_News para = strData.JSONStringToObj<UpdateNews_News>();

                INewsCtrl newsCtrl = new NewsCtrl();

                Biz_News news = newsCtrl.GetNewsById(para.Id);
                news.NewsTitle = para.NewsTitle;
                news.NewsAbstract = para.NewsAbstract;
                news.NewsContent = para.NewsContent;
                news.PublishDate = para.PublishDate.ConvertToDateTime();

                newsCtrl.UpdateNews(new List<Biz_News>() { news });
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = ex.Message;
            }
            return Json(result);
        }
        #endregion

        #region 删除新闻
        public class DeleteNews_NewsIds
        {
            public List<string> IdList { get; set; }
        }
        public JsonResult DeleteNews(string strData)
        {
            ResultMessage result = new ResultMessage();
            try
            {
                DeleteNews_NewsIds para = strData.JSONStringToObj<DeleteNews_NewsIds>();

                INewsCtrl newsCtrl = new NewsCtrl();

                newsCtrl.DeleteNews(para.IdList);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = ex.Message;
            }
            return Json(result);
        }
        #endregion
    }
}
