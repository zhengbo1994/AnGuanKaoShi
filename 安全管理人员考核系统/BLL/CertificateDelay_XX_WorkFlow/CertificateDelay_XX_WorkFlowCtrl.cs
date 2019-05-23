﻿using DAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Library.baseFn;
using EntityFramework.Extensions;

namespace BLL
{
    public class CertificateDelay_XX_WorkFlowCtrl : ICertificateDelay_XX_WorkFlowCtrl
    {
        private Uow uow;
        private Sys_Account loginAccount;

        public CertificateDelay_XX_WorkFlowCtrl(Sys_Account account)
        {
            if (uow == null)
            {
                uow = new Uow();
            }
            this.loginAccount = account;
        }

        #region 证书延期申请
        public void CertificateDelayApply(int trainingInstitutionId, int certificateId, string remark)
        {

            List<string> notDelayExamType = new List<string>() { "C" };
            int existsNotDelayExamType = uow.Biz_Certificate.GetAll().Where(p => notDelayExamType.Contains(p.ExamType) && p.Id == certificateId).Count();
            if (existsNotDelayExamType > 0)
            {
                throw new Exception("C类证书不能延期");
            }
            int existsCnt = uow.Biz_CertificateDelayApplyRecord.GetAll().Where(p => p.CertificateId == certificateId).Count();
            if (existsCnt > 0)
            {
                throw new Exception("已经提交线下延期申请,不能提交");
            }
            int existsXXCnt = uow.Biz_XX_CertificateDelayApplyRecord.GetAll().Where(p => p.CertificateId == certificateId).Count();
            if (existsXXCnt > 0)
            {
                throw new Exception("已经提交申请,不能再提交");
            }

            Biz_XX_CertificateDelayApplyRecord certificateDelayApplyRecord = new Biz_XX_CertificateDelayApplyRecord()
            {

                CertificateId = certificateId,
                TrainingInstitutionId = trainingInstitutionId,
                Remark = remark,
                CreateById = this.loginAccount.Id,
                CreateDate = DateTime.Now,
                SummitById = 0,
                SubmitStatus = false,
                SummitDate = null,
                OperationStatus = false,
                Invalid = false
            };
            uow.Biz_XX_CertificateDelayApplyRecord.Add(certificateDelayApplyRecord);
            uow.Commit();
        }
        #endregion

        #region 提交证书延期申请
        public void SubmitCertificateDelayApply(List<int> certificateIdList)
        {

            int existCnt = uow.Biz_XX_CertificateDelayApplyRecord.GetAll().Where(p => certificateIdList.Contains(p.CertificateId) && p.OperationStatus == true).Count();
            if (existCnt > 0)
            {
                throw new Exception("存在已经提交的延期申请");
            }
            uow.Biz_XX_CertificateDelayApplyRecord.GetAll().Where(p => certificateIdList.Contains(p.CertificateId)).Update(p => new Biz_XX_CertificateDelayApplyRecord()
            {
                SubmitStatus = true,
                SummitById = this.loginAccount.Id,
                SummitDate = DateTime.Now
            });
            uow.Commit();
        }
        #endregion

