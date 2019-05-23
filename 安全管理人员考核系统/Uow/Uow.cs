using System;
using System.Collections.Generic;
using System.Transactions;
using Model;


namespace DAL
{
    public class Uow : IDisposable
    {
        private DatabaseContext DbContext { get; set; }

        public Uow()
        {
            CreateDbContext("DefaultConnection");
            //CreateDbContext("entityFramework");
        }

        public Uow(string connection)
        {
            CreateDbContext(connection);
        }

        public void Commit()
        {
            DbContext.SaveChanges();
        }

        private void CreateDbContext(string connection)
        {
            DbContext = new DatabaseContext(connection);
            DbContext.Configuration.ProxyCreationEnabled = false;
            DbContext.Configuration.LazyLoadingEnabled = false;
            DbContext.Configuration.ValidateOnSaveEnabled = false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public static void CreateTransaction(List<Uow> uowList)
        {
            TransactionScope scope = new TransactionScope();
            foreach (Uow uow in uowList)
            {
                uow.Commit();
            }
            scope.Complete();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (DbContext != null)
                    DbContext.Dispose();
            }
        }

        private Repository<Sys_Account> _rSys_Account;
        public Repository<Sys_Account> Sys_Account
        {
            get
            {
                if (_rSys_Account == null)
                    _rSys_Account = new Repository<Sys_Account>(DbContext);
                return _rSys_Account;
            }
        }

        private Repository<Sys_Role> _rSys_Role;
        public Repository<Sys_Role> Sys_Role
        {
            get
            {
                if (_rSys_Role == null)
                    _rSys_Role = new Repository<Sys_Role>(DbContext);
                return _rSys_Role;
            }
        }

        private Repository<Sys_Permission> _rSys_Permission;
        public Repository<Sys_Permission> Sys_Permission
        {
            get
            {
                if (_rSys_Permission == null)
                    _rSys_Permission = new Repository<Sys_Permission>(DbContext);
                return _rSys_Permission;
            }
        }

        private Repository<Sys_Page> _rSys_Page;
        public Repository<Sys_Page> Sys_Page
        {
            get
            {
                if (_rSys_Page == null)
                    _rSys_Page = new Repository<Sys_Page>(DbContext);
                return _rSys_Page;
            }
        }

        private Repository<Sys_RelAccountRole> _rSys_RelAccountRole;
        public Repository<Sys_RelAccountRole> Sys_RelAccountRole
        {
            get
            {
                if (_rSys_RelAccountRole == null)
                    _rSys_RelAccountRole = new Repository<Sys_RelAccountRole>(DbContext);
                return _rSys_RelAccountRole;
            }
        }

        private Repository<Sys_RelRolePermission> _rSys_RelRolePermission;
        public Repository<Sys_RelRolePermission> Sys_RelRolePermission
        {
            get
            {
                if (_rSys_RelRolePermission == null)
                    _rSys_RelRolePermission = new Repository<Sys_RelRolePermission>(DbContext);
                return _rSys_RelRolePermission;
            }
        }

        private Repository<Sys_RelPermissionPage> _rSys_RelPermissionPage;
        public Repository<Sys_RelPermissionPage> Sys_RelPermissionPage
        {
            get
            {
                if (_rSys_RelPermissionPage == null)
                    _rSys_RelPermissionPage = new Repository<Sys_RelPermissionPage>(DbContext);
                return _rSys_RelPermissionPage;
            }
        }

        private Repository<Sys_DataPermissionHead> _rSys_DataPermissionHead;
        public Repository<Sys_DataPermissionHead> Sys_DataPermissionHead
        {
            get
            {
                if (_rSys_DataPermissionHead == null)
                    _rSys_DataPermissionHead = new Repository<Sys_DataPermissionHead>(DbContext);
                return _rSys_DataPermissionHead;
            }
        }

