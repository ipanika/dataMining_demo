using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.AnalysisServices.AdomdClient;
using Microsoft.AnalysisServices;
using System.Data.SqlClient;

namespace dataMining_demo
{
    public partial class FormMiningStructure : Form
    {

        Server svr = new Server();
        Database db = new Database();

        public FormMiningStructure()
        {
            InitializeComponent();
        }

        
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selName = comboBox1.Text;

            //dataGridView1.AllowUserToAddRows = true;
            dataGridView1.Rows.Clear();

            // функция заполняет DataGridView
            fillDataGridView(selName);

            //dataGridView1.AllowUserToAddRows = false;

        }

        private void MiningStructureForm_Load(object sender, EventArgs e)
        {
            
            // создать соединение с БД
            SqlConnection cn = new SqlConnection("Data Source=localhost; Initial Catalog=demo_dm; Integrated Security=true");
            if (cn.State == ConnectionState.Closed)
                cn.Open();

            DataTable dt = new DataTable();
            // загрузка имеющихся представлений ИАД
            SqlDataAdapter sqlDA = new SqlDataAdapter("select [name] FROM [selections]", cn);
            sqlDA.Fill(dt);

            comboBox1.DataSource = dt;
            comboBox1.DisplayMember = "name";

            string selName = comboBox1.Text;

            dataGridView1.Rows.Clear();

            // функция заполняет DataGridView
            fillDataGridView(selName);

            //DataSourceView dsv = new DataSourceView();
            //DataSet ds;

            //dsv = db.DataSourceViews.FindByName(comboBox1.Text);
            //if (dsv != null)
            //{
            //    ds = dsv.Schema;
            //    int i = 0;
            //    while (i < dsv.Schema.Tables[0].Columns.Count)
            //    {
            //        // заполнение комбинированного dataGridView1 из представления dsv
            //        DataGridViewRow dvr = (DataGridViewRow)dataGridView1.Rows[0].Clone();

            //        string colName = dsv.Schema.Tables[0].Columns[i].ColumnName;
            //        dvr.Cells[0].Value = colName;

            //        DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)dvr.Cells[1];
            //        chk.Value = true;
                    
            //        chk = (DataGridViewCheckBoxCell) dvr.Cells[2];
            //        chk.Value = false;
                    
            //        // заполнение столбца типа данных:
            //        // выбор из схемы БД информации о типе данных столбца
            //        dt = new DataTable();
                    

            //        fillDataTable(colName, dt, cn);

            //        int j = 0;
            //        foreach (DataRow dr in dt.Rows)
            //        {
            //            switch(dr[0].ToString())
            //            {
            //                case "nvarchar":
            //                case "text":
            //                    dvr.Cells[3].Value = "TEXT";
            //                    break;
            //                case "float":
            //                    dvr.Cells[3].Value = "DOUBLE";
            //                    break;
            //                case "int":
            //                case "bigint":
            //                    dvr.Cells[3].Value = "LONG";
            //                    break;
            //                case "date": 
            //                case "datetime":
            //                case "datetime2":
            //                    dvr.Cells[3].Value = "DATE";
            //                    break;
            //            }
            //            j++;
            //        };                    

            //        // запрос к БД для получения типов содержимого
            //        dt = new DataTable();
            //        sqlDA = new SqlDataAdapter("select DISTINCT [data_content_name] FROM [demo_data_content]", cn);// WHERE [data_type_name] = '" + cmbbx.Value.ToString() + "'
            //        sqlDA.Fill(dt);

            //        DataGridViewComboBoxCell cmbbx = (DataGridViewComboBoxCell)dvr.Cells[4];
            //        cmbbx.DataSource = dt;
            //        cmbbx.DisplayMember = "data_content_name";

            //        // добавление строки в dataGridView
            //        dataGridView1.Rows.Add(dvr);

            //        i += 1;
            //    }
            //}

            dataGridView1.EditMode = DataGridViewEditMode.EditOnEnter;

            
        }
                

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.AllowUserToAddRows = false;
            
            string strName = textBox1.Text;
            string selName = comboBox1.Text;
            string test_ratio = textBox2.Text;

            // запись в таблицу structures информации о создаваемой структуре
            SqlConnection cn = new SqlConnection("Data Source=localhost; Initial Catalog=demo_dm; Integrated Security=true");
            if (cn.State == ConnectionState.Closed)
                cn.Open();
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandText = "SELECT id_selection FROM selections WHERE name = '" + selName + "'";
            sqlCmd.Connection = cn;
            string idSel = sqlCmd.ExecuteScalar().ToString();
                        
            sqlCmd.CommandText = "INSERT INTO [structures] VALUES ('" + idSel + "', '" + strName + "', '" + test_ratio + "')";
            sqlCmd.ExecuteNonQuery();

            svr.Connect("localhost");

            if ((svr != null) && (svr.Connected))
                db = svr.Databases.FindByName("demo_DM");

