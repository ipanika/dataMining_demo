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
    public partial class FormSelection : Form
    {
        public FormSelection()
        {
            InitializeComponent();
        }

        private void FormSelection_Load(object sender, EventArgs e)
        {
            // получение списка доступных представлений:
            SqlConnection cn = new SqlConnection("Data Source=localhost; Initial Catalog=demo_dm; Integrated Security=true");
            cn.Open();

            DataTable dt = new DataTable();

            SqlDataAdapter sqlDA = new SqlDataAdapter("SELECT [name] FROM [data_source_views]", cn);
            sqlDA.Fill(dt);

            comboBox1.DataSource = dt;
            comboBox1.DisplayMember = "name";
            string dsvName = comboBox1.Text;
            string filter = textBox1.Text;
            dataGridView1.AllowUserToAddRows = false;

            fillDataGridView(dsvName, filter);
            
        }

        // функция заполняет dataGridView выборкой данных из представления
        private void fillDataGridView(string dsvName, string filter)
        {
            dataGridView1.DataSource = null;

            // получение списка доступных представлений:
            //string strQuery = "";
            SqlConnection cn = new SqlConnection("Data Source=localhost; Initial Catalog=demo_dm; Integrated Security=true");
            cn.Open();

            try
            {
                SqlCommand cmd = cn.CreateCommand();
                cmd.Connection = cn;
                cmd.CommandText = "SELECT [column_name] " +
                      "FROM [dbo].[dsv_columns] INNER JOIN [dbo].[data_source_views] " +
                      "ON [dbo].[data_source_views].name = '" + dsvName + "' " +
                      "AND [dbo].[dsv_columns].id_dsv = dbo.data_source_views.id_dsv";

                List<string> colNames = new List<string>();
                List<string> colNumbers = new List<string>();

                try 
                {
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        //strQuery += " [" + reader.GetString(0) + "],";
                        string colNm = reader.GetString(0);

                        colNames.Add(colNm);

                        SqlConnection cn2 = new SqlConnection("Data Source=localhost; Initial Catalog=DW; Integrated Security=true");
                        cn2.Open();
                        SqlCommand sqlCmd = new SqlCommand();

                        sqlCmd.CommandText = "SELECT numLine FROM BalanceLine WHERE description = '" + colNm + "'";
                        sqlCmd.Connection = cn2;
                        string numLine = "";
                        object obj = sqlCmd.ExecuteScalar();

                        try
                        {
                            if (obj != null)
                            {
                                numLine = obj.ToString();
                                colNumbers.Add("[" + numLine + "]");
                            }
                            else
                                colNumbers.Add(" ");
                        }
                        catch (Exception e1)
                        {
                            MessageBox.Show(e1.Message);
                        }
                    }
                }
                catch (Exception e1)
                {
                    MessageBox.Show(e1.Message);
                }
                //if (strQuery != "")
                //    strQuery = strQuery.Substring(0, strQuery.Length - 1);
                
                //if (strQuery != "")
                if (colNames.Count > 0)
                {
                    cn = new SqlConnection("Data Source=localhost; Initial Catalog=DW; Integrated Security=true");
                    cn.Open();

                    string strSelect = "SELECT ";// +strQuery + " " + "FROM SourceData$";
                    for (int i = 0; i < colNames.Count; i++)
                    {
                        if (colNumbers[i] != " ")
                            strSelect += " " + colNumbers[i] + " AS '" + colNames[i] + "',";
                        else
                            strSelect += " " + colNames[i] + ",";
                    }

                    strSelect = strSelect.Substring(0, strSelect.Length - 1);

                    strSelect += "FROM " +
                                    "(SELECT     BalanceReport.companyID, Company.Name, BalanceReport.YearID,  " +
                                    "BalanceLine.numLine, BalanceReport.PeriodEnd FROM BalanceReport " +
                                    "INNER JOIN BalanceLine ON BalanceReport.balanceLineID = BalanceLine.lineID " +
                                    "INNER JOIN Company ON BalanceReport.CompanyID = Company.CompanyID " +
                                    "INNER JOIN Year ON BalanceReport.YearID = Year.YearID) p " +
                                    "PIVOT (sum (PeriodEnd) FOR numline IN (";
                    for (int i = 0; i < colNumbers.Count; i++)
                    {
                        if (colNumbers[i] != " ")
                            strSelect += " " + colNumbers[i] + ",";
                    }
                    
                    strSelect = strSelect.Substring(0, strSelect.Length - 1);

                    strSelect += ") ) AS pvt";

                    if (filter != "")
                    {
                        strSelect += " WHERE " + filter;
                    }

                    strSelect += " ORDER BY pvt.CompanyID";

                    SqlDataAdapter sqlDA;
                    sqlDA = new SqlDataAdapter(strSelect, cn);

                    DataTable dt = new DataTable();
                    sqlDA.Fill(dt);

                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            
            
            
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string dsvName = comboBox1.Text;
            string filter = textBox1.Text;
            
            fillDataGridView(dsvName, filter);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string dsvName = comboBox1.Text;
            string filter = textBox1.Text;

            fillDataGridView(dsvName, filter);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SqlConnection cn = new SqlConnection("Data Source=localhost; Initial Catalog=demo_dm; Integrated Security=true");
            if (cn.State == ConnectionState.Closed)
                cn.Open();

            // заполнение таблицы selections
            SqlCommand sqlCmd = new SqlCommand();//("INSERT INTO [selections]  VALUES ('" + dsvName + "')", cn);
            sqlCmd.CommandText = "SELECT id_dsv FROM data_source_views WHERE name = '" +  comboBox1.Text + "'";
            sqlCmd.Connection = cn;
            string idDSV = "";
            try
            {
                idDSV = sqlCmd.ExecuteScalar().ToString();
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message);
            }
            string selName = textBox2.Text;
            string filter = textBox1.Text;

            //проверка вставляемых значений на наличие кавычечк
            selName = checkQuotes(selName);
            filter = checkQuotes(filter);

            sqlCmd.CommandText = "INSERT INTO [selections] ([id_dsv], [name], [filter])  VALUES ('" + idDSV+ "', '" +selName + "', '" + filter+ "')";
            try
            {
                sqlCmd.ExecuteNonQuery();
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message);
            }


            
            // получение id созданной выборки
            sqlCmd.CommandText = "SELECT id_selection FROM selections WHERE name = '" + selName + "'";
            sqlCmd.Connection = cn;
            string idSel = "";
            try
            {
                idSel = sqlCmd.ExecuteScalar().ToString();
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message);
            }
            string dsvName = textBox2.Text;

           

            // заполнение таблицы selection_content

            // получение списка идентификаторов столбцов для текущей выборки 
            DataTable dt = new DataTable();
            SqlDataAdapter sqlDA = new SqlDataAdapter("SELECT [id_column] FROM [dsv_columns] WHERE id_dsv = '" + idDSV + "'", cn);
            sqlDA.Fill(dt);

            // шаблон многострочного INSERT:
            // INSERT INTO COL_1, COL_2, COL_3 
            //      VALUES (VAL_11, VAL_12, VAL_13),
            //      (VAL_21, VAL_22, VAL_23), .....

            int rowCount = dataGridView1.RowCount;
            int colCount = dataGridView1.ColumnCount;

            string strInsert = "";
            
            int i;
            for (i = 0; i < rowCount; i++)
            {

                // сохранение записи о сохраняемой новой стркое
                sqlCmd.CommandText = "INSERT INTO [selection_rows] ([id_selection])  VALUES ('" + idSel + "')";
                try
                {
                    sqlCmd.ExecuteNonQuery();
                }
                catch (Exception e1)
                {
                    MessageBox.Show(e1.Message);
                }

                // получение id созданной выборки
                sqlCmd.CommandText = "SELECT TOP 1 id_row FROM selection_rows WHERE id_selection = " + idSel + " ORDER BY id_row DESC";
                sqlCmd.Connection = cn;
                string idRow = "";
                try
                {
                    idRow = sqlCmd.ExecuteScalar().ToString();
                }
                catch (Exception e1)
                {
                    MessageBox.Show(e1.Message);
                }


                // формируется строка значений одной записи
                int j;
                for (j = 0; j < colCount; j++)
                {
                    strInsert += " (" + idRow.ToString() + ","; //id_row
                    strInsert += " " + dt.Rows[j][0].ToString() + ","; // id_column
                    //if (dataGridView1.Rows[i].Cells[j].Value != null)
                        strInsert += " '" + dataGridView1.Rows[i].Cells[j].Value + "'),"; // column_value
                    //else
                        //strInsert += " null'),"; // column_value
                }
            }

            strInsert = strInsert.Substring(0, strInsert.Length - 1);

            sqlCmd.CommandText = "insert into [selection_content] values " + strInsert;
            sqlCmd.Connection = cn;
            try
            {
                sqlCmd.ExecuteNonQuery();
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message);
            }

            this.Close();

        }

        private string checkQuotes(string word)
        {
            string newWord = "";

            for (int i = 0; i < word.Length; i++ )
            {
                newWord += word[i];
                if (word[i].ToString() == "'")
                {
                    newWord += word[i];
                }

            }

            return newWord;
        }
    }
}
