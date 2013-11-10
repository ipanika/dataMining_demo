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
            // получение списка доступных представлений:
            string strQuery = "";
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

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    strQuery += " [" + reader.GetString(0) + "],";
                }

                if (strQuery != "")
                    strQuery = strQuery.Substring(0, strQuery.Length - 1);

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            
            
            if (strQuery != "")
            {
                cn = new SqlConnection("Data Source=localhost; Initial Catalog=demo_source; Integrated Security=true");
                cn.Open();
                
                string strSelect = "SELECT " + strQuery + " " + "FROM SourceData$";
                if (filter != "")
                {
                    strSelect += " WHERE " + filter;
                }

                SqlDataAdapter sqlDA;
                sqlDA = new SqlDataAdapter(strSelect, cn);

                DataTable dt = new DataTable();
                sqlDA.Fill(dt);

                dataGridView1.DataSource = dt;
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
            string idDSV = sqlCmd.ExecuteScalar().ToString();
            string selName = textBox2.Text;
            string filter = textBox1.Text;

            sqlCmd.CommandText = "INSERT INTO [selections] ([id_dsv], [name], [filter])  VALUES ('" + idDSV+ "', '" +selName + "', '" + filter+ "')";
            sqlCmd.ExecuteNonQuery();

            // заполнение таблицы selection_content

            // получение id созданной выборки
            sqlCmd.CommandText = "SELECT id_selection FROM selections WHERE name = '" + selName + "'";
            sqlCmd.Connection = cn;
            string idSel = sqlCmd.ExecuteScalar().ToString();
            string dsvName = textBox2.Text;

            // получение списка идентификаторов столбцов для текущей выборки 
            DataTable dt = new DataTable();
            SqlDataAdapter sqlDA = new SqlDataAdapter("SELECT [id_column] FROM [dsv_columns] WHERE id_dsv = '" + idDSV + "'", cn);
            sqlDA.Fill(dt);

            //шаблон многострочного INSERT:
            // INSERT INTO COL_1, COL_2, COL_3 
            //      VALUES (VAL_11, VAL_12, VAL_13),
            //      (VAL_21, VAL_22, VAL_23), .....

            int rowCount = dataGridView1.RowCount;
            int colCount = dataGridView1.ColumnCount;

            string strInsert = "";
            
            int i;
            for (i = 0; i < rowCount; i++)
            {
                // формируется строка значений одной записи
                int j;
                for (j = 0; j < colCount; j++)
                {
                    strInsert += " (" + idSel + ","; //id_selection
                    strInsert += " " + dt.Rows[j][0].ToString() + ","; // id_column
                    strInsert += " '" + dataGridView1.Rows[i].Cells[j].Value + "'),"; // column_value
                }
            }

            strInsert = strInsert.Substring(0, strInsert.Length - 1);

            sqlCmd.CommandText = "insert into [selection_content] values " + strInsert;
            sqlCmd.Connection = cn;
            sqlCmd.ExecuteNonQuery();

            this.Close();

        }
    }
}
