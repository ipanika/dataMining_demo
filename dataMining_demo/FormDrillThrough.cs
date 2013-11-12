using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.AnalysisServices.AdomdClient;
using System.Windows.Forms.DataVisualization.Charting;

namespace dataMining_demo
{
    public partial class FormDrillThrough : Form
    {
        public FormDrillThrough()
        {
            InitializeComponent();
        }

        private void DrillThroughForm_Load(object sender, EventArgs e)
        {
            // запрос к метаданным модели, выбранной на главной форме
            AdomdConnection cn = new AdomdConnection();
            cn.ConnectionString = "Data Source = localhost; Initial Catalog = demo_DM";
            cn.Open();

            AdomdCommand cmd = cn.CreateCommand();
            string modelName = FormMain.modelName;// MainForm.comboBox3.Text;
            cmd.CommandText = "SELECT NODE_CAPTION FROM [" + modelName + "].CONTENT";
            //cmd.CommandText = "SELECT NODE_CAPTION, NODE_DISTRIBUTION FROM [mod_drill].CONTENT";

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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // запрос к метаданным модели, выбранной на главной форме
            AdomdConnection cn = new AdomdConnection();
            cn.ConnectionString = "Data Source = localhost; Initial Catalog = demo_DM";
            cn.Open();

            AdomdCommand cmd = cn.CreateCommand();
            string modelName = FormMain.modelName;// MainForm.comboBox3.Text;
            cmd.CommandText = "CALL System.GetModelAttributes('" + modelName + "')";
            
            AdomdDataReader reader = cmd.ExecuteReader();
            List<string> _sideList = new List<string>();
            
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (i % 7 == 1 ) 
                        _sideList.Add(reader.GetValue(i).ToString());
                }
            }

            comboBox2.DataSource = _sideList;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

            // запрос к метаданным модели, выбранной на главной форме
            AdomdConnection cn = new AdomdConnection();
            cn.ConnectionString = "Data Source = localhost; Initial Catalog = demo_DM";
            cn.Open();

            AdomdCommand cmd = cn.CreateCommand();
            string modelName = FormMain.modelName;// MainForm.comboBox3.Text;
            cmd.CommandText = " SELECT flattened (SELECT  ATTRIBUTE_VALUE, [SUPPORT]" +
                                "FROM NODE_DISTRIBUTION where ATTRIBUTE_NAME = '"+comboBox2.Text+"') " +
                                "FROM ["+ modelName + "].CONTENT where node_caption = '"+comboBox1.Text + "'";

            AdomdDataReader reader = cmd.ExecuteReader();
            List<string> attr_value = new List<string>();
            List<string> attr_support = new List<string>();

            Series series;
            
            chart1.Series.Clear();
            //chart1.ChartAreas.Clear();

            while (reader.Read())
            {
                for (int  i = 0; i < reader.FieldCount; i++)
                {
                    if (i % 2 == 0)
                        attr_value.Add(reader.GetValue(i).ToString());
                    else
                        attr_support.Add(reader.GetValue(i).ToString());
                    
                }                
            }

            for (int i = 0; i < attr_support.Count; i++)
            {
                series = chart1.Series.Add(attr_value[i]);
                double pnt = Convert.ToDouble(attr_support[i]);
                series.Points.Add(pnt);
            }
        }
    }
}
