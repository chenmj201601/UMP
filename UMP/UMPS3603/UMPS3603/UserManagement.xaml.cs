using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using Common3603;
using UMPS3603.Wcf36031;
using UMPS3603.Models;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.UMP.Controls.Wcf11012;
using VoiceCyber.Wpf.CustomControls;

namespace UMPS3603
{
    /// <summary>
    /// Interaction logic for UerManagement.xaml
    /// </summary>
    public partial class UserManagement
    {
        #region members

        public ExamProductionView PageParent;
        public TestInfoParam MTestInfoParam;
        public OrgUserItem MRootItem;

        private readonly List<string> _mListTestUsers;
        private readonly List<OrgUserItem> _mListOrgUserItems;
        private BackgroundWorker _mWorker;

        #endregion

        public UserManagement()
        {
            InitializeComponent();

            MRootItem = new OrgUserItem();
            _mListOrgUserItems = new List<OrgUserItem>();
            _mListTestUsers = new List<string>();
            Loaded += TestUserManagement_Loaded;
            Loaded += UCCustomSetting_Loaded;
        }

        void UCCustomSetting_Loaded(object sender, RoutedEventArgs e)
        {
           
            Init();
            ChangeLanguage();
            
        }

        private void TestUserManagement_Loaded(object sender, RoutedEventArgs e)
        {
            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
            TvObjects.ItemsSource = MRootItem.Children;
        }

        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            SetTestUser();
        }


        #region Init andLoad

        private void Init()
        {
            MRootItem.Children.Clear();
            _mListOrgUserItems.Clear();
            if (PageParent != null)
            {
                PageParent.SetBusy(true, CurrentApp.GetMessageLanguageInfo("001", "Loading data, please wait..."));
            }
            _mWorker = new BackgroundWorker();
            _mWorker.DoWork += (s, de) =>
            {
                LoadAvaliableObject();
                LoadTestUser();
            };
            _mWorker.RunWorkerCompleted += (s, re) =>
            {
                _mWorker.Dispose();
                if (PageParent != null)
                {
                    PageParent.SetBusy(false, CurrentApp.GetMessageLanguageInfo("001", "Ready"));
                }
                SetObjectCheckState();
            };
            _mWorker.RunWorkerAsync();
        }

