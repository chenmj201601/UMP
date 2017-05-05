//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    1995608e-5fa3-40a1-9f73-6673899c04c8
//        CLR Version:              4.0.30319.18444
//        Name:                     UCExportRecordOption
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102
//        File Name:                UCExportRecordOption
//
//        created by Charley at 2014/11/20 15:30:18
//        http://www.voicecyber.com
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using UMPS3102.Codes;
using UMPS3102.Models;
using UMPS3102.Wcf11012;
using UMPS3102.Wcf31021;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31021;
using VoiceCyber.UMP.CommonService03;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using Binding = System.Windows.Data.Binding;

namespace UMPS3102
{
    /// <summary>
    /// UCExportRecordOption.xaml 的交互逻辑
    /// </summary>
    public partial class UCExportRecordOption
    {

        #region Members

        private const string PATH_FORMAT_DEFUALT = @"\*YYYY*MM\*DD\*4VID\*5CID\";

        private const int STATE_WAITING = 0;
        private const int STATE_SUCCESS = 1;
        private const int STATE_FAIL = 2;
        private const int STATE_DOWNLOADING = 3;

        public QMMainView MainPage;
        public List<RecordInfoItem> ListRecordItems;
        public List<SftpServerInfo> ListSftpServers;
        public List<DownloadParamInfo> ListDownloadParams;
        public List<SettingInfo> ListUserSettingInfos;
        public List<RecordEncryptInfo> ListEncryptInfo;
        public Service03Helper Service03Helper;

        private List<SettingInfo> mListSettingInfos;
        private List<ViewColumnInfo> mListDownloadColumns;
        private ObservableCollection<RecordDownloadItem> mListDownloadItems;
        private List<string> mListServerAddresses;
        private RecordDownloadItem mCurrentItem;
        private BackgroundWorker mWorker;

        #endregion


        public UCExportRecordOption()
        {
            InitializeComponent();

            mListSettingInfos = new List<SettingInfo>();
            mListDownloadColumns = new List<ViewColumnInfo>();
            mListDownloadItems = new ObservableCollection<RecordDownloadItem>();
            mListServerAddresses = new List<string>();

            Loaded += UCExportRecordOption_Loaded;
            BtnApply.Click += BtnApply_Click;
            BtnClose.Click += BtnClose_Click;
            BtnBrowser.Click += BtnBrowser_Click;
            BtnAvaliable.Click += BtnAvaliable_Click;
            CbRemember.Click += CbRemember_Click;
            CbIgnorePathFormat.Click += CbIgnorePathFormat_Click;
            CbEncryptRecord.Click += CbEncryptRecord_Click;
        }

        private void UCExportRecordOption_Loaded(object sender, RoutedEventArgs e)
        {
            LvExportRecord.ItemsSource = mListDownloadItems;

            InitDownloadColumns();
            LoadUserSettingInfos();
            Init();
            CreateDownloadColumns();
            InitDownloadItems();
            ChangeLanguage();

            //自动导出
            AutoExportRecord();
        }


        #region Init and Load

        private void InitDownloadColumns()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserViewColumnList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("3102007");
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
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
                mListDownloadColumns.Clear();
                for (int i = 0; i < listColumns.Count; i++)
                {
                    mListDownloadColumns.Add(listColumns[i]);
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
                SettingInfo settingInfo;
                bool isRemember = false;
                settingInfo =
                    mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_REMEMBER);
                if (settingInfo != null && settingInfo.StringValue == "1")
                {
                    isRemember = true;
                }
                CbRemember.IsChecked = isRemember;
                TxtSaveDir.Text = string.Empty;
                CbNotShow.IsChecked = false;
                TxtPathFormat.Text = PATH_FORMAT_DEFUALT;
                CbIgnorePathFormat.IsChecked = true;
                CbEncryptRecord.IsChecked = false;
                TxtEncryptPassword.Password = string.Empty;
                CbConvertPCM.IsChecked = true;
                CbDecrypFile.IsChecked = true;
                CbGenerateDB.IsChecked = false;
                CbReplaceFile.IsChecked = true;
                CbExportVoice.IsChecked = true;
                CbExportScreen.IsChecked = true;

