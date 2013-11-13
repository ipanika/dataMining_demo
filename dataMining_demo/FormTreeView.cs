﻿using System;
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

                                DataTable dtMod = new DataTable();
                                sqlDA = new SqlDataAdapter("select [id_model], [name] from [models] where [id_structure] = '" + strID + "'", cn);
                                sqlDA.Fill(dtMod);

                                if (dtMod != null)
                                    for (int l = 0; l < dtMod.Rows.Count; l++)
                                    {
                                        string modID = dtMod.Rows[l][0].ToString();
                                        string modName = dtMod.Rows[l][1].ToString();
                                        TreeNode modNode = new TreeNode(modName);

                                        strNode.Nodes.Add(modNode);

                                        DataTable dtAlgVar = new DataTable();
                                        sqlDA = new SqlDataAdapter("select [algorithm_variants].[name] from [algorithm_variants] JOIN [models] ON "+
                                                  " models.id_algorithm_variant = algorithm_variants.id_algorithm_variant AND " +
                                                  " models.name =  '" + modName + "'", cn);
                                        sqlDA.Fill(dtAlgVar);

                                        if (dtAlgVar != null)
                                            for (int m = 0; m < dtAlgVar.Rows.Count; m++)
                                            {
                                                //string algVarID = dtMod.Rows[m][0].ToString();
                                                string algVarName = dtAlgVar.Rows[m][0].ToString();
                                                TreeNode algVarNode = new TreeNode(algVarName);

                                                modNode.Nodes.Add(algVarNode);
                                            }
                                    }
                            }
                    }
                
            }
            
        }
    }
}
