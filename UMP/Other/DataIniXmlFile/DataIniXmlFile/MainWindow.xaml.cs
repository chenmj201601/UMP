using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Web;
using System.Windows.Controls;
using System.Collections;
using System.Windows.Forms;
using System.Web.UI;
using System.Web.UI.WebControls;
using MessageBox = System.Windows.Forms.MessageBox;
using DialogResult=System.Windows.Forms.DialogResult;
using TreeNode = System.Web.UI.WebControls.TreeNode;
using TreeView = System.Web.UI.WebControls.TreeView;
using System.IO;
using System.Data.Common;
using Oracle.DataAccess.Client;
using System.Data.OleDb;

namespace DataIniXmlFile
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    /// 
    

    public partial class MainWindow : Window
    {
        private ConfigInfo mConfigInfo;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {

            TxtServer.Text = "192.168.5.39";//192.168.4.64
            TxtPort.Text = "1433";
            TxtDBName.Text = "UMPDataDB";
            TxtLoginName.Text = "sa";
            TxtPassword.Password = "PF,123";//voicecyber
            table_ini_yes.IsChecked = true;
            tenant_no.IsChecked = true;
            oracle.IsChecked = true;
            type_newadd.IsChecked = true;
           // wheresql.Text = " c001='2052'";
           // SaveFileName.Text = "T_00_005_2052";


            SetConfig();

            /* TxtServer.Text = "192.168.8.225";//192.168.4.64
             TxtPort.Text = "1433";
             TxtDBName.Text = "IMPDatabase";
             TxtLoginName.Text = "sa";
             TxtPassword.Password = "voicecyber";//voicecyber
             TxtBeginTime.Text = DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd HH:mm:ss");
             TxtEndTime.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss");*/

            //取得表数据
            DataTable dt;
            dt = GetTableList();
            dt.Select();

            DataView dv = dt.DefaultView;
            dv.RowFilter = "table_data='1'";

            table_name.Items.Clear();

            for (int i = 0; i < dv.Count; i++)
            {
                table_name.Items.Add(dv[i].Row["table_name"].ToString());
            }


            string strOracleConn = CreateOracleConnectionString();
            OleDbConnection oracleconn = new OleDbConnection(strOracleConn);
            OleDbDataAdapter sda = new OleDbDataAdapter("select * from t_00_001  ", oracleconn); 
            DataSet ds = new DataSet();
            sda.Fill(ds, "t_00_001 ");
 

        }



        private void Table_Name_Click(object sender, RoutedEventArgs e)
        {  
    
             string table_ini_yes_state, tenant_yes_state,type_state,where_stat,dir_state,savefile_state;
             string curItem = table_name.SelectedItem.ToString();
             table_name.Text = curItem;

             //取得表数据
             DataTable dt;
             dt = GetTableList();
             dt.Select();

             DataView dv = dt.DefaultView;
             dv.RowFilter = "table_data='1' and table_name='" + curItem + "'";

             table_ini_yes_state = dv[0].Row["table_ini"].ToString();

             if (table_ini_yes_state == "1") 
               {table_ini_yes.IsChecked=true;
                table_ini_no.IsChecked=false;}
             else
             { table_ini_yes.IsChecked = false;
               table_ini_no.IsChecked=true;
              }

             tenant_yes_state = dv[0].Row["tenant_ini"].ToString();

             if (tenant_yes_state == "1")
             {
                 tenant_yes.IsChecked = true;
                 tenant_no.IsChecked = false;
             }
             else
             {
                 tenant_yes.IsChecked = false;
                 tenant_no.IsChecked = true;
             }

             type_state = dv[0].Row["ini_type"].ToString();
             
            if (type_state=="I")
            {type_newadd.IsChecked=true;}

            where_stat = dv[0].Row["xml_where"].ToString();
            wheresql.Text = where_stat;

            dir_state = dv[0].Row["xml_savedir"].ToString();
            filedir.Text = dir_state;

            savefile_state = dv[0].Row["xml_savefilename"].ToString();
            SaveFileName.Text = savefile_state;



        }


        private void ORACLE_Checked(object sender, RoutedEventArgs e)
        {

            TxtServer.Text = "192.168.4.182";//192.168.4.64
            TxtPort.Text = "1521";
            TxtDBName.Text = "PFOrcl";
            TxtLoginName.Text = "PFDEV832";
            TxtPassword.Password = "pfdev832";

        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void exportfile_OnClick(object sender, RoutedEventArgs e)
        {
            string table_ini_flag;
            string tenant_ini_flag;
            string type_ini_flag;
            string xmlfilename;
            int    returnvalue;
            string table_name_sub;
            string table_name_sub2;


            //生成文件
            table_name_sub=table_name.Text;
            table_name_sub2=table_name_sub.Substring(1 - 1, 8);

            if (table_name_sub2 == "T_00_005")
            { table_name_sub = "T_00_005"; }
            
            table_ini_flag = "1";
            tenant_ini_flag = "1";
            type_ini_flag = "I";

            if (table_ini_yes.IsChecked == true)
            { table_ini_flag = "1"; }
            else
            { table_ini_flag = "0"; }

            if (tenant_yes.IsChecked == true)
            { tenant_ini_flag = "1"; }
            else
            { tenant_ini_flag = "0"; }

            if (type_newadd.IsChecked == true)
            { type_ini_flag = "I"; }

            if (type_update.IsChecked == true)
            { type_ini_flag = "U"; }

            if (type_updateadd.IsChecked == true)
            { type_ini_flag = "A"; }

            if (type_deleteadd.IsChecked == true)
            { type_ini_flag = "T"; }

            //MessageBox.Show(wheresql.Text + "sql", "提示");

            returnvalue = single_xmlfilecreate(table_name_sub, wheresql.Text, table_ini_flag, tenant_ini_flag, type_ini_flag, SaveFileName.Text);
            if (returnvalue == 0)

            //提示成功
            {
                if (SaveFileName.Text == "")
                { xmlfilename = table_name.Text.Substring(0, 8); }
                else
                { xmlfilename = SaveFileName.Text; }

                if (File.Exists(@filedir.Text + "\\" + xmlfilename + ".XML"))
                {
                    MessageBox.Show(xmlfilename + "文件生成成功！", "提示");
                }
                else
                {
                    MessageBox.Show(xmlfilename + "文件生成失败！", "提示");
                }
            }
            else
            {
                MessageBox.Show(table_name.Text + "过程执行失败！", "提示");
            }


        }

        private void all_exportfile_OnClick(object sender, RoutedEventArgs e)
        {
            string table_name_flag,table_ini_flag,savefile_name_flag;
            string tenant_ini_flag;
            string type_ini_flag;
            string where_sql_flag;
            string xmlfilename;
            int returnvalue;

            DataTable dt_list;
            dt_list = GetTableList();
            dt_list.Select();
            DataView dv = dt_list.DefaultView;
            dv.RowFilter = "table_data='1'";


            for (int i = 0; i < dv.Count; i++)
            {
                table_name_flag = dv[i].Row["table_name"].ToString();
                table_ini_flag = dv[i].Row["table_ini"].ToString();
                tenant_ini_flag = dv[i].Row["tenant_ini"].ToString();
                type_ini_flag  = dv[i].Row["ini_type"].ToString();
                where_sql_flag = dv[i].Row["xml_where"].ToString();
                savefile_name_flag = dv[i].Row["xml_savefilename"].ToString();

                returnvalue = single_xmlfilecreate(table_name_flag, where_sql_flag, table_ini_flag, tenant_ini_flag, type_ini_flag, savefile_name_flag);
                if (returnvalue == 0)

                //提示成功
                {
                    xmlfilename = savefile_name_flag;

                    if (File.Exists(@filedir.Text + "\\" + xmlfilename + ".XML"))
                    {
                    }
                    else
                    {
                        MessageBox.Show(xmlfilename + "文件生成失败！", "提示");
                    }
                }
                else
                {
                    MessageBox.Show(table_name.Text + "过程执行失败！", "提示");
                }

            }

        }

        private int single_xmlfilecreate(string table_name, string where_sql, string tableflag, string in_tenantflag, string in_inidatatype,string savefile_name)
        {

            //执行文件生成存储过程
            int  returnvalue;

            string strOracleConn = CreateOracleConnectionString();
            OracleConnection oracleconn = new OracleConnection(strOracleConn);

            if (oracleconn.State == ConnectionState.Open)
            {

            }
            else
            {
                oracleconn.Open();
            }

            OracleCommand cmd = oracleconn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "zzy_data_xmlfile_create";

            cmd.Parameters.Add("in_table_name", Oracle.DataAccess.Client.OracleDbType.Varchar2, 50).Direction = ParameterDirection.Input;
            cmd.Parameters.Add("in_where_sql", Oracle.DataAccess.Client.OracleDbType.Varchar2, 500).Direction = ParameterDirection.Input;
            cmd.Parameters.Add("in_tableflag", Oracle.DataAccess.Client.OracleDbType.Varchar2, 50).Direction = ParameterDirection.Input;
            cmd.Parameters.Add("in_tenantflag", Oracle.DataAccess.Client.OracleDbType.Varchar2, 50).Direction = ParameterDirection.Input;
            cmd.Parameters.Add("in_inidatatype", Oracle.DataAccess.Client.OracleDbType.Varchar2, 50).Direction = ParameterDirection.Input;

            cmd.Parameters.Add("out_errornumber", Oracle.DataAccess.Client.OracleDbType.Int32, 10).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("out_errorstring", Oracle.DataAccess.Client.OracleDbType.NVarchar2, 200).Direction = ParameterDirection.Output;


            cmd.Parameters["in_table_name"].Value = table_name;
            cmd.Parameters["in_where_sql"].Value = where_sql;
            
            cmd.Parameters["in_tableflag"].Value = tableflag;
            cmd.Parameters["in_tenantflag"].Value = in_tenantflag;
            cmd.Parameters["in_inidatatype"].Value = in_inidatatype;

            

            try
            { cmd.ExecuteNonQuery();
              returnvalue = (int)cmd.Parameters["out_errornumber"].Value;

              //数据导出成时间
              OracleCommand cmd2 = oracleconn.CreateCommand();
              cmd2.CommandText = string.Format("update xml_table_list set DATA_UPDATE='{0}' where table_name='{1}'", DateTime.Now.ToString(), table_name);
              cmd2.ExecuteNonQuery();

            }
            catch (Exception )
            {
                returnvalue = 1;
            }
            finally
            {
                if (oracleconn.State == ConnectionState.Open)
                {
                    oracleconn.Close();
                }
                oracleconn.Dispose();
            }


            //取得表数据
            DataTable dt;
            string xmlfilename;
            dt = GetDataTable();

            dt.Select();

            DataView dv = dt.DefaultView;
            dv.RowFilter = "xml_filename='" + table_name + "' ";

            //写入文件
            if (SaveFileName.Text=="") 
            { xmlfilename = table_name.Substring(0, 8);}
            else
            { xmlfilename = savefile_name; }

            if (File.Exists(@filedir.Text + "\\" + xmlfilename + ".XML"))
            {
                File.Delete(@filedir.Text + "\\" + xmlfilename + ".XML");
            }

            
            FileStream fs = new FileStream(filedir.Text + "\\" + xmlfilename + ".XML", FileMode.Create);

            StreamWriter sw = new StreamWriter(fs);


            for (int i = 0; i < dv.Count; i++)
            {
                string xmlstring = dv[i].Row["xml_string"].ToString();
                //xmlstring=xmlstring.Replace("&", "&amp;");
                //xmlstring=xmlstring.Replace("<", "&lt;");
                //xmlstring=xmlstring.Replace(">", "&gt;");

                sw.WriteLine(xmlstring);
            }

            sw.Close();

            return returnvalue;

        }

        private void SetConfig()
        {
            mConfigInfo = new ConfigInfo();
            mConfigInfo.DBServer = TxtServer.Text;
            mConfigInfo.DBPort = int.Parse(TxtPort.Text);
            mConfigInfo.DBName = TxtDBName.Text;
            mConfigInfo.DBUser = TxtLoginName.Text;
            mConfigInfo.DBPassword = TxtPassword.Password;
        }

        private string CreateOracleConnectionString()
        {
            return string.Format(@"Data Source=(DESCRIPTION =  
                                                                  (ADDRESS_LIST =  
                                                                      (ADDRESS = (PROTOCOL = TCP)(HOST = {0})(PORT = 1521))  
                                                                    )  
                                                                    (CONNECT_DATA =  
                                                                     (SID = {1})  
                                                                     (SERVER = DEDICATED)  
                                                                    )  
                                                                  );User Id={2};Password={3};"

              /* , mConfigInfo.DBServer
                , mConfigInfo.DBName
                , mConfigInfo.DBUser
                , mConfigInfo.DBPassword */
               , "192.168.4.182"
                , "PFOrcl"
                , "PFDEV832"
                , "pfdev832"
              /* , "192.168.5.39"
                , "orcl"
                , "PFDEV"
                , "PF,123"*/

                );
        }

        public DataTable GetDataTable()
        {
            SetConfig();

            string strOracleConn = CreateOracleConnectionString();
            OracleConnection orclConnection = new OracleConnection(strOracleConn);
            OracleCommand oracleCommand = new OracleCommand("select * from xml_table_file where  xml_type=2 order by xml_id", orclConnection);
            OracleDataAdapter adapter = new OracleDataAdapter("select * from xml_table_file where xml_type=2 order by xml_id", strOracleConn);
            DataSet ds = new DataSet();
            adapter.Fill(ds, "xml_table_file");
            orclConnection.Dispose();
            oracleCommand.Dispose();
            adapter.Dispose();
            ds.Dispose();
            return ds.Tables[0];


        }

        public DataTable GetTableList()
        {
            //SetConfig();

            string strOracleConn = CreateOracleConnectionString();
            OracleConnection orclConnection = new OracleConnection(strOracleConn);
            OracleCommand oracleCommand = new OracleCommand("select * from xml_table_list order by table_name ", orclConnection);
            OracleDataAdapter adapter = new OracleDataAdapter("select * from xml_table_list order by table_name ", strOracleConn);
            DataSet ds = new DataSet();
            adapter.Fill(ds, "xml_table_list");
            orclConnection.Dispose();
            oracleCommand.Dispose();
            adapter.Dispose();
            ds.Dispose();
            return ds.Tables[0];

        }

        private void Button_SaveDir_Click(object sender, RoutedEventArgs e)
        {   
             FolderBrowserDialog dialog = new FolderBrowserDialog();
      

             dialog.Description = "请选择文件路径";

             if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filedir.Text = dialog.SelectedPath;
            }
        }




    }
}
