//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    4091c46b-40c1-4a00-96cc-4eb54e71e0a6
//        CLR Version:              4.0.30319.18444
//        Name:                     UCPlayBox
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102
//        File Name:                UCPlayBox
//
//        created by Charley at 2014/11/14 9:43:31
//        http://www.voicecyber.com
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using UMPS3102.Codes;
using UMPS3102.Models;
using UMPS3102.Wcf31021;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31021;
using VoiceCyber.UMP.CommonService03;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.UMP.Controls.Players;

namespace UMPS3102
{
    /// <summary>
    /// UCPlayBox.xaml 的交互逻辑
    /// 对VoicePlayer控件的一次封装
    /// 公用播放控件，音频播放，声道控制，音量控制，速度控制，循环控制
    /// </summary>
    public partial class UCPlayBox
    {

        #region Members

        public QMMainView MainPage;
        public RecordInfoItem RecordInfoItem;//录音信息
        public RecordPlayItem RecordPlayItem;
        public bool IsAutoPlay;
        /// <summary>
        /// 0       不循环
        /// 1       单循环
        /// 2       列表循环
        /// </summary>
        public int CircleMode;
        public double StartPosition;
        public double StopPostion;
        public bool ShowKeyword;        //是否显示关键词

        public List<SftpServerInfo> ListSftpServers;
        public List<DownloadParamInfo> ListDownloadParams;
        public List<SettingInfo> ListUserSettingInfos;
        public List<RecordEncryptInfo> ListEncryptInfo;
        public Service03Helper Service03Helper;

        private TimeSpan mCurrentTime;
        private BackgroundWorker mWorker;
        private bool mIsInited;
        private List<KeywordResultInfo> mListKeywordInfos;
        private ObservableCollection<KeywordResultItem> mListKeywordItems;

        #endregion


        public UCPlayBox()
        {
            InitializeComponent();

            Loaded += UCPlayBox_Loaded;
            Unloaded += UCPlayBox_Unloaded;
            VoicePlayer.PlayerEvent += VoicePlayer_PlayerEvent;
            BorderKeywords.SizeChanged += (s, e) => SetKeywordItemOffset();

            mListKeywordInfos = new List<KeywordResultInfo>();
            mListKeywordItems = new ObservableCollection<KeywordResultItem>();

            IsAutoPlay = false;
            CircleMode = 0;

            //IsCircleModeStop = false;
        }

        void UCPlayBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (!mIsInited)
            {
                Init();
                mIsInited = true;
            }

            //if (RecordPlayItem != null)
            //{
            //    RecordInfoItem = RecordPlayItem.RecordInfoItem;
            //}
            //if (IsAutoPlay)
            //{
            //    Play();
            //}
        }

