using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.AnalysisServices;

namespace dataMining_demo
{
    public partial class MiningStructureForm : Form
    {

        Server svr = new Server();
        Database db = new Database();

        public MiningStructureForm()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //db.DataSourceViews.FindByName();
        }

        private void MiningStructureForm_Load(object sender, EventArgs e)
        {
            svr.Connect("localhost");

            if ((svr != null) && (svr.Connected))
            {
                db = svr.Databases.FindByName("demo_DM");

                //db = svr.Databases.Add("demo_DM");
                //db.Update();
            }

            // загрузка имеющихся представлений ИАД
            
        }
    }
}
