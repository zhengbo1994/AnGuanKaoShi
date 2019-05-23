using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Web.Security;

namespace SafetyProfessionalAssessmentSystem.WCF
{
    [DataContract]
    public class EmployeeInfo
    {

        [DataMember]
        public int EmployeeId { get; set; }

        [DataMember]
        public string EmployeeName { get; set; }

        [DataMember]
        public string Sex { get; set; }

        [DataMember]
        public DateTime Birthday { get; set; }

        [DataMember]
        public string IDNumber { get; set; }

        [DataMember]
        public string EnterpriseName { get; set; }

        [DataMember]
        public string Job { get; set; }

        [DataMember]
        public string Title4Technical { get; set; }

        [DataMember]
        public string ExamType { get; set; }

        [DataMember]
        public string Industry { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string ConstructorCertificateNo { get; set; }

        [DataMember]
        public string Remark { get; set; }

        //培训类型(线上/线下/内训)
        [DataMember]
        public string TrainingType { get; set; }

        [DataMember]
        public string TrainingInstitutionName { get; set; }

    }
}
