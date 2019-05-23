using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    /// <summary>
    /// 人员考试结果记录
    /// </summary>
    public class Biz_EmployeeExamResultRecord
    {
        [Key]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int ExamPlanRecordId { get; set; }

        public double? SafetyKnowledgeExamScore { get; set; }

        public double? ManagementAbilityExamScore { get; set; }
        //实操考试结果
        public bool? FieldExamResult { get; set; }
        public bool OperationStatus { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateById { get; set; }
        public bool SummitStatus { get; set; }
        public DateTime? SummitDate { get; set; }
        public int? SummitById { get; set; }

        [NotMapped]
        public string SafetyKnowledgeExamResult { get; set; }

        [NotMapped]
        public string ManagementAbilityExamResult { get; set; }

        [NotMapped]
        public string ActualOperationExamResult { get; set; }

        [NotMapped]
        public string FinalExamResult { get; set; }


    }
}
