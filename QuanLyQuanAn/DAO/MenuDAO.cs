﻿using QuanLyQuanAn.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanAn.DAO
{
    public class MenuDAO
    {
        private static MenuDAO instance;
        public static MenuDAO Instance {  
            get { if (instance == null) instance = new MenuDAO(); return MenuDAO.instance; }
            set { MenuDAO.instance = value; }
        }

        private MenuDAO() { }

        public List<Menu> GetListMenuByTable(int id)
        {
            List<Menu> listMenu = new List<Menu> ();
            string query = "select f.name, bi.count, f.price, f.price*bi.count as totalPrice from BillInfo as bi, Bill as b, Food as f where bi.idBill = b.id and bi.idFood = f.id and b.status = 0 and b.idTable = " + id;
            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow row in data.Rows) {
                Menu menu = new Menu(row);
                listMenu.Add(menu);
            }

            return listMenu;
        }
    }
}
