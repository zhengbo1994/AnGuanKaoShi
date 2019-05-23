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
    public partial class NewsController : BaseController
    {
        //
        // GET: /News/
        public ActionResult Index()
        {
            return View();
        }

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
    }
}
