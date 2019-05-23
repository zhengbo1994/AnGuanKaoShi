using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Library.baseFn;
using System.IO;

namespace SafetyProfessionalAssessmentSystem.Controllers
{
    [AuthorizeFilter]
    public class ManageExamresultController : BaseController
    {
        //
        // GET: /管理考试结果/

        public ActionResult Index()
        {
            return View();
        }
        #region 获取考试结果列表
        public JsonResult GetEmployeeExamResultListForJqgrid(GetEmployeeExamResultParam param)
        {
            int totalCount = 0;
            List<GetEmployeeExamResultItem> GetEmployeeExamResultItemList = new List<GetEmployeeExamResultItem>();
            try
            {
                Sys_Account account = LoginAccount as Sys_Account;
                IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
                IEmployeeCtrl employeeCtrl = new EmployeeCtrl(account);
                //考试结果人员 主
                List<Biz_Employee> employeeList = workFlowCtrl.EmployeeForExamResultList(param.ExamPlanNumber, param.EmployeeName, param.IDNumber, param.Industry, param.ExamType, param.SubmitStatus, param.page, param.rows, ref totalCount);
                List<int> employeeIdList = employeeList.Select(p => p.Id).ToList();

                //人员考试结果
                List<Biz_EmployeeExamResultRecord> employeeExamResultRecordList = employeeCtrl.GetEmployeeExamResultRecordListByEmployeeId(employeeIdList);
                //考试安排记录
                List<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordList = workFlowCtrl.GetEmployeeForExamPlanRecordByEmployeeIdList(employeeIdList);
                //考试计划记录
                List<Biz_ExamPlanRecord> examPlanRecord = workFlowCtrl.GetExamPlanRecordByIdList(employeeForExamPlanRecordList.Select(p => p.ExamPlanRecordId).ToList());

                GetEmployeeExamResultItemList = employeeList.Join(employeeForExamPlanRecordList, a => a.Id, b => b.EmployeeId, (a, b) => new { a, b })
                .Join(examPlanRecord, o => o.b.ExamPlanRecordId, c => c.Id, (o, c) => new { o.a, o.b, c })
                .GroupJoin(employeeExamResultRecordList, o => o.a.Id, d => d.EmployeeId, (o, d) => new { o.a, o.b, o.c, d = d.Count() > 0 ? d.FirstOrDefault() : null })
                .Select(o => new GetEmployeeExamResultItem()
                {
                    ExamPlanNumber = o.c.ExamPlanNumber,
                    ExamPlanId = o.c.Id,
                    EmployeeId = o.a.Id,
                    EmployeeName = o.a.EmployeeName,
                    Sex = o.a.Sex,
                    Age = DateTime.Now.Year - o.a.Birthday.Year,
                    IDNumber = o.a.IDNumber,
                    Industry = o.a.Industry,
                    ExamType = o.a.ExamType,
                    SafetyKnowledgeExamResult = o.d == null ? "" : o.d.SafetyKnowledgeExamResult,
                    SafetyKnowledgeExamScore = (o.d == null || o.d.SafetyKnowledgeExamScore == null) ? "" : o.d.SafetyKnowledgeExamScore.ToString(),
                    ManagementAbilityExamResult = (o.d == null) ? "" : o.d.ManagementAbilityExamResult,
                    ManagementAbilityExamScore = (o.d == null || o.d.ManagementAbilityExamScore == null) ? "" : o.d.ManagementAbilityExamScore.ToString(),
                    FieldExamResult = o.d == null ? "" : o.d.ActualOperationExamResult,
                    FinalExamResult = o.d == null ? "" : o.d.FinalExamResult,
                    SubmitStatus = o.d == null ? "" : o.d.SummitStatus ? "已提交" : "未提交"
                }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            JqGridData dataResult = ConvertIListToJqGridData(param.page, param.rows, totalCount, GetEmployeeExamResultItemList);
            return Json(dataResult, JsonRequestBehavior.AllowGet);
        }
        public class GetEmployeeExamResultParam
        {
            public string ExamPlanNumber { get; set; }
            public string EmployeeName { get; set; }
            public string IDNumber { get; set; }
            public string Industry { get; set; }
            public string ExamType { get; set; }
            public string SubmitStatus { get; set; }
            public int page { get; set; }
            public int rows { get; set; }
        }

        public class GetEmployeeExamResultItem
        {
            public int ExamPlanId { get; set; }
            public string ExamPlanNumber { get; set; }
            public int EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public string Sex { get; set; }
            public int Age { get; set; }
            public string IDNumber { get; set; }
            public string Industry { get; set; }
            public string ExamType { get; set; }
            public string SafetyKnowledgeExamResult { get; set; }
            public string SafetyKnowledgeExamScore { get; set; }
            public string ManagementAbilityExamResult { get; set; }
            public string ManagementAbilityExamScore { get; set; }
            public string FieldExamResult { get; set; }
            public string FinalExamResult { get; set; }
            public string SubmitStatus { get; set; }
        }
        #endregion

        #region 获取科目列表
        public JsonResult GetEmployeeExamTypeList()
        {
            Sys_Account account = LoginAccount as Sys_Account;
            IWorkFlowCtrl employeeCtrl = new WorkFlowCtrl(account);
            List<GetEmployeeExamTypeListResult> employeeExamTypeList = employeeCtrl.GetEmployeeSubjectList().Select(p => new GetEmployeeExamTypeListResult()
            {
                ItemText = p.ItemText,
                ItemValue = p.ItemValue
            }).ToList();
            return Json(employeeExamTypeList, JsonRequestBehavior.AllowGet);
        }
        public class GetEmployeeExamTypeListResult
        {
            public string ItemText { get; set; }
            public string ItemValue { get; set; }
        }
        #endregion

        #region 获取行业列表
        public JsonResult GetEmployeeIndustryList()
        {
            Sys_Account account = LoginAccount as Sys_Account;
            IWorkFlowCtrl employeeCtrl = new WorkFlowCtrl(account);
            List<GetEmployeeIndustryListResult> employeeIndustryList = employeeCtrl.GetEmployeeIndustryList().Select(p => new GetEmployeeIndustryListResult()
            {
                ItemText = p.ItemText,
                ItemValue = p.ItemValue
            }).ToList();
            return Json(employeeIndustryList, JsonRequestBehavior.AllowGet);
        }
        public class GetEmployeeIndustryListResult
        {
            public string ItemText { get; set; }
            public string ItemValue { get; set; }
        }
        #endregion

        #region 获取考试结果
        public JsonResult GetExamResult(int employeeId)
        {
            GetExamResultItem getExamResultItem = new GetExamResultItem();
            try
            {
                Sys_Account account = LoginAccount as Sys_Account;
                IWorkFlowCtrl workFlow = new WorkFlowCtrl(account);
                IEmployeeCtrl employeeCtrl = new EmployeeCtrl(account);
                List<int> employeeIdList = new List<int> { employeeId };

                Biz_EmployeeExamResultRecord employeeExamResultRecord = employeeCtrl.GetEmployeeExamResultRecordListByEmployeeId(employeeIdList).FirstOrDefault();
                if (employeeExamResultRecord != null)
                {
                    getExamResultItem.EmployeeId = employeeExamResultRecord.EmployeeId;
                    getExamResultItem.ExamPlanRecordId = employeeExamResultRecord.ExamPlanRecordId;
                    getExamResultItem.SafetyKnowledgeExamResult = employeeExamResultRecord.SafetyKnowledgeExamResult;
                    getExamResultItem.ManagementAbilityExamResult = employeeExamResultRecord.ManagementAbilityExamResult;
                    getExamResultItem.FieldExamResult = employeeExamResultRecord.FieldExamResult;
                    getExamResultItem.ImgFileList = workFlow.GetEmployeeExamResultRecordFileListByEmployeeExamResultRecordId(employeeExamResultRecord.Id).Select(p => new ImgFile()
                    {
                        Id = p.Id,
                        fileKey = p.FileKey
                    }).ToList();
                }
            }
            catch (Exception ex)
            {

            }
            return Json(getExamResultItem, JsonRequestBehavior.AllowGet);
        }
        public class GetExamResultItem
        {
            public int EmployeeId { get; set; }
            public int ExamPlanRecordId { get; set; }
            //安全知识考试结果
            public string SafetyKnowledgeExamResult { get; set; }

            //管理能力考试结果
            public string ManagementAbilityExamResult { get; set; }
            //实操考试结果
            public bool? FieldExamResult { get; set; }
            public List<ImgFile> ImgFileList { get; set; }

        }
        public class ImgFile
        {
            public string fileKey { get; set; }
            public int Id { get; set; }
        }
        #endregion

        #region 获取考试结果图片文件
        public FileResult GetExamResultFile(string imgId)
        {
            int employeeExamResultImgFileId = imgId.ConvertToInt32();
            Sys_Account account = LoginAccount as Sys_Account;
            IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
            Biz_EmployeeExamResultRecordFile employeeExamResultRecordFile = workFlowCtrl.GetEmployeeExamResultImgFileById(employeeExamResultImgFileId);
            string FilePath = GetAbsolutePath(employeeExamResultRecordFile.FilePath);
            return File(FilePath, "*");
        }
        #endregion

        #region 删除考试结果图片
        public JsonResult DeleteFieldExamImgFile(int imgId)
        {
            ResultMessage resultMessage = new ResultMessage();
            resultMessage.IsSuccess = true;
            try
            {
                Sys_Account account = LoginAccount as Sys_Account;
                IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
                workFlowCtrl.DeleteEmployeeExamResultFile(imgId);
            }
            catch (Exception ex)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = ex.Message;
            }
            return Json(resultMessage, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 提交考试结果
        public class SubmitExamResultParam
        {
            public List<int> employeeIdList { get; set; }
        }
        public JsonResult SubmitExamResult(string strData)
        {
            ResultMessage resultMessage = new ResultMessage();
            resultMessage.IsSuccess = true;
            try
            {
                SubmitExamResultParam param = strData.JSONStringToObj<SubmitExamResultParam>();
                Sys_Account account = LoginAccount as Sys_Account;
                IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
                workFlowCtrl.SummitExamResult(param.employeeIdList);
            }
            catch (Exception ex)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = ex.Message;
            }
            return Json(resultMessage, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 保存实操图片
        public JsonResult SaveFieldExamImg(SaveFieldExamImgParam param)
        {
            ResultMessage resultMessage = new ResultMessage();
            resultMessage.IsSuccess = true;
            Sys_Account account = LoginAccount as Sys_Account;
            IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
            try
            {
                //保存成绩记录
                Biz_EmployeeExamResultRecord employeeExamResultRecord = new Biz_EmployeeExamResultRecord()
                {
                    EmployeeId = param.EmployeeId,
                    FieldExamResult = param.FieldExamResult
                };
                int employeeExamResultRecordId = workFlowCtrl.SaveEmployeeExamResult(employeeExamResultRecord);

                for (int i = 0; param.FieldExamImg != null && i < param.FieldExamImg.Count; i++)
                {
                    HttpPostedFileWrapper file = param.FieldExamImg[i];
                    //保存文件
                    //获取后缀名
                    string extensions = file.FileName.Substring(file.FileName.LastIndexOf("."));
                    string relativePath = "考试结果图片\\" + param.ExamPlanId + "_" + param.EmployeeId + "\\" + Guid.NewGuid() + extensions;
                    string AbsolutePath = GetAbsolutePath(relativePath);
                    file.SaveAs(AbsolutePath);
                    //保存文件记录
                    Biz_EmployeeExamResultRecordFile employeeExamResultRecordFile = new Biz_EmployeeExamResultRecordFile()
                    {
                        EmployeeExamResultRecordId = employeeExamResultRecordId,
                        FileKey = "FieldExamImg",
                        FilePath = relativePath
                    };
                    workFlowCtrl.SaveEmployeeExamResultFile(employeeExamResultRecordFile);
                }
            }
            catch (Exception ex)
            {
                resultMessage.IsSuccess = false;
                resultMessage.ErrorMessage = ex.Message;
            }
            return Json(resultMessage, JsonRequestBehavior.AllowGet);
        }
        public class SaveFieldExamImgParam
        {
            public int ExamPlanId { get; set; }
            public int EmployeeId { get; set; }
            public bool FieldExamResult { get; set; }
            public List<HttpPostedFileWrapper> FieldExamImg { get; set; }

        }
        #endregion

        #region 通过相对路径返回文件绝对路径
        protected string RootFolderPath
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["FileRootDirectory"];
            }
        }
        protected string GetAbsolutePath(string relativePath)
        {
            string absolutePath = string.Format("{0}\\{1}", RootFolderPath.TrimEnd('\\'), relativePath.TrimStart('\\'));
            FileInfo fileinfo = new FileInfo(absolutePath);
            if (!fileinfo.Directory.Exists)
            {
                fileinfo.Directory.Create();
            }
            //if (fileinfo.Exists)
            //{
            //    fileinfo.Delete();
            //}

            return absolutePath;
        }
        #endregion

        #region 保存文件

        protected void SaveFile(Stream inputFileStream, string relativePath)
        {
            string absolutePath = GetAbsolutePath(relativePath);
            FileInfo file = new FileInfo(absolutePath);
            if (!file.Directory.Exists)
            {
                file.Directory.Create();
            }

            FileStream fs = null;
            try
            {
                fs = new FileStream(absolutePath, FileMode.Create);
                int bufferSize = 1024 * 1024 * 4;
                byte[] buffer = new byte[bufferSize];
                long lPos = 0;
                int readLength = bufferSize;
                while (readLength > 0)
                {
                    inputFileStream.Seek(lPos, SeekOrigin.Begin);
                    readLength = inputFileStream.Read(buffer, 0, buffer.Length);
                    fs.Seek(lPos, SeekOrigin.Begin);
                    fs.Write(buffer, 0, readLength);
                    lPos += readLength;
                }
                fs.Flush();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (null != fs)
                {
                    fs.Close();
                    fs = null;
                }
            }
        }
        #endregion


    }
}