        #region 审核证书延期资料
        public void CheckCertificateDelayData(bool inValidityDate, bool annualSafetyTraining, bool notBadBehavior, bool trainingWith24Hours, bool passStatus, string checkedMark, List<int> certificateIdList)
        {
            //验证是否存在审核过的人
            int existsCheckCnt = uow.Biz_XX_CertificateDelayDataCheckedRecord.GetAll().Where(p => certificateIdList.Contains(p.CertificateId)).Count();
            if (existsCheckCnt > 0)
            {
                throw new Exception("存在已经审核过的人员");
            }
            if (passStatus == true)//必须拍照后才能 审核通过
            {
                int photoCount = uow.Biz_XX_CertificateDelayFile.GetAll().Where(p => p.FileType == "登记照" && certificateIdList.Contains(p.CertificateId)).Count();
                if (photoCount < certificateIdList.Count)
                {
                    throw new Exception("没有拍照不能审核通过");
                }
                int authenticationCount = uow.Biz_XX_CertificateDelayAuthentication.GetAll().Where(p => certificateIdList.Contains(p.CertificateId)).Count();
                if (authenticationCount < certificateIdList.Count)
                {
                    throw new Exception("没有实名不能审核通过");
                }
            }
            //审核
            foreach (int certificateId in certificateIdList)
            {
                Biz_XX_CertificateDelayApplyRecord certificateDelayApplyRecord = uow.Biz_XX_CertificateDelayApplyRecord.GetAll().Where(p => p.CertificateId == certificateId).Single();
                certificateDelayApplyRecord.OperationStatus = true;
                uow.Biz_XX_CertificateDelayApplyRecord.Update(certificateDelayApplyRecord);

                Biz_XX_CertificateDelayDataCheckedRecord newDataCheckedRecord = new Biz_XX_CertificateDelayDataCheckedRecord()
                {
                    CertificateId = certificateId,
                    InValidityDate = inValidityDate,
                    AnnualSafetyTraining = annualSafetyTraining,
                    NotBadBehavior = notBadBehavior,
                    TrainingWith24Hours = trainingWith24Hours,
                    // DelayConditions = delayConditions,`
                    PassStatus = passStatus,
                    OperationStatus = false,
                    CreateDate = DateTime.Now,
                    CreateById = loginAccount.Id,
                    CheckedMark = checkedMark
                };
                uow.Biz_XX_CertificateDelayDataCheckedRecord.Add(newDataCheckedRecord);
            }
            uow.Commit();
            this.SyncDataToAnGuan(certificateIdList);
        }
        private void SyncDataToAnGuan(List<int> certificateIdList)
        {
            int existsCertificateDelayApplyRecordCount = uow.Biz_CertificateDelayApplyRecord.GetAll().Where(p => certificateIdList.Contains(p.CertificateId)).Count();
            int existsCertificateDelayDataCheckedRecordCount = uow.Biz_CertificateDelayDataCheckedRecord.GetAll().Where(p => certificateIdList.Contains(p.CertificateId)).Count();
            if (existsCertificateDelayApplyRecordCount > 0 || existsCertificateDelayDataCheckedRecordCount > 0)
            {
                throw new Exception("已经在【湖北省建筑安管人员考核管理系统】中,申请了延期,不能重复申请");
            }
            foreach (int certificateId in certificateIdList)
            {
                //同步证书延期申请表
                {
                    Biz_XX_CertificateDelayApplyRecord xx_CertificateDelayApplyRecord = uow.Biz_XX_CertificateDelayApplyRecord.GetAll().Where(p => p.CertificateId == certificateId).OrderByDescending(p => p.Id).FirstOrDefault();
                    Biz_CertificateDelayApplyRecord certificateDelayApplyRecord = new Biz_CertificateDelayApplyRecord()
                    {
                        CertificateId = xx_CertificateDelayApplyRecord.CertificateId,
                        TrainingInstitutionId = xx_CertificateDelayApplyRecord.TrainingInstitutionId,
                        Remark = xx_CertificateDelayApplyRecord.Remark,
                        CreateById = xx_CertificateDelayApplyRecord.CreateById,
                        CreateDate = xx_CertificateDelayApplyRecord.CreateDate,
                        SummitById = xx_CertificateDelayApplyRecord.SummitById,
                        SubmitStatus = xx_CertificateDelayApplyRecord.SubmitStatus,
                        SummitDate = xx_CertificateDelayApplyRecord.SummitDate,
                        OperationStatus = xx_CertificateDelayApplyRecord.OperationStatus,
                        Invalid = xx_CertificateDelayApplyRecord.Invalid
                    };
                    uow.Biz_CertificateDelayApplyRecord.Add(certificateDelayApplyRecord);
                }
                //同步证书延期资格审核表
                {
                    Biz_XX_CertificateDelayDataCheckedRecord xx_CertificateDelayDataCheckedRecord = uow.Biz_XX_CertificateDelayDataCheckedRecord.GetAll().Where(p => p.CertificateId == certificateId).OrderByDescending(p => p.Id).FirstOrDefault();
                    Biz_CertificateDelayDataCheckedRecord newCertificateDelayDataCheckedRecord = new Biz_CertificateDelayDataCheckedRecord()
                    {
                        CertificateId = xx_CertificateDelayDataCheckedRecord.CertificateId,
                        InValidityDate = xx_CertificateDelayDataCheckedRecord.InValidityDate,
                        AnnualSafetyTraining = xx_CertificateDelayDataCheckedRecord.AnnualSafetyTraining,
                        NotBadBehavior = xx_CertificateDelayDataCheckedRecord.NotBadBehavior,
                        TrainingWith24Hours = xx_CertificateDelayDataCheckedRecord.TrainingWith24Hours,
                        DelayConditions = xx_CertificateDelayDataCheckedRecord.DelayConditions,
                        PassStatus = xx_CertificateDelayDataCheckedRecord.PassStatus,
                        OperationStatus = xx_CertificateDelayDataCheckedRecord.OperationStatus,
                        CreateDate = xx_CertificateDelayDataCheckedRecord.CreateDate,
                        CreateById = xx_CertificateDelayDataCheckedRecord.CreateById,
                        CheckedMark = xx_CertificateDelayDataCheckedRecord.CheckedMark
                    };
                    uow.Biz_CertificateDelayDataCheckedRecord.Add(newCertificateDelayDataCheckedRecord);
                }
                //同步实名认证信息
                {
                    Biz_XX_CertificateDelayAuthentication xx_CertificateDelayAuthentication = uow.Biz_XX_CertificateDelayAuthentication.GetAll().Where(p => p.CertificateId == certificateId).OrderByDescending(p => p.Id).FirstOrDefault();
                    Biz_CertificateAuthentication newCertificateDelayAuthentication = new Biz_CertificateAuthentication()
                    {
                        CertificateId = xx_CertificateDelayAuthentication.CertificateId,
                        PartyName = xx_CertificateDelayAuthentication.PartyName,
                        Gender = xx_CertificateDelayAuthentication.Gender,
                        Nation = xx_CertificateDelayAuthentication.Nation,
                        BornDay = xx_CertificateDelayAuthentication.BornDay,
                        CertAddress = xx_CertificateDelayAuthentication.CertAddress,
                        CertNumber = xx_CertificateDelayAuthentication.CertNumber,
                        CertOrg = xx_CertificateDelayAuthentication.CertOrg,
                        EffDate = xx_CertificateDelayAuthentication.EffDate,
                        ExpDate = xx_CertificateDelayAuthentication.ExpDate,
                        PictureBase64 = xx_CertificateDelayAuthentication.PictureBase64,
                        SamId = xx_CertificateDelayAuthentication.SamId,
                        CreateById = xx_CertificateDelayAuthentication.CreateById,
                        CreateDate = xx_CertificateDelayAuthentication.CreateDate
                    };
                    uow.Biz_CertificateAuthentication.Add(newCertificateDelayAuthentication);
                }
            }
            uow.Commit();
        }
        #endregion

