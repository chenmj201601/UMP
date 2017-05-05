using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using UMPS3103.Codes;
using UMPS3103.Models;
using UMPS3103.Wcf31021;
using UMPS3103.Wcf31031;
using VoiceCyber.Common;
using VoiceCyber.NAudio.Controls;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31031;
using VoiceCyber.UMP.CommonService03;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.UMP.Controls.Players;

namespace UMPS3103
{
    /// <summary>
    /// UCPlayBox.xaml 的交互逻辑
    /// 公用播放控件，音频播放，声道控制，音量控制，速度控制，循环控制
    /// </summary>
    public partial class UCPlayBox
    {
        #region Members
        public TaskAssign ParentPage1;
        public DoubleTask.DoubleTaskAssign ParentPage2;
        public TaskRecordDetail TaskPage1;
        public TaskScoreForm TaskPage2;
        public RecordInfoItem RecordInfoItem;//录音信息
        public RecordPlayItem RecordPlayItem;

        public List<SftpServerInfo> ListSftpServers;
        public List<DownloadParamInfo> ListDownloadParams;
        public List<SettingInfo> ListUserSettingInfos;
        public List<RecordEncryptInfo> ListEncryptInfo;
        public Service03Helper Service03Helper;
        private BackgroundWorker mWorker;
        private int workMsg = 0;
        //private RecordInfoItem mListCurrentRecord;

        public TaskInfoDetail TaskRecordInfoItem;
        public bool IsAutoPlay;
        public bool mIsInited;
        public double StartPosition;
        public double StopPostion;
        public event Action PlayStopped;
        private TimeSpan mCurrentTime;
        public event RoutedPropertyChangedEventHandler<UMPEventArgs> PlayerEvent;
        /// <summary>
        /// 0       不循环
        /// 1       单循环
        /// 2       列表循环
        /// </summary>
        public int CircleMode;
        private List<KeywordResultInfo> mListKeywordInfos;
        private ObservableCollection<KeywordResultItem> mListKeywordItems;
        public bool ShowKeyword;
        #endregion
        public UCPlayBox()
        {
            InitializeComponent();
            ListSftpServers = S3103App.mListSftpServers;
            Service03Helper = S3103App.mService03Helper;
            ListEncryptInfo = S3103App.mListRecordEncryptInfos;
            ListDownloadParams = S3103App.mListDownloadParams;
            Loaded += UCPlayBox_Loaded;
            Unloaded += UCPlayBox_Unloaded;

            BorderKeywords.SizeChanged += (s, e) => SetKeyWordItemOffset();
            VoicePlayer.PlayerEvent += VoicePlayer_PlayerEvent;

            IsAutoPlay = false;
            CircleMode = 0;
        }

        void UCPlayBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (!mIsInited) 
            {
                Init();
                mIsInited = true;
            }
            //VoicePlayer.PlayerEvent += VoicePlayer_PlayerEvent;
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

