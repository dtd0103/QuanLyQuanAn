using QuanLyQuanAn.DAO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyQuanAn
{
    public partial class fAdmin: Form
    {
        public fAdmin()
        {
            InitializeComponent();

            //LoadAccountList();
            //LoadFoodList();
        }

        //void LoadFoodList()
        //{
        //    string query = "SELECT * FROM FOOD";

        //    DataProvider provider = new DataProvider();

        //    dtgvFood.DataSource = DataProvider.Instance.ExecuteQuery(query);

        //}

        //void LoadAccountList ()
        //{
        //    string query = "EXEC dbo.USP_GetAccountByUserName @userName";

        //    DataProvider provider = new DataProvider();

        //    dtgvAcccount.DataSource = DataProvider.Instance.ExecuteQuery(query, new object[] {"staff"});

        //}

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void fAdmin_Load(object sender, EventArgs e)
        {

        }
    }
}
