using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Model
{
    public class Biz_EmployeeExamResultRecordFile
    {
        [Key]
        public int Id { get; set; }
        //人员考试结果Id
        public int EmployeeExamResultRecordId { get; set; }
        //文件名称
        public string FileKey { get; set; }
        //文件相对路径
        public string FilePath { get; set; }
    }
}
