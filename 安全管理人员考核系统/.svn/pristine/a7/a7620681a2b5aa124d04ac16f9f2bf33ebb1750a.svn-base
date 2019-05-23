using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Model;
using Library.baseFn;

namespace BLL
{
    public interface IStatisticalReportCtrl
    {
        #region 人员信息查询
        List<Biz_Employee> GetEmployeeList(string employeeName, string idNumber, string enterpriseName, string examType, string industry, string workFlowStatus, int page, int rows, ref int totalCount, List<string> cityList);
        Biz_Employee GetEmployeeById(int id);
        // Biz_EmployeeAssignForCheckRecord GetEmployeeAssignForCheckRecordByEmployeeId(int employeeId);
        Biz_TrainingRecord GetTrainingRecord(int employeeId);
        Biz_EmployeeCheckedRecord GetEmployeeCheckedRecordByEmployeeId(int employeeId);
        Biz_EmployeeForExamPlanRecord GetEmployeeForExamPlanRecordByEmployeeId(int employeeId);
        Biz_ExamPlanRecord GetExamPlanRecordByExamPlanId(int examPlanId);
        Biz_EmployeeExamResultRecord GetEmployeeExamResultRecordByEmployeeId(int employeeId);
        Biz_EmployeeExamResultCheckedRecord GetEmployeeExamResultCheckedRecordByEmployeeId(int employeeId);
        Biz_EmployeeCertificateIssuanceRecord GetEmployeeCertificateIssuanceRecordByEmployeeId(int employeeId);
        string GetUserName(int accountId);
        #endregion

        #region 考试计划信息查询
        List<Biz_ExamPlanRecord> GetExamPlanRecord(string examPlanNumber, string examDatetimeBegin, string examDatetimeEnd, string traningInstitutionName, string employeeName, string iDNumber, string examStatus, int page, int rows, ref int totalCount, List<string> cityList);
        #endregion
        //考试结果查询
        Biz_EmployeeExamResultRecord ExamResultSearch(string iDNumber);
      
    }
}
