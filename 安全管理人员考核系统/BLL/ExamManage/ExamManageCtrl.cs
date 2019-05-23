using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Model;
using DAL;
using Library.baseFn;
using System.IO;
using Library;
using System.Threading.Tasks;

namespace BLL
{
    public class ExamManageCtrl : IExamManageCtrl
    {
        private Uow uow;
        private Sys_Account account;
        public ExamManageCtrl(Sys_Account account)
        {
            if (uow == null)
            {
                uow = new Uow();
            }
            this.account = account;

        }
        static object convertToPDFLocker = new object();

        #region 考试列表
        //根据考试计划Id 获取考试分配记录
        public List<Biz_EmployeeForExamPlanRecord> GetEmployeeForExamPlanRecordListByExamPlanIdList(List<int> examPlanIdList)
        {
            List<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordList = uow.Biz_EmployeeForExamPlanRecord.GetAll().Where(p => examPlanIdList.Contains(p.ExamPlanRecordId)).ToList();
            return employeeForExamPlanRecordList;
        }
        //获取正在考试的 考试计划
        public List<Biz_ExamPlanRecord> GetExamPlanRecordListInExaming(List<string> cityList)
        {
            DateTime currentDateTime = DateTime.Now;
            IQueryable<Biz_ExamPlanRecord> examPlanRecordQueryable = uow.Biz_ExamPlanRecord.GetAll().Where(p => p.SubmitStatus == true).Where(p => DateTime.Compare(p.ExamDateTimeBegin, currentDateTime) <= 0).Where(p => DateTime.Compare(p.ExamDateTimeEnd, currentDateTime) >= 0);
            IQueryable<Biz_ExaminationPoint> trainingInstitutionQueryable = uow.Biz_ExaminationPoint.GetAll();
            string roleType = account.RoleList.Select(p => p.RoleType).FirstOrDefault();
            if (roleType == RoleCtrl.RoleType.Manager)
            {
                //看本城市考核点的 考试计划
                Sys_Role accountRole = account.RoleList.FirstOrDefault();
                List<int> accountIdList = uow.Sys_RelAccountRole.GetAll().Where(p => p.RoleId == accountRole.Id).Select(p => p.AccountId).ToList();
                examPlanRecordQueryable = examPlanRecordQueryable.Where(p => accountIdList.Contains(p.CreateById));
            }
            else if (roleType == RoleCtrl.RoleType.TrainingInstitution)
            {
                trainingInstitutionQueryable = trainingInstitutionQueryable.Where(p => p.Id == account.UserId);
            }
            else if (roleType == RoleCtrl.RoleType.Master)
            {

            }
            else//
            {
                examPlanRecordQueryable = examPlanRecordQueryable.Where(p => (1 > 2));
            }

            IQueryable<Biz_ExaminationRoom> examRoomQueryable = uow.Biz_ExaminationRoom.GetAll().Where(p => trainingInstitutionQueryable.Select(q => q.Id).Contains(p.ExaminationPointId));
            IQueryable<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordQueryable = uow.Biz_EmployeeForExamPlanRecord.GetAll().Where(p => examRoomQueryable.Select(q => q.Id).Contains(p.ExamRoomId));
            examPlanRecordQueryable = examPlanRecordQueryable.Where(p => employeeForExamPlanRecordQueryable.Select(q => q.ExamPlanRecordId).Contains(p.Id));
            List<Biz_ExamPlanRecord> examPlanRecordList = examPlanRecordQueryable.ToList();
            return examPlanRecordList;
        }
        //根据考场id获取考场
        public List<Biz_ExaminationRoom> GetExamRoomByIdList(List<int> IdList)
        {
            List<Biz_ExaminationRoom> examRoomList = uow.Biz_ExaminationRoom.GetAll().Where(p => IdList.Contains(p.Id)).ToList();
            return examRoomList;
        }
        //根据考场Id获取考核点
        public List<Biz_ExaminationPoint> GetTrainingInstitutionByRoomIdList(List<int> examRoomIdList)
        {
            IQueryable<Biz_ExaminationRoom> examRoomQueryable = uow.Biz_ExaminationRoom.GetAll().Where(p => examRoomIdList.Contains(p.Id));
            List<Biz_ExaminationPoint> trainingInstitutionList = uow.Biz_ExaminationPoint.GetAll().Where(p => examRoomQueryable.Select(q => q.ExaminationPointId).Contains(p.Id)).ToList();
            return trainingInstitutionList;
        }
        #endregion
        #region 准考证
        public AdmissionticketInfo GetAdmissionticketInfo(int employeeId, IWorkFlowCtrl workFlowCtrl, IEnterpriseCtrl enterpriseCtrl, IExaminationPointCtrl examinationPointCtrl)
        {
            AdmissionticketInfo result = new AdmissionticketInfo();
            Biz_Employee employee = workFlowCtrl.GetEmployeeInfoById(employeeId);
            Biz_Enterprise enterprise = enterpriseCtrl.GetEnterpriseById(employee.EnterpriseId);
            Biz_ExamPlanRecord examPlan = workFlowCtrl.GetFirstExamPlanByEmployeeId(employeeId);
            if (examPlan == null)
            {
                throw new Exception("接下来没有要参加的考试");
            }
            Biz_ExaminationRoom examRoom = workFlowCtrl.GetExamRoom(examPlan.Id, employeeId);
            Biz_EmployeeForExamPlanRecord employeeForExamPlanRecord = workFlowCtrl.GetEmployeeForExamPlanRecordByEmployeeIdList(new List<int>() { employeeId }).FirstOrDefault();
            Biz_ExaminationPoint examinationPoint = examinationPointCtrl.GetExaminationPointById(examRoom.ExaminationPointId);
            //写入返回值
            result.EmployeeName = employee.EmployeeName;
            result.ExamRegistrationNumber = employeeForExamPlanRecord.ExamRegistrationNumber.IsNull() ? "" : employeeForExamPlanRecord.ExamRegistrationNumber + "(座号:" + employeeForExamPlanRecord.ExamSeatNumber + ")";
            result.IDNumber = employee.IDNumber;
            result.ExamType = employee.ExamType;
            result.Industry = employee.Industry;
            result.Sex = employee.Sex;
            result.ExamCoreExamId = examPlan.ExamCoreExamId;
            result.ExamDateTimeBegin = examPlan.ExamDateTimeBegin.ToString("yyyy-MM-dd HH:mm");
            result.ExamDateTimeEnd = examPlan.ExamDateTimeEnd.ToString("yyyy-MM-dd HH:mm");
            result.ExamRoomName = examRoom.ExamRoomName;
            result.EnterpriseName = enterprise.EnterpriseName;
            result.TrainingInstitutionName = examinationPoint.InstitutionName;
            result.TrainingInstitutionAddress = examinationPoint.Address;
            result.ExamAddress = result.TrainingInstitutionName + "(" + result.TrainingInstitutionAddress + ")";
            return result;
        }

