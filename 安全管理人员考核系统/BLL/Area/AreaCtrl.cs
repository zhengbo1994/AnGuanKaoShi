using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
using Library.baseFn;

namespace BLL
{
    public class AreaCtrl : IAreaCtrl
    {
        private Uow uow;

        public AreaCtrl()
        {
            if (uow == null)
            {
                uow = new Uow();
            }
        }

        public List<string> GetAreaListByCityName(string cityName)
        {
            int cityId = uow.Biz_City.GetAll().Where(p => p.CityName == cityName).Select(p => p.Id).FirstOrDefault();
            IQueryable<Biz_Area> cityIQueryable = uow.Biz_Area.GetAll();
            if (cityId!=0)
            {
                cityIQueryable = cityIQueryable.Where(p => p.CityId == cityId);
            }
            List<string> cityList = cityIQueryable.OrderBy(p => p.Seq).Select(p => p.AreaName).ToList();
            return cityList;
        }
    }
}
