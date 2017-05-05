using Common11031;
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
using UMPS1103.Models;
using UMPS1103.Wcf11031;
using VoiceCyber.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS1103
{
    /// <summary>
    /// UCAgentModify.xaml 的交互逻辑
    /// </summary>
    public partial class UCAgentModify 
    {
        #region Members

        public UCAgentMaintenance PageParent;
        public bool IsModify;
        public ObjectItem AgentItem;
        public ObjectItem ParentItem;
        List<string> strList = new List<string>();
        private bool mIsInited;

        #endregion


        public UCAgentModify()
        {
            InitializeComponent();

            Loaded += UCAgentModify_Loaded;          
            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
        }

        private void GetAllAgent()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1103Codes.GetAllAgent;
                //Service11031Client client = new Service11031Client();
                Service11031Client client =
                    new Service11031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                        WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11031"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result) 
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;                  
                }
                for (int i = 0; i < webReturn.ListData.Count; i++) 
                {
                    strList.Add(webReturn.ListData[i]);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


        void UCAgentModify_Loaded(object sender, RoutedEventArgs e)
        {
            if (!mIsInited)
            {
                Init();
                GetAllAgent();
                ChangeLanguage();
                mIsInited = true;
            }
        }

        private void Init()
        {
            try
            {
                if (!IsModify)
                {
                    if (ParentItem == null) { return; }
                    TxtAgentID.Text = string.Empty;
                 //   TxtAgentID.IsReadOnly = false;
                    TxtAgentName.Text = string.Empty;
                    TxtAgentOrg.Text = ParentItem.Name;
                    RadioStateEnable.IsChecked = true;
                    RadioStateDisable.IsChecked = false;
                    ComboBoxTenure.SelectedIndex = 0;
               
                }
                else
                {
                    if (AgentItem == null) { return; }
                    TxtAgentID.Text = AgentItem.Name;         
                  //  TxtAgentID.IsReadOnly = true;
                    TxtAgentName.Text = AgentItem.FullName;
                    var parentItem = AgentItem.Parent as ObjectItem;
                    if (parentItem != null)
                    {
                        TxtAgentOrg.Text = parentItem.Name;
                    }
                    RadioStateEnable.IsChecked = (AgentItem.State & 2) == 0;
                    RadioStateDisable.IsChecked = (AgentItem.State & 2) != 0;
                    ComboBoxTenure.SelectedIndex = AgentItem.AgentTenure;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


        #region Operations

        private void AddAgent()
        {
            try
            {
                AgentInfo agentInfo = new AgentInfo();
                if (string.IsNullOrEmpty(TxtAgentID.Text))
                {
                    ShowException(string.Format("AgentID empty!"));
                    return;
                }
                if (TxtAgentID.Text.Length > 128)
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("S1103040", "Add content should not exceed 120 characters."));
                    return;                
                }
                else 
                {
                    agentInfo.AgentID = TxtAgentID.Text;
                }
                if (TxtAgentName.Text.Length > 128)
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("S1103040", "Add content should not exceed 120 characters."));
                    return;    
                }
                else
                {
                    agentInfo.AgentName = TxtAgentName.Text;
                }
                if (ParentItem == null) { return; }
                agentInfo.OrgID = ParentItem.ObjID;
                agentInfo.State = RadioStateEnable.IsChecked == true ? 0 : 2;
                agentInfo.Tenure = ComboBoxTenure.SelectedIndex;
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
                        webRequest.ListData.Add("1");
                        webRequest.ListData.Add(strInfo);
                        // Service11031Client client = new Service11031Client();
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
                        CurrentApp.WriteLog("AddAgent", string.Format("End.\t{0}", strSerialID));
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
                    ShowInformation(string.Format("Add agnet successful!"));

                    if (PageParent != null)
                    {
                        PageParent.ReloadData();
                    }
                    var panel = Parent as PopupPanel;
                    if (panel != null)
                    {
                        panel.IsOpen = false;
                    }
                };
                worker.RunWorkerAsync();
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
                if (AgentItem == null) { return; }
                var agentInfo = AgentItem.Data as AgentInfo;
                if (agentInfo == null) { return; }
                if (string.IsNullOrEmpty(TxtAgentID.Text))
                {
                    ShowException(string.Format("AgentID empty!"));
                    return;
                }
                if (TxtAgentID.Text.Length > 128)
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("S1103040", "Add content should not exceed 120 characters."));
                    return;    
                }
                else
                {
                    agentInfo.AgentID = TxtAgentID.Text;
                }
                if (TxtAgentName.Text.Length > 128)
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("S1103040", "Add content should not exceed 120 characters."));
                    return; 
                }
                else
                {
                    agentInfo.AgentName = TxtAgentName.Text;
                }
                agentInfo.State = RadioStateEnable.IsChecked == true ? 1 : 2;
                agentInfo.Tenure = ComboBoxTenure.SelectedIndex;
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
                        webRequest.ListData.Add("3");
                        webRequest.ListData.Add(strInfo);
                        //Service11031Client client = new Service11031Client();
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
                        CurrentApp.WriteLog("ModifyAgent", string.Format("End.\t{0}", strSerialID));
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
                    ShowInformation(string.Format("Modify agnet successful!"));

                    if (PageParent != null)
                    {
                        PageParent.ReloadData();
                    }
                    var panel = Parent as PopupPanel;
                    if (panel != null)
                    {
                        panel.IsOpen = false;
                    }
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            var panel = Parent as PopupPanel;
            if (panel == null) { return; }
            panel.IsOpen = false;
        }

        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (IsModify)
            {
                if (CheckAgentInfo())
                {
                ModifyAgent();
                }
            }
            else
            {
                AddAgent();
            }
        }
        private bool CheckAgentInfo()
        {
            if (AgentItem == null) { return false; }
            var agentInfo = AgentItem.Data as AgentInfo;
            if (agentInfo == null) { return false; }
             string strAgentNameID=agentInfo.AgentID;
            string agentID = TxtAgentID.Text.Trim();
            if (agentID == string.Empty)
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("S1103022", "AgentID is empty"));
                return false;
            }
            int AgentNum = 0;

            foreach (string a in strList)
            {
                if (a == agentInfo.AgentID)
                {
                    AgentNum++;
                }
            }
                               
                if (strAgentNameID == agentID)
                {
                    if (AgentNum > 1)
                    {
                        CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("S1103034", "AgentID is not available"));
                        return false;
                    }
                }
                else
                {
                    if (AgentNum > 0)
                    {
                        CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("S1103034", "AgentID is not available"));
                        return false;
                    }
                }
                return true;
            }
         
    

        #region ChangLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.Title = CurrentApp.GetLanguageInfo("S1103005", "Agent Modify");
                }

                //InitColumns();
                //CreatButton();
                LbAgentID.Text = CurrentApp.GetLanguageInfo("S1103007", "Agent ID");
                LbAgentName.Text = CurrentApp.GetLanguageInfo("S1103008", "Agent Name");
                LbAgentOrg.Text = CurrentApp.GetLanguageInfo("S1103006", "Orgnization");
                LbAgentState.Text = CurrentApp.GetLanguageInfo("S1103011", "State");
                RadioStateEnable.Content = CurrentApp.GetLanguageInfo("S1103009", "Enable");
                RadioStateDisable.Content = CurrentApp.GetLanguageInfo("S1103010", "Disable");
                BtnConfirm.Content = CurrentApp.GetLanguageInfo("S1103003", "Confirm");
                BtnClose.Content = CurrentApp.GetLanguageInfo("S1103004", "Close");
                ComboBoxItemLockTenure0.Content = CurrentApp.GetLanguageInfo("S1103035", "Newbie");
                ComboBoxItemLockTenure1.Content = CurrentApp.GetLanguageInfo("S1103036", "Junior");
                ComboBoxItemLockTenure2.Content = CurrentApp.GetLanguageInfo("S1103037", "Senior");
                LabelAgentTenure.Text = CurrentApp.GetLanguageInfo("S1103038", "Agent Tenure");
            }
            catch { }

        }

        #endregion
    }
}
