using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Model;
using Library.baseFn;
using DAL;

namespace BLL
{
    public class StatisticalReportCtrl : IStatisticalReportCtrl
    {
        private Uow uow;
        private Sys_Account account;

        public StatisticalReportCtrl(Sys_Account account)
        {
            if (uow == null)
            {
                uow = new Uow();
            }
            this.account = account;

        }

        #region 人员信息查询
        public List<Biz_Employee> GetEmployeeList(string employeeName, string idNumber, string enterpriseName, string examType, string industry, string workFlowStatus, int page, int rows, ref int totalCount, List<string> cityList)
        {
            //IQueryable<Biz_Employee> employeeQueryable = uow.Biz_Employee.GetAll().Where(p => !p.Invalid);

            IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);

            IQueryable<Biz_Employee> employeeQueryable = workFlowCtrl.getEmployeeQueryableByParam(employeeName, idNumber, examType, industry, workFlowStatus, null, enterpriseName, null);

            #region 数据权限
            string roleTypeStr = account.RoleList.FirstOrDefault().RoleType;
            if (RoleCtrl.RoleType.Enterprise.ToLower() == roleTypeStr.ToLower())
            {
                //数据权限  各企业看自己的人员
                employeeQueryable = employeeQueryable.Where(p => p.EnterpriseId == account.UserId);
            }
            else if (RoleCtrl.RoleType.Manager.ToLower() == roleTypeStr.ToLower())//管理部门 看城市的人
            {
                employeeQueryable = employeeQueryable.Where(p => cityList.Contains(p.City));
            }
            else if (RoleCtrl.RoleType.TrainingInstitution.ToLower() == roleTypeStr.ToLower())//考核点看分配到自己的人
            {
                //资料审核 分配记录
                //IQueryable<Biz_EmployeeAssignForCheckRecord> employeeAssignForCheckRecordQueryable = uow.Biz_EmployeeAssignForCheckRecord.GetAll().Where(p => p.TrainingInstitutionId == account.UserId);
                //参加本培训机构 培训的人员
                //考试计划分配记录
                IQueryable<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordQueryable = uow.Biz_EmployeeForExamPlanRecord.GetAll().Where(p => uow.Biz_ExaminationRoom.GetAll().Where(q => q.ExaminationPointId == account.UserId).Select(e => e.Id).Contains(p.ExamRoomId));
                employeeQueryable = employeeQueryable.Where(p => employeeForExamPlanRecordQueryable.Select(q => q.EmployeeId).Contains(p.Id) || p.TrainingInstitutionId == account.UserId);
            }
            else if (RoleCtrl.RoleType.Employee.ToLower() == roleTypeStr.ToLower())//个人看自己
            {
                employeeQueryable = employeeQueryable.Where(p => p.Id == account.UserId);
            }
            else if (RoleCtrl.RoleType.Admin.ToLower() == roleTypeStr.ToLower() || RoleCtrl.RoleType.Master.ToLower() == roleTypeStr.ToLower())//管理员和省总站 不做限制
            {
                //不显示测试人员数据
                employeeQueryable = employeeQueryable.Where(p => p.City != "测试市");
            }
            else//其他角色 啥也看不到
            {
                employeeQueryable = employeeQueryable.Where(p => (1 == 2));
            }
            #endregion
            //if (!enterpriseName.IsNull())
            //{
            //    IQueryable<Biz_Enterprise> enterpriseQueryable = uow.Biz_Enterprise.GetAll();
            //    enterpriseQueryable = enterpriseQueryable.Where(p => p.EnterpriseName.Contains(enterpriseName));
            //    employeeQueryable = employeeQueryable.Where(p => enterpriseQueryable.Select(q => q.Id).Contains(p.EnterpriseId));
            //}
            //if (!employeeName.IsNull())
            //{
            //    employeeQueryable = employeeQueryable.Where(p => p.EmployeeName.Contains(employeeName));
            //}
            //if (!idNumber.IsNull())
            //{
            //    employeeQueryable = employeeQueryable.Where(p => p.IDNumber == idNumber);
            //}
            //if (!examType.IsNull())
            //{
            //    employeeQueryable = employeeQueryable.Where(p => p.ExamType == examType);
            //}
            //if (!industry.IsNull())
            //{
            //    employeeQueryable = employeeQueryable.Where(p => p.Industry == industry);
            //}
            //if (!workFlowStatus.IsNull())
            //{
            //    IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
            //    List<WorkFlowStatus> WorkFlowStatusList = workFlowCtrl.GetCurrentWorkFlowStatusByEmployeeIdList(employeeQueryable.Select(p => p.Id).ToList()).ToList();
            //    WorkFlowStatusList = WorkFlowStatusList.Where(p => p.WorkFlowStatusTag == workFlowStatus).ToList();
            //    List<int> employeeIdList = WorkFlowStatusList.Select(p => p.employeeId).ToList();
            //    employeeQueryable = employeeQueryable.Where(p => employeeIdList.Contains(p.Id));
            //}
            totalCount = employeeQueryable.Count();
            int indexBegin = (page - 1) * rows;
            List<Biz_Employee> employeeList = employeeQueryable.OrderByDescending(P => P.Id).Skip(indexBegin).Take(rows).ToList();
            return employeeList;
        }
        public Biz_Employee GetEmployeeById(int id)
        {
            return uow.Biz_Employee.GetById(id);
        }

