//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    737385da-b8cf-4025-b336-77f9c4893581
//        CLR Version:              4.0.30319.18444
//        Name:                     UCCustomSetting
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102
//        File Name:                UCCustomSetting
//
//        created by Charley at 2014/11/6 15:20:15
//        http://www.voicecyber.com
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Forms;
using UMPS3102.Models;
using UMPS3102.Wcf11012;
using UMPS3102.Wcf31021;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31021;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using Binding = System.Windows.Data.Binding;

namespace UMPS3102
{
    /// <summary>
    /// UCCustomSetting.xaml 的交互逻辑
    /// </summary>
    public partial class UCCustomSetting
    {

        #region Memeber

        public QMMainView PageParent;

        private List<SettingInfo> mListSettingInfos;
        private ObservableCollection<ViewColumnInfo> mListColumnViewColumns;
        private ObservableCollection<ViewColumnInfoItem> mListCustomColumns;//
        private ViewColumnInfoItem mCurrentViewColumnItem;
        private UCCustomConditionDesigner mConditionDesigner;
        private UCBookmarkRankSetting mBookmarkRankSetting;

        #endregion


        public UCCustomSetting()
        {
            InitializeComponent();

            mListSettingInfos = new List<SettingInfo>();
            mListColumnViewColumns = new ObservableCollection<ViewColumnInfo>();
            mListCustomColumns = new ObservableCollection<ViewColumnInfoItem>();

            Loaded += UCCustomSetting_Loaded;

            ExportRecordCbIgnorePathFormat.Click +=
                (s, ce) =>
                {
                    ExportRecordTxtPathFormat.IsEnabled = ExportRecordCbIgnorePathFormat.IsChecked != true;
                    ExportRecordBtnSelect.IsEnabled = ExportRecordCbIgnorePathFormat.IsChecked != true;
                };
            ExportRecordCbEncryptRecord.Click += (s, ce) =>
            {
                ExportRecordTxtEncryptPassword.IsEnabled = ExportRecordCbEncryptRecord.IsChecked == true;
                ExportRecordCbDecryptFile.IsEnabled = ExportRecordCbEncryptRecord.IsChecked != true;
                if (ExportRecordCbEncryptRecord.IsChecked == true)
                {
                    ExportRecordCbDecryptFile.IsChecked = true;
                }
            };
        }

        void UCCustomSetting_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            BtnApply.Click += BtnApply_Click;
            BtnClose.Click += BtnClose_Click;
            ExportDataCbRemember.Click += ExportDataCbRemember_Click;
            ExportRecordCbRemember.Click += ExportRecordCbRemember_Click;
            ExportRecordBtnBrowser.Click += ExportRecordBtnBrowser_Click;
            ExportRecordBtnSelect.Click += ExportRecordBtnSelect_Click;
            ColumnLvColumns.SelectionChanged += ColumnLvColumns_SelectionChanged;
            ColumnBtnUp.Click += ColumnBtnUp_Click;
            ColumnBtnDown.Click += ColumnBtnDown_Click;
            ColumnBtnApply.Click += ColumnBtnApply_Click;

            ColumnLvColumns.ItemsSource = mListCustomColumns;

            LoadColumnViewColumns();
            LoadUserSettingInfos();
            LoadCustomColumnData();

            CreateColumnViewColumns();
            Init();

            ChangeLanguage();
        }


        #region Init and Load

