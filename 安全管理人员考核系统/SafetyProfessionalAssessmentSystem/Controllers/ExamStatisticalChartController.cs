using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
using BLL;
using Library.baseFn;

namespace SafetyProfessionalAssessmentSystem.Controllers
{
    [AuthorizeFilter]
    public class ExamStatisticalChartController : BaseController
    {
        //
        // GET: /ExamStatisticalChart/

        public ActionResult Index()
        {
            return View();
        }

        #region 获取各地市考试情况
        [JsonException]
        public JsonResult GetPassRateDataForCity(GetPassRateDataForCityParam param)
        {

            Sys_Account account = LoginAccount as Sys_Account;

            IEmployeeCtrl employeeCtrl = new EmployeeCtrl(account);

            DateTime beginDate = param.ExamDateTimeBegin.ConvertToDateTime();

            DateTime endDate = param.ExamDateTimeEnd.ConvertToDateTime().AddDays(1);

            List<GetPassRateDataForCityResult> getPassRateDataForCityResultList = new List<GetPassRateDataForCityResult>();

            GetPassRateDataForCityResult totalGetPassRateDataForCityResult = new GetPassRateDataForCityResult();
            totalGetPassRateDataForCityResult.name = "报考人数";
            totalGetPassRateDataForCityResult.type = "column";
            totalGetPassRateDataForCityResult.data = employeeCtrl.GetTotalEmployeeCityAndCountList(beginDate, endDate);
            getPassRateDataForCityResultList.Add(totalGetPassRateDataForCityResult);

            GetPassRateDataForCityResult takeGetPassRateDataForCityResult = new GetPassRateDataForCityResult();
            takeGetPassRateDataForCityResult.name = "参考人数";
            takeGetPassRateDataForCityResult.type = "column";
            takeGetPassRateDataForCityResult.data = employeeCtrl.GetTakeEmployeeCityAndCountList(beginDate, endDate);
            getPassRateDataForCityResultList.Add(takeGetPassRateDataForCityResult);

            GetPassRateDataForCityResult passGetPassRateDataForCityResult = new GetPassRateDataForCityResult();
            passGetPassRateDataForCityResult.name = "合格人数";
            passGetPassRateDataForCityResult.type = "column";
            passGetPassRateDataForCityResult.data = employeeCtrl.GetPassEmployeeCityAndCountList(beginDate, endDate);
            getPassRateDataForCityResultList.Add(passGetPassRateDataForCityResult);

            GetPassRateDataForCityResult rateGetPassRateDataForCityResult = new GetPassRateDataForCityResult();
            rateGetPassRateDataForCityResult.name = "通过率";
            rateGetPassRateDataForCityResult.type = "line";
            rateGetPassRateDataForCityResult.yAxis = 1;

            List<double> passRateList = new List<double>();
            for (int i = 0; i < passGetPassRateDataForCityResult.data.Count; i++)
            {
                double passRate = takeGetPassRateDataForCityResult.data[i] == 0 ? 0 : Math.Round(Convert.ToDouble(passGetPassRateDataForCityResult.data[i]) * 100 / Convert.ToDouble(takeGetPassRateDataForCityResult.data[i]), 2);
                passRateList.Add(passRate);
            }

            rateGetPassRateDataForCityResult.data = passRateList;
            getPassRateDataForCityResultList.Add(rateGetPassRateDataForCityResult);

            return Json(getPassRateDataForCityResultList, JsonRequestBehavior.AllowGet);

        }

        public class GetPassRateDataForCityParam
        {
            public string ExamDateTimeBegin { get; set; }
            public string ExamDateTimeEnd { get; set; }
        }

        public class GetPassRateDataForCityResult
        {
            public string name { get; set; }

            public string type { get; set; }
            public List<double> data { get; set; }

            public int yAxis { get; set; }
        }

        #endregion

