﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    /// <summary>
    /// 继续教育报名人员审核记录
    /// </summary>
    public class Biz_RP_EmployeeCheckedRecord
    {
        [Key]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int CertificateId { get; set; }
        public bool PassStatus { get; set; }
        public bool OperationStatus { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateById { get; set; }
        public string CheckedMark { get; set; }
    }
}
