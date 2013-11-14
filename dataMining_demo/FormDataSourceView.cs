using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using Microsoft.AnalysisServices;

namespace dataMining_demo
{
    public partial class FormDataSourceView : Form
    {
        //Database db = dataMining_demo.Form1.db;
        //Server svr = dataMining_demo.Form1.svr;
         Server svr = new Server();
         Database db = new Database();

        public FormDataSourceView()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            svr.Connect("localhost");

            if ((svr != null) && (svr.Connected))
            {
                db = svr.Databases.FindByName("demo_DM");
                
            }

            // создать соединение с БД
            DataTable dt = new DataTable();
            SqlConnection cn = new SqlConnection("Data Source=localhost; Initial Catalog=DW; Integrated Security=true");
            if (cn.State == ConnectionState.Closed)
                cn.Open();

            SqlDataAdapter sqlDA = new SqlDataAdapter("select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_CATALOG = 'demo_source' AND TABLE_NAME = 'SourceData$'", cn);
            sqlDA.Fill(dt);

            
            if (dt.Rows.Count > 0)
            {
                checkedListBox1.CheckOnClick = true;
                checkedListBox1.DataSource = dt;
                checkedListBox1.DisplayMember = "COLUMN_NAME";
            }

            
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
                checkedListBox1.SetItemChecked(i, true);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var colForDSV = new List<string>();
            int i = 0;
            
            // создание массива с именами столбцов, выбранных для представления
            foreach (object obj in checkedListBox1.CheckedItems)
            {
                DataRowView drv;
                drv = (System.Data.DataRowView) obj;
                colForDSV.Add(drv["COLUMN_NAME"].ToString());
                i += 1;
            }

            CreateDataAccessObjects(db, colForDSV);

            this.Close();
        }

        void CreateDataAccessObjects(Database db, List<string> columnNames )
        {
            // Create a relational data source
            // by specifying the name and the id
            string dsName = "demo_ds";
            RelationalDataSource ds = new RelationalDataSource(dsName, Utils.GetSyntacticallyValidID(dsName, typeof(Database)));
            ds.ConnectionString = "Provider=SQLNCLI11.1;Data Source=localhost;Integrated Security=SSPI;Initial Catalog=DW";

            if (db.DataSources.FindByName(dsName) == null){
                
                db.DataSources.Add(ds);

            }

            // Create connection to datasource cto extract schema to a dataset
            DataSet dset = new DataSet();
            SqlConnection cn = new SqlConnection("Data Source=localhost; Initial Catalog=DW; Integrated Security=true");

            string argsForQuery = " ";
            string strQuery = "";

            int i;
            for (i = 0; i < columnNames.Count-1; i++){
                argsForQuery += "'"+columnNames[i] + "', ";
            }

            argsForQuery += "'" + columnNames[i] + "' ";

            strQuery = "SELECT "+ argsForQuery + " FROM SourceData$";

            // определение имени представления данных
            string dsvName = textBox1.Text;

            // сохранение в БД приложения информации о созданных источниках и
            // представлениях данных
            SqlConnection cnToDSV = new SqlConnection("Data Source=localhost; Initial Catalog=demo_dm; Integrated Security=true");
            if (cnToDSV.State == ConnectionState.Closed)
                cnToDSV.Open();

            // заполнение таблицы data_source_views
            SqlCommand sqlCmd2 = new SqlCommand("INSERT INTO [data_source_views]  VALUES ('" + dsvName + "')", cnToDSV);
            sqlCmd2.ExecuteNonQuery();

            // получение id созданного представления для связи с таблицей dsv_columns
            sqlCmd2 = new SqlCommand("SELECT [id_dsv] FROM [data_source_views]  WHERE [name] = ('" + dsvName + "')", cnToDSV);
            string dsvID = sqlCmd2.ExecuteScalar().ToString();

            // заполнение таблицы [dsv_columns] данными представлений
            for (i = 0; i < columnNames.Count; i++)
            {
                sqlCmd2 = new SqlCommand("INSERT INTO [dsv_columns]  VALUES ('" + dsvID + "', '" + columnNames[i] + "')", cnToDSV);
                sqlCmd2.ExecuteNonQuery();
            }

        }

        
    }
}
