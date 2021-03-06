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
        public FormTreeView()
        {
            InitializeComponent();
        }

        private void TreeViewForm_Load(object sender, EventArgs e)
        {

            DataTable dtTsk = new DataTable();

            SqlConnection cn = new SqlConnection(FormMain.app_connectionString);
            if (cn.State == ConnectionState.Closed)
                cn.Open();

            TreeNode node_task = new TreeNode("Задачи:");
            treeView1.Nodes.Add(node_task);

            node_task.NodeFont = new Font(treeView1.Font, FontStyle.Italic);

            SqlDataAdapter sqlDA = new SqlDataAdapter("select [id_task], [name] from [tasks]", cn);
            sqlDA.Fill(dtTsk);

            for (int h = 0; h < dtTsk.Rows.Count; h++)
            {

                string tskID = dtTsk.Rows[h][0].ToString();
                string tskName = dtTsk.Rows[h][1].ToString();
                TreeNode tskNode = new TreeNode(tskName);

                //treeView1.Nodes.Add(tskNode);
                node_task.Nodes.Add(tskNode);

                TreeNode node_dsv = new TreeNode("Представления данных:");
                tskNode.Nodes.Add(node_dsv);

                node_dsv.NodeFont = new Font(treeView1.Font, FontStyle.Italic);
                //------------


                DataTable dtDsv = new DataTable();
                string sqlQuery = "SELECT [data_source_views].[id_dsv], [data_source_views].[name] FROM [data_source_views] INNER JOIN tasks " +
                                " ON data_source_views.id_task = tasks.id_task WHERE tasks.id_task = " + tskID;
                sqlDA = new SqlDataAdapter(sqlQuery, cn);
                sqlDA.Fill(dtDsv);

                for (int i = 0; i < dtDsv.Rows.Count; i++)
                {
                    string dsvID = dtDsv.Rows[i][0].ToString();
                    string dsvName = dtDsv.Rows[i][1].ToString();
                    TreeNode dsvNode = new TreeNode(dsvName);

                    node_dsv.Nodes.Add(dsvNode);

                    TreeNode node_selec = new TreeNode("Выборки данных:");
                    dsvNode.Nodes.Add(node_selec);
                    node_selec.NodeFont = new Font(treeView1.Font, FontStyle.Italic);

                    DataTable dtSel = new DataTable();
                    sqlDA = new SqlDataAdapter("select [id_selection], [name] from [selections] where [id_dsv] = '" + dsvID + "'", cn);
                    sqlDA.Fill(dtSel);

                    if (dtSel != null)
                        for (int j = 0; j < dtSel.Rows.Count; j++)
                        {
                            string selID = dtSel.Rows[j][0].ToString();
                            string selName = dtSel.Rows[j][1].ToString();
                            TreeNode selNode = new TreeNode(selName);

                            node_selec.Nodes.Add(selNode);

                            DataTable dtStr = new DataTable();
                            sqlDA = new SqlDataAdapter("select [id_structure], [name] from [structures] where [id_selection] = '" + selID + "'", cn);
                            sqlDA.Fill(dtStr);

                            TreeNode node_str = new TreeNode("Структуры данных:");
                            selNode.Nodes.Add(node_str);
                            node_str.NodeFont = new Font(treeView1.Font, FontStyle.Italic);

                            if (dtStr != null)
                                for (int k = 0; k < dtStr.Rows.Count; k++)
                                {
                                    string strID = dtStr.Rows[k][0].ToString();
                                    string strName = dtStr.Rows[k][1].ToString();
                                    TreeNode strNode = new TreeNode(strName);

                                    node_str.Nodes.Add(strNode);

                                    DataTable dtMod = new DataTable();
                                    sqlDA = new SqlDataAdapter("select [id_model], [name] from [models] where [id_structure] = '" + strID + "'", cn);
                                    sqlDA.Fill(dtMod);

                                    TreeNode node_model = new TreeNode("Модели данных:");
                                    strNode.Nodes.Add(node_model);
                                    node_model.NodeFont = new Font(treeView1.Font, FontStyle.Italic);

                                    if (dtMod != null)
                                        for (int l = 0; l < dtMod.Rows.Count; l++)
                                        {
                                            string modID = dtMod.Rows[l][0].ToString();
                                            string modName = dtMod.Rows[l][1].ToString();
                                            TreeNode modNode = new TreeNode(modName);

                                            node_model.Nodes.Add(modNode);
                                            
                                            DataTable dtAlgVar = new DataTable();
                                            sqlDA = new SqlDataAdapter("select [algorithm_variants].[name] from [algorithm_variants] JOIN [models] ON " +
                                                      " models.id_algorithm_variant = algorithm_variants.id_algorithm_variant AND " +
                                                      " models.name =  '" + modName + "'", cn);
                                            sqlDA.Fill(dtAlgVar);

                                            if (dtAlgVar != null)
                                                for (int m = 0; m < dtAlgVar.Rows.Count; m++)
                                                {
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
}
