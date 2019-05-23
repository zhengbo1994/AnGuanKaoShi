﻿using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
using Library.baseFn;

namespace BLL
{
    public class CityCtrl : ICityCtrl
    {
        private Uow uow;
        private Sys_Account loginAccount;

        public CityCtrl(Sys_Account account)
        {
            if (uow == null)
            {
                uow = new Uow();
            }
            this.loginAccount = account;
        }

        public List<string> GetCityList()
        {
            IQueryable<Biz_City> cityQueryable = uow.Biz_City.GetAll();
            if (loginAccount != null && loginAccount.RoleList.Where(p => p.RoleType == RoleCtrl.RoleType.Master).Count() > 0)
            {
                cityQueryable = cityQueryable.Where(p => p.CityName != "测试市");
            }
            List<string> cityList = cityQueryable.OrderBy(p => p.Seq).Select(p => p.CityName).ToList();
            return cityList;
        }

        public List<Biz_City> GetAllCityList()
        {
            IQueryable<Biz_City> cityQueryable = uow.Biz_City.GetAll();
            if (loginAccount != null && loginAccount.RoleList.Where(p => p.RoleType == RoleCtrl.RoleType.Master).Count() > 0)
            {
                cityQueryable = cityQueryable.Where(p => p.CityName != "测试市");
            }
            List<Biz_City> cityList = cityQueryable.OrderBy(p => p.Seq).ToList();
            return cityList;
        }

    }
}
