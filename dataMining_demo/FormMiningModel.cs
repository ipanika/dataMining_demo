using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.AnalysisServices.AdomdClient;
using System.Data.SqlClient;

namespace dataMining_demo
{
    public partial class FormMiningModel : Form
    {
        //Server svr = new Server();
        //Database db = new Database();
        //MiningStructure ms = new MiningStructure();

        public FormMiningModel()
        {
            InitializeComponent();
        }

        private void MiningModelForm_Load(object sender, EventArgs e)
        {

            selectDataSourceViews();
            selectAlg();

            //// создать соединение с БД
            //SqlConnection cn = new SqlConnection(FormMain.app_connectionString);
            //if (cn.State == ConnectionState.Closed)
            //    cn.Open();

            //string sqlQuery = "SELECT [structures].[name] FROM [structures] INNER JOIN " + 
            //                   " selections ON structures.id_selection = selections.id_selection INNER JOIN " +
            //                   " data_source_views ON data_source_views.id_dsv = selections.id_dsv INNER JOIN" + 
            //                   " relations ON relations.id_dsv = data_source_views.id_dsv INNER JOIN " + 
            //                   " tasks ON tasks.id_task = relations.id_task WHERE tasks.task_type = " + FormMain.taskType;

            //DataTable dt1 = new DataTable();
            //// загрузка имеющихся представлений ИАД
            //SqlDataAdapter sqlDA = new SqlDataAdapter(sqlQuery, cn);
            //sqlDA.Fill(dt1);

            //comboBox1.DataSource = dt1;
            //comboBox1.DisplayMember = "name";

            

        }

