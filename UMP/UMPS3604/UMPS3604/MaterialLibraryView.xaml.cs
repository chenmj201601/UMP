using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Common3604;
using UMPS3604.Models;
using UMPS3604.Wcf36041;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.Wpf.AvalonDock.Layout;
using Button = System.Windows.Controls.Button;
using MessageBox = System.Windows.MessageBox;

namespace UMPS3604
{
    /// <summary>
    /// Interaction logic for MaterialLibraryView.xaml
    /// </summary>
    public partial class MaterialLibraryView
    {
        #region Members

        private BackgroundWorker _mWorker;
        private readonly List<PanelItem> _mListPanels;
        private List<ContentsTree> _mLstContentsTreeParam;
        private List<ContentsTree> _mLstContentsTreeNodes;
        private readonly ContentsTree _mContentsRootNode;
        private readonly List<ContentsTree> _mLstSearchContentsNode;
        private List<long> _mLstContentsNum;
        private ContentsTree _mContentsNodeTemp;
        private List<FileProperty> _mlstUploadFiles; 

        #endregion

        public MaterialLibraryView()
        {
            _mLstSearchContentsNode = new List<ContentsTree>();
            _mLstContentsTreeNodes = new List<ContentsTree>();
            _mListPanels = new List<PanelItem>();
            _mLstContentsTreeParam = new List<ContentsTree>();
            _mContentsRootNode = new ContentsTree();
            _mLstContentsNum = new List<long>();
            _mContentsNodeTemp = new ContentsTree(); 
            _mlstUploadFiles = new List<FileProperty>();
            InitializeComponent();
        }

        #region 初始化 & 全局消息

