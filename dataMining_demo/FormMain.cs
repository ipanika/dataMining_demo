using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.AnalysisServices.AdomdClient;
using Microsoft.AnalysisServices;

namespace dataMining_demo
{
    public partial class FormMain : Form
    {
        public static Server svr;
        public static Database db;
        public static string modelName;

        public static int taskType = 1; // тип задачи по умолчанию: кластеризация
                
        public FormMain()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            svr = new Server();
            svr.Connect("localhost");

            db = CreateDatabase();

            selectDataSourceViews();

            taskType = 1;
            кластеризацииToolStripMenuItem.Checked = true;
            прогнозированияToolStripMenuItem.Checked = false;
            
        }

        private void selectDataSourceViews()
        {

            comboBox1.DataSource = null;

            SqlConnection cn = new SqlConnection("Data Source=localhost; Initial Catalog=DM; Integrated Security=true");
            DataTable dt = new DataTable();

            string sqlQuery = "SELECT [data_source_views].[name] FROM [data_source_views] INNER JOIN relations " + 
                                " ON relations.id_dsv = data_source_views.id_dsv INNER JOIN tasks " +
                                " ON tasks.id_task = relations.id_task WHERE tasks.task_type = " + FormMain.taskType.ToString();

            // Create data adapters from database tables and load schemas
            SqlDataAdapter sqlDA = new SqlDataAdapter(sqlQuery, cn);
            sqlDA.Fill(dt);
            
            int rowsConunt = dt.Rows.Count;
            int itemsCount = comboBox1.Items.Count;
            // если появились новые данные - обновить список представлений
            if ( rowsConunt != itemsCount)
            {
                comboBox1.DataSource = dt;
                comboBox1.DisplayMember = "name";
            }
        }

        
        
        private void button1_Click(object sender, EventArgs e)
        {
            // проверка существования источника данных demo_dm
            checkDataSource();

            string strName = comboBox3.Text;

            try
            {
                string dmxQuery = "";

                dmxQuery += "INSERT INTO [" + strName + "] (";

                // получение списка столбцов текущей структуры

                AdomdConnection cn = new AdomdConnection();
                cn.ConnectionString = "Data Source = localhost; Initial Catalog = SSAS_DM";
                cn.Open();
                AdomdCommand cmd = cn.CreateCommand();
                //string modelName = FormMain.modelName;// MainForm.comboBox3.Text;
                cmd.CommandText = "SELECT COLUMN_NAME, DATA_TYPE FROM [$system].[DMSCHEMA_MINING_COLUMNS] where model_name ='" + modelName + "'";

                List<string> _sideList = new List<string>();
                List<string> _typeList = new List<string>();
                try
                {
                    AdomdDataReader reader = cmd.ExecuteReader();
                    

                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (i % 2 == 0)
                                _sideList.Add(reader.GetValue(i).ToString());
                            else if (i % 2 == 1)
                            {
                                switch (reader.GetValue(i).ToString())
                                {
                                    case ("5"):
                                        _typeList.Add("real");
                                        break;
                                    case ("7"):
                                        _typeList.Add("date");
                                        break;
                                    case ("20"):
                                        _typeList.Add("bigint");
                                        break;
                                    case ("130"):
                                        _typeList.Add("nchar");
                                        break;
                                    default:
                                        _typeList.Add("nchar");
                                        break;
                                }
                            }
                        }

                    }

                   // comboBox2.DataSource = _sideList;
                }
                catch (Exception e1)
                {
                    MessageBox.Show(e1.Message);
                }

                // строка, содержащая имена столбцов структуры (модели)
                string colNames = "";
                string colNames2 = "";
                for (int i = 0; i < _sideList.Count; i++)
                {
                    colNames += " [" + _sideList[i] + "],";
                    colNames2 += " cast([" + _sideList[i] + "] as " + _typeList[i] + ") as [" + _sideList[i] + "],";
                }

                colNames = colNames.Substring(0, colNames.Length - 1);
                colNames2 = colNames2.Substring(0, colNames2.Length - 1);
                
