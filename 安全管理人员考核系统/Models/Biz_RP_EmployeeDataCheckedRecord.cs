﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    /// <summary>
    /// 继续教育报名人员资料审核记录
    /// </summary>
    public class Biz_RP_EmployeeDataCheckedRecord
    {
        [Key]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int CertificateId { get; set; }
        public bool InValidityDate { get; set; }
        public bool AnnualSafetyTraining { get; set; }
        public bool NotBadBehavior { get; set; }
        public bool TrainingWith24Hours { get; set; }
        public bool DelayConditions { get; set; }
        public bool PassStatus { get; set; }
        public bool OperationStatus { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateById { get; set; }
        public string CheckedMark { get; set; }
    }
}