        private Repository<Sys_DataPermissionDetail> _rSys_DataPermissionDetail;
        public Repository<Sys_DataPermissionDetail> Sys_DataPermissionDetail
        {
            get
            {
                if (_rSys_DataPermissionDetail == null)
                    _rSys_DataPermissionDetail = new Repository<Sys_DataPermissionDetail>(DbContext);
                return _rSys_DataPermissionDetail;
            }
        }

        private Repository<Sys_RelRoleDataPermissionDetail> _rSys_RelRoleDataPermissionDetail;
        public Repository<Sys_RelRoleDataPermissionDetail> Sys_RelRoleDataPermissionDetail
        {
            get
            {
                if (_rSys_RelRoleDataPermissionDetail == null)
                    _rSys_RelRoleDataPermissionDetail = new Repository<Sys_RelRoleDataPermissionDetail>(DbContext);
                return _rSys_RelRoleDataPermissionDetail;
            }
        }

        private Repository<Biz_Employee> _rBiz_Employee;
        public Repository<Biz_Employee> Biz_Employee
        {
            get
            {
                if (_rBiz_Employee == null)
                    _rBiz_Employee = new Repository<Biz_Employee>(DbContext);
                return _rBiz_Employee;
            }
        }

        //private Repository<Biz_EmployeeAssignForCheckRecord> _rBiz_EmployeeAssignForCheckRecord;
        //public Repository<Biz_EmployeeAssignForCheckRecord> Biz_EmployeeAssignForCheckRecord
        //{
        //    get
        //    {
        //        if (_rBiz_EmployeeAssignForCheckRecord == null)
        //            _rBiz_EmployeeAssignForCheckRecord = new Repository<Biz_EmployeeAssignForCheckRecord>(DbContext);
        //        return _rBiz_EmployeeAssignForCheckRecord;
        //    }
        //}

        private Repository<Biz_EmployeeCheckedRecord> _rBiz_EmployeeCheckedRecord;
        public Repository<Biz_EmployeeCheckedRecord> Biz_EmployeeCheckedRecord
        {
            get
            {
                if (_rBiz_EmployeeCheckedRecord == null)
                    _rBiz_EmployeeCheckedRecord = new Repository<Biz_EmployeeCheckedRecord>(DbContext);
                return _rBiz_EmployeeCheckedRecord;
            }
        }

        private Repository<Biz_ExamPlanRecord> _rBiz_ExamPlanRecord;
        public Repository<Biz_ExamPlanRecord> Biz_ExamPlanRecord
        {
            get
            {
                if (_rBiz_ExamPlanRecord == null)
                    _rBiz_ExamPlanRecord = new Repository<Biz_ExamPlanRecord>(DbContext);
                return _rBiz_ExamPlanRecord;
            }
        }

        private Repository<Biz_EmployeeForExamPlanRecord> _rBiz_EmployeeForExamPlanRecord;
        public Repository<Biz_EmployeeForExamPlanRecord> Biz_EmployeeForExamPlanRecord
        {
            get
            {
                if (_rBiz_EmployeeForExamPlanRecord == null)
                    _rBiz_EmployeeForExamPlanRecord = new Repository<Biz_EmployeeForExamPlanRecord>(DbContext);
                return _rBiz_EmployeeForExamPlanRecord;
            }
        }

        private Repository<Biz_EmployeeExamResultRecord> _rBiz_EmployeeExamResultRecord;
        public Repository<Biz_EmployeeExamResultRecord> Biz_EmployeeExamResultRecord
        {
            get
            {
                if (_rBiz_EmployeeExamResultRecord == null)
                    _rBiz_EmployeeExamResultRecord = new Repository<Biz_EmployeeExamResultRecord>(DbContext);
                return _rBiz_EmployeeExamResultRecord;
            }
        }

        private Repository<Biz_EmployeeExamResultCheckedRecord> _rBiz_EmployeeExamResultCheckedRecord;
        public Repository<Biz_EmployeeExamResultCheckedRecord> Biz_EmployeeExamResultCheckedRecord
        {
            get
            {
                if (_rBiz_EmployeeExamResultCheckedRecord == null)
                    _rBiz_EmployeeExamResultCheckedRecord = new Repository<Biz_EmployeeExamResultCheckedRecord>(DbContext);
                return _rBiz_EmployeeExamResultCheckedRecord;
            }
        }