        #region 获取证书Id对应的流程状态
        public CertificateDelay_XX_WorkFlowCtrl.WorkFlowStatus GetCurrentWorkFlowStatus(int certificateId)
        {
            WorkFlowStatus workFlowStatus = null;
            //延期确认
            {
                Biz_CertificateDelayConfirmRecord certificateDelayConfirmRecord = uow.Biz_CertificateDelayConfirmRecord.GetAll().Where(p => p.CertificateId == certificateId).FirstOrDefault();
                if (!certificateDelayConfirmRecord.IsNull())
                {
                    workFlowStatus = new WorkFlowStatus();
                    if (certificateDelayConfirmRecord.PassStatus)
                    {
                        workFlowStatus.WorkFlowStatusTag = CertificateDelay_XX_WorkFlowCtrl.WorkFlowStatusTag.ConfirmPassed;
                    }
                    else
                    {
                        workFlowStatus.WorkFlowStatusTag = CertificateDelay_XX_WorkFlowCtrl.WorkFlowStatusTag.ConfirmUnpassed;
                    }
                    workFlowStatus.CreateDate = certificateDelayConfirmRecord.CreateDate;
                    workFlowStatus.CreateById = certificateDelayConfirmRecord.CreateById;
                    workFlowStatus.Sequence = 4;
                    return workFlowStatus;
                }
            }
            //资料审核
            {
                Biz_XX_CertificateDelayDataCheckedRecord certificateDelayDataCheckedRecord = uow.Biz_XX_CertificateDelayDataCheckedRecord.GetAll().Where(p => p.CertificateId == certificateId).FirstOrDefault();
                if (!certificateDelayDataCheckedRecord.IsNull())
                {
                    workFlowStatus = new WorkFlowStatus();
                    if (certificateDelayDataCheckedRecord.PassStatus)
                    {
                        workFlowStatus.WorkFlowStatusTag = CertificateDelay_XX_WorkFlowCtrl.WorkFlowStatusTag.DataCheckPassed;
                    }
                    else
                    {
                        workFlowStatus.WorkFlowStatusTag = CertificateDelay_XX_WorkFlowCtrl.WorkFlowStatusTag.DataCheckUnpassed;
                    }
                    workFlowStatus.CreateDate = certificateDelayDataCheckedRecord.CreateDate;
                    workFlowStatus.CreateById = certificateDelayDataCheckedRecord.CreateById;
                    workFlowStatus.Sequence = 3;
                    return workFlowStatus;
                }
            }
            //延期申报
            {
                Biz_XX_CertificateDelayApplyRecord certificateDelayApplyRecord = uow.Biz_XX_CertificateDelayApplyRecord.GetAll().Where(p => p.Id == certificateId).FirstOrDefault();
                if (!certificateDelayApplyRecord.IsNull())
                {
                    workFlowStatus = new WorkFlowStatus()
                    {

                        WorkFlowStatusTag = WorkFlowStatusTag.DelayApplySubmited,
                        CreateDate = certificateDelayApplyRecord.CreateDate,
                        CreateById = certificateDelayApplyRecord.CreateById,
                        Sequence = 2
                    };
                }
            }
            //延期申报未提交
            {
                Biz_Certificate certificate = uow.Biz_Certificate.GetAll().Where(p => p.Id == certificateId).FirstOrDefault();
                if (!certificate.IsNull())
                {
                    workFlowStatus = new WorkFlowStatus()
                    {

                        certificateId = certificate.Id,
                        WorkFlowStatusTag = CertificateDelay_XX_WorkFlowCtrl.WorkFlowStatusTag.DelayApplied,
                        CreateDate = DateTime.Now,
                        CreateById = 0,
                        Sequence = 1,
                    };
                }
            }
            return workFlowStatus;
        }
        #endregion