               // string mName = comboBox4.Text;
                dmxQuery += colNames + ")" +
                        " openquery ([demo_ds_origin], ' SELECT " + colNames2 +
                        " FROM (select selection_content.id_row, column_value, " + 
                        " dsv_columns.column_name from selection_content INNER JOIN " + 
	                    " selection_rows ON selection_content.id_row = selection_rows.id_row INNER JOIN " +
	                    " selections ON selection_rows.id_selection = selections.id_selection INNER JOIN " +
	                    " dsv_columns ON dsv_columns.id_column = selection_content.id_column " +
                        " INNER JOIN structures ON structures.id_selection = selections.id_selection " +
		                " INNER JOIN models ON models.id_structure = structures.id_structure "+
                        " AND models.name =  ''" + modelName + "'') p" +
                        " PIVOT ( max(column_value) FOR column_name IN (" + colNames + ") ) AS pvt')";


                // создание соединения с сервером и команды для отправки dmx-запроса
                AdomdConnection adomdCn = new AdomdConnection();
                adomdCn.ConnectionString = "Data Source = localhost; Initial Catalog = SSAS_DM";
                adomdCn.Open();

                AdomdCommand adomdCmd = adomdCn.CreateCommand();
                adomdCmd.CommandText = dmxQuery;
            
                adomdCmd.Execute();

                MessageBox.Show("Анализ данных успешно завершен.");    
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message);
            }
       
             
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FormDataSourceView f2 = new FormDataSourceView();

            f2.ShowDialog();
        }

        Database CreateDatabase()
        {
            // Create a database and set the properties
            Database db = null; 
            if ((svr != null) && (svr.Connected))
            {
                db = svr.Databases.FindByName("SSAS_DM");
                if (db == null)
                {
                    db = svr.Databases.Add("SSAS_DM");
                    db.Update();
                }

            }

            return db;
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

            SqlConnection cn = new SqlConnection("Data Source=localhost; Initial Catalog=DM; Integrated Security=true");
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

            SqlConnection cn = new SqlConnection("Data Source=localhost; Initial Catalog=DM; Integrated Security=true");
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

            //comboBox1.Text = null;
            //selectDataSourceViews();
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

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox4.DataSource = null;
            comboBox4.Text = null;

            SqlConnection cn = new SqlConnection("Data Source=localhost; Initial Catalog=DM; Integrated Security=true");
            DataTable dt = new DataTable();

            string strName = comboBox3.Text;
            if (strName != "")
            {
                // Create data adapters from database tables and load schemas
                SqlDataAdapter sqlDA = new SqlDataAdapter("SELECT models.name FROM [models] JOIN structures ON " +
                                                            " structures.name = '" + strName + "' AND " +
                                                            " structures.id_structure = models.id_structure", cn);
                sqlDA.Fill(dt);
                if (dt != null)
                {
                    comboBox4.DataSource = dt;
                    comboBox4.DisplayMember = "name";
                }
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            modelName = comboBox4.Text;
        }

        private void checkDataSource()
        {
            DataSource ds = db.DataSources.FindByName("demo_ds_origin");

            if (ds == null)
            {
                ds = new RelationalDataSource("demo_ds_origin", Utils.GetSyntacticallyValidID("demo_ds_origin", typeof(Database)));
                ds.ConnectionString = "Provider=SQLNCLI11.1;Data Source=localhost;Integrated Security=SSPI;Initial Catalog=DW";

                db.DataSources.Add(ds);

                // Update the database to create the objects on the server
                db.Update(UpdateOptions.ExpandFull);
            }
        }

        
        private void кластеризацииToolStripMenuItem_Click(object sender, EventArgs e)
        {
            taskType = 1; // кластеризация
            кластеризацииToolStripMenuItem.Checked = true;
            прогнозированияToolStripMenuItem.Checked = false;
            selectDataSourceViews();
        }

        // установка типа решаемой задачи
        private void прогнозированияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            taskType = 2; // прогнозирование
            прогнозированияToolStripMenuItem.Checked = true;
            кластеризацииToolStripMenuItem.Checked = false;
            selectDataSourceViews();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            FormViewer f = new FormViewer();

            f.Show();
        }

    }
}