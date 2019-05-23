using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SafetyProfessionalAssessmentSystem.Controllers
{
    [AuthorizeFilter]
    public class CertificateEmployeeInfoChangeAuditController : BaseController
    {
        //
        // GET: /CertificateEmployeeInfoChangeAudit/

        public ActionResult Index()
        {
            return View();
        }

    }
}
