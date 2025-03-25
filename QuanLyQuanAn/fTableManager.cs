using QuanLyQuanAn;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Button = System.Windows.Forms.Button;
using ComboBox = System.Windows.Forms.ComboBox;
using Menu = QuanLyQuanAn.DTO.Menu;

namespace QuanLyQuanAn
{
    public partial class fTableManager: Form
    {
        public fTableManager()
        {
            InitializeComponent();

            loadTable();
            loadCategory();
        }
        #region Method

        void loadCategory()
        {
            List<Category> listCategory = CategoryDAO.Instance.GetListCategory();
            cbCategory.DataSource = listCategory;
            cbCategory.DisplayMember = "name";
        }

        void loadFoodByCategoryID(int id)
        {
            List<Food> listFood = FoodDAO.Instance.GetFoodByCategoryID(id);
            cbFood.DataSource = listFood;
            cbFood.DisplayMember = "name";
        }

        void loadTable()
        {
            flowLayoutPanel1.Controls.Clear();
            List<Table> tablelist = TableDAO.Instance.loadTableList();
            foreach (Table item in tablelist) { 
                Button btn = new Button() { 
                    Width = TableDAO.TableWidth,
                    Height = TableDAO.TableHeight,
                };
                btn.Text = item.Name + Environment.NewLine + item.Status;
                btn.Click += btn_click;
                btn.Tag = item;
                switch (item.Status)
                {
                    case "Trống" :
                        btn.BackColor = Color.Aqua;
                        break;
                    default:
                        btn.BackColor = Color.LightPink;
                        break;
                }
                flowLayoutPanel1.Controls.Add(btn);

            }

        }

        void show_bill(int id)
        {
            lsvBill.Items.Clear();
            List<Menu> listBillInfo = MenuDAO.Instance.GetListMenuByTable(id);
            float ttp = 0;

            foreach(Menu item in listBillInfo)
            {
                ListViewItem lsvItem = new ListViewItem(item.Foodname.ToString());
                lsvItem.SubItems.Add(item.Count.ToString());
                lsvItem.SubItems.Add(item.Price.ToString());
                lsvItem.SubItems.Add(item.Totalprice.ToString());
                ttp += item.Totalprice;

                lsvBill.Items.Add(lsvItem);
            }
            textBox2.Text = ttp.ToString("c");
            lsvBill.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            lsvBill.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        #endregion

        #region Event 

        void btn_click(object sender, EventArgs e)
        {
            int tableID = ((sender as Button).Tag as Table).ID;
            lsvBill.Tag = (sender as Button).Tag;
            show_bill(tableID);

        }

        #endregion

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lsvBill_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void fTableManager_Load(object sender, EventArgs e)
        {

        }

        private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void adminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fAdmin f = new fAdmin();
            f.ShowDialog();
        }

        private void thôngTinCáNhânToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fAccountProfile f = new fAccountProfile();
            f.ShowDialog();
        }

        private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = 0;

            ComboBox cb = sender as ComboBox;

            if (cb.SelectedItem == null) return;

            Category selected = cb.SelectedItem as Category;
            id = selected.Id;

            loadFoodByCategoryID(id);
        }

        private void btnAddFood_Click(object sender, EventArgs e)
        {
            Table table = lsvBill.Tag as Table;
            int idBill = BillDAO.Instance.GetUnCheckBillByTableID(table.ID);
            int idFood = (cbFood.SelectedItem as Food).Id;
            int count = (int)numFoodCount.Value;
            if (idBill == -1) { 
                BillDAO.Instance.insertBill(table.ID);
                BillInfoDAO.Instance.insertBillInfo(BillDAO.Instance.getMaxIDBill(), idFood, count);
            } else
            {
                BillInfoDAO.Instance.insertBillInfo(idBill, idFood, count);
            }
            show_bill(table.ID);
            loadTable();
        }

        private void btnCheckOut_Click(object sender, EventArgs e)
        {
            Table table = lsvBill.Tag as Table;
            int idBill = BillDAO.Instance.GetUnCheckBillByTableID(table.ID);
            if(idBill != -1) { 
                if(MessageBox.Show("Bạn có chắc thanh toán hóa đơn cho bàn " + table.Name, "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK){
                    BillDAO.Instance.checkout(idBill);
                    show_bill(idBill);
                    loadTable();
                }
            }
              
        }
    }
}