        #region 获取证书当前的流程状态
        public List<CertificateDelay_XX_WorkFlowCtrl.WorkFlowStatus> GetCurrentWorkFlowStatusByCertificateIdList(List<int> certificateIdList)
        {
            List<CertificateDelay_XX_WorkFlowCtrl.WorkFlowStatus> WorkFlowStatusListResult = new List<CertificateDelay_XX_WorkFlowCtrl.WorkFlowStatus>();
            //延期确认
            {
                List<Biz_CertificateDelayConfirmRecord> CertificateDelayConfirmRecordList = uow.Biz_CertificateDelayConfirmRecord.GetAll().Where(p => certificateIdList.Contains(p.CertificateId)).ToList();
                if (CertificateDelayConfirmRecordList.Count() > 0)
                {

                    List<CertificateDelay_XX_WorkFlowCtrl.WorkFlowStatus> WorkFlowStatusList = CertificateDelayConfirmRecordList.Select(p => new CertificateDelay_XX_WorkFlowCtrl.WorkFlowStatus()
                    {
                        certificateId = p.CertificateId,
                        WorkFlowStatusTag = p.PassStatus ? CertificateDelay_XX_WorkFlowCtrl.WorkFlowStatusTag.ConfirmPassed : CertificateDelay_XX_WorkFlowCtrl.WorkFlowStatusTag.ConfirmUnpassed,
                        CreateDate = p.CreateDate,
                        CreateById = p.CreateById,
                        Sequence = 4
                    }).ToList();
                    WorkFlowStatusListResult.AddRange(WorkFlowStatusList);
                    certificateIdList.RemoveAll(p => WorkFlowStatusList.Select(q => q.certificateId).Contains(p));
                }
            }
            //资料审核
            {
                List<Biz_XX_CertificateDelayDataCheckedRecord> certificateDelayDataCheckedRecordList = uow.Biz_XX_CertificateDelayDataCheckedRecord.GetAll().Where(p => certificateIdList.Contains(p.CertificateId)).ToList();
                if (certificateDelayDataCheckedRecordList.Count() > 0)
                {

                    List<CertificateDelay_XX_WorkFlowCtrl.WorkFlowStatus> WorkFlowStatusList = certificateDelayDataCheckedRecordList.Select(p => new CertificateDelay_XX_WorkFlowCtrl.WorkFlowStatus()
                    {
                        certificateId = p.CertificateId,
                        WorkFlowStatusTag = p.PassStatus ? CertificateDelay_XX_WorkFlowCtrl.WorkFlowStatusTag.DataCheckPassed : CertificateDelay_XX_WorkFlowCtrl.WorkFlowStatusTag.DataCheckUnpassed,
                        CreateDate = p.CreateDate,
                        CreateById = p.CreateById,
                        Sequence = 3
                    }).ToList();
                    WorkFlowStatusListResult.AddRange(WorkFlowStatusList);
                    certificateIdList.RemoveAll(p => WorkFlowStatusList.Select(q => q.certificateId).Contains(p));
                }
            }
            //延期申报
            {
                List<Biz_XX_CertificateDelayApplyRecord> certificateDelayApplyRecordList = uow.Biz_XX_CertificateDelayApplyRecord.GetAll().Where(p => certificateIdList.Contains(p.CertificateId)).ToList();
                if (certificateDelayApplyRecordList.Count() > 0)
                {
                    List<CertificateDelay_XX_WorkFlowCtrl.WorkFlowStatus> WorkFlowStatusList = certificateDelayApplyRecordList.Select(p => new CertificateDelay_XX_WorkFlowCtrl.WorkFlowStatus()
                    {
                        certificateId = p.CertificateId,
                        WorkFlowStatusTag = CertificateDelay_XX_WorkFlowCtrl.WorkFlowStatusTag.DelayApplySubmited,
                        CreateDate = p.CreateDate,
                        CreateById = p.CreateById,
                        Sequence = 2,
                    }).ToList();
                    WorkFlowStatusListResult.AddRange(WorkFlowStatusList);
                    certificateIdList.RemoveAll(p => WorkFlowStatusList.Select(q => q.certificateId).Contains(p));
                }
            }
            //延期申报未提交
            {
                List<Biz_Certificate> certificateList = uow.Biz_Certificate.GetAll().Where(p => certificateIdList.Contains(p.Id)).ToList();
                if (certificateList.Count() > 0)
                {
                    List<CertificateDelay_XX_WorkFlowCtrl.WorkFlowStatus> WorkFlowStatusList = certificateList.Select(p => new CertificateDelay_XX_WorkFlowCtrl.WorkFlowStatus()
                    {
                        certificateId = p.Id,
                        WorkFlowStatusTag = CertificateDelay_XX_WorkFlowCtrl.WorkFlowStatusTag.DelayApplied,
                        CreateDate = DateTime.Now,
                        CreateById = 0,
                        Sequence = 1,
                    }).ToList();
                    WorkFlowStatusListResult.AddRange(WorkFlowStatusList);
                    certificateIdList.RemoveAll(p => WorkFlowStatusList.Select(q => q.certificateId).Contains(p));
                }
            }
            if (certificateIdList.Count() > 0)
            {
                throw new Exception("异常的延期申报流程状态");
            }
            return WorkFlowStatusListResult;

        }
        #endregion

