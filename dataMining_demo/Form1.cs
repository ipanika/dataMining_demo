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
        
        private void button1_Click(object sender, EventArgs e)
        {
            
            // Create server object and connect
            svr = new Server();
            svr.Connect("localhost");

            Database db = CreateDatabase();


            CreateDataAccessObjects(db);
            /*MiningStructure ms = CreateMiningStructure(db);

            CreateModels(ms);

            ProcessDatabase(db);

            
            db = svr.Databases["demo_DM"];
            db.Update(UpdateOptions.ExpandFull);
            SetModelPermissions(db, db.MiningStructures[0].MiningModels[0]);
            */
            //// Disconnect from the server
            svr.Disconnect();

            MessageBox.Show("OK");           
             
        } 

        //create database for SSAS
        Database CreateDatabase()
        {
            // Create a database and set the properties
            Database db = null; 
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

        void CreateDataAccessObjects(Database db)
        {
            // Create a relational data source
            // by specifying the name and the id
            RelationalDataSource ds = new RelationalDataSource("demo_ds", Utils.GetSyntacticallyValidID("demo_ds", typeof(Database)));
            ds.ConnectionString = "Provider=SQLNCLI11.1;Data Source=localhost;Integrated Security=SSPI;Initial Catalog=demo_source";
			
            db.DataSources.Add(ds);
            /*
            //// Create the DSV, ad the dataset and add to the database
            DataSourceView dsv = new DataSourceView("demo_ds", "demo_ds");
            dsv.DataSourceID = "demo_ds";
            dsv.Schema = dst.Clone();
            db.DataSourceViews.Add(dsv);
            db.DataSourceViews[0].ID = dsv.ID;

            // Update the database to create the objects on the server
            db.Update(UpdateOptions.ExpandFull);
            */

            // Create connection to datasource cto extract schema to a dataset
            DataSet dset = new DataSet();
            SqlConnection cn = new SqlConnection("Data Source=localhost; Initial Catalog=demo_source; Integrated Security=true");

            // Create data adapters from database tables and load schemas
            SqlDataAdapter daCustomers = new SqlDataAdapter("SELECT * FROM SourceData$", cn);
            daCustomers.FillSchema(dset, SchemaType.Mapped, "SourceData$");

            //SqlDataAdapter daChannels = new SqlDataAdapter("SELECT * FROM Channels", cn);
            //daChannels.FillSchema(dset, SchemaType.Mapped, "Channels");

            //// Add relationship between Customers and Channels
            //DataRelation drCustomerChannels = new DataRelation(
            //                                        "Customer_Channels",
            //                                        dset.Tables["Customers"].Columns["SurveyTakenID"],
            //                                        dset.Tables["Channels"].Columns["SurveyTakenID"]);
            //dset.Relations.Add(drCustomerChannels);

            // Create the DSV, ad the dataset and add to the database
            DataSourceView dsv = new DataSourceView("demo_dsv", "demo_dsv");
            dsv.DataSourceID = "demo_ds";
            dsv.Schema = dset.Clone();
            db.DataSourceViews.Add(dsv);
            db.DataSourceViews[0].ID = dsv.ID;

            // Update the database to create the objects on the server
            db.Update(UpdateOptions.ExpandFull);
        }
/*
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
            MiningStructure ms = new MiningStructure("demo_structure", "demo_structure");
            ms.Source = new DataSourceViewBinding(db.DataSourceViews[0].ID);

            // Create the columns of the mining structure 
            // setting the type, content and data binding

            //// User Id column
            ScalarMiningStructureColumn col1 = new ScalarMiningStructureColumn("ID", "ID");
            col1.Type = MiningStructureColumnTypes.Long;
            col1.Content = MiningStructureColumnContents.Key;
            col1.IsKey = true;
            //// Add data binding to the column
            col1.KeyColumns.Add("Table1", "ID", System.Data.OleDb.OleDbType.Integer);
            //// Add the column to the mining structure
            ms.Columns.Add(col1);

            //// Generation column
            ScalarMiningStructureColumn col2 = new ScalarMiningStructureColumn("Marital Status", "Marital Status");
            col2.Type = MiningStructureColumnTypes.Text;
            col2.Content = MiningStructureColumnContents.Discrete;
            // Add data binding to the column
            col2.KeyColumns.Add("Table1", "Marital Status", System.Data.OleDb.OleDbType.WChar);
            // Add the column to the mining structure
            ms.Columns.Add(col2);

            ScalarMiningStructureColumn col3 = new ScalarMiningStructureColumn("Gender", "Gender");
            col3.Type = MiningStructureColumnTypes.Text;
            col3.Content = MiningStructureColumnContents.Discrete;
            // col3 data binding to the column
            col3.KeyColumns.Add("Table1", "Gender", System.Data.OleDb.OleDbType.WChar);
            // Add the column to the mining structure
            ms.Columns.Add(col3);

            ScalarMiningStructureColumn col4 = new ScalarMiningStructureColumn("Yearly Income", "Yearly Income");
            col4.Type = MiningStructureColumnTypes.Long;
            col4.Content = MiningStructureColumnContents.Discretized;
            // col3 data binding to the column
            col4.KeyColumns.Add("Table1", "Yearly Income", System.Data.OleDb.OleDbType.Integer);
            // Add the column to the mining structure
            ms.Columns.Add(col4);

            ScalarMiningStructureColumn col5 = new ScalarMiningStructureColumn("Children", "Children");
            col5.Type = MiningStructureColumnTypes.Long;
            col5.Content = MiningStructureColumnContents.Discrete;
            // col3 data binding to the column
            col5.KeyColumns.Add("Table1", "Children", System.Data.OleDb.OleDbType.Integer);
            // Add the column to the mining structure
            ms.Columns.Add(col5);

            ScalarMiningStructureColumn col6 = new ScalarMiningStructureColumn("Education", "Education");
            col6.Type = MiningStructureColumnTypes.Text;
            col6.Content = MiningStructureColumnContents.Discrete;
            // col3 data binding to the column
            col6.KeyColumns.Add("Table1", "Education", System.Data.OleDb.OleDbType.WChar);
            // Add the column to the mining structure
            ms.Columns.Add(col6);

            ScalarMiningStructureColumn col7 = new ScalarMiningStructureColumn("Occupation", "Occupation");
            col7.Type = MiningStructureColumnTypes.Text;
            col7.Content = MiningStructureColumnContents.Discrete;
            // col3 data binding to the column
            col7.KeyColumns.Add("Table1", "Occupation", System.Data.OleDb.OleDbType.WChar);
            // Add the column to the mining structure
            ms.Columns.Add(col7);

            ScalarMiningStructureColumn col8 = new ScalarMiningStructureColumn("Home Owner", "Home Owner");
            col8.Type = MiningStructureColumnTypes.Text;
            col8.Content = MiningStructureColumnContents.Discrete;
            // col3 data binding to the column
            col8.KeyColumns.Add("Table1", "Home Owner", System.Data.OleDb.OleDbType.WChar);
            // Add the column to the mining structure
            ms.Columns.Add(col8);

            ScalarMiningStructureColumn col9 = new ScalarMiningStructureColumn("Cars", "Cars");
            col9.Type = MiningStructureColumnTypes.Long;
            col9.Content = MiningStructureColumnContents.Discrete;
            // col3 data binding to the column
            col9.KeyColumns.Add("Table1", "Cars", System.Data.OleDb.OleDbType.Integer);
            // Add the column to the mining structure
            ms.Columns.Add(col9);

            ScalarMiningStructureColumn col10 = new ScalarMiningStructureColumn("Commute Distance", "Commute Distance");
            col10.Type = MiningStructureColumnTypes.Text;
            col10.Content = MiningStructureColumnContents.Discrete;
            // col3 data binding to the column
            col10.KeyColumns.Add("Table1", "Commute Distance", System.Data.OleDb.OleDbType.WChar);
            // Add the column to the mining structure
            ms.Columns.Add(col10);

            ScalarMiningStructureColumn col11 = new ScalarMiningStructureColumn("Region", "Region");
            col11.Type = MiningStructureColumnTypes.Text;
            col11.Content = MiningStructureColumnContents.Discrete;
            // col3 data binding to the column
            col11.KeyColumns.Add("Table1", "Region", System.Data.OleDb.OleDbType.WChar);
            // Add the column to the mining structure
            ms.Columns.Add(col11);

            ScalarMiningStructureColumn col12 = new ScalarMiningStructureColumn("Age", "Age");
            col12.Type = MiningStructureColumnTypes.Long;
            col12.Content = MiningStructureColumnContents.Continuous;
            // col3 data binding to the column
            col12.KeyColumns.Add("Table1", "Age", System.Data.OleDb.OleDbType.Integer);
            // Add the column to the mining structure
            ms.Columns.Add(col12);

            ScalarMiningStructureColumn col13 = new ScalarMiningStructureColumn("BikeBuyer", "BikeBuyer");
            col13.Type = MiningStructureColumnTypes.Text;
            col13.Content = MiningStructureColumnContents.Discrete;
            // col3 data binding to the column
            col13.KeyColumns.Add("Table1", "BikeBuyer", System.Data.OleDb.OleDbType.WChar);
            // Add the column to the mining structure
            ms.Columns.Add(col13);
            //// Add Nested table by creating a table column and adding
            //// a key column to the nested table
            //TableMiningStructureColumn PayChannels = new TableMiningStructureColumn("PayChannels", "PayChannels");
            //PayChannels.ForeignKeyColumns.Add("PayChannels", "SurveyTakenID", System.Data.OleDb.OleDbType.Integer);

            //ScalarMiningStructureColumn Channel = new ScalarMiningStructureColumn("Channel", "Channel");
            //Channel.Type = MiningStructureColumnTypes.Text;
            //Channel.Content = MiningStructureColumnContents.Key;
            //Channel.IsKey = true;
            //// Add data binding to the column
            //Channel.KeyColumns.Add("PayChannels", "Channel", System.Data.OleDb.OleDbType.WChar);
            //PayChannels.Columns.Add(Channel);
            //ms.Columns.Add(PayChannels);

            // Add the mining structure to the database
            db.MiningStructures.Add(ms);
            ms.Update();

            return ms;

        }

        void CreateModels(MiningStructure ms)
        {
            MiningModel ClusterModel;
            //MiningModel TreeModel;
          //  MiningModelColumn mmc;

            // Create the Cluster model and set the algorithm 
            // and parameters
            ClusterModel = ms.CreateMiningModel(true, "BikeBuyer Clusters");
            ClusterModel.Algorithm = "Microsoft_Clustering";
            ClusterModel.AlgorithmParameters.Add("CLUSTER_COUNT", 0);

            // The CreateMiningModel method adds 
            // all the structure columns to the collection

            // Submit the models to the server
            ClusterModel.Update();
        }*/

        //void ProcessDatabase(Database db)
        //{
        //    Trace t;
        //    TraceEvent e;

        //    // create the trace object to trace progress reports
        //    // and add the column containing the progress description
        //    t = svr.Traces.Add();
        //    e = t.Events.Add(TraceEventClass.ProgressReportCurrent);
        //    e.Columns.Add(TraceColumn.TextData);
        //    t.Update();

        //    // Add the handler for the trace event
        //    t.OnEvent += new TraceEventHandler(ProgressReportHandler);
        //    try
        //    {
        //        // start the trace, process of the database, then stop it
        //        t.Start();
        //        db.Process(ProcessType.ProcessFull);
        //        t.Stop();


        //    }
        //    catch (System.Exception /*ex*/)
        //    {
        //    }

        //}
        void ProgressReportHandler(object sender, TraceEventArgs e)
        {
            Console.WriteLine(e[TraceColumn.TextData]);
        }

        void SetModelPermissions(Database db, MiningModel mm)
        {
            // Create a new role and add members
            Role r = new Role("ModelReader", "ModelReader");

            String userName = SystemInformation.UserName;
            String PCName = Environment.MachineName;

            //r.Members.Add(new RoleMember("user-ПК\\user"));
            r.Members.Add(new RoleMember(PCName + "\\" + userName));

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