        protected override void Init()
        {
            try
            {
                PageName = "MaterialLibrary";
                StylePath = "UMPS3604/MainPageStyle.xaml";
                base.Init();

                CurrentApp.SendLoadedMessage();
                string strSql = string.Format("SELECT * FROM T_36_001_{0}", CurrentApp.Session.RentInfo.Token);
                InitContentsTreeInfo(strSql);
                InitPanels();
                CreateToolBarButtons();
                ChangeLanguage();
                PapersListInfoLoaded();
                ContentsCheckableTree.SelectedItemChanged += OrgContentsTree_SelectedItemChanged;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void PapersListInfoLoaded()
        {
            try
            {
                SetBusy(true, CurrentApp.GetMessageLanguageInfo("001", "Loading data, please wait..."));
                _mWorker = new BackgroundWorker();
                _mWorker.WorkerReportsProgress = true;
                _mWorker.WorkerSupportsCancellation = true;
                //注册线程主体方法
                _mWorker.DoWork += (s, de) =>
                {
                    
                };
                _mWorker.RunWorkerCompleted += (s, re) =>
                {
                    ChangeLanguage();
                    _mWorker.Dispose();
                    SetBusy(false, CurrentApp.GetMessageLanguageInfo("001", "Ready"));
                };
                _mWorker.RunWorkerAsync(); //触发DoWork事件
            }
            catch (Exception)
            {
                SetBusy(false, CurrentApp.GetMessageLanguageInfo("001", "Ready"));
            }
        }

        private void InitPanels()
        {
            try
            {
                _mListPanels.Clear();
                PanelItem panelItem = new PanelItem();
                panelItem.PanelId = S3604Consts.PANEL_ID_TREE;
                panelItem.Name = S3604Consts.PANEL_NAME_TREE;
                panelItem.ContentId = S3604Consts.PANEL_CONTENTID_TREE;
                panelItem.Title = CurrentApp.GetLanguageInfo("3604T00006", "Material Contents");
                panelItem.Icon = "Images/tree.png";
                panelItem.IsVisible = true;
                panelItem.CanClose = true;
                _mListPanels.Add(panelItem);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateToolBarButtons()
        {
            try
            {
                PanelToolButton.Children.Clear();
                for (int i = 0; i < _mListPanels.Count; i++)
                {
                    PanelItem item = _mListPanels[i];
                    if (!item.CanClose)
                    {
                        continue;
                    }
                    var toolItem = new ToolButtonItem();
                    toolItem.Name = "TB" + item.Name;
                    toolItem.Display = item.Title;
                    toolItem.Tip = item.Title;
                    toolItem.Icon = item.Icon;
                    var toggleBtn = new ToggleButton();
                    toggleBtn.Click += PanelToggleButton_Click;
                    toggleBtn.DataContext = toolItem;
                    toggleBtn.IsChecked = item.IsVisible;
                    toggleBtn.SetResourceReference(StyleProperty, "ToolBarToggleBtnStyle");
                    PanelToolButton.Children.Add(toggleBtn);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion

        #region 样式&语言

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
                    //S3603App.ShowExceptionMessage("1" + ex.Message);
                }
            }
            if (!bPage)
            {
                //如果通过Url没有加载成功，就从已经编译到程序集的默认资源加载
                try
                {
                    string uri = string.Format("/UMPS3604;component/Themes/{0}/{1}",
                        "Default"
                        , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                }
                catch (Exception)
                {
                    //S3603App.ShowExceptionMessage("2" + ex.Message);
                }
            }

            try
            {
                string uri = "/UMPS3604;component/Themes/Default/UMPS3604/AvalonDock.xaml";
                ResourceDictionary resource = new ResourceDictionary();
                resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                Resources.MergedDictionaries.Add(resource);
            }
            catch (Exception)
            {
                //S3602App.ShowExceptionMessage("2" + ex.Message);
            }
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();
            InitButton();
            InitLanguage();

            PageName = "MaterialLibrary";
            CurrentApp.AppTitle = CurrentApp.GetLanguageInfo("3604T00001", "Material Library");
            PanelObjectTreeBox.Title = CurrentApp.GetLanguageInfo("3604T00006", "Material Contents");
            MaterialsDocument.Title = CurrentApp.GetLanguageInfo("3604T00008", "Material");
        }

        private void InitLanguage()
        {
            ContentsExpandOpt.Header = CurrentApp.GetLanguageInfo("3604T00002", "Contents");
            MaterialExpandOpt.Header = CurrentApp.GetLanguageInfo("3604T00005", "Material");
            DeleteExpandOpt.Header = CurrentApp.GetLanguageInfo("3604T00004", "Delete");
        }

        private void InitButton()
        {
            try
            {
                ButSearchContent.Children.Clear();

                var btn = new Button();
                btn.Click += SearchContents_Click;
                var opt = new OperationInfo { Icon = "Images/search.png" };
                TbSearchContent.Text = CurrentApp.GetLanguageInfo("3604T00007", "Search Content");
                btn.DataContext = opt;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                ButSearchContent.Children.Add(btn);    
            }
            catch (Exception ex)
            {
                ShowException( ex.Message );
            }    
        }

        #endregion

        #region Click

        void PanelToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var toggleBtn = e.Source as ToggleButton;
            if (toggleBtn != null)
            {
                ToolButtonItem item = toggleBtn.DataContext as ToolButtonItem;
                if (item != null)
                {
                    PanelItem panelItem = _mListPanels.FirstOrDefault(p => string.Format("TB{0}", p.Name) == item.Name);
                    if (panelItem == null)
                    {
                        return;
                    }
                    panelItem.IsVisible = toggleBtn.IsChecked == true;
                }
                SetPanelVisible();
            }
        }

        private void CreateContents_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            if (btn != null)
            {
                try
                {
                    ContentsTree nodeInfo = ContentsCheckableTree.SelectedItem as ContentsTree;
                    if (nodeInfo == null) { return; }

                    _mContentsNodeTemp = nodeInfo;
                    S3604App.GiOptContentsInfo = (int)S3604Consts.OPT_Add;
                    List<ContentsTree> lstCategoryTreeChild = GetAllChildInfo(_mContentsNodeTemp);
                    if (lstCategoryTreeChild == null)
                    {
                        return;
                    }
                    _mContentsNodeTemp.LstChildInfos = lstCategoryTreeChild;
                    S3604App.GContentsTree = _mContentsNodeTemp;
                    S3604App.GQueryModify = false;
                    ContentsProperties newPage = new ContentsProperties
                    {
                        ParentPage = this,
                        CurrentApp = CurrentApp
                    };
                    PopupPanelInfo.Content = newPage;
                    PopupPanelInfo.Title = CurrentApp.GetLanguageInfo("3604T00003", "Create Contents");
                    PopupPanelInfo.IsOpen = true;
                }
                catch (Exception ex)
                {
                    ShowException(ex.Message);
                }
            }
        }

        private void ChangeContents_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                S3604App.GiOptContentsInfo = (int)S3604Consts.OPT_Change;
                if (_mContentsNodeTemp.LongParentNodeId == 0)
                {
                    return;
                }
                List<ContentsTree> lstContentsTreeChild = GetAllChildInfo(_mContentsNodeTemp);
                if (lstContentsTreeChild == null)
                {
                    return;
                }
                _mContentsNodeTemp.LstChildInfos = lstContentsTreeChild;
                List<ContentsTree> lstContentsTreeNode = GetAllNodeInfo(_mContentsNodeTemp);
                if (lstContentsTreeNode == null)
                {
                    return;
                }
                _mContentsNodeTemp.LstNodeInfos = lstContentsTreeNode;
                S3604App.GContentsTree = _mContentsNodeTemp;
                S3604App.GQueryModify = false;
                ContentsProperties newPage = new ContentsProperties();
                newPage.ParentPage = this;
                newPage.CurrentApp = CurrentApp;
                PopupPanelInfo.Content = newPage;
                PopupPanelInfo.Title = CurrentApp.GetLanguageInfo("3604T00010", "Change Contents");
                PopupPanelInfo.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void UploadResources_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ContentsTree contentsTree = ContentsCheckableTree.SelectedItem as ContentsTree;
                if (contentsTree == null)
                {
                    return;
                }

                _mlstUploadFiles.Clear();
                OpenFileDialog openFileDialog = new OpenFileDialog();
                //允许多选
                openFileDialog.Multiselect = true; 
                openFileDialog.RestoreDirectory = true;
                openFileDialog.FilterIndex = 1;
                openFileDialog.Title = CurrentApp.GetLanguageInfo("3604T00035", "Upload Resource");
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string filePath in openFileDialog.FileNames)
                    {
                        System.IO.FileInfo file = new System.IO.FileInfo(filePath);
                        FileProperty fileProperty = new FileProperty();
                        fileProperty.LSize = (file.Length)/1024 + 1;
                        fileProperty.StrName = file.Name;
                        fileProperty.StrPath = file.DirectoryName;
                        fileProperty.StrFileType = file.Extension;
                        _mlstUploadFiles.Add(fileProperty);
                    }

                    UploadResourceFilesPage newPage = new UploadResourceFilesPage();
                    newPage.ParentPage = this;
                    newPage.MLstFileProperties = _mlstUploadFiles;
                    newPage.CurrentApp = CurrentApp;
                    PopupPanelInfo.Content = newPage;
                    PopupPanelInfo.Title = CurrentApp.GetLanguageInfo("3604T00036", "Upload Resource");
                    PopupPanelInfo.IsOpen = true;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void MaterialsListView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void MaterialsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void MaterialsDocument_OnIsSelectedChanged(object sender, EventArgs e)
        {

        }

        private void PanelDocument_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                e.Cancel = true;
                var panel = sender as LayoutAnchorable;
                if (panel != null)
                {
                    panel.Hide();
                }
            }
            catch
            {
                // ignored
            }
        }

