using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using DAL;
using Library.baseFn;
using EntityFramework.Extensions;

namespace BLL
{
    public class RP_EmployeeCtrl : IRP_EmployeeCtrl
    {
        private Uow uow;
        private Sys_Account loginAccount;
        public RP_EmployeeCtrl(Sys_Account account)
        {
            if (uow == null)
            {
                uow = new Uow();
            }
            this.loginAccount = account;
        }

        List<Biz_Certificate> GetCertificateListForDelayed(string employeeName, string idNumber, string enterpsirseName, string trainingInstitutionName, string workFlowStatus, int page, int rows, ref int totalCount)
        {
            IQueryable<Biz_Certificate> certificateQueryable = uow.Biz_Certificate.GetAll().Where(p => p.Invalid == false);
            certificateQueryable.GroupJoin(uow.Biz_RP_EmployeeRegistration.GetAll().Where(p => p.Invalid == false), a => a.CertificateNo, b => b.OldCertificateNo, (a, b) => new { a, b = b.OrderByDescending(x => x.SummitDate).FirstOrDefault() });
            return null;
        }

        #region 获取报名人员List
        public List<Biz_RP_EmployeeRegistration> GetEmployeeList(string employeeName, string idNumber, string trainingType, string examType, string industry, string workFlowStatus, int page, int rows, ref int totalCount)
        {
            IQueryable<Biz_RP_EmployeeRegistration> employeeQueryable = uow.Biz_RP_EmployeeRegistration.GetAll().Where(p => !p.Invalid);
            //数据权限  各企业看自己的人员
            employeeQueryable = employeeQueryable.Where(p => p.EnterpriseId == loginAccount.UserId);
            if (!employeeName.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.EmployeeName.Contains(employeeName));
            }
            if (!idNumber.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.IDNumber == idNumber);
            }
            if (!examType.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.ExamType == examType);
            }
            if (!industry.IsNull())
            {
                employeeQueryable = employeeQueryable.Where(p => p.Industry == industry);
            }
            if (workFlowStatus == "已提交")
            {
                employeeQueryable = employeeQueryable.Where(p => p.SubmitStatus);
            }
            else if (workFlowStatus == "未提交")
            {
                employeeQueryable = employeeQueryable.Where(p => p.SubmitStatus == false);
            }

            totalCount = employeeQueryable.Count();

            int indexBegin = (page - 1) * rows;
            List<Biz_RP_EmployeeRegistration> rp_employeeList = employeeQueryable.OrderByDescending(P => P.Id).Skip(indexBegin).Take(rows).ToList();
            return rp_employeeList;
        }
        #endregion

        #region 删除报名信息
        public void DeleteRP_EmployeeRegistrationById(int rp_employeeRegistrationId)
        {
            Biz_RP_EmployeeRegistration employee = uow.Biz_RP_EmployeeRegistration.GetById(rp_employeeRegistrationId);
            if (employee.SubmitStatus)
            {
                throw new Exception("人员信息已提交,不能删除");
            }
            uow.Biz_RP_EmployeeRegistration.Delete(employee);
            uow.Commit();
        }
        #endregion

        #region 获取报名信息
        public Biz_RP_EmployeeRegistration GetRP_EmployeeRegistrationById(int id)
        {
            Biz_RP_EmployeeRegistration rpemployee = uow.Biz_RP_EmployeeRegistration.GetById(id);
            return rpemployee;
        }
        #endregion
        public List<Biz_RP_EmployeeRegistration> GetRP_EmployeeRegistrationByIdList(List<int> idList)
        {
            IQueryable<Biz_RP_EmployeeRegistration> rpEmployeeRegistrationQueryable = uow.Biz_RP_EmployeeRegistration.GetAll().Where(p => idList.Contains(p.Id));
            return rpEmployeeRegistrationQueryable.ToList();
        }

        #region 更新报名信息
        public void UpdateEmployeeRegistrationInfo(Biz_RP_EmployeeRegistration rp_employee)
        {

            IRP_WorkFlowCtrl rpWorkFlowCtrl = new RP_WorkFlowCtrl(this.loginAccount);
            Biz_RP_EmployeeRegistration oldRPEmployee = uow.Biz_RP_EmployeeRegistration.GetById(rp_employee.Id);
            RP_WorkFlowCtrl.WorkFlowStatus workFlowStatus = rpWorkFlowCtrl.GetCurrentWorkFlowStatus(oldRPEmployee.Id);

            if (oldRPEmployee.SubmitStatus == true)//已提交的 重新注册
            {
                uow.Biz_RP_EmployeeRegistration.GetAll().Where(p => p.Id == oldRPEmployee.Id).Update(p => new Biz_RP_EmployeeRegistration() { Invalid = true });
                Biz_RP_EmployeeRegistration newEmployee = new Biz_RP_EmployeeRegistration()
                {
                    EmployeeName = oldRPEmployee.EmployeeName,
                    TrainingInstitutionId = oldRPEmployee.TrainingInstitutionId,
                    Sex = oldRPEmployee.Sex,
                    Birthday = oldRPEmployee.Birthday,
                    IDNumber = oldRPEmployee.IDNumber,
                    Job = oldRPEmployee.Job,
                    Title4Technical = oldRPEmployee.Title4Technical,
                    City = oldRPEmployee.City,

                    ExamType = oldRPEmployee.ExamType,
                    Industry = oldRPEmployee.Industry,
                    Remark = oldRPEmployee.Remark,
                    PrintCertificate = oldRPEmployee.PrintCertificate,
                    OldCertificateNo = oldRPEmployee.OldCertificateNo,
                    StartCertificateDate = oldRPEmployee.StartCertificateDate,
                    EndCertificateDate = oldRPEmployee.EndCertificateDate,
                    CreateById = this.loginAccount.Id,
                    CreateDate = DateTime.Now,
                    EnterpriseId = this.loginAccount.UserId,
                    SubmitStatus = false,
                    OperationStatus = false,
                    Invalid = false
                };
                rpWorkFlowCtrl.RegisterRPEmployee(newEmployee);
            }
            else//未提交 可以修改信息
            {
                oldRPEmployee.EmployeeName = rp_employee.EmployeeName;
                oldRPEmployee.TrainingInstitutionId = rp_employee.TrainingInstitutionId;
                oldRPEmployee.Sex = rp_employee.Sex;
                oldRPEmployee.Birthday = rp_employee.Birthday;
                oldRPEmployee.IDNumber = rp_employee.IDNumber;
                oldRPEmployee.Job = rp_employee.Job;
                oldRPEmployee.Title4Technical = rp_employee.Title4Technical;
                oldRPEmployee.City = rp_employee.City;
                oldRPEmployee.ExamType = rp_employee.ExamType;
                oldRPEmployee.Industry = rp_employee.Industry;
                oldRPEmployee.Remark = rp_employee.Remark;
                oldRPEmployee.PrintCertificate = rp_employee.PrintCertificate;
                oldRPEmployee.OldCertificateNo = rp_employee.OldCertificateNo;
                oldRPEmployee.StartCertificateDate = rp_employee.StartCertificateDate;
                oldRPEmployee.EndCertificateDate = rp_employee.EndCertificateDate;
                VerifyRPEmployeeRegistration(oldRPEmployee);
                uow.Biz_RP_EmployeeRegistration.Update(oldRPEmployee);
            }
            uow.Commit();
        }
        #endregion

        #region 验证报名信息
        public void VerifyRPEmployeeRegistration(Biz_RP_EmployeeRegistration rp_employeeRegistration)
        {
            //验证证书是否有未走完的流程
            IQueryable<Biz_RP_EmployeeRegistration> employeeExistQueryable = uow.Biz_RP_EmployeeRegistration.GetAll().Where(p => !p.Invalid).Where(p => p.Id != rp_employeeRegistration.Id && p.OldCertificateNo == rp_employeeRegistration.OldCertificateNo);
            List<int> rpEmployeeIdList = employeeExistQueryable.Select(p => p.Id).ToList();
            IRP_WorkFlowCtrl rpWorkFlowCtrl = new RP_WorkFlowCtrl(this.loginAccount);
            List<RP_WorkFlowCtrl.WorkFlowStatus> workFlowStatusList = rpWorkFlowCtrl.GetCurrentWorkFlowStatusByEmployeeIdList(rpEmployeeIdList);
            int existsInRPWorkFlowCount = workFlowStatusList.Where(p => p.WorkFlowStatusTag != RP_WorkFlowCtrl.WorkFlowStatusTag.CheckUnpassed && p.WorkFlowStatusTag != RP_WorkFlowCtrl.WorkFlowStatusTag.DataCheckUnpassed).Count();

            if (existsInRPWorkFlowCount > 0)
            {
                throw new Exception("该证书正在继续教育流程中");
            }
        }
        #endregion

        #region 获取资料审核报名记录List
        public List<Biz_RP_EmployeeRegistration> GetEmployeeListForDataCheck(string enterpriseName, string employeeName, string idNumber, string examType, string industry, string checkStatus, bool? photograph, int page, int rows, ref int totalCount)
        {
            IQueryable<Biz_RP_EmployeeRegistration> rpEmployeeQueryable = uow.Biz_RP_EmployeeRegistration.GetAll().Where(p => p.Invalid == false);
            //数据权限
            //培训机构看 属于自己的继续教育人员
            rpEmployeeQueryable = rpEmployeeQueryable.Where(p => p.TrainingInstitutionId == loginAccount.UserId);

            if (!enterpriseName.IsNull())
            {
                IQueryable<Biz_Enterprise> enterpriseNameQueryable = uow.Biz_Enterprise.GetAll().Where(p => p.EnterpriseName.Contains(enterpriseName));
                rpEmployeeQueryable = rpEmployeeQueryable.Where(p => enterpriseNameQueryable.Select(q => q.Id).Contains(p.EnterpriseId));
            }
            if (!employeeName.IsNull())
            {
                rpEmployeeQueryable = rpEmployeeQueryable.Where(p => p.EmployeeName.Contains(employeeName));
            }
            if (!idNumber.IsNull())
            {
                rpEmployeeQueryable = rpEmployeeQueryable.Where(p => p.IDNumber == idNumber);
            }
            if (!examType.IsNull())
            {
                rpEmployeeQueryable = rpEmployeeQueryable.Where(p => p.ExamType == examType);
            }
            if (!industry.IsNull())
            {
                rpEmployeeQueryable = rpEmployeeQueryable.Where(p => p.Industry == industry);
            }
            if (!checkStatus.IsNull())
            {
                if (checkStatus == "未审核")
                {
                    rpEmployeeQueryable = rpEmployeeQueryable.Where(p => p.OperationStatus == false);
                }
                else
                {
                    IQueryable<Biz_RP_EmployeeDataCheckedRecord> dataCheckedRecordQueryable = uow.Biz_RP_EmployeeDataCheckedRecord.GetAll();
                    if (checkStatus == "审核通过")
                    {
                        dataCheckedRecordQueryable = dataCheckedRecordQueryable.Where(p => p.PassStatus == true);
                    }
                    else if (checkStatus == "审核不通过")
                    {
                        dataCheckedRecordQueryable = dataCheckedRecordQueryable.Where(p => p.PassStatus == false);
                    }
                    rpEmployeeQueryable = rpEmployeeQueryable.Where(p => dataCheckedRecordQueryable.Select(q => q.EmployeeId).Contains(p.Id));
                }
            }

            if (!photograph.IsNull())
            {
                IQueryable<Biz_RP_EmployeeFile> rpEmployeeFileQueryable = uow.Biz_RP_EmployeeFile.GetAll().Where(p => p.FileType == "登记照");
                rpEmployeeQueryable.Where(p => rpEmployeeFileQueryable.Select(q => q.RPEmployeeId).Contains(p.Id) == photograph);
            }

            totalCount = rpEmployeeQueryable.Count();
            int indexBegin = (page - 1) * rows;
            List<Biz_RP_EmployeeRegistration> rp_employeeList = rpEmployeeQueryable.OrderByDescending(P => P.Id).Skip(indexBegin).Take(rows).ToList();
            return rp_employeeList;
        }
        #endregion

        #region 获取资料审核记录
        public List<Biz_RP_EmployeeDataCheckedRecord> GetEmployeeDataCheckedRecordListByRPEmployeeIdList(List<int> rpEmployeeIdList)
        {
            IQueryable<Biz_RP_EmployeeDataCheckedRecord> dataCheckedRecordQueryable = uow.Biz_RP_EmployeeDataCheckedRecord.GetAll().Where(p => rpEmployeeIdList.Contains(p.EmployeeId));
            return dataCheckedRecordQueryable.ToList();
        }
        #endregion

        #region 获取成绩审核人员记录
        public List<Biz_RP_EmployeeRegistration> GetEmployeeListForViewStudyRecordCheck(
            string employeeName, string enterpriseName, string trainingInstitutionName, string idNumber, string oldCertificateNo,
            string examType, string checkSatus, string checkDateBegin, string checkDateEnd, int page, int rows, ref int totalCount, List<string> cityList)
        {
            IQueryable<Biz_RP_EmployeeRegistration> rpemployeeQueryable = uow.Biz_RP_EmployeeRegistration.GetAll().Where(p => !p.Invalid);
            //资料审核已通过的人
            IQueryable<Biz_RP_EmployeeDataCheckedRecord> rpEmployeeDataCheckedRecord = uow.Biz_RP_EmployeeDataCheckedRecord.GetAll().Where(q => q.PassStatus == true);
            rpemployeeQueryable = rpemployeeQueryable.Where(p => rpEmployeeDataCheckedRecord.Select(q => q.EmployeeId).Contains(p.Id));

            #region 数据权限
            if (loginAccount.RoleList.Where(p => p.RoleType == AccountCtrl.RoleType.Master).Count() > 0)//省管理部门
            {
                //看所有
            }
            else if (loginAccount.RoleList.Where(p => p.RoleType == AccountCtrl.RoleType.Admin).Count() > 0)//管理员
            {
                //看所有
            }
            else if (loginAccount.RoleList.Where(p => p.RoleType == AccountCtrl.RoleType.Manager).Count() > 0)  //市管理部门看本市的
            {
                rpemployeeQueryable = rpemployeeQueryable.Where(p => cityList.Contains(p.City));
            }
            else if (loginAccount.RoleList.Where(p => p.RoleType == AccountCtrl.RoleType.Enterprise).Count() > 0)//企业看自己的
            {
                rpemployeeQueryable = rpemployeeQueryable.Where(p => p.EnterpriseId == loginAccount.UserId);
            }
            else//其他角色都看不到
            {
                rpemployeeQueryable = rpemployeeQueryable.Where(p => 1 > 2);
            }
            #endregion

            if (!employeeName.IsNull())
            {
                rpemployeeQueryable = rpemployeeQueryable.Where(p => p.EmployeeName.Contains(employeeName));
            }
            if (!enterpriseName.IsNull())
            {
                IQueryable<Biz_Enterprise> enterpriseQueryable = uow.Biz_Enterprise.GetAll().Where(p => p.EnterpriseName.Contains(enterpriseName));
                rpemployeeQueryable = rpemployeeQueryable.Where(p => enterpriseQueryable.Select(q => q.Id).Contains(p.EnterpriseId));
            }
            if (!trainingInstitutionName.IsNull())
            {
                IQueryable<Biz_TrainingInstitution> trainingInstitutionQueryable = uow.Biz_TrainingInstitution.GetAll().Where(p => p.InstitutionName.Contains(trainingInstitutionName));
                rpemployeeQueryable = rpemployeeQueryable.Where(p => trainingInstitutionQueryable.Select(q => q.Id).Contains(p.TrainingInstitutionId));
            }
            if (!idNumber.IsNull())
            {
                rpemployeeQueryable = rpemployeeQueryable.Where(p => p.IDNumber.Contains(idNumber));
            }
            if (!oldCertificateNo.IsNull())
            {
                rpemployeeQueryable = rpemployeeQueryable.Where(p => p.OldCertificateNo.Contains(oldCertificateNo));
            }
            if (!examType.IsNull())
            {
                rpemployeeQueryable = rpemployeeQueryable.Where(p => p.ExamType == examType);
            }

            if (checkSatus == "未审核")
            {
                rpemployeeQueryable.Where(p => uow.Biz_RP_EmployeeDataCheckedRecord.GetAll().Where(q => q.OperationStatus == false).Select(q => q.EmployeeId).Contains(p.Id));
            }


            if (checkSatus == "审核通过")
            {
                IQueryable<Biz_RP_EmployeeCheckedRecord> employeeCheckedRecordQueryable = uow.Biz_RP_EmployeeCheckedRecord.GetAll().Where(q => q.PassStatus == true);
                rpemployeeQueryable = rpemployeeQueryable.Where(p => employeeCheckedRecordQueryable.Select(q => q.EmployeeId).Contains(p.Id));
            }
            else if (checkSatus == "审核不通过")
            {
                IQueryable<Biz_RP_EmployeeCheckedRecord> employeeCheckedRecordQueryable = uow.Biz_RP_EmployeeCheckedRecord.GetAll().Where(q => q.PassStatus == false);
                rpemployeeQueryable = rpemployeeQueryable.Where(p => employeeCheckedRecordQueryable.Select(q => q.EmployeeId).Contains(p.Id));
            }

            if (!checkDateBegin.IsNull())
            {
                DateTime datatime = DateTime.ParseExact(checkDateBegin, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                IQueryable<Biz_RP_EmployeeCheckedRecord> employeeCheckedRecordQueryable = uow.Biz_RP_EmployeeCheckedRecord.GetAll().Where(p => datatime <= p.CreateDate);
                rpemployeeQueryable = rpemployeeQueryable.Where(p => employeeCheckedRecordQueryable.Select(q => q.EmployeeId).Contains(p.Id));

            }
            if (!checkDateEnd.IsNull())
            {
                DateTime datatime = DateTime.ParseExact(checkDateEnd, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                IQueryable<Biz_RP_EmployeeCheckedRecord> employeeCheckedRecordQueryable = uow.Biz_RP_EmployeeCheckedRecord.GetAll().Where(p => p.CreateDate <= datatime);
                rpemployeeQueryable = rpemployeeQueryable.Where(p => employeeCheckedRecordQueryable.Select(q => q.EmployeeId).Contains(p.Id));
            }

            totalCount = rpemployeeQueryable.Count();
            int indexBegin = (page - 1) * rows;
            List<Biz_RP_EmployeeRegistration> rpemployeeList = rpemployeeQueryable.OrderByDescending(P => P.SummitDate).Skip(indexBegin).Take(rows).ToList();
            return rpemployeeList;
        }
        #endregion

        #region 获取人员成绩审核记录
        public List<Biz_RP_EmployeeCheckedRecord> GetEmployeeCheckedRecordByEmployeeIdList(List<int> employeeIdList)
        {
            IQueryable<Biz_RP_EmployeeCheckedRecord> employeeCheckedRecord = uow.Biz_RP_EmployeeCheckedRecord.GetAll().Where(p => employeeIdList.Contains(p.EmployeeId));
            return employeeCheckedRecord.ToList();
        }
        #endregion

        #region 近一年在线学习记录查询
        public class StudyRecord
        {
            public string IDNumber { get; set; }
            public double TotalHours { get; set; }
            public int OnlineExerciseCnt { get; set; }
            public double OnlineExerciseMaxCore { get; set; }
            public int SimulatedExamCnt { get; set; }
            public double SimulatedExamMaxCore { get; set; }
        }
        public List<StudyRecord> GetOneYearGetOnlineStudyRecord(List<string> idNumberList)
        {
            idNumberList = idNumberList.Distinct().ToList();
            DateTime beginDateTime = DateTime.Now.AddMonths(-12);
            IQueryable<Biz_StudyByVideoComplete> studyByVideoCompleteQueryable = uow.Biz_StudyByVideoComplete.GetAll().Where(p => p.CreateDate >= beginDateTime);
            IQueryable<Biz_OnlineExerciseRecord> onlineExerciseRecordQueryable = uow.Biz_OnlineExerciseRecord.GetAll().Where(p => p.CreateDate >= beginDateTime);
            IQueryable<Biz_SimulatedExamRecord> simulatedExamRecordQueryable = uow.Biz_SimulatedExamRecord.GetAll().Where(p => p.CreateDate >= beginDateTime);

            List<StudyRecord> videoStudyRecordList = studyByVideoCompleteQueryable.GroupBy(p => p.IDNumber).Select(p => new StudyRecord { IDNumber = p.Key, TotalHours = p.Count() }).ToList();
            List<StudyRecord> onlineExerciseRecordList = onlineExerciseRecordQueryable.GroupBy(p => p.IDNumber).Select(p => new StudyRecord { IDNumber = p.Key, OnlineExerciseCnt = p.Count(), OnlineExerciseMaxCore = p.Max(m => m.Score) }).ToList();
            List<StudyRecord> imulatedExamRecordList = simulatedExamRecordQueryable.GroupBy(p => p.IDNumber).Select(p => new StudyRecord { IDNumber = p.Key, SimulatedExamCnt = p.Count(), SimulatedExamMaxCore = p.Max(m => m.Score) }).ToList();

            List<StudyRecord> studyRecordList = idNumberList.GroupJoin(videoStudyRecordList, a => a, b => b.IDNumber, (a, b) => new { IDNumber = a, b })
                .GroupJoin(onlineExerciseRecordList, o => o.IDNumber, c => c.IDNumber, (o, c) => new { o.IDNumber, o.b, c })
                .GroupJoin(imulatedExamRecordList, o => o.IDNumber, d => d.IDNumber, (o, d) => new { o.IDNumber, o.b, o.c, d })
                .Select(o => new StudyRecord()
                {
                    IDNumber = o.IDNumber,
                    TotalHours = o.b.Count() == 0 ? 0 : o.b.First().TotalHours,
                    OnlineExerciseCnt = o.c.Count() == 0 ? 0 : o.c.First().OnlineExerciseCnt,
                    OnlineExerciseMaxCore = o.c.Count() == 0 ? 0 : o.c.First().OnlineExerciseMaxCore,
                    SimulatedExamCnt = o.d.Count() == 0 ? 0 : o.d.First().SimulatedExamCnt,
                    SimulatedExamMaxCore = o.d.Count() == 0 ? 0 : o.d.First().SimulatedExamMaxCore,
                }).ToList();
            return studyRecordList;
        }
        #endregion

        #region 获取近一年视频学习记录
        public List<Biz_StudyByVideoComplete> GetOneYearStudyByVideoComplete(List<string> idNumberList)
        {
            idNumberList = idNumberList.Distinct().ToList();
            DateTime beginDateTime = DateTime.Now.AddMonths(-12);
            IQueryable<Biz_StudyByVideoComplete> studyByVideoCompleteQueryable = uow.Biz_StudyByVideoComplete.GetAll()
                .Where(p => p.CreateDate >= beginDateTime && idNumberList.Contains(p.IDNumber));
            return studyByVideoCompleteQueryable.ToList();
        }
        #endregion

        #region 获取近一年在线练习记录
        public List<Biz_OnlineExerciseRecord> GetOneYearOnlineExerciseRecord(List<string> idNumberList)
        {
            idNumberList = idNumberList.Distinct().ToList();
            DateTime beginDateTime = DateTime.Now.AddMonths(-12);
            IQueryable<Biz_OnlineExerciseRecord> onlineExerciseRecordQueryable = uow.Biz_OnlineExerciseRecord.GetAll()
                .Where(p => p.CreateDate >= beginDateTime && idNumberList.Contains(p.IDNumber));
            return onlineExerciseRecordQueryable.ToList();
        }
        #endregion

        #region 获取近一年在模拟考试记录
        public List<Biz_SimulatedExamRecord> GetOneYearSimulatedExamRecord(List<string> idNumberList)
        {
            idNumberList = idNumberList.Distinct().ToList();
            DateTime beginDateTime = DateTime.Now.AddMonths(-12);
            IQueryable<Biz_SimulatedExamRecord> simulatedExamRecordQueryable = uow.Biz_SimulatedExamRecord.GetAll()
                .Where(p => p.CreateDate >= beginDateTime && idNumberList.Contains(p.IDNumber));
            return simulatedExamRecordQueryable.ToList();
        }
        #endregion

        public List<Biz_Certificate> GetEmployeeListInPrintCertificate(string employeeName, string idNumber, string examType, string industry, string enterpriseName, List<string> cityList, int page, int rows, ref int totalCount)
        {
            //证书表
            IQueryable<Biz_Certificate> certificateQueryable = uow.Biz_Certificate.GetAll().Where(p => !p.Invalid);
            //数据权限 查看本市的证书
            {
                IQueryable<Biz_RelRPEmployeeCertificate> relRPEmployeeCertificateQueryable = uow.Biz_RelRPEmployeeCertificate.GetAll()
                    .Join(uow.Biz_RP_EmployeeRegistration.GetAll(), a => a.RPEmployeeId, b => b.Id, (a, b) => new { a, b }).Where(o => cityList.Contains(o.b.City)).Select(o => o.a);
                certificateQueryable = certificateQueryable.Where(p => relRPEmployeeCertificateQueryable.Select(q => q.CertificateId).Contains(p.Id));
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
            totalCount = certificateQueryable.Count();
            int indexBegin = (page - 1) * rows;
            certificateQueryable = certificateQueryable.OrderByDescending(p => p.StartCertificateDate).Skip(indexBegin).Take(rows);
            return certificateQueryable.ToList();
        }

        public void SaveRPCertificatePrintRecord(Biz_RP_CertificatePrintRecord rpCertificatePrintRecord)
        {
            uow.Biz_RP_CertificatePrintRecord.Add(rpCertificatePrintRecord);
            uow.Commit();
        }

        public void SaveRPEmployeeFile(int rpEmployeeId, string fileType, string fileName, string filePath)
        {
            Biz_RP_EmployeeFile rpEmployeeFile = uow.Biz_RP_EmployeeFile.GetAll().Where(p => p.FileType == fileType && p.RPEmployeeId == rpEmployeeId).SingleOrDefault();
            if (rpEmployeeFile == null)
            {
                Biz_RP_EmployeeFile newRPEmployeeFile = new Biz_RP_EmployeeFile()
                {
                    RPEmployeeId = rpEmployeeId,
                    FileType = fileType,
                    FileName = fileName,
                    FilePath = filePath
                };
                uow.Biz_RP_EmployeeFile.Add(newRPEmployeeFile);
            }
            else
            {
                rpEmployeeFile.FileName = fileName;
                rpEmployeeFile.FilePath = filePath;
                uow.Biz_RP_EmployeeFile.Update(rpEmployeeFile);
            }
            uow.Commit();
        }

        public List<Biz_RP_EmployeeFile> GetRPEmployeePhotoListByRPEmployeeIdList(List<int> rpEmployeeIdList)
        {
            IQueryable<Biz_RP_EmployeeFile> rpEmployeeFileQueryable = uow.Biz_RP_EmployeeFile.GetAll().Where(p => rpEmployeeIdList.Contains(p.RPEmployeeId) && p.FileType == "登记照");
            return rpEmployeeFileQueryable.ToList();
        }
        public Biz_RP_EmployeeFile GetRPEmployeePhoto(int rpEmployeeId)
        {
            IQueryable<Biz_RP_EmployeeFile> rpEmployeeFileQueryable = uow.Biz_RP_EmployeeFile.GetAll().Where(p => p.RPEmployeeId == rpEmployeeId && p.FileType == "登记照");
            return rpEmployeeFileQueryable.OrderByDescending(p => p.Id).FirstOrDefault();
        }

        public Biz_RP_EmployeeDataCheckedRecord GetRP_EmployeeDataCheckedRecordByRPEmployeeId(int rpEmployeeId)
        {
            return uow.Biz_RP_EmployeeDataCheckedRecord.GetAll().Where(p => p.EmployeeId == rpEmployeeId).SingleOrDefault();
        }
    }
}
