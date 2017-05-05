using Common1111;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using UMPS1101.Models;
using UMPS1101.Wcf11011;
using UMPS1101.Wcf11012;
using UMPS1101.Wcf11111;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common11011;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS1101
{
    /// <summary>
    /// UserResourcesPage.xaml 的交互逻辑
    /// </summary>
    public partial class UserResourcesPage
    {

        #region Memembers

        public OUMMainView PageParent;
        public ObjectItem UserItem;

        private ObjectItem mRootItem;
        private List<string> mListUserObjects;
        private List<ObjectItem> mListObjectItems;
        private List<ResourceNameNum> mListResources;
        private List<string> ListVCLog;
        private List<string> ListAgent;
        private List<string> ListExt;
        private List<ResourceInfo> mListResourcesInfo;
        private BackgroundWorker mWorder;
        private bool IsCheckSelfPrimission;
        //public S1101App CurrentApp;
        #endregion

        public UserResourcesPage()
        {
            InitializeComponent();
            mRootItem = new ObjectItem();
            mListUserObjects = new List<string>();
            mListObjectItems = new List<ObjectItem>();
            mListResources = new List<ResourceNameNum>();
            mListResourcesInfo = new List<ResourceInfo>();
            ListVCLog = new List<string>();
            ListAgent = new List<string>();
            ListExt = new List<string>();
            Loaded += UserResourcesPage_Loaded;
        }

        private void UserResourcesPage_Loaded(object sender, RoutedEventArgs e)
        {
            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
            this.CombResources.SelectionChanged += CombResources_SelectionChanged;
            TvObjects.ItemsSource = mRootItem.Children;
            this.CombResources.ItemsSource = mListResources;
            Init();
            ChangeLanguage();
        }

        private void Init()
        {
            mRootItem.Children.Clear();
            mListObjectItems.Clear();
            if (PageParent != null)
            {
                PageParent.SetBusy(true, string.Empty);
            }
            mWorder = new BackgroundWorker();
            mWorder.DoWork += (s, de) =>
            {
                InitListResources();
                //InitListAgent();
                //InitListExt();
                InitVCLog();
                InitListResourceInfo();
                //LoadResourcesObject();
            };
            mWorder.RunWorkerCompleted += (s, re) =>
            {
                mWorder.Dispose();
                if (PageParent != null)
                {
                    PageParent.SetBusy(false, string.Empty);
                }
                mRootItem.IsExpanded = true;
                if (mRootItem.Children.Count > 0)
                {
                    mRootItem.Children[0].IsExpanded = true;
                }
                mRootItem.IsChecked = false;
                SetObjectCheckState();
            };
            mWorder.RunWorkerAsync();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            IsCheckSelfPrimission = true;
            SetUserControlObject();
            if (IsCheckSelfPrimission == false)
                return;
            //Close();
        }

        private void Close()
        {
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }

        void CombResources_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mRootItem.Children.Clear();
            ResourceNameNum RNN = this.CombResources.SelectedItem as ResourceNameNum;
            string ResourceType = string.Empty;
            LoadResourcesObject(RNN.ResourceCode.ToString());
        }


        #region Languages

        public override void ChangeLanguage()
        {
            try
            {
                base.ChangeLanguage();

                BtnConfirm.Content = CurrentApp.GetLanguageInfo("110110", "Confirm");
                BtnClose.Content = CurrentApp.GetLanguageInfo("110111", "Close");

                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.Title = CurrentApp.GetLanguageInfo("11011500", "Set User Resources");
                }
            }
            catch (Exception ex)
            { }
        }

        #endregion

        #region Load

        private void InitListResources()
        {
            mListResources.Clear();
            ResourceNameNum rnn = new ResourceNameNum();
            rnn.ResourceNum = S1101Consts.VCLogServer;
            rnn.ResourceName = CurrentApp.GetLanguageInfo(rnn.ResourceNum.ToString(), "录音服务器");
            rnn.ResourceCode = S1101Consts.RESOURCE_VCLog;
            mListResources.Add(rnn);
            //ResourceNameNum rnn1 = new ResourceNameNum();
            //rnn1.ResourceNum = S1101Consts.Agent;
            //rnn1.ResourceName = S1101App.GetLanguageInfo(rnn1.ResourceNum.ToString(), "坐席");
            //rnn1.ResourceCode = S1101Consts.RESOURCE_AGENT;
            //mListResources.Add(rnn1);
            //ResourceNameNum rnn2 = new ResourceNameNum();
            //rnn2.ResourceNum = S1101Consts.Exten;
            //rnn2.ResourceName = S1101App.GetLanguageInfo(rnn2.ResourceNum.ToString(), "分机");
            //rnn2.ResourceCode = S1101Consts.RESOURCE_EXTENSION;
            //mListResources.Add(rnn2);
        }

        private void InitVCLog()
        {
            ListVCLog.Clear();
            WebRequest webRequest = new WebRequest();
            webRequest.Session = CurrentApp.Session;
            webRequest.Code = (int)S1101Codes.GetAgentOrExt;
            webRequest.ListData.Add(UserItem.ObjID.ToString());
            webRequest.ListData.Add("221");
            Service11011Client client = new Service11011Client(
               WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
               WebHelper.CreateEndpointAddress(
                   CurrentApp.Session.AppServerInfo,
                   "Service11011"));
            //Service1111Client client = new Service1111Client();
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
                ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                return;
            }
            ListVCLog = webReturn.ListData;
        }

        //private void InitListAgent()
        //{
        //    ListAgent.Clear();
        //    WebRequest webRequest = new WebRequest();
        //    webRequest.Session = S1101App.Session;
        //    webRequest.Code = (int)S1101Codes.GetAgentOrExt;
        //    webRequest.ListData.Add(UserItem.ObjID.ToString());
        //    webRequest.ListData.Add("103");
        //    Service11011Client client = new Service11011Client(
        //       WebHelper.CreateBasicHttpBinding(S1101App.Session),
        //       WebHelper.CreateEndpointAddress(
        //           S1101App.Session.AppServerInfo,
        //           "Service11011"));
        //    //Service1111Client client = new Service1111Client();
        //    WebHelper.SetServiceClient(client);
        //    WebReturn webReturn = client.DoOperation(webRequest);
        //    client.Close();
        //    if (!webReturn.Result)
        //    {
        //        S1101App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
        //        return;
        //    }
        //    if (webReturn.ListData == null)
        //    {
        //        S1101App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
        //        return;
        //    }
        //    ListAgent = webReturn.ListData;
        //}

        //private void InitListExt()
        //{
        //    ListExt.Clear();
        //    WebRequest webRequest = new WebRequest();
        //    webRequest.Session = S1101App.Session;
        //    webRequest.Code = (int)S1101Codes.GetAgentOrExt;
        //    webRequest.ListData.Add(UserItem.ObjID.ToString());
        //    webRequest.ListData.Add("104");
        //    Service11011Client client = new Service11011Client(
        //        WebHelper.CreateBasicHttpBinding(S1101App.Session),
        //        WebHelper.CreateEndpointAddress(
        //            S1101App.Session.AppServerInfo,
        //            "Service11011"));
        //    //Service1111Client client = new Service1111Client();
        //    WebHelper.SetServiceClient(client);
        //    WebReturn webReturn = client.DoOperation(webRequest);
        //    client.Close();
        //    if (!webReturn.Result)
        //    {
        //        S1101App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
        //        return;
        //    }
        //    if (webReturn.ListData == null)
        //    {
        //        S1101App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
        //        return;
        //    }
        //    ListExt = webReturn.ListData;
        //}
       //加载录音服务器数据
        private void InitListResourceInfo()
        {
            mListResourcesInfo.Clear();
            WebRequest webRequest = new WebRequest();
            webRequest.Session = CurrentApp.Session;
            webRequest.Code = (int)WebCodes.GetVoiceIP_Name201;
            Service11111Client client = new Service11111Client(
               WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
               WebHelper.CreateEndpointAddress(
                   CurrentApp.Session.AppServerInfo,
                   "Service11111"));
            //Service1111Client client = new Service1111Client();
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
                ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                return;
            }
            OperationReturn optReturn;
            for (int i = 0; i < webReturn.ListData.Count; i++)
            {
                optReturn = XMLHelper.DeserializeObject<ResourceInfo>(webReturn.ListData[i]);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                ResourceInfo info = optReturn.Data as ResourceInfo;
                if (info == null)
                {
                    ShowException(string.Format("ResourcePropertyInfo is null"));
                    return;
                }
                //info.Description = info.ToString();
                mListResourcesInfo.Add(info);
            }
            CurrentApp.WriteLog("PageLoad", string.Format("Init ResourceInfo"));
        }

        private void LoadResourcesObject(string ResourceType)
        {
            if (UserItem == null) { return; }
            var parentItem = UserItem.Parent as ObjectItem;
            if (parentItem != null)
            {
                LoadResources(mRootItem, parentItem.ObjID.ToString(), ResourceType);
            }
            SetObjectCheckState();
        }

        private void LoadResources(ObjectItem parentItem, string parentID, string ResourceType)
        {

            switch (ResourceType)
            {
                case "221":
                    LoadVCLog(parentItem, parentID);
                    mListUserObjects = ListVCLog;
                    break;
               
                default:
                    break;
            }
        }

        
        #endregion
        //加载录音服务器树
        private void LoadVCLog(ObjectItem parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1101Codes.GetResourceObjList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("221");
                //webRequest.ListData.Add("1");
                Service11011Client client = new Service11011Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service11011"));
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
                    ResourceInfo RI = new ResourceInfo();
                    for (int j = 0; j < mListResourcesInfo.Count; j++)
                    {
                        if (strInfo == mListResourcesInfo[j].ResourceID.ToString())
                        {
                            ObjectItem item = new ObjectItem();
                            item.ObjType = ConstValue.RESOURCE_ORG;
                            item.ObjID = mListResourcesInfo[j].ResourceID;
                            item.Name = mListResourcesInfo[j].Tostring();
                            item.Data = mListResourcesInfo[j];
                            item.Icon = "Images/voiceserver.png";
                            Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
                            mListObjectItems.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #region Others

        private void SetObjectCheckState()
        {
            SetObjectCheckState(mRootItem);
        }

        private void SetObjectCheckState(ObjectItem parentItem)
        {
            if (parentItem == null) { return; }
            try
            {
                if (parentItem.Children.Count > 0)
                {
                    for (int i = 0; i < parentItem.Children.Count; i++)
                    {
                        SetObjectCheckState(parentItem.Children[i] as ObjectItem);
                    }
                }
                else
                {
                    if (mListUserObjects.Contains(parentItem.ObjID.ToString()))
                    {
                        parentItem.IsChecked = true;
                    }
                    else
                    {
                        parentItem.IsChecked = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
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

        #region Operations

        private void SetUserControlObject()
        {
            if (UserItem == null) { return; }
            BasicUserInfo userInfo = UserItem.Data as BasicUserInfo;
            if (userInfo == null) { return; }
            List<string> listObjectState = new List<string>();
            SetUserControlObject(mRootItem, ref listObjectState);

            if (IsCheckSelfPrimission)
            {
                if (listObjectState.Count > 0)
                {
                    int count = listObjectState.Count;
                    if (PageParent != null)
                    {
                        PageParent.SetBusy(true, string.Empty);
                    }
                    mWorder = new BackgroundWorker();
                    mWorder.DoWork += (s, de) =>
                    {
                        try
                        {
                            WebRequest webRequest = new WebRequest();
                            webRequest.Session = CurrentApp.Session;
                            webRequest.Code = (int)S1101Codes.SetUserControlObject;
                            webRequest.ListData.Add(userInfo.UserID.ToString());
                            webRequest.ListData.Add(count.ToString());
                            for (int i = 0; i < count; i++)
                            {
                                webRequest.ListData.Add(listObjectState[i]);
                            }
                            Service11011Client client = new Service11011Client(
                                WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                                WebHelper.CreateEndpointAddress(
                                    CurrentApp.Session.AppServerInfo,
                                    "Service11011"));
                            WebHelper.SetServiceClient(client);
                            WebReturn webReturn = client.DoOperation(webRequest);
                            client.Close();
                            if (!webReturn.Result)
                            {
                                ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                            }

                            #region 记录日志

                            string strAdded = string.Empty;
                            string strRemoved = string.Empty;
                            List<string> listLogParams = new List<string>();
                            if (webReturn.ListData != null && webReturn.ListData.Count > 0)
                            {
                                for (int i = 0; i < webReturn.ListData.Count; i++)
                                {
                                    string strInfo = webReturn.ListData[i];
                                    string[] arrInfos = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                                        StringSplitOptions.RemoveEmptyEntries);
                                    if (arrInfos.Length >= 2)
                                    {
                                        if (arrInfos[0] == "A")
                                        {
                                            var objItem =
                                                mListObjectItems.FirstOrDefault(o => o.ObjID.ToString() == arrInfos[1]);
                                            if (objItem != null)
                                            {
                                                strAdded += objItem.Name + ",";
                                            }
                                            else
                                            {
                                                strAdded += arrInfos[1] + ",";
                                            }
                                        }
                                        if (arrInfos[0] == "D")
                                        {
                                            var objItem =
                                                mListObjectItems.FirstOrDefault(o => o.ObjID.ToString() == arrInfos[1]);
                                            if (objItem != null)
                                            {
                                                strRemoved += objItem.Name + ",";
                                            }
                                            else
                                            {
                                                strRemoved += arrInfos[1] + ",";
                                            }
                                        }
                                    }
                                }
                                strAdded = strAdded.TrimEnd(new[] { ',' });
                                strRemoved = strRemoved.TrimEnd(new[] { ',' });
                            }
                            listLogParams.Add(userInfo.FullName);
                            listLogParams.Add(strAdded);
                            listLogParams.Add(strRemoved);
                            CurrentApp.WriteOperationLog(S1101Consts.OPT_SETUSERRESOURCEMANAGEMENT.ToString(), ConstValue.OPT_RESULT_SUCCESS, "LOG1101001", listLogParams);

                            #endregion
                        }
                        catch (Exception ex)
                        {
                            ShowException(ex.Message);
                        }
                    };
                    mWorder.RunWorkerCompleted += (s, re) =>
                    {
                        mWorder.Dispose();
                        if (PageParent != null)
                        {
                            PageParent.SetBusy(false, string.Empty);
                        }
                        var parent = Parent as PopupPanel;
                        if (parent != null)
                        {
                            parent.IsOpen = false;
                        }
                    };
                    mWorder.RunWorkerAsync();
                }
            }
        }

        private void SetUserControlObject(ObjectItem objItem, ref List<string> listObjectState)
        {
            for (int i = 0; i < objItem.Children.Count; i++)
            {
                ObjectItem child = objItem.Children[i] as ObjectItem;
                if (child == null) { continue; }
                if (child.ObjID == UserItem.ObjID)
                {
                    if (child.IsChecked == false)
                    {
                        CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1101N025", "You cannot cancel the management authority."));
                        IsCheckSelfPrimission = false;
                    }
                }
                if (child.IsChecked == true)
                {
                    listObjectState.Add(string.Format("{0}{1}{2}", child.ObjID, ConstValue.SPLITER_CHAR, "1"));
                }
                else if (child.IsChecked == false)
                {
                    listObjectState.Add(string.Format("{0}{1}{2}", child.ObjID, ConstValue.SPLITER_CHAR, "0"));
                }
                SetUserControlObject(child, ref listObjectState);
            }
        }

        #endregion

    }
}
