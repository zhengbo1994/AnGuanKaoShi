using System;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    /// <summary>
    /// 安排人员到考试计划记录
    /// </summary>
    public class Biz_EmployeeForExamPlanRecord
    {
        [Key]
        public int Id { get; set; }
        //准考证号
        public string ExamRegistrationNumber { get; set; }

        public int EmployeeId { get; set; }

        public int ExamPlanRecordId { get; set; }

        public int ExamRoomId { get; set; }

        public bool OperationStatus { get; set; }

        public int CreateById { get; set; }

        public DateTime CreateDate { get; set; }

        public int ExamSeatNumber { get; set; }
    }
}
