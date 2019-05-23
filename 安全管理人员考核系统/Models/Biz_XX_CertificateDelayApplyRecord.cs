using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Security;

namespace Model
{
    /// <summary>
    /// 证书延期申请
    /// </summary>
    public class Biz_XX_CertificateDelayApplyRecord
    {
        [Key]
        public int Id { get; set; }
        public int CertificateId { get; set; }
        public int TrainingInstitutionId { get; set; }
        public string Remark { get; set; }
        public int CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public int SummitById { get; set; }
        public bool SubmitStatus { get; set; }
        public DateTime? SummitDate { get; set; }
        public bool OperationStatus { get; set; }
        public bool Invalid { get; set; } //无效的数据
    }
}