        private Repository<Biz_EmployeeCertificateIssuanceRecord> _rBiz_EmployeeCertificateIssuanceRecord;
        public Repository<Biz_EmployeeCertificateIssuanceRecord> Biz_EmployeeCertificateIssuanceRecord
        {
            get
            {
                if (_rBiz_EmployeeCertificateIssuanceRecord == null)
                    _rBiz_EmployeeCertificateIssuanceRecord = new Repository<Biz_EmployeeCertificateIssuanceRecord>(DbContext);
                return _rBiz_EmployeeCertificateIssuanceRecord;
            }
        }
        private Repository<Biz_City> _rBiz_City;
        public Repository<Biz_City> Biz_City
        {
            get
            {
                if (_rBiz_City == null)
                    _rBiz_City = new Repository<Biz_City>(DbContext);
                return _rBiz_City;
            }
        }
        private Repository<Biz_Area> _rBiz_Area;
        public Repository<Biz_Area> Biz_Area
        {
            get
            {
                if (_rBiz_Area == null)
                    _rBiz_Area = new Repository<Biz_Area>(DbContext);
                return _rBiz_Area;
            }
        }
        private Repository<Sys_DropdownListItem> _rSys_DropdownListItem;
        public Repository<Sys_DropdownListItem> Sys_DropdownListItem
        {
            get
            {
                if (_rSys_DropdownListItem == null)
                    _rSys_DropdownListItem = new Repository<Sys_DropdownListItem>(DbContext);
                return _rSys_DropdownListItem;
            }
        }
        private Repository<Biz_Enterprise> _rBiz_Enterprise;
        public Repository<Biz_Enterprise> Biz_Enterprise
        {
            get
            {
                if (_rBiz_Enterprise == null)
                    _rBiz_Enterprise = new Repository<Biz_Enterprise>(DbContext);
                return _rBiz_Enterprise;
            }
        }

        private Repository<Biz_ExaminationPoint> _rBiz_ExaminationPoint;
        public Repository<Biz_ExaminationPoint> Biz_ExaminationPoint
        {
            get
            {
                if (_rBiz_ExaminationPoint == null)
                    _rBiz_ExaminationPoint = new Repository<Biz_ExaminationPoint>(DbContext);
                return _rBiz_ExaminationPoint;
            }
        }
        private Repository<Biz_TrainingInstitution> _rBiz_TrainingInstitution;
        public Repository<Biz_TrainingInstitution> Biz_TrainingInstitution
        {
            get
            {
                if (_rBiz_TrainingInstitution == null)
                    _rBiz_TrainingInstitution = new Repository<Biz_TrainingInstitution>(DbContext);
                return _rBiz_TrainingInstitution;
            }
        }

        private Repository<Biz_ExaminationRoom> _rBiz_ExaminationRoom;
        public Repository<Biz_ExaminationRoom> Biz_ExaminationRoom
        {
            get
            {
                if (_rBiz_ExaminationRoom == null)
                    _rBiz_ExaminationRoom = new Repository<Biz_ExaminationRoom>(DbContext);
                return _rBiz_ExaminationRoom;
            }
        }

        private Repository<Biz_EmployeeExamResultRecordFile> _rBiz_EmployeeExamResultRecordFile;
        public Repository<Biz_EmployeeExamResultRecordFile> Biz_EmployeeExamResultRecordFile
        {
            get
            {
                if (_rBiz_EmployeeExamResultRecordFile == null)
                    _rBiz_EmployeeExamResultRecordFile = new Repository<Biz_EmployeeExamResultRecordFile>(DbContext);
                return _rBiz_EmployeeExamResultRecordFile;
            }
        }