        #region 获取对应状态下的证书Queryable
        public IQueryable<Biz_Certificate> GetCertificateWorkFlowStatusQueryable(string workflowStatus, Uow workUnit)
        {
            IQueryable<Biz_Certificate> certificateQueryable = workUnit.Biz_Certificate.GetAll();
            if (!workflowStatus.IsNull())
            {
                if (workflowStatus == CertificateDelay_XX_WorkFlowCtrl.WorkFlowStatusTag.DelayApplied)
                {
                    IQueryable<Biz_XX_CertificateDelayApplyRecord> certificateDelayApplyRecordQueryable = workUnit.Biz_XX_CertificateDelayApplyRecord.GetAll();
                    certificateQueryable = certificateQueryable.Where(p => !certificateDelayApplyRecordQueryable.Select(q => q.CertificateId).Contains(p.Id));
                }

                if (workflowStatus == CertificateDelay_XX_WorkFlowCtrl.WorkFlowStatusTag.DelayApplySubmited)
                {
                    IQueryable<Biz_XX_CertificateDelayApplyRecord> certificateDelayApplyRecordQueryable = workUnit.Biz_XX_CertificateDelayApplyRecord.GetAll().Where(p => p.OperationStatus == false);
                    certificateQueryable = certificateQueryable.Join(certificateDelayApplyRecordQueryable, a => a.Id, b => b.CertificateId, (a, b) => new { a, b }).Select(o => o.a);
                }
                if (workflowStatus == CertificateDelay_XX_WorkFlowCtrl.WorkFlowStatusTag.DataCheckUnpassed)
                {
                    IQueryable<Biz_XX_CertificateDelayDataCheckedRecord> certificateDelayDataCheckedRecordQueryable = workUnit.Biz_XX_CertificateDelayDataCheckedRecord.GetAll().Where(p => p.PassStatus == false && p.OperationStatus == false);
                    certificateQueryable = certificateQueryable.Join(certificateDelayDataCheckedRecordQueryable, a => a.Id, b => b.CertificateId, (a, b) => new { a, b }).Select(o => o.a);
                }
                if (workflowStatus == CertificateDelay_XX_WorkFlowCtrl.WorkFlowStatusTag.DataCheckPassed)
                {
                    IQueryable<Biz_XX_CertificateDelayDataCheckedRecord> certificateDelayDataCheckedRecordQueryable = workUnit.Biz_XX_CertificateDelayDataCheckedRecord.GetAll().Where(p => p.PassStatus == true && p.OperationStatus == false);
                    certificateQueryable = certificateQueryable.Join(certificateDelayDataCheckedRecordQueryable, a => a.Id, b => b.CertificateId, (a, b) => new { a, b }).Select(o => o.a);
                }
                if (workflowStatus == CertificateDelay_XX_WorkFlowCtrl.WorkFlowStatusTag.ConfirmUnpassed)
                {
                    IQueryable<Biz_CertificateDelayConfirmRecord> certificateDelayConfirmRecordQueryable = workUnit.Biz_CertificateDelayConfirmRecord.GetAll().Where(p => p.PassStatus == false && p.OperationStatus == false);
                    certificateQueryable = certificateQueryable.Join(certificateDelayConfirmRecordQueryable, a => a.Id, b => b.CertificateId, (a, b) => new { a, b }).Select(o => o.a);
                }
                if (workflowStatus == CertificateDelay_XX_WorkFlowCtrl.WorkFlowStatusTag.ConfirmPassed)
                {
                    IQueryable<Biz_CertificateDelayConfirmRecord> certificateDelayConfirmRecordQueryable = workUnit.Biz_CertificateDelayConfirmRecord.GetAll().Where(p => p.PassStatus == true && p.OperationStatus == false);
                    certificateQueryable = certificateQueryable.Join(certificateDelayConfirmRecordQueryable, a => a.Id, b => b.CertificateId, (a, b) => new { a, b }).Select(o => o.a);
                }
            }
            else
            {
                certificateQueryable = certificateQueryable.Where(p => 1 == 2);
            }
            return certificateQueryable;

        }
        #endregion

        public static class WorkFlowStatusTag
        {
            public static string DelayApplied { get { return "延期申请未提交"; } }
            public static string DelayApplySubmited { get { return "延期申请已提交"; } }
            public static string DataCheckUnpassed { get { return "延期资格审核未通过"; } }
            public static string DataCheckPassed { get { return "延期资格审核通过"; } }
            public static string ConfirmUnpassed { get { return "延期确认未通过"; } }
            public static string ConfirmPassed { get { return "延期确认通过"; } }
        }

        public class WorkFlowStatus
        {
            public int certificateId { get; set; }
            public string WorkFlowStatusTag { get; set; }
            public int Sequence { get; set; }
            public DateTime CreateDate { get; set; }
            public int CreateById { get; set; }

        }



