using System;
using System.Collections.Generic;
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
using UMP.PF.MAMT.Classes;
using UMP.PF.MAMT.WCF_ServerConfig;
using System.ComponentModel;
using System.Net;
using PFShareClassesC;

namespace UMP.PF.MAMT
{
    public partial class ConfigurationTypeSelect : Window
    {
        BackgroundWorker LoginWorker = null;
        OperationDataArgs LoginResult = null;
        List<DBInfo> lstDBsInUmp = null;        //连接的UMP服务器上配置的数据库信息

        public ConfigurationTypeSelect()
        {
            InitializeComponent();
            //加载界面元素的事件
            this.Loaded += ConfigurationTypeSelect_Loaded;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUser.Text))
            {
                MessageBox.Show(this.TryFindResource("Error009").ToString(),
                    this.TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrEmpty(txtPwd.Password))
            {
                MessageBox.Show(this.TryFindResource("Error010").ToString(),
                    this.TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrEmpty(txtPort.Text))
            {
                MessageBox.Show(this.TryFindResource("Error011").ToString(),
                   this.TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrEmpty(cmbServers.Text))
            {
                MessageBox.Show(this.TryFindResource("Error012").ToString(),
                  this.TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ServerInfomation serverInfo = new ServerInfomation();
            serverInfo.Host = cmbServers.Text;
            serverInfo.Port = txtPort.Text;
            serverInfo.UserName = txtUser.Text;
            
            App.GCurrentUmpServer = serverInfo;

            LoginWorker = new BackgroundWorker();
            LoginWorker.DoWork += LoginWorker_DoWork;
            LoginWorker.RunWorkerCompleted += LoginWorker_RunWorkerCompleted;
            List<string> lstArgs = new List<string>();
            bool? isLoginMethodChecked = chkLoginMethod.IsChecked;
            string strLoginMethod = isLoginMethodChecked == true ? "F" : "N";
            lstArgs.Add(strLoginMethod);
            lstArgs.Add(cmbServers.Text);
            lstArgs.Add(txtPort.Text);
            lstArgs.Add(txtUser.Text);
            lstArgs.Add(txtPwd.Password);
            LoginWorker.RunWorkerAsync(lstArgs);
        }

        void LoginWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<string> lstArgs = e.Argument as List<string>;
            //获得该UMP服务器上配置的数据库信息
            lstDBsInUmp = ServerConfigOperationInServer.GetAllDBs(lstArgs[1], lstArgs[2]);

            string strHostName = System.Net.Dns.GetHostName();
            LoginResult = ServerConfigOperationInServer.UserLogin(lstArgs[1], lstArgs[2], lstArgs[3], lstArgs[4], lstArgs[0], strHostName);
        }

        void LoginWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DBInfo db = null;
            if (lstDBsInUmp.Count > 0)
            {
                db = lstDBsInUmp[0];
            }
            else
            {
                //如果未找到数据库配置 则返回
                MessageBox.Show(this.TryFindResource("Error013").ToString(),
                    this.TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string strMessage = string.Empty;
            if (LoginResult != null)
            {
                if (!LoginResult.BoolReturn)
                {
                    strMessage = ServerConfigOperationInServer.GetLanguageItemInDBByMessageID(App.GCurrentUmpServer, db, "S0000028", App.GStrCurrentLanguage);
                    MessageBox.Show(strMessage, this.TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                List<string> lstLoginReturn = LoginResult.StringReturn.Split(App.GStrSpliterCharater.ToCharArray()).ToList();
                //将收到的消息解密
                string LStrVerificationCode104 = Common.CreateVerificationCode(PFShareClassesC.EncryptionAndDecryption.UMPKeyAndIVType.M104);
                for (int i = 0; i < lstLoginReturn.Count; i++)
                {
                    lstLoginReturn[i] = EncryptionAndDecryption.EncryptDecryptString(lstLoginReturn[i], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                }
                if (lstLoginReturn[0].StartsWith("S"))
                {
                    App.GLstConnectedServers.Add(App.GCurrentUmpServer);
                    App.GCurrentUser.TenantID = lstLoginReturn[1];
                    App.GCurrentUser.UserID = lstLoginReturn[2];
                    App.GCurrentUser.SessionID = lstLoginReturn[3];
                    App.GCurrentUser.UserName = lstLoginReturn[4];

                    MainWin main = new MainWin();
                    main.Show();
                    this.Close();
                    return;
                }

                switch (lstLoginReturn[0])
                {
                    //case "S01A00":
                    //   // UserLoginReturnS01A01(LStrLoginReturn);
                        //break;
                    case "E01A32":
                        strMessage = ServerConfigOperationInServer.GetLanguageItemInDBByObjectIDAndPage(App.GCurrentUmpServer, db, lstLoginReturn[0], App.GStrCurrentLanguage, "Page00000A");
                        strMessage=string.Format(strMessage,DateTime.Parse( lstLoginReturn[1]).ToLocalTime().ToString("G"), lstLoginReturn[2], lstLoginReturn[3]);
                        MessageBox.Show(strMessage, this.TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    case "E01A99":
                        strMessage = ServerConfigOperationInServer.GetLanguageItemInDBByMessageID(App.GCurrentUmpServer, db, "S0000028", App.GStrCurrentLanguage);
                        MessageBox.Show(strMessage, this.TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    default:
                        strMessage = ServerConfigOperationInServer.GetLanguageItemInDBByMessageID(App.GCurrentUmpServer, db, "S0000029", App.GStrCurrentLanguage);
                        strMessage += "\n\n";
                        strMessage += ServerConfigOperationInServer.GetLanguageItemInDBByObjectIDAndPage(App.GCurrentUmpServer, db, lstLoginReturn[0], App.GStrCurrentLanguage, "Page00000A");
                        MessageBox.Show(strMessage, this.TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;
                }

                //if (lstLoginReturn[0].StartsWith("S"))
                //{

                //    MainWin main = new MainWin();
                //    main.Show();
                //    this.Close();
                //}
                //else
                //{
                //    MessageBox.Show(strMessage, this.TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                //}
            }
        }

        /// <summary>
        /// 打开下拉列表事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ButtonApplicationMenu_Click(object sender, RoutedEventArgs e)
        {
            Button ClickedButton = sender as Button;
            //目标   
            ClickedButton.ContextMenu.PlacementTarget = ClickedButton;
            //位置   
            ClickedButton.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            //显示菜单   
            ClickedButton.ContextMenu.IsOpen = true;
        }

        /// <summary>
        /// 关闭应用程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ButtonCloseConnect_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// page_load事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfigurationTypeSelect_Loaded(object sender, RoutedEventArgs e)
        {
            ButtonCloseConnect.Click += ButtonCloseConnect_Click;
            this.ButtonApplicationMenu.Click += ButtonApplicationMenu_Click;
            btnLogin.Click += btnLogin_Click;
            btnClose.Click += ButtonCloseConnect_Click;
            cmbServers.SelectionChanged += cmbServers_SelectionChanged;
            App.DrawWindowsBackGround(this, @"Images\00000000.png");
            ImageWindowLogo.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000001.ico"), UriKind.RelativeOrAbsolute));
            this.Icon = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000001.ico"), UriKind.RelativeOrAbsolute));
            InitMenu();
            InitWindowControl();
            txtPwd.Focus();
        }

        /// <summary>
        /// 加载语言菜单
        /// </summary>
        private void InitMenu()
        {
            ContextMenu LocalContextMenu = Common.CreateApplicationMenu(false);
            ButtonApplicationMenu.ContextMenu = LocalContextMenu;
        }

        /// <summary>
        /// 初始化窗口上的组件
        /// </summary>
        private void InitWindowControl()
        {
            cmbServers.ItemsSource = App.GlstServers;
            cmbServers.DisplayMemberPath = "Host";
            cmbServers.SelectedValuePath = "Host";
            if (App.GlstServers.Count > 0)
            {
                cmbServers.SelectedIndex = 0;
                txtPort.Text = App.GlstServers[0].Port;
                txtUser.Text = App.GlstServers[0].UserName;
            }
        }

        void cmbServers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbServers.SelectedValue != null)
            {
                string strHost = cmbServers.SelectedValue.ToString();
                List<ServerInfomation> servers = App.GlstServers.Where(p => p.Host == strHost).ToList();
                if (servers.Count > 0)
                {
                    txtPort.Text = servers[0].Port;
                    txtUser.Text = servers[0].UserName;
                }
            }
            else
            {
                txtPort.Text = "8081";
                txtUser.Text = "";
            }
        }

    }
}