        public string CreateNewAdmissionticketPDF(AdmissionticketInfo examInfo, string templateFilePath, string rootFolderPath)
        {

            ExcelHelper eh = null;
            try
            {
                //参数检查
                if (examInfo.IsNull() || examInfo.IDNumber.IsNull() || examInfo.ExamCoreExamId.IsNull())
                {
                    throw new ArgumentException("examInfo，examInfo.IDNumber，examInfo.ExamCoreExamId信息为空或者不完整，请检查");
                }
                if (templateFilePath.IsNull())
                {
                    throw new ArgumentException("templateFilePath不能为空");
                }
                if (rootFolderPath.IsNull())
                {
                    throw new ArgumentException("rootFolderPath不能为空");
                }

                FileInfo templateFile = new FileInfo(templateFilePath);
                if (!templateFile.Exists)
                {
                    throw new Exception("模板文件不存在:" + templateFilePath);
                }

                string postFix = templateFilePath.Substring(templateFilePath.LastIndexOf('.'));
                string targetFileName = string.Format(@"{0}-{1}-{2}" + postFix
                    , examInfo.IDNumber
                    , examInfo.ExamCoreExamId
                    , DateTime.Now.ToString("yyyyMMdd"));

                string targetFilePath = string.Format(@"{0}\准考证（可删除）\{1}"
                    , rootFolderPath.TrimEnd('\\')
                    , targetFileName);
                FileInfo targetFile = new FileInfo(targetFilePath);
                if (!targetFile.Directory.Exists)
                {
                    targetFile.Directory.Create();
                }
                else if (targetFile.Exists)
                {
                    targetFile.Attributes = FileAttributes.Normal;
                    targetFile.Delete();
                }
                templateFile.CopyTo(targetFilePath, true);
                targetFile.Refresh();
                targetFile.Attributes = FileAttributes.Normal;


                eh = new ExcelHelper(targetFilePath);
                //eh.OpenExcel(targetFilePath);

                //eh.SetCellValue(2, 2, 2, examInfo.EmployeeName);//考生姓名	
                //eh.SetCellValue(2, 2, 3, examInfo.IDNumber);//身份证号	
                //eh.SetCellValue(2, 2, 4, examInfo.Sex);//性别	
                //eh.SetCellValue(2, 2, 5, examInfo.ExamRegistrationNumber);//准考证号	
                //eh.SetCellValue(2, 2, 6, examInfo.ExamType);//报考科目	
                //eh.SetCellValue(2, 2, 7, examInfo.ExamDateTimeBegin + "--" + examInfo.ExamDateTimeEnd);//考试时间	
                //eh.SetCellValue(2, 2, 8, examInfo.ExamRoomName);//考场
                //eh.SetCellValue(2, 2, 9, examInfo.EnterpriseName);//工作单位
                //eh.SetCellValue(2, 2, 10, examInfo.ExamAddress);//考试地点
                //eh.SetSheetActive(1);

                //eh.Save(null);
                List<ExcelHelper.Cell> cellList = new List<ExcelHelper.Cell>();
                cellList.Add(new ExcelHelper.Cell() { CellName = "考生姓名", CellValue = examInfo.EmployeeName, RowNumber = 1 });
                cellList.Add(new ExcelHelper.Cell() { CellName = "身份证号", CellValue = examInfo.IDNumber, RowNumber = 1 });
                cellList.Add(new ExcelHelper.Cell() { CellName = "性别", CellValue = examInfo.Sex, RowNumber = 1 });
                cellList.Add(new ExcelHelper.Cell() { CellName = "准考证号", CellValue = examInfo.ExamRegistrationNumber, RowNumber = 1 });
                cellList.Add(new ExcelHelper.Cell() { CellName = "报考科目", CellValue = examInfo.ExamType, RowNumber = 1 });
                string examDate = examInfo.ExamDateTimeBegin + "--" + examInfo.ExamDateTimeEnd;
                cellList.Add(new ExcelHelper.Cell() { CellName = "考试时间", CellValue = examDate, RowNumber = 1 });
                cellList.Add(new ExcelHelper.Cell() { CellName = "考场", CellValue = examInfo.ExamRoomName, RowNumber = 1 });
                cellList.Add(new ExcelHelper.Cell() { CellName = "工作单位", CellValue = examInfo.EnterpriseName, RowNumber = 1 });
                cellList.Add(new ExcelHelper.Cell() { CellName = "考试地点", CellValue = examInfo.ExamAddress, RowNumber = 1 });
                eh.InsertExcelValueList("数据表", cellList);


                string targetPDFFile = targetFilePath.Substring(0, targetFilePath.LastIndexOf('.')) + ".pdf";
                FileInfo pdfFile = new FileInfo(targetPDFFile);
                if (pdfFile.Exists)
                {
                    pdfFile.Attributes = FileAttributes.Normal;
                    pdfFile.Delete();
                }
                //eh.ExcelToPdf(targetPDFFile, 1);
                eh.ConvertToPDF(targetFilePath, targetPDFFile);
                try
                {
                    string folderPath = targetPDFFile.Substring(0, targetPDFFile.LastIndexOf('\\'));
                    DeleteOldFiles(folderPath, 1, new string[] { "*" + postFix, "*.pdf" });
                }
                catch (Exception ex)
                {

                }

                return targetPDFFile;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // if (null != eh) eh.Dispose();
            }

        }

