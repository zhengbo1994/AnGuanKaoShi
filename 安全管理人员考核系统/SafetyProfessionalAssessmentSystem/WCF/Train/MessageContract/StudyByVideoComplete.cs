using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SafetyProfessionalAssessmentSystem.WCF
{
    [DataContract(Name = "StudyByVideoComplete", Namespace = "WCF.TrainService")]
    public class StudyByVideoComplete
    {
        [DataMember]
        public int TID { get; set; }
        [DataMember]
        public int VideoId { get; set; }
        [DataMember]
        public string VideoName { get; set; }
        [DataMember]
        public string IDNumber { get; set; }
        [DataMember]
        public DateTime CreateDate { get; set; }
    }
}
