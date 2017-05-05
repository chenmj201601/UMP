using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UMPS4601.Models;
using UMPS4601.Wcf11012;
using UMPS4601.Wcf46011;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common4601;
using VoiceCyber.UMP.Communications;

namespace UMPS4601
{
    /// <summary>
    /// UC_PMParameterBindingPage.xaml 的交互逻辑
    /// </summary>
    public partial class UC_PMParameterBindingPage
    {

        #region Members
        private ObjectItem mRootItem_Org;
        private ObjectItem mRootItem_Skill;
        private BackgroundWorker mWorker;
        public string GroupingWay;//分组方式

        public ObservableCollection<ObjectItem> mListAllObjects;
        public ObjectItem mCurrentObjectItem;
        private KpiMapObjectInfoItem mCurrentKpiMapObjectInfoItem;

        public PMMainView ParentPage = null;
        private ObservableCollection<ViewColumnInfo> mListObjcetKPIDetailColumns;
        public static ObservableCollection<OperationInfo> ListOperations = new ObservableCollection<OperationInfo>();

        private List<KpiMapObjectInfo> mlistKpiMapObjectInfo;
        private ObservableCollection<KpiMapObjectInfoItem> mListKpiMapObjectInfoItem;
        #endregion

        public UC_PMParameterBindingPage()
        {
            InitializeComponent();
            mRootItem_Org = new ObjectItem();
            mRootItem_Skill = new ObjectItem();
            mListObjcetKPIDetailColumns = new ObservableCollection<ViewColumnInfo>();
            mlistKpiMapObjectInfo = new List<KpiMapObjectInfo>();
            mListKpiMapObjectInfoItem = new ObservableCollection<KpiMapObjectInfoItem>();
            mListAllObjects = new ObservableCollection<ObjectItem>();

          
            Loaded += UC_PMParameterBindingPageLoaded;
            TvOrg.SelectedItemChanged += TvOrg_SelectedItemChanged;
            TvSkillGroup.SelectedItemChanged += TvSkillGroup_SelectedItemChanged;
            LvRecordScoreDetail.SelectionChanged += LvRecordScoreDetail_SelectionChanged;
        }

        private void UC_PMParameterBindingPageLoaded(object sender, RoutedEventArgs e)
        {
            TvOrg.ItemsSource = mRootItem_Org.Children;
            TvSkillGroup.ItemsSource = mRootItem_Skill.Children;
            LvRecordScoreDetail.ItemsSource = mListKpiMapObjectInfoItem;
            mRootItem_Org.Children.Clear();
            mRootItem_Skill.Children.Clear();
            mWorker = new BackgroundWorker();
            mWorker.DoWork += (s, de) =>
            {
                LoadGroupingMethodParams();
                InitControlOrgs();
                InitControlSkillGroup();
                InitOperations();
                InitObjcetKPIDetailColumns();
            };
            mWorker.RunWorkerCompleted += (s, re) =>
            {
                mWorker.Dispose();
                CreateOptButtons();
                CreateObjcetKPIDetailColumns();
                ChangeLanguage();
            };
            mWorker.RunWorkerAsync();
        }

        #region 加载组织机构和技能组下的管理对象
        private void InitControlOrgs()
        {
            InitControlOrgs(mRootItem_Org, "-1");

            CurrentApp.WriteLog("PageLoad", string.Format("Load ControlObject"));
        }