        /// <summary>
        /// 清理旧文件
        /// </summary>
        private void DeleteOldFiles(string folderPath, int fileRemainDays, IList<string> searchPatterns)
        {
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(folderPath);
            try
            {
                foreach (string pattern in searchPatterns)
                {
                    System.IO.FileInfo[] files = dir.GetFiles(pattern, System.IO.SearchOption.TopDirectoryOnly);
                    foreach (var f in files)
                    {
                        if (f.CreationTime.AddDays(fileRemainDays) < DateTime.Now)
                        {
                            try
                            {
                                f.Attributes = System.IO.FileAttributes.Normal;
                                f.Delete();
                            }
                            catch { continue; }
                        }
                    }
                }
            }
            catch { throw; }
        }
        #endregion
        #region  监考
        public List<Biz_Employee> GetEmployeeList(int examPlanId, int examRoomId)
        {
            IQueryable<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordQueryable = uow.Biz_EmployeeForExamPlanRecord.GetAll().Where(p => p.ExamPlanRecordId == examPlanId).Where(p => p.ExamRoomId == examRoomId);
            List<Biz_Employee> employeeList = uow.Biz_Employee.GetAll().Where(p => employeeForExamPlanRecordQueryable.Select(q => q.EmployeeId).Contains(p.Id)).ToList();
            return employeeList;
        }
        public List<Biz_PaperForExamType> GetPaperForExamTypeList(int employeeId)
        {
            Biz_Employee employee = uow.Biz_Employee.GetById(employeeId);
            List<Biz_PaperForExamType> paperForExamTypeList = uow.Biz_PaperForExamType.GetAll().Where(p => p.ExamType == employee.ExamType).Where(p => p.Industry == employee.Industry).ToList();
            return paperForExamTypeList;
        }
        #endregion