        private string GetRecordInfoByRef(string SerialID)//20140212... 2014年2月12号
        {
            string recordreference = string.Empty;
            try
            {
                string tablename = ConstValue.TABLE_NAME_RECORD + "_" + CurrentApp.Session.RentInfo.Token;
                var tableInfo =
                        CurrentApp.Session.ListPartitionTables.FirstOrDefault(
                            t => t.TableName == ConstValue.TABLE_NAME_RECORD && t.PartType == TablePartType.DatetimeRange);
                if (tableInfo != null)//有分表 当前仅按年月分表 ex：T_21_001_00000_1405
                {
                    tablename += "_" + SerialID.Substring(0, 4);
                }
                string strSql = string.Format("SELECT * FROM {0} WHERE C002={1}", tablename, SerialID);
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3103Codes.GetRecordData;
                webRequest.ListData.Add(strSql);
                webRequest.ListData.Add(tablename);
                webRequest.ListData.Add("mark");
                //Service31031Client client = new Service31031Client();
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    if (webReturn.Message != S3103Consts.Err_TableNotExit)
                       ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return string.Empty;
                }
                if (webReturn.ListData == null)
                {
                   ShowException(string.Format("Fail. ListData is null"));
                    return string.Empty;
                }
                if (webReturn.ListData.Count <= 0) { return string.Empty; }

                OperationReturn optReturn = XMLHelper.DeserializeObject<RecordInfo>(webReturn.ListData[0]);
                if (!optReturn.Result)
                {
                   ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return string.Empty;
                }
                RecordInfo recordInfo = optReturn.Data as RecordInfo;
                if (recordInfo == null)
                {
                   ShowException(string.Format("Fail. RecordInfo is null"));
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
                if (RecordInfoItem != null )
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

                    /*
                    if (mListCurrentRecord == null)
                        return;

                    if (ListSftpServers == null || ListSftpServers.Count <= 0)
                    {
                        App.ShowExceptionMessage(string.Format("{0}",
                            App.GetLanguageInfo("3103T00122", "SftpServer not exist")));
                        return;
                    }
                    var sftpServer = ListSftpServers.FirstOrDefault(s => s.HostAddress == mListCurrentRecord.VoiceIP);
                    if (sftpServer == null)
                    {
                        App.ShowExceptionMessage(string.Format("{0}\r\n\r\n{1}",
                            App.GetLanguageInfo("3103T00122", "SftpServer not exist"),
                            mListCurrentRecord.VoiceIP));
                        return;
                    }
                    string strPartInfo = string.Empty;
                    PartitionTableInfo partInfo =
                        App.Session.ListPartitionTables.FirstOrDefault(
                            p =>
                                p.TableName == ConstValue.TABLE_NAME_RECORD && p.PartType == TablePartType.DatetimeRange);
                    if (partInfo != null)
                    {
                        DateTime startTime = Convert.ToDateTime(mListCurrentRecord.StartRecordTime);
                        strPartInfo = string.Format("{0}{1}", startTime.ToString("yy"), startTime.ToString("MM"));
                    }
                    if (Service03Helper == null)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\tService03Helper is null"));
                        return;
                    }
                    RequestMessage request = new RequestMessage();
                    request.Command = (int)Service03Command.DownloadRecordFile;
                    request.ListData.Add(sftpServer.HostAddress);
                    request.ListData.Add(sftpServer.HostPort.ToString());
                    request.ListData.Add(string.Format("{0}|{1}", App.Session.UserID, App.Session.RentInfo.Token));
                    request.ListData.Add(App.Session.UserInfo.Password);
                    request.ListData.Add(mListCurrentRecord.RowID.ToString());
                    request.ListData.Add(mListCurrentRecord.SerialID.ToString());
                    request.ListData.Add(strPartInfo);

                    OperationReturn optReturn = null;
                    VoicePlayer.IsEnabled = false;
                    mWorker = new BackgroundWorker();
                    mWorker.DoWork += (s, de) =>
                    {
                        optReturn = Service03Helper.DoRequest(request);
                    };
                    mWorker.RunWorkerCompleted += (s, re) =>
                    {
                        mWorker.Dispose();
                        VoicePlayer.IsEnabled = true;
                        if (!optReturn.Result)
                        {
                            App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        ReturnMessage retMessage = optReturn.Data as ReturnMessage;
                        if (retMessage == null)
                        {
                            App.ShowExceptionMessage(string.Format("ReturnMessage is null"));
                            return;
                        }
                        if (!retMessage.Result)
                        {
                            App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", retMessage.Code, retMessage.Message));
                            return;
                        }
                        try
                        {
                            string path = retMessage.Data;
                            path = string.Format("{0}://{1}:{2}/MediaData/{3}",
                                App.Session.AppServerInfo.Protocol,
                                App.Session.AppServerInfo.Address,
                                App.Session.AppServerInfo.Port,
                                path);
                            App.WriteLog(string.Format("Url:{0}", path));
                            VoicePlayer.Url = path;
                            VoicePlayer.Title = TaskRecordInfoItem.RecoredReference.ToString();
                            VoicePlayer.Play();
                        }
                        catch (Exception ex)
                        {
                            App.ShowExceptionMessage(ex.Message);
                        }
                    };
                    mWorker.RunWorkerAsync();
                    */
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

        void VoicePlayer_PlayerEvent(object sender, RoutedPropertyChangedEventArgs<UMPEventArgs> e)
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
                        //App.WriteLog("MediaPlayer", string.Format("MediaPlayer debug.\t{0}", param));
                        break;
                }
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }

        #region Init and Load