        #endregion

        #region tree opt

        private void SearchContents_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(TxtSearchContent.Text))
                {
                    return;
                }

                _mLstSearchContentsNode.Clear();
                foreach (var param in _mLstContentsTreeParam)
                {
                    if (param.StrNodeName.IndexOf(TxtSearchContent.Text) != -1)
                    {
                        _mLstSearchContentsNode.Add(param);
                    }
                }

                _mLstContentsNum = new List<long>();
                foreach (var categoryTree in _mLstSearchContentsNode)
                {
                    GetCategoryNum(categoryTree.LongNodeId);
                }
                _mContentsRootNode.Children.Clear();
                _mLstContentsTreeNodes.Clear();
                InitContentsTree(_mLstContentsTreeParam, 0, _mContentsRootNode);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DeleteContents_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string strChildNode = String.Empty;
                ContentsTree nodeInfo = ContentsCheckableTree.SelectedItem as ContentsTree;
                List<ContentsTree> lstContentsTreeChild = GetAllChildInfo(nodeInfo);
                if (lstContentsTreeChild.Count > 0)
                {
                    strChildNode = string.Format("{0} {1}", lstContentsTreeChild.Count, CurrentApp.GetLanguageInfo("3604T00028", "Child Node Information"));
                }
                else
                {
                    strChildNode = string.Format("{0}", CurrentApp.GetLanguageInfo("3604T00029", "Child Node Information"));
                }

