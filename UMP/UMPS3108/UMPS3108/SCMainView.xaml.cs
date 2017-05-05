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
using UMPS3108.Common31081;
using UMPS3108.Models;
using UMPS3108.Wcf11012;
using UMPS3108.Wcf31081;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31081;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS3108
{
    /// <summary>
    /// SCMainPage.xaml 的交互逻辑
    /// </summary>
    public partial class SCMainView
    {
        #region Members

        //这个变量放统计参数大项的列表
        public ObservableCollection<StatisticalParam> mListStatisticalParam;

        private BackgroundWorker mWorker;

        private ObjectItem mRoot;
        private List<ObjectItem> mListObjectItems;
        //private ObjectItem objectItem;
        private ObjectItem objectItemTree;
        private List<OperationInfo> ListOpt;
        private ObservableCollection<OperationInfo> ListOperation;

        public List<StatisticalParamItemDetail> mListStatisticalParamItemDetails_;
        #endregion

        #region 3106
        private ObservableCollection<string> mListOperations = new ObservableCollection<string>();

        private List<QualityParam> ListQP = new List<QualityParam>();

        private List<string> AuthorityList = new List<string>();
        private QualityParamConfig QPCPage = new QualityParamConfig();
        private QualityParamTimeConfig QPTCPage = new QualityParamTimeConfig();
        CombStatiParaItemsDesigner mSonPage;
        private OrgPage Orgpage;
        public ParamsItemsConfigPage paramsItemsconfigPage = new ParamsItemsConfigPage();
        //public CombStatiParaItemsDesigner combStatiParaItemsDesigner = new CombStatiParaItemsDesigner();
        #endregion

        public SCMainView()
        {
            InitializeComponent();
            paramsItemsconfigPage.CurrentApp = CurrentApp;
            QPCPage.CurrentApp = CurrentApp;
            mListStatisticalParam = new ObservableCollection<StatisticalParam>();
            Orgpage = new OrgPage();
            Orgpage.CurrentApp = CurrentApp;
            mRoot = new ObjectItem();
            mListObjectItems = new List<ObjectItem>();
            //objectItem = new ObjectItem();
            objectItemTree = new ObjectItem();
            ListOpt = new List<OperationInfo>();
            ListOperation = new ObservableCollection<OperationInfo>();

            TvSample.ItemsSource = mRoot.Children;

            TvSample.SelectedItemChanged += TreeView_SelectedItemChanged;
        }

        #region Init and Load

        protected override void Init()
        {
            try
            {
                PageName = "SCMainView";
                StylePath = "UMPS3108/SCMainPage.xaml";

                base.Init();
                mListObjectItems.Clear();
                mListStatisticalParamItemDetails_ = new List<StatisticalParamItemDetail>();
                //ChangeTheme();
                //ChangeLanguage();

                SetBusy(true, CurrentApp.GetMessageLanguageInfo("001", string.Format("Loading data, please wait...")));
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    //触发Loaded消息
                    CurrentApp.SendLoadedMessage();

                    InitOperation();
                    InitStatisticalParam();
                    InitQualityParam();
                    InitParam();

                    CurrentApp.WriteLog("PageLoad", string.Format("All data loaded"));
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    SetBusy(false, string.Empty);

                    InitOperation();
                    LoadTree(mRoot, "0", 0);

                    ChangeLanguage();
                    CurrentApp.WriteLog(string.Format("Load\t\tPage load end"));
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion

        #region operations

        private void Layout()
        {
            PopupPanel.Title = CurrentApp.GetLanguageInfo("FO3108001", "Design");
            mSonPage = new CombStatiParaItemsDesigner();
            mSonPage.CurrentApp = CurrentApp;
            mSonPage.PageParent = this;
            PopupPanel.Content = mSonPage;
            PopupPanel.IsOpen = true;
        }

        private void AddStatistic()
        {
            if (Orgpage != null)
            {
                if (Orgpage.ObjItem != null)
                    if (Orgpage.ObjItem.ItemID != null && Orgpage.ObjItem.ItemID.Length > 3)
                    {
                        //保存机构和技能组
                        ABCDConfigPage abcdPage = new ABCDConfigPage();
                        abcdPage.CurrentApp = CurrentApp;
                        this.PopupPanel.Title = CurrentApp.GetLanguageInfo("310802P001", "KPI Design");
                        abcdPage.ParentPage = this.Orgpage;
                        abcdPage.PageGrandParent = this;
                        abcdPage.OrgItem = Orgpage.ObjItem;
                        abcdPage.OperationCode = Orgpage.OparetionCode;
                        this.PopupPanel.Content = abcdPage;
                        this.PopupPanel.IsOpen = true;
                    }
            }
        }

        private void DeleteStatistic()
        {
            if (Orgpage.DeleteStatisticParam != null && Orgpage.ObjItem != null)
            {
                if (Orgpage.DeleteStatisticParamFromDB())
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("", "成功"));
                }
                else
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("", "失败"));
                }
            }
        }

        private void SaveConfig()
        {
            switch (objectItemTree.ObjParentID)
            {
                case "101":
                    //点击保存。获取设置的内容，保存到数据库 
                    List<QualityParam> ListQPTemp = new List<QualityParam>();
                    if (objectItemTree.ItemID == "31080101004")
                    {
                        QualityParamTimeConfig UCPage = PanelPropertyList.Child as QualityParamTimeConfig;
                        UCPage.CurrentApp = CurrentApp;
                        if (UCPage == null)
                        { return; }
                        if (!UCPage.CollectionData()) { return; }
                        ListQPTemp = UCPage.Display;
                    }
                    else
                    {
                        QualityParamConfig UCPage = PanelPropertyList.Child as QualityParamConfig;
                        UCPage.CurrentApp = CurrentApp;
                        if (UCPage == null)
                        { return; }
                        UCPage.CollectionData();
                        ListQPTemp = UCPage.Display;
                    }
                    //UCPage.CurrentApp = CurrentApp;
                    //if (UCPage == null)
                    //{ return; }
                    //UCPage.CollectionData();
                    //ListQPTemp.Add(UCPage.Display1);
                    //ListQPTemp.Add(UCPage.Display2);
                    if (SaveParam(ListQPTemp))
                    {
                        ShowInformation(string.Format("{0}{1}", objectItemTree.Name, CurrentApp.GetLanguageInfo("3108T001", "保存成功")));
                        #region 记录日志
                        string msg = string.Format("{0}{1}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO310800{0}", objectItemTree.ItemID.Substring(10))));
                        CurrentApp.WriteOperationLog(string.Format("310800{0}", objectItemTree.ItemID.Substring(10)), ConstValue.OPT_RESULT_SUCCESS, msg);
                        #endregion
                    }
                    else
                    {
                        ShowInformation(string.Format("{0}{1}", objectItemTree.Name, CurrentApp.GetLanguageInfo("3108T002", "保存失败")));
                        #region 记录日志
                        string msg = string.Format("{0}{1}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO310800{0}", objectItemTree.ItemID.Substring(10))));
                        CurrentApp.WriteOperationLog(string.Format("310800{0}", objectItemTree.ItemID.Substring(10)), ConstValue.OPT_RESULT_FAIL, msg);
                        #endregion
                    }
                    break;
                case "102":
                    if (paramsItemsconfigPage != null)
                    {
                        bool aaa = paramsItemsconfigPage.CreateConditionResultList();
                        mListStatisticalParamItemDetails_.Clear();
                        if (paramsItemsconfigPage.ListStatisticalParamItemDetails == null)
                        {
                            return;
                        }
                        for (int i = 0; i < paramsItemsconfigPage.ListStatisticalParamItemDetails.Count; i++)
                        {
                            mListStatisticalParamItemDetails_.Add(paramsItemsconfigPage.ListStatisticalParamItemDetails[i]);
                        }
                        paramsItemsconfigPage.ModifyStatisticParam();
                        OperationReturn result = new OperationReturn();
                        result = SaveValueToDB();
                        if (result.Result == false || aaa == false)
                        {
                            ShowInformation(CurrentApp.GetLanguageInfo("3108T002", "保存失败"));
                            #region 记录日志
                            string msg = string.Format("{0}{1}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO3108005")));
                            CurrentApp.WriteOperationLog(string.Format("3108005"), ConstValue.OPT_RESULT_FAIL, msg);
                            #endregion
                        }
                        else
                        {
                            #region 记录日志
                            string msg = string.Format("{0}{1}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO3108005")));
                            CurrentApp.WriteOperationLog(string.Format("3108005"), ConstValue.OPT_RESULT_SUCCESS, msg);
                            #endregion
                            ShowInformation(CurrentApp.GetLanguageInfo("3108T001", "保存成功"));
                        }
                    }
                    break;
            }
        }

        #endregion
        //导入统计配置参数大项,从T_31_050
        private void InitStatisticalParam()
        {
            try
            {
                mListStatisticalParam.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3108Codes.GetStatisticalParam;
                webRequest.Session = CurrentApp.Session;
                Service31081Client client = new Service31081Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31081"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("InitStatisticalParam:Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<StatisticalParam>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    StatisticalParam SPInfo = optReturn.Data as StatisticalParam;
                    string code = (SPInfo.StatisticalParamID - 3110000000000000000).ToString();
                    //mListObjectItems.FirstOrDefault(s => s.ObjType == 3 && s.ObjParentID == "102" && s.ObjID == "10200" + code);
                    ObjectItem objItem = new ObjectItem();
                    objItem.ObjID = string.Format("10200{0}", code);
                    objItem.ObjParentID = "102";
                    objItem.ObjType = 3;
                    objItem.ItemID = string.Format("3108010200{0}", code);
                    //if (objItem == null) { return; }
                    objItem.Data = SPInfo;
                    //objItem.Name = SPInfo.StatisticalParamName;
                    //objItem.Description = SPInfo.StatisticalParamName;
                    objItem.Name = CurrentApp.GetLanguageInfo(string.Format("FO{0}", objItem.ItemID), SPInfo.StatisticalParamName);
                    objItem.Description = CurrentApp.GetLanguageInfo(string.Format("3108D{0}", objItem.ItemID), SPInfo.StatisticalParamName);
                    mListObjectItems.Add(objItem);
                    if (SPInfo != null)
                    {
                        mListStatisticalParam.Add(SPInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #region 3106
        private void InitOperation()
        {
            //this.OptStaPanel.Children.Clear();
            ListOpt.Clear();
            OperationInfo item;
            //保存按钮
            item = new OperationInfo();
            item.ID = 1;
            item.Icon = "Images/save.png";
            item.Display = CurrentApp.GetLanguageInfo("3108B001", "Save");
            ListOpt.Add(item);

            item = new OperationInfo();
            item.ID = 2;
            item.Icon = "Images/setting.png";
            item.Display = CurrentApp.GetLanguageInfo("310802B001", "SET");
            ListOpt.Add(item);

            item = new OperationInfo();
            item.ID = 3;
            item.Icon = "Images/setting.png";
            item.Display = CurrentApp.GetLanguageInfo("310802B003", "DELETE");
            ListOpt.Add(item);

            item = new OperationInfo();
            item.ID = 4;
            item.Icon = "Images/savelayout.png";
            item.Display = CurrentApp.GetLanguageInfo("310801B003", "LayOut");
            ListOpt.Add(item);
        }

        private void InitBasicOperations()
        {
            ListOperation.Clear();
            if (objectItemTree != null)
            {
                if (objectItemTree.ObjType == 3)
                {
                    ListOperation.Add(ListOpt.FirstOrDefault(p => p.ID == 1));//质检参数配置保存
                    if (objectItemTree.ItemID == "31080102001" || objectItemTree.ItemID == "31080102002")//多多的保存和布局
                    {
                        ListOperation.Add(ListOpt.FirstOrDefault(p => p.ID == 4));
                    }
                }
                else if (objectItemTree.ObjType == 2 && objectItemTree.ObjParentID == "2")//我这边的添加和删除
                {
                    ListOperation.Add(ListOpt.FirstOrDefault(p => p.ID == 2));
                    ListOperation.Add(ListOpt.FirstOrDefault(p => p.ID == 3));
                }
            }
            CreatButton();
        }

        private void CreatButton()
        {
            this.OptStaPanel.Children.Clear();
            for (int i = 0; i < ListOperation.Count; i++)
            {
                OperationInfo item = ListOperation[i];
                //基本操作按钮
                Button btn = new Button();
                btn.Click += ButtonRight_Click;
                btn.DataContext = item;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                OptStaPanel.Children.Add(btn);
            }
        }

        //加载该用户3106的权限3108
        private void InitParam()
        {
            try
            {
                WebRequest webRequest = new WebRequest();

                webRequest.Code = (int)S3108Codes.GetAuthorityParam;
                webRequest.Session = CurrentApp.Session;
                Service31081Client client = new Service31081Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31081"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("InitParam:WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    //OperationReturn optReturn = XMLHelper.DeserializeObject<OperationInfo>(webReturn.ListData[i]);
                    //if (!optReturn.Result)
                    //{
                    //    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    //    return;
                    //}
                    //OperationInfo SPInfo = optReturn.Data as OperationInfo;
                    //string Code = SPInfo.ID.ToString();
                    string Code = webReturn.ListData[i];
                    if (Code.Length > 4)
                    {
                        ObjectItem item = new ObjectItem();
                        //Code = webReturn.ListData[i];
                        item.ItemID = Code;
                        item.Name = CurrentApp.GetLanguageInfo(string.Format("FO{0}", Code), Code);
                        item.Description = CurrentApp.GetLanguageInfo(string.Format("3108D{0}", Code), Code);
                        Code = Code.Substring(4);
                        item.ObjID = Code.Substring(1);
                        switch (Code.Count())
                        {
                            case 2:
                                item.ObjType = 1;
                                item.ObjParentID = "0";
                                item.Data = item;
                                break;
                            case 4:
                                item.ObjType = 2;
                                item.ObjParentID = Code.Substring(1, 1);
                                item.Data = item;
                                break;
                            case 7:
                                item.ObjType = 3;
                                item.ObjParentID = Code.Substring(1, 3);
                                QualityParam tempQP = ListQP.Find(p => p.ParentTreeID == item.ItemID);
                                item.Data = tempQP;
                                break;
                            default:
                                break;
                        }
                        mListObjectItems.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitQualityParam()
        {
            ListQP.Clear();
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3108Codes.GetQualityParam;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add("31");
                webRequest.ListData.Add("0");
                webRequest.ListData.Add("310101");
                Service31081Client client = new Service31081Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31081"));
                //Service31061Client client = new Service31061Client();
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("InitQualityParam:WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                List<QualityParam> listGlobalParam = new List<QualityParam>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<QualityParam>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    QualityParam GlobalParamInfo = optReturn.Data as QualityParam;
                    //将获取的数据一条条加入ListQP,并且获取name和description
                    string LanguageCode = string.Empty; string GroupCode = string.Empty;
                    GetGroupID(GlobalParamInfo.GroupID.ToString().Substring(4), out GroupCode);
                    GetParamID(GlobalParamInfo.GroupID.ToString().Substring(4), out LanguageCode);

                    LanguageCode += GlobalParamInfo.SortID.ToString();
                    GlobalParamInfo.Name = CurrentApp.GetLanguageInfo(string.Format("3108{0}", LanguageCode), "");
                    GlobalParamInfo.Description = CurrentApp.GetLanguageInfo(string.Format("3108D{0}", LanguageCode), "");
                    switch (GlobalParamInfo.GroupID - 310100)
                    {
                        case 1:
                            GlobalParamInfo.ParentTreeID = "31080101001";
                            break;
                        case 2:
                            GlobalParamInfo.ParentTreeID = "31080101002";
                            break;
                        case 3:
                            GlobalParamInfo.ParentTreeID = "31080101003";
                            break;
                        case 4:
                            GlobalParamInfo.ParentTreeID = "31080101004";
                            break;
                    }
                    ListQP.Add(GlobalParamInfo);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadTree(ObjectItem Item, string parent, int flag)
        {
            flag++;
            List<ObjectItem> temp = (from item in mListObjectItems
                                     where item.ObjType == flag && item.ObjParentID == parent
                                     orderby item.ObjID
                                     select item).ToList<ObjectItem>();

            for (int i = 0; i < temp.Count; i++)
            {
                ObjectItem item = temp[i];
                AddChildObjectItem(Item, item);
                LoadTree(item, item.ObjID, flag);
            }
        }

        private bool SaveParam(List<QualityParam> ListQP)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3108Codes.SaveQualityParam;
                webRequest.Session = CurrentApp.Session;

                for (int i = 0; i < ListQP.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.SeriallizeObject<QualityParam>(ListQP[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return false;
                    }
                    webRequest.ListData.Add(optReturn.Data.ToString());
                }

                Service31081Client client = new Service31081Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31081"));
                //Service31061Client client = new Service31061Client();
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));

                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                ShowException(ex.ToString());

                return false;
            }
        }

        void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            objectItemTree = this.TvSample.SelectedItem as ObjectItem;
            string Code = objectItemTree.ItemID;
            string tempDescription = string.Format("{0}:\r\n", objectItemTree.Name);
            TxtDescription.LineHeight = 30;
            InitBasicOperations();
            switch (Code)
            {
                //查询质检参数配置++-
                case "31080101001":
                case "31080101002":
                case "31080101003":
                    List<QualityParam> QPTempList = (from LQP in ListQP
                                                     where LQP.ParentTreeID == objectItemTree.ItemID
                                                     orderby LQP.SortID
                                                     select LQP).ToList();
                    if (QPTempList.Count < 2) { return; }
                    CreatePropertyList(QPTempList);
                    //TxtDescription.Text = string.Format("  {0}:\n\n  {1}\n\n  {2}", objectItemTree.Name, QPTempList[0].Description, QPTempList[1].Description);
                    foreach (QualityParam info in QPTempList)
                    {
                        tempDescription += string.Format("{0}\r\n",info.Description);
                    }
                    TxtDescription.Text = tempDescription;
                    break;
                case "31080101004":
                    List<QualityParam> QPTTempList = (from LQP in ListQP
                                                      where LQP.ParentTreeID == objectItemTree.ItemID
                                                      orderby LQP.SortID
                                                      select LQP).ToList();
                    if (QPTTempList.Count < 2) { return; }
                    CreateTimePropertyList(QPTTempList);
                    //TxtDescription.Text = string.Format("  {0}:\n\n  {1}\n\n  {2}", objectItemTree.Name, QPTTempList[0].Description, QPTTempList[1].Description);
                    foreach (QualityParam info in QPTTempList)
                    {
                        tempDescription += string.Format("{0}\r\n",info.Description);
                    }
                    TxtDescription.Text = tempDescription;
                    break;
                //abcd参数配置
                case "31080102001":
                case "31080102002":
                    //CreatButton();
                    OpenABCD();
                    TxtDescription.Text = "\n  " + objectItemTree.Description;
                    break;
                case "":
                    break;
                //机构和技能组
                case "31080201":
                case "31080202":
                    //CreatButton();
                    if (objectItemTree == null) { return; }
                    Orgpage.Mainpage = this;
                    Orgpage.CurrentApp = CurrentApp;
                    Orgpage.OparetionCode = Code;
                    TxtDescription.Text = "\n  " + objectItemTree.Description;
                    if (PanelPropertyList.Child != null)
                    {
                        Orgpage.LoadPage();
                    }
                    PanelPropertyList.Child = Orgpage;
                    break;
                default:
                    if (Code.Length > 8)
                    {
                        OpenABCD();
                        TxtDescription.Text = "\n  " + objectItemTree.Description;
                    }
                    break;
            }

        }

        private void OpenABCD()
        {
            try
            {
                if (objectItemTree == null) { return; }
                paramsItemsconfigPage = new ParamsItemsConfigPage();
                paramsItemsconfigPage.CurrentApp = CurrentApp;
                StatisticalParam SelectedStatisticalParam = objectItemTree.Data as StatisticalParam;
                paramsItemsconfigPage.StatisticalParam = SelectedStatisticalParam;
                ParamsItemsConfigPage.StatisticalParam_ = SelectedStatisticalParam;
                this.PanelPropertyList.Child = null;
                paramsItemsconfigPage.SCParent = this;
                this.PanelPropertyList.Child = paramsItemsconfigPage;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }

        }

        void ButtonRight_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            if (btn != null)
            {
                var optItem = btn.DataContext as OperationInfo;
                if (optItem != null)
                {
                    switch (optItem.ID)
                    {
                        case 1:
                            SaveConfig();
                            break;
                        case 2:
                            AddStatistic();
                            break;
                        case 3:
                            DeleteStatistic();
                            break;
                        case 4:
                            if (paramsItemsconfigPage.DesignAndSaveIsEnabled == false)
                            {
                                ShowInformation(CurrentApp.GetLanguageInfo("3108T005", "该参数已经绑定到了机构或者技能组,不能设计"));
                                break;
                            }
                            Layout();
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        //bord
        private void CreatePropertyList(List<QualityParam> QPTempList)
        {
            try
            {
                PanelPropertyList.Child = null;
                if (objectItemTree == null) { return; }

                QPCPage.Display = QPTempList;
                QPCPage.mainPage = this;
                QPCPage.SetCheck();
                PanelPropertyList.Child = QPCPage;
                QPCPage.ChangeLanguage();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateTimePropertyList(List<QualityParam> QPTempList)
        {
            try
            {
                PanelPropertyList.Child = null;
                if (objectItemTree == null) { return; }

                QPTCPage.Display = QPTempList;
                QPTCPage.mainPage = this;
                QPTCPage.SetCheck();
                PanelPropertyList.Child = QPTCPage;
                QPTCPage.ChangeLanguage();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
        private void GetGroupID(string Temp, out string GroupID)
        {
            switch (Temp)
            {
                case "01":
                    GroupID = "001";
                    break;
                case "02":
                    GroupID = "002";
                    break;
                case "03":
                    GroupID = "003";
                    break;
                default:
                    GroupID = "";
                    break;
            }
        }

        private void GetParamID(string Temp, out string ParamID)
        {
            switch (Temp)
            {
                case "01":
                    ParamID = "11";
                    break;
                case "02":
                    ParamID = "12";
                    break;
                case "03":
                    ParamID = "13";
                    break;
                default:
                    ParamID = "";
                    break;
            }
        }

        #endregion 3106end

        #region Others

        private void ClearChildren(ObjectItem item)
        {
            if (item == null) { return; }
            for (int i = 0; i < item.Children.Count; i++)
            {
                var child = item.Children[i] as ObjectItem;
                if (child != null)
                {
                    var temp = mListObjectItems.FirstOrDefault(j => j.ObjID == child.ObjID);
                    if (temp != null) { mListObjectItems.Remove(temp); }
                    ClearChildren(child);
                }
            }
            Dispatcher.Invoke(new Action(() => item.Children.Clear()));
        }

        private void AddChildObjectItem(ObjectItem parent, ObjectItem child)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (parent != null)
                {
                    parent.AddChild(child);
                }
            }));
        }

        #endregion

        //将界面上的值存入数据库,在这里也就是将mListStatisticalParamItemDetails_里的内容存入数据库
        private OperationReturn SaveValueToDB()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            if (mListStatisticalParamItemDetails_ == null)
            {
                return optReturn;
            }
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3108Codes.SaveParamItemValue;
                for (int i = 0; i < mListStatisticalParamItemDetails_.Count; i++)
                {
                    optReturn = XMLHelper.SeriallizeObject(mListStatisticalParamItemDetails_[i]);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    webRequest.ListData.Add(optReturn.Data.ToString());
                }
                Service31081Client client = new Service31081Client(
                        WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                        WebHelper.CreateEndpointAddress(
                            CurrentApp.Session.AppServerInfo,
                            "Service31081"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    optReturn.Result = false;
                    optReturn.Code = webReturn.Code;
                    optReturn.Message = webReturn.Message;
                    return optReturn;
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        #region  EventHandlers

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
                    //ShowException("1" + ex.Message);
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
                    //ShowException("2" + ex.Message);
                }
            }

            //固定资源(有些资源包含转换器，命令等自定义类型，
            //这些资源不能通过url来动态加载，他只能固定的编译到程序集中
            try
            {
                string uri = string.Format("/Themes/Default/UMPS3108/ABCDItem.xaml");
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
                string uri = string.Format("/Themes/Default/UMPS3108/CombinedParamItemsPreViewItem.xaml");
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
                string uri = string.Format("/Themes/Default/UMPS3108/CombinedDesignerStatic.xaml");
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
                string uri = string.Format("/Themes/Default/UMPS3108/ParamItemViewItem.xaml");
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
                string uri = string.Format("/Themes/Default/UMPS3108/SCMainPage.xaml");
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
            //PageHead.AppName = CurrentApp.GetLanguageInfo("3108000", "UMP Statistical Param Configuration");
            CurrentApp.AppTitle = CurrentApp.GetLanguageInfo(string.Format("FO{0}", CurrentApp.ModuleID), "Advance Configuration");

            LbLeft.Text = CurrentApp.GetLanguageInfo("3108L001", "Statistical Param List");
            LbCurrentObject.Text = CurrentApp.GetLanguageInfo("3108L002", "参数");
            LbRight.Text = CurrentApp.GetLanguageInfo("3108L003", "操作");
            LbDescriptions.Text = CurrentApp.GetLanguageInfo("3108L004", "描述");

            for (int i = 0; i < ListQP.Count; i++)
            {
                string ID = string.Empty;
                //GetParamID(ListQP[i].GroupID.ToString().Substring(4), out ID);
                ID = ListQP[i].ParamID.ToString().Substring(5).Replace("0", "");
                ListQP[i].Name = CurrentApp.GetLanguageInfo(string.Format("31081{0}", ID), ListQP[i].GroupID.ToString());
                ListQP[i].Description = CurrentApp.GetLanguageInfo(string.Format("3108D1{0}", ID), ListQP[i].GroupID.ToString());
            }
            //更新左边树的语言
            for (int i = 0; i < mListObjectItems.Count; i++)
            {
                ObjectItem item = mListObjectItems[i];
                item.Name = CurrentApp.GetLanguageInfo(string.Format("FO{0}", item.ItemID), "change lang");
            }
            //更新操作的语言
            InitOperation();
            InitBasicOperations();
            //更新Border语言
            if (this.PanelPropertyList.Child != null)
            {
                string Code = objectItemTree.ItemID;
                string tempDescription = string.Format("{0}:\r\n", objectItemTree.Name);
                switch (Code)
                {
                    //查询质检参数配置
                    case "31080101001":
                    case "31080101002":
                    case "31080101003":
                        QPCPage.ChangeLanguage();
                        QualityParam QPTemp = objectItemTree.Data as QualityParam;
                        List<QualityParam> QPTempList = (from LQP in ListQP
                                                         where LQP.GroupID == QPTemp.GroupID
                                                         orderby LQP.SortID
                                                         select LQP).ToList();

                        TxtDescription.Text = string.Format("{0}:\n\n{1}\n\n{2}", QPTemp.Name, QPTempList[0].Description, QPTempList[1].Description);
                        break;
                    case "31080101004":
                        QPTCPage.ChangeLanguage();
                        List<QualityParam> QPTTempList = (from LQP in ListQP
                                                          where LQP.ParentTreeID == objectItemTree.ItemID
                                                          orderby LQP.SortID
                                                          select LQP).ToList();
                        //TxtDescription.Text = string.Format("  {0}:\n\n  {1}\n\n  {2}", objectItemTree.Name, QPTTempList[0].Description, QPTTempList[1].Description);
                        foreach (QualityParam info in QPTTempList)
                        {
                            tempDescription += string.Format("{0}\r\n", info.Description);
                        }
                        TxtDescription.Text = tempDescription;
                        break;
                    //abcd参数配置
                    case "31080102001":
                    case "31080102002":
                        paramsItemsconfigPage.ChangeLanguage();
                        //combStatiParaItemsDesigner.ChangeLanguage();
                        TxtDescription.Text = objectItemTree.Name;
                        CreatButton();
                        break;
                    case "":
                        break;
                    //机构和技能组
                    case "31080201":
                    case "31080202":
                        Orgpage.ChangeLanguage();
                        TxtDescription.Text = objectItemTree.Name;
                        CreatButton();
                        break;
                    default:
                        if (Code.Length > 8)
                        {
                            paramsItemsconfigPage.ChangeLanguage();
                            TxtDescription.Text = objectItemTree.Name;
                        }
                        break;
                }
            }
            //更新panel语言
            if (PopupPanel.Content != null)
            {
                string Code = objectItemTree.ItemID;
                switch (Code)
                {
                    //abcd参数配置
                    case "31080102001":
                    case "31080102002":
                        break;
                    case "":
                        break;
                    //机构和技能组
                    case "31080201":
                    case "31080202":
                        ABCDConfigPage abcdPage = PopupPanel.Content as ABCDConfigPage;
                        abcdPage.CurrentApp = CurrentApp;
                        abcdPage.ChangeLanguage();
                        break;
                    default:
                        if (Code.Length > 8)
                        {
                        }
                        break;
                }
            }
        }
        #endregion

    }
}