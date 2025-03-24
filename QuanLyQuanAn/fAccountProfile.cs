using QuanLyQuanAn.DAO;
using QuanLyQuanAn.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyQuanAn
{
    public partial class fAccountProfile: Form
    {

        private Account loginAccount;
        public Account LoginAccount { get => loginAccount; set { loginAccount = value; ChangeAccount(loginAccount); } }
        public fAccountProfile(Account acc)
        {
            InitializeComponent();
            LoginAccount = acc;
        }

        
        void ChangeAccount(Account acc)
        {
            txbUserName.Text = LoginAccount.UserName;
            txbUserDisplayName.Text = LoginAccount.Name;
        }

        void UpdateAccount()
        {
            string displayName = txbUserDisplayName.Text;
            string password = txbUserPassword.Text;
            string newpass = txbNewPassword.Text;
            string reenterPass = txbRetypePassword.Text;
            string userName = txbUserName.Text;

            if(!newpass.Equals(reenterPass))
            {
                MessageBox.Show("Vui lòng nhập lại mật khẩu đúng với mật khẩu mới!");
            }

            else
            {
                if (AccountDAO.Instance.UpdateAccount(userName, displayName, password, newpass))
                {
                    MessageBox.Show("Cập nhật thành công!");
                    if (updateAccountEvent != null)
                    {
                        updateAccountEvent(this, new AccountEvent(AccountDAO.Instance.GetAccountByUserName(userName)));
                    }    
                }
                else
                {
                    MessageBox.Show("Vui lòng nhập đúng mật khẩu!");
                }
            } 
        }

        private event EventHandler<AccountEvent> updateAccountEvent;
        public event EventHandler<AccountEvent> UpdateAccountEvent
        {
            add { updateAccountEvent += value; }
            remove { updateAccountEvent -= value; }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnUpdateAccount_Click(object sender, EventArgs e)
        {
            UpdateAccount();
        }

        private void Profile_Load(object sender, EventArgs e)
        {

        }

        
    }

    public class AccountEvent : EventArgs
    {
        private Account acc;

        public Account Acc { get => acc; set => acc = value; }

        public AccountEvent(Account acc)
        {
            this.Acc = acc;
        }
    }
}
