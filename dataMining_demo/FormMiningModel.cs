using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.AnalysisServices;
using System.Data.SqlClient;

namespace dataMining_demo
{
    public partial class FormMiningModel : Form
    {
        Server svr = new Server();
        Database db = new Database();
        MiningStructure ms = new MiningStructure();

        public FormMiningModel()
        {
            InitializeComponent();
        }

        private void MiningModelForm_Load(object sender, EventArgs e)
        {
            svr.Connect("localhost");

            if ((svr != null) && (svr.Connected))
                db = svr.Databases.FindByName("demo_DM");

            // создать соединение с БД
            SqlConnection cn = new SqlConnection("Data Source=localhost; Initial Catalog=demo_source; Integrated Security=true");
            if (cn.State == ConnectionState.Closed)
                cn.Open();

            DataTable dt = new DataTable();
            // загрузка имеющихся представлений ИАД
            SqlDataAdapter sqlDA = new SqlDataAdapter("select [mstr_name] FROM [demo_mstr]", cn);
            sqlDA.Fill(dt);

            comboBox1.DataSource = dt;
            comboBox1.DisplayMember = "mstr_name";

            dataGridView1.Rows.Add("CLUSTERING_METHOD", "");
            dataGridView1.Rows.Add("CLUSTER_COUNT", "");

        }

        private void button1_Click(object sender, EventArgs e)
        {
            MiningModel ClusterModel;

            // Create the Cluster model and set the algorithm 
            // and parameters
            string modelName = textBox1.Text;
            string strName = comboBox1.Text;

            ms = db.MiningStructures.FindByName(strName);
            ClusterModel = ms.CreateMiningModel(true, modelName);
            ClusterModel.Algorithm = "Microsoft_Clustering";

            // считывание параметров алгоритма из dataGridView
            for (int i = 0; i < dataGridView1.Rows.Count-1; i++)
            {
                DataGridViewRow drv = dataGridView1.Rows[i];
                if (drv.Cells[1].Value.ToString() != "")
                {
                    string parName = drv.Cells[0].Value.ToString();
                    string parValue = drv.Cells[1].Value.ToString();
                    ClusterModel.AlgorithmParameters.Add(parName, parValue);
                };
            }

            ClusterModel.AllowDrillThrough = true;

            SqlConnection cn = new SqlConnection("Data Source=localhost; Initial Catalog=demo_source; Integrated Security=true");

            if (cn.State == ConnectionState.Closed)
                cn.Open();

            SqlCommand sqlCmd = new SqlCommand("INSERT INTO [demo_mm]  VALUES ('" + modelName + "', '"+strName +"')", cn);
            sqlCmd.ExecuteNonQuery();
            // Submit the models to the server
            ClusterModel.Update();
            this.Close();
        }
    }
}
