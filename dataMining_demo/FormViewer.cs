using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.AnalysisServices.Viewers;
using Microsoft.AnalysisServices.AdomdClient;


namespace dataMining_demo
{
    public partial class FormViewer : Form
    {
        public FormViewer()
        {
            InitializeComponent();
        }

        private void FormViewer_Load(object sender, EventArgs e)
        {
            MiningModelViewerControl viewer = null;
            MiningModel model = null;
            MiningService service = null;

            AdomdConnection cn = new AdomdConnection();
            cn.ConnectionString = "Data Source = localhost; Initial Catalog = SSAS_DM";
            cn.Open();

            
            string modelName = FormMain.modelName;// MainForm.comboBox3.Text;

            model = cn.MiningModels[modelName];
            service = cn.MiningServices[model.Algorithm];

            if (service.ViewerType == "Microsoft_Cluster_Viewer")
                viewer = new ClusterViewer();
            else if (service.ViewerType == "Microsoft_TimeSeries_Viewer")
                viewer = new TimeSeriesViewer();
            else throw new System.Exception("Custom Viewers not supported");

            // Set up and load the viewer
            viewer.ConnectionString = "Provider=MSOLAP; Integrated Security=SSPI; Data Source = localhost; Initial Catalog = SSAS_DM";
            viewer.MiningModelName = modelName;
            viewer.Dock = DockStyle.Fill;
            panel1.Controls.Add(viewer);
            viewer.LoadViewerData(null);
        }
    }
}
