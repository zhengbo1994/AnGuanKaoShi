using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using DAL;
using Library.baseFn;

namespace BLL
{
    public class EmployeeCtrl : IEmployeeCtrl
    {
        private Uow uow;
        private Sys_Account account;
        public EmployeeCtrl(Sys_Account account)
        {
            if (uow == null)
            {
                uow = new Uow();
            }
            this.account = account;

        }
       public Biz_Employee GetEmployeeById(int id)
        {
            return uow.Biz_Employee.GetById(id);
        }


        public List<Biz_EmployeeExamResultRecord> GetEmployeeExamResultRecordListByEmployeeId(List<int> employeeIdList)
        {

            List<Biz_EmployeeExamResultRecord> employeeExamResultRecordList = uow.Biz_EmployeeExamResultRecord.GetAll().Where(p => employeeIdList.Contains(p.EmployeeId)).ToList();

            List<Biz_Employee> employeeList = uow.Biz_Employee.GetAll().Where(p => employeeIdList.Contains(p.Id)).ToList();
            //找到相关试卷
            List<string> indeustryList = employeeList.Select(p => p.Industry).ToList();
            List<string> examTypeList = employeeList.Select(p => p.ExamType).ToList();
            List<Biz_PaperForExamType> paperForExamTypeList = uow.Biz_PaperForExamType.GetAll().Where(p => indeustryList.Contains(p.Industry) && examTypeList.Contains(p.ExamType)).ToList();


            foreach (Biz_EmployeeExamResultRecord employeeExamResultRecord in employeeExamResultRecordList)
            {
                Biz_Employee employee = employeeList.Where(p => p.Id == employeeExamResultRecord.EmployeeId).Single();
                //判断安全知识考核是否合格
                Biz_PaperForExamType safetyKnowledgeExamPaper = paperForExamTypeList.Where(p => p.Industry == employee.Industry && p.ExamType == employee.ExamType && p.PaperType == "SafetyKnowledgeExam").Single();
                employeeExamResultRecord.SafetyKnowledgeExamResult = employeeExamResultRecord.SafetyKnowledgeExamScore >= safetyKnowledgeExamPaper.PassScore ? "合格" : "不合格";
                //判断管理能力是否合格
                Biz_PaperForExamType managementAbilityExamPaper = paperForExamTypeList.Where(p => p.Industry == employee.Industry && p.ExamType == employee.ExamType && p.PaperType == "ManagementAbilityExam").Single();
                employeeExamResultRecord.ManagementAbilityExamResult = employeeExamResultRecord.ManagementAbilityExamScore >= managementAbilityExamPaper.PassScore ? "合格" : "不合格";
                employeeExamResultRecord.ActualOperationExamResult = employeeExamResultRecord.FieldExamResult == true ? "合格" : employeeExamResultRecord.FieldExamResult == false ? "不合格" : "";

                bool ActualOperationFlag = false;
                int count = employeeList.Where(p => (p.ExamType == "C1" || p.ExamType == "C3") && p.Id == employeeExamResultRecord.EmployeeId).Count();
                if (count > 0)
                {
                    ActualOperationFlag = true;
                }

                if (ActualOperationFlag)
                {
                    if (employeeExamResultRecord.SafetyKnowledgeExamResult == "" || employeeExamResultRecord.ManagementAbilityExamResult == "" || employeeExamResultRecord.ActualOperationExamResult == "")
                    {
                        employeeExamResultRecord.FinalExamResult = "";
                        continue;
                    }
                    else if (employeeExamResultRecord.SafetyKnowledgeExamResult == "合格" && employeeExamResultRecord.ManagementAbilityExamResult == "合格" && employeeExamResultRecord.ActualOperationExamResult == "合格")
                    {
                        employeeExamResultRecord.FinalExamResult = "合格";
                        continue;
                    }
                    else
                    {
                        employeeExamResultRecord.FinalExamResult = "不合格";
                        continue;
                    }
                }
                else
                {
                    if (employeeExamResultRecord.SafetyKnowledgeExamResult == "" || employeeExamResultRecord.ManagementAbilityExamResult == "")
                    {
                        employeeExamResultRecord.FinalExamResult = "";
                        continue;
                    }
                    else if (employeeExamResultRecord.SafetyKnowledgeExamResult == "合格" && employeeExamResultRecord.ManagementAbilityExamResult == "合格")
                    {
                        employeeExamResultRecord.FinalExamResult = "合格";
                        continue;
                    }
                    else
                    {
                        employeeExamResultRecord.FinalExamResult = "不合格";
                        continue;
                    }
                }

            }

            return employeeExamResultRecordList;
        }

        public List<Biz_EmployeeExamResultRecord> GetEmployeeExamResultRecordListByEmployeeIdList(List<int> employeeIdList)
        {

            List<Biz_EmployeeExamResultRecord> employeeExamResultRecordList = uow.Biz_EmployeeExamResultRecord.GetAll().Where(p => employeeIdList.Contains(p.EmployeeId)).ToList();

            return employeeExamResultRecordList;
        }

