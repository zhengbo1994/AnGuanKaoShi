using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;

namespace BLL
{
    public interface IWorkFlowCtrl
    {
        #region  流程推进
        void RegisterEmployee(Biz_Employee employee);

        void SummitEmployee(List<int> employeeIdLsit);
        void SaveTrainingRecord(Biz_TrainingRecord trainingRecord);
        void SubmitTrainingRecord(List<int> employeeIdLsit);
        void CheckTrainingRecord(List<int> employeeIdList, string studyTime, string practiceTime, string abilityTestResult, string remark, bool passFlag);
        void CheckEmployeeList(List<int> employeeIdList, bool passFlag, string checkedMark);
        void ReturnEmployeeList(List<int> employeeIdList, string checkedMark);

        //void AssignEmployeeListForCheck(List<int> employeeIdList, int trainingInstitutionId);
        //void AssignEmployeeListForCheck(int employeeCount, int trainingInstitutionId, List<string> cityList);

        //void CheckEmployeeList(List<int> employeeIdList, bool passFlag, string checkedMark);

        //void MakeExamPlan(bool autoAssignEmployee, List<int> employeeIdList, int employeeCount, int TrainingInstitutionIdA, DateTime ExamDateTimeA,
        //    int TrainingInstitutionIdB, DateTime ExamDateTimeB, int TrainingInstitutionIdC, DateTime ExamDateTimeC);

        //void ManageExamResult(int employeeId, string SubjectName, bool ExamResult);

        void SummitExamResult(List<int> employeeIdList);

        void CheckedExamResult(List<int> employeeIdList, bool checkedStatus, string checkedMark, string startCertificateDate);

        void IssuanceCertificate(List<int> employeeIdList, int examPlanRecordId, int trainingInstitutionId, string remark);
        #endregion


        #region 报名
        // 获取取证科目列表
        List<Sys_DropdownListItem> GetEmployeeSubjectList();
        //获取行业列表
        List<Sys_DropdownListItem> GetEmployeeIndustryList();
        // 根据人员ID获取人员信息
        Biz_Employee GetEmployeeInfoById(int employeeId);
        // List<Biz_Employee> GetEmployeeByIdList(List<int> employeeIdList);
        //删除报名人员信息
        void DeleteEmployeeById(int employeeId);
        //更新报名信息
        void UpdateEmployeeInfo(Biz_Employee employee);


        // 根据查询条件 获取人员列表
        List<Biz_Employee> GetEmployeeList(string employeeName, string idNumber, string trainingType, string examType, string industry, string workFlowStatus, int page, int rows, ref int totalCount);

        // 根据查询条件 获取人员IQueryable
        IQueryable<Biz_Employee> getEmployeeQueryableByParam(string employeeName, string IdNumber, string examType, string industry, string workflowStatus,
            string institutionName, string enterpriseName, string socialCreditCode);
        #endregion

        #region 培训记录
        List<Biz_Employee> GetEmployeeList_TrainingRecord(string enterpriseName, string employeeName, string idNumber, string trainingType, string examType, string industry, string trainingStatus, int page, int rows, ref int totalCount);
        List<Biz_TrainingRecord> GetTrainingRecordListByEmployeeIdList(List<int> employeeIdList);

        Biz_TrainingRecord GetTrainingRecord(int employeeId);

        #endregion

        #region 人员审核
        List<Biz_Employee> GetEmployeeList_EmployeeCheck(string enterpriseName, string employeeName, string idNumber,
               string examType, string industry, string checkStatus, string checkDateBegin, string checkDateEnd, string trainingInstitutionName, int page, int rows, ref int totalCount, List<string> cityList);
        List<Biz_EmployeeCheckedRecord> GetEmployeeCheckedRecordListByEmployeeIdList(List<int> employeeIdList);

        #endregion

        #region 获取用户名称
        string GetUserName(int accountId);
        #endregion


