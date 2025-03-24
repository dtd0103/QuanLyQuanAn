using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Configuration;

namespace QuanLyBill
{
    public partial class BillManagementForm : Form
    {
        // Khai báo chuỗi kết nối được lấy từ App.config
        private string connectionString;
        private DataTable billDataTable = new DataTable();

        public BillManagementForm()
        {
            InitializeComponent();

            // Đọc chuỗi kết nối từ App.config
            connectionString = ConfigurationManager.ConnectionStrings["qlqaConnection"].ConnectionString;

            // Khởi tạo dữ liệu khi form được tạo
            LoadBills();
            LoadTables();
            LoadFoods();
        }

        // Load danh sách Bill
        private void LoadBills()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"SELECT b.id, t.name AS TableName, b.dateCheckIn, b.dateCheckOut, 
                                    CASE WHEN b.status = 1 THEN N'Đã thanh toán' ELSE N'Chưa thanh toán' END AS Status
                                    FROM Bill b
                                    JOIN Table_Food t ON b.idTable = t.id
                                    ORDER BY b.id DESC";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    billDataTable.Clear();
                    adapter.Fill(billDataTable);
                    dgvBills.DataSource = billDataTable;

                    // Đặt tên cột
                    dgvBills.Columns[0].HeaderText = "Mã hóa đơn";
                    dgvBills.Columns[1].HeaderText = "Bàn";
                    dgvBills.Columns[2].HeaderText = "Ngày vào";
                    dgvBills.Columns[3].HeaderText = "Ngày ra";
                    dgvBills.Columns[4].HeaderText = "Trạng thái";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách hóa đơn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Load danh sách bàn ăn
        private void LoadTables()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT id, name FROM Table_Food ORDER BY name";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable tableDataTable = new DataTable();
                    adapter.Fill(tableDataTable);

                    cboTable.DataSource = tableDataTable;
                    cboTable.DisplayMember = "name";
                    cboTable.ValueMember = "id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách bàn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Load danh sách món ăn
        private void LoadFoods()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT id, name FROM Food ORDER BY name";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable foodDataTable = new DataTable();
                    adapter.Fill(foodDataTable);

                    cboFood.DataSource = foodDataTable;
                    cboFood.DisplayMember = "name";
                    cboFood.ValueMember = "id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách món ăn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Load chi tiết hóa đơn
        private void LoadBillDetails(int billId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"SELECT bi.id, f.name AS FoodName, bi.count, f.price, (f.price * bi.count) AS TotalPrice
                                    FROM BillInfo bi
                                    JOIN Food f ON bi.idFood = f.id
                                    WHERE bi.idBill = @BillId";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@BillId", billId);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable billDetailDataTable = new DataTable();
                    adapter.Fill(billDetailDataTable);

                    dgvBillDetails.DataSource = billDetailDataTable;

                    // Đặt tên cột
                    dgvBillDetails.Columns[0].HeaderText = "Mã chi tiết";
                    dgvBillDetails.Columns[1].HeaderText = "Tên món";
                    dgvBillDetails.Columns[2].HeaderText = "Số lượng";
                    dgvBillDetails.Columns[3].HeaderText = "Đơn giá";
                    dgvBillDetails.Columns[4].HeaderText = "Thành tiền";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải chi tiết hóa đơn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Sự kiện chọn một Bill trong DataGridView
        private void dgvBills_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvBills.Rows[e.RowIndex];

                int billId = Convert.ToInt32(row.Cells[0].Value);
                txtBillId.Text = billId.ToString();

                // Load thông tin chi tiết Bill từ cơ sở dữ liệu
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT idTable, dateCheckIn, dateCheckOut, status FROM Bill WHERE id = @BillId";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@BillId", billId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cboTable.SelectedValue = reader.GetInt32(0);
                            dtpCheckIn.Value = reader.GetDateTime(1);

                            if (!reader.IsDBNull(2))
                                dtpCheckOut.Value = reader.GetDateTime(2);
                            else
                                dtpCheckOut.Value = DateTime.Now;