        public void SaveStudyByVideoRecord(Biz_StudyByVideoRecoder studyByVideoRecoder)
        {
            Biz_StudyByVideoRecoder oldStudyByVideoRecoder = uow.Biz_StudyByVideoRecoder.GetAll().Where(p => p.IDNumber == studyByVideoRecoder.IDNumber && p.VideoName == studyByVideoRecoder.VideoName).FirstOrDefault();
            if (oldStudyByVideoRecoder == null)
            {
                studyByVideoRecoder.CreateDateTime = DateTime.Now;
                studyByVideoRecoder.UpdateDateTime = DateTime.Now;
                uow.Biz_StudyByVideoRecoder.Add(studyByVideoRecoder);

            }
            else
            {
                oldStudyByVideoRecoder.Studyhours = studyByVideoRecoder.Studyhours;
                oldStudyByVideoRecoder.UpdateDateTime = DateTime.Now;
                uow.Biz_StudyByVideoRecoder.Update(oldStudyByVideoRecoder);
            }
            uow.Commit();
        }
        public void SaveStudyByVideoComplete(Biz_StudyByVideoComplete StudyByVideoComplete)
        {
            Biz_StudyByVideoComplete oldStudyByVideoComplete = uow.Biz_StudyByVideoComplete.GetAll()
                .Where(p => p.IDNumber == StudyByVideoComplete.IDNumber)
                .Where(p => p.VideoName == StudyByVideoComplete.VideoName)
                .Where(p => p.VideoId == StudyByVideoComplete.VideoId)
                .SingleOrDefault();
            if (oldStudyByVideoComplete == null)
            {
                uow.Biz_StudyByVideoComplete.Add(StudyByVideoComplete);
                uow.Commit();
            }
        }
        public List<Biz_StudyByVideoComplete> GetStudyByVideoCompleteList(List<string> idNumberList)
        {
            IQueryable<Biz_StudyByVideoComplete> studyByVideoCompleteQueryable = uow.Biz_StudyByVideoComplete.GetAll().Where(p => idNumberList.Contains(p.IDNumber));
            return studyByVideoCompleteQueryable.ToList();
        }
        public List<Biz_StudyByVideoComplete> GetStudyByVideoCompleteList(string idNumber)
        {
            IQueryable<Biz_StudyByVideoComplete> studyByVideoCompleteQueryable = uow.Biz_StudyByVideoComplete.GetAll().Where(p => p.IDNumber == idNumber);
            return studyByVideoCompleteQueryable.ToList();
        }
        public List<Biz_StudyByVideoRecoder> GetStudyByVideoRecoderListByEmployeeId(int employeeId)
        {
            Biz_Employee employee = uow.Biz_Employee.GetById(employeeId);
            return this.GetStudyByVideoRecoderListByIDNumber(employee.IDNumber);
        }
        public List<Biz_StudyByVideoRecoder> GetStudyByVideoRecoderListByIDNumber(string idNumber)
        {
            IQueryable<Biz_StudyByVideoRecoder> StudyByVideoRecoderQueryable = uow.Biz_StudyByVideoRecoder.GetAll().Where(p => p.IDNumber == idNumber);
            return StudyByVideoRecoderQueryable.ToList();
        }
        public List<Biz_OnlineExerciseRecord> GetOnlineExerciseRecordListByIDNumber(string iDNumber)
        {
            IQueryable<Biz_OnlineExerciseRecord> onlineExerciseRecordQueryable = uow.Biz_OnlineExerciseRecord.GetAll().Where(p => p.IDNumber == iDNumber);
            List<Biz_OnlineExerciseRecord> onlineExerciseRecordList = onlineExerciseRecordQueryable.ToList();
            return onlineExerciseRecordList;
        }
        public Biz_EmployeeFile GetEmployeeFile(int employeeId, string fileType)
        {
            Biz_EmployeeFile employeeFile = uow.Biz_EmployeeFile.GetAll().Where(p => p.EmployeeId == employeeId).Where(p => p.FileType == fileType).OrderByDescending(p => p.Id).FirstOrDefault();
            return employeeFile;
        }
        public Biz_EmployeeFile GetEmployeeFile(int certificateId)
        {
            Biz_EmployeeFile employeeFile = uow.Biz_Certificate.GetAll()
              .Join(uow.Biz_RelEmployeeCertificate.GetAll(), a => a.Id, b => b.CertificateId, (a, b) => new { a, b })
              .Join(uow.Biz_EmployeeFile.GetAll(), o => o.b.EmployeeId, c => c.EmployeeId, (o, c) => new { o.a, o.b, c })
              .Where(o => o.a.Id == certificateId && o.c.FileType == "进场照片").OrderByDescending(o => o.c.Id).Select(o => o.c).FirstOrDefault();
            return employeeFile;
        }
        public void AddOnlineExerciseRecord(Biz_OnlineExerciseRecord onlineExerciseRecord)
        {
            onlineExerciseRecord.CreateDate = DateTime.Now;
            uow.Biz_OnlineExerciseRecord.Add(onlineExerciseRecord);
            uow.Commit();
        }

