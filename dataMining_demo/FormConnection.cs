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
    public partial class FormConnection : Form
    {
        public FormConnection()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormMain.dw_dataSource = textBox1.Text;
            FormMain.dw_initCatalog = textBox2.Text;
            FormMain.dw_connectionString = "Data Source=" + textBox1.Text + "; Initial Catalog=" + textBox2.Text + "; Integrated Security=SSPI";

            FormMain.app_dataSource = textBox3.Text;
            FormMain.app_initCatalog = textBox4.Text;
            FormMain.app_connectionString = "Data Source=" + textBox3.Text + "; Initial Catalog=" + textBox4.Text +
                                      "; Integrated Security=SSPI";
            Properties.Settings.Default.Save();
            this.Close();
        }

        
    }
}
