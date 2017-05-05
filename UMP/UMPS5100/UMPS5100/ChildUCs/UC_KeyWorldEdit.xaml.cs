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
using Common5100;
using UMPS5100.MainUserControls;
using UMPS5100.Service51001;
using UMPS5100.Entities;
using VoiceCyber.Common;
using VoiceCyber.UMP.Communications;
using System.Collections.ObjectModel;
using VoiceCyber.UMP.Common;

namespace UMPS5100.ChildUCs
{
    /// <summary>
    /// UC_KeyWorldEdit.xaml 的交互逻辑
    /// </summary>
    public partial class UC_KeyWorldEdit
    {
        #region 变量定义
        public OperationType iAddOrModify = 0;
        public UC_KeyWorld ParentPage = null;
        public static ObservableCollection<BookmarkLevelEntity> lstLevels = new ObservableCollection<BookmarkLevelEntity>();
        public KeyWorldsEntityInList keyWorldInModify = null;
        #endregion

        public UC_KeyWorldEdit()
        {
            InitializeComponent();
            Loaded += UC_KeyWorldEdit_Loaded;
            BtnApply.Click += BtnApply_Click;
            BtnCancel.Click += BtnCancel_Click;
        }

        void UC_KeyWorldEdit_Loaded(object sender, RoutedEventArgs e)
        {
            InitLanguage();
            GetAllBookmarkLevel();
            if (iAddOrModify == OperationType.Modify)
            {
                txtName.Text = keyWorldInModify.KeyWorldContent;
                txtName.IsEnabled = false;
                BookmarkLevelEntity level = null;
                foreach (ComboBoxItem item in cmbLevels.Items)
                {
                    level = item.DataContext as BookmarkLevelEntity;
                    if (level.BookmarkLevelID == keyWorldInModify.BookmarkLevelID)
                    {
                        cmbLevels.SelectedItem = item;
                    }
                }
            }
        }

        #region Init
        private void InitLanguage()
        {
            LblKeyWorld.Content = CurrentApp.GetLanguageInfo("5102005", "Key World");
            LblLevel.Content = CurrentApp.GetLanguageInfo("5102006", "Bookmark level");
            LblColor.Content = CurrentApp.GetLanguageInfo("5102007", "Color");
            BtnCancel.Content = CurrentApp.GetLanguageInfo("5102008", "Close");
            BtnApply.Content = CurrentApp.GetLanguageInfo("5102009", "Confirm");
        }

        /// <summary>
        /// 获得所有可用的标签等级
        /// </summary>
        private void GetAllBookmarkLevel()
        {
            Service510011Client client = null;
            lstLevels.Clear();
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S5100RequestCode.GetAllBookmarkLevels;
                webRequest.Session = CurrentApp.Session;
                client = new Service510011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                      WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service51001"));
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    if (webReturn.Code != Defines.RET_FAIL)
                    {
                        ShowException(CurrentApp.GetLanguageInfo(((int)webReturn.Code).ToString(), string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message)));
                        return;
                    }
                    else
                    {
                        ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return;
                    }
                }
                List<string> lstRecords = webReturn.ListData;
                OperationReturn optReturn = null;
                BookmarkLevelEntity level = null;
                BookmarkLevelEntityInList levelInList = null;
                for (int i = 0; i < lstRecords.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<BookmarkLevelEntity>(lstRecords[i]);
                    if (optReturn.Result)
                    {
                        level = optReturn.Data as BookmarkLevelEntity;
                        if (level.BookmarkLevelStatus == "1")
                        {
                            lstLevels.Add(level);
                        }
                    }
                }
                ComboBoxItem item = null;
                for (int i = 0; i < lstLevels.Count; i++)
                {
                    item = new ComboBoxItem();
                    item.Content = lstLevels[i].BookmarkLevelName;
                    item.DataContext = lstLevels[i];
                    cmbLevels.Items.Add(item);
                }
                if (cmbLevels.Items.Count > 0)
                {
                    cmbLevels.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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

        #region Overried
        public override void ChangeLanguage()
        {
            InitLanguage();
        }

        #endregion

        #region 事件
        void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ParentPage.PopupPanel.IsOpen = false;
        }

