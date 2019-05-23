using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Library.baseFn;
using Model;
using DAL;
using System.Data;
using System.Security.Cryptography;
using System.IO;
using Library.LogFn;

namespace BLL
{
    public class SmrzServiceCtrl : ISmrzServiceCtrl
    {
        private Uow uow;
        private Sys_Account loginAccount;
        private string SMRZUserName = null;
        private string SMRZPassword = null;
        private bool ENABLE = true;//实名认证是否启用
        public SmrzServiceCtrl(Sys_Account account)
        {
            if (uow == null)
            {
                uow = new Uow();
            }
            loginAccount = account;
            SMRZUserName = AppFn.GetAppSettingsValue("SMRZUserName");
            SMRZPassword = AppFn.GetAppSettingsValue("SMRZPassword");
            ENABLE = AppFn.GetAppSettingsValue("SMRZService") == "启用" ? true : false;
        }

        public void newCertificateAuthentication(int employeeId)
        {
            if (!ENABLE)
            {
                return;
            }
            IQueryable<Biz_RelEmployeeCertificate> relEmployeeCertificate = uow.Biz_RelEmployeeCertificate.GetAll().Where(q => q.EmployeeId == employeeId);
            Biz_Certificate certificate = uow.Biz_Certificate.GetAll().Where(p => relEmployeeCertificate.Select(q => q.CertificateId).Contains(p.Id)).Single();
            Biz_Enterprise enterprise = uow.Biz_Enterprise.GetAll().Where(p => p.EnterpriseName == certificate.EnterpriseName).OrderByDescending(p => p.Id).First();
            Biz_EmployeeAuthentication employeeAuthentication = uow.Biz_EmployeeAuthentication.GetAll().Where(p => p.EmployeeId == employeeId).Single();

            Smrz.RyxxModel ryxxModel = new Smrz.RyxxModel()
            {
                XM = employeeAuthentication.PartyName,//姓名
                XB = employeeAuthentication.Gender,//性别
                MZ = employeeAuthentication.Nation,//民族
                CSRQ = employeeAuthentication.BornDay.Replace("年", "-").Replace("月", "-").Replace("日", "").ConvertToDateTime(),//出生日期
                JTDZ = employeeAuthentication.CertAddress,//家庭地址
                SFZH = employeeAuthentication.CertNumber,//身份证号
                FZJG = employeeAuthentication.CertOrg,//发证机关
                SFZYXKSRQ = employeeAuthentication.EffDate.Replace(".", "-").ConvertToDateTime(),//身份证有效开始日期
                SFZYXJZRQ = (employeeAuthentication.ExpDate.IsNull() || employeeAuthentication.ExpDate == "长期") ? employeeAuthentication.EffDate.Replace(".", "-").ConvertToDateTime().AddYears(100) : employeeAuthentication.EffDate.Replace(".", "-").ConvertToDateTime(),//身份证有效结束日期
                SBH = employeeAuthentication.SamId,//实名认证设备号
                ZP = employeeAuthentication.PictureBase64.IsNull() ? null : Convert.FromBase64String(employeeAuthentication.PictureBase64),//身份证照片
                SJHM = "",//手机号码
                QYMC = certificate.EnterpriseName,//企业名称
                ZZJGDM = enterprise.SocialCreditCode,//组织机构代码
                ZW = certificate.Job,//职务
                ZC = certificate.Title4Technical,//职称
                ZSLX = certificate.ExamType,//证书类型
                ZSBH = certificate.CertificateNo,//证书编号
                ZSBM = certificate.CertificateNo,//证书编码
                ZSMC = "安全生产考核合格证书",//证书名称
                FZRQ = certificate.StartCertificateDate,//发证日期
                ZSYXJZRQ = certificate.EndCertificateDate,//证书有效截止日期
                STATE = "正常",//证书状态
                ZSSCSJ = DateTime.Now//证书生成时间
            };

            try
            {
                Smrz.SmrzService smrzService = new Smrz.SmrzService();
                string result = smrzService.AddOrUpdateRyksl(ryxxModel, SMRZUserName, SMRZPassword);
                if (result.ToLower() != "true")
                {
                    throw new Exception(result);
                }
                else
                {
                    #region 记录成功传输的证书

                    this.AddCertificateAuthenticationLog(ryxxModel, "newCertificateAuthentication" + result);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                #region 记录传输出错的证书
                this.AddCertificateAuthenticationLog(ryxxModel, "newCertificateAuthentication" + ex.Message);
                #endregion
                string errorMessage = string.Format("证书实名认证失败：身份证号：{0} 证书编号：{1} 错误信息{2}", ryxxModel.SFZH, ryxxModel.ZSBH, ex.Message);
                throw new Exception(errorMessage, ex);
            }
        }

        private void AddCertificateAuthenticationLog(Smrz.RyxxModel ryxxModel, string result)
        {
            Biz_CertificateAuthenticationLog certificateAuthenticationLog = new Biz_CertificateAuthenticationLog()
            {
                XM = ryxxModel.XM,
                XB = ryxxModel.XB,
                MZ = ryxxModel.MZ,
                CSRQ = ryxxModel.CSRQ,
                JTDZ = ryxxModel.JTDZ,
                SFZH = ryxxModel.SFZH,
                FZJG = ryxxModel.FZJG,
                SFZYXKSRQ = ryxxModel.SFZYXKSRQ,
                SFZYXJZRQ = ryxxModel.SFZYXJZRQ,
                SBH = ryxxModel.SBH,
                ZP = ryxxModel.ZP,
                SJHM = ryxxModel.SJHM,
                QYMC = ryxxModel.QYMC,
                ZZJGDM = ryxxModel.ZZJGDM,
                ZW = ryxxModel.ZW,
                ZC = ryxxModel.ZC,
                ZSLX = ryxxModel.ZSLX,
                ZSBH = ryxxModel.ZSBH,
                ZSBM = ryxxModel.ZSBM,
                FZRQ = ryxxModel.FZRQ,
                ZSYXJZRQ = ryxxModel.ZSYXJZRQ,
                STATE = ryxxModel.STATE,
                Result = result,
                CreateDate = DateTime.Now
            };
            uow.Biz_CertificateAuthenticationLog.Add(certificateAuthenticationLog);
            uow.Commit();
        }

        public void DelayCertificateAuthentication(int certificateId)
        {
            if (!ENABLE)
            {
                return;
            }
            Biz_Certificate certificate = uow.Biz_Certificate.GetById(certificateId);
            Biz_Enterprise enterprise = uow.Biz_Enterprise.GetAll().Where(p => p.EnterpriseName == certificate.EnterpriseName).OrderByDescending(p => p.Id).First();
            IQueryable<Biz_RelCertificateAuthentication> relCertificateAuthenticationQueryable = uow.Biz_RelCertificateAuthentication.GetAll().Where(q => q.CertificateId == certificateId);
            Biz_CertificateAuthentication certificateDelayAuthentication = uow.Biz_CertificateAuthentication.GetAll().Where(p => relCertificateAuthenticationQueryable.Select(q => q.CertificateAuthenticationId).Contains(p.Id)).Single();

            Smrz.RyxxModel ryxxModel = new Smrz.RyxxModel()
                  {
                      XM = certificateDelayAuthentication.PartyName,//姓名
                      XB = certificateDelayAuthentication.Gender,//性别
                      MZ = certificateDelayAuthentication.Nation,//民族
                      CSRQ = certificateDelayAuthentication.BornDay.Replace("年", "-").Replace("月", "-").Replace("日", "-").ConvertToDateTime(),//出生日期
                      JTDZ = certificateDelayAuthentication.CertAddress,//家庭地址
                      SFZH = certificateDelayAuthentication.CertNumber,//身份证号
                      FZJG = certificateDelayAuthentication.CertOrg,//发证机关
                      SFZYXKSRQ = certificateDelayAuthentication.EffDate.Replace(".", "-").ConvertToDateTime(),//身份证有效开始日期
                      SFZYXJZRQ = (certificateDelayAuthentication.ExpDate.IsNull() || certificateDelayAuthentication.ExpDate == "长期") ? certificateDelayAuthentication.EffDate.Replace(".", "-").ConvertToDateTime().AddYears(100) : certificateDelayAuthentication.EffDate.Replace(".", "-").ConvertToDateTime(),//身份证有效结束日期
                      SBH = certificateDelayAuthentication.SamId,//实名认证设备号
                      ZP = certificateDelayAuthentication.PictureBase64.IsNull() ? null : Convert.FromBase64String(certificateDelayAuthentication.PictureBase64),//身份证照片
                      SJHM = "",//手机号码
                      QYMC = certificate.EnterpriseName,//企业名称
                      ZZJGDM = enterprise.SocialCreditCode,//组织机构代码
                      ZW = certificate.Job,//职务
                      ZC = certificate.Title4Technical,//职称
                      ZSLX = certificate.ExamType,//证书类型
                      ZSBH = certificate.CertificateNo,//证书编号
                      ZSBM = certificate.CertificateNo,//证书编码
                      ZSMC = "安全生产考核合格证书",//证书名称
                      FZRQ = certificate.StartCertificateDate,//发证日期
                      ZSYXJZRQ = certificate.EndCertificateDate,//证书有效截止日期
                      STATE = "正常",//证书状态
                      ZSSCSJ = DateTime.Now//证书生成时间
                  };

            try
            {
                Smrz.SmrzService smrzService = new Smrz.SmrzService();
                string result = smrzService.AddOrUpdateRyksl(ryxxModel, SMRZUserName, SMRZPassword);
                if (result.ToLower() != "true")
                {
                    throw new Exception(result);
                }
                this.AddCertificateAuthenticationLog(ryxxModel, "DelayCertificateAuthentication_" + result);
            }
            catch (Exception ex)
            {
                this.AddCertificateAuthenticationLog(ryxxModel, "DelayCertificateAuthentication_" + ex.Message);
                string errorMessage = string.Format("延期证书实名认证失败：身份证号：{0} 证书编号：{1} 错误信息{2}", ryxxModel.SFZH, ryxxModel.ZSBH, ex.Message);
                throw new Exception(errorMessage, ex);
            }
        }

        public void ChangeCertificateStatus(int certificateId, string status)
        {
            if (!ENABLE)
            {
                return;
            }
            List<string> statusList = new List<string>() { "正常", "注销" };
            if (!statusList.Contains(status))
            {
                throw new Exception("不合法的证书状态");
            }

            Smrz.RyxxModel ryxxModel = new Smrz.RyxxModel();
            //实名认证信息 
            {
                //先在证书实名认证表中找  未找到 就在初次取证人员实名认证表中找(证书延期 和 证书企业变更 的 实名信息 都存在 证书实名认证表 中)
                IQueryable<Biz_RelCertificateAuthentication> certificateAuthenticationQueryable = uow.Biz_RelCertificateAuthentication.GetAll().Where(q => q.CertificateId == certificateId);
                Biz_CertificateAuthentication certificateAuthentication = uow.Biz_CertificateAuthentication.GetAll().Where(p => certificateAuthenticationQueryable.Select(q => q.CertificateAuthenticationId).Contains(p.Id)).OrderByDescending(p => p.Id).FirstOrDefault();
                if (certificateAuthentication != null)
                {
                    ryxxModel.XM = certificateAuthentication.PartyName;//姓名
                    ryxxModel.XB = certificateAuthentication.Gender;//性别
                    ryxxModel.MZ = certificateAuthentication.Nation;//民族
                    ryxxModel.CSRQ = certificateAuthentication.BornDay.Replace("年", "-").Replace("月", "-").Replace("日", "-").ConvertToDateTime();//出生日期
                    ryxxModel.JTDZ = certificateAuthentication.CertAddress;//家庭地址
                    ryxxModel.SFZH = certificateAuthentication.CertNumber;//身份证号
                    ryxxModel.FZJG = certificateAuthentication.CertOrg;//发证机关
                    ryxxModel.SFZYXKSRQ = certificateAuthentication.EffDate.Replace(".", "-").ConvertToDateTime();//身份证有效开始日期
                    ryxxModel.SFZYXJZRQ = (certificateAuthentication.ExpDate.IsNull() || certificateAuthentication.ExpDate == "长期") ? certificateAuthentication.EffDate.Replace(".", "-").ConvertToDateTime().AddYears(100) : certificateAuthentication.EffDate.Replace(".", "-").ConvertToDateTime();//身份证有效结束日期
                    ryxxModel.SBH = certificateAuthentication.SamId;//实名认证设备号
                    ryxxModel.ZP = certificateAuthentication.PictureBase64.IsNull() ? null : Convert.FromBase64String(certificateAuthentication.PictureBase64);//身份证照片
                }
                else
                {
                    //初次取证实名认证信息
                    IQueryable<Biz_RelEmployeeCertificate> relEmployeeCertificateQueryable = uow.Biz_RelEmployeeCertificate.GetAll().Where(q => q.CertificateId == certificateId);
                    Biz_EmployeeAuthentication employeeAuthentication = uow.Biz_EmployeeAuthentication.GetAll().Where(p => relEmployeeCertificateQueryable.Select(q => q.EmployeeId).Contains(p.EmployeeId)).OrderByDescending(p => p.Id).FirstOrDefault();
                    if (employeeAuthentication != null)//人员初次领证实名认证数据不为空
                    {
                        ryxxModel.XM = employeeAuthentication.PartyName;//姓名
                        ryxxModel.XB = employeeAuthentication.Gender;//性别
                        ryxxModel.MZ = employeeAuthentication.Nation;//民族
                        ryxxModel.CSRQ = employeeAuthentication.BornDay.Replace("年", "-").Replace("月", "-").Replace("日", "-").ConvertToDateTime();//出生日期
                        ryxxModel.JTDZ = employeeAuthentication.CertAddress;//家庭地址
                        ryxxModel.SFZH = employeeAuthentication.CertNumber;//身份证号
                        ryxxModel.FZJG = employeeAuthentication.CertOrg;//发证机关
                        ryxxModel.SFZYXKSRQ = employeeAuthentication.EffDate.Replace(".", "-").ConvertToDateTime();//身份证有效开始日期
                        ryxxModel.SFZYXJZRQ = (employeeAuthentication.ExpDate.IsNull() || employeeAuthentication.ExpDate == "长期") ? employeeAuthentication.EffDate.Replace(".", "-").ConvertToDateTime().AddYears(100) : employeeAuthentication.EffDate.Replace(".", "-").ConvertToDateTime();//身份证有效结束日期
                        ryxxModel.SBH = employeeAuthentication.SamId;//实名认证设备号
                        ryxxModel.ZP = employeeAuthentication.PictureBase64.IsNull() ? null : Convert.FromBase64String(employeeAuthentication.PictureBase64);//身份证照片
                    }
                    else//人员初次领证实名认证数据为空 则将证书信息填入
                    {
                        Biz_Employee employee = uow.Biz_Employee.GetAll().Where(p => relEmployeeCertificateQueryable.Select(q => q.EmployeeId).Contains(p.Id)).OrderByDescending(p => p.Id).FirstOrDefault();
                        if (employee != null)
                        {
                            ryxxModel.XM = employee.EmployeeName;//姓名
                            ryxxModel.XB = employee.Sex;//性别
                            ryxxModel.MZ = "";//民族
                            ryxxModel.CSRQ = employee.Birthday;//出生日期
                            ryxxModel.JTDZ = "";//家庭地址
                            ryxxModel.SFZH = employee.IDNumber;//身份证号
                            ryxxModel.FZJG = "";//发证机关
                            ryxxModel.SFZYXKSRQ = DateTime.Now;//身份证有效开始日期
                            ryxxModel.SFZYXJZRQ = DateTime.Now;//身份证有效结束日期
                            ryxxModel.SBH = "";//实名认证设备号
                            ryxxModel.ZP = null;//身份证照片
                        }
                        else
                        {
                            throw new Exception("未找到实名认证信息");
                        }
                    }
                }
            }
            //证书信息
            {
                Biz_Certificate certificate = uow.Biz_Certificate.GetById(certificateId);
                ryxxModel.SJHM = "";//手机号码
                ryxxModel.QYMC = certificate.EnterpriseName;//企业名称
                ryxxModel.ZW = certificate.Job;//职务
                ryxxModel.ZC = certificate.Title4Technical;//职称
                ryxxModel.ZSLX = certificate.ExamType;//证书类型
                ryxxModel.ZSBH = certificate.CertificateNo;//证书编号
                ryxxModel.ZSBM = certificate.CertificateNo;//证书编码
                ryxxModel.ZSMC = "安全生产考核合格证书";//证书名称
                ryxxModel.FZRQ = certificate.StartCertificateDate;//发证日期
                ryxxModel.ZSYXJZRQ = certificate.EndCertificateDate;//证书有效截止日期
                ryxxModel.ZSSCSJ = DateTime.Now;//证书生成时间
            }
            //企业信息
            {
                Biz_Enterprise enterprise = uow.Biz_Enterprise.GetAll().Where(p => p.EnterpriseName == ryxxModel.QYMC).OrderByDescending(p => p.Id).First();
                ryxxModel.ZZJGDM = enterprise.SocialCreditCode;//组织机构代码
            }
            //证书状态
            {
                ryxxModel.STATE = status;//证书状态
            }

            try
            {
                Smrz.SmrzService smrzService = new Smrz.SmrzService();
                string result = smrzService.AddOrUpdateRyksl(ryxxModel, SMRZUserName, SMRZPassword);
                if (result.ToLower() != "true")
                {
                    throw new Exception(result);
                }
                this.AddCertificateAuthenticationLog(ryxxModel, "ChangeCertificateStatus_" + result);
            }
            catch (Exception ex)
            {
                this.AddCertificateAuthenticationLog(ryxxModel, "ChangeCertificateStatus_" + ex.Message);
                string errorMessage = string.Format("证书状态变更实名失败：身份证号：{0} 证书编号：{1} 错误信息{2}", ryxxModel.SFZH, ryxxModel.ZSBH, ex.Message);
                throw new Exception(errorMessage, ex);
            }
        }

        public string KeyLogin(string keyCode, string socialCreditCode)
        {
            string result = "true";
            try
            {
                Smrz.SmrzService smrzService = new Smrz.SmrzService();
                result = smrzService.GetKeyLogin(keyCode, socialCreditCode, SMRZUserName, SMRZPassword);
            }
            catch (Exception ex)
            {
                result = string.Format("加密锁验证失败：{0}", ex.Message);
            }
            return result;
        }
        public class CerificateInfo
        {
            /// <summary>
            /// 姓名
            /// </summary>
            public string name { get; set; }
            /// <summary>
            /// 性别
            /// </summary>
            public string sex { get; set; }
            /// <summary>
            /// 生日
            /// </summary>
            public string born { get; set; }
            /// <summary>
            /// 证书编号
            /// </summary>
            public string cardno { get; set; }
            /// <summary>
            /// 企业名称
            /// </summary>
            public string qymc { get; set; }
            /// <summary>
            /// 职务
            /// </summary>
            public string zhiwu { get; set; }
            /// <summary>
            /// 职称
            /// </summary>
            public string zhicheng { get; set; }
            /// <summary>
            /// 证书编号
            /// </summary>
            public string zsbh { get; set; }
            /// <summary>
            /// 有效期开始时间
            /// </summary>
            public DateTime yxqkssj { get; set; }
            /// <summary>
            /// 有效期结束时间
            /// </summary>
            public DateTime yxqjssj { get; set; }

        }
        public List<CerificateInfo> GetCertificateListByIDNumber(string iDNumber)
        {
            List<CerificateInfo> resultList = new List<CerificateInfo>();
            try
            {
                Smrz.SmrzService smrzService = new Smrz.SmrzService();
                DataSet ds = smrzService.GetZSByZSBH(iDNumber, SMRZUserName, SMRZPassword);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count != 0)//找到以后存下来
                {
                    int rowsCount = ds.Tables[0].Rows.Count;
                    for (int i = 0; i < rowsCount; i++)
                    {
                        DataRow dataRow = ds.Tables[0].Rows[0];
                        CerificateInfo result = new CerificateInfo()
                        {
                            name = dataRow["name"].ConvertToString(),
                            sex = dataRow["sex"].ConvertToString(),
                            born = dataRow["born"].ConvertToString(),
                            cardno = dataRow["cardno"].ConvertToString(),
                            qymc = dataRow["qymc"].ConvertToString(),
                            zhiwu = dataRow["zhiwu"].ConvertToString(),
                            zhicheng = dataRow["zhicheng"].ConvertToString(),
                            zsbh = dataRow["zsbh"].ConvertToString(),
                            yxqkssj = dataRow["yxqkssj"].ConvertToDateTime(),
                            yxqjssj = dataRow["yxqjssj"].ConvertToDateTime()
                        };
                        resultList.Add(result);
                    }

                }
            }
            catch (Exception ex)
            {
                string strMsg = string.Format("从建委信息中心获取证书失败：{0}", ex.Message);
                throw new Exception(strMsg, ex);
            }
            return resultList;
        }

