using QuanLyQuanAn.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanAn.DAO
{
    public class BillDAO
    {
        private static BillDAO instance;

        public static BillDAO Instance
        {
            get { if (instance == null) instance = new BillDAO(); return BillDAO.instance; }
            private set { BillDAO.instance = value; }
        }

        private BillDAO() { }

        /// <summary>
        /// Thanh cong bill ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public int GetUnCheckBillByTableID(int id)
        {
            //DataTable data = DataProvider.Instance.ExecuteQuery("SELECT * FROM Bill WHERE idTable = @id AND status = 0", new object[] { id });
            DataTable data = DataProvider.Instance.ExecuteQuery("SELECT * FROM Bill WHERE idTable = " + id +" AND status = 0");
            if (data.Rows.Count > 0)
            {
                Bill bill = new Bill(data.Rows[0]);
                return bill.ID;
            }
            return -1;
        }

        public void insertBill(int id)
        {
            DataProvider.Instance.ExecuteNonQuery("exec USP_insertBill @idTable", new object[] { id });
        }
        public int getMaxIDBill()
        {
            try
            {
                return (int)DataProvider.Instance.ExecuteScalar("Select MAX(id) from Bill");
            }catch (Exception ex)
            {
                return 1;
            }
        }

        public void checkout(int id)
        {
            string query = "update bill set status = 1 where id = "+ id;
            DataProvider.Instance.ExecuteNonQuery(query);
        }  

    }
}
