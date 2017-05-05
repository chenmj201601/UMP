using Common11031;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using UMPS1103.Models;
using UMPS1103.Wcf11012;
using UMPS1103.Wcf11031;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS1103
{
    /// <summary>
    /// UCAgentMaintenance.xaml 的交互逻辑
    /// </summary>
    public partial class UCAgentMaintenance
    {
        #region Members

        private ObservableCollection<ViewColumnInfo> mListGridTreeColumns;
        private ObservableCollection<OperationInfo> mListOperations;
        private List<ObjectItem> mListCtlObjects;
        ObjectItem mRoot;
        public ObjectItem mCurrentItem;

        #endregion


        public UCAgentMaintenance()
        {
            InitializeComponent();
            mListCtlObjects = new List<ObjectItem>();
            mListOperations = new ObservableCollection<OperationInfo>();
            mListGridTreeColumns = new ObservableCollection<ViewColumnInfo>();
            mRoot = new ObjectItem();
            TreeViewOrgAgent.SelectedItemChanged += TreeViewOrgAgent_SelectedItemChanged;
            TreeViewOrgAgent.ItemsSource = mRoot.Children;
            TvObjects.SelectedItemChanged += TvObjects_SelectedItemChanged;
            TvObjects.ItemsSource = mRoot.Children;
        }

        #region Init and Load

        protected override void Init()
        {
            try
            {
                PageName = "UCAgentMaintenance";
                StylePath = "UMPS1103/UCAgentMaintenance.xaml";

                base.Init();

                ChangeTheme();
                ChangeLanguage();

                if (CurrentApp != null)
                {
                    //触发Loaded消息
                    CurrentApp.SendLoadedMessage();
                }

                mRoot.Children.Clear();
                mListCtlObjects.Clear();

                SetBusy(true, string.Empty);
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadOperations();
                  //  LoadColumnData();
                    LoadCtlOrg(mRoot, -1);
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();
                    SetBusy(false, string.Empty);
                    InitColumns();
                    CreatButton();
                    InitColumns();

                    if (mRoot.Children.Count > 0)
                    {
                        mRoot.Children[0].IsExpanded = true;
                    }
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadOperations()
        {
            try
            {
                mListOperations.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserOptList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("11");
                webRequest.ListData.Add("1103");
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                webReturn.ListData.Sort();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<OperationInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    OperationInfo roleInfo = optReturn.Data as OperationInfo;
                    if (roleInfo != null)
                    {
                        roleInfo.Display = CurrentApp.GetLanguageInfo("FO" + roleInfo.ID, roleInfo.Display);
                        mListOperations.Add(roleInfo);
                    }
                }
                CurrentApp.WriteLog("PageInit", string.Format("InitOperations"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadColumnData()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserViewColumnList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("1101001");
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("InitColumnData Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("No columns"));
                    return;
                }
                mListGridTreeColumns.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<ViewColumnInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("InitColumnData Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ViewColumnInfo column = optReturn.Data as ViewColumnInfo;
                    if (column != null) { mListGridTreeColumns.Add(column); }
                }

                CurrentApp.WriteLog("PageInit", string.Format("Init ViewColumn"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadCtlOrg(ObjectItem parentItem, long parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.WSGetUserCtlObjList;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(ConstValue.RESOURCE_ORG.ToString());
                webRequest.ListData.Add(parentID.ToString());
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("ListData is null"));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<ResourceObject>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ResourceObject obj = optReturn.Data as ResourceObject;
                    if (obj == null)
                    {
                        ShowException(string.Format("ResourceObject is null."));
                        return;
                    }

                    ObjectItem item = new ObjectItem();
                    item.Data = obj;
                    item.ObjID = obj.ObjID;
                    item.ObjType = ConstValue.RESOURCE_ORG;
                    item.Name = obj.Name;
                    string strFullName = obj.FullName;
                    if (string.IsNullOrEmpty(strFullName))
                    {
                        strFullName = obj.Name;
                    }
                    item.FullName = strFullName;
                    item.Description = strFullName;
                    if (item.ObjID == ConstValue.ORG_ROOT)
                    {
                        item.Icon = "Images/root.ico";
                    }
                    else
                    {
                        item.Icon = "Images/org.ico";
                    }
                    item.State = obj.State;

                    LoadCtlOrg(item, item.ObjID);
                    LoadCtlAgent(item, item.ObjID);

                    Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
                    mListCtlObjects.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadCtlAgent(ObjectItem parentItem, long parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.WSGetUserCtlObjList;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(ConstValue.RESOURCE_AGENT.ToString());
                webRequest.ListData.Add(parentID.ToString());
                webRequest.ListData.Add("2");
            //    Service11012Client client = new Service11012Client();
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("ListData is null"));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<ResourceObject>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ResourceObject obj = optReturn.Data as ResourceObject;
                    if (obj == null)
                    {
                        ShowException(string.Format("ResourceObject is null."));
                        return;
                    }
                    AgentInfo agentInfo = new AgentInfo();
                    agentInfo.SerialID = obj.ObjID;
                    agentInfo.AgentID = obj.Name;
                    agentInfo.AgentName = obj.FullName;
                    agentInfo.OrgID = parentID;
                    agentInfo.State = obj.State;
                    agentInfo.Tenure = obj.Tenure;


                    ObjectItem item = new ObjectItem();
                    item.Data = agentInfo;
                    item.ObjID = agentInfo.SerialID;
                    item.ObjType = ConstValue.RESOURCE_AGENT;
                    item.Name = agentInfo.AgentID;
                    string strFullName = agentInfo.AgentName;
                    if (string.IsNullOrEmpty(strFullName))
                    {
                        strFullName = obj.Name;
                    }
                    item.FullName = strFullName;
                    item.Description = strFullName;
                    //  item.Icon = "Images/agent.ico";
                    item.State = agentInfo.State;
                    if (item.State == 0)
                    {
                        item.Icon = "Images/agent.ico";
                    }else
                    {
                        item.Icon = "Images/agentforbid.ico";
                    }
                    item.AgentTenure = agentInfo.Tenure;

                    Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
                    mListCtlObjects.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion

        #region Create

        private void CreatButton()
        {
            try
            {
                PanelBasicOpts.Children.Clear();
                for (int i = 0; i < mListOperations.Count; i++)
                {
                    OperationInfo item = mListOperations[i];
                    //基本操作按钮
                    Button btn = new Button();
                    btn.Click += OptBtn_Click;
                    btn.DataContext = item;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    PanelBasicOpts.Children.Add(btn);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitColumns()
        {
            try
            {
                ViewColumnInfo column;
                int nameColumnWidth;
                GridViewColumn gvc;
                GridViewColumnHeader gvch;
                List<GridViewColumn> listColumns = new List<GridViewColumn>();
                column = mListGridTreeColumns.FirstOrDefault(c => c.ColumnName == "Name");
                gvch = new GridViewColumnHeader();
                gvch.Content = string.Empty;
                if (column != null)
                {
                    nameColumnWidth = column.Width;
                }
                else
                {
                    nameColumnWidth = 250;
                }

                gvc = new GridViewColumn();
                gvc.Header = CurrentApp.GetLanguageInfo("COL1103001", "State");
                gvc.Width = 80;
                DataTemplate objectStateTemplate = (DataTemplate)Resources["ObjectStateCellTemplate"];
                if (objectStateTemplate != null)
                {
                    gvc.CellTemplate = objectStateTemplate;
                }
                else
                {
                    gvc.DisplayMemberBinding = new Binding("State");
                }
                listColumns.Add(gvc);


                gvc = new GridViewColumn();
                gvc.Header = CurrentApp.GetLanguageInfo("COL1103002", "Full Name");
                gvc.Width = 250;
                DataTemplate fullNameTemplate = (DataTemplate)Resources["FullNameCellTemplate"];
                if (fullNameTemplate != null)
                {
                    gvc.CellTemplate = fullNameTemplate;
                }
                else
                {
                    gvc.DisplayMemberBinding = new Binding("FullName");
                }
                listColumns.Add(gvc);

                gvc = new GridViewColumn();
                gvc.Header = CurrentApp.GetLanguageInfo("COL1103003", "Description");
                gvc.Width = 300;
                DataTemplate descriptionTemplate = (DataTemplate)Resources["DescriptionCellTemplate"];
                if (descriptionTemplate != null)
                {
                    gvc.CellTemplate = descriptionTemplate;
                }
                else
                {
                    gvc.DisplayMemberBinding = new Binding("Description");
                }
                listColumns.Add(gvc);


                gvc = new GridViewColumn();
                gvc.Header = CurrentApp.GetLanguageInfo("COL1103004", "Lock");
                gvc.Width = 80;
                DataTemplate lockMethodTemplate = (DataTemplate)Resources["LockMethodCellTemplate"];
                if (lockMethodTemplate != null)
                {
                    gvc.CellTemplate = lockMethodTemplate;
                }
                else
                {
                    gvc.DisplayMemberBinding = new Binding("LockMethod");
                }
                listColumns.Add(gvc);

                DataTemplate nameColumnTemplate = (DataTemplate)Resources["NameColumnTemplate"];
                if (nameColumnTemplate != null)
                {
                    TvObjects.SetColumns(nameColumnTemplate, gvch, nameColumnWidth, listColumns);
                }
                else
                {
                    TvObjects.SetColumns(gvch, nameColumnWidth, listColumns);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion

        #region Operations

        private void ShowObjectDetail()
        {
            try
            {
                if (mCurrentItem == null) { return; }
                ObjectDetail.Title = mCurrentItem.Name;
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(string.Format("/UMPS1103;component/Themes/Default/UMPS1103/{0}", mCurrentItem.Icon), UriKind.Relative);
                image.EndInit();
                ObjectDetail.Icon = image;
                List<PropertyItem> listProperties = new List<PropertyItem>();
                PropertyItem property;
                switch (mCurrentItem.ObjType)
                {
                    case ConstValue.RESOURCE_ORG:
                        property = new PropertyItem();
                        property.Name = CurrentApp.GetLanguageInfo("S1103013", "name");
                        property.ToolTip = property.Name;
                        property.Value = mCurrentItem.Name;
                        listProperties.Add(property);
                        property = new PropertyItem();
                        property.Name = CurrentApp.GetLanguageInfo("S1103012", "bianhao");
                        property.ToolTip = property.Name;
                        property.Value = mCurrentItem.ObjID.ToString();
                        listProperties.Add(property);
                        break;
                    case ConstValue.RESOURCE_AGENT:
                        property = new PropertyItem();
                        property.Name = CurrentApp.GetLanguageInfo("S1103008", "name");
                        property.ToolTip = property.Name;
                        property.Value = mCurrentItem.Name;
                        listProperties.Add(property);
                        property = new PropertyItem();
                        property.Name = CurrentApp.GetLanguageInfo("S1103012", "bianma");
                        property.ToolTip = property.Name;
                        property.Value = mCurrentItem.ObjID.ToString();
                        listProperties.Add(property);
                        property = new PropertyItem();
                        property.Name = CurrentApp.GetLanguageInfo("S1103011", "状态");
                        property.ToolTip = property.Name;
                        property.Value = mCurrentItem.State.ToString();
                        if (property.Value == "1")
                        {
                            property.Value = CurrentApp.GetLanguageInfo("S1103009", "zheengcha");
                        }
                        else
                        {
                            property.Value = CurrentApp.GetLanguageInfo("S1103010", "jinyong");
                        }
                        listProperties.Add(property);
                        break;
                }
                ObjectDetail.ItemsSource = listProperties;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void AddAgent()
        {
            try
            {
                var item = TvObjects.SelectedItem as ObjectItem;
                if (item == null)
                {
                    item = TreeViewOrgAgent.SelectedItem as ObjectItem;
                    if (item == null) { return; }
                }
                if (item.ObjType != ConstValue.RESOURCE_ORG) { return; }

                UCAgentModify uc = new UCAgentModify();
                uc.CurrentApp = CurrentApp;
                uc.PageParent = this;
                uc.ParentItem = item;
                uc.IsModify = false;
                PopupPanel.Content = uc;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ModifyAgent()
        {
            try
            {
                var item = TvObjects.SelectedItem as ObjectItem;
                if (item == null)
                {
                    item = TreeViewOrgAgent.SelectedItem as ObjectItem;
                    if (item == null) { return; }
                }
                if (item.ObjType != ConstValue.RESOURCE_AGENT) { return; }

                UCAgentModify uc = new UCAgentModify();
                uc.CurrentApp = CurrentApp;
                uc.PageParent = this;
                uc.AgentItem = item;
                uc.IsModify = true;
                PopupPanel.Content = uc;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DeleteAgent()
        {
            try
            {
                var item = TvObjects.SelectedItem as ObjectItem;
                if (item == null)
                {
                    item = TreeViewOrgAgent.SelectedItem as ObjectItem;
                    if (item == null) { return; }
                }
                if (item.ObjType != ConstValue.RESOURCE_AGENT) { return; }
                var agentInfo = item.Data as AgentInfo;
                if (agentInfo == null) { return; }

                var result = MessageBox.Show(string.Format("Confirm delete agent?\r\n\r\n{0}", agentInfo.AgentID),
                    CurrentApp.AppTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes) { return; }

                OperationReturn optReturn;
                optReturn = XMLHelper.SeriallizeObject(agentInfo);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                string strInfo = optReturn.Data.ToString();

                int errCode = 0;
                string strMsg = string.Empty;
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    try
                    {
                        WebRequest webRequest = new WebRequest();
                        webRequest.Session = CurrentApp.Session;
                        webRequest.Code = (int)S1103Codes.ModifyAgent;
                        webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                        webRequest.ListData.Add("2");
                        webRequest.ListData.Add(strInfo);
                        Service11031Client client =
                            new Service11031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                                WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11031"));
                        WebReturn webReturn = client.DoOperation(webRequest);
                        client.Close();
                        if (!webReturn.Result)
                        {
                            errCode = webReturn.Code;
                            strMsg = webReturn.Message;
                            return;
                        }
                        if (webReturn.ListData == null
                            || webReturn.ListData.Count < 1)
                        {
                            errCode = Defines.RET_PARAM_INVALID;
                            strMsg = string.Format("WS return fail.");
                            return;
                        }
                        string strSerialID = webReturn.ListData[0];
                        CurrentApp.WriteLog("DeleteAgent", string.Format("End.\t{0}", strSerialID));
                    }
                    catch (Exception ex)
                    {
                        errCode = Defines.RET_FAIL;
                        strMsg = ex.Message;
                    }
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    if (errCode > 0)
                    {
                        ShowException(string.Format("{0}\t{1}", errCode, strMsg));
                        return;
                    }
                    ShowInformation(string.Format("Delete agent successful!"));

                    ReloadData();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void AgentMMT()
        {
            try
            {
                var item = TvObjects.SelectedItem as ObjectItem;
                if (item == null)
                {
                    item = TreeViewOrgAgent.SelectedItem as ObjectItem;
                    if (item == null) { return; }
                }
                if (item.ObjType != ConstValue.RESOURCE_AGENT) { return; }
                UCAgentMMT uc = new UCAgentMMT();
                uc.CurrentApp = CurrentApp;
                uc.PageParent = this;
                uc.AgentItem = item;
                PopupPanel.Content = uc;
                PopupPanel.IsOpen = true;

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void ReloadData()
        {
            try
            {
                mRoot.Children.Clear();
                mListCtlObjects.Clear();
                SetBusy(true, string.Empty);
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadCtlOrg(mRoot, -1);
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    SetBusy(false, string.Empty);
                    if (mRoot.Children.Count > 0)
                    {
                        mRoot.Children[0].IsExpanded = true;
                    }
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ModifyAgentObj()
        {
            try
            {
                var item = TvObjects.SelectedItem as ObjectItem;
                if (item == null)
                {
                    item = TreeViewOrgAgent.SelectedItem as ObjectItem;
                    if (item == null) { return; }
                }
                if (item.ObjType != ConstValue.RESOURCE_AGENT) { return; }

                UCModifyAgentObj ucMAO = new UCModifyAgentObj();
                ucMAO.CurrentApp = CurrentApp;
                ucMAO.PageParent = this;
                ucMAO.AgentItem = item;
                //   ucMAO.IsModify = false;
                PopupPanel.Content = ucMAO;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion

        #region Event Handlers

        private void TvObjects_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                var item = TvObjects.SelectedItem as ObjectItem;
                if (item == null) { return; }
                mCurrentItem = item;
                ShowObjectDetail();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void TreeViewOrgAgent_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                var item = TreeViewOrgAgent.SelectedItem as ObjectItem;
                if (item == null) { return; }
                mCurrentItem = item;
                ShowObjectDetail();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void OptBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = e.Source as Button;
                if (btn == null) { return; }
                var optItem = btn.DataContext as OperationInfo;
                if (optItem == null) { return; }
                switch (optItem.ID)
                {
                    case S1103Consts.OPT_ADDAGENT:
                        AddAgent();
                        break;
                    case S1103Consts.OPT_DELETEAGENT:
                        DeleteAgent();
                        break;
                    case S1103Consts.OPT_MODIFYAGNET:
                        ModifyAgent();
                        break;
                    case S1103Consts.OPT_AGENTMMT:
                        AgentMMT();
                        break;
                    case S1103Consts.OPT_MODIFYAGENTPASSWORD:
                        System.Windows.Forms.DialogResult rr = System.Windows.Forms.MessageBox.Show(string.Format(CurrentApp.GetLanguageInfo("S1103029", "确定还原该坐席密码吗？"), mCurrentItem.Name), CurrentApp.AppName, System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
                        if (rr == System.Windows.Forms.DialogResult.Yes)
                        {
                            ModifyAgentPassword();
                        }
                        break;
                    case S1103Consts.OPT_MODIFYAGENTOBJ:
                        ModifyAgentObj();
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

    

        private void ModifyAgentPassword()
        {
            GlobalParamInfo GlobalParam = new GlobalParamInfo();
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.WSGetGlobalParamList;
                webRequest.ListData.Add("11");
                webRequest.ListData.Add("11010501");
                webRequest.ListData.Add(string.Empty);

                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }

                OperationReturn optReturn = XMLHelper.DeserializeObject<GlobalParamInfo>(webReturn.ListData[0]);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("InitColumnData Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                GlobalParam = optReturn.Data as GlobalParamInfo;
                if (GlobalParam == null) { return; }

            }
            catch (Exception ex) { ShowException(ex.Message); }
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1103Codes.UPAgentPwd;
                webRequest.ListData.Add(mCurrentItem.ObjID.ToString());
                //  webRequest.ListData.Add(mCurrentItem.Name.ToString());
                webRequest.ListData.Add(GlobalParam.ParamValue.Substring(8));//新密码
                //  Service11031Client client = new Service11031Client();
                Service11031Client client = new Service11031Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11031"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (webReturn.Result)
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("S1103030", "Modify Sucessed"));
                    #region 写操作日志
                    string msg_success = string.Format("{0} {1} :{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString("FO1103004"), mCurrentItem.Name);
                    CurrentApp.WriteOperationLog("1103004", ConstValue.OPT_RESULT_SUCCESS, msg_success);
                    #endregion
                }
                else//失败
                {
                    ShowException(string.Format("{0}:{1}", CurrentApp.GetLanguageInfo("S1103031", webReturn.Message), webReturn.Message));
                    #region 写操作日志
                    string msg_Fail = string.Format("{0} {1} :{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString("FO1103004"), mCurrentItem.Name);
                    CurrentApp.WriteOperationLog("1103004".ToString(), ConstValue.OPT_RESULT_FAIL, msg_Fail);
                    #endregion
                }

            }
            catch (Exception ex) { ShowException(ex.Message); }
        }

        #endregion

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
                    string uri = string.Format("/UMPS1103;component/Themes/{0}/{1}",
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
                string uri = string.Format("/UMPS1103;component/Themes/Default/UMPS1103/UCAgentMaintenanceStatic.xaml");
                ResourceDictionary resource = new ResourceDictionary();
                resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                Resources.MergedDictionaries.Add(resource);
            }
            catch (Exception ex)
            {
                //ShowException("3" + ex.Message);
            }

            try
            {
                string uri = string.Format("/UMPS1103;component/Themes/Default/UMPS1103/MoreInfo.xaml");
                ResourceDictionary resource = new ResourceDictionary();
                resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                Resources.MergedDictionaries.Add(resource);
            }
            catch (Exception ex)
            {
                //ShowException("3" + ex.Message);
            }
        }

        #endregion


        #region ChangLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                CurrentApp.AppTitle = CurrentApp.GetLanguageInfo("FO1103", "UMP Agent Management");
                if (this.PopupPanel.IsOpen)
                {
                    this.PopupPanel.Title = CurrentApp.GetLanguageInfo("S1103005", "");
                    this.PopupPanel.ChangeLanguage();
                }
                this.TabSampleView.Header = CurrentApp.GetLanguageInfo("S1103101", "");
                this.TabGridView.Header = CurrentApp.GetLanguageInfo("S1103102", "");
                this.ExpBasicOpt.Header = CurrentApp.GetLanguageInfo("S1103027", "jichucaozuo");
                this.LabTreeHeard.Content = CurrentApp.GetLanguageInfo("S1103001", "zhubiaoti");

                InitColumns();
                CreatButton();
            }
            catch { }

        }

        #endregion

    }
}
