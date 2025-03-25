using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanAn.DTO
{
    public class Account
    {
        public Account(string userName, string name, int type, string password = null)
        {
            this.UserName = userName;
            this.Name = name;
            this.Type = type;
            this.Password = password;
        }

        public Account (DataRow row)
        {
            this.UserName = row["userName"].ToString();
            this.Name = row["name"].ToString();
            this.Type = (int)row["type"];
            this.Password = row["password"].ToString();
        }

        private int type;
        public int Type { get => type; set => type = value; }

        private string userName;
        public string UserName { get => userName; set => userName = value; }
        
        private string password;
        public string Password { get => password; set => password = value; }

        private string name;
        public string Name { get => name; set => name = value; }
        
    }
}
