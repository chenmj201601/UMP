using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.DirectoryServices;
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
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common11011;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.VCLDAP;

namespace UMPS1101
{
    /// <summary>
    /// UCLDAPLinkPage.xaml 的交互逻辑
    /// </summary>
    public partial class UCLDAPLinkPage
    {
        private BackgroundWorker mWorker;
        private string IStrADPath;
        private string IStrADDomain;
        private string IStrADUser;
        private string IStrADPassword;

        private List<BindItem> IListADUsers;
        private ObservableCollection<BasicDomainInfo> IListDomainInfo;
        private ADUtility util;

        public OUMMainView ParenntPage;
        public ObjectItem OrgObjItem;
        private DomainObjectItem mRoot;
        private List<DomainObjectItem> mListDomainObjItem;
        private BasicDomainInfo domainInfo;
        //public S1101App CurrentApp;

        public UCLDAPLinkPage()
        {
            InitializeComponent();

            mRoot = new DomainObjectItem();
            mListDomainObjItem = new List<DomainObjectItem>();
            util = new ADUtility();

            Loaded += UCLDAPLinkPage_Loaded;
            this.ComboxDomain.SelectionChanged += ComboxDomain_SelectionChanged;
            this.TvDomian.ItemsSource = mRoot.Children;
            IListADUsers = new List<BindItem>();
            IListDomainInfo = new ObservableCollection<BasicDomainInfo>();
        }

        void UCLDAPLinkPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.ComboxDomain.ItemsSource = IListDomainInfo;
            this.BtnConfirm.Click += BtnConfirm_Click;
            this.BtnClose.Click += BtnClose_Click;
            ChangeLanguage();
            //this.ComboxDomain.SelectedIndex = 0;
            IListDomainInfo.Clear();

            ParenntPage.SetBusy(true, string.Empty);
            mWorker = new BackgroundWorker();
            mWorker.DoWork += (s, de) =>
            {
                InitDomainInfo();
            };
            mWorker.RunWorkerCompleted += (s, re) =>
            {
                ParenntPage.SetBusy(false, string.Empty);
                mWorker.Dispose();
            };
            mWorker.RunWorkerAsync();
        }

