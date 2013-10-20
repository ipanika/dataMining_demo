﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using Microsoft.AnalysisServices;

namespace dataMining_demo
{
    public partial class Form2 : Form
    {
        //Database db = dataMining_demo.Form1.db;
        //Server svr = dataMining_demo.Form1.svr;
         Server svr = new Server();
         Database db = new Database();


        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            svr.Connect("localhost");

            if ((svr != null) && (svr.Connected))
            {
                db = svr.Databases.FindByName("demo_DM");
                
                //db = svr.Databases.Add("demo_DM");
                db.Update();
            }

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
                checkedListBox1.DataSource = dt;
                checkedListBox1.DisplayMember = "COLUMN_NAME";
            }

            
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
                checkedListBox1.SetItemChecked(i, true);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var colForDSV = new List<string>();
            int i = 0;
            
            // создание массива с именами столбцов, выбранных для представления
            foreach (object obj in checkedListBox1.CheckedItems)
            {
                DataRowView drv;
                drv = (System.Data.DataRowView) obj;
                colForDSV.Add(drv["COLUMN_NAME"].ToString());
                i += 1;
            }

            CreateDataAccessObjects(db, colForDSV);
        }

        
        void CreateDataAccessObjects(Database db, List<string> columnNames )
        {
            // Create a relational data source
            // by specifying the name and the id
            RelationalDataSource ds = new RelationalDataSource("demo_ds", Utils.GetSyntacticallyValidID("demo_ds", typeof(Database)));
            ds.ConnectionString = "Provider=SQLNCLI11.1;Data Source=localhost;Integrated Security=SSPI;Initial Catalog=demo_source";
            
            db.DataSources.Add(ds);

            
            // Create connection to datasource cto extract schema to a dataset
            DataSet dset = new DataSet();
            SqlConnection cn = new SqlConnection("Data Source=localhost; Initial Catalog=demo_source; Integrated Security=true");

            string argsForQuery = " ";
            string strQuery = "";

            int i;
            for (i = 0; i < columnNames.Count-1; i++)
            {
                argsForQuery += "["+columnNames[i] + "], ";
            }

            argsForQuery += "[" + columnNames[i] + "] ";

            MessageBox.Show(argsForQuery);
            
            strQuery = "SELECT "+ argsForQuery + " FROM SourceData$";

            // Create data adapters from database tables and load schemas
            SqlDataAdapter daCustomers = new SqlDataAdapter(strQuery, cn);
            daCustomers.FillSchema(dset, SchemaType.Mapped, "SourceData$");

            // Create the DSV, ad the dataset and add to the database
            DataSourceView dsv = new DataSourceView("demo_dsv", "demo_dsv");
            dsv.DataSourceID = "demo_ds";
            dsv.Schema = dset.Clone();
            db.DataSourceViews.Add(dsv);
            db.DataSourceViews[0].ID = dsv.ID;

            // Update the database to create the objects on the server
            db.Update(UpdateOptions.ExpandFull);
        }

        
    }
}
