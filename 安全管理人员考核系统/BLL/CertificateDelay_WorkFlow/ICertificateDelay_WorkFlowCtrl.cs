using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL
{
    public interface ICertificateDelay_WorkFlowCtrl
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

        #region 换证审核
        void CertificateDelayConfirm(List<int> certificateIdList, bool checkPass, string checkedMark);

        #endregion

        #region 获取证书Id对应的流程状态
        CertificateDelay_WorkFlowCtrl.WorkFlowStatus GetCurrentWorkFlowStatus(int certificateId);

        #endregion

        #region 获取证书当前的流程状态
        List<CertificateDelay_WorkFlowCtrl.WorkFlowStatus> GetCurrentWorkFlowStatusByCertificateIdList(List<int> certificateIdList);
        #endregion

        #region 获取对应状态下的证书Queryable
        IQueryable<Biz_Certificate> GetCertificateWorkFlowStatusQueryable(string workflowStatus, DAL.Uow workUnit);
        #endregion

        List<Biz_CertificateDelayApplyRecord> GetCertificateDelayApplyRecordListByCertificateIdList(List<int> certificateIdList);
    }
}
