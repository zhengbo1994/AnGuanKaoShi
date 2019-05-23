using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;

namespace BLL
{
    public interface ITrainingInstitutionCtrl
    {
        #region 培训机构相关操作
        void InsertTrainingInstitution(Biz_TrainingInstitution traininginstitution);
        void DeleteTrainingInstitutionById(int trainingInstitutionId);
        void UpdateTrainingInstitution(Biz_TrainingInstitution traininginstitution);

        List<Biz_TrainingInstitution> GetTrainingInstitutionList(string InstitutionName,
            string SocialCreditCode, string City, string Area, int page,
            int rows, ref int totalCount, List<string> cityList);
        List<Biz_TrainingInstitution> GetTrainingInstitutionListByCityList(List<string> cityList);
        List<Biz_TrainingInstitution> GetAccreditTrainingInstitutionList();
        Biz_TrainingInstitution GetTrainingInstitutionById(int trainingInstitutionId);
        List<Biz_TrainingInstitution> GetTrainingInstitutionByIdList(List<int> trainingInstitutionIdList);
        List<Biz_TrainingInstitution> GetTrainingInstitutionList();
        #endregion

        bool VerifyTrainingInstitutionAccredit(int trainingInstitutionId);

    }
}