        private Repository<Biz_PaperForExamType> _rBiz_PaperForExamType;
        public Repository<Biz_PaperForExamType> Biz_PaperForExamType
        {
            get
            {
                if (_rBiz_PaperForExamType == null)
                    _rBiz_PaperForExamType = new Repository<Biz_PaperForExamType>(DbContext);
                return _rBiz_PaperForExamType;
            }
        }

        private Repository<Biz_EmployeeFile> _rBiz_EmployeeFile;
        public Repository<Biz_EmployeeFile> Biz_EmployeeFile
        {
            get
            {
                if (_rBiz_EmployeeFile == null)
                    _rBiz_EmployeeFile = new Repository<Biz_EmployeeFile>(DbContext);
                return _rBiz_EmployeeFile;
            }
        }

        private Repository<Biz_TrainingRecord> _rBiz_TrainingRecord;
        public Repository<Biz_TrainingRecord> Biz_TrainingRecord
        {
            get
            {
                if (_rBiz_TrainingRecord == null)
                    _rBiz_TrainingRecord = new Repository<Biz_TrainingRecord>(DbContext);
                return _rBiz_TrainingRecord;
            }
        }
        private Repository<Biz_CertificatePrintRecord> _rBiz_CertificatePrintRecord;
        public Repository<Biz_CertificatePrintRecord> Biz_CertificatePrintRecord
        {
            get
            {
                if (_rBiz_CertificatePrintRecord == null)
                    _rBiz_CertificatePrintRecord = new Repository<Biz_CertificatePrintRecord>(DbContext);
                return _rBiz_CertificatePrintRecord;
            }
        }


        private Repository<Biz_Certificate> _rBiz_Certificate;
        public Repository<Biz_Certificate> Biz_Certificate
        {
            get
            {
                if (_rBiz_Certificate == null)
                    _rBiz_Certificate = new Repository<Biz_Certificate>(DbContext);
                return _rBiz_Certificate;
            }
        }



        private Repository<Biz_News> _rBiz_News;
        public Repository<Biz_News> Biz_News
        {
            get
            {
                if (_rBiz_News == null)
                    _rBiz_News = new Repository<Biz_News>(DbContext);
                return _rBiz_News;
            }
        }

        private Repository<Biz_EnterpriseWebServiceResult> _rBiz_EnterpriseWebServiceResult;
        public Repository<Biz_EnterpriseWebServiceResult> Biz_EnterpriseWebServiceResult
        {
            get
            {
                if (_rBiz_EnterpriseWebServiceResult == null)
                    _rBiz_EnterpriseWebServiceResult = new Repository<Biz_EnterpriseWebServiceResult>(DbContext);
                return _rBiz_EnterpriseWebServiceResult;
            }
        }
        private Repository<Biz_CertificateOperationRecord> _rBiz_CertificateOperationRecord;
        public Repository<Biz_CertificateOperationRecord> Biz_CertificateOperationRecord
        {
            get
            {
                if (_rBiz_CertificateOperationRecord == null)
                    _rBiz_CertificateOperationRecord = new Repository<Biz_CertificateOperationRecord>(DbContext);
                return _rBiz_CertificateOperationRecord;
            }
        }
        private Repository<Biz_CertificateChangeRecord> _rBiz_CertificateChangeRecord;
        public Repository<Biz_CertificateChangeRecord> Biz_CertificateChangeRecord
        {
            get
            {
                if (_rBiz_CertificateChangeRecord == null)
                    _rBiz_CertificateChangeRecord = new Repository<Biz_CertificateChangeRecord>(DbContext);
                return _rBiz_CertificateChangeRecord;
            }
        }
        private Repository<Biz_StudyByVideoRecoder> _rBiz_StudyByVideoRecoder;
        public Repository<Biz_StudyByVideoRecoder> Biz_StudyByVideoRecoder
        {
            get
            {
                if (_rBiz_StudyByVideoRecoder == null)
                    _rBiz_StudyByVideoRecoder = new Repository<Biz_StudyByVideoRecoder>(DbContext);
                return _rBiz_StudyByVideoRecoder;
            }
        }

