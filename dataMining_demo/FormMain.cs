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
        // объявление глобальных переменных:
        public static Server svr; // сервер SSAS
        public static Database db; // БД сервера SSAS
        public static string modelName; // имя выбранной модели (combobox4)

        public static int taskType = 1; // тип задачи по умолчанию: кластеризация

        public static string app_dataSource = "localhost"; // сервер БД приложения
        public static string app_initCatalog = "DM"; // имя БД приложения
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

            // создание экземпляра сервера SSAS
            svr = new  Server();
            svr.Connect(as_dataSource);
            
            db = checkAnalysisServicesDB();
            try
            {
                /*
                 * проверка соединений с БД приложения и БД SSAS,
                 * загрузка списка доступных представлений данных
                 */
                checkAppDB();
                checkDataSource();
                selectDataSourceViews();
            }
            catch(Exception e1)
            {
                MessageBox.Show(e1.Message);
                FormConnection f = new FormConnection();
                f.ShowDialog();
                try
                {
                    checkAppDB();
                    checkDataSource();
                    selectDataSourceViews();
                }
                catch (Exception e2)
                {
                    MessageBox.Show(e2.Message);
                }
            }

            // выставление флага решаемой задачи, по умолчанию - кластеризация
            taskType = 1;
            кластеризацииToolStripMenuItem.Checked = true;
            прогнозированияToolStripMenuItem.Checked = false;
            
        }

        // функция подключения к базе данных приложения (DM)
        private void checkAppDB()
        {
            SqlConnection cn = new SqlConnection(app_connectionString);
            cn.Open();
        }


        // функция получения списка существующих представлений данных, в результате 
        private void selectDataSourceViews()
        {
            try
            {
                comboBox1.DataSource = null;
                comboBox2.DataSource = null;
                comboBox3.DataSource = null;
                comboBox4.DataSource = null;

                SqlConnection cn = new SqlConnection(app_connectionString);
                DataTable dt = new DataTable();

                string sqlQuery = "SELECT [data_source_views].[name] FROM [data_source_views] INNER JOIN tasks " +
                                    " ON tasks.id_task = data_source_views.id_task WHERE tasks.task_type = " + FormMain.taskType.ToString();

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

        // обработчик кнопки "Обработать модель"
        private void button1_Click(object sender, EventArgs e)
        {
            // проверка существования источника данных 
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

                // удаление данных ранее обработанной структуры, 
                // тк иначе не возможо обрботать структуру при уже обработанных моделях
                adomdCmd.CommandText = "DELETE FROM [" + strName + "]";

                adomdCmd.Execute();

                // dmx-запрос для обучения структуры
                dmxQuery += "INSERT INTO [" + strName + "] (";

                // получение списка столбцов и типов данных текущей структуры 
                // для последующего формирования pivot-запроса
                AdomdConnection cn = new AdomdConnection();
                cn.ConnectionString = FormMain.as_connectionString;
                cn.Open();
                AdomdCommand cmd = cn.CreateCommand();
                
                cmd.CommandText = "SELECT COLUMN_NAME, DATA_TYPE FROM [$system].[DMSCHEMA_MINING_COLUMNS] where model_name ='" + modelName + "'";
                /*
                 * В столбце DATA_TYPE содержится код типа данных
                 * 5 - real,
                 * 7 - datetime
                 * 20 - integer
                 * 130 - string
                 */
                // список названий столбцов структуры
                List<string> columnNameList = new List<string>(); 
                // список типов данных структуры
                List<string> dataTypeList = new List<string>();

                try
                {
                    AdomdDataReader reader = cmd.ExecuteReader();
                    
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (i % 2 == 0)
                                columnNameList.Add(reader.GetValue(i).ToString());
                            else if (i % 2 == 1)
                            {
                                // декодирование типов данных структуры, хранящихся в столбце DATA_TYPE
                                switch (reader.GetValue(i).ToString())
                                {
                                    case ("5"):
                                        dataTypeList.Add("real");
                                        break;
                                    case ("7"):
                                        dataTypeList.Add("date");
                                        break;
                                    case ("20"):
                                        dataTypeList.Add("bigint");
                                        break;
                                    case ("130"):
                                        dataTypeList.Add("nchar");
                                        break;
                                    default:
                                        dataTypeList.Add("nchar");
                                        break;
                                }
                            }
                        }

                    }
                }
                catch (Exception e1)
                {
                    MessageBox.Show(e1.Message);
                }

                // строка, содержащая имена столбцов структуры (модели)
                string colNames = "";
                string colNames2 = "";
                for (int i = 0; i < columnNameList.Count; i++)
                {
                    colNames += " [" + columnNameList[i] + "],";
                    /*
                     * приведение к необходимому типу данных значений для обучения структуры, 
                     * хранящихся в БД прилоежния (DM)
                    */
                    colNames2 += " cast([" + columnNameList[i] + "] as " + dataTypeList[i] + ") as [" + columnNameList[i] + "],";
                }

                colNames = colNames.Substring(0, colNames.Length - 1);
                colNames2 = colNames2.Substring(0, colNames2.Length - 1);
                
                // DMX-запрос с PIVOT-запросом к БД приложения, используемой как истоничк данных для обучения
                dmxQuery += colNames + ")" +
                        " openquery ([" + as_dataSourceName + "], ' SELECT " + colNames2 +
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

        /*
         * функция проверки существования БД службе SQL Server Analysis Services
         * Если БД отсутствует, то она создается.
        */
         Database checkAnalysisServicesDB()
        {
            // Create a database and set the properties
             Database db = null; 
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

        // обработчик события выбора представления источника данных в выпадающем списке:
        // выбираются доступные выборик данных
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectSelections();
        }

        /*
         * функция формирования списка доступных выборок данных соответствующего представления данных
         */
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

        // обработчик события выбора выборки данных в выпадающем списке:
        // выбираются доступные структуры ИАД
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectStructures();
        }
        /*
         * функция формирования списка доступных структур ИАД соответствующей выборки данных
         */
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

        // обработчик события выбора сттруктуры данных в выпадающем списке:
        // выбираются доступные модели ИАД
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectModels();
        }

        /*
         * функция формирования списка доступных моделей ИАД соответствующей структуры данных
         */
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

        // обработчик события выбора модели данных в выпадающем списке:
        // сохраняется имя выбранной модели
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            modelName = comboBox4.Text;
        }

        /*
         * функция проверки существования источника данных в БД SSAS 
         * если источник данных отсутствует, то он создается
         */
        private void checkDataSource()
        {
            DataSource ds = db.DataSources.FindByName(as_dataSourceName);

            if (ds == null)
            {
                ds = new RelationalDataSource(as_dataSourceName, Utils.GetSyntacticallyValidID(as_dataSourceName, typeof( Database)));
                ds.ConnectionString = "Data Source="+ app_dataSource +";Integrated Security=SSPI;Initial Catalog=" + app_initCatalog;

                db.DataSources.Add(ds);

                // Update the database to create the objects on the server
                db.Update(UpdateOptions.ExpandFull);
            }
        }

        // установка типа решаемой задачи
        private void кластеризацииToolStripMenuItem_Click(object sender, EventArgs e)
        {
            taskType = 1; // кластеризация
            кластеризацииToolStripMenuItem.Checked = true;
            прогнозированияToolStripMenuItem.Checked = false;
            selectDataSourceViews();
            метаданныеToolStripMenuItem.Enabled = true;
        }

        // установка типа решаемой задачи
        private void прогнозированияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            taskType = 2; // прогнозирование
            прогнозированияToolStripMenuItem.Checked = true;
            кластеризацииToolStripMenuItem.Checked = false;
            selectDataSourceViews();
            метаданныеToolStripMenuItem.Enabled = false;
        }

       
        // ОБРАБОТЧИКИ ЭЛЕМЕНТОВ МЕНЮ:

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