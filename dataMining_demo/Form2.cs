﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace dataMining_demo
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            // создать соединение с БД
            DataSet dset = new DataSet();
            DataTable dt = new DataTable();
            SqlConnection cn = new SqlConnection("Data Source=localhost; Initial Catalog=demo_source; Integrated Security=true");
            if (cn.State == ConnectionState.Closed)
            {
                cn.Open();
            }
            SqlDataAdapter sqlDA = new SqlDataAdapter("select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_CATALOG = 'demo_source' AND TABLE_NAME = 'SourceData$'", cn);
            sqlDA.FillSchema(dset, SchemaType.Mapped, "SourceData$");
            sqlDA.Fill(dt);

            
            if (dt.Rows.Count > 0)
            {
                //((ListBox)checkedListBox1).DataSource = dset;
                checkedListBox1.DataSource = dt;
                checkedListBox1.DisplayMember = "COLUMN_NAME";
            }

            
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, true);
            }



            // получить список столбцов таблицы SourceData$
            // отобразить их в checkedListBox
        }
    }
}