        private void LoadColumnViewColumns()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserViewColumnList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("3102004");
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                List<ViewColumnInfo> listColumns = new List<ViewColumnInfo>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<ViewColumnInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ViewColumnInfo columnInfo = optReturn.Data as ViewColumnInfo;
                    if (columnInfo != null)
                    {
                        columnInfo.Display = columnInfo.ColumnName;
                        listColumns.Add(columnInfo);
                    }
                }
                listColumns = listColumns.OrderBy(c => c.SortID).ToList();
                mListColumnViewColumns.Clear();
                for (int i = 0; i < listColumns.Count; i++)
                {
                    mListColumnViewColumns.Add(listColumns[i]);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadUserSettingInfos()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.GetUserSettingList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                string strGroupInfo = string.Format("310201{0}310202{0}310203{0}310204", ConstValue.SPLITER_CHAR);
                webRequest.ListData.Add(strGroupInfo);
                webRequest.ListData.Add(string.Empty);
                Service31021Client client = new Service31021Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service31021"));
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
                    OperationReturn optReturn = XMLHelper.DeserializeObject<SettingInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        CurrentApp.WriteLog("LoadSetting", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    SettingInfo settingInfo = optReturn.Data as SettingInfo;
                    if (settingInfo == null)
                    {
                        CurrentApp.WriteLog("LoadSetting", string.Format("Fail.\tSettingInfo is null"));
                        continue;
                    }
                    var temp =
                        mListSettingInfos.FirstOrDefault(
                            s => s.ParamID == settingInfo.ParamID && s.UserID == settingInfo.UserID);
                    if (temp == null)
                    {
                        mListSettingInfos.Add(settingInfo);
                    }
                    else
                    {
                        mListSettingInfos.Remove(temp);
                        mListSettingInfos.Add(settingInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void Init()
        {
            try
            {
                #region InitState

                mConditionDesigner = new UCCustomConditionDesigner();
                mConditionDesigner.CurrentApp = CurrentApp;
                TabCustomCondition.Content = mConditionDesigner;
                mBookmarkRankSetting = new UCBookmarkRankSetting();
                mBookmarkRankSetting.CurrentApp = CurrentApp;
                TabBookmarkRank.Content = mBookmarkRankSetting;


                BasicComboPageSize.SelectedIndex = 2;
                BasicComboMaxRecords.SelectedIndex = 2;
                BasicCbSkipConditionPanel.IsChecked = true;
                BasicCbQueryVoiceRecord.IsChecked = true;
                BasicCbQueryScreenRecord.IsChecked = false;
                BasicCbAutoRelativePlay.IsChecked = true;

                ExportDataRadioCurrentSelected.IsChecked = true;
                ExportDataRadioCurrentPage.IsChecked = false;
                ExportDataRadioAllPage.IsChecked = false;
                ExportDataCbRemember.IsChecked = false;
                ExportDataCbNotShow.IsChecked = false;

                ExportRecordCbRemember.IsChecked = false;
                ExportRecordCbNotShow.IsChecked = false;
                ExportRecordTxtSaveDir.Text = string.Empty;
                ExportRecordTxtPathFormat.Text = @"\vox\*YYYY*MM\*DD\*VID\*CID\";
                ExportRecordTxtEncryptPassword.Password = string.Empty;
                ExportRecordCbIgnorePathFormat.IsChecked = true;
                ExportRecordCbEncryptRecord.IsChecked = false;
                ExportRecordCbConvertPCM.IsChecked = true;
                ExportRecordCbDecryptFile.IsChecked = true;
                ExportRecordCbGenerateDB.IsChecked = false;
                ExportRecordCbReplaceFile.IsChecked = true;
                ExportRecordCbExportVoice.IsChecked = true;
                ExportRecordCbExportScreen.IsChecked = true;

                PlayScreenCbNoPlay.IsChecked = false;
                PlayScreenCbTopMost.IsChecked = true;
                PlayScreenComboScale.SelectedIndex = 0;

                #endregion


                SettingInfo settingInfo;
                int intValue;


                #region Basic

                settingInfo = mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_PAGESIZE);
                if (settingInfo != null
                    && !string.IsNullOrEmpty(settingInfo.StringValue)
                    && int.TryParse(settingInfo.StringValue, out intValue))
                {
                    for (int i = 0; i < BasicComboPageSize.Items.Count; i++)
                    {
                        var comboItem = BasicComboPageSize.Items[i] as ComboBoxItem;
                        if (comboItem != null && comboItem.Tag.ToString() == settingInfo.StringValue)
                        {
                            BasicComboPageSize.SelectedItem = comboItem;
                        }
                    }
                }
                settingInfo = mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_MAXRECORDS);
                if (settingInfo != null
                    && !string.IsNullOrEmpty(settingInfo.StringValue)
                    && int.TryParse(settingInfo.StringValue, out intValue))
                {
                    for (int i = 0; i < BasicComboMaxRecords.Items.Count; i++)
                    {
                        var comboItem = BasicComboMaxRecords.Items[i] as ComboBoxItem;
                        if (comboItem != null && comboItem.Tag.ToString() == settingInfo.StringValue)
                        {
                            BasicComboMaxRecords.SelectedItem = comboItem;
                        }
                    }
                }
                settingInfo = mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_SKIPCONDITIONPANEL);
                if (settingInfo != null)
                {
                    BasicCbSkipConditionPanel.IsChecked = settingInfo.StringValue == "1";
                }
                settingInfo = mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_SKIPPASSWORDPANEL);
                if (settingInfo != null)
                {
                    BasicCbSkipPasswordPanel.IsChecked = settingInfo.StringValue == "1";
                }
                settingInfo = mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_QUERYVOICERECORD);
                if (settingInfo != null)
                {
                    BasicCbQueryVoiceRecord.IsChecked = settingInfo.StringValue == "1";
                }
                settingInfo = mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_QUERYSCREENRECORD);
                if (settingInfo != null)
                {
                    BasicCbQueryScreenRecord.IsChecked = settingInfo.StringValue == "1";
                }
                settingInfo = mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_AUTORELATIVEPLAY);
                if (settingInfo != null)
                {
                    BasicCbAutoRelativePlay.IsChecked = settingInfo.StringValue == "1";
                }

                #endregion


                #region Export Data

                bool exportDataIsRemember = false;
                string strExportDataType = "1";
                settingInfo =
                    mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTDATA_REMEMBER);
                if (settingInfo != null && settingInfo.StringValue == "1")
                {
                    exportDataIsRemember = true;
                }
                ExportDataCbRemember.IsChecked = exportDataIsRemember;
                settingInfo = mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTDATA_TYPE);
                if (settingInfo != null
                    && !string.IsNullOrEmpty(settingInfo.StringValue)
                    && exportDataIsRemember)
                {
                    strExportDataType = settingInfo.StringValue;
                }
                ExportDataRadioCurrentSelected.IsChecked = strExportDataType == "1";
                ExportDataRadioCurrentPage.IsChecked = strExportDataType == "2";
                ExportDataRadioAllPage.IsChecked = strExportDataType == "3";
                settingInfo =
                    mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTDATA_NOTSHOW);
                if (settingInfo != null && exportDataIsRemember)
                {
                    ExportDataCbNotShow.IsChecked = settingInfo.StringValue == "1";
                }

                #endregion


                #region Export Record

                bool exportRecordIsRemember = false;
                settingInfo = mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_REMEMBER);
                if (settingInfo != null && settingInfo.StringValue == "1")
                {
                    exportRecordIsRemember = true;
                }
                ExportRecordCbRemember.IsChecked = exportRecordIsRemember;
                settingInfo = mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_NOTSHOW);
                if (settingInfo != null && exportRecordIsRemember)
                {
                    ExportRecordCbNotShow.IsChecked = settingInfo.StringValue == "1";
                }
                settingInfo = mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_SAVEDIR);
                if (settingInfo != null && exportRecordIsRemember && !string.IsNullOrEmpty(settingInfo.StringValue))
                {
                    ExportRecordTxtSaveDir.Text = settingInfo.StringValue;
                }
                else
                {
                    ExportRecordTxtSaveDir.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                }
                settingInfo = mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_PATHFORMAT);
                if (settingInfo != null && exportRecordIsRemember && !string.IsNullOrEmpty(settingInfo.StringValue))
                {
                    ExportRecordTxtPathFormat.Text = settingInfo.StringValue;
                }
                settingInfo = mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_ENCRYPTPASSWORD);
                if (settingInfo != null && exportRecordIsRemember && !string.IsNullOrEmpty(settingInfo.StringValue))
                {
                    ExportRecordTxtEncryptPassword.Password = S3102App.DecryptString(settingInfo.StringValue);
                }
                settingInfo = mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_IGNOREPATHFORMAT);
                if (settingInfo != null && exportRecordIsRemember)
                {
                    ExportRecordCbIgnorePathFormat.IsChecked = settingInfo.StringValue == "1";
                }
                settingInfo = mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_ENCRYPTRECORD);
                if (settingInfo != null && exportRecordIsRemember)
                {
                    ExportRecordCbEncryptRecord.IsChecked = settingInfo.StringValue == "1";
                }
                settingInfo = mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_CONVERTPCM);
                if (settingInfo != null && exportRecordIsRemember)
                {
                    ExportRecordCbConvertPCM.IsChecked = settingInfo.StringValue == "1";
                }
                settingInfo = mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_DECRYPTFILE);
                if (settingInfo != null && exportRecordIsRemember)
                {
                    ExportRecordCbDecryptFile.IsChecked = settingInfo.StringValue == "1";
                }
                settingInfo = mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_GENERATEDB);
                if (settingInfo != null && exportRecordIsRemember)
                {
                    ExportRecordCbGenerateDB.IsChecked = settingInfo.StringValue == "1";
                }
                settingInfo = mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_REPLACEFILE);
                if (settingInfo != null && exportRecordIsRemember)
                {
                    ExportRecordCbReplaceFile.IsChecked = settingInfo.StringValue == "1";
                }
                settingInfo = mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_EXPORTVOICE);
                if (settingInfo != null && exportRecordIsRemember)
                {
                    ExportRecordCbExportVoice.IsChecked = settingInfo.StringValue == "1";
                }
                settingInfo = mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_EXPORTSCREEN);
                if (settingInfo != null && exportRecordIsRemember)
                {
                    ExportRecordCbExportScreen.IsChecked = settingInfo.StringValue == "1";
                }

                //生成数据库信息，暂未实现，先不可用
                ExportRecordCbGenerateDB.IsEnabled = false;

                //设置PathFormat可用性
                ExportRecordTxtPathFormat.IsEnabled = ExportRecordCbIgnorePathFormat.IsChecked != true;
                ExportRecordBtnSelect.IsEnabled = ExportRecordCbIgnorePathFormat.IsChecked != true;

                //导出加密文件，必须要选择解密文件
                ExportRecordTxtEncryptPassword.IsEnabled = ExportRecordCbEncryptRecord.IsChecked == true;
                ExportRecordCbDecryptFile.IsEnabled = ExportRecordCbEncryptRecord.IsChecked != true;
                if (ExportRecordCbEncryptRecord.IsChecked == true)
                {
                    ExportRecordCbDecryptFile.IsChecked = true;
                }

                #endregion


                #region Play Screen

                settingInfo = mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_PLAYSCREEN_NOPLAY);
                if (settingInfo != null)
                {
                    PlayScreenCbNoPlay.IsChecked = settingInfo.StringValue == "1";
                }
                settingInfo = mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_PLAYSCREEN_TOPMOST);
                if (settingInfo != null)
                {
                    PlayScreenCbTopMost.IsChecked = settingInfo.StringValue == "1";
                }
                settingInfo = mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_PLAYSCREEN_SCALE);
                if (settingInfo != null
                    && !string.IsNullOrEmpty(settingInfo.StringValue)
                    && int.TryParse(settingInfo.StringValue, out intValue))
                {
                    for (int i = 0; i < PlayScreenComboScale.Items.Count; i++)
                    {
                        var comboItem = PlayScreenComboScale.Items[i] as ComboBoxItem;
                        if (comboItem != null && comboItem.Tag.ToString() == settingInfo.StringValue)
                        {
                            PlayScreenComboScale.SelectedItem = comboItem;
                        }
                    }
                }

                #endregion

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadCustomColumnData()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserViewColumnList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("3102001");
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                List<ViewColumnInfo> listColumns = new List<ViewColumnInfo>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<ViewColumnInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ViewColumnInfo columnInfo = optReturn.Data as ViewColumnInfo;
                    if (columnInfo != null)
                    {
                        columnInfo.Display = columnInfo.ColumnName;
                        listColumns.Add(columnInfo);
                    }
                }
                listColumns = listColumns.OrderBy(c => c.SortID).ToList();
                mListCustomColumns.Clear();
                for (int i = 0; i < listColumns.Count; i++)
                {
                    ViewColumnInfoItem item = new ViewColumnInfoItem(listColumns[i]);
                    item.Display = CurrentApp.GetLanguageInfo(string.Format("COL3102001{0}", listColumns[i].ColumnName), listColumns[i].ColumnName);
                    item.StrIsVisible = CurrentApp.GetLanguageInfo(string.Format("3102TIP002{0}", listColumns[i].Visibility), listColumns[i].Visibility);
                    mListCustomColumns.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Others

        private void CreateColumnViewColumns()
        {
            try
            {
                GridView gv = new GridView();
                GridViewColumn gvc;
                GridViewColumnHeader gvch;
                for (int i = 0; i < mListColumnViewColumns.Count; i++)
                {
                    ViewColumnInfo columnInfo = mListColumnViewColumns[i];
                    if (columnInfo.Visibility == "1")
                    {
                        gvc = new GridViewColumn();
                        gvch = new GridViewColumnHeader();
                        gvch.Content = columnInfo.Display;
                        gvch.Content = CurrentApp.GetLanguageInfo(string.Format("COL3102004{0}", columnInfo.ColumnName), columnInfo.Display);
                        gvch.ToolTip = CurrentApp.GetLanguageInfo(string.Format("COL3102004{0}", columnInfo.ColumnName), columnInfo.Display);
                        gvc.Header = gvch;
                        gvc.Width = columnInfo.Width;
                        gvc.DisplayMemberBinding = new Binding(columnInfo.ColumnName);
                        if (columnInfo.ColumnName == "IsVisible")
                        {
                            gvc.DisplayMemberBinding = new Binding("StrIsVisible");
                        }
                        if (columnInfo.ColumnName == "ColumnName")
                        {
                            gvc.DisplayMemberBinding = new Binding("Display");
                        }
                        gv.Columns.Add(gvc);
                    }
                }
                ColumnLvColumns.View = gv;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region EventHandler

        void BtnClose_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }

        void BtnApply_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SaveConfig();
        }

        void ExportDataCbRemember_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ExportDataCbNotShow.IsEnabled = ExportDataCbRemember.IsChecked == true;
            if (ExportDataCbRemember.IsChecked == false)
            {
                ExportDataCbNotShow.IsChecked = false;
            }
        }

        void ExportRecordBtnSelect_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        void ExportRecordBtnBrowser_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = string.Format("Please select save directory");
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string dir = dialog.SelectedPath;
                if (!Directory.Exists(dir))
                {
                    ShowException(string.Format("Directory not exist"));
                    return;
                }
                ExportRecordTxtSaveDir.Text = dir;
            }
        }

        void ExportRecordCbRemember_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ExportRecordCbNotShow.IsEnabled = ExportRecordCbRemember.IsChecked == true;
            if (ExportRecordCbRemember.IsChecked == false)
            {
                ExportRecordCbNotShow.IsChecked = false;
            }
        }

        void ColumnLvColumns_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = ColumnLvColumns.SelectedItem as ViewColumnInfoItem;
            if (item != null)
            {
                mCurrentViewColumnItem = item;
                ColumnLbColumnName.Text = CurrentApp.GetLanguageInfo(string.Format("COL3102001{0}", item.ColumnName),
                    item.ColumnName);
                ColumnCbIsVisible.IsChecked = item.IsVisible;
                ColumnTxtWidth.Text = item.Width.ToString();

            }
        }

        void ColumnBtnApply_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (mCurrentViewColumnItem != null)
            {
                int intValue;
                if (!int.TryParse(ColumnTxtWidth.Text, out intValue)||int.Parse(ColumnTxtWidth.Text)<0)
                {
                    ShowException(string.Format(CurrentApp.GetLanguageInfo("3102N041", "Width invalid")));
                    return;
                }
                mCurrentViewColumnItem.IsVisible = ColumnCbIsVisible.IsChecked == true;
                mCurrentViewColumnItem.ViewColumnInfo.Visibility = ColumnCbIsVisible.IsChecked == true ? "1" : "0";
                mCurrentViewColumnItem.StrIsVisible =
                    CurrentApp.GetLanguageInfo(
                        string.Format("3102TIP002{0}", mCurrentViewColumnItem.ViewColumnInfo.Visibility),
                        mCurrentViewColumnItem.ViewColumnInfo.Visibility);
                mCurrentViewColumnItem.Width = intValue;
                mCurrentViewColumnItem.ViewColumnInfo.Width = intValue;
            }
        }

        void ColumnBtnDown_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var item = ColumnLvColumns.SelectedItem as ViewColumnInfoItem;
            if (item != null)
            {
                int index = ColumnLvColumns.SelectedIndex;
                if (index < mListCustomColumns.Count - 1)
                {
                    int sortID;
                    var nextItem = mListCustomColumns[index + 1];
                    sortID = item.SortID;
                    item.SortID = nextItem.SortID;
                    item.ViewColumnInfo.SortID = nextItem.SortID;
                    nextItem.SortID = sortID;
                    nextItem.ViewColumnInfo.SortID = sortID;
                    mListCustomColumns.Remove(item);
                    mListCustomColumns.Insert(index + 1, item);
                    ColumnLvColumns.SelectedIndex = index + 1;
                }
            }
        }

        void ColumnBtnUp_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var item = ColumnLvColumns.SelectedItem as ViewColumnInfoItem;
            if (item != null)
            {
                int index = ColumnLvColumns.SelectedIndex;
                if (index > 0)
                {
                    int sortID;
                    var preItem = mListCustomColumns[index - 1];
                    sortID = item.SortID;
                    item.SortID = preItem.SortID;
                    item.ViewColumnInfo.SortID = preItem.SortID;
                    preItem.SortID = sortID;
                    preItem.ViewColumnInfo.SortID = sortID;
                    mListCustomColumns.Remove(item);
                    mListCustomColumns.Insert(index - 1, item);
                    ColumnLvColumns.SelectedIndex = index - 1;
                }
            }
        }

        #endregion


        #region Operations

        private void SaveConfig()
        {
            var tabItem = TabControlSetting.SelectedItem as TabItem;
            if (tabItem == null) { return; }
            string strType = tabItem.Tag.ToString();
            switch (strType)
            {
                case "0":
                    SaveBasicConfig();
                    break;
                case "1":
                    SaveColumnInfos();
                    break;
                case "2":
                    SaveConditionInfos();
                    break;
                case "3":
                    SaveDataExportConfig();
                    break;
                case "4":
                    SaveRecordExportConfig();
                    break;
                case "5":
                    SaveBookmarkRankInfos();
                    break;
                case  "6":
                    SavePlayScreenInfos();
                    break;
            }
        }

        private void SaveBasicConfig()
        {
            try
            {
                string strLog = string.Empty;
                int intPageSize, intMaxRecords;
                var tabItem = BasicComboPageSize.SelectedItem as ComboBoxItem;
                if (tabItem == null)
                {
                    ShowException(string.Format("PageSize invalid"));
                    return;
                }
                if (!int.TryParse(tabItem.Tag.ToString(), out intPageSize))
                {
                    ShowException(string.Format("PageSize invalid"));
                    return;
                }
                strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("31021300"), intPageSize);
                tabItem = BasicComboMaxRecords.SelectedItem as ComboBoxItem;
                if (tabItem == null)
                {
                    ShowException(string.Format("MaxRecords invalid"));
                    return;
                }
                if (!int.TryParse(tabItem.Tag.ToString(), out intMaxRecords))
                {
                    ShowException(string.Format("MaxRecords invalid"));
                    return;
                }
                strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("31021301"), intMaxRecords);
                if (BasicCbSkipConditionPanel.IsChecked == true)
                {
                    strLog += string.Format("{0} ", Utils.FormatOptLogString("31021302"));
                }
                if (BasicCbSkipPasswordPanel.IsChecked == true)
                {
                    strLog += string.Format("{0} ", Utils.FormatOptLogString("31021306"));
                }
                if (BasicCbQueryVoiceRecord.IsChecked == true)
                {
                    strLog += string.Format("{0} ", Utils.FormatOptLogString("31021303"));
                }
                if (BasicCbQueryScreenRecord.IsChecked == true)
                {
                    strLog += string.Format("{0} ", Utils.FormatOptLogString("31021304"));
                }
                if (BasicCbAutoRelativePlay.IsChecked == true)
                {
                    strLog += string.Format("{0} ", Utils.FormatOptLogString("31021305"));
                }
                List<SettingInfo> listSettingInfos = new List<SettingInfo>();
                SettingInfo settingInfo;
                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_PAGESIZE;
                settingInfo.GroupID = 310201;
                settingInfo.SortID = 0;
                settingInfo.DataType = 2;
                settingInfo.StringValue = intPageSize.ToString();
                listSettingInfos.Add(settingInfo);
                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_MAXRECORDS;
                settingInfo.GroupID = 310201;
                settingInfo.SortID = 1;
                settingInfo.DataType = 2;
                settingInfo.StringValue = intMaxRecords.ToString();
                listSettingInfos.Add(settingInfo);
                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_SKIPCONDITIONPANEL;
                settingInfo.GroupID = 310201;
                settingInfo.SortID = 2;
                settingInfo.DataType = 2;
                settingInfo.StringValue = BasicCbSkipConditionPanel.IsChecked == true ? "1" : "0";
                listSettingInfos.Add(settingInfo);
                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_SKIPPASSWORDPANEL;
                settingInfo.GroupID = 310201;
                settingInfo.SortID = 3;
                settingInfo.DataType = 2;
                settingInfo.StringValue = BasicCbSkipPasswordPanel.IsChecked == true ? "1" : "0";
                listSettingInfos.Add(settingInfo);
                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_QUERYVOICERECORD;
                settingInfo.GroupID = 310201;
                settingInfo.SortID = 4;
                settingInfo.DataType = 2;
                settingInfo.StringValue = BasicCbQueryVoiceRecord.IsChecked == true ? "1" : "0";
                listSettingInfos.Add(settingInfo);
                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_QUERYSCREENRECORD;
                settingInfo.GroupID = 310201;
                settingInfo.SortID = 5;
                settingInfo.DataType = 2;
                settingInfo.StringValue = BasicCbQueryScreenRecord.IsChecked == true ? "1" : "0";
                listSettingInfos.Add(settingInfo);
                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_AUTORELATIVEPLAY;
                settingInfo.GroupID = 310201;
                settingInfo.SortID = 6;
                settingInfo.DataType = 2;
                settingInfo.StringValue = BasicCbAutoRelativePlay.IsChecked == true ? "1" : "0";
                listSettingInfos.Add(settingInfo);
                OperationReturn optReturn = SaveConfig(listSettingInfos);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }

                #region 写操作日志

                strLog = string.Format("{0} {1}", Utils.FormatOptLogString("31021210"), strLog);
                CurrentApp.WriteOperationLog(S3102Consts.OPT_CUSTOMSETTING.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);

                #endregion

                CurrentApp.ShowInfoMessage(CurrentApp.GetMessageLanguageInfo("008", "Save config end"));
                if (PageParent != null)
                {
                    PageParent.ReloadUserSettings();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SaveDataExportConfig()
        {
            try
            {
                string strLog = string.Empty;
                List<SettingInfo> listSettingInfos = new List<SettingInfo>();
                SettingInfo settingInfo;

                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_EXPORTDATA_REMEMBER;
                settingInfo.GroupID = 310202;
                settingInfo.SortID = 0;
                settingInfo.DataType = 2;
                settingInfo.StringValue = ExportDataCbRemember.IsChecked == true ? "1" : "0";
                listSettingInfos.Add(settingInfo);
                if (ExportDataCbRemember.IsChecked == true)
                {
                    strLog += string.Format("{0} ", Utils.FormatOptLogString("31021503"));
                }

                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_EXPORTDATA_TYPE;
                settingInfo.GroupID = 310202;
                settingInfo.SortID = 1;
                settingInfo.DataType = 2;
                settingInfo.StringValue = ExportDataRadioCurrentPage.IsChecked == true
                    ? "2"
                    : ExportDataRadioAllPage.IsChecked == true ? "3" : "1";
                listSettingInfos.Add(settingInfo);
                strLog += string.Format("{0} ", ExportDataRadioCurrentPage.IsChecked == true
                        ? Utils.FormatOptLogString("31021501")
                        : ExportDataRadioAllPage.IsChecked == true ? Utils.FormatOptLogString("31021502") : Utils.FormatOptLogString("31021500"));

                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_EXPORTDATA_NOTSHOW;
                settingInfo.GroupID = 310202;
                settingInfo.SortID = 2;
                settingInfo.DataType = 2;
                settingInfo.StringValue = ExportDataCbNotShow.IsChecked == true ? "1" : "0";
                listSettingInfos.Add(settingInfo);
                if (ExportDataCbNotShow.IsChecked == true)
                {
                    strLog += string.Format("{0} ", Utils.FormatOptLogString("31021504"));
                }

                OperationReturn optReturn = SaveConfig(listSettingInfos);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }

                #region 写操作日志

                strLog = string.Format("{0} {1}", Utils.FormatOptLogString("31021213"), strLog);
                CurrentApp.WriteOperationLog(S3102Consts.OPT_CUSTOMSETTING.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);

                #endregion

                CurrentApp.ShowInfoMessage(CurrentApp.GetMessageLanguageInfo("008", "Save config end"));
                if (PageParent != null)
                {
                    PageParent.ReloadUserSettings();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SaveRecordExportConfig()
        {
            try
            {
                string strLog = string.Empty;
                if (string.IsNullOrEmpty(ExportRecordTxtSaveDir.Text))
                {
                    ShowException(string.Format(CurrentApp.GetLanguageInfo("3102N039","Save directory is empty")));
                    return;
                }
                if (string.IsNullOrEmpty(ExportRecordTxtPathFormat.Text))
                {
                    ShowException(string.Format(CurrentApp.GetLanguageInfo("3102N040","Path format is empty")));
                    return;
                }

                List<SettingInfo> listSettingInfos = new List<SettingInfo>();
                SettingInfo settingInfo;

                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_EXPORTRECORD_REMEMBER;
                settingInfo.GroupID = 310203;
                settingInfo.SortID = 0;
                settingInfo.DataType = 2;
                settingInfo.StringValue = ExportRecordCbRemember.IsChecked == true ? "1" : "0";
                listSettingInfos.Add(settingInfo);
                if (ExportRecordCbRemember.IsChecked == true)
                {
                    strLog += string.Format("{0} ", Utils.FormatOptLogString("31021600"));
                }

                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_EXPORTRECORD_NOTSHOW;
                settingInfo.GroupID = 310203;
                settingInfo.SortID = 1;
                settingInfo.DataType = 2;
                settingInfo.StringValue = ExportRecordCbNotShow.IsChecked == true ? "1" : "0";
                listSettingInfos.Add(settingInfo);
                if (ExportRecordCbNotShow.IsChecked == true)
                {
                    strLog += string.Format("{0} ", Utils.FormatOptLogString("31021601"));
                }

                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_EXPORTRECORD_SAVEDIR;
                settingInfo.GroupID = 310203;
                settingInfo.SortID = 2;
                settingInfo.DataType = 14;
                settingInfo.StringValue = ExportRecordTxtSaveDir.Text;
                listSettingInfos.Add(settingInfo);
                strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("31021602"),
                    ExportRecordTxtSaveDir.Text);

                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_EXPORTRECORD_PATHFORMAT;
                settingInfo.GroupID = 310203;
                settingInfo.SortID = 3;
                settingInfo.DataType = 14;
                settingInfo.StringValue = ExportRecordTxtPathFormat.Text;
                listSettingInfos.Add(settingInfo);
                strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("31021603"),
                    ExportRecordTxtPathFormat.Text);

                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_EXPORTRECORD_ENCRYPTPASSWORD;
                settingInfo.GroupID = 310203;
                settingInfo.SortID = 3;
                settingInfo.DataType = 14;
                settingInfo.StringValue = S3102App.EncryptString(ExportRecordTxtEncryptPassword.Password);
                listSettingInfos.Add(settingInfo);
                strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("31021612"),
                    ExportRecordTxtPathFormat.Text);

                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_EXPORTRECORD_IGNOREPATHFORMAT;
                settingInfo.GroupID = 310203;
                settingInfo.SortID = 4;
                settingInfo.DataType = 2;
                settingInfo.StringValue = ExportRecordCbIgnorePathFormat.IsChecked == true ? "1" : "0";
                listSettingInfos.Add(settingInfo);
                if (ExportRecordCbIgnorePathFormat.IsChecked == true)
                {
                    strLog += string.Format("{0} ", Utils.FormatOptLogString("31021610"));
                }

                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_EXPORTRECORD_ENCRYPTRECORD;
                settingInfo.GroupID = 310203;
                settingInfo.SortID = 4;
                settingInfo.DataType = 2;
                settingInfo.StringValue = ExportRecordCbEncryptRecord.IsChecked == true ? "1" : "0";
                listSettingInfos.Add(settingInfo);
                if (ExportRecordCbEncryptRecord.IsChecked == true)
                {
                    strLog += string.Format("{0} ", Utils.FormatOptLogString("31021610"));
                }

                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_EXPORTRECORD_CONVERTPCM;
                settingInfo.GroupID = 310203;
                settingInfo.SortID = 5;
                settingInfo.DataType = 2;
                settingInfo.StringValue = ExportRecordCbConvertPCM.IsChecked == true ? "1" : "0";
                listSettingInfos.Add(settingInfo);
                if (ExportRecordCbConvertPCM.IsChecked == true)
                {
                    strLog += string.Format("{0} ", Utils.FormatOptLogString("31021604"));
                }

                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_EXPORTRECORD_DECRYPTFILE;
                settingInfo.GroupID = 310203;
                settingInfo.SortID = 6;
                settingInfo.DataType = 2;
                settingInfo.StringValue = ExportRecordCbDecryptFile.IsChecked == true ? "1" : "0";
                listSettingInfos.Add(settingInfo);
                if (ExportRecordCbDecryptFile.IsChecked == true)
                {
                    strLog += string.Format("{0} ", Utils.FormatOptLogString("31021605"));
                }

                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_EXPORTRECORD_GENERATEDB;
                settingInfo.GroupID = 310203;
                settingInfo.SortID = 7;
                settingInfo.DataType = 2;
                settingInfo.StringValue = ExportRecordCbGenerateDB.IsChecked == true ? "1" : "0";
                listSettingInfos.Add(settingInfo);
                if (ExportRecordCbGenerateDB.IsChecked == true)
                {
                    strLog += string.Format("{0} ", Utils.FormatOptLogString("31021606"));
                }

                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_EXPORTRECORD_REPLACEFILE;
                settingInfo.GroupID = 310203;
                settingInfo.SortID = 8;
                settingInfo.DataType = 2;
                settingInfo.StringValue = ExportRecordCbReplaceFile.IsChecked == true ? "1" : "0";
                listSettingInfos.Add(settingInfo);
                if (ExportRecordCbReplaceFile.IsChecked == true)
                {
                    strLog += string.Format("{0} ", Utils.FormatOptLogString("31021607"));
                }

                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_EXPORTRECORD_EXPORTVOICE;
                settingInfo.GroupID = 310203;
                settingInfo.SortID = 9;
                settingInfo.DataType = 2;
                settingInfo.StringValue = ExportRecordCbExportVoice.IsChecked == true ? "1" : "0";
                listSettingInfos.Add(settingInfo);
                if (ExportRecordCbExportVoice.IsChecked == true)
                {
                    strLog += string.Format("{0} ", Utils.FormatOptLogString("31021608"));
                }

                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_EXPORTRECORD_EXPORTSCREEN;
                settingInfo.GroupID = 310203;
                settingInfo.SortID = 10;
                settingInfo.DataType = 2;
                settingInfo.StringValue = ExportRecordCbExportScreen.IsChecked == true ? "1" : "0";
                listSettingInfos.Add(settingInfo);
                if (ExportRecordCbExportScreen.IsChecked == true)
                {
                    strLog += string.Format("{0} ", Utils.FormatOptLogString("31021609"));
                }

                OperationReturn optReturn = SaveConfig(listSettingInfos);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }

                #region 写操作日志

                strLog = string.Format("{0} {1}", Utils.FormatOptLogString("31021214"), strLog);
                CurrentApp.WriteOperationLog(S3102Consts.OPT_CUSTOMSETTING.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);

                #endregion

                CurrentApp.ShowInfoMessage(CurrentApp.GetMessageLanguageInfo("008", "Save config end"));
                if (PageParent != null)
                {
                    PageParent.ReloadUserSettings();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private OperationReturn SaveConfig(List<SettingInfo> listSettingInfos)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.SaveUserSettingInfos;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(listSettingInfos.Count.ToString());
                for (int i = 0; i < listSettingInfos.Count; i++)
                {
                    optReturn = XMLHelper.SeriallizeObject(listSettingInfos[i]);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    webRequest.ListData.Add(optReturn.Data.ToString());
                }
                Service31021Client client = new Service31021Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    optReturn.Result = false;
                    optReturn.Code = webReturn.Code;
                    optReturn.Message = webReturn.Message;
                    return optReturn;
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private void SaveColumnInfos()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.SaveViewColumnInfos;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("3102001");
                int intCount = mListCustomColumns.Count;
                webRequest.ListData.Add(intCount.ToString());
                OperationReturn optReturn;
                for (int i = 0; i < intCount; i++)
                {
                    optReturn = XMLHelper.SeriallizeObject(mListCustomColumns[i].ViewColumnInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    webRequest.ListData.Add(optReturn.Data.ToString());
                }
                Service31021Client client = new Service31021Client(
                   WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                   WebHelper.CreateEndpointAddress(
                       CurrentApp.Session.AppServerInfo,
                       "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (PageParent != null)
                {
                    PageParent.ReloadRecordDataColumns();
                }

                #region 写操作日志

                string strLog = string.Format("{0}", Utils.FormatOptLogString("31021211"));
                CurrentApp.WriteOperationLog(S3102Consts.OPT_CUSTOMSETTING.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);

                #endregion

                CurrentApp.ShowInfoMessage(CurrentApp.GetMessageLanguageInfo("008", "Save config end"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SaveConditionInfos()
        {
            if (mConditionDesigner != null)
            {
                OperationReturn optReturn = mConditionDesigner.SaveConfig();
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }

                #region 写操作日志

                string strLog = string.Format("{0}", Utils.FormatOptLogString("31021212"));
                CurrentApp.WriteOperationLog(S3102Consts.OPT_CUSTOMSETTING.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);

                #endregion

                CurrentApp.ShowInfoMessage(CurrentApp.GetMessageLanguageInfo("008", "Save config end"));
            }
        }

        private void SaveBookmarkRankInfos()
        {
            if (mBookmarkRankSetting != null)
            {
                OperationReturn optReturn = mBookmarkRankSetting.SaveConfig();
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }

                #region 写操作日志

                string strLog = string.Format("{0}", Utils.FormatOptLogString("31021215"));
                CurrentApp.WriteOperationLog(S3102Consts.OPT_CUSTOMSETTING.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);

                #endregion

                CurrentApp.ShowInfoMessage(CurrentApp.GetMessageLanguageInfo("008", "Save config end"));
            }
        }

        private void SavePlayScreenInfos()
        {
            try
            {
                string strLog = string.Empty;
                if (PlayScreenCbNoPlay.IsChecked == true)
                {
                    strLog += string.Format("{0} ", Utils.FormatOptLogString("31022001"));
                }
                if (PlayScreenCbTopMost.IsChecked == true)
                {
                    strLog += string.Format("{0} ", Utils.FormatOptLogString("31022002"));
                }
                int intScale;
                var tabItem = PlayScreenComboScale.SelectedItem as ComboBoxItem;
                if (tabItem == null)
                {
                    ShowException(string.Format("Scale invalid"));
                    return;
                }
                if (!int.TryParse(tabItem.Tag.ToString(), out intScale))
                {
                    ShowException(string.Format("Scale invalid"));
                    return;
                }
                strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("31022003"), intScale);
                List<SettingInfo> listSettingInfos = new List<SettingInfo>();
                SettingInfo settingInfo;
                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_PLAYSCREEN_NOPLAY;
                settingInfo.GroupID = S3102Consts.USER_PARAM_GROUP_PLAYSCREEN;
                settingInfo.SortID = 0;
                settingInfo.DataType = 2;
                settingInfo.StringValue = PlayScreenCbNoPlay.IsChecked == true ? "1" : "0";
                listSettingInfos.Add(settingInfo);
                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_PLAYSCREEN_TOPMOST;
                settingInfo.GroupID = S3102Consts.USER_PARAM_GROUP_PLAYSCREEN;
                settingInfo.SortID = 1;
                settingInfo.DataType = 2;
                settingInfo.StringValue = PlayScreenCbTopMost.IsChecked == true ? "1" : "0";
                listSettingInfos.Add(settingInfo);
                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_PLAYSCREEN_SCALE;
                settingInfo.GroupID = S3102Consts.USER_PARAM_GROUP_PLAYSCREEN;
                settingInfo.SortID = 2;
                settingInfo.DataType = 2;
                settingInfo.StringValue = intScale.ToString();
                listSettingInfos.Add(settingInfo);
               
                OperationReturn optReturn = SaveConfig(listSettingInfos);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }

                #region 写操作日志

                strLog = string.Format("{0} {1}", Utils.FormatOptLogString("31021216"), strLog);
                CurrentApp.WriteOperationLog(S3102Consts.OPT_CUSTOMSETTING.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);

                #endregion

                CurrentApp.ShowInfoMessage(CurrentApp.GetMessageLanguageInfo("008", "Save config end"));
                if (PageParent != null)
                {
                    PageParent.ReloadUserSettings();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                BtnApply.Content = CurrentApp.GetLanguageInfo("31022", "Apply");
                BtnClose.Content = CurrentApp.GetLanguageInfo("31021", "Close");

                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.Title = CurrentApp.GetLanguageInfo("31021200", "Custom Setting");
                }
                TabBasic.Header = CurrentApp.GetLanguageInfo("31021210", "Basic");
                TabColumnSetting.Header = CurrentApp.GetLanguageInfo("31021211", "Record Column");
                TabCustomCondition.Header = CurrentApp.GetLanguageInfo("31021212", "Select Condition");
                TabDataExport.Header = CurrentApp.GetLanguageInfo("31021213", "Data Export");
                TabRecordExport.Header = CurrentApp.GetLanguageInfo("31021214", "Record Export");
                TabBookmarkRank.Header = CurrentApp.GetLanguageInfo("31021215", "Bookmark Rank");
                TabPlayScreen.Header = CurrentApp.GetLanguageInfo("31021216", "Screen Play");

                BasicLbPageSize.Text = CurrentApp.GetLanguageInfo("31021300", "Page Size");
                BasicLbMaxRecords.Text = CurrentApp.GetLanguageInfo("31021301", "Max Records");
                BasicCbSkipConditionPanel.Content = CurrentApp.GetLanguageInfo("31021302","Skip condition panel when QuickQuery");
                BasicCbQueryVoiceRecord.Content = CurrentApp.GetLanguageInfo("31021303","Query voice record");
                BasicCbQueryScreenRecord.Content = CurrentApp.GetLanguageInfo("31021304","Query screen record");
                BasicCbAutoRelativePlay.Content = CurrentApp.GetLanguageInfo("31021305","Auto relative screen record");
                BasicCbSkipPasswordPanel.Content = CurrentApp.GetLanguageInfo("31021306", "Skip password panel if password remember");

                ColumnCbIsVisible.Content = CurrentApp.GetLanguageInfo("31021400", "Is Visible");

                ExportDataRadioCurrentSelected.Content = CurrentApp.GetLanguageInfo("31021500", "Current Selected Records");
                ExportDataRadioCurrentPage.Content = CurrentApp.GetLanguageInfo("31021501", "Current Page");
                ExportDataRadioAllPage.Content = CurrentApp.GetLanguageInfo("31021502", "All Page");
                ExportDataCbRemember.Content = CurrentApp.GetLanguageInfo("31021503", "Remember current config");
                ExportDataCbNotShow.Content = CurrentApp.GetLanguageInfo("31021504", "Not show next time");

                ExportRecordCbRemember.Content = CurrentApp.GetLanguageInfo("31021600", "Remember current config");
                ExportRecordCbNotShow.Content = CurrentApp.GetLanguageInfo("31021601", "Not show next time");
                ExportRecordLbSaveDir.Text = CurrentApp.GetLanguageInfo("31021602", "Save directory");
                ExportRecordLbPathForm.Text = CurrentApp.GetLanguageInfo("31021603", "Path format");
                ExportRecordLbEncryptPassword.Text = CurrentApp.GetLanguageInfo("31021612", "Encrypt password");
                ExportRecordCbIgnorePathFormat.Content = CurrentApp.GetLanguageInfo("31021610", "Ignore path format");
                ExportRecordCbEncryptRecord.Content = CurrentApp.GetLanguageInfo("31021611", "Encrypt record while export");
                ExportRecordCbConvertPCM.Content = CurrentApp.GetLanguageInfo("31021604", "Convert to PCM format");
                ExportRecordCbDecryptFile.Content = CurrentApp.GetLanguageInfo("31021605", "Decrypt file");
                ExportRecordCbGenerateDB.Content = CurrentApp.GetLanguageInfo("31021606", "Generate database");
                ExportRecordCbReplaceFile.Content = CurrentApp.GetLanguageInfo("31021607", "Replace file");
                ExportRecordCbExportVoice.Content = CurrentApp.GetLanguageInfo("31021608", "Export voice");
                ExportRecordCbExportScreen.Content = CurrentApp.GetLanguageInfo("31021609", "Export screen");

                PlayScreenCbNoPlay.Content = CurrentApp.GetLanguageInfo("31022001", "Never Player");
                PlayScreenCbTopMost.Content = CurrentApp.GetLanguageInfo("31022002", "Player Top Most");
                PlayScreenLbScale.Text = CurrentApp.GetLanguageInfo("31022003", "Player Scale");

                ColumnBtnApply.ToolTip = CurrentApp.GetLanguageInfo("3102TOOLTIP0001","Set");

                //视图列
                CreateColumnViewColumns();

                //子控件
                if (mConditionDesigner != null)
                {
                    mConditionDesigner.ChangeLanguage();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion

    }
}
