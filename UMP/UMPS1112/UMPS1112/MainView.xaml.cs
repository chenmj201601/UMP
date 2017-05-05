using Common11121;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UMPS1112.Converters;
using UMPS1112.Wcf11121;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.UMP.Controls.Wcf11012;

namespace UMPS1112
{
    /// <summary>
    /// Page1.xaml 的交互逻辑
    /// </summary>
    public partial class MainView
    {
        private BackgroundWorker mWorker;

        private ObservableCollection<ViewColumnInfo> mListDomainDataColumns;
        //private ObservableCollection<OperationInfo> mListBasicOperations;
        //这个是各种操作的列表  可以看OperationInfo这个类里面的,其中每一个都是代表的一种操作
        public static ObservableCollection<OperationInfo> ListOperations = new ObservableCollection<OperationInfo>();
        public ObservableCollection<BasicDomainInfo> mListDomainInfo;
        private BasicDomainInfo Domaininfo;
        private S1112App S1112App;
        public MainView()
        {
            InitializeComponent();

            mListDomainDataColumns = new ObservableCollection<ViewColumnInfo>();
            ListOperations = new ObservableCollection<OperationInfo>();
            //mListBasicOperations = new ObservableCollection<OperationInfo>();
            mListDomainInfo = new ObservableCollection<BasicDomainInfo>();
            //Domaininfo = new DomainInfo();
            LvDomianData.ItemsSource = mListDomainInfo;

            this.LvDomianData.SelectionChanged += LvDomianData_SelectionChanged;
        }

        protected override void Init()
        {
            try
            {
                PageName = "MainPage";
                StylePath = "UMPS1112/MainPage.xaml";

                base.Init();
                if (CurrentApp != null)
                {
                    S1112App = CurrentApp as S1112App;
                }
                else
                {
                    S1112App = new S1112App(false);
                }
                ChangeTheme();
                ChangeLanguage();

                mListDomainInfo.Clear();
                SetBusy(true, CurrentApp.GetMessageLanguageInfo("001", string.Format("Loading data, please wait...")));
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    InitOperations();
                    InitRecordDataColumns();
                    InitDomainInfo();
                    CurrentApp.WriteLog("PageLoad", string.Format("All data loaded"));
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    SetBusy(false, string.Empty);
                    CreateOptButtons();
                    CreateDomainDataColumns();
                    //触发Loaded消息
                    CurrentApp.SendLoadedMessage();

                    ChangeTheme();
                    ChangeLanguage();

                    CurrentApp.WriteLog(string.Format("Load\t\tPage load end"));
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(ex.Message);
            }
        }

        #region ChangTheme

        public override void ChangeTheme()
        {
            base.ChangeTheme();

            bool bPage = false;
            if (AppServerInfo != null)
            {
                //优先从服务器上加载资源文件
                try
                {
                    string uri = string.Format("{0}://{1}:{2}/Themes/{3}/{4}",
                        AppServerInfo.Protocol,
                        AppServerInfo.Address,
                        AppServerInfo.Port,
                        ThemeInfo.Name
                        , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                    bPage = true;
                }
                catch (Exception)
                {
                    //App.ShowExceptionMessage("1" + ex.Message);
                }
            }
            if (!bPage)
            {
                //如果通过Url没有加载成功，就从已经编译到程序集的默认资源加载
                try
                {
                    string uri = string.Format("/UMPS1112;component/Themes/{0}/{1}",
                        "Default"
                        , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                }
                catch (Exception ex)
                {
                    //App.ShowExceptionMessage("2" + ex.Message);
                }
            }

            //固定资源(有些资源包含转换器，命令等自定义类型，
            //这些资源不能通过url来动态加载，他只能固定的编译到程序集中
            try
            {
                string uri = string.Format("/UMPS1112;component/Themes/Default/UMPS1112/MainPageStatic.xaml");
                ResourceDictionary resource = new ResourceDictionary();
                resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                Resources.MergedDictionaries.Add(resource);
            }
            catch (Exception ex)
            {
                //App.ShowExceptionMessage("3" + ex.Message);
            }
        }

        #endregion

        #region ChangLanguage

        public override void ChangeLanguage()
        {
            try
            {
                base.ChangeLanguage();
                CurrentApp.AppTitle = CurrentApp.GetLanguageInfo("FO1112", "UMP Domain Management");
                //Operation
                for (int i = 0; i < ListOperations.Count; i++)
                {
                    ListOperations[i].Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", ListOperations[i].ID),
                        ListOperations[i].ID.ToString());
                }
                CreateOptButtons();
                this.ExpBasicOpt.Header = CurrentApp.GetLanguageInfo("1112001", "基本操作");

                //InitOperations(); CreateOptButtons();
                CreateDomainDataColumns();
                if (PopupPanel.IsOpen)
                {
                    PopupPanel.Title = CurrentApp.GetLanguageInfo("1112P001", "");
                    PopupPanel.ChangeLanguage();
                }
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("ChangeLang", string.Format("ChangeLang fail.\t{0}", ex.Message));
            }
        }

        #endregion

        private void InitOperations()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserOptList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("11");
                webRequest.ListData.Add("1112");
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    CurrentApp.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                ListOperations.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<OperationInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        CurrentApp.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    OperationInfo optInfo = optReturn.Data as OperationInfo;
                    if (optInfo != null)
                    {
                        optInfo.Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", optInfo.ID), optInfo.ID.ToString());
                        optInfo.Description = optInfo.Display;
                        Dispatcher.Invoke(new Action(() => ListOperations.Add(optInfo)));
                    }
                }