        #region 考试计划和考试结果
        private IQueryable<Biz_ExaminationPoint> GetTrainingInstitutionByExamPlanId(int examPlanId)
        {
            IQueryable<Biz_EmployeeForExamPlanRecord> qRelEmployeeOfExamPlan = uow.Biz_EmployeeForExamPlanRecord.GetAll()
                .Where(x => x.ExamPlanRecordId == examPlanId);
            IQueryable<Biz_ExaminationRoom> qRoom = uow.Biz_ExaminationRoom.GetAll()
                .Where(x => qRelEmployeeOfExamPlan
                    .Select(rel => rel.ExamRoomId)
                    .Contains(x.Id));
            IQueryable<Biz_ExaminationPoint> qTrainingInstitution = uow.Biz_ExaminationPoint.GetAll()
                .Where(x => qRoom
                    .Select(r => r.ExaminationPointId)
                    .Contains(x.Id));
            return qTrainingInstitution;
        }

        private IQueryable<Biz_Employee> GetEmployeeListByExamPlanId(int examPlanId)
        {
            IQueryable<Biz_EmployeeForExamPlanRecord> qRelEmployeeOfExamPlan = uow.Biz_EmployeeForExamPlanRecord.GetAll()
                .Where(x => x.ExamPlanRecordId == examPlanId);
            IQueryable<Biz_Employee> qEmployees = uow.Biz_Employee.GetAll()
                .Where(x => qRelEmployeeOfExamPlan
                    .Select(rel => rel.EmployeeId)
                    .Contains(x.Id));
            return qEmployees;
        }

