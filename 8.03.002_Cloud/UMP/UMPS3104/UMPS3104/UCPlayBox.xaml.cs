using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using UMPS3104.Models;
using UMPS3104.S3102Codes;
using UMPS3104.Wcf31041;
using VoiceCyber.Common;
using VoiceCyber.NAudio.Controls;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31041;
using VoiceCyber.UMP.Common31041.Common3102;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.UMP.Controls.Players;

namespace UMPS3104
{
    /// <summary>
    /// UCPlayBox.xaml 的交互逻辑
    /// 公用播放控件，音频播放，声道控制，音量控制，速度控制，循环控制
    /// </summary>
    public partial class UCPlayBox
    {
        /// <summary>
        /// 播放结束事件
        /// </summary>
        public AgentIntelligentClient MainPage;

        public RecordInfoItem RecordInfoItem;
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

        public List<SftpServerInfo> ListSftpServers;
        public List<DownloadParamInfo> ListDownloadParams;
        public List<SettingInfo> ListUserSettingInfos;
        public List<RecordEncryptInfo> ListEncryptInfo;
        public Service03Helper Service03Helper;

        private TimeSpan mCurrentTime;
        private BackgroundWorker mWorker;
        public string PlayUrl;

        public UCPlayBox()
        {
            InitializeComponent();
            ListSftpServers = new List<SftpServerInfo>();
            //LoadSftpServerList();

            Loaded += UCPlayBox_Loaded;
            Unloaded += UCPlayBox_Unloaded;
            VoicePlayer.PlayerEvent += VoicePlayer_PlayerEvent;

            IsAutoPlay = false;
            CircleMode = 0;
        }

        void UCPlayBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(PlayUrl))
            {
                VoicePlayer.MediaType = 1;
                VoicePlayer.AudioUrl = PlayUrl;
                VoicePlayer.Play();
            }
            if (RecordInfoItem != null)
            {
                //关于oracle下载关联录屏失败，对时间的处理，转换成UTC时间
                RecordInfoItem.StartRecordTime = RecordInfoItem.StartRecordTime.ToUniversalTime();
                RecordInfoItem.RecordInfo.StartRecordTime = RecordInfoItem.StartRecordTime.ToUniversalTime();
                RecordInfoItem.RecordInfo.StopRecordTime = RecordInfoItem.StopRecordTime.ToUniversalTime();

            }
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

