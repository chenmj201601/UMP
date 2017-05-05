using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using UMPS3106.Models;
using UMPS3106.Wcf31021;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31031;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.UMP.Controls.Players;

namespace UMPS3106
{
    /// <summary>
    /// UCPlayBox.xaml 的交互逻辑
    /// </summary>
    public partial class UCPlayBox 
    {
        /// <summary>
        /// 播放结束事件
        /// </summary>
        public event Action PlayStopped;
        public event RoutedPropertyChangedEventHandler<UMPEventArgs> PlayerEvent;

        public TutorialRepertoryMainView ParentPage;
        public string PlayUrl;

        public RecordInfoItem RecordInfoItem;//录音信息
        public List<SftpServerInfo> ListSftpServers;
        public List<DownloadParamInfo> ListDownloadParams;
        public List<SettingInfo> ListUserSettingInfos;
        public List<RecordEncryptInfo> ListEncryptInfo;
        public Service03Helper Service03Helper;
        private BackgroundWorker mWorker;
        
        private TimeSpan mCurrentTime;
        public bool IsAutoPlay;
        public double StartPosition;
        public double StopPostion;
        /// <summary>
        /// 0       不循环
        /// 1       单循环
        /// 2       列表循环
        /// </summary>
        public int CircleMode;


        public UCPlayBox()
        {
            InitializeComponent();
            ListSftpServers = S3106App.mListSftpServers;
            Service03Helper = S3106App.mService03Helper;
            ListEncryptInfo = S3106App.mListRecordEncryptInfos;
            ListDownloadParams = S3106App.mListDownloadParams;
            VoicePlayer.PlayerEvent += VoicePlayer_PlayerEvent;
            Loaded += UCPlayBox_Loaded;
            Unloaded += UCPlayBox_Unloaded;

            IsAutoPlay = false;
            CircleMode = 0;
        }