        public void AddSimulatedExamRecord(Biz_SimulatedExamRecord simulatedExamRecord)
        {
            simulatedExamRecord.CreateDate = DateTime.Now;
            uow.Biz_SimulatedExamRecord.Add(simulatedExamRecord);
            uow.Commit();
        }

        public List<Biz_Employee> GetEmployeeListForAuthentication(string examPlanNumber, int examRoomId)
        {
            Biz_ExamPlanRecord examPlan = uow.Biz_ExamPlanRecord.GetAll().Where(p => p.ExamPlanNumber == examPlanNumber).Single();
            List<Biz_Employee> employeeList = uow.Biz_Employee.GetAll()
                .Join(uow.Biz_EmployeeForExamPlanRecord.GetAll(), p => p.Id, q => q.EmployeeId, (p, q) => new { p, q })
                .Where(o => o.q.ExamPlanRecordId == examPlan.Id && o.q.ExamRoomId == examRoomId).Select(o => o.p).ToList();
            return employeeList;
        }

        #region 取证人员考试情况统计

        public List<double> GetTotalEmployeeCityAndCountList(DateTime beginDate, DateTime endDate)
        {

            IQueryable<Biz_Employee> employeeList = uow.Biz_ExamPlanRecord.GetAll()
                .Join(uow.Biz_EmployeeForExamPlanRecord.GetAll(), p => p.Id, q => q.ExamPlanRecordId, (p, q) => new { p, q })
                .Join(uow.Biz_Employee.GetAll(), o => o.q.EmployeeId, m => m.Id, (o, m) => new { o.p, o.q, m })
                .Where(o => o.p.ExamDateTimeBegin > beginDate && o.p.ExamDateTimeBegin < endDate)
                .Select(o => o.m);



            List<double> countList = uow.Biz_City.GetAll().Where(p => p.CityName != "测试市")
                .GroupJoin(employeeList, p => p.CityName, q => q.City, (p, q) => new { p, q })
                .OrderBy(o => o.p.Seq)
                .Select(o => o.q.Count())
                .ToList()
                .Select(p => Convert.ToDouble(p))
                .ToList();


            return countList;
        }

        public List<double> GetTakeEmployeeCityAndCountList(DateTime beginDate, DateTime endDate)
        {

            IQueryable<Biz_Employee> employeeList = uow.Biz_ExamPlanRecord.GetAll()
                .Join(uow.Biz_EmployeeForExamPlanRecord.GetAll(), p => p.Id, q => q.ExamPlanRecordId, (p, q) => new { p, q })
                .Join(uow.Biz_Employee.GetAll(), o => o.q.EmployeeId, m => m.Id, (o, m) => new { o.p, o.q, m })
                .Join(uow.Biz_EmployeeExamResultRecord.GetAll(), o => o.q.EmployeeId, n => n.EmployeeId, (o, n) => new { o.p, o.q, o.m })
                .Where(o => o.p.ExamDateTimeBegin > beginDate && o.p.ExamDateTimeBegin < endDate)
                .Select(o => o.m);

            List<double> countList = uow.Biz_City.GetAll().Where(p => p.CityName != "测试市")
                .GroupJoin(employeeList, p => p.CityName, q => q.City, (p, q) => new { p, q })
                .OrderBy(o => o.p.Seq)
                .Select(o => o.q.Count())
                .ToList()
                .Select(p => Convert.ToDouble(p))
                .ToList();

            return countList;
        }

        public List<double> GetPassEmployeeCityAndCountList(DateTime beginDate, DateTime endDate)
        {

            IQueryable<Biz_Employee> employeeList = uow.Biz_ExamPlanRecord.GetAll()
                .Join(uow.Biz_EmployeeForExamPlanRecord.GetAll(), p => p.Id, q => q.ExamPlanRecordId, (p, q) => new { p, q })
                .Join(uow.Biz_Employee.GetAll(), o => o.q.EmployeeId, m => m.Id, (o, m) => new { o.p, o.q, m })
                .Join(uow.Biz_EmployeeExamResultRecord.GetAll(), o => o.q.EmployeeId, n => n.EmployeeId, (o, n) => new { o.p, o.q, o.m, n })
                .Join(uow.Biz_PaperForExamType.GetAll().Where(p => p.PaperType == "SafetyKnowledgeExam"), o => new { o.m.ExamType, o.m.Industry }, x => new { x.ExamType, x.Industry }, (o, x) => new { o.p, o.q, o.m, o.n, x })
                .Join(uow.Biz_PaperForExamType.GetAll().Where(p => p.PaperType == "ManagementAbilityExam"), o => new { o.m.ExamType, o.m.Industry }, y => new { y.ExamType, y.Industry }, (o, y) => new { o.p, o.q, o.m, o.n, o.x, y })
                .Where(o => o.p.ExamDateTimeBegin > beginDate && o.p.ExamDateTimeBegin < endDate && o.n.SafetyKnowledgeExamScore >= o.x.PassScore && o.n.ManagementAbilityExamScore >= o.y.PassScore)
                .Select(o => o.m);

            List<double> countList = uow.Biz_City.GetAll().Where(p => p.CityName != "测试市")
                .GroupJoin(employeeList, p => p.CityName, q => q.City, (p, q) => new { p, q })
                .OrderBy(o => o.p.Seq)
                .Select(o => o.q.Count())
                .ToList()
                .Select(p => Convert.ToDouble(p))
                .ToList();

            return countList;
        }

