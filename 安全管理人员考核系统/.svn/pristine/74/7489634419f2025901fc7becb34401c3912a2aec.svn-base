using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using Library.baseFn;
using Model;
using BLL;

namespace PrintApp
{
    public partial class FrmLogin : Form
    {
        public enum Enum_LoginResult
        {
            Success, Fail, Cancel
        }

        public Enum_LoginResult LoginResult = Enum_LoginResult.Fail;
        public Sys_Account account = null;
        public FrmLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            btnLogin.Enabled = false;
            string username = txtLoginName.Text;
            string password = txtPassword.Text;
            Login(username, password);
            btnLogin.Enabled = true;
        }

        private void Login(string username, string password)
        {
            //LoginResult = Enum_LoginResult.Success;
            //this.Close();
            //return;
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    txtMsg.Text = "请输入用户名和密码";
                    txtMsg.SelectAll();
                    txtMsg.SelectionAlignment = HorizontalAlignment.Center;
                    txtMsg.Select(0, 0);
                    return;
                }
                IAccountCtrl acc = new AccountCtrl(account);
                if (acc.ValidAccount(username, password.MD5()))
                {

                    LoginResult = Enum_LoginResult.Success;
                    account = acc.GetAccountByAccountName(username);
                    int existsCount = account.RoleList.Select(p => p.RoleType).Where(p => p.Contains("Manager")).Count();
                    if (existsCount < 1)
                    {
                        txtMsg.Text = "非法用户";
                        txtMsg.SelectAll();
                        txtMsg.SelectionAlignment = HorizontalAlignment.Center;
                        txtMsg.Select(0, 0);
                        LoginResult = Enum_LoginResult.Cancel;
                        return;
                    }
                    this.Close();
                }
                else
                {
                    txtMsg.Text = "用户名或密码错误";
                    txtMsg.SelectAll();
                    txtMsg.SelectionAlignment = HorizontalAlignment.Center;
                    txtMsg.Select(0, 0);
                }
            }
            catch (Exception ex)
            {
                txtMsg.Text = ex.Message;
            }
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            btnLogin_Click(null, null);
        }
    }
}