        #region 获取延期申请的证书
        public List<Biz_Certificate> GetCertificateListForDelayApply(string employeeName, string idNumber, string certificateNo, string trainingInstitutionName, string workFlowStatus, int page, int rows, ref int totalCount)
        {
            IQueryable<Biz_Certificate> certificateQueryable = uow.Biz_Certificate.GetAll();
            #region 数据权限
            string enterpriseName = uow.Biz_Enterprise.GetById(loginAccount.UserId).EnterpriseName;
            certificateQueryable = certificateQueryable.Where(p => p.EnterpriseName == enterpriseName);
            #endregion

            if (!employeeName.IsNull())
            {
                certificateQueryable = certificateQueryable.Where(p => p.EmployeeName.Contains(employeeName));
            }
            if (!idNumber.IsNull())
            {
                certificateQueryable = certificateQueryable.Where(p => p.IDNumber.Contains(idNumber));
            }
            if (!certificateNo.IsNull())
            {
                certificateQueryable = certificateQueryable.Where(p => p.CertificateNo.Contains(certificateNo));
            }
            if (!trainingInstitutionName.IsNull())
            {
                IQueryable<Biz_TrainingInstitution> trainingInstitutionQueryable = uow.Biz_TrainingInstitution.GetAll().Where(p => p.InstitutionName.Contains(trainingInstitutionName));
                IQueryable<Biz_XX_CertificateDelayApplyRecord> certificateDelayApplyRecordQueryable = uow.Biz_XX_CertificateDelayApplyRecord.GetAll().Join(trainingInstitutionQueryable, a => a.TrainingInstitutionId, b => b.Id, (a, b) => new { a, b }).Select(o => o.a);
                certificateQueryable = certificateQueryable.Where(p => certificateDelayApplyRecordQueryable.Select(q => q.CertificateId).Contains(p.Id));
            }
            if (!workFlowStatus.IsNull())
            {
                ICertificateDelay_XX_WorkFlowCtrl certificateDelay_WorkFlowCtrl = new CertificateDelay_XX_WorkFlowCtrl(this.loginAccount);
                IQueryable<Biz_Certificate> certificateWorkFlowStatusQueryable = certificateDelay_WorkFlowCtrl.GetCertificateWorkFlowStatusQueryable(workFlowStatus, this.uow);
                certificateQueryable = certificateQueryable.Join(certificateWorkFlowStatusQueryable, a => a.Id, b => b.Id, (a, b) => a);
            }
            else
            {
                DateTime enddate = new DateTime(2017, 1, 1);
                certificateQueryable = certificateQueryable.Where(p => p.Invalid == false && p.EndCertificateDate >= enddate);
            }
            certificateQueryable = certificateQueryable
                .GroupJoin(certificateQueryable, p => p.CertificateNo, q => q.CertificateNo, (p, q) => new { p, q })
                .Where(o => o.q.Max(x => x.Id) == o.p.Id).Select(o => o.p);

            totalCount = certificateQueryable.Count();
            int indexBegin = (page - 1) * rows;
            certificateQueryable = certificateQueryable.OrderByDescending(P => P.Id).Skip(indexBegin).Take(rows);
            return certificateQueryable.ToList();
        }
        #endregion

        #region 获取延期资料审核的证书
        public List<Biz_Certificate> GetCertificateListForDelayDataCheck(string employeeName, string idNumber, string enterpriseName, bool? photograph, string checkStatus, string totalHoursFlag, int page, int rows, ref int totalCount)
        {
            IQueryable<Biz_Certificate> certificateQueryable = uow.Biz_Certificate.GetAll();

            #region 数据权限
            //查看提交到本培训机构的证书
            IQueryable<Biz_XX_CertificateDelayApplyRecord> certificateDelayApplyRecordQueryable = uow.Biz_XX_CertificateDelayApplyRecord.GetAll().Where(p => p.TrainingInstitutionId == this.loginAccount.UserId && p.SubmitStatus == true);
            certificateQueryable = certificateQueryable.Join(certificateDelayApplyRecordQueryable, a => a.Id, b => b.CertificateId, (a, b) => new { a, b }).Select(o => o.a);
            #endregion

            if (!employeeName.IsNull())
            {
                certificateQueryable = certificateQueryable.Where(p => p.EmployeeName.Contains(employeeName));
            }
            if (!idNumber.IsNull())
            {
                certificateQueryable = certificateQueryable.Where(p => p.IDNumber.Contains(idNumber));
            }
            if (!enterpriseName.IsNull())
            {
                certificateQueryable = certificateQueryable.Where(p => p.EnterpriseName.Contains(enterpriseName));
            }
            if (!photograph.IsNull())
            {
                IQueryable<Biz_XX_CertificateDelayFile> certificateDelayFileQueryable = uow.Biz_XX_CertificateDelayFile.GetAll().Where(p => p.FileType == "登记照");
                if (photograph == true)
                {

                    certificateQueryable = certificateQueryable.Where(p => certificateDelayFileQueryable.Select(q => q.CertificateId).Contains(p.Id));
                }
                else if (photograph == false)
                {
                    certificateQueryable = certificateQueryable.Where(p => !certificateDelayFileQueryable.Select(q => q.CertificateId).Contains(p.Id));
                }
            }
            if (!totalHoursFlag.IsNull())
            {
                DateTime dtBegin = DateTime.Now.AddMonths(-12);
                DateTime dtEnd = DateTime.Now;
                IQueryable<Biz_StudyByVideoComplete> studyByVideoCompleteQueryable = uow.Biz_StudyByVideoComplete.GetAll().Where(p => dtBegin <= p.CreateDate && p.CreateDate <= dtEnd);


                if (totalHoursFlag == "是")
                {
                    certificateQueryable = certificateQueryable.GroupJoin(studyByVideoCompleteQueryable, a => a.IDNumber, b => b.IDNumber, (a, b) => new { a, b }).Where(o => o.b.Count() >= 24).Select(o => o.a);
                }
                else if (totalHoursFlag == "否")
                {
                    certificateQueryable = certificateQueryable.GroupJoin(studyByVideoCompleteQueryable, a => a.IDNumber, b => b.IDNumber, (a, b) => new { a, b }).Where(o => o.b.Count() < 24).Select(o => o.a);
                }
            }
            if (!checkStatus.IsNull())
            {
                IQueryable<Biz_XX_CertificateDelayDataCheckedRecord> certificateDelayDataCheckedRecordQueryable = uow.Biz_XX_CertificateDelayDataCheckedRecord.GetAll();
                if (checkStatus == "未审核")
                {

                    certificateQueryable = certificateQueryable.Where(p => !certificateDelayDataCheckedRecordQueryable.Select(q => q.CertificateId).Contains(p.Id));
                }
                else if (checkStatus == "审核通过")
                {
                    certificateDelayDataCheckedRecordQueryable = certificateDelayDataCheckedRecordQueryable.Where(p => p.PassStatus == true);
                    certificateQueryable = certificateQueryable.Where(p => certificateDelayDataCheckedRecordQueryable.Select(q => q.CertificateId).Contains(p.Id));
                }
                else if (checkStatus == "审核不通过")
                {
                    certificateDelayDataCheckedRecordQueryable = certificateDelayDataCheckedRecordQueryable.Where(p => p.PassStatus == false);
                    certificateQueryable = certificateQueryable.Where(p => certificateDelayDataCheckedRecordQueryable.Select(q => q.CertificateId).Contains(p.Id));
                }
            }

            totalCount = certificateQueryable.Count();
            int indexBegin = (page - 1) * rows;
            certificateQueryable = certificateQueryable.OrderByDescending(P => P.Id).Skip(indexBegin).Take(rows);
            return certificateQueryable.ToList();
        }
        #endregion

