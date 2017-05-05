//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7618eaeb-0f6a-4859-88b6-9595a3e1d6f7
//        CLR Version:              4.0.30319.18063
//        Name:                     UCSystemSetting
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPLanguageManager
//        File Name:                UCSystemSetting
//
//        created by Charley at 2015/6/5 16:35:25
//        http://www.voicecyber.com
//
//======================================================================

using System;
using System.Windows;
using System.Windows.Controls;
using UMPLanguageManager.Models;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;

namespace UMPLanguageManager
{
    /// <summary>
    /// UCSystemSetting.xaml 的交互逻辑
    /// </summary>
    public partial class UCSystemSetting
    {
        public LangConfigInfo ConfigInfo;

        private DatabaseInfo mDatabaseInfo;
        private DatabaseInfo mSyncDBInfo;

        public UCSystemSetting()
        {
            InitializeComponent();

            Loaded += UCSystemSetting_Loaded;
            BtnSave.Click += BtnSave_Click;
            BtnClose.Click += BtnClose_Click;
        }

        void UCSystemSetting_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Init()
        {
            try
            {
                if (ConfigInfo == null)
                {
                    return;
                }
                DatabaseInfo dbInfo = ConfigInfo.DataBaseInfo;
                if (dbInfo != null)
                {
                    mDatabaseInfo = dbInfo;
                }
                dbInfo = ConfigInfo.SyncDBInfo;
                if (dbInfo != null)
                {
                    mSyncDBInfo = dbInfo;
                }
                if (mDatabaseInfo != null)
                {
                    int dbType = mDatabaseInfo.TypeID;
                    for (int i = 0; i < ComboDBType.Items.Count; i++)
                    {
                        var item = ComboDBType.Items[i] as ComboBoxItem;
                        if (item == null) { continue; }
                        if (item.Tag.ToString() == dbType.ToString())
                        {
                            item.IsSelected = true;
                            break;
                        }
                    }
                    TxtDBHost.Text = mDatabaseInfo.Host;
                    TxtDBPort.Text = mDatabaseInfo.Port.ToString();
                    TxtDBName.Text = mDatabaseInfo.DBName;
                    TxtDBUser.Text = mDatabaseInfo.LoginName;
                    TxtDBPassword.Password = mDatabaseInfo.Password; 
                }
                if (mSyncDBInfo != null)
                {
                    int dbType = mSyncDBInfo.TypeID;
                    for (int i = 0; i < ComboSyncDBType.Items.Count; i++)
                    {
                        var item = ComboSyncDBType.Items[i] as ComboBoxItem;
                        if (item == null) { continue; }
                        if (item.Tag.ToString() == dbType.ToString())
                        {
                            item.IsSelected = true;
                            break;
                        }
                    }
                    TxtSyncDBHost.Text = mSyncDBInfo.Host;
                    TxtSyncDBPort.Text = mSyncDBInfo.Port.ToString();
                    TxtSyncDBName.Text = mSyncDBInfo.DBName;
                    TxtSyncDBUser.Text = mSyncDBInfo.LoginName;
                    TxtSyncDBPassword.Password = mSyncDBInfo.Password; 
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            var parent = Parent as PopupWindow;
            if (parent != null)
            {
                parent.DialogResult = false;
                parent.Close();
            }
        }

        void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //保存数据库信息
                var item = ComboDBType.SelectedItem as ComboBoxItem;
                if (item == null) { return; }
                int dbType;
                if (!int.TryParse(item.Tag.ToString(), out dbType))
                {
                    ShowErrorMessage(string.Format("DBType invalid"));
                    return;
                }
                string strHost = TxtDBHost.Text;
                string strPort = TxtDBPort.Text;
                string strDBName = TxtDBName.Text;
                string strDBUser = TxtDBUser.Text;
                string strDBPassword = TxtDBPassword.Password;
                if (string.IsNullOrEmpty(strHost)
                    || string.IsNullOrEmpty(strPort)
                    || string.IsNullOrEmpty(strDBName)
                    || string.IsNullOrEmpty(strDBUser)
                    || string.IsNullOrEmpty(strDBPassword))
                {
                    ShowErrorMessage(string.Format("DBParam is empty"));
                    return;
                }
                int intPort;
                if (!int.TryParse(strPort, out intPort))
                {
                    ShowErrorMessage(string.Format("DBPort invalid"));
                    return;
                }
                DatabaseInfo dbInfo = new DatabaseInfo();
                dbInfo.TypeID = dbType;
                dbInfo.TypeName = dbType == 3 ? "Oracle" : "MSSQL";
                dbInfo.Host = strHost;
                dbInfo.Port = intPort;
                dbInfo.DBName = strDBName;
                dbInfo.LoginName = strDBUser;
                dbInfo.Password = strDBPassword;
                dbInfo.RealPassword = strDBPassword;

                if (!TestDatabaseConnection(dbInfo)) { return; }

                mDatabaseInfo = dbInfo;
                if (ConfigInfo == null)
                {
                    ConfigInfo=new LangConfigInfo();
                }
                ConfigInfo.DataBaseInfo = mDatabaseInfo;

                //保存同步数据库信息
                item = ComboSyncDBType.SelectedItem as ComboBoxItem;
                if (item == null)
                {
                    dbType = 2;
                }
                else
                {
                    if (!int.TryParse(item.Tag.ToString(), out dbType))
                    {
                        ShowErrorMessage(string.Format("DBType invalid"));
                        return;
                    }
                }
                strHost = TxtSyncDBHost.Text;
                strPort = TxtSyncDBPort.Text;
                strDBName = TxtSyncDBName.Text;
                strDBUser = TxtSyncDBUser.Text;
                strDBPassword = TxtSyncDBPassword.Password;
                if (!int.TryParse(strPort, out intPort))
                {
                    intPort = 0;
                }
                dbInfo = new DatabaseInfo();
                dbInfo.TypeID = dbType;
                dbInfo.TypeName = dbType == 3 ? "Oracle" : "MSSQL";
                dbInfo.Host = strHost;
                dbInfo.Port = intPort;
                dbInfo.DBName = strDBName;
                dbInfo.LoginName = strDBUser;
                dbInfo.Password = strDBPassword;
                dbInfo.RealPassword = strDBPassword;

                mSyncDBInfo = dbInfo;
                if (ConfigInfo == null)
                {
                    ConfigInfo = new LangConfigInfo();
                }
                ConfigInfo.SyncDBInfo = mSyncDBInfo;

                var parent = Parent as PopupWindow;
                if (parent != null)
                {
                    parent.DialogResult = true;
                    parent.Close();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private bool TestDatabaseConnection(DatabaseInfo dbInfo)
        {
            string strCon = dbInfo.GetConnectionString();
            string strSql = string.Format("Select C001 from t_00_005 where 1 = 2");
            OperationReturn optReturn;
            switch (dbInfo.TypeID)
            {
                case 2:
                    optReturn = MssqlOperation.GetDataSet(strCon, strSql);
                    if (!optReturn.Result)
                    {
                        ShowErrorMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return false;
                    }
                    break;
                case 3:
                    optReturn = OracleOperation.GetDataSet(strCon, strSql);
                    if (!optReturn.Result)
                    {
                        ShowErrorMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return false;
                    }
                    break;
                default:
                    ShowErrorMessage(string.Format("DBType invalid"));
                    return false;
            }
            return true;
        }

        #region Basic

        private void ShowErrorMessage(string msg)
        {
            Dispatcher.Invoke(new Action(() => MessageBox.Show(msg, "UMP Language Manager", MessageBoxButton.OK, MessageBoxImage.Error)));
        }

        private void ShowInfoMessage(string msg)
        {
            Dispatcher.Invoke(new Action(() => MessageBox.Show(msg, "UMP Language Manager", MessageBoxButton.OK, MessageBoxImage.Information)));
        }

        #endregion
    }
}
