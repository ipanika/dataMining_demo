using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using DataTable = System.Data.DataTable;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Office.Interop.Excel;
using Microsoft.AnalysisServices;

namespace dataMining_demo
{
    public partial class Form1 : Form
    {
        // member variable -- the Analysis Services server connection
        Server svr;

        public Form1()
        {
            InitializeComponent();
        }

        private Microsoft.Office.Interop.Excel.Application objExcel;
        private Microsoft.Office.Interop.Excel.Workbook objWorkBook;
        private Microsoft.Office.Interop.Excel.Worksheet objWorkSheet;

        private void button1_Click(object sender, EventArgs e)
        {
            Stream myStream = null;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "excel files (.xls,.xlsx)|*.xlsx;*.xls|All files|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            objExcel = new Microsoft.Office.Interop.Excel.Application();
                            objWorkBook = objExcel.Workbooks.Open(openFileDialog1.FileName);
                            objWorkSheet = objExcel.ActiveSheet as Microsoft.Office.Interop.Excel.Worksheet;
                            Microsoft.Office.Interop.Excel.Range rg = null;

                            Int16 row = 1;
                            dataGridView1.Rows.Clear();

                            string[] arr = new string[13];

                            while (objWorkSheet.get_Range("a" + row, "a" + row).Value != null)
                            {
                                rg = objWorkSheet.get_Range("a" + row, "m" + row);

                                Int16 i = 0;

                                foreach (Microsoft.Office.Interop.Excel.Range item in rg)
                                {

                                    try
                                    {
                                        arr[i] = item.Value.ToString().Trim();
                                        i += 1;

                                    }
                                    catch { arr[0] = "zero"; }
                                }
                                dataGridView1.Rows.Add(arr);
                                row++;
                            }
                            MessageBox.Show("OK");
                            objWorkBook.Close();


                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file" + ex.Message);
                }
            }

            // Create server object and connect
            svr = new Server();
            svr.Connect("localhost");

            Database db = CreateDatabase();

            CreateDataAccessObjects(db, dataGridView1);
            AddNewDataAccessObjects(db);
            MiningStructure ms = CreateMiningStructure(db);

            CreateModels(ms);

            ProcessDatabase(db);


            db = svr.Databases["demo_DM"];
            db.Update(UpdateOptions.ExpandFull);
            SetModelPermissions(db, db.MiningStructures[0].MiningModels[0]);

