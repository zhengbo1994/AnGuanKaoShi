using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Model;

namespace BLL
{
    public interface IExamManageCtrl
    {
        #region  考试列表
        List<Biz_EmployeeForExamPlanRecord> GetEmployeeForExamPlanRecordListByExamPlanIdList(List<int> examPlanIdList);
        List<Biz_ExamPlanRecord> GetExamPlanRecordListInExaming(List<string> cityList);
        List<Biz_ExaminationRoom> GetExamRoomByIdList(List<int> IdList);
        List<Biz_ExaminationPoint> GetTrainingInstitutionByRoomIdList(List<int> examRoomIdList);
        #endregion
        #region 准考证
        AdmissionticketInfo GetAdmissionticketInfo(int employeeId, IWorkFlowCtrl workFlowCtrl, IEnterpriseCtrl enterpriseCtrl, IExaminationPointCtrl examinationPointCtrl);
        string CreateNewAdmissionticketPDF(AdmissionticketInfo examInfo, string templateFilePath, string rootFolderPath);
        #endregion
        #region  监考
        List<Biz_Employee> GetEmployeeList(int examPlanId, int examRoomId);
        List<Biz_PaperForExamType> GetPaperForExamTypeList(int employeeId);

        #endregion

        #region 考试计划和考试结果
        Biz_ExamPlanRecord GetExamPlanRecordById(int id);
        List<ExamPlanAbstractInfo> GetPublishedExamPlanList(int pageIndex, int pageSize);
        ExamPlanDetails GetExamPlanDetailsById(int id, int hiddenCharCount);

        List<ExamResultAbstractInfo> GetPublishedExamResultList(int pageIndex, int pageSize);
        ExamResultDetails GetExamResultDetailsById(int id, int hiddenCharCount);
        #endregion

        List<Biz_ExamPlanRecord> ExamPlanPublicity();
        List<Biz_ExamPlanRecord> GetPublishedExamResultList();

    }
}
