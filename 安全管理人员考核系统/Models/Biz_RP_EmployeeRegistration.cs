using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Security;

namespace Model
{
    /// <summary>
    /// 继续教育报名人员
    /// </summary>
    public class Biz_RP_EmployeeRegistration
    {

        [Key]
        public int Id { get; set; }
        public string EmployeeName { get; set; }
        public string Sex { get; set; }
        public string  Birthday { get; set; }
        public string IDNumber { get; set; }
        public int EnterpriseId { get; set; }
        public string Job { get; set; }
        public string Title4Technical { get; set; }
        public string ExamType { get; set; }
        public string Industry { get; set; }
        public string OldCertificateNo { get; set; }
        public DateTime StartCertificateDate { get; set; }
        public DateTime EndCertificateDate { get; set; }
        public string City { get; set; }
        public int TrainingInstitutionId { get; set; }
        public string Remark { get; set; }
        //是否打印证书
        public bool PrintCertificate { get; set; }
        public int CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public int SummitById { get; set; }
        public bool SubmitStatus { get; set; }
        public DateTime? SummitDate { get; set; }
        public bool OperationStatus { get; set; }
        public bool Invalid { get; set; } //无效的数据

        [NotMapped]
        public string CurrentWorkFlowStatus { get; set; }

    }
}