        void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            if (iAddOrModify == OperationType.Add)
            {
                AddKeyWorld();
            }
            else if (iAddOrModify == OperationType.Modify)
            {
                ModifyKeyWorld();
            }

        }

        private void AddKeyWorld()
        {
            Service510011Client client = null;
            try
            {
                KeyWorldsEntityInList keyworld = new KeyWorldsEntityInList();
                BookmarkLevelEntity level = (cmbLevels.SelectedItem as ComboBoxItem).DataContext as BookmarkLevelEntity;
                keyworld.KeyWorldContent = txtName.Text;
                keyworld.BookmarkLevelID = level.BookmarkLevelID;
                keyworld.BookmarkLevelColor = level.BookmarkLevelColor;
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S5100RequestCode.AddKeyWorld;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(txtName.Text);
                webRequest.ListData.Add(level.BookmarkLevelID);
                client = new Service510011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                      WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service51001"));
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                string msg = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO5102001")), txtName.Text);

                if (!webReturn.Result)
                {
                    CurrentApp.WriteOperationLog("5102001", ConstValue.OPT_RESULT_FAIL, msg);
                    if (webReturn.Code != Defines.RET_FAIL)
                    {
                        if (webReturn.Code == (int)S5100WcfErrorCode.UploadKeyWorldXmlFailed)
                        {
                            string strMsg = CurrentApp.GetLanguageInfo(((int)webReturn.Code).ToString(), "Upload file failed, may be right enough, please complete control of license to the '{0}' folder Everyone");
                            strMsg = string.Format(strMsg, webReturn.Message);
                            ShowException(strMsg);
                        }
                        else
                        {
                            ShowException(CurrentApp.GetLanguageInfo(((int)webReturn.Code).ToString(), string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message)));
                        }
                    }
                    else
                    {
                        ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return;
                    }
                    if ( webReturn.Code > (int)S5100WcfErrorCode.GenerateKeyWorldXmlException)
                    {
                        keyworld.KeyWorldID = webReturn.Data;
                        ParentPage.UpdateListView(keyworld, OperationType.Add);
                    }
                }
                else
                {
                    CurrentApp.WriteOperationLog("5102001", ConstValue.OPT_RESULT_SUCCESS, msg);
                    keyworld.KeyWorldID = webReturn.Data;
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("OPR1", "Success"));
                    CurrentApp.WriteOperationLog("5102001", ConstValue.OPT_RESULT_SUCCESS, msg);
                    ParentPage.UpdateListView(keyworld, OperationType.Add);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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

        private void ModifyKeyWorld()
        {
            Service510011Client client = null;
            try
            {
                KeyWorldsEntityInList keyworld = new KeyWorldsEntityInList();
                BookmarkLevelEntity level = (cmbLevels.SelectedItem as ComboBoxItem).DataContext as BookmarkLevelEntity;
                keyworld.KeyWorldID = keyWorldInModify.KeyWorldID;
                keyworld.KeyWorldContent = txtName.Text;
                keyworld.BookmarkLevelID = level.BookmarkLevelID;
                keyworld.BookmarkLevelColor = level.BookmarkLevelColor;
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S5100RequestCode.ModifyKeyWorld;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(keyworld.KeyWorldID);
                webRequest.ListData.Add(level.BookmarkLevelID);
                client = new Service510011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                      WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service51001"));
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                string msg = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO5102002")), txtName.Text);
                msg += Utils.FormatOptLogString(string.Format("5102012")) + keyWorldInModify.LevelName + " -- " + level.BookmarkLevelName;
                if (!webReturn.Result)
                {
                    CurrentApp.WriteOperationLog("5102002", ConstValue.OPT_RESULT_FAIL, msg);
                    if (webReturn.Code != Defines.RET_FAIL)
                    {
                        if (webReturn.Code == (int)S5100WcfErrorCode.UploadKeyWorldXmlFailed)
                        {
                            string strMsg = CurrentApp.GetLanguageInfo(((int)webReturn.Code).ToString(), "Upload file failed, may be right enough, please complete control of license to the '{0}' folder Everyone");
                            strMsg = string.Format(strMsg, webReturn.Message);
                            ShowException(strMsg);
                        }
                        else
                        {
                            ShowException(CurrentApp.GetLanguageInfo(((int)webReturn.Code).ToString(), string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message)));
                        }
                    }
                    else
                    {
                        ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return;
                    }
                    if (webReturn.Code > (int)S5100WcfErrorCode.GenerateKeyWorldXmlException)
                    {
                        ParentPage.UpdateListView(keyworld, OperationType.Modify);
                    }
                }
                else
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("OPR1", "Success"));
                    CurrentApp.WriteOperationLog("5102002", ConstValue.OPT_RESULT_SUCCESS, msg);
                    ParentPage.UpdateListView(keyworld, OperationType.Modify);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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
    }
}
