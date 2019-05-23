namespace IDCardInformationReader
{
    partial class Main
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.imgIDCard = new System.Windows.Forms.PictureBox();
            this.btnReadIDCard = new System.Windows.Forms.Button();
            this.videoSourcePlayer = new AForge.Controls.VideoSourcePlayer();
            this.label3 = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.cbCameras = new System.Windows.Forms.ComboBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.Photograph = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbExamRoomName = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cbExamPlanNumber = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtRemark = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtEnterpriseName = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtExamType = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtIndustry = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtTechnical = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtDuty = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtBirthday = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtIDNumber = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSex = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pbImageFormat = new System.Windows.Forms.PictureBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.imgIDCard)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbImageFormat)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // imgIDCard
            // 
            this.imgIDCard.Location = new System.Drawing.Point(815, 35);
            this.imgIDCard.Name = "imgIDCard";
            this.imgIDCard.Size = new System.Drawing.Size(108, 143);
            this.imgIDCard.TabIndex = 9;
            this.imgIDCard.TabStop = false;
            // 
            // btnReadIDCard
            // 
            this.btnReadIDCard.Location = new System.Drawing.Point(798, 32);
            this.btnReadIDCard.Name = "btnReadIDCard";
            this.btnReadIDCard.Size = new System.Drawing.Size(130, 23);
            this.btnReadIDCard.TabIndex = 10;
            this.btnReadIDCard.Text = "读取身份证信息";
            this.btnReadIDCard.UseVisualStyleBackColor = true;
            this.btnReadIDCard.Click += new System.EventHandler(this.btnReadIDCard_Click);
            // 
            // videoSourcePlayer
            // 
            this.videoSourcePlayer.Location = new System.Drawing.Point(308, 36);
            this.videoSourcePlayer.Name = "videoSourcePlayer";
            this.videoSourcePlayer.Size = new System.Drawing.Size(364, 287);
            this.videoSourcePlayer.TabIndex = 16;
            this.videoSourcePlayer.Text = "videoSourcePlayer";
            this.videoSourcePlayer.VideoSource = null;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 15);
            this.label3.TabIndex = 15;
            this.label3.Text = "视频输入设备:";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(122, 82);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(140, 23);
            this.btnConnect.TabIndex = 14;
            this.btnConnect.Text = "连接摄像头";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // cbCameras
            // 
            this.cbCameras.FormattingEnabled = true;
            this.cbCameras.Location = new System.Drawing.Point(122, 36);
            this.cbCameras.Name = "cbCameras";
            this.cbCameras.Size = new System.Drawing.Size(140, 23);
            this.cbCameras.TabIndex = 13;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(122, 125);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(138, 23);
            this.btnClose.TabIndex = 12;
            this.btnClose.Text = "关闭摄像头";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // Photograph
            // 
            this.Photograph.Location = new System.Drawing.Point(122, 167);
            this.Photograph.Name = "Photograph";
            this.Photograph.Size = new System.Drawing.Size(138, 23);
            this.Photograph.TabIndex = 11;
            this.Photograph.Text = "拍照";
            this.Photograph.UseVisualStyleBackColor = true;
            this.Photograph.Click += new System.EventHandler(this.Photograph_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbExamRoomName);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.cbExamPlanNumber);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.btnReadIDCard);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(959, 80);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "考场选择";
            // 
            // cbExamRoomName
            // 
            this.cbExamRoomName.DisplayMember = "ExamRoomName";
            this.cbExamRoomName.FormattingEnabled = true;
            this.cbExamRoomName.Location = new System.Drawing.Point(364, 32);
            this.cbExamRoomName.Name = "cbExamRoomName";
            this.cbExamRoomName.Size = new System.Drawing.Size(140, 23);
            this.cbExamRoomName.TabIndex = 19;
            this.cbExamRoomName.ValueMember = "Id";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(273, 35);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 15);
            this.label6.TabIndex = 19;
            this.label6.Text = "考场名称：";
            // 
            // cbExamPlanNumber
            // 
            this.cbExamPlanNumber.DisplayMember = "ExamPlanNumber";
            this.cbExamPlanNumber.FormattingEnabled = true;
            this.cbExamPlanNumber.Location = new System.Drawing.Point(121, 32);
            this.cbExamPlanNumber.Name = "cbExamPlanNumber";
            this.cbExamPlanNumber.Size = new System.Drawing.Size(140, 23);
            this.cbExamPlanNumber.TabIndex = 18;
            this.cbExamPlanNumber.ValueMember = "Id";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 35);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(97, 15);
            this.label5.TabIndex = 18;
            this.label5.Text = "考试流水号：";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtRemark);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.txtEnterpriseName);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.txtExamType);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.txtIndustry);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.txtTechnical);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.txtDuty);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.txtBirthday);
            this.groupBox2.Controls.Add(this.imgIDCard);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.txtIDNumber);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.txtSex);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.txtName);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(12, 108);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(960, 235);
            this.groupBox2.TabIndex = 18;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "考生信息";
            // 
            // txtRemark
            // 
            this.txtRemark.Location = new System.Drawing.Point(122, 193);
            this.txtRemark.Name = "txtRemark";
            this.txtRemark.ReadOnly = true;
            this.txtRemark.Size = new System.Drawing.Size(642, 24);
            this.txtRemark.TabIndex = 38;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(63, 196);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(52, 15);
            this.label13.TabIndex = 37;
            this.label13.Text = "备注：";
            // 
            // txtEnterpriseName
            // 
            this.txtEnterpriseName.Location = new System.Drawing.Point(122, 154);
            this.txtEnterpriseName.Name = "txtEnterpriseName";
            this.txtEnterpriseName.ReadOnly = true;
            this.txtEnterpriseName.Size = new System.Drawing.Size(642, 24);
            this.txtEnterpriseName.TabIndex = 36;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(33, 157);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(82, 15);
            this.label12.TabIndex = 35;
            this.label12.Text = "所属企业：";
            // 
            // txtExamType
            // 
            this.txtExamType.Location = new System.Drawing.Point(365, 114);
            this.txtExamType.Name = "txtExamType";
            this.txtExamType.ReadOnly = true;
            this.txtExamType.Size = new System.Drawing.Size(140, 24);
            this.txtExamType.TabIndex = 34;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(276, 117);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(82, 15);
            this.label8.TabIndex = 33;
            this.label8.Text = "报考类型：";
            // 
            // txtIndustry
            // 
            this.txtIndustry.Location = new System.Drawing.Point(122, 114);
            this.txtIndustry.Name = "txtIndustry";
            this.txtIndustry.ReadOnly = true;
            this.txtIndustry.Size = new System.Drawing.Size(140, 24);
            this.txtIndustry.TabIndex = 32;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(33, 117);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(82, 15);
            this.label9.TabIndex = 31;
            this.label9.Text = "报考行业：";
            // 
            // txtTechnical
            // 
            this.txtTechnical.Location = new System.Drawing.Point(624, 74);
            this.txtTechnical.Name = "txtTechnical";
            this.txtTechnical.ReadOnly = true;
            this.txtTechnical.Size = new System.Drawing.Size(140, 24);
            this.txtTechnical.TabIndex = 30;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(537, 77);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(82, 15);
            this.label10.TabIndex = 29;
            this.label10.Text = "技术职称：";
            // 
            // txtDuty
            // 
            this.txtDuty.Location = new System.Drawing.Point(365, 74);
            this.txtDuty.Name = "txtDuty";
            this.txtDuty.ReadOnly = true;
            this.txtDuty.Size = new System.Drawing.Size(140, 24);
            this.txtDuty.TabIndex = 28;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(306, 77);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(52, 15);
            this.label11.TabIndex = 27;
            this.label11.Text = "职务：";
            // 
            // txtBirthday
            // 
            this.txtBirthday.Location = new System.Drawing.Point(122, 74);
            this.txtBirthday.Name = "txtBirthday";
            this.txtBirthday.ReadOnly = true;
            this.txtBirthday.Size = new System.Drawing.Size(140, 24);
            this.txtBirthday.TabIndex = 26;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(35, 77);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(82, 15);
            this.label7.TabIndex = 25;
            this.label7.Text = "出生日期：";
            // 
            // txtIDNumber
            // 
            this.txtIDNumber.Location = new System.Drawing.Point(624, 35);
            this.txtIDNumber.Name = "txtIDNumber";
            this.txtIDNumber.ReadOnly = true;
            this.txtIDNumber.Size = new System.Drawing.Size(140, 24);
            this.txtIDNumber.TabIndex = 24;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(537, 38);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 15);
            this.label4.TabIndex = 23;
            this.label4.Text = "身份证号：";
            // 
            // txtSex
            // 
            this.txtSex.Location = new System.Drawing.Point(365, 35);
            this.txtSex.Name = "txtSex";
            this.txtSex.ReadOnly = true;
            this.txtSex.Size = new System.Drawing.Size(140, 24);
            this.txtSex.TabIndex = 22;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(306, 38);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 15);
            this.label2.TabIndex = 21;
            this.label2.Text = "性别：";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(122, 35);
            this.txtName.Name = "txtName";
            this.txtName.ReadOnly = true;
            this.txtName.Size = new System.Drawing.Size(140, 24);
            this.txtName.TabIndex = 20;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(63, 38);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 15);
            this.label1.TabIndex = 19;
            this.label1.Text = "姓名：";
            // 
            // pbImageFormat
            // 
            this.pbImageFormat.Location = new System.Drawing.Point(722, 36);
            this.pbImageFormat.Name = "pbImageFormat";
            this.pbImageFormat.Size = new System.Drawing.Size(209, 287);
            this.pbImageFormat.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbImageFormat.TabIndex = 19;
            this.pbImageFormat.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button1);
            this.groupBox3.Controls.Add(this.cbCameras);
            this.groupBox3.Controls.Add(this.pbImageFormat);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.btnConnect);
            this.groupBox3.Controls.Add(this.btnClose);
            this.groupBox3.Controls.Add(this.videoSourcePlayer);
            this.groupBox3.Controls.Add(this.Photograph);
            this.groupBox3.Location = new System.Drawing.Point(12, 365);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(960, 341);
            this.groupBox3.TabIndex = 20;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "拍照上传";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(122, 212);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(138, 23);
            this.button1.TabIndex = 20;
            this.button1.Text = "保存照片";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(985, 712);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("宋体", 11F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "取证人员身份验证系统";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            ((System.ComponentModel.ISupportInitialize)(this.imgIDCard)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbImageFormat)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox imgIDCard;
        private System.Windows.Forms.Button btnReadIDCard;
        private AForge.Controls.VideoSourcePlayer videoSourcePlayer;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.ComboBox cbCameras;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button Photograph;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cbExamRoomName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cbExamPlanNumber;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtSex;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtRemark;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtEnterpriseName;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtExamType;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtIndustry;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtTechnical;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtDuty;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtBirthday;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtIDNumber;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pbImageFormat;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button button1;
    }
}

