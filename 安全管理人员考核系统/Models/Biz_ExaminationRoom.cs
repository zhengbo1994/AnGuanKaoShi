using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Model
{
    /// <summary>
    /// 考场
    /// </summary>
    public class Biz_ExaminationRoom
    {
        [Key]
        public int Id { get; set; }
        public String ExamRoomName { get; set; }
        public int PersonCount { get; set; }
        public DateTime CreatDate { get; set; }
        public int CreatById { get; set; }

        public int Sequence { get; set; }
        public bool Enabled { get; set; }
        public string Remark { get; set; }
        public int ExaminationPointId { get; set; }

    }
}