        private void VoicePlayer_PlayerEvent(object sender, RoutedPropertyChangedEventArgs<VoiceCyber.UMP.Controls.UMPEventArgs> e)
        {
            try
            {
                OnPlayerEvent(sender, e);
                if (e.NewValue == null) { return; }
                int code = e.NewValue.Code;
                var param = e.NewValue.Data;

                TimeSpan ts;
                switch (code)
                {
                    case Defines.EVT_PAGE_LOADED:
                        if (RecordInfoItem == null)
                        {
                            return;
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
                            if (CircleMode != 0)//当选择循环的时候,那么就会设置播放的开始位置
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
                            if (CircleMode == 1)
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


        private void UCPlayBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(PlayUrl))
            {
                VoicePlayer.MediaType = 1;
                VoicePlayer.AudioUrl = PlayUrl;
                VoicePlayer.Play();
            }
        }

        private void UCPlayBox_Unloaded(object sender, RoutedEventArgs e)
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

        public void Stop()
        {
            try
            {
                VoicePlayer.Close();
            }
            catch { }
        }

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
                         s => s.ParamID == S3103Consts.USER_PARAM_PLAYSCREEN_TOPMOST);
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
                         s => s.ParamID == S3103Consts.USER_PARAM_PLAYSCREEN_SCALE);
                    if (settingInfo != null && int.TryParse(settingInfo.StringValue, out intScale))
                    {

                    }
                }
                VoicePlayer.ScreenScale = intScale;
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
                uc.PasswordManagerEvent += PasswordManagement_PasswordManagementEvent;
                uc.CurrentApp = CurrentApp;
                uc.ParentPage = ParentPage;
                uc.ListUserSettingInfos = ListUserSettingInfos;
                uc.RecordInfoItem = RecordInfoItem;
                uc.ListRecordEncryptInfos = ListEncryptInfo;
                if (ParentPage != null)
                {
                    ParentPage.OpenPasswordPanel(uc);
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
                if (ParentPage != null)
                {
                    ParentPage.SetBusy(true,string.Empty);
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
                            ShowRecordOperatorMessage(optReturn);
                            CurrentApp.WriteLog("GetRealPass",
                                string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
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
                    if (ParentPage != null)
                    {
                        ParentPage.SetBusy(false,string.Empty);
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
                if (RecordInfoItem == null) { return; }
                RecordInfo recordInfo = RecordInfoItem.RecordInfo;
                if (recordInfo == null) { return; }
                if (ParentPage != null)
                {
                    ParentPage.SetBusy(true,string.Empty);
                }
                string fileName = string.Empty;
                string relativeName = string.Empty;
                VoicePlayer.IsEnabled = false;
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
                        var relativeRecord = RecordInfoItem.ListRelativeInfos.FirstOrDefault();
                        if (relativeRecord != null)
                        {
                            //如果有关联的录屏文件，下载录屏文件到AppServer
                            recordOperator.RecordInfo = relativeRecord;
                            optReturn = recordOperator.DownloadFileToAppServer();
                            if (!optReturn.Result)
                            {
                                CurrentApp.WriteLog("DownloadAppServer",
                                    string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            }
                            else
                            {
                                relativeName = optReturn.Data.ToString();
                            }
                        }
                        recordOperator.RecordInfo = recordInfo;
                        //原始解密
                        optReturn = recordOperator.OriginalDecryptRecord(fileName);
                        if (!optReturn.Result)
                        {
                            ShowRecordOperatorMessage(optReturn);
                            CurrentApp.WriteLog("OriginalDecrypt",
                                string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        fileName = optReturn.Data.ToString();
                        recordOperator.RecordInfo = recordInfo;
                        //解密文件
                        optReturn = recordOperator.DecryptRecord(fileName);
                        if (!optReturn.Result)
                        {
                            ShowRecordOperatorMessage(optReturn);
                            CurrentApp.WriteLog("DecryptRecord",
                                string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        fileName = optReturn.Data.ToString();
                        recordOperator.RecordInfo = recordInfo;
                        //转换格式
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
                    VoicePlayer.IsEnabled = true;
                    if (ParentPage != null)
                    {
                        ParentPage.SetBusy(false,string.Empty);
                    }
                    PlayRecord(fileName, relativeName);
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void PlayRecord(string fileName, string relativeName)
        {
            try
            {
                if (RecordInfoItem == null) { return; }
                int mediaType = RecordInfoItem.MediaType;
                bool noPlayScreen = false;
                if (ListUserSettingInfos != null)
                {
                    var setting =
                        ListUserSettingInfos.FirstOrDefault(
                            c => c.ParamID == S3103Consts.USER_PARAM_PLAYSCREEN_NOPLAY);
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
                            c => c.ParamID == S3103Consts.USER_PARAM_AUTORELATIVEPLAY);
                    if (setting != null && setting.StringValue == "0")
                    {
                        autoRelative = false;
                    }
                }
                if (string.IsNullOrEmpty(fileName))
                {
                    CurrentApp.WriteLog("PlayRecord", string.Format("FileName is empty"));
                    return;
                }
                if (string.IsNullOrEmpty(relativeName))
                {
                    CurrentApp.WriteLog("PlayRecord", string.Format("RelativeName is empty"));
                    autoRelative = false;
                }
                string audioUrl = string.Format("{0}://{1}:{2}/{3}/{4}",
                    CurrentApp.Session.AppServerInfo.SupportHttps ? "https" : "http",
                    CurrentApp.Session.AppServerInfo.Address,
                    CurrentApp.Session.AppServerInfo.Port,
                    ConstValue.TEMP_DIR_MEDIADATA,
                    fileName);
                string videoPath;
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
                            VoicePlayer.VideoUrl = videoPath;
                            CurrentApp.WriteLog("PlayRecord", string.Format("VideoPath:{0}", videoPath));
                            VoicePlayer.Play();
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
                            VoicePlayer.MediaType = 3;
                            VoicePlayer.AudioUrl = audioUrl;
                            CurrentApp.WriteLog("PlayRecord", string.Format("AudioUrl:{0}", audioUrl));
                            videoPath = string.Empty;
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
                            VoicePlayer.VideoUrl = videoPath;
                            CurrentApp.WriteLog("PlayRecord", string.Format("VideoPath:{0}", videoPath));
                            VoicePlayer.Play();
                        }
                        break;
                    case 2:
                        if (!noPlayScreen)
                        {
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
                            VoicePlayer.VideoUrl = videoPath;
                            CurrentApp.WriteLog("PlayRecord", string.Format("VideoPath:{0}", videoPath));
                            VoicePlayer.Play();
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
                //bool isAutoRelative = false;
                //if (ListUserSettingInfos != null)
                //{
                //    var setting =
                //        ListUserSettingInfos.FirstOrDefault(s => s.ParamID == S3103Consts.USER_PARAM_AUTORELATIVEPLAY);
                //    if (setting != null && setting.StringValue == "1")
                //    {
                //        isAutoRelative = true;
                //    }
                //}
                //if (!isAutoRelative) { return; }
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
                webRequest.Code = 41;//(int)S3102Codes.GetRelativeRecordList
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
                        ShowException(CurrentApp.GetLanguageInfo("3106T00039", "Download server not exist."));
                        break;
                    case RecordOperator.RET_DOWNLOAD_APPSERVER_FAIL:
                        ShowException(CurrentApp.GetLanguageInfo("3106T00038", "Download fail"));
                        break;
                    case RecordOperator.RET_GET_REAL_PASSWORD_FAIL:
                        ShowException(CurrentApp.GetLanguageInfo("3106T00036", "Get real password fail."));
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
                        GetRealPassword(info);
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void OnPlayStopped()
        {
            if (PlayStopped != null)
            {
                PlayStopped();
            }
        }

        private void OnPlayerEvent(object sender, RoutedPropertyChangedEventArgs<UMPEventArgs> e)
        {
            if (PlayerEvent != null)
            {
                PlayerEvent(sender, e);
            }
        }

    }
}
