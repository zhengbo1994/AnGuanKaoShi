using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SafetyProfessionalAssessmentTrainSystem
{
    // 使方法可以被js跨域请求到的处理
    public class WebAPIFilterAttribute : System.Web.Mvc.ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.AddHeader("Access-Control-Allow-Origin", "*");
            filterContext.HttpContext.Response.AddHeader("Access-Control-Allow-Methods", "POST, GET, OPTIONS, PUT, DELETE, HEAD");
            filterContext.HttpContext.Response.AddHeader("Access-Control-Allow-Headers", "X-PINGOTHER, Origin, X-Requested-With, Content-Type, Accept");
            filterContext.HttpContext.Response.AddHeader("Access-Control-Max-Age", "1728000");

            base.OnActionExecuting(filterContext);
        }
    }
}