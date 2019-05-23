﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Model
{
    public class Biz_Certificate
    {
        [Key]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Sex { get; set; }
        public string Birthday { get; set; }
        public string IDNumber { get; set; }
        public string EnterpriseName { get; set; }
        public string Job { get; set; }
        public string Title4Technical { get; set; }
        public string CertificateNo { get; set; }
        public string ExamType { get; set; }
        public string Industry { get; set; }
        public DateTime StartCertificateDate { get; set; }
        public DateTime EndCertificateDate { get; set; }
        public string PhotoPath { get; set; }
        //是否有效数据
        public bool Invalid { get; set; }
        //是否导入
        public bool Import { get; set; }
    }
}