        public List<Biz_XX_CertificateDelayApplyRecord> GetCertificateDelayApplyRecordListByCertificateIdList(List<int> certificateIdList)
        {
            IQueryable<Biz_XX_CertificateDelayApplyRecord> certificateDelayApplyRecordQueryable = uow.Biz_XX_CertificateDelayApplyRecord.GetAll().Where(p => certificateIdList.Contains(p.CertificateId));
            return certificateDelayApplyRecordQueryable.ToList();
        }

        #region 获取证书延期登记照片
        public List<Biz_XX_CertificateDelayFile> GetCertificateDelayPhotoList(List<int> certificateIdList)
        {
            IQueryable<Biz_XX_CertificateDelayFile> certificateDelayFileQueryable = uow.Biz_XX_CertificateDelayFile.GetAll().Where(p => certificateIdList.Contains(p.CertificateId));
            return certificateDelayFileQueryable.ToList();
        }
        #endregion

        #region 获取证书延期资料审核记录
        public List<Biz_XX_CertificateDelayDataCheckedRecord> GetCertificateDelayDataCheckedRecordList(List<int> certificateIdList)
        {
            IQueryable<Biz_XX_CertificateDelayDataCheckedRecord> certificateDelayDataCheckedRecordQueryable = uow.Biz_XX_CertificateDelayDataCheckedRecord.GetAll().Where(p => certificateIdList.Contains(p.CertificateId));
            return certificateDelayDataCheckedRecordQueryable.ToList();
        }
        #endregion

        #region 保存证书延期照片
        public void SaveCertificateDelayFile(int certificateId, string fileType, string fileName, string filePath)
        {
            Biz_XX_CertificateDelayFile certificateDelayFile = uow.Biz_XX_CertificateDelayFile.GetAll().Where(p => p.FileType == fileType && p.CertificateId == certificateId).SingleOrDefault();
            if (certificateDelayFile == null)
            {
                Biz_XX_CertificateDelayFile newCertificateDelayFile = new Biz_XX_CertificateDelayFile()
                {
                    CertificateId = certificateId,
                    FileType = fileType,
                    FileName = fileName,
                    FilePath = filePath
                };
                uow.Biz_XX_CertificateDelayFile.Add(newCertificateDelayFile);
            }
            else
            {
                certificateDelayFile.FileName = fileName;
                certificateDelayFile.FilePath = filePath;
                uow.Biz_XX_CertificateDelayFile.Update(certificateDelayFile);
            }
            uow.Commit();
        }
        #endregion

        #region 获取证书延期照片
        public Biz_XX_CertificateDelayFile GetCertificateDelayPhoto(int certificateId)
        {
            IQueryable<Biz_XX_CertificateDelayFile> certificateDelayFileQueryable = uow.Biz_XX_CertificateDelayFile.GetAll().Where(p => p.CertificateId == certificateId && p.FileType == "登记照");
            return certificateDelayFileQueryable.OrderByDescending(p => p.Id).FirstOrDefault();
        }
        #endregion

