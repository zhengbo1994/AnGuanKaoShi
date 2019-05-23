﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Model
{
    /// <summary>
    /// 培训记录
    /// </summary>
    public class Biz_TrainingRecord
    {
        [Key]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string StudyTime { get; set; }
        public string PracticeTime { get; set; }
        public string AbilityTestResult { get; set; }
        public string Remark { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateById { get; set; }
        public bool SubmitStatus { get; set; }
        public bool PassStatus { get; set; }
        public bool OperationStatus { get; set; }
        public DateTime? SubmitDate { get; set; }
        public int SubmitById { get; set; }
    }
}
