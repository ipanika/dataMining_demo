using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.AnalysisServices.AdomdClient;

namespace dataMining_demo
{
    public partial class MetaDataForm : Form
    {
        public MetaDataForm()
        {
            InitializeComponent();
        }

        private void MetaDataForm_Load(object sender, EventArgs e)
        {
            // запрос к метаданным модели, выбранной на главной форме
            AdomdConnection cn = new AdomdConnection();
            cn.ConnectionString = "Data Source = localhost; Initial Catalog = demo_DM";
            cn.Open();

            AdomdCommand cmd = cn.CreateCommand();
            cmd.CommandText = "SELECT NODE_CAPTION, NODE_DESCRIPTION FROM [mod_drill].CONTENT";

            AdomdDataReader reader = cmd.ExecuteReader();
            dataGridView1.AutoGenerateColumns = true;

            while (reader.Read())
            {
                DataGridViewRow dvr = (DataGridViewRow)dataGridView1.Rows[0].Clone();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    dvr.Cells[i].Value = reader.GetValue(i);
                    
                    //MessageBox.Show(reader.GetValue(i).ToString());
                }
                dataGridView1.Rows.Add(dvr);
            }
        }
    }
}