        private string HideChar(string source, int hideCount, char replacement)
        {

            if (source.IsNull() || hideCount < 1)
            {
                return source;
            }

            if (hideCount > source.Length)
            {
                hideCount = source.Length;
            }

            StringBuilder sbResult = new StringBuilder();
            sbResult.Append(source.Substring(0, source.Length - hideCount));
            for (int i = 0; i < hideCount; i++)
            {
                sbResult.Append(replacement);
            }
            return sbResult.ToString();
        }

        private List<Biz_EmployeeExamResultRecord> GetExamResultRecordsByExamPlanId(int examPlanId)
        {
            IQueryable<Biz_Employee> qEmployees = GetEmployeeListByExamPlanId(examPlanId);
            IEmployeeCtrl employeeCtrl = new EmployeeCtrl(account);
            List<Biz_EmployeeExamResultRecord> qResult = employeeCtrl.GetEmployeeExamResultRecordListByEmployeeId(qEmployees.Select(p => p.Id).ToList());
            return qResult;
        }

        public Biz_ExamPlanRecord GetExamPlanRecordById(int id)
        {
            return uow.Biz_ExamPlanRecord.GetAll().First(x => x.Id == id);
        }

        public List<ExamPlanAbstractInfo> GetPublishedExamPlanList(int pageIndex, int pageSize)
        {
            IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
            //测试账户
            IQueryable<Sys_Account> testAccountQueryable = uow.Sys_Account.GetAll().Where(p => p.AccountName.Contains("test"));
            IQueryable<Biz_ExamPlanRecord> qExamPlan = uow.Biz_ExamPlanRecord.GetAll().Where(p => !testAccountQueryable.Select(q => q.Id).Contains(p.CreateById)).Where(x => x.SubmitStatus).OrderByDescending(x => x.SubmitDate).Take(50);
            int totalCount = qExamPlan.Count();

            int totalPage = (totalCount / pageSize) + ((totalCount % pageSize) > 0 ? 1 : 0);

            int skipQuestionCount = (pageIndex - 1) * pageSize;

            List<ExamPlanAbstractInfo> resultList = qExamPlan
                .OrderByDescending(x => x.ExamDateTimeBegin)
                .Skip(skipQuestionCount)
                .Take(pageSize)
                .Select(x => new ExamPlanAbstractInfo()
                {
                    Id = x.Id,
                    ExamPlanNumber = x.ExamPlanNumber,
                    ExamDateTimeBegin = x.ExamDateTimeBegin
                })
                .ToList();

            return resultList;
        }

        public ExamPlanDetails GetExamPlanDetailsById(int id, int hiddenCharCount)
        {
            Biz_ExamPlanRecord examPlan = GetExamPlanRecordById(id);
            List<Biz_ExaminationPoint> qTrainingInstitution = GetTrainingInstitutionByExamPlanId(id).ToList();
            List<Biz_Employee> qEmployee = GetEmployeeListByExamPlanId(id).ToList();

            ExamPlanDetails result = new ExamPlanDetails()
            {
                Id = examPlan.Id,
                ExamPlanNumber = examPlan.ExamPlanNumber,
                ExamDateTimeBegin = examPlan.ExamDateTimeBegin,
                // AExamDateTimeEnd = examPlan.AExamDateTimeEnd,
                // BExamDateTimeBegin = examPlan.BExamDateTimeBegin,
                ExamDateTimeEnd = examPlan.ExamDateTimeEnd,
                TrainingInstitutions = qTrainingInstitution.Select(inst => new ExamPlanTrainingInstitution()
                {
                    Id = inst.Id,
                    InstitutionName = inst.InstitutionName,
                    SocialCreditCode = inst.SocialCreditCode,
                    LegalRepresentative = inst.LegalRepresentative,
                    LegalRepresentativeNumber = inst.LegalRepresentativeNumber,
                    ContactPerson = inst.ContactPerson,
                    ContactNumber = inst.ContactNumber,
                    City = inst.City,
                    Area = inst.Area,
                    Email = inst.Email,
                    Address = inst.Address,
                    Employees = qEmployee.Select(em => new ExamPlanEmployee()
                    {
                        Id = em.Id,
                        EmployeeName = em.EmployeeName,
                        IDNumber = HideChar(em.IDNumber, hiddenCharCount, '*')
                    }).ToList()
                    //Employees = qEmployee.Where(em => em.TrainingInstitutionId == inst.Id)
                    //.Select(em => new ExamPlanEmployee()
                    //{
                    //    Id = em.Id,
                    //    EmployeeName = em.EmployeeName,
                    //    IDNumber = HideChar(em.IDNumber, hiddenCharCount, '*')
                    //}).ToList()
                }).ToList()
            };

            return result;
        }

