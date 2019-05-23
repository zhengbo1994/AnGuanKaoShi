using System;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    /// <summary>
    /// 证书发放记录
    /// </summary>
    public class Biz_EmployeeCertificateIssuanceRecord
    {
        [Key]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int ExamPlanRecordId { get; set; }
        public int TrainingInstitutionId { get; set; }
        public string Remark { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateById { get; set; }
    }
}