        public Biz_TrainingRecord GetTrainingRecord(int employeeId)
        {
            return uow.Biz_TrainingRecord.GetAll().Where(p => p.EmployeeId == employeeId).OrderByDescending(p => p.CreateDate).FirstOrDefault();
        }
        //public Biz_EmployeeAssignForCheckRecord GetEmployeeAssignForCheckRecordByEmployeeId(int employeeId)
        //{
        //    return uow.Biz_EmployeeAssignForCheckRecord.GetAll().Where(p => p.EmployeeId == employeeId).FirstOrDefault();
        //}
        public Biz_EmployeeCheckedRecord GetEmployeeCheckedRecordByEmployeeId(int employeeId)
        {
            return uow.Biz_EmployeeCheckedRecord.GetAll().Where(p => p.EmployeeId == employeeId).FirstOrDefault();
        }
        public Biz_EmployeeForExamPlanRecord GetEmployeeForExamPlanRecordByEmployeeId(int employeeId)
        {
            return uow.Biz_EmployeeForExamPlanRecord.GetAll().Where(p => p.EmployeeId == employeeId).FirstOrDefault();
        }
        public Biz_ExamPlanRecord GetExamPlanRecordByExamPlanId(int examPlanId)
        {
            return uow.Biz_ExamPlanRecord.GetById(examPlanId);
        }
        public Biz_EmployeeExamResultRecord GetEmployeeExamResultRecordByEmployeeId(int employeeId)
        {
            return uow.Biz_EmployeeExamResultRecord.GetAll().Where(p => p.EmployeeId == employeeId).FirstOrDefault();
        }
        public Biz_EmployeeExamResultCheckedRecord GetEmployeeExamResultCheckedRecordByEmployeeId(int employeeId)
        {
            return uow.Biz_EmployeeExamResultCheckedRecord.GetAll().Where(p => p.EmployeeId == employeeId).FirstOrDefault();
        }
        public Biz_EmployeeCertificateIssuanceRecord GetEmployeeCertificateIssuanceRecordByEmployeeId(int employeeId)
        {
            return uow.Biz_EmployeeCertificateIssuanceRecord.GetAll().Where(p => p.EmployeeId == employeeId).FirstOrDefault();
        }
        public string GetUserName(int accountId)
        {
            IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(this.account);
            return workFlowCtrl.GetUserName(accountId);
        }
        #endregion

        #region 考试计划查询

