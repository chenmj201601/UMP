using System;
using System.Collections.Generic;
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
using System.Collections.ObjectModel;
using System.DirectoryServices;
using VoiceCyber.VCLDAP;

namespace VCLDAPDemo
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        private ObservableCollection<BindItem> IListADGroups;
        private ObservableCollection<BindItem> IListADUsers;
        private string IStrADPath;
        private string IStrADDomain;
        private string IStrADUser;
        private string IStrADPassword;

        public Window1()
        {
            InitializeComponent();

            IListADGroups = new ObservableCollection<BindItem>();
            IListADUsers = new ObservableCollection<BindItem>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtDomain.Text = "voicecyber";
            txtUser.Text = "charley chen";
            txtPassword.Password = "Mingjian521178910";

            listboxGroups.ItemsSource = IListADGroups;
            listboxUsers.ItemsSource = IListADUsers;

        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckInput())
            {
                SubDebug(string.Format("Input invalid."));
                return;
            }

            string strMsg = string.Empty;
            IStrADPath = string.Format("LDAP://{0}", IStrADDomain);
            ADUtility util = new ADUtility(IStrADPath, IStrADUser, IStrADPassword);
            try
            {
                ADUser adUser = util.GetADUser(IStrADUser); ;
               SubDebug(string.Format("{0}", adUser.AccountName));
            }
            catch (Exception ex)
            {
                SubDebug(string.Format("{0}", ex.Message));
            }

            //InitADGroupData();

            //IListADUsers.Clear();
            //IStrADPath = string.Format("LDAP://{0}", IStrADDomain);
            //ADUtility util = new ADUtility(IStrADPath, IStrADUser, IStrADPassword);
            //try
            //{
            //    ADUserCollection users = util.GetAllUsers();
            //    foreach (ADUser user in users)
            //    {
            //        BindItem item = new BindItem();
            //        item.Name = user.AccountName;
            //        item.Display = user.DisplayName;
            //        item.Obj = item;
            //        IListADUsers.Add(item);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    SubDebug(string.Format("Get all AD users fail.\t{0}", ex.Message));
            //}

            //IListADUsers.Clear();
            //IStrADPath = string.Format("LDAP://{0}", IStrADDomain);
            //string strFilter = "(&(objectclass=user)(!objectclass=computer)(cn=*))";
            //DirectoryEntry de = new DirectoryEntry(IStrADPath, IStrADUser, IStrADPassword);
            //DirectorySearcher searcher = new DirectorySearcher(de);
            //searcher.Filter = strFilter;
            //searcher.SearchScope = SearchScope.Subtree;
            //try
            //{
            //    SearchResultCollection items = searcher.FindAll();
            //    foreach (SearchResult result in items)
            //    {
            //        DirectoryEntry deItem = result.GetDirectoryEntry();
            //        string name = string.Empty;
            //        string display = string.Empty;
            //        try
            //        {
            //            name = deItem.Properties["SAMAccountName"].Value.ToString();
            //        }
            //        catch (Exception ex)
            //        {
            //            SubDebug(string.Format("Get Name property value fail.\t{0}", ex.Message));
            //        }
            //        try
            //        {
            //            display = deItem.Properties["displayName"].Value.ToString();
            //        }
            //        catch (Exception ex)
            //        {
            //            SubDebug(string.Format("Get display property value fail.\t{0}", ex.Message));
            //        }
            //        BindItem item = new BindItem();
            //        item.Name = name;
            //        item.Display = display;
            //        item.Obj = item;
            //        IListADUsers.Add(item);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    SubDebug(string.Format("Get all ad users fail.\t{0}", ex.Message));
            //}
        }

        private void listboxGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitADUserData();
        }

        private void btnGetGroup_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckInput())
            {
                SubDebug(string.Format("Input invalid."));
                return;
            }

            InitADGroupData();
        }

        private void btnGetUser_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckInput())
            {
                SubDebug(string.Format("Input invalid."));
                return;
            }

            IListADUsers.Clear();
            IStrADPath = string.Format("LDAP://{0}", IStrADDomain);
            ADUtility util = new ADUtility(IStrADPath, IStrADUser, IStrADPassword);
            try
            {
                ADUserCollection users = util.GetAllUsers();
                foreach (ADUser user in users)
                {
                    BindItem item = new BindItem();
                    item.Name = user.AccountName;
                    item.Display = user.DisplayName;
                    item.Obj = item;
                    IListADUsers.Add(item);
                }
            }
            catch (Exception ex)
            {
                SubDebug(string.Format("Get all AD users fail.\t{0}", ex.Message));
            }
        }

        private void btnAutoLogin_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckInput())
            {
                SubDebug(string.Format("Input invalid."));
                return;
            }

            try
            {
                string strDomain = Environment.UserDomainName.ToUpper();
                string strUser = Environment.UserName.ToUpper(); ;
                if (strDomain.IndexOf(IStrADDomain.ToUpper()) < 0)
                {
                    SubDebug(string.Format("Domain invalid.\t{0}", strDomain));
                    return;
                }
                if (strUser.IndexOf(IStrADUser.ToUpper()) < 0)
                {
                    SubDebug(string.Format("AD user invalid.\t{0}", strUser));
                    return;
                }
                SubDebug(string.Format("Auto login successful.\t{0}", strUser));
            }
            catch (Exception ex)
            {
                SubDebug(string.Format("Auto login fail.\t{0}", ex.Message));
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void InitADGroupData()
        {
            IListADGroups.Clear();
            IStrADPath = string.Format("LDAP://{0}", IStrADDomain);
            ADUtility util = new ADUtility(IStrADPath, IStrADUser, IStrADPassword);
            try
            {
                ADGroupCollection groups = util.GetAllGroups();
                foreach (ADGroup group in groups.AllItem)
                {
                    BindItem item = new BindItem();
                    item.Name = group.Name;
                    item.Display = group.DisplayName;
                    item.Obj = group;
                    IListADGroups.Add(item);
                }
            }
            catch (Exception ex)
            {
                SubDebug(string.Format("Get all AD groups fail.\t{0}", ex.Message));
            }
        }

        private void InitADUserData()
        {
            IListADUsers.Clear();
            IStrADPath = string.Format("LDAP://{0}", IStrADDomain);
            ADUtility util = new ADUtility(IStrADPath, IStrADUser, IStrADPassword);
            try
            {
                BindItem groupItem = listboxGroups.SelectedItem as BindItem;
                if (groupItem == null) { return; }
                ADGroup groupUser = groupItem.Obj as ADGroup;
                if (groupUser == null) { return; }

                string[] users = util.GetUsersForGroup(groupUser.Name);
                foreach (string user in users)
                {
                    BindItem item = new BindItem();
                    item.Name = user;
                    IListADUsers.Add(item);
                }
            }
            catch (Exception ex)
            {
                SubDebug(string.Format("Get all AD users fail.\t{0}", ex.Message));
            }
        }

        private bool CheckInput()
        {
            if (string.IsNullOrEmpty(txtDomain.Text))
            {
                SubDebug(string.Format("Domain empty."));
                return false;
            }
            IStrADDomain = txtDomain.Text;
            if (string.IsNullOrEmpty(txtUser.Text))
            {
                SubDebug(string.Format("Domain user empty."));
                return false;
            }
            IStrADUser = txtUser.Text;
            if (string.IsNullOrEmpty(txtPassword.Password))
            {
                SubDebug(string.Format("Password empty."));
                return false;
            }
            IStrADPassword = txtPassword.Password;
            return true;
        }

        private void SubDebug(string msg)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                txtMsg.Text = string.Format("{0}\t{1}\r\n{2}", DateTime.Now.ToString("HH:mm:ss"), msg, txtMsg.Text);
            }));
        }



    }

    public class BindItem
    {
        public string Name { get; set; }
        public string Display { get; set; }
        public object Obj { get; set; }

        public BindItem()
        {
            Name = string.Empty;
            Display = string.Empty;
            Obj = null;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Display))
            {
                return Name;
            }
            else
            {
                return string.Format("{0}({1})", Display, Name);
            }
        }
    }
}