        #region 制定考试计划
        List<Biz_Employee> GetEmployeeListByEmployeeIdList(List<int> employeeIdList);
        //取考试计划流水号
        string GetExamPlanNumber(string cityName);
        int GetNotInExamPlanEmployeeCountByCityList(List<string> cityList);

        void MakeExamPlanAndAutoAssign(Biz_ExamPlanRecord examPlan, int trainingInstitutionId, int autoAssignCount, string cityName);
        void MakeExamPlanAndManualAssign(Biz_ExamPlanRecord examPlan, int ExamRoomId, List<int> employeeIdList);
        List<Biz_Employee> GetMakeExamPlanEmployeeList(string examPlanNumber, int trainingInstitutionId, int examRoomId, string conditionStr, int page, int rows, ref int totalCount, List<string> cityList);
        List<Biz_EmployeeForExamPlanRecord> GetEmployeeForExamPlanRecordByEmployeeIdList(List<int> employeeIdList);
        List<Biz_EmployeeForExamPlanRecord> GetEmployeeForExamPlanRecordByExamPlanIdList(List<int> examPlanIdList);
        List<Biz_EmployeeForExamPlanRecord> GetEmployeeForExamPlanRecordByExamPlanId(int examPlanId);
        List<Biz_ExamPlanRecord> GetExamPlanRecord(string examPlanNumber, string examDatetimeBegin, string examDatetimeEnd, string traningInstitutionName, string employeeName, string iDNumber, string submitStatus, int page, int rows, ref int totalCount, List<string> cityList);

        List<Biz_Employee> GetEmployeeForExamPlanRecord(string examPlanNumber, string traningInstitutionName, string employeeName, string iDNumber, string submitStatus, int page, int rows, ref int totalCount, List<string> cityList);
        void SubmitExamPlan(int examPlanId, int examCoreExamId);
        Biz_ExamPlanRecord GetExamPlanRecordById(int examPlanId);
        List<Biz_ExaminationRoom> GetExamRoomByExamPlanId(int examPlanId);
        Biz_ExamPlanRecord GetExamPlanRecord(int examPlanId, int employeeId);
        Biz_ExamPlanRecord GetExamPlanRecordByExamCoreExamId(int examCoreExamId, int employeeId);
        List<Biz_ExamPlanRecord> GetExamPlanRecordByIdList(List<int> idList);
        //根据考试计划Id获取paperId
        List<Biz_PaperForExamType> GetPaperForExamTypeByExamPlanId(int examPlanId);
        #endregion

        #region 开始考试
        //获取离现在最近的一次考试 已考试结束时间为准
        Biz_ExamPlanRecord GetFirstExamPlanByEmployeeId(int employeeId);
        //根据考试计划Id和人员Id 获取考场信息
        Biz_ExaminationRoom GetExamRoom(int examPlanId, int employeeId);
        //获取待完成的试卷Id
        Biz_PaperForExamType GetPaperIdForStartExam(int examPlanId, int employeeId);
        Biz_PaperForExamType GetPaper(int employeeId, string paperType);
        void SaveExamResult(int examCoreExamId, string idNumber, int paperId, bool examPassedFlag, double score);
        //获取考试结果
        Biz_EmployeeExamResultRecord GetEmployeeExamResultRecord(int examCoreExamId, int employeeId);

        Biz_Employee GetEmployee(int examCoreExamId, string idNumber);

        #endregion

        #region 管理考试结果
        List<Biz_Employee> EmployeeForExamResultList(string examPlanNumber, string employeeName, string iDNumber, string industry, string examType, string submitStatus, int page, int rows, ref int totalCount);
        List<Biz_EmployeeExamResultRecord> GetEmployeeExamResultRecordList(List<int> employeeIdList);

        List<Biz_EmployeeExamResultRecordFile> GetEmployeeExamResultRecordFileListByEmployeeExamResultRecordId(int employeeExamResultRecordId);
        Biz_EmployeeExamResultRecordFile GetEmployeeExamResultImgFileById(int Id);
        Biz_EmployeeExamResultRecord GetEmployeeExamResultRecord(int employeeId);
        int SaveEmployeeExamResult(Biz_EmployeeExamResultRecord employeeExamResultRecord);
        void SaveEmployeeExamResultFile(Biz_EmployeeExamResultRecordFile employeeExamResultRecordFile);
        void DeleteEmployeeExamResultFile(int id);
        #endregion

