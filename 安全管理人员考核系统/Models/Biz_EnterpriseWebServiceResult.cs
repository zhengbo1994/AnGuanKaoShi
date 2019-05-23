using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Model
{
    public class Biz_EnterpriseWebServiceResult
    {
        [Key]
        public int Id { get; set; }
        public string SocialCreditCode { get; set; }
        public string EnterpriseType { get; set; }
        public string ServiceAccountName { get; set; }
        public string ServicePassword { get; set; }
        public string ServiceResultXML { get; set; }
        public string Remark { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