        #region 获取各企业考试情况
        [JsonException]
        public JsonResult GetPassRateDataForEnterprise(GetPassRateDataForEnterpriseParam param)
        {
            int totalCount = 0;

            Sys_Account account = LoginAccount as Sys_Account;

            IEmployeeCtrl employeeCtrl = new EmployeeCtrl(account);

            DateTime beginDate = param.ExamDateTimeBegin.ConvertToDateTime();

            DateTime endDate = param.ExamDateTimeEnd.ConvertToDateTime().AddDays(1);

            List<EnterpriseExamPassRate> enterpriseExamPassRateList = employeeCtrl.GetEnterpriseExamPassRate(beginDate, endDate, param.EnterpriseName, param.page, param.rows, ref  totalCount);
            List<GetPassRateDataForEnterpriseResult> getPassRateDataForEnterpriseResultList = enterpriseExamPassRateList.Select(p => new GetPassRateDataForEnterpriseResult()
            {
                EnterpriseName = p.EnterpriseName,
                TotalCount = p.TotalCount,
                TakeCount = p.TakeCount,
                PassCount = p.PassCount,
                //百分制通过率保留两位小数
                PassRate = (p.TakeCount == 0 ? 0 : Math.Round(Convert.ToDouble(p.PassCount) * 100 / Convert.ToDouble(p.TakeCount), 2)).ToString()
            }).ToList();


            JqGridData dataResult = ConvertIListToJqGridData(param.page, param.rows, totalCount, getPassRateDataForEnterpriseResultList);
            return Json(dataResult, JsonRequestBehavior.AllowGet);
        }

        public class GetPassRateDataForEnterpriseParam
        {
            public string ExamDateTimeBegin { get; set; }
            public string ExamDateTimeEnd { get; set; }
            public string EnterpriseName { get; set; }
            public int page { get; set; }
            public int rows { get; set; }
            public string sidx { get; set; }//排序字段名称
            public string sord { get; set; }//排序方式asc desc
        }

        public class GetPassRateDataForEnterpriseResult
        {

            public string EnterpriseName { get; set; }
            public int TotalCount { get; set; }
            public int TakeCount { get; set; }
            public int PassCount { get; set; }
            public string PassRate { get; set; }


        }


        #endregion

        #region 获取各培训机构考试情况
        [JsonException]
        public JsonResult GetPassRateDataForInstitution(GetPassRateDataForInstitutionParam param)
        {
            int totalCount = 0;

            Sys_Account account = LoginAccount as Sys_Account;

            IEmployeeCtrl employeeCtrl = new EmployeeCtrl(account);

            DateTime beginDate = param.ExamDateTimeBegin.ConvertToDateTime();

            DateTime endDate = param.ExamDateTimeEnd.ConvertToDateTime().AddDays(1);

            List<InstitutionExamPassRate> institutionExamPassRateList = employeeCtrl.GetInstitutionExamPassRate(beginDate, endDate, param.InstitutionName, param.page, param.rows, ref  totalCount);
            List<GetPassRateDataForInstitutionResult> getPassRateDataForInstitutionResultList = institutionExamPassRateList.Select(p => new GetPassRateDataForInstitutionResult()
            {
                InstitutionName = p.InstitutionName,
                TotalCount = p.TotalCount,
                TakeCount = p.TakeCount,
                PassCount = p.PassCount,
                //百分制通过率保留两位小数
                PassRate = (p.TakeCount == 0 ? 0 : Math.Round(Convert.ToDouble(p.PassCount) * 100 / Convert.ToDouble(p.TakeCount), 2)).ToString()
            }).ToList();


            JqGridData dataResult = ConvertIListToJqGridData(param.page, param.rows, totalCount, getPassRateDataForInstitutionResultList);
            return Json(dataResult, JsonRequestBehavior.AllowGet);
        }