        public List<ExamResultAbstractInfo> GetPublishedExamResultList(int pageIndex, int pageSize)
        {
            IQueryable<Biz_EmployeeExamResultCheckedRecord> qBiz_EmployeeExamResultRecord = uow.Biz_EmployeeExamResultCheckedRecord.GetAll();

            //测试账户
            IQueryable<Sys_Account> testAccountQueryable = uow.Sys_Account.GetAll().Where(p => p.AccountName.Contains("test"));

            IQueryable<Biz_ExamPlanRecord> qExamPlan = uow.Biz_ExamPlanRecord.GetAll().Where(x =>
                qBiz_EmployeeExamResultRecord
                .Select(res => res.ExamPlanRecordId)
                .Contains(x.Id)
                && x.SubmitStatus).Where(p => !testAccountQueryable.Select(q => q.Id).Contains(p.CreateById));

            int totalCount = qExamPlan.Count();

            int totalPage = (totalCount / pageSize) + ((totalCount % pageSize) > 0 ? 1 : 0);

            int skipQuestionCount = (pageIndex - 1) * pageSize;

            List<ExamResultAbstractInfo> resultList = qExamPlan
                .OrderByDescending(x => x.ExamDateTimeBegin)
                .Skip(skipQuestionCount)
                .Take(pageSize)
                .Select(x => new ExamResultAbstractInfo()
                {
                    Id = x.Id,
                    ExamPlanNumber = x.ExamPlanNumber,
                    ExamDateTimeBegin = x.ExamDateTimeBegin
                })
                .ToList();

            return resultList;
        }
        public ExamResultDetails GetExamResultDetailsById(int id, int hiddenCharCount)
        {

            Biz_ExamPlanRecord examPlan = GetExamPlanRecordById(id);
            List<Biz_ExaminationPoint> qTrainingInstitution = GetTrainingInstitutionByExamPlanId(id).ToList();
            List<Biz_Employee> qEmployee = GetEmployeeListByExamPlanId(id).ToList();
            IEmployeeCtrl employeeCtrl = new EmployeeCtrl(account);
            IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
            List<int> employeeIdList = qEmployee.Select(p => p.Id).ToList();
            List<Biz_EmployeeExamResultRecord> qExamResult = employeeCtrl.GetEmployeeExamResultRecordListByEmployeeId(employeeIdList);
            List<Biz_EmployeeExamResultCheckedRecord> qExamResultChecked = workFlowCtrl.GetEmployeeExamResultCheckedRecordListByEmployeeIdList(employeeIdList, null);

            ExamResultDetails result = new ExamResultDetails()
            {
                Id = examPlan.Id,
                ExamPlanNumber = examPlan.ExamPlanNumber,
                ExamDateTimeBegin = examPlan.ExamDateTimeBegin,
                //AExamDateTimeEnd = examPlan.AExamDateTimeEnd,
                //BExamDateTimeBegin = examPlan.BExamDateTimeBegin,
                ExamDateTimeEnd = examPlan.ExamDateTimeEnd,
                TrainingInstitutions = qTrainingInstitution.Select(inst => new ExamResultTrainingInstitution()
                {
                    Id = inst.Id,
                    InstitutionName = inst.InstitutionName,
                    SocialCreditCode = inst.SocialCreditCode,
                    LegalRepresentative = inst.LegalRepresentative,
                    LegalRepresentativeNumber = inst.LegalRepresentativeNumber,
                    ContactPerson = inst.ContactPerson,
                    ContactNumber = inst.ContactNumber,
                    City = inst.City,
                    Area = inst.Area,
                    Email = inst.Email,
                    Address = inst.Address,
                    Employees = qEmployee.Select(em => new ExamResultEmployee()
                    {
                        Id = em.Id,
                        EmployeeName = em.EmployeeName,
                        IDNumber = HideChar(em.IDNumber, hiddenCharCount, '*'),
                        ExamResult = qExamResult.Where(re => re.EmployeeId == em.Id)
                                      .Join(qExamResultChecked, a => a.EmployeeId, b => b.EmployeeId, (a, b) => new { a, b })
                        .Select(o => new ExamResult()
                        {
                            SafetyKnowledgeExamResult = o.a.SafetyKnowledgeExamResult,
                            SafetyKnowledgeExamScore = o.a.SafetyKnowledgeExamScore,
                            ManagementAbilityExamResult = o.a.ManagementAbilityExamResult,
                            ManagementAbilityExamScore = o.a.ManagementAbilityExamScore,
                            FieldExamResult = o.a.ActualOperationExamResult,
                            FinalExamResult = o.a.FinalExamResult,
                            CheckedResult = o.b.CheckedStatus ? "审核通过" : "审核不通过"
                        })
                        .ToList()
                    }).ToList()
                }).ToList()
            };

            return result;
        }

