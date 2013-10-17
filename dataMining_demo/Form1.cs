using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Office.Interop.Excel;

namespace dataMining_demo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Microsoft.Office.Interop.Excel.Application objExcel;
        private Microsoft.Office.Interop.Excel.Workbook objWorkBook;
        private Microsoft.Office.Interop.Excel.Worksheet objWorkSheet;

        private void button1_Click(object sender, EventArgs e)
        {
            Stream myStream = null;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "C:\\resources";
            openFileDialog1.Filter = "excel files (.xls,.xlsx)|*.xlsx;*.xls|All files|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            objExcel = new Microsoft.Office.Interop.Excel.Application();
                            objWorkBook = objExcel.Workbooks.Open(openFileDialog1.FileName);
                            objWorkSheet = objExcel.ActiveSheet as Microsoft.Office.Interop.Excel.Worksheet;
                            Microsoft.Office.Interop.Excel.Range rg = null;

                            Int16 row = 1;
                            dataGridView1.Rows.Clear();

                            string[] arr = new string[13];
                            
                            while (objWorkSheet.get_Range("a" + row, "a" + row).Value != null)
                            {
                                rg = objWorkSheet.get_Range("a" + row, "m" + row);
                                
                                Int16 i = 0;

                                foreach (Microsoft.Office.Interop.Excel.Range item in rg)
                                {
                                    
                                    try 
                                    {
                                        arr[i] = item.Value.ToString().Trim();
                                        i += 1;

                                    }
                                    catch {arr[0] = "zero";}
                                }
                                dataGridView1.Rows.Add(arr);
                                row++;
                            }
                            MessageBox.Show("OK");


                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file" + ex.Message);
                }
            }
        }
    }
}
