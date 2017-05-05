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
using UMPS6106.Service11012;
using UMPS6106.UCCharts;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using Common6106;
using VoiceCyber.UMP.Controls;

namespace UMPS6106
{
    /// <summary>
    /// Page1.xaml 的交互逻辑
    /// </summary>
    public partial class MainPage
    {
        #region 通话量统计变量声明
        BackgroundWorker bgwRecordCount = null;
        public static ObservableCollection<OperationInfo> AllListDashBoard = new ObservableCollection<OperationInfo>();
        private BackgroundWorker mWorker;
        #endregion

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void Init()
        {
            PageName = "MainPage";
            StylePath = "UMPS6106/MainPageResource.xaml";
            base.Init();
            SendLoadedMessage();
            Load_Page();
        }

        private void Load_Page()
        {
            //UC_RecordCount uc_RecCountChart = new UC_RecordCount(20);
            //UC_UMPUsedCount uc_UMPUsedCount = new UC_UMPUsedCount(20);
            //grdMain.Children.Add(uc_UMPUsedCount);
            GetUserOpts("6106");
            CreateNav();
        }

        /// <summary>
        /// 获得用户可操作权限
        /// </summary>
        private void GetUserOpts(string strParent)
        {
            WebRequest webRequest = new WebRequest();
            webRequest.Code = (int)RequestCode.WSGetUserOptList;
            webRequest.Session = App.Session;
            webRequest.ListData.Add(App.Session.UserID.ToString());
            webRequest.ListData.Add("61");
            webRequest.ListData.Add(strParent);
            Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(App.Session),
                 WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service11012"));
            WebReturn webReturn = client.DoOperation(webRequest);
            App.MonitorHelper.AddWebReturn(webReturn);
            client.Close();
            if (!webReturn.Result)
            {
                App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                return;
            }
            for (int i = 0; i < webReturn.ListData.Count; i++)
            {
                OperationReturn optReturn = XMLHelper.DeserializeObject<OperationInfo>(webReturn.ListData[i]);
                if (!optReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                OperationInfo optInfo = optReturn.Data as OperationInfo;
                if (optInfo != null)
                {
                    optInfo.Display = App.GetLanguageInfo(string.Format("FO{0}", optInfo.ID), optInfo.ID.ToString());
                    optInfo.Description = optInfo.Display;
                    AllListDashBoard.Add(optInfo);
                    GetUserOpts(optInfo.ID.ToString());
                }
            }
        }

        /// <summary>
        /// 创建左边导航栏
        /// </summary>
        private void CreateNav()
        {
            var groups = AllListDashBoard.Where(p => p.ParentID == 6106).ToList();

            if (groups.Count <= 0)
            {
                return;
            }
            Expander exp = null;
            for (int i = 0; i < groups.Count; i++)
            {
                exp = new Expander();
                exp.Header = groups[i].Display;
                exp.Content = groups[i];
                exp.SetResourceReference(StyleProperty, "ExpandStyle");
                exp.MouseDoubleClick += exp_MouseDoubleClick;
                //添加子项
                CreateChiledNav(exp, groups[i]);
                PanelOperationButtons.Children.Add(exp);
                exp.Height = 500;
            }
        }

        void exp_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            wrapPanel.Children.Clear();
            //双击组时 显示所有的图表
            UC_RecordCount uc1 = new UC_RecordCount(App.iStatisticsDay, App.UCHeightInGroup, App.UCWidthInGroup);
            uc1.Margin = new Thickness(20);
            wrapPanel.Children.Add(uc1);

            UC_UMPUsedCount uc2 = new UC_UMPUsedCount(App.iStatisticsDay, App.UCHeightInGroup, App.UCWidthInGroup);
            uc2.Margin = new Thickness(20);
            wrapPanel.Children.Add(uc2);
        }

        private void CreateChiledNav(Expander exp, OperationInfo optInfo)
        {
            var childs = AllListDashBoard.Where(p => p.ParentID == optInfo.ID).ToList();
            if (childs.Count <= 0)
            {
                return;
            }
            OperationInfo item;
            Button btn;
            StackPanel sp = new StackPanel();
            sp.Height = 300;
            for (int i = 0; i < childs.Count; i++)
            {
                item = childs[i];
                //基本操作按钮
                btn = new Button();
                btn.Click += BasicOpt_Click;
                btn.DataContext = item;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                sp.Children.Add(btn);
            }
            exp.Content = sp;
        }

