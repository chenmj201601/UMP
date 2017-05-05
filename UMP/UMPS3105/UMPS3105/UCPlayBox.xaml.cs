using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using Common3105;
using UMPS3105.Codes;
using UMPS3105.Models;
using VoiceCyber.Common;
using VoiceCyber.NAudio.Controls;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.CommonService03;
using System.Linq;
using VoiceCyber.UMP.Communications;
using UMPS3105.Wcf31031;
using VoiceCyber.UMP.Controls;
using UMPS3105.Wcf31021;
using VoiceCyber.UMP.Controls.Players;

namespace UMPS3105
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
        public event Action PlayStopped;
        public event RoutedPropertyChangedEventHandler<UMPEventArgs> PlayerEvent;

        //public AppealManagePage ParentPage1;
        public AppealRecheckMainView ParentPage2;
        public AppealApprovalMainView ParentPage3;

        public TaskScoreMainView ScorePage;
        private List<SftpServerInfo> ListSftpServers;
        public List<DownloadParamInfo> ListDownloadParams;
        public List<SettingInfo> ListUserSettingInfos;
        public List<RecordEncryptInfo> ListEncryptInfo;
        private Service03Helper Service03Helper;
        private BackgroundWorker mWorker;

        public string RecoredReference;
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

        private TimeSpan mCurrentTime;

        public UCPlayBox()
        {
            InitializeComponent();
            ListSftpServers = S3105App.mListSftpServers;
            Service03Helper = S3105App.mService03Helper;
            ListEncryptInfo = S3105App.mListRecordEncryptInfos;
            ListDownloadParams = S3105App.mListDownloadParams;

            Loaded += UCPlayBox_Loaded;
            Unloaded += UCPlayBox_Unloaded;
            VoicePlayer.PlayerEvent += VoicePlayer_PlayerEvent;

            IsAutoPlay = false;
            CircleMode = 0;
        }

        void UCPlayBox_Loaded(object sender, RoutedEventArgs e)
        {
            if(!GetRecordInfoByRef(RecoredReference))
            {
                ShowException(string.Format("Get Recore Fail "));
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

        public TimeSpan GetCurrentTime()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(RecoredReference))
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
            { VoicePlayer.Close(); }
            catch { }
        }

        private bool GetRecordInfoByRef(string recordreference)//20140212... 2014年2月12号
        {
            bool ret = false;
            try
            {
                string tablename = ConstValue.TABLE_NAME_RECORD + "_" + CurrentApp.Session.RentInfo.Token;
                var tableInfo =
                        CurrentApp.Session.ListPartitionTables.FirstOrDefault(
                            t => t.TableName == ConstValue.TABLE_NAME_RECORD && t.PartType == TablePartType.DatetimeRange);
                if (tableInfo != null)//有分表 当前仅按年月分表 ex：T_21_001_00000_1405
                {
                    tablename += "_" + recordreference.Substring(0, 4);
                }
                string strSql = string.Format("SELECT * FROM {0} WHERE C002={1}", tablename, recordreference);
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = 13;
                webRequest.ListData.Add(strSql);
                webRequest.ListData.Add(tablename);
                webRequest.ListData.Add("mark");
                //Service31031Client client = new Service31031Client();
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    if (webReturn.Message != "ERR_TABLE_NOT_EXIT")
                        ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return false;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3105T00101", string.Format("ListData is null")));
                    return false;
                }
                if (webReturn.ListData.Count <= 0) { return false; }

                OperationReturn optReturn = XMLHelper.DeserializeObject<RecordInfo>(webReturn.ListData[0]);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                RecordInfo recordInfo = optReturn.Data as RecordInfo;
                if (recordInfo == null)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3105T00101", string.Format("ListData is null")));
                    return false;
                }
                RecordInfoItem = new RecordInfoItem(recordInfo);
                ret = true;
            }
            catch
            {
                ret = false;
            }
            return ret;
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

        private void OnPlayStopped()
        {
            if (PlayStopped != null)
            {
                PlayStopped();
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
                         s => s.ParamID == S3105Consts.USER_PARAM_PLAYSCREEN_TOPMOST);
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
                         s => s.ParamID == S3105Consts.USER_PARAM_PLAYSCREEN_SCALE);
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
                uc.CurrentApp = CurrentApp;
                uc.PasswordManagerEvent += PasswordManagement_PasswordManagementEvent;
                //uc.ParentPage1 = ParentPage1;
                uc.ParentPage2 = ParentPage2;
                uc.ParentPage3 = ParentPage3;
                uc.ScorePage = ScorePage;
                uc.ListUserSettingInfos = ListUserSettingInfos;
                uc.RecordInfoItem = RecordInfoItem;
                uc.ListRecordEncryptInfos = ListEncryptInfo;
                //if (ParentPage1 != null)
                //{
                //    ParentPage1.OpenPasswordPanel(uc);
                //} 
                if (ParentPage2 != null)
                {
                    ParentPage2.OpenPasswordPanel(uc);
                }
                if (ParentPage3 != null)
                {
                    ParentPage3.OpenPasswordPanel(uc);
                }
                if (ScorePage != null)
                {
                    ScorePage.OpenPasswordPanel(uc);
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
                if (ParentPage3 != null)
                {
                    ParentPage3.SetBusy(true,string.Empty);
                }
                if (ParentPage2 != null)
                {
                    ParentPage2.SetBusy(true,string.Empty);
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
                    if (ParentPage3 != null)
                    {
                        ParentPage3.SetBusy(false,string.Empty);
                    }
                    if (ParentPage2 != null)
                    {
                        ParentPage2.SetBusy(false,string.Empty);
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
                if (ParentPage3 != null)
                {
                    ParentPage3.SetBusy(true,string.Empty);
                }
                if (ParentPage2 != null)
                {
                    ParentPage2.SetBusy(true,string.Empty);
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
                    if (ParentPage3 != null)
                    {
                        ParentPage3.SetBusy(false,string.Empty);
                    }
                    if (ParentPage2 != null)
                    {
                        ParentPage2.SetBusy(false,string.Empty);
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
                            c => c.ParamID == S3105Consts.USER_PARAM_PLAYSCREEN_NOPLAY);
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
                            c => c.ParamID == S3105Consts.USER_PARAM_AUTORELATIVEPLAY);
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
                        ShowException(CurrentApp.GetLanguageInfo("3105T00113", "Download server not exist."));
                        break;
                    case RecordOperator.RET_DOWNLOAD_APPSERVER_FAIL:
                        ShowException(CurrentApp.GetLanguageInfo("3105T00112", "Download fail"));
                        break;
                    case RecordOperator.RET_GET_REAL_PASSWORD_FAIL:
                        ShowException(CurrentApp.GetLanguageInfo("3105T00110", "Get real password fail."));
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


        private void OnPlayerEvent(object sender, RoutedPropertyChangedEventArgs<UMPEventArgs> e)
        {
            if (PlayerEvent != null)
            {
                PlayerEvent(sender, e);
            }
        }
    }
}
