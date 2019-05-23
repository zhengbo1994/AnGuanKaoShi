using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class Biz_EmployeeFile
    {
        [Key]
        public int Id { get; set; }
        //人员编号
        public int EmployeeId { get; set; }
        //文件类型
        public string FileType { get; set; }
        //文件名称
        public string FileName { get; set; }
        //文件相对路径
        public string FilePath { get; set; }
    }
}
