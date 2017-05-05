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
using UMPS3108.Models;
using UMPS3108.Wcf31081;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31081;
using VoiceCyber.UMP.Communications;

namespace UMPS3108
{
    /// <summary>
    /// OrgPage.xaml 的交互逻辑
    /// </summary>
    public partial class OrgPage
    {
        #region menbers

        private BackgroundWorker mWorker;
        private ObjectItem mRoot;
        private List<ObjectItem> mListObjectItems;
        public SCMainView Mainpage;
        public ObjectItem ObjItem;//本界面选中的树节点
        //private ABCDConfigPage abcdPage;
        public string OparetionCode;//传进来的操作编号，用于区分是加载机构还是技能组。
        private ObservableCollection<StatisticParamModel> ListStatistical;
        public StatisticParamModel DeleteStatisticParam;
        private List<StatisticParam> ListOrgStatistic;
        #endregion

        public OrgPage()
        {
            InitializeComponent();

            mWorker = new BackgroundWorker();
            mRoot = new ObjectItem();
            mListObjectItems = new List<ObjectItem>();
            ObjItem = new ObjectItem();
            ListStatistical = new ObservableCollection<StatisticParamModel>();
            ListOrgStatistic = new List<StatisticParam>();

            this.Loaded += OrgPage_Loaded;
            this.TvOrg.ItemsSource = mRoot.Children;
            this.LBStatistical.ItemsSource = ListStatistical;
            //this.LBStatistical.SelectionChanged += LBStatistical_SelectionChanged;
            TvOrg.SelectedItemChanged += TvOrg_SelectedItemChanged;
            this.LBStatistical.SelectionChanged += LBStatistical_SelectionChanged;
        }

        void OrgPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPage();
        }

        public void LoadPage()
        {
            //InitStatistical();
            if (OparetionCode == "31080201")
            {
                InitControledOrgs();
            }
            else
            {
                InitSkillGroup();
            }
        }