        private void selectDataSourceViews()
        {
            try
            {
                comboBox3.DataSource = null;

                SqlConnection cn = new SqlConnection(FormMain.app_connectionString);
                DataTable dt = new DataTable();

                string sqlQuery = "SELECT [data_source_views].[name] FROM [data_source_views] INNER JOIN relations " +
                                    " ON relations.id_dsv = data_source_views.id_dsv INNER JOIN tasks " +
                                    " ON tasks.id_task = relations.id_task WHERE tasks.task_type = " + FormMain.taskType.ToString();

                // Create data adapters from database tables and load schemas
                SqlDataAdapter sqlDA = new SqlDataAdapter(sqlQuery, cn);
                sqlDA.Fill(dt);

                int rowsConunt = dt.Rows.Count;
                int itemsCount = comboBox3.Items.Count;
                // если появились новые данные - обновить список представлений
                if (rowsConunt != itemsCount)
                {
                    comboBox3.DataSource = dt;
                    comboBox3.DisplayMember = "name";
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void selectAlg()
        {
            SqlConnection cn = new SqlConnection(FormMain.app_connectionString);
            if (cn.State == ConnectionState.Closed)
                cn.Open();
            
            string sqlQuery = "SELECT algorithm_variants.name FROM algorithm_variants " +
                        " INNER JOIN algorithms ON algorithm_variants.id_algorithm = algorithms.id_algorithm" +
                        " INNER JOIN tasks ON tasks.task_type = algorithms.task_type WHERE tasks.task_type = " + FormMain.taskType;
            DataTable dt = new DataTable();
            
            SqlDataAdapter sqlDA = new SqlDataAdapter(sqlQuery, cn);
            sqlDA.Fill(dt);

            comboBox2.DataSource = dt;
            comboBox2.DisplayMember = "name";
        }

        private void button1_Click(object sender, EventArgs e)
        {

            string modelName = textBox1.Text;
            string strName = comboBox1.Text;
            string algVarName = comboBox2.Text;

            string dmxQuery = "";
            string sqlQuery = "";

            
            SqlConnection cn = new SqlConnection(FormMain.app_connectionString);
            if (cn.State == ConnectionState.Closed)
                cn.Open();

            SqlCommand sqlCmd = new SqlCommand();
            // получение используемого id_structure
            sqlQuery = "SELECT id_structure FROM structures WHERE name = '" + strName + "'";
            sqlCmd.CommandText = sqlQuery;
            sqlCmd.Connection = cn;
            string idStr = sqlCmd.ExecuteScalar().ToString();

            // получение используемого id_algorithm_variant 
            sqlQuery = "SELECT id_algorithm_variant FROM algorithm_variants WHERE name = '" + algVarName + "'";
            sqlCmd.CommandText = sqlQuery;
            string idAlgVar = sqlCmd.ExecuteScalar().ToString();

            // получение имени используемого алгоритма ИАД
            sqlQuery = "SELECT [algorithms].name FROM algorithms JOIN algorithm_variants" + 
                        " ON algorithm_variants.id_algorithm = algorithms.id_algorithm AND"+
                        " algorithm_variants.name = '" + algVarName + "'";
            sqlCmd.CommandText = sqlQuery;
            string nameAlg = sqlCmd.ExecuteScalar().ToString();



            dmxQuery = "ALTER MINING STRUCTURE [" + strName + "]" +
                        " ADD MINING MODEL [" + modelName + "]";

            string strSelect;
            SqlConnection cn2 = new SqlConnection(FormMain.app_connectionString);
                cn2.Open();
            SqlDataAdapter sqlDA;
            DataTable dtColumns;
            // если проводится кластеризация, то прогнозируемый столбец не требуется:
            if (FormMain.taskType == 2)     
            {
                // вид DMX - запроса: 
                //ADD MINING MODEL <model>
                //( <structure column name>  [AS <model column name>]  [<modeling flags>]    [<prediction>]
                //  [(<nested column definition list>) [WITH FILTER (<nested filter criteria>)]] )
                //USING <algorithm> [(<parameter list>)] 

                // запроса столбцов структуры:

                

                // получение списка столбцов из выборки данных
                strSelect = "SELECT column_name FROM dsv_columns JOIN selections " + 
                            " ON dsv_columns.id_dsv = selections.id_dsv INNER JOIN structures "+
                            " ON structures.id_selection = selections.id_selection WHERE structures.name = '" + strName + "'";

                sqlDA = new SqlDataAdapter(strSelect, cn2);
                dtColumns = new DataTable();
                sqlDA.Fill(dtColumns);

                List<string> colNames = new List<string>();


                for (int i = 0; i < dtColumns.Rows.Count; i++)
                    colNames.Add(dtColumns.Rows[i][0].ToString());
                
                dmxQuery += "(";

                for (int i = 0; i < colNames.Count; i++)
                {
                    string curName = colNames[i];

                    // для колонки YearID пропускаем ключ PREDICT 
                    if (curName.Contains("Year"))
                        dmxQuery += " [" + curName + "],";
                    else
                        dmxQuery += " [" + curName +  "] PREDICT,";
                }
                dmxQuery = dmxQuery.Substring(0, dmxQuery.Length -1);

                dmxQuery += ")";
            }

            dmxQuery += " USING " + nameAlg + " ";
            
            // получение параметров модели:
            strSelect = "SELECT parameters.name, parameters.value FROM parameters INNER JOIN algorithm_variants " +
                                " ON algorithm_variants.id_algorithm_variant = parameters.id_algorithm_variant " +
                                " where algorithm_variants.name = '" + algVarName + "' ";

            sqlDA = new SqlDataAdapter(strSelect, cn2);
            dtColumns = new DataTable();
            sqlDA.Fill(dtColumns);

            // если есть параметры для добавления
            if (dtColumns.Rows.Count > 0)
                dmxQuery += " (";

            for (int i = 0; i < dtColumns.Rows.Count; i++)
                dmxQuery += " " + dtColumns.Rows[i][0].ToString() + " = " + dtColumns.Rows[i][1].ToString() + ",";   

            dmxQuery = dmxQuery.Substring(0, dmxQuery.Length - 1);

            if (dtColumns.Rows.Count > 0)
                dmxQuery += ") ";

            // возможность детализации модели:
            dmxQuery += " WITH DRILLTHROUGH ";

            // создание объекта соединения с БД SSAS и команды для отправки dmx-запроса
            AdomdConnection adomdCn = new AdomdConnection();
            adomdCn.ConnectionString = FormMain.as_connectionString;
            adomdCn.Open();

            AdomdCommand adomdCmd = adomdCn.CreateCommand();
            adomdCmd.CommandText = dmxQuery;
            try
            {
                adomdCmd.Execute();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
            // сохранение информации о создаваемой модели в БД
            sqlQuery = "INSERT INTO [models] VALUES ('" + idStr + "', '" + modelName + "', '" + idAlgVar + "')";
            sqlCmd.CommandText = sqlQuery;
            sqlCmd.ExecuteNonQuery();
            
            this.Close();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectSelections();
        }

        private void selectSelections()
        {
            comboBox5.DataSource = null;
            comboBox5.Text = null;

            SqlConnection cn = new SqlConnection(FormMain.app_connectionString);
            DataTable dt = new DataTable();

            String dsvName = comboBox3.Text;

            if (dsvName != "")
            {
                string strSel = "SELECT dbo.selections.name FROM [dbo].selections JOIN dbo.data_source_views" +
                                    " ON dbo.data_source_views.id_dsv = dbo.selections.id_dsv" +
                                    " and dbo.data_source_views.name = '" + dsvName + "'";

                SqlDataAdapter sqlDA = new SqlDataAdapter(strSel, cn);
                sqlDA.Fill(dt);

                if (dt != null)
                {
                    comboBox5.DataSource = dt;
                    comboBox5.DisplayMember = "name";
                }
            }
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectStructures();
        }

        private void selectStructures()
        {
            comboBox1.DataSource = null;
            comboBox1.Text = null;

            SqlConnection cn = new SqlConnection(FormMain.app_connectionString);
            DataTable dt = new DataTable();

            String selName = comboBox5.Text;
            if (selName != "")
            {
                // Create data adapters from database tables and load schemas
                SqlDataAdapter sqlDA = new SqlDataAdapter("SELECT structures.name FROM [structures] JOIN selections ON " +
                                                            " selections.name = '" + selName + "' AND " +
                                                            " selections.id_selection = structures.id_selection", cn);
                sqlDA.Fill(dt);
                if (dt != null)
                {
                    comboBox1.DataSource = dt;
                    comboBox1.DisplayMember = "name";
                }
            }
        }
    }
}