            // Disconnect from the server
            svr.Disconnect();

        }//DataGridView processed

       //create database for SSAS
        Database CreateDatabase()
        {
            // Create a database and set the properties
            Database db = null; //new Database();
            if ((svr != null) && (svr.Connected))
            {
                db = svr.Databases.FindByName("demo_DM");
                if (db != null)
                {
                    db.Drop();
                }

                db = svr.Databases.Add("demo_DM");
                db.Update();
            }
           
            return db;
        }

        void CreateDataAccessObjects(Database db, DataGridView dtGrd)
        {
            // Create a relational data source
            // by specifying the name and the id
            RelationalDataSource ds = new RelationalDataSource("excel_file", "excel_file");
            ds.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\"C:\\Users\\user\\Documents\\Visual Studio 2010\\Projects\\dataMining_demo\\123.xlsx\";Extended Properties=\"Excel 12.0 Xml;HDR=YES\";";
            db.DataSources.Add(ds);

            // Create connection to datasource cto extract schema to a dataset
            DataSet dset = new DataSet();
            SqlConnection cn = new SqlConnection("Data Source=localhost; Initial Catalog=Chapter 16; Integrated Security=true");

            // Create data adapters from database tables and load schemas
            SqlDataAdapter daCustomers = new SqlDataAdapter("SELECT * FROM Customers", cn);
            daCustomers.FillSchema(dset, SchemaType.Mapped, "Customers");

            SqlDataAdapter daChannels = new SqlDataAdapter("SELECT * FROM Channels", cn);
            daChannels.FillSchema(dset, SchemaType.Mapped, "Channels");

            // Add relationship between Customers and Channels
            DataRelation drCustomerChannels = new DataRelation(
                                                    "Customer_Channels",
                                                    dset.Tables["Customers"].Columns["SurveyTakenID"],
                                                    dset.Tables["Channels"].Columns["SurveyTakenID"]);
            dset.Relations.Add(drCustomerChannels);

            // Create the DSV, ad the dataset and add to the database
            DataSourceView dsv = new DataSourceView("SimpleMovieClick", "SimpleMovieClick");
            dsv.DataSourceID = "MovieClick";
            dsv.Schema = dset.Clone();
            db.DataSourceViews.Add(dsv);

            // Update the database to create the objects on the server
            db.Update(UpdateOptions.ExpandFull);
        }

        void AddNewDataAccessObjects(Database db)
        {
            // Create connection to datasource cto extract schema to a dataset
            DataSet dset = new DataSet();
            SqlConnection cn = new SqlConnection("Data Source=localhost; Initial Catalog=Chapter 16; Integrated Security=true");

            // Create the Customers data adapter with the calculated appended
            SqlDataAdapter daCustomers = new SqlDataAdapter(
                    "SELECT *, " +
                    "(CASE WHEN (Age < 30) THEN 'GenY' " +
                    " WHEN (Age >= 30 AND Age < 40) THEN 'GenX' " +
                    "ELSE 'Baby Boomer' END) AS Generation " +
                    "FROM Customers", cn);
            daCustomers.FillSchema(dset, SchemaType.Mapped, "Customers");
            // Add Extended properties to the Generation column indicating to 
            // Analysis Services that it is a calculated column
            DataColumn genColumn = dset.Tables["Customers"].Columns["Generation"];
            genColumn.ExtendedProperties.Add("DbColumnName", "Generation");
            genColumn.ExtendedProperties.Add("Description", "Customer generation");
            genColumn.ExtendedProperties.Add("IsLogical", "true");
            genColumn.ExtendedProperties.Add("ComputedColumnExpression",
                                "CASE WHEN (Age < 30) THEN 'GenY' " +
                                "WHEN (Age >= 30 AND Age < 40) THEN 'GenX' " +
                                "ELSE 'Baby Boomer' END");

            // Create a 'Pay Channels' data adapter with a customer query
            // for our named query
            SqlDataAdapter daPayChannels = new SqlDataAdapter(
                    "SELECT * FROM Channels " +
                    "WHERE Channel IN ('Cinemax', 'Encore', 'HBO', 'Showtime', " +
                    "'STARZ!', 'The Movie Channel')", cn);
            daPayChannels.FillSchema(dset, SchemaType.Mapped, "PayChannels");
            // Add Extended properties to the PayChannels table indicating to 
            // Analysis Services that it is a named query
            DataTable pcTable = dset.Tables["PayChannels"];
            pcTable.ExtendedProperties.Add("IsLogical", "true");
            pcTable.ExtendedProperties.Add("Description", "Channels requiring an additional fee");
            pcTable.ExtendedProperties.Add("TableType", "View");
            pcTable.ExtendedProperties.Add("QueryDefinition",
                    "SELECT * FROM Channels " +
                    "WHERE Channel IN ('Cinemax', 'Encore', 'HBO', 'Showtime', " +
                    "'STARZ!', 'The Movie Channel')");

            // Add relationship between Customers and PayChannels
            DataRelation drCustomerPayChannels = new DataRelation(
                                                    "CustomerPayChannels",
                                                    dset.Tables["Customers"].Columns["SurveyTakenID"],
                                                    dset.Tables["PayChannels"].Columns["SurveyTakenID"]);
            dset.Relations.Add(drCustomerPayChannels);

            // Access the data source and the DSV created previously
            // by specifying the ID
            DataSourceView dsv = new DataSourceView("MovieClick", "MovieClick");
            dsv.DataSourceID = "MovieClick";
            dsv.Schema = dset.Clone();
            db.DataSourceViews.Add(dsv);

            // Update the database to create the objects on the server
            db.Update(UpdateOptions.ExpandFull);

        }

        MiningStructure CreateMiningStructure(Database db)
        {
            // Initialize a new mining structure
            MiningStructure ms = new MiningStructure("PayChannelAnalysis", "PayChannelAnalysis");
            ms.Source = new DataSourceViewBinding("MovieClick");

            // Create the columns of the mining structure 
            // setting the type, content and data binding

            // User Id column
            ScalarMiningStructureColumn UserID = new ScalarMiningStructureColumn("UserId", "UserId");
            UserID.Type = MiningStructureColumnTypes.Long;
            UserID.Content = MiningStructureColumnContents.Key;
            UserID.IsKey = true;
            // Add data binding to the column
            UserID.KeyColumns.Add("Customers", "SurveyTakenID", System.Data.OleDb.OleDbType.Integer);
            // Add the column to the mining structure
            ms.Columns.Add(UserID);

            // Generation column
            ScalarMiningStructureColumn Generation = new ScalarMiningStructureColumn("Generation", "Generation");
            Generation.Type = MiningStructureColumnTypes.Text;
            Generation.Content = MiningStructureColumnContents.Discrete;
            // Add data binding to the column
            Generation.KeyColumns.Add("Customers", "Generation", System.Data.OleDb.OleDbType.WChar);
            // Add the column to the mining structure
            ms.Columns.Add(Generation);

            // Add Nested table by creating a table column and adding
            // a key column to the nested table
            TableMiningStructureColumn PayChannels = new TableMiningStructureColumn("PayChannels", "PayChannels");
            PayChannels.ForeignKeyColumns.Add("PayChannels", "SurveyTakenID", System.Data.OleDb.OleDbType.Integer);

            ScalarMiningStructureColumn Channel = new ScalarMiningStructureColumn("Channel", "Channel");
            Channel.Type = MiningStructureColumnTypes.Text;
            Channel.Content = MiningStructureColumnContents.Key;
            Channel.IsKey = true;
            // Add data binding to the column
            Channel.KeyColumns.Add("PayChannels", "Channel", System.Data.OleDb.OleDbType.WChar);
            PayChannels.Columns.Add(Channel);
            ms.Columns.Add(PayChannels);

            // Add the mining structure to the database
            db.MiningStructures.Add(ms);
            ms.Update();

            return ms;

        }

        void CreateModels(MiningStructure ms)
        {
            MiningModel ClusterModel;
            MiningModel TreeModel;
            MiningModelColumn mmc;

            // Create the Cluster model and set the algorithm 
            // and parameters
            ClusterModel = ms.CreateMiningModel(true, "Premium Generation Clusters");
            ClusterModel.Algorithm = "Microsoft_Clustering";
            ClusterModel.AlgorithmParameters.Add("CLUSTER_COUNT", 0);

            // The CreateMiningModel method adds 
            // all the structure columns to the collection


            // Copy the Cluster model and change the necessary properties
            TreeModel = ClusterModel.Clone();
            TreeModel.Name = "Generation Trees";
            TreeModel.ID = "Generation Trees";
            TreeModel.Algorithm = "Microsoft_Decision_Trees";
            TreeModel.AlgorithmParameters.Clear();
            TreeModel.Columns["Generation"].Usage = "Predict";
            TreeModel.Columns["PayChannels"].Usage = "Predict";

            // Add an aliased copy of the PayChannels table to the trees model
            mmc = TreeModel.Columns.Add("PayChannels_Hbo_Encore");
            mmc.SourceColumnID = "PayChannels";
            mmc = mmc.Columns.Add("Channel");
            mmc.SourceColumnID = "Channel";
            mmc.Usage = "Key";

            // Now set a filter on the PayChannels_Hbo_Encore table and use it 
            // as input to predict other channels
            TreeModel.Columns["PayChannels_Hbo_Encore"].Filter = "Channel='HBO' OR Channel='Encore'";

            // Set a complementary filter on the payChannels predictable nested table
            TreeModel.Columns["PayChannels"].Filter = "Channel<>'HBO' AND Channel<>'Encore'";



            ms.MiningModels.Add(TreeModel);

            // Submit the models to the server
            ClusterModel.Update();
            TreeModel.Update();
        }

        void ProcessDatabase(Database db)
        {
            Trace t;
            TraceEvent e;

            // create the trace object to trace progress reports
            // and add the column containing the progress description
            t = svr.Traces.Add();
            e = t.Events.Add(TraceEventClass.ProgressReportCurrent);
            e.Columns.Add(TraceColumn.TextData);
            t.Update();

            // Add the handler for the trace event
            t.OnEvent += new TraceEventHandler(ProgressReportHandler);
            try
            {
                // start the trace, process of the database, then stop it
                t.Start();
                db.Process(ProcessType.ProcessFull);
                t.Stop();


            }
            catch (System.Exception /*ex*/)
            {
            }

        }
        void ProgressReportHandler(object sender, TraceEventArgs e)
        {
            Console.WriteLine(e[TraceColumn.TextData]);
        }

        void SetModelPermissions(Database db, MiningModel mm)
        {
            // Create a new role and add members
            Role r = new Role("ModelReader", "ModelReader");

            r.Members.Add(new RoleMember("user-ПК\\user"));

            // Add the role to the database and update
            db.Roles.Add(r);
            r.Update();


            // Create a permission object referring the role
            MiningModelPermission mmp = new MiningModelPermission();
            mmp.Name = "ModelReader";
            mmp.ID = "ModelReader";
            mmp.RoleID = "ModelReader";

            // Assign access rights to the permission
            mmp.Read = ReadAccess.Allowed;
            mmp.AllowBrowsing = true;
            mmp.AllowDrillThrough = true;
            mmp.ReadDefinition = ReadDefinitionAccess.Allowed;



            // Add permission to the model and update
            mm.MiningModelPermissions.Add(mmp);
            mmp.Update();
        }




    }
}
