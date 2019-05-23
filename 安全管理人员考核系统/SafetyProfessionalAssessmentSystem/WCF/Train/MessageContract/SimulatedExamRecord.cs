using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Web;

namespace SafetyProfessionalAssessmentSystem.WCF
{
    [DataContract(Name = "SimulatedExamRecord", Namespace = "WCF.TrainService")]
    public class SimulatedExamRecord
    {
        [DataMember]
        public int TID { get; set; }
        [DataMember]
        public string IDNumber { get; set; }
        [DataMember]
        public DateTime StartDateTime { get; set; }
        [DataMember]
        public DateTime EndDateTime { get; set; }
        [DataMember]
        public double Score { get; set; }
    }
}