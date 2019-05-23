using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Model;
using Library.baseFn;
using DAL;
using System.Reflection;

namespace BLL
{
    public class ExaminationPointCtrl : IExaminationPointCtrl
    {
        private Sys_Account account;
        private Uow uow;

        public ExaminationPointCtrl(Sys_Account account)
        {
            // TODO: Complete member initialization
            this.account = account;
            if (uow == null)
            {
                uow = new Uow();
            }
        }
        public void InsertExaminationPoint(Biz_ExaminationPoint traininginstitution)
        {
            bool exaistSocialCreditCode = uow.Biz_ExaminationPoint.GetAll().Where(p => p.SocialCreditCode == traininginstitution.SocialCreditCode).Where(p => p.Id != traininginstitution.Id).Count() > 0 ? true : false;
            if (exaistSocialCreditCode)
            {
                throw new Exception("社会信用代码已经存在");
            }
            uow.Biz_ExaminationPoint.Add(traininginstitution);
            uow.Commit();
            string defaultPassword = System.Web.Configuration.WebConfigurationManager.AppSettings["DefaultPassword"];

            IAccountCtrl accountCtrl = new AccountCtrl(account);
            Sys_Account newaccount = new Sys_Account()
            {
                AccountName = traininginstitution.SocialCreditCode,
                Password = defaultPassword,
                UserId = traininginstitution.Id
            };
            accountCtrl.AddAccount(newaccount);
            IRoleCtrl roleCtrl = new RoleCtrl(account);
            roleCtrl.Assigning2Role(newaccount.Id, "ExaminationPoint");
        }
        public void DeleteExaminationPointById(int trainingInstitutionId)
        {
            Biz_ExaminationPoint trainingInstitution = uow.Biz_ExaminationPoint.GetById(trainingInstitutionId);
            if (trainingInstitution.IsNull())
            {
                throw new Exception("人员信息不存在");
            }
            uow.Biz_ExaminationPoint.Delete(trainingInstitution);
            uow.Commit();
        }
        public void UpdateExaminationPoint(Biz_ExaminationPoint traininginstitution)
        {

            bool exaistSocialCreditCode = uow.Biz_ExaminationPoint.GetAll().Where(p => p.SocialCreditCode == traininginstitution.SocialCreditCode).Where(p => p.Id != traininginstitution.Id).Count() > 0 ? true : false;
            if (exaistSocialCreditCode)
            {
                throw new Exception("社会信用代码已经存在");
            }
            Biz_ExaminationPoint oldExaminationPoint = uow.Biz_ExaminationPoint.GetById(traininginstitution.Id);
            oldExaminationPoint.Id = traininginstitution.Id;
            oldExaminationPoint.InstitutionName = traininginstitution.InstitutionName;
            oldExaminationPoint.SocialCreditCode = traininginstitution.SocialCreditCode;
            oldExaminationPoint.LegalRepresentative = traininginstitution.LegalRepresentative;
            oldExaminationPoint.LegalRepresentativeNumber = traininginstitution.LegalRepresentativeNumber;
            oldExaminationPoint.ContactPerson = traininginstitution.ContactPerson;
            oldExaminationPoint.ContactNumber = traininginstitution.ContactNumber;
            oldExaminationPoint.City = traininginstitution.City;
            oldExaminationPoint.Area = traininginstitution.Area;
            oldExaminationPoint.Email = traininginstitution.Email;
            oldExaminationPoint.Address = traininginstitution.Address;
            uow.Biz_ExaminationPoint.Update(oldExaminationPoint);
            uow.Commit();
        }
        public List<Biz_ExaminationPoint> GetExaminationPointList(string InstitutionName, string SocialCreditCode, string City, string Area, int page, int rows, ref int totalCount, List<string> cityList)
        {
            IQueryable<Biz_ExaminationPoint> iQueryable = uow.Biz_ExaminationPoint.GetAll();
            #region //数据权限 本市看本市自己的考核点 管理员和省总站看所有
            if (account.RoleList.Where(p => p.RoleType == RoleCtrl.RoleType.Admin).Count() > 0)
            {

            }
            else if (account.RoleList.Where(p => p.RoleType == RoleCtrl.RoleType.Master).Count() > 0)
            {//不显示测试市 数据
                iQueryable = iQueryable.Where(p => p.City != "测试市");
            }
            else if (account.RoleList.Where(p => p.RoleType == RoleCtrl.RoleType.Manager).Count() > 0)
            {
                iQueryable = iQueryable.Where(p => cityList.Contains(p.City));
            }
            else
            {
                iQueryable = iQueryable.Where(p => false);
            }
            #endregion
            if (!InstitutionName.IsNull())
            {
                iQueryable = iQueryable.Where(p => p.InstitutionName.Contains(InstitutionName));
            }
            if (!SocialCreditCode.IsNull())
            {
                iQueryable = iQueryable.Where(p => p.SocialCreditCode == SocialCreditCode);
            }
            if (!City.IsNull())
            {
                iQueryable = iQueryable.Where(p => p.City == City);
            }
            if (!Area.IsNull())
            {
                iQueryable = iQueryable.Where(p => p.Area == Area);
            }

            totalCount = iQueryable.Count();
            int indexBegin = (page - 1) * rows;
            iQueryable = iQueryable.OrderBy(P => P.Id).Skip(indexBegin).Take(rows);

            List<Biz_ExaminationPoint> trainingInstitutionList = iQueryable.ToList();

            return trainingInstitutionList;
        }
        public List<Biz_ExaminationPoint> GetExaminationPointListByCityList(List<string> cityList)
        {
            List<Biz_ExaminationPoint> trainingInstitutionList = uow.Biz_ExaminationPoint.GetAll().Where(p => cityList.Contains(p.City)).OrderBy(p => p.Id).ToList();
            return trainingInstitutionList;
        }
        public List<Biz_ExaminationPoint> GetExaminationPointList()
        {
            List<Biz_ExaminationPoint> trainingInstitutionList = uow.Biz_ExaminationPoint.GetAll().ToList();
            return trainingInstitutionList;
        }
        public Biz_ExaminationPoint GetExaminationPointById(int examinationPointId)
        {
            Biz_ExaminationPoint trainingInstitution = uow.Biz_ExaminationPoint.GetById(examinationPointId);
            return trainingInstitution;
        }
        public List<Biz_ExaminationPoint> GetExaminationPointByIdList(List<int> examinationPointIdList)
        {
            List<Biz_ExaminationPoint> trainingInstitutionList = uow.Biz_ExaminationPoint.GetAll().Where(p => examinationPointIdList.Contains(p.Id)).ToList();
            return trainingInstitutionList;
        }