                            chkStatus.Checked = reader.GetInt32(3) == 1;
                        }
                    }
                }

                // Load chi tiết hóa đơn
                LoadBillDetails(billId);
            }
        }

        // Sự kiện chọn một BillDetail trong DataGridView
        private void dgvBillDetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvBillDetails.Rows[e.RowIndex];

                // Lấy thông tin từ dòng được chọn
                int billInfoId = Convert.ToInt32(row.Cells[0].Value);
                string foodName = row.Cells[1].Value.ToString();
                int quantity = Convert.ToInt32(row.Cells[2].Value);

                // Đặt giá trị cho các control
                // Tìm món ăn trong combobox
                for (int i = 0; i < cboFood.Items.Count; i++)
                {
                    DataRowView item = (DataRowView)cboFood.Items[i];
                    if (item["name"].ToString() == foodName)
                    {
                        cboFood.SelectedIndex = i;
                        break;
                    }
                }

                numQuantity.Value = quantity;
            }
        }

        // Thêm mới Bill
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (cboTable.SelectedIndex < 0)
            {
                MessageBox.Show("Vui lòng chọn bàn", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"INSERT INTO Bill (idTable, dateCheckIn, dateCheckOut, status)
                                    VALUES (@IdTable, @DateCheckIn, @DateCheckOut, @Status);
                                    SELECT SCOPE_IDENTITY();";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@IdTable", cboTable.SelectedValue);
                    command.Parameters.AddWithValue("@DateCheckIn", dtpCheckIn.Value);

                    if (chkStatus.Checked)
                        command.Parameters.AddWithValue("@DateCheckOut", dtpCheckOut.Value);
                    else
                        command.Parameters.AddWithValue("@DateCheckOut", DBNull.Value);

                    command.Parameters.AddWithValue("@Status", chkStatus.Checked ? 1 : 0);

                    // Lấy ID của Bill vừa thêm
                    int newBillId = Convert.ToInt32(command.ExecuteScalar());
                    txtBillId.Text = newBillId.ToString();

                    MessageBox.Show("Thêm hóa đơn mới thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadBills();
                    LoadBillDetails(newBillId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm hóa đơn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Cập nhật Bill
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBillId.Text))
            {
                MessageBox.Show("Vui lòng chọn hóa đơn cần cập nhật", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"UPDATE Bill
                                    SET idTable = @IdTable,
                                        dateCheckIn = @DateCheckIn,
                                        dateCheckOut = @DateCheckOut,
                                        status = @Status
                                    WHERE id = @BillId";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@BillId", Convert.ToInt32(txtBillId.Text));
                    command.Parameters.AddWithValue("@IdTable", cboTable.SelectedValue);
                    command.Parameters.AddWithValue("@DateCheckIn", dtpCheckIn.Value);

                    if (chkStatus.Checked)
                        command.Parameters.AddWithValue("@DateCheckOut", dtpCheckOut.Value);
                    else
                        command.Parameters.AddWithValue("@DateCheckOut", DBNull.Value);

                    command.Parameters.AddWithValue("@Status", chkStatus.Checked ? 1 : 0);

                    command.ExecuteNonQuery();

                    MessageBox.Show("Cập nhật hóa đơn thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadBills();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật hóa đơn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xóa Bill
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBillId.Text))
            {
                MessageBox.Show("Vui lòng chọn hóa đơn cần xóa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Xác nhận xóa
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa hóa đơn này không?",
                                                  "Xác nhận xóa",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);
            if (result == DialogResult.No)
                return;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Xóa các chi tiết hóa đơn trước
                    string deleteDetailsQuery = "DELETE FROM BillInfo WHERE idBill = @BillId";
                    SqlCommand deleteDetailsCommand = new SqlCommand(deleteDetailsQuery, connection);
                    deleteDetailsCommand.Parameters.AddWithValue("@BillId", Convert.ToInt32(txtBillId.Text));
                    deleteDetailsCommand.ExecuteNonQuery();

                    // Xóa hóa đơn
                    string deleteBillQuery = "DELETE FROM Bill WHERE id = @BillId";
                    SqlCommand deleteBillCommand = new SqlCommand(deleteBillQuery, connection);
                    deleteBillCommand.Parameters.AddWithValue("@BillId", Convert.ToInt32(txtBillId.Text));
                    deleteBillCommand.ExecuteNonQuery();

                    MessageBox.Show("Xóa hóa đơn thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadBills();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa hóa đơn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xóa trắng các trường
        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            txtBillId.Clear();
            if (cboTable.Items.Count > 0)
                cboTable.SelectedIndex = 0;
            dtpCheckIn.Value = DateTime.Now;
            dtpCheckOut.Value = DateTime.Now;
            chkStatus.Checked = false;
            dgvBillDetails.DataSource = null;
        }

        // Thêm món ăn vào hóa đơn
        private void btnAddFood_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBillId.Text))
            {
                MessageBox.Show("Vui lòng chọn hoặc tạo hóa đơn trước", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Kiểm tra xem món đã có trong bill chưa
                    string checkQuery = @"SELECT id, count FROM BillInfo 
                                        WHERE idBill = @BillId AND idFood = @FoodId";

                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@BillId", Convert.ToInt32(txtBillId.Text));
                    checkCommand.Parameters.AddWithValue("@FoodId", cboFood.SelectedValue);

                    object existingId = checkCommand.ExecuteScalar();

                    if (existingId != null) // Món ăn đã tồn tại trong bill
                    {
                        // Cập nhật số lượng
                        string updateQuery = @"UPDATE BillInfo
                                            SET count = count + @Count
                                            WHERE id = @Id";

                        SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                        updateCommand.Parameters.AddWithValue("@Count", numQuantity.Value);
                        updateCommand.Parameters.AddWithValue("@Id", existingId);
                        updateCommand.ExecuteNonQuery();

                        MessageBox.Show("Cập nhật số lượng món ăn thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else // Món ăn chưa tồn tại trong bill
                    {
                        // Thêm mới
                        string insertQuery = @"INSERT INTO BillInfo (idBill, idFood, count)
                                            VALUES (@BillId, @FoodId, @Count)";

                        SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                        insertCommand.Parameters.AddWithValue("@BillId", Convert.ToInt32(txtBillId.Text));
                        insertCommand.Parameters.AddWithValue("@FoodId", cboFood.SelectedValue);
                        insertCommand.Parameters.AddWithValue("@Count", numQuantity.Value);
                        insertCommand.ExecuteNonQuery();

                        MessageBox.Show("Thêm món ăn vào hóa đơn thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    // Làm mới danh sách chi tiết hóa đơn
                    LoadBillDetails(Convert.ToInt32(txtBillId.Text));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm món ăn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Cập nhật món ăn trong hóa đơn
        private void btnUpdateFood_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBillId.Text) || dgvBillDetails.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn món ăn cần cập nhật", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                int billInfoId = Convert.ToInt32(dgvBillDetails.SelectedRows[0].Cells[0].Value);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    // Cập nhật chi tiết hóa đơn
                    string query = @"UPDATE BillInfo
                                    SET idFood = @FoodId,
                                        count = @Count
                                    WHERE id = @BillInfoId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@BillInfoId", billInfoId);
                    command.Parameters.AddWithValue("@FoodId", cboFood.SelectedValue);
                    command.Parameters.AddWithValue("@Count", numQuantity.Value);
                    command.ExecuteNonQuery();

                    MessageBox.Show("Cập nhật món ăn thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadBillDetails(Convert.ToInt32(txtBillId.Text));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật món ăn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnDeleteFood_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBillId.Text) || dgvBillDetails.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn món ăn cần xóa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Xác nhận xóa
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa món ăn này khỏi hóa đơn không?",
                                                  "Xác nhận xóa",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);
            if (result == DialogResult.No)
                return;

            try
            {
                int billInfoId = Convert.ToInt32(dgvBillDetails.SelectedRows[0].Cells[0].Value);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Xóa chi tiết hóa đơn
                    string query = "DELETE FROM BillInfo WHERE id = @BillInfoId";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@BillInfoId", billInfoId);
                    command.ExecuteNonQuery();

                    MessageBox.Show("Xóa món ăn khỏi hóa đơn thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadBillDetails(Convert.ToInt32(txtBillId.Text));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa món ăn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}