        public Biz_Certificate GetCertificateForAuthentication(string iDNumber)
        {
            //已经提交且未实名的延期申请
            IQueryable<Biz_XX_CertificateDelayAuthentication> certificateDelayAuthenticationQueryable = uow.Biz_XX_CertificateDelayAuthentication.GetAll();
            IQueryable<Biz_XX_CertificateDelayApplyRecord> certificateDelayApplyRecordQueryable = uow.Biz_XX_CertificateDelayApplyRecord.GetAll().Where(p => p.SubmitStatus == true).Where(p => !certificateDelayAuthenticationQueryable.Select(q => q.CertificateId).Contains(p.CertificateId));
            IQueryable<Biz_Certificate> certificateQueryable = uow.Biz_Certificate.GetAll().Where(p => p.IDNumber == iDNumber);
            certificateQueryable = certificateQueryable.Join(certificateDelayApplyRecordQueryable, a => a.Id, b => b.CertificateId, (a, b) => new { a, b }).OrderBy(o => o.b.SummitDate).Select(o => o.a);
            return certificateQueryable.FirstOrDefault();
        }
        public void AddCertificateDelayAuthentication(Biz_XX_CertificateDelayAuthentication certificateDelayAuthentication)
        {
            certificateDelayAuthentication.CreateById = this.loginAccount.Id;
            certificateDelayAuthentication.CreateDate = DateTime.Now;
            IQueryable<Biz_XX_CertificateDelayAuthentication> certificateDelayAuthenticationQueryable = uow.Biz_XX_CertificateDelayAuthentication.GetAll().Where(p => p.CertificateId == certificateDelayAuthentication.CertificateId);
            if (certificateDelayAuthenticationQueryable.Count() > 0)
            {
                return;
            }
            uow.Biz_XX_CertificateDelayAuthentication.Add(certificateDelayAuthentication);
            uow.Commit();
        }

        #region 获取证书延期实名认证记录
        public List<Biz_XX_CertificateDelayAuthentication> GetCertificateDelayAuthenticationList(List<int> certificateIdList)
        {
            IQueryable<Biz_XX_CertificateDelayAuthentication> certificateDelayAuthenticationQueryable = uow.Biz_XX_CertificateDelayAuthentication.GetAll().Where(p => certificateIdList.Contains(p.CertificateId));
            return certificateDelayAuthenticationQueryable.ToList();
        }
        #endregion

        #region
        public List<Biz_XX_CertificateDelayApplyRecord> GetAllCertificateDelayRecordList(string employeeName, string idNumber, string certificateNo, string enterpriseName, List<string> cityList, int page, int rows, ref int totalCount)
        {
            IQueryable<Biz_XX_CertificateDelayApplyRecord> certificateDelayApplyRecordQueryable = uow.Biz_XX_CertificateDelayApplyRecord.GetAll();

            #region 数据权限


            if (this.loginAccount.RoleList.Where(p => p.RoleType == RoleCtrl.RoleType.Manager).Count() > 0)
            {
                IQueryable<Biz_Enterprise> enterpriseQueryable = uow.Biz_Enterprise.GetAll();
                IQueryable<Biz_Certificate> certificateQueryable = uow.Biz_Certificate.GetAll();

                certificateDelayApplyRecordQueryable = certificateDelayApplyRecordQueryable.Join(certificateQueryable, p => p.CertificateId, q => q.Id, (p, q) => new { p, q })
                    .Join(enterpriseQueryable, o => o.q.EnterpriseName, m => m.EnterpriseName, (o, m) => new { o.p, o.q, m })
                    .Where(o => cityList.Contains(o.m.City)).Select(o => o.p);
            }
            else if (this.loginAccount.RoleList.Where(p => p.RoleType == RoleCtrl.RoleType.Enterprise).Count() > 0)
            {
                certificateDelayApplyRecordQueryable = certificateDelayApplyRecordQueryable.Where(p => p.CreateById == loginAccount.Id);
            }
            else if (this.loginAccount.RoleList.Where(p => p.RoleType == RoleCtrl.RoleType.TrainingInstitution).Count() > 0)
            {
                certificateDelayApplyRecordQueryable = certificateDelayApplyRecordQueryable.Where(p => p.TrainingInstitutionId == loginAccount.UserId);
            }
            else
            {
                certificateDelayApplyRecordQueryable = certificateDelayApplyRecordQueryable.Where(p => 1 == 2);
            }
            #endregion


            if (!employeeName.IsNull() || !idNumber.IsNull() || !certificateNo.IsNull() || !enterpriseName.IsNull())
            {
                IQueryable<Biz_Certificate> certificateQueryable = uow.Biz_Certificate.GetAll();

                if (!employeeName.IsNull())
                {
                    certificateQueryable = certificateQueryable.Where(p => p.EmployeeName.Contains(employeeName));

                }

                if (!idNumber.IsNull())
                {
                    certificateQueryable = certificateQueryable.Where(p => p.IDNumber.Contains(idNumber));

                }

                if (!certificateNo.IsNull())
                {
                    certificateQueryable = certificateQueryable.Where(p => p.CertificateNo.Contains(certificateNo));
                }

                if (!enterpriseName.IsNull())
                {
                    certificateQueryable = certificateQueryable.Where(p => p.EnterpriseName.Contains(enterpriseName));
                }

                certificateDelayApplyRecordQueryable = certificateDelayApplyRecordQueryable
                    .Join(certificateQueryable, p => p.CertificateId, q => q.Id, (p, q) => p);
            }

            totalCount = certificateDelayApplyRecordQueryable.Count();
            int indexBegin = (page - 1) * rows;
            certificateDelayApplyRecordQueryable = certificateDelayApplyRecordQueryable.OrderByDescending(P => P.Id).Skip(indexBegin).Take(rows);
            return certificateDelayApplyRecordQueryable.ToList();


        }




        #endregion
    }
}
