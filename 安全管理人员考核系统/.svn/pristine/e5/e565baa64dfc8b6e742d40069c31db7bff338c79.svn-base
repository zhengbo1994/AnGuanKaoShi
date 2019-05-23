using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL;
using Model;
using Library.baseFn;

namespace SafetyProfessionalAssessmentSystem.Controllers
{
    public class PersonalAttachmentsController : BaseController
    {
        //
        // GET: /PersonalAttachments/

        public ActionResult Index()
        {
            return View();
        }

        #region 保存用户考试图片（进场、考前、考中（切换下一张试卷之前）、考后）
        public class SaveEmployeeExamPhotoPara
        {
            public string IdNumber { get; set; }
            public int? ExamId { get; set; }
            public int? PaperId { get; set; }
            public string FileType { get; set; }
            public string FileInBase64 { get; set; }
        }
        private string[] ExamPhotoTypes = { "进场照片", "考前照片", "考中照片", "考后照片" };
        public JsonResult SaveEmployeeExamPhoto(string userData)
        {
            ResultMessage result = new ResultMessage();
            FileStream fs = null;
            MemoryStream ms = null;
            try
            {
                SaveEmployeeExamPhotoPara para = userData.JSONStringToObj<SaveEmployeeExamPhotoPara>();
                if (!ExamPhotoTypes.Contains(para.FileType))
                {
                    throw new Exception("FileType设置错误");
                }

                if (string.IsNullOrEmpty(para.FileInBase64))
                {
                    throw new Exception("没有上传文件");
                }
                Sys_Account account = base.LoginAccount as Sys_Account;
                IWorkFlowCtrl wf = new WorkFlowCtrl(account);

                Biz_Employee employee = wf.GetEmployee((int)para.ExamId, para.IdNumber);

                //6位随机数
                Random rnd = new Random(DateTime.Now.Millisecond);
                string strRnd = "000000" + rnd.Next(1000000);
                strRnd = strRnd.Substring(strRnd.Length - 6);

                string rootFolder = getFileRootDirectory();
                string fileName = string.Format("{0}-{1}-{2}-{3}-{4}-{5}.png"
                    , para.FileType
                    , employee.Id
                    , para.ExamId
                    , para.PaperId
                    , DateTime.Now.ToString("yyyyMMdd")
                    , strRnd);//文件名

                string relativePath = string.Format(@"\{0}\{1}\{2}"
                    , DateTime.Now.ToString("yyyyMMdd")
                    , employee.Id
                    , fileName);//数据库相对路径
                string absolutePath = string.Format(@"{0}\{1}"
                    , rootFolder.TrimEnd('\\')
                    , relativePath.TrimStart('\\'));//文件绝对路径

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

                byte[] arrFileData = Convert.FromBase64String(para.FileInBase64);
                ms = new MemoryStream(arrFileData);
                fs = new FileStream(absolutePath, FileMode.CreateNew);
                ms.CopyToStream(fs);
                fs.Flush();

                IEmployeeFileCtrl eFile = new EmployeeFileCtrl();

                List<Biz_EmployeeFile> employeeFiles = new List<Biz_EmployeeFile>();
                Biz_EmployeeFile employeeFile = new Biz_EmployeeFile()
                {
                    EmployeeId = employee.Id,
                    FileType = para.FileType,
                    FileName = fileName,
                    FilePath = relativePath
                };
                employeeFiles.Add(employeeFile);

                eFile.SaveEmployeeFile(employeeFiles);

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                if (null != fs) fs.Close();
                if (null != ms) ms.Close();
            }
            return Json(result);
        }
        #endregion
    }
}
