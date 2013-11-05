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

           
            dsv = db.DataSourceViews.FindByName(comboBox1.Text);
            if (dsv != null)
            {
                ds = dsv.Schema;
                int i = 0;
                while (i < dsv.Schema.Tables[0].Columns.Count)
                {
                    // заполнение комбинированного dataGridView1 из представления dsv
                    DataGridViewRow dvr = (DataGridViewRow)dataGridView1.Rows[0].Clone();

                    //dvr.Cells[0].Value = dsv.Schema.Tables[0].Columns[i].ColumnName;
                    //string colName = dsv.Schema.Tables[0].Columns[0].ColumnName;


                    string colName = dsv.Schema.Tables[0].Columns[i].ColumnName;
                    dvr.Cells[0].Value = colName;


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

                    dt = new DataTable();

                    fillDataTable(colName, dt, cn);

                    int j = 0;
                    foreach (DataRow dr in dt.Rows)
                    {
                        switch (dr[0].ToString())
                        {
                            case "nvarchar":
                                dvr.Cells[3].Value = "TEXT";
                                break;
                            case "float":
                                dvr.Cells[3].Value = "DOUBLE";
                                break;
                            case "int":
                            case "bigint":
                                dvr.Cells[3].Value = "LONG";
                                break;
                            case "date":
                            case "datetime":
                            case "datetime2":
                                dvr.Cells[3].Value = "DATE";
                                break;
                        }
                        j++;
                    };   

                    // запрос к БД для получения типов содержимого
                    dt = new DataTable();
                    SqlDataAdapter sqlDA = new SqlDataAdapter("select DISTINCT [data_content_name] FROM [demo_data_content]", cn);// WHERE [data_type_name] = '" + cmbbx.Value.ToString() + "'
                    sqlDA.Fill(dt);

                    DataGridViewComboBoxCell cmbbx = (DataGridViewComboBoxCell)dvr.Cells[4];
                    cmbbx.DataSource = dt;
                    cmbbx.DisplayMember = "data_content_name";

                    // добавление строки в dataGridView
                    dataGridView1.Rows.Add(dvr);
 
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
                while (i < dsv.Schema.Tables[0].Columns.Count)
                {
                    // заполнение комбинированного dataGridView1 из представления dsv
                    DataGridViewRow dvr = (DataGridViewRow)dataGridView1.Rows[0].Clone();

                    string colName = dsv.Schema.Tables[0].Columns[i].ColumnName;
                    dvr.Cells[0].Value = colName;

                    DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)dvr.Cells[1];
                    chk.Value = true;
                    
                    chk = (DataGridViewCheckBoxCell) dvr.Cells[2];
                    chk.Value = false;
                    
                    // заполнение столбца типа данных:
                    // выбор из схемы БД информации о типе данных столбца
                    dt = new DataTable();
                    

                    fillDataTable(colName, dt, cn);

                    int j = 0;
                    foreach (DataRow dr in dt.Rows)
                    {
                        switch(dr[0].ToString())
                        {
                            case "nvarchar":
                                dvr.Cells[3].Value = "TEXT";
                                break;
                            case "float":
                                dvr.Cells[3].Value = "DOUBLE";
                                break;
                            case "int":
                            case "bigint":
                                dvr.Cells[3].Value = "LONG";
                                break;
                            case "date": 
                            case "datetime":
                            case "datetime2":
                                dvr.Cells[3].Value = "DATE";
                                break;
                        }
                        j++;
                    };                    

                    // запрос к БД для получения типов содержимого
                    dt = new DataTable();
                    sqlDA = new SqlDataAdapter("select DISTINCT [data_content_name] FROM [demo_data_content]", cn);// WHERE [data_type_name] = '" + cmbbx.Value.ToString() + "'
                    sqlDA.Fill(dt);

                    DataGridViewComboBoxCell cmbbx = (DataGridViewComboBoxCell)dvr.Cells[4];
                    cmbbx.DataSource = dt;
                    cmbbx.DisplayMember = "data_content_name";

                    // добавление строки в dataGridView
                    dataGridView1.Rows.Add(dvr);

                    i += 1;
                }
            }
            dataGridView1.EditMode = DataGridViewEditMode.EditOnEnter;
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


            for (i = 0; i<dataGridView1.Rows.Count-1; i++)

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

            
            db.MiningStructures.Add(ms);
            ms.Update();

            SqlConnection cn = new SqlConnection("Data Source=localhost; Initial Catalog=demo_source; Integrated Security=true");
                
            if (cn.State == ConnectionState.Closed)
                cn.Open();

            SqlCommand sqlCmd = new SqlCommand("INSERT INTO [demo_mstr]  VALUES ('" + strName + "', '" + dsvName + "')", cn);
            sqlCmd.ExecuteNonQuery();

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

        private void fillDataTable(string colName, DataTable dt, SqlConnection cn)
        {
            SqlDataAdapter sqlDA = new SqlDataAdapter("SELECT t.name AS type_name FROM sys.columns AS c  " +
                                               " JOIN sys.types AS t ON c.user_type_id=t.user_type_id " +
                                               " WHERE c.object_id = OBJECT_ID('dbo.SourceData$') AND c.name = '" +
                                               colName + "'" + " ORDER BY c.column_id;", cn);
            sqlDA.Fill(dt);
        }
    }
}
