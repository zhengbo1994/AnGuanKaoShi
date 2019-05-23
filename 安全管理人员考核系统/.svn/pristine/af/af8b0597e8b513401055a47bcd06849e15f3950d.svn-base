using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using BLL;
using Model;
using PrintApp.Common;

namespace PrintApp
{
    public partial class FrmMain : Form
    {
        public Dictionary<string, string> ExamTypes = null;
        public Dictionary<string, string> Industries = null;

        Sys_Account account = null;
        IWorkFlowCtrl workFlowCtrl = null;
        public FrmMain()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            try
            {
                InitLogin();
                InitSelect();
                InitCertificateGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message + "\r\n" + ex.StackTrace, "错误");
                this.Close();
            }
        }

        public bool CheckIsLogin()
        {
            return false;
        }

        private void InitLogin()
        {
            if (!CheckIsLogin())
            {
                FrmLogin frmLogin = new FrmLogin();
                frmLogin.ShowDialog();
                if (FrmLogin.Enum_LoginResult.Success != frmLogin.LoginResult)
                {
                    this.Close();
                }
                account = frmLogin.account;
                workFlowCtrl = new WorkFlowCtrl(account);
                //修改页面标题
                string username = workFlowCtrl.GetUserName(account.Id);
                this.Text = string.Format(this.Text, username);
            }

        }

        private void InitSelect()
        {

            List<Sys_DropdownListItem> ExamTypeDropdownListItemList = workFlowCtrl.GetEmployeeSubjectList();
            ExamTypeDropdownListItemList.Insert(0, new Sys_DropdownListItem() { ItemText = "全部", ItemValue = "" });
            List<Sys_DropdownListItem> IndustryDropdownListItemList = workFlowCtrl.GetEmployeeIndustryList();
            IndustryDropdownListItemList.Insert(0, new Sys_DropdownListItem() { ItemText = "全部", ItemValue = "" });

            cmbCerfiticate_ExamType.ValueMember = "ItemValue";
            cmbCerfiticate_ExamType.DisplayMember = "ItemText";
            cmbCerfiticate_ExamType.DataSource = ExamTypeDropdownListItemList;
            cmbCerfiticate_ExamType.SelectedValue = "";
            cmbCerfiticate_Industry.ValueMember = "ItemValue";
            cmbCerfiticate_Industry.DisplayMember = "ItemText";
            cmbCerfiticate_Industry.DataSource = IndustryDropdownListItemList;
            cmbCerfiticate_Industry.SelectedValue = "";

            //ExamTypes = new Dictionary<string, string>() { { "", "全部" }, { "A", "A" }, { "B", "B" }, { "C1", "C1" }, { "C2", "C2" }, { "C3", "C3" } };
            //Industries = new Dictionary<string, string>() { { "", "全部" }, { "建筑施工", "建筑施工" }, { "化工", "化工" }, { "交通", "交通" }, { "铁路", "铁路" } };
            //cmbCerfiticate_ExamType.ValueMember = "ValueMember";
            //cmbCerfiticate_ExamType.DisplayMember = "DisplayMember";
            //cmbCerfiticate_ExamType.Items.AddRange(ExamTypes.Select(x => new { ValueMember = x.Key, DisplayMember = x.Value }).ToArray());
            //cmbCerfiticate_ExamType.SelectedIndex = 0;
            //cmbCerfiticate_Industry.ValueMember = "ValueMember";
            //cmbCerfiticate_Industry.DisplayMember = "DisplayMember";
            //cmbCerfiticate_Industry.Items.AddRange(Industries.Select(x => new { ValueMember = x.Key, DisplayMember = x.Value }).ToArray());
            //cmbCerfiticate_Industry.SelectedIndex = 0;
        }

        class Column
        {
            public string DataPropertyName { get; set; }
            public string HeaderText { get; set; }

            DataGridViewColumn _ColumnType = new DataGridViewTextBoxColumn();
            public DataGridViewColumn ColumnType
            {
                get { return _ColumnType; }
                set { _ColumnType = value; }
            }


            bool _ReadOnly = true;
            public bool ReadOnly
            {
                get { return _ReadOnly; }
                set { _ReadOnly = value; }
            }

            bool _Visible = true;
            public bool Visible
            {
                get { return _Visible; }
                set { _Visible = value; }
            }

            DataGridViewAutoSizeColumnMode _AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            public DataGridViewAutoSizeColumnMode AutoSizeMode
            {
                get { return _AutoSizeMode; }
                set { _AutoSizeMode = value; }
            }

            int _Width = 0;
            public int Width
            {
                get { return _Width; }
                set
                {
                    _AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    _Width = value;
                }
            }

        }
        void initGrid()
        {

            dgvCertificate.AutoGenerateColumns = false;
            dgvCertificate.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCertificate.RowHeadersVisible = false;

            List<Column> cols = new List<Column>()
            {
               // new Column(){DataPropertyName="Checked",HeaderText="选择",ReadOnly=false,ColumnType= new DataGridViewCheckBoxColumn(),Width=50 } ,
                new Column(){DataPropertyName="EmployeeId",HeaderText="EmployeeId",Visible=false } ,
                new Column(){DataPropertyName="EmployeeName",HeaderText="姓名",Width=80 } ,
                new Column(){DataPropertyName="Sex",HeaderText="性别",Width=75 } ,
                new Column(){DataPropertyName="Birthday",HeaderText="出生年月",Width=100 } ,
                new Column(){DataPropertyName="IDNumber",HeaderText="身份证号",Width=180 } ,
                new Column(){DataPropertyName="EnterpriseName",HeaderText="企业名称" ,Width=200 } ,
                new Column(){DataPropertyName="Job",HeaderText="职务",Width=80 } ,
                new Column(){DataPropertyName="Title4Technical",HeaderText="技术职称",Width=100 },
                new Column(){DataPropertyName="ExamPlanNumber",HeaderText="考试计划流水号",Width=150 } ,
                new Column(){DataPropertyName="TrainingInstutionName",HeaderText="考核点名称",Width=200 } ,
                new Column(){DataPropertyName="Industry",HeaderText="证书行业",Width=100 } ,
                new Column(){DataPropertyName="ExamType",HeaderText="证书类型",Width=100 } ,
                new Column(){DataPropertyName="CertificateNo",HeaderText="证书编号",Width=150 } ,
                new Column(){DataPropertyName="StartCertificateDate",HeaderText="发证日期",Width=150 },
                new Column(){DataPropertyName="EndCertificateDate",HeaderText="证书有效期",Width=150 }
            };
            for (int i = 0; i < cols.Count(); i++)
            {
                Column col = cols[i];
                DataGridViewColumn dataCol = col.ColumnType;
                dataCol.DataPropertyName = col.DataPropertyName;
                dataCol.HeaderText = col.HeaderText;
                dataCol.ReadOnly = col.ReadOnly;
                dataCol.Visible = col.Visible;
                dataCol.AutoSizeMode = col.AutoSizeMode;
                dataCol.Width = col.Width;
                dgvCertificate.Columns.Add(dataCol);
            }

        }
        void LoadData()
        {
            Dictionary<string, string> dicCondition = GetValues(this.groupBoxCerfiticate_QueryArea);
            IEnterpriseCtrl enterpriseCtrl = new EnterpriseCtrl(account);
            IExaminationPointCtrl trainingInstitutionCtrl = new ExaminationPointCtrl(account);
            List<Biz_Employee> employeeList = workFlowCtrl.GetEmployeeListInPrintCertificate(
                dicCondition["txtCerfiticate_EmployeeName"],
                dicCondition["txtCerfiticate_IDNumber"],
                dicCondition["cmbCerfiticate_ExamType"],
                dicCondition["cmbCerfiticate_Industry"],
                dicCondition["txtCerfiticate_ExamPlanNumber"],
                dicCondition["txtCerfiticate_Enterprise"],
                null, null);
            List<Biz_EmployeeForExamPlanRecord> employeeForExamPlanRecordList = workFlowCtrl.GetEmployeeForExamPlanRecordByEmployeeIdList(employeeList.Select(p => p.Id).ToList());
            List<Biz_ExamPlanRecord> examPlanRecordList = workFlowCtrl.GetExamPlanRecordByIdList(employeeForExamPlanRecordList.Select(p => p.ExamPlanRecordId).ToList());
            List<Biz_Enterprise> enterpriseList = enterpriseCtrl.GetEnterpriseInfoByIdList(employeeList.Select(p => p.EnterpriseId).ToList());
            List<Biz_ExaminationRoom> examRoomList = trainingInstitutionCtrl.GetExaminationRoomByIdList(employeeForExamPlanRecordList.Select(p => p.ExamRoomId).ToList());
            List<Biz_ExaminationPoint> trainingInstitutionList = trainingInstitutionCtrl.GetExaminationPointByIdList(examRoomList.Select(p => p.ExaminationPointId).ToList());
            //证书List
            List<Biz_Certificate> certificateList = workFlowCtrl.GetCertificateList(employeeList);

            List<CertificateInfo> CertificateInfoList = employeeList.Join(employeeForExamPlanRecordList, a => a.Id, b => b.EmployeeId, (a, b) => new { a, b })
                                                       .Join(examPlanRecordList, c => c.b.ExamPlanRecordId, d => d.Id, (c, d) => new { c, d })
                                                       .Join(examRoomList, e => e.c.b.ExamRoomId, f => f.Id, (e, f) => new { e, f })
                                                       .Join(trainingInstitutionList, g => g.f.ExaminationPointId, h => h.Id, (g, h) => new { g, h })
                                                       .Join(enterpriseList, i => i.g.e.c.a.EnterpriseId, j => j.Id, (i, j) => new { i, j })
                                                       .GroupJoin(certificateList, l => new { l.i.g.e.c.a.IDNumber, l.i.g.e.c.a.Industry, l.i.g.e.c.a.ExamType }, m => new { m.IDNumber, m.Industry, m.ExamType }, (l, m) => new
                                                       {
                                                           l,
                                                           CertificateNo = m.Count() == 0 ? "" : m.FirstOrDefault().CertificateNo,
                                                           StartCertificateDate = m.Count() == 0 ? "" : m.FirstOrDefault().StartCertificateDate.ToString("yyyy-MM-dd"),
                                                           EndCertificateDate = m.Count() == 0 ? "" : m.FirstOrDefault().EndCertificateDate.ToString("yyyy-MM-dd")
                                                       })
                                                       .Select(p => new CertificateInfo()
                                                       {
                                                           EmployeeId = p.l.i.g.e.c.a.Id,
                                                           EmployeeName = p.l.i.g.e.c.a.EmployeeName,
                                                           Sex = p.l.i.g.e.c.a.Sex,
                                                           Birthday = p.l.i.g.e.c.a.Birthday.ToString("yyyy-MM-dd"),
                                                           IDNumber = p.l.i.g.e.c.a.IDNumber,
                                                           EnterpriseName = p.l.j.EnterpriseName,
                                                           Job = p.l.i.g.e.c.a.Job,
                                                           Title4Technical = p.l.i.g.e.c.a.Title4Technical,
                                                           ExamPlanNumber = p.l.i.g.e.d.ExamPlanNumber,
                                                           TrainingInstutionName = p.l.i.h.InstitutionName,
                                                           Industry = p.l.i.g.e.c.a.Industry,
                                                           ExamType = p.l.i.g.e.c.a.ExamType,
                                                           CertificateNo = p.CertificateNo,
                                                           StartCertificateDate = p.StartCertificateDate,
                                                           EndCertificateDate = p.EndCertificateDate
                                                       }).ToList();




            dgvCertificate.DataSource = CertificateInfoList;
        }
        private void InitCertificateGrid()
        {
            initGrid();
            LoadData();
        }

        Dictionary<string, string> GetValues(GroupBox container)
        {
            Dictionary<string, string> dicItem = new Dictionary<string, string>();
            foreach (Control item in container.Controls)
            {
                if (item.GetType() == typeof(ComboBox))
                {
                    ComboBox cbo = item as ComboBox;
                    dicItem.Add(cbo.Name, cbo.SelectedValue.ToString());
                }
                else if (item.GetType() == typeof(TextBox))
                {
                    TextBox txt = item as TextBox;
                    dicItem.Add(txt.Name, txt.Text.Trim());
                }

            }
            return dicItem;
        }
        private void InitCertificateChangeGrid()
        {

        }

        private void InitCertificateExtensionGrid()
        {

        }
        //证书打印查询
        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadData();
        }
        //证书打印
        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                int SelectedRowsCount = dgvCertificate.SelectedRows.Count;
                if (SelectedRowsCount < 1)
                {
                    throw new Exception("请选择需要打印的记录");
                }
                string printTemplatePath = AppDomain.CurrentDomain.BaseDirectory + "证书模板.xls";
                Dictionary<string, string> dicRowData = new Dictionary<string, string>();
                int selecedIndex = dgvCertificate.CurrentRow.Index;
                DataGridViewRow dataRow = dgvCertificate.Rows[selecedIndex];
                foreach (DataGridViewCell cell in dataRow.Cells)
                {
                    dicRowData.Add(cell.OwningColumn.HeaderText, Convert.ToString(cell.Value));
                }

                CertificatePrinter certificatePrinter = new CertificatePrinter();
                certificatePrinter.PrintCertificate(printTemplatePath, dicRowData);
                Biz_CertificatePrintRecord certificatePrintRecord = new Biz_CertificatePrintRecord()
                {
                    CertificateNo  = Convert.ToString(dicRowData["CertificateNo"]),
                    PrintType = "PrintCertificate",
                    Remark = "新打证",
                    CreateDate = DateTime.Now,
                    CreateById = account.Id
                };
                workFlowCtrl.SaveCertificatePrintRecord(certificatePrintRecord);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