        void ComboxDomain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            domainInfo = this.ComboxDomain.SelectedItem as BasicDomainInfo;
            //if (domainInfo == null) { this.ComboxDomain.SelectedIndex = 0; }
            ParenntPage.SetBusy(true,string.Empty);
            InitDomainTree();
            ParenntPage.SetBusy(false, string.Empty);
        }

        private void InitDomainInfo()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S1101Codes.GetDomainInfo;
                webRequest.Session = CurrentApp.Session;

                Service11011Client client = new Service11011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11011"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                //Dispatcher.Invoke(new Action(() => mListDomainInfo.Clear()));
                List<BasicDomainInfo> ListDomainInfo = new List<BasicDomainInfo>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<BasicDomainInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    BasicDomainInfo domainInfo = optReturn.Data as BasicDomainInfo;
                    if (domainInfo != null)
                    {
                        domainInfo.DomainUserPassWord = domainInfo.DomainUserPassWord.Substring(20);
                        ListDomainInfo.Add(domainInfo);
                    }
                }
                ListDomainInfo = ListDomainInfo.OrderBy(p => p.DomainCode).ToList();
                foreach (BasicDomainInfo di in ListDomainInfo)
                {
                    if (!di.IsDelete)
                        Dispatcher.Invoke(new Action(() => IListDomainInfo.Add(di)));
                }
                CurrentApp.WriteLog("PageLoad", string.Format("Load Operation"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitDomainTree()
        {
            if (domainInfo == null) { return; }
            IStrADDomain = domainInfo.DomainName;
            IStrADPassword = domainInfo.DomainUserPassWord;
            IStrADUser = domainInfo.DomainUserName;
            if (IStrADDomain == string.Empty || IStrADPassword == string.Empty || IStrADUser == string.Empty) { return; }
            string LDAPInfo = string.Format("{0}:{1}({2}:{3})", CurrentApp.GetLanguageInfo("11011602", "域账号"), IStrADUser, CurrentApp.GetLanguageInfo("11011601", "域名"), IStrADDomain);
            IListADUsers.Clear();
            List<BindItem> listItems = new List<BindItem>();
            IStrADPath = string.Format("LDAP://{0}", IStrADDomain);
            util = new ADUtility(IStrADPath, IStrADUser, IStrADPassword);
            ClearChildren(mRoot);//树中会清理掉list里面的部分内容
            mListDomainObjItem.Clear();//清理剩余的内容
            try
            {
                //获取下面所有的OU
                ADGroupCollection OUs = util.GetAllOrganizationalUnit();
                foreach (ADGroup group in OUs.AllItem)
                {
                    DomainObjectItem doi = new DomainObjectItem();
                    doi.Name = group.Name;
                    doi.FullName = group.Name;
                    doi.mGuid = group.Guid;
                    doi.ObjType = 111;
                    doi.IsChecked = false;
                    doi.Icon = "Images/org.ico";
                    
                    DirectoryEntry de = group.MyDirectoryEntry;
                    doi.ParentGuid = de.Parent.Guid;
                    doi.ParentName = de.Parent.Name.Substring(3);
                    doi.ParentFullName = doi.ParentName;
                    mListDomainObjItem.Add(doi);
                }
                //获取下面所有用户
                ADUserCollection Users = util.GetAllUsers();
                foreach (ADUser user in Users)
                {
                    //判断用户是否禁用
                    int ControlCode = user.UserAccountControl;
                    string UserControlCode = Convert.ToString(ControlCode, 2);
                    UserControlCode = UserControlCode.Substring(UserControlCode.Count() - 2, 1);
                    if (UserControlCode == "1") { continue; }

                    DomainObjectItem doi = new DomainObjectItem();
                    doi.Name = user.Name.Substring(3).ToLower();
                    doi.FullName = user.AccountName;
                    if (doi.FullName == string.Empty)
                    {
                        doi.FullName = string.Format("{0}@{1}", IStrADDomain.ToLower(), user.AccountFullName.ToLower());
                    }
                    else
                    {
                        List<string> listsp = doi.FullName.Split('@').ToList();
                        if (listsp.Count == 2)
                        {
                            doi.FullName = string.Format("{0}@{1}", IStrADDomain.ToLower(), listsp[0].ToLower());
                        }
                    }
                    doi.mGuid = user.Guid;
                    doi.ObjType = 112;
                    doi.IsChecked = false;
                    doi.Icon = "Images/user.ico";

                    DirectoryEntry de = user.MyDirectoryEntry;
                    doi.ParentGuid = de.Parent.Guid;
                    doi.ParentName = de.Parent.Name.Substring(3);
                    doi.ParentFullName = doi.ParentName;
                    mListDomainObjItem.Add(doi);
                }
                //======================================================================
                //获取组织结构distinguishedName
                ADUser Duser = util.GetADUser(IStrADUser);
                //ADGroupCollection groups = Duser.MemberOf;
                object obj = Duser.GetProperty("distinguishedName");
                string OUCollection = obj.ToString();
                //拆分string，获取dc下的第一个机构
                List<string> OUsName = OUCollection.Split(',').ToList();
                int count = 0;
                string OUName = string.Empty; string DName = OUsName[OUsName.Count - 2];
                for (; count < OUsName.Count(); count++)
                {
                    string tempOU = OUsName[count];
                    if (tempOU.Substring(0, 2) == "OU")
                    {
                        OUName = tempOU.Substring(3);
                        DomainObjectItem objItem = new DomainObjectItem();
                        objItem.Name = OUName;
                        objItem.FullName = objItem.Name;
                        objItem.ObjType = 111;
                        objItem.IsChecked = false;
                        objItem.Icon = "Images/org.ico";
                        mRoot.AddChild(objItem);
                        mListDomainObjItem.Add(objItem);
                        GetChild(objItem);
                        break;
                    }
                }
                if (OUName == string.Empty && count == OUsName.Count)//是域下面的用户，直接获取整个结构树
                {
                    mRoot.Name = DName.Substring(3);
                    GetChild(mRoot);
                }
            }
            catch (Exception ex)
            {
                ShowException(string.Format("Get Users Info From LDAP Fail:{0}", ex.Message));
                CurrentApp.WriteLog(string.Format("Get all AD users fail.\t{0}", ex.Message));
            }
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            List<DomainObjectItem> mListSaveDomainInfo = new List<DomainObjectItem>();
            //获取所有打钩的对象
            GetSaveObject(mRoot, ref mListSaveDomainInfo);
            if (mListSaveDomainInfo.Count == 0) 
            { CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("", "请选择用户")); return; }
            try
            {
                foreach (DomainObjectItem BItem in mListSaveDomainInfo)
                {
                    BasicUserInfo basicUserInfo = new BasicUserInfo();
                    basicUserInfo.SourceFlag = "L";

                    basicUserInfo.CreateTime = DateTime.Now;
                    basicUserInfo.StartTime = DateTime.Parse("2014/1/1 00:00:00");
                    basicUserInfo.EndTime = DateTime.Parse(S1101Consts.Default_StrEndTime.ToString());
                    basicUserInfo.StrCreateTime = basicUserInfo.CreateTime.ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");
                    basicUserInfo.StrStartTime = basicUserInfo.StartTime.ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");
                    basicUserInfo.StrEndTime = basicUserInfo.EndTime.ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");
                    if (BItem.FullName.Contains("@"))
                    {
                        basicUserInfo.Account = BItem.FullName;
                    }
                    else
                        basicUserInfo.Account = string.Format("{0}@{1}", IStrADDomain, BItem.FullName);
                    basicUserInfo.Creator = CurrentApp.Session.UserID;
                    basicUserInfo.FullName = BItem.Name;
                    basicUserInfo.OrgID = OrgObjItem.ObjID;
                    basicUserInfo.Password = string.Empty;
                    basicUserInfo.IsActived = "1";


                    OperationReturn optReturn = XMLHelper.SeriallizeObject(basicUserInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)S1101Codes.LoadNewUser;
                    webRequest.Data = optReturn.Data.ToString();
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
                        if (webReturn.Code == Defines.RET_DBACCESS_EXIST)
                        {
                            ShowException(CurrentApp.GetMessageLanguageInfo("007", "User account already exist"));
                            //return;
                            continue;
                        }
                        ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return;
                    }
                    basicUserInfo.UserID = Convert.ToInt64(webReturn.Data);
                    //修改密码为M003加码
                    webRequest.Code = (int)S1101Codes.ModifyUserPasswordM003;
                    webRequest.ListData.Add(basicUserInfo.UserID.ToString());
                    webRequest.ListData.Add("0");
                    webRequest.ListData.Add(string.Empty);
                    Service11011Client clientCP = new Service11011Client(
                        WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                        WebHelper.CreateEndpointAddress(
                            CurrentApp.Session.AppServerInfo,
                            "Service11011"));
                    WebHelper.SetServiceClient(clientCP);
                    WebReturn webReturnCP = clientCP.DoOperation(webRequest);
                    clientCP.Close();
                    if (!webReturnCP.Result)
                    {
                        ShowException(string.Format("Change Passworld Fail:{0}\t{1}", webReturnCP.Code, webReturnCP.Message));
                    }
                }
                ParenntPage.ReloadData();
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1101N029", "Import User Data Successful"));

                string msg = string.Format("{0}:{1}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString("FO1101012"), IStrADDomain);
                CurrentApp.WriteOperationLog(S1101Consts.OPT_LDAP.ToString(), ConstValue.OPT_RESULT_SUCCESS, msg);
            }
            catch (Exception ex)
            {
                ShowException(CurrentApp.GetLanguageInfo("1101N037", "导入失败") + ex.Message);
                #region 日志
                string msg = string.Format("{0}:{1}", Utils.FormatOptLogString("FO1101012"), IStrADDomain);
                CurrentApp.WriteOperationLog("1101012", ConstValue.OPT_RESULT_FAIL, msg);
                #endregion
            }
            PageClose();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            PageClose();
        }

        private void PageClose()
        {
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }

        #region Languages
        public override void ChangeLanguage()
        {
            try
            {
                base.ChangeLanguage();

                this.TexbOrg.Text = OrgObjItem.Name;
                
                BtnConfirm.Content = CurrentApp.GetLanguageInfo("110110", "Confirm");
                BtnClose.Content = CurrentApp.GetLanguageInfo("110111", "Close");

                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.Title = CurrentApp.GetLanguageInfo("11011600", "域");
                }
            }
            catch (Exception ex)
            { }
        }

        #endregion

        private void GetChild(DomainObjectItem item)
        {
            try
            {
                string OUName = item.Name;
                //加载该ou下的所有用户，并绑给该item
                List<DomainObjectItem> ListADuser = mListDomainObjItem.Where(p => p.ParentName == OUName && p.ObjType == 112).ToList();
                if(ListADuser!=null)
                    foreach (DomainObjectItem adu in ListADuser)
                    {
                        item.Children.Add(adu);
                    }
                //获得该OU下的所有OU集合
                List<DomainObjectItem> ListOUs = mListDomainObjItem.Where(p => p.ParentName == OUName && p.ObjType == 111).ToList();
                foreach (DomainObjectItem doi in ListOUs)
                {
                    //将查出的OU集合绑给传进来的item
                    item.Children.Add(doi);
                    //继续调用GetChild（）
                    GetChild(doi);
                }
            }
            catch (Exception ex)
            { ShowException(ex.Message); }
        }
        private void ClearChildren(DomainObjectItem item)
        {
            if (item == null) { return; }
            for (int i = 0; i < item.Children.Count; i++)
            {
                var child = item.Children[i] as DomainObjectItem;
                if (child != null)
                {
                    var temp = mListDomainObjItem.FirstOrDefault(j => j.mGuid == child.mGuid);
                    if (temp != null) { mListDomainObjItem.Remove(temp); }
                    ClearChildren(child);
                }
            }
            Dispatcher.Invoke(new Action(() => item.Children.Clear()));
        }
        private void GetSaveObject(DomainObjectItem objItem, ref List<DomainObjectItem> mListSaveDomainInfo)
        {
            for (int i = 0; i < objItem.Children.Count; i++)
            {
                DomainObjectItem child = objItem.Children[i] as DomainObjectItem;
                if (child == null) { continue; }
                if (child.ObjType == 112 && child.IsChecked == true)
                {
                    mListSaveDomainInfo.Add(child);
                }
                GetSaveObject(child, ref mListSaveDomainInfo);
            }
        }
    }
}