        public List<EnterpriseExamPassRate> GetEnterpriseExamPassRate(DateTime beginDate, DateTime endDate, string enterpriseName, int page, int rows, ref int totalCount)
        {
            const int DATAPERMISSION_CITY = 1;

            IQueryable<Biz_Enterprise> enterpriseQueryable = uow.Biz_Enterprise.GetAll();
            if (!enterpriseName.IsNull())
            {
                enterpriseQueryable = enterpriseQueryable.Where(p => p.EnterpriseName.Contains(enterpriseName));
            }

            //总站角色看所有企业信息，地市管理部门看各地市企业信息
            if (account.RoleList.Where(p => p.RoleType == "Master").Count() > 0)
            {

            }
            else if (account.RoleList.Where(p => p.RoleType == "Manager").Count() > 0)
            {
                List<string> cityList = new List<string>();

                foreach (Sys_Role role in account.RoleList.Where(p => p.RoleType == "Manager").ToList())
                {
                    cityList.AddRange(role.DataPermissionDetailList.Where(p => p.HeadId == DATAPERMISSION_CITY).Select(p => p.DetailName).ToList());
                }
                cityList = cityList.Distinct().ToList();

                if (cityList.Count > 0)
                {
                    enterpriseQueryable = enterpriseQueryable.Where(p => cityList.Contains(p.City));
                }
            }


            IQueryable<Biz_Employee> totalEmployeeQueryable = uow.Biz_ExamPlanRecord.GetAll().Where(p => p.SubmitStatus && p.ExamDateTimeEnd < DateTime.Now)
                .Join(uow.Biz_EmployeeForExamPlanRecord.GetAll(), p => p.Id, q => q.ExamPlanRecordId, (p, q) => new { p, q })
                .Join(uow.Biz_Employee.GetAll(), o => o.q.EmployeeId, m => m.Id, (o, m) => new { o.p, o.q, m })
                .Where(o => o.p.ExamDateTimeBegin > beginDate && o.p.ExamDateTimeBegin < endDate)
                .Select(o => o.m);

            //企业报名人数必须大于0
            enterpriseQueryable = enterpriseQueryable.Join(totalEmployeeQueryable, p => p.Id, q => q.EnterpriseId, (p, q) => p).Distinct();


            totalCount = enterpriseQueryable.Count();
            int indexBegin = (page - 1) * rows;
            enterpriseQueryable = enterpriseQueryable.OrderByDescending(P => P.Id).Skip(indexBegin).Take(rows);


            IQueryable<Biz_Employee> takeEmployeeQueryable = uow.Biz_ExamPlanRecord.GetAll().Where(p => p.SubmitStatus && p.ExamDateTimeEnd < DateTime.Now)
                .Join(uow.Biz_EmployeeForExamPlanRecord.GetAll(), p => p.Id, q => q.ExamPlanRecordId, (p, q) => new { p, q })
                .Join(uow.Biz_Employee.GetAll(), o => o.q.EmployeeId, m => m.Id, (o, m) => new { o.p, o.q, m })
                .Join(uow.Biz_EmployeeExamResultRecord.GetAll(), o => o.q.EmployeeId, n => n.EmployeeId, (o, m) => new { o.p, o.q, o.m })
                .Where(o => o.p.ExamDateTimeBegin > beginDate && o.p.ExamDateTimeBegin < endDate)
                .Select(o => o.m);

            IQueryable<Biz_Employee> passEmployeeQueryable = uow.Biz_ExamPlanRecord.GetAll().Where(p => p.SubmitStatus && p.ExamDateTimeEnd < DateTime.Now)
                .Join(uow.Biz_EmployeeForExamPlanRecord.GetAll(), p => p.Id, q => q.ExamPlanRecordId, (p, q) => new { p, q })
                .Join(uow.Biz_Employee.GetAll(), o => o.q.EmployeeId, m => m.Id, (o, m) => new { o.p, o.q, m })
                .Join(uow.Biz_EmployeeExamResultRecord.GetAll(), o => o.q.EmployeeId, n => n.EmployeeId, (o, n) => new { o.p, o.q, o.m, n })
                .Join(uow.Biz_PaperForExamType.GetAll().Where(p => p.PaperType == "SafetyKnowledgeExam"), o => new { o.m.ExamType, o.m.Industry }, x => new { x.ExamType, x.Industry }, (o, x) => new { o.p, o.q, o.m, o.n, x })
                .Join(uow.Biz_PaperForExamType.GetAll().Where(p => p.PaperType == "ManagementAbilityExam"), o => new { o.m.ExamType, o.m.Industry }, y => new { y.ExamType, y.Industry }, (o, y) => new { o.p, o.q, o.m, o.n, o.x, y })
                .Where(o => o.p.ExamDateTimeBegin > beginDate && o.p.ExamDateTimeBegin < endDate && o.n.SafetyKnowledgeExamScore >= o.x.PassScore && o.n.ManagementAbilityExamScore >= o.y.PassScore)
                .Select(o => o.m);

            List<EnterpriseExamPassRate> enterpriseExamPassRateList = enterpriseQueryable
                .GroupJoin(totalEmployeeQueryable, p => p.Id, q => q.EnterpriseId, (p, q) => new { p, q })
                .GroupJoin(takeEmployeeQueryable, o => o.p.Id, m => m.EnterpriseId, (o, m) => new { o.p, o.q, m })
                .GroupJoin(passEmployeeQueryable, o => o.p.Id, n => n.EnterpriseId, (o, n) => new { o.p, o.q, o.m, n })
                .Select(o => new EnterpriseExamPassRate()
                {
                    EnterpriseName = o.p.EnterpriseName,
                    TotalCount = o.q.Count(),
                    TakeCount = o.m.Count(),
                    PassCount = o.n.Count()
                }).ToList();

            return enterpriseExamPassRateList;

        }

