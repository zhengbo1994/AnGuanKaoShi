using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using DAL;
using Library.baseFn;
using EntityFramework.Extensions;

namespace BLL
{
    public class NewsWithNoContent
    {
        public string Id { get; set; }
        public string NewsTitle { get; set; }
        public string NewsAbstract { get; set; }
        public DateTime PublishDate { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateBy { get; set; }
        public bool IsDeleted { get; set; }
    }
    public class PagedNewsList
    {
        public int TotalPage { get; set; }
        public int TotalCount { get; set; }
        public List<NewsWithNoContent> Data { get; set; }
    }

    public static class NewsExtention
    {
        //不带内容的List
        public static List<NewsWithNoContent> ToNoContentList(this IQueryable<Biz_News> source)
        {
            return source.Select(x => new NewsWithNoContent()
            {
                Id = x.Id,
                NewsTitle = x.NewsTitle,
                NewsAbstract = x.NewsAbstract,
                PublishDate = x.PublishDate,
                CreateDate = x.CreateDate,
                CreateBy = x.CreateBy,
                IsDeleted = x.IsDeleted
            }).ToList();
        }
    }

    public class NewsCtrl : INewsCtrl
    {
        private Uow _uow;

        private IQueryable<Biz_News> AvailableNews
        {
            get
            {
                return _uow.Biz_News.GetAll().Where(x => x.IsDeleted == false);
            }
        }

        public NewsCtrl()
        {
            _uow = new Uow();
        }

        public List<NewsWithNoContent> GetAllNews()
        {
            return AvailableNews.OrderByDescending(x => x.PublishDate).ToNoContentList();
        }

        public List<NewsWithNoContent> GetAllNews(string newsTitle, string publishDate_From, string publishDate_To)
        {
            IQueryable<Biz_News> queryNews = AvailableNews;
            if (!newsTitle.IsNull())
            {
                queryNews = queryNews.Where(x => x.NewsTitle.Contains(newsTitle));
            }
            if (!publishDate_From.IsNull())
            {
                DateTime dtFrom = publishDate_From.ConvertToDateTime();
                dtFrom = dtFrom.Date;
                queryNews = queryNews.Where(x => x.PublishDate >= dtFrom);
            }
            if (!publishDate_To.IsNull())
            {
                DateTime dtTo = publishDate_To.ConvertToDateTime();
                dtTo = dtTo.AddDays(1).Date;
                queryNews = queryNews.Where(x => x.PublishDate < dtTo);
            }
            return queryNews.ToNoContentList();
        }

        public List<NewsWithNoContent> GetPublishedNewsList()
        {
            return AvailableNews.OrderByDescending(x => x.PublishDate).ToNoContentList();
        }

        public PagedNewsList GetPagedPublishedNewsList(int pageIndex, int pageSize)
        {
            int totalCount = AvailableNews.Count();

            int totalPage = (totalCount / pageSize) + ((totalCount % pageSize) > 0 ? 1 : 0);

            int skipCount = (pageIndex - 1) * pageSize;

            List<NewsWithNoContent> dataList = AvailableNews
                .OrderByDescending(x => x.PublishDate)
                .Skip(skipCount)
                .Take(pageSize).ToNoContentList();

            PagedNewsList pagedList = new PagedNewsList()
            {
                TotalPage = totalPage,
                TotalCount = totalCount,
                Data = dataList
            };

            return pagedList;
        }

        public Biz_News GetNewsById(string newsId)
        {
            return AvailableNews.First(x => x.Id == newsId);
        }

        List<NewsWithNoContent> INewsCtrl.GetNewsByIdList(IList<string> newsIdList)
        {
            List<NewsWithNoContent> result = AvailableNews.Where(x => newsIdList.Contains(x.Id)).ToNoContentList();
            return result;
        }

        public void InsertNews(IList<Biz_News> newsList)
        {
            foreach (Biz_News news in newsList)
            {
                _uow.Biz_News.Add(news);
            }
            _uow.Commit();
        }

        public void UpdateNews(IList<Biz_News> newsList)
        {
            foreach (Biz_News news in newsList)
            {
                _uow.Biz_News.Update(news);
            }
            _uow.Commit();
        }

        public void DeleteNews(IList<string> newsIds)
        {
            AvailableNews.Where(x => newsIds.Contains(x.Id)).Update(x => new Biz_News() { IsDeleted = true });
            _uow.Commit();
        }
    }
}
