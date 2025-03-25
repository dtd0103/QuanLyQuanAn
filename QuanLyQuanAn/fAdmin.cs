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

            loadAccountList();
        }

        void loadAccountList()
        {
            /*
            string connectionSTR = "Data Source=DESKTOP-2TIKS5H;Initial Catalog=qlqa;Integrated Security=True";
            SqlConnection connection = new SqlConnection(connectionSTR);

            string query = "Select * from Account";

            connection.Open();

            SqlCommand command = new SqlCommand(query, connection);
            DataTable data = new DataTable();

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            
            adapter.Fill(data);
            connection.Close();
            
            dtgvAccount.DataSource = data;
            */
            string query = "Select * from Account";

           // DataProvider provider = new DataProvider();

            dtgvAccount.DataSource = DataProvider.Instance.ExecuteQuery(query);

        }

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
    }
}
