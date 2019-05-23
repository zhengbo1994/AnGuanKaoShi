using System;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    /// <summary>
    /// 人员考试结果审核记录
    /// </summary>
    public class Biz_EmployeeExamResultCheckedRecord
    {
        [Key]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int ExamPlanRecordId { get; set; }
        public bool CheckedStatus { get; set; }
        public bool OperationStatus { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateById { get; set; }
        public string CheckedMark { get; set; }
    }
}
