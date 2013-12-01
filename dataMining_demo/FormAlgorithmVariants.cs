using System;
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
    public partial class FormAlgorithmVariants : Form
    {
        public FormAlgorithmVariants()
        {
            InitializeComponent();
        }

        private void FormAlgorithmVariants_Load(object sender, EventArgs e)
        {
            SqlConnection cn = new SqlConnection(FormMain.app_connectionString);
            if (cn.State == ConnectionState.Closed)
                cn.Open();

            DataTable dt = new DataTable();
            
            // загрузка имеющихся методов ИАД
            SqlDataAdapter sqlDA = new SqlDataAdapter("select [name] FROM [algorithms]", cn);
            sqlDA.Fill(dt);

            comboBox1.DataSource = dt;
            comboBox1.DisplayMember = "name";

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            
            fillDataGrid();
            
        }

        private void fillDataGrid()
        {
            dataGridView1.AllowUserToAddRows = true;
            if (comboBox1.Text == "Microsoft_Clustering")
            {
                dataGridView1.Rows.Add(9);
                dataGridView1.Rows[0].Cells[0].Value = "CLUSTERING_METHOD";
                dataGridView1.Rows[0].Cells[2].Value = 1;
                dataGridView1.Rows[0].Cells[3].Value = "1, 2, 3, 4";
                dataGridView1.Rows[1].Cells[0].Value = "CLUSTER_COUNT";
                dataGridView1.Rows[1].Cells[2].Value = 10;
                dataGridView1.Rows[1].Cells[3].Value = "[0,...)";
                dataGridView1.Rows[2].Cells[0].Value = "CLUSTER_SEED";
                dataGridView1.Rows[2].Cells[2].Value = 0;
                dataGridView1.Rows[2].Cells[3].Value = "[0,...)";
                dataGridView1.Rows[3].Cells[0].Value = "MINIMUM_SUPPORT";
                dataGridView1.Rows[3].Cells[2].Value = 1;
                dataGridView1.Rows[3].Cells[3].Value = "(0,...)";
                dataGridView1.Rows[4].Cells[0].Value = "MODELLING_CARDINALITY";
                dataGridView1.Rows[4].Cells[2].Value = 10;
                dataGridView1.Rows[4].Cells[3].Value = "[1, 50]";
                dataGridView1.Rows[5].Cells[0].Value = "STOPPING_TOLERANCE";
                dataGridView1.Rows[5].Cells[2].Value = 10;
                dataGridView1.Rows[5].Cells[3].Value = "(0, ...)";
                dataGridView1.Rows[6].Cells[0].Value = "SAMPLE_SIZE";
                dataGridView1.Rows[6].Cells[2].Value = 50000;
                dataGridView1.Rows[6].Cells[3].Value = "0, [100, ...]";
                dataGridView1.Rows[7].Cells[0].Value = "MAXIMUM_INPUT_ATTRIBUTES";
                dataGridView1.Rows[7].Cells[2].Value = 255;
                dataGridView1.Rows[7].Cells[3].Value = "[0, 65535]";
                dataGridView1.Rows[8].Cells[0].Value = "MAXIMUM_STATES";
                dataGridView1.Rows[8].Cells[2].Value = 100;
                dataGridView1.Rows[8].Cells[3].Value = "0, [2, 65535]";
            }
            else if (comboBox1.Text == "Microsoft_Time_Series")
            {
                dataGridView1.Rows.Add(10);
                dataGridView1.Rows[0].Cells[0].Value = "AUTO_DETECT_PERIODICITY";
                dataGridView1.Rows[0].Cells[2].Value = "0.6";
                dataGridView1.Rows[0].Cells[3].Value = "[0.0, 1.0]";
                dataGridView1.Rows[1].Cells[0].Value = "COMPLEXITY_PENALTY";
                dataGridView1.Rows[1].Cells[2].Value = "0.1";
                dataGridView1.Rows[1].Cells[3].Value = "(..., 1.0)";
                dataGridView1.Rows[2].Cells[0].Value = "FORECAST_METHOD";
                dataGridView1.Rows[2].Cells[2].Value = "MIXED";
                dataGridView1.Rows[2].Cells[3].Value = "ARIMA, ARTXP, MIXED";
                dataGridView1.Rows[3].Cells[0].Value = "HISTORIC_MODEL_COUNT";
                dataGridView1.Rows[3].Cells[2].Value = 1;
                dataGridView1.Rows[3].Cells[3].Value = "[0, 100]";
                dataGridView1.Rows[4].Cells[0].Value = "HISTORICAL_MODEL_GAP";
                dataGridView1.Rows[4].Cells[2].Value = 10;
                dataGridView1.Rows[4].Cells[3].Value = "[1, ...)";
                dataGridView1.Rows[5].Cells[0].Value = "INSTABILITY_SENSITIVITY";
                dataGridView1.Rows[5].Cells[2].Value = 1;
                dataGridView1.Rows[5].Cells[3].Value = "[0.0, 1.0]";
                dataGridView1.Rows[6].Cells[0].Value = "MINIMUM_SUPPORT";
                dataGridView1.Rows[6].Cells[2].Value = 10;
                dataGridView1.Rows[6].Cells[3].Value = "[1, ...)";
                dataGridView1.Rows[7].Cells[0].Value = "MISSING_VALUE_SUBSTITUTION";
                dataGridView1.Rows[7].Cells[2].Value = "Mean";
                dataGridView1.Rows[7].Cells[3].Value = "None, Previous, Mean";
                dataGridView1.Rows[8].Cells[0].Value = "PERIODICITY_HINT";
                dataGridView1.Rows[8].Cells[2].Value = 1;
                dataGridView1.Rows[8].Cells[3].Value = "{integers}";
                dataGridView1.Rows[9].Cells[0].Value = "PREDICTION_SMOOTHING";
                dataGridView1.Rows[9].Cells[2].Value = "0.5";
                dataGridView1.Rows[9].Cells[3].Value = "[0.0, 1.0]";
            }

            dataGridView1.AllowUserToAddRows = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection cn = new SqlConnection(FormMain.app_connectionString);
            if (cn.State == ConnectionState.Closed)
                cn.Open();

            string algName = comboBox1.Text;
            string algVarName = textBox1.Text;

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandText = "SELECT id_algorithm FROM algorithms WHERE name = '" + algName + "'";
            sqlCmd.Connection = cn;

            string idAlg = "";
            try
            {
                idAlg = sqlCmd.ExecuteScalar().ToString();
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message);
            }

            sqlCmd.CommandText = "INSERT INTO [algorithm_variants] VALUES ('" + idAlg + "', '" + algVarName + "')";
            try
            {
                sqlCmd.ExecuteNonQuery();
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message);
            }

            sqlCmd.CommandText = "SELECT id_algorithm_variant FROM algorithm_variants WHERE name = '" + algVarName + "'";
            sqlCmd.Connection = cn;

            string idVarAlg = sqlCmd.ExecuteScalar().ToString();

            // сохранение параметров варианта алгоритма в БД
            string strQuery = "INSERT INTO [parameters] VALUES";

            int countPars = 0; // счетчик параметров, если он = 0 , то сохранять данные в БД не надо.
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                // выбор значения параметра из ячейки dataGridView
                if (dataGridView1.Rows[i].Cells[1].Value != null)
                {
                    strQuery += " ('" + idVarAlg + "', '" + dataGridView1.Rows[i].Cells[0].Value.ToString() + "',";
                    strQuery += " '" + dataGridView1.Rows[i].Cells[1].Value.ToString() + "'),";

                    countPars += 1;
                }
            }

            strQuery = strQuery.Substring(0, strQuery.Length - 1);
            
            sqlCmd.CommandText = strQuery;
            if (countPars > 0)
                sqlCmd.ExecuteNonQuery();
            
            this.Close();
        }
    }
}