        void LBStatistical_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.LBStatistical.SelectedIndex != 0)
                //选中要删除的大项
                DeleteStatisticParam = this.LBStatistical.SelectedItem as StatisticParamModel;
        }

        void TvOrg_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ObjItem = TvOrg.SelectedItem as ObjectItem;
            //Mainpage.obj
            InitListBox();
        }

        public void InitListBox()
        {
            try
            {
                ListStatistical.Clear();
                if (ObjItem == null) { return; }
                InitLBFirstItem();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3108Codes.GetABCDInfo;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(ObjItem.ItemID.ToString());
                //webRequest.ListData.Add(Statistic.StatisticKey.ToString());
                Service31081Client client = new Service31081Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31081"));
                WebHelper.SetServiceClient(client);
                //Service31081Client client = new Service31081Client();
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("InitStatisticConfig Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                //if (webReturn.ListData.Count == 0)
                //{
                //    foreach (StatisticParam SP in ListOrgStatistic)
                //    {
                //        ListStatistical.Add(SP);
                //    }
                //}
                //else
                //{
                    for (int i = 0; i < webReturn.ListData.Count; i++)
                    {
                        OperationReturn optReturn = XMLHelper.DeserializeObject<StatisticParam>(webReturn.ListData[i]);
                        if (!optReturn.Result)
                        {
                            ShowException(string.Format("InitStatisticConfig Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        StatisticParam SP = optReturn.Data as StatisticParam;
                        if (SP.StatisticalParamID == 3110000000000000008)
                        {
                            SP.TableType = 2;
                        }
                        else
                        {
                            SP.TableType = 1;
                        }
                        string spID = SP.StatisticalParamID.ToString();
                        spID = spID.Substring(spID.Length - 1);
                        SP.StatisticalParamName = CurrentApp.GetLanguageInfo(string.Format("FO3108010200{0}",spID), SP.StatisticalParamID.ToString());
                        ListStatistical.Add(new StatisticParamModel(SP));
                    }
            }
            //}
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        //void LBStatistical_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    int num = this.LBStatistical.SelectedIndex;
        //    if (num == 0) { return; }
        //    StatisticalParam SP = this.LBStatistical.SelectedItem as StatisticalParam;
        //    OparetionCode = SP.StatisticalParamID.ToString();
        //}

        private void InitStatistical()
        {
            try
            {
                ListOrgStatistic.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3108Codes.GetABCDList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add("1");
                Service31081Client client = new Service31081Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31081"));
                WebHelper.SetServiceClient(client);
                //Service31081Client client = new Service31081Client();
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("InitListABCD Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<StatisticParam>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("InitListABCD Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    StatisticParam StatisticP = optReturn.Data as StatisticParam;
                    if (StatisticP != null)
                    {
                        ListOrgStatistic.Add(StatisticP);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitSkillGroup()
        {
            try
            {
                ClearChildren(mRoot);
                mListObjectItems.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3108Codes.GetSkillGroupList;
                webRequest.Session = CurrentApp.Session;
                Service31081Client client = new Service31081Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31081"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("InitControledOrgs Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                List<ObjectItem> listChild = new List<ObjectItem>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string orgInfo = webReturn.ListData[i];
                    if (orgInfo != null)
                    {
                        ObjectItem item = new ObjectItem();
                        List<string> temp = orgInfo.Split(';').ToList();
                        if (temp.Count != 5) { return; }
                        item.ObjID = temp[1];
                        item.ItemID = temp[0];
                        item.Name = temp[2];
                        item.Description = temp[3];
                        item.ObjType = Convert.ToInt32(temp[4]);
                        //item.Description = string.Format("{0}:{1}", temp[0], temp[1]);
                        //item.Icon = "Images/org.ico";
                        listChild.Add(item);
                    }
                }
                listChild = listChild.OrderBy(o => o.ObjID).ToList();
                for (int i = 0; i < listChild.Count; i++)
                {
                    mListObjectItems.Add(listChild[i]);
                    AddChildObjectItem(mRoot, listChild[i]);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitControledOrgs()
        {
            ClearChildren(mRoot);
            mListObjectItems.Clear();
            InitControledOrgs(mRoot, "-1");
            //展开到下一级
            mRoot.IsExpanded = true;
            if (mRoot.Children.Count > 0)
            {
                for (int i = 0; i < mRoot.Children.Count; i++)
                {
                    mRoot.Children[i].IsExpanded = true;
                }
            }

            CurrentApp.WriteLog("PageInit", string.Format("Init OrgAndUser"));
        }

        private void InitControledOrgs(ObjectItem parentObj, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3108Codes.GetOrganizationList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add(parentID);
                Service31081Client client = new Service31081Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31081"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("InitControledOrgs Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                List<ObjectItem> listChild = new List<ObjectItem>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string orgInfo = webReturn.ListData[i];
                    if (orgInfo != null)
                    {
                        ObjectItem item = new ObjectItem();
                        List<string> temp = orgInfo.Split(';').ToList();
                        if (temp.Count != 2) { return; }
                        item.Name = temp[1];
                        item.ItemID = temp[0];
                        item.ObjParentID = parentID;
                        item.Description = string.Format("{0}:{1}", temp[0], temp[1]);
                        item.Icon = "Images/org.ico";
                        InitControledOrgs(item, temp[0]);
                        listChild.Add(item);
                    }
                }
                listChild = listChild.OrderBy(o => o.Name).ToList();
                for (int i = 0; i < listChild.Count; i++)
                {
                    mListObjectItems.Add(listChild[i]);
                    AddChildObjectItem(parentObj, listChild[i]);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public bool DeleteStatisticParamFromDB()
        {
            WebRequest webRequest = new WebRequest();
            webRequest.Code = (int)S3108Codes.DeleteConfig;
            webRequest.Session = CurrentApp.Session;
            StatisticParam SP = new StatisticParam();
            SP.CycleTime = DeleteStatisticParam.CycleTime;
            SP.StatisticKey = DeleteStatisticParam.StatisticKey;
            SP.TableType = DeleteStatisticParam.TableType;
            SP.RowNum = DeleteStatisticParam.RowNum;
            SP.StatisticalParamID = DeleteStatisticParam.StatisticalParamID;
            SP.StatisticalParamName = DeleteStatisticParam.StatisticalParamName;

            SP.CycleTimeParam = DeleteStatisticParam.CycleTimeParam;
            SP.IsCombine = DeleteStatisticParam.IsCombine;
            SP.OrgID = DeleteStatisticParam.OrgID;
            OperationReturn optReturn = XMLHelper.SeriallizeObject<StatisticParam>(SP);
            if (!optReturn.Result)
            {
                ShowException(string.Format("DeleteStatisticParamFromDB xml Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                return false;
            }
            string StatisticC = optReturn.Data as string;
            webRequest.ListData.Add(StatisticC);
            Service31081Client client = new Service31081Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31081"));
            WebHelper.SetServiceClient(client);
            //Service31081Client client = new Service31081Client();
            WebReturn webReturn = client.DoOperation(webRequest);
            client.Close();
            if (!webReturn.Result)
            {
                ShowException(string.Format("deleteConfig Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                #region 记录日志
                string orgname = mListObjectItems.FirstOrDefault(p => p.ItemID == SP.OrgID.ToString()).Name;
                string msg = string.Format("{0}{1}{2}:{3}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO3108007")), orgname, SP.StatisticalParamName);
                CurrentApp.WriteOperationLog("3108007", ConstValue.OPT_RESULT_FAIL, msg);
                #endregion
                return false;
            }
            InitListBox();
            #region 记录日志
            string orgnameT = mListObjectItems.FirstOrDefault(p => p.ItemID == SP.OrgID.ToString()).Name;
            string msgT = string.Format("{0}{1}{2}:{3}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO3108007")), orgnameT, SP.StatisticalParamName);
            CurrentApp.WriteOperationLog("3108007", ConstValue.OPT_RESULT_SUCCESS, msgT);
            #endregion
            return true;
        }

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

        private void InitLBFirstItem()
        {
            StatisticParam SP = new StatisticParam();
            SP.StatisticalParamName = CurrentApp.GetLanguageInfo("310802001", "该机构所拥有的大项列表：");
            SP.StatisticalParamID = 0;
            ListStatistical.Add(new StatisticParamModel(SP));
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            for (int i = 0; i < ListStatistical.Count(); i++)
            {
                StatisticParamModel temp = ListStatistical[i];
                if (i == 0)
                {
                    temp.StatisticalParamName = CurrentApp.GetLanguageInfo("310802001", "该机构所拥有的大项列表：");
                }
                else
                {
                    string code = temp.StatisticalParamID.ToString();
                    code = code.Substring(code.Length - 1);
                    temp.StatisticalParamName = CurrentApp.GetLanguageInfo(string.Format("FO3108010200{0}",code),"3108010200");
                }
            }
        }
        #endregion

    }
}
