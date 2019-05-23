﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Model;
using DAL;
using Library.baseFn;
using EntityFramework.Extensions;
using System.Globalization;

namespace BLL
{

    public class WorkFlowCtrl : IWorkFlowCtrl
    {
        private Uow uow;
        private Sys_Account account;

        private const string SAFETYKNOWLEDGEEXAM = "SafetyKnowledgeExam";
        private const string MANAGEMENTABILITYEXAM = "ManagementAbilityExam";

        public WorkFlowCtrl(Sys_Account account)
        {
            if (uow == null)
            {
                uow = new Uow();
            }
            this.account = account;

        }

        #region 流程推进
        public void RegisterEmployee(Biz_Employee employee)
        {
            VerifyIDNumber(employee);
            VerifyConstructorCertificateNo(employee);
            uow.Biz_Employee.Add(employee);
            uow.Commit();
        }
        private void VerifyIDNumber(Biz_Employee employee)
        {
            IQueryable<Biz_Employee> employeeExistQueryable = uow.Biz_Employee.GetAll().Where(p => !p.Invalid).Where(p => p.IDNumber == employee.IDNumber).Where(p => p.Id != employee.Id);
            int employeeExistCount = employeeExistQueryable.Count();
            if (employeeExistCount > 0)
            {
                IQueryable<Biz_ExamPlanRecord> examPlanRecordQueryable = uow.Biz_ExamPlanRecord.GetAll();
                IQueryable<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordQueryable = uow.Biz_EmployeeForExamPlanRecord.GetAll();
                //不能报名的情况 1.未安排考试 2.安排了考试计划且未提交 3.安排考试计划提交且考试时间未过
                //1.未安排考试
                IQueryable<Biz_Employee> employeeIdList_NoExam = employeeExistQueryable.Where(p => !employeeForExamPlanRecordQueryable.Select(q => q.EmployeeId).Contains(p.Id));
                //2.安排了考试计划且未提交
                IQueryable<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecord_NoSubmit = employeeForExamPlanRecordQueryable.Where(p => examPlanRecordQueryable.Where(q => q.SubmitStatus == false).Select(q => q.Id).Contains(p.ExamPlanRecordId));
                IQueryable<Biz_Employee> employeeIdList_InExamNoSubmit = employeeExistQueryable.Where(p => employeeForExamPlanRecord_NoSubmit.Select(q => q.EmployeeId).Contains(p.Id));
                //3.安排考试计划提交且考试时间未过
                IQueryable<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecord_Submit = employeeForExamPlanRecordQueryable.Where(p => examPlanRecordQueryable.Where(q => q.SubmitStatus).Where(q => DateTime.Compare(q.ExamDateTimeEnd, DateTime.Now) > 0).Select(q => q.Id).Contains(p.ExamPlanRecordId));
                IQueryable<Biz_Employee> employeeIdList_InExamSubmit = employeeExistQueryable.Where(p => employeeForExamPlanRecord_Submit.Select(q => q.EmployeeId).Contains(p.Id));

                if (employeeIdList_NoExam.Count() > 0 || employeeIdList_InExamNoSubmit.Count() > 0 || employeeIdList_InExamSubmit.Count() > 0)
                {
                    throw new Exception("该取证人员已经报名");
                }
            }
        }
        //验证建造师证书编号 不能重复
        private void VerifyConstructorCertificateNo(Biz_Employee employee)
        {
            if (employee.ConstructorCertificateNo.IsNull())
            {
                return;
            }
            IQueryable<Biz_Employee> employeeExistQueryable = uow.Biz_Employee.GetAll().Where(p => !p.Invalid).Where(p => p.ConstructorCertificateNo == employee.ConstructorCertificateNo).Where(p => p.Id != employee.Id);
            int employeeExistCount = employeeExistQueryable.Count();
            if (employeeExistCount > 0)
            {
                IQueryable<Biz_ExamPlanRecord> examPlanRecordQueryable = uow.Biz_ExamPlanRecord.GetAll();
                IQueryable<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordQueryable = uow.Biz_EmployeeForExamPlanRecord.GetAll();
                //不能报名的情况 1.未安排考试 2.安排了考试计划且未提交 3.安排考试计划提交且考试时间未过
                //1.未安排考试
                IQueryable<Biz_Employee> employeeIdList_NoExam = employeeExistQueryable.Where(p => !employeeForExamPlanRecordQueryable.Select(q => q.EmployeeId).Contains(p.Id));
                //2.安排了考试计划且未提交
                IQueryable<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecord_NoSubmit = employeeForExamPlanRecordQueryable.Where(p => examPlanRecordQueryable.Where(q => q.SubmitStatus == false).Select(q => q.Id).Contains(p.ExamPlanRecordId));
                IQueryable<Biz_Employee> employeeIdList_InExamNoSubmit = employeeExistQueryable.Where(p => employeeForExamPlanRecord_NoSubmit.Select(q => q.EmployeeId).Contains(p.Id));
                //3.安排考试计划提交且考试时间未过
                IQueryable<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecord_Submit = employeeForExamPlanRecordQueryable.Where(p => examPlanRecordQueryable.Where(q => q.SubmitStatus).Where(q => DateTime.Compare(q.ExamDateTimeEnd, DateTime.Now) > 0).Select(q => q.Id).Contains(p.ExamPlanRecordId));
                IQueryable<Biz_Employee> employeeIdList_InExamSubmit = employeeExistQueryable.Where(p => employeeForExamPlanRecord_Submit.Select(q => q.EmployeeId).Contains(p.Id));

                if (employeeIdList_NoExam.Count() > 0 || employeeIdList_InExamNoSubmit.Count() > 0 || employeeIdList_InExamSubmit.Count() > 0)
                {
                    throw new Exception("该建造师证书已经存在");
                }
            }
        }
        public void SummitEmployee(List<int> employeeIdList)
        {
            int employeeSummitCount = uow.Biz_Employee.GetAll().Where(p => employeeIdList.Contains(p.Id) && p.SubmitStatus == true).Count();

            if (employeeSummitCount > 0)
            {
                throw new Exception("不能重复提交！");
            }


            foreach (int employeeId in employeeIdList)
            {
                Biz_Employee employee = uow.Biz_Employee.GetById(employeeId);
                CreateEmployeeAccount(employee);
                employee.SubmitStatus = true;
                employee.SummitDate = DateTime.Now;
                employee.SummitById = account.Id;
                uow.Biz_Employee.Update(employee);

            }
            uow.Commit();
        }
        private void CreateEmployeeAccount(Biz_Employee employee)
        {
            const string ROLETYPE_EMPLOYEE = "employee";
            IAccountCtrl accountCtrl = new AccountCtrl(this.account);
            Sys_Role employeeRole = uow.Sys_Role.GetAll().Where(p => p.RoleType.ToLower() == ROLETYPE_EMPLOYEE.ToLower()).FirstOrDefault();
            string DefaultPassword = System.Web.Configuration.WebConfigurationManager.AppSettings["DefaultPassword"];
            Sys_Account oldAccount = uow.Sys_Account.GetAll().Where(p => p.AccountName == employee.IDNumber).FirstOrDefault();
            if (oldAccount.IsNull())
            {
                Sys_Account newAccount = new Sys_Account()
                {
                    AccountName = employee.IDNumber,
                    Password = DefaultPassword,
                    UserId = employee.Id
                };
                accountCtrl.AddAccount(newAccount);

                //分配到角色
                Sys_RelAccountRole relAccountRole = new Sys_RelAccountRole()
                {
                    AccountId = newAccount.Id,
                    RoleId = employeeRole.Id
                };
                uow.Sys_RelAccountRole.Add(relAccountRole);
            }
            else
            {
                oldAccount.UserId = employee.Id;
                uow.Sys_Account.Update(oldAccount);
            }
        }

        public void SaveTrainingRecord(Biz_TrainingRecord trainingRecord)
        {
            Biz_TrainingRecord oldTrainingRecord = uow.Biz_TrainingRecord.GetById(trainingRecord.Id);
            if (oldTrainingRecord.IsNull())
            {

                trainingRecord.CreateById = account.Id;
                trainingRecord.CreateDate = DateTime.Now;
                trainingRecord.Id = 0;
                uow.Biz_TrainingRecord.Add(trainingRecord);
                //修改上一流程操作状态
                Biz_Employee employee = uow.Biz_Employee.GetById(trainingRecord.EmployeeId);
                employee.OperationStatus = true;
                uow.Biz_Employee.Update(employee);
            }
            else
            {
                if (oldTrainingRecord.SubmitStatus == true)
                {
                    throw new Exception("已经提交，不能编辑");
                }
                oldTrainingRecord.EmployeeId = trainingRecord.EmployeeId;
                oldTrainingRecord.StudyTime = trainingRecord.StudyTime;
                oldTrainingRecord.PracticeTime = trainingRecord.PracticeTime;
                oldTrainingRecord.AbilityTestResult = trainingRecord.AbilityTestResult;
                oldTrainingRecord.Remark = trainingRecord.Remark;
                uow.Biz_TrainingRecord.Update(oldTrainingRecord);
            }
            uow.Commit();
        }
        public void SubmitTrainingRecord(List<int> employeeIdList)
        {
            int TrainingRecordSummitCount = uow.Biz_TrainingRecord.GetAll().Where(p => employeeIdList.Contains(p.EmployeeId) && p.SubmitStatus == true).Count();

            if (TrainingRecordSummitCount > 0)
            {
                throw new Exception("不能重复提交！");
            }
            IQueryable<Biz_TrainingRecord> trainingRecordQueryable = uow.Biz_TrainingRecord.GetAll().Where(p => employeeIdList.Contains(p.EmployeeId));
            if (trainingRecordQueryable.Count() != employeeIdList.Count())
            {
                throw new Exception("没有培训记录的人 不能提交！");
            }
            trainingRecordQueryable.Update(p => new Biz_TrainingRecord()
            {
                SubmitStatus = true,
                SubmitDate = DateTime.Now,
                SubmitById = account.Id
            });
            uow.Commit();
        }
        //审核培训记录
        public void CheckTrainingRecord(List<int> employeeIdList, string studyTime, string practiceTime, string abilityTestResult, string remark, bool passFlag)
        {


            int existsNotSubmitCount = uow.Biz_Employee.GetAll().Where(p => employeeIdList.Contains(p.Id) && !p.SubmitStatus).Count();
            if (existsNotSubmitCount > 0)
            {
                throw new Exception("未提交的人,不能审核");
            }
            int existsOnlineTrainingCount = uow.Biz_Employee.GetAll().Where(p => employeeIdList.Contains(p.Id) && p.TrainingType == "线上培训").Count();
            if (existsOnlineTrainingCount > 0)
            {
                throw new Exception("不能审核线上培训的人");
            }
            int existsCheckedCount = uow.Biz_TrainingRecord.GetAll().Where(p => employeeIdList.Contains(p.EmployeeId)).Count();
            if (existsCheckedCount > 0)
            {
                throw new Exception("不能重复审核");
            }
            //添加培训记录
            foreach (int employeeId in employeeIdList)
            {
                Biz_TrainingRecord newTrainingRecord = new Biz_TrainingRecord()
                  {
                      EmployeeId = employeeId,
                      StudyTime = studyTime,
                      PracticeTime = practiceTime,
                      AbilityTestResult = abilityTestResult,
                      Remark = remark,
                      CreateDate = DateTime.Now,
                      CreateById = this.account.Id,
                      PassStatus = passFlag,
                      OperationStatus = false
                  };
                uow.Biz_TrainingRecord.Add(newTrainingRecord);
            };
            if (passFlag == false)//审核不通过的时候 将退回意见存到备注里
            {
                uow.Biz_Employee.GetAll().Where(p => employeeIdList.Contains(p.Id)).Update(p => new Biz_Employee() { OperationStatus = true, Remark = p.Remark + remark });
            }
            else
            {
                //将上一流程的记录 操作状态改为 true
                uow.Biz_Employee.GetAll().Where(p => employeeIdList.Contains(p.Id)).Update(p => new Biz_Employee() { OperationStatus = true });
            }
            uow.Commit();
        }

        //报名确认
        public void CheckEmployeeList(List<int> employeeIdList, bool passFlag, string checkedMark)
        {
            int existsNotSubmitCount = uow.Biz_Employee.GetAll().Where(p => employeeIdList.Contains(p.Id) && p.SubmitStatus == false).Count();
            if (existsNotSubmitCount > 0)
            {
                throw new Exception("报名未提交的人,不能审核");
            }
            int existsTrainingNoPassCount = uow.Biz_TrainingRecord.GetAll().Where(p => employeeIdList.Contains(p.EmployeeId) && p.PassStatus == false)
                                            .Join(uow.Biz_Employee.GetAll().Where(p => p.TrainingType == "线下培训"), a => a.EmployeeId, b => b.Id, (a, b) => new { a, b }).Count();
            if (existsTrainingNoPassCount > 0)
            {
                throw new Exception("培训未通过,不能审核");
            }


            int EmployeeCheckedRecordPassCnt = uow.Biz_EmployeeCheckedRecord.GetAll().Where(p => employeeIdList.Contains(p.EmployeeId)).Count();
            if (EmployeeCheckedRecordPassCnt > 0)
            {
                throw new Exception("已经审核过的人,不能重复审核");
            }

            foreach (int employeeId in employeeIdList)
            {
                Biz_EmployeeCheckedRecord employeeCheckedRecord = new Biz_EmployeeCheckedRecord()
                {
                    EmployeeId = employeeId,
                    OperationStatus = false,
                    PassStatus = passFlag,
                    CreateById = account.Id,
                    CreateDate = DateTime.Now,
                    CheckedMark = checkedMark
                };
                uow.Biz_EmployeeCheckedRecord.Add(employeeCheckedRecord);
            }
            if (passFlag == false)//审核不通过  将意见添加到人员备注信息
            {
                uow.Biz_Employee.GetAll().Where(p => employeeIdList.Contains(p.Id)).Update(u => new Biz_Employee() { OperationStatus = true, Remark = u.Remark + "退回意见：" + checkedMark });
            }
            else
            {
                //线上人员的上一流程未报名
                uow.Biz_Employee.GetAll().Where(p => employeeIdList.Contains(p.Id)).Update(u => new Biz_Employee() { OperationStatus = true });
            }
            //将上一流程状态改为True 线下人员
            uow.Biz_TrainingRecord.GetAll().Where(p => employeeIdList.Contains(p.EmployeeId)).Update(p => new Biz_TrainingRecord() { OperationStatus = true });
            uow.Commit();
        }
        public void ReturnEmployeeList(List<int> employeeIdList, string checkedMark)
        {
            //没有审核通过的 不能退回
            int EmployeeCheckedRecordPassCnt = uow.Biz_EmployeeCheckedRecord.GetAll().Where(p => employeeIdList.Contains(p.EmployeeId)).Where(p => p.PassStatus == true).Count();
            if (EmployeeCheckedRecordPassCnt < employeeIdList.Count)
            {
                throw new Exception("存在没有审核通过的人,不能退回");
            }
            //安排了考试计划的人 不能退回
            int EmployeeInExamCnt = uow.Biz_EmployeeForExamPlanRecord.GetAll().Where(p => employeeIdList.Contains(p.EmployeeId)).Count();
            if (EmployeeInExamCnt > 0)
            {
                throw new Exception("存在已经安排到考试的人,不能退回");
            }
            //更新人员信息
            uow.Biz_Employee.GetAll().Where(p => employeeIdList.Contains(p.Id)).Update(u => new Biz_Employee() { Invalid = true, OperationStatus = true, Remark = u.Remark + "退回意见：" + checkedMark });
            //更新审核记录表
            //uow.Biz_EmployeeCheckedRecord.GetAll().Where(p => employeeIdList.Contains(p.EmployeeId)).Where(p => p.PassStatus == true)
            //    .Update(u => new Biz_EmployeeCheckedRecord() { PassStatus = false, CheckedMark = checkedMark });
            uow.Commit();
        }

        public void SummitExamResult(List<int> employeeIdList)
        {
            foreach (int employeeId in employeeIdList)
            {
                Biz_EmployeeExamResultRecord employeeExamResultRecord = uow.Biz_EmployeeExamResultRecord.GetAll().Where(p => p.EmployeeId == employeeId && p.SummitStatus == false).FirstOrDefault();
                if (employeeExamResultRecord.IsNull())
                {
                    continue;
                }
                else if (employeeExamResultRecord.SummitStatus)
                {
                    continue;
                }
                employeeExamResultRecord.SummitStatus = true;
                employeeExamResultRecord.SummitDate = DateTime.Now;
                employeeExamResultRecord.SummitById = account.Id;
                uow.Biz_EmployeeExamResultRecord.Update(employeeExamResultRecord);

            }
            uow.Commit();
        }

        public void CheckedExamResult(List<int> employeeIdList, bool checkedStatus, string checkedMark, string startCertificateDate)
        {

            int existCnt = uow.Biz_EmployeeExamResultCheckedRecord.GetAll().Where(p => employeeIdList.Contains(p.EmployeeId)).Count();
            if (existCnt > 0)
            {
                throw new Exception("存在已经审核过的人");
            }
            IEmployeeCtrl employeeCtrl = new EmployeeCtrl(account);
            ISmrzServiceCtrl smrzServiceCtrl = new SmrzServiceCtrl(account);

            List<Biz_EmployeeExamResultRecord> Biz_EmployeeExamResultRecordList = employeeCtrl.GetEmployeeExamResultRecordListByEmployeeId(employeeIdList);
            foreach (int employeeId in employeeIdList)
            {
                Biz_EmployeeExamResultRecord employeeExamResultRecord = Biz_EmployeeExamResultRecordList.Where(p => p.EmployeeId == employeeId).Single();
                employeeExamResultRecord.OperationStatus = true;
                uow.Biz_EmployeeExamResultRecord.Update(employeeExamResultRecord);
                int examPlanRecordId = employeeExamResultRecord.ExamPlanRecordId;
                Biz_EmployeeExamResultCheckedRecord employeeExamResultCheckedRecord = new Biz_EmployeeExamResultCheckedRecord()
                {
                    EmployeeId = employeeId,
                    ExamPlanRecordId = examPlanRecordId,
                    CheckedStatus = checkedStatus,
                    CheckedMark = checkedMark,
                    OperationStatus = false,
                    CreateDate = DateTime.Now,
                    CreateById = account.Id
                };

                uow.Biz_EmployeeExamResultCheckedRecord.Add(employeeExamResultCheckedRecord);
                if (checkedStatus && employeeExamResultRecord.FinalExamResult == "合格")
                {

                    if (startCertificateDate.IsNull())
                    {
                        throw new Exception("发证日期不能为空");
                    }
                    int employeeAuthenticationCount = uow.Biz_EmployeeAuthentication.GetAll().Where(p => p.EmployeeId == employeeId).Count();
                    if (employeeAuthenticationCount < 1)
                    {
                        throw new Exception("未通过实名认证,不能发证");
                    }
                    Biz_Employee employee = uow.Biz_Employee.GetById(employeeId);
                    this.SaveCertificate(employee, Convert.ToDateTime(startCertificateDate));
                    uow.Commit();
                    smrzServiceCtrl.newCertificateAuthentication(employeeId);
                }
            }
            uow.Commit();
        }