        #endregion

        #region 获取需要公式的考试计划
        public List<Biz_ExamPlanRecord> ExamPlanPublicity()
        {
            DateTime startDate = DateTime.Now.ToString("yyyy-MM-dd").ConvertToDateTime();
            DateTime endDate = startDate.AddDays(5);
            IQueryable<Biz_ExamPlanRecord> examPlanRecordQueryable = uow.Biz_ExamPlanRecord.GetAll().Where(p => p.SubmitStatus == true && startDate <= p.ExamDateTimeBegin && p.ExamDateTimeBegin <= endDate).OrderBy(p => p.ExamDateTimeBegin);
            return examPlanRecordQueryable.ToList();
        }
        #endregion
        public List<Biz_ExamPlanRecord> GetPublishedExamResultList()
        {
            IQueryable<Biz_EmployeeExamResultRecord> employeeExamResultRecordQueryable = uow.Biz_EmployeeExamResultRecord.GetAll().Where(q => q.OperationStatus == true);
            IQueryable<Biz_ExamPlanRecord> examPlanRecordQueryable = uow.Biz_ExamPlanRecord.GetAll().Where(p => employeeExamResultRecordQueryable.Select(q => q.ExamPlanRecordId).Contains(p.Id)).OrderByDescending(p => p.ExamDateTimeBegin).Take(100);
            return examPlanRecordQueryable.ToList();
        }
    }
    #region 准考证信息
    public class AdmissionticketInfo
    {
        public string EmployeeName { get; set; }
        public string ExamRegistrationNumber { get; set; }
        public string IDNumber { get; set; }
        public string ExamType { get; set; }
        public string Industry { get; set; }
        public string Sex { get; set; }
        public int ExamCoreExamId { get; set; }
        public string ExamDateTimeBegin { get; set; }
        public string ExamDateTimeEnd { get; set; }
        public string ExamRoomName { get; set; }
        public string EnterpriseName { get; set; }
        public string TrainingInstitutionName { get; set; }
        public string TrainingInstitutionAddress { get; set; }
        public string ExamAddress { get; set; }
    }
    #endregion
    #region 考试计划
    public class ExamPlanAbstractInfo
    {
        public int Id { get; set; }
        public string ExamPlanNumber { get; set; }
        public DateTime ExamDateTimeBegin { get; set; }
    }

