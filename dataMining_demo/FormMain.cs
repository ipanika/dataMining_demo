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
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace dataMining_demo
{
    public partial class FormMain : Form
    {
        public static Microsoft.AnalysisServices.Server svr;
        public static Microsoft.AnalysisServices.Database db;
        public static string modelName;

        public static int taskType = 1; // тип задачи по умолчанию: кластеризация

        public static string app_dataSource = "localhost"; // сервер БД приложения
        public static string app_initCatalog = "demo_dm"; // имя БД приложения
        public static string app_connectionString = "Data Source=" + app_dataSource + "; Initial Catalog=" + app_initCatalog +
                                      "; Integrated Security=SSPI";

        public static string as_dataSource = "localhost"; // сервер службы SSAS
        public static string as_initCatalog = "SSAS_DM"; // имя БД службы SSAS
        public static string as_dataSourceName = "dataSource_DM"; // имя источника данных
        public static string as_connectionString = "Provider=MSOLAP; Integrated Security=SSPI; Data Source = " + as_dataSource + "; Initial Catalog = " + as_initCatalog;

        public static string dw_dataSource = "localhost"; // сервер ХД
        public static string dw_initCatalog = "DW"; // имя ХД
        public static string dw_connectionString = "Data Source="+ dw_dataSource +"; Initial Catalog="+dw_initCatalog +"; Integrated Security=SSPI";

                
        public FormMain()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            svr = new Microsoft.AnalysisServices.Server();
            svr.Connect(as_dataSource);

            db = CreateDatabase();
            try
            {
                checkAppDB();

                selectDataSourceViews();
            }
            catch(Exception e1)
            {
                MessageBox.Show(e1.Message);
                FormConnection f = new FormConnection();
                f.ShowDialog();
                checkAppDB();

                selectDataSourceViews();
            }

            taskType = 1;
            кластеризацииToolStripMenuItem.Checked = true;
            прогнозированияToolStripMenuItem.Checked = false;
            
        }

        // функция проверки существования базы данных приложения
        private void checkAppDB()
        {
            //try
            {
                SqlConnection cn = new SqlConnection(app_connectionString);
                cn.Open();
            }
            // в случае провала - запустить скрипт создания БД
          //  catch
            {
             //   MessageBox.Show("Ошибка соединения с БД приложения.");
                //string sqlConnectionString = "Data Source="+ app_dataSource + "; Initial Catalog = master; Integrated Security=SSPI";
                //FileInfo file = new FileInfo("C:\\Users\\ipanika\\Documents\\Visual Studio 2010\\Projects\\dataMining_demo\\dataMining_demo\\create_db.sql");
                //string script = file.OpenText().ReadToEnd();
                //SqlConnection conn = new SqlConnection(sqlConnectionString);
                //Microsoft.SqlServer.Management.Smo.Server sqlServer = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection(conn));
                //sql1Server.ConnectionContext.ExecuteNonQuery(script);

                //file = new FileInfo("C:\\Users\\ipanika\\Documents\\Visual Studio 2010\\Projects\\dataMining_demo\\dataMining_demo\\fill_db.sql");
                //script = file.OpenText().ReadToEnd();
                //sqlServer.ConnectionContext.ExecuteNonQuery(script);
            }
        }

        private void selectDataSourceViews()
        {
            try
            {
                comboBox1.DataSource = null;

                SqlConnection cn = new SqlConnection(app_connectionString);
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
                if (rowsConunt != itemsCount)
                {
                    comboBox1.DataSource = dt;
                    comboBox1.DisplayMember = "name";
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
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


                // создание соединения с сервером и команды для отправки dmx-запроса
                AdomdConnection adomdCn = new AdomdConnection();
                adomdCn.ConnectionString = FormMain.as_connectionString;
                adomdCn.Open();
                AdomdCommand adomdCmd = adomdCn.CreateCommand();

                // удаление данных ранее обработанной структуры
                adomdCmd.CommandText = "DELETE FROM [" + strName + "]";

                adomdCmd.Execute();

                dmxQuery += "INSERT INTO [" + strName + "] (";

                // получение списка столбцов текущей структуры

                AdomdConnection cn = new AdomdConnection();
                cn.ConnectionString = FormMain.as_connectionString;
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

        Microsoft.AnalysisServices.Database CreateDatabase()
        {
            // Create a database and set the properties
            Microsoft.AnalysisServices.Database db = null; 
            if ((svr != null) && (svr.Connected))
            {
                db = svr.Databases.FindByName(FormMain.as_initCatalog);
                if (db == null)
                {
                    db = svr.Databases.Add(FormMain.as_initCatalog);
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
            selectSelections();
        }

        private void selectSelections()
        {
            comboBox2.DataSource = null;
            comboBox2.Text = null;

            SqlConnection cn = new SqlConnection(FormMain.app_connectionString);
            DataTable dt = new DataTable();

            String dsvName = comboBox1.Text;

            if (dsvName != "")
            {
                string strSel = "SELECT dbo.selections.name FROM [dbo].selections JOIN dbo.data_source_views" +
                                    " ON dbo.data_source_views.id_dsv = dbo.selections.id_dsv" +
                                    " and dbo.data_source_views.name = '" + dsvName + "'";

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
            selectStructures();
        }

        private void selectStructures()
        {
            comboBox3.DataSource = null;
            comboBox3.Text = null;

            SqlConnection cn = new SqlConnection(FormMain.app_connectionString);
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
            selectModels();
        }

        private void selectModels()
        {
            comboBox4.DataSource = null;
            comboBox4.Text = null;

            SqlConnection cn = new SqlConnection(FormMain.app_connectionString);
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
                ds = new RelationalDataSource("demo_ds_origin", Utils.GetSyntacticallyValidID("demo_ds_origin", typeof(Microsoft.AnalysisServices.Database)));
                ds.ConnectionString = "Data Source=localhost;Integrated Security=SSPI;Initial Catalog=" + app_initCatalog;

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

        private void представлениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormDataSourceView f = new FormDataSourceView();
            f.ShowDialog();
            selectDataSourceViews();
        }

        private void выборкуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSelection f = new FormSelection();
            f.ShowDialog();
            selectSelections();
            
        }

       
        private void структуруToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FormMiningStructure f = new FormMiningStructure();
            f.ShowDialog();
            selectStructures();
        }

        private void модельToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormMiningModel f = new FormMiningModel();
            f.ShowDialog();
            selectModels();
        }

        private void вариантАлгоритмаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAlgorithmVariants f = new FormAlgorithmVariants();
            f.Show();
        }
        
        private void схемаОбъектовToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormTreeView f = new FormTreeView();
            f.Show();
        }

        private void обозревательAnalysisServicesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormViewer f = new FormViewer();
            f.Show();
        }

        private void метаданныеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormMetaData f6 = new FormMetaData();
            f6.Show();
        }

        private void настройкаСоединенияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormConnection f = new FormConnection();
            f.Show();
        }


        

        

      
    }
}