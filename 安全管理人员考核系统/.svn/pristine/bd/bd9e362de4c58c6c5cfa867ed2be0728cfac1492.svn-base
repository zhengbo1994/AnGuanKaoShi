using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    /// <summary>
    /// 考试计划记录
    /// </summary>
    public class Biz_ExamPlanRecord
    {
        [Key]
        public int Id { get; set; }
        //考试核心 考试Id
        public int ExamCoreExamId { get; set; }
        public string ExamPlanNumber { get; set; }
        public DateTime ExamDateTimeBegin { get; set; }
        [NotMapped]
        public DateTime AExamDateTimeEnd { get; set; }
        [NotMapped]
        public DateTime BExamDateTimeBegin { get; set; }
        public DateTime ExamDateTimeEnd { get; set; }
        public int SafetyKnowledgePassMark { get; set; }
        public int ManagementAbilityPassMark { get; set; }
        public bool SubmitStatus { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateById { get; set; }
        public int? SubmitById { get; set; }
        public DateTime? SubmitDate { get; set; }

    }
}