        public CerificateInfo GetCertificateByCertificateNo(string certificateNo)
        {
            CerificateInfo result = null;
            try
            {
                Smrz.SmrzService smrzService = new Smrz.SmrzService();
                DataSet ds = smrzService.GetZSByZSBH(certificateNo, SMRZUserName, SMRZPassword);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count != 0)//找到以后存下来
                {
                    DataRow dataRow = ds.Tables[0].Rows[0];
                    result = new CerificateInfo()
                    {
                        name = dataRow["name"].ConvertToString(),
                        sex = dataRow["sex"].ConvertToString(),
                        born = dataRow["born"].ConvertToString(),
                        cardno = dataRow["cardno"].ConvertToString(),
                        qymc = dataRow["qymc"].ConvertToString(),
                        zhiwu = dataRow["zhiwu"].ConvertToString(),
                        zhicheng = dataRow["zhicheng"].ConvertToString(),
                        zsbh = dataRow["zsbh"].ConvertToString(),
                        yxqkssj = dataRow["yxqkssj"].ConvertToDateTime(),
                        yxqjssj = dataRow["yxqjssj"].ConvertToDateTime()
                    };
                }

                if (result.IsNull())
                {
                    throw new Exception("此证书不存在");
                }
                if (result.yxqjssj < DateTime.Now)
                {
                    throw new Exception("证书超过有效期");
                }
            }
            catch (Exception ex)
            {
                string strMsg = string.Format("从建委信息中心获取证书失败：{0}", ex.Message);
                new Exception(strMsg, ex);
            }
            return result;
        }

