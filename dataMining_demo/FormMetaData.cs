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
    public partial class FormMetaData : Form
    {
        public FormMetaData()
        {
            InitializeComponent();
        }

        private void MetaDataForm_Load(object sender, EventArgs e)
        {
           

            getNodeName();
        }

        private void getNodeName()
        {
            try
            {

                // запрос к метаданным модели, выбранной на главной форме
                AdomdConnection cn = new AdomdConnection();
                cn.ConnectionString = FormMain.as_connectionString;
                cn.Open();

                AdomdCommand cmd = cn.CreateCommand();
                string modelName = FormMain.modelName;// MainForm.comboBox3.Text;
                cmd.CommandText = "SELECT NODE_CAPTION FROM [" + modelName + "].CONTENT";

                AdomdDataReader reader = cmd.ExecuteReader();
                List<string> _sideList = new List<string>();
                while (reader.Read())
                {

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        _sideList.Add(reader.GetValue(i).ToString());
                    }
                }

                comboBox1.DataSource = _sideList;
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message);
                this.Close();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            try
            {
                // запрос к метаданным модели, выбранной на главной форме
                AdomdConnection cn = new AdomdConnection();
                cn.ConnectionString = FormMain.as_connectionString;
                cn.Open();

                AdomdCommand cmd = cn.CreateCommand();
                string modelName = FormMain.modelName;// MainForm.comboBox3.Text;
                cmd.CommandText = " SELECT flattened (SELECT ATTRIBUTE_NAME, ATTRIBUTE_VALUE, [SUPPORT], [PROBABILITY]" +
                                    "FROM NODE_DISTRIBUTION) " +
                                    "FROM [" + modelName + "].CONTENT where node_caption = '" + comboBox1.Text + "'";

                AdomdDataReader reader = cmd.ExecuteReader();
                dataGridView1.AutoGenerateColumns = true;

                while (reader.Read())
                {
                    DataGridViewRow dvr = (DataGridViewRow)dataGridView1.Rows[0].Clone();

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        dvr.Cells[i].Value = reader.GetValue(i);

                    }
                    dataGridView1.Rows.Add(dvr);
                }
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message);
            }
        }
    }


}
