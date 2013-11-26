using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace dataMining_demo
{
    public partial class FormRelabel : Form
    {
        public FormRelabel()
        {
            InitializeComponent();
        }

        private void FormRelabel_Load(object sender, EventArgs e)
        {
            comboBox1.DataSource = FormSelection.glob_columnNames;
            dataGridView1.DataSource = FormSelection.glob_dt;
        }
    }
}
