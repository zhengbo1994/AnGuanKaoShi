using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;

namespace BLL
{
    public interface IExaminationPointCtrl
    {
        #region 考核点相关操作
        void InsertExaminationPoint(Biz_ExaminationPoint traininginstitution);
        void DeleteExaminationPointById(int trainingInstitutionId);
        void UpdateExaminationPoint(Biz_ExaminationPoint traininginstitution);

        List<Biz_ExaminationPoint> GetExaminationPointList(string InstitutionName,
            string SocialCreditCode, string City, string Area, int page,
            int rows, ref int totalCount, List<string> cityList);
        List<Biz_ExaminationPoint> GetExaminationPointListByCityList(List<string> cityList);
        Biz_ExaminationPoint GetExaminationPointById(int trainingInstitutionId);
        List<Biz_ExaminationPoint> GetExaminationPointByIdList(List<int> trainingInstitutionIdList);
        List<Biz_ExaminationPoint> GetExaminationPointList();
        #endregion
        #region 考场基本信息维护
        void InsertExaminationRoom(Biz_ExaminationRoom examinationRoom);
        void DeleteExaminationRoomById(int examinationRoomId);
        void UpdateExaminationRoom(Biz_ExaminationRoom examinationRoom);

        List<Biz_ExaminationRoom> GetExaminationRoomList(int trainingInstitutionId, string examinationRoomName, int page, int rows, ref int totalCount);
        Biz_ExaminationRoom GetExaminationRoomById(int id);
        List<Biz_ExaminationRoom> GetExaminationRoomByIdList(List<int> idList);
        List<Biz_ExaminationRoom> GetExaminationRoomListByExaminationPointId(int trainingInstitutionId, bool? enable);
        Biz_ExamRoomNVR GetExamRoomNVRByExamRoomId(int examRoomId);
        #endregion

    }
}