        private Repository<Biz_ExamRoomNVR> _rBiz_ExamRoomNVR;
        public Repository<Biz_ExamRoomNVR> Biz_ExamRoomNVR
        {
            get
            {
                if (_rBiz_ExamRoomNVR == null)
                    _rBiz_ExamRoomNVR = new Repository<Biz_ExamRoomNVR>(DbContext);
                return _rBiz_ExamRoomNVR;
            }
        }
        private Repository<Biz_OnlineExerciseRecord> _rBiz_OnlineExerciseRecord;
        public Repository<Biz_OnlineExerciseRecord> Biz_OnlineExerciseRecord
        {
            get
            {
                if (_rBiz_OnlineExerciseRecord == null)
                    _rBiz_OnlineExerciseRecord = new Repository<Biz_OnlineExerciseRecord>(DbContext);
                return _rBiz_OnlineExerciseRecord;
            }
        }
        private Repository<Biz_SimulatedExamRecord> _rBiz_SimulatedExamRecord;
        public Repository<Biz_SimulatedExamRecord> Biz_SimulatedExamRecord
        {
            get
            {
                if (_rBiz_SimulatedExamRecord == null)
                    _rBiz_SimulatedExamRecord = new Repository<Biz_SimulatedExamRecord>(DbContext);
                return _rBiz_SimulatedExamRecord;
            }
        }


        private Repository<Biz_StudyByVideoComplete> _rBiz_StudyByVideoComplete;
        public Repository<Biz_StudyByVideoComplete> Biz_StudyByVideoComplete
        {
            get
            {
                if (_rBiz_StudyByVideoComplete == null)
                    _rBiz_StudyByVideoComplete = new Repository<Biz_StudyByVideoComplete>(DbContext);
                return _rBiz_StudyByVideoComplete;
            }
        }
        private Repository<Biz_RP_EmployeeRegistration> _rBiz_RP_EmployeeRegistration;
        public Repository<Biz_RP_EmployeeRegistration> Biz_RP_EmployeeRegistration
        {
            get
            {
                if (_rBiz_RP_EmployeeRegistration == null)
                    _rBiz_RP_EmployeeRegistration = new Repository<Biz_RP_EmployeeRegistration>(DbContext);
                return _rBiz_RP_EmployeeRegistration;
            }
        }
        private Repository<Biz_RP_EmployeeDataCheckedRecord> _rBiz_RP_EmployeeDataCheckedRecord;
        public Repository<Biz_RP_EmployeeDataCheckedRecord> Biz_RP_EmployeeDataCheckedRecord
        {
            get
            {
                if (_rBiz_RP_EmployeeDataCheckedRecord == null)
                    _rBiz_RP_EmployeeDataCheckedRecord = new Repository<Biz_RP_EmployeeDataCheckedRecord>(DbContext);
                return _rBiz_RP_EmployeeDataCheckedRecord;
            }
        }

        private Repository<Biz_RP_EmployeeCheckedRecord> _rBiz_RP_EmployeeCheckedRecord;
        public Repository<Biz_RP_EmployeeCheckedRecord> Biz_RP_EmployeeCheckedRecord
        {
            get
            {
                if (_rBiz_RP_EmployeeCheckedRecord == null)
                    _rBiz_RP_EmployeeCheckedRecord = new Repository<Biz_RP_EmployeeCheckedRecord>(DbContext);
                return _rBiz_RP_EmployeeCheckedRecord;
            }
        }


        private Repository<Biz_RelEmployeeCertificate> _rBiz_RelEmployeeCertificate;
        public Repository<Biz_RelEmployeeCertificate> Biz_RelEmployeeCertificate
        {
            get
            {
                if (_rBiz_RelEmployeeCertificate == null)
                    _rBiz_RelEmployeeCertificate = new Repository<Biz_RelEmployeeCertificate>(DbContext);
                return _rBiz_RelEmployeeCertificate;
            }
        }

