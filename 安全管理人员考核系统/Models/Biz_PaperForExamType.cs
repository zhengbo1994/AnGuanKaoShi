using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    public class Biz_PaperForExamType
    {
        [Key]
        public int Id { get; set; }
        public string ExamType { get; set; }
        public string Industry { get; set; }
        public int PaperId { get; set; }
        public int SEQ { get; set; }
        public string PaperType { get; set; }
        //考试时长 分钟 
        public int Duration { get; set; }
        public double PassScore { get; set; }
    }
}
