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
    public partial class FormTreeView : Form
    {

        Server svr = new Server();
        Database db = new Database();

        public FormTreeView()
        {
            InitializeComponent();
        }

        private void TreeViewForm_Load(object sender, EventArgs e)
        {
            svr.Connect("localhost");

            if ((svr != null) && (svr.Connected))
            {
                db = svr.Databases.FindByName("demo_DM");
            }


            DataTable dtDsv = new DataTable();

            SqlConnection cn = new SqlConnection("Data Source=localhost; Initial Catalog=demo_source; Integrated Security=true");
            if (cn.State == ConnectionState.Closed)
                cn.Open();

            SqlDataAdapter sqlDA = new SqlDataAdapter("select [dsv_name] from [demo_dsv]", cn);
            sqlDA.Fill(dtDsv);
            
            for (int i = 0; i < dtDsv.Rows.Count; i++)
            {
                DataTable dtStr = new DataTable();
                string dsvName = dtDsv.Rows[i]["dsv_name"].ToString();
                TreeNode dsvNode = new TreeNode(dsvName);

                treeView1.Nodes.Add(dsvNode);

                sqlDA = new SqlDataAdapter("select [mstr_name] from [demo_mstr] where [dsv_name] = '"+dsvName + "'", cn);

                sqlDA.Fill(dtStr);
                if (dtStr != null)
                    for (int j = 0; j < dtStr.Rows.Count; j++)
                    {
                        DataTable dtM = new DataTable();
                        string strName = dtStr.Rows[j]["mstr_name"].ToString();
                        TreeNode strNode = new TreeNode(strName);

                        dsvNode.Nodes.Add(strNode);

                        sqlDA = new SqlDataAdapter("select [mm_name] from [demo_mm] where [mstr_name] = '" + strName + "'", cn);
                        sqlDA.Fill(dtM);

                        if (dtM != null)
                            for (int k = 0; k < dtM.Rows.Count; k++)
                            {
                                string mName = dtM.Rows[k]["mm_name"].ToString();
                                TreeNode mNode = new TreeNode(mName);

                                strNode.Nodes.Add(mNode);
                            }
                    }
                
            }
            
        }
    }
}