        public List<InstitutionExamPassRate> GetInstitutionExamPassRate(DateTime beginDate, DateTime endDate, string institutionName, int page, int rows, ref int totalCount)
        {
            const int DATAPERMISSION_CITY = 1;

            IQueryable<Biz_TrainingInstitution> institutionQueryable = uow.Biz_TrainingInstitution.GetAll().Where(p => p.InstitutionName != "测试培训中心");
            if (!institutionName.IsNull())
            {
                institutionQueryable = institutionQueryable.Where(p => p.InstitutionName.Contains(institutionName));
            }

            //总站角色看所有培训机构信息，地市管理部门看各地市培训机构信息
            if (account.RoleList.Where(p => p.RoleType == "Master").Count() > 0)
            {

            }
            else if (account.RoleList.Where(p => p.RoleType == "Manager").Count() > 0)
            {
                List<string> cityList = new List<string>();

                foreach (Sys_Role role in account.RoleList.Where(p => p.RoleType == "Manager").ToList())
                {
                    cityList.AddRange(role.DataPermissionDetailList.Where(p => p.HeadId == DATAPERMISSION_CITY).Select(p => p.DetailName).ToList());
                }
                cityList = cityList.Distinct().ToList();

                if (cityList.Count > 0)
                {
                    institutionQueryable = institutionQueryable.Where(p => cityList.Contains(p.City));
                }
            }


            IQueryable<Biz_Employee> totalEmployeeQueryable = uow.Biz_ExamPlanRecord.GetAll().Where(p => p.SubmitStatus && p.ExamDateTimeEnd < DateTime.Now)
                .Join(uow.Biz_EmployeeForExamPlanRecord.GetAll(), p => p.Id, q => q.ExamPlanRecordId, (p, q) => new { p, q })
                .Join(uow.Biz_Employee.GetAll(), o => o.q.EmployeeId, m => m.Id, (o, m) => new { o.p, o.q, m })
                .Where(o => o.p.ExamDateTimeBegin > beginDate && o.p.ExamDateTimeBegin < endDate)
                .Select(o => o.m);

            //培训机构报名人数必须大于0
            institutionQueryable = institutionQueryable.Join(totalEmployeeQueryable, p => p.Id, q => q.TrainingInstitutionId, (p, q) => p).Distinct();


            totalCount = institutionQueryable.Count();
            int indexBegin = (page - 1) * rows;
            institutionQueryable = institutionQueryable.OrderByDescending(P => P.Id).Skip(indexBegin).Take(rows);


            IQueryable<Biz_Employee> takeEmployeeQueryable = uow.Biz_ExamPlanRecord.GetAll().Where(p => p.SubmitStatus && p.ExamDateTimeEnd < DateTime.Now)
                .Join(uow.Biz_EmployeeForExamPlanRecord.GetAll(), p => p.Id, q => q.ExamPlanRecordId, (p, q) => new { p, q })
                .Join(uow.Biz_Employee.GetAll(), o => o.q.EmployeeId, m => m.Id, (o, m) => new { o.p, o.q, m })
                .Join(uow.Biz_EmployeeExamResultRecord.GetAll(), o => o.q.EmployeeId, n => n.EmployeeId, (o, m) => new { o.p, o.q, o.m })
                .Where(o => o.p.ExamDateTimeBegin > beginDate && o.p.ExamDateTimeBegin < endDate)
                .Select(o => o.m);

            IQueryable<Biz_Employee> passEmployeeQueryable = uow.Biz_ExamPlanRecord.GetAll().Where(p => p.SubmitStatus && p.ExamDateTimeEnd < DateTime.Now)
                .Join(uow.Biz_EmployeeForExamPlanRecord.GetAll(), p => p.Id, q => q.ExamPlanRecordId, (p, q) => new { p, q })
                .Join(uow.Biz_Employee.GetAll(), o => o.q.EmployeeId, m => m.Id, (o, m) => new { o.p, o.q, m })
                .Join(uow.Biz_EmployeeExamResultRecord.GetAll(), o => o.q.EmployeeId, n => n.EmployeeId, (o, n) => new { o.p, o.q, o.m, n })
                .Join(uow.Biz_PaperForExamType.GetAll().Where(p => p.PaperType == "SafetyKnowledgeExam"), o => new { o.m.ExamType, o.m.Industry }, x => new { x.ExamType, x.Industry }, (o, x) => new { o.p, o.q, o.m, o.n, x })
                .Join(uow.Biz_PaperForExamType.GetAll().Where(p => p.PaperType == "ManagementAbilityExam"), o => new { o.m.ExamType, o.m.Industry }, y => new { y.ExamType, y.Industry }, (o, y) => new { o.p, o.q, o.m, o.n, o.x, y })
                .Where(o => o.p.ExamDateTimeBegin > beginDate && o.p.ExamDateTimeBegin < endDate && o.n.SafetyKnowledgeExamScore >= o.x.PassScore && o.n.ManagementAbilityExamScore >= o.y.PassScore)
                .Select(o => o.m);

            List<InstitutionExamPassRate> institutionExamPassRateList = institutionQueryable
                .GroupJoin(totalEmployeeQueryable, p => p.Id, q => q.TrainingInstitutionId, (p, q) => new { p, q })
                .GroupJoin(takeEmployeeQueryable, o => o.p.Id, m => m.TrainingInstitutionId, (o, m) => new { o.p, o.q, m })
                .GroupJoin(passEmployeeQueryable, o => o.p.Id, n => n.TrainingInstitutionId, (o, n) => new { o.p, o.q, o.m, n })
                .Select(o => new InstitutionExamPassRate()
                {
                    InstitutionName = o.p.InstitutionName,
                    TotalCount = o.q.Count(),
                    TakeCount = o.m.Count(),
                    PassCount = o.n.Count()
                }).ToList();

            return institutionExamPassRateList;

        }