        #region 當前播放位置
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
                App.ShowExceptionMessage(ex.Message);
            }
            return mCurrentTime;
        }

        /// <summary>
        /// 开始播放
        /// </summary>
        private string GetRecordInfoByRef(string SerialID)//20140212... 2014年2月12号
        {
            string recordreference = string.Empty;
            try
            {
                string tablename = ConstValue.TABLE_NAME_RECORD + "_" + App.Session.RentInfo.Token;
                if (App.isCutMonth)//有分表 当前仅按年月分表 ex：T_21_001_00000_1405
                {
                    tablename += "_" + SerialID.Substring(0, 4);
                }
                string strSql = string.Format("SELECT * FROM {0} WHERE C002={1}", tablename, SerialID);
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S3104Codes.GetRecordData;
                webRequest.ListData.Add(strSql);
                webRequest.ListData.Add(tablename);
                //Service31041Client client = new Service31041Client();
                Service31041Client client = new Service31041Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service31041"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPClientOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    if (webReturn.Message != S3104Consts.Err_TableNotExit)
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return string.Empty;
                }
                if (webReturn.ListData == null)
                {
                    App.ShowExceptionMessage(string.Format("Fail. ListData is null"));
                    return string.Empty;
                }
                if (webReturn.ListData.Count <= 0) { return string.Empty; }

                OperationReturn optReturn = XMLHelper.DeserializeObject<RecordInfo>(webReturn.ListData[0]);
                if (!optReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return string.Empty;
                }
                RecordInfo recordInfo = optReturn.Data as RecordInfo;
                if (recordInfo == null)
                {
                    App.ShowExceptionMessage(string.Format("Fail. RecordInfo is null"));
                    return string.Empty;
                }
                recordreference = recordInfo.RecordReference;
            }
            catch
            {
                recordreference = string.Empty;
            }
            return recordreference;
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
                App.ShowExceptionMessage(ex.Message);
            }
        }
        #endregion

        void VoicePlayer_PlayerEvent(object sender, RoutedPropertyChangedEventArgs<UMPEventArgs> e)
        {
            try
            {
                OnPlayerEvent(sender, e);
                if (e.NewValue == null) { return; }
                int code = e.NewValue.Code;
                var param = e.NewValue.Data;

                //For Test
                //App.WriteLog("Player", string.Format("{0}\t{1}", code, param));

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
                        App.WriteLog("MediaPlayer", string.Format("MediaPlayer exception.\t{0}", param));
                        break;
                    case Defines.EVT_COMMON:
                        //App.WriteLog("MediaPlayer", string.Format("MediaPlayer debug.\t{0}", param));
                        break;
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
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
                        GetRealPassword(info);
                        break;
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }


        /// <summary>
        /// 播放结束事件
        /// </summary>
        public event Action PlayStopped;
        private void OnPlayStopped()
        {
            try
            {
                if (PlayStopped != null)
                {
                    PlayStopped();
                }
            }
            catch (Exception)
            {
                
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
                         s => s.ParamID == S3104Consts.USER_PARAM_PLAYSCREEN_TOPMOST);
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
                         s => s.ParamID == S3104Consts.USER_PARAM_PLAYSCREEN_SCALE);
                    if (settingInfo != null && int.TryParse(settingInfo.StringValue, out intScale))
                    {

                    }
                }
                VoicePlayer.ScreenScale = intScale;
            }
            catch (Exception ex)
            {
                App.WriteLog("SetScreenPlayer", string.Format("Fail.\t{0}", ex.Message));
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
                var encryptInfo = ListEncryptInfo.FirstOrDefault(s => s.UserID == App.Session.UserID
                                                                      && s.ServerAddress == strServerAddress);
                if (encryptInfo == null)
                {
                    encryptInfo = new RecordEncryptInfo();
                    encryptInfo.UserID = App.Session.UserID;
                    encryptInfo.ServerAddress = strServerAddress;
                    encryptInfo.StartTime = DateTime.Now.ToUniversalTime();
                    encryptInfo.EndTime = DateTime.Now.AddDays(1).ToUniversalTime();
                    encryptInfo.IsRemember = false;
                    ListEncryptInfo.Add(encryptInfo);
                }
                UCPasswordManagement uc = new UCPasswordManagement();
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
                App.ShowExceptionMessage(ex.Message);
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
                    MainPage.SetBusy(true, "Getting real extension");
                }
                bool isSuccess = false;
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    try
                    {
                        OperationReturn optReturn;
                        RecordOperator recordOperator = new RecordOperator(recordInfo);
                        recordOperator.Debug += (cat, msg) => App.WriteLog(cat, msg);
                        recordOperator.Session = App.Session;
                        recordOperator.ListSftpServers = ListSftpServers;
                        recordOperator.ListDownloadParams = ListDownloadParams;
                        recordOperator.ListEncryptInfo = ListEncryptInfo;
                        recordOperator.Service03Helper = Service03Helper;
                        optReturn = recordOperator.GetRealPassword(info);
                        if (!optReturn.Result)
                        {
                            ShowRecordOperatorMessage(optReturn);
                            App.WriteLog("GetRealPass",
                                string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        isSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        App.WriteLog("GetRealPass", string.Format("Fail.\t{0}", ex.Message));
                    }
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    if (MainPage != null)
                    {
                        MainPage.SetBusy(false, "");
                    }
                    if (!isSuccess)
                    {
                        App.WriteLog("GetRealPass", string.Format("Get real password fail."));
                        return;
                    }
                    PlayRecord();
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void PlayRecord()
        {
            try
            {
                if (RecordInfoItem == null) { return; }
                RecordInfo recordInfo = RecordInfoItem.RecordInfo;
                if (recordInfo == null) { return; }
                recordInfo.RecordReference = GetRecordInfoByRef(recordInfo.SerialID.ToString());
                if (string.IsNullOrWhiteSpace(recordInfo.RecordReference)) { return; }
                if (MainPage != null)
                {
                    MainPage.SetBusy(true, "Downloading record file...");
                }
                string fileName = string.Empty;
                string relativeName = string.Empty;
                VoicePlayer.IsEnabled = false;
                mWorker = new BackgroundWorker();
                mWorker.WorkerReportsProgress = true;
                mWorker.WorkerSupportsCancellation = true;
                mWorker.DoWork += (s, de) =>
                {
                    try
                    {
                        OperationReturn optReturn;

                        //获取关联的录屏文件
                        GetRelativeRecordInfos();

                        //处理录音记录
                        RecordOperator recordOperator = new RecordOperator(recordInfo);
                        recordOperator.Debug += (cat, msg) => App.WriteLog(cat, msg);
                        recordOperator.Session = App.Session;
                        recordOperator.ListSftpServers = ListSftpServers;
                        recordOperator.ListDownloadParams = ListDownloadParams;
                        recordOperator.ListEncryptInfo = ListEncryptInfo;
                        recordOperator.Service03Helper = Service03Helper;
                        //下载文件到AppServer
                        optReturn = recordOperator.DownloadFileToAppServer();
                        if (!optReturn.Result)
                        {
                            ShowRecordOperatorMessage(optReturn);
                            App.WriteLog("DownloadAppServer",
                                string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        fileName = optReturn.Data.ToString();
                        if (recordInfo.MediaType == 2)
                        {
                            mWorker.ReportProgress(2);
                            mWorker.Dispose();
                            return;
                        }
                        var relativeRecord = RecordInfoItem.ListRelativeInfos.FirstOrDefault();
                        if (relativeRecord != null)
                        {
                            //如果有关联的录屏文件，下载录屏文件到AppServer
                            recordOperator.RecordInfo = relativeRecord;
                            optReturn = recordOperator.DownloadFileToAppServer();
                            if (!optReturn.Result)
                            {
                                App.WriteLog("DownloadAppServer",
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
                            App.WriteLog("OriginalDecrypt",
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
                            App.WriteLog("DecryptRecord",
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
                            App.WriteLog("ConvertWaveFormat",
                                string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        fileName = optReturn.Data.ToString();
                    }
                    catch (Exception ex)
                    {
                        App.WriteLog("PlayRecord", string.Format("Fail.\t{0}", ex.Message));
                    }
                };
                mWorker.ProgressChanged += (s, pe) =>
                {
                    VoicePlayer.MediaType = 2;
                    VoicePlayer.VideoUrl = fileName;
                    VoicePlayer.Play();
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    VoicePlayer.IsEnabled = true;
                    if (MainPage != null)
                    {
                        MainPage.SetBusy(false, "");
                    }
                    PlayRecord(fileName, relativeName);
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
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
                            c => c.ParamID == S3104Consts.USER_PARAM_PLAYSCREEN_NOPLAY);
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
                            c => c.ParamID == S3104Consts.USER_PARAM_AUTORELATIVEPLAY);
                    if (setting != null && setting.StringValue == "0")
                    {
                        autoRelative = false;
                    }
                }
                if (string.IsNullOrEmpty(fileName))
                {
                    App.WriteLog("PlayRecord", string.Format("FileName is empty"));
                    return;
                }
                if (string.IsNullOrEmpty(relativeName))
                {
                    App.WriteLog("PlayRecord", string.Format("RelativeName is empty"));
                    autoRelative = false;
                }
                string audioUrl = string.Format("{0}://{1}:{2}/{3}/{4}",
                    App.Session.AppServerInfo.SupportHttps ? "https" : "http",
                    App.Session.AppServerInfo.Address,
                    App.Session.AppServerInfo.Port,
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
                            App.WriteLog("PlayRecord", string.Format("AudioUrl:{0}", audioUrl));
                            VoicePlayer.Play();
                        }
                        else
                        {
                            VoicePlayer.MediaType = 3;
                            VoicePlayer.AudioUrl = audioUrl;
                            App.WriteLog("PlayRecord", string.Format("AudioUrl:{0}", audioUrl));
                            videoPath = string.Empty;
                            RecordOperator recordOperator = new RecordOperator();
                            recordOperator.Debug += (cat, msg) => App.WriteLog(cat, msg);
                            recordOperator.Session = App.Session;
                            OperationReturn optReturn = recordOperator.DownloadFileToLocal(fileName);
                            if (!optReturn.Result)
                            {
                                ShowRecordOperatorMessage(optReturn);
                                App.WriteLog("DownloadToLocal",
                                    string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            }
                            else
                            {
                                videoPath = optReturn.Data.ToString();
                            }
                            VoicePlayer.VideoUrl = videoPath;
                            App.WriteLog("PlayRecord", string.Format("VideoPath:{0}", videoPath));
                            VoicePlayer.Play();
                        }
                        break;
                    case 1:
                        if (noPlayScreen
                            || !autoRelative)
                        {
                            VoicePlayer.MediaType = 1;
                            VoicePlayer.AudioUrl = audioUrl;
                            App.WriteLog("PlayRecord", string.Format("AudioUrl:{0}", audioUrl));
                            VoicePlayer.Play();
                        }
                        else
                        {
                            VoicePlayer.MediaType = 3;
                            VoicePlayer.AudioUrl = audioUrl;
                            App.WriteLog("PlayRecord", string.Format("AudioUrl:{0}", audioUrl));
                            videoPath = string.Empty;
                            RecordOperator recordOperator = new RecordOperator();
                            recordOperator.Debug += (cat, msg) => App.WriteLog(cat, msg);
                            recordOperator.Session = App.Session;
                            OperationReturn optReturn = recordOperator.DownloadFileToLocal(relativeName);
                            if (!optReturn.Result)
                            {
                                ShowRecordOperatorMessage(optReturn);
                                App.WriteLog("DownloadToLocal",
                                    string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            }
                            else
                            {
                                videoPath = optReturn.Data.ToString();
                            }
                            VoicePlayer.VideoUrl = videoPath;
                            App.WriteLog("PlayRecord", string.Format("VideoPath:{0}", videoPath));
                            VoicePlayer.Play();
                        }
                        break;
                    case 2:
                        if (!noPlayScreen)
                        {
                            VoicePlayer.MediaType = 2;
                            videoPath = string.Empty;
                            RecordOperator recordOperator = new RecordOperator();
                            recordOperator.Debug += (cat, msg) => App.WriteLog(cat, msg);
                            recordOperator.Session = App.Session;
                            OperationReturn optReturn = recordOperator.DownloadFileToLocal(fileName);
                            if (!optReturn.Result)
                            {
                                ShowRecordOperatorMessage(optReturn);
                                App.WriteLog("DownloadToLocal",
                                    string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            }
                            else
                            {
                                videoPath = optReturn.Data.ToString();
                            }
                            VoicePlayer.VideoUrl = videoPath;
                            App.WriteLog("PlayRecord", string.Format("VideoPath:{0}", videoPath));
                            VoicePlayer.Play();
                        }
                        break;
                    default:
                        App.WriteLog("PlayRecord", string.Format("MediaType invalid.\t{0}", mediaType));
                        break;
                }
            }
            catch (Exception ex)
            {
                App.WriteLog("PlayRecord", string.Format("Fail.\t{0}", ex.Message));
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
                //        ListUserSettingInfos.FirstOrDefault(s => s.ParamID == S3104Consts.USER_PARAM_AUTORELATIVEPLAY);
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
                    App.WriteLog("GetRelativeInfos", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S3104Codes.GetRelativeRecordList;
                webRequest.ListData.Add(optReturn.Data.ToString());
                Service31041Client client = new Service31041Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service31041"));
                WebReturn webReturn = client.UMPClientOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    App.WriteLog("GetRelativeInfos", string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    App.WriteLog("GetRelativeInfos", string.Format("WSFail.\tListData is null"));
                    return;
                }
                int count = webReturn.ListData.Count;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<RecordInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        App.WriteLog("GetRelativeInfos", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    RecordInfo relativeRecord = optReturn.Data as RecordInfo;
                    if (relativeRecord == null)
                    {
                        App.WriteLog("GetRelativeInfos", string.Format("WSFail.\tRelativeRecordInfo is null"));
                        return;
                    }
                    RecordInfoItem.ListRelativeInfos.Add(relativeRecord);
                    App.WriteLog("GetRelativeInfos", string.Format("{0}", relativeRecord.SerialID));
                }
                App.WriteLog("GetRelativeInfos", string.Format("End.\t{0}", count));
            }
            catch (Exception ex)
            {
                App.WriteLog("GetRelativeInfos", string.Format("Fail.\t{0}", ex.Message));
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
                        App.ShowExceptionMessage(App.GetLanguageInfo("3104T00155", "Download server not exist."));
                        break;
                    case RecordOperator.RET_DOWNLOAD_APPSERVER_FAIL:
                        App.ShowExceptionMessage(App.GetLanguageInfo("3104T00154", "Download fail"));
                        break;
                    case RecordOperator.RET_GET_REAL_PASSWORD_FAIL:
                        App.ShowExceptionMessage(App.GetLanguageInfo("3104T00152", "Get real password fail."));
                        break;
                    case RecordOperator.RET_NO_RECORDINFO:
                        App.ShowExceptionMessage(string.Format("RecordInfo is null"));
                        break;
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        #endregion
    }
}
