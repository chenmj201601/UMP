// ***********************************************************************
// Assembly         : UMPS2400
// Author           : Luoyihua
// Created          : 07-28-2015
//
// Last Modified By : Luoyihua
// Last Modified On : 07-30-2015
// ***********************************************************************
// <copyright file="UC_EncryptionPolicyBindding.xaml.cs" company="VoiceCyber">
//     Copyright (c) VoiceCyber. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Common2400;
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
using UMPS2400.Entries;
using UMPS2400.Service11012;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using UMPS2400.ChildUCs;
using UMPS2400.Service24021;
using System.Data;
using UMPS2400.Service24011;

namespace UMPS2400.MainUserControls
{
    /// <summary>
    /// UC_EncryptionPolicyBindding.xaml 的交互逻辑
    /// </summary>
    public partial class UC_EncryptionPolicyBindding
    {
        #region 变量定义
        //能管理的录音服务器
        public List<CVoiceServer> LLstVoiceServer;
        /// <summary>
        /// 当前管理的录音服务器IP对应的资源编码
        /// </summary>
        public string mCurrentSelectIPSourceID;
        //listview要显示的数据ViewID
        private string mViewID;
        //listview需要显示的列的集合
        private ObservableCollection<ViewColumnInfo> mListColumnItems;
        public static ObservableCollection<PolicyInfoInList> lstAllPolicies = new ObservableCollection<PolicyInfoInList>();
        public static ObservableCollection<OperationInfo> ListOperationsOnlyAdd = new ObservableCollection<OperationInfo>();
        /// <summary>
        /// 当前用户可操作的权限
        /// </summary>
        public static ObservableCollection<OperationInfo> AllListOperations = new ObservableCollection<OperationInfo>();
        BackgroundWorker mBackgroundWorker = null;


        /// <summary>
        ///服务器已经绑定的策略
        /// </summary>
        public IList<CVoiceServerBindStrategy> lstPolicyBindding;
        public CVoiceServerBindStrategy mcurrentPoBind;

        public EncryptMainPage parentWin = null;
        private IList<ResourceInfo> mListResourcesInfo;
        #endregion
        public UC_EncryptionPolicyBindding()
        {
            InitializeComponent();
            mCurrentSelectIPSourceID = "";
            LLstVoiceServer = new List<CVoiceServer>();
            mListResourcesInfo = new List<ResourceInfo>();
            lstPolicyBindding = new List<CVoiceServerBindStrategy>();
            this.Loaded += UC_EncryptionPolicyBindding_Loaded;
        }

        void UC_EncryptionPolicyBindding_Loaded(object sender, RoutedEventArgs e)
        {
            ContextMenuService.SetContextMenu(LvStrategies, null);
            AllListOperations.Clear();
            ChangeLanguage();
            //LvVoiceServer.MouseDoubleClick += LvVoiceServer_MouseDoubleClick;
            LvVoiceServer.SelectionChanged += LvVoiceServer_SelectionChanged;
            parentWin.ShowStausMessage(CurrentApp.GetLanguageInfo("2403001", "Loading") + "....", true);
            mBackgroundWorker = new BackgroundWorker();
            mBackgroundWorker.DoWork += (s, de) =>
            {
                //获得用户可使用的按钮(操作权限)
                GetUserOpts();
            };
            mBackgroundWorker.RunWorkerCompleted += (s, de) =>
            {
                InitListResourceInfo();
                GetCanOperationVoiceServer();
                parentWin.ShowStausMessage(string.Empty, false);
            };
            mBackgroundWorker.RunWorkerAsync();
        }

        void LvVoiceServer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object o = LvVoiceServer.SelectedItem;
            if (o == null)
                return;
            CVoiceServer cvs = (o as ListViewItem).Content as CVoiceServer;
            if (cvs != null && cvs.NumEanbleEncryption == "1" && lstPolicyBindding.Where(p => p.CusFiled1 == cvs.IPResourceID).ToList().Count == 0)
                GetPoliciesByVoiceIPSource(cvs.IPResourceID);
        }