        public List<DateTime> GetQueryableMonthList()
        {
            DateTime startDateTime = DateTime.Now.AddYears(-1);
            DateTime endDateTime = DateTime.Now;
            List<DateTime> monthList = new List<DateTime>();
            startDateTime = new DateTime(startDateTime.Year, startDateTime.Month, 1);
            endDateTime = new DateTime(endDateTime.Year, endDateTime.Month, 1);
            while (startDateTime <= endDateTime)
            {
                monthList.Add(startDateTime);
                startDateTime = startDateTime.AddMonths(1);
            }

            return monthList;
        }

        public List<double> GetTotalEmployeeMonthAndCountList()
        {
            DateTime startDateTime = DateTime.Now.AddYears(-1);
            DateTime endDateTime = DateTime.Now;

            List<int> monthList = GetQueryableMonthList().Select(p => p.Month).ToList(); ;

            var employeeCount = uow.Biz_ExamPlanRecord.GetAll()
                .Join(uow.Biz_EmployeeForExamPlanRecord.GetAll(), p => p.Id, q => q.ExamPlanRecordId, (p, q) => new { p, q })
                .Join(uow.Biz_Employee.GetAll(), o => o.q.EmployeeId, m => m.Id, (o, m) => new { o.p, o.q, m })
                .Where(o => o.p.ExamDateTimeBegin > startDateTime && o.p.ExamDateTimeBegin < endDateTime && o.p.SubmitStatus == true)
                .Select(o => new
                {
                    Id = o.m.Id,
                    SubmitDate = o.p.SubmitDate.Value.Month
                }).GroupBy(p => p.SubmitDate)
                .Select(x => new { SubmitDate = x.Key, PersonCount = x.Count() })
                .ToList();

            List<double> countList = monthList.GroupJoin(employeeCount, p => p, q => q.SubmitDate, (p, q) => q == null ? 0 : q.Sum(x => x.PersonCount)).ToList().Select(p => Convert.ToDouble(p))
                .ToList();

            return countList;
        }

