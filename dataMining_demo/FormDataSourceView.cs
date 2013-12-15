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
        public FormDataSourceView()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            // заполнение поля доступных атрибутов для создания представления данных
            List<string> columnNames = new List<string>();
            fillListBox(columnNames);
        }

        // сохранение информации о представлении данных
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

            CreateDataSourceView(colForDSV);

            this.Close();
            
        }

        /*
         * сохранение списка столбцов представления данных
         */
        void CreateDataSourceView(List<string> columnNames )
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

            // получение id задачи 
            SqlCommand sqlCmd2 = new SqlCommand("SELECT [id_task] FROM [tasks]  WHERE [task_type] = '" + FormMain.taskType.ToString() + "'", cnToDSV);
            string taskID = sqlCmd2.ExecuteScalar().ToString();

            // заполнение таблицы data_source_views
            sqlCmd2 = new SqlCommand("INSERT INTO [data_source_views]  VALUES ('" + dsvName + "', '" + taskID + "')", cnToDSV);
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

        // заполнение списка элементов в зависимости от значения элемента checkedBox
        private void fillListBox(List<string> columnNames)
        {
            // если тип решаемой задачи - прогнозирование, то создается представление для прогнозирования
            if (FormMain.taskType == 2)
                getColumnNames(columnNames);
            // если решается задача кластеризации, то добавляется столбец с идентификатором компании:
            else
                columnNames.Add("CompanyID");
             
            getColumnNames(columnNames);

            checkedListBox1.DataSource = columnNames;
            checkedListBox1.CheckOnClick = true;

            
        }


        private void getColumnNames(List<string> columnNames)
        {
            // SQL-запрос к ХД для получения списка доступных атрибутов
            // не используется, тк есть атрибуты, использующие знаки пунктуации, не допустимые для 
            // использования в DMX-запросах

            /*
            SqlConnection cn = new SqlConnection(FormMain.dw_connectionString);
            if (cn.State == ConnectionState.Closed)
                cn.Open();

            string sqlQuery = "SELECT [description] FROM [BalanceLine] ";

            DataTable dt1 = new DataTable();
            // загрузка имеющихся представлений ИАД
            SqlDataAdapter sqlDA = new SqlDataAdapter(sqlQuery, cn);
            sqlDA.Fill(dt1);
            */

            columnNames.Add("CompanyName");
            columnNames.Add("YearID");
            columnNames.Add("Нематериальные активы");
            columnNames.Add("Основные средства");
            columnNames.Add("Незавершенное строительство");
            columnNames.Add("Отложенные налоговые активы");
            columnNames.Add("Прочие внеоборотные активы");
            columnNames.Add("Запасы");
            columnNames.Add("животные на выращивание и откорме");
            columnNames.Add("затраты в НЗП");
            columnNames.Add("готовая продукция и товары");
            columnNames.Add("товары отгруженные");
            columnNames.Add("РБП");
            columnNames.Add("прочие запасы и затраты");
            columnNames.Add("НДС");
            columnNames.Add("ДЗ долгосрочная");
            columnNames.Add("ДЗ краткосрочная");
            columnNames.Add("Краткосрочные финансовые вложения");
            columnNames.Add("Денежные средства");
            columnNames.Add("Прочие оборотные активы");
            columnNames.Add("Итого по разделу 2");
            columnNames.Add("Уставный капитал");
            columnNames.Add("Добавочный капитал");
            columnNames.Add("Резервный капитал");
            columnNames.Add("Итого по разделу 3");
            columnNames.Add("Займы и кредиты долгосрочные");
            columnNames.Add("Итого по разделу 4");
            
        }

        // инвертирование выбранных элементов
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {            
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
                if (checkedListBox1.GetItemChecked(i))
                    checkedListBox1.SetItemChecked(i, false);
                else 
                    checkedListBox1.SetItemChecked(i, true);
        }

        
    }
}
