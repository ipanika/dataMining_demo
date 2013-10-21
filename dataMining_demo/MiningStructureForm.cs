using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.AnalysisServices;
using System.Data.SqlClient;

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
            DataSourceView dsv = new DataSourceView();
            DataSet ds;

            dataGridView1.Rows.Clear();

            checkedListBox1.Items.Clear();

            dsv = db.DataSourceViews.FindByName(comboBox1.Text);
            if (dsv != null)
            {
                ds = dsv.Schema;
                int i = 0;
                while (i < dsv.Schema.Tables[0].Columns.Count)
                {

                    // заполнение комбинированного dataGridView1 из представления dsv
                    DataGridViewRow dvr = (DataGridViewRow)dataGridView1.Rows[0].Clone();

                    dvr.Cells[0].Value = dsv.Schema.Tables[0].Columns[i].ColumnName;

                    DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)dvr.Cells[1];
                    chk.Value = true;

                    chk = (DataGridViewCheckBoxCell)dvr.Cells[2];
                    chk.Value = false;

                    // запрос к БД для получения типов данных
                    DataTable dt = new DataTable();

                    // создать соединение с БД
                    SqlConnection cn = new SqlConnection("Data Source=localhost; Initial Catalog=demo_source; Integrated Security=true");
                    if (cn.State == ConnectionState.Closed)
                        cn.Open();

                    SqlDataAdapter sqlDA = new SqlDataAdapter("select DISTINCT [data_type_name] FROM [demo_data_content]", cn);
                    sqlDA.Fill(dt);

                    DataGridViewComboBoxCell cmbbx = (DataGridViewComboBoxCell)dvr.Cells[3];
                    cmbbx.DataSource = dt;
                    cmbbx.DisplayMember = "data_type_name";

                    // запрос к БД для получения типов содержимого
                    dt = new DataTable();
                    sqlDA = new SqlDataAdapter("select DISTINCT [data_content_name] FROM [demo_data_content]", cn);// WHERE [data_type_name] = '" + cmbbx.Value.ToString() + "'
                    sqlDA.Fill(dt);

                    cmbbx = (DataGridViewComboBoxCell)dvr.Cells[4];
                    cmbbx.DataSource = dt;
                    cmbbx.DisplayMember = "data_content_name";

                    // добавление строки в dataGridView
                    dataGridView1.Rows.Add(dvr);

                    checkedListBox1.CheckOnClick = true;
                    checkedListBox1.Items.Add(dsv.Schema.Tables[0].Columns[i].ColumnName);
                    checkedListBox1.SetItemChecked(i, true);
                    
                    i += 1;
                }                    
            }
        }

        private void MiningStructureForm_Load(object sender, EventArgs e)
        {
            svr.Connect("localhost");

            if ((svr != null) && (svr.Connected))
                db = svr.Databases.FindByName("demo_DM");
            
            // создать соединение с БД
            SqlConnection cn = new SqlConnection("Data Source=localhost; Initial Catalog=demo_source; Integrated Security=true");
            if (cn.State == ConnectionState.Closed)
                cn.Open();

            DataTable dt = new DataTable();
            // загрузка имеющихся представлений ИАД
            SqlDataAdapter sqlDA = new SqlDataAdapter("select [dsv_name] FROM [demo_dsv]", cn);
            sqlDA.Fill(dt);

            comboBox1.DataSource = dt;
            comboBox1.DisplayMember = "dsv_name";

            dataGridView1.Rows.Clear();

            DataSourceView dsv = new DataSourceView();
            DataSet ds;

            dsv = db.DataSourceViews.FindByName(comboBox1.Text);
            if (dsv != null)
            {
                ds = dsv.Schema;
                int i = 0;
                checkedListBox1.Items.Clear();
                while (i < dsv.Schema.Tables[0].Columns.Count)
                {
                    // заполнение комбинированного dataGridView1 из представления dsv
                    DataGridViewRow dvr = (DataGridViewRow)dataGridView1.Rows[0].Clone();

                    dvr.Cells[0].Value = dsv.Schema.Tables[0].Columns[i].ColumnName;

                    DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)dvr.Cells[1];
                    chk.Value = true;
                    
                    chk = (DataGridViewCheckBoxCell) dvr.Cells[2];
                    chk.Value = false;
                    
                    // запрос к БД для получения типов данных
                    dt = new DataTable();
                    sqlDA = new SqlDataAdapter("select DISTINCT [data_type_name] FROM [demo_data_content]", cn);
                    sqlDA.Fill(dt);

                    DataGridViewComboBoxCell cmbbx = (DataGridViewComboBoxCell) dvr.Cells[3];
                    cmbbx.DataSource = dt;
                    cmbbx.DisplayMember = "data_type_name";
                    
                    // запрос к БД для получения типов содержимого
                    dt = new DataTable();
                    sqlDA = new SqlDataAdapter("select DISTINCT [data_content_name] FROM [demo_data_content]", cn);// WHERE [data_type_name] = '" + cmbbx.Value.ToString() + "'
                    sqlDA.Fill(dt);

                    cmbbx = (DataGridViewComboBoxCell)dvr.Cells[4];
                    cmbbx.DataSource = dt;
                    cmbbx.DisplayMember = "data_content_name";

                    // добавление строки в dataGridView
                    dataGridView1.Rows.Add(dvr);

                    checkedListBox1.CheckOnClick = true;
                    checkedListBox1.Items.Add(dsv.Schema.Tables[0].Columns[i].ColumnName);
                    checkedListBox1.SetItemChecked(i, true);

                    i += 1;
                }
            }
            dataGridView1.EditMode = DataGridViewEditMode.EditOnEnter;

            comboBox2.DataSource = null;
            comboBox2.DataSource = checkedListBox1.CheckedItems;

        }

        private void checkedListBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            comboBox2.DataSource = null;
            comboBox2.DataSource = checkedListBox1.CheckedItems;
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            String strName = textBox1.Text;
            String dsvName = comboBox1.Text;

            // Initialize a new mining structure
            MiningStructure ms = new MiningStructure(strName, strName);
            ms.Source = new DataSourceViewBinding(dsvName);

            var colForMStr = new List<ScalarMiningStructureColumn>();
            int i = 0;
            /*
            // создание массива с именами столбцов, выбранных для представления
            foreach (object obj in checkedListBox1.CheckedItems)
            {
                ScalarMiningStructureColumn colItem = new ScalarMiningStructureColumn(obj.ToString(), obj.ToString());
                //colItem.Type = MiningStructureColumnTypes.Long;
                //colItem.Content = MiningStructureColumnContents.Key;
                if (obj.ToString() == comboBox2.Text)
                {
                    colItem.IsKey = true;
                    colItem.Content = MiningStructureColumnContents.Key;
                }
                colForMStr.Add(colItem);
                ms.Columns.Add(colItem);
                i += 1;
            }*/

            for (i = 0; i<dataGridView1.Rows.Count-1; i++)
            //foreach (DataGridViewRow drv in dataGridView1.Rows)
            {
                DataGridViewRow drv = dataGridView1.Rows[i];
                // если параметр отмечен для входа, то для ее параметров создать 
                // столбец в структуре ИАД
                if (drv.Cells[1].Value.Equals(true))
                {
                    string rowName = drv.Cells[0].Value.ToString();
                    ScalarMiningStructureColumn colItem = new ScalarMiningStructureColumn(rowName, rowName);
                    
                    // если параметр отмечен как ключ:
                    if (drv.Cells[2].Value.Equals(true))
                    {
                        colItem.IsKey = true;
                        colItem.Content = MiningStructureColumnContents.Key;
                    }

                    switch (drv.Cells[3].Value.ToString())
                    {
                        case "TEXT":
                            colItem.Type = MiningStructureColumnTypes.Text;
                            colItem.KeyColumns.Add("SourceData$", rowName, System.Data.OleDb.OleDbType.WChar);
                            break;
                        case "LONG":
                            colItem.Type = MiningStructureColumnTypes.Long;
                            colItem.KeyColumns.Add("SourceData$", rowName, System.Data.OleDb.OleDbType.Integer);
                            break;
                        case "DOUBLE":
                            colItem.Type = MiningStructureColumnTypes.Double;
                            colItem.KeyColumns.Add("SourceData$", rowName, System.Data.OleDb.OleDbType.Double);
                            break;
                        case "DATE":
                            colItem.Type = MiningStructureColumnTypes.Date;
                            colItem.KeyColumns.Add("SourceData$", rowName, System.Data.OleDb.OleDbType.Date);
                            break;
                    };

                    switch (drv.Cells[4].Value.ToString())
                    {
                        case "CONTINUOUS":
                            colItem.Content = MiningStructureColumnContents.Continuous;
                            break;
                        case "DISCRETE":
                            colItem.Content = MiningStructureColumnContents.Discrete;
                            break;
                        case "DISCRETIZED":
                            colItem.Content = MiningStructureColumnContents.Discretized;
                            break;
                        case "KEY":
                            colItem.Content = MiningStructureColumnContents.Key;
                            break;
                    }

                    colForMStr.Add(colItem);
                    ms.Columns.Add(colItem);
                };
                

            }
            

            // Create the columns of the mining structure 
            // setting the type, content and data binding

            ////// User Id column
            //ScalarMiningStructureColumn col1 = new ScalarMiningStructureColumn("ID", "ID");
            //col1.Type = MiningStructureColumnTypes.Long;
            //col1.Content = MiningStructureColumnContents.Key;
            //col1.IsKey = true;
            ////// Add data binding to the column
            //col1.KeyColumns.Add("SourceData$", "ID", System.Data.OleDb.OleDbType.Integer);
            ////// Add the column to the mining structure
            //ms.Columns.Add(col1);

            ////// Generation column
            //ScalarMiningStructureColumn col2 = new ScalarMiningStructureColumn("Marital Status", "Marital Status");
            //col2.Type = MiningStructureColumnTypes.Text;
            //col2.Content = MiningStructureColumnContents.Discrete;
            //// Add data binding to the column
            //col2.KeyColumns.Add("SourceData$", "Marital Status", System.Data.OleDb.OleDbType.WChar);
            //// Add the column to the mining structure
            //ms.Columns.Add(col2);

            //ScalarMiningStructureColumn col3 = new ScalarMiningStructureColumn("Gender", "Gender");
            //col3.Type = MiningStructureColumnTypes.Text;
            //col3.Content = MiningStructureColumnContents.Discrete;
            //// col3 data binding to the column
            //col3.KeyColumns.Add("SourceData$", "Gender", System.Data.OleDb.OleDbType.WChar);
            //// Add the column to the mining structure
            //ms.Columns.Add(col3);

            //ScalarMiningStructureColumn col4 = new ScalarMiningStructureColumn("Yearly Income", "Yearly Income");
            //col4.Type = MiningStructureColumnTypes.Long;
            //col4.Content = MiningStructureColumnContents.Discretized;
            //// col3 data binding to the column
            //col4.KeyColumns.Add("SourceData$", "Yearly Income", System.Data.OleDb.OleDbType.Integer);
            //// Add the column to the mining structure
            //ms.Columns.Add(col4);

            //ScalarMiningStructureColumn col5 = new ScalarMiningStructureColumn("Children", "Children");
            //col5.Type = MiningStructureColumnTypes.Long;
            //col5.Content = MiningStructureColumnContents.Discrete;
            //// col3 data binding to the column
            //col5.KeyColumns.Add("SourceData$", "Children", System.Data.OleDb.OleDbType.Integer);
            //// Add the column to the mining structure
            //ms.Columns.Add(col5);

            //ScalarMiningStructureColumn col6 = new ScalarMiningStructureColumn("Education", "Education");
            //col6.Type = MiningStructureColumnTypes.Text;
            //col6.Content = MiningStructureColumnContents.Discrete;
            //// col3 data binding to the column
            //col6.KeyColumns.Add("SourceData$", "Education", System.Data.OleDb.OleDbType.WChar);
            //// Add the column to the mining structure
            //ms.Columns.Add(col6);

            //ScalarMiningStructureColumn col7 = new ScalarMiningStructureColumn("Occupation", "Occupation");
            //col7.Type = MiningStructureColumnTypes.Text;
            //col7.Content = MiningStructureColumnContents.Discrete;
            //// col3 data binding to the column
            //col7.KeyColumns.Add("SourceData$", "Occupation", System.Data.OleDb.OleDbType.WChar);
            //// Add the column to the mining structure
            //ms.Columns.Add(col7);

            //ScalarMiningStructureColumn col8 = new ScalarMiningStructureColumn("Home Owner", "Home Owner");
            //col8.Type = MiningStructureColumnTypes.Text;
            //col8.Content = MiningStructureColumnContents.Discrete;
            //// col3 data binding to the column
            //col8.KeyColumns.Add("SourceData$", "Home Owner", System.Data.OleDb.OleDbType.WChar);
            //// Add the column to the mining structure
            //ms.Columns.Add(col8);

            //ScalarMiningStructureColumn col9 = new ScalarMiningStructureColumn("Cars", "Cars");
            //col9.Type = MiningStructureColumnTypes.Long;
            //col9.Content = MiningStructureColumnContents.Discrete;
            //// col3 data binding to the column
            //col9.KeyColumns.Add("SourceData$", "Cars", System.Data.OleDb.OleDbType.Integer);
            //// Add the column to the mining structure
            //ms.Columns.Add(col9);

            //ScalarMiningStructureColumn col10 = new ScalarMiningStructureColumn("Commute Distance", "Commute Distance");
            //col10.Type = MiningStructureColumnTypes.Text;
            //col10.Content = MiningStructureColumnContents.Discrete;
            //// col3 data binding to the column
            //col10.KeyColumns.Add("SourceData$", "Commute Distance", System.Data.OleDb.OleDbType.WChar);
            //// Add the column to the mining structure
            //ms.Columns.Add(col10);

            //ScalarMiningStructureColumn col11 = new ScalarMiningStructureColumn("Region", "Region");
            //col11.Type = MiningStructureColumnTypes.Text;
            //col11.Content = MiningStructureColumnContents.Discrete;
            //// col3 data binding to the column
            //col11.KeyColumns.Add("SourceData$", "Region", System.Data.OleDb.OleDbType.WChar);
            //// Add the column to the mining structure
            //ms.Columns.Add(col11);

            //ScalarMiningStructureColumn col12 = new ScalarMiningStructureColumn("Age", "Age");
            //col12.Type = MiningStructureColumnTypes.Long;
            //col12.Content = MiningStructureColumnContents.Continuous;
            //// col3 data binding to the column
            //col12.KeyColumns.Add("SourceData$", "Age", System.Data.OleDb.OleDbType.Integer);
            //// Add the column to the mining structure
            //ms.Columns.Add(col12);

            //ScalarMiningStructureColumn col13 = new ScalarMiningStructureColumn("BikeBuyer", "BikeBuyer");
            //col13.Type = MiningStructureColumnTypes.Text;
            //col13.Content = MiningStructureColumnContents.Discrete;
            //// col3 data binding to the column
            //col13.KeyColumns.Add("SourceData$", "BikeBuyer", System.Data.OleDb.OleDbType.WChar);
            //// Add the column to the mining structure
            //ms.Columns.Add(col13);

            // Add the mining structure to the database
            db.MiningStructures.Add(ms);
            ms.Update();

            this.Close();

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            MessageBox.Show("CellContentClick");
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            MessageBox.Show("CellValueChanged");
        }

        private void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            MessageBox.Show("CurrentCellChanged");
        }
        

    }
}
