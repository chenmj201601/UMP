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
using MessageBox=System.Windows.Forms.MessageBox;
using TreeNode = System.Web.UI.WebControls.TreeNode;
using TreeView = System.Web.UI.WebControls.TreeView;
using System.IO;
using System.Data.Common;
using Oracle.DataAccess.Client;
using System.Data.OleDb;


namespace FunctionTree
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        #region Static Members

        private static char[] ArrRandomChars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        private static string[] ArrVoiceIPs =
        {
            "192.168.4.10",
            "192.168.4.11", "192.168.4.12", "192.168.4.13", "192.168.4.14",
            "192.168.4.15", "192.168.4.16", "192.168.4.17", "192.168.4.18", 
            "192.168.4.19"
        };
        private static int[] ArrVoiceIDs =
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9
        };

        string databasetype;

        #endregion
      

        private List<VoiceInfo> mListVoiceInfos;
        private ConfigInfo mConfigInfo;
   
        public MainWindow()
        {
            InitializeComponent();

            mListVoiceInfos = new List<VoiceInfo>();

        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {

            TxtServer.Text = "192.168.4.182";//192.168.4.64
            TxtPort.Text = "1433";
            TxtDBName.Text = "UMPDataDB";
            TxtLoginName.Text = "PFDEV832";
            //TxtPassword.Password = "PF,123";//voicecyber
            TxtPassword.Password = "pfdev832";
            database_oracle.IsChecked = true;
            oracle.IsChecked = true;

            getData();

           /* TxtServer.Text = "192.168.8.225";//192.168.4.64
            TxtPort.Text = "1433";
            TxtDBName.Text = "IMPDatabase";
            TxtLoginName.Text = "sa";
            TxtPassword.Password = "voicecyber";//voicecyber
            TxtBeginTime.Text = DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd HH:mm:ss");
            TxtEndTime.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss");*/
            
        }

        void getData()
        {
            SetConfig();

            string strOracleConn = CreateOracleConnectionString();
            OracleConnection orclConnection = new OracleConnection(strOracleConn);
            OracleCommand oracleCommand = new OracleCommand("select t.table_name from xml_table_list t order by table_name", orclConnection);
            OracleDataAdapter adapter = new OracleDataAdapter("select t.table_name from xml_table_list t order by table_name", strOracleConn);
            DataSet ds = new DataSet();
            adapter.Fill(ds, "xml_table_list");
            orclConnection.Dispose();
            oracleCommand.Dispose();
            adapter.Dispose();
            ds.Dispose();

            table_name.Items.Clear();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {

                DataRow dr = ds.Tables[0].Rows[i];
                table_name.Items.Add(dr["table_name"].ToString());
            }


        }

        private void Table_Name_Click(object sender, RoutedEventArgs e)
        {
            SetConfig();
            string strSql;
            string curItem = table_name.SelectedItem.ToString();

            string strOracleConn = CreateOracleConnectionString();
            OracleConnection orclConnection = new OracleConnection(strOracleConn);
            strSql = string.Format("select t.table_ini,t.tenant_ini from xml_table_list t where  t.table_name='{0}' "
                                      , curItem);
            OracleCommand oracleCommand = new OracleCommand(strSql, orclConnection);
            OracleDataAdapter adapter = new OracleDataAdapter(strSql, strOracleConn);
            DataSet ds = new DataSet();
            adapter.Fill(ds, "xml_table_list");
            orclConnection.Dispose();
            oracleCommand.Dispose();
            adapter.Dispose();
            ds.Dispose();

            DataRow dr = ds.Tables[0].Rows[0];

            if (dr["table_ini"].ToString() == "1")
            {
                table_ini.IsChecked = true;
            }
            else
            { table_ini.IsChecked = false; }

            if (dr["tenant_ini"].ToString() == "1")
            {
                tenant_ini.IsChecked = true;
            }
            else
            { tenant_ini.IsChecked = false; }
        }



        public void DatabaseIni()
        {

            SetConfig();
            string strConn = CreateConnectionString();
            SqlConnection objConn = GetConnection(strConn);

            string strSql = string.Empty;

            try
            {
                if (objConn.State != ConnectionState.Open)
                {
                    objConn.Open();

                }


            }
            catch (Exception ex)
            {
                SubDebug(string.Format("Insert fail.\t{0}\t{1}", ex.Message, strSql));

            }
        }

        private void BtnClose_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }



        private void CreateInsertSqlString(IMPRecordDataInfo recordInfo, ref string strSql)
        {
            strSql =
                    string.Format(
                        "insert into recordoriginaldata (RecordReference,VoiceIP,RootDisk,VoiceID,Channel,StartRecordTime,StopRecordTime,RecordLength,AgentID,CallerID");
            strSql += string.Format(",CalledID,Extension,DirectionFlag,CallerDTMF,CalledDTMF,FileFormat,EncryFlag,InsertTime,BackupCount,DeleteFlag,DeleteTime,IMPField01");
            //strSql += string.Format(",DeleteIdentify,VoiceSite,MediaType)");
            strSql += string.Format(",DeleteIdentify)");
            //strSql += string.Format(" values ('{0}','{1}','{2}',{3},{4},'{5}','{6}',{7},'{8}','{9}','{10}','{11}','{12}','{13}','{14}',{15},'{16}','{17}',{18},'{19}','{20}','{21}',{22},{23})"
            strSql += string.Format(" values ('{0}','{1}','{2}',{3},{4},'{5}','{6}',{7},'{8}','{9}','{10}','{11}','{12}','{13}','{14}',{15},'{16}','{17}',{18},'{19}','{20}','{21}','{22}')"
                , recordInfo.RecordReference
                , recordInfo.VoiceIP
                , recordInfo.RootDisk
                , recordInfo.VoiceID
                , recordInfo.Channel
                , recordInfo.StartRecordTime
                , recordInfo.StopRecordTime
                , recordInfo.RecordLength
                , recordInfo.AgentID
                , recordInfo.CallerID
                , recordInfo.CalledID
                , recordInfo.Extension
                , recordInfo.DirectionFlag
                , recordInfo.CallerDTMF
                , recordInfo.CalledDTMF
                , recordInfo.FileFormat
                , recordInfo.EncryFlag
                , recordInfo.InsertTime
                , recordInfo.BackupCount
                , recordInfo.DeleteFlag
                , recordInfo.DeleteTime
                , recordInfo.RecordReference
                , recordInfo.DeleteIdentify
                );
              //  , recordInfo.VoiceSite
              //  , recordInfo.MediaType);
        }

        private string CreateConnectionString()
        {
            return string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}"
                , mConfigInfo.DBServer
                , mConfigInfo.DBPort
                , mConfigInfo.DBName
                , mConfigInfo.DBUser
                , mConfigInfo.DBPassword);
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

        private void SetProgressStatues(bool isWorking)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (isWorking)
                {
                    ProgressStatues.Visibility = Visibility.Visible;
                }
                else
                {
                    ProgressStatues.Visibility = Visibility.Collapsed;
                }
            }));
        }

        private void InitProgressStatues(int max)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                ProgressStatues.Maximum = max;
            }));
        }

        private void SetProgressStatuesValue(int value)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                ProgressStatues.Value = value;
            }));
        }

        private void SetStatues(string msg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                LbInfo.Content = msg;
            }));
        }

        private void SubDebug(string msg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                TxtMsg.AppendText(string.Format("{0}\t{1}\r\n", DateTime.Now.ToString("HH:mm:ss.fff"), msg));
                TxtMsg.ScrollToEnd();
            }));
        }
       
        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public DataTable GetDataTable(string table_name)
        {
            SetConfig();

            string strOracleConn = CreateOracleConnectionString();
            OracleConnection orclConnection = new OracleConnection(strOracleConn);
            OracleCommand oracleCommand = new OracleCommand("select * from xml_table_file order by xml_id", orclConnection);
            OracleDataAdapter adapter = new OracleDataAdapter("select * from xml_table_file order by xml_id", strOracleConn);
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
            SetConfig();

            string strOracleConn = CreateOracleConnectionString();
            OracleConnection orclConnection = new OracleConnection(strOracleConn);
            OracleCommand oracleCommand = new OracleCommand("select * from xml_table_list ", orclConnection);
            OracleDataAdapter adapter = new OracleDataAdapter("select * from xml_table_list ", strOracleConn);
            DataSet ds = new DataSet();
            adapter.Fill(ds, "xml_table_list");
            orclConnection.Dispose();
            oracleCommand.Dispose();
            adapter.Dispose();
            ds.Dispose();
            return ds.Tables[0];

        }

        private void single_OnClick(object sender, RoutedEventArgs e)
        {

            DataTable dt_list;
            dt_list = GetTableList();
            dt_list.Select();
            DataView dv = dt_list.DefaultView;
            dv.RowFilter = " table_name='" + table_name.Text + "' ";

            for (int i = 0; i < dv.Count; i++)
            {

                single_xmlfilecreate(dv[i].Row["table_name"].ToString());
            }

            
            //提示成功

            MessageBox.Show(table_name.Text+"文件生成成功！", "提示");
           

        }

        private void single_xmlfilecreate(string table_name)
        {

            //执行文件生成存储过程

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
            cmd.CommandText = "zzy_table_xmlfile_create";

            cmd.Parameters.Add("in_table_name", Oracle.DataAccess.Client.OracleDbType.Varchar2,50).Direction = ParameterDirection.Input;


            cmd.Parameters.Add("out_errornumber", Oracle.DataAccess.Client.OracleDbType.Int32,10).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("out_errorstring", Oracle.DataAccess.Client.OracleDbType.NVarchar2,200).Direction = ParameterDirection.Output;

            cmd.Parameters["in_table_name"].Value = table_name;

                        
            cmd.ExecuteNonQuery();

            //表结构生成时间

            OracleCommand cmd2 = oracleconn.CreateCommand();
            cmd2.CommandText = string.Format("update xml_table_list set STRU_UPDATE='{0}' where table_name='{1}'", DateTime.Now.ToString(), table_name);
            cmd2.ExecuteNonQuery();

            //取得表数据



            DataTable dt;
            string xmlfilename;
            dt = GetDataTable(table_name);

            dt.Select();

            DataView dv = dt.DefaultView;
            dv.RowFilter = "xml_filename='" + table_name + "' ";

            //写入文件
            if (File.Exists(@"e:\xmlfile\" + table_name + ".XML"))
            {
                File.Delete(@"e:\xmlfile\" + table_name + ".XML");
            }

            if (table_name.Substring(0, 6) != "T_44_0")

            { xmlfilename = table_name.Substring(0, 8); }

            else
            { xmlfilename = table_name;            
            
            }

            FileStream fs = new FileStream(filedir.Text + "\\" + xmlfilename + ".XML", FileMode.Create);

            StreamWriter sw = new StreamWriter(fs);


            for (int i = 0; i < dv.Count; i++)
            {

                sw.WriteLine(dv[i].Row["xml_string"].ToString());
                if (dv[i].Row["xml_string"].ToString()=="</TableDefine>" )
                {break;}

                
                
            }
            //MessageBox.Show((dv.Count-1).ToString(), "提示");
            //MessageBox.Show(dv[dv.Count-1].Row["xml_string"].ToString(), "提示");

            sw.Close();


        }


        private void Show(char p)
        {
 	        throw new NotImplementedException();
        }

        private void all_OnClick(object sender, RoutedEventArgs e)
        {

            DataTable dt_list;
            dt_list = GetTableList();
            dt_list.Select();
            DataView dv = dt_list.DefaultView;
            dv.RowFilter = " TABLE_STRUC='" + 1 + "' ";

            for (int i = 0; i < dv.Count; i++)
            {

                single_xmlfilecreate(dv[i].Row["table_name"].ToString());
            }


            //提示成功

            MessageBox.Show("全部文件生成成功！", "提示");

        }

        public static SqlConnection GetConnection(string strConn)
        {

            return new SqlConnection(strConn);
        }

        public enum OracleDataType
        {
            Varchar2,
            Nvarchar2,
            Int32,
            Char,
            Date
        }

        #region databasetype

        private void ORACLE_Checked(object sender, RoutedEventArgs e)
        {
            databasetype = "oracle";
            TxtServer.Text = "192.168.4.182";//192.168.4.64
            TxtPort.Text = "1521";
            TxtDBName.Text = "PFOrcl";
            TxtLoginName.Text = "PFDEV832";

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
        
        #endregion


    }
}
