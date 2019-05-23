using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Library.baseFn;
using BLL;
using Model;

namespace IDCardInformationReader
{
    public partial class frmLogin : Form
    {
        public bool loginSuccess { get; set; }

        public Sys_Account loginAccount { get; set; }
        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string accountName = txtAccountName.Text;
                string password = txtPassword.Text;

                if (accountName.IsNull() || password.IsNull())
                {
                    throw new Exception("用户名或密码不能为空！");
                }

                IAccountCtrl accountCtrl = new AccountCtrl(null);

                password = password.MD5();
                bool loginFlag = accountCtrl.ValidAccount(accountName, password);
                if (!loginFlag)
                {
                    throw new Exception("用户名或密码错误！");
                }

                Sys_Account account = accountCtrl.GetAccountByAccountName(accountName);

                int roleCount = account.RoleList.Where(p => p.RoleType == "ExaminationPoint").Count();

                if (roleCount < 1)
                {
                    throw new Exception("请使用考核点账号登录！");
                }

                loginSuccess = true;
                loginAccount = account;
                this.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }
    }
}