        #region 审核考试结果
        List<Biz_ExamPlanRecord> GetExamPlanRecordInCheckExamResult(string examPlanNumber, string employeeName, string iDNumber, string industry, string examType, string checkStatus, int page, int rows, ref int totalCount, List<string> cityList);
        List<Biz_EmployeeExamResultRecord> GetEmployeeExamResultRecordByExamPlanIdList(List<int> examPlanIdList, bool? submitStatus);
        List<Biz_Employee> GetEmployeeListInExamResultCheck(int examPlanId, string employeeName, string iDNumber, string industry, string examType, string checkStatus, int page, int rows, ref int totalCount);
        List<Biz_EmployeeExamResultCheckedRecord> GetEmployeeExamResultCheckedRecordListByEmployeeIdList(List<int> employeeIdList, bool? checkedStatus);
        void CheckByEmployee(int employeeId, bool passStatus, string checkedMark, string startCertificateDate);
        void CheckByExamPlan(int examPlanId, bool passStatus, string checkedMark, string startCertificateDate);
        #endregion

        #region 证书打印
        List<Biz_Employee> GetEmployeeListInPrintCertificate(string employeeName, string idNumber, string examType, string industry, string examPlanNumber, string enterpriseName, string trainingInstutionName, bool? IsPrinted);
        List<Biz_Certificate> GetEmployeeListInPrintCertificate(string employeeName, string idNumber, string examType, string industry, string examPlanNumber, string enterpriseName, string trainingInstutionName, bool? IsPrinted, int page, int rows, ref int totalCount);
        List<Biz_Certificate> GetCertificateList(List<Biz_Employee> employeeList);
        List<Biz_CertificatePrintRecord> GetCertificatePrintRecordList(List<string> certificateNoList);
        void SaveCertificatePrintRecord(Biz_CertificatePrintRecord certificatePrintRecord);
        Biz_Certificate GetCertificateByEmployeeId(int employeeId);
        #endregion

        #region 证书发放
        //获取需要发放证书的人
        List<Biz_Employee> GetEmployeeListInCertificateIssuance(List<int> employeeIdList);
        List<Biz_ExamPlanRecord> getExamPlanListInCertificateIssuance(string examPlanNumber, string trainingInsititutionName, string issuanceStatus, string issuanceDateTimeBegin, string issuanceDateTimeEnd, int page, int rows, ref int totalcount, List<string> cityList);
        List<Biz_EmployeeCertificateIssuanceRecord> GetEmployeeCertificateIssuanceRecordList(List<int> employeeIdList);
        //根据计划和学校发放
        void IssuanceByExamPlan(int examPlanId, int trainingInsititutionId, string remark);
        //单个人员发放
        void IssuanceByEmployee(int employeeId, int examPlanId, int trainingInsititutionId, string remark);
        #endregion

        #region 获取人员对应的流程状态
        WorkFlowStatus GetCurrentWorkFlowStatus(int employeeId);
        List<WorkFlowStatus> GetCurrentWorkFlowStatusByEmployeeIdList(List<int> employeeIdList);

        #endregion


        #region 考核点查看分配给自己的考试计划部分
        List<Biz_ExamPlanRecord> GetExamPlanRecordListByTrainingInstitutionId(int trainingInstitutionId);
        List<Biz_ExaminationRoom> GetExaminationRoomListByTrainingInstitutionId(int trainingInstitutionId);

        Biz_Employee GetEmployeeForExamRoom(string idCardNumber, int examPlanId, int examRoomId);

        #endregion

        #region 考试计划提醒
        List<Biz_ExamPlanRecord> GetExamPlanListForRemind();
        Biz_ExamPlanRecord GetExamPlanByExamPlanNumber(string examPlanNumber);

        #endregion


    }
}
