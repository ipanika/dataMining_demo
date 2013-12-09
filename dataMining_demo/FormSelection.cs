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

        public static DataTable glob_dt; // DataTeble для передачи dataGridView в форму переразметки
        public static int glob_relabelColumnIndex; // индекс переразмечиваемого столбца, устанавливается 
                                                   // в форме переразметки, в текущей форме необходим для
                                                   // обновления содержимого 
        public static List<string> glob_columnNames = new List<string>(); // список названий столбцов, доступных для переразметки
        public static List<string> glob_previousLabels = new List<string>(); // список заменяемых меток
        public static List<string> glob_currentLabels = new List<string>();  // список новых (заменяющих) меток
        public static ComboBox cmb = new ComboBox();

        public FormSelection()
        {
            InitializeComponent();
        }

        private void FormSelection_Load(object sender, EventArgs e)
        {
            // получение списка доступных представлений:
            SqlConnection cn = new SqlConnection(FormMain.app_connectionString);
            cn.Open();

            DataTable dt = new DataTable();

            string strQuery = "SELECT [data_source_views].[name] FROM [data_source_views]"+
                                " INNER JOIN relations ON [data_source_views].[id_dsv] = " +
                                " relations.id_dsv INNER JOIN tasks ON relations.id_task =  tasks.id_task " + 
                                " WHERE tasks.task_type = "+FormMain.taskType.ToString(); 
            SqlDataAdapter sqlDA = new SqlDataAdapter(strQuery, cn);
            sqlDA.Fill(dt);

            comboBox1.DataSource = dt;
            comboBox1.DisplayMember = "name";
            string dsvName = comboBox1.Text;
            
            dataGridView1.AllowUserToAddRows = false;
            
            if (FormMain.taskType == 2)
            {
                textBox1.Visible = false;
                // добавление нового выпадающего списка на форму
                initializeCmb(cmb);
            }
            // заполнение dataGridView
            fillDataGridView(dsvName);

            // если решается задача прогнозирования, то скрыть кнопку "Переразметить"
            if (FormMain.taskType == 2)
                button3.Visible = false; 
            
        }

        // функция заполняет dataGridView выборкой данных из представления
        private void fillDataGridView(string dsvName)
        {
            dataGridView1.DataSource = null;
            string filter = "";

            // обращаемся к методу заполнения в зависимости от решаемой задачи
            if (FormMain.taskType == 1)
            {
                filter = textBox1.Text;
            }
            else
            {
                filter = cmb.Text;
                dataGridView1.AllowUserToAddRows = true;
                dataGridView1.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
            }

            // запрос данных из хранилища и заполнение ими dataGridView
            sqlSelect(dsvName, filter);

        }
        
        // добавление нового выпадающего списка на форму
        private void initializeCmb(ComboBox cmb)
        {
            cmb.FormattingEnabled = true;
            cmb.Location = new System.Drawing.Point(12, 120);
            cmb.Name = "comboBox2";
            cmb.Size = new System.Drawing.Size(313, 20);
            cmb.TabIndex = 4;
            cmb.SelectedIndexChanged += new System.EventHandler(this.cmb_SelectedIndexChanged);

            // получение списка доступных предприятий:
            SqlConnection cn2 = new SqlConnection(FormMain.dw_connectionString);
            cn2.Open();
            SqlCommand sqlCmd = new SqlCommand();
            // получение списка предприятий
            SqlDataAdapter sqlDA = new SqlDataAdapter("SELECT name FROM Company", cn2);

            DataTable dt = new DataTable();
            sqlDA.Fill(dt);

            cmb.DataSource = dt;
            cmb.DisplayMember = "name";
            
            Controls.Add(cmb);
            
        }

        private void sqlSelect(string dsvName, string filter)
        {
            try
            {
                // получение списка доступных представлений:
                SqlConnection cn = new SqlConnection(FormMain.app_connectionString);
                cn.Open();

                // получение имен столбцов для текущего представления
                SqlCommand cmd = cn.CreateCommand();
                cmd.Connection = cn;
                cmd.CommandText = "SELECT [column_name] " +
                      " FROM [dbo].[dsv_columns] INNER JOIN [dbo].[data_source_views] " +
                      " ON [dbo].[data_source_views].name = '" + dsvName + "' " +
                      " AND [dbo].[dsv_columns].id_dsv = dbo.data_source_views.id_dsv";

                List<string> colNames = new List<string>();
                List<string> colNumbers = new List<string>();
                glob_columnNames.Clear();

                try
                {
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string colNm = reader.GetString(0);

                        colNames.Add(colNm);
                        glob_columnNames.Add(colNm);

                        SqlConnection cn2 = new SqlConnection(FormMain.dw_connectionString);
                        cn2.Open();
                        SqlCommand sqlCmd = new SqlCommand();

                        // получение номеров строк баланса 
                        sqlCmd.CommandText = " SELECT numLine FROM BalanceLine WHERE description = '" + colNm + "'";
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
                    cn = new SqlConnection(FormMain.dw_connectionString);
                    cn.Open();

                    // формирование pivot-запроса:
                    string strSelect = " SELECT ";
                    // выбор заголовков столбцов
                    for (int i = 0; i < colNames.Count; i++)
                    {
                        if (colNumbers[i] != " ")
                            strSelect += " " + colNumbers[i] + " AS '" + colNames[i] + "',";
                        else
                            strSelect += " " + colNames[i] + ",";
                    }

                    strSelect = strSelect.Substring(0, strSelect.Length - 1);

                    strSelect += " FROM " +
                                    "( SELECT     BalanceReport.companyID, Company.companyName, BalanceReport.YearID,  " +
                                    " BalanceLine.numLine, cast(BalanceReport.PeriodEnd as nvarchar) as PeriodEnd FROM BalanceReport " +
                                    " INNER JOIN BalanceLine ON BalanceReport.balanceLineID = BalanceLine.lineID " +
                                    " INNER JOIN Company ON BalanceReport.CompanyID = Company.CompanyID " +
                                    " INNER JOIN Year ON BalanceReport.YearID = Year.YearID ) p " +
                                    " PIVOT (max(PeriodEnd) FOR numline IN (";
                    for (int i = 0; i < colNumbers.Count; i++)
                    {
                        if (colNumbers[i] != " ")
                            strSelect += " " + colNumbers[i] + ",";
                    }

                    strSelect = strSelect.Substring(0, strSelect.Length - 1);

                    strSelect += ") ) AS pvt";

                    if (filter != "")
                    {
                        strSelect += " WHERE ";
                        // модификация запроса для кластеризации
                        if (FormMain.taskType == 1)
                        {
                            strSelect += filter;
                            strSelect += " ORDER BY pvt.CompanyID";
                        }
                        // модификация запроса для прогнозирования
                        else
                            strSelect += " name = '" + filter + "'";
                    }


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
            fillDataGridView(dsvName);

        }

        private void cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            string dsvName = comboBox1.Text;

            fillDataGridView(dsvName);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string dsvName = comboBox1.Text;

            fillDataGridView(dsvName);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SqlConnection cn = new SqlConnection(FormMain.app_connectionString);
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

            if (FormMain.taskType == 2)
                rowCount -= 1;

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

        // нажатие на кнопку "Переразметить"
        private void button3_Click(object sender, EventArgs e)
        {
            
            glob_dt = (DataTable) dataGridView1.DataSource;
            FormRelabel form = new FormRelabel();

            form.Show();
        }

        private void FormSelection_Activated(object sender, EventArgs e)
        {
            // если в массиве glob_columnNames есть данные для переразметки, то обновить dataGridView
            if (glob_previousLabels.Count != 0 && glob_relabelColumnIndex >= 0)
            {
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    string previousLabel = dataGridView1.Rows[i].Cells[glob_relabelColumnIndex].Value.ToString();
                    string newLabel = relabelString(previousLabel);

                    dataGridView1.Rows[i].Cells[glob_relabelColumnIndex].Value = newLabel;
                }
                
                // очищаем данных глобальных переменных
                glob_relabelColumnIndex = -1;
                glob_previousLabels.Clear();
                glob_currentLabels.Clear();
            }
        }

        private string relabelString(string previous)
        {
            int index = glob_previousLabels.Count;

            for (int i = 0; i < index; i++)
            {
                if (previous == glob_previousLabels[i].ToString())
                    return glob_currentLabels[i].ToString();
            }

            return "";
        }
    }
}