            string dmxQuery;
            dmxQuery = "CREATE MINING STRUCTURE ["+strName+"] (";

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                DataGridViewRow drv = dataGridView1.Rows[i];
                if (drv.Cells[1].Value.Equals(true))
                {
                    // получение имени столбца
                    string parName = drv.Cells[0].Value.ToString();

                    // замена недопустимых символов
                    parName = parName.Replace(".", "_");
                    parName = parName.Replace(",", "_");

                    parName = parName.Replace(";", "_");
                    parName = parName.Replace("'", "_");

                    parName = parName.Replace("`", "_");
                    parName = parName.Replace(":", "_");

                    parName = parName.Replace("/", "_");
                    parName = parName.Replace("\\", "_");
                    parName = parName.Replace("*", "_");
                    parName = parName.Replace("|", "_");

                    parName = parName.Replace("?", "_");
                    parName = parName.Replace("\"", "_");
                    parName = parName.Replace("&", "_");
                    parName = parName.Replace("%", "_");

                    parName = parName.Replace("$", "_");
                    parName = parName.Replace("!", "_");
                    parName = parName.Replace("+", "_");
                    parName = parName.Replace("=", "_");

                    parName = parName.Replace("(", "_");
                    parName = parName.Replace(")", "_");
                    parName = parName.Replace("{", "_");
                    parName = parName.Replace("}", "_");

                    parName = parName.Replace("<", "_");
                    parName = parName.Replace(">", "_");

                    if (parName.Length > 99)
                        parName = parName.Substring(0, 99);

                    dmxQuery += " [" + parName + "]";

                    // получение типа данных столбца
                    dmxQuery += " " + drv.Cells[3].Value;

                    // получение типа содержимого столбца
                    if (drv.Cells[2].Value.Equals(true))
                        dmxQuery += " KEY,";
                    else
                        dmxQuery += " " + drv.Cells[4].Value.ToString() + ",";
                }
            }
            
            dmxQuery = dmxQuery.Substring(0, dmxQuery.Length - 1);
            dmxQuery += ")";

            if (test_ratio != "")
                dmxQuery += " WITH HOLDOUT (" + test_ratio + " PERCENT)";

            // создание соединения с сервером и команды для отправки dmx-запроса
            AdomdConnection adomdCn = new AdomdConnection();
            adomdCn.ConnectionString = "Data Source = localhost; Initial Catalog = demo_DM";
            adomdCn.Open();

            AdomdCommand adomdCmd = adomdCn.CreateCommand();
            adomdCmd.CommandText = dmxQuery;
            try
            {
                adomdCmd.Execute();
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message);
            }
            
            this.Close();

        }
        
        private void getColumnType(string colName, DataTable dt, SqlConnection cn)
        {
            cn = new SqlConnection("Data Source=localhost; Initial Catalog=DW; Integrated Security=true");
            cn.Open();
            SqlDataAdapter sqlDA = new SqlDataAdapter("SELECT t.name AS type_name FROM sys.columns AS c  " +
                                               " JOIN sys.types AS t ON c.user_type_id=t.user_type_id " +
                                               " WHERE c.object_id = OBJECT_ID('dbo.SourceData$') AND c.name = '" +
                                               colName + "'" + " ORDER BY c.column_id;", cn);
            sqlDA.Fill(dt);
        }

        private void fillDataGridView(string selName)
        {
            SqlConnection cn = new SqlConnection("Data Source=localhost; Initial Catalog=demo_dm; Integrated Security=true");
            cn.Open();

            try
            {
                // получение списка столбцов из выборки данных
                string strSelect = "SELECT column_name FROM dsv_columns JOIN  selections " + 
                            "ON dsv_columns.id_dsv = selections.id_dsv and " +
	                        "selections.name = '" + selName + "'";
                SqlDataAdapter sqlDA = new SqlDataAdapter(strSelect, cn);
                DataTable dtColumns = new DataTable();
                sqlDA.Fill(dtColumns);
                // список типов содержимого
                var contentType = new List<string>();
                
                contentType.Add("KEY");
                contentType.Add("DISCRETE");
                contentType.Add("DISCRETIZED");
                contentType.Add("CONTINUOUS");

                if (dtColumns.Rows.Count > 0)
                {
                    for (int i = 0; i < dtColumns.Rows.Count; i++)
                    {
                        // заполнение комбинированного dataGridView1 из представления dsv
                        DataGridViewRow dvr = (DataGridViewRow)dataGridView1.Rows[0].Clone();

                        string colName = dtColumns.Rows[i][0].ToString();
                        dvr.Cells[0].Value = colName;

                        DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)dvr.Cells[1];
                        chk.Value = true;

                        chk = (DataGridViewCheckBoxCell)dvr.Cells[2];
                        chk.Value = false;

                        // заполнение столбца типа данных:
                        // выбор из схемы БД информации о типе данных столбца
                        DataTable dt = new DataTable();

                        getColumnType(colName, dt, cn);

                        if (colName.Contains("Company"))
                            dvr.Cells[3].Value = "LONG";
                        else if (colName.Contains("Year"))
                            dvr.Cells[3].Value = "DATE";
                        else if (colName.Contains("Name"))
                            dvr.Cells[3].Value = "TEXT";
                        else
                            dvr.Cells[3].Value = "DOUBLE";
                        //int j = 0;
                        //foreach (DataRow dr in dt.Rows)
                        //{
                        //    switch (dr[0].ToString())
                        //    {
                        //        case "nvarchar":
                        //        case "text":
                        //            dvr.Cells[3].Value = "TEXT";
                        //            break;
                        //        case "float":
                        //            dvr.Cells[3].Value = "DOUBLE";
                        //            break;
                        //        case "int":
                        //        case "bigint":
                        //            dvr.Cells[3].Value = "LONG";
                        //            break;
                        //        case "date":
                        //        case "datetime":
                        //        case "datetime2":
                        //            dvr.Cells[3].Value = "DATE";
                        //            break;
                        //    }
                        //    j++;
                        //}

                        DataGridViewComboBoxCell cmbbx = (DataGridViewComboBoxCell)dvr.Cells[4];
                        cmbbx.DataSource = contentType;

                        dataGridView1.Rows.Add(dvr);
                    }
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

    }
}
