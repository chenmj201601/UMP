using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using UMP.Tools.BasicModule;
using UMP.Tools.PublicClasses;
using UMP.Tools.UMPWcfService00003;

namespace UMP.Tools.OnlineUserManagement
{
    public partial class OnlineUserListViews : UserControl, OperationsInterface
    {
        public event EventHandler<OperationEventArgs> IOperationEvent;
        public SystemMainWindow IWindowsParent = null;

        //注销在线用户 BackgroundWorker
        private BackgroundWorker IBackgroundWorkerA = null;

        private OperationDataArgs I00003OperationReturn = new OperationDataArgs();

        public OnlineUserListViews()
        {
            InitializeComponent();
            this.Loaded += OnlineUserListViews_Loaded;
            this.Unloaded += OnlineUserListViews_Unloaded;
        }

        #region 显示界面语言/语言改变
        public void DisplayElementCharacters(bool ABoolLanguageChange)
        {
            DisplayElementObjectCharacters.DisplayUIObjectCharacters(ListViewOnlineUserDetail, ListViewOnlineUserDetail);
        }
        #endregion

        private void OnlineUserListViews_Unloaded(object sender, RoutedEventArgs e)
        {
            App.GSystemMainWindow.IOperationEvent -= GSystemMainWindow_IOperationEvent;
        }

        private void OnlineUserListViews_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayElementCharacters(false);
            App.GSystemMainWindow.IOperationEvent += GSystemMainWindow_IOperationEvent;
        }

        private void GSystemMainWindow_IOperationEvent(object sender, OperationEventArgs e)
        {
            if (e.StrElementTag == "CLID") { DisplayElementCharacters(true); return; }
            if (e.StrElementTag == "FEXIT") { CancellationOnlineUser(); return; }
        }

        public void WriteDataTable2ListView(DataTable ADataTableOnlineUseres)
        {
            int LIntCurrent = 0;
            string LStrRentToken = string.Empty;

            try
            {
                LStrRentToken = ADataTableOnlineUseres.TableName.Substring(6);
                foreach (DataRow LDataRowSingleLanguage in ADataTableOnlineUseres.Rows)
                {
                    LIntCurrent += 1;

                    ListViewItemSingleOnlineUser LListViewItemSingleUser = new ListViewItemSingleOnlineUser();

                    LListViewItemSingleUser.IntItemIndex = LIntCurrent;
                    LListViewItemSingleUser.UserID = LDataRowSingleLanguage["UserID"].ToString();
                    LListViewItemSingleUser.UserAccount = LDataRowSingleLanguage["UserAccount"].ToString();
                    LListViewItemSingleUser.LoginTime = LDataRowSingleLanguage["LoginTime"].ToString();
                    LListViewItemSingleUser.LoginHost = LDataRowSingleLanguage["LoginHost"].ToString();
                    LListViewItemSingleUser.LoginIP = LDataRowSingleLanguage["LoginIP"].ToString();
                    LListViewItemSingleUser.SessionID = LDataRowSingleLanguage["SessionID"].ToString();
                    LListViewItemSingleUser.RentToken = LStrRentToken;

                    ListViewOnlineUserDetail.Items.Add(LListViewItemSingleUser);
                }
            }
            catch { }
        }

        #region 注销在线用户
        private void CancellationOnlineUser()
        {
            try
            {
                ListViewItemSingleOnlineUser LListViewItemSingleOnlineUser = (ListViewItemSingleOnlineUser)ListViewOnlineUserDetail.SelectedItem;
                if (LListViewItemSingleOnlineUser == null) { return; }
                MessageBoxResult LResult = MessageBox.Show(string.Format(App.GetDisplayCharater("M01042"), LListViewItemSingleOnlineUser.UserAccount), App.GStrApplicationReferredTo, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (LResult != MessageBoxResult.Yes) { return; }
                
                I00003OperationReturn.BoolReturn = true;
                I00003OperationReturn.StringReturn = string.Empty;

                App.ShowCurrentStatus(1, string.Format(App.GetDisplayCharater("M01043"), LListViewItemSingleOnlineUser.UserAccount), true);
                if (IBackgroundWorkerA == null) { IBackgroundWorkerA = new BackgroundWorker(); }
                IBackgroundWorkerA.RunWorkerCompleted += IBackgroundWorkerA_RunWorkerCompleted;
                IBackgroundWorkerA.DoWork += IBackgroundWorkerA_DoWork;
                IBackgroundWorkerA.RunWorkerAsync(LListViewItemSingleOnlineUser);
            }
            catch(Exception ex)
            {
                App.ShowCurrentStatus(int.MaxValue, string.Empty, true);
                if (IBackgroundWorkerA != null)
                {
                    IBackgroundWorkerA.Dispose(); IBackgroundWorkerA = null;
                }
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void IBackgroundWorkerA_DoWork(object sender, DoWorkEventArgs e)
        {
            string LStrCallReturn = string.Empty;

            try
            {
                ListViewItemSingleOnlineUser LListViewItemSingleOnlineUser = e.Argument as ListViewItemSingleOnlineUser;
                I00003OperationReturn.BoolReturn = OnlineUserOperations.CancellationOnlineUser(LListViewItemSingleOnlineUser, ref LStrCallReturn);
                I00003OperationReturn.StringReturn = LStrCallReturn;
                e.Result = LListViewItemSingleOnlineUser;
            }
            catch (Exception ex)
            {
                I00003OperationReturn.BoolReturn = false;
                I00003OperationReturn.StringReturn = "UMP002E001" + App.GStrSpliterChar + ex.Message;
            }
        }

        private void IBackgroundWorkerA_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string LStrMessageBody = string.Empty;

            try
            {
                App.ShowCurrentStatus(int.MaxValue, string.Empty, true);
                ListViewItemSingleOnlineUser LListViewItemSingleOnlineUser = e.Result as ListViewItemSingleOnlineUser;
                if (!I00003OperationReturn.BoolReturn)
                {
                    string[] LStrOperationReturn = I00003OperationReturn.StringReturn.Split(App.GStrSpliterChar.ToCharArray());
                    LStrMessageBody = string.Format(App.GetDisplayCharater("M01045"), LListViewItemSingleOnlineUser.UserAccount);
                    LStrMessageBody += "\n" + App.GetDisplayCharater(LStrOperationReturn[0]);
                    LStrMessageBody += "\n" + LStrOperationReturn[1];
                    MessageBox.Show(LStrMessageBody, App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                OperationEventArgs LEventArgs = new OperationEventArgs();
                LEventArgs.StrElementTag = "RONLINE";
                LEventArgs.ObjSource = LListViewItemSingleOnlineUser;
                IOperationEvent(this, LEventArgs);
                LStrMessageBody = string.Format(App.GetDisplayCharater("M01044"), LListViewItemSingleOnlineUser.UserAccount);
                MessageBox.Show(LStrMessageBody, App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch { }
            finally
            {
                if (IBackgroundWorkerA != null)
                {
                    IBackgroundWorkerA.Dispose(); IBackgroundWorkerA = null;
                }
            }
        }
        #endregion
    }

    public class ListViewItemSingleOnlineUser
    {
        public int IntItemIndex { get; set; }
        public string UserID { get; set; }
        public string UserAccount { get; set; }
        public string LoginTime { get; set; }
        public string LoginHost { get; set; }
        public string LoginIP { get; set; }
        public string SessionID { get; set; }
        public string RentToken { get; set; }
    }
}
