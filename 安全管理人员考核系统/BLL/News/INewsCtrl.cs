using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using DAL;
using Library.baseFn;

namespace BLL
{
    public interface INewsCtrl
    {
        List<NewsWithNoContent> GetAllNews();
        List<NewsWithNoContent> GetAllNews(string newsTitle, string publishDate_From, string publishDate_To);
        List<NewsWithNoContent> GetPublishedNewsList();
        PagedNewsList GetPagedPublishedNewsList(int pageIndex, int pageSize);
        Biz_News GetNewsById(string newsId);
        List<NewsWithNoContent> GetNewsByIdList(IList<string> newsIdList);
        void InsertNews(IList<Biz_News> newsList);
        void UpdateNews(IList<Biz_News> newsList);
        void DeleteNews(IList<string> newsIds);
    }
}
