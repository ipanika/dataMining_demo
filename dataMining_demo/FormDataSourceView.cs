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
                db = svr.Databases.FindByName(FormMain.as_initCatalog);

            }

            List<string> columnNames = new List<string>();
            fillListBox(columnNames);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var colForDSV = new List<string>();
            int i = 0;
            
            // создание массива с именами столбцов, выбранных для представления
            foreach (object obj in checkedListBox1.CheckedItems)
            {
                colForDSV.Add(obj.ToString());
                i += 1;
            }

            CreateDataAccessObjects(colForDSV);

            this.Close();
            
        }

        void CreateDataAccessObjects(List<string> columnNames )
        {
            string argsForQuery = " ";
            
            int i;
            for (i = 0; i < columnNames.Count-1; i++){
                argsForQuery += "'"+columnNames[i] + "', ";
            }

            argsForQuery += "'" + columnNames[i] + "' ";


            // определение имени представления данных
            string dsvName = textBox1.Text;

            // сохранение в БД приложения информации о созданных источниках и
            // представлениях данных
            SqlConnection cnToDSV = new SqlConnection(FormMain.app_connectionString);
            if (cnToDSV.State == ConnectionState.Closed)
                cnToDSV.Open();

            // заполнение таблицы data_source_views
            SqlCommand sqlCmd2 = new SqlCommand("INSERT INTO [data_source_views]  VALUES ('" + dsvName + "')", cnToDSV);
            sqlCmd2.ExecuteNonQuery();


            // получение id созданного представления для связи с таблицей dsv_columns
            sqlCmd2 = new SqlCommand("SELECT [id_dsv] FROM [data_source_views]  WHERE [name] = ('" + dsvName + "')", cnToDSV);
            string dsvID = sqlCmd2.ExecuteScalar().ToString();

            // заполнение таблицы relations для связи задачи с представлением данных
            sqlCmd2 = new SqlCommand("INSERT INTO [relations]  VALUES ('"+ FormMain.taskType + "', '" + dsvID + "')", cnToDSV);
            sqlCmd2.ExecuteNonQuery();

            // заполнение таблицы [dsv_columns] данными представлений
            for (i = 0; i < columnNames.Count; i++)
            {
                sqlCmd2 = new SqlCommand("INSERT INTO [dsv_columns]  VALUES ('" + dsvID + "', '" + columnNames[i] + "')", cnToDSV);
                sqlCmd2.ExecuteNonQuery();
            }

        }

        // заполнение списка элементов в зависимости от значения элемента checkedBox
        private void fillListBox(List<string> columnNames)
        {
            // если тип решаемой задачи - прогнозирование, то создается представление для прогнозирования
            if (FormMain.taskType == 2)
            {
                columnNames.Add("CompanyID");
                columnNames.Add("CompanyName");
                fillColumnNames(columnNames);
                
                checkedListBox1.DataSource = columnNames;
                checkedListBox1.CheckOnClick = true;

                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                    checkedListBox1.SetItemChecked(i, true);

            }
            // если решается задача кластеризации:
            else
            {
                columnNames.Add("CompanyID");
                columnNames.Add("CompanyName");
                
                fillColumnNames(columnNames);

                checkedListBox1.DataSource = columnNames;
                checkedListBox1.CheckOnClick = true;

                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                    checkedListBox1.SetItemChecked(i, true);
            }

        }

        private void fillColumnNames(List<string> columnNames)
        {
            columnNames.Add("YearID");
            columnNames.Add("Нематериальные активы");
            columnNames.Add("Основные средства");
            columnNames.Add("Незавершенное строительство");
            columnNames.Add("Доходные вложения в материал.ценности");
            columnNames.Add("Отложенные налоговые активы");
            columnNames.Add("Прочие внеоборотные активы");
            columnNames.Add("Запасы");
            columnNames.Add("в т.ч. сырье и материалы");
            columnNames.Add("животные на выращивание и откорме");
            columnNames.Add("затраты в НЗП");
            columnNames.Add("готовая продукция и товары");
            columnNames.Add("товары отгруженные");
            columnNames.Add("РБП");
            columnNames.Add("прочие запасы и затраты");
            columnNames.Add("НДС");
            columnNames.Add("ДЗ долгосрочная");
            columnNames.Add("ДЗ краткосрочная");
            columnNames.Add("покупатели и заказчики");
        }

        
    }
}
