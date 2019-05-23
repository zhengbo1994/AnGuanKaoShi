using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrintApp.Common
{
    public class CertificateChangeInfo
    {
        public int Id { get; set; }
        public string PersonName { get; set; }
        public string Sex { get; set; }
        public DateTime BirthDate { get; set; }
        public string IDNumber { get; set; }
        public string EnterpriseName { get; set; }
        public string Duty { get; set; }
        public string TechnicalGrade { get; set; }
        public string CertificateNo { get; set; }
        public DateTime StartCertificateDate { get; set; }
        public DateTime EndCertificateDate { get; set; }
        public string ChangeLog { get; set; }
    }
}
