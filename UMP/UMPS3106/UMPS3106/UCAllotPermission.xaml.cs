using Common3106;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UMPS3106.Models;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Controls;
using VoiceCyber.Wpf.CustomControls;

namespace UMPS3106
{
    /// <summary>
    /// UCAllotPermission.xaml 的交互逻辑
    /// </summary>
    public partial class UCAllotPermission
    {
        public TutorialRepertoryMainView ParentPage;
        public FolderTree folderInfo;

        private ObjectItem mListQA;
        /// <summary>
        /// 修改权限时，获取的QA跟座席
        /// </summary>
        public List<string> QAItems;
        /// <summary>
        /// 保存当前选择的QA跟座席
        /// </summary>
        string strQAList = string.Empty;

        BackgroundWorker mWorker;

        public UCAllotPermission()
        {
            InitializeComponent();
            QAItems = new List<string>();
            this.Loaded += UCAllotPermission_Loaded;
            TvPermission.AddHandler(CheckableTree.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.tv_MouseLeftButtonDown), true);
        }

        void UCAllotPermission_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                mListQA = new ObjectItem();
                ChangeLanguage();
                InitControlQA();
                ParentPage.SetBusy(true,string.Empty);
                mWorker = new BackgroundWorker();
                mWorker.WorkerReportsProgress = true;
                mWorker.WorkerSupportsCancellation = true;
                //注册线程主体方法
                mWorker.DoWork += (s, de) =>
                {
                    InitOrgAndQA(mListQA, ((S3106App)CurrentApp).CurrentOrg);
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    TvPermission.ItemsSource = mListQA.Children;
                    mWorker.Dispose();
                    ParentPage.SetBusy(false,string.Empty);
                };
                mWorker.RunWorkerAsync(); //触发DoWork事件            
            }
            catch (Exception)
            {
                ParentPage.SetBusy(false,string.Empty);                
            }
        }
        void InitControlQA()
        {
            try
            {
                strQAList = folderInfo.UserID1 + folderInfo.UserID2 + folderInfo.UserID3;
                string[] tempQA = strQAList.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                QAItems = new List<string>(tempQA);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #region QA&座席树
        void InitOrgAndQA(ObjectItem parentItem, string parentID)
        {
            try
            {
                mWorker = new BackgroundWorker();
                //注册线程主体方法
                mWorker.DoWork += (s, de) =>
                { 
                    List<CtrolOrg> lstCtrolOrgTemp = new List<CtrolOrg>();
                    lstCtrolOrgTemp = ((S3106App)CurrentApp).ListCtrolOrgInfos.Where(p => p.OrgParentID == parentID).ToList();
                    foreach (CtrolOrg org in lstCtrolOrgTemp)
                    {
                        ObjectItem item = new ObjectItem();
                        item.ObjType = ConstValue.RESOURCE_ORG;
                        item.ObjID = Convert.ToInt64(org.ID);
                        item.Name = org.OrgName;
                        item.Data = org;
                        if (org.ID == ConstValue.ORG_ROOT.ToString())
                        {
                            item.Icon = "/Themes/Default/UMPS3106/Images/rootorg.ico";
                        }
                        else
                        {
                            item.Icon = "/Themes/Default/UMPS3106/Images/org.ico";
                        }
                        InitOrgAndQA(item, org.ID);
                        InitControlQA(item, org.ID);
                        InitControlAgents(item, org.ID);
                        AddChildObject(parentItem, item);
                    }
                };
                mWorker.RunWorkerCompleted += (s, re) => 
                {
                    mWorker.Dispose();
                };
                mWorker.RunWorkerAsync();      
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }         
        }

        private void InitControlQA(ObjectItem parentItem, string parentID)
        {
            try
            {
                List<CtrolQA> lstCtrolQATemp = new List<CtrolQA>();
                lstCtrolQATemp = ((S3106App)CurrentApp).ListCtrolQAInfos.Where(p => p.OrgID == parentID).ToList();
                foreach (CtrolQA qa in lstCtrolQATemp)
                {
                    ObjectItem item = new ObjectItem();
                    bool IsAdd = true;
                    item.ObjType = ConstValue.RESOURCE_USER;
                    item.ObjID = Convert.ToInt64(qa.UserID);
                    item.Name = qa.UserName;
                    if (QAItems.Where(p => p == item.ObjID.ToString()).Count() > 0)
                    {
                        item.IsChecked = true;
                    }
                    item.Description = qa.UserFullName;
                    item.Data = qa;
                    item.Icon = "/Themes/Default/UMPS3106/Images/user.ico";
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
                List<CtrolAgent> lstCtrolAgentTemp = new List<CtrolAgent>();
                lstCtrolAgentTemp = ((S3106App)CurrentApp).ListCtrolAgentInfos.Where(p => p.AgentOrgID == parentID).ToList();
                foreach (CtrolAgent agent in lstCtrolAgentTemp)
                {
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_AGENT;
                    item.ObjID = Convert.ToInt64(agent.AgentID);
                    item.Name = agent.AgentName;
                    if (QAItems.Where(p => p == item.ObjID.ToString()).Count() > 0)
                    {
                        item.IsChecked = true;
                    }
                    item.Description = agent.AgentFullName;
                    item.Data = agent;
                    item.Icon = "/Themes/Default/UMPS3106/Images/agent.ico";
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

        private void GetQAIsCheck(ObjectItem parent, ref List<CtrolQA> lstCtrolQa)
        {
            foreach (ObjectItem o in parent.Children)
            {
                if (o.ObjType == ConstValue.RESOURCE_USER)
                {
                    CtrolQA ctrolqa = new CtrolQA();
                    ctrolqa = (CtrolQA)o.Data;
                    //if ((ctrolqa.UserID == ConstValue.USER_ADMIN.ToString())) { o.IsChecked = true; }
                    if (o.IsChecked == true )
                    {
                        lstCtrolQa.Add(ctrolqa);
                    }
                }
                
                if (o.ObjType == ConstValue.RESOURCE_ORG && o.Children.Count > 0)
                {
                    GetQAIsCheck(o, ref lstCtrolQa);
                }
            }

        }

        private void GetAgentIsCheck(ObjectItem parent, ref List<CtrolAgent> lstCtrolAgent)
        {
            foreach (ObjectItem o in parent.Children)
            {
                if (o.IsChecked == true && o.ObjType == ConstValue.RESOURCE_AGENT)
                {
                    CtrolAgent ctrolAgent = new CtrolAgent();
                    ctrolAgent.AgentID = o.ObjID.ToString();
                    ctrolAgent.AgentName = o.Name;
                    ctrolAgent.AgentFullName = o.Description;
                    lstCtrolAgent.Add(ctrolAgent);
                }
                if (o.ObjType == ConstValue.RESOURCE_ORG && o.Children.Count > 0)
                {
                    GetAgentIsCheck(o, ref lstCtrolAgent);
                }
            }
        }

        private void tv_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Thread t = new Thread(() =>
            {
                Thread.Sleep(500);//次线程休眠1秒
                Dispatcher.Invoke(new Action(() =>
                {
                    List<CtrolAgent> lstCtrolAgentTemp = new List<CtrolAgent>();
                    List<CtrolQA> lstCtrolQATemp = new List<CtrolQA>();
                    GetQAIsCheck(mListQA, ref lstCtrolQATemp);
                    GetAgentIsCheck(mListQA, ref lstCtrolAgentTemp);
                    if (lstCtrolQATemp.Count > 0)
                    {
                        string stra = string.Empty;
                        foreach (CtrolQA ca in lstCtrolQATemp)
                        {
                            stra += ca.UserID + ",";
                        }
                        stra = stra.TrimEnd(',');
                        strQAList = stra;
                    }
                    else { strQAList = CurrentApp.Session.UserID.ToString(); }
                    if (lstCtrolAgentTemp.Count > 0)
                    {
                        string stra = string.Empty;
                        foreach (CtrolAgent ca in lstCtrolAgentTemp)
                        {
                            stra += ca.AgentID + ",";
                        }
                        stra = stra.TrimEnd(',');
                        if (string.IsNullOrWhiteSpace(strQAList)) { strQAList = stra; }
                        else { strQAList = strQAList + "," + stra; }
                    }
                }));
            });
            t.Start();
        }

        #endregion

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            FolderInfo Info = new FolderInfo();
            try
            {
                if (string.IsNullOrWhiteSpace(strQAList))
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3106T00027", "Please Select One QA"));
                    return ;
                }
                if (strQAList.Length > 6000)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3106T00028", "QA Is Too Larger"));
                    return;
                }
                if (strQAList.Length > 4000 && strQAList.Length < 6000)
                {
                    Info.UserID1 = strQAList.Substring(0, 2000);
                    Info.UserID2 = strQAList.Substring(2000, 2000);
                    Info.UserID3 = strQAList.Substring(4000, strQAList.Length - 4);
                }
                if (strQAList.Length > 2000 && strQAList.Length <= 4000)
                {
                    Info.UserID1 = strQAList.Substring(0, 2000);
                    Info.UserID2 = strQAList.Substring(2000, strQAList.Length - 2000);
                    Info.UserID3 = string.Empty;
                }
                if (strQAList.Length <= 2000)
                {
                    Info.UserID1 = strQAList;
                    Info.UserID2 = string.Empty;
                    Info.UserID3 = string.Empty;
                }
                Info.FolderID = folderInfo.FolderID;
                Info.TreeParentID = folderInfo.TreeParentID;
                Info.FolderName = folderInfo.FolderName;
                Info.TreeParentName = folderInfo.TreeParentName;
                Info.CreatorId = folderInfo.CreatorId;
                Info.CreatorName = folderInfo.CreatorName;
                Info.CreatorTime = folderInfo.CreatorTime;
                bool flag = ParentPage.AllotPermission(Info);
                if (flag)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3106T00014", "Operation Sucessed!"));
                }
                else
                {
                    ShowException(CurrentApp.GetLanguageInfo("3106T00015", "Operation Failed."));
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.IsOpen = false;
                }
            }
            catch (Exception)
            {

            }
        }

        public  void ChangeLanguage()
        {
            try
            {
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.Title = CurrentApp.GetLanguageInfo("FO3106004", "Allot Permission");
                }
                BtnConfirm.Content = CurrentApp.GetLanguageInfo("3106T00012", "Confirm");
                BtnClose.Content = CurrentApp.GetLanguageInfo("3106T00013", "Cancel");
            }
            catch { }
        }

        private void CheckBox_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
