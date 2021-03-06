﻿using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL
{
    public interface ICertificateDelay_XX_WorkFlowCtrl
    {

        #region 证书延期申请
        void CertificateDelayApply(int trainingInstitutionId, int certificateId, string remark);
        #endregion

        #region 提交证书延期申请
        void SubmitCertificateDelayApply(List<int> certificateIdList);

        #endregion

        #region 审核证书延期资料
        void CheckCertificateDelayData(bool inValidityDate, bool annualSafetyTraining, bool notBadBehavior, bool trainingWith24Hours, bool passStatus, string checkedMark, List<int> certificateIdList);

        #endregion

        #region 获取证书Id对应的流程状态
        CertificateDelay_XX_WorkFlowCtrl.WorkFlowStatus GetCurrentWorkFlowStatus(int certificateId);

        #endregion

        #region 获取证书当前的流程状态
        List<CertificateDelay_XX_WorkFlowCtrl.WorkFlowStatus> GetCurrentWorkFlowStatusByCertificateIdList(List<int> certificateIdList);
        #endregion

        #region 获取对应状态下的证书Queryable
        IQueryable<Biz_Certificate> GetCertificateWorkFlowStatusQueryable(string workflowStatus, DAL.Uow workUnit);
        #endregion


        #region 获取延期申请的证书
        List<Biz_Certificate> GetCertificateListForDelayApply(string employeeName, string idNumber, string certificateNo, string trainingInstitutionName, string workFlowStatus, int page, int rows, ref int totalCount);
        #endregion

        #region 获取延期资料审核的证书
        List<Biz_Certificate> GetCertificateListForDelayDataCheck(string employeeName, string idNumber, string enterpriseName, bool? photograph, string checkStatus, string totalHoursFlag, int page, int rows, ref int totalCount);

        #endregion
        List<Biz_XX_CertificateDelayApplyRecord> GetCertificateDelayApplyRecordListByCertificateIdList(List<int> certificateIdList);

        List<Biz_XX_CertificateDelayFile> GetCertificateDelayPhotoList(List<int> certificateIdList);

        List<Biz_XX_CertificateDelayDataCheckedRecord> GetCertificateDelayDataCheckedRecordList(List<int> certificateIdList);

        #region 保存证书延期照片
        void SaveCertificateDelayFile(int certificateId, string fileType, string fileName, string filePath);
        #endregion

        #region 获取证书延期照片
        Biz_XX_CertificateDelayFile GetCertificateDelayPhoto(int certificateId);
        #endregion
        Biz_Certificate GetCertificateForAuthentication(string iDNumber);
        void AddCertificateDelayAuthentication(Biz_XX_CertificateDelayAuthentication certificateDelayAuthentication);
        #region 获取证书延期实名认证记录
        List<Biz_XX_CertificateDelayAuthentication> GetCertificateDelayAuthenticationList(List<int> certificateIdList);

        #endregion

        List<Biz_XX_CertificateDelayApplyRecord> GetAllCertificateDelayRecordList(string employeeName, string idNumber, string certificateNo, string enterpriseName, List<string> cityList, int page, int rows, ref int totalCount);
    }
}
