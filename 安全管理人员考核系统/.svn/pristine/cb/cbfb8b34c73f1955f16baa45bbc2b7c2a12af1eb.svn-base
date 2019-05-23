using System;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    /// <summary>
    /// 人员审核记录
    /// </summary>
    public class Biz_EmployeeCheckedRecord
    {
        [Key]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public bool PassStatus { get; set; }
        public bool OperationStatus { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateById { get; set; }
        public string CheckedMark { get; set; }
    }
}