        private void LoadAvaliableObject()
        {
            try
            {
                LoadAvaliableOrgs(MRootItem, "-1");
                Dispatcher.Invoke(new Action(() =>
                {
                    MRootItem.IsChecked = false;
                    if (MRootItem.Children.Count > 0)
                    {
                        MRootItem.Children[0].IsExpanded = true;
                    }
                }));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadTestUser()
        {
            string strLog;
            try
            {
                if (MTestInfoParam == null)
                {
                    return;
                }
                string testNum = MTestInfoParam.LongTestNum.ToString();
                WebRequest webRequest = new WebRequest
                {
                    Code = (int) S3603Codes.OptGetTestUserList,
                    Session = CurrentApp.Session
                };
                webRequest.ListData.Add(testNum);
                Service36031Client client = new Service36031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36031"));
                //var client = new Service36031Client();
                WebReturn webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3603T00093", "Load User"));
                if (!webReturn.Result)
                {
                    #region 写操作日志

                    strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3603T00093"),
                        Utils.FormatOptLogString("3603T00095"), webReturn.Message);
                    CurrentApp.WriteOperationLog(S3603Consts.OPT_LoadUser.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                    #endregion

                    CurrentApp.WriteLog(webReturn.Message);

                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException("WebReturn ListData is null");
                    return;
                }

                #region 写操作日志

                strLog = string.Format("{0} {1} ", Utils.FormatOptLogString("3603T00093"),
                    Utils.FormatOptLogString("3603T00094"));
                CurrentApp.WriteOperationLog(S3603Consts.OPT_LoadUser.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                #endregion

                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3603T00094", "Load User Success"));

                foreach (string strData in webReturn.ListData)
                {
                    _mListTestUsers.Add(strData);
                }
            }
            catch (Exception ex)
            {
                #region 写操作日志

                strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3603T00093"),
                    Utils.FormatOptLogString("3603T00095"), ex.Message);
                CurrentApp.WriteOperationLog(S3603Consts.OPT_LoadUser.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                #endregion

                CurrentApp.WriteLog(ex.Message);
                ShowException(ex.Message);
            }
        }

        private void LoadAvaliableOrgs(OrgUserItem parentItem, string parentId)
        {
            try
            {
                WebRequest webRequest = new WebRequest
                {
                    Session = CurrentApp.Session,
                    Code = (int) RequestCode.WSGetUserObjList
                };
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(ConstValue.RESOURCE_ORG.ToString());
                webRequest.ListData.Add(parentId);
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException("Fail.\tListData is null");
                    return;
                }
                foreach (string strInfo in webReturn.ListData)
                {
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 2) { continue; }
                    string strId = arrInfo[0];
                    string strName = arrInfo[1];
                    OrgUserItem item = new OrgUserItem
                    {
                        ObjType = ConstValue.RESOURCE_ORG,
                        ObjID = Convert.ToInt64(strId),
                        Name = strName,
                        Data = strInfo,
                        Icon = strId == ConstValue.ORG_ROOT.ToString() ? "Images/rootorg.ico" : "Images/org.ico"
                    };
                    LoadAvaliableOrgs(item, strId);
                    //LoadAvaliableUsers(item, strID);
                    if (S3603App.GroupingWay.Contains("A"))
                    {
                        LoadAvaliableAgent(item, strId);
                    }

                    Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
                    _mListOrgUserItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


        private void LoadAvaliableAgent(OrgUserItem parentItem, string parentId)
        {
            try
            {
                WebRequest webRequest = new WebRequest
                {
                    Session = CurrentApp.Session,
                    Code = (int) S3603Codes.OptGetCtrolAgent
                };
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(parentId);
                Service36031Client client = new Service36031Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service36031"));
                //var client = new Service36031Client();
                WebReturn webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException("Fail.\tListData is null");
                    return;
                }
                foreach (string strInfo in webReturn.ListData)
                {
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 3) { continue; }
                    string strId = arrInfo[0];
                    string strName = arrInfo[1];
                    string strFullName = arrInfo[2];
                    OrgUserItem item = new OrgUserItem
                    {
                        ObjType = ConstValue.RESOURCE_AGENT,
                        ObjID = Convert.ToInt64(strId),
                        Name = strName,
                        Description = strFullName,
                        Data = strInfo,
                        Icon = "Images/user_suit.png"
                    };
                    Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
                    _mListOrgUserItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Others

        private void SetObjectCheckState()
        {
            SetObjectCheckState(MRootItem);
        }

        private void SetObjectCheckState(OrgUserItem parentItem)
        {
            if (parentItem == null) { return; }
            try
            {
                if (parentItem.Children.Count > 0)
                {
                    foreach (CheckableItemBase checkableItemBase in parentItem.Children)
                    {
                        SetObjectCheckState(checkableItemBase as OrgUserItem);
                    }
                }
                else
                {
                    if (_mListTestUsers.Contains(parentItem.ObjID.ToString()))
                    {
                        parentItem.IsChecked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Operations
        private void SetTestUser()
        {
            if (MTestInfoParam == null) { return; }
            List<string> listObjectState = new List<string>();
            SetTestUser(MRootItem, ref listObjectState);

            if (listObjectState.Count > 0)
            {
                if (PageParent != null)
                {
                    PageParent.SetBusy(true, CurrentApp.GetMessageLanguageInfo("001", "Ready"));
                }
                _mWorker = new BackgroundWorker();
                _mWorker.DoWork += (s, de) =>
                {
                    string strLog;
                    try
                    {
                        int count = listObjectState.Count;
                        WebRequest webRequest = new WebRequest
                        {
                            Session = CurrentApp.Session,
                            Code = (int) S3603Codes.OptSetTestUserT11201
                        };
                        webRequest.ListData.Add(MTestInfoParam.LongTestNum.ToString());
                        webRequest.ListData.Add(count.ToString());
                        for (int i = 0; i < count; i++)
                        {
                            webRequest.ListData.Add(listObjectState[i]);
                        }
                        Service36031Client client = new Service36031Client(
                            WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                            WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36031"));

                        //var client = new Service36031Client();
                        WebReturn webReturn = client.UmpTaskOperation(webRequest);
                        client.Close();
                        CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3603T00007", "Set User"));
                        if (!webReturn.Result)
                        {
                            #region 写操作日志

                            strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3603T00007"),
                                Utils.FormatOptLogString("3603T00097"), webReturn.Message);
                            CurrentApp.WriteOperationLog(S3603Consts.OPT_SetUser.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                            #endregion

                            CurrentApp.WriteLog(webReturn.Message);
                            ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                            return;
                        }
                        CurrentApp.WriteLog("SetTestUser", webReturn.Data);
                        SetTestUser(listObjectState);

                        #region 写操作日志

                        strLog = string.Format("{0} {1}", Utils.FormatOptLogString("3603T00007"),
                            Utils.FormatOptLogString("3603T00096"));
                        CurrentApp.WriteOperationLog(S3603Consts.OPT_SetUser.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                        #endregion

                        ShowInformation(CurrentApp.GetLanguageInfo("3603T00076", "Set Test user end"));
                    }
                    catch (Exception ex)
                    {
                        #region 写操作日志

                        strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3603T00007"),
                            Utils.FormatOptLogString("3603T00097"), ex.Message);
                        CurrentApp.WriteOperationLog(S3603Consts.OPT_SetUser.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                        #endregion

                        CurrentApp.WriteLog(ex.Message);
                        ShowException(ex.Message);
                    }
                };
                _mWorker.RunWorkerCompleted += (s, re) =>
                {
                    _mWorker.Dispose();
                    if (PageParent != null)
                    {
                        PageParent.SetBusy(false, CurrentApp.GetMessageLanguageInfo("001", "Ready"));
                    }
                    var parent = Parent as PopupPanel;
                    if (parent != null)
                    {
                        if (PageParent != null) PageParent.RefreshTestUserInfo();
                        parent.IsOpen = false;
                    }
                };
                _mWorker.RunWorkerAsync();
            }
        }

        private void SetTestUser(OrgUserItem objItem, ref List<string> listObjectState)
        {
            try
            {
                foreach (CheckableItemBase checkableItemBase in objItem.Children)
                {
                    OrgUserItem child = checkableItemBase as OrgUserItem;
                    if (child == null)
                    {
                        continue;
                    }
                    if (child.ObjType == ConstValue.RESOURCE_AGENT || child.ObjType == ConstValue.RESOURCE_REALEXT ||
                        child.ObjType == ConstValue.RESOURCE_EXTENSION)
                    {
                        if (child.IsChecked == true)
                        {
                            listObjectState.Add(string.Format("{0}{1}{2}{3}{4}{5}{6}", child.ObjID, ConstValue.SPLITER_CHAR,
                                "1", ConstValue.SPLITER_CHAR, child.Name, ConstValue.SPLITER_CHAR, child.Description));
                        }
                        else if (child.IsChecked == false)
                        {
                            listObjectState.Add(string.Format("{0}{1}{2}{3}{4}{5}{6}", child.ObjID, ConstValue.SPLITER_CHAR,
                                "0", ConstValue.SPLITER_CHAR, child.Name, ConstValue.SPLITER_CHAR, child.Description));
                        }
                    }
                    SetTestUser(child, ref listObjectState);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SetTestUser(List<string> listObjectState)
        {
            string strLog;
            try
            {
                List<TestUserParam> lstTestUserParams = new List<TestUserParam>();
                foreach (var objectState in listObjectState)
                {
                    string[] strTemp = objectState.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    TestUserParam testUserParam = new TestUserParam
                    {
                        LongTestNum = MTestInfoParam.LongTestNum,
                        LongTestUserNum = Convert.ToInt64(strTemp[0]),
                        StrTestUserName = strTemp[3],
                        LongPaperNum = MTestInfoParam.LongPaperNum,
                        StrPaperName = MTestInfoParam.StrPaperName,
                        StrTestStatue = "N",
                        IntEable = Convert.ToInt16(strTemp[1])
                    };
                    lstTestUserParams.Add(testUserParam);
                }
                WebRequest webRequest = new WebRequest
                {
                    Session = CurrentApp.Session,
                    Code = (int) S3603Codes.OptSetTestUserT36036
                };
                Service36031Client client = new Service36031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                                          WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36031"));
                //var client = new Service36031Client();
                OperationReturn optReturn = XMLHelper.SeriallizeObject(lstTestUserParams);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                WebReturn webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3603T00007", "Set User"));
                if (!webReturn.Result)
                {
                    #region 写操作日志

                    strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3603T00007"),
                        Utils.FormatOptLogString("3603T00097"), webReturn.Message);
                    CurrentApp.WriteOperationLog(S3603Consts.OPT_SetUser.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                    #endregion

                    CurrentApp.WriteLog(webReturn.Message);
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                #region 写操作日志

                strLog = string.Format("{0} {1}", Utils.FormatOptLogString("3603T00007"),
                    Utils.FormatOptLogString("3603T00096"));
                CurrentApp.WriteOperationLog(S3603Consts.OPT_SetUser.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                #endregion
                CurrentApp.WriteLog("SetTestUser", webReturn.Data);
            }
            catch (Exception ex)
            {
                #region 写操作日志

                strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3603T00007"),
                    Utils.FormatOptLogString("3603T00097"), ex.Message);
                CurrentApp.WriteOperationLog(S3603Consts.OPT_SetUser.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                #endregion

                CurrentApp.WriteLog(ex.Message);
                ShowException(ex.Message);
            }
            

        }

        #endregion

        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.Title = CurrentApp.GetLanguageInfo("3603T00007", "ScoreSheet User Management");
                }
                BtnConfirm.Content = CurrentApp.GetLanguageInfo("3603T00051", "Confirm");
                BtnClose.Content = CurrentApp.GetLanguageInfo("3603T00052", "Close");
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion
    }
}
