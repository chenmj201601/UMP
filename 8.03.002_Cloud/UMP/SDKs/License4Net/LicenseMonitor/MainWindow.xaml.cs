using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using VoiceCyber.Common;
using VoiceCyber.SDKs.Licenses;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Encryptions;
using License = VoiceCyber.SDKs.Licenses.License;

namespace LicenseMonitor
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : IEncryptable
    {

        #region Members

        public const string LIC_ID_UMP_01 = "1100101";

        private ObservableCollection<ClientViewItem> mListClients;
        private ObservableCollection<LicensePoolViewItem> mListLicensePools;
        private ObservableCollection<SoftdogViewItem> mListSoftdogs;
        private ObservableCollection<LicenseServerViewItem> mListLicenseServers;
        private ObservableCollection<PropertyViewItem> mListProperties;

        private LicConnector mLicConnector;
        private bool mIsConnected;
        private string mPropetySource;
        private bool mIsLogDetail;

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;

            BtnAbout.Click += BtnAbout_OnClick;
            BtnClose.Click += BtnClose_OnClick;
            BtnConnect.Click += BtnConnect_OnClick;
            BtnDisconnect.Click += BtnConnect_OnClick;
            CbServer.Click += CheckBoxView_OnClick;
            CbProperty.Click += CheckBoxView_OnClick;
            CbMessage.Click += CheckBoxView_OnClick;
            CbStatus.Click += CheckBoxView_OnClick;
            CbLogDebug.Click += CbLogDebug_Click;

            LvClient.SelectionChanged += LvClient_OnSelectionChanged;
            LvLicensePool.SelectionChanged += LvLicensePool_OnSelectionChanged;
            LvLicenseServer.SelectionChanged += LvLicenseServer_OnSelectionChanged;
            LvSoftDog.SelectionChanged += LvSoftDog_OnSelectionChanged;
            LvProperty.MouseDoubleClick += LvProperty_MouseDoubleClick;

            LvClient.MouseUp += LvClient_OnMouseUp;
            LvLicensePool.MouseUp += LvLicensePool_OnMouseUp;
            LvLicenseServer.MouseUp += LvLicenseServer_OnMouseUp;
            LvSoftDog.MouseUp += LvSoftDog_OnMouseUp;

            mListClients = new ObservableCollection<ClientViewItem>();
            mListLicensePools = new ObservableCollection<LicensePoolViewItem>();
            mListSoftdogs = new ObservableCollection<SoftdogViewItem>();
            mListLicenseServers = new ObservableCollection<LicenseServerViewItem>();
            mListProperties = new ObservableCollection<PropertyViewItem>();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LvClient.ItemsSource = mListClients;
            LvLicensePool.ItemsSource = mListLicensePools;
            LvSoftDog.ItemsSource = mListSoftdogs;
            LvLicenseServer.ItemsSource = mListLicenseServers;
            LvProperty.ItemsSource = mListProperties;

            Init();
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (mLicConnector != null)
            {
                mLicConnector.Close();
                mLicConnector = null;
            }
        }


        #region Init and Load


        private void Init()
        {
            try
            {
                TxtName.Text = "Charley";
                TxtServer.Text = "192.168.4.182";
                TxtPort.Text = "3070";
                mIsConnected = false;
                mPropetySource = "C";
                mIsLogDetail = false;

                BtnConnect.IsEnabled = true;
                BtnDisconnect.IsEnabled = false;

                CbServer.IsChecked = true;
                CbProperty.IsChecked = true;
                CbMessage.IsChecked = true;

                CheckBoxView_OnClick(null, null);

                InitListView();

                #region ViewGroup

                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(LvClient.ItemsSource);
                if (view != null)
                {
                    if (view.GroupDescriptions != null && view.GroupDescriptions.Count == 0)
                    {
                        view.GroupDescriptions.Add(new PropertyGroupDescription("Type"));
                    }
                    else
                    {
                        if (view.GroupDescriptions != null) view.GroupDescriptions.Clear();
                    }
                }
                view = (CollectionView)CollectionViewSource.GetDefaultView(LvProperty.ItemsSource);
                if (view != null)
                {
                    if (view.GroupDescriptions != null && view.GroupDescriptions.Count == 0)
                    {
                        view.GroupDescriptions.Add(new PropertyGroupDescription("Catetory"));
                    }
                    else
                    {
                        if (view.GroupDescriptions != null) view.GroupDescriptions.Clear();
                    }
                }

                #endregion


                CbLogDebug.IsChecked = mIsLogDetail;

            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void InitListView()
        {
            //Client
            GridView gv = new GridView();

            GridViewColumn gvc = new GridViewColumn();
            GridViewColumnHeader gvch = new GridViewColumnHeader();
            gvch.Content = "Host";
            gvc.Header = gvch;
            gvc.Width = 150;
            gvc.DisplayMemberBinding = new Binding("Endpoint");
            gv.Columns.Add(gvc);

            gvc = new GridViewColumn();
            gvch = new GridViewColumnHeader();
            gvch.Content = "ModuleNumber";
            gvc.Header = gvch;
            gvc.Width = 150;
            gvc.DisplayMemberBinding = new Binding("ModuleNumber");
            gv.Columns.Add(gvc);

            gvc = new GridViewColumn();
            gvch = new GridViewColumnHeader();
            gvch.Content = "Name";
            gvc.Header = gvch;
            gvc.Width = 150;
            gvc.DisplayMemberBinding = new Binding("Name");
            gv.Columns.Add(gvc);

            gvc = new GridViewColumn();
            gvch = new GridViewColumnHeader();
            gvch.Content = "Connecting Time";
            gvc.Header = gvch;
            gvc.Width = 150;
            gvc.DisplayMemberBinding = new Binding("ConnTime");
            gv.Columns.Add(gvc);

            gvc = new GridViewColumn();
            gvch = new GridViewColumnHeader();
            gvch.Content = "Protocol";
            gvc.Header = gvch;
            gvc.Width = 150;
            gvc.DisplayMemberBinding = new Binding("Protocol");
            gv.Columns.Add(gvc);

            gvc = new GridViewColumn();
            gvch = new GridViewColumnHeader();
            gvch.Content = "Expiration";
            gvc.Header = gvch;
            gvc.Width = 150;
            gvc.DisplayMemberBinding = new Binding("Expiration");
            gv.Columns.Add(gvc);

            LvClient.View = gv;

            //License Pool
            gv = new GridView();

            gvc = new GridViewColumn();
            gvch = new GridViewColumnHeader();
            gvch.Content = "Name";
            gvc.Header = gvch;
            gvc.Width = 150;
            gvc.DisplayMemberBinding = new Binding("Name");
            gv.Columns.Add(gvc);

            gvc = new GridViewColumn();
            gvch = new GridViewColumnHeader();
            gvch.Content = "Expiration";
            gvc.Header = gvch;
            gvc.Width = 150;
            gvc.DisplayMemberBinding = new Binding("Expiration");
            gv.Columns.Add(gvc);

            LvLicensePool.View = gv;

            //Softdog
            gv = new GridView();

            gvc = new GridViewColumn();
            gvch = new GridViewColumnHeader();
            gvch.Content = "";
            gvc.Header = gvch;
            gvc.Width = 30;
            gvc.DisplayMemberBinding = new Binding("CurTag");
            gv.Columns.Add(gvc);

            gvc = new GridViewColumn();
            gvch = new GridViewColumnHeader();
            gvch.Content = "SerialNumber";
            gvc.Header = gvch;
            gvc.Width = 150;
            gvc.DisplayMemberBinding = new Binding("SerialNumber");
            gv.Columns.Add(gvc);

            gvc = new GridViewColumn();
            gvch = new GridViewColumnHeader();
            gvch.Content = "Type";
            gvc.Header = gvch;
            gvc.Width = 150;
            gvc.DisplayMemberBinding = new Binding("Type");
            gv.Columns.Add(gvc);

            gvc = new GridViewColumn();
            gvch = new GridViewColumnHeader();
            gvch.Content = "Master";
            gvc.Header = gvch;
            gvc.Width = 150;
            gvc.DisplayMemberBinding = new Binding("Master");
            gv.Columns.Add(gvc);

            gvc = new GridViewColumn();
            gvch = new GridViewColumnHeader();
            gvch.Content = "Periods";
            gvc.Header = gvch;
            gvc.Width = 150;
            gvc.DisplayMemberBinding = new Binding("Periods");
            gv.Columns.Add(gvc);

            gvc = new GridViewColumn();
            gvch = new GridViewColumnHeader();
            gvch.Content = "Expiration";
            gvc.Header = gvch;
            gvc.Width = 150;
            gvc.DisplayMemberBinding = new Binding("Expiration");
            gv.Columns.Add(gvc);

            LvSoftDog.View = gv;

            //License Server
            gv = new GridView();

            gvc = new GridViewColumn();
            gvch = new GridViewColumnHeader();
            gvch.Content = "Host";
            gvc.Header = gvch;
            gvc.Width = 150;
            gvc.DisplayMemberBinding = new Binding("Host");
            gv.Columns.Add(gvc);

            gvc = new GridViewColumn();
            gvch = new GridViewColumnHeader();
            gvch.Content = "Port";
            gvc.Header = gvch;
            gvc.Width = 150;
            gvc.DisplayMemberBinding = new Binding("Port");
            gv.Columns.Add(gvc);

            gvc = new GridViewColumn();
            gvch = new GridViewColumnHeader();
            gvch.Content = "ModuleNumber";
            gvc.Header = gvch;
            gvc.Width = 150;
            gvc.DisplayMemberBinding = new Binding("ModuleNumber");
            gv.Columns.Add(gvc);

            LvLicenseServer.View = gv;

            //Preoperty
            gv = new GridView();

            gvc = new GridViewColumn();
            gvch = new GridViewColumnHeader();
            gvch.Content = "Name";
            gvc.Header = gvch;
            gvc.Width = 150;
            gvc.DisplayMemberBinding = new Binding("Name");
            gv.Columns.Add(gvc);

            gvc = new GridViewColumn();
            gvch = new GridViewColumnHeader();
            gvch.Content = "SerialNo";
            gvc.Header = gvch;
            gvc.Width = 150;
            gvc.DisplayMemberBinding = new Binding("SerialNo");
            gv.Columns.Add(gvc);

            gvc = new GridViewColumn();
            gvch = new GridViewColumnHeader();
            gvch.Content = "Type";
            gvc.Header = gvch;
            gvc.Width = 80;
            gvc.DisplayMemberBinding = new Binding("LicType");
            gv.Columns.Add(gvc);

            gvc = new GridViewColumn();
            gvch = new GridViewColumnHeader();
            gvch.Content = "Value";
            gvc.Header = gvch;
            gvc.Width = 150;
            //gvc.DisplayMemberBinding = new Binding("Value");
            var dt = Resources["CellLicValueTemplate"] as DataTemplate;
            if (dt != null)
            {
                gvc.CellTemplate = dt;
            }
            else
            {
                gvc.DisplayMemberBinding = new Binding("Value");
            }
            gv.Columns.Add(gvc);

            gvc = new GridViewColumn();
            gvch = new GridViewColumnHeader();
            gvch.Content = "Expiration";
            gvc.Header = gvch;
            gvc.Width = 250;
            gvc.DisplayMemberBinding = new Binding("Expiration");
            gv.Columns.Add(gvc);

            gvc = new GridViewColumn();
            gvch = new GridViewColumnHeader();
            gvch.Content = "MajorID";
            gvc.Header = gvch;
            gvc.Width = 80;
            gvc.DisplayMemberBinding = new Binding("MajorID");
            gv.Columns.Add(gvc);

            gvc = new GridViewColumn();
            gvch = new GridViewColumnHeader();
            gvch.Content = "MinorID";
            gvc.Header = gvch;
            gvc.Width = 80;
            gvc.DisplayMemberBinding = new Binding("MinorID");
            gv.Columns.Add(gvc);

            LvProperty.View = gv;
        }

        private void CreateLicensePoolItems()
        {
            mListLicensePools.Clear();
            LicensePoolViewItem lpViewItem = new LicensePoolViewItem();
            lpViewItem.Name = "Free";
            mListLicensePools.Add(lpViewItem);
            lpViewItem = new LicensePoolViewItem();
            lpViewItem.Name = "Total";
            mListLicensePools.Add(lpViewItem);
        }

        #endregion


        #region EventHandlers

        private void BtnAbout_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void BtnClose_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnConnect_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!CheckInput()) { return; }
                if (mIsConnected)
                {
                    DisConnect();
                    BtnConnect.IsEnabled = true;
                    BtnDisconnect.IsEnabled = false;
                    mIsConnected = false;
                    CbServer.IsChecked = true;
                    CheckBoxView_OnClick(null, null);
                }
                else
                {
                    CreateLicensePoolItems();
                    if (mLicConnector != null)
                    {
                        mLicConnector.Close();
                        mLicConnector = null;
                    }
                    mLicConnector = new LicConnector();
                    mLicConnector.Debug += LicConnector_Debug;
                    mLicConnector.ServerConnectionEvent += LicConnector_ServerConnectionEvent;
                    mLicConnector.MessageReceivedEvent += LicConnector_MessageReceivedEvent;
                    mLicConnector.EncryptObject = this;
                    mLicConnector.Client = TxtName.Text;
                    mLicConnector.ModuleTypeID = 7692;    //作为监控端
                    mLicConnector.Host = TxtServer.Text;
                    mLicConnector.Port = int.Parse(TxtPort.Text);
                    mLicConnector.Connect();
                    //mLicConnector.BeginConnect();

                    BtnConnect.IsEnabled = false;
                    BtnDisconnect.IsEnabled = true;
                    mIsConnected = true;
                    CbServer.IsChecked = false;
                    CheckBoxView_OnClick(null, null);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void CheckBoxView_OnClick(object sender, RoutedEventArgs e)
        {
            GridServer.Visibility = CbServer.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            RdMessage.Height = CbMessage.IsChecked == true ? new GridLength(30) : new GridLength(0);
            if (CbProperty.IsChecked == true)
            {
                if (CdProperty.Width.Value <= 0)
                {
                    CdProperty.Width = new GridLength(400);
                }
            }
            else
            {
                CdProperty.Width = new GridLength(0);
            }
            if (CbStatus.IsChecked == true)
            {
                if (RdStatus.Height.Value <= 0)
                {
                    RdStatus.Height = new GridLength(80);
                }
            }
            else
            {
                RdStatus.Height = new GridLength(0);
            }
        }

        void CbLogDebug_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                mIsLogDetail = CbLogDebug.IsChecked == true;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void LvClient_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            mPropetySource = "C";
            BindPropertyView();
        }

        private void LvLicensePool_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            mPropetySource = "LP";
            BindPropertyView();
        }

        private void LvSoftDog_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            mPropetySource = "S";
            BindPropertyView();
        }

        private void LvLicenseServer_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            mPropetySource = "LS";
            BindPropertyView();
        }

        private void LvClient_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mPropetySource = "C";
            BindPropertyView();
        }

        private void LvLicensePool_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mPropetySource = "LP";
            BindPropertyView();
        }

        private void LvSoftDog_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mPropetySource = "S";
            BindPropertyView();
        }

        private void LvLicenseServer_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mPropetySource = "LS";
            BindPropertyView();
        }

        void LvProperty_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenLiceDetail();
        }

        #endregion


        #region ProcessMessage

        void LicConnector_Debug(LogMode mode, string client, string msg)
        {
            try
            {
                if (!mIsLogDetail && (int)mode <= 1) { return; }
                WriteLog(string.Format("{0}\t{1}", client, msg));
            }
            catch { }
        }

        void LicConnector_ServerConnectionEvent(int code, string client, string msg)
        {
            try
            {
                switch (code)
                {
                    case Defines.EVT_NET_CONNECTED:
                        WriteLog(string.Format("{0}\tServer connected.\t{1}", client, msg));
                        break;
                    case Defines.EVT_NET_DISCONNECTED:
                        WriteLog(string.Format("{0}\tServer disconnected.\t{1}", client, msg));
                        break;
                    case Defines.EVT_NET_AUTHED:
                        WriteLog(string.Format("{0}\tAuthenticate.\t{1}", client, msg));
                        break;
                    default:
                        WriteLog(string.Format("{0}\tConnectionEvent\t{1}\t{2}", client, code, msg));
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        void LicConnector_MessageReceivedEvent(object sender, MessageReceivedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    string strInfo = e.StringData;
                    JsonObject json = new JsonObject(strInfo);
                    if (mIsLogDetail)
                    {
                        //WriteLog(json.ToString("F"));
                        WriteLog(json.ToString());
                    }
                    int classid = (int)json[LicDefines.KEYWORD_MSG_COMMON_CLASSID].Number;
                    int messageid = (int)json[LicDefines.KEYWORD_MSG_COMMON_MESSAGEID].Number;
                    WriteLog(string.Format("Recv\tClass: {0};\tMsg: {1}", LicUtils.GetClassDesc(classid), LicUtils.GetMessageDesc(classid, messageid)));
                    if (classid == LicDefines.LICENSE_MSG_CLASS_NOTIFY)
                    {
                        //通知消息
                        switch (messageid)
                        {
                            case LicDefines.LICENSE_MSG_NOTIFY_NEW_CLIENT:      //新的客户端连接
                                ProcessMessageNewClient(json);
                                break;
                            case LicDefines.LICENSE_MSG_NOTIFY_DEL_CLIENT:      //客户端断开连接
                                ProcessMessageDelClient(json);
                                break;
                            case LicDefines.LICENSE_MSG_NOTIFY_APPLICATION_LICENSE:     //当前Application的License信息
                                ProcessMessageAppLicense(json);
                                break;
                            case LicDefines.LICENSE_MSG_NOTIFY_LICESNE_POOL:            //当前LicensePool信息
                                ProcessMessageLicensePool(json);
                                break;
                            case LicDefines.LICENSE_MSG_NOTIFY_LICENSE_SERVERS: //LicenseServer信息
                                ProcessMessageLicenseServer(json);
                                break;
                            case LicDefines.LICENSE_MSG_NOTIFY_SOFTDOGS_INFO:   //软件狗信息
                                ProcessMessageSoftdog(json);
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowErrorMessage(ex.Message);
                }
            }));
        }

        private void ProcessMessageNewClient(JsonObject json)
        {
            try
            {
                var appInfo = json[LicDefines.KEYWORD_MSG_COMMON_DATA][LicDefines.KEYWORD_MSG_APPINFO];
                if (appInfo != null)
                {
                    //更新ApplicationInfo信息
                    string strSession = appInfo[LicDefines.KEYWORD_MSG_APPINFO_SESSION].Value;
                    var temp = mListClients.FirstOrDefault(c => c.Session == strSession);
                    if (temp != null)
                    {
                        mListClients.Remove(temp);
                    }
                    temp = new ClientViewItem();
                    temp.Session = strSession;
                    temp.Name = appInfo[LicDefines.KEYWORD_MSG_APPINFO_MODULENAME].Value;
                    var moduleNumber = (int)appInfo[LicDefines.KEYWORD_MSG_APPINFO_MODULENUMBER].Number;
                    if (moduleNumber < 0)
                    {
                        temp.ModuleNumber = "-";
                    }
                    else
                    {
                        temp.ModuleNumber = moduleNumber.ToString();
                    }
                    temp.Host = appInfo[LicDefines.KEYWORD_MSG_APPINFO_HOST].Value;
                    temp.ConnTime = appInfo[LicDefines.KEYWORD_MSG_APPINFO_CONNECTTIME].Value;
                    temp.Protocol = appInfo[LicDefines.KEYWORD_MSG_AUTH_PROTOCOL].Value;
                    temp.Port = (int)appInfo[LicDefines.KEYWORD_MSG_APPINFO_PORT].Number;
                    temp.Endpoint = string.Format("{0}:{1}", temp.Host, temp.Port);
                    if (appInfo[LicDefines.KEYWORD_MSG_APPINFO_CLIENTLICPOOL] != null)
                    {
                        temp.Data =
                            appInfo[LicDefines.KEYWORD_MSG_APPINFO_CLIENTLICPOOL][
                                LicDefines.KEYWORD_MSG_APPINFO_LICENSES];
                    }
                    else
                    {
                        temp.Data = null;
                    }
                    mListClients.Add(temp);
                    temp.OnPropertyChanged();
                }
                var licPool = json[LicDefines.KEYWORD_MSG_COMMON_DATA][LicDefines.KEYWORD_MSG_SERVER_LICPOOL];
                if (licPool != null)
                {
                    //更新LicensePool信息
                    var free = licPool[LicDefines.KEYWORD_MSG_SERVER_LICPOOL_FREE];
                    if (free != null)
                    {
                        var freeItem = mListLicensePools.FirstOrDefault(p => p.Name == "Free");
                        if (freeItem != null)
                        {
                            freeItem.Data = free;
                        }
                    }
                    var total = licPool[LicDefines.KEYWORD_MSG_SERVER_LICPOOL_TOTAL];
                    if (total != null)
                    {
                        var totalItem = mListLicensePools.FirstOrDefault(p => p.Name == "Total");
                        if (totalItem != null)
                        {
                            totalItem.Data = total;
                        }
                    }
                }

                BindPropertyView();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void ProcessMessageDelClient(JsonObject json)
        {
            try
            {
                var appInfo = json[LicDefines.KEYWORD_MSG_COMMON_DATA][LicDefines.KEYWORD_MSG_APPINFO];
                if (appInfo != null)
                {
                    string strSession = appInfo[LicDefines.KEYWORD_MSG_APPINFO_SESSION].Value;
                    var temp = mListClients.FirstOrDefault(c => c.Session == strSession);
                    if (temp != null)
                    {
                        mListClients.Remove(temp);
                    }
                }
                var licPool = json[LicDefines.KEYWORD_MSG_COMMON_DATA][LicDefines.KEYWORD_MSG_SERVER_LICPOOL];
                if (licPool != null)
                {
                    //更新LicensePool信息
                    var free = licPool[LicDefines.KEYWORD_MSG_SERVER_LICPOOL_FREE];
                    if (free != null)
                    {
                        var freeItem = mListLicensePools.FirstOrDefault(p => p.Name == "Free");
                        if (freeItem != null)
                        {
                            freeItem.Data = free;
                        }
                    }
                    var total = licPool[LicDefines.KEYWORD_MSG_SERVER_LICPOOL_TOTAL];
                    if (total != null)
                    {
                        var totalItem = mListLicensePools.FirstOrDefault(p => p.Name == "Total");
                        if (totalItem != null)
                        {
                            totalItem.Data = total;
                        }
                    }
                }

                BindPropertyView();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void ProcessMessageAppLicense(JsonObject json)
        {
            try
            {
                var appInfo = json[LicDefines.KEYWORD_MSG_COMMON_DATA][LicDefines.KEYWORD_MSG_APPINFO];
                if (appInfo != null)
                {
                    //更新ApplicationInfo信息
                    string strSession = appInfo[LicDefines.KEYWORD_MSG_APPINFO_SESSION].Value;
                    var temp = mListClients.FirstOrDefault(c => c.Session == strSession);
                    if (temp == null) { return; }
                    if (appInfo[LicDefines.KEYWORD_MSG_APPINFO_CLIENTLICPOOL] != null)
                    {
                        temp.Data =
                            appInfo[LicDefines.KEYWORD_MSG_APPINFO_CLIENTLICPOOL][
                                LicDefines.KEYWORD_MSG_APPINFO_LICENSES];
                    }
                    temp.OnPropertyChanged();
                }
                var licPool = json[LicDefines.KEYWORD_MSG_COMMON_DATA][LicDefines.KEYWORD_MSG_SERVER_LICPOOL];
                if (licPool != null)
                {
                    //更新LicensePool信息
                    var free = licPool[LicDefines.KEYWORD_MSG_SERVER_LICPOOL_FREE];
                    if (free != null)
                    {
                        var freeItem = mListLicensePools.FirstOrDefault(p => p.Name == "Free");
                        if (freeItem != null)
                        {
                            freeItem.Data = free;
                        }
                    }
                    var total = licPool[LicDefines.KEYWORD_MSG_SERVER_LICPOOL_TOTAL];
                    if (total != null)
                    {
                        var totalItem = mListLicensePools.FirstOrDefault(p => p.Name == "Total");
                        if (totalItem != null)
                        {
                            totalItem.Data = total;
                        }
                    }
                }

                BindPropertyView();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void ProcessMessageLicensePool(JsonObject json)
        {
            try
            {
                var licPool = json[LicDefines.KEYWORD_MSG_COMMON_DATA][LicDefines.KEYWORD_MSG_SERVER_LICPOOL];
                if (licPool != null)
                {
                    //更新LicensePool信息
                    var free = licPool[LicDefines.KEYWORD_MSG_SERVER_LICPOOL_FREE];
                    if (free != null)
                    {
                        var freeItem = mListLicensePools.FirstOrDefault(p => p.Name == "Free");
                        if (freeItem != null)
                        {
                            freeItem.Data = free;
                        }
                    }
                    var total = licPool[LicDefines.KEYWORD_MSG_SERVER_LICPOOL_TOTAL];
                    if (total != null)
                    {
                        var totalItem = mListLicensePools.FirstOrDefault(p => p.Name == "Total");
                        if (totalItem != null)
                        {
                            totalItem.Data = total;
                        }
                    }
                }

                BindPropertyView();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void ProcessMessageLicenseServer(JsonObject json)
        {
            try
            {
                var licServers = json[LicDefines.KEYWORD_MSG_COMMON_DATA][LicDefines.KEYWORD_MSG_LICENSESERVERS];
                if (licServers != null)
                {
                    for (int i = 0; i < licServers.Count; i++)
                    {
                        var licServer = licServers[i];
                        string strHost = licServer[LicDefines.KEYWORD_MSG_APPINFO_HOST].Value;
                        int intPort = (int)licServer[LicDefines.KEYWORD_MSG_APPINFO_PORT].Number;
                        int moduleNumber = (int)licServer[LicDefines.KEYWORD_MSG_APPINFO_MODULENUMBER].Number;
                        var temp = mListLicenseServers.FirstOrDefault(s => s.Host == strHost && s.Port == intPort);
                        if (temp != null)
                        {
                            mListLicenseServers.Remove(temp);
                        }
                        temp = new LicenseServerViewItem();
                        temp.Host = strHost;
                        temp.Port = intPort;
                        temp.ModuleNumber = moduleNumber;
                        mListLicenseServers.Add(temp);
                    }
                }

                BindPropertyView();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void ProcessMessageSoftdog(JsonObject json)
        {
            try
            {
                var dogs = json[LicDefines.KEYWORD_MSG_COMMON_DATA][LicDefines.KEYWORD_MSG_SOFTDOGS];
                if (dogs != null)
                {
                    for (int i = 0; i < dogs.Count; i++)
                    {
                        var dog = dogs[i];
                        SoftdogViewItem item = new SoftdogViewItem();
                        item.SerialNumber = dog[LicDefines.KEYWORD_MSG_SOFTDOG_SERIALNUMBER].Value;
                        item.Expiration = dog[LicDefines.KEYWORD_MSG_SOFTDOG_EXPIRATION].Value;
                        item.Type = (SoftdogType)(int)dog[LicDefines.KEYWORD_MSG_SOFTDOG_TYPE].Number;
                        item.Data = dog[LicDefines.KEYWORD_MSG_APPINFO_LICENSES];
                        var temp = mListSoftdogs.FirstOrDefault(s => s.SerialNumber == item.SerialNumber);
                        if (temp != null)
                        {
                            mListSoftdogs.Remove(temp);
                        }
                        mListSoftdogs.Add(item);
                    }
                }

                BindPropertyView();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        #endregion


        #region Operations

        private void OpenLiceDetail()
        {
            try
            {
                var item = LvProperty.SelectedItem as PropertyViewItem;
                if (item == null) { return; }
                if (!item.SerialNo.Equals(LIC_ID_UMP_01)) { return; }
                LicDetailWindow win=new LicDetailWindow();
                win.LicViewItem = item;
                win.ListAllLicItems = mListProperties;
                var result = win.ShowDialog();
                if (result == true)
                {
                    
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        #endregion


        #region Basics

        private void DisConnect()
        {
            mListClients.Clear();
            mListLicensePools.Clear();
            mListSoftdogs.Clear();
            mListLicenseServers.Clear();
            mListProperties.Clear();
            mIsConnected = false;
            if (mLicConnector != null)
            {
                mLicConnector.Close();
                mLicConnector = null;
            }
            SetStatues();
        }

        private bool CheckInput()
        {
            if (string.IsNullOrEmpty(TxtName.Text))
            {
                ShowErrorMessage(string.Format("Name is empty."));
                return false;
            }
            if (string.IsNullOrEmpty(TxtServer.Text))
            {
                ShowErrorMessage(string.Format("Server address is empty."));
                return false;
            }
            int intValue;
            if (!int.TryParse(TxtPort.Text, out intValue))
            {
                ShowErrorMessage(string.Format("Port invalid."));
                return false;
            }
            return true;
        }

        private void BindPropertyView()
        {
            try
            {
                ViewItem viewItem;
                JsonProperty jsonProperty;
                mListProperties.Clear();
                switch (mPropetySource)
                {
                    case "C":
                        viewItem = LvClient.SelectedItem as ClientViewItem;
                        if (viewItem != null)
                        {
                            jsonProperty = viewItem.Data as JsonProperty;
                            if (jsonProperty != null)
                            {
                                CreatePropertyView(jsonProperty);
                            }
                        }
                        break;
                    case "S":
                        viewItem = LvSoftDog.SelectedItem as SoftdogViewItem;
                        if (viewItem != null)
                        {
                            jsonProperty = viewItem.Data as JsonProperty;
                            if (jsonProperty != null)
                            {
                                CreatePropertyView(jsonProperty);
                            }
                        }
                        break;
                    case "LP":
                        viewItem = LvLicensePool.SelectedItem as LicensePoolViewItem;
                        if (viewItem != null)
                        {
                            jsonProperty = viewItem.Data as JsonProperty;
                            if (jsonProperty != null)
                            {
                                CreatePropertyView(jsonProperty);
                            }
                        }
                        break;
                }
                SetStatues();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void CreatePropertyView(JsonProperty jsonProperty)
        {
            try
            {
                mListProperties.Clear();
                if (jsonProperty != null && jsonProperty.GetValue() != null)
                {
                    var lics = jsonProperty;
                    for (int i = 0; i < lics.Count; i++)
                    {
                        var lic = lics[i];
                        License license = new License();
                        license.SerialNo = (long)lic[LicDefines.KEYWORD_MSG_LICENSE_LICENSEID].Number;
                        license.Name = lic[LicDefines.KEYWORD_MSG_LICENSE_DISPLAY].Value;
                        license.Expiration = lic[LicDefines.KEYWORD_MSG_LICENSE_EXPIRATION].Value;
                        license.MajorID =
                            (int)lic[LicDefines.KEYWORD_MSG_LICENSE_MODULEMARJORTYPEID].Number;
                        license.MinorID =
                            (int)lic[LicDefines.KEYWORD_MSG_LICENSE_MODULEMINJORTYPEID].Number;
                        license.Type =
                            (LicOwnerType)
                                (int)lic[LicDefines.KEYWORD_MSG_LICENSE_OWNERTYPE].Number;
                        license.Value = lic[LicDefines.KEYWORD_MSG_LICENSE_VALUE].Value;
                        license.DataType =
                            (LicDataType)(int)lic[LicDefines.KEYWORD_MSG_LICENSE_VALUETYPE].Number;
                        PropertyViewItem propItem = new PropertyViewItem();
                        propItem.Name = license.Name;
                        propItem.SerialNo = license.SerialNo.ToString();
                        propItem.LicType = license.Type == LicOwnerType.Mono ? "Mono" : "Share";
                        propItem.Catetory = propItem.LicType;
                        propItem.Expiration = license.Expiration;
                        propItem.Value = license.Value;
                        propItem.MajorID = license.MajorID.ToString();
                        propItem.MinorID = license.MinorID.ToString();
                        propItem.Data = license;
                        mListProperties.Add(propItem);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void SetStatues()
        {
            LbClientNum.Content = mListClients.Count.ToString();
            LbSoftdogNum.Content = mListSoftdogs.Count.ToString();
            LbLicenseServerNum.Content = mListLicenseServers.Count.ToString();
            LbPropertyNum.Content = mListProperties.Count.ToString();
        }

        private void ShowErrorMessage(string msg)
        {
            MessageBox.Show(string.Format("{0}", msg), "License Monitor", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ShowInfoMessage(string msg)
        {
            MessageBox.Show(string.Format("{0}", msg), "License Monitor", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void WriteLog(string msg)
        {
            ThreadPool.QueueUserWorkItem(a =>
            {
                try
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        if (TxtMsg.LineCount > 10000)
                        {
                            TxtMsg.Clear();
                        }
                        TxtMsg.AppendText(string.Format("{0}\t{1}\r\n", DateTime.Now.ToString("HH:mm:ss"), msg));
                        TxtMsg.ScrollToEnd();
                    }));
                }
                catch { }
            });
        }

        #endregion


        #region Encrypt and Decrypt

        public string DecryptString(string source, int mode)
        {
            //return source;
            return ServerAESEncryption.DecryptString(source, (EncryptionMode)mode, Encoding.ASCII);
        }

        public string DecryptString(string source)
        {
            return source;
        }

        public string EncryptString(string source, int mode)
        {
            //return source;
            return ServerAESEncryption.EncryptString(source, (EncryptionMode)mode, Encoding.ASCII);
        }

        public string EncryptString(string source)
        {
            return source;
        }

        public byte[] DecryptBytes(byte[] source, int mode)
        {
            return ServerAESEncryption.DecryptBytes(source, (EncryptionMode)mode);
        }

        public byte[] DecryptBytes(byte[] source)
        {
            return source;
        }

        public string DecryptString(string source, int mode, Encoding encoding)
        {
            return ServerAESEncryption.DecryptString(source, (EncryptionMode)mode, encoding);
        }

        public byte[] EncryptBytes(byte[] source, int mode)
        {
            return ServerAESEncryption.EncryptBytes(source, (EncryptionMode)mode);
        }

        public byte[] EncryptBytes(byte[] source)
        {
            return source;
        }

        public string EncryptString(string source, int mode, Encoding encoding)
        {
            return source;
        }

        #endregion

    }
}