        public void InsertExaminationRoom(Biz_ExaminationRoom examinationRoom)
        {
            CheckExamRoom(examinationRoom);
            examinationRoom.CreatById = account.UserId;
            examinationRoom.CreatDate = DateTime.Now;
            examinationRoom.ExaminationPointId = account.UserId;
            uow.Biz_ExaminationRoom.Add(examinationRoom);
            uow.Commit();
        }

        public void DeleteExaminationRoomById(int examinationRoomId)
        {
            Biz_ExaminationRoom examinationRoom = uow.Biz_ExaminationRoom.GetById(examinationRoomId);
            if (examinationRoom.IsNull())
            {
                throw new Exception("人员信息不存在");
            }
            uow.Biz_ExaminationRoom.Delete(examinationRoom);
            uow.Commit();
        }

        public void UpdateExaminationRoom(Biz_ExaminationRoom examinationRoom)
        {
            CheckExamRoom(examinationRoom);
            Biz_ExaminationRoom oldExaminationRoom = uow.Biz_ExaminationRoom.GetById(examinationRoom.Id);
            oldExaminationRoom.Id = examinationRoom.Id;
            oldExaminationRoom.ExamRoomName = examinationRoom.ExamRoomName;
            oldExaminationRoom.PersonCount = examinationRoom.PersonCount;
            oldExaminationRoom.Enabled = examinationRoom.Enabled;
            uow.Commit();
        }
        private void CheckExamRoom(Biz_ExaminationRoom examinationRoom)
        {
            int repeateExamRoomCnt = uow.Biz_ExaminationRoom.GetAll().Where(p => p.ExamRoomName == examinationRoom.ExamRoomName && p.Id != examinationRoom.Id && p.ExaminationPointId == examinationRoom.ExaminationPointId).Count();
            if (repeateExamRoomCnt > 0)
            {
                throw new Exception("本考核点存在相同名称的考场");
            }
        }
        public List<Biz_ExaminationRoom> GetExaminationRoomList(int examinationPointId, string examinationRoomName, int page, int rows, ref int totalCount)
        {
            IQueryable<Biz_ExaminationRoom> iQueryable = uow.Biz_ExaminationRoom.GetAll().Where(p => p.ExaminationPointId == examinationPointId);
            if (!examinationRoomName.IsNull())
            {
                iQueryable = iQueryable.Where(p => p.ExamRoomName.Contains(examinationRoomName));
            }
            if (account.RoleList.Single().RoleType == "CheckPoint")//角色为考核点的  只能看到自己的考场
            {
                iQueryable = iQueryable.Where(p => p.ExaminationPointId == account.UserId);
            }
            totalCount = iQueryable.Count();
            int indexBegin = (page - 1) * rows;
            iQueryable = iQueryable.OrderBy(P => P.Id).Skip(indexBegin).Take(rows);
            List<Biz_ExaminationRoom> examinationRoomList = iQueryable.ToList();
            return examinationRoomList;
        }

        public Biz_ExaminationRoom GetExaminationRoomById(int examinationRoomId)
        {
            return uow.Biz_ExaminationRoom.GetById(examinationRoomId);
        }
        public List<Biz_ExaminationRoom> GetExaminationRoomByIdList(List<int> idList)
        {
            List<Biz_ExaminationRoom> examinationRoomLsit = uow.Biz_ExaminationRoom.GetAll().Where(p => idList.Contains(p.Id)).ToList();
            return examinationRoomLsit;
        }
        public List<Biz_ExaminationRoom> GetExaminationRoomListByExaminationPointId(int trainingInstitutionId, bool? enable)
        {
            IQueryable<Biz_ExaminationRoom> examRoomQueryable = uow.Biz_ExaminationRoom.GetAll().Where(p => p.ExaminationPointId == trainingInstitutionId);
            if (!enable.IsNull())
            {
                examRoomQueryable = examRoomQueryable.Where(p => p.Enabled == enable);
            }
            List<Biz_ExaminationRoom> examRoomList = examRoomQueryable.ToList();
            return examRoomList;
        }
        public Biz_ExamRoomNVR GetExamRoomNVRByExamRoomId(int examRoomId)
        {
            Biz_ExamRoomNVR examRoomNVR = uow.Biz_ExamRoomNVR.GetAll().Where(p => p.ExamRoomId == examRoomId).FirstOrDefault();
            return examRoomNVR;
        }
    }
}
