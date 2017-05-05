using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using UMPServicePack.PublicClasses;
using UMPServicePackCommon;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using System.IO;
using System.Data;
using System.Threading;


namespace UMPServicePack
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        #region 定义变量
        BackgroundWorker mBackgroundWorker = null;
        //界面是否加载完成
        bool bIsloaded = false;
        //数据加载是否成功（包括检查环境、获取数据库等）
        bool bIsLoadDataSuccess = false;
        #endregion

        public Login()
        {
            InitializeComponent();
            Loaded += Login_Loaded;
        }

        void Login_Loaded(object sender, RoutedEventArgs e)
        {
            DrawingBackground.DrawWindowsBackgond(this);

            #region Init
            InitLanguageMenu();
            #endregion

            #region 关联事件
            MouseLeftButtonDown += (s, be) => DragMove();
            ButtonApplicationMenu.Click += ButtonApplicationMenu_Click;
            ButtonCloseWindow.Click += ButtonCloseWindow_Click;
            ButtonCloseLogin.Click += ButtonCloseWindow_Click;
            ButtonLogin.Click += ButtonLogin_Click;
            #endregion
            bIsloaded = true;
            ChangeLanguage(App.gStrCurrLang);
            BeginLoading();
        }



        #region 窗口控件的事件
        void ButtonApplicationMenu_Click(object sender, RoutedEventArgs e)
        {
            Button LButtonClicked = sender as Button;
            //目标   
            LButtonClicked.ContextMenu.PlacementTarget = LButtonClicked;
            //位置   
            LButtonClicked.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            //显示菜单   
            LButtonClicked.ContextMenu.IsOpen = true;
        }

        /// <summary>
        /// 语言菜单单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LanguageItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            ChangeLanguage(item.Tag.ToString());
            MenuItem LanItem = null;
            for (int i = 0; i < ButtonApplicationMenu.ContextMenu.Items.Count; i++)
            {
                LanItem = ButtonApplicationMenu.ContextMenu.Items[i] as MenuItem;
                if (LanItem.IsChecked)
                {
                    LanItem.IsChecked = false;
                }
            }
            item.IsChecked = true;
            App.gStrCurrLang = item.Tag.ToString();
        }

        /// <summary>
        /// 关闭窗口 退出程序 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ButtonCloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUserName.Text) || string.IsNullOrEmpty(txtPwd.Password))
            {
                App.ShowException(App.GetLanguage("string11", "string11"));
                return;
            }
            OperationReturn optReturn = DatabaseOperator.CheckUser(txtUserName.Text, txtPwd.Password);
            if (!optReturn.Result)
            {
                string strMessageID = string.Empty;
                //string strErrorMessage = "Error code :" + optReturn.Code + ". ";
                string strErrorMessage = string.Format("ErrorCode:{0};\tErrorMsg:{1}", optReturn.Code, optReturn.Message);
                switch (optReturn.Code)
                {
                    case ConstDefines.Get_UserName_Pwd_Exception:
                        strMessageID = "string15";
                        strErrorMessage += optReturn.Message;
                        break;
                    case ConstDefines.UserName_Or_Pwd_Not_Exists:
                        strMessageID = "string12";
                        strErrorMessage += "User " + txtUserName + " login failed.User name does not exist or password error";
                        break;
                    case ConstDefines.Check_User_Exception:
                        strMessageID = "string15";
                        strErrorMessage += "Abnormal login process. " + optReturn.Message;
                        break;
                    case ConstDefines.User_Overdue:
                        strMessageID = "string14";
                        strErrorMessage += "The user has expired";
                        break;
                    case ConstDefines.Get_User_Role_Failed:
                        strMessageID = "string15";
                        App.WriteLog("Get user roles failed. " + optReturn.Message);
                        break;
                    case ConstDefines.User_Not_Admin:
                        strMessageID = "string13";
                        strErrorMessage += "The user entered is not an administrator role";
                        break;
                }
                App.ShowException(App.GetLanguage(strMessageID, strMessageID));
                App.WriteLog(strErrorMessage);
                return;
            }
            App.CurrUserID = long.Parse(optReturn.Data.ToString());
            App.CurrUserName = txtUserName.Text;
            //如果登录成功 进入主界面
            MainWindow mainWin = new MainWindow();
            this.Hide();
            mainWin.ShowDialog();
        }
        #endregion

        #region Init controls content
        //使用线程 在界面显示后加载数据
        private void BeginLoading()
        {
            mBackgroundWorker = new BackgroundWorker();
            mBackgroundWorker.DoWork += (s, de) =>
                {
                    while (true)
                    {
                        if (bIsloaded)
                        {
                            GetServiceStatus();
                            GetAppsInstalled();
                            GetServerType();
                            if (!App.bIsLoggingServer && !App.bIsUMPServer)
                            {
                                Dispatcher.Invoke(new Action(() =>
                                {
                                    App.ShowMessage(App.GetLanguage("string8", "string8"));
                                }));
                                App.WriteLog("No products installed on this server, please install UMP and then upgrade");
                                break;
                            }
                            OperationReturn optReturn = GetDatabaseInfo();
                            if (!optReturn.Result)
                            {
                                switch (optReturn.Code)
                                {
                                    case ConstDefines.RET_Database_Null:
                                        Dispatcher.Invoke(new Action(() =>
                                            {
                                                App.ShowException(App.GetLanguage("string9", "string9"));
                                            }));
                                        App.WriteLog("Get database infomation failed.");
                                        break;
                                    case ConstDefines.Get_Database_Info_Exception:
                                        Dispatcher.Invoke(new Action(() =>
                                        {
                                            App.ShowException(App.GetLanguage("string10", "string10"));
                                        }));
                                        App.WriteLog("Get database infomation failed." + optReturn.Message);
                                        break;
                                }
                                break;
                            }
                            DatabaseInfo dbInfo = optReturn.Data as DatabaseInfo;

                            //OperationReturn optReturn;
                            //DatabaseInfo dbInfo = new DatabaseInfo();
                            //dbInfo.TypeID = 2;
                            //dbInfo.Host = "192.168.4.182";
                            //dbInfo.Port = 1433;
                            //dbInfo.DBName = "UMPDataDB0722";
                            //dbInfo.LoginName = "PFDEV";
                            //dbInfo.Password = "PF,123";

                            App.WriteLog(string.Format("DatabaseInfo:{0}", dbInfo));


                            App.currDBInfo = dbInfo;
                            optReturn = DatabaseOperator.GetLastVersion();
                            if (!optReturn.Result)
                            {
                                if (optReturn.Code == ConstDefines.Get_T000_Failed || optReturn.Code == ConstDefines.Get_Version_Exception)
                                {
                                    Dispatcher.Invoke(new Action(() =>
                                        {
                                            App.ShowException(App.GetLanguage("string18", "string18"));
                                        }));
                                    App.WriteLog("Error code :" + optReturn.Code + ". " + optReturn.Message);
                                    break;
                                }
                                else if (optReturn.Code == ConstDefines.T000_Is_Null)
                                {
                                    Dispatcher.Invoke(new Action(() =>
                                    {
                                        App.ShowException(App.GetLanguage("string19", "string19"));
                                    }));
                                    App.WriteLog("Error code :" + optReturn.Code + ". T_00_000 is null");
                                }
                                else
                                {
                                    App.WriteLog("Error code :" + optReturn.Code + ". " + optReturn.Message);
                                }
                            }
                            string str = optReturn.Data.ToString();
                            App.gStrLastVersion = str;

                            //App.gStrLastVersion = "8.03.001";

                            App.WriteLog(string.Format("Version:{0}", App.gStrLastVersion));

                            bIsLoadDataSuccess = true;
                            break;
                        }
                    }
                };
            mBackgroundWorker.RunWorkerCompleted += (s, de) =>
                {
                    myWaiter.Visibility = Visibility.Collapsed;
                    loginWin.IsEnabled = true;
                    if (!bIsLoadDataSuccess)
                    {
                        ButtonLogin.IsEnabled = false;
                    }
                    txtUserName.Focus();
                };
            myWaiter.Visibility = Visibility.Visible;
            loginWin.IsEnabled = false;
            mBackgroundWorker.RunWorkerAsync();
        }

        /// <summary>
        /// 初始化语言菜单
        /// </summary>
        private void InitLanguageMenu()
        {
            ContextMenu menu = new ContextMenu();
            MenuItem item = new MenuItem();
            item.Header = Application.Current.FindResource("English");
            item.Click += LanguageItem_Click;
            item.Tag = "1033";
            if (item.Tag.ToString() == App.gStrCurrLang)
            {
                item.IsChecked = true;
            }
            menu.Items.Add(item);

            item = new MenuItem();
            item.Header = Application.Current.FindResource("Chinese");
            item.Click += LanguageItem_Click;
            item.Tag = "2052";
            if (item.Tag.ToString() == App.gStrCurrLang)
            {
                item.IsChecked = true;
            }
            menu.Items.Add(item);

            item = new MenuItem();
            item.Header = Application.Current.FindResource("ChineseTaiWan");
            item.Click += LanguageItem_Click;
            item.Tag = "1028";
            if (item.Tag.ToString() == App.gStrCurrLang)
            {
                item.IsChecked = true;
            }
            menu.Items.Add(item);

            item = new MenuItem();
            item.Header = Application.Current.FindResource("Japanese");
            item.Click += LanguageItem_Click;
            item.Tag = "1041";
            if (item.Tag.ToString() == App.gStrCurrLang)
            {
                item.IsChecked = true;
            }
            menu.Items.Add(item);

            ButtonApplicationMenu.ContextMenu = menu;
        }

        /// <summary>
        /// 获得打补丁之前 各个服务的状态
        /// </summary>
        private void GetServiceStatus()
        {
            App.dicAllServiceStatus.Clear();
            OperationReturn optReturn = null;
            for (int i = 0; i < App.lstAllServiceNames.Count; i++)
            {
                optReturn = CommonFuncs.GetComputerServiceStatus(App.lstAllServiceNames[i]);
                if (!optReturn.Result)
                {
                    App.WriteLog("Service " + App.lstAllServiceNames[i] + " not installed");
                    App.WriteLog(optReturn.Message);
                }
                else
                {
                    ServiceEnty service = optReturn.Data as ServiceEnty;
                    App.dicAllServiceStatus.Add(App.lstAllServiceNames[i], service);
                    switch (service.ServiceStatus)
                    {
                        case (int)ServiceStatusType.Not_Exit:
                            App.WriteLog("Service " + App.lstAllServiceNames[i] + " not installed");
                            break;
                        case (int)ServiceStatusType.Started:
                            App.WriteLog("Service " + App.lstAllServiceNames[i] + " installed and started");
                            break;
                        case (int)ServiceStatusType.Stoped:
                            App.WriteLog("Service " + App.lstAllServiceNames[i] + " installed and stoped");
                            break;
                    }
                }
            }

        }

        /// <summary>
        /// 根据服务状态判断是Logging服务器还是UMP服务器 从而从相应位置拿数据库配置参数
        /// </summary>
        private void GetServerType()
        {
            if (App.dicAppInstalled.Keys.Contains(ConstDefines.UMP))
            {
                //已经安装了UMP 
                App.bIsUMPServer = true;
            }
            if (App.dicAppInstalled.Keys.Contains(ConstDefines.UMPAlarmClient) || App.dicAppInstalled.Keys.Contains(ConstDefines.UMPCMServer) ||
                App.dicAppInstalled.Keys.Contains(ConstDefines.UMPAlarmServer) || App.dicAppInstalled.Keys.Contains(ConstDefines.UMPCQC) ||
                App.dicAppInstalled.Keys.Contains(ConstDefines.UMPCTIHub) || App.dicAppInstalled.Keys.Contains(ConstDefines.UMPDEC) ||
                App.dicAppInstalled.Keys.Contains(ConstDefines.UMPLicenseManager) || App.dicAppInstalled.Keys.Contains(ConstDefines.UMPLicenseServer) ||
                App.dicAppInstalled.Keys.Contains(ConstDefines.UMPScreenServer) || App.dicAppInstalled.Keys.Contains(ConstDefines.UMPSFTP) ||
                App.dicAppInstalled.Keys.Contains(ConstDefines.UMPSoftRecord) || App.dicAppInstalled.Keys.Contains(ConstDefines.UMPSpeechAnalysis) ||
                App.dicAppInstalled.Keys.Contains(ConstDefines.UMPVoice) || App.dicAppInstalled.Keys.Contains(ConstDefines.VCLogWebSDK) ||
                App.dicAppInstalled.Keys.Contains(ConstDefines.IxPatch) || App.dicAppInstalled.Keys.Contains(ConstDefines.ASM))
            {
                App.bIsLoggingServer = true;
            }

        }

        /// <summary>
        /// 获得已安装的UMP安装包及其信息
        /// </summary>
        private void GetAppsInstalled()
        {
            App.dicAppInstalled.Clear();
            Dictionary<string, string> dicAppGUIDs = new Dictionary<string, string>();
            dicAppGUIDs.Add(ConstDefines.UMPAlarmServer, ConstDefines.UMPAlarmServerGUID);
            dicAppGUIDs.Add(ConstDefines.UMPAlarmClient, ConstDefines.UMPAlarmClientGUID);
            dicAppGUIDs.Add(ConstDefines.UMPLicenseManager, ConstDefines.UMPLicenseManagerGUID);
            dicAppGUIDs.Add(ConstDefines.UMPAgentClient, ConstDefines.UMPAgentClientGUID);
            dicAppGUIDs.Add(ConstDefines.UMPCMServer, ConstDefines.UMPCMServerGUID);
            dicAppGUIDs.Add(ConstDefines.UMPCQC, ConstDefines.UMPCQCGUID);
            dicAppGUIDs.Add(ConstDefines.UMPCTIHub, ConstDefines.UMPCTIHubGUID);
            dicAppGUIDs.Add(ConstDefines.UMPDEC, ConstDefines.UMPDECGUID);
            dicAppGUIDs.Add(ConstDefines.UMPLicenseServer, ConstDefines.UMPLicenseServerGUID);
            dicAppGUIDs.Add(ConstDefines.UMPScreenServer, ConstDefines.UMPScreenServerGUID);
            dicAppGUIDs.Add(ConstDefines.UMPSoftRecord, ConstDefines.UMPSoftRecordGUID);
            dicAppGUIDs.Add(ConstDefines.UMPSpeechAnalysis, ConstDefines.UMPSpeechAnalysisGUID);
            dicAppGUIDs.Add(ConstDefines.IxPatch, ConstDefines.IxPatchGUID);
            //dicAppGUIDs.Add(Defines.VCLogWebSDK, Defines.VCLogWebSDKGUID);
            dicAppGUIDs.Add(ConstDefines.UMPVoice, ConstDefines.UMPVoiceGUID);
            dicAppGUIDs.Add(ConstDefines.ASM, ConstDefines.ASMGUID);
            dicAppGUIDs.Add(ConstDefines.UMP, ConstDefines.UMPGUID);
            dicAppGUIDs.Add(ConstDefines.UMPSFTP, ConstDefines.UMPSFTPGUID);

            OperationReturn optReturn = null;
            UMPAppInfo info = null;
            foreach (KeyValuePair<string, string> item in dicAppGUIDs)
            {
                optReturn = RegistryOperator.GetAppInfoByGUID(item.Value);
                if (!optReturn.Result)
                {
                    App.WriteLog("App " + item.Key + " not installed");
                    continue;
                }
                info = optReturn.Data as UMPAppInfo;
                App.dicAppInstalled.Add(info.AppName, info);
            }
        }

        /// <summary>
        /// 获得数据库信息
        /// </summary>
        /// <returns></returns>
        private OperationReturn GetDatabaseInfo()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = false;
            optReturn.Code = Defines.RET_FAIL;

            if (App.bIsUMPServer)
            {
                //UMP服务器 从programdata下读数据库参数
                string strDBFileath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                DatabaseInfo dbInfo = null;
                optReturn = DatabaseXmlOperator.GetDBInfo(strDBFileath, ref dbInfo);
            }
            else if (App.bIsLoggingServer)
            {
                //voice服务器 从programdata\VoiceServer下读数据库参数
                string strDBFileath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\VoiceServer";
                DatabaseInfo dbInfo = null;
                optReturn = DatabaseXmlOperator.GetDBInfo(strDBFileath, ref dbInfo);
            }
            return optReturn;
        }

        #endregion

        #region 被调用的函数
        private void ChangeLanguage(string strLangID)
        {
            try
            {
                string languagefileName = string.Format(@"Languages/{0}.xaml", strLangID);
                if (Application.Current.Resources.MergedDictionaries.Count > 0)
                {
                    Application.Current.Resources.MergedDictionaries[0] = new ResourceDictionary()
                   {
                       Source = new Uri(languagefileName, UriKind.RelativeOrAbsolute)
                   };
                }
            }
            catch { }

        }


        #endregion


    }
}
