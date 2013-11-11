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
    public partial class FormAlgorithmVariants : Form
    {
        public FormAlgorithmVariants()
        {
            InitializeComponent();
        }

        private void FormAlgorithmVariants_Load(object sender, EventArgs e)
        {
            SqlConnection cn = new SqlConnection("Data Source=localhost; Initial Catalog=demo_dm; Integrated Security=true");
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
                dataGridView1.Rows[0].Cells[1].Value = 1;
                dataGridView1.Rows[1].Cells[0].Value = "CLUSTER_COUNT";
                dataGridView1.Rows[1].Cells[1].Value = 10;
                dataGridView1.Rows[2].Cells[0].Value = "CLUSTER_SEED";
                dataGridView1.Rows[2].Cells[1].Value = 0;
                dataGridView1.Rows[3].Cells[0].Value = "MINIMUM_SUPPORT";
                dataGridView1.Rows[3].Cells[1].Value = 1;
                dataGridView1.Rows[4].Cells[0].Value = "MODELLING_CARDINALITY";
                dataGridView1.Rows[4].Cells[1].Value = 10;
                dataGridView1.Rows[5].Cells[0].Value = "STOPPING_TOLERANCE";
                dataGridView1.Rows[5].Cells[1].Value = 10;
                dataGridView1.Rows[6].Cells[0].Value = "SAMPLE_SIZE";
                dataGridView1.Rows[6].Cells[1].Value = 50000;
                dataGridView1.Rows[7].Cells[0].Value = "MAXIMUM_INPUT_ATTRIBUTES";
                dataGridView1.Rows[7].Cells[1].Value = 255;
                dataGridView1.Rows[8].Cells[0].Value = "MAXIMUM_STATES";
                dataGridView1.Rows[8].Cells[1].Value = 100;
            }
            else if (comboBox1.Text == "Microsoft_TimeSeries")
            {
                dataGridView1.Rows.Add(9);
                dataGridView1.Rows[0].Cells[0].Value = "AUTO_DETECT_PERIODICITY";
                dataGridView1.Rows[0].Cells[1].Value = 0.6;
                dataGridView1.Rows[1].Cells[0].Value = "COMPLEXITY_PENALTY";
                dataGridView1.Rows[1].Cells[1].Value = 0.1;
                dataGridView1.Rows[2].Cells[0].Value = "FORECAST_METHOD";
                dataGridView1.Rows[2].Cells[1].Value = "MIXED";
                dataGridView1.Rows[3].Cells[0].Value = "HISTORIC_MODEL_COUNT";
                dataGridView1.Rows[3].Cells[1].Value = 1;
                dataGridView1.Rows[4].Cells[0].Value = "HISTORICAL_MODEL_GAP";
                dataGridView1.Rows[4].Cells[1].Value = 10;
                dataGridView1.Rows[5].Cells[0].Value = "INSTABILITY_SENSITIVITY";
                dataGridView1.Rows[5].Cells[1].Value = 1;
                dataGridView1.Rows[6].Cells[0].Value = "MINIMUM_SUPPORT";
                dataGridView1.Rows[6].Cells[1].Value = 10;
                dataGridView1.Rows[7].Cells[0].Value = "MISSING_VALUE_SUBSTITUTION";
                dataGridView1.Rows[7].Cells[1].Value = "Mean";
                dataGridView1.Rows[8].Cells[0].Value = "PERIODICITY_HINT";
                dataGridView1.Rows[8].Cells[1].Value = 1;
            }

            dataGridView1.AllowUserToAddRows = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection cn = new SqlConnection("Data Source=localhost; Initial Catalog=demo_dm; Integrated Security=true");
            if (cn.State == ConnectionState.Closed)
                cn.Open();

            string algName = comboBox1.Text;
            string algVarName = textBox1.Text;

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandText = "SELECT id_algorithm FROM algorithms WHERE name = '" + algName + "'";
            sqlCmd.Connection = cn;
            
            string idAlg = sqlCmd.ExecuteScalar().ToString();

            sqlCmd.CommandText = "INSERT INTO [algorithm_variants] VALUES ('" + idAlg + "', '" + algVarName + "')";
            sqlCmd.ExecuteNonQuery();

            sqlCmd.CommandText = "SELECT id_algorithm_variant FROM algorithm_variants WHERE name = '" + algVarName + "'";
            sqlCmd.Connection = cn;

            string idVarAlg = sqlCmd.ExecuteScalar().ToString();

            // сохранение параметров варианта алгоритма в БД
            string strQuery = "INSERT INTO [parameters] VALUES";

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                strQuery += " ('" + idVarAlg + "', '" + dataGridView1.Rows[i].Cells[0].Value.ToString() + "',";
                strQuery += " '" + dataGridView1.Rows[i].Cells[1].Value.ToString() + "'),";
            }

            strQuery = strQuery.Substring(0, strQuery.Length - 1);
            
            sqlCmd.CommandText = strQuery;
            sqlCmd.ExecuteNonQuery();
            
            this.Close();


        }
    }
}