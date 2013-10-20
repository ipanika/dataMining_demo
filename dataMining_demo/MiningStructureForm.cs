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

            checkedListBox1.Items.Clear();

            dsv = db.DataSourceViews.FindByName(comboBox1.Text);
            if (dsv != null)
            {
                ds = dsv.Schema;
                int i = 0;
                while (i < dsv.Schema.Tables[0].Columns.Count)
                {
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
           
            SqlConnection cn = new SqlConnection("Data Source=localhost; Initial Catalog=demo_source; Integrated Security=true");
            if (cn.State == ConnectionState.Closed)
                cn.Open();

            DataTable dt = new DataTable();
            // загрузка имеющихся представлений ИАД
            SqlDataAdapter sqlDA = new SqlDataAdapter("select [dsv_name] FROM [demo_dsv]", cn);
            sqlDA.Fill(dt);

            comboBox1.DataSource = dt;
            comboBox1.DisplayMember = "dsv_name";

            // создать соединение с БД
            cn = new SqlConnection("Data Source=localhost; Initial Catalog=demo_source; Integrated Security=true");
            if (cn.State == ConnectionState.Closed)
                cn.Open();

            DataSourceView dsv = new DataSourceView();
            DataSet ds;

            dsv = db.DataSourceViews.FindByName(comboBox1.Text);
            if (dsv != null)
            {
                ds = dsv.Schema;
                int i = 0;
                while (i < dsv.Schema.Tables[0].Columns.Count)
                {
                    checkedListBox1.CheckOnClick = true;
                    checkedListBox1.Items.Add(dsv.Schema.Tables[0].Columns[i].ColumnName);
                    checkedListBox1.SetItemChecked(i, true);

                    i += 1;
                }
            }

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

            // создание массива с именами столбцов, выбранных для представления
            foreach (object obj in checkedListBox1.CheckedItems)
            {
                DataRowView drv;
                drv = (System.Data.DataRowView) obj;
                ScalarMiningStructureColumn colItem = new ScalarMiningStructureColumn(obj.ToString(), obj.ToString());
                colItem.Type = MiningStructureColumnTypes.Long;
                colItem.Content = MiningStructureColumnContents.Key;
                colItem.IsKey = true;
                colForMStr.Add(colItem);
                i += 1;
            }

            

            // Create the columns of the mining structure 
            // setting the type, content and data binding

            //// User Id column
            ScalarMiningStructureColumn col1 = new ScalarMiningStructureColumn("ID", "ID");
            col1.Type = MiningStructureColumnTypes.Long;
            col1.Content = MiningStructureColumnContents.Key;
            col1.IsKey = true;
            //// Add data binding to the column
            col1.KeyColumns.Add("SourceData$", "ID", System.Data.OleDb.OleDbType.Integer);
            //// Add the column to the mining structure
            ms.Columns.Add(col1);

            //// Generation column
            ScalarMiningStructureColumn col2 = new ScalarMiningStructureColumn("Marital Status", "Marital Status");
            col2.Type = MiningStructureColumnTypes.Text;
            col2.Content = MiningStructureColumnContents.Discrete;
            // Add data binding to the column
            col2.KeyColumns.Add("SourceData$", "Marital Status", System.Data.OleDb.OleDbType.WChar);
            // Add the column to the mining structure
            ms.Columns.Add(col2);

            ScalarMiningStructureColumn col3 = new ScalarMiningStructureColumn("Gender", "Gender");
            col3.Type = MiningStructureColumnTypes.Text;
            col3.Content = MiningStructureColumnContents.Discrete;
            // col3 data binding to the column
            col3.KeyColumns.Add("SourceData$", "Gender", System.Data.OleDb.OleDbType.WChar);
            // Add the column to the mining structure
            ms.Columns.Add(col3);

            ScalarMiningStructureColumn col4 = new ScalarMiningStructureColumn("Yearly Income", "Yearly Income");
            col4.Type = MiningStructureColumnTypes.Long;
            col4.Content = MiningStructureColumnContents.Discretized;
            // col3 data binding to the column
            col4.KeyColumns.Add("SourceData$", "Yearly Income", System.Data.OleDb.OleDbType.Integer);
            // Add the column to the mining structure
            ms.Columns.Add(col4);

            ScalarMiningStructureColumn col5 = new ScalarMiningStructureColumn("Children", "Children");
            col5.Type = MiningStructureColumnTypes.Long;
            col5.Content = MiningStructureColumnContents.Discrete;
            // col3 data binding to the column
            col5.KeyColumns.Add("SourceData$", "Children", System.Data.OleDb.OleDbType.Integer);
            // Add the column to the mining structure
            ms.Columns.Add(col5);

            ScalarMiningStructureColumn col6 = new ScalarMiningStructureColumn("Education", "Education");
            col6.Type = MiningStructureColumnTypes.Text;
            col6.Content = MiningStructureColumnContents.Discrete;
            // col3 data binding to the column
            col6.KeyColumns.Add("SourceData$", "Education", System.Data.OleDb.OleDbType.WChar);
            // Add the column to the mining structure
            ms.Columns.Add(col6);

            ScalarMiningStructureColumn col7 = new ScalarMiningStructureColumn("Occupation", "Occupation");
            col7.Type = MiningStructureColumnTypes.Text;
            col7.Content = MiningStructureColumnContents.Discrete;
            // col3 data binding to the column
            col7.KeyColumns.Add("SourceData$", "Occupation", System.Data.OleDb.OleDbType.WChar);
            // Add the column to the mining structure
            ms.Columns.Add(col7);

            ScalarMiningStructureColumn col8 = new ScalarMiningStructureColumn("Home Owner", "Home Owner");
            col8.Type = MiningStructureColumnTypes.Text;
            col8.Content = MiningStructureColumnContents.Discrete;
            // col3 data binding to the column
            col8.KeyColumns.Add("SourceData$", "Home Owner", System.Data.OleDb.OleDbType.WChar);
            // Add the column to the mining structure
            ms.Columns.Add(col8);

            ScalarMiningStructureColumn col9 = new ScalarMiningStructureColumn("Cars", "Cars");
            col9.Type = MiningStructureColumnTypes.Long;
            col9.Content = MiningStructureColumnContents.Discrete;
            // col3 data binding to the column
            col9.KeyColumns.Add("SourceData$", "Cars", System.Data.OleDb.OleDbType.Integer);
            // Add the column to the mining structure
            ms.Columns.Add(col9);

            ScalarMiningStructureColumn col10 = new ScalarMiningStructureColumn("Commute Distance", "Commute Distance");
            col10.Type = MiningStructureColumnTypes.Text;
            col10.Content = MiningStructureColumnContents.Discrete;
            // col3 data binding to the column
            col10.KeyColumns.Add("SourceData$", "Commute Distance", System.Data.OleDb.OleDbType.WChar);
            // Add the column to the mining structure
            ms.Columns.Add(col10);

            ScalarMiningStructureColumn col11 = new ScalarMiningStructureColumn("Region", "Region");
            col11.Type = MiningStructureColumnTypes.Text;
            col11.Content = MiningStructureColumnContents.Discrete;
            // col3 data binding to the column
            col11.KeyColumns.Add("SourceData$", "Region", System.Data.OleDb.OleDbType.WChar);
            // Add the column to the mining structure
            ms.Columns.Add(col11);

            ScalarMiningStructureColumn col12 = new ScalarMiningStructureColumn("Age", "Age");
            col12.Type = MiningStructureColumnTypes.Long;
            col12.Content = MiningStructureColumnContents.Continuous;
            // col3 data binding to the column
            col12.KeyColumns.Add("SourceData$", "Age", System.Data.OleDb.OleDbType.Integer);
            // Add the column to the mining structure
            ms.Columns.Add(col12);

            ScalarMiningStructureColumn col13 = new ScalarMiningStructureColumn("BikeBuyer", "BikeBuyer");
            col13.Type = MiningStructureColumnTypes.Text;
            col13.Content = MiningStructureColumnContents.Discrete;
            // col3 data binding to the column
            col13.KeyColumns.Add("SourceData$", "BikeBuyer", System.Data.OleDb.OleDbType.WChar);
            // Add the column to the mining structure
            ms.Columns.Add(col13);

            // Add the mining structure to the database
            db.MiningStructures.Add(ms);
            ms.Update();

        }
    }
}