        public void IssuanceCertificate(List<int> employeeIdList, int examPlanRecordId, int trainingInstitutionId, string remark)
        {
            int existsCount = uow.Biz_EmployeeCertificateIssuanceRecord.GetAll().Where(p => employeeIdList.Contains(p.EmployeeId)).Count();
            if (existsCount > 0)
            {
                throw new Exception("存在已经发放过证书的人员");
            }
            foreach (int employeeId in employeeIdList)
            {
                Biz_EmployeeExamResultCheckedRecord employeeExamResultCheckedRecord = uow.Biz_EmployeeExamResultCheckedRecord.GetAll()
                    .Where(p => p.EmployeeId == employeeId && p.OperationStatus == false && p.CheckedStatus == true).Single();
                employeeExamResultCheckedRecord.OperationStatus = true;
                uow.Biz_EmployeeExamResultCheckedRecord.Update(employeeExamResultCheckedRecord);

                Biz_EmployeeCertificateIssuanceRecord employeeCertificateIssuanceRecord = new Biz_EmployeeCertificateIssuanceRecord()
                {
                    EmployeeId = employeeId,
                    ExamPlanRecordId = examPlanRecordId,
                    TrainingInstitutionId = trainingInstitutionId,
                    Remark = remark,
                    CreateDate = DateTime.Now,
                    CreateById = account.Id
                };
                uow.Biz_EmployeeCertificateIssuanceRecord.Add(employeeCertificateIssuanceRecord);
            }
            uow.Commit();
        }

        #endregion

