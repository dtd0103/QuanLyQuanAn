﻿using QuanLyQuanAn.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanAn.DAO
{
    public class CategoryDAO
    {
        private static CategoryDAO instance;
        public static CategoryDAO Instance
        {
            get { if (instance == null) instance = new CategoryDAO(); return CategoryDAO.instance; }
            set { CategoryDAO.instance = value; }
        }
        private CategoryDAO() { }

        public List<Category> GetListCategory() { 
            List<Category> list = new List<Category>();

            string query = "Select * From F_category";

            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow row in data.Rows)
            {
                Category category = new Category(row);
                list.Add(category);
            }
            return list;
        }
    }
}