        private void Init()
        {
            try
            {
                ListKeyWords.ItemsSource = mListKeywordItems;
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
                    SetKeyWordItemOffset();
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
                webRequest.Code = (int)S3103Codes.GetKeywordResultList;
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
                uc.CurrentApp = CurrentApp;
                uc.PasswordManagerEvent += PasswordManagement_PasswordManagementEvent;
                uc.ParentPage1 = ParentPage1;
                uc.ParentPage2 = ParentPage2;
                uc.TaskPage1 = TaskPage1;
                uc.TaskPage2 = TaskPage2;
                uc.ListUserSettingInfos = ListUserSettingInfos;
                uc.RecordInfoItem = RecordInfoItem;
                uc.ListRecordEncryptInfos = ListEncryptInfo;
                if (ParentPage1 != null)
                {
                    ParentPage1.OpenPasswordPanel(uc);
                } if (ParentPage2 != null)
                {
                    ParentPage2.OpenPasswordPanel(uc);
                }
                if (TaskPage1 != null)
                {
                    TaskPage1.OpenPasswordPanel(uc);
                }
                if (TaskPage2 != null)
                {
                    TaskPage2.OpenPasswordPanel(uc);
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
                recordInfo.RecordReference = GetRecordInfoByRef(recordInfo.SerialID.ToString());
                if (string.IsNullOrWhiteSpace(recordInfo.RecordReference)) { return; }
                if (ParentPage1 != null)
                {
                    ParentPage1.SetBusy(true);
                }
                if (ParentPage2 != null)
                {
                    ParentPage2.SetBusy(true);
                }
                bool isSuccess = false;
                mWorker = new BackgroundWorker();
                mWorker.WorkerReportsProgress = true;
                mWorker.WorkerSupportsCancellation = true;
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
                            workMsg = optReturn.Code;
                            CurrentApp.WriteLog("GetRealPass",
                                string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            mWorker.ReportProgress(0);
                            mWorker.Dispose();
                            return;
                        }
                        isSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        CurrentApp.WriteLog("GetRealPass", string.Format("Fail.\t{0}", ex.Message));
                    }
                };
                mWorker.ProgressChanged += (s, pe) =>
                {
                    ShowRecordOperatorMessage(workMsg);
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    if (ParentPage1 != null)
                    {
                        ParentPage1.SetBusy(false);
                    }
                    if (ParentPage2 != null)
                    {
                        ParentPage2.SetBusy(false);
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
                recordInfo.RecordReference = GetRecordInfoByRef(recordInfo.SerialID.ToString());
                if (string.IsNullOrWhiteSpace(recordInfo.RecordReference)) { return; }
                if (ParentPage1 != null)
                {
                    ParentPage1.SetBusy(true);
                }
                if (ParentPage2 != null)
                {
                    ParentPage2.SetBusy(true);
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
                            workMsg = optReturn.Code;
                            CurrentApp.WriteLog("DownloadAppServer", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            mWorker.ReportProgress(0);
                            mWorker.Dispose();
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
                            workMsg = optReturn.Code;
                            CurrentApp.WriteLog("OriginalDecrypt",string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            mWorker.ReportProgress(1);
                            mWorker.Dispose();
                            return;
                        }
                        fileName = optReturn.Data.ToString();
                        recordOperator.RecordInfo = recordInfo;
                        //解密文件
                        optReturn = recordOperator.DecryptRecord(fileName);
                        if (!optReturn.Result)
                        {
                            workMsg = optReturn.Code;
                            CurrentApp.WriteLog("DecryptRecord", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            mWorker.ReportProgress(2);
                            mWorker.Dispose();
                            return;
                        }
                        fileName = optReturn.Data.ToString();
                        recordOperator.RecordInfo = recordInfo;
                        //转换格式
                        optReturn = recordOperator.ConvertWaveFormat(fileName);
                        if (!optReturn.Result)
                        {
                            workMsg = optReturn.Code;
                            CurrentApp.WriteLog("ConvertWaveFormat", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            mWorker.ReportProgress(3);
                            mWorker.Dispose();
                            return;
                        }
                        fileName = optReturn.Data.ToString();
                    }
                    catch (Exception ex)
                    {
                        CurrentApp.WriteLog("PlayRecord", string.Format("Fail.\t{0}", ex.Message));
                    }
                };
                mWorker.ProgressChanged += (s, pe) =>
                {
                    ShowRecordOperatorMessage(workMsg);
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    VoicePlayer.IsEnabled = true;
                    if (ParentPage1 != null)
                    {
                        ParentPage1.SetBusy(false);
                    }
                    if (ParentPage2 != null)
                    {
                        ParentPage2.SetBusy(false);
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
                CurrentApp.WriteLog("PlayRecord", string.Format("FileName:{0};RelativeName:{1}", fileName, relativeName));
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
                                ShowRecordOperatorMessage(optReturn.Code);
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
                                ShowRecordOperatorMessage(optReturn.Code);
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
                                ShowRecordOperatorMessage(optReturn.Code);
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
                if (RecordInfoItem.MediaType !=1)
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
        //新加
        private void SetKeyWordItemOffset() 
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

        private void ShowRecordOperatorMessage(int code)
        {
            try
            {
                switch (code)
                {
                    case RecordOperator.RET_DOWNLOADSERVER_NOT_EXIST:
                       ShowException(CurrentApp.GetLanguageInfo("3103T00167", "Download server not exist."));
                        break;
                    case RecordOperator.RET_DOWNLOAD_APPSERVER_FAIL:
                       ShowException(CurrentApp.GetLanguageInfo("3103T00166", "Download fail"));
                        break;
                    case RecordOperator.RET_GET_REAL_PASSWORD_FAIL:
                       ShowException(CurrentApp.GetLanguageInfo("3103T00164", "Get real password fail."));
                        UCPasswordManagement ucPwdMana = new UCPasswordManagement();
                        ucPwdMana.CurrentApp = CurrentApp;
                        ucPwdMana.PasswordManagerEvent += PasswordManagement_PasswordManagementEvent;
                        ucPwdMana.TaskPage2 = TaskPage2;
                        ucPwdMana.ListUserSettingInfos = ListUserSettingInfos;
                        ucPwdMana.RecordInfoItem = RecordInfoItem;
                        ucPwdMana.ListRecordEncryptInfos = ListEncryptInfo;
                        if (TaskPage2 != null)
                        {
                            TaskPage2.OpenPasswordPanel(ucPwdMana);
                        }
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