        #region 企业报名
        public List<Sys_DropdownListItem> GetEmployeeSubjectList()
        {
            const string ITEMNAME = "EmployeeSubject";
            List<Sys_DropdownListItem> dropdownList = uow.Sys_DropdownListItem.GetAll().Where(p => p.ItemName == ITEMNAME).ToList();
            return dropdownList;
        }
        public List<Sys_DropdownListItem> GetEmployeeIndustryList()
        {
            const string ITEMNAME = "EmployeeIndustry";
            List<Sys_DropdownListItem> dropdownList = uow.Sys_DropdownListItem.GetAll().Where(p => p.ItemName == ITEMNAME).ToList();
            return dropdownList;
        }
        public Biz_Employee GetEmployeeInfoById(int employeeID)
        {
            Biz_Employee employee = uow.Biz_Employee.GetById(employeeID);
            return employee;
        }
        public List<Biz_Employee> GetEmployeeList(string employeeName, string idNumber, string trainingType, string examType, string industry, string workFlowStatus, int page, int rows, ref int totalCount)
        {
            IQueryable<Biz_Employee> employeeQueryable = uow.Biz_Employee.GetAll().Where(p => !p.Invalid);
            //数据权限  各企业看自己的人员
            employeeQueryable = employeeQueryable.Where(p => p.EnterpriseId == account.UserId);
            if (!employeeName.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.EmployeeName.Contains(employeeName));
            }
            if (!idNumber.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.IDNumber == idNumber);
            }
            if (!trainingType.IsNull())
            {
                if (trainingType == "不培训")
                {
                    employeeQueryable = employeeQueryable.Where(p => p.IsTraining == false);
                }
                else
                {
                    employeeQueryable = employeeQueryable.Where(p => p.TrainingType == trainingType);
                }
            }
            if (!examType.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.ExamType == examType);
            }
            if (!industry.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.Industry == industry);
            }

            List<Biz_Employee> employeeList = employeeQueryable.ToList();

            List<WorkFlowStatus> WorkFlowStatusList = GetCurrentWorkFlowStatusByEmployeeIdList(employeeList.Select(p => p.Id).ToList()).ToList();
            if (!workFlowStatus.IsNull())
            {
                employeeList = employeeList.Join(WorkFlowStatusList, p => p.Id, q => q.employeeId, (p, q) => new { p, q })
                    .Where(o => o.q.WorkFlowStatusTag == workFlowStatus).Select(o => o.p).ToList();
            }

            totalCount = employeeList.Count;

            int indexBegin = (page - 1) * rows;
            employeeList = employeeList.OrderByDescending(P => P.Id).Skip(indexBegin).Take(rows).ToList();

            //if (WorkFlowStatusList.Count>0)
            //{
            //    foreach (Biz_Employee employee in employeeList)
            //    {
            //        employee.CurrentWorkFlowStatus = WorkFlowStatusList.First(p => p.employeeId == employee.Id).WorkFlowStatusTag;

            //    }
            //}

            return employeeList;
        }
        public void DeleteEmployeeById(int employeeId)
        {
            Biz_Employee oldEmployee = uow.Biz_Employee.GetById(employeeId);

            WorkFlowStatus workFlowStatus = GetCurrentWorkFlowStatus(employeeId);
            if (oldEmployee.SubmitStatus && workFlowStatus.WorkFlowStatusTag != WorkFlowStatusTag.CheckUnpassed)
            {
                throw new Exception("人员信息已提交,不能删除");
            }
            if (oldEmployee.SubmitStatus == false)
            {
                uow.Biz_Employee.Delete(oldEmployee);
            }
            else
            {
                oldEmployee.Invalid = true;
                uow.Biz_Employee.Update(oldEmployee);
            }
            uow.Commit();
        }
        public List<Biz_Employee> GetEmployeeByIdList(List<int> employeeIdList)
        {
            List<Biz_Employee> employeeList = uow.Biz_Employee.GetAll().Where(p => employeeIdList.Contains(p.Id)).ToList();
            return employeeList;
        }
        public void UpdateEmployeeInfo(Biz_Employee employee)
        {

            Biz_Employee oldEmployee = uow.Biz_Employee.GetById(employee.Id);
            WorkFlowStatus workFlowStatus = GetCurrentWorkFlowStatus(employee.Id);
            if (oldEmployee.SubmitStatus && workFlowStatus.WorkFlowStatusTag != WorkFlowStatusTag.CheckUnpassed && workFlowStatus.WorkFlowStatusTag != WorkFlowStatusTag.TrainingUnPassed)
            {
                throw new Exception("人员信息已提交,不能更新信息");
            }

            if (workFlowStatus.WorkFlowStatusTag == WorkFlowStatusTag.CheckUnpassed || workFlowStatus.WorkFlowStatusTag == WorkFlowStatusTag.TrainingUnPassed)
            {
                uow.Biz_Employee.GetAll().Where(p => p.Id == employee.Id).Update(p => new Biz_Employee() { Invalid = true });

                Biz_Employee newEmployee = new Biz_Employee()
                {
                    EmployeeName = employee.EmployeeName,
                    Sex = employee.Sex,
                    Birthday = employee.Birthday,
                    IDNumber = employee.IDNumber,
                    Job = employee.Job,
                    Title4Technical = employee.Title4Technical,
                    City = employee.City,
                    IsTraining = employee.IsTraining,
                    TrainingType = employee.TrainingType,
                    TrainingInstitutionId = employee.TrainingInstitutionId,
                    ExamType = employee.ExamType,
                    Industry = employee.Industry,
                    Remark = employee.Remark,
                    PrintCertificate = employee.PrintCertificate,
                    ConstructorCertificateNo = employee.ConstructorCertificateNo,
                    CreateById = account.Id,
                    CreateDate = DateTime.Now,
                    EnterpriseId = account.UserId,
                    SubmitStatus = false,
                    OperationStatus = false,
                    Invalid = false
                };
                VerifyIDNumber(newEmployee);
                VerifyConstructorCertificateNo(newEmployee);
                uow.Biz_Employee.Add(newEmployee);

            }
            else
            {
                oldEmployee.EmployeeName = employee.EmployeeName;
                oldEmployee.Sex = employee.Sex;
                oldEmployee.Birthday = employee.Birthday;
                oldEmployee.IDNumber = employee.IDNumber;
                oldEmployee.Job = employee.Job;
                oldEmployee.Title4Technical = employee.Title4Technical;
                oldEmployee.City = employee.City;
                oldEmployee.IsTraining = employee.IsTraining;
                oldEmployee.TrainingType = employee.TrainingType;
                oldEmployee.TrainingInstitutionId = employee.TrainingInstitutionId;
                oldEmployee.ExamType = employee.ExamType;
                oldEmployee.Industry = employee.Industry;
                oldEmployee.Remark = employee.Remark;
                oldEmployee.PrintCertificate = employee.PrintCertificate;
                oldEmployee.ConstructorCertificateNo = employee.ConstructorCertificateNo;
                VerifyIDNumber(oldEmployee);
                VerifyConstructorCertificateNo(oldEmployee);
                uow.Biz_Employee.Update(oldEmployee);
            }
            uow.Commit();
        }


        #endregion

        #region 培训记录
        public List<Biz_Employee> GetEmployeeList_TrainingRecord(string enterpriseName, string employeeName, string idNumber, string trainingType, string examType, string industry, string trainingStatus, int page, int rows, ref int totalCount)
        {
            IQueryable<Biz_Employee> employeeQueryable = uow.Biz_Employee.GetAll().Where(p => !p.Invalid && p.SubmitStatus == true);

            #region //数据权限  各企业看自己的人员
            employeeQueryable = employeeQueryable.Where(p => p.TrainingInstitutionId == account.UserId);
            #endregion

            if (trainingStatus == "审核通过")
            {
                IQueryable<Biz_TrainingRecord> trainingRecordQueryableTmp = uow.Biz_TrainingRecord.GetAll().Where(p => p.PassStatus == true);
                employeeQueryable = employeeQueryable.Where(p => trainingRecordQueryableTmp.Select(q => q.EmployeeId).Contains(p.Id));
            }
            else if (trainingStatus == "审核不通过")
            {
                IQueryable<Biz_TrainingRecord> trainingRecordQueryableTmp = uow.Biz_TrainingRecord.GetAll().Where(p => p.PassStatus == false);
                employeeQueryable = employeeQueryable.Where(p => trainingRecordQueryableTmp.Select(q => q.EmployeeId).Contains(p.Id));
            }
            else if (trainingStatus == "未审核")
            {
                IQueryable<Biz_TrainingRecord> trainingRecordQueryableTmp = uow.Biz_TrainingRecord.GetAll();
                employeeQueryable = employeeQueryable.Where(p => !trainingRecordQueryableTmp.Select(q => q.EmployeeId).Contains(p.Id));
            }


            if (!enterpriseName.IsNull())
            {
                IQueryable<Biz_Enterprise> enterpriseQueryable = uow.Biz_Enterprise.GetAll().Where(p => p.EnterpriseName.Contains(enterpriseName));
                employeeQueryable = employeeQueryable.Where(p => enterpriseQueryable.Select(q => q.Id).Contains(p.EnterpriseId));
            }
            if (!employeeName.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.EmployeeName.Contains(employeeName));
            }
            if (!idNumber.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.IDNumber.Contains(idNumber));
            }
            if (!trainingType.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.TrainingType == trainingType);
            }
            if (!examType.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.ExamType == examType);
            }
            if (!industry.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.Industry == industry);
            }

            List<Biz_Employee> employeeList = employeeQueryable.ToList();
            totalCount = employeeList.Count;
            int indexBegin = (page - 1) * rows;
            employeeList = employeeList.OrderByDescending(P => P.SummitDate).Skip(indexBegin).Take(rows).ToList();

            return employeeList;
        }
        public List<Biz_TrainingRecord> GetTrainingRecordListByEmployeeIdList(List<int> employeeIdList)
        {
            List<Biz_TrainingRecord> TrainingRecordList = uow.Biz_TrainingRecord.GetAll().Where(p => employeeIdList.Contains(p.EmployeeId)).ToList();
            return TrainingRecordList;
        }

        public Biz_TrainingRecord GetTrainingRecord(int employeeId)
        {
            Biz_TrainingRecord trainingRecord = uow.Biz_TrainingRecord.GetAll().Where(p => p.EmployeeId == employeeId).FirstOrDefault();
            return trainingRecord;
        }

        #endregion

        #region 人员审核
        public List<Biz_Employee> GetEmployeeList_EmployeeCheck(string enterpriseName, string employeeName, string idNumber,
                                            string examType, string industry, string checkStatus, string checkDateBegin,
                                            string checkDateEnd, string trainingInstitutionName, int page, int rows, ref int totalCount, List<string> cityList)
        {
            IQueryable<Biz_Employee> employeeQueryable = uow.Biz_Employee.GetAll().Where(p => !p.Invalid && p.SubmitStatus == true);
            IQueryable<Biz_TrainingRecord> trainingRecordQueryable = uow.Biz_TrainingRecord.GetAll().Where(p => p.PassStatus == true);

            #region //数据权限 看报考到本市的考生
            employeeQueryable = employeeQueryable.Where(p => cityList.Contains(p.City));
            #endregion

            //人员审核  线下培训通过的人  或者 线上培训且已经提交的人
            employeeQueryable = employeeQueryable.Where(p =>
                (trainingRecordQueryable.Where(q => q.EmployeeId == p.Id).Select(q => q.EmployeeId).Contains(p.Id) && p.TrainingType == "线下培训")
                || p.TrainingType == "线上培训"
                );

            if (!enterpriseName.IsNull())
            {
                IQueryable<Biz_Enterprise> enterpriseQueryable = uow.Biz_Enterprise.GetAll().Where(p => p.EnterpriseName.Contains(enterpriseName));
                employeeQueryable = employeeQueryable.Where(p => enterpriseQueryable.Select(q => q.Id).Contains(p.EnterpriseId));
            }
            if (!trainingInstitutionName.IsNull())
            {
                IQueryable<Biz_TrainingInstitution> trainingInstitutionQueryable = uow.Biz_TrainingInstitution.GetAll().Where(p => p.InstitutionName.Contains(trainingInstitutionName));
                employeeQueryable = employeeQueryable.Where(p => trainingInstitutionQueryable.Select(q => q.Id).Contains(p.TrainingInstitutionId));
            }
            if (!employeeName.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.EmployeeName.Contains(employeeName));
            }
            if (!idNumber.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.IDNumber.Contains(idNumber));
            }
            if (!examType.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.ExamType == examType);
            }
            if (!industry.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.Industry == industry);
            }

            if (!checkStatus.IsNull() || !checkDateBegin.IsNull() || !checkDateEnd.IsNull())
            {
                IQueryable<Biz_EmployeeCheckedRecord> employeeCheckedRecordQueryable = uow.Biz_EmployeeCheckedRecord.GetAll();
                if (checkStatus == "未审核")
                {
                    employeeQueryable = employeeQueryable.Where(p => employeeCheckedRecordQueryable.Select(q => q.EmployeeId).Contains(p.Id) == false);
                }
                else
                {
                    if (checkStatus == "审核通过")
                    {
                        employeeCheckedRecordQueryable = employeeCheckedRecordQueryable.Where(p => p.PassStatus == true);

                    }
                    else if (checkStatus == "审核不通过")
                    {
                        employeeCheckedRecordQueryable = employeeCheckedRecordQueryable.Where(p => p.PassStatus == false);
                    }

                    if (!checkDateBegin.IsNull())
                    {
                        DateTime dtBegin = DateTime.ParseExact(checkDateBegin, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        employeeCheckedRecordQueryable = employeeCheckedRecordQueryable.Where(p => dtBegin <= p.CreateDate);
                    }

                    if (!checkDateEnd.IsNull())
                    {
                        DateTime dtEnd = DateTime.ParseExact(checkDateEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        employeeCheckedRecordQueryable = employeeCheckedRecordQueryable.Where(p => p.CreateDate <= dtEnd);
                    }
                    employeeQueryable = employeeQueryable.Where(p => employeeCheckedRecordQueryable.Select(q => q.EmployeeId).Contains(p.Id));
                }
            }
            totalCount = employeeQueryable.Count();
            int indexBegin = (page - 1) * rows;
            List<Biz_Employee> employeeList = employeeQueryable.OrderByDescending(P => P.SummitDate).Skip(indexBegin).Take(rows).ToList();
            return employeeList;
        }

        public List<Biz_EmployeeCheckedRecord> GetEmployeeCheckedRecordListByEmployeeIdList(List<int> employeeIdList)
        {
            List<Biz_EmployeeCheckedRecord> employeeCheckedRecordList = uow.Biz_EmployeeCheckedRecord.GetAll().Where(p => employeeIdList.Contains(p.EmployeeId)).ToList();
            return employeeCheckedRecordList;
        }
        #endregion

        #region 获取用户名称
        public string GetUserName(int accountId)
        {
            IAccountCtrl accountCtrl = new AccountCtrl(this.account);
            return accountCtrl.GetUserName(accountId);
        }
        #endregion

        #region 制定考试计划
        public List<Biz_Employee> GetEmployeeListByEmployeeIdList(List<int> employeeIdList)
        {
            return uow.Biz_Employee.GetAll().Where(p => employeeIdList.Contains(p.Id)).ToList();
        }

        public string GetExamPlanNumber(string cityName)
        {
            const int SERIALNUMBERLENGTH = 3;
            //考试流水号格式  城市代码+yyyyMMdd+3位流水号
            string examPlanNumberPrefix = "";
            string cityCode = uow.Biz_City.GetAll().Where(p => p.CityName == cityName).SingleOrDefault().CityCode;

            examPlanNumberPrefix += cityCode + DateTime.Now.ToString("yyyyMMdd");
            //取后面流水号
            int serialNumber = uow.Biz_ExamPlanRecord.GetAll().Where(p => p.ExamPlanNumber.IndexOf(examPlanNumberPrefix) >= 0).Count();
            serialNumber += 1;
            string strSerialNumber = serialNumber.ToString();


            if (strSerialNumber.Length >= SERIALNUMBERLENGTH)
            {
                throw new Exception("流水号溢出");
            }
            while (strSerialNumber.Length < SERIALNUMBERLENGTH)
            {
                strSerialNumber = "0" + strSerialNumber;
            }
            string examPlanNumber = examPlanNumberPrefix + strSerialNumber;
            return examPlanNumber;
        }
        public int GetNotInExamPlanEmployeeCountByCityList(List<string> cityList)
        {
            //可用报名人员
            IQueryable<Biz_Employee> employeeQueryable = uow.Biz_Employee.GetAll().Where(q => !q.Invalid).Where(q => cityList.Contains(q.City));
            IQueryable<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordQueryable = uow.Biz_EmployeeForExamPlanRecord.GetAll();
            IQueryable<Biz_EmployeeCheckedRecord> employeeCheckedRecordQueryable = uow.Biz_EmployeeCheckedRecord.GetAll()
                  .Where(p => employeeQueryable.Select(q => q.Id).Contains(p.EmployeeId))//是有效的报名人员信息
                  .Where(p => p.PassStatus == true).Where(p => employeeForExamPlanRecordQueryable.Select(q => q.EmployeeId).Contains(p.EmployeeId) == false);//不存在与一分配的人员
            int NotInExamPlanEmployeeCount = employeeCheckedRecordQueryable.Select(p => p.EmployeeId).Distinct().Count();//不重复的人员Id
            return NotInExamPlanEmployeeCount;
        }
        public void MakeExamPlanAndAutoAssign(Biz_ExamPlanRecord examPlan, int trainingInstitutionId, int autoAssignCount, string cityName)
        {
            IQueryable<Biz_EmployeeCheckedRecord> employeeQueryable = uow.Biz_EmployeeCheckedRecord.GetAll()
                   .Join(uow.Biz_Employee.GetAll().Where(p => !p.Invalid).Where(p => p.City == cityName), a => a.EmployeeId, b => b.Id, (a, b) => new { a, b })
                   .Join(uow.Biz_Enterprise.GetAll(), o => o.b.EnterpriseId, c => c.Id, (o, c) => new { o, c })
                   .Select(q => q.o.a)
                   .Where(p => p.PassStatus == true && p.OperationStatus == false);

            int employeeCount = employeeQueryable.Count();

            if (employeeCount < autoAssignCount)
            {
                throw new Exception("自动分配的人数超过待分配的人数");
            }

            List<Biz_ExaminationRoom> existExamRoomList = uow.Biz_ExaminationRoom.GetAll()
                .Where(p => p.ExaminationPointId == trainingInstitutionId)
                .Where(p => p.Enabled == true).OrderBy(p => p.Sequence).ToList();
            int examRoomCnt = existExamRoomList.Count();
            if (examRoomCnt == 0)
            {
                throw new Exception("考核点无可用考场");
            }

            while (existExamRoomList.Count() > 0 && autoAssignCount > 0)
            {
                Biz_ExaminationRoom examRoom = existExamRoomList.First();
                int assignCount = examRoom.PersonCount < autoAssignCount ? examRoom.PersonCount : autoAssignCount;
                employeeQueryable = employeeQueryable.Take(assignCount);
                string examPlanNumber = GetExamPlanNumber(cityName);
                examPlan.ExamPlanNumber = examPlanNumber;
                MakeExamPlanAndManualAssign(examPlan, examRoom.Id, employeeQueryable.Select(p => p.EmployeeId).ToList());
                existExamRoomList.Remove(examRoom);
                autoAssignCount = autoAssignCount - assignCount;
            }


        }
        public void MakeExamPlanAndManualAssign(Biz_ExamPlanRecord examPlan, int examRoomId, List<int> employeeIdList)
        {
            //存在无效的人员
            int invalidEmployeeCnt = uow.Biz_Employee.GetAll().Where(p => p.Invalid == true && employeeIdList.Contains(p.Id)).Count();
            if (invalidEmployeeCnt > 0)
            {
                throw new Exception("存在无效的人员！");
            }
            //不能在其他考试计划中存在
            int repeateExamPlanCnt = uow.Biz_EmployeeForExamPlanRecord.GetAll().Join(uow.Biz_ExamPlanRecord.GetAll(), a => a.ExamPlanRecordId, b => b.Id, (a, b) => new { a, b })
                            .Where(o => employeeIdList.Contains(o.a.EmployeeId))
                            .Where(o => o.b.ExamPlanNumber != examPlan.ExamPlanNumber).Count();
            if (repeateExamPlanCnt > 0)
            {
                throw new Exception("当前选中人员已安排到其他考试计划！");
            }

            //已经安排过的人 不能再安排考试计划
            int existsCnt = uow.Biz_EmployeeForExamPlanRecord.GetAll()
                .Where(p => employeeIdList.Contains(p.EmployeeId)).Where(p => p.ExamRoomId != examRoomId).Count();
            if (existsCnt > 0)
            {
                throw new Exception("当前选中人员已安排到其它考场！");
            }
            Biz_ExamPlanRecord examPlanRecord;

            bool newExamPlanFlag = false;

            int examPlanRecordCount = uow.Biz_ExamPlanRecord.GetAll().Where(p => p.ExamPlanNumber == examPlan.ExamPlanNumber).Count();

            if (examPlanRecordCount > 0)
            {
                examPlanRecord = uow.Biz_ExamPlanRecord.GetAll().Where(p => p.ExamPlanNumber == examPlan.ExamPlanNumber).Single();
            }
            else
            {
                examPlanRecord = new Biz_ExamPlanRecord();
                newExamPlanFlag = true;
            }
            if (examPlanRecord.SubmitStatus)
            {
                throw new Exception("考试计划已经提交 不能修改");
            }





            examPlanRecord.ExamPlanNumber = examPlan.ExamPlanNumber;
            examPlanRecord.ExamDateTimeBegin = examPlan.ExamDateTimeBegin;
            //examPlanRecord.AExamDateTimeEnd = examPlan.AExamDateTimeEnd;
            //examPlanRecord.BExamDateTimeBegin = examPlan.BExamDateTimeBegin;
            //examPlanRecord.ExamDateTimeEnd = examPlan.ExamDateTimeEnd;
            //如果当前考场没有人 不重新计算考试结束时间
            if (employeeIdList.Count != 0)
            {
                //获取最长的考试时长 计算出考试结束时间 （取证类型不同 试卷考试答题时间不同）
                IQueryable<Biz_Employee> employeeQueryable = uow.Biz_Employee.GetAll().Where(p => employeeIdList.Contains(p.Id));

                int MaxAllDuration = uow.Biz_PaperForExamType.GetAll().Join(employeeQueryable, a => new { ExamType = a.ExamType, Industry = a.Industry }, b => new { ExamType = b.ExamType, Industry = b.Industry }, (a, b) => new { a, b })
                       .GroupBy(o => new { o.b.Industry, o.b.ExamType, o.a.PaperId, o.a.Duration })
                       .Select(o => new
                       {
                           Industry = o.Key.ExamType,
                           ExamType = o.Key.Industry,
                           PaperId = o.Key.PaperId,
                           Duration = o.Key.Duration
                       })
                       .GroupBy(p => new { p.Industry, p.ExamType })
                       .Select(p => new
                       {
                           Industry = p.Key.ExamType,
                           ExamType = p.Key.Industry,
                           AllDuration = p.Sum(q => q.Duration)
                       })
                       .Max(p => p.AllDuration);
                // string strExamInterval = AppFn.GetAppSettingsValue("ExamInterval");
                // int examInterval = Convert.ToInt32(strExamInterval);
                // examPlanRecord.ExamDateTimeEnd = examPlanRecord.ExamDateTimeBegin.AddMinutes(MaxAllDuration + examInterval);
                examPlanRecord.ExamDateTimeEnd = examPlanRecord.ExamDateTimeBegin.AddMinutes(MaxAllDuration);
            }
            else
            {
                examPlanRecord.ExamDateTimeEnd = examPlanRecord.ExamDateTimeBegin;
            }

            if (newExamPlanFlag)
            {
                examPlanRecord.CreateById = account.Id;
                examPlanRecord.CreateDate = DateTime.Now;
                examPlanRecord.SafetyKnowledgePassMark = 60;
                examPlanRecord.ManagementAbilityPassMark = 60;
                uow.Biz_ExamPlanRecord.Add(examPlanRecord);
                uow.Commit();
            }
            else
            {
                //考试计划已存在 

                //更新考试计划
                uow.Biz_ExamPlanRecord.Update(examPlanRecord);
                //更新人员审核记录操作状态为未操作
                IQueryable<Biz_EmployeeForExamPlanRecord> EmployeeForExamPlanRecordQueryable = uow.Biz_EmployeeForExamPlanRecord.GetAll()
                    .Where(p => p.ExamPlanRecordId == examPlanRecord.Id)
                    .Where(p => p.ExamRoomId == examRoomId);

                uow.Biz_EmployeeCheckedRecord.GetAll().Where(p => EmployeeForExamPlanRecordQueryable.Select(q => q.EmployeeId).Contains(p.EmployeeId))
                    .Update(p => new Biz_EmployeeCheckedRecord()
                            {
                                OperationStatus = false
                            });
                //删除当前考场已分配的人
                uow.Biz_EmployeeForExamPlanRecord.GetAll().Where(p => p.ExamPlanRecordId == examPlanRecord.Id).Where(p => p.ExamRoomId == examRoomId).Delete();
            }

            //生成考试座位号               
            int examSeatNumber = 0;
            foreach (int employeeId in employeeIdList)
            {
                examSeatNumber += 1;
                //新增考试记录
                Biz_EmployeeForExamPlanRecord employeeForExamPlanRecord = new Biz_EmployeeForExamPlanRecord()
                {
                    EmployeeId = employeeId,
                    ExamPlanRecordId = examPlanRecord.Id,
                    ExamRoomId = examRoomId,
                    OperationStatus = false,
                    CreateById = account.Id,
                    CreateDate = DateTime.Now,
                    ExamSeatNumber = examSeatNumber
                };
                uow.Biz_EmployeeForExamPlanRecord.Add(employeeForExamPlanRecord);
            }
            if (employeeIdList.Count() > 0)
            {
                //将资料审核记录的  操作状态改为true
                uow.Biz_EmployeeCheckedRecord.GetAll().Where(p => employeeIdList.Contains(p.EmployeeId))
                    .Update(o => new Biz_EmployeeCheckedRecord() { OperationStatus = true });
            }
            uow.Commit();
        }
        public List<Biz_Employee> GetMakeExamPlanEmployeeList(string examPlanNumber, int trainingInstitutionId, int examRoomId, string conditionStr, int page, int rows, ref int totalCount, List<string> cityList)
        {
            //资料已审核通过的人
            IQueryable<Biz_EmployeeCheckedRecord> employeeCheckedRecordQueryable = uow.Biz_EmployeeCheckedRecord.GetAll().Where(p => p.PassStatus == true);
            IQueryable<Biz_Employee> employeeQueryable = uow.Biz_Employee.GetAll().Where(p => !p.Invalid).Where(p => employeeCheckedRecordQueryable.Select(c => c.EmployeeId).Contains(p.Id));
            if (!cityList.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => cityList.Contains(p.City));
            }
            //制定考试计划的记录
            IQueryable<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordQueryable = uow.Biz_EmployeeForExamPlanRecord.GetAll();
            IQueryable<Biz_ExamPlanRecord> examPlanRecordQueryable = uow.Biz_ExamPlanRecord.GetAll();
            IQueryable<Biz_EmployeeForExamPlanRecord> sortEmployeeForExamPlanRecordQueryable = uow.Biz_EmployeeForExamPlanRecord.GetAll();
            //需要显示已分配到该考场的和未分配的人
            //1.查找已经分配到本次考试计划且是本考场的人 //2.查找未分配的人
            if (!examPlanNumber.IsNull())
            {
                examPlanRecordQueryable = examPlanRecordQueryable.Where(p => p.ExamPlanNumber == examPlanNumber);
                sortEmployeeForExamPlanRecordQueryable = sortEmployeeForExamPlanRecordQueryable.Where(p => examPlanRecordQueryable.Select(q => q.Id).Contains(p.ExamPlanRecordId));
            }
            if (trainingInstitutionId != 0)
            {
                IQueryable<Biz_ExaminationRoom> examRoomQueryable = uow.Biz_ExaminationRoom.GetAll().Where(p => p.ExaminationPointId == trainingInstitutionId);
                sortEmployeeForExamPlanRecordQueryable = sortEmployeeForExamPlanRecordQueryable.Where(p => examRoomQueryable.Select(q => q.Id).Contains(p.ExamRoomId));
            }
            if (examRoomId != 0)
            {
                sortEmployeeForExamPlanRecordQueryable = sortEmployeeForExamPlanRecordQueryable.Where(p => p.ExamRoomId == examRoomId);
            }
            employeeQueryable = employeeQueryable.Where(p => (sortEmployeeForExamPlanRecordQueryable.Select(e => e.EmployeeId).Contains(p.Id) || !employeeForExamPlanRecordQueryable.Select(e => e.EmployeeId).Contains(p.Id)));

            if (!conditionStr.IsNull())
            {
                //人员条件 都是或者
                IQueryable<Biz_Enterprise> SortEnterpriseQueryable = uow.Biz_Enterprise.GetAll().Where(e => e.EnterpriseName.Contains(conditionStr));
                employeeQueryable = employeeQueryable.Where(p => (SortEnterpriseQueryable.Select(e => e.Id).Contains(p.EnterpriseId) || p.EmployeeName.Contains(conditionStr) || p.IDNumber.Contains(conditionStr) || p.Industry == conditionStr || p.ExamType == conditionStr));
            }
            totalCount = employeeQueryable.Count();
            int indexBegin = (page - 1) * rows;
            employeeQueryable = employeeQueryable.OrderBy(P => P.Id).Skip(indexBegin).Take(rows);
            List<Biz_Employee> employeeList = employeeQueryable.ToList();
            return employeeList;
        }
        public List<Biz_EmployeeForExamPlanRecord> GetEmployeeForExamPlanRecordByEmployeeIdList(List<int> employeeIdList)
        {
            List<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordList = uow.Biz_EmployeeForExamPlanRecord.GetAll().Where(p => employeeIdList.Contains(p.EmployeeId)).ToList();
            return employeeForExamPlanRecordList;
        }
        public List<Biz_EmployeeForExamPlanRecord> GetEmployeeForExamPlanRecordByExamPlanIdList(List<int> examPlanIdList)
        {
            List<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordList = uow.Biz_EmployeeForExamPlanRecord.GetAll().Where(p => examPlanIdList.Contains(p.ExamPlanRecordId)).ToList();
            return employeeForExamPlanRecordList;
        }

        public List<Biz_EmployeeForExamPlanRecord> GetEmployeeForExamPlanRecordByExamPlanId(int examPlanId)
        {
            IQueryable<Biz_Employee> employeeQueryable = uow.Biz_Employee.GetAll().Where(p => !p.Invalid);
            List<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordList = uow.Biz_EmployeeForExamPlanRecord.GetAll().Where(p => employeeQueryable.Select(q => q.Id).Contains(p.EmployeeId)).Where(p => p.ExamPlanRecordId == examPlanId).ToList();
            return employeeForExamPlanRecordList;
        }
        public List<Biz_ExamPlanRecord> GetExamPlanRecord(string examPlanNumber, string examDatetimeBegin, string examDatetimeEnd, string traningInstitutionName, string employeeName, string iDNumber, string submitStatus, int page, int rows, ref int totalCount, List<string> cityList)
        {
            IQueryable<Biz_ExamPlanRecord> examPlanRecordQueryable = uow.Biz_ExamPlanRecord.GetAll();

            //数据权限 那个角色制定的考试计划  就由那个角色审核考试结果
            Sys_Role accountRole = account.RoleList.FirstOrDefault();
            List<int> accountIdList = uow.Sys_RelAccountRole.GetAll().Where(p => p.RoleId == accountRole.Id).Select(p => p.AccountId).ToList();
            examPlanRecordQueryable = examPlanRecordQueryable.Where(p => accountIdList.Contains(p.CreateById));


            if (!examPlanNumber.IsNull())
            {
                examPlanRecordQueryable = examPlanRecordQueryable.Where(p => p.ExamPlanNumber.Contains(examPlanNumber));
            }
            if (!examDatetimeBegin.IsNull())
            {
                DateTime dt_ExamDatetimeBegin = DateTime.ParseExact(examDatetimeBegin, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.CurrentCulture);
                examPlanRecordQueryable = examPlanRecordQueryable.Where(p => dt_ExamDatetimeBegin <= p.ExamDateTimeBegin);
            }
            if (!examDatetimeEnd.IsNull())
            {
                DateTime dt_ExamDatetimeEnd = DateTime.ParseExact(examDatetimeEnd, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.CurrentCulture);
                examPlanRecordQueryable = examPlanRecordQueryable.Where(p => p.ExamDateTimeEnd <= dt_ExamDatetimeEnd);
            }
            if (submitStatus == "已提交")
            {
                examPlanRecordQueryable = examPlanRecordQueryable.Where(p => p.SubmitStatus == true);
            }
            if (submitStatus == "未提交")
            {
                examPlanRecordQueryable = examPlanRecordQueryable.Where(p => p.SubmitStatus == false);
            }

            if (!traningInstitutionName.IsNull() || !employeeName.IsNull() || !iDNumber.IsNull())
            {
                IQueryable<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordQueryable = uow.Biz_EmployeeForExamPlanRecord.GetAll();
                IQueryable<Biz_Employee> employeeQueryable = uow.Biz_Employee.GetAll().Where(p => !p.Invalid);

                if (!employeeName.IsNull())
                {
                    employeeQueryable = employeeQueryable.Where(p => p.EmployeeName.Contains(employeeName));
                }
                if (!iDNumber.IsNull())
                {
                    employeeQueryable = employeeQueryable.Where(p => p.IDNumber.Contains(iDNumber));
                }
                if (!traningInstitutionName.IsNull())
                {
                    IQueryable<Biz_ExaminationPoint> trainingInstitutionQueryable = uow.Biz_ExaminationPoint.GetAll().Where(t => t.InstitutionName.Contains(traningInstitutionName));
                    IQueryable<Biz_ExaminationRoom> examinationRoomQueryable = uow.Biz_ExaminationRoom.GetAll().Where(R => trainingInstitutionQueryable.Select(t => t.Id).Contains(R.ExaminationPointId));
                    employeeForExamPlanRecordQueryable = employeeForExamPlanRecordQueryable.Where(P => examinationRoomQueryable.Select(e => e.Id).Contains(P.ExamRoomId));
                }
                employeeForExamPlanRecordQueryable = employeeForExamPlanRecordQueryable.Where(P => employeeQueryable.Select(e => e.Id).Contains(P.EmployeeId));
                examPlanRecordQueryable = examPlanRecordQueryable.Where(p => employeeForExamPlanRecordQueryable.Select(q => q.ExamPlanRecordId).Contains(p.Id));
            }
            totalCount = examPlanRecordQueryable.Count();
            int indexBegin = (page - 1) * rows;
            examPlanRecordQueryable = examPlanRecordQueryable.OrderByDescending(P => P.Id).Skip(indexBegin).Take(rows);

            List<Biz_ExamPlanRecord> examPlanRecordLise = examPlanRecordQueryable.ToList();
            return examPlanRecordLise;

        }
        public List<Biz_Employee> GetEmployeeForExamPlanRecord(string examPlanNumber, string traningInstitutionName, string employeeName, string iDNumber, string submitStatus, int page, int rows, ref int totalCount, List<string> cityList)
        {
            IQueryable<Biz_Employee> employeeQueryable = uow.Biz_Employee.GetAll().Where(p => !p.Invalid);
            IQueryable<Biz_ExamPlanRecord> examPlanRecordQueryable = uow.Biz_ExamPlanRecord.GetAll();
            IQueryable<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordQueryable = uow.Biz_EmployeeForExamPlanRecord.GetAll();

            if (!cityList.IsNull() && cityList.Count != 0)
            {
                employeeQueryable = employeeQueryable.Where(p => cityList.Contains(p.City));
            }

            if (!examPlanNumber.IsNull())
            {
                examPlanRecordQueryable = examPlanRecordQueryable.Where(p => p.ExamPlanNumber == examPlanNumber);
            }
            if (!traningInstitutionName.IsNull())
            {
                IQueryable<Biz_ExaminationPoint> trainingInstitutionQueryable = uow.Biz_ExaminationPoint.GetAll().Where(t => t.InstitutionName.Contains(traningInstitutionName));
                IQueryable<Biz_ExaminationRoom> examinationRoomQueryable = uow.Biz_ExaminationRoom.GetAll().Where(R => trainingInstitutionQueryable.Select(t => t.Id).Contains(R.ExaminationPointId));
                employeeForExamPlanRecordQueryable = employeeForExamPlanRecordQueryable.Where(P => examinationRoomQueryable.Select(e => e.Id).Contains(P.ExamRoomId));
            }
            if (!employeeName.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.EmployeeName.Contains(employeeName));
            }
            if (!iDNumber.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.IDNumber.Contains(iDNumber));
            }
            employeeForExamPlanRecordQueryable = employeeForExamPlanRecordQueryable.Where(p => examPlanRecordQueryable.Select(e => e.Id).Contains(p.ExamPlanRecordId));
            employeeQueryable = employeeQueryable.Where(p => employeeForExamPlanRecordQueryable.Select(q => q.EmployeeId).Contains(p.Id));


            totalCount = employeeQueryable.Count();
            int indexBegin = (page - 1) * rows;
            employeeQueryable = employeeQueryable.OrderBy(P => P.Id).Skip(indexBegin).Take(rows);

            List<Biz_Employee> examPlanRecordLise = employeeQueryable.ToList();
            return examPlanRecordLise;

        }
        public void SubmitExamPlan(int examPlanId, int examCoreExamId)
        {
            Biz_ExamPlanRecord examplan = uow.Biz_ExamPlanRecord.GetById(examPlanId);
            if (examplan.SubmitStatus)
            {
                throw new Exception("已经提交过,不能继续提交");
            }
            examplan.SubmitStatus = true;
            examplan.SubmitById = account.Id;
            examplan.SubmitDate = DateTime.Now;
            examplan.ExamCoreExamId = examCoreExamId;
            uow.Biz_ExamPlanRecord.Update(examplan);
            // CreateEmployeeAccount(examplan.Id);
            CreateExamRegistrationNumber(examplan.Id);
            uow.Commit();
        }
        //创建考生账户
        void CreateEmployeeAccount(int examPlanId)
        {
            const string ROLETYPE_EMPLOYEE = "employee";
            IQueryable<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordQueryable = uow.Biz_EmployeeForExamPlanRecord.GetAll().Where(p => p.ExamPlanRecordId == examPlanId);
            List<Biz_Employee> employeeList = uow.Biz_Employee.GetAll().Where(p => employeeForExamPlanRecordQueryable.Select(q => q.EmployeeId).Contains(p.Id)).ToList();
            Sys_Role employeeRole = uow.Sys_Role.GetAll().Where(p => p.RoleType.ToLower() == ROLETYPE_EMPLOYEE.ToLower()).FirstOrDefault();
            string DefaultPassword = System.Web.Configuration.WebConfigurationManager.AppSettings["DefaultPassword"];
            foreach (Biz_Employee employee in employeeList)
            {
                Sys_Account oldAccount = uow.Sys_Account.GetAll().Where(p => p.AccountName == employee.IDNumber).FirstOrDefault();
                if (oldAccount.IsNull())
                {
                    Sys_Account newAccount = new Sys_Account()
                    {
                        AccountName = employee.IDNumber,
                        Password = DefaultPassword,
                        UserId = employee.Id
                    };
                    uow.Sys_Account.Add(newAccount);
                    uow.Commit();
                    //分配到角色
                    Sys_RelAccountRole relAccountRole = new Sys_RelAccountRole()
                    {
                        AccountId = newAccount.Id,
                        RoleId = employeeRole.Id
                    };
                    uow.Sys_RelAccountRole.Add(relAccountRole);
                }
                else
                {
                    oldAccount.UserId = employee.Id;
                    oldAccount.Password = DefaultPassword;
                    uow.Sys_Account.Update(oldAccount);
                }
            }
        }
        //分配准考证号
        void CreateExamRegistrationNumber(int examPlanId)
        {
            List<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordList = uow.Biz_EmployeeForExamPlanRecord.GetAll().Where(p => p.ExamPlanRecordId == examPlanId).ToList();
            string examRegistrationNumber = null;
            foreach (Biz_EmployeeForExamPlanRecord employeeForExamPlanRecord in employeeForExamPlanRecordList)
            {
                examRegistrationNumber = this.GetExamRegistrationNumber(employeeForExamPlanRecord.EmployeeId, examPlanId);
                employeeForExamPlanRecord.ExamRegistrationNumber = examRegistrationNumber;
                uow.Biz_EmployeeForExamPlanRecord.Update(employeeForExamPlanRecord);
                uow.Commit();
            }

        }
        //生成流水号
        string GetExamRegistrationNumber(int employeeId, int examPlanId)
        {
            const int SERIALNUMBER_LENGTH = 4;
            string examRegistrationNumber = "";
            string cityCode = uow.Biz_Employee.GetAll().Where(p => p.Id == employeeId)
                 .Join(uow.Biz_City.GetAll(), a => a.City, b => b.CityName, (a, b) => new { b.CityCode }).FirstOrDefault().CityCode;
            Biz_ExamPlanRecord examplan = uow.Biz_ExamPlanRecord.GetById(examPlanId);
            string examDateStr = examplan.ExamDateTimeBegin.ToString("yyyyMMdd");
            //准考证编号 cityCode+Date(yyyyMMdd)
            examRegistrationNumber = cityCode + examDateStr;
            IQueryable<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordQueryable = uow.Biz_EmployeeForExamPlanRecord.GetAll().Where(p => p.ExamRegistrationNumber.IndexOf(examRegistrationNumber) == 0);
            int ExamRegistrationNumberCnt = employeeForExamPlanRecordQueryable.Count();
            ExamRegistrationNumberCnt = ExamRegistrationNumberCnt + 1;
            string serialNumberStr = ExamRegistrationNumberCnt.ToString();
            //补零
            while (serialNumberStr.Length <= SERIALNUMBER_LENGTH)
            {
                serialNumberStr = "0" + serialNumberStr;
            }
            //准考证编号 cityCode+Date(yyyyMMdd)+流水号
            examRegistrationNumber = examRegistrationNumber + serialNumberStr;
            return examRegistrationNumber;
        }

        public Biz_ExamPlanRecord GetExamPlanRecordById(int examPlanId)
        {
            return uow.Biz_ExamPlanRecord.GetById(examPlanId);
        }
        public List<Biz_ExaminationRoom> GetExamRoomByExamPlanId(int examPlanId)
        {
            IQueryable<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordQueryable = uow.Biz_EmployeeForExamPlanRecord.GetAll().Where(p => p.ExamPlanRecordId == examPlanId);
            IQueryable<Biz_ExaminationRoom> examinationRoomQueryable = uow.Biz_ExaminationRoom.GetAll().Where(p => employeeForExamPlanRecordQueryable.Select(q => q.ExamRoomId).Contains(p.Id));
            IQueryable<Biz_ExaminationRoom> examinationRoomResultQueryable = examinationRoomQueryable.GroupJoin(employeeForExamPlanRecordQueryable, a => a.Id, b => b.ExamRoomId, (a, b) => new { a, b = b.OrderByDescending(p => p.CreateDate).FirstOrDefault() })
                                                            .OrderBy(o => o.b.CreateDate).Select(o => o.a);
            return examinationRoomResultQueryable.ToList();

        }
        public Biz_ExamPlanRecord GetExamPlanRecord(int examPlanId, int employeeId)
        {
            Biz_ExamPlanRecord examPlanRecord = uow.Biz_ExamPlanRecord.GetById(examPlanId);
            Biz_Employee employee = uow.Biz_Employee.GetById(employeeId);
            //补全时间(根据取证类型的不同 每场考试的时长不同)
            List<Biz_PaperForExamType> paperForExamTypeList = uow.Biz_PaperForExamType.GetAll().Where(p => p.Industry == employee.Industry).Where(p => p.ExamType == employee.ExamType).ToList();
            Biz_PaperForExamType safetyKnowledgeExamPaper = paperForExamTypeList.Where(p => p.PaperType == SAFETYKNOWLEDGEEXAM).FirstOrDefault();
            Biz_PaperForExamType managementAbilityExamPaper = paperForExamTypeList.Where(p => p.PaperType == MANAGEMENTABILITYEXAM).FirstOrDefault();
            //第二场考试开始时间距离开始时间的的间隔(分钟)
            int examInterval = Convert.ToInt32(AppFn.GetAppSettingsValue("ExamInterval"));

            examPlanRecord.AExamDateTimeEnd = examPlanRecord.ExamDateTimeBegin.AddMinutes(safetyKnowledgeExamPaper.Duration);
            examPlanRecord.BExamDateTimeBegin = examPlanRecord.ExamDateTimeBegin.AddMinutes(examInterval);

            //examPlanRecord.BExamDateTimeBegin = examPlanRecord.AExamDateTimeEnd.AddMinutes(examInterval);
            //examPlanRecord.ExamDateTimeEnd = examPlanRecord.BExamDateTimeBegin.AddMinutes(managementAbilityExamPaper.Duration);
            return examPlanRecord;

        }
        public Biz_ExamPlanRecord GetExamPlanRecordByExamCoreExamId(int examCoreExamId, int employeeId)
        {

            Biz_ExamPlanRecord examPlan = uow.Biz_ExamPlanRecord.GetAll().Where(p => p.ExamCoreExamId == examCoreExamId).OrderByDescending(p => p.Id).FirstOrDefault();
            examPlan = this.GetExamPlanRecord(examPlan.Id, employeeId);
            return examPlan;
        }
        public List<Biz_ExamPlanRecord> GetExamPlanRecordByIdList(List<int> idList)
        {
            List<Biz_ExamPlanRecord> examPlanRecordList = uow.Biz_ExamPlanRecord.GetAll().Where(p => idList.Contains(p.Id)).ToList();
            return examPlanRecordList;

        }
        public List<Biz_PaperForExamType> GetPaperForExamTypeByExamPlanId(int examPlanId)
        {
            IQueryable<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordQueryable = uow.Biz_EmployeeForExamPlanRecord.GetAll().Where(p => p.ExamPlanRecordId == examPlanId);
            IQueryable<Biz_Employee> employeeQueryable = uow.Biz_Employee.GetAll().Where(q => employeeForExamPlanRecordQueryable.Select(p => p.EmployeeId).Contains(q.Id));
            IQueryable<Biz_PaperForExamType> paperForExamTypeQueryable = uow.Biz_PaperForExamType.GetAll();
            List<Biz_PaperForExamType> paperForExamTypeList = paperForExamTypeQueryable.Join(employeeQueryable, a => new { a.Industry, a.ExamType }, b => new { b.Industry, b.ExamType }, (a, b) => new { a }).Select(p => p.a).Distinct().OrderBy(p => p.SEQ).ToList();
            return paperForExamTypeList;
        }
        #endregion

        #region 开始考试
        //获取离现在最近的一次考试 已考试结束时间为准
        public Biz_ExamPlanRecord GetFirstExamPlanByEmployeeId(int employeeId)
        {
            DateTime currentDate = DateTime.Now;
            IQueryable<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordQueryable = uow.Biz_EmployeeForExamPlanRecord.GetAll().Where(p => p.EmployeeId == employeeId);
            Biz_ExamPlanRecord examPlanRecord = uow.Biz_ExamPlanRecord.GetAll().Where(p => employeeForExamPlanRecordQueryable.Select(q => q.ExamPlanRecordId).Contains(p.Id)).Where(p => p.SubmitStatus == true)
                .Where(p => p.ExamDateTimeEnd >= currentDate)
                .OrderBy(p => p.ExamDateTimeBegin).SingleOrDefault();
            examPlanRecord = GetExamPlanRecord(examPlanRecord.Id, employeeId);
            return examPlanRecord;
        }
        //根据考试计划Id和人员Id 获取考场信息
        public Biz_ExaminationRoom GetExamRoom(int examPlanId, int employeeId)
        {
            IQueryable<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordQueryable = uow.Biz_EmployeeForExamPlanRecord.GetAll().Where(p => p.EmployeeId == employeeId).Where(p => p.EmployeeId == employeeId).Where(p => p.ExamPlanRecordId == examPlanId);
            Biz_ExaminationRoom examRoom = uow.Biz_ExaminationRoom.GetById(employeeForExamPlanRecordQueryable.Single().ExamRoomId);
            return examRoom;
        }
        //public Biz_PaperForExamType GetPaperIdForStartExam(int examPlanId, int employeeId)
        //{
        //    string SAFETYKNOWLEDGEEXAM = "SafetyKnowledgeExam";
        //    string MANAGEMENTABILITYEXAM = "ManagementAbilityExam";
        //    Biz_PaperForExamType paperForExamType = new Biz_PaperForExamType();
        //    Biz_Employee employee = uow.Biz_Employee.GetById(employeeId);
        //    Biz_EmployeeExamResultRecord employeeExamResult = uow.Biz_EmployeeExamResultRecord.GetAll().Where(p => p.EmployeeId == employeeId).Where(p => p.ExamPlanRecordId == examPlanId).SingleOrDefault();
        //    IQueryable<Biz_PaperForExamType> paperForExamTypeQueryable = uow.Biz_PaperForExamType.GetAll().Where(p => p.Industry == employee.Industry).Where(p => p.ExamType == employee.ExamType).OrderBy(p => p.SEQ);
        //    if (employeeExamResult.IsNull())//没有考试成绩
        //    {
        //        paperForExamType = paperForExamTypeQueryable.FirstOrDefault();
        //    }
        //    else//有考试成绩 
        //    {
        //        if (employeeExamResult.SafetyKnowledgeExamScore.IsNull())
        //        {
        //            paperForExamType = paperForExamTypeQueryable.Where(p => p.PaperType == SAFETYKNOWLEDGEEXAM).FirstOrDefault();
        //        }
        //        else if (employeeExamResult.ManagementAbilityExamScore.IsNull())
        //        {
        //            paperForExamType = paperForExamTypeQueryable.Where(p => p.PaperType == MANAGEMENTABILITYEXAM).FirstOrDefault();
        //        }
        //        else
        //        {
        //            paperForExamType = null;
        //        }
        //    }
        //    return paperForExamType;
        //}
        public Biz_PaperForExamType GetPaperIdForStartExam(int examPlanId, int employeeId)
        {

            Biz_PaperForExamType paperForExamType = new Biz_PaperForExamType();
            Biz_Employee employee = uow.Biz_Employee.GetById(employeeId);
            Biz_ExamPlanRecord exam = this.GetExamPlanRecord(examPlanId, employeeId);
            List<Biz_PaperForExamType> paperForExamTypeList = uow.Biz_PaperForExamType.GetAll().Where(p => p.Industry == employee.Industry).Where(p => p.ExamType == employee.ExamType).ToList();
            Biz_PaperForExamType safetyKnowledgeExamPaper = paperForExamTypeList.Where(p => p.PaperType == SAFETYKNOWLEDGEEXAM).FirstOrDefault();
            Biz_PaperForExamType managementAbilityExamPaper = paperForExamTypeList.Where(p => p.PaperType == MANAGEMENTABILITYEXAM).FirstOrDefault();

            if (exam.ExamDateTimeBegin <= DateTime.Now && DateTime.Now < exam.AExamDateTimeEnd)
            {
                paperForExamType = safetyKnowledgeExamPaper;
            }
            else if (exam.BExamDateTimeBegin <= DateTime.Now && DateTime.Now < exam.ExamDateTimeEnd)
            {
                paperForExamType = managementAbilityExamPaper;
            }
            else
            {
                paperForExamType = null;
            }
            return paperForExamType;
        }
        public Biz_PaperForExamType GetPaper(int employeeId, string paperType)
        {
            Biz_Employee employee = uow.Biz_Employee.GetById(employeeId);
            IQueryable<Biz_PaperForExamType> paperForExamTypeQueryable = uow.Biz_PaperForExamType.GetAll().Where(p => p.Industry == employee.Industry && p.ExamType == employee.ExamType && p.PaperType == paperType);
            return paperForExamTypeQueryable.Single();
        }
        public void AddEmployeeExamResultRecord(Biz_EmployeeExamResultRecord employeeExamResultRecord)
        {
            employeeExamResultRecord.CreateDate = DateTime.Now;
            employeeExamResultRecord.CreateById = account.IsNull() ? 0 : account.Id;
            uow.Biz_EmployeeExamResultRecord.Add(employeeExamResultRecord);
            Biz_EmployeeForExamPlanRecord employeeForExamPlanRecord = uow.Biz_EmployeeForExamPlanRecord.GetAll().Where(p => p.EmployeeId == employeeExamResultRecord.EmployeeId).FirstOrDefault();
            employeeForExamPlanRecord.OperationStatus = true;
            uow.Biz_EmployeeForExamPlanRecord.Update(employeeForExamPlanRecord);
            uow.Commit();
        }
        public void SaveExamResult(int examCoreExamId, string idNumber, int paperId, bool examPassedFlag, double score)
        {
            string SAFETYKNOWLEDGEEXAM = "SafetyKnowledgeExam";
            string MANAGEMENTABILITYEXAM = "ManagementAbilityExam";
            //此身份证对应的报名信息
            IQueryable<Biz_Employee> employeeQueryable = uow.Biz_Employee.GetAll().Where(p => p.IDNumber == idNumber);
            //考试计划
            Biz_ExamPlanRecord examPlan = uow.Biz_ExamPlanRecord.GetAll().Where(p => p.ExamCoreExamId == examCoreExamId).OrderByDescending(p => p.Id).FirstOrDefault();
            //考试计划分配记录
            Biz_EmployeeForExamPlanRecord employeeForExamPlanRecord = uow.Biz_EmployeeForExamPlanRecord.GetAll().Where(p => p.ExamPlanRecordId == examPlan.Id).Where(p => employeeQueryable.Select(q => q.Id).Contains(p.EmployeeId)).FirstOrDefault();
            //人的信息
            Biz_Employee employee = employeeQueryable.Where(p => p.Id == employeeForExamPlanRecord.EmployeeId).FirstOrDefault();
            //考试结果
            Biz_EmployeeExamResultRecord oldEmployeeExamResultRecord = uow.Biz_EmployeeExamResultRecord.GetAll().Where(p => p.EmployeeId == employeeForExamPlanRecord.EmployeeId).FirstOrDefault();
            //试卷和人员
            Biz_PaperForExamType paperForExamType = uow.Biz_PaperForExamType.GetAll().Where(p => p.PaperId == paperId).Where(p => p.ExamType == employee.ExamType).Where(p => p.Industry == employee.Industry).FirstOrDefault();

            if (oldEmployeeExamResultRecord.IsNull())//没有考试结果 新增考试结果
            {
                Biz_EmployeeExamResultRecord newEmployeeExamResultRecord = new Biz_EmployeeExamResultRecord();

                newEmployeeExamResultRecord.EmployeeId = employeeForExamPlanRecord.EmployeeId;
                newEmployeeExamResultRecord.ExamPlanRecordId = examPlan.Id;
                if (paperForExamType.PaperType == SAFETYKNOWLEDGEEXAM)
                {
                    //安全知识考试结果

                    newEmployeeExamResultRecord.SafetyKnowledgeExamScore = score;
                }
                if (paperForExamType.PaperType == MANAGEMENTABILITYEXAM)
                {
                    //管理能力考试结果
                    newEmployeeExamResultRecord.ManagementAbilityExamScore = score;
                }
                AddEmployeeExamResultRecord(newEmployeeExamResultRecord);

            }
            else//有考试结果更新考试结果
            {
                if (paperForExamType.PaperType == SAFETYKNOWLEDGEEXAM)
                {
                    //安全知识考试结果

                    oldEmployeeExamResultRecord.SafetyKnowledgeExamScore = score;
                }
                if (paperForExamType.PaperType == MANAGEMENTABILITYEXAM)
                {
                    //管理能力考试结果
                    oldEmployeeExamResultRecord.ManagementAbilityExamScore = score;
                }
                //判定考试成绩
                uow.Biz_EmployeeExamResultRecord.Update(oldEmployeeExamResultRecord);
            }
            uow.Commit();

        }

        public Biz_EmployeeExamResultRecord GetEmployeeExamResultRecord(int examCoreExamId, int employeeId)
        {
            Biz_ExamPlanRecord examPlan = uow.Biz_ExamPlanRecord.GetAll().Where(p => p.ExamCoreExamId == examCoreExamId).FirstOrDefault();
            Biz_EmployeeExamResultRecord employeeExamResultRecord = uow.Biz_EmployeeExamResultRecord.GetAll().Where(p => p.ExamPlanRecordId == examPlan.Id).Where(p => p.EmployeeId == employeeId).FirstOrDefault();
            return employeeExamResultRecord;
        }

        public Biz_Employee GetEmployee(int examId, string idNumber)
        {
            IQueryable<Biz_Employee> employeeQueryable = uow.Biz_Employee.GetAll().Where(p => p.IDNumber == idNumber);
            IQueryable<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordQueryable = uow.Biz_EmployeeForExamPlanRecord.GetAll().Where(p => p.ExamPlanRecordId == examId);
            Biz_Employee employee = employeeQueryable.Where(p => employeeForExamPlanRecordQueryable.Select(q => q.EmployeeId).Contains(p.Id)).FirstOrDefault();
            return employee;
        }
        #endregion

        #region 管理考试结果
        public List<Biz_Employee> EmployeeForExamResultList(string examPlanNumber, string employeeName, string iDNumber, string industry, string examType, string submitStatus, int page, int rows, ref int totalCount)
        {
            //考试计划已经提交的人
            IQueryable<Biz_Employee> employeeQueryable = uow.Biz_Employee.GetAll().Where(p => !p.Invalid);//中心
            //数据权限 考核点只能看到  分配到自己考核点 且 考试计划已经提交 的人
            int trainingInstitutionId = uow.Biz_ExaminationPoint.GetById(account.UserId).Id;
            IQueryable<Biz_ExaminationRoom> examinationRoom = uow.Biz_ExaminationRoom.GetAll().Where(p => p.ExaminationPointId == trainingInstitutionId);
            IQueryable<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordQueryable = uow.Biz_EmployeeForExamPlanRecord.GetAll().Where(p => examinationRoom.Select(q => q.Id).Contains(p.ExamRoomId));
            IQueryable<Biz_ExamPlanRecord> examPlanRecord = uow.Biz_ExamPlanRecord.GetAll().Where(p => p.SubmitStatus == true);
            IQueryable<Biz_EmployeeExamResultRecord> EmployeeExamResultRecordQueryable = uow.Biz_EmployeeExamResultRecord.GetAll();


            if (!examPlanNumber.IsNull())
            {
                examPlanRecord = examPlanRecord.Where(p => p.ExamPlanNumber.Contains(examPlanNumber));
            }
            employeeForExamPlanRecordQueryable = employeeForExamPlanRecordQueryable.Where(p => examPlanRecord.Select(q => q.Id).Contains(p.ExamPlanRecordId));
            employeeQueryable = employeeQueryable.Where(p => employeeForExamPlanRecordQueryable.Select(q => q.EmployeeId).Contains(p.Id));

            // EmployeeExamResultRecordQueryable = EmployeeExamResultRecordQueryable.Where(p => p.SummitStatus == true);
            if (submitStatus == "已提交")
            {
                employeeQueryable = employeeQueryable.Where(p => EmployeeExamResultRecordQueryable.Where(q => q.SummitStatus == true).Select(q => q.EmployeeId).Contains(p.Id));
            }
            else if (submitStatus == "未提交")
            {
                employeeQueryable = employeeQueryable.Where(p => EmployeeExamResultRecordQueryable.Where(q => q.SummitStatus == false).Select(q => q.EmployeeId).Contains(p.Id));
            }

            if (!employeeName.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.EmployeeName.Contains(employeeName));
            }
            if (!iDNumber.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.IDNumber.Contains(iDNumber.Trim()));
            }
            if (!industry.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.Industry == industry);
            }
            if (!examType.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.ExamType == examType);
            }

            totalCount = employeeQueryable.Count();
            int indexBegin = (page - 1) * rows;
            employeeQueryable = employeeQueryable.OrderByDescending(P => P.Id).Skip(indexBegin).Take(rows);
            List<Biz_Employee> employeeList = employeeQueryable.ToList();
            return employeeList;
        }
        public List<Biz_EmployeeExamResultRecord> GetEmployeeExamResultRecordList(List<int> employeeIdList)
        {
            List<Biz_EmployeeExamResultRecord> employeeExamResultRecordList = uow.Biz_EmployeeExamResultRecord.GetAll().Where(p => employeeIdList.Contains(p.EmployeeId)).ToList();
            return employeeExamResultRecordList;
        }
        public List<Biz_EmployeeExamResultRecordFile> GetEmployeeExamResultRecordFileListByEmployeeExamResultRecordId(int employeeExamResultRecordId)
        {
            List<Biz_EmployeeExamResultRecordFile> employeeExamResultRecordFileList = uow.Biz_EmployeeExamResultRecordFile.GetAll().Where(p => p.EmployeeExamResultRecordId == employeeExamResultRecordId).ToList();
            return employeeExamResultRecordFileList;
        }
        public Biz_EmployeeExamResultRecordFile GetEmployeeExamResultImgFileById(int id)
        {
            return uow.Biz_EmployeeExamResultRecordFile.GetById(id);
        }
        public Biz_EmployeeExamResultRecord GetEmployeeExamResultRecord(int employeeId)
        {
            Biz_EmployeeExamResultRecord employeeExamResultRecord = uow.Biz_EmployeeExamResultRecord.GetAll().Where(p => p.EmployeeId == employeeId).SingleOrDefault();
            return employeeExamResultRecord;
        }
        public int SaveEmployeeExamResult(Biz_EmployeeExamResultRecord employeeExamResultRecord)
        {
            int employeeExamResultRecordId = 0;
            Biz_EmployeeExamResultRecord oldEmployeeExamResultRecord = uow.Biz_EmployeeExamResultRecord.GetAll().Where(p => p.EmployeeId == employeeExamResultRecord.EmployeeId).SingleOrDefault();
            if (oldEmployeeExamResultRecord.IsNull())
            {
                // uow.Biz_EmployeeExamResultRecord.Add(employeeExamResultRecord);
                this.AddEmployeeExamResultRecord(employeeExamResultRecord);
            }
            else
            {
                oldEmployeeExamResultRecord.FieldExamResult = employeeExamResultRecord.FieldExamResult;
                uow.Biz_EmployeeExamResultRecord.Update(oldEmployeeExamResultRecord);
            }
            uow.Commit();
            employeeExamResultRecordId = uow.Biz_EmployeeExamResultRecord.GetAll().Where(p => p.EmployeeId == employeeExamResultRecord.EmployeeId).SingleOrDefault().Id;
            return employeeExamResultRecordId;
        }
        public void SaveEmployeeExamResultFile(Biz_EmployeeExamResultRecordFile employeeExamResultRecordFile)
        {
            uow.Biz_EmployeeExamResultRecordFile.Add(employeeExamResultRecordFile);
            uow.Commit();
        }
        public void DeleteEmployeeExamResultFile(int id)
        {
            Biz_EmployeeExamResultRecordFile employeeExamResultRecordFile = uow.Biz_EmployeeExamResultRecordFile.GetById(id);
            uow.Biz_EmployeeExamResultRecordFile.Delete(employeeExamResultRecordFile);
            uow.Commit();
        }
        #endregion

        #region  审核考试结果
        public List<Biz_ExamPlanRecord> GetExamPlanRecordInCheckExamResult(string examPlanNumber, string employeeName, string iDNumber, string industry, string examType, string checkStatus, int page, int rows, ref int totalCount, List<string> cityList)
        {
            //上一流程已提交 的数据
            IQueryable<Biz_EmployeeExamResultRecord> employeeExamResultRecordQueryable = uow.Biz_EmployeeExamResultRecord.GetAll().Where(p => p.SummitStatus == true);
            IQueryable<Biz_Employee> employeeQueryable = uow.Biz_Employee.GetAll().Where(p => !p.Invalid);

            IQueryable<Biz_ExamPlanRecord> examPlanRecordQueryable = uow.Biz_ExamPlanRecord.GetAll();

            //数据权限 那个角色制定的考试计划  就由那个角色审核考试结果
            Sys_Role accountRole = account.RoleList.FirstOrDefault();
            List<int> accountIdList = uow.Sys_RelAccountRole.GetAll().Where(p => p.RoleId == accountRole.Id).Select(p => p.AccountId).ToList();
            examPlanRecordQueryable = examPlanRecordQueryable.Where(p => accountIdList.Contains(p.CreateById));

            if (!examPlanNumber.IsNull())
            {
                examPlanRecordQueryable = examPlanRecordQueryable.Where(p => p.ExamPlanNumber.Contains(examPlanNumber));
            }

            if (!employeeName.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.EmployeeName.Contains(employeeName));
            }
            if (!iDNumber.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.IDNumber.Contains(iDNumber));
            }
            if (!industry.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.Industry.Contains(industry));
            }
            if (!examType.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.ExamType.Contains(examType));
            }
            //找到考试结果记录
            employeeExamResultRecordQueryable = employeeExamResultRecordQueryable.Where(p => employeeQueryable.Select(q => q.Id).Contains(p.EmployeeId));
            if (!checkStatus.IsNull())
            {
                IQueryable<Biz_EmployeeExamResultCheckedRecord> employeeExamResultCheckedRecordQueryable = uow.Biz_EmployeeExamResultCheckedRecord.GetAll();
                if (checkStatus == "审核通过")
                {
                    employeeExamResultCheckedRecordQueryable = employeeExamResultCheckedRecordQueryable.Where(p => p.CheckedStatus == true);
                }
                if (checkStatus == "审核不通过")
                {
                    employeeExamResultCheckedRecordQueryable = employeeExamResultCheckedRecordQueryable.Where(p => p.CheckedStatus == false);
                }
                employeeExamResultRecordQueryable = employeeExamResultRecordQueryable.Where(p => employeeExamResultCheckedRecordQueryable.Select(q => q.EmployeeId).Contains(p.EmployeeId));
            }

            //通过 考试结果记录找到 分配考试计划记录
            IQueryable<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordQueryable =
                uow.Biz_EmployeeForExamPlanRecord.GetAll()
                .Where(p => employeeExamResultRecordQueryable.Select(q => q.EmployeeId).Contains(p.EmployeeId));

            examPlanRecordQueryable = examPlanRecordQueryable.Where(p => employeeForExamPlanRecordQueryable.Select(q => q.ExamPlanRecordId).Contains(p.Id));


            totalCount = examPlanRecordQueryable.Count();
            int indexBegin = (page - 1) * rows;
            examPlanRecordQueryable = examPlanRecordQueryable.OrderByDescending(P => P.ExamPlanNumber).Skip(indexBegin).Take(rows);
            List<Biz_ExamPlanRecord> examPlanRecordLise = examPlanRecordQueryable.ToList();
            return examPlanRecordLise;
        }

        public List<Biz_EmployeeExamResultRecord> GetEmployeeExamResultRecordByExamPlanIdList(List<int> examPlanIdList, bool? submitStatus)
        {
            IQueryable<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordQueryable = uow.Biz_EmployeeForExamPlanRecord.GetAll().Where(p => examPlanIdList.Contains(p.ExamPlanRecordId));
            IQueryable<Biz_EmployeeExamResultRecord> employeeExamResultRecordQueryable = uow.Biz_EmployeeExamResultRecord.GetAll().Where(p => employeeForExamPlanRecordQueryable.Select(q => q.EmployeeId).Contains(p.EmployeeId));
            if (!submitStatus.IsNull())
            {
                employeeExamResultRecordQueryable = employeeExamResultRecordQueryable.Where(p => p.SummitStatus == submitStatus);
            }
            List<Biz_EmployeeExamResultRecord> employeeExamResultRecordList = employeeExamResultRecordQueryable.ToList();
            return employeeExamResultRecordList;
        }
        public List<Biz_Employee> GetEmployeeListInExamResultCheck(int examPlanId, string employeeName, string iDNumber, string industry, string examType, string checkStatus, int page, int rows, ref int totalCount)
        {
            //上一流程已提交 的数据
            IQueryable<Biz_EmployeeExamResultRecord> employeeExamResultRecordQueryable = uow.Biz_EmployeeExamResultRecord.GetAll().Where(p => p.SummitStatus == true).Where(p => p.ExamPlanRecordId == examPlanId);
            IQueryable<Biz_Employee> employeeQueryable = uow.Biz_Employee.GetAll();
            IQueryable<Biz_EmployeeExamResultCheckedRecord> employeeExamResultCheckedRecordQueryable = uow.Biz_EmployeeExamResultCheckedRecord.GetAll();
            if (!employeeName.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.EmployeeName.Contains(employeeName));
            }
            if (!iDNumber.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.IDNumber.Contains(iDNumber));
            }
            if (!industry.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.Industry.Contains(industry));
            }
            if (!examType.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.ExamType.Contains(examType));
            }
            if (!checkStatus.IsNull())
            {
                if (checkStatus == "已审核")
                {
                    employeeExamResultRecordQueryable = employeeExamResultRecordQueryable.Where(p => p.OperationStatus == true);
                }
                else if (checkStatus == "未审核")
                {
                    employeeExamResultRecordQueryable = employeeExamResultRecordQueryable.Where(p => p.OperationStatus == false);
                }
                else if (checkStatus == "审核通过")
                {
                    employeeExamResultCheckedRecordQueryable = employeeExamResultCheckedRecordQueryable.Where(p => p.CheckedStatus == true);
                }
                else if (checkStatus == "审核未通过")
                {
                    employeeExamResultCheckedRecordQueryable = employeeExamResultCheckedRecordQueryable.Where(p => p.CheckedStatus == true);
                }
                employeeQueryable = employeeQueryable.Where(p => employeeExamResultCheckedRecordQueryable.Select(q => q.EmployeeId).Contains(p.Id));//有审核记录
            }
            employeeQueryable = employeeQueryable.Where(p => employeeExamResultRecordQueryable.Select(q => q.EmployeeId).Contains(p.Id));//上个流程提交的人




            totalCount = employeeQueryable.Count();
            int indexBegin = (page - 1) * rows;
            employeeQueryable = employeeQueryable.OrderByDescending(P => P.Id).Skip(indexBegin).Take(rows);
            List<Biz_Employee> employeeList = employeeQueryable.ToList();
            return employeeList;
        }
        public List<Biz_EmployeeExamResultCheckedRecord> GetEmployeeExamResultCheckedRecordListByEmployeeIdList(List<int> employeeIdList, bool? checkedStatus)
        {
            IQueryable<Biz_EmployeeExamResultCheckedRecord> employeeExamResultCheckedRecordQueryable = uow.Biz_EmployeeExamResultCheckedRecord.GetAll().Where(p => employeeIdList.Contains(p.EmployeeId));
            if (!checkedStatus.IsNull())
            {
                employeeExamResultCheckedRecordQueryable = employeeExamResultCheckedRecordQueryable.Where(p => p.CheckedStatus == checkedStatus);
            }
            List<Biz_EmployeeExamResultCheckedRecord> EmployeeExamResultCheckedRecordList = employeeExamResultCheckedRecordQueryable.ToList();

            return EmployeeExamResultCheckedRecordList;
        }
        public void CheckByEmployee(int employeeId, bool passStatus, string checkedMark, string startCertificateDate)
        {
            List<int> employeeIdList = new List<int>() { employeeId };
            this.CheckedExamResult(employeeIdList, passStatus, checkedMark, startCertificateDate);
        }
        public void CheckByExamPlan(int examPlanId, bool passStatus, string checkedMark, string startCertificateDate)
        {
            //考试计划分配记录
            IQueryable<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordQueryable = uow.Biz_EmployeeForExamPlanRecord.GetAll().Where(p => p.ExamPlanRecordId == examPlanId);
            //考试结果记录 已提交的
            IQueryable<Biz_EmployeeExamResultRecord> employeeExamResultRecordQueryable = uow.Biz_EmployeeExamResultRecord.GetAll().Where(p => employeeForExamPlanRecordQueryable.Select(q => q.EmployeeId).Contains(p.EmployeeId)).Where(p => p.SummitStatus == true);
            ////本次考试的总人数
            //int employeeForExamPlanRecordCnt = employeeForExamPlanRecordQueryable.Count();
            ////有考试结果的人数
            //int employeeExamResultRecordCnt = employeeExamResultRecordQueryable.Count();
            //if (employeeForExamPlanRecordCnt != employeeExamResultRecordCnt)
            //{
            //    throw new Exception("本次考试不是所有人都有考试结果");
            //}

            IQueryable<Biz_EmployeeExamResultCheckedRecord> employeeExamResultCheckedRecord = uow.Biz_EmployeeExamResultCheckedRecord.GetAll().Where(p => p.ExamPlanRecordId == examPlanId);
            //当前考试计划没有被审核的人
            List<int> employeeIdList = employeeExamResultRecordQueryable.Where(p => !employeeExamResultCheckedRecord.Select(q => q.EmployeeId).Contains(p.EmployeeId)).Select(p => p.EmployeeId).ToList();
            if (employeeIdList.Count < 1)
            {
                throw new Exception("本考试计划提交的考试结果已经全部被审核");
            }
            this.CheckedExamResult(employeeIdList, passStatus, checkedMark, startCertificateDate);
        }
        //生成证书编号
        public string GetCertificateNo(Biz_Employee employee)
        {
            int SERIALNUMBER_LENGTH = 7;
            string certificateNo = "鄂建安" + employee.ExamType + "（" + DateTime.Now.Year + "）";
            IQueryable<Biz_Certificate> CertificateQueryable = uow.Biz_Certificate.GetAll().Where(p => p.CertificateNo.Contains(certificateNo));
            int certCount = CertificateQueryable.Count();
            certCount += 1;
            string serialNumber = certCount.ToString();
            if (serialNumber.Length > SERIALNUMBER_LENGTH)
            {
                throw new Exception("证书编号流水号 溢出");
            }
            while (serialNumber.Length < SERIALNUMBER_LENGTH)
            {
                serialNumber = "0" + serialNumber;
            }
            certificateNo = certificateNo + serialNumber;
            return certificateNo;
        }
        void SaveCertificate(Biz_Employee employee, DateTime startCertificateDate)
        {
            const int CERTIFICATETIMESPAN = 3;
            IEmployeeCtrl employeeCtrl = new EmployeeCtrl(this.account);
            Biz_EmployeeFile photoFile = employeeCtrl.GetEmployeePhoto(employee.Id);
            if (photoFile == null)
            {
                throw new Exception("照片文件不存在");
            }
            Biz_Enterprise enterprise = uow.Biz_Enterprise.GetById(employee.EnterpriseId);
            Biz_Certificate certificate = new Biz_Certificate()
            {
                // EmployeeId = 0,
                EmployeeName = employee.EmployeeName,
                Sex = employee.Sex,
                Birthday = employee.Birthday.ToString("yyyy-MM-dd"),
                IDNumber = employee.IDNumber,
                EnterpriseName = enterprise.EnterpriseName,
                Job = employee.Job,
                ExamType = employee.ExamType,
                Industry = employee.Industry,
                Title4Technical = employee.Title4Technical,
                CertificateNo = this.GetCertificateNo(employee),
                StartCertificateDate = startCertificateDate,
                EndCertificateDate = startCertificateDate.AddMonths(CERTIFICATETIMESPAN * 12),
                PhotoPath = photoFile.FilePath
            };
            ICertificateCtrl certificateManagementCtrl = new CertificateCtrl(account);
            certificateManagementCtrl.AddCertificate(certificate);
            Biz_RelEmployeeCertificate relEmployeeCertificate = new Biz_RelEmployeeCertificate()
            {
                EmployeeId = employee.Id,
                CertificateId = certificate.Id
            };
            uow.Biz_RelEmployeeCertificate.Add(relEmployeeCertificate);
        }
        #endregion

        #region 证书打印
        public List<Biz_Employee> GetEmployeeListInPrintCertificate(string employeeName, string idNumber, string examType, string industry, string examPlanNumber, string enterpriseName, string trainingInstutionName, bool? IsPrinted)
        {
            //人员报名记录有效 
            IQueryable<Biz_Employee> employeeQueryable = uow.Biz_Employee.GetAll().Where(p => !p.Invalid);
            //有证书的人
            IQueryable<Biz_Certificate> certificateQueryable = uow.Biz_Certificate.GetAll().Where(p => !p.Invalid);
            //考试计划相关表
            IQueryable<Biz_ExamPlanRecord> examPlanRecordQueryable = uow.Biz_ExamPlanRecord.GetAll();
            IQueryable<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordQueryable = uow.Biz_EmployeeForExamPlanRecord.GetAll();
            //企业表
            IQueryable<Biz_Enterprise> enterpriseQueryable = uow.Biz_Enterprise.GetAll();

            //数据权限 显示本角色制定的考试计划
            Sys_Role accountRole = account.RoleList.FirstOrDefault();
            List<int> accountIdList = uow.Sys_RelAccountRole.GetAll().Where(p => p.RoleId == accountRole.Id).Select(p => p.AccountId).ToList();
            examPlanRecordQueryable = examPlanRecordQueryable.Where(p => accountIdList.Contains(p.CreateById));

            //人员报名信息条件筛选
            if (!employeeName.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.EmployeeName.Contains(employeeName));
            }
            if (!idNumber.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.IDNumber.Contains(idNumber));
            }
            if (!examType.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.ExamType.Contains(examType));
            }
            if (!industry.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.Industry.Contains(industry));
            }
            //企业信息筛选
            if (!enterpriseName.IsNull())
            {
                enterpriseQueryable = enterpriseQueryable.Where(p => p.EnterpriseName.Contains(enterpriseName));
            }
            //考试计划信息筛选
            if (!examPlanNumber.IsNull())
            {
                examPlanRecordQueryable = examPlanRecordQueryable.Where(p => p.ExamPlanNumber.Contains(examPlanNumber));
            }
            if (!trainingInstutionName.IsNull())
            {
                IQueryable<Biz_ExaminationPoint> trainingInstitution = uow.Biz_ExaminationPoint.GetAll().Where(p => p.InstitutionName.Contains(trainingInstutionName));
                IQueryable<Biz_ExaminationRoom> examroomQueryable = uow.Biz_ExaminationRoom.GetAll().Where(p => trainingInstitution.Select(q => q.Id).Contains(p.ExaminationPointId));
                employeeForExamPlanRecordQueryable = employeeForExamPlanRecordQueryable.Where(p => examroomQueryable.Select(q => q.Id).Contains(p.ExamRoomId));
            }
            //是否显示已打印的信息
            if (!IsPrinted.IsNull())
            {
                IQueryable<Biz_CertificatePrintRecord> certificatePrintRecordQueryable = uow.Biz_CertificatePrintRecord.GetAll();
                if (IsPrinted == true)//只显示已打印的人
                {
                    certificateQueryable = certificateQueryable.Where(p => certificatePrintRecordQueryable.Select(q => q.CertificateNo).Contains(p.CertificateNo));
                }
                else if (IsPrinted == false)//只显示未打印的人
                {
                    certificateQueryable = certificateQueryable.Where(p => !certificatePrintRecordQueryable.Select(q => q.CertificateNo).Contains(p.CertificateNo));
                }
            }

            List<Biz_Employee> employeeList = employeeQueryable.Join(enterpriseQueryable, a => a.EnterpriseId, b => b.Id, (a, b) => new { a, b })
                                              .Join(employeeForExamPlanRecordQueryable, o => o.a.Id, c => c.EmployeeId, (o, c) => new { o.a, o.b, c })
                                              .Join(examPlanRecordQueryable, o => o.c.ExamPlanRecordId, d => d.Id, (o, d) => new { o.a, o.b, o.c, d })
                                              .Join(certificateQueryable, o => new { o.a.ExamType, o.a.Industry, o.a.IDNumber }, e => new { e.ExamType, e.Industry, e.IDNumber }, (o, e) => new { o.a, o.b, o.c, o.d, e })
                                              .OrderByDescending(o => new { o.d.ExamPlanNumber, o.b.EnterpriseName, o.a.ExamType, o.a.Industry, o.a.Id })
                                              .Select(o => o.a).ToList();
            return employeeList;
        }
        public List<Biz_Certificate> GetEmployeeListInPrintCertificate(string employeeName, string idNumber, string examType, string industry, string examPlanNumber, string enterpriseName, string trainingInstutionName, bool? IsPrinted, int page, int rows, ref int totalCount)
        {
            //证书表
            IQueryable<Biz_Certificate> certificateQueryable = uow.Biz_Certificate.GetAll().Where(p => !p.Invalid);
            //考试计划相关表
            IQueryable<Biz_ExamPlanRecord> examPlanRecordQueryable = uow.Biz_ExamPlanRecord.GetAll();
            //考试计划与人员关联表
            IQueryable<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordQueryable = uow.Biz_EmployeeForExamPlanRecord.GetAll();

            //数据权限 显示本角色制定的考试计划


            if (account.RoleList.Where(p => p.RoleType == RoleCtrl.RoleType.Admin).Count() > 0)
            {

            }
            else if (account.RoleList.Where(p => p.RoleType == RoleCtrl.RoleType.Master).Count() > 0)//省总站不看测试企业的证书
            {
                certificateQueryable = certificateQueryable.Where(p => !p.EnterpriseName.Contains("测试"));
            }
            else if (account.RoleList.Where(p => p.RoleType == RoleCtrl.RoleType.Manager).Count() > 0)//管理部门显示整个部门制定的考试计划通过 取得的证书
            {
                Sys_Role accountRole = account.RoleList.Where(p => p.RoleType == RoleCtrl.RoleType.Manager).Single();
                List<int> accountIdList = uow.Sys_RelAccountRole.GetAll().Where(p => p.RoleId == accountRole.Id).Select(p => p.AccountId).ToList();
                examPlanRecordQueryable = examPlanRecordQueryable.Where(p => accountIdList.Contains(p.CreateById));
            }
            else if (account.RoleList.Where(p => p.RoleType == RoleCtrl.RoleType.Enterprise).Count() > 0)
            {
                Biz_Enterprise enterprise = uow.Biz_Enterprise.GetById(account.UserId);
                certificateQueryable = certificateQueryable.Where(p => p.EnterpriseName == enterprise.EnterpriseName);
            }
            else
            {
                certificateQueryable = certificateQueryable.Where(p => 1 == 2);
            }
            //人员报名信息条件筛选
            if (!employeeName.IsNull())
            {
                certificateQueryable = certificateQueryable.Where(p => p.EmployeeName.Contains(employeeName));
            }
            if (!idNumber.IsNull())
            {
                certificateQueryable = certificateQueryable.Where(p => p.IDNumber.Contains(idNumber));
            }
            if (!examType.IsNull())
            {
                certificateQueryable = certificateQueryable.Where(p => p.ExamType == examType);
            }
            if (!industry.IsNull())
            {
                certificateQueryable = certificateQueryable.Where(p => p.Industry.Contains(industry));
            }
            //企业信息筛选
            if (!enterpriseName.IsNull())
            {
                certificateQueryable = certificateQueryable.Where(p => p.EnterpriseName.Contains(enterpriseName));
            }
            //考试计划信息筛选
            if (!examPlanNumber.IsNull())
            {
                examPlanRecordQueryable = examPlanRecordQueryable.Where(p => p.ExamPlanNumber.Contains(examPlanNumber));
            }
            if (!trainingInstutionName.IsNull())
            {
                IQueryable<Biz_ExaminationPoint> trainingInstitution = uow.Biz_ExaminationPoint.GetAll().Where(p => p.InstitutionName.Contains(trainingInstutionName));
                IQueryable<Biz_ExaminationRoom> examroomQueryable = uow.Biz_ExaminationRoom.GetAll().Where(p => trainingInstitution.Select(q => q.Id).Contains(p.ExaminationPointId));
                employeeForExamPlanRecordQueryable = employeeForExamPlanRecordQueryable.Where(p => examroomQueryable.Select(q => q.Id).Contains(p.ExamRoomId));
            }
            //是否显示已打印的信息
            if (!IsPrinted.IsNull())
            {
                IQueryable<Biz_CertificatePrintRecord> certificatePrintRecordQueryable = uow.Biz_CertificatePrintRecord.GetAll();
                if (IsPrinted == true)//只显示已打印的人
                {
                    certificateQueryable = certificateQueryable.Where(p => certificatePrintRecordQueryable.Select(q => q.CertificateNo).Contains(p.CertificateNo));
                }
                else if (IsPrinted == false)//只显示未打印的人
                {
                    certificateQueryable = certificateQueryable.Where(p => !certificatePrintRecordQueryable.Select(q => q.CertificateNo).Contains(p.CertificateNo));
                }
            }

            certificateQueryable = certificateQueryable
                .Join(uow.Biz_RelEmployeeCertificate.GetAll(), a1 => a1.Id, b1 => b1.CertificateId, (a1, b1) => new { a1, b1, })
                .Join(employeeForExamPlanRecordQueryable, o => o.b1.EmployeeId, b => b.EmployeeId, (o, b) => new { a = o.a1, b })
                                              .Join(examPlanRecordQueryable, o => o.b.ExamPlanRecordId, c => c.Id, (o, c) => new { o.a, o.b, c })
                                              .OrderByDescending(o => new { o.c.ExamPlanNumber, o.a.EnterpriseName, o.a.ExamType, o.a.Industry, o.a.Id })
                                              .Select(o => o.a);
            totalCount = certificateQueryable.Count();
            int indexBegin = (page - 1) * rows;
            certificateQueryable = certificateQueryable.Skip(indexBegin).Take(rows);
            List<Biz_Certificate> certificateList = certificateQueryable.ToList();
            return certificateList;
        }
        public List<Biz_Certificate> GetCertificateList(List<Biz_Employee> employeeList)
        {
            IQueryable<Biz_Certificate> certificateQueryable = uow.Biz_Certificate.GetAll();
            List<Biz_Certificate> certificateList = certificateQueryable.Join(certificateQueryable, a => new { a.IDNumber, a.ExamType, a.Industry }, b => new { b.IDNumber, b.ExamType, b.Industry }, (a, b) => new { a, b }).Select(p => p.a).ToList();
            return certificateList;
        }
        public List<Biz_CertificatePrintRecord> GetCertificatePrintRecordList(List<string> certificateNoList)
        {
            IQueryable<Biz_CertificatePrintRecord> certificatePrintRecordQueryable = uow.Biz_CertificatePrintRecord.GetAll();
            List<Biz_CertificatePrintRecord> certificatePrintRecordList = certificatePrintRecordQueryable.Where(p => certificateNoList.Contains(p.CertificateNo)).ToList();
            return certificatePrintRecordList;
        }
        public void SaveCertificatePrintRecord(Biz_CertificatePrintRecord certificatePrintRecord)
        {
            certificatePrintRecord.CreateById = account.Id;
            certificatePrintRecord.CreateDate = DateTime.Now;
            uow.Biz_CertificatePrintRecord.Add(certificatePrintRecord);
            uow.Commit();
        }

        public Biz_Certificate GetCertificateByEmployeeId(int employeeId)
        {
            IQueryable<Biz_Certificate> certificateQueryable = uow.Biz_Certificate.GetAll().Join(uow.Biz_RelEmployeeCertificate.GetAll(), a => a.Id, b => b.CertificateId, (a, b) => new { a, b }).Where(o => o.b.EmployeeId == employeeId).Select(o => o.a);
            return certificateQueryable.SingleOrDefault();
        }
        #endregion

        #region 证书发放
        public List<Biz_ExamPlanRecord> getExamPlanListInCertificateIssuance(string examPlanNumber, string trainingInsititutionName, string issuanceStatus, string issuanceDateTimeBegin, string issuanceDateTimeEnd, int page, int rows, ref int totalCount, List<string> cityList)
        {
            IQueryable<Biz_ExamPlanRecord> examplanQueryable = uow.Biz_ExamPlanRecord.GetAll();
            //数据权限 那个角色制定的考试计划  就由那个角色审核考试结果
            Sys_Role accountRole = account.RoleList.FirstOrDefault();
            List<int> accountIdList = uow.Sys_RelAccountRole.GetAll().Where(p => p.RoleId == accountRole.Id).Select(p => p.AccountId).ToList();
            examplanQueryable = examplanQueryable.Where(p => accountIdList.Contains(p.CreateById));

            IQueryable<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordQueryable = uow.Biz_EmployeeForExamPlanRecord.GetAll();
            IQueryable<Biz_ExaminationRoom> examRoomQueryable = uow.Biz_ExaminationRoom.GetAll();
            IQueryable<Biz_ExaminationPoint> trainingInstitution = uow.Biz_ExaminationPoint.GetAll();
            //考试结果审核记录
            IQueryable<Biz_EmployeeExamResultCheckedRecord> employeeExamResultCheckedRecordQueryable = uow.Biz_EmployeeExamResultCheckedRecord.GetAll().Where(p => p.CheckedStatus == true);

            if (!examPlanNumber.IsNull())
            {
                examplanQueryable = examplanQueryable.Where(p => p.ExamPlanNumber == examPlanNumber);
            }
            if (!trainingInsititutionName.IsNull())
            {
                trainingInstitution = trainingInstitution.Where(p => p.InstitutionName.Contains(trainingInsititutionName));
            }
            if (issuanceStatus == "已发放")
            {
                employeeExamResultCheckedRecordQueryable = employeeExamResultCheckedRecordQueryable.Where(p => p.OperationStatus == true);
            }
            else if (issuanceStatus == "未发放")
            {
                employeeExamResultCheckedRecordQueryable = employeeExamResultCheckedRecordQueryable.Where(p => p.OperationStatus == false);
            }

            if (!issuanceDateTimeBegin.IsNull() || !issuanceDateTimeEnd.IsNull())
            {
                IQueryable<Biz_EmployeeCertificateIssuanceRecord> employeeCertificateIssuanceRecord = uow.Biz_EmployeeCertificateIssuanceRecord.GetAll();
                if (!issuanceDateTimeBegin.IsNull())
                {
                    DateTime dtBegin = issuanceDateTimeBegin.ConvertToDateTime();
                    employeeCertificateIssuanceRecord = employeeCertificateIssuanceRecord.Where(p => dtBegin <= p.CreateDate);
                }
                if (!issuanceDateTimeEnd.IsNull())
                {
                    DateTime dtEnd = issuanceDateTimeEnd.ConvertToDateTime();
                    employeeCertificateIssuanceRecord = employeeCertificateIssuanceRecord.Where(p => p.CreateDate <= dtEnd);
                }
                employeeExamResultCheckedRecordQueryable = employeeExamResultCheckedRecordQueryable.Where(p => employeeCertificateIssuanceRecord.Select(q => q.EmployeeId).Contains(p.EmployeeId));
            }

            employeeForExamPlanRecordQueryable = employeeForExamPlanRecordQueryable.Where(p => employeeExamResultCheckedRecordQueryable.Select(q => q.EmployeeId).Contains(p.EmployeeId));





            examplanQueryable = examplanQueryable.Join(employeeForExamPlanRecordQueryable, a => a.Id, b => b.ExamPlanRecordId, (a, b) => new { a, b })
                          .Join(examRoomQueryable, c => c.b.ExamRoomId, d => d.Id, (c, d) => new { c, d })
                          .Join(trainingInstitution, e => e.d.ExaminationPointId, f => f.Id, (e, f) => new { e, f })
                          .GroupBy(p => new { p.e.c.a, p.f }).Select(p => p.Key.a);

            int indexBegin = (page - 1) * rows;
            examplanQueryable = examplanQueryable.OrderByDescending(p => p.ExamPlanNumber).Skip(indexBegin).Take(rows);
            totalCount = examplanQueryable.Count();
            List<Biz_ExamPlanRecord> examPlanRecordList = uow.Biz_ExamPlanRecord.GetAll().Where(p => examplanQueryable.Select(q => q.Id).Contains(p.Id)).ToList();
            return examPlanRecordList;
        }
        //获取需要发放证书的人
        public List<Biz_Employee> GetEmployeeListInCertificateIssuance(List<int> employeeIdList)
        {
            IQueryable<Biz_Employee> employeeQueryable = uow.Biz_Employee.GetAll().Where(p => employeeIdList.Contains(p.Id)).Where(p => !p.Invalid).Where(p => p.PrintCertificate == true);
            IQueryable<Biz_Certificate> certificateQueryable = uow.Biz_Certificate.GetAll().Where(p => !p.Invalid);

            employeeQueryable = employeeQueryable.Join(certificateQueryable, a => new { a.IDNumber, a.Industry, a.ExamType }, b => new { b.IDNumber, b.Industry, b.ExamType }, (a, b) => new { a, b }).Select(o => o.a);
            List<Biz_Employee> employeeList = employeeQueryable.ToList();
            return employeeList;
        }
        //发证记录
        public List<Biz_EmployeeCertificateIssuanceRecord> GetEmployeeCertificateIssuanceRecordList(List<int> employeeIdList)
        {
            List<Biz_EmployeeCertificateIssuanceRecord> employeeCertificateIssuanceRecordList = uow.Biz_EmployeeCertificateIssuanceRecord.GetAll().Where(p => employeeIdList.Contains(p.EmployeeId)).ToList();
            return employeeCertificateIssuanceRecordList;
        }

        public void IssuanceByExamPlan(int examPlanId, int trainingInsititutionId, string remark)
        {
            //信息有效且需要打印证书的人
            IQueryable<Biz_Employee> employeeQueryable = uow.Biz_Employee.GetAll().Where(p => p.Invalid == false && p.PrintCertificate == true);
            //当前考核点和计划的人员记录
            IQueryable<Biz_ExaminationRoom> examRoomQueryable = uow.Biz_ExaminationRoom.GetAll().Where(p => p.ExaminationPointId == trainingInsititutionId);
            IQueryable<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordQueryable = uow.Biz_EmployeeForExamPlanRecord.GetAll()
                .Where(p => examRoomQueryable.Select(q => q.Id).Contains(p.ExamRoomId))
                .Where(p => p.ExamPlanRecordId == examPlanId);
            //证书和人员关联表
            IQueryable<Biz_RelEmployeeCertificate> relEmployeeCertificateQueryable = uow.Biz_RelEmployeeCertificate.GetAll();
            //已经发证的记录
            IQueryable<Biz_EmployeeCertificateIssuanceRecord> employeeCertificateIssuanceRecordQueryable = uow.Biz_EmployeeCertificateIssuanceRecord.GetAll();

            //结果集为 报名人员有效信息 且 需要打证 且 有证书的人 且  当前考试计划和当前学校 且 没有发放证书的人员
            employeeQueryable = employeeQueryable.Where(p => relEmployeeCertificateQueryable.Select(q => q.EmployeeId).Contains(p.Id))
                              .Where(p => employeeForExamPlanRecordQueryable.Select(q => q.EmployeeId).Contains(p.Id))
                              .Where(p => employeeCertificateIssuanceRecordQueryable.Select(q => q.EmployeeId).Contains(p.Id) != true);
            this.IssuanceCertificate(employeeQueryable.Select(p => p.Id).ToList(), examPlanId, trainingInsititutionId, remark);
        }
        public void IssuanceByEmployee(int employeeId, int examPlanId, int trainingInsititutionId, string remark)
        {
            this.IssuanceCertificate(new List<int>() { employeeId }, examPlanId, trainingInsititutionId, remark);
        }
        #endregion

        #region 获取人员对应的流程状态
        public WorkFlowStatus GetCurrentWorkFlowStatus(int employeeId)
        {
            WorkFlowStatus workFlowStatus = new WorkFlowStatus();

            //人员证书发放登记
            {
                Biz_EmployeeCertificateIssuanceRecord employeeCertificateIssuance = uow.Biz_EmployeeCertificateIssuanceRecord.GetAll().Where(p => p.EmployeeId == employeeId).FirstOrDefault();
                if (!employeeCertificateIssuance.IsNull())
                {
                    workFlowStatus.WorkFlowStatusTag = WorkFlowStatusTag.CertificateIssued;
                    workFlowStatus.CreateDate = employeeCertificateIssuance.CreateDate;
                    workFlowStatus.CreateById = employeeCertificateIssuance.CreateById;
                    workFlowStatus.Sequence = 9;
                    return workFlowStatus;
                }
            }
            //考试结果审核 状态包括 考核通过 考核未通过
            {
                Biz_EmployeeExamResultCheckedRecord employeeExamResultCheckedRecord = uow.Biz_EmployeeExamResultCheckedRecord.GetAll().Where(p => p.EmployeeId == employeeId).FirstOrDefault();
                if (!employeeExamResultCheckedRecord.IsNull())
                {
                    if (employeeExamResultCheckedRecord.CheckedStatus)
                    {
                        workFlowStatus.WorkFlowStatusTag = WorkFlowStatusTag.ExamPassed;
                    }
                    else
                    {
                        workFlowStatus.WorkFlowStatusTag = WorkFlowStatusTag.ExamUnpassed;
                    }
                    workFlowStatus.CreateDate = employeeExamResultCheckedRecord.CreateDate;
                    workFlowStatus.CreateById = employeeExamResultCheckedRecord.CreateById;
                    workFlowStatus.Sequence = 8;
                    return workFlowStatus;
                }
            }
            //管理考试结果
            {
                Biz_EmployeeExamResultRecord employeeExamResultRecord = uow.Biz_EmployeeExamResultRecord.GetAll().Where(p => p.EmployeeId == employeeId && p.SummitStatus == true).FirstOrDefault();
                if (!employeeExamResultRecord.IsNull())
                {
                    workFlowStatus.WorkFlowStatusTag = WorkFlowStatusTag.ExamResultSummit;
                    workFlowStatus.CreateDate = employeeExamResultRecord.SummitDate.ConvertToDateTime();
                    workFlowStatus.CreateById = employeeExamResultRecord.SummitById.ConvertToInt();
                    workFlowStatus.Sequence = 7;
                    return workFlowStatus;
                }
            }

            //制定考试计划 状态包括 考试计划已制定 考试计划已提交
            {
                Biz_EmployeeForExamPlanRecord employeeForExamPlanRecord = uow.Biz_EmployeeForExamPlanRecord.GetAll().Where(p => p.EmployeeId == employeeId).FirstOrDefault();
                if (!employeeForExamPlanRecord.IsNull())
                {
                    Biz_ExamPlanRecord examPlanRecord = uow.Biz_ExamPlanRecord.GetAll().Where(p => p.Id == employeeForExamPlanRecord.ExamPlanRecordId).FirstOrDefault();
                    workFlowStatus.WorkFlowStatusTag = examPlanRecord.SubmitStatus ? WorkFlowStatusTag.SubmitExamPlan : WorkFlowStatusTag.MakeExamPlan;
                    workFlowStatus.CreateDate = employeeForExamPlanRecord.CreateDate;
                    workFlowStatus.CreateById = employeeForExamPlanRecord.CreateById;
                    workFlowStatus.Sequence = examPlanRecord.SubmitStatus ? 6 : 5;
                    return workFlowStatus;
                }
            }
            //资料审核
            {
                Biz_EmployeeCheckedRecord employeeCheckedRecord = uow.Biz_EmployeeCheckedRecord.GetAll().Where(p => p.EmployeeId == employeeId).FirstOrDefault();
                if (!employeeCheckedRecord.IsNull())
                {
                    if (employeeCheckedRecord.PassStatus)
                    {
                        workFlowStatus.WorkFlowStatusTag = WorkFlowStatusTag.CheckPassed;
                    }
                    else
                    {
                        workFlowStatus.WorkFlowStatusTag = WorkFlowStatusTag.CheckUnpassed;
                    }
                    workFlowStatus.CreateDate = employeeCheckedRecord.CreateDate;
                    workFlowStatus.CreateById = employeeCheckedRecord.CreateById;
                    workFlowStatus.Sequence = 4;
                    return workFlowStatus;
                }
            }

            //培训
            {
                Biz_TrainingRecord trainingRecord = uow.Biz_TrainingRecord.GetAll().Where(p => p.EmployeeId == employeeId).FirstOrDefault();
                if (!trainingRecord.IsNull())
                {
                    if (trainingRecord.PassStatus)
                    {
                        workFlowStatus.WorkFlowStatusTag = WorkFlowStatusTag.TrainingPassed;
                    }
                    else
                    {
                        workFlowStatus.WorkFlowStatusTag = WorkFlowStatusTag.TrainingUnPassed;
                    }
                    workFlowStatus.CreateDate = trainingRecord.CreateDate;
                    workFlowStatus.CreateById = trainingRecord.CreateById;
                    workFlowStatus.Sequence = 3;
                    return workFlowStatus;


                }
            }
            //报名
            {
                Biz_Employee employee = uow.Biz_Employee.GetAll().Where(p => p.Id == employeeId).FirstOrDefault();
                if (!employee.IsNull())
                {
                    if (employee.SubmitStatus)
                    {
                        workFlowStatus.WorkFlowStatusTag = WorkFlowStatusTag.RegisterSummit;
                        workFlowStatus.CreateDate = employee.SummitDate.ConvertToDateTime();
                        workFlowStatus.CreateById = employee.SummitById.ConvertToInt();
                        workFlowStatus.Sequence = 2;
                    }
                    else
                    {
                        workFlowStatus.WorkFlowStatusTag = WorkFlowStatusTag.RegisterDraft;
                        workFlowStatus.CreateDate = employee.CreateDate;
                        workFlowStatus.CreateById = employee.CreateById;
                        workFlowStatus.Sequence = 1;
                    }

                    return workFlowStatus;
                }
            }
            throw new Exception("异常的取证人员考核流程状态。");
        }

        public List<WorkFlowStatus> GetCurrentWorkFlowStatusByEmployeeIdList(List<int> employeeIdList)
        {
            List<WorkFlowStatus> WorkFlowStatusListResult = new List<WorkFlowStatus>();
            //人员证书发放登记
            {
                List<Biz_EmployeeCertificateIssuanceRecord> employeeCertificateIssuanceList = uow.Biz_EmployeeCertificateIssuanceRecord.GetAll().Where(p => employeeIdList.Contains(p.EmployeeId)).ToList();
                if (employeeCertificateIssuanceList.Count() > 0)
                {
                    List<WorkFlowStatus> WorkFlowStatusList = employeeCertificateIssuanceList.Select(p => new WorkFlowStatus()
                    {
                        employeeId = p.EmployeeId,
                        WorkFlowStatusTag = WorkFlowStatusTag.CertificateIssued,
                        CreateDate = p.CreateDate,
                        CreateById = p.CreateById,
                        Sequence = 9
                    }).ToList();

                    WorkFlowStatusListResult.AddRange(WorkFlowStatusList);
                    employeeIdList.RemoveAll(p => WorkFlowStatusList.Select(q => q.employeeId).Contains(p));
                }
            }
            //考试结果审核 状态包括 考核通过 考核未通过
            {
                List<Biz_EmployeeExamResultCheckedRecord> employeeExamResultCheckedRecordList = uow.Biz_EmployeeExamResultCheckedRecord.GetAll().Where(p => employeeIdList.Contains(p.EmployeeId)).ToList();
                if (employeeExamResultCheckedRecordList.Count() > 0)
                {

                    List<WorkFlowStatus> WorkFlowStatusList = employeeExamResultCheckedRecordList.Select(p => new WorkFlowStatus()
                    {
                        employeeId = p.EmployeeId,
                        WorkFlowStatusTag = p.CheckedStatus ? WorkFlowStatusTag.ExamPassed : WorkFlowStatusTag.ExamUnpassed,
                        CreateDate = p.CreateDate,
                        CreateById = p.CreateById,
                        Sequence = 8
                    }).ToList();

                    WorkFlowStatusListResult.AddRange(WorkFlowStatusList);
                    employeeIdList.RemoveAll(p => WorkFlowStatusList.Select(q => q.employeeId).Contains(p));
                }
            }
            //管理考试结果
            {
                List<Biz_EmployeeExamResultRecord> employeeExamResultRecordList = uow.Biz_EmployeeExamResultRecord.GetAll().Where(p => employeeIdList.Contains(p.EmployeeId) && p.SummitStatus == true).ToList();
                if (employeeExamResultRecordList.Count() > 0)
                {

                    List<WorkFlowStatus> WorkFlowStatusList = employeeExamResultRecordList.Select(p => new WorkFlowStatus()
                    {
                        employeeId = p.EmployeeId,
                        WorkFlowStatusTag = WorkFlowStatusTag.ExamResultSummit,
                        CreateDate = p.SummitDate.ConvertToDateTime(),
                        CreateById = p.SummitById.ConvertToInt(),
                        Sequence = 7
                    }).ToList();

                    WorkFlowStatusListResult.AddRange(WorkFlowStatusList);
                    employeeIdList.RemoveAll(p => WorkFlowStatusList.Select(q => q.employeeId).Contains(p));
                }
            }

            //提交考试计划
            {
                List<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordList = uow.Biz_EmployeeForExamPlanRecord.GetAll()
                    .Join(uow.Biz_ExamPlanRecord.GetAll(), p => p.ExamPlanRecordId, q => q.Id, (p, q) => new { p, q })
                    .Where(o => o.q.SubmitStatus == true)
                    .Select(o => o.p)
                    .Where(p => employeeIdList.Contains(p.EmployeeId)).ToList();

                if (employeeForExamPlanRecordList.Count() > 0)
                {
                    List<WorkFlowStatus> WorkFlowStatusList = employeeForExamPlanRecordList.Select(p => new WorkFlowStatus()
                    {
                        employeeId = p.EmployeeId,
                        WorkFlowStatusTag = WorkFlowStatusTag.SubmitExamPlan,
                        CreateDate = p.CreateDate,
                        CreateById = p.CreateById,
                        Sequence = 6
                    }).ToList();
                    WorkFlowStatusListResult.AddRange(WorkFlowStatusList);
                    employeeIdList.RemoveAll(p => WorkFlowStatusList.Select(q => q.employeeId).Contains(p));
                }
            }

            //制定考试计划
            {
                List<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordList = uow.Biz_EmployeeForExamPlanRecord.GetAll()
                    .Join(uow.Biz_ExamPlanRecord.GetAll(), p => p.ExamPlanRecordId, q => q.Id, (p, q) => new { p, q })
                    .Where(o => o.q.SubmitStatus == false)
                    .Select(o => o.p)
                    .Where(p => employeeIdList.Contains(p.EmployeeId)).ToList();

                if (employeeForExamPlanRecordList.Count() > 0)
                {
                    List<WorkFlowStatus> WorkFlowStatusList = employeeForExamPlanRecordList.Select(p => new WorkFlowStatus()
                    {
                        employeeId = p.EmployeeId,
                        WorkFlowStatusTag = WorkFlowStatusTag.MakeExamPlan,
                        CreateDate = p.CreateDate,
                        CreateById = p.CreateById,
                        Sequence = 5
                    }).ToList();
                    WorkFlowStatusListResult.AddRange(WorkFlowStatusList);
                    employeeIdList.RemoveAll(p => WorkFlowStatusList.Select(q => q.employeeId).Contains(p));
                }
            }
            //资料审核
            {
                List<Biz_EmployeeCheckedRecord> employeeCheckedRecordList = uow.Biz_EmployeeCheckedRecord.GetAll().Where(p => employeeIdList.Contains(p.EmployeeId)).ToList();
                if (employeeCheckedRecordList.Count() > 0)
                {

                    List<WorkFlowStatus> WorkFlowStatusList = employeeCheckedRecordList.Select(p => new WorkFlowStatus()
                    {
                        employeeId = p.EmployeeId,
                        WorkFlowStatusTag = p.PassStatus ? WorkFlowStatusTag.CheckPassed : WorkFlowStatusTag.CheckUnpassed,
                        CreateDate = p.CreateDate,
                        CreateById = p.CreateById,
                        Sequence = 4
                    }).ToList();
                    WorkFlowStatusListResult.AddRange(WorkFlowStatusList);
                    employeeIdList.RemoveAll(p => WorkFlowStatusList.Select(q => q.employeeId).Contains(p));
                }
            }
            //培训
            {
                List<Biz_TrainingRecord> trainingRecordList = uow.Biz_TrainingRecord.GetAll().Where(p => employeeIdList.Contains(p.EmployeeId)).ToList();
                if (trainingRecordList.Count() > 0)
                {
                    List<WorkFlowStatus> WorkFlowStatusList = trainingRecordList.Select(p => new WorkFlowStatus()
                    {
                        employeeId = p.EmployeeId,
                        WorkFlowStatusTag = p.PassStatus == true ? WorkFlowStatusTag.TrainingPassed : WorkFlowStatusTag.TrainingUnPassed,
                        CreateDate = p.CreateDate,
                        CreateById = p.CreateById,
                        Sequence = 3,
                    }).ToList();
                    WorkFlowStatusListResult.AddRange(WorkFlowStatusList);
                    employeeIdList.RemoveAll(p => WorkFlowStatusList.Select(q => q.employeeId).Contains(p));

                }
            }

            //报名
            {
                List<Biz_Employee> employeeList = uow.Biz_Employee.GetAll().Where(p => employeeIdList.Contains(p.Id)).ToList();
                if (employeeList.Count() > 0)
                {

                    List<WorkFlowStatus> WorkFlowStatusList = employeeList.Select(p => new WorkFlowStatus()
                    {
                        employeeId = p.Id,
                        WorkFlowStatusTag = p.SubmitStatus ? WorkFlowStatusTag.RegisterSummit : WorkFlowStatusTag.RegisterDraft,
                        CreateDate = p.SubmitStatus ? p.SummitDate.ConvertToDateTime() : p.CreateDate,
                        CreateById = p.SubmitStatus ? p.SummitById.ConvertToInt() : p.CreateById,
                        Sequence = p.SubmitStatus ? 2 : 1,
                    }).ToList();
                    WorkFlowStatusListResult.AddRange(WorkFlowStatusList);
                    employeeIdList.RemoveAll(p => WorkFlowStatusList.Select(q => q.employeeId).Contains(p));

                }
            }
            if (employeeIdList.Count() > 0)
            {
                throw new Exception("异常的取证人员考核流程状态");
            }
            return WorkFlowStatusListResult;

        }
        #endregion

        #region 根据查询条件返回取证人员的IQueryable对象

        public IQueryable<Biz_Employee> getEmployeeQueryableByParam(string employeeName, string IdNumber, string examType, string industry, string workflowStatus,
            string institutionName, string enterpriseName, string socialCreditCode)
        {
            IQueryable<Biz_Employee> employeeQueryable = uow.Biz_Employee.GetAll();
            if (!employeeName.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.EmployeeName.Contains(employeeName));
            }
            if (!IdNumber.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.IDNumber.Contains(IdNumber));
            }
            if (!examType.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.ExamType == examType);
            }
            if (!industry.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.Industry == industry);
            }

            if (!institutionName.IsNull())
            {
                IQueryable<Biz_TrainingInstitution> trainingInstitutionQueryable = uow.Biz_TrainingInstitution.GetAll()
                    .Where(x => x.InstitutionName == institutionName);
                employeeQueryable = employeeQueryable.Join(trainingInstitutionQueryable, p => p.TrainingInstitutionId, q => q.Id, (p, q) => p);
            }

            IQueryable<Biz_Enterprise> enterpriseQueryable = uow.Biz_Enterprise.GetAll();

            if (!enterpriseName.IsNull())
            {
                enterpriseQueryable = enterpriseQueryable.Where(p => p.EnterpriseName.Contains(enterpriseName));
            }

            if (!socialCreditCode.IsNull())
            {
                enterpriseQueryable = enterpriseQueryable.Where(p => p.SocialCreditCode.Contains(socialCreditCode));
            }

            if (!enterpriseName.IsNull() || !socialCreditCode.IsNull())
            {
                employeeQueryable = employeeQueryable.Join(enterpriseQueryable, p => p.EnterpriseId, q => q.Id, (p, q) => p);
            }

            if (!workflowStatus.IsNull())
            {
                if (workflowStatus == WorkFlowStatusTag.RegisterDraft)
                {
                    employeeQueryable = employeeQueryable.Where(p => p.SubmitStatus == false && p.OperationStatus == false);
                }

                if (workflowStatus == WorkFlowStatusTag.RegisterSummit)
                {
                    employeeQueryable = employeeQueryable.Where(p => p.SubmitStatus == true && p.OperationStatus == false);
                }


                IQueryable<Biz_TrainingRecord> trainingRecordQueryable = uow.Biz_TrainingRecord.GetAll();

                if (workflowStatus == WorkFlowStatusTag.TrainingPassed)
                {
                    trainingRecordQueryable = trainingRecordQueryable.Where(p => p.PassStatus == true && p.OperationStatus == false);
                    employeeQueryable = employeeQueryable.Join(trainingRecordQueryable, p => p.Id, q => q.EmployeeId, (p, q) => p);
                }

                if (workflowStatus == WorkFlowStatusTag.TrainingUnPassed)
                {
                    trainingRecordQueryable = trainingRecordQueryable.Where(p => p.PassStatus == false && p.OperationStatus == false);
                    employeeQueryable = employeeQueryable.Join(trainingRecordQueryable, p => p.Id, q => q.EmployeeId, (p, q) => p);
                }

                IQueryable<Biz_EmployeeCheckedRecord> employeeCheckedRecordQueryable = uow.Biz_EmployeeCheckedRecord.GetAll();

                if (workflowStatus == WorkFlowStatusTag.CheckUnpassed)
                {
                    employeeCheckedRecordQueryable = employeeCheckedRecordQueryable.Where(p => p.PassStatus == false && p.OperationStatus == false);
                    employeeQueryable = employeeQueryable.Join(employeeCheckedRecordQueryable, p => p.Id, q => q.EmployeeId, (p, q) => p);
                }

                if (workflowStatus == WorkFlowStatusTag.CheckPassed)
                {
                    employeeCheckedRecordQueryable = employeeCheckedRecordQueryable.Where(p => p.PassStatus == true && p.OperationStatus == false);
                    employeeQueryable = employeeQueryable.Join(employeeCheckedRecordQueryable, p => p.Id, q => q.EmployeeId, (p, q) => p);
                }

                IQueryable<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordQueryable = uow.Biz_EmployeeForExamPlanRecord.GetAll();

                if (workflowStatus == WorkFlowStatusTag.MakeExamPlan)
                {
                    employeeForExamPlanRecordQueryable = employeeForExamPlanRecordQueryable.Where(p => p.OperationStatus == false)
                        .Join(uow.Biz_ExamPlanRecord.GetAll().Where(x => x.SubmitStatus == false), p => p.ExamPlanRecordId, q => q.Id, (p, q) => p);

                    //employeeQueryable = employeeQueryable.Where(p => p.OperationStatus == false)
                    //    .Join(employeeForExamPlanRecordQueryable, p => p.Id, q => q.EmployeeId, (p, q) => p);
                    employeeQueryable = employeeQueryable.Where(p => p.OperationStatus == true)
                       .Join(employeeForExamPlanRecordQueryable, p => p.Id, q => q.EmployeeId, (p, q) => p);
                }

                if (workflowStatus == WorkFlowStatusTag.SubmitExamPlan)
                {
                    employeeForExamPlanRecordQueryable = employeeForExamPlanRecordQueryable
                        .Join(uow.Biz_ExamPlanRecord.GetAll().Where(x => x.SubmitStatus == true), p => p.ExamPlanRecordId, q => q.Id, (p, q) => p);

                    //employeeQueryable = employeeQueryable.Where(p => p.OperationStatus == false)
                    //    .Join(employeeForExamPlanRecordQueryable, p => p.Id, q => q.EmployeeId, (p, q) => p);
                    employeeQueryable = employeeQueryable.Where(p => p.OperationStatus == true)
                       .Join(employeeForExamPlanRecordQueryable, p => p.Id, q => q.EmployeeId, (p, q) => p);
                }

                if (workflowStatus == WorkFlowStatusTag.ExamResultSummit)
                {
                    //IQueryable<Biz_EmployeeExamResultRecord> employeeExamResultRecordQueryable = uow.Biz_EmployeeExamResultRecord.GetAll()
                    //    .Where(p => p.OperationStatus == false);
                    IQueryable<Biz_EmployeeExamResultRecord> employeeExamResultRecordQueryable = uow.Biz_EmployeeExamResultRecord.GetAll()
                      .Where(p => p.SummitStatus == true);
                    employeeQueryable = employeeQueryable.Join(employeeExamResultRecordQueryable, p => p.Id, q => q.EmployeeId, (p, q) => p);
                }


                IQueryable<Biz_EmployeeExamResultCheckedRecord> employeeExamResultCheckedRecordQueryable = uow.Biz_EmployeeExamResultCheckedRecord.GetAll();
                if (workflowStatus == WorkFlowStatusTag.ExamPassed)
                {
                    employeeExamResultCheckedRecordQueryable = employeeExamResultCheckedRecordQueryable.Where(p => p.OperationStatus == false && p.CheckedStatus == true);
                    employeeQueryable = employeeQueryable.Join(employeeExamResultCheckedRecordQueryable, p => p.Id, q => q.EmployeeId, (p, q) => p);
                }

                if (workflowStatus == WorkFlowStatusTag.ExamUnpassed)
                {
                    employeeExamResultCheckedRecordQueryable = employeeExamResultCheckedRecordQueryable.Where(p => p.OperationStatus == false && p.CheckedStatus == false);
                    employeeQueryable = employeeQueryable.Join(employeeExamResultCheckedRecordQueryable, p => p.Id, q => q.EmployeeId, (p, q) => p);
                }

                if (workflowStatus == WorkFlowStatusTag.CertificateIssued)
                {
                    IQueryable<Biz_EmployeeCertificateIssuanceRecord> employeeCertificateIssuanceRecordQueryable = uow.Biz_EmployeeCertificateIssuanceRecord.GetAll();
                    employeeQueryable = employeeQueryable.Join(employeeCertificateIssuanceRecordQueryable, p => p.Id, q => q.EmployeeId, (p, q) => p);
                }
            }

            return employeeQueryable;

        }


        #endregion

        #region 考核点查看分配给自己的考试计划部分

        public List<Biz_ExamPlanRecord> GetExamPlanRecordListByTrainingInstitutionId(int trainingInstitutionId)
        {
            List<Biz_ExamPlanRecord> examPlanRecordList = uow.Biz_EmployeeForExamPlanRecord.GetAll()
                .Join(uow.Biz_ExamPlanRecord.GetAll(), a => a.ExamPlanRecordId, b => b.Id, (a, b) => new { a, b })
                .Where(x => x.b.SubmitStatus && x.b.ExamDateTimeEnd > DateTime.Now)
                .Join(uow.Biz_ExaminationRoom.GetAll(), x => x.a.ExamRoomId, c => c.Id, (x, c) => new { x, c })
                .Where(y => y.c.ExaminationPointId == trainingInstitutionId).Select(y => y.x.b).Distinct().ToList();

            return examPlanRecordList;

        }

        public List<Biz_ExaminationRoom> GetExaminationRoomListByTrainingInstitutionId(int trainingInstitutionId)
        {
            List<Biz_ExaminationRoom> examinationRoomList = uow.Biz_ExaminationRoom.GetAll().Where(p => p.ExaminationPointId == trainingInstitutionId).ToList();
            return examinationRoomList;

        }

        public Biz_Employee GetEmployeeForExamRoom(string idCardNumber, int examPlanId, int examRoomId)
        {
            Biz_Employee employee = uow.Biz_EmployeeForExamPlanRecord.GetAll()
                .Join(uow.Biz_Employee.GetAll(), a => a.EmployeeId, b => b.Id, (a, b) => new { a, b })
                .Where(o => o.a.ExamPlanRecordId == examPlanId && o.a.ExamRoomId == examRoomId && o.b.IDNumber == idCardNumber)
                .Select(o => o.b).FirstOrDefault();

            return employee;

        }

        #endregion

        #region 考试计划提醒
        public List<Biz_ExamPlanRecord> GetExamPlanListForRemind()
        {
            IQueryable<Biz_ExamPlanRecord> examPlanRecordQueryable = uow.Biz_ExamPlanRecord.GetAll();

            int managerCount = account.RoleList.Where(p => p.RoleType == "Manager").Count();
            if (managerCount > 0)
            {
                examPlanRecordQueryable = examPlanRecordQueryable.Where(p => p.CreateById == account.Id);
            }
            //如果是省总站  不显示测试市的数据
            int masterCount = account.RoleList.Where(p => p.RoleType == RoleCtrl.RoleType.Master).Count();
            if (masterCount > 0)
            {
                examPlanRecordQueryable = examPlanRecordQueryable.Where(p => !p.ExamPlanNumber.Contains("T"));
            }

            List<Biz_ExamPlanRecord> examPlanRecordList = examPlanRecordQueryable.ToList();
            return examPlanRecordList;
        }

        public Biz_ExamPlanRecord GetExamPlanByExamPlanNumber(string examPlanNumber)
        {
            Biz_ExamPlanRecord examPlan = uow.Biz_ExamPlanRecord.GetAll().Where(p => p.ExamPlanNumber == examPlanNumber).Single();
            return examPlan;
        }

        #endregion
    }
    public static class WorkFlowStatusTag
    {
        public static string RegisterDraft { get { return "报名未提交"; } }
        public static string RegisterSummit { get { return "报名已提交"; } }
        public static string TrainingPassed { get { return "培训审核通过"; } }
        public static string TrainingUnPassed { get { return "培训审核未通过"; } }
        public static string CheckUnpassed { get { return "报名确认未通过"; } }
        public static string CheckPassed { get { return "报名确认通过"; } }
        public static string MakeExamPlan { get { return "考试计划已制定"; } }
        public static string SubmitExamPlan { get { return "考试计划已提交"; } }
        public static string ExamResultSummit { get { return "考核结果已提交"; } }
        public static string ExamPassed { get { return "考核结果审核通过"; } }
        public static string ExamUnpassed { get { return "考核结果审核未通过"; } }
        public static string CertificateIssued { get { return "证书已发放"; } }
    }

    public class WorkFlowStatus
    {
        public int employeeId { get; set; }
        public string WorkFlowStatusTag { get; set; }
        public int Sequence { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateById { get; set; }

    }

}