        private void LvVoiceServer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            object o = LvVoiceServer.SelectedItem;
            if (o == null)
                return;
            CVoiceServer cvs = (o as ListViewItem).Content as CVoiceServer;
            if (cvs != null && cvs.NumEanbleEncryption == "1" && lstPolicyBindding.Where(p => p.CusFiled1 == cvs.IPResourceID).ToList().Count == 0)
                GetPoliciesByVoiceIPSource(cvs.IPResourceID);
        }

        #region Override
        public override void ChangeLanguage()
        {
            base.ChangeLanguage();
            InitLvVoiceServerColumns();
            InitLvStrategiesColumns();
            BindContent();
            BindStrategyContextMenu();
            GetCanOperationVoiceServer();
            PopupPanel.ChangeLanguage();
        }

        #endregion

        /// <summary>
        /// 初始化录音服务器列信息
        /// </summary>
        private void InitLvVoiceServerColumns()
        {
            try
            {
                string[] lans = "240300001,240300002".Split(',');
                string[] cols = "VoiceServer,EanbleEncryption".Split(',');
                int[] colwidths = { 140, 120 };
                GridView columngv = new GridView();
                for (int i = 0; i < 2; i++)
                {
                    DataTemplate CellTemplate = (DataTemplate)Resources[cols[i]];
                    SetColumnGridView(cols[i], ref columngv, lans[i], cols[i], CellTemplate, colwidths[i]);
                }
                LvVoiceServer.View = columngv;
            }
            catch(Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SetColumnGridView(string columnname, ref GridView ColumnGridView, string langid, string diaplay, DataTemplate datatemplate, int width)
        {
            GridViewColumn gvc = new GridViewColumn();
            gvc.Header = CurrentApp.GetLanguageInfo(langid, diaplay);
            gvc.Width = width;
            gvc.HeaderStringFormat = columnname;
            if (datatemplate != null)
            {
                gvc.CellTemplate = datatemplate;
            }
            else
                gvc.DisplayMemberBinding = new Binding(columnname);
            ColumnGridView.Columns.Add(gvc);
        }

        #region VoiceServer Operation
        /// <summary>
        /// 获取录音服务器
        /// </summary>
        private void InitListResourceInfo()
        {
            mListResourcesInfo.Clear();
            WebRequest webRequest = new WebRequest();
            webRequest.Session = CurrentApp.Session;
            webRequest.Code = (int)S2400RequestCode.GetVoiceIP_Name201;
            Service24011Client client = new Service24011Client(
               WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
               WebHelper.CreateEndpointAddress(
                   CurrentApp.Session.AppServerInfo,
                   "Service24011"));
            //Service24011Client client = new Service24011Client();
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

        /// <summary>
        /// 能管理的录音服务器
        /// </summary>
        private void GetCanOperationVoiceServer()
        {
            try
            {
                if (mListResourcesInfo == null)
                    return;
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S2400RequestCode.GetResourceObjList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("221");
                //webRequest.ListData.Add("1");

                //Service24011Client client = new Service24011Client();
                Service24011Client client = new Service24011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo,"Service24011"));
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

                LvVoiceServer.Items.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    ListViewItem AddEncryptionObjectItem = new ListViewItem();
                    ContextMenu LocalContextMenu = new ContextMenu();
                    string[] strInfo = webReturn.ListData[i].Split('|');
                    if (strInfo.Count() == 2)
                    {
                        ResourceInfo ri = mListResourcesInfo.Where(p => p.ResourceID.ToString() == strInfo[0]).FirstOrDefault();
                        if (ri != null)
                        {
                            CVoiceServer cvs = new CVoiceServer();
                            cvs.VoiceServer = ri.ResourceName;
                            cvs.NumEanbleEncryption = strInfo[1];
                            cvs.IPResourceID = ri.ResourceID.ToString();
                            cvs.EanbleEncryption = strInfo[1] == "1" ? CurrentApp.GetLanguageInfo("240300003", "YES") : CurrentApp.GetLanguageInfo("240300004", "NO");
                            LLstVoiceServer.Add(cvs);

                            if (strInfo[1] == "1" && HasPermission(S2400Const.Opt_PolicyBinddingDisable))//已经设置加密绑定
                            {
                                MenuItem LocalMenuItem = new MenuItem();
                                SetMenuItem(ref LocalMenuItem, CurrentApp.GetLanguageInfo("FO2403002", "Deny Encryption Bind"), S2400Const.Opt_PolicyBinddingDisable.ToString(), strInfo[0] + "|" + cvs.VoiceServer);
                                LocalContextMenu.Items.Add(LocalMenuItem);
                            }
                            else if (strInfo[1] == "0" && HasPermission(S2400Const.OPT_PolicyBinddingEnable))
                            {
                                MenuItem LocalMenuItem = new MenuItem();
                                SetMenuItem(ref LocalMenuItem, CurrentApp.GetLanguageInfo("FO2403001", "Grant Encryption Bind"), S2400Const.OPT_PolicyBinddingEnable.ToString(), strInfo[0] + "|" + cvs.VoiceServer);
                                LocalContextMenu.Items.Add(LocalMenuItem);
                            }
                            AddEncryptionObjectItem.Content = cvs;
                            if (LocalContextMenu.Items.Count > 0) { ContextMenuService.SetContextMenu(AddEncryptionObjectItem, LocalContextMenu); }
                            LvVoiceServer.Items.Add(AddEncryptionObjectItem);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
        #endregion

        #region 操作权限
        /// <summary>
        /// 获得用户可操作权限
        /// </summary>
        private void GetUserOpts()
        {
            WebRequest webRequest = new WebRequest();
            webRequest.Code = (int)RequestCode.WSGetUserOptList;
            webRequest.Session = CurrentApp.Session;
            webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
            webRequest.ListData.Add("24");
            webRequest.ListData.Add("2403");
            Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                 WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
            WebReturn webReturn = client.DoOperation(webRequest);
            CurrentApp.MonitorHelper.AddWebReturn(webReturn);
            client.Close();
            if (!webReturn.Result)
            {
                ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                return;
            }
            ListOperationsOnlyAdd.Clear();
            AllListOperations.Clear();
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
                    AllListOperations.Add(optInfo);
                }
            }
        }
        #endregion

        /// <summary>
        /// 设置或者取消加密绑定
        /// </summary>
        /// <param name="serverip">IP资源ID|IP地址</param>
        /// <param name="enable">1：设置  0：取消</param>
        private void PolicyBinddingEnableOrDisenable(string serverip, string enable)
        {
            string messageBoxText = "";
            if (enable == "1")
            {
                messageBoxText = CurrentApp.GetLanguageInfo("240300024", "Confirm set encryption binding.");
            }
            else
            {
                messageBoxText = CurrentApp.GetLanguageInfo("240300025", "Confirm cancel the encryption binding.");                
            }
            MessageBoxButton button = MessageBoxButton.OKCancel;
            MessageBoxImage icon = MessageBoxImage.Warning;
            //显示消息框
            MessageBoxResult result = MessageBox.Show(messageBoxText, "2400", button, icon);
            //处理消息框信息
            switch (result)
            {
                case MessageBoxResult.Cancel:
                    return;
                    break;
            }

            try
            {
                WebReturn webReturn = null;
                string [] arrip =  serverip.Split('|');
                if(arrip.Count()!=2){return;}
                string ipsource = arrip[0];
                string ip = arrip[1];
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S2400RequestCode.SetPolicyBindding;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(ipsource);
                webRequest.ListData.Add(enable);
                webRequest.ListData.Add(ip);
                //Service24011Client client = new Service24011Client();
                Service24011Client client = new Service24011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service24011"));
                webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                client.Close();

                if (enable == "1")
                    GetPoliciesByVoiceIPSource(ipsource);
                else
                {
                    ContextMenuService.SetContextMenu(LvStrategies, null); 
                    lstPolicyBindding.Clear();
                    LvStrategies.Items.Clear();
                }
                GetCanOperationVoiceServer();//设置后刷新
                string vsIP = LLstVoiceServer.Where(p => p.IPResourceID == ipsource).FirstOrDefault().VoiceServer;
                string msg = "";
                if (!SendMsgToService00(vsIP, ref msg))
                {
                    if (enable == "1")
                        CurrentApp.WriteOperationLog("2403001", ConstValue.OPT_RESULT_FAIL, "");
                    else
                        CurrentApp.WriteOperationLog("2403002", ConstValue.OPT_RESULT_FAIL, "");
                    ShowException(string.Format("{0}\t{1}", CurrentApp.GetLanguageInfo("240300022", "Message sent failure."), msg));
                }
                else
                {
                    if (enable == "1")
                    {
                        string strLog = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO2403001")), ip);
                        CurrentApp.WriteOperationLog("2403001", ConstValue.OPT_RESULT_SUCCESS, strLog);
                    }
                    else
                    {
                        string strLog = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO2403002")), ip);
                        CurrentApp.WriteOperationLog("2403002", ConstValue.OPT_RESULT_SUCCESS, strLog);
                    }
                }
            }
            catch (Exception ex)
            {
                parentWin.ShowStausMessage(string.Empty, false);
                ShowException(ex.Message);
            }
        }

        #region 策略绑定 LvStrategies相关
        /// <summary>
        /// 初始化策略listview
        /// </summary>
        private void InitLvStrategiesColumns()
        {
            try
            {
                string[] lans = "240300015,240300016,240300017,240300018,240300019".Split(',');
                string[] cols = "CusFiled3,DurationbeginStr,DurationendStr,CusFiled2,SettedtimeStr".Split(',');
                int[] colwidths = { 200, 150, 150, 120, 150 };
                GridView columngv = new GridView();
                for (int i = 0; i < 5; i++)
                {
                    DataTemplate CellTemplate = (DataTemplate)Resources[cols[i]];
                    SetColumnGridView(cols[i], ref columngv, lans[i], cols[i], CellTemplate, colwidths[i]);
                }
                LvStrategies.View = columngv;
            }
            catch(Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 根据录音服务器IP的资源编码 获取绑定的策略
        /// </summary>
        /// <param name="stripSource"></param>
        public void GetPoliciesByVoiceIPSource(string stripSource)
        {
            lstPolicyBindding.Clear();
            mCurrentSelectIPSourceID = stripSource;
            Service24021Client client = null;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S2400RequestCode.GetPoliciesByVoiceIPSource;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(stripSource);
                client = new Service24021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                       WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service24021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, CurrentApp.GetLanguageInfo(webReturn.Code.ToString(), webReturn.Message)));
                    return;
                }
                if (webReturn.DataSetData.Tables.Count <= 0)
                {
                    return;
                }
                DataTable dt = webReturn.DataSetData.Tables[0];
                CVoiceServerBindStrategy policyItem = null;
                foreach (DataRow row in dt.Rows)
                {
                    if (row["PONAME"] == null || string.IsNullOrEmpty(row["PONAME"].ToString()))
                        continue;
                    policyItem = new CVoiceServerBindStrategy();
                    policyItem.Durationbegin = Convert.ToInt64(GetObjectStr(row["C004"]));
                    policyItem.Durationend = Convert.ToInt64(GetObjectStr(row["C005"]));
                    policyItem.Settedtime = Convert.ToInt64(GetObjectStr(row["C007"]));
                    policyItem.Objecttype = GetObjectStr(row["C001"]);
                    policyItem.Objectvalue = GetObjectStr(row["C002"]);
                    policyItem.Bindingpolicyid = Convert.ToInt64(GetObjectStr(row["C003"]));
                    policyItem.DurationbeginStr = CommonFunctions.StringToDateTime(row["C004"].ToString()).ToString();

                    if (policyItem.Durationend > 20990000000000)
                        policyItem.DurationendStr = CurrentApp.GetLanguageInfo("240300032", "Never expires");
                    else
                        policyItem.DurationendStr= CommonFunctions.StringToDateTime(row["C005"].ToString()).ToString();

                    policyItem.Setteduserid = Convert.ToInt64(GetObjectStr(row["C006"]));
                    policyItem.SettedtimeStr = CommonFunctions.StringToDateTime(row["C007"].ToString()).ToString();
                    policyItem.Grantencryption = GetObjectStr(row["C008"]);
                    policyItem.CusFiled1 = GetObjectStr(row["C009"]);
                    policyItem.CusFiled2 = GetObjectStr(row["C010"]);
                    policyItem.CusFiled3 = GetObjectStr(row["C011"]);

                    lstPolicyBindding.Add(policyItem);
                }
                BindStrategyContextMenu();
            }
            catch (Exception ex)
            {
                ShowException("Failed." + ex.Message);
            }
            finally
            {
                if (client != null)
                {
                    if (client.State == System.ServiceModel.CommunicationState.Opened)
                    {
                        client.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 策略listview右键菜单
        /// </summary>
        private void BindStrategyContextMenu()
        {
            LvStrategies.Items.Clear();
            if (lstPolicyBindding == null)
                return;
            lstPolicyBindding = lstPolicyBindding.OrderBy(P => P.Durationbegin).ToList();
            for (int i = 0; i < lstPolicyBindding.Count;i++ )
            {
                ListViewItem AddObjectItem = new ListViewItem();
                AddObjectItem.Content = lstPolicyBindding[i];
                ContextMenu LocalContextMenu = new ContextMenu();
                foreach (OperationInfo OPInfo in AllListOperations.Where(p => p.ID > 2403002 && p.ID < 2403007).ToList())//已经绑定有策略
                {
                    string tag = "";
                    if (OPInfo.ID == S2400Const.OPT_PolicyInsert)//插入策略.先不管
                    { continue; }
                    if (OPInfo.ID == S2400Const.OPT_PolicyAppend)//追加策略
                    {
                        if (lstPolicyBindding[i].Durationbegin > 20990000000000)//当做不失效
                        { continue; }
                    }
                    tag = lstPolicyBindding[i].Durationbegin.ToString() + "|" 
                        + lstPolicyBindding[i].Bindingpolicyid.ToString() + "|"
                        + lstPolicyBindding[i].CusFiled1;
                    MenuItem LocalMenuItem = new MenuItem();
                    SetMenuItem(ref LocalMenuItem, CurrentApp.GetLanguageInfo("FO" + OPInfo.ID.ToString(), OPInfo.ID.ToString()),
                         OPInfo.ID.ToString(),tag);
                    LocalContextMenu.Items.Add(LocalMenuItem);
                }
                ContextMenuService.SetContextMenu(AddObjectItem, LocalContextMenu);
                LvStrategies.Items.Add(AddObjectItem);
            }
            if (lstPolicyBindding.Count == 0 && HasPermission(S2400Const.OPT_PolicyAppend))//开启了策略绑定，但是没有绑定策略
            {
                ContextMenu LocalContextMenuTop = new ContextMenu();
                MenuItem LocalMenuItemAdd = new MenuItem();
                SetMenuItem(ref LocalMenuItemAdd, CurrentApp.GetLanguageInfo("FO2403003", "Additional Binding Strategy"),
                     S2400Const.OPT_PolicyAppend.ToString(), S2400Const.OPT_PolicyAppend.ToString());
                LocalContextMenuTop.Items.Add(LocalMenuItemAdd);
                ContextMenuService.SetContextMenu(LvStrategies, LocalContextMenuTop);
            }
            else { ContextMenuService.SetContextMenu(LvStrategies, null); }
        }

        /// <summary>
        /// 追加策略
        /// </summary>
        /// <param name="strtag">开始时间num|策略编码|服务器IP资源编码</param>
        private void EncrytionObjectAppendStrategy(string strtag)
        {
            string[] strIPAndStartNum = strtag.Split('|');
            if (strIPAndStartNum.Count() == 3)
            {
                PolicyInfoInList selPolicy = new PolicyInfoInList();
                GetCurrentPolicy(ref selPolicy, strIPAndStartNum);
                mcurrentPoBind = lstPolicyBindding.Where(p => p.Durationbegin.ToString() == strIPAndStartNum[0] &&
                    p.Bindingpolicyid.ToString() == strIPAndStartNum[1] &&
                    p.CusFiled1 == strIPAndStartNum[2]).FirstOrDefault();
                AppendOrInsertBindStrategy abs = new AppendOrInsertBindStrategy();
                abs.CurrentApp = CurrentApp;
                abs.mOperationType = "1";
                abs.mParentpage = this;
                PopupPanel.Content = abs;
                PopupPanel.Title = CurrentApp.GetLanguageInfo("240300008", "Binding Key Strategy") + "";
                PopupPanel.IsOpen = true;
            }
            else if (strIPAndStartNum.Count() == 1)
            {
                AppendOrInsertBindStrategy abs = new AppendOrInsertBindStrategy();
                abs.CurrentApp = CurrentApp;
                abs.mOperationType = "1";
                abs.mParentpage = this;
                PopupPanel.Content = abs;
                PopupPanel.Title = CurrentApp.GetLanguageInfo("240300008", "Binding Key Strategy") + "";
                PopupPanel.IsOpen = true;
            }
        }

        /// <summary>
        /// 删除策略
        /// </summary>
        /// <param name="strtag">开始时间num|策略编码|服务器IP资源编码</param>
        private void EncrytionObjectStrategyDelete(string strtag)
        {
            try
            {
                string[] strIPAndStartNum = strtag.Split('|');
                string temppolicname = "";
                if (strIPAndStartNum.Count() == 3)
                {
                    temppolicname = lstPolicyBindding.Where(p => p.Bindingpolicyid == long.Parse(strIPAndStartNum[1])).First().CusFiled3;
                    WebRequest webRequest = new WebRequest();
                    webRequest.Code = (int)S2400RequestCode.DeleteBindedStragegy;
                    webRequest.Session = CurrentApp.Session;
                    webRequest.ListData.Add(strIPAndStartNum[0]);
                    webRequest.ListData.Add(strIPAndStartNum[1]);
                    webRequest.ListData.Add(strIPAndStartNum[2]);
                    //Service24011Client client = new Service24011Client();
                    Service24011Client client = new Service24011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service24011"));
                    WebReturn webReturn = client.DoOperation(webRequest);
                    CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        if (webReturn.Message == S2400Const.Msg_StragegyExit)
                            ShowInformation(CurrentApp.GetLanguageInfo("240300023", "Strategy Is Used,Can not Delete."));
                        else
                            ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return;
                    }
                    else
                    {
                        string strLog = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO2403005")), temppolicname);
                        CurrentApp.WriteOperationLog("2403005", ConstValue.OPT_RESULT_SUCCESS, strLog);

                        CVoiceServerBindStrategy podel = lstPolicyBindding.Where(p => p.Durationbegin.ToString() == strIPAndStartNum[0]
                            && p.Bindingpolicyid.ToString() == strIPAndStartNum[1]).FirstOrDefault();
                        lstPolicyBindding.Remove(podel);
                        if (podel!=null)
                            BindStrategyContextMenu();
                    }
                }
            }
            catch (Exception ex)
            {
                CurrentApp.WriteOperationLog("2403005", ConstValue.OPT_RESULT_FAIL, "");
                ShowException(ex.Message);
            }
        }

        /// <summary>
        /// 修改策略
        /// </summary>
        /// <param name="strtag">开始时间num|策略编码|服务器IP资源编码</param>
        private void EncrytionObjectStrategyModify(string strtag)
        {
            string[] strIPAndStartNum = strtag.Split('|');
            if (strIPAndStartNum.Count() == 3)
            { 
                PolicyInfoInList selPolicy = new PolicyInfoInList();
                GetCurrentPolicy(ref selPolicy, strIPAndStartNum);
                mcurrentPoBind = lstPolicyBindding.Where(p => p.Durationbegin.ToString() == strIPAndStartNum[0] &&
                    p.Bindingpolicyid.ToString() == strIPAndStartNum[1] &&
                    p.CusFiled1 == strIPAndStartNum[2]).FirstOrDefault();

                if (selPolicy.PolicyID != null && mcurrentPoBind.Bindingpolicyid!=null)
                {
                    //if (mcurrentPoBind != null && mcurrentPoBind.Durationend > Convert.ToInt64(DateTime.UtcNow.ToString("yyyyMMddHHmmss")))
                    //{
                    AppendOrInsertBindStrategy02 abs2 = new AppendOrInsertBindStrategy02();
                    abs2.CurrentApp = CurrentApp;
                    abs2.mFirstPage = this;
                    abs2.selPolicyInfo = selPolicy;
                    abs2.lstAllPolicies = lstAllPolicies;
                    abs2.mOpType = "2";
                    PopupPanel.Content = abs2;
                    PopupPanel.Title = CurrentApp.GetLanguageInfo("FO2403006", "Modify Binding Time");
                    PopupPanel.IsOpen = true;
                    //}
                    //else
                    //    ShowInformation(CurrentApp.GetLanguageInfo("240300021", "Cannot be modified"));
                }
            }
        }

        private void GetCurrentPolicy(ref PolicyInfoInList selPolicy, string[] strIPAndStartNum)
        {
            Service24021Client client = null;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S2400RequestCode.GetAllPolicies;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                client = new Service24021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                       WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service24021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, CurrentApp.GetLanguageInfo(webReturn.Code.ToString(), webReturn.Message)));
                    return;
                }
                if (webReturn.DataSetData.Tables.Count <= 0)
                {
                    return;
                }
                DataTable dt = webReturn.DataSetData.Tables[0];
                PolicyInfoInList policyItem = null;
                foreach (DataRow row in dt.Rows)
                {
                    policyItem = new PolicyInfoInList();
                    policyItem.PolicyID = row["C001"].ToString();
                    policyItem.PolicyName = row["C002"].ToString();
                    //= row["004"].ToString();
                    string strType = row["C004"].ToString();
                    policyItem.PolicyType = strType == "U" ? CurrentApp.GetLanguageInfo("240300027", "Custom (user input)") : CurrentApp.GetLanguageInfo("240300028", "Periodic update key (randomly generated)");
                    if (strType == "C")
                    {
                        string strOccursFrequency = row["C007"].ToString();
                        switch (strOccursFrequency)
                        {
                            case "D":
                                policyItem.PolicyOccursFrequency = CurrentApp.GetLanguageInfo("240300029", "Day");
                                break;
                            case "W":
                                policyItem.PolicyOccursFrequency = CurrentApp.GetLanguageInfo("240300030", "Week");
                                break;
                            case "M":
                                policyItem.PolicyOccursFrequency = CurrentApp.GetLanguageInfo("240300031", "Month");
                                break;
                            case "U":
                                policyItem.PolicyOccursFrequency = row["C010"].ToString() + CurrentApp.GetLanguageInfo("240300029", "Day");
                                break;
                        }
                    }
                    else
                    {
                        policyItem.PolicyOccursFrequency = string.Empty;
                    }
                    policyItem.PolicyStartTime = CommonFunctions.StringToDateTime(row["C008"].ToString()).ToString();
                    long longTime = 0;
                    long.TryParse(row["C008"].ToString(), out longTime);
                    policyItem.PolicyStartTimeNumber = longTime;
                    if (row["C009"].ToString() != "20991231235959")
                    {
                        policyItem.PolicyEndTime = CommonFunctions.StringToDateTime(row["C009"].ToString()).ToString();
                    }
                    else
                    {
                        policyItem.PolicyEndTime = CurrentApp.GetLanguageInfo("240300032", "Never expires");
                    }
                    long.TryParse(row["C009"].ToString(), out longTime);
                    policyItem.PolicyEndTimeNumber = longTime;
                    policyItem.PolicyIsEnabled = row["C003"].ToString();
                    if (strType == "C")
                    {
                        policyItem.IsStrongPwd = row["C012"].ToString() == "1" ? CurrentApp.GetLanguageInfo("240300003", "Yes") : string.Empty;
                    }
                    else
                    {
                        policyItem.IsStrongPwd = string.Empty;
                    }
                    long lNow = 0;
                    CommonFunctions.DateTimeToNumber(DateTime.Now, ref lNow);
                    if (policyItem.PolicyID == strIPAndStartNum[1])
                    {
                        selPolicy = policyItem;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException("Failed." + ex.Message);
            }
            finally
            {
                if (client != null)
                {
                    if (client.State == System.ServiceModel.CommunicationState.Opened)
                    {
                        client.Close();
                    }
                }
            }
        }
        #endregion

        #region OTHER
        private void SetLvServerLang()
        {
            //foreach (ListViewItem item in LvVoiceServer.Items)
            //{
            //    ((CVoiceServer)item.Content).
            //}
        }

        private void BindContent()
        {
            PanelVoiceServer.Title = CurrentApp.GetLanguageInfo("240300001", "Voice Server");
            PanelBindStrategy.Title = CurrentApp.GetLanguageInfo("240300005", "Strategy Bindding");
        }

        public void SetPanelOpenState(bool isOpen)
        {
            PopupPanel.IsOpen = isOpen;
        }

        private bool HasPermission(long optid)
        {
            if (AllListOperations.Where(p => p.ID == optid).ToList().Count() > 0)
                return true;
            else
                return false;
        }

        private void SetMenuItem(ref MenuItem LocalMenuItem, string header, string datacontext, string tag)
        {
            LocalMenuItem.Header = header;
            LocalMenuItem.DataContext = datacontext;
            LocalMenuItem.Tag = tag;
            LocalMenuItem.VerticalContentAlignment = VerticalAlignment.Center;
            LocalMenuItem.VerticalContentAlignment = VerticalAlignment.Center;
            LocalMenuItem.FontFamily = new FontFamily("SimSun");
            LocalMenuItem.FontSize = 12;
            LocalMenuItem.Click += new RoutedEventHandler(LocalRightClickedMenu_Click);
        }

        /// <summary>
        /// 右键菜单点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LocalRightClickedMenu_Click(object sender, RoutedEventArgs e)
        {
            string LStrMenuOperation = string.Empty;
            string LStrMenuDataContent = string.Empty;
            string LStrMenuTag = string.Empty;
            try
            {
                MenuItem ClickedItem = (MenuItem)sender;

                LStrMenuDataContent = ClickedItem.DataContext.ToString();
                LStrMenuTag = ClickedItem.Tag.ToString();
                LStrMenuOperation = LStrMenuDataContent;

                if (LStrMenuOperation == S2400Const.OPT_PolicyBinddingEnable.ToString()) { PolicyBinddingEnableOrDisenable(LStrMenuTag, "1"); }//设置加密绑定
                if (LStrMenuOperation == S2400Const.Opt_PolicyBinddingDisable.ToString()) { PolicyBinddingEnableOrDisenable(LStrMenuTag, "0"); }//取消加密绑定
                if (LStrMenuOperation == S2400Const.OPT_PolicyAppend.ToString()) { EncrytionObjectAppendStrategy(LStrMenuTag); }//追加
                if (LStrMenuOperation == S2400Const.OPT_PolicyInsert.ToString()) { }//插入
                if (LStrMenuOperation == S2400Const.OPT_PolicyDelete.ToString()) { EncrytionObjectStrategyDelete(LStrMenuTag); }//删除
                if (LStrMenuOperation == S2400Const.OPT_PolicyModify.ToString()) { EncrytionObjectStrategyModify(LStrMenuTag); }//修改

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private string GetObjectStr(object obj)
        {
            if (obj != null)
                return obj.ToString();
            else
                return "";
        }

        public bool SendMsgToService00(string voiceIP,ref string msg)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S2400RequestCode.SendMsgToService00;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(voiceIP);
                //Service24011Client client = new Service24011Client();
                Service24011Client client = new Service24011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service24011"));
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    msg = webReturn.Message;
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return false;
            }
        }
        #endregion
    }
}
