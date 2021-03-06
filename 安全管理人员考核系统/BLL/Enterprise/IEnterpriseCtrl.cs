﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;

namespace BLL
{
    public interface IEnterpriseCtrl
    {
        void InsertEnterprise(Biz_Enterprise enterprise, string accountName = null, string password = null);

        void UpdateEnterprise(Biz_Enterprise enterprise);

        void DeleteEnterprise(int enterpriseId);

        Biz_Enterprise GetEnterpriseById(int enterpriseById);

        List<Biz_Enterprise> GetEnterpriseList(string EnterpriseName, string SocialCreditCode, string City, string Area, int page, int rows, ref int totalCount,List<string> cityList);
        List<Biz_Enterprise> GetEnterpriseInfoByIdList(List<int> enterpriseIdList);
        void RegisterEnterprise(string socialCreditCode, string enterpriseName, string city, string area);
        void RefreshEnterprise(int enterpriseId, string socialCreditCode, string enterpriseType, string serviceAccountName, string servicePassword, System.Xml.XmlNode dataSetXmlNode);
        List<Biz_Enterprise> GetEnterpriseListByCityList(List<string> cityList);
    }
}
