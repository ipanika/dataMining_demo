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

        //Server svr = new Server();
        //Database db = new Database();

        public FormTreeView()
        {
            InitializeComponent();
        }

        private void TreeViewForm_Load(object sender, EventArgs e)
        {
            //svr.Connect("localhost");

            //if ((svr != null) && (svr.Connected))
            //{
            //    db = svr.Databases.FindByName("demo_DM");
            //}


            DataTable dtDsv = new DataTable();

            SqlConnection cn = new SqlConnection("Data Source=localhost; Initial Catalog=demo_dm; Integrated Security=true");
            if (cn.State == ConnectionState.Closed)
                cn.Open();

            SqlDataAdapter sqlDA = new SqlDataAdapter("select [id_dsv], [name] from [data_source_views]", cn);
            sqlDA.Fill(dtDsv);
            
            for (int i = 0; i < dtDsv.Rows.Count; i++)
            {
                string dsvID = dtDsv.Rows[i][0].ToString();
                string dsvName = dtDsv.Rows[i][1].ToString();
                TreeNode dsvNode = new TreeNode(dsvName);

                treeView1.Nodes.Add(dsvNode);

                DataTable dtSel = new DataTable();
                sqlDA = new SqlDataAdapter("select [id_selection], [name] from [selections] where [id_dsv] = '"+dsvID + "'", cn);
                sqlDA.Fill(dtSel);

                if (dtSel != null)
                    for (int j = 0; j < dtSel.Rows.Count; j++)
                    {
                        string selID = dtSel.Rows[j][0].ToString();
                        string selName = dtSel.Rows[j][1].ToString();
                        TreeNode selNode = new TreeNode(selName);

                        dsvNode.Nodes.Add(selNode);

                        DataTable dtStr = new DataTable();
                        sqlDA = new SqlDataAdapter("select [id_structure], [name] from [structures] where [id_selection] = '" + selID + "'", cn);
                        sqlDA.Fill(dtStr);

                        if (dtStr != null)
                            for (int k = 0; k < dtStr.Rows.Count; k++)
                            {
                                string strID = dtStr.Rows[k][0].ToString();
                                string strName = dtStr.Rows[k][1].ToString();
                                TreeNode strNode = new TreeNode(strName);

                                selNode.Nodes.Add(strNode);
                            }
                    }
                
            }
            
        }
    }
}
