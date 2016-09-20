using PFShareClassesC;
using PFShareControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS1103
{
    /// <summary>
    /// UCAgentBasicInformation.xaml 的交互逻辑
    /// </summary>
    public partial class UCAgentBasicInformation
    {
        private BackgroundWorker mWorder;
        private List<UserInfo> ListOrg;
        //public S1103App CurrentApp;

        public UCAgentMaintenance IPageParent = null;
        public string IStrAgentID;
        public string IStrOrgID;
        public string ModelID;
        private ObjectItem mRootItem;
        private ObjectItem mRootItemSG;
        //private List<ObjectItem> mListObjectItems;
        //private List<ObjectItem> mListObjectItem; 

        public UCAgentBasicInformation()
        {
            InitializeComponent();

            IStrAgentID = string.Empty;
            IStrOrgID = string.Empty;
            ListOrg = new List<UserInfo>();
            mRootItem = new ObjectItem();
            mRootItemSG = new ObjectItem();
            //mListObjectItems = new List<ObjectItem>();
            //mListObjectItem = new List<ObjectItem>();

            this.TvObject.ItemsSource = mRootItemSG.Children;
            this.TvObjects.ItemsSource = mRootItem.Children;
            this.Loaded += UCAgentBasicInformation_Loaded;
            this.ComboBoxOrg.ItemsSource = ListOrg;
        }

        private void UCAgentBasicInformation_Loaded(object sender, RoutedEventArgs e)
        {
            GridObjectView.Visibility = System.Windows.Visibility.Visible;

            ButtonSelectOrg.Click += ButtonOperationClick;
            ButtonOrgCancel.Click += ButtonOperationClick;
            Init();
            ChangeLanguage();
            DisplayCombox();
        }
        private void Init()
        {
            if (IPageParent != null)
            {
                IPageParent.SetBusy(true, string.Empty);
            }
            mWorder = new BackgroundWorker();
            mWorder.DoWork += (s, de) =>
            {
                LoadAvaliableObject();
            };
            mWorder.RunWorkerCompleted += (s, re) =>
            {
                mWorder.Dispose();

                this.ComboBoxOrg.SelectedIndex = 0;
                DisplayControl();

                if (IPageParent != null)
                {
                    IPageParent.SetBusy(false, string.Empty);
                }
            };
            mWorder.RunWorkerAsync();
        }

        private void Close()
        {
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            this.LabelOrg.Content = CurrentApp.GetLanguageInfo("S1103006", "Org");
            this.LabelAgentID.Content = CurrentApp.GetLanguageInfo("S1103007", "AgentID");
            this.LabelAgentName.Content = CurrentApp.GetLanguageInfo("S1103008", "AgentName");
            this.LabelAgentStatus.Content = CurrentApp.GetLanguageInfo("S1103011", "AgentStatus");
            this.ButtonOrgCancel.Content = CurrentApp.GetLanguageInfo("S1103004", "Cancel");
            this.ButtonSelectOrg.Content = CurrentApp.GetLanguageInfo("S1103003", "OK");
            this.LabelAgentSkillGroup.Content = CurrentApp.GetLanguageInfo("S1103014", "SkillGroup");
            this.LabelAgentManager.Content = CurrentApp.GetLanguageInfo("S1103015", "Manager");
            this.ComboBoxItemLockStatus0.Content = CurrentApp.GetLanguageInfo("S1103010", "forbide");
            this.ComboBoxItemLockStatus1.Content = CurrentApp.GetLanguageInfo("S1103009", "normal");
        }

        private void DisplayCombox()
        {
            ListOrg.Clear();
            foreach (DataRow drOrg in S1103App.IDataTable11006.Rows)
            {
                UserInfo orgInfo = new UserInfo();
                orgInfo.UserID = Convert.ToInt64(drOrg["C001"].ToString());
                orgInfo.UserName = S1103App.DecryptString(drOrg["C002"].ToString());
                ListOrg.Add(orgInfo);
            }
        }

        private void DisplayControl()
        {
            //if (IStrAgentID == string.Empty) { return; }
            if (ModelID == "E")
            {
                //坐席id、名称、机构
                DataRow[] DR_AgentOrg = S1103App.IDataTable11101.Select(string.Format("C001={0} AND C002=1", IStrAgentID));
                this.TextBoxAgentID.Text = DR_AgentOrg[0]["C017"].ToString();
                this.TextBoxAgentName.Text = S1103App.DecryptString(DR_AgentOrg[0]["C018"].ToString());
                string AgentCondition = DR_AgentOrg[0]["C012"].ToString();
                if (AgentCondition == "1")
                {
                    this.ComboBoxStatus.SelectedIndex = 0;
                }
                else
                {
                    this.ComboBoxStatus.SelectedIndex = 1;
                }
                string AgentOrg = DR_AgentOrg[0]["C011"].ToString();
                this.ComboBoxOrg.SelectedIndex = ListOrg.FindIndex(p => p.UserID.ToString() == AgentOrg);
                //技能组
                List<string> SkillGroup = new List<string>();
                DataRow[] DR_AgentSG = S1103App.IDataTable11201SA.Select(string.Format("C004={0}", IStrAgentID));
                foreach (DataRow dr in DR_AgentSG)
                {
                    SkillGroup.Add(dr["C003"].ToString());
                }
                CheckTreeItem(mRootItemSG, SkillGroup);
                //管理者
                List<string> Manager = new List<string>();
                DataRow[] DR_AgentManager = S1103App.IDataTable11201UA.Select(string.Format("C004={0}", IStrAgentID));
                foreach (DataRow dr in DR_AgentManager)
                {
                    Manager.Add(dr["C003"].ToString());
                }
                CheckTreeItem(mRootItem, Manager);
            }
            else
            {
                this.BtnMore.Visibility = Visibility.Collapsed;
                //set org chose
                if (IStrOrgID != string.Empty)
                {
                    int indexOrg = -1;
                    for (int i = 0; i < ListOrg.Count; i++)
                    {
                        UserInfo TempUserInfo = ListOrg[i];
                        if (TempUserInfo != null)
                        {
                            if (IStrOrgID == TempUserInfo.UserID.ToString())
                            {
                                indexOrg = i; break;
                            }
                        }
                    }
                    this.ComboBoxOrg.SelectedIndex = indexOrg;
                }
            }
        }

        private void CheckTreeItem(ObjectItem mRoot, List<string> ListCheck)
        {
            if (mRoot == null) { return; }
            try
            {
                if (mRoot.Children.Count > 0)
                {
                    for (int i = 0; i < mRoot.Children.Count; i++)
                    {
                        CheckTreeItem(mRoot.Children[i] as ObjectItem, ListCheck);
                    }
                }
                else
                {
                    if (ListCheck.Contains(mRoot.ObjID.ToString()))
                    {
                        mRoot.IsChecked = true;
                        //int index = ListResource.FindIndex(p => p.ObjID == mRoot.ObjID);
                        //ListResource[index].IsChecked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadAvaliableObject()
        {
            LoadManager(mRootItem, "0");
            LoadSkillGroup(mRootItemSG);
        }

        private void LoadSkillGroup(ObjectItem mRootItemSG)
        {
            foreach (DataRow drOrg in S1103App.IDataTable11009.Rows)
            {
                ObjectItem item = new ObjectItem();
                item.Icon = "Images/skillgroup.png";
                item.ObjType = 906;
                item.ObjID = Convert.ToInt64(drOrg["C001"].ToString());
                item.Name = string.Format("{0}({1})", S1103App.DecryptString(drOrg["C006"].ToString()), drOrg["C008"].ToString());
                item.IsChecked = false;
                Dispatcher.Invoke(new Action(() => mRootItemSG.AddChild(item)));
                //mListObjectItem.Add(item);
            }
        }

        private void LoadManager(ObjectItem parentItem, string ParentID)
        {
            DataRow[] DT = S1103App.IDataTable11006.Select(string.Format("C004={0}", ParentID));

            foreach (DataRow drOrg in DT)
            {
                ObjectItem item = new ObjectItem();
                item.ObjType = ConstValue.RESOURCE_ORG;
                item.ObjID = Convert.ToInt64(drOrg["C001"].ToString());
                item.Name = S1103App.DecryptString(drOrg["C002"].ToString());
                item.IsChecked = false;
                if (item.ObjID.ToString() == ConstValue.ORG_ROOT.ToString())
                {
                    item.Icon = "Images/root.ico";
                }
                else
                {
                    item.Icon = "Images/org.ico";
                }
                //加载下面的机构和用户
                LoadManager(item, item.ObjID.ToString());
                LoadUser(item, item.ObjID.ToString());

                Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
            }
        }

        private void LoadUser(ObjectItem parentItem, string ParentID)
        {
            DataRow[] DT_User = S1103App.IDataTable11005.Select(string.Format("C006={0}", ParentID));

            foreach (DataRow drOrg in DT_User)
            {
                ObjectItem item = new ObjectItem();
                item.ObjType = ConstValue.RESOURCE_USER;
                item.ObjID = Convert.ToInt64(drOrg["C001"].ToString());
                item.Name = string.Format("{0}({1})", S1103App.DecryptString(drOrg["C002"].ToString()), S1103App.DecryptString(drOrg["C003"].ToString()));
                item.Icon = "Images/user.ico";
                item.IsChecked = false;
                //加载下面的机构和用户
                LoadManager(item, item.ObjID.ToString());
                Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
            }
        }

        private void ButtonOperationClick(object sender, RoutedEventArgs e)
        {
            string LStrSenderName = string.Empty;

            Button LButtonClicked = sender as Button;
            LStrSenderName = LButtonClicked.Name;

            if (LStrSenderName == "ButtonOrgCancel")
            {
                Close();
            }
            if (LStrSenderName == "ButtonSelectOrg")
            {
                //获取信息
                if (CheckAgentInfo())
                    IPageParent.SaveAgentInformation();
                //关闭
                //Close();
                return;
            }
        }

        public List<string> GetElementSettedData()
        {
            List<string> LListStrReturn = new List<string>();
            string LStrAgentID = string.Empty;
            string LStrAgentName = string.Empty;

            try
            {
                LStrAgentID = TextBoxAgentID.Text.Trim();
                LStrAgentName = TextBoxAgentName.Text.Trim();
                if (string.IsNullOrEmpty(LStrAgentID))
                {
                    LListStrReturn.Clear();
                    MessageBox.Show(CurrentApp.GetLanguageInfo("S1103022", ""), CurrentApp.Session.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return LListStrReturn;
                }
                if (LStrAgentID.Length > 128)
                {
                    LListStrReturn.Clear();
                    MessageBox.Show(CurrentApp.GetLanguageInfo("S1103025", ""), CurrentApp.Session.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return LListStrReturn;
                }
                if (LStrAgentName.Length > 128)
                {
                    LListStrReturn.Clear();
                    MessageBox.Show(CurrentApp.GetLanguageInfo("S1103026", ""), CurrentApp.Session.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return LListStrReturn;
                }
                IPageParent.LStrItemData = LStrAgentName;
                UserInfo TempUserInfo = new UserInfo();
                TempUserInfo = ComboBoxOrg.SelectedItem as UserInfo;
                LListStrReturn.Add("B01" + ConstValue.SPLITER_CHAR + TempUserInfo.UserID.ToString());
                LListStrReturn.Add("B07" + ConstValue.SPLITER_CHAR + LStrAgentID);
                if (string.IsNullOrEmpty(LStrAgentName)) { LListStrReturn.Add("B08" + ConstValue.SPLITER_CHAR + LStrAgentID); }
                else { LListStrReturn.Add("B08" + ConstValue.SPLITER_CHAR + LStrAgentName); }

                if (ComboBoxStatus.SelectedIndex == 0) { LListStrReturn.Add("B02" + ConstValue.SPLITER_CHAR + "1"); }
                else { LListStrReturn.Add("B02" + ConstValue.SPLITER_CHAR + "0"); }
            }
            catch { LListStrReturn.Clear(); }

            return LListStrReturn;
        }

        public List<string> GetManagerData()
        {
            List<string> LListStrReturn = new List<string>();
            List<string> LManager = new List<string>();
            GetDataFromTree(mRootItem, ConstValue.RESOURCE_USER, ref LManager);
            //管理者用户
            foreach (string LObjectSingle in LManager)
            {
                LListStrReturn.Add("U00" + ConstValue.SPLITER_CHAR + LObjectSingle);
            }
            return LListStrReturn;
        }

        public List<string> GetSGData()
        {
            List<string> LListStrReturn = new List<string>();
            List<string> LSG = new List<string>();
            GetDataFromTree(mRootItemSG, 906, ref LSG);
            //技能组
            foreach (string LObjectSingle in LSG)
            {
                LListStrReturn.Add("S00" + ConstValue.SPLITER_CHAR + LObjectSingle);
            }
            return LListStrReturn;
        }

        private void GetDataFromTree(ObjectItem ObjItem, int code, ref List<string> listObjectState)
        {
            List<string> LListStrReturn = new List<string>();
            for (int i = 0; i < ObjItem.Children.Count; i++)
            {
                ObjectItem child = ObjItem.Children[i] as ObjectItem;
                if (child == null) { continue; }
                if (child.IsChecked == true && child.ObjType == code)
                {
                    listObjectState.Add(child.ObjID.ToString());
                }

                GetDataFromTree(child, code, ref listObjectState);
            }
        }

        private bool CheckAgentInfo()
        {
            string agentID = this.TextBoxAgentID.Text.Trim();
            if (agentID == string.Empty)
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("S1103022", "AgentID is empty"));
                return false;
            }
            int AgentNum = 0;

            foreach (DataRow dr in S1103App.IDataTable11101.Rows)
            {
                if (agentID == dr["C017"].ToString())
                {
                    AgentNum++;
                }
            }
            if (ModelID == "E")
            {
                if (AgentNum > 1)
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("S1103034", "AgentID is not available"));
                    return false;
                }
            }
            else
            {
                if (AgentNum == 1)
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("S1103034", "AgentID is not available"));
                    return false;
                }
            }
            return true;
        }
    }
}
