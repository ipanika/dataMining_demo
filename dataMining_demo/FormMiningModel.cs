﻿using System;
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
            //svr.Connect("localhost");

            //if ((svr != null) && (svr.Connected))
            //    db = svr.Databases.FindByName("SSAS_DM");

            // создать соединение с БД
            SqlConnection cn = new SqlConnection("Data Source=localhost; Initial Catalog=DM; Integrated Security=true");
            if (cn.State == ConnectionState.Closed)
                cn.Open();

            DataTable dt1 = new DataTable();
            // загрузка имеющихся представлений ИАД
            SqlDataAdapter sqlDA = new SqlDataAdapter("select [name] FROM [structures]", cn);
            sqlDA.Fill(dt1);

            comboBox1.DataSource = dt1;
            comboBox1.DisplayMember = "name";

            DataTable dt = new DataTable();
            sqlDA = new SqlDataAdapter("select [name] FROM [algorithm_variants]", cn);
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

            
            SqlConnection cn = new SqlConnection("Data Source=localhost; Initial Catalog=DM; Integrated Security=true");
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

            // если проводится кластеризация, то прогнозируемый столбец не требуется:
            if (FormMain.taskType == 2)     
            {
                // вид DMX - запроса: 
                //ADD MINING MODEL <model>
                //( <structure column name>  [AS <model column name>]  [<modeling flags>]    [<prediction>]
                //  [(<nested column definition list>) [WITH FILTER (<nested filter criteria>)]] )
                //USING <algorithm> [(<parameter list>)] 

                // запроса столбцов структуры:

                SqlConnection cn2 = new SqlConnection("Data Source=localhost; Initial Catalog=DM; Integrated Security=true");
                cn2.Open();

                // получение списка столбцов из выборки данных
                string strSelect = "SELECT column_name FROM dsv_columns JOIN selections " + 
                            " ON dsv_columns.id_dsv = selections.id_dsv INNER JOIN structures "+
                            " ON structures.id_selection = selections.id_selection WHERE structures.name = '" + strName + "'";

                SqlDataAdapter sqlDA = new SqlDataAdapter(strSelect, cn2);
                DataTable dtColumns = new DataTable();
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
                    {
                        dmxQuery += " [" + curName + "],";
                    }
                    else
                    {
                        dmxQuery += " [" + curName +  "] PREDICT,";
                    }
                }
                dmxQuery = dmxQuery.Substring(0, dmxQuery.Length -1);

                dmxQuery += ")";
            }

            dmxQuery += " USING " + nameAlg + " WITH DRILLTHROUGH ";

            // создание объекта соединения с БД SSAS и команды для отправки dmx-запроса
            AdomdConnection adomdCn = new AdomdConnection();
            adomdCn.ConnectionString = "Data Source = localhost; Initial Catalog = SSAS_DM";
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
    }
}