        void UCPlayBox_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (VoicePlayer != null)
                {
                    VoicePlayer.Close();
                }
            }
            catch { }
        }


        #region Init and Load

        private void Init()
        {
            try
            {
                ListBoxKeywords.ItemsSource = mListKeywordItems;
                CommandBindings.Add(new CommandBinding(KeywordClickCommand, KeywordClickCommand_Executed,
                    (s, e) => e.CanExecute = true));
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadKeywordInfos();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    CreateKeywordItems();
                    SetKeywordItemOffset();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadKeywordInfos()
        {
            try
            {
                mListKeywordInfos.Clear();
                if (!ShowKeyword) { return; }
                if (RecordPlayItem != null)
                {
                    RecordInfoItem = RecordPlayItem.RecordInfoItem;
                }
                if (RecordInfoItem == null) { return; }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.GetKeywordResultList;
                webRequest.ListData.Add(RecordInfoItem.SerialID.ToString());
                Service31021Client client = new Service31021Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null."));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<KeywordResultInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    KeywordResultInfo info = optReturn.Data as KeywordResultInfo;
                    if (info == null)
                    {
                        ShowException(string.Format("Fail.\tKeywordResultInfo is null."));
                        return;
                    }
                    mListKeywordInfos.Add(info);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Create

        private void CreateKeywordItems()
        {
            try
            {
                mListKeywordItems.Clear();
                for (int i = 0; i < mListKeywordInfos.Count; i++)
                {
                    var info = mListKeywordInfos[i];
                    KeywordResultItem item = new KeywordResultItem();
                    item.Info = info;
                    item.Name = info.KeywordName;
                    item.Description = string.Format("{0} ({1}) ", info.KeywordName, info.KeywordContent);
                    mListKeywordItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Event Handlers

        void VoicePlayer_PlayerEvent(object sender, RoutedPropertyChangedEventArgs<UMPEventArgs> e)
        {
            try
            {
                OnPlayerEvent(sender, e);
                if (e.NewValue == null) { return; }
                int code = e.NewValue.Code;
                var param = e.NewValue.Data;

                //For Test
                //CurrentApp.WriteLog("Player", string.Format("{0}\t{1}", code, param));

                TimeSpan ts;
                switch (code)
                {
                    case Defines.EVT_PAGE_LOADED:
                        if (RecordPlayItem != null)
                        {
                            RecordInfoItem = RecordPlayItem.RecordInfoItem;
                        }
                        if (IsAutoPlay)
                        {
                            Play(true);
                        }
                        break;
                    case MediaPlayerEventCodes.PLAYING:
                        ts = TimeSpan.Parse(param.ToString());
                        mCurrentTime = ts;
                        if (ts.TotalMilliseconds < StartPosition - 500)
                        {
                            if (CircleMode != 0
                                && StartPosition >= 0
                                && StopPostion > StartPosition)//当选择循环的时候,那么就会设置播放的开始位置
                            {
                                VoicePlayer.SetPosition(TimeSpan.FromMilliseconds(StartPosition + 500));
                            }
                        }
                        if (ts.TotalMilliseconds > StopPostion - 500)
                        {
                            if (CircleMode != 0)
                            {
                                OnPlayStopped();
                            }
                            if (CircleMode == 1
                                && StartPosition >= 0
                                && StopPostion > StartPosition)
                            {
                                VoicePlayer.SetPosition(TimeSpan.FromMilliseconds(StartPosition));
                            }
                        }
                        break;
                    case MediaPlayerEventCodes.PLAYBACKSTOPPED:
                        if (CircleMode != 0)
                        {
                            OnPlayStopped();
                        }
                        break;
                    case MediaPlayerEventCodes.BTN_STOP:
                        OnPlayStopped();
                        break;
                    case Defines.EVT_EXCEPTION:
                        CurrentApp.WriteLog("MediaPlayer", string.Format("MediaPlayer exception.\t{0}", param));
                        break;
                    case Defines.EVT_COMMON:
                        //CurrentApp.WriteLog("MediaPlayer", string.Format("MediaPlayer debug.\t{0}", param));
                        break;
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
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Public Functions
        /// <summary>
        /// 获得当前播放的位置
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetCurrentTime()
        {
            try
            {
                if (RecordInfoItem == null)
                {
                    return TimeSpan.FromSeconds(0);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
            return mCurrentTime;
        }
        /// <summary>
        /// 开始播放
        /// </summary>
        public void Play(bool isDownload)
        {
            try
            {
                if (RecordInfoItem != null)
                {
                    //录屏窗口控制
                    SetScreenPlayer();

                    if (isDownload)
                    {

                        #region 写操作日志

                        string strLog = string.Format("{0} {1}", Utils.FormatOptLogString("COL3102001RecordReference"), RecordInfoItem.RecordReference);
                        CurrentApp.WriteOperationLog(S3102Consts.OPT_PLAYRECORD.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);

                        #endregion


                        if (RecordInfoItem.EncryptFlag != "0")
                        {
                            SetDecryptPassword();
                        }
                        else
                        {
                            PlayRecord();
                        }
                    }
                    else
                    {
                        //无需下载，直接播放
                        VoicePlayer.Play();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            try
            {
                if (VoicePlayer != null)
                {
                    VoicePlayer.Close();
                }
            }
            catch { }
        }
        #endregion


        #region Operations

        private void SetScreenPlayer()
        {
            //录屏窗口控制
            try
            {
                SettingInfo settingInfo;

                if (RecordInfoItem == null) { return; }

                //是否勾选窗口置顶
                bool top = true;
                if (ListUserSettingInfos != null)
                {
                    settingInfo =
                     ListUserSettingInfos.FirstOrDefault(
                         s => s.ParamID == S3102Consts.USER_PARAM_PLAYSCREEN_TOPMOST);
                    if (settingInfo != null && settingInfo.StringValue == "0")
                    {
                        top = false;
                    }
                }
                VoicePlayer.ScreenTopMost = top;

                //缩放大小
                int intScale = 0;
                if (ListUserSettingInfos != null)
                {
                    settingInfo =
                     ListUserSettingInfos.FirstOrDefault(
                         s => s.ParamID == S3102Consts.USER_PARAM_PLAYSCREEN_SCALE);
                    if (settingInfo != null && int.TryParse(settingInfo.StringValue, out intScale))
                    {

                    }
                }
                VoicePlayer.ScreenScale = intScale;

                //默认时长
                VoicePlayer.Duration = RecordInfoItem.Duration;
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("SetScreenPlayer", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void SetDecryptPassword()
        {
            try
            {
                if (RecordInfoItem == null) { return; }
                if (RecordInfoItem.EncryptFlag != "2") { return; }
                if (ListEncryptInfo == null) { return; }
                string strServerAddress = RecordInfoItem.VoiceIP;
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
                uc.RecordInfoItem = RecordInfoItem;
                uc.ListRecordEncryptInfos = ListEncryptInfo;
                if (MainPage != null)
                {
                    MainPage.OpenPasswordPanel(uc);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void GetRealPassword(RecordEncryptInfo info)
        {
            try
            {
                if (RecordInfoItem == null) { return; }
                RecordInfo recordInfo = RecordInfoItem.RecordInfo;
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
                            //获取实际密钥失败，不再弹出错误提示框，写一条错误日志，实际解密的时候使用输入密钥作为实际密钥

                            //ShowRecordOperatorMessage(optReturn);
                            CurrentApp.WriteLog("GetRealPass",
                                string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            //return;
                        }
                        isSuccess = true;
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
                        MainPage.SetBusy(false);
                    }
                    if (!isSuccess)
                    {
                        CurrentApp.WriteLog("GetRealPass", string.Format("Get real password fail."));
                        return;
                    }
                    PlayRecord();
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void PlayRecord()
        {
            try
            {
                if (RecordInfoItem == null)
                {
                    return;
                }
                RecordInfo recordInfo = RecordInfoItem.RecordInfo;
                if (recordInfo == null) { return; }
                string fileName = string.Empty;
                VoicePlayer.IsEnabled = false;

                List<string> listRelativeNames = new List<string>();

                //艺赛旗录屏无需下载，也不用解密转换等操作，直接跳过以下操作
                if (recordInfo.MediaType == 3)
                {
                    fileName = recordInfo.IsaRefID;
                    VoicePlayer.IsEnabled = true;
                    PlayRecord(fileName, listRelativeNames);
                    return;
                }

                if (MainPage != null)
                {
                    MainPage.SetBusy(true, string.Format("{0}",
                        CurrentApp.GetMessageLanguageInfo("015", "Downloading record file...")));
                }
                bool isOptSuccess = true;
                if (MainPage != null)
                {
                    MainPage.IsBusy = true;
                }
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    try
                    {
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
                            return;
                        }
                        fileName = optReturn.Data.ToString();

                        var relativeRecords = RecordInfoItem.ListRelativeInfos;
                        if (relativeRecords.Count > 0)
                        {
                            //如果有关联的录屏文件
                            for (int i = 0; i < relativeRecords.Count; i++)
                            {
                                var relativeRecord = relativeRecords[i];
                                if (relativeRecord.MediaType == 3)
                                {
                                    //艺赛旗录屏无需下载，也不用解密转换等操作，直接跳过以下操作
                                    listRelativeNames.Add(relativeRecord.IsaRefID);
                                }
                                else
                                {
                                    //下载录屏文件到AppServer
                                    recordOperator.RecordInfo = relativeRecord;
                                    optReturn = recordOperator.DownloadFileToAppServer();
                                    if (!optReturn.Result)
                                    {
                                        CurrentApp.WriteLog("DownloadAppServer",
                                            string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                    }
                                    else
                                    {
                                        listRelativeNames.Add(optReturn.Data.ToString());
                                    }
                                }
                            }
                        }


                        if (recordInfo.RecordVersion == 101)
                        {
                            //华为V3录音不需要解密，此处不做操作

                        }
                        else
                        {
                            //原始解密
                            recordOperator.RecordInfo = recordInfo;
                            optReturn = recordOperator.OriginalDecryptRecord(fileName);
                            if (!optReturn.Result)
                            {
                                ShowRecordOperatorMessage(optReturn);
                                CurrentApp.WriteLog("OriginalDecrypt",
                                    string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                return;
                            }
                            fileName = optReturn.Data.ToString();

                            //解密文件
                            recordOperator.RecordInfo = recordInfo;
                            optReturn = recordOperator.DecryptRecord(fileName);
                            if (!optReturn.Result)
                            {
                                ShowRecordOperatorMessage(optReturn);
                                CurrentApp.WriteLog("DecryptRecord",
                                    string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                if (optReturn.Code == Service03Consts.DECRYPT_PASSWORD_ERROR)
                                {
                                    isOptSuccess = false;
                                }
                                return;
                            }
                            fileName = optReturn.Data.ToString();
                        }

                        //转换格式
                        recordOperator.RecordInfo = recordInfo;
                        optReturn = recordOperator.ConvertWaveFormat(fileName);
                        if (!optReturn.Result)
                        {
                            ShowRecordOperatorMessage(optReturn);
                            CurrentApp.WriteLog("ConvertWaveFormat",
                                string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        fileName = optReturn.Data.ToString();
                    }
                    catch (Exception ex)
                    {
                        CurrentApp.WriteLog("PlayRecord", string.Format("Fail.\t{0}", ex.Message));
                    }
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    if (MainPage != null)
                    {
                        MainPage.SetBusy(false);
                    }
                    if (MainPage != null)
                    {
                        MainPage.IsBusy = false;
                    }
                    if (!isOptSuccess) { return; }
                    VoicePlayer.IsEnabled = true;
                    PlayRecord(fileName, listRelativeNames);
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void PlayRecord(string fileName, List<string> listRelativeNames)
        {
            try
            {
                if (RecordInfoItem == null) { return; }
                RecordInfo recordInfo = RecordInfoItem.RecordInfo;
                if (recordInfo == null) { return; }
                int mediaType = recordInfo.MediaType;


                #region 自定义参数设置

                bool noPlayScreen = false;
                if (ListUserSettingInfos != null)
                {
                    var setting =
                        ListUserSettingInfos.FirstOrDefault(
                            c => c.ParamID == S3102Consts.USER_PARAM_PLAYSCREEN_NOPLAY);
                    if (setting != null && setting.StringValue == "1")
                    {
                        noPlayScreen = true;
                    }
                }
                bool autoRelative = true;
                if (ListUserSettingInfos != null)
                {
                    var setting =
                        ListUserSettingInfos.FirstOrDefault(
                            c => c.ParamID == S3102Consts.USER_PARAM_AUTORELATIVEPLAY);
                    if (setting != null && setting.StringValue == "0")
                    {
                        autoRelative = false;
                    }
                }

                #endregion


                if (string.IsNullOrEmpty(fileName))
                {
                    CurrentApp.WriteLog("PlayRecord", string.Format("FileName is empty"));
                    return;
                }
                if (listRelativeNames.Count <= 0)
                {
                    CurrentApp.WriteLog("PlayRecord", string.Format("RelativeName is empty"));
                    autoRelative = false;
                }
                VoicePlayer.Session = CurrentApp.Session;
                if (mediaType == 3)
                {

                    #region 艺赛旗录屏播放

                    //艺赛旗录屏播放，需要设置服务器地址和艺赛旗流水号
                    if (noPlayScreen) { return; }
                    CurrentApp.WriteLog("PlayRecord", string.Format("VedioPath:{0}(IsaVedio)", fileName));
                    VoicePlayer.MediaType = 2;
                    VoicePlayer.IsIsaScreen = true;
                    VoicePlayer.IsaServer = recordInfo.VoiceIP;
                    VoicePlayer.IsaRefID = fileName;
                    VoicePlayer.Play();

                    #endregion

                    return;
                }
                string audioUrl = string.Format("{0}://{1}:{2}/{3}/{4}",
                    CurrentApp.Session.AppServerInfo.SupportHttps ? "https" : "http",
                    CurrentApp.Session.AppServerInfo.Address,
                    CurrentApp.Session.AppServerInfo.Port,
                    ConstValue.TEMP_DIR_MEDIADATA,
                    fileName);
                string videoPath;
                string relativeName = string.Empty;
                switch (mediaType)
                {
                    case 0:
                        if (noPlayScreen)
                        {
                            VoicePlayer.MediaType = 1;
                            VoicePlayer.AudioUrl = audioUrl;
                            CurrentApp.WriteLog("PlayRecord", string.Format("AudioUrl:{0}", audioUrl));
                            VoicePlayer.Play();
                        }
                        else
                        {

                            #region VCLog 录屏播放

                            VoicePlayer.ListVideoUrls.Clear();
                            VoicePlayer.MediaType = 3;
                            VoicePlayer.AudioUrl = audioUrl;
                            CurrentApp.WriteLog("PlayRecord", string.Format("AudioUrl:{0}", audioUrl));
                            videoPath = string.Empty;
                            RecordOperator recordOperator = new RecordOperator();
                            recordOperator.Debug += (cat, msg) => CurrentApp.WriteLog(cat, msg);
                            recordOperator.Session = CurrentApp.Session;
                            OperationReturn optReturn = recordOperator.DownloadFileToLocal(fileName);
                            if (!optReturn.Result)
                            {
                                ShowRecordOperatorMessage(optReturn);
                                CurrentApp.WriteLog("DownloadToLocal",
                                    string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            }
                            else
                            {
                                videoPath = optReturn.Data.ToString();
                            }
                            VoicePlayer.IsIsaScreen = false;
                            VoicePlayer.ListVideoUrls.Add(videoPath);
                            CurrentApp.WriteLog("PlayRecord", string.Format("VideoPath:{0}", videoPath));
                            VoicePlayer.Play();

                            #endregion

                        }
                        break;
                    case 1:
                        if (noPlayScreen
                            || !autoRelative)
                        {
                            VoicePlayer.MediaType = 1;
                            VoicePlayer.AudioUrl = audioUrl;
                            CurrentApp.WriteLog("PlayRecord", string.Format("AudioUrl:{0}", audioUrl));
                            VoicePlayer.Play();
                        }
                        else
                        {
                            VoicePlayer.ListVideoUrls.Clear();
                            VoicePlayer.MediaType = 3;
                            VoicePlayer.AudioUrl = audioUrl;
                            CurrentApp.WriteLog("PlayRecord", string.Format("AudioUrl:{0}", audioUrl));
                            videoPath = string.Empty;

                            var relativeRecords = RecordInfoItem.ListRelativeInfos;
                            for (int i = 0; i < relativeRecords.Count; i++)
                            {
                                var relativeRecord = relativeRecords[i];

                                if (relativeRecord.MediaType == 3)
                                {

                                    #region 艺赛旗录屏播放

                                    if (listRelativeNames.Count > i)
                                    {
                                        relativeName = listRelativeNames[i];
                                    }
                                    CurrentApp.WriteLog("PlayRecord",
                                        string.Format("VedioPath:{0}(IsaVedio)", relativeName));
                                    VoicePlayer.IsIsaScreen = true;
                                    VoicePlayer.IsaServer = relativeRecord.VoiceIP;
                                    VoicePlayer.IsaRefID = relativeName;

                                    #endregion

                                }
                                else
                                {

                                    #region VCLog 录屏播发

                                    if (listRelativeNames.Count > i)
                                    {
                                        relativeName = listRelativeNames[i];
                                    }
                                    if (!string.IsNullOrEmpty(relativeName))
                                    {
                                        RecordOperator recordOperator = new RecordOperator();
                                        recordOperator.Debug += (cat, msg) => CurrentApp.WriteLog(cat, msg);
                                        recordOperator.Session = CurrentApp.Session;
                                        OperationReturn optReturn = recordOperator.DownloadFileToLocal(relativeName);
                                        if (!optReturn.Result)
                                        {
                                            ShowRecordOperatorMessage(optReturn);
                                            CurrentApp.WriteLog("DownloadToLocal",
                                                string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                        }
                                        else
                                        {
                                            videoPath = optReturn.Data.ToString();
                                        }
                                        VoicePlayer.IsIsaScreen = false;
                                        VoicePlayer.ListVideoUrls.Add(videoPath);
                                        CurrentApp.WriteLog("PlayRecord", string.Format("VideoPath:{0}", videoPath));

                                    }

                                    #endregion

                                }
                            }
                            VoicePlayer.Play();
                        }
                        break;
                    case 2:
                        if (!noPlayScreen)
                        {

                            #region VCLog 录屏播放

                            VoicePlayer.MediaType = 2;
                            videoPath = string.Empty;
                            RecordOperator recordOperator = new RecordOperator();
                            recordOperator.Debug += (cat, msg) => CurrentApp.WriteLog(cat, msg);
                            recordOperator.Session = CurrentApp.Session;
                            OperationReturn optReturn = recordOperator.DownloadFileToLocal(fileName);
                            if (!optReturn.Result)
                            {
                                ShowRecordOperatorMessage(optReturn);
                                CurrentApp.WriteLog("DownloadToLocal",
                                    string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            }
                            else
                            {
                                videoPath = optReturn.Data.ToString();
                            }
                            VoicePlayer.IsIsaScreen = false;
                            VoicePlayer.ListVideoUrls.Add(videoPath);
                            CurrentApp.WriteLog("PlayRecord", string.Format("VideoPath:{0}", videoPath));
                            VoicePlayer.Play();

                            #endregion

                        }
                        break;
                    default:
                        CurrentApp.WriteLog("PlayRecord", string.Format("MediaType invalid.\t{0}", mediaType));
                        break;
                }
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("PlayRecord", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void GetRelativeRecordInfos()
        {
            try
            {
                if (RecordInfoItem == null) { return; }
                RecordInfoItem.ListRelativeInfos.Clear();
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
                if (RecordInfoItem.MediaType != 1)
                {
                    return;
                }
                var recordInfo = RecordInfoItem.RecordInfo;
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
                    RecordInfoItem.ListRelativeInfos.Add(relativeRecord);
                    CurrentApp.WriteLog("GetRelativeInfos", string.Format("{0}", relativeRecord.SerialID));
                }
                CurrentApp.WriteLog("GetRelativeInfos", string.Format("End.\t{0}", count));
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("GetRelativeInfos", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void SetKeywordItemOffset()
        {
            try
            {
                if (RecordInfoItem == null) { return; }
                var recordInfo = RecordInfoItem.RecordInfo;
                if (recordInfo == null) { return; }
                int duration = recordInfo.Duration;
                int totalDuration = duration * 1000;
                if (totalDuration <= 0) { return; }
                var totalWidth = BorderKeywords.ActualWidth;
                if (totalWidth <= 0) { return; }
                for (int i = 0; i < mListKeywordItems.Count; i++)
                {
                    var item = mListKeywordItems[i];

                    var info = item.Info;
                    if (info == null) { continue; }

                    int offset = info.Offset;
                    var left = offset * 1.0 / (totalDuration * 1.0) * totalWidth;
                    item.CanvasLeft = left;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Others

        private void ShowRecordOperatorMessage(OperationReturn optReturn)
        {
            try
            {
                int code = optReturn.Code;
                switch (code)
                {
                    case RecordOperator.RET_DOWNLOADSERVER_NOT_EXIST:
                        ShowException(CurrentApp.GetMessageLanguageInfo("014", "Download server not exist."));
                        break;
                    case RecordOperator.RET_DOWNLOAD_APPSERVER_FAIL:
                        ShowException(CurrentApp.GetMessageLanguageInfo("035", "Download fail"));
                        break;
                    case RecordOperator.RET_GET_REAL_PASSWORD_FAIL:
                        ShowException(CurrentApp.GetMessageLanguageInfo("043", "Get real password fail."));
                        break;
                    case Service03Consts.DECRYPT_PASSWORD_ERROR:
                        ShowException(CurrentApp.GetMessageLanguageInfo("048", "Decrypt password error"));
                        break;
                    case Defines.RET_TIMEOUT:
                        ShowException(CurrentApp.GetLanguageInfo("3102N047", "Receive message timeout"));
                        break;
                    case RecordOperator.RET_NO_RECORDINFO:
                        ShowException(string.Format("RecordInfo is null"));
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region PlayStopped
        /// <summary>
        /// 播放结束事件
        /// </summary>
        public event Action PlayStopped;

        private void OnPlayStopped()
        {
            if (PlayStopped != null)
            {
                PlayStopped();
            }
        }

        #endregion


        #region PlayerEvent
        /// <summary>
        /// 播放器事件
        /// </summary>
        public event RoutedPropertyChangedEventHandler<UMPEventArgs> PlayerEvent;

        private void OnPlayerEvent(object sender, RoutedPropertyChangedEventArgs<UMPEventArgs> e)
        {
            if (PlayerEvent != null)
            {
                PlayerEvent(sender, e);
            }
        }

        #endregion


        #region KeywordClickCommand

        private static RoutedUICommand mKeywordClickCommand = new RoutedUICommand();

        public static RoutedUICommand KeywordClickCommand
        {
            get { return mKeywordClickCommand; }
        }

        private void KeywordClickCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var item = e.Parameter as KeywordResultItem;
                if (item == null) { return; }
                var info = item.Info;
                if (info == null) { return; }
                int offset = info.Offset;
                if (offset >= 0)
                {
                    VoicePlayer.SetPosition(TimeSpan.FromMilliseconds(offset));
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
