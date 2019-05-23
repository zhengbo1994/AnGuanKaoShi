using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Web;

namespace SafetyProfessionalAssessmentSystem.WCF
{
    [DataContract(Name = "StudyByVideoRecoder", Namespace = "WCF.TrainService")]
    public class StudyByVideoRecoder
    {
        [DataMember]
        public string VideoName { get; set; }
        [DataMember]
        public double Studyhours { get; set; }
        [DataMember]
        public string IDNumber { get; set; }
    }
}