using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using Model;
using BLL;
using Library.baseFn;
using Library.LogFn;

namespace SafetyProfessionalAssessmentSystem.WCF
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“TrainService”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 TrainService.svc 或 TrainService.svc.cs，然后开始调试。
    [LogException]
    public class TrainService : ITrainService
    {
        public void DoWork()
        {
        }
        /// <summary>
        /// 保存在线学习记录
        /// </summary>
        /// <param name="studyByVideoRecoder"></param>
        public void SaveStudyByVideoRecord(StudyByVideoRecoder studyByVideoRecoder)
        {
            LogHelper loghelper = new LogHelper();
            try
            {
                loghelper.WriteInfo(this.GetType(), "参数：" + studyByVideoRecoder.ObjToJSONString());
                IEmployeeCtrl employeeCtrl = new EmployeeCtrl(null);

                Biz_StudyByVideoRecoder newStudyByVideoRecoder = new Biz_StudyByVideoRecoder()
                {
                    VideoName = studyByVideoRecoder.VideoName,
                    Studyhours = studyByVideoRecoder.Studyhours,
                    IDNumber = studyByVideoRecoder.IDNumber
                };
                employeeCtrl.SaveStudyByVideoRecord(newStudyByVideoRecoder);
            }
            catch (Exception ex)
            {
                loghelper.WriteLog(this.GetType(), ex);
            }
        }
        /// <summary>
        /// 保存在线学习完视频
        /// </summary>
        /// <param name="studyByVideoComplete"></param>
        public void SaveStudyByVideoComplete(StudyByVideoComplete studyByVideoComplete)
        {
            LogHelper loghelper = new LogHelper();
            try
            {
                loghelper.WriteInfo(this.GetType(), "参数：" + studyByVideoComplete.ObjToJSONString());
                IEmployeeCtrl employeeCtrl = new EmployeeCtrl(null);

                Biz_StudyByVideoComplete newstudyByVideoComplete = new Biz_StudyByVideoComplete()
                {
                    VideoId = studyByVideoComplete.VideoId,
                    VideoName = studyByVideoComplete.VideoName,
                    IDNumber = studyByVideoComplete.IDNumber,
                    CreateDate = studyByVideoComplete.CreateDate,
                    CreateById = 0,
                    TID = studyByVideoComplete.TID
                    
                };
                employeeCtrl.SaveStudyByVideoComplete(newstudyByVideoComplete);
            }
            catch (Exception ex)
            {
                loghelper.WriteLog(this.GetType(), ex);
            }
        }
        /// <summary>
        /// 保存在线练习记录
        /// </summary>
        /// <param name="onlineExerciseRecord"></param>
        public void SaveExerciseRecord(OnlineExerciseRecord onlineExerciseRecord)
        {
            LogHelper loghelper = new LogHelper();
            try
            {
                loghelper.WriteInfo(this.GetType(), "参数：" + onlineExerciseRecord.ObjToJSONString());
                Biz_OnlineExerciseRecord newOnlineExerciseRecord = new Biz_OnlineExerciseRecord()
                {
                    IDNumber = onlineExerciseRecord.IDNumber,
                    StartDateTime = onlineExerciseRecord.StartDateTime,
                    EndDateTime = onlineExerciseRecord.EndDateTime,
                    Score = onlineExerciseRecord.Score,
                    TID = onlineExerciseRecord.TID
                };
                IEmployeeCtrl employeeCtrl = new EmployeeCtrl(null);
                employeeCtrl.AddOnlineExerciseRecord(newOnlineExerciseRecord);
            }
            catch (Exception ex)
            {
                loghelper.WriteLog(this.GetType(), ex);
            }
        }
        /// <summary>
        /// 保存模拟考试记录
        /// </summary>
        /// <param name="simulatedExamRecord"></param>
        public void SaveSimulatedExamRecord(SimulatedExamRecord simulatedExamRecord)
        {
            LogHelper loghelper = new LogHelper();
            try
            {
                loghelper.WriteInfo(this.GetType(), "参数：" + simulatedExamRecord.ObjToJSONString());
                Biz_SimulatedExamRecord newSimulatedExamRecord = new Biz_SimulatedExamRecord()
                {
                    IDNumber = simulatedExamRecord.IDNumber,
                    StartDateTime = simulatedExamRecord.StartDateTime,
                    EndDateTime = simulatedExamRecord.EndDateTime,
                    Score = simulatedExamRecord.Score,
                    TID = simulatedExamRecord.TID
                };
                IEmployeeCtrl employeeCtrl = new EmployeeCtrl(null);
                employeeCtrl.AddSimulatedExamRecord(newSimulatedExamRecord);
            }
            catch (Exception ex)
            {
                loghelper.WriteLog(this.GetType(), ex);
            }
        }
    }
}
