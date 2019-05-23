using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BLL
{
    public interface ICityCtrl
    {
        List<Biz_City> GetAllCityList();
        List<string> GetCityList();
    }
}
