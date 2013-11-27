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
    public partial class FormRelabel : Form
    {
        public FormRelabel()
        {
            InitializeComponent();
        }

        private void FormRelabel_Load(object sender, EventArgs e)
        {
            comboBox1.DataSource = FormSelection.glob_columnNames;
            //dataGridView1.DataSource = FormSelection.glob_dt;
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control.GetType() == typeof(DataGridViewComboBoxEditingControl))
            {
                DataGridViewComboBoxEditingControl cbo =
                    e.Control as DataGridViewComboBoxEditingControl;
                cbo.DropDownStyle = ComboBoxStyle.DropDown;
            }
        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            


        }

        // получение списка уникальных значений для переразметки
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();

            // список уникальных значений в столбце dataGrid
            List<string> uniqLabels = new List<string>();

            // из всех строк global_dt выделить уникальные значения:
            uniqLabels = drillDataTable();

            // заполнить первую колонку dataGrid уникальными значениями
            fillDataGrid(uniqLabels);

        }
        
        // из всех строк global_dt выделить уникальные значения:
        private List<string> drillDataTable()
        {
            int colIndex = comboBox1.SelectedIndex;
            List<string> uniqLabels = new List<string>();
            uniqLabels.Add(FormSelection.glob_dt.Rows[0][colIndex].ToString());

            // проход всех строк в dataTable
            for (int i = 0; i < FormSelection.glob_dt.Rows.Count; i++)
            {
                // получение значения текущей строки
                string curLabel = FormSelection.glob_dt.Rows[i][colIndex].ToString();

                // проверка на наличие в списке уникальных значений
                if (!checkLabel(curLabel, uniqLabels))
                    uniqLabels.Add(curLabel);
            }

            return uniqLabels;
        }

        // проверка на наличие в списке уникальных значений
        private bool checkLabel(string label, List<string> listOfLabels)
        {
            for (int i = 0; i < listOfLabels.Count; i++)
                if (listOfLabels[i] == label)
                    return true;

            return false;
        }

        private void fillDataGrid(List<string> labels)
        {
            for (int i = 0; i < labels.Count; i++)
            {
                // заполнение комбинированного dataGridView1 из списка labels
                DataGridViewRow dvr = (DataGridViewRow)dataGridView1.Rows[0].Clone();
                dvr.Cells[0].Value = labels[i];
                dataGridView1.Rows.Add(dvr);
            }
        }

        
    }
}