                settingInfo =
                    mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_SAVEDIR);
                if (settingInfo != null && isRemember)
                {
                    if (!string.IsNullOrEmpty(settingInfo.StringValue))
                    {
                        TxtSaveDir.Text = settingInfo.StringValue;
                    }
                }
                settingInfo =
                    mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_NOTSHOW);
                if (settingInfo != null && isRemember)
                {
                    if (settingInfo.StringValue == "1")
                    {
                        CbNotShow.IsChecked = true;
                    }
                }
                settingInfo =
                    mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_PATHFORMAT);
                if (settingInfo != null && isRemember)
                {
                    if (!string.IsNullOrEmpty(settingInfo.StringValue))
                    {
                        TxtPathFormat.Text = settingInfo.StringValue;
                    }
                }
                settingInfo =
                   mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_IGNOREPATHFORMAT);
                if (settingInfo != null && isRemember)
                {
                    if (settingInfo.StringValue == "0")
                    {
                        CbIgnorePathFormat.IsChecked = false;
                    }
                }
                settingInfo =
                 mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_ENCRYPTRECORD);
                if (settingInfo != null && isRemember)
                {
                    if (settingInfo.StringValue == "1")
                    {
                        CbEncryptRecord.IsChecked = true;
                    }
                }
                settingInfo =
                  mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_ENCRYPTPASSWORD);
                if (settingInfo != null && isRemember)
                {
                    if (!string.IsNullOrEmpty(settingInfo.StringValue))
                    {
                        TxtEncryptPassword.Password = S3102App.DecryptString(settingInfo.StringValue);
                    }
                }
                settingInfo =
                    mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_CONVERTPCM);
                if (settingInfo != null && isRemember)
                {
                    if (settingInfo.StringValue == "0")
                    {
                        CbConvertPCM.IsChecked = false;
                    }
                }
                settingInfo =
                    mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_DECRYPTFILE);
                if (settingInfo != null && isRemember)
                {
                    if (settingInfo.StringValue == "0")
                    {
                        CbDecrypFile.IsChecked = false;
                    }
                }
                settingInfo =
                    mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_GENERATEDB);
                if (settingInfo != null && isRemember)
                {
                    if (settingInfo.StringValue == "1")
                    {
                        CbGenerateDB.IsChecked = true;
                    }
                }
                settingInfo =
                    mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_REPLACEFILE);
                if (settingInfo != null && isRemember)
                {
                    if (settingInfo.StringValue == "0")
                    {
                        CbReplaceFile.IsChecked = false;
                    }
                }
                settingInfo =
                    mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_EXPORTVOICE);
                if (settingInfo != null && isRemember)
                {
                    if (settingInfo.StringValue == "0")
                    {
                        CbExportVoice.IsChecked = false;
                    }
                }
                settingInfo =
                    mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_EXPORTSCREEN);
                if (settingInfo != null && isRemember)
                {
                    if (settingInfo.StringValue == "0")
                    {
                        CbExportScreen.IsChecked = false;
                    }
                }

                //生成数据库信息，暂未实现，先不可用
                CbGenerateDB.IsEnabled = false;

                //设置PathFormat可用性
                TxtPathFormat.IsEnabled = CbIgnorePathFormat.IsChecked != true;
                BtnAvaliable.IsEnabled = CbIgnorePathFormat.IsChecked != true;

                //设置EncryptPassword可用性
                TxtEncryptPassword.IsEnabled = CbEncryptRecord.IsChecked == true;
                //导出加密文件，必须要选择解密文件
                CbDecrypFile.IsEnabled = CbEncryptRecord.IsChecked != true;
                if (CbEncryptRecord.IsChecked == true)
                {
                    CbDecrypFile.IsChecked = true;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitDownloadItems()
        {
            try
            {
                mListDownloadItems.Clear();
                if (ListRecordItems != null)
                {
                    for (int i = 0; i < ListRecordItems.Count; i++)
                    {
                        RecordInfoItem recordItem = ListRecordItems[i];
                        RecordInfo recordInfo = recordItem.RecordInfo;
                        if (recordInfo != null)
                        {
                            if (recordInfo.MediaType == 3)
                            {
                                //艺赛旗录屏不能导出，这里直接忽略调
                                continue;
                            }
                        }
                        RecordDownloadItem item = new RecordDownloadItem();
                        item.RowNumber = i + 1;
                        item.SerialID = recordItem.SerialID;
                        item.RecordLength = Converter.Second2Time(recordItem.Duration);
                        item.State = string.Empty;
                        item.Foreground = Brushes.Black;
                        item.Error = string.Empty;
                        item.RecordItem = recordItem;
                        mListDownloadItems.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                CurrentApp.ShowInfoMessage(ex.Message);
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
                webRequest.ListData.Add("310203");
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

        private void CreateDownloadColumns()
        {
            try
            {
                GridView gv = new GridView();
                GridViewColumn gvc;
                GridViewColumnHeader gvch;
                for (int i = 0; i < mListDownloadColumns.Count; i++)
                {
                    ViewColumnInfo columnInfo = mListDownloadColumns[i];
                    if (columnInfo.Visibility == "1")
                    {
                        gvc = new GridViewColumn();
                        gvch = new GridViewColumnHeader();
                        gvch.Content = columnInfo.Display;
                        gvch.Content = CurrentApp.GetLanguageInfo(string.Format("COL3102007{0}", columnInfo.ColumnName),
                            columnInfo.Display);
                        gvch.ToolTip = CurrentApp.GetLanguageInfo(string.Format("COL3102007{0}", columnInfo.ColumnName),
                            columnInfo.Display);
                        gvc.Header = gvch;
                        gvc.Width = columnInfo.Width;
                        gvc.DisplayMemberBinding = new Binding(columnInfo.ColumnName);
                        gv.Columns.Add(gvc);
                    }
                }
                LvExportRecord.View = gv;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region EventHandlers

        private void CbRemember_Click(object sender, RoutedEventArgs e)
        {
            if (CbRemember.IsChecked == false)
            {
                CbNotShow.IsChecked = false;
            }
            CbNotShow.IsEnabled = CbRemember.IsChecked == true;
        }

        private void BtnAvaliable_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnBrowser_Click(object sender, RoutedEventArgs e)
        {
            //====================================在导出录音的地方加了几个提示的语言包=========================
            //=====================================by 汤澈=====================================================
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = string.Format(CurrentApp.GetLanguageInfo("3102N020", "Please select save directory"));
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string dir = dialog.SelectedPath;
                if (!Directory.Exists(dir))
                {
                    ShowException(string.Format(CurrentApp.GetLanguageInfo("3102N020", "Directory not exist")));
                    return;
                }
                TxtSaveDir.Text = dir;
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

        private void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckInput())
            {
                return;
            }
            SetConfig();
            if (CbRemember.IsChecked == true)
            {
                SaveConfig();
            }
            DownloadRecordList();
        }

        void CbIgnorePathFormat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TxtPathFormat.IsEnabled = CbIgnorePathFormat.IsChecked != true;
                BtnAvaliable.IsEnabled = CbIgnorePathFormat.IsChecked != true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void CbEncryptRecord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TxtEncryptPassword.IsEnabled = CbEncryptRecord.IsChecked == true;
                CbDecrypFile.IsEnabled = CbEncryptRecord.IsChecked != true;
                if (CbEncryptRecord.IsChecked == true)
                {
                    CbDecrypFile.IsChecked = true;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void PasswordManagement_PasswordManagementEvent(object sender, RoutedPropertyChangedEventArgs<UMPEventArgs> e)
        {
            try
            {
                var args = e.NewValue;
                if (args == null) { return; }
                switch (args.Code)
                {
                    case PasswordManagerEventCode.PASS_SETTED:
                        RecordEncryptInfo info = args.Data as RecordEncryptInfo;
                        if (info == null) { return; }
                        info.RealPassword = info.Password;      //默认把输入密钥作为实际解密密钥
                        GetRealPassword(info);
                        break;
                    case PasswordManagerEventCode.PASS_CANCEL:

                        #region 写操作日志

                        string strLog = string.Format("{0} {1}", mListDownloadItems.Count, Utils.FormatOptLogString("31021002"));
                        CurrentApp.WriteOperationLog(S3102Consts.OPT_EXPORTRECORD.ToString(), ConstValue.OPT_RESULT_CANCEL, strLog);

                        #endregion

                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Operations

        private void AutoExportRecord()
        {
            try
            {
                var setting = mListSettingInfos.FirstOrDefault(t => t.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_NOTSHOW);
                if (setting != null
                    && setting.StringValue == "1")
                {
                    //自动导出
                    DownloadRecordList();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SaveConfig()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.SaveUserSettingInfos;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(mListSettingInfos.Count.ToString());
                for (int i = 0; i < mListSettingInfos.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.SeriallizeObject(mListSettingInfos[i]);
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
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DownloadRecordList()
        {
            try
            {
                if (TabSetting.Items.Count > 0)
                {
                    TabSetting.SelectedIndex = 0;
                }
                for (int i = 0; i < mListDownloadItems.Count; i++)
                {
                    RecordDownloadItem item = mListDownloadItems[i];

                    ////临时操作
                    //var recordItem = item.RecordItem;
                    //if (recordItem != null)
                    //{
                    //    if (recordItem.EncryptFlag != "0")
                    //    {
                    //        ShowException(CurrentApp.GetMessageLanguageInfo("044",
                    //            "Encrypt record exists, please export record by UMP CQC."));
                    //        return;
                    //    }
                    //}

                    SetDownloadItemState(item, Brushes.Black, STATE_WAITING, string.Empty);
                }
                if (Service03Helper == null)
                {
                    ShowException(string.Format("Fail.\tService03Helper is null"));
                    return;
                }
                if (mListDownloadItems.Count <= 0)
                {
                    return;
                }
                mListServerAddresses.Clear();
                mCurrentItem = mListDownloadItems[0];
                PrepareDownloadRecordItem();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void PrepareDownloadRecordItem()
        {
            if (mCurrentItem == null) { return; }
            try
            {
                RecordInfoItem recordItem = mCurrentItem.RecordItem;
                if (recordItem == null) { return; }
                RecordInfo recordInfo = recordItem.RecordInfo;
                if (recordInfo == null) { return; }
                bool isNotDecrypt = false;
                SettingInfo setting;
                setting =
                    mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_DECRYPTFILE);
                if (setting != null && setting.StringValue == "0")
                {
                    isNotDecrypt = true;
                }
                if (!isNotDecrypt)
                {
                    if (!mListServerAddresses.Contains(recordInfo.VoiceIP))
                    {
                        //判断是否需要解密
                        if (recordInfo.EncryptFlag != "0")
                        {
                            SetDecryptPassword();
                            return;
                        }
                    }
                }
                //从第一条记录循环到最后一条记录，依次弹出输入密钥的面板
                int index = mListDownloadItems.IndexOf(mCurrentItem);
                if (index < mListDownloadItems.Count - 1)
                {
                    mCurrentItem = mListDownloadItems[index + 1];
                    PrepareDownloadRecordItem();
                    return;
                }

                //解密处理完成后
                if (MainPage != null)
                {
                    MainPage.SetBusy(true, string.Format("{0}",
                                 CurrentApp.GetMessageLanguageInfo("015", "Downloading record file...")));
                }
                BtnApply.IsEnabled = false;
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    try
                    {
                        for (int i = 0; i < mListDownloadItems.Count; i++)
                        {
                            mCurrentItem = mListDownloadItems[i];
                            SetDownloadItemState(mCurrentItem, Brushes.Blue, STATE_DOWNLOADING, string.Empty);
                            Dispatcher.Invoke(new Action(() => LvExportRecord.ScrollIntoView(mCurrentItem)));
                            DownloadRecordItem();
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowException(ex.Message);
                    }
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    if (MainPage != null)
                    {
                        MainPage.SetBusy(false,string.Empty);
                    }
                    BtnApply.IsEnabled = true;

                    #region 写操作日志

                    string strLog = string.Format("{0} {1}", mListDownloadItems.Count, Utils.FormatOptLogString("31021002"));
                    CurrentApp.WriteOperationLog(S3102Consts.OPT_EXPORTRECORD.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);

                    #endregion

                    setting = mListSettingInfos.FirstOrDefault(t => t.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_NOTSHOW);
                    if (setting != null
                        && setting.StringValue == "1")
                    {
                        //自动关闭面板
                        var parent = Parent as PopupPanel;
                        if (parent != null)
                        {
                            parent.IsOpen = false;
                            return;
                        }
                    }
                    //这里是当后台操作已完成或者已取消或者异常的时候  这里应该出现的提示框应该不一样  这里应该修改下，来个什么标志之类的 放在这里    2015年10月12日  
                    CurrentApp.ShowInfoMessage(CurrentApp.GetMessageLanguageInfo("007", "Export record end"));
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                SetDownloadItemState(mCurrentItem, Brushes.Red, STATE_FAIL, ex.Message);
            }
        }

        private void DownloadRecordItem()
        {
            if (mCurrentItem == null) { return; }
            try
            {
                RecordInfoItem recordItem = mCurrentItem.RecordItem;
                if (recordItem == null) { return; }
                RecordInfo recordInfo = recordItem.RecordInfo;
                if (recordInfo == null) { return; }
                RecordInfo relativeRecord;
                string fileName;
                string relativeName = string.Empty;
                string strLog;
                OperationReturn optReturn;

                //获取关联的录屏文件
                GetRelativeRecordInfos();

                //处理录音记录
                RecordOperator recordOperator = new RecordOperator(recordInfo);
                recordOperator.Debug += (cat, msg) => CurrentApp.WriteLog(cat, msg);
                recordOperator.Session = CurrentApp.Session;
                recordOperator.ListSftpServers = ListSftpServers;
                recordOperator.ListDownloadParams = ListDownloadParams;
                recordOperator.ListEncryptInfo = ListEncryptInfo;
                recordOperator.Service03Helper = Service03Helper;
                //下载文件到AppServer
                optReturn = recordOperator.DownloadFileToAppServer();
                if (!optReturn.Result)
                {
                    ShowRecordOperatorMessage(optReturn);
                    CurrentApp.WriteLog("DownloadAppServer",
                        string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));

                    #region 写操作日志
                    optReturn.Message = CurrentApp.GetLanguageInfo("31021652", optReturn.Message);
                    strLog = string.Format("{0}", string.Format("{0}\t{1}", optReturn.Code, optReturn.Message));
                    CurrentApp.WriteOperationLog(S3102Consts.OPT_EXPORTRECORD.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                    #endregion

                    return;
                }
                fileName = optReturn.Data.ToString();
                relativeRecord = recordItem.ListRelativeInfos.FirstOrDefault();
                if (relativeRecord != null)
                {
                    //如果有关联的录屏文件，下载录屏文件到AppServer
                    recordOperator.RecordInfo = relativeRecord;
                    optReturn = recordOperator.DownloadFileToAppServer();
                    if (!optReturn.Result)
                    {
                        CurrentApp.WriteLog("DownloadAppServer",
                            string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));

                        #region 写操作日志
                        optReturn.Message = CurrentApp.GetLanguageInfo("31021652", optReturn.Message);
                        strLog = string.Format("{0}", string.Format("{0}\t{1}", optReturn.Code, optReturn.Message));
                        CurrentApp.WriteOperationLog(S3102Consts.OPT_EXPORTRECORD.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                        #endregion

                    }
                    else
                    {
                        relativeName = optReturn.Data.ToString();
                    }
                }

                if (recordInfo.RecordVersion == 101)
                {
                    //华为V3录音不需要解密，此处不做操作

                }
                else
                {
                    recordOperator.RecordInfo = recordInfo;
                    //原始解密
                    optReturn = recordOperator.OriginalDecryptRecord(fileName);
                    if (!optReturn.Result)
                    {
                        ShowRecordOperatorMessage(optReturn);
                        CurrentApp.WriteLog("OriginalDecrypt",
                            string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));

                        #region 写操作日志
                        optReturn.Message = CurrentApp.GetLanguageInfo("3102N048", optReturn.Message);
                        strLog = string.Format("{0}", string.Format("{0}\t{1}", optReturn.Code, optReturn.Message));
                        CurrentApp.WriteOperationLog(S3102Consts.OPT_EXPORTRECORD.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                        #endregion

                        return;
                    }
                    fileName = optReturn.Data.ToString();
                    if (recordInfo.EncryptFlag == "2")
                    {
                        if (mListSettingInfos != null)
                        {
                            var setting =
                                mListSettingInfos.FirstOrDefault(
                                    s => s.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_DECRYPTFILE);
                            if (setting != null && setting.StringValue == "1")
                            {
                                //解密文件
                                recordOperator.RecordInfo = recordInfo;
                                optReturn = recordOperator.DecryptRecord(fileName);
                                bool isDecrypted = false;
                                if (!optReturn.Result)
                                {
                                    if (optReturn.Code == Service03Consts.DECRYPT_PASSWORD_ERROR)
                                    {
                                        //如果解密密码错误，重新获取一下实际密钥，然后再做一次解密操作
                                        RecordEncryptInfo temp = null;
                                        if (ListEncryptInfo != null)
                                        {
                                            temp = ListEncryptInfo.FirstOrDefault(e => e.ServerAddress == recordInfo.VoiceIP);
                                        }
                                        if (temp == null)
                                        {
                                            CurrentApp.WriteLog("DecryptRecord", string.Format("Fail.\tEncryptInfo is null"));
                                            return;
                                        }
                                        RecordEncryptInfo info = new RecordEncryptInfo();
                                        info.ServerAddress = temp.ServerAddress;
                                        info.UserID = temp.UserID;
                                        info.IsRemember = temp.IsRemember;
                                        info.StartTime = temp.StartTime;
                                        info.EndTime = temp.EndTime;
                                        info.Password = temp.Password;
                                        info.RealPassword = temp.Password;
                                        recordOperator.RecordInfo = recordInfo;
                                        optReturn = recordOperator.GetRealPassword(info);
                                        string password;
                                        if (!optReturn.Result)
                                        {
                                            CurrentApp.WriteLog("GetRealPass", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                            password = info.Password;
                                        }
                                        else
                                        {
                                            password = info.RealPassword;
                                        }
                                        //再次进行解密操作
                                        recordOperator.RecordInfo = recordInfo;
                                        optReturn = recordOperator.DecryptRecord(fileName, password);
                                        if (optReturn.Result)
                                        {
                                            isDecrypted = true;
                                        }
                                    }
                                    if (!isDecrypted)
                                    {
                                        ShowRecordOperatorMessage(optReturn);
                                        CurrentApp.WriteLog("DecryptRecord",
                                            string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));

                                        #region 写操作日志
                                        optReturn.Message = CurrentApp.GetLanguageInfo("3102N048", optReturn.Message);
                                        strLog = string.Format("{0}", string.Format("{0}\t{1}", optReturn.Code, optReturn.Message));
                                        CurrentApp.WriteOperationLog(S3102Consts.OPT_EXPORTRECORD.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                                        #endregion

                                        return;
                                    }
                                }
                            }
                        }
                    }
                    fileName = optReturn.Data.ToString();

                }

                if (recordInfo.RecordVersion == 101)
                {
                    //华为V3录音不需要加密，此处不做操作

                }
                else
                {
                    if (mListSettingInfos != null)
                    {
                        var setting =
                              mListSettingInfos.FirstOrDefault(
                                  s => s.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_ENCRYPTRECORD);
                        if (setting != null && setting.StringValue == "1")
                        {
                            //加密文件
                            setting =
                                mListSettingInfos.FirstOrDefault(
                                    s => s.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_ENCRYPTPASSWORD);
                            if (setting != null && !string.IsNullOrEmpty(setting.StringValue))
                            {
                                string strPassword = S3102App.DecryptString(setting.StringValue);
                                recordOperator.RecordInfo = recordInfo;
                                optReturn = recordOperator.EncryptRecord(fileName, strPassword);
                                if (!optReturn.Result)
                                {
                                    ShowRecordOperatorMessage(optReturn);
                                    CurrentApp.WriteLog("EncryptRecord",
                                        string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));

                                    #region 写操作日志

                                    optReturn.Message = CurrentApp.GetLanguageInfo("3102N048", optReturn.Message);
                                    strLog = string.Format("{0}", string.Format("{0}\t{1}", optReturn.Code, optReturn.Message));
                                    CurrentApp.WriteOperationLog(S3102Consts.OPT_EXPORTRECORD.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                                    #endregion

                                    return;
                                }
                                fileName = optReturn.Data.ToString();
                            }
                        }
                    }
                }

                if (recordInfo.RecordVersion == 101)
                {
                    //华为V3录音转格式，必须转换
                    recordOperator.RecordInfo = recordInfo;
                    optReturn = recordOperator.ConvertWaveFormat(fileName);
                    if (!optReturn.Result)
                    {
                        ShowRecordOperatorMessage(optReturn);
                        CurrentApp.WriteLog("ConvertWaveFormat",
                            string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));

                        #region 写操作日志
                        //这里要给message语言包  因为转换格式不正确在数据库里没有语言包的   2015年10月12日
                        strLog = string.Format("{0}", string.Format("{0}\t{1}", optReturn.Code, optReturn.Message));
                        CurrentApp.WriteOperationLog(S3102Consts.OPT_EXPORTRECORD.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                        #endregion

                        return;
                    }
                    fileName = optReturn.Data.ToString();
                }
                else
                {
                    if (recordInfo.WaveFormat == RecordOperator.VOICEFORMAT_G729_A)
                    {
                        if (mListSettingInfos != null)
                        {
                            var setting =
                                mListSettingInfos.FirstOrDefault(
                                    s => s.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_CONVERTPCM);
                            if (setting != null && setting.StringValue == "1")
                            {
                                //转换格式
                                recordOperator.RecordInfo = recordInfo;
                                optReturn = recordOperator.ConvertWaveFormat(fileName);
                                if (!optReturn.Result)
                                {
                                    ShowRecordOperatorMessage(optReturn);
                                    CurrentApp.WriteLog("ConvertWaveFormat",
                                        string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));

                                    #region 写操作日志
                                    //这里要给message语言包  因为转换格式不正确在数据库里没有语言包的   2015年10月12日
                                    strLog = string.Format("{0}", string.Format("{0}\t{1}", optReturn.Code, optReturn.Message));
                                    CurrentApp.WriteOperationLog(S3102Consts.OPT_EXPORTRECORD.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                                    #endregion

                                    return;
                                }
                                fileName = optReturn.Data.ToString();
                            }
                        }
                    }
                }
               
                CurrentApp.WriteLog("ExportRecord", string.Format("FileName:{0};RelativeName:{1}", fileName, relativeName));
                optReturn = DownloadRecordToLocal(recordInfo, fileName);
                if (!optReturn.Result)
                {
                    SetDownloadItemState(mCurrentItem, Brushes.Red, STATE_FAIL,
                        string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));

                    #region 写操作日志
                    optReturn.Message = CurrentApp.GetLanguageInfo("31021652", optReturn.Message);
                    strLog = string.Format("{0}", string.Format("{0}\t{1}", optReturn.Code, optReturn.Message));
                    CurrentApp.WriteOperationLog(S3102Consts.OPT_EXPORTRECORD.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                    #endregion

                    return;
                }
                if (relativeRecord != null && !string.IsNullOrEmpty(relativeName))
                {
                    //如果存在关联的文件，下载关联的文件
                    optReturn = DownloadRecordToLocal(relativeRecord, relativeName);
                    if (!optReturn.Result)
                    {
                        SetDownloadItemState(mCurrentItem, Brushes.Red, STATE_FAIL,
                            string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));

                        #region 写操作日志
                        optReturn.Message = CurrentApp.GetLanguageInfo("31021652", optReturn.Message);
                        strLog = string.Format("{0}", string.Format("{0}\t{1}", optReturn.Code, optReturn.Message));
                        CurrentApp.WriteOperationLog(S3102Consts.OPT_EXPORTRECORD.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                        #endregion

                        return;
                    }
                }

                #region 写操作日志

                string strLog2 = string.Format("{0} {1}", Utils.FormatOptLogString("COL3102001SerialID"), mCurrentItem.SerialID);
                CurrentApp.WriteOperationLog(S3102Consts.OPT_EXPORTRECORD.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog2);

                #endregion

                SetDownloadItemState(mCurrentItem, Brushes.Green, STATE_SUCCESS, string.Empty);
            }
            catch (Exception ex)
            {
                SetDownloadItemState(mCurrentItem, Brushes.Red, STATE_FAIL, ex.Message);
            }
        }

        private void SetDecryptPassword()
        {
            if (mCurrentItem == null) { return; }
            try
            {
                RecordInfoItem recordItem = mCurrentItem.RecordItem;
                if (recordItem == null) { return; }
                RecordInfo recordInfo = recordItem.RecordInfo;
                if (recordInfo == null) { return; }
                if (recordInfo.EncryptFlag != "2")
                {
                    SetDownloadItemState(mCurrentItem, Brushes.Red, STATE_FAIL, string.Format("EncryptFlag invalid"));
                    return;
                }
                if (ListEncryptInfo == null) { return; }
                string strServerAddress = recordInfo.VoiceIP;
                var encryptInfo = ListEncryptInfo.FirstOrDefault(s => s.UserID == CurrentApp.Session.UserID
                                                                      && s.ServerAddress == strServerAddress);
                if (encryptInfo == null)
                {
                    encryptInfo = new RecordEncryptInfo();
                    encryptInfo.UserID = CurrentApp.Session.UserID;
                    encryptInfo.ServerAddress = strServerAddress;
                    encryptInfo.StartTime = DateTime.Now.ToUniversalTime();
                    encryptInfo.EndTime = DateTime.Now.AddDays(1).ToUniversalTime();
                    encryptInfo.IsRemember = false;
                    ListEncryptInfo.Add(encryptInfo);
                }
                UCPasswordManagement uc = new UCPasswordManagement();
                uc.CurrentApp = CurrentApp;
                uc.PasswordManagerEvent += PasswordManagement_PasswordManagementEvent;
                uc.MainPage = MainPage;
                uc.ListUserSettingInfos = ListUserSettingInfos;
                uc.RecordInfoItem = recordItem;
                uc.ListRecordEncryptInfos = ListEncryptInfo;
                if (MainPage != null)
                {
                    MainPage.OpenPasswordPanel(uc);
                }
            }
            catch (Exception ex)
            {
                SetDownloadItemState(mCurrentItem, Brushes.Red, STATE_FAIL, ex.Message);
            }
        }

        private void GetRealPassword(RecordEncryptInfo info)
        {
            if (mCurrentItem == null) { return; }
            try
            {
                RecordInfoItem recordItem = mCurrentItem.RecordItem;
                if (recordItem == null) { return; }
                RecordInfo recordInfo = recordItem.RecordInfo;
                if (recordInfo == null) { return; }
                if (MainPage != null)
                {
                    MainPage.SetBusy(true, CurrentApp.GetMessageLanguageInfo("042", "Getting real password"));
                }
                bool isSuccess = false;
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    try
                    {
                        OperationReturn optReturn;
                        RecordOperator recordOperator = new RecordOperator(recordInfo);
                        recordOperator.Debug += (cat, msg) => CurrentApp.WriteLog(cat, msg);
                        recordOperator.Session = CurrentApp.Session;
                        recordOperator.ListSftpServers = ListSftpServers;
                        recordOperator.ListDownloadParams = ListDownloadParams;
                        recordOperator.ListEncryptInfo = ListEncryptInfo;
                        recordOperator.Service03Helper = Service03Helper;
                        optReturn = recordOperator.GetRealPassword(info);
                        if (!optReturn.Result)
                        {
                            //SetDownloadItemState(mCurrentItem, Brushes.Red, STATE_FAIL,
                            //    string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            //return;

                            CurrentApp.WriteLog("GetRealPass", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        }
                        isSuccess = true;
                        mListServerAddresses.Add(info.ServerAddress);
                    }
                    catch (Exception ex)
                    {
                        CurrentApp.WriteLog("GetRealPass", string.Format("Fail.\t{0}", ex.Message));
                    }
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    if (MainPage != null)
                    {
                        MainPage.SetBusy(false,string.Empty);
                    }
                    if (!isSuccess)
                    {
                        CurrentApp.WriteLog("GetRealPass", string.Format("Get real password fail."));
                        return;
                    }
                    int index = mListDownloadItems.IndexOf(mCurrentItem);
                    if (index < mListDownloadItems.Count - 1)
                    {
                        mCurrentItem = mListDownloadItems[index + 1];
                        PrepareDownloadRecordItem();
                        return;
                    }
                    PrepareDownloadRecordItem();
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                SetDownloadItemState(mCurrentItem, Brushes.Red, STATE_FAIL, ex.Message);
            }
        }

        private void GetRelativeRecordInfos()
        {
            try
            {
                if (mCurrentItem == null) { return; }
                RecordInfoItem recordItem = mCurrentItem.RecordItem;
                if (recordItem == null) { return; }
                recordItem.ListRelativeInfos.Clear();
                bool isAutoRelative = false;
                if (ListUserSettingInfos != null)
                {
                    var setting =
                        ListUserSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_AUTORELATIVEPLAY);
                    if (setting != null && setting.StringValue == "1")
                    {
                        isAutoRelative = true;
                    }
                }
                if (!isAutoRelative) { return; }
                if (recordItem.MediaType != 1)
                {
                    return;
                }
                var recordInfo = recordItem.RecordInfo;
                if (recordInfo == null) { return; }
                OperationReturn optReturn;
                optReturn = XMLHelper.SeriallizeObject(recordInfo);
                if (!optReturn.Result)
                {
                    CurrentApp.WriteLog("GetRelativeInfos", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.GetRelativeRecordList;
                webRequest.ListData.Add(optReturn.Data.ToString());
                Service31021Client client = new Service31021Client(
                   WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                   WebHelper.CreateEndpointAddress(
                       CurrentApp.Session.AppServerInfo,
                       "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    CurrentApp.WriteLog("GetRelativeInfos", string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    CurrentApp.WriteLog("GetRelativeInfos", string.Format("WSFail.\tListData is null"));
                    return;
                }
                int count = webReturn.ListData.Count;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<RecordInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        CurrentApp.WriteLog("GetRelativeInfos", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    RecordInfo relativeRecord = optReturn.Data as RecordInfo;
                    if (relativeRecord == null)
                    {
                        CurrentApp.WriteLog("GetRelativeInfos", string.Format("WSFail.\tRelativeRecordInfo is null"));
                        return;
                    }
                    recordItem.ListRelativeInfos.Add(relativeRecord);
                    CurrentApp.WriteLog("GetRelativeInfos", string.Format("{0}", relativeRecord.SerialID));
                }
                CurrentApp.WriteLog("GetRelativeInfos", string.Format("End.\t{0}", count));
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("GetRelativeInfos", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private OperationReturn DownloadRecordToLocal(RecordInfo recordInfo, string fileName)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                SettingInfo settingInfo;
                if (recordInfo.MediaType == 0
                    || recordInfo.MediaType == 1)
                {
                    settingInfo =
                        mListSettingInfos.FirstOrDefault(
                            s => s.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_EXPORTVOICE);
                    if (settingInfo != null && settingInfo.StringValue == "0")
                    {
                        //录音不导出
                        CurrentApp.WriteLog("Download",
                            string.Format("No need to export.\t{0}\t{1}", recordInfo.MediaType, recordInfo.SerialID));
                        return optReturn;
                    }
                }
                if (recordInfo.MediaType == 2)
                {
                    settingInfo =
                       mListSettingInfos.FirstOrDefault(
                           s => s.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_EXPORTSCREEN);
                    if (settingInfo != null && settingInfo.StringValue == "0")
                    {
                        CurrentApp.WriteLog("Download",
                           string.Format("No need to export.\t{0}\t{1}", recordInfo.MediaType, recordInfo.SerialID));
                        return optReturn;
                    }
                }
                string savePath = string.Empty;
                settingInfo =
                    mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_SAVEDIR);
                if (settingInfo != null)
                {
                    savePath = settingInfo.StringValue;
                }
                settingInfo =
                    mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_IGNOREPATHFORMAT);
                if (settingInfo != null && settingInfo.StringValue == "0")
                {
                    //使用指定的路径格式
                    string pathFormat = PATH_FORMAT_DEFUALT;
                    settingInfo =
                        mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_PATHFORMAT);
                    if (settingInfo != null && !string.IsNullOrEmpty(settingInfo.StringValue))
                    {
                        pathFormat = settingInfo.StringValue;
                    }
                    pathFormat = GetRecordPath(recordInfo, pathFormat);
                    savePath = Path.Combine(savePath, pathFormat.TrimStart('\\'));
                }
                if (string.IsNullOrEmpty(savePath))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_STRING_EMPTY;
                    optReturn.Message = string.Format("SavePath is empty");
                    return optReturn;
                }
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }
                string strName = recordInfo.SerialID.ToString();
                string strExt = recordInfo.MediaType == 2 ? ".vls" : ".wav";
                string strPath = Path.Combine(savePath, string.Format("{0}{1}", strName, strExt));
                settingInfo =
                   mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTRECORD_REPLACEFILE);
                if (settingInfo != null && settingInfo.StringValue == "0")
                {
                    //如果没有选择替换文件，而文件又已经存在了，则重命名文件（在文件名后加上序号）
                    int i = 0;
                    while (File.Exists(strPath))
                    {
                        i++;
                        strPath = Path.Combine(savePath, string.Format("{0}{1}{2}", strName, i, strExt));
                    }
                }
                DownloadConfig config = new DownloadConfig();
                config.Method = CurrentApp.Session.AppServerInfo.SupportHttps ? 2 : 1;
                config.Host = CurrentApp.Session.AppServerInfo.Address;
                config.Port = CurrentApp.Session.AppServerInfo.Port;
                config.IsAnonymous = true;
                config.RequestPath = string.Format("{0}/{1}", ConstValue.TEMP_DIR_MEDIADATA, fileName);
                config.SavePath = strPath;
                config.IsReplace = true;
                optReturn = DownloadHelper.DownloadFile(config);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                CurrentApp.WriteLog("Download", string.Format("Download to local end.\t{0}", strPath));
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        #endregion


        #region Others

        private bool CheckInput()
        {
            //=====================为下面两个提示消息增加了语言包================================
            //===================================by 汤澈=========================================
            if (string.IsNullOrEmpty(TxtSaveDir.Text))
            {
                ShowException(string.Format(CurrentApp.GetLanguageInfo("3102N022", "Save directory is empty")));
                return false;
            }
            //在这里加个判断,来看是否选中了导出的录音  by 汤澈=======================================
            if (mListDownloadItems.Count == 0)
            {
                ShowException(string.Format(CurrentApp.GetLanguageInfo("3102N028", "No Record")));
                return false;
            }

            //=======================================================================================
            if (string.IsNullOrEmpty(TxtPathFormat.Text))
            {
                ShowException(string.Format(CurrentApp.GetLanguageInfo("3102N023", "Path format is empty")));
                return false;
            }
            return true;
        }

        private void SetConfig()
        {
            try
            {
                mListSettingInfos.Clear();
                SettingInfo settingInfo;
                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_EXPORTRECORD_REMEMBER;
                settingInfo.GroupID = 310203;
                settingInfo.SortID = 0;
                settingInfo.StringValue = CbRemember.IsChecked == true ? "1" : "0";
                settingInfo.DataType = 2;
                mListSettingInfos.Add(settingInfo);

                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_EXPORTRECORD_NOTSHOW;
                settingInfo.GroupID = 310203;
                settingInfo.SortID = 1;
                settingInfo.StringValue = CbNotShow.IsChecked == true ? "1" : "0";
                settingInfo.DataType = 2;
                mListSettingInfos.Add(settingInfo);

                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_EXPORTRECORD_SAVEDIR;
                settingInfo.GroupID = 310203;
                settingInfo.SortID = 2;
                settingInfo.StringValue = TxtSaveDir.Text;
                settingInfo.DataType = 14;
                mListSettingInfos.Add(settingInfo);

                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_EXPORTRECORD_PATHFORMAT;
                settingInfo.GroupID = 310203;
                settingInfo.SortID = 3;
                settingInfo.StringValue = TxtPathFormat.Text;
                settingInfo.DataType = 14;
                mListSettingInfos.Add(settingInfo);

                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_EXPORTRECORD_IGNOREPATHFORMAT;
                settingInfo.GroupID = 310203;
                settingInfo.SortID = 4;
                settingInfo.StringValue = CbIgnorePathFormat.IsChecked == true ? "1" : "0";
                settingInfo.DataType = 2;
                mListSettingInfos.Add(settingInfo);

                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_EXPORTRECORD_ENCRYPTRECORD;
                settingInfo.GroupID = 310203;
                settingInfo.SortID = 5;
                settingInfo.StringValue = CbEncryptRecord.IsChecked == true ? "1" : "0";
                settingInfo.DataType = 2;
                mListSettingInfos.Add(settingInfo);

                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_EXPORTRECORD_ENCRYPTPASSWORD;
                settingInfo.GroupID = 310203;
                settingInfo.SortID = 6;
                settingInfo.StringValue = S3102App.EncryptString(TxtEncryptPassword.Password);
                settingInfo.DataType = 14;
                mListSettingInfos.Add(settingInfo);

                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_EXPORTRECORD_CONVERTPCM;
                settingInfo.GroupID = 310203;
                settingInfo.SortID = 7;
                settingInfo.StringValue = CbConvertPCM.IsChecked == true ? "1" : "0";
                settingInfo.DataType = 2;
                mListSettingInfos.Add(settingInfo);

                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_EXPORTRECORD_DECRYPTFILE;
                settingInfo.GroupID = 310203;
                settingInfo.SortID = 8;
                settingInfo.StringValue = CbDecrypFile.IsChecked == true ? "1" : "0";
                settingInfo.DataType = 2;
                mListSettingInfos.Add(settingInfo);

                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_EXPORTRECORD_GENERATEDB;
                settingInfo.GroupID = 310203;
                settingInfo.SortID = 9;
                settingInfo.StringValue = CbGenerateDB.IsChecked == true ? "1" : "0";
                settingInfo.DataType = 2;
                mListSettingInfos.Add(settingInfo);

                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_EXPORTRECORD_REPLACEFILE;
                settingInfo.GroupID = 310203;
                settingInfo.SortID = 10;
                settingInfo.StringValue = CbReplaceFile.IsChecked == true ? "1" : "0";
                settingInfo.DataType = 2;
                mListSettingInfos.Add(settingInfo);

                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_EXPORTRECORD_EXPORTVOICE;
                settingInfo.GroupID = 310203;
                settingInfo.SortID = 11;
                settingInfo.StringValue = CbExportVoice.IsChecked == true ? "1" : "0";
                settingInfo.DataType = 2;
                mListSettingInfos.Add(settingInfo);

                settingInfo = new SettingInfo();
                settingInfo.UserID = CurrentApp.Session.UserID;
                settingInfo.ParamID = S3102Consts.USER_PARAM_EXPORTRECORD_EXPORTSCREEN;
                settingInfo.GroupID = 310203;
                settingInfo.SortID = 12;
                settingInfo.StringValue = CbExportScreen.IsChecked == true ? "1" : "0";
                settingInfo.DataType = 2;
                mListSettingInfos.Add(settingInfo);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ShowRecordOperatorMessage(OperationReturn optReturn)
        {
            try
            {
                int code = optReturn.Code;
                switch (code)
                {
                    case RecordOperator.RET_DOWNLOADSERVER_NOT_EXIST:
                        SetDownloadItemState(mCurrentItem, Brushes.Red, STATE_FAIL, CurrentApp.GetMessageLanguageInfo("014", "Download server not exist."));
                        break;
                    case RecordOperator.RET_DOWNLOAD_APPSERVER_FAIL:
                        SetDownloadItemState(mCurrentItem, Brushes.Red, STATE_FAIL, CurrentApp.GetMessageLanguageInfo("035", "Download fail"));
                        break;
                    case RecordOperator.RET_GET_REAL_PASSWORD_FAIL:
                        SetDownloadItemState(mCurrentItem, Brushes.Red, STATE_FAIL, CurrentApp.GetMessageLanguageInfo("043", "Get real password fail."));
                        break;
                    case Defines.RET_TIMEOUT:
                        SetDownloadItemState(mCurrentItem, Brushes.Red, STATE_FAIL, CurrentApp.GetLanguageInfo("3102N047", "Receive message timeout"));
                        break;
                    case Service03Consts.DECRYPT_PASSWORD_ERROR:
                        SetDownloadItemState(mCurrentItem, Brushes.Red, STATE_FAIL, CurrentApp.GetMessageLanguageInfo("048", "Decrypt password error"));
                        break;
                    case RecordOperator.RET_NO_RECORDINFO:
                        SetDownloadItemState(mCurrentItem, Brushes.Red, STATE_FAIL, string.Format("RecordInfo is null"));
                        break;
                    default:
                        SetDownloadItemState(mCurrentItem, Brushes.Red, STATE_FAIL,
                            string.Format("Fail.\t{0}\t{1}", code, optReturn.Message));
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SetDownloadItemState(RecordDownloadItem item, Brush foreground, int state, string error)
        {
            //State
            //0     Waiting
            //1     Success
            //2     Fail
            //3     Downloading
            string strState = string.Format("{0}", state);
            switch (state)
            {
                case STATE_WAITING:
                    strState = CurrentApp.GetLanguageInfo("31021650", "Waiting");
                    break;
                case STATE_SUCCESS:
                    strState = CurrentApp.GetLanguageInfo("31021651", "Successful");
                    break;
                case STATE_FAIL:
                    strState = CurrentApp.GetLanguageInfo("31021652", "Fail");
                    break;
                case STATE_DOWNLOADING:
                    strState = CurrentApp.GetLanguageInfo("31021653", "Downloading");
                    break;
            }
            Dispatcher.Invoke(new Action(() =>
            {
                if (foreground != null)
                {
                    item.Foreground = foreground;
                }
                item.State = strState;
                item.Error = error;
            }));
        }

        private string GetRecordPath(RecordInfo recordInfo, string pathFormat)
        {
            string fmt = pathFormat;
            int iYear, iMonth, iDay, iVoiceID, iChannelID;
            DateTime dtStartRecordTime;
            dtStartRecordTime = recordInfo.StartRecordTime;
            iYear = dtStartRecordTime.Year;
            iMonth = dtStartRecordTime.Month;
            iDay = dtStartRecordTime.Day;
            iVoiceID = recordInfo.VoiceID;
            iChannelID = recordInfo.ChannelID;
            Match reVID = Regex.Match(fmt, @"(\*)(\d*)(VID)");
            Match reCID = Regex.Match(fmt, @"(\*)(\d*)(CID)");
            if (reVID.Success)
            {
                if (reVID.Groups[2].Value == string.Empty)
                {
                    fmt = Regex.Replace(fmt, @"(\*)(\d*)(VID)", iVoiceID.ToString("D04"));
                }
                else
                {
                    fmt = Regex.Replace(fmt, @"(\*)(\d*)(VID)", iVoiceID.ToString("D" + reVID.Groups[2]));
                }
            }
            if (reCID.Success)
            {
                if (reCID.Groups[2].Value == string.Empty)
                {
                    fmt = Regex.Replace(fmt, @"(\*)(\d*)(CID)", iChannelID.ToString("D05"));
                }
                else
                {
                    fmt = Regex.Replace(fmt, @"(\*)(\d*)(CID)", iChannelID.ToString("D" + reCID.Groups[2]));
                }
            }
            fmt = Regex.Replace(fmt, @"(\*YYYY)", iYear.ToString("D04"));
            fmt = Regex.Replace(fmt, @"(\*YY)", (iYear % 100).ToString("D02"));
            fmt = Regex.Replace(fmt, @"(\*MM)", iMonth.ToString("D02"));
            fmt = Regex.Replace(fmt, @"(\*DD)", iDay.ToString("D02"));
            return fmt;
        }

        #endregion


        #region ChangeLanguages

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                BtnApply.Content = CurrentApp.GetLanguageInfo("31020", "Confirm");
                BtnClose.Content = CurrentApp.GetLanguageInfo("31021", "Close");
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.Title = CurrentApp.GetLanguageInfo("31021900", "Export data option");
                }
                TabBasic.Header = CurrentApp.GetLanguageInfo("31021901", "Basic settings");
                TabOther.Header = CurrentApp.GetLanguageInfo("31021902", "Other settings");
                CbRemember.Content = CurrentApp.GetLanguageInfo("31021600", "Remember current settings");
                CbNotShow.Content = CurrentApp.GetLanguageInfo("31021601", "Not show next time");
                LbSaveDir.Text = CurrentApp.GetLanguageInfo("31021602", "Save directory");
                LbPathFormat.Text = CurrentApp.GetLanguageInfo("31021603", "Path format");
                LbEncryptPassword.Text = CurrentApp.GetLanguageInfo("31021612", "Encrypt password");
                CbIgnorePathFormat.Content = CurrentApp.GetLanguageInfo("31021610", "Ignore path format");
                CbEncryptRecord.Content = CurrentApp.GetLanguageInfo("31021611", "Encrypt record while export");
                CbConvertPCM.Content = CurrentApp.GetLanguageInfo("31021604", "Convert to PCM");
                CbDecrypFile.Content = CurrentApp.GetLanguageInfo("31021605", "Decrypt file");
                CbGenerateDB.Content = CurrentApp.GetLanguageInfo("31021606", "Generate database");
                CbReplaceFile.Content = CurrentApp.GetLanguageInfo("31021607", "Replace file");
                CbExportVoice.Content = CurrentApp.GetLanguageInfo("31021608", "Export voice");
                CbExportScreen.Content = CurrentApp.GetLanguageInfo("31021609", "Export screen");

                CreateDownloadColumns();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion

    }
}
