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
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.UMP.Controls.Wcf11012;

namespace UMPS1103
{
    /// <summary>
    /// UCModifyAgentObj.xaml 的交互逻辑
    /// </summary>
    public partial class UCModifyAgentObj 
    {
        #region Members

        public UCAgentMaintenance PageParent;
        public ObjectItem AgentItem;

        private bool mIsInited;
        private ObjectItem mRootItem;
        private List<ObjectItem> mListCtlObjects;
        private List<long> mListCtledUserIDs;

        #endregion

        public UCModifyAgentObj()
        {

            InitializeComponent();

            mRootItem = new ObjectItem();
            mListCtlObjects = new List<ObjectItem>();
            mListCtledUserIDs = new List<long>();

            Loaded += UCModifyAgentObj_Loaded;
            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
        }

        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            var panel = Parent as PopupPanel;
            if (panel == null) { return; }
            panel.IsOpen = false;
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            ModifyAgentOrg();
        }

        private void ModifyAgentOrg()
        {
            if (AgentItem == null) { return; }
            var agentInfo = AgentItem.Data as AgentInfo;
            List<string> listObjectState = new List<string>();
            ModifyAgentOrg(mRootItem, ref listObjectState);

            int errCode = 0;
            string strMsg = string.Empty;
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (s, de) =>
            {
                try
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)S1103Codes.ModifyAgentORGC;
                    webRequest.ListData.Add(agentInfo.SerialID.ToString());
                    //// webRequest.ListData.Add(listObjectState.Count().ToString());
                    if (listObjectState.Count >= 2)
                    {
                      //  MessageBox.Show("您不能选择多个机构。。");
                        MessageBox.Show(CurrentApp.GetLanguageInfo("S1103039", "您不能选择多个机构。。"));
                        return;
                    }
                    else
                    {
                        for (int i = 0; i < listObjectState.Count(); i++)
                        {
                            webRequest.ListData.Add(listObjectState[i]);
                        }
                    }

                    //    Service11031Client client = new Service11031Client();
                    Service11031Client client = new Service11031Client(
                        WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                        WebHelper.CreateEndpointAddress(
                            CurrentApp.Session.AppServerInfo,
                            "Service11031"));
                    WebHelper.SetServiceClient(client);
                    WebReturn webReturn = client.DoOperation(webRequest);
                    client.Close();

                    if (!webReturn.Result)
                    {
                        errCode = webReturn.Code;
                        strMsg = webReturn.Message;
                        return;
                    }
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

        private void ModifyAgentOrg(ObjectItem objItem, ref List<string> listObjectState)
        {
            for (int i = 0; i < objItem.Children.Count; i++)
            {
                ObjectItem child = objItem.Children[i] as ObjectItem;
                if (child == null) { continue; }
                if (child.IsChecked == true || child.IsChecked == null)
                {
                    listObjectState.Add(string.Format("{0}{1}", child.ObjID, ConstValue.SPLITER_CHAR));

                }

                ModifyAgentOrg(child, ref listObjectState);
            }
        }

        private void UCModifyAgentObj_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                TvManagers.ItemsSource = mRootItem.Children;

                mRootItem.Children.Clear();
                mListCtlObjects.Clear();
                mListCtledUserIDs.Clear();
                ChangeLanguage();
                if (PageParent != null)
                {
                    PageParent.SetBusy(true, string.Empty);
                }
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadCtledUserIDs();
                    LoadCtlOrg(mRootItem, -1);
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    if (PageParent != null)
                    {
                        PageParent.SetBusy(false, string.Empty);
                    }
                    if (mRootItem.Children.Count > 0)
                    {
                        mRootItem.Children[0].IsExpanded = true;
                    }
                    mRootItem.IsChecked = false;
                    SetCheckState();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


        private void LoadCtledUserIDs()
        {
            try
            {
                if (AgentItem == null) { return; }
                long agentObjID = AgentItem.ObjID;
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1103Codes.GetAgentCtledUserIDs;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add(agentObjID.ToString());
                Service11031Client client = new Service11031Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11031"));
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
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strID = webReturn.ListData[i];
                    long id;
                    if (long.TryParse(strID, out id))
                    {
                        mListCtledUserIDs.Add(id);
                    }
                }

                CurrentApp.WriteLog("LoadCtledUserIDs", string.Format("End.\t{0}", mListCtledUserIDs.Count));
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
                int count = 0;
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

                    LoadCtlOrg(parentItem, item.ObjID);
                    //     LoadCtlUser(item, item.ObjID);

                    Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
                    mListCtlObjects.Add(parentItem);
                    count++;
                }

                CurrentApp.WriteLog("LoadCtlOrg", string.Format("End.\t{0}\t{1}", parentID, count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SetCheckState()
        {
            try
            {
                for (int i = 0; i < mListCtledUserIDs.Count; i++)
                {
                    long id = mListCtledUserIDs[i];

                    var temp = mListCtlObjects.FirstOrDefault(u => u.ObjID == id);
                    if (temp != null)
                    {
                        temp.IsChecked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
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
                    parent.Title = CurrentApp.GetLanguageInfo("S1103006", " Modify AgentORG ");
                }

                //InitColumns();
                //CreatButton();            
                BtnConfirm.Content = CurrentApp.GetLanguageInfo("S1103003", "Confirm");
                BtnClose.Content = CurrentApp.GetLanguageInfo("S1103004", "Close");

            }
            catch { }

        }

        #endregion
    }
}
