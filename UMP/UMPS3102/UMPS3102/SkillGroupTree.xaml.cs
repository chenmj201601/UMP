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
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31021;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS3102
{
    /// <summary>
    /// SkillGroupTree.xaml 的交互逻辑
    /// </summary>
    public partial class SkillGroupTree
    {
        public UserControl UC;

        public UCQueryCondition PageParent;

        public ObjectItem mRootItem_SkillGroup;
        public ObjectItem itemRootSG;

        public List<ObjectItem> mListSelectedObjects_SkillGroup;

        private BackgroundWorker mWorker;

        public SkillGroupTree()
        {
            mRootItem_SkillGroup = new ObjectItem();
            mListSelectedObjects_SkillGroup = new List<ObjectItem>();
            itemRootSG = new ObjectItem();

            InitializeComponent();
            Loaded += InspectorList_Loaded;

         
        }

        //加载质检员
        //void InspectorList_Loaded(object sender, RoutedEventArgs e)
        //{
        //    InspectorTree.ItemsSource = mRootItem_SkillGroup.Children;
        //    mRootItem_SkillGroup.Children.Clear();
        //    mWorker = new BackgroundWorker();
        //    mWorker.DoWork += (s, de) =>
        //    {
        //        InitInspectorParents();
        //    };
        //    mWorker.RunWorkerCompleted += (s, re) =>
        //    {
        //        mWorker.Dispose();
        //        mRootItem_SkillGroup.IsChecked = false;
        //        if (mRootItem_SkillGroup.Children.Count > 0)
        //        {
        //            mRootItem_SkillGroup.Children[0].IsExpanded = true;
        //        }
        //    };
        //    mWorker.RunWorkerAsync();
        //}

        //加载机器名和机器IP       
        void InspectorList_Loaded(object sender, RoutedEventArgs e)
        {
            itemRootSG.Description = CurrentApp.GetLanguageInfo("3102N045", "ALL SELECT");//在界面上资源里应该绑定这个作为显示的内容
            BtnConfirm.Content = CurrentApp.GetLanguageInfo("31020", "Confirm");
            BtnClose.Content = CurrentApp.GetLanguageInfo("31021", "Close");
            InspectorTree.ItemsSource = mRootItem_SkillGroup.Children;
            mRootItem_SkillGroup.Children.Clear();
            mWorker = new BackgroundWorker();
            mWorker.DoWork += (s, de) =>
            {
                InitSkillGroup();
            };
            mWorker.RunWorkerCompleted += (s, re) =>
            {
                mWorker.Dispose();
                mRootItem_SkillGroup.IsChecked = false;
                if (mRootItem_SkillGroup.Children.Count > 0)
                {
                    mRootItem_SkillGroup.Children[0].IsExpanded = true;
                }
            };
            mWorker.RunWorkerAsync();
        }

        private void GetCheckedSkillGroup()
        {
            try
            {
                mListSelectedObjects_SkillGroup.Clear();
                GetCheckedSkillGroup(itemRootSG);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }//这个函数是放到确定这个按钮的地方的

        private void GetCheckedSkillGroup(ObjectItem parentItem)
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
                            GetCheckedSkillGroup(item);
                        }
                        else
                        {
                            if (item.IsChecked == true)
                            {
                                mListSelectedObjects_SkillGroup.Add(item);
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

        private void InitSkillGroup()
        {
            AddChildObject(mRootItem_SkillGroup, itemRootSG);
            InitSkillGroup(itemRootSG, "-1");
        }

        private void InitSkillGroup(ObjectItem parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.GetSkillGroupInfo;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("-1");
                Service31021Client client = new Service31021Client(
                   WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                   WebHelper.CreateEndpointAddress(
                       CurrentApp.Session.AppServerInfo,
                       "Service31021"));
                //Service61012Client client = new Service61012Client();
                WebHelper.SetServiceClient(client);
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
                    item.ObjType = ConstValue.RESOURCE_OPERATIONLOG_UMP;
                    item.Name = strInfo.SkillGroupID;
                    item.Description = strInfo.SkillGroupName;
                    item.ObjID = Convert.ToInt64(strInfo.SkillGroupCode);
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
            GetCheckedSkillGroup();
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }

            var ucItem = UC as UCConditionItem;
            ucItem.CurrentApp = CurrentApp;
            if (ucItem != null)
            {
                ucItem.DoOperation(mListSelectedObjects_SkillGroup);
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