        public List<Biz_ExamPlanRecord> GetExamPlanRecord(string examPlanNumber, string examDatetimeBegin, string examDatetimeEnd, string traningInstitutionName, string employeeName, string iDNumber, string examStatus, int page, int rows, ref int totalCount, List<string> cityList)
        {
            IQueryable<Biz_ExamPlanRecord> examPlanRecordQueryable = uow.Biz_ExamPlanRecord.GetAll();
            IQueryable<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordQueryable = uow.Biz_EmployeeForExamPlanRecord.GetAll();
            IQueryable<Biz_Employee> employeeQueryable = uow.Biz_Employee.GetAll();
            //数据权限
            string roleType = account.RoleList.FirstOrDefault().RoleType;
            if (roleType.ToLower() == RoleCtrl.RoleType.Manager.ToLower())//管理部门  看本角色下所有账户创建的计划
            {
                Sys_Role accountRole = account.RoleList.FirstOrDefault();
                List<int> accountIdList = uow.Sys_RelAccountRole.GetAll().Where(p => p.RoleId == accountRole.Id).Select(p => p.AccountId).ToList();
                examPlanRecordQueryable = examPlanRecordQueryable.Where(p => accountIdList.Contains(p.CreateById));
            }
            else if (roleType.ToLower() == RoleCtrl.RoleType.Master.ToLower() || roleType.ToLower() == RoleCtrl.RoleType.Admin.ToLower())//省总站和管理员看所有
            {
                //不显示测试账号的考试计划
                List<int> accountIdList = uow.Sys_Account.GetAll().Where(p => p.AccountName.Contains("test")).Select(p => p.Id).ToList();
                examPlanRecordQueryable = examPlanRecordQueryable.Where(p => !accountIdList.Contains(p.CreateById));
            }
            else if (roleType.ToLower() == RoleCtrl.RoleType.ExaminationPoint.ToLower())//考核点看安排到自己的考试计划
            {
                IQueryable<Biz_ExaminationRoom> examinationRoomQueryable = uow.Biz_ExaminationRoom.GetAll().Where(R => R.ExaminationPointId == account.UserId);
                employeeForExamPlanRecordQueryable = employeeForExamPlanRecordQueryable.Where(P => examinationRoomQueryable.Select(e => e.Id).Contains(P.ExamRoomId));
            }
            else if (roleType.ToLower() == RoleCtrl.RoleType.Enterprise.ToLower())//企业看与本企业有关的考试计划
            {
                employeeQueryable = employeeQueryable.Where(p => p.EnterpriseId == account.UserId);
            }
            else if (roleType.ToLower() == RoleCtrl.RoleType.Employee.ToLower())//个人看与自己相关的考试计划
            {
                employeeQueryable = employeeQueryable.Where(p => p.IDNumber == account.AccountName);
            }
            else//其他人 啥也看不到
            {
                examPlanRecordQueryable = examPlanRecordQueryable.Where(p => 1 == 2);
            }

            if (!examPlanNumber.IsNull())
            {
                examPlanRecordQueryable = examPlanRecordQueryable.Where(p => p.ExamPlanNumber.Contains(examPlanNumber));
            }
            if (!examDatetimeBegin.IsNull())
            {
                DateTime dtBegin = examDatetimeBegin.ConvertToDateTime();
                examPlanRecordQueryable = examPlanRecordQueryable.Where(p => dtBegin <= p.ExamDateTimeBegin);
            }
            if (!examDatetimeEnd.IsNull())
            {
                DateTime dtEnd = examDatetimeEnd.ConvertToDateTime();
                examPlanRecordQueryable = examPlanRecordQueryable.Where(p => p.ExamDateTimeEnd <= dtEnd);
            }

            if (examStatus == "未提交")
            {
                examPlanRecordQueryable = examPlanRecordQueryable.Where(p => p.SubmitStatus == false);
            }
            if (examStatus == "待考试")
            {
                examPlanRecordQueryable = examPlanRecordQueryable.Where(p => p.SubmitStatus == true).Where(p => p.ExamDateTimeBegin > DateTime.Now);
            }
            if (examStatus == "正在考试")
            {
                examPlanRecordQueryable = examPlanRecordQueryable.Where(p => p.SubmitStatus == true).Where(p => p.ExamDateTimeBegin <= DateTime.Now).Where(p => p.ExamDateTimeEnd >= DateTime.Now);
            }
            if (examStatus == "已结束")
            {
                examPlanRecordQueryable = examPlanRecordQueryable.Where(p => p.SubmitStatus == true).Where(p => p.ExamDateTimeEnd < DateTime.Now);
            }
            if (!traningInstitutionName.IsNull())
            {
                IQueryable<Biz_ExaminationPoint> trainingInstitutionQueryable = uow.Biz_ExaminationPoint.GetAll().Where(t => t.InstitutionName.Contains(traningInstitutionName));
                List<Biz_ExaminationRoom> examinationRoomList = uow.Biz_ExaminationRoom.GetAll().Where(R => trainingInstitutionQueryable.Select(t => t.Id).Contains(R.ExaminationPointId)).ToList();
                List<int> roomIdList = examinationRoomList.Select(e => e.Id).ToList();
                employeeForExamPlanRecordQueryable = employeeForExamPlanRecordQueryable.Where(P => roomIdList.Contains(P.ExamRoomId));
            }
            if (!employeeName.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.EmployeeName.Contains(employeeName));
            }
            if (!iDNumber.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.IDNumber.Contains(iDNumber));
            }
            employeeForExamPlanRecordQueryable = employeeForExamPlanRecordQueryable.Where(P => employeeQueryable.Select(e => e.Id).Contains(P.EmployeeId));
            examPlanRecordQueryable = examPlanRecordQueryable.Where(p => employeeForExamPlanRecordQueryable.Select(q => q.ExamPlanRecordId).Contains(p.Id));

            totalCount = examPlanRecordQueryable.Count();
            int indexBegin = (page - 1) * rows;
            examPlanRecordQueryable = examPlanRecordQueryable.OrderByDescending(P => P.ExamDateTimeBegin).Skip(indexBegin).Take(rows);

            List<Biz_ExamPlanRecord> examPlanRecordLise = examPlanRecordQueryable.ToList();
            return examPlanRecordLise;

        }
        #endregion

     
        //考试结果查询
        public Biz_EmployeeExamResultRecord ExamResultSearch(string iDNumber)
        {
            IEmployeeCtrl employeeCtrl = new EmployeeCtrl(account);
            //相关的人员ID
            // List<int> employeeIdList = uow.Biz_Employee.GetAll().Where(p => p.IDNumber == iDNumber).OrderByDescending(p => p.CreateDate).Take(1).Select(p => p.Id).ToList();
            IQueryable<Biz_Employee> employeeQueryable = uow.Biz_Employee.GetAll();
            //所有这个身份证 已经审核通过的考试结果
            IQueryable<Biz_EmployeeExamResultCheckedRecord> EmployeeExamResultCheckedRecord =
                uow.Biz_EmployeeExamResultCheckedRecord.GetAll().Where(p => p.CheckedStatus == true)
                .Where(p => employeeQueryable.Where(q => q.IDNumber == iDNumber).Select(q => q.Id).Contains(p.EmployeeId));
            //在考试成绩已经审核通过里  最新的一次考试成绩
            Biz_EmployeeExamResultRecord examresult = uow.Biz_ExamPlanRecord.GetAll()
                 .Join(EmployeeExamResultCheckedRecord, a => a.Id, b => b.ExamPlanRecordId, (a, b) => new { a, b })
                 .Join(uow.Biz_EmployeeExamResultRecord.GetAll(), o => new { o.b.EmployeeId, o.b.ExamPlanRecordId }, c => new { c.EmployeeId, c.ExamPlanRecordId }, (o, c) => new { o.a, o.b, c })
                 .OrderByDescending(o => o.a.ExamDateTimeBegin).Select(o => o.c).FirstOrDefault();

            //Biz_EmployeeExamResultRecord examresult = employeeCtrl.GetEmployeeExamResultRecordListByEmployeeId(employeeIdList).FirstOrDefault();
            return examresult;

        }



    }
}