                MessageBoxResult result =
                    MessageBox.Show(
                        string.Format("{0}\n{1}",
                            CurrentApp.GetLanguageInfo("3604T00030",
                                "Confirm whether delete categories? Delete nodes will also delete all child nodes."),
                            strChildNode),
                        CurrentApp.GetLanguageInfo("3604T00031", "Warning"),
                        MessageBoxButton.OKCancel);
                if (result != MessageBoxResult.OK)
                    return;

                var btn = e.Source as Button;
                ContentsParam contentsParam = new ContentsParam();
                if (nodeInfo != null)
                {
                    contentsParam.LongNodeId = nodeInfo.LongNodeId;
                    contentsParam.StrNodeName = nodeInfo.StrNodeName;
                    contentsParam.LongParentNodeId = nodeInfo.LongParentNodeId;
                    contentsParam.StrParentNodeName = nodeInfo.StrParentNodeName;
                    contentsParam.LongFounderId = nodeInfo.LongFounderId;
                    contentsParam.StrFounderName = nodeInfo.StrFounderName;
                    contentsParam.StrDateTime = nodeInfo.StrDateTime;
                }
                else
                {
                    ShowException(CurrentApp.GetLanguageInfo("3604T00032", "The Contents information for failure!"));
                    return;
                }

                if (btn != null)
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)S3604Codes.OptDeleteContents;
//                     Service36041Client client = new Service36041Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
//                         WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36011"));
//                     OperationReturn optReturn = XMLHelper.SeriallizeObject(contentsParam);
//                     if (!optReturn.Result)
//                     {
//                         ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
//                         return;
//                     }
//                     webRequest.ListData.Add(optReturn.Data.ToString());
//                     var client = new Service36041Client();
//                     WebReturn webReturn = client.UmpTaskOperation(webRequest);
//                     client.Close();
//                     CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3604T00033", "Delete"));
//                     string strLog;
//                     if (!webReturn.Result)
//                     {
//                         ShowException(CurrentApp.GetLanguageInfo("3107T00092", "Delete Failed"));
//                         #region 写操作日志
//                         strLog = string.Format("{0} {1}{2}", Utils.FormatOptLogString("3601T00014"), Utils.FormatOptLogString("3601T00083"), papersCategoryParam.StrName);
//                         CurrentApp.WriteOperationLog(S3604Consts.OPT_Delete.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
//                         #endregion
//                         CurrentApp.WriteLog(webReturn.Message);
//                         return;
//                     }
//                     if (webReturn.Message == S3604Consts.HadUse)// 该查询条件被使用无法删除
//                     {
//                         #region 写操作日志
//                         strLog = string.Format("{0} {1}{2}", Utils.FormatOptLogString("3601T00014"), Utils.FormatOptLogString("3601T00011"), papersCategoryParam.StrName);
//                         CurrentApp.WriteOperationLog(S3601Consts.OPT_Delete.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
//                         #endregion
//                         CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3601T00011", "Can't Delete"));
//                         ShowInformation(CurrentApp.GetLanguageInfo("3601T00011", "Can't Delete"));
//                     }
//                     else
//                     {
//                         CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3601T00012", "Delete Sucessed"));
//                         ShowInformation(CurrentApp.GetLanguageInfo("3601T00012", "Delete Sucessed"));
//                         string strSql = string.Format("SELECT * FROM T_36_021_{0}", CurrentApp.Session.RentInfo.Token);
//                         InitCategoryTreeInfo(strSql);
//                     }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void InitContentsTreeInfo(string strSql)
        {
            try
            {
                _mLstContentsTreeParam.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3604Codes.OptGetContents;
                //Service36041Client client = new Service36041Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                //    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36041"));
                var client = new Service36041Client();
                OperationReturn optReturn = XMLHelper.SeriallizeObject(strSql);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                WebReturn webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(webReturn.Message);
                    return;
                }
                    
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<ContentsParam>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    ContentsParam param = optReturn.Data as ContentsParam;
                    if (param == null)
                    {
                        ShowException("Fail. queryItem is null");
                        return;
                    }

                    ContentsTree tempTree = new ContentsTree();
                    tempTree.LongNodeId = param.LongNodeId;
                    tempTree.StrNodeName = param.StrNodeName;
                    tempTree.LongParentNodeId = param.LongParentNodeId;
                    tempTree.StrParentNodeName = param.StrParentNodeName;
                    tempTree.LongFounderId = param.LongFounderId;
                    tempTree.StrFounderName = param.StrFounderName;
                    tempTree.StrDateTime = param.StrDateTime;
                    _mLstContentsTreeParam.Add(tempTree);
                }

                _mContentsRootNode.Children.Clear();
                _mLstContentsTreeNodes.Clear();
                ContentsCheckableTree.ItemsSource = _mContentsRootNode.Children;
                InitContentsTree(_mLstContentsTreeParam, 0, _mContentsRootNode);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void InitContentsTree(List<ContentsTree> listPapersContentsParam, long longParentNodeId, ContentsTree contentsNodes)
        {
            foreach (ContentsTree param in listPapersContentsParam)
            {
                if (param.LongParentNodeId == longParentNodeId)
                {
                    var nodeTemp = GetContentsNodeInfo(contentsNodes, param);
                    InitContentsTree(listPapersContentsParam, param.LongNodeId, nodeTemp);
                }
            }
        }

        public ContentsTree GetContentsNodeInfo(ContentsTree parentInfo, ContentsTree param)
        {
            ContentsTree temp = new ContentsTree();
            try
            {
                temp.Icon = "/UMPS3604;component/Themes/Default/UMPS3604/Images/document.ico";
                temp.LongNodeId = param.LongNodeId;
                temp.StrNodeName = param.LongParentNodeId == 0 ? CurrentApp.GetLanguageInfo("3604T00009", "Contents") : param.StrNodeName;
                temp.LongParentNodeId = param.LongParentNodeId;
                if (_mLstContentsNum.Count <= 0)
                {
                    if (param.LongParentNodeId == 0) temp.IsExpanded = true;
                }
                else
                {
                    int iCount = 0;
                    foreach (var num in _mLstContentsNum)
                    {
                        iCount++;
                        if (param.LongNodeId == num)
                        {
                            temp.IsExpanded = true;
                            if (iCount == _mLstContentsNum.Count)
                                temp.IsChecked = true;
                        }
                    }
                    foreach (var contentsTree in _mLstSearchContentsNode)
                    {
                        if (param.LongNodeId == contentsTree.LongNodeId)
                        {
                            temp.ChangeBrush = Brushes.Gold;
                        }
                    }
                }
                temp.StrParentNodeName = param.StrParentNodeName;
                temp.LongFounderId = param.LongFounderId;
                temp.StrFounderName = param.StrFounderName;
                temp.StrDateTime = param.StrDateTime;
                _mLstContentsTreeNodes.Add(temp);
                AddChildNode(parentInfo, temp);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
            return temp;
        }

        private void AddChildNode(ContentsTree parentItem, ContentsTree item)
        {
            Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
        }

        private void SetTreeBrush(ContentsTree nodeInfo)
        {
            foreach (ContentsTree param in _mLstContentsTreeNodes)
            {
                if (nodeInfo.LongNodeId == param.LongNodeId)
                {
                    param.ChangeBrush = null;
                }
            }
        }

        private void OrgContentsTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                ContentsTree nodeInfo = ContentsCheckableTree.SelectedItem as ContentsTree;
                if (nodeInfo == null) { return; }
                _mContentsNodeTemp = nodeInfo;
                SetTreeBrush(nodeInfo);

                ButContents.Children.Clear();
                ButDelete.Children.Clear();
                ButMaterial.Children.Clear();

                var btn = new Button();
                btn.Click += CreateContents_Click;
                var opt = new OperationInfo();
                opt.Display = CurrentApp.GetLanguageInfo("3604T00003", "Create Contents");
                opt.Icon = "Images/add.png";
                btn.DataContext = opt;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                ButContents.Children.Add(btn);

                btn = new Button();
                btn.Click += ChangeContents_Click;
                opt = new OperationInfo();
                opt.Display = CurrentApp.GetLanguageInfo("3604T00010", "Change Contents");
                opt.Icon = "Images/change.png";
                btn.DataContext = opt;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                ButContents.Children.Add(btn);

                if (nodeInfo.LongParentNodeId != 0)
                {
                    btn = new Button();
                    btn.Click += DeleteContents_Click;
                    opt = new OperationInfo();
                    opt.Display = CurrentApp.GetLanguageInfo("3604T00011", "Delete Contents");
                    opt.Icon = "Images/Delete.png";
                    btn.DataContext = opt;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    ButDelete.Children.Add(btn);
                }

                btn = new Button();
                btn.Click += UploadResources_Click;
                opt = new OperationInfo();
                opt.Display = CurrentApp.GetLanguageInfo("3604T00034", "Upload Resources");
                opt.Icon = "Images/Upload.png";
                btn.DataContext = opt;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                ButMaterial.Children.Add(btn);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void RefreshTree(List<ContentsTree> contentsTree)
        {
            foreach (var tree in contentsTree)
            {
                _mLstContentsTreeParam.Add(tree);
                _mLstContentsNum = new List<long>();
                GetCategoryNum(tree.LongNodeId);

                _mContentsRootNode.Children.Clear();
                _mLstContentsTreeNodes.Clear();
                InitContentsTree(_mLstContentsTreeParam, 0, _mContentsRootNode);
            }
        }

        public void RefreshTree(ContentsTree contentsTree)
        {
            var temp = _mLstContentsTreeParam.FirstOrDefault(p => p.LongNodeId == contentsTree.LongNodeId);
            _mLstContentsTreeParam.Remove(temp);
            _mLstContentsTreeParam.Add(contentsTree);
            _mLstContentsNum = new List<long>();
            GetCategoryNum(contentsTree.LongNodeId); 

            _mLstSearchContentsNode.Clear();
            _mContentsRootNode.Children.Clear();
            _mLstContentsTreeNodes.Clear();
            InitContentsTree(_mLstContentsTreeParam, 0, _mContentsRootNode);
        }

        private void GetCategoryNum(long longCategoryNum)
        {
            if (longCategoryNum == 0)
            {
                return;
            }
            foreach (var categoryTree in _mLstContentsTreeParam)
            {
                if (categoryTree.LongNodeId == longCategoryNum)
                {
                    long lNum = categoryTree.LongParentNodeId;
                    GetCategoryNum(lNum);
                    _mLstContentsNum.Add(lNum);
                    return;
                }
            }
        }

        private List<ContentsTree> GetAllChildInfo(ContentsTree contentsTreeNode)
        {
            return _mLstContentsTreeParam.Count <= 0 ? null : _mLstContentsTreeParam.Where(param => param.LongParentNodeId == contentsTreeNode.LongNodeId).ToList();
        }

        private List<ContentsTree> GetAllNodeInfo(ContentsTree categoryTreeNode)
        {
            List<ContentsTree> lstContentsTreeNode = new List<ContentsTree>();
            if (_mLstContentsTreeParam.Count <= 0)
            {
                return null;
            }
            foreach (var categoryTree in _mLstContentsTreeParam)
            {
                if (categoryTreeNode.LongParentNodeId == categoryTree.LongParentNodeId)
                {
                    lstContentsTreeNode.Add(categoryTree);
                }
            }
            return lstContentsTreeNode;
        }

        #endregion

        #region Operations

        private void SetPanelVisible()
        {
            try
            {
                for (int i = 0; i < _mListPanels.Count; i++)
                {
                    var item = _mListPanels[i];
                    var panel =
                        PanelManager.Layout.Descendents()
                            .OfType<LayoutAnchorable>()
                            .FirstOrDefault(p => p.ContentId == item.ContentId);
                    if (panel != null)
                    {
                        panel.Title = CurrentApp.GetLanguageInfo("3604T00006", "Material Contents");

                        if (item.IsVisible)
                        {
                            panel.Show();
                        }
                        else
                        {
                            panel.Hide();
                        }
                        LayoutAnchorable panel1 = panel;
                        panel.IsVisibleChanged += (s, e) =>
                        {
                            item.IsVisible = panel1.IsVisible;
                            SetViewStatus();
                        };
                        panel.Closing += PanelDocument_Closing;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SetViewStatus()
        {
            for (int i = 0; i < PanelToolButton.Children.Count; i++)
            {
                var toggleBtn = PanelToolButton.Children[i] as ToggleButton;
                if (toggleBtn != null)
                {
                    var item = toggleBtn.DataContext as ToolButtonItem;
                    if (item == null) { continue; }
                    PanelItem panelItem = _mListPanels.FirstOrDefault(p => string.Format("TB{0}", p.Name) == item.Name);
                    if (panelItem != null)
                    {
                        toggleBtn.IsChecked = panelItem.IsVisible;
                    }
                }
            }
        }

        #endregion
    }
}
