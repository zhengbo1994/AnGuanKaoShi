using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Configuration;
using System.Net;
using System.Collections.Specialized;
using AForge.Video.DirectShow;

using System.Linq;

using Model;
using BLL;
using Library.baseFn;


namespace IDCardInformationReader
{
    public partial class Main : Form
    {

        int iRetUSB = 0;
        int iRetCOM = 0;

        Sys_Account account;

        string imageResultPath = "";
        //是否读取过身份证
        private bool READ_IDCard = false;

        public Main()
        {
            InitializeComponent();
            try
            {
                CVRSDK.InitIDCardReader(ref iRetUSB, ref iRetCOM);
                InitCamera();
                btnConnect_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                System.Environment.Exit(0);
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            try
            {
                frmLogin loginPage = new frmLogin();
                loginPage.ShowDialog(this);

                if (!loginPage.loginSuccess)
                {
                    this.Close();
                }

                account = loginPage.loginAccount;
                InitExamInformation();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                CVRSDK.CVR_CloseComm();
                CameraClose();
                System.Environment.Exit(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void InitExamInformation()
        {
            IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
            List<Biz_ExamPlanRecord> examPlanRecordList = workFlowCtrl.GetExamPlanRecordListByTrainingInstitutionId(account.UserId);
            List<Biz_ExaminationRoom> examinationRoomList = workFlowCtrl.GetExaminationRoomListByTrainingInstitutionId(account.UserId);

            if (null == examPlanRecordList || examPlanRecordList.Count == 0)
            {
                throw new Exception("没有符合条件的考试计划");
            }

            if (null == examinationRoomList || examinationRoomList.Count == 0)
            {
                throw new Exception("没有符合条件的考场");
            }

            cbExamPlanNumber.Items.AddRange(examPlanRecordList.ToArray());
            cbExamPlanNumber.SelectedIndex = 0;

            cbExamRoomName.Items.AddRange(examinationRoomList.ToArray());
            cbExamRoomName.SelectedIndex = 0;



        }


        #region 身份证读取操作
        private void btnReadIDCard_Click(object sender, EventArgs e)
        {
            try
            {
                if ((iRetCOM == 1) || (iRetUSB == 1))
                {
                    int authenticate = CVRSDK.CVR_Authenticate();
                    if (authenticate == 1)
                    {
                        int readContent = CVRSDK.CVR_Read_Content(4);
                        if (readContent == 1)
                        {
                            FillData();
                        }
                        else
                        {
                            throw new Exception("读卡操作失败");
                        }
                    }
                    else
                    {
                        throw new Exception("未放卡或卡片放置不正确");
                    }
                }
                else
                {
                    throw new Exception("初始化失败！");
                }
                READ_IDCard = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                READ_IDCard = false;
            }
        }


        string iDNumber = "";
        private void FillData()
        {
            if (cbExamPlanNumber.SelectedItem.IsNull() || cbExamRoomName.SelectedItem.IsNull())
            {
                throw new Exception("请选择考试流水号和考场名称");
            }

            iDNumber = CVRSDK.IDCardNumber;
            //string iDNumber = "211322198509260317";
            int examPlanId = (cbExamPlanNumber.SelectedItem as Biz_ExamPlanRecord).Id;
            int examRoomId = (cbExamRoomName.SelectedItem as Biz_ExaminationRoom).Id;

            IWorkFlowCtrl workFlowCtrl = new WorkFlowCtrl(account);
            IEnterpriseCtrl enterpriseCtrl = new EnterpriseCtrl(account);

            Biz_Employee employee = workFlowCtrl.GetEmployeeForExamRoom(iDNumber, examPlanId, examRoomId);

            if (employee.IsNull())
            {
                throw new Exception("无效的考生");
            }

            Biz_Enterprise enterprise = enterpriseCtrl.GetEnterpriseById(employee.EnterpriseId);

            txtName.Text = employee.EmployeeName;
            txtSex.Text = employee.Sex;
            txtIDNumber.Text = employee.IDNumber;
            txtBirthday.Text = employee.Birthday.ToString("yyyy-MM-dd");
            txtDuty.Text = employee.Job;
            txtTechnical.Text = employee.Title4Technical;
            txtIndustry.Text = employee.Industry;
            txtExamType.Text = employee.ExamType;
            txtEnterpriseName.Text = enterprise.EnterpriseName;
            txtRemark.Text = employee.Remark;

            imgIDCard.ImageLocation = Application.StartupPath + "\\" + CVRSDK.IDCardImageName;
        }


        #endregion

        #region 摄像头操作
        private FilterInfoCollection videoDevices;

        private void InitCamera()
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (videoDevices.Count == 0)
            {
                throw new Exception("无可用的摄像头");
            }

            foreach (FilterInfo device in videoDevices)
            {
                cbCameras.Items.Add(device.Name);
            }

            cbCameras.SelectedIndex = 0;

        }

        //连接摄像头
        private void CameraConn()
        {

            if (videoSourcePlayer != null && videoSourcePlayer.IsRunning)
            {
                return;
            }

            if (cbCameras.SelectedItem.ToString().IsNull())
            {
                return;
            }

            VideoCaptureDevice videoSource = new VideoCaptureDevice(videoDevices[cbCameras.SelectedIndex].MonikerString);
            videoSourcePlayer.VideoSource = videoSource;
            videoSourcePlayer.Start();
        }

        private void CameraClose()
        {
            if (videoSourcePlayer != null && videoSourcePlayer.IsRunning)
            {
                videoSourcePlayer.SignalToStop();
                videoSourcePlayer.WaitForStop();
            }

        }

        private void GraphImage()
        {
            if (videoSourcePlayer.IsRunning)
            {
                BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                                videoSourcePlayer.GetCurrentVideoFrame().GetHbitmap(),
                                IntPtr.Zero,
                                 Int32Rect.Empty,
                                BitmapSizeOptions.FromEmptyOptions());
                PngBitmapEncoder pE = new PngBitmapEncoder();
                pE.Frames.Add(BitmapFrame.Create(bitmapSource));
                string imagePath = GetImagePath();
                string picName = imagePath + "\\" + "employee" + iDNumber + ".jpg";
                if (File.Exists(picName))
                {
                    try
                    {
                        File.Delete(picName);
                    }
                    catch
                    {
                        return;
                    }
                }
                using (Stream stream = File.Create(picName))
                {

                    FileInfo file = new FileInfo(picName);

                    pE.Save(stream);
                }

                imageResultPath = ImageHelper.formatPhoto(picName, imagePath, iDNumber);

                pbImageFormat.ImageLocation = imageResultPath;

            }


        }

        private void UploadPhoto()
        {

            string filePath = imageResultPath;
            string strUploadUrl = ConfigurationManager.AppSettings["PhotoUploadUrl"];
            FileInfo file = new FileInfo(filePath);
            if (!file.Exists)
            {
                throw new IOException("文件不存在：" + filePath);
            }

            FileStream fs = null;
            MemoryStream ms = null;
            try
            {
                fs = file.OpenRead();
                ms = new MemoryStream();

                fs.CopyToStream(ms);
                byte[] fileData = ms.ToArray();
                string strFileData = Convert.ToBase64String(fileData);

                WebClient wc = new WebClient();
                NameValueCollection uploadData = new NameValueCollection();
                string examId = (null == (cbExamPlanNumber.SelectedItem as Biz_ExamPlanRecord)
                    ? string.Empty
                    : (cbExamPlanNumber.SelectedItem as Biz_ExamPlanRecord).Id.ToString());
                string idNumber = txtIDNumber.Text;
                if (string.IsNullOrEmpty(examId))
                {
                    throw new Exception("请选择考试流水号");
                }
                if (string.IsNullOrEmpty(idNumber))
                {
                    throw new Exception("未读取到身份证号");
                }
                var data = new { ExamId = examId, IdNumber = idNumber, FileType = "进场照片", FileInBase64 = strFileData };
                string strData = data.ObjToJSONString();
                uploadData.Add("userData", strData);
                wc.UploadValues(strUploadUrl, uploadData);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (null != fs) fs.Close();
                if (null != ms) ms.Close();
            }
        }

        private string GetImagePath()
        {
            string personImgPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)
                         + Path.DirectorySeparatorChar.ToString() + "PersonImg";
            if (!Directory.Exists(personImgPath))
            {
                Directory.CreateDirectory(personImgPath);
            }

            return personImgPath;
        }

        //拍照
        private void Photograph_Click(object sender, EventArgs e)
        {

            try
            {
                if (!READ_IDCard)//拍照时 如果未识别身份证 则开始识别身份证
                {
                    btnReadIDCard_Click(null, null);
                }
                if (!READ_IDCard)//识别身份证不成功的  不能拍照
                {
                    return;
                }
                GraphImage();
                UploadPhoto();
                READ_IDCard = false;//照片上传完成之后 将省份证阅读标识重置为 未识别
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                CameraConn();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            CameraClose();
        }

        #endregion





    }
}