        public List<double> GetTakeEmployeeMonthAndCountList()
        {
            DateTime startDateTime = DateTime.Now.AddYears(-1);
            DateTime endDateTime = DateTime.Now;

            List<int> monthList = GetQueryableMonthList().Select(p => p.Month).ToList(); ;

            var employeeCount = uow.Biz_ExamPlanRecord.GetAll()
                .Join(uow.Biz_EmployeeForExamPlanRecord.GetAll(), p => p.Id, q => q.ExamPlanRecordId, (p, q) => new { p, q })
                .Join(uow.Biz_Employee.GetAll(), o => o.q.EmployeeId, m => m.Id, (o, m) => new { o.p, o.q, m })
                .Join(uow.Biz_EmployeeExamResultRecord.GetAll(), o => o.q.EmployeeId, n => n.EmployeeId, (o, n) => new { o.p, o.q, o.m })
                .Where(o => o.p.ExamDateTimeBegin > startDateTime && o.p.ExamDateTimeBegin < endDateTime && o.p.SubmitStatus == true)
                .Select(o => new
                {
                    Id = o.m.Id,
                    SubmitDate = o.p.SubmitDate.Value.Month
                }).GroupBy(p => p.SubmitDate)
                .Select(x => new { SubmitDate = x.Key, PersonCount = x.Count() })
                .ToList();

            List<double> countList = monthList.GroupJoin(employeeCount, p => p, q => q.SubmitDate, (p, q) => q == null ? 0 : q.Sum(x => x.PersonCount)).ToList().Select(p => Convert.ToDouble(p))
                .ToList();

            return countList;
        }

        public List<double> GetPassEmployeeMonthAndCountList()
        {
            DateTime startDateTime = DateTime.Now.AddYears(-1);
            DateTime endDateTime = DateTime.Now;

            List<int> monthList = GetQueryableMonthList().Select(p => p.Month).ToList(); ;

            var employeeCount = uow.Biz_ExamPlanRecord.GetAll()
                .Join(uow.Biz_EmployeeForExamPlanRecord.GetAll(), p => p.Id, q => q.ExamPlanRecordId, (p, q) => new { p, q })
                .Join(uow.Biz_Employee.GetAll(), o => o.q.EmployeeId, m => m.Id, (o, m) => new { o.p, o.q, m })
                .Join(uow.Biz_EmployeeExamResultRecord.GetAll(), o => o.q.EmployeeId, n => n.EmployeeId, (o, n) => new { o.p, o.q, o.m, n })
                .Join(uow.Biz_PaperForExamType.GetAll().Where(p => p.PaperType == "SafetyKnowledgeExam"), o => new { o.m.ExamType, o.m.Industry }, x => new { x.ExamType, x.Industry }, (o, x) => new { o.p, o.q, o.m, o.n, x })
                .Join(uow.Biz_PaperForExamType.GetAll().Where(p => p.PaperType == "ManagementAbilityExam"), o => new { o.m.ExamType, o.m.Industry }, y => new { y.ExamType, y.Industry }, (o, y) => new { o.p, o.q, o.m, o.n, o.x, y })
                .Where(o => o.p.ExamDateTimeBegin > startDateTime && o.p.ExamDateTimeBegin < endDateTime && o.p.SubmitStatus == true && o.n.SafetyKnowledgeExamScore >= o.x.PassScore && o.n.ManagementAbilityExamScore >= o.y.PassScore)
                .Select(o => new
                {
                    Id = o.m.Id,
                    SubmitDate = o.p.SubmitDate.Value.Month
                }).GroupBy(p => p.SubmitDate)
                .Select(x => new { SubmitDate = x.Key, PersonCount = x.Count() })
                .ToList();

            List<double> countList = monthList.GroupJoin(employeeCount, p => p, q => q.SubmitDate, (p, q) => q == null ? 0 : q.Sum(x => x.PersonCount)).ToList().Select(p => Convert.ToDouble(p))
                .ToList();

            return countList;
        }

        #endregion

        public List<Biz_PaperForExamType> GetPaperForExamType()
        {
            return uow.Biz_PaperForExamType.GetAll().ToList();

        }

        public List<Biz_EmployeeExamResultRecord> GetEmployeeExamResultRecord(List<int> employeeIdList)
        {

            return uow.Biz_EmployeeExamResultRecord.GetAll().Where(p => employeeIdList.Contains(p.EmployeeId)).ToList();

        }

