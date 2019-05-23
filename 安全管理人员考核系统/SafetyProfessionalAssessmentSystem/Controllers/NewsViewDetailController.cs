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
    public class NewsViewDetailController : BaseController
    {
        //
        // GET: /NewsViewDetail/

        public ActionResult Index(string newsid)
        {
            ViewBag.News = GetNewsDetail(newsid);
            return View();
        }

        #region 获取新闻详情
        public class GetNewsDetail_News
        {
            public string Id { get; set; }
            public string NewsTitle { get; set; }
            public string NewsContent { get; set; }
            public string PublishDate { get; set; }
        }
        private GetNewsDetail_News GetNewsDetail(string newsId)
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
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion
    }
}