        void BasicOpt_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            if (btn == null)
            {
                return;
            }
            var optItem = btn.DataContext as OperationInfo;
            if (optItem == null) { return; }
            switch (optItem.ID)
            {
                case (int)S6106Const.OPT_RecCount:
                    UC_RecordCount ucRecCount = new UC_RecordCount(App.iStatisticsDay, App.UCHeight, App.UCWidth);
                    wrapPanel.Children.Clear();
                    wrapPanel.Children.Add(ucRecCount);
                    break;
                case (int)S6106Const.OPT_UmpUsedCount:
                    UC_UMPUsedCount ucUmpUsedCount = new UC_UMPUsedCount(App.iStatisticsDay, App.UCHeight, App.UCWidth);
                    wrapPanel.Children.Clear();
                    wrapPanel.Children.Add(ucUmpUsedCount);
                    break;
                case (int)S6106Const.OPT_RecLength:
                    UC_RecordLength ucRecLength = new UC_RecordLength(App.iStatisticsDay, App.UCHeight, App.UCWidth);
                    wrapPanel.Children.Clear();
                    wrapPanel.Children.Add(ucRecLength);
                    break;
                case (int)S6106Const.OPT_QutityCount:
                    UC_QutityCount ucQutityCount = new UC_QutityCount(App.iStatisticsDay, App.UCHeight, App.UCWidth);
                    wrapPanel.Children.Clear();
                    wrapPanel.Children.Add(ucQutityCount);
                    break;
                case (int)S6106Const.OPT_AppealCount:
                    UC_AppealCount ucAppealCount = new UC_AppealCount(App.iStatisticsDay, App.UCHeight, App.UCWidth);
                    wrapPanel.Children.Clear();
                    wrapPanel.Children.Add(ucAppealCount);
                    break;
                case (int)S6106Const.OPT_WariningCount:
                    UC_WarningCount ucWarningCount = new UC_WarningCount(App.iStatisticsDay, App.UCHeight, App.UCWidth);
                    wrapPanel.Children.Clear();
                    wrapPanel.Children.Add(ucWarningCount);
                    break;
                case (int)S6106Const.OPT_ReplayCount:
                    UC_ReplayCount ucReplayCount = new UC_ReplayCount(App.iStatisticsDay, App.UCHeight, App.UCWidth);
                    wrapPanel.Children.Clear();
                    wrapPanel.Children.Add(ucReplayCount);
                    break;
                case (int)S6106Const.OPT_AvgScore:
                    UC_AvgScore ucAvgScore = new UC_AvgScore(App.iStatisticsDay, App.UCHeight, App.UCWidth);
                     wrapPanel.Children.Clear();
                     wrapPanel.Children.Add(ucAvgScore);
                    break;
            }
        }

        #region Overried
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
                    string uri = string.Format("/Themes/{0}/{1}",
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
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

        }

        /// <summary>
        /// 重写pageHead的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void PageHead_PageHeadEvent(object sender, PageHeadEventArgs e)
        {
            base.PageHead_PageHeadEvent(sender, e);

            switch (e.Code)
            {
                //切换主题
                case 100:
                    ThemeInfo themeInfo = e.Data as ThemeInfo;
                    if (themeInfo != null)
                    {
                        ThemeInfo = themeInfo;
                        App.Session.ThemeInfo = themeInfo;
                        App.Session.ThemeName = themeInfo.Name;
                        ChangeTheme();
                        // ChangeUsercontrolThemes();
                        SendThemeChangeMessage();
                    }
                    break;
                //切换语言
                case 110:
                    LangTypeInfo langType = e.Data as LangTypeInfo;
                    if (langType != null)
                    {
                        LangTypeInfo = langType;
                        App.Session.LangTypeInfo = langType;
                        App.Session.LangTypeID = langType.LangID;
                        MyWaiter.Visibility = Visibility.Visible;
                        mWorker = new BackgroundWorker();
                        mWorker.DoWork += (s, de) => App.InitLanguageInfos("6106");
                        mWorker.RunWorkerCompleted += (s, re) =>
                        {
                            mWorker.Dispose();
                            MyWaiter.Visibility = Visibility.Hidden;
                            ChangeLanguage();
                            // ChangeUsercontrolThemes();
                            //PopupPanel.ChangeLanguage();
                            SendLanguageChangeMessage();
                        };
                        mWorker.RunWorkerAsync();
                    }

                    break;
            }
        }
        #endregion
    }
}
