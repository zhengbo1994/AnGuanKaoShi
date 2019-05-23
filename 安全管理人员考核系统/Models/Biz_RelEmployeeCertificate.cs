using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    /// <summary>
    /// 初次取证人员和证书关联表
    /// </summary>
    public class Biz_RelEmployeeCertificate
    {
        [Key]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int CertificateId { get; set; }
    }
}