        public class GetPassRateDataForInstitutionParam
        {
            public string ExamDateTimeBegin { get; set; }
            public string ExamDateTimeEnd { get; set; }
            public string InstitutionName { get; set; }
            public int page { get; set; }
            public int rows { get; set; }
            public string sidx { get; set; }//排序字段名称
            public string sord { get; set; }//排序方式asc desc
        }

        public class GetPassRateDataForInstitutionResult
        {

            public string InstitutionName { get; set; }
            public int TotalCount { get; set; }
            public int TakeCount { get; set; }
            public int PassCount { get; set; }
            public string PassRate { get; set; }


        }


        #endregion

        #region 获取年度考试情况
        [JsonException]
        public JsonResult GetPassRateDataForYear()
        {

            Sys_Account account = LoginAccount as Sys_Account;

            IEmployeeCtrl employeeCtrl = new EmployeeCtrl(account);



            List<GetPassRateDataForYearResult> getPassRateDataForYearResultList = new List<GetPassRateDataForYearResult>();

            GetPassRateDataForYearResult totalGetPassRateDataForYearResult = new GetPassRateDataForYearResult();
            totalGetPassRateDataForYearResult.name = "报考人数";
            totalGetPassRateDataForYearResult.type = "column";
            totalGetPassRateDataForYearResult.data = employeeCtrl.GetTotalEmployeeMonthAndCountList();
            getPassRateDataForYearResultList.Add(totalGetPassRateDataForYearResult);

            GetPassRateDataForYearResult takeGetPassRateDataForYearResult = new GetPassRateDataForYearResult();
            takeGetPassRateDataForYearResult.name = "参考人数";
            takeGetPassRateDataForYearResult.type = "column";
            takeGetPassRateDataForYearResult.data = employeeCtrl.GetTakeEmployeeMonthAndCountList();
            getPassRateDataForYearResultList.Add(takeGetPassRateDataForYearResult);

            GetPassRateDataForYearResult passGetPassRateDataForYearResult = new GetPassRateDataForYearResult();
            passGetPassRateDataForYearResult.name = "合格人数";
            passGetPassRateDataForYearResult.type = "column";
            passGetPassRateDataForYearResult.data = employeeCtrl.GetPassEmployeeMonthAndCountList();
            getPassRateDataForYearResultList.Add(passGetPassRateDataForYearResult);

            GetPassRateDataForYearResult rateGetPassRateDataForYearResult = new GetPassRateDataForYearResult();
            rateGetPassRateDataForYearResult.name = "通过率";
            rateGetPassRateDataForYearResult.type = "line";
            rateGetPassRateDataForYearResult.yAxis = 1;

            List<double> passRateList = new List<double>();
            for (int i = 0; i < passGetPassRateDataForYearResult.data.Count; i++)
            {
                double passRate = takeGetPassRateDataForYearResult.data[i] == 0 ? 0 : Math.Round(Convert.ToDouble(passGetPassRateDataForYearResult.data[i]) * 100 / Convert.ToDouble(takeGetPassRateDataForYearResult.data[i]), 2);
                passRateList.Add(passRate);
            }

            rateGetPassRateDataForYearResult.data = passRateList;
            getPassRateDataForYearResultList.Add(rateGetPassRateDataForYearResult);

            return Json(getPassRateDataForYearResultList, JsonRequestBehavior.AllowGet);
        }

        public class GetPassRateDataForYearResult
        {
            public string name { get; set; }
            public string type { get; set; }
            public List<double> data { get; set; }
            public int yAxis { get; set; }
        }

        #endregion

        public JsonResult GetMonthList()
        {
            Sys_Account account = LoginAccount as Sys_Account;
            IEmployeeCtrl employeeCtrl = new EmployeeCtrl(account);

            List<string> monthList = employeeCtrl.GetQueryableMonthList().Select(p => p.ToString("MMM")).ToList();

            return Json(monthList, JsonRequestBehavior.AllowGet);

        }
    }
}
