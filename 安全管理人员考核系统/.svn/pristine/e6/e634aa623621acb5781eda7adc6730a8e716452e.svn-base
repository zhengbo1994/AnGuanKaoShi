using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using Model;

namespace SafetyProfessionalAssessmentSystem.WCF
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“ITrainService”。
    [ServiceContract]
    public interface ITrainService
    {
        [OperationContract]
        void DoWork();
        [OperationContract]
        void SaveStudyByVideoRecord(StudyByVideoRecoder studyByVideoRecoder);
        [OperationContract]
        void SaveStudyByVideoComplete(StudyByVideoComplete studyByVideoComplete);
        [OperationContract]
        void SaveExerciseRecord(OnlineExerciseRecord onlineExerciseRecord);
        [OperationContract]
        void SaveSimulatedExamRecord(SimulatedExamRecord simulatedExamRecord);
    }
}