        private void InitControlOrgs(ObjectItem parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4601Codes.GetControlOrgInfoList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                //Service46011Client client = new Service46011Client();
                Service46011Client client = new Service46011Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service46011"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 2) { continue; }
                    string strID = arrInfo[0];
                    string strName = arrInfo[1];
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_ORG;
                    item.ObjID = Convert.ToInt64(strID);
                    item.Name = strName;
                    item.Data = strInfo;
                    item.ParantID = strID;
                    if (strID == ConstValue.ORG_ROOT.ToString())
                    {
                        item.Icon = "Images/rootorg.ico";
                    }
                    else
                    {
                        item.Icon = "Images/org.ico";
                    }


                    mListAllObjects.Add(item);
                    InitControlOrgs(item, strID);
                    InitControlUsers(item, strID);
                    if (GroupingWay.IndexOf("R") >= 0)
                    {
                        InitControlRealExtensions(item, strID);
                    }
                    if (GroupingWay.IndexOf("E") >= 0)
                    {
                        InitControlExtensions(item, strID);
                    }
                    if (GroupingWay.IndexOf("A") >= 0)
                    {
                        InitControlAgents(item, strID);
                    }
                    AddChildObject(parentItem, item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitControlAgents(ObjectItem parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4601Codes.GetControlAgentInfoList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                //Service46011Client client = new Service46011Client();
                Service46011Client client = new Service46011Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service46011"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                //mListControlAgent = new List<string>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 3) { continue; }
                    string strID = arrInfo[0];
                    string strName = arrInfo[1];
                    string strFullName = arrInfo[2];
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_AGENT;
                    item.ObjID = Convert.ToInt64(strID);
                    item.Name = strName;
                    item.Description = strFullName;
                    item.Data = strInfo;
                    item.Icon = "Images/agent.ico";
                    item.ParantID = parentID;
                    mListAllObjects.Add(item);
                    AddChildObject(parentItem, item);
                    //mListControlAgent.Add(strName);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitControlExtensions(ObjectItem parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4601Codes.GetControlExtensionInfoList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                //Service46011Client client = new Service46011Client();
                Service46011Client client = new Service46011Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service46011"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 3) { continue; }
                    string strID = arrInfo[0];
                    string strIP = arrInfo[1];
                    string strName = arrInfo[2];
                    string strFullName = string.Empty;
                    ObjectItem item = new ObjectItem();
                    if (arrInfo.Length == 4)
                    {
                        strFullName = arrInfo[3];
                    }
                    item.ObjType = ConstValue.RESOURCE_EXTENSION;
                    item.ObjID = Convert.ToInt64(strID);
                    if (strFullName == string.Empty)
                    {
                        strFullName = strName;
                    }
                    item.Name = strName;
                    item.FullName = strIP;
                    item.Description = strFullName;
                    item.Data = strInfo;
                    item.Icon = "Images/extension.ico";
                    item.ParantID = parentID;
                    mListAllObjects.Add(item);
                    AddChildObject(parentItem, item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitControlRealExtensions(ObjectItem parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4601Codes.GetControlRealExtensionInfoList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                //Service46011Client client = new Service46011Client();
                Service46011Client client = new Service46011Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service46011"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 3) { continue; }
                    string strID = arrInfo[0];
                    string strName = arrInfo[1];
                    string strFullName = arrInfo[2];
                    //string strFullName = string.Empty;
                    ObjectItem item = new ObjectItem();
                    if (arrInfo.Length == 4)
                    {
                        strFullName = arrInfo[3];
                    }
                    item.ObjType = ConstValue.RESOURCE_REALEXT;
                    item.ObjID = Convert.ToInt64(strID);
                    if (strFullName == string.Empty)
                    {
                        strFullName = strName;
                    }
                    item.Name = strName;
                    item.FullName = strFullName;
                    item.Description = strName;
                    item.Data = strInfo;
                    item.Icon = "Images/RealExtension.ico";
                    item.ParantID = parentID;
                    mListAllObjects.Add(item);
                    AddChildObject(parentItem, item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitControlUsers(ObjectItem parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4601Codes.GetControlUserInfoList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                //Service46011Client client = new Service46011Client();
                Service46011Client client = new Service46011Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service46011"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 3) { continue; }
                    string strID = arrInfo[0];
                    string strName = arrInfo[1];
                    string strFullName = arrInfo[2];
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_USER;
                    item.ObjID = Convert.ToInt64(strID);
                    //用户全名 在界面上显示的
                    item.Name = strFullName;
                    //用户账号  不在界面上显示的
                    item.FullName = strName;
                    item.Data = strInfo;
                    item.Description = string.Format("{0}({1})", strFullName, strName);
                    item.Icon = "Images/user.ico";
                    item.ParantID = parentID;
                    mListAllObjects.Add(item);
                    AddChildObject(parentItem, item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadGroupingMethodParams()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetGlobalParamList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add("11");
                webRequest.ListData.Add("12010401");
                webRequest.ListData.Add("120104");
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                //List<GlobalParamInfo> listGlobalParam = new List<GlobalParamInfo>();

                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string str = webReturn.ListData[i];
                    str = str.Replace("&#x1B;", "");
                    OperationReturn optReturn = XMLHelper.DeserializeObject<GlobalParamInfo>(str);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    GlobalParamInfo GlobalParamInfo = optReturn.Data as GlobalParamInfo;
                    if (GlobalParamInfo == null) { return; }
                    string tempGroupWay = GlobalParamInfo.ParamValue.Substring(8);
                    //string tempIsScore = GlobalParamInfo.ParamValue.Substring(GlobalParamInfo.ParamValue.Length - 1, 1);
                    //ScoreParamsInfo tempThisClass = new ScoreParamsInfo();
                    //tempThisClass.Value = tempGroupWay;
                    //tempThisClass.Value = tempIsScore;
                    //mScoreParams.Add(tempThisClass);
                    GroupingWay = tempGroupWay;
                }
                //MessageBox.Show(GroupingWay);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitControlSkillGroup()
        {
            InitControlSkillGroup(mRootItem_Skill, "-1");
        }

        private void InitControlSkillGroup(ObjectItem parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4601Codes.GetControlSkillGroupInfoList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("-1");
                //Service46011Client client = new Service46011Client();
                Service46011Client client = new Service46011Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service46011"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                List<SkillGroupInfo> listSkillGroupInfo = new List<SkillGroupInfo>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<SkillGroupInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    SkillGroupInfo columnInfo = optReturn.Data as SkillGroupInfo;
                    if (columnInfo != null)
                    {
                        listSkillGroupInfo.Add(columnInfo);
                    }
                }

                for (int i = 0; i < listSkillGroupInfo.Count; i++)
                {
                    SkillGroupInfo strInfo = listSkillGroupInfo[i];
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_TECHGROUP;
                    item.Name = strInfo.SkillGroupName;
                    item.Description = strInfo.SkillGroupCode;
                    item.ObjID = Convert.ToInt64(strInfo.SkillGroupID);
                    item.Icon = "Images/SkillGroup.ico";
                    item.ParantID = strInfo.SkillGroupID;
                    mListAllObjects.Add(item);
                    InitControlObjectItemInSkillGroup(item, item.ObjID.ToString());
                    AddChildObject(parentItem, item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitControlObjectItemInSkillGroup(ObjectItem parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4601Codes.GetControlObjectInfoListInSkillGroup;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                webRequest.ListData.Add(GroupingWay);
                //Service46011Client client = new Service46011Client();
                Service46011Client client = new Service46011Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service46011"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    string strID = string.Empty;
                    string strIP = string.Empty;
                    string strName = string.Empty;
                    string strFullName = string.Empty;
                    ObjectItem item = new ObjectItem();
                    if (arrInfo.Length < 3) { continue; }
                    if (arrInfo.Length == 4)
                    {
                        strID = arrInfo[0];
                        strIP = arrInfo[1];
                        strName = arrInfo[2];
                        strFullName = arrInfo[3];
                        item.ObjID =Convert.ToInt64(strID);
                        item.FullName = strIP;
                        item.Description = strFullName;
                        item.Name = strName;
                        item.ObjType = ConstValue.RESOURCE_EXTENSION;
                        item.Icon = "Images/extension.ico";
                    }
                    else
                    {
                        strID = arrInfo[0];
                        strName = arrInfo[1];
                        strFullName = arrInfo[2];
                        if (strID.IndexOf("103") == 0)
                        {
                            item.ObjType = ConstValue.RESOURCE_AGENT;
                            item.ObjID = Convert.ToInt64(strID);
                            item.Name = strName;
                            item.Description = strFullName;
                            item.Data = strInfo;
                            item.Icon = "Images/agent.ico";
                        }
                        else 
                        {
                            item.ObjType = ConstValue.RESOURCE_REALEXT;
                            item.ObjID = Convert.ToInt64(strID);
                            item.Name = strName;
                            item.Description = strFullName;
                            item.Data = strInfo;
                            item.Icon = "Images/RealExtension.ico";
                        }
                    }
                    item.ParantID = parentID;
                    mListAllObjects.Add(item);
                    AddChildObject(parentItem, item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void AddChildObject(ObjectItem parentItem, ObjectItem item)
        {
            Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
        }
        #endregion

        #region 加载操作和列
        private void InitOperations()
        {
            try 
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserOptList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("46");
                webRequest.ListData.Add("4601");
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                ListOperations.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<OperationInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    OperationInfo optInfo = optReturn.Data as OperationInfo;
                    if (optInfo != null)
                    {
                        optInfo.Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", optInfo.ID), optInfo.ID.ToString());
                        optInfo.Description = optInfo.Display;
                        ListOperations.Add(optInfo);
                    }
                }
                CurrentApp.WriteLog("PageLoad", string.Format("Load Operation"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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
                btn = new Button();
                btn.Click += BasicOpt_Click;
                btn.DataContext = item;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                PanelBasicOpts.Children.Add(btn);
            }
        }

        private void InitObjcetKPIDetailColumns()
        {
            try 
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserViewColumnList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("4601001");
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                List<ViewColumnInfo> listColumns = new List<ViewColumnInfo>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<ViewColumnInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ViewColumnInfo columnInfo = optReturn.Data as ViewColumnInfo;
                    if (columnInfo != null)
                    {
                        columnInfo.Display = columnInfo.ColumnName;
                        if (columnInfo.ColumnName.Equals("StartTime") || columnInfo.ColumnName.Equals("StopTime"))
                        {
                            columnInfo.Visibility = "0";
                        }
                        listColumns.Add(columnInfo);
                    }
                }
                listColumns = listColumns.OrderBy(c => c.SortID).ToList();
                mListObjcetKPIDetailColumns.Clear();
                for (int i = 0; i < listColumns.Count; i++)
                {
                    mListObjcetKPIDetailColumns.Add(listColumns[i]);
                    //暂时
                }

                CurrentApp.WriteLog("PageLoad", string.Format("Load ObjcetKPIDetailColumn"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateObjcetKPIDetailColumns()
        {
            try 
            {
                GridView gv = new GridView();
                GridViewColumn gvc;
                GridViewColumnHeader gvch;
                for (int i = 0; i < mListObjcetKPIDetailColumns.Count; i++)
                {
                    ViewColumnInfo columnInfo = mListObjcetKPIDetailColumns[i];
                    if (columnInfo.Visibility == "1")
                    {
                        gvc = new GridViewColumn();
                        gvch = new GridViewColumnHeader();
                        gvch.Content = columnInfo.Display;
                        gvch.Content = CurrentApp.GetLanguageInfo(string.Format("COL4601001{0}", columnInfo.ColumnName),
                            columnInfo.Display);
                        gvch.ToolTip = CurrentApp.GetLanguageInfo(string.Format("COL4601001{0}", columnInfo.ColumnName),
                            columnInfo.Display);
                        gvc.Header = gvch;
                        gvc.Width = columnInfo.Width;
                        gvc.DisplayMemberBinding = new Binding(columnInfo.ColumnName);
                        if (columnInfo.ColumnName == "KPIName")
                        {
                            gvc.DisplayMemberBinding = new Binding("StrKPIName");
                        }
                        if (columnInfo.ColumnName == "ObjectType")
                        {
                            gvc.DisplayMemberBinding = new Binding("StrObjectType");
                        }
                        if (columnInfo.ColumnName == "IsActive")
                        {
                            gvc.DisplayMemberBinding = new Binding("StrIsActive");
                        }
                        if (columnInfo.ColumnName == "DropDown")
                        {
                            gvc.DisplayMemberBinding = new Binding("StrDropDown");
                        }
                        if (columnInfo.ColumnName == "ApplyAll")
                        {
                            gvc.DisplayMemberBinding = new Binding("StrApplyAll");
                        }
                        if (columnInfo.ColumnName == "ApplyCycle")
                        {
                            gvc.DisplayMemberBinding = new Binding("StrApplyCycle");
                        }
                        gv.Columns.Add(gvc);
                    }
                }
                LvRecordScoreDetail.View = gv;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


        #endregion

        #region EventHandlers
        private void BasicOpt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = e.Source as Button;
                if (btn != null)
                {
                    var optItem = btn.DataContext as OperationInfo;
                    if (optItem == null)
                    {
                        return;
                    }
                    switch (optItem.ID)
                    {
                        case S4601Consts.OPT_SEECONTENT://现在这个功能要改为修改这条记录的值KPI
                            if (mCurrentKpiMapObjectInfoItem == null)
                            {
                                ShowInformation(CurrentApp.GetLanguageInfo("4601TIP00007", "Please select a record"));
                                return;
                            }
                            OpenBandingPageToModify();
                            break;
                        case S4601Consts.OPT_BANDINGKPI:
                            if (mCurrentObjectItem == null)
                            {
                                ShowInformation(CurrentApp.GetLanguageInfo("4601TIP00008", "Please select a binding object"));
                                return;
                            }
                            OpenBandingPage();
                            break;
                        case S4601Consts.OPT_DELETEKPI:
                            if (mCurrentKpiMapObjectInfoItem == null)
                            {
                                ShowInformation(CurrentApp.GetLanguageInfo("4601TIP00007", "Please select a record"));
                                return;
                            }
                            DeleteKpiMapObjectInfo();
                            break;
                    }
                }
            }
            catch (Exception EX)
            {
                ShowException(EX.ToString());
            }
        }

        private void TvOrg_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //每次加载时 这个都先置为空
            mCurrentKpiMapObjectInfoItem = null;
            ObjectItem item = TvOrg.SelectedItem as ObjectItem;
            
            if (item != null)
            {
                mCurrentObjectItem = item;
                //需要加载选中的对象的列
                LbCurrentObject.Text = mCurrentObjectItem.Name;
                LoadKpiMapObjectInfo();
            }
        }

        private void TvSkillGroup_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            mCurrentKpiMapObjectInfoItem = null;
            ObjectItem item = TvSkillGroup.SelectedItem as ObjectItem;
            if (item != null)
            {
                mCurrentObjectItem = item;
                //不需要加载选中的对象的列
                LbCurrentObject.Text = mCurrentObjectItem.Name;
                LoadKpiMapObjectInfo();
            }
        }

        private void LvRecordScoreDetail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            KpiMapObjectInfoItem item = LvRecordScoreDetail.SelectedItem as KpiMapObjectInfoItem;
            if (item != null)
            {
                mCurrentKpiMapObjectInfoItem = item;
                LvRecordScoreDetail.SelectedItem = mCurrentKpiMapObjectInfoItem;
            }
        }

        #endregion

        #region   Operations
        private void OpenBandingPageToModify()
        {
            try 
            {
                PopupPanel.Title = "Banding Page";
                BandingPage bandingPage = new BandingPage();
                bandingPage.CurrentApp = CurrentApp;
                bandingPage.PageParent = this;
                bandingPage.CurrentKpiMapObjectInfoItem = mCurrentKpiMapObjectInfoItem;
                bandingPage.CurrentSelectObjectItem = mCurrentKpiMapObjectInfoItem.ObjectItem;
                bandingPage.IntoThisPageWay = "1";
                PopupPanel.Content = bandingPage;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void OpenBandingPage()
        {
            try
            {
                PopupPanel.Title = "Banding Page";
                BandingPage bandingPage = new BandingPage();
                bandingPage.CurrentApp = CurrentApp;
                bandingPage.PageParent = this;
                //加需要传入到绑定界面的内容
                bandingPage.CurrentSelectObjectItem = mCurrentObjectItem;
                bandingPage.IntoThisPageWay = "0";
                PopupPanel.Content = bandingPage;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void LoadKpiMapObjectInfo()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S4601Codes.LoadKpiMapObjectInfo;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(mCurrentObjectItem.ObjID.ToString());//对象ID
                webRequest.ListData.Add(mCurrentObjectItem.ParantID.ToString());//对象的ParantID
                Service46011Client client = new Service46011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service46011"));
                //Service46011Client client = new Service46011Client();
                WebReturn webReturn = client.DoOperation(webRequest);
                 client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                mlistKpiMapObjectInfo.Clear();
                mListKpiMapObjectInfoItem.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<KpiMapObjectInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    KpiMapObjectInfo kpiInfo = optReturn.Data as KpiMapObjectInfo;
                    if (kpiInfo != null)
                    {
                        mlistKpiMapObjectInfo.Add(kpiInfo);
                    }
                    KpiMapObjectInfoItem item = new KpiMapObjectInfoItem(kpiInfo);
                    //新增的属性 每个绑定的数据源里都有一个对象 然后就将选中的对象赋过去
                    item.ObjectItem = mCurrentObjectItem;
                    mListKpiMapObjectInfoItem.Add(item);
                }
                for (int i = 0; i < mListKpiMapObjectInfoItem.Count; i++)
                {
                    KpiMapObjectInfoItem item = mListKpiMapObjectInfoItem[i];
                    item.StrKPIName = CurrentApp.GetLanguageInfo(string.Format("4601KPI{0}", item.KpiMapObjectInfo.KpiID), item.KPIName.ToString());
                    item.StrApplyCycle = CurrentApp.GetLanguageInfo(string.Format("4601BP{0}", item.ApplyCycle), item.ApplyCycle.ToString());
                    item.StrIsActive = CurrentApp.GetLanguageInfo(string.Format("4601IsActive{0}", item.IsActive), item.IsActive.ToString());
                    item.StrDropDown = CurrentApp.GetLanguageInfo(string.Format("4601BPDropDown{0}", item.DropDown), item.DropDown.ToString());
                    item.StrApplyAll = CurrentApp.GetLanguageInfo(string.Format("4601BPApplyAll{0}", item.ApplyAll), item.ApplyAll.ToString());
                    if (Regex.Matches(item.ObjectType, @"1").Count > 1)//这个字符串中1的个数大于1
                    {
                        item.StrObjectType = string.Empty;
                        if (item.ObjectType.Substring(0, 1) == "1")
                        {
                            item.StrObjectType += CurrentApp.GetLanguageInfo(string.Format("4601BPOBJ{0}", "1000000000"), "1000000000")+"  ";
                        }
                        if (item.ObjectType.Substring(1, 1) == "1")
                        {
                            item.StrObjectType += CurrentApp.GetLanguageInfo(string.Format("4601BPOBJ{0}", "0100000000"), "0100000000")+"  ";
                        }
                        if (item.ObjectType.Substring(2, 1) == "1")
                        {
                            item.StrObjectType += CurrentApp.GetLanguageInfo(string.Format("4601BPOBJ{0}", "0010000000"), "0010000000") + "  ";
                        }
                        if (item.ObjectType.Substring(3, 1) == "1")
                        {
                            item.StrObjectType += CurrentApp.GetLanguageInfo(string.Format("4601BPOBJ{0}", "0001000000"), "0001000000") + "  ";
                        }
                    }
                    else 
                    {
                        item.StrObjectType = CurrentApp.GetLanguageInfo(string.Format("4601BPOBJ{0}", item.ObjectType), item.ObjectType.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
        
        private void DeleteKpiMapObjectInfo()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S4601Codes.DeleteKpiMapObjectInfo;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(mCurrentKpiMapObjectInfoItem.KpiMapObjectInfo.KpiMappingID);//对象ID
                Service46011Client client = new Service46011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service46011"));
                //Service46011Client client = new Service46011Client();
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                else 
                {
                    mListKpiMapObjectInfoItem.Remove(mCurrentKpiMapObjectInfoItem);
                    ShowInformation(CurrentApp.GetLanguageInfo("4601TIP00001", "chenggong"));
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
        #endregion

        #region ChangeLanguage
        public override void ChangeLanguage()
        {
            try 
            {
                base.ChangeLanguage();
                //Operation
                for (int i = 0; i < ListOperations.Count; i++)
                {
                    ListOperations[i].Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", ListOperations[i].ID), ListOperations[i].ID.ToString());
                }
                CreateOptButtons();

                 // Other
                tabOrg.Header = CurrentApp.GetLanguageInfo("46010001", "Organizition");
                tabSkill.Header = CurrentApp.GetLanguageInfo("46010002", "Skill Group");

                //Columns
                CreateObjcetKPIDetailColumns();

                //记录列(ListView)中的内容
                for (int i = 0; i < mListKpiMapObjectInfoItem.Count; i++)
                {
                    KpiMapObjectInfoItem item = mListKpiMapObjectInfoItem[i];
                    item.StrKPIName = CurrentApp.GetLanguageInfo(string.Format("4601KPI{0}", item.KpiMapObjectInfo.KpiID), item.KPIName.ToString());
                    item.StrApplyCycle = CurrentApp.GetLanguageInfo(string.Format("4601BP{0}", item.ApplyCycle), item.ApplyCycle.ToString());
                    item.StrIsActive = CurrentApp.GetLanguageInfo(string.Format("4601IsActive{0}", item.IsActive), item.IsActive.ToString());
                    item.StrDropDown = CurrentApp.GetLanguageInfo(string.Format("4601BPDropDown{0}", item.DropDown), item.DropDown.ToString());
                    item.StrApplyAll = CurrentApp.GetLanguageInfo(string.Format("4601BPApplyAll{0}", item.ApplyAll), item.ApplyAll.ToString());
                    item.StrObjectType = CurrentApp.GetLanguageInfo(string.Format("4601BPOBJ{0}", item.ObjectType), item.ObjectType.ToString());
                }

                PopupPanel.ChangeLanguage();
            }
            catch (Exception ex)
            {
 
            }
        }
        #endregion


    }
}
