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
using UMPS3102.Models;
using UMPS3102.Wcf31021;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31021;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS3102
{
    /// <summary>
    /// InspectorConditionTree.xaml 的交互逻辑
    /// </summary>
    public partial class InspectorConditionTree
    {
        public UserControl UC;

        public UCQueryCondition PageParent;

        public ObjectItem mRootItem_Inspector;

        public List<ObjectItem> mListSelectedObjects_Inspector;

        private BackgroundWorker mWorker;

        public InspectorConditionTree()
        {
            mRootItem_Inspector = new ObjectItem();
            mListSelectedObjects_Inspector = new List<ObjectItem>();
            InitializeComponent();
            Loaded += InspectorList_Loaded;
        }

        //加载质检员
        void InspectorList_Loaded(object sender, RoutedEventArgs e)
        {
            BtnConfirm.Content = CurrentApp.GetLanguageInfo("31020", "Confirm");
            BtnClose.Content = CurrentApp.GetLanguageInfo("31021", "Close");
            InspectorTree.ItemsSource = mRootItem_Inspector.Children;
            mRootItem_Inspector.Children.Clear();
            mWorker = new BackgroundWorker();
            mWorker.DoWork += (s, de) =>
            {
                InitInspectorParents();
            };
            mWorker.RunWorkerCompleted += (s, re) =>
            {
                mWorker.Dispose();
                mRootItem_Inspector.IsChecked = false;
                if (mRootItem_Inspector.Children.Count > 0)
                {
                    mRootItem_Inspector.Children[0].IsExpanded = true;
                }
            };
            mWorker.RunWorkerAsync();
        }

        public void GetCheckedInspector()
        {
            try
            {
                mListSelectedObjects_Inspector.Clear();
                GetCheckedInspector(mRootItem_Inspector);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void GetCheckedInspector(ObjectItem parentItem)
        {
            try
            {
                for (int i = 0; i < parentItem.Children.Count; i++)
                {
                    ObjectItem item = parentItem.Children[i] as ObjectItem;
                    if (item != null)
                    {
                        if (item.ObjType == ConstValue.RESOURCE_ORG)//
                        {
                            if (item.IsChecked == false) { continue; }
                            GetCheckedInspector(item);
                        }
                        else
                        {
                            if (item.IsChecked == true)
                            {
                                mListSelectedObjects_Inspector.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitInspectorParents()
        {
            InitControlOrgsOfInspector(mRootItem_Inspector, "-1");
        }

        private void InitControlOrgsOfInspector(ObjectItem parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.GetControlOrgInfoList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                Service31021Client client = new Service31021Client(
                     WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                     WebHelper.CreateEndpointAddress(
                         CurrentApp.Session.AppServerInfo,
                         "Service31021"));
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
                    if (arrInfo.Length < 2) { continue; }
                    string strID = arrInfo[1];
                    string strName = arrInfo[0];
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_ORG;
                    item.ObjID = Convert.ToInt64(strID);
                    item.Name = strName;
                    item.Data = strInfo;
                    item.Description = string.Format("{0}", strName);
                    if (strID == ConstValue.ORG_ROOT.ToString())
                    {
                        item.Icon = "Images/rootorg.ico";
                    }
                    else
                    {
                        item.Icon = "Images/org.ico";
                    }

                    InitControlOrgsOfInspector(item, strID);
                    InitInspector(item, strID);
                    AddChildObject(parentItem, item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitInspector(ObjectItem parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.GetInspectorList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                Service31021Client client = new Service31021Client(
                     WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                     WebHelper.CreateEndpointAddress(
                         CurrentApp.Session.AppServerInfo,
                         "Service31021"));
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
                    item.Name = strName;
                    item.FullName = strFullName;
                    item.Data = strInfo;
                    item.Description = string.Format("{0}({1})", strFullName, strName);
                    item.Icon = "Images/inspector.ico";
                    AddChildObject(parentItem, item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #region 其他
        private void AddChildObject(ObjectItem parentItem, ObjectItem item)
        {
            Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));//这句话得理解下 
        }
        #endregion

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            GetCheckedInspector();
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }

            var ucItem = UC as UCConditionItem;
            ucItem.CurrentApp = CurrentApp;
            if (ucItem != null)
            {
                ucItem.DoOperation(mListSelectedObjects_Inspector);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }



      

    }
}
