using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Model
{
    public class Biz_StudyByVideoRecoder
    {
        [Key]
        public int Id { get; set; }
        public int TID { get; set; }//E测评的记录ID
        public string VideoName { get; set; }
        public double Studyhours { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int EmployeeId { get; set; }
        public string IDNumber { get; set; }
        public string SubjectName { get; set; }
    }
}