        private Repository<Biz_RelRPEmployeeCertificate> _rBiz_RelRPEmployeeCertificate;
        public Repository<Biz_RelRPEmployeeCertificate> Biz_RelRPEmployeeCertificate
        {
            get
            {
                if (_rBiz_RelRPEmployeeCertificate == null)
                    _rBiz_RelRPEmployeeCertificate = new Repository<Biz_RelRPEmployeeCertificate>(DbContext);
                return _rBiz_RelRPEmployeeCertificate;
            }
        }

        private Repository<Biz_EmployeeAuthentication> _rBiz_EmployeeAuthentication;
        public Repository<Biz_EmployeeAuthentication> Biz_EmployeeAuthentication
        {
            get
            {
                if (_rBiz_EmployeeAuthentication == null)
                    _rBiz_EmployeeAuthentication = new Repository<Biz_EmployeeAuthentication>(DbContext);
                return _rBiz_EmployeeAuthentication;
            }
        }
        private Repository<Biz_RP_CertificatePrintRecord> _rBiz_RP_CertificatePrintRecord;
        public Repository<Biz_RP_CertificatePrintRecord> Biz_RP_CertificatePrintRecord
        {
            get
            {
                if (_rBiz_RP_CertificatePrintRecord == null)
                    _rBiz_RP_CertificatePrintRecord = new Repository<Biz_RP_CertificatePrintRecord>(DbContext);
                return _rBiz_RP_CertificatePrintRecord;
            }
        }

        private Repository<Biz_RP_EmployeeFile> _rBiz_RP_EmployeeFile;
        public Repository<Biz_RP_EmployeeFile> Biz_RP_EmployeeFile
        {
            get
            {
                if (_rBiz_RP_EmployeeFile == null)
                    _rBiz_RP_EmployeeFile = new Repository<Biz_RP_EmployeeFile>(DbContext);
                return _rBiz_RP_EmployeeFile;
            }
        }
        private Repository<Biz_CertificateAuthenticationLog> _rBiz_CertificateAuthenticationLog;
        public Repository<Biz_CertificateAuthenticationLog> Biz_CertificateAuthenticationLog
        {
            get
            {
                if (_rBiz_CertificateAuthenticationLog == null)
                    _rBiz_CertificateAuthenticationLog = new Repository<Biz_CertificateAuthenticationLog>(DbContext);
                return _rBiz_CertificateAuthenticationLog;
            }
        }

        private Repository<Biz_CertificateDelayApplyRecord> _rBiz_CertificateDelayApplyRecord;
        public Repository<Biz_CertificateDelayApplyRecord> Biz_CertificateDelayApplyRecord
        {
            get
            {
                if (_rBiz_CertificateDelayApplyRecord == null)
                    _rBiz_CertificateDelayApplyRecord = new Repository<Biz_CertificateDelayApplyRecord>(DbContext);
                return _rBiz_CertificateDelayApplyRecord;
            }
        }


        private Repository<Biz_CertificateDelayDataCheckedRecord> _rBiz_CertificateDelayDataCheckedRecord;
        public Repository<Biz_CertificateDelayDataCheckedRecord> Biz_CertificateDelayDataCheckedRecord
        {
            get
            {
                if (_rBiz_CertificateDelayDataCheckedRecord == null)
                    _rBiz_CertificateDelayDataCheckedRecord = new Repository<Biz_CertificateDelayDataCheckedRecord>(DbContext);
                return _rBiz_CertificateDelayDataCheckedRecord;
            }
        }

        private Repository<Biz_CertificateDelayConfirmRecord> _rBiz_CertificateDelayConfirmRecord;
        public Repository<Biz_CertificateDelayConfirmRecord> Biz_CertificateDelayConfirmRecord
        {
            get
            {
                if (_rBiz_CertificateDelayConfirmRecord == null)
                    _rBiz_CertificateDelayConfirmRecord = new Repository<Biz_CertificateDelayConfirmRecord>(DbContext);
                return _rBiz_CertificateDelayConfirmRecord;
            }
        }