                CurrentApp.WriteLog("PageLoad", string.Format("Load Operation"));
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(ex.Message);
            }
        }

        private void InitRecordDataColumns()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserViewColumnList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("1112001");
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    CurrentApp.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                List<ViewColumnInfo> listColumns = new List<ViewColumnInfo>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<ViewColumnInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        CurrentApp.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ViewColumnInfo columnInfo = optReturn.Data as ViewColumnInfo;
                    if (columnInfo != null)
                    {
                        columnInfo.Display = columnInfo.ColumnName;
                        listColumns.Add(columnInfo);
                    }
                }
                listColumns = listColumns.OrderBy(c => c.SortID).ToList();
                mListDomainDataColumns.Clear();
                for (int i = 0; i < listColumns.Count; i++)
                {
                    Dispatcher.Invoke(new Action(() => mListDomainDataColumns.Add(listColumns[i])));
                }

                CurrentApp.WriteLog("PageLoad", string.Format("Load RecordColumn"));
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(ex.Message);
            }
        }

        public void InitDomainInfo()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S1112Codes.GetDomainInfo;
                webRequest.Session = CurrentApp.Session;

                Service11121Client client = new Service11121Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11121"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    CurrentApp.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                //Dispatcher.Invoke(new Action(() => mListDomainInfo.Clear()));
                List<BasicDomainInfo> ListDomainInfo = new List<BasicDomainInfo>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<BasicDomainInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        CurrentApp.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    BasicDomainInfo domainInfo = optReturn.Data as BasicDomainInfo;
                    if (domainInfo != null)
                    {
                        domainInfo.DomainUserPassWord = domainInfo.DomainUserPassWord.Substring(20);
                        ListDomainInfo.Add(domainInfo);
                    }
                }
                ListDomainInfo = ListDomainInfo.OrderBy(p => p.DomainCode).ToList();
                foreach (BasicDomainInfo di in ListDomainInfo)
                {
                    if (!di.IsDelete)
                    {
                        //DomainInfos dis = new DomainInfos(di);
                        Dispatcher.Invoke(new Action(() => mListDomainInfo.Add(di)));
                    }
                }
                CurrentApp.WriteLog("PageLoad", string.Format("Load Operation"));
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(ex.Message);
            }
        }

        private void CreateOptButtons()
        {
            PanelBasicOpts.Children.Clear();
            OperationInfo item;
            Button btn;
            for (int i = 0; i < ListOperations.Count; i++)
            {
                item = ListOperations[i];
                //基本操作按钮
                btn = new Button();
                btn.Click += BasicOpt_Click;
                btn.DataContext = item;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                PanelBasicOpts.Children.Add(btn);
            }
        }

        private void CreateDomainDataColumns()
        {
            try
            {
                GridView gv = new GridView();
                GridViewColumn gvc;
                GridViewColumnHeader gvch;
                for (int i = 0; i < mListDomainDataColumns.Count; i++)
                {
                    ViewColumnInfo columnInfo = mListDomainDataColumns[i];
                    if (columnInfo.Visibility == "1")
                    {
                        gvc = new GridViewColumn();
                        gvch = new GridViewColumnHeader();
                        gvch.Content = columnInfo.Display;
                        gvch.Content = CurrentApp.GetLanguageInfo(string.Format("COL1112001{0}", columnInfo.ColumnName), columnInfo.Display);
                        gvch.ToolTip = CurrentApp.GetLanguageInfo(string.Format("COL1112001{0}", columnInfo.ColumnName), columnInfo.Display);
                        //gvch.Command = QMMainPageCommands.GridViewColumnHeaderCommand;
                        gvch.CommandParameter = columnInfo;
                        gvc.Header = gvch;
                        gvc.Width = columnInfo.Width;
                        //Binding binding;
                        DataTemplate dt = null;
                        if (columnInfo.ColumnName == "SerialID")
                        {
                            dt = Resources["CellRecordListSerialIDTemplate"] as DataTemplate;
                        }
                        if (columnInfo.ColumnName == "State")
                        {
                            dt = Resources["ObjectStateCellTemplate"] as DataTemplate;
                        }
                        //if (columnInfo.ColumnName == "Password")
                        //{
                        //    dt = Resources["ObjectPasswordCellTemplate"] as DataTemplate;
                        //}
                        if (dt != null)
                        {
                            gvc.CellTemplate = dt;
                        }
                        else
                        {
                            gvc.DisplayMemberBinding = new Binding(columnInfo.ColumnName);
                        }

                        if (columnInfo.ColumnName == "Name")
                        {
                            gvc.DisplayMemberBinding = new Binding("DomainName");
                        }
                        if (columnInfo.ColumnName == "Sort")
                        {
                            gvc.DisplayMemberBinding = new Binding("DomainCode");
                        }
                        if (columnInfo.ColumnName == "User")
                        {
                            gvc.DisplayMemberBinding = new Binding("DomainUserName");
                        }

                        gv.Columns.Add(gvc);
                    }
                }
                LvDomianData.View = gv;
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(ex.Message);
            }
        }

        void BasicOpt_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            if (btn != null)
            {
                var optItem = btn.DataContext as OperationInfo;
                if (optItem == null) { return; }
                switch (optItem.ID)
                {
                    case S1112Consts.OPT_ADDDOMAIN:
                        ModifyDomain(1);
                        break;
                    case S1112Consts.OPT_MODIFY:
                        ModifyDomain(2);
                        break;
                    case S1112Consts.OPT_DELETEDOMAIN:
                        DeleteDomianInfo();
                        break;
                }
            }
        }

        void LvDomianData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Domaininfo = this.LvDomianData.SelectedItem as BasicDomainInfo;
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(ex.Message);
            }
        }

        private void DeleteDomianInfo()
        {
            try
            {
                if (Domaininfo == null) { return; }
                string strInfoMsg = CurrentApp.GetLanguageInfo("1112T008", "确定删除改域信息吗？");
                var result = MessageBox.Show(string.Format("{0}\r\n\r\n{1}",
                    CurrentApp.GetMessageLanguageInfo("003", "Confirm delete user ?"),
                    strInfoMsg), "UMP",
                   MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Code = (int)S1112Codes.SaveDomainInfo;
                    webRequest.Session = CurrentApp.Session;
                    webRequest.ListData.Add("0");
                    OperationReturn optR = XMLHelper.SeriallizeObject(Domaininfo);
                    if (!optR.Result)
                    {
                        CurrentApp.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optR.Code, optR.Message));
                        return;
                    }
                    webRequest.ListData.Add(optR.Data as string);
                    Service11121Client client = new Service11121Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                        WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11121"));
                    WebReturn webReturn = client.DoOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        //if (webReturn.Code == 99)
                        //{
                        //    App.ShowExceptionMessage(App.GetLanguageInfo("", "域名已存在"));
                        //}
                        //else
                        //{
                        CurrentApp.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        //}
                        return;
                    }
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1112T002", "域信息删除成功"));
                    //写操作日志
                    string msg = string.Format("{0}:{1}", CurrentApp.GetLanguageInfo("FO1112003", "delete"), Domaininfo.DomainName);
                    CurrentApp.WriteOperationLog(string.Format("1112003"), ConstValue.OPT_RESULT_SUCCESS, msg);
                    mListDomainInfo.Clear();
                    InitDomainInfo();
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}:{1}", CurrentApp.GetLanguageInfo("FO1112003", "delete"), Domaininfo.DomainName);
                CurrentApp.WriteOperationLog(string.Format("1112003"), ConstValue.OPT_RESULT_FAIL, msg);
                CurrentApp.ShowExceptionMessage(ex.Message);
            }
        }

        private void ModifyDomain(int intMode)
        {
            if (intMode == 2 && Domaininfo == null)
            {
                return;
            }
            //打开窗口
            PopupPanel.Title = CurrentApp.GetLanguageInfo("1112P001", "Design");

            UCDomainModifyPage ModifyPage = new UCDomainModifyPage();
            ModifyPage.ParentPage = this;
            ModifyPage.IsAdd = intMode;
            ModifyPage.domainInfo = Domaininfo;
            ModifyPage.CurrentApp = S1112App;
            PopupPanel.Content = ModifyPage;
            PopupPanel.IsOpen = true;
        }
    }
}
