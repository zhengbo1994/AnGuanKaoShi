using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace PrintApp.Common
{
    public class CertificateInfo
    {

        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Sex { get; set; }
        public string Birthday { get; set; }
        public string IDNumber { get; set; }
        public string EnterpriseName { get; set; }
        public string Job { get; set; }
        public string Title4Technical { get; set; }
        public string ExamPlanNumber { get; set; }
        public string TrainingInstutionName { get; set; }
        public string Industry { get; set; }
        public string ExamType { get; set; }
        public string CertificateNo { get; set; }
        public string StartCertificateDate { get; set; }
        public string  EndCertificateDate { get; set; }
    }
}