        private Repository<Biz_CertificateDelayFile> _rBiz_CertificateDelayFile;
        public Repository<Biz_CertificateDelayFile> Biz_CertificateDelayFile
        {
            get
            {
                if (_rBiz_CertificateDelayFile == null)
                    _rBiz_CertificateDelayFile = new Repository<Biz_CertificateDelayFile>(DbContext);
                return _rBiz_CertificateDelayFile;
            }
        }
        private Repository<Biz_XX_CertificateDelayApplyRecord> _rBiz_XX_CertificateDelayApplyRecord;
        public Repository<Biz_XX_CertificateDelayApplyRecord> Biz_XX_CertificateDelayApplyRecord
        {
            get
            {
                if (_rBiz_XX_CertificateDelayApplyRecord == null)
                    _rBiz_XX_CertificateDelayApplyRecord = new Repository<Biz_XX_CertificateDelayApplyRecord>(DbContext);
                return _rBiz_XX_CertificateDelayApplyRecord;
            }
        }

        private Repository<Biz_XX_CertificateDelayDataCheckedRecord> _rBiz_XX_CertificateDelayDataCheckedRecord;
        public Repository<Biz_XX_CertificateDelayDataCheckedRecord> Biz_XX_CertificateDelayDataCheckedRecord
        {
            get
            {
                if (_rBiz_XX_CertificateDelayDataCheckedRecord == null)
                    _rBiz_XX_CertificateDelayDataCheckedRecord = new Repository<Biz_XX_CertificateDelayDataCheckedRecord>(DbContext);
                return _rBiz_XX_CertificateDelayDataCheckedRecord;
            }
        }
        private Repository<Biz_XX_CertificateDelayFile> _rBiz_XX_CertificateDelayFile;
        public Repository<Biz_XX_CertificateDelayFile> Biz_XX_CertificateDelayFile
        {
            get
            {
                if (_rBiz_XX_CertificateDelayFile == null)
                    _rBiz_XX_CertificateDelayFile = new Repository<Biz_XX_CertificateDelayFile>(DbContext);
                return _rBiz_XX_CertificateDelayFile;
            }
        }

        private Repository<Biz_TrainingInstitutionAccredit> _rBiz_TrainingInstitutionAccredit;
        public Repository<Biz_TrainingInstitutionAccredit> Biz_TrainingInstitutionAccredit
        {
            get
            {
                if (_rBiz_TrainingInstitutionAccredit == null)
                    _rBiz_TrainingInstitutionAccredit = new Repository<Biz_TrainingInstitutionAccredit>(DbContext);
                return _rBiz_TrainingInstitutionAccredit;
            }
        }

        private Repository<Biz_CertificateAuthentication> _rBiz_CertificateAuthentication;
        public Repository<Biz_CertificateAuthentication> Biz_CertificateAuthentication
        {
            get
            {
                if (_rBiz_CertificateAuthentication == null)
                    _rBiz_CertificateAuthentication = new Repository<Biz_CertificateAuthentication>(DbContext);
                return _rBiz_CertificateAuthentication;
            }
        }
        private Repository<Biz_RelCertificateAuthentication> _rBiz_RelCertificateAuthentication;
        public Repository<Biz_RelCertificateAuthentication> Biz_RelCertificateAuthentication
        {
            get
            {
                if (_rBiz_RelCertificateAuthentication == null)
                    _rBiz_RelCertificateAuthentication = new Repository<Biz_RelCertificateAuthentication>(DbContext);
                return _rBiz_RelCertificateAuthentication;
            }
        }


        private Repository<Biz_XX_CertificateDelayAuthentication> _rBiz_XX_CertificateDelayAuthentication;
        public Repository<Biz_XX_CertificateDelayAuthentication> Biz_XX_CertificateDelayAuthentication
        {
            get
            {
                if (_rBiz_XX_CertificateDelayAuthentication == null)
                    _rBiz_XX_CertificateDelayAuthentication = new Repository<Biz_XX_CertificateDelayAuthentication>(DbContext);
                return _rBiz_XX_CertificateDelayAuthentication;
            }
        }
        
    }
}