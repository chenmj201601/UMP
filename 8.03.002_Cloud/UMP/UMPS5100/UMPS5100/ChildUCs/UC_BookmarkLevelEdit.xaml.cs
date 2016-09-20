using Common5100;
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
using UMPS5100.Entities;
using UMPS5100.MainUserControls;
using UMPS5100.Service51001;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;

namespace UMPS5100.ChildUCs
{
    /// <summary>
    /// UC_BookmarkLevelEdit.xaml 的交互逻辑
    /// </summary>
    public partial class UC_BookmarkLevelEdit
    {
        public UC_BookmarkLevel ParentPage = null;
        public OperationType iAddOrModify = 0;
        public BookmarkLevelEntityInList LevelInModify = null;

        public UC_BookmarkLevelEdit()
        {
            InitializeComponent();
            Loaded += UC_BookmarkLevelEdit_Loaded;
            BtnApply.Click += BtnApply_Click;
            BtnCancel.Click += BtnCancel_Click;
        }

        #region Init
        private void InitLanguage()
        {
            LblName.Content = CurrentApp.GetLanguageInfo("5101011", "Level Name");
            LblColor.Content = CurrentApp.GetLanguageInfo("5101012", "Color");
            BtnApply.Content = CurrentApp.GetLanguageInfo("5101013", "Confirm");
            BtnCancel.Content = CurrentApp.GetLanguageInfo("5101014", "Close");
        }

        #endregion

        #region Overried
        public override void ChangeLanguage()
        {
            InitLanguage();
        }
        #endregion

        void UC_BookmarkLevelEdit_Loaded(object sender, RoutedEventArgs e)
        {
            InitLanguage();
            #region 修改 初始化界面元素
            if (iAddOrModify == OperationType.Modify)
            {
                txtName.Text = LevelInModify.BookmarkLevelName;
                txtName.IsEnabled = false;
                colorLevel.SelectedColor = Utils.GetColorFromRgbString(LevelInModify.BookmarkLevelColor);
            }
            #endregion
        }

        #region 按钮事件
        void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                ShowException(CurrentApp.GetLanguageInfo("5101005", "Class name can not be empty"));
                return;
            }

            Service510011Client client = null;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                string strColor = colorLevel.SelectedColor.ToString().Substring(3);

                #region Add
                if (iAddOrModify == 0)
                {
                    webRequest.Code = (int)S5100RequestCode.AddBookmarkLevel;
                    webRequest.ListData.Add(txtName.Text);
                    webRequest.ListData.Add(strColor);
                }
                #endregion
                #region Modify
                else if (iAddOrModify == OperationType.Modify)
                {
                    webRequest.Code = (int)S5100RequestCode.ModifyBookmarkLevel;
                    webRequest.ListData.Add(LevelInModify.BookmarkLevelID);
                    webRequest.ListData.Add(strColor);
                }
                client = new Service510011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                       WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service51001"));
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
					 if (iAddOrModify == OperationType.Add)
                        {
                            string msg = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO5101001")), txtName.Text);
                            CurrentApp.WriteOperationLog("5101001", ConstValue.OPT_RESULT_FAIL, msg);
                        }
                        else if (iAddOrModify == OperationType.Modify)
                        {
                            string msg = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO5101002")), txtName.Text);
                            CurrentApp.WriteOperationLog("5101002", ConstValue.OPT_RESULT_FAIL, msg);
                        }
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
                //添加成功

                BookmarkLevelEntityInList level = new BookmarkLevelEntityInList();
                if (iAddOrModify == OperationType.Add)
                {
                    string msg = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO5101001")), txtName.Text);
                    CurrentApp.WriteOperationLog("5101001", ConstValue.OPT_RESULT_SUCCESS, msg);

                    string str = webReturn.Data;
                    long levelID = 0;
                    long.TryParse(str, out levelID);
                    if (levelID == 0)
                    {
                        return;
                    }

                    level.BookmarkLevelID = levelID.ToString();
                    level.BookmarkLevelName = txtName.Text;
                    level.BookmarkLevelStatus = "1";
                    level.BookmarkLevelStatusIcon = "Images/avaliable.ico";
                    level.BookmarkLevelColor = strColor;
                    ParentPage.UpdateListView(level, OperationType.Add);
                }
                else if (iAddOrModify == OperationType.Modify)
                {
                    string msg = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO5101002")), txtName.Text);
                    CurrentApp.WriteOperationLog("5101002", ConstValue.OPT_RESULT_SUCCESS, msg);
                    level.BookmarkLevelColor = strColor;
                    level.BookmarkLevelID = LevelInModify.BookmarkLevelID;
                    ParentPage.UpdateListView(level, OperationType.Modify);
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
                #endregion
        }

        void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ParentPage.PopupPanel.IsOpen = false;
        }
        #endregion

    }
}
