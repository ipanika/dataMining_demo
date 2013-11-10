﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using DataTable = System.Data.DataTable;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Office.Interop.Excel;
using Microsoft.AnalysisServices;

namespace dataMining_demo
{
    public partial class FormMain : Form
    {
        // member variable -- the Analysis Services server connection
        public static Server svr;
        public static Database db;
        public static string modelName;
                
        public FormMain()
        {
            InitializeComponent();

            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            // Create SSAS-server object and connect
            svr = new Server();
            svr.Connect("localhost");

            db = CreateDatabase();



            SqlConnection cn = new SqlConnection("Data Source=localhost; Initial Catalog=demo_dm; Integrated Security=true");
            DataTable dt = new DataTable();

            // Create data adapters from database tables and load schemas
            SqlDataAdapter sqlDA = new SqlDataAdapter("SELECT [name] FROM [data_source_views]", cn);
            sqlDA.Fill(dt);

            comboBox1.DataSource = dt;
            comboBox1.DisplayMember = "name";

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string modelName = comboBox3.Text;
            string strName = comboBox2.Text;
            
            MiningStructure ms = db.MiningStructures.FindByName(strName);
            MiningModel mm = ms.MiningModels.FindByName(modelName);
            mm.Process();
            mm.Update();
            //ProcessDatabase(db);
 
            //db = svr.Databases["demo_DM"];
            //db.Update(UpdateOptions.ExpandFull);
            //SetModelPermissions(db, db.MiningStructures[0].MiningModels[0]);
            
            //svr.Disconnect();

            MessageBox.Show("Анализ данных успешно завершен.");           
             
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FormDataSourceView f2 = new FormDataSourceView();

            f2.Show();
        }

        Database CreateDatabase()
        {
            // Create a database and set the properties
            Database db = null; 
            if ((svr != null) && (svr.Connected))
            {
                db = svr.Databases.FindByName("demo_DM");
                if (db == null)
                {
                    db = svr.Databases.Add("demo_DM");
                    db.Update();
                }

            }


            return db;
        }

        void ProcessDatabase(Database db)
        {
            //Trace t;
            //TraceEvent e;

            // create the trace object to trace progress reports
            // and add the column containing the progress description
            //t = svr.Traces.Add();
            //e = t.Events.Add(TraceEventClass.ProgressReportCurrent);
            //e.Columns.Add(TraceColumn.TextData);
            //t.Update();

            // Add the handler for the trace event
            //t.OnEvent += new TraceEventHandler(ProgressReportHandler);
            try
            {
                // start the trace, process of the database, then stop it
                //t.Start();
                db.Process(ProcessType.ProcessFull);
                //t.Stop();


            }
            catch (System.Exception /*ex*/)
            {
                MessageBox.Show("Произошла ошибка обработки БД");
            }

        }

        void ProgressReportHandler(object sender, TraceEventArgs e)
        {
            Console.WriteLine(e[TraceColumn.TextData]);
        }

        void SetModelPermissions(Database db, MiningModel mm)
        {
            // Create a new role and add members

            Role r = db.Roles.FindByName("ModelReader");
            if (r == null)
            {
                r = new Role("ModelReader", "ModelReader");

                String userName = SystemInformation.UserName;
                String PCName = Environment.MachineName;

                r.Members.Add(new RoleMember(PCName + "\\" + userName));

                // Add the role to the database and update
                db.Roles.Add(r);
                r.Update();

                // Create a permission object referring the role
                MiningModelPermission mmp = new MiningModelPermission();
                mmp.Name = "ModelReader";
                mmp.ID = "ModelReader";
                mmp.RoleID = "ModelReader";

                // Assign access rights to the permission
                mmp.Read = ReadAccess.Allowed;
                mmp.AllowBrowsing = true;
                mmp.AllowDrillThrough = true;
                mmp.ReadDefinition = ReadDefinitionAccess.Allowed;


                // Add permission to the model and update
                mm.MiningModelPermissions.Add(mmp);
                mmp.Update();
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FormMiningStructure f3 = new FormMiningStructure();
            f3.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FormMiningModel f4 = new FormMiningModel();
            f4.Show();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.DataSource = null;
            comboBox2.Text = null;

            SqlConnection cn = new SqlConnection("Data Source=localhost; Initial Catalog=demo_dm; Integrated Security=true");
            DataTable dt = new DataTable();
            
            String dsvName = comboBox1.Text;

            if (dsvName != "")
            {
                string strSel = "SELECT dbo.selections.name FROM [dbo].selections JOIN dbo.data_source_views" +
                                    " ON dbo.data_source_views.id_dsv = dbo.selections.id_dsv" +
                                    " and dbo.data_source_views.name = '" + dsvName +"'";

                SqlDataAdapter sqlDA = new SqlDataAdapter(strSel, cn);
                sqlDA.Fill(dt);

                if (dt != null)
                {
                    comboBox2.DataSource = dt;
                    comboBox2.DisplayMember = "name";
                }
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox3.DataSource = null;
            comboBox3.Text = null;

            SqlConnection cn = new SqlConnection("Data Source=localhost; Initial Catalog=demo_dm; Integrated Security=true");
            DataTable dt = new DataTable();

            String selName = comboBox2.Text;
            if (selName != "")
            {
                // Create data adapters from database tables and load schemas
                SqlDataAdapter sqlDA = new SqlDataAdapter("SELECT structures.name FROM [structures] JOIN selections ON " +
                                                            " selections.name = '" + selName + "' AND " +
                                                            " selections.id_selection = structures.id_selection", cn);
                sqlDA.Fill(dt);
                if (dt != null)
                {
                    comboBox3.DataSource = dt;
                    comboBox3.DisplayMember = "name";
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {

            FormTreeView f5 = new FormTreeView();
            f5.Show();
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            SqlConnection cn = new SqlConnection("Data Source=localhost; Initial Catalog=demo_dm; Integrated Security=true");
            DataTable dt = new DataTable();

            // Create data adapters from database tables and load schemas
            SqlDataAdapter sqlDA = new SqlDataAdapter("SELECT [dsv_name] FROM [demo_dsv]", cn);
            sqlDA.Fill(dt);

            comboBox1.DataSource = dt;
            comboBox1.DisplayMember = "dsv_name";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            FormMetaData f6 = new FormMetaData();
            f6.Show();

        }

        private void button7_Click(object sender, EventArgs e)
        {
            FormDrillThrough f7 = new FormDrillThrough();
            f7.Show();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            modelName = comboBox3.Text;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            FormSelection f8 = new FormSelection();
            f8.Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            FormAlgorithmVariants f9 = new FormAlgorithmVariants();
            f9.Show();

        }

    }
}
