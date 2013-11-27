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
        }

        // событие, позволяющее редактировать содержимое ячейки combobox в dataGridView
        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control.GetType() == typeof(DataGridViewComboBoxEditingControl))
            {
                DataGridViewComboBoxEditingControl cbo =
                    e.Control as DataGridViewComboBoxEditingControl;
                cbo.DropDownStyle = ComboBoxStyle.DropDown;
            }
        }

        // редактирование значения ячейки
        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {

            // МАГИЯ СО STACKOVERFLOW: 
            // если редактируется столбец-combobox
            if (dataGridView1.CurrentCell.GetType() == typeof(DataGridViewComboBoxCell))
            {
                // после проверки добавляем новое значение в список listOfNewLabels
                if (!Column2.Items.Contains(e.FormattedValue.ToString()))
                {
                    Column2.Items.Add(e.FormattedValue);
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = e.FormattedValue.ToString();
                }
            }

        }

        // получение списка уникальных значений для переразметки
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.AllowUserToAddRows = true;

            // список уникальных значений в столбце dataGrid
            List<string> uniqLabels = new List<string>();

            // из всех строк global_dt выделить уникальные значения:
            uniqLabels = drillDataTable();

            // заполнить первую колонку dataGrid уникальными значениями
            fillDataGrid(uniqLabels);

            dataGridView1.AllowUserToAddRows = false;

        }
        
        // из всех строк global_dt выделить уникальные значения:
        private List<string> drillDataTable()
        {
            FormSelection.glob_relabelColumnIndex = comboBox1.SelectedIndex;
            List<string> uniqLabels = new List<string>();
            uniqLabels.Add(FormSelection.glob_dt.Rows[0][FormSelection.glob_relabelColumnIndex].ToString());

            // проход всех строк в dataTable
            for (int i = 0; i < FormSelection.glob_dt.Rows.Count; i++)
            {
                // получение значения текущей строки
                string curLabel = FormSelection.glob_dt.Rows[i][FormSelection.glob_relabelColumnIndex].ToString();

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

        // сохранение новых меток по нажатию на кнопку "Обновить"
        private void button1_Click(object sender, EventArgs e)
        {
            
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                try
                {
                    // предыдущие метки:
                    if (dataGridView1.Rows[i].Cells[0].Value != null)
                        FormSelection.glob_previousLabels.Add(dataGridView1.Rows[i].Cells[0].Value.ToString());
                    else
                        FormSelection.glob_previousLabels.Add("");

                    // новые метки:
                    if (dataGridView1.Rows[i].Cells[1].Value != null)
                        FormSelection.glob_currentLabels.Add(dataGridView1.Rows[i].Cells[1].Value.ToString());
                    else
                        FormSelection.glob_currentLabels.Add("");
                }
                catch (Exception e1)
                {
                    MessageBox.Show(e1.Message);
                }

            }

            this.Close();
        }

        
    }
}
