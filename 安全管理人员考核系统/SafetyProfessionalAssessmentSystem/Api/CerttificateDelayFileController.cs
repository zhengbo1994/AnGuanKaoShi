using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Library.baseFn;

namespace SafetyProfessionalAssessmentSystem.Controllers
{
    public class CerttificateDelayFileController : BaseController
    {
        //
        // GET: /证书延期文件/

        public class SaveCerttificateDelayFileParam
        {
            public int certificateId { get; set; }
            //文件类型
            public string fileType { get; set; }
            //文件名称
            public string fileName { get; set; }
            //文件相对路径
            public string filePath { get; set; }
            public string fileBase64 { get; set; }
        }
        [HttpPost]
        public JsonResult SaveCerttificateDelayFile(string strParam)
        {


            FileStream fs = null;
            MemoryStream ms = null;
            try
            {
                SaveCerttificateDelayFileParam param = strParam.JSONStringToObj<SaveCerttificateDelayFileParam>();
                Sys_Account account = base.LoginAccount as Sys_Account;
                ICertificateCtrl certificateCtrl = new CertificateCtrl(account);
                Biz_CertificateDelayFile certificateDelayFile = certificateCtrl.GetCertificateDelayPhoto(param.certificateId);
                if (certificateDelayFile != null)
                {
                    throw new Exception("登记照已存在");
                }

                byte[] arrFileData = Convert.FromBase64String(param.fileBase64);
                ms = new MemoryStream(arrFileData);

                string rootFolder = base.getFileRootDirectory();

                string absolutePath = string.Format(@"{0}\{1}", rootFolder.TrimEnd('\\'), param.filePath.TrimStart('\\'));//文件绝对路径
                FileInfo file = new FileInfo(absolutePath);
                if (!file.Directory.Exists)
                {
                    file.Directory.Create();
                }
                else if (file.Exists)
                {
                    file.Attributes = FileAttributes.Normal;
                    file.Delete();
                }
                fs = new FileStream(absolutePath, FileMode.CreateNew);
                ms.CopyToStream(fs);
                fs.Flush();

                //保存数据记录
                certificateCtrl.SaveCertificateDelayFile(param.certificateId,param.fileType,param.fileName,param.filePath);

                ResultMessage result = new ResultMessage() { IsSuccess = true };
                return Json(result);
            }
            catch (Exception ex)
            {
                ResultMessage result = new ResultMessage() { IsSuccess = false, ErrorMessage = ex.Message };
                return Json(result);
            }
            finally
            {
                if (null != fs) fs.Close();
                if (null != ms) ms.Close();
            }
        }

    }
}