        public bool VerifyEmployeeForAuthentication(string idNumber, string examPlanNumber, int examRoomId)
        {
            bool result = false;

            Biz_ExamPlanRecord examPlan = uow.Biz_ExamPlanRecord.GetAll().Where(p => p.ExamPlanNumber == examPlanNumber).Single();
            IQueryable<Biz_Employee> employeeQueryable = uow.Biz_Employee.GetAll()
                .Join(uow.Biz_EmployeeForExamPlanRecord.GetAll(), p => p.Id, q => q.EmployeeId, (p, q) => new { p, q })
                .Where(o => o.q.ExamPlanRecordId == examPlan.Id && o.q.ExamRoomId == examRoomId && o.p.IDNumber == idNumber).Select(o => o.p);

            if (employeeQueryable.Count() > 0)
            {
                result = true;
            }
            return result;

        }
        public int GetEmployeeIdForAuthentication(string idNumber, string examPlanNumber, int examRoomId)
        {


            Biz_ExamPlanRecord examPlan = uow.Biz_ExamPlanRecord.GetAll().Where(p => p.ExamPlanNumber == examPlanNumber).Single();
            Biz_Employee employee = uow.Biz_Employee.GetAll()
                .Join(uow.Biz_EmployeeForExamPlanRecord.GetAll(), p => p.Id, q => q.EmployeeId, (p, q) => new { p, q })
                .Where(o => o.q.ExamPlanRecordId == examPlan.Id && o.q.ExamRoomId == examRoomId && o.p.IDNumber == idNumber).Select(o => o.p).Single();
            return employee.Id;

        }

        public void AddEmployeeAuthentication(Biz_EmployeeAuthentication employeeAuthentication)
        {
            IQueryable<Biz_EmployeeAuthentication> employeeAuthenticationQueryable = uow.Biz_EmployeeAuthentication.GetAll().Where(p => p.EmployeeId == employeeAuthentication.EmployeeId);
            if (employeeAuthenticationQueryable.Count() > 0)
            {
                return;
            }
            uow.Biz_EmployeeAuthentication.Add(employeeAuthentication);
            uow.Commit();
        }
        public Biz_EmployeeAuthentication GetEmployeeAuthenticationByEmployeeId(int employeeId)
        {
            return uow.Biz_EmployeeAuthentication.GetAll().Where(p => p.EmployeeId == employeeId).Single();

        }
        public List<Biz_EmployeeAuthentication> GetEmployeeAuthenticationListByEmployeeIdList(List<int> employeeIdList)
        {
            IQueryable<Biz_EmployeeAuthentication> employeeAuthenticationQueryable = uow.Biz_EmployeeAuthentication.GetAll().Where(p => employeeIdList.Contains(p.EmployeeId));
            return employeeAuthenticationQueryable.ToList();
        }

        public void SaveEmployeeFile(int employeeId, string fileType, string fileName, string filePath)
        {
            Biz_EmployeeFile rpEmployeeFile = uow.Biz_EmployeeFile.GetAll().Where(p => p.FileType == fileType && p.EmployeeId == employeeId).OrderByDescending(p => p.Id).FirstOrDefault();
            if (rpEmployeeFile == null)
            {
                Biz_EmployeeFile newEmployeeFile = new Biz_EmployeeFile()
                {
                    EmployeeId = employeeId,
                    FileType = fileType,
                    FileName = fileName,
                    FilePath = filePath
                };
                uow.Biz_EmployeeFile.Add(newEmployeeFile);
            }
            else
            {
                rpEmployeeFile.FileName = fileName;
                rpEmployeeFile.FilePath = filePath;
                uow.Biz_EmployeeFile.Update(rpEmployeeFile);
            }
            uow.Commit();
        }

        public List<Biz_EmployeeFile> GetEmployeePhotoListByEmployeeIdList(List<int> employeeIdList)
        {
            IQueryable<Biz_EmployeeFile> employeeFileQueryable = uow.Biz_EmployeeFile.GetAll().Where(p => employeeIdList.Contains(p.EmployeeId) && p.FileType == "进场照片");
            return employeeFileQueryable.ToList();
        }

        public Biz_EmployeeFile GetEmployeePhoto(int employeeId)
        {
            IQueryable<Biz_EmployeeFile> employeeFileQueryable = uow.Biz_EmployeeFile.GetAll().Where(p => p.EmployeeId == employeeId && p.FileType == "进场照片");
            return employeeFileQueryable.OrderByDescending(p => p.Id).FirstOrDefault();
        }
        public Biz_EmployeeExamResultRecord GetPublishEmployeeExamResultRecordByIdNumber(string iDNumber)
        {
            List<int> employeeIdList = uow.Biz_Employee.GetAll().Where(p => p.IDNumber == iDNumber).Select(p => p.Id).ToList();

            List<Biz_EmployeeExamResultRecord> employeeExamResultRecordList = this.GetEmployeeExamResultRecordListByEmployeeId(employeeIdList);
            return employeeExamResultRecordList.OrderByDescending(p => p.Id).FirstOrDefault();

        }

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
    }

    public class EnterpriseExamPassRate
    {
        public string EnterpriseName { get; set; }
        public int TotalCount { get; set; }
        public int TakeCount { get; set; }
        public int PassCount { get; set; }
    }

    public class InstitutionExamPassRate
    {
        public string InstitutionName { get; set; }
        public int TotalCount { get; set; }
        public int TakeCount { get; set; }
        public int PassCount { get; set; }
    }

}