    public class ExamPlanDetails
    {
        public int Id { get; set; }
        public string ExamPlanNumber { get; set; }
        public DateTime ExamDateTimeBegin { get; set; }
        public DateTime AExamDateTimeEnd { get; set; }
        public DateTime BExamDateTimeBegin { get; set; }
        public DateTime ExamDateTimeEnd { get; set; }
        public List<ExamPlanTrainingInstitution> TrainingInstitutions { get; set; }
    }

    public class ExamPlanTrainingInstitution
    {
        public int Id { get; set; }
        //考核点名称
        public string InstitutionName { get; set; }
        //社会信用代码
        public string SocialCreditCode { get; set; }
        //法定代表人
        public string LegalRepresentative { get; set; }
        //法定代表人电话
        public string LegalRepresentativeNumber { get; set; }
        //联系人
        public string ContactPerson { get; set; }
        //联系人电话
        public string ContactNumber { get; set; }
        //考核城市
        public string City { get; set; }
        //考核区域
        public string Area { get; set; }
        public string Email { get; set; }
        //地址
        public string Address { get; set; }
        public List<ExamPlanEmployee> Employees { get; set; }
    }

    public class ExamPlanEmployee
    {
        public int Id { get; set; }

        public string EmployeeName { get; set; }

        public string IDNumber { get; set; }
    }
    #endregion
    #region 考试结果
    public class ExamResultAbstractInfo
    {
        public int Id { get; set; }
        public string ExamPlanNumber { get; set; }
        public DateTime ExamDateTimeBegin { get; set; }
    }

    public class ExamResultDetails
    {
        public int Id { get; set; }
        public string ExamPlanNumber { get; set; }
        public DateTime ExamDateTimeBegin { get; set; }
        public DateTime AExamDateTimeEnd { get; set; }
        public DateTime BExamDateTimeBegin { get; set; }
        public DateTime ExamDateTimeEnd { get; set; }
        public List<ExamResultTrainingInstitution> TrainingInstitutions { get; set; }
    }

    public class ExamResultTrainingInstitution
    {
        public int Id { get; set; }
        //考核点名称
        public string InstitutionName { get; set; }
        //社会信用代码
        public string SocialCreditCode { get; set; }
        //法定代表人
        public string LegalRepresentative { get; set; }
        //法定代表人电话
        public string LegalRepresentativeNumber { get; set; }
        //联系人
        public string ContactPerson { get; set; }
        //联系人电话
        public string ContactNumber { get; set; }
        //考核城市
        public string City { get; set; }
        //考核区域
        public string Area { get; set; }
        public string Email { get; set; }
        //地址
        public string Address { get; set; }
        public List<ExamResultEmployee> Employees { get; set; }
    }

    public class ExamResultEmployee
    {
        public int Id { get; set; }

        public string EmployeeName { get; set; }

        public string IDNumber { get; set; }
        public List<ExamResult> ExamResult { get; set; }
    }

    public class ExamResult
    {
        //安全知识考试结果
        public string SafetyKnowledgeExamResult { get; set; }
        public double? SafetyKnowledgeExamScore { get; set; }

        //管理能力考试结果
        public string ManagementAbilityExamResult { get; set; }
        public double? ManagementAbilityExamScore { get; set; }
        //实操考试结果
        public string FieldExamResult { get; set; }
        //最终考核结果
        public string FinalExamResult { get; set; }
        //审核结果
        public string CheckedResult { get; set; }
    }
    #endregion
}
