using System;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    /// <summary>
    /// 证书与证书实名认证关联表
    /// </summary>
    public class Biz_RelCertificateAuthentication
    {
        [Key]
        public int Id { get; set; }
        //延期后新证书ID
        public int CertificateId { get; set; }
        public int CertificateAuthenticationId { get; set; }
        public int CreateById { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
