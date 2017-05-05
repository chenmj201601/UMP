using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.CommonService04;
using VoiceCyber.UMP.Communications;

namespace UMPS2102Demo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        private MonitorClient mNetMonitorClient;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            BtnTest.Click += BtnTest_Click;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DatabaseInfo dbInfo = new DatabaseInfo();
                dbInfo.TypeID = 2;
                dbInfo.Host = "192.168.4.182";
                dbInfo.Port = 1433;
                dbInfo.DBName = "UMPDataDB0713";
                dbInfo.LoginName = "PFDEV";
                dbInfo.Password = "PF,123";
                //dbInfo.TypeID = 2;
                //dbInfo.Host = "10.26.1.52";
                //dbInfo.Port = 1433;
                //dbInfo.DBName = "UMPDataDB";
                //dbInfo.LoginName = "sa";
                //dbInfo.Password = "Voicecodes9";
                string strConn = dbInfo.GetConnectionString();
                DataSet objDataSet;
                string strSql;
                SqlConnection objConn;
                SqlDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                DataRow dr;
                bool isAdd;
                List<ExtensionInfo> listExtInfos = new List<ExtensionInfo>();
                ExtensionInfo extInfo;
                strSql =
                    string.Format(
                        "select * from t_11_101_00000 where c001 > 2250000000000000000 and c001 < 2260000000000000000 and c002 = 1 order by c001,c002");
                objConn = new SqlConnection(strConn);
                objAdapter = new SqlDataAdapter(strSql, objConn);
                objDataSet = new DataSet();
                try
                {
                    objAdapter.Fill(objDataSet);
                    for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                    {
                        dr = objDataSet.Tables[0].Rows[i];
                        int key = Convert.ToInt32(dr["C011"]);
                        extInfo = new ExtensionInfo();
                        extInfo.ObjID = 1040000000000000001 + key;
                        extInfo.ChanObjID = Convert.ToInt64(dr["C001"]);
                        listExtInfos.Add(extInfo);
                    }
                    for (int i = 0; i < listExtInfos.Count; i++)
                    {
                        extInfo = listExtInfos[i];
                        strSql = string.Format("select * from t_11_101_00000 where c001 = {0} order by c001,c002", extInfo.ChanObjID);
                        objAdapter.SelectCommand = new SqlCommand(strSql, objConn);
                        objDataSet = new DataSet();
                        objAdapter.Fill(objDataSet);
                        for (int j = 0; j < objDataSet.Tables[0].Rows.Count; j++)
                        {
                            dr = objDataSet.Tables[0].Rows[j];

                            int row = Convert.ToInt32(dr["C002"]);
                            if (row == 1)
                            {
                                extInfo.ID = Convert.ToInt32(dr["C012"]);
                                extInfo.ServerObjID = Convert.ToInt64(dr["C013"]);
                                extInfo.ChanID = extInfo.ID;
                            }
                            if (row == 2)
                            {
                                extInfo.Extension = dr["C012"].ToString();
                                extInfo.ChanName = extInfo.Extension;
                            }
                        }
                    }
                    strSql =
                        string.Format(
                            "select * from t_11_101_00000 where c001 > 1040000000000000000 and c001 < 1050000000000000000 and c002 = 1 order by c001,c002");
                    objAdapter.SelectCommand = new SqlCommand(strSql, objConn);
                    objDataSet = new DataSet();
                    objCmdBuilder = new SqlCommandBuilder(objAdapter);
                    objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                    objCmdBuilder.SetAllValues = false;
                    objAdapter.Fill(objDataSet);
                    for (int i = 0; i < listExtInfos.Count; i++)
                    {
                        extInfo = listExtInfos[i];
                        isAdd = false;
                        dr =
                            objDataSet.Tables[0].Select(string.Format("C001 = {0} and C002 = {1}", extInfo.ObjID, 1))
                                .FirstOrDefault();
                        if (dr == null)
                        {
                            dr = objDataSet.Tables[0].NewRow();
                            isAdd = true;
                        }
                        dr["C001"] = extInfo.ObjID;
                        dr["C002"] = 1;
                        dr["C011"] = 1010000000000000001;
                        dr["C017"] = extInfo.Extension;
                        dr["C018"] = extInfo.ChanName;
                        if (isAdd)
                        {
                            objDataSet.Tables[0].Rows.Add(dr);
                        }
                    }

                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();

                    strSql =
                        string.Format(
                            "select * from t_11_101_00000 where c001 > 1040000000000000000 and c001 < 1050000000000000000 and c002 = 2 order by c001,c002");
                    objAdapter.SelectCommand = new SqlCommand(strSql, objConn);
                    objDataSet = new DataSet();
                    objCmdBuilder = new SqlCommandBuilder(objAdapter);
                    objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                    objCmdBuilder.SetAllValues = false;
                    objAdapter.Fill(objDataSet);
                    for (int i = 0; i < listExtInfos.Count; i++)
                    {
                        extInfo = listExtInfos[i];
                        isAdd = false;
                        dr =
                            objDataSet.Tables[0].Select(string.Format("C001 = {0} and C002 = {1}", extInfo.ObjID, 2))
                                .FirstOrDefault();
                        if (dr == null)
                        {
                            dr = objDataSet.Tables[0].NewRow();
                            isAdd = true;
                        }
                        dr["C001"] = extInfo.ObjID;
                        dr["C002"] = 2;
                        dr["C015"] = extInfo.ServerObjID;
                        dr["C016"] = extInfo.ChanID;
                        dr["C020"] = extInfo.ChanObjID;
                        if (isAdd)
                        {
                            objDataSet.Tables[0].Rows.Add(dr);
                        }
                    }

                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();

                    strSql =
                       string.Format(
                           "select * from t_11_201_00000 where c003 = 1020000000000000001 and c004 > 1040000000000000000 and c004 < 1050000000000000000");
                    objAdapter.SelectCommand = new SqlCommand(strSql, objConn);
                    objDataSet = new DataSet();
                    objCmdBuilder = new SqlCommandBuilder(objAdapter);
                    objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                    objCmdBuilder.SetAllValues = false;
                    objAdapter.Fill(objDataSet);
                    for (int i = 0; i < listExtInfos.Count; i++)
                    {
                        extInfo = listExtInfos[i];
                        isAdd = false;
                        dr =
                            objDataSet.Tables[0].Select(string.Format("C004 = {0}", extInfo.ObjID))
                                .FirstOrDefault();
                        if (dr == null)
                        {
                            dr = objDataSet.Tables[0].NewRow();
                            isAdd = true;
                        }
                        dr["C001"] = 0;
                        dr["C002"] = 0;
                        dr["C003"] = 1020000000000000001;
                        dr["C004"] = extInfo.ObjID;
                        dr["C005"] = DateTime.Parse("2014/1/1");
                        dr["C006"] = DateTime.Parse("2199/12/31");
                        if (isAdd)
                        {
                            objDataSet.Tables[0].Rows.Add(dr);
                        }
                    }

                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();


                    AppendMessage(string.Format("End.\t{0}", listExtInfos.Count));

                }
                catch (Exception ex)
                {
                    AppendMessage(ex.Message);
                }
                finally
                {
                    if (objConn.State == ConnectionState.Open)
                    {
                        objConn.Close();
                    }
                    objConn.Dispose();
                }
            }
            catch (Exception ex)
            {
                AppendMessage(string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void AppendMessage(string msg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                TxtMsg.AppendText(string.Format("{0}\t{1}\r\n", DateTime.Now.ToString("HH:mm:ss.f"), msg));
                TxtMsg.ScrollToEnd();
            }));
        }


        private void InitNMonMonitorClient()
        {
            try
            {
                if (mNetMonitorClient != null)
                {
                    mNetMonitorClient.Stop();
                    mNetMonitorClient = null;
                }

                //string strAddress = App.Session.AppServerInfo.Address;
                //int intPort = App.Session.AppServerInfo.SupportHttps
                //    ? App.Session.AppServerInfo.Port - 5
                //    : App.Session.AppServerInfo.Port - 4;

                string strAddress = "192.168.5.31";
                int intPort = 8081 - 4;

                mNetMonitorClient = new MonitorClient();
                mNetMonitorClient.Debug += (mode, msg) => AppendMessage(string.Format("MonitorClient\t{0}", msg));
                mNetMonitorClient.ReturnMessageReceived += NMonMonitorClient_ReturnMessageReceived;
                mNetMonitorClient.NotifyMessageReceived += NMonMonitorClient_NotifyMessageReceived;
                mNetMonitorClient.Connect(strAddress, intPort);
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
            }
        }

        void NMonMonitorClient_NotifyMessageReceived(NotifyMessage notMessage)
        {

        }

        void NMonMonitorClient_ReturnMessageReceived(ReturnMessage retMessage)
        {

        }
    }
}
