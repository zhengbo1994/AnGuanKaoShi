using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Model
{
    public class Biz_CertificateChangeRecord
    {
        [Key]
        public int Id { get; set; }
        public string EmployeeName { get; set; }
        public string Sex { get; set; }
        public string Birthday { get; set; }
        public string IDNumber { get; set; }
        public string OldEnterpriseName { get; set; }
        public string EnterpriseName { get; set; }
        public string Job { get; set; }
        public string Title4Technical { get; set; }
        public string CertificateNo { get; set; }
        public string ExamType { get; set; }
        public string Industry { get; set; }
        public DateTime StartCertificateDate { get; set; }
        public DateTime EndCertificateDate { get; set; }
        public bool SubmitStatus { get; set; }
        public DateTime? SubmitDate { get; set; }
        public int? SubmitById { get; set; }
        public bool? PassFlag { get; set; }
        public DateTime? CheckDate { get; set; }
        public string Remark { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateById { get; set; }
    }
}