        #region 获取企业信息
        public class AGRYEnterpriseInfo
        {
            /// <summary>
            /// 组织机构代码
            /// </summary>
            public string zzjgdm3 { get; set; }
            /// <summary>
            /// 企业名称
            /// </summary>
            public string qymc { get; set; }
            /// <summary>
            /// 0表示注销 1表示正常
            /// </summary>
            public string zhuxiao { get; set; }

        }
        public AGRYEnterpriseInfo GetAGRYEnterpriseInfo(string keyCode)
        {
            AGRYEnterpriseInfo agryEnterprise = new AGRYEnterpriseInfo();
            try
            {
                Smrz.SmrzService webService = new Smrz.SmrzService();
                string encryptKey = EncryptDog(keyCode);
                DataSet enterpriseDS = webService.GetKeyLoginStr(encryptKey, SMRZUserName, SMRZPassword);

                if (enterpriseDS.Tables.Count < 1 || enterpriseDS.Tables[0].Rows.Count < 1)
                {
                    agryEnterprise = null;
                }
                else
                {
                    agryEnterprise.zhuxiao = enterpriseDS.Tables[0].Rows[0]["zhuxiao"].ToString();
                    agryEnterprise.zzjgdm3 = enterpriseDS.Tables[0].Rows[0]["zzjgdm3"].ToString();
                    agryEnterprise.qymc = enterpriseDS.Tables[0].Rows[0]["qymc"].ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("从建委信息中心获取企业信息出错：{0}", ex.Message), ex);
            }
            return agryEnterprise;
        }

        #region 加密锁 加密
        public string EncryptDog(string PassWord)
        {
            string DogWordKey = "cdgsabcd";
            return this.Encrypt(PassWord, DogWordKey);
        }
        private string Encrypt(string pToEncrypt, string sKey)
        {
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            byte[] bytes = Encoding.Default.GetBytes(pToEncrypt);
            provider.Key = Encoding.ASCII.GetBytes(sKey);
            provider.IV = Encoding.ASCII.GetBytes(sKey);
            MemoryStream stream = new MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, provider.CreateEncryptor(), CryptoStreamMode.Write);
            stream2.Write(bytes, 0, bytes.Length);
            stream2.FlushFinalBlock();
            StringBuilder builder = new StringBuilder();
            foreach (byte num in stream.ToArray())
            {
                builder.AppendFormat("{0:X2}", num);
            }
            builder.ToString();
            return builder.ToString();
        }
        #endregion
        #endregion
    }
}
