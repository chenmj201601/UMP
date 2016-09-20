using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
using System.Xml;
using Common3106;
using UMPS3106.Commands;
using UMPS3106.Models;
using UMPS3106.Wcf31061;
using VoiceCyber.Common;
using VoiceCyber.NAudio.Controls;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31031;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS3106
{
    /// <summary>
    /// TutorialRepertoryMainView.xaml 的交互逻辑
    /// </summary>
    public partial class TutorialRepertoryMainView
    {

        #region Members
        public ObservableCollection<FolderTree> mListFolders;
        /// <summary>
        /// 保存数据库中的信息
        /// </summary>
        public List<FolderTree> ListFolderInfo;
        public FolderTree mCurentFolder;
        private BackgroundWorker mWorker;
        public FolderTree CurentFolderInfo = new FolderTree();
        string FolderPath = string.Empty;//根树地址

        public ObservableCollection<FilesItemInfo> mListFilesInfo;
        public List<string> FilesName = new List<string>();

        private UCPlayBox mUCPlayBox;
        public event RoutedPropertyChangedEventHandler<PlayerEventArgs> PlayerEvent;
        public RecordInfoItem recordInfo;
        private int mCircleMode;
        /// <summary>
        /// 老挝专用，保存Url
        /// </summary>
        private string mLaosUrl = string.Empty;
        #endregion


        public TutorialRepertoryMainView()
        {
            InitializeComponent();

            mCurentFolder = new FolderTree();
            mListFolders = new ObservableCollection<FolderTree>();
            ListFolderInfo = new List<FolderTree>();
            mWorker = new BackgroundWorker();
            mListFilesInfo = new ObservableCollection<FilesItemInfo>();
            recordInfo = new RecordInfoItem();
            this.Loaded += TutorialRepertory_Loaded;
            OrgTree.SelectedItemChanged += OrgTree_SelectedItemChanged;
            OrgTree.ItemsSource = mListFolders;
            LvDocument.ItemsSource = mListFilesInfo;
        }

        void TutorialRepertory_Loaded(object sender, RoutedEventArgs e)
        {
            //if (CurrentApp.Session.UserID == ConstValue.USER_ADMIN) { BindCommands(); }//仅限管理员有右键权限
            BindCommands();
            try
            {
                SetBusy(true, string.Empty);
                mWorker = new BackgroundWorker();
                mWorker.WorkerReportsProgress = true;
                mWorker.WorkerSupportsCancellation = true;
                //注册线程主体方法
                mWorker.DoWork += (s, de) =>
                {
                    GetUMPSetupPath();
                    mWorker.ReportProgress(1);
                    GetFolders(mCurentFolder, -1, FolderPath);
                };
                mWorker.ProgressChanged += (s, pe) =>
                {
                    mListFolders.Clear();
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    LaosLinkSetting();
                    InitDocumentColumns();
                    SetBusy(false, string.Empty);
                };
                mWorker.RunWorkerAsync(); //触发DoWork事件
            }
            catch (Exception)
            {
                SetBusy(false, string.Empty);
            }
        }

        /// <summary>
        /// 获取UMP安装的根目录
        /// </summary>
        private void GetUMPSetupPath()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3106Codes.GetUMPSetupPath;
                Service31061Client client = new Service31061Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31061"));
                //Service31061Client client = new Service31061Client();
                WebReturn webReturn = client.UMPTreeOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(webReturn.Message);
                    return;
                }
                if (string.IsNullOrWhiteSpace(webReturn.Data)) { return; }
                FolderPath = webReturn.Data;
                CurrentApp.WriteLog("UMPFolderPath", FolderPath);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #region 文件夹树

        void GetFoldersFromDB()
        {
            try
            {
                ListFolderInfo.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3106Codes.GetFolder;
                Service31061Client client = new Service31061Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31061"));
                //Service31061Client client = new Service31061Client();
                WebReturn webReturn = client.UMPTreeOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(webReturn.Message);
                    return;
                }
                if (webReturn.ListData.Count <= 0) { return; }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<FolderInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    FolderInfo folderInfo = optReturn.Data as FolderInfo;
                    if (folderInfo == null)
                    {
                        ShowException(string.Format("Fail. folderInfo is null"));
                        return;
                    }
                    FolderTree folderItem = new FolderTree();
                    folderItem.FolderID = folderInfo.FolderID;
                    folderItem.FolderName = folderInfo.FolderName;
                    folderItem.TreeParentID = folderInfo.TreeParentID;
                    folderItem.TreeParentName = folderInfo.TreeParentName;
                    folderItem.CreatorId = folderInfo.CreatorId;
                    folderItem.CreatorName = folderInfo.CreatorName;
                    folderItem.CreatorTime = folderInfo.CreatorTime;
                    folderItem.UserID1 = folderInfo.UserID1;
                    folderItem.UserID2 = folderInfo.UserID2;
                    folderItem.UserID3 = folderInfo.UserID3;


                    folderItem.TipNew = CurrentApp.GetLanguageInfo("FO3106001", "New");
                    folderItem.TipRename = CurrentApp.GetLanguageInfo("FO3106002", "Rename");
                    folderItem.TipDelete = CurrentApp.GetLanguageInfo("FO3106003", "Delete");
                    folderItem.TipAllot = CurrentApp.GetLanguageInfo("FO3106004", "Allot");
                    folderItem.TipUpload = CurrentApp.GetLanguageInfo("FO3106005", "Upload");
                    ListFolderInfo.Add(folderItem);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void GetFolders(FolderTree parentInfo, long PId, string folderPath)//传入的path是为了匹配数据库的信息跟物理地址下的信息保持一致,避免被物理删除的文件夹仍然可以继续在数据库写其他数据
        {
            try
            {
                if (PId == -1)
                {
                    GetFoldersFromDB();
                }
                int i = ListFolderInfo.Where(p => p.TreeParentID == PId).Count();
                FolderTree Item = new FolderTree();
                for (int s = 0; s < i; s++)
                {
                    Item = ListFolderInfo.Where(p => p.TreeParentID == PId).ElementAt(s);
                    if (Item == null)
                    {
                        return;
                    }
                    Item.Icon = "/Themes/Default/UMPS3106/Images/document.ico";
                    Item.ParentDiskPath = folderPath;
                    string childPath = System.IO.Path.Combine(folderPath, string.Format("{0}", Item.FolderName));
                    GetFolders(Item, Item.FolderID, childPath);
                    AddChildFolder(parentInfo, Item);
                }


                //DirectoryInfo dir = new DirectoryInfo(folderPath);
                //DirectoryInfo[] Finfo = dir.GetDirectories();
                //foreach(DirectoryInfo Info in Finfo)
                //{

                //}
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }

        }

        private void AddChildFolder(FolderTree parentItem, FolderTree item)
        {
            Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
            if (item.TreeParentID == -1)//只有根树才能添加进来
            {
                Dispatcher.Invoke(new Action(() => mListFolders.Add(item)));
            }
        }

        #endregion

        #region 文件操作
        void InitDocumentColumns()
        {
            try
            {
                string[] lans = "3106T00001,3106T00003,3106T00004,3106T00005,3106T00006,3106T00007".Split(',');
                string[] cols = "RowNumber,FileName,FilePath,FileDescription,mFromType,mFileType".Split(',');
                int[] colwidths = { 60, 120, 160, 200, 85, 85, 40, 40 };
                GridView ColumnGridView = new GridView();
                GridViewColumn gvc;
                if (!string.IsNullOrWhiteSpace(mLaosUrl))
                {
                    lans = "3106T00001,3106T00003,3106T00004,3106T00005,3106T00006,3106T00007,".Split(',');
                    cols = "RowNumber,FileName,FilePath,FileDescription,mFromType,mFileType,Link".Split(',');
                    if (((S3106App)CurrentApp).ListOperationInfos.Where(p => p.ID == S3106Consts.OPT_BrowseHistory).Count() > 0)
                    {
                        lans = "3106T00001,3106T00003,3106T00004,3106T00005,3106T00006,3106T00007, ,FO3106009".Split(',');
                        cols = "RowNumber,FileName,FilePath,FileDescription,mFromType,mFileType,Link,Browse".Split(',');
                    }
                }

                for (int i = 0; i < cols.Count(); i++)
                {
                    gvc = new GridViewColumn();
                    gvc.Header = CurrentApp.GetLanguageInfo(lans[i], cols[i]);
                    gvc.Width = colwidths[i];
                    if (cols[i] == "Browse")
                    {
                        DataTemplate dt = Resources["CellBrowseTemplate"] as DataTemplate;
                        gvc.CellTemplate = dt;
                    }
                    else if (cols[i] == "Link")
                    {
                        DataTemplate dt = Resources["CellLaosLinkTemplate"] as DataTemplate;
                        gvc.CellTemplate = dt;
                    }
                    else
                    {
                        gvc.DisplayMemberBinding = new Binding(cols[i]);
                    }
                    ColumnGridView.Columns.Add(gvc);
                }
                LvDocument.View = ColumnGridView;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void InitDocumentFiles(string FolderID)
        {
            mListFilesInfo.Clear();
            FilesName.Clear();
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3106Codes.GetFiles;
                webRequest.ListData.Add(FolderID);
                Service31061Client client = new Service31061Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31061"));
                //Service31061Client client = new Service31061Client();
                WebReturn webReturn = client.UMPTreeOperation(webRequest);
                client.Close();
                if (!webReturn.Result) { return; }
                if (webReturn.ListData.Count <= 0) { return; }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<FilesItemInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    FilesItemInfo filesItem = optReturn.Data as FilesItemInfo;
                    if (filesItem == null)
                    {
                        ShowException(string.Format("Fail. filesItem is null"));
                        return;
                    }
                    filesItem.RowNumber = i + 1;
                    filesItem.mFileType = filesItem.FileType == "0" ? CurrentApp.GetLanguageInfo("3106T00017", "Is't Wav Document") : CurrentApp.GetLanguageInfo("3106T00016", "Is Wav Document");
                    filesItem.mFromType = filesItem.FromType == "0" ? CurrentApp.GetLanguageInfo("3106T00018", "Manual Uploaded") : CurrentApp.GetLanguageInfo("3106T00019", "Query Upload");


                    FilesName.Add(filesItem.FileName);
                    mListFilesInfo.Add(filesItem);
                }

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        /// <summary>
        /// 单击树事件
        /// </summary>
        private void OrgTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                FolderTree Item = OrgTree.SelectedItem as FolderTree;
                if (Item == null) { return; }
                string strQA = Item.UserID1 + Item.UserID2 + Item.UserID3;
                if (!strQA.Contains(CurrentApp.Session.UserID.ToString()))
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3106T00030", "No Permission."));
                    return;
                }
                InitDocumentFiles(Item.FolderID.ToString());
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void LvDocument_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = LvDocument.SelectedItem as FilesItemInfo;
            if (item != null)
            {
                BrowseHistory(item);
                if (item.FileType == "0")
                {
                    bool flag = DownloadFiles(item.FilePath, item.FileName);
                    if (!flag)
                    {
                        #region 写操作日志
                        string strLog = string.Format("{0} {1}",Utils.FormatOptLogString("FO3106006"), item.FileName);
                        CurrentApp.WriteOperationLog(S3106Consts.OPT_DownLoad.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                        #endregion
                        ShowException(CurrentApp.GetLanguageInfo("3106T00015", "Operation Falied."));
                        return;
                    }
                    else
                    {
                        #region 写操作日志
                        string strLog = string.Format("{0} {1}", Utils.FormatOptLogString("FO3106006"), item.FileName);
                        CurrentApp.WriteOperationLog(S3106Consts.OPT_DownLoad.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                        #endregion
                        ShowInformation(CurrentApp.GetLanguageInfo("3106T00033", "DownLoad To DeskTop"));
                        return;
                    }
                }
                else//wav文件
                {
                    if (item.FromType == "0")
                    {
                        PlayFiles(item.FilePath, item.FileName);
                    }
                    else if (item.FromType == "1")//如果是查询添加的录音，录音文件路径随便写没问题，根据录音流水号去查录音信息，然后跟查询播放一样的流程。
                    {
                        //PlayFiles(string.Empty, item.FileName);
                        PlayRecord(item);
                    }
                }
            }
        }

        private void PlayRecord(FilesItemInfo fileInfo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileInfo.FileName)) { return; }
                recordInfo = GetRecordInfo(fileInfo.FileName);
                if (recordInfo == null) { return; }
                mUCPlayBox = new UCPlayBox();
                mUCPlayBox.CurrentApp = CurrentApp;
                mUCPlayBox.PlayStopped += mUCPlayBox_PlayStopped;
                mUCPlayBox.ParentPage = this;
                mUCPlayBox.RecordInfoItem = recordInfo;
                mUCPlayBox.IsAutoPlay = true;
                mUCPlayBox.CircleMode = mCircleMode;
                mUCPlayBox.StartPosition = 0.0;
                mUCPlayBox.StopPostion = Convert.ToDouble(recordInfo.Duration);
                BorderPlayBox.Child = mUCPlayBox;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public RecordInfoItem GetRecordInfo(string FileName)
        {
            RecordInfoItem item = null;
            try
            {
                #region 判断是否分表
                string tablename = ConstValue.TABLE_NAME_RECORD + "_" + CurrentApp.Session.RentInfo.Token;
                var tableInfo =
                        CurrentApp.Session.ListPartitionTables.FirstOrDefault(
                            t => t.TableName == ConstValue.TABLE_NAME_RECORD && t.PartType == TablePartType.DatetimeRange);
                if (tableInfo != null)//有分表 当前仅按年月分表 ex：T_21_001_00000_1405
                {
                    tablename += "_" + FileName.Substring(0, 4);
                }
                #endregion

                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3106Codes.GetRecordInfo;
                webRequest.ListData.Add(tablename);//录音表表名
                webRequest.ListData.Add(FileName);//记录流水号 t21.c002
                //Service31061Client client = new Service31061Client();
                Service31061Client client = new Service31061Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31061"));
                WebReturn webReturn = client.UMPTreeOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return null;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail. ListData is null"));
                    return null;
                }
                if (webReturn.ListData.Count <= 0) { return null; }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<RecordInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    RecordInfo recordInfo = optReturn.Data as RecordInfo;
                    if (recordInfo == null)
                    {
                        ShowException(string.Format("Fail. RecordInfo is null"));
                        return null;
                    }
                    item = new RecordInfoItem(recordInfo);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return null;
            }
            return item;
        }

        void mUCPlayBox_PlayStopped()//播放停止的按钮
        {
            try
            {
                //if (mCircleMode == 2)//当mCircleMode为列表循环的时候
                //{
                //    var playItem = mUCPlayBox.RecordPlayItem;
                //    if (playItem != null)
                //    {
                //        int index = mListRecordPlayItems.IndexOf(playItem);
                //        if (index < 0) { return; }
                //        if (index == mListRecordPlayItems.Count - 1)
                //        {
                //            playItem = mListRecordPlayItems[0];
                //        }
                //        else
                //        {
                //            playItem = mListRecordPlayItems[index + 1];
                //        }
                //        LvRecordData.SelectedItem = playItem;
                //        mUCPlayBox = new UCPlayBox();
                //        mUCPlayBox.PlayStopped += mUCPlayBox_PlayStopped;
                //        mUCPlayBox.ParentPage = this;
                //        mUCPlayBox.RecordPlayItem = playItem;
                //        mUCPlayBox.IsAutoPlay = true;
                //        mUCPlayBox.CircleMode = mCircleMode;
                //        mUCPlayBox.StartPosition = playItem.StartPosition;
                //        mUCPlayBox.StopPostion = playItem.StopPosition;
                //        BorderPlayBox.Child = mUCPlayBox;
                //    }
                //}
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


        /// <summary>
        /// 上传文件
        /// </summary>
        public bool UpLoadFiles(FilesItemInfo filesInfo, string localPath)
        {
            try
            {
                #region Upload To Disk
                //FileStream fs = new FileStream(localPath, FileMode.Open);
                //StreamReader reader = new StreamReader(fs);
                //string str = reader.ReadToEnd();
                //fs.Close();
                //reader.Close();
                ////string ServerPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, filesInfo.FilePath);
                ////string ServerPath = System.IO.Path.Combine(String.Format("C:\\Program Files\\VoiceCyber\\UMP"), filesInfo.FilePath);
                //string ServerPath = System.IO.Path.Combine(FolderPath, filesInfo.FilePath);
                //WebRequest webRequest = new WebRequest();
                //webRequest.ListData.Clear();
                //webRequest.Session = CurrentApp.Session;
                //webRequest.Code = (int)S3106Codes.UploadToDisk;
                //webRequest.ListData.Add(ServerPath);
                //webRequest.ListData.Add(str);
                ////Service31061Client client = new Service31061Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31061"));
                //Service31061Client client = new Service31061Client();
                //WebReturn webReturn = client.UMPTreeOperation(webRequest);
                //client.Close(); 
                //if (!webReturn.Result)
                //{
                //    CurrentApp.WriteLog("Upload File To Disk  Failed.", string.Format("localPath : {0} \t   ServerPath :{1} \t  Message :{2}", localPath, ServerPath, webReturn.Message));
                //    ShowException(CurrentApp.GetLanguageInfo("3106T00015", "Upload Failed."));
                //    return false;
                //}
                //CurrentApp.WriteLog("Upload File To Disk  Sucessed!", string.Format("localPath : {0} \t   ServerPath :{1} ", localPath, ServerPath));


                string ServerPath = System.IO.Path.Combine(FolderPath, filesInfo.FilePath);
                //string ServerPath = System.IO.Path.Combine(String.Format("C:\\Program Files\\VoiceCyber\\UMP"), filesInfo.FilePath);
                //string ServerPath = System.IO.Path.Combine(FolderPath, filesInfo.FilePath);
                byte[] FileArrayRead = System.IO.File.ReadAllBytes(localPath);
                WebReturn webReturn = null;
                UpRequest upRquest = new UpRequest();
                List<byte> listFileByte = new List<byte>();
                Service31061Client client = new Service31061Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31061"));
                //Service31061Client client = new Service31061Client();
                int count = 0;
                foreach (byte tempByte in FileArrayRead)
                {
                    listFileByte.Add(tempByte);
                    count += 1;
                    if (count == 1048576)
                    {
                        upRquest.SvPath = ServerPath;
                        upRquest.ListByte = listFileByte.ToArray();
                        webReturn = client.UMPUpOperation(upRquest);
                        listFileByte.Clear();
                        count = 0;
                    }
                }
                if (listFileByte.Count > 0)
                {
                    upRquest.SvPath = ServerPath;
                    upRquest.ListByte = listFileByte.ToArray();
                    webReturn = client.UMPUpOperation(upRquest);
                }
                CurrentApp.WriteLog("UpPath", ServerPath);

                #endregion

                #region DB Operation
                WebRequest webRequest = new WebRequest();
                webRequest.ListData.Clear();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.WSGetSerialID;
                webRequest.ListData.Clear();
                webRequest.ListData.Add("31");
                webRequest.ListData.Add("362");
                webRequest.ListData.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
                client = new Service31061Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31061"));
                //client = new Service31061Client();
                webReturn = client.UMPTreeOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Can't Create New FileID"));
                    return false;
                }
                string fileID = webReturn.Data;
                if (string.IsNullOrWhiteSpace(fileID))
                {
                    ShowException(string.Format("Can't Create New FileID"));
                    return false;
                }
                filesInfo.RowNumber = mListFilesInfo.Count + 1;
                filesInfo.FileID = Convert.ToInt64(fileID);
                filesInfo.mFileType = filesInfo.FileType == "0" ? CurrentApp.GetLanguageInfo("3106T00017", "Is't Wav Document") : CurrentApp.GetLanguageInfo("3106T00016", "Is Wav Document");
                filesInfo.mFromType = filesInfo.FromType == "0" ? CurrentApp.GetLanguageInfo("3106T00018", "Manual Uploaded") : CurrentApp.GetLanguageInfo("3106T00019", "Query Upload");
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Clear();
                webRequest.Code = (int)S3106Codes.UploadFile;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(filesInfo);//文件夹树信息
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                client = new Service31061Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31061"));
                //client = new Service31061Client();
                webReturn = client.UMPTreeOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    CurrentApp.WriteLog("Upload File To DB Failed.", string.Format("FileName : {0} \t Message :{1}", filesInfo.FileName, webReturn.Message));
                    return false;
                }
                CurrentApp.WriteLog("Upload File To DB  Sucessed!", filesInfo.FileName);
                #endregion

                #region 写操作日志
                string strLog = string.Format("{0} {1}",  Utils.FormatOptLogString("FO3106005"), filesInfo.FileName);
                CurrentApp.WriteOperationLog(S3106Consts.OPT_UpLoad.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                #endregion
            }
            catch (Exception ex)
            {
                #region 写操作日志
                string strLog = string.Format("{0} {1}",Utils.FormatOptLogString("FO3106005"), filesInfo.FileName);
                CurrentApp.WriteOperationLog(S3106Consts.OPT_UpLoad.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                #endregion
                ShowException(ex.Message);
                return false;
            }
            mListFilesInfo.Add(filesInfo);
            return true;
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        bool DownloadFiles(string downloadPath, string Fname)
        {
            System.Net.HttpWebRequest LHttpWebRequest = null;
            Stream LStreamResponse = null;
            System.IO.FileStream LStreamCertificateFile = null;
            Fname = downloadPath.Substring(downloadPath.LastIndexOf(Fname));
            string LStrFileFullName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), Fname);
            string ServerPath = System.IO.Path.Combine(string.Format("http://{0}:{1}", CurrentApp.Session.AppServerInfo.Address, CurrentApp.Session.AppServerInfo.Port - 1), downloadPath);
            //string path = System.IO.Path.Combine(string.Format("http://192.168.4.166:8081"), downloadPath);
            ServerPath = ServerPath.Replace("\\", "/");
            try
            {
                LStreamCertificateFile = new FileStream(LStrFileFullName, FileMode.Create);
                LHttpWebRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(ServerPath);
                long LContenctLength = LHttpWebRequest.GetResponse().ContentLength;
                LHttpWebRequest.AddRange(0);
                LStreamResponse = LHttpWebRequest.GetResponse().GetResponseStream();

                byte[] LbyteRead = new byte[1024];
                int LIntReadedSize = LStreamResponse.Read(LbyteRead, 0, 1024);
                while (LIntReadedSize > 0)
                {
                    LStreamCertificateFile.Write(LbyteRead, 0, LIntReadedSize);
                    LIntReadedSize = LStreamResponse.Read(LbyteRead, 0, 1024);
                }
                LStreamCertificateFile.Close(); LStreamCertificateFile.Dispose();
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("DownLoad File To Disk  Failed.", string.Format("FileName : {0} \t  SavePath : {1} \t   ServerPath :{2} \t  Message :{3}", Fname, LStrFileFullName, ServerPath, ex.Message));
                ShowException(ex.Message);
                return false;
            }
            finally
            {
                if (LHttpWebRequest != null) { LHttpWebRequest.Abort(); }
                if (LStreamResponse != null) { LStreamResponse.Close(); LStreamResponse.Dispose(); }
                if (LStreamCertificateFile != null) { LStreamCertificateFile.Close(); LStreamCertificateFile.Dispose(); }
            }
            CurrentApp.WriteLog("DownLoad File To Disk  Sucessed!", string.Format("SavePath : {0} \t   ServerPath :{1} \t  ", LStrFileFullName, ServerPath));
            return true;
        }

        /// <summary>
        /// 播放音频
        /// </summary>
        void PlayFiles(string downloadPath, string Fname)
        {
            string ServerPath = System.IO.Path.Combine(string.Format("http://{0}:{1}", CurrentApp.Session.AppServerInfo.Address, CurrentApp.Session.AppServerInfo.Port - 1), downloadPath);
            //string ServerPath = System.IO.Path.Combine(string.Format("http://192.168.4.166:8081"), downloadPath);
            ServerPath = ServerPath.Replace("\\", "/");
            try
            {
                mUCPlayBox = new UCPlayBox();
                mUCPlayBox.CurrentApp = CurrentApp;
                mUCPlayBox.ParentPage = this;
                BorderPlayBox.Child = mUCPlayBox;
                mUCPlayBox.PlayUrl = ServerPath;
                PanelPlayBox.IsVisible = true;
            }
            catch (Exception ex)
            {
                #region 写操作日志
                string strLog = string.Format("{0} {1}", Utils.FormatOptLogString("FO3106007"), Fname);
                CurrentApp.WriteOperationLog(S3106Consts.OPT_Play.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                #endregion
                CurrentApp.WriteLog("Play File Failed.", string.Format("ServerPath : {0} \t   Message :{1}", ServerPath, ex.Message));
                ShowException(ex.Message);
            }
            #region 写操作日志
            string strLog1 = string.Format("{0} {1}", Utils.FormatOptLogString("FO3106007"), Fname);
            CurrentApp.WriteOperationLog(S3106Consts.OPT_Play.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog1);
            #endregion
            CurrentApp.WriteLog("Play File Sucessed!", string.Format("ServerPath : {0} ", ServerPath));
        }

        #endregion

        #region 文件夹右键事件

        /// <summary>
        /// 绑定右键事件
        /// </summary>
        private void BindCommands()
        {
            if (((S3106App)CurrentApp).ListOperationInfos.Where(p => p.ID == S3106Consts.OPT_New).Count() > 0)
            {
                CommandBindings.Add(new CommandBinding(ContentMenuCommand.NewCommand,
                    NewCommand_Click, (s, e) => e.CanExecute = true));
            }
            if (((S3106App)CurrentApp).ListOperationInfos.Where(p => p.ID == S3106Consts.OPT_Rename).Count() > 0)
            {
                CommandBindings.Add(new CommandBinding(ContentMenuCommand.RenameCommand,
                    RenameCommand_Click, (s, e) => e.CanExecute = true));
            }
            if (((S3106App)CurrentApp).ListOperationInfos.Where(p => p.ID == S3106Consts.OPT_Delete).Count() > 0)
            {
                CommandBindings.Add(new CommandBinding(ContentMenuCommand.DeleteCommand,
                    DeleteCommand_Click, (s, e) => e.CanExecute = true));
            }
            if (((S3106App)CurrentApp).ListOperationInfos.Where(p => p.ID == S3106Consts.OPT_Allot).Count() > 0)
            {
                CommandBindings.Add(new CommandBinding(ContentMenuCommand.AllotCommand,
                    AllotCommand_Click, (s, e) => e.CanExecute = true));
            }
            if (((S3106App)CurrentApp).ListOperationInfos.Where(p => p.ID == S3106Consts.OPT_UpLoad).Count() > 0)
            {
                CommandBindings.Add(new CommandBinding(ContentMenuCommand.UploadCommand,
                    UploadCommand_Click, (s, e) => e.CanExecute = true));
            }
            CommandBindings.Add(new CommandBinding(ContentMenuCommand.LaosLinkCommand,
                LaosLinkCommand_Click, (s, e) => e.CanExecute = true));

            CommandBindings.Add(new CommandBinding(ContentMenuCommand.BrowseHistoryCommand,
                BrowseHistoryCommand_Click, (s, e) => e.CanExecute = true));
        }


        private void NewCommand_Click(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                CurentFolderInfo = e.Parameter as FolderTree;
                if (CurentFolderInfo == null) { return; }
                InitDocumentFiles(CurentFolderInfo.FolderID.ToString());
                List<FolderTree> tempFolder = new List<FolderTree>();
                tempFolder = ListFolderInfo.Where(f => f.TreeParentID == CurentFolderInfo.FolderID).ToList();
                string folderName = string.Empty;
                foreach (FolderTree temp in tempFolder)
                {
                    folderName += temp.FolderName;
                }
                PopupPanel.Title = CurrentApp.GetLanguageInfo("3106T00020", "New Folder");
                UCRenamePage newFolderPage = new UCRenamePage();
                newFolderPage.CurrentApp = CurrentApp;
                newFolderPage.ParentPage = this;
                newFolderPage.OperationType = 0;
                newFolderPage.FolderInfo = CurentFolderInfo;
                newFolderPage.FolderNameList = folderName;
                PopupPanel.Content = newFolderPage;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
        private void DeleteCommand_Click(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                CurentFolderInfo = e.Parameter as FolderTree;
                if (CurentFolderInfo == null) { return; }
                InitDocumentFiles(CurentFolderInfo.FolderID.ToString());
                if (CurentFolderInfo.TreeParentID == -1)
                {
                    //CommandBindings.RemoveAt(1);
                    ShowInformation(CurrentApp.GetLanguageInfo("3106T00024", "Root Folder Can't Delete"));
                    return;
                }
                if (CurentFolderInfo.Children.Count() > 0)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3106T00026", "This Folder Can't Delete"));
                    return;
                }
                if (MessageBox.Show(CurrentApp.GetLanguageInfo("3106T00009", "Delete Folder and Document?"), CurrentApp.GetLanguageInfo("3106T00025", "Message Info"), MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
                {
                    bool flag = DeleteFolderDBO(0, CurentFolderInfo.FolderID.ToString(), CurentFolderInfo.FolderName);
                    if (flag)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3106T00014", "Delete Sucessed!"));
                    }
                    else
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3106T00015", "Delete Failed."));
                    }
                }
                else { return; }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
        private void RenameCommand_Click(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                CurentFolderInfo = e.Parameter as FolderTree;
                if (CurentFolderInfo == null) { return; }
                InitDocumentFiles(CurentFolderInfo.FolderID.ToString());
                if (CurentFolderInfo.TreeParentID == -1)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3106T00024", "Root Folder Can't Rename"));
                    return;
                }
                if (mListFilesInfo.Count > 0)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3106T00035", "This Folder Can't Rename"));
                    return;
                }

                List<FolderTree> tempFolder = new List<FolderTree>();
                tempFolder = ListFolderInfo.Where(f => f.TreeParentID == CurentFolderInfo.TreeParentID).ToList();
                string folderName = string.Empty;
                foreach (FolderTree temp in tempFolder)
                {
                    folderName += temp.FolderName;
                }

                PopupPanel.Title = CurrentApp.GetLanguageInfo("3106T00021", "Rename Folder");
                UCRenamePage renameFolderPage = new UCRenamePage();
                renameFolderPage.CurrentApp = CurrentApp;
                renameFolderPage.ParentPage = this;
                renameFolderPage.OperationType = 1;
                renameFolderPage.FolderInfo = CurentFolderInfo;
                renameFolderPage.FolderNameList = folderName;
                PopupPanel.Content = renameFolderPage;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
        private void AllotCommand_Click(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                CurentFolderInfo = e.Parameter as FolderTree;
                if (CurentFolderInfo == null) { return; }
                InitDocumentFiles(CurentFolderInfo.FolderID.ToString());
                PopupPanel.Title = CurrentApp.GetLanguageInfo("FO3106004", "Allot Permission");
                UCAllotPermission permissionPage = new UCAllotPermission();
                permissionPage.CurrentApp = CurrentApp;
                permissionPage.ParentPage = this;
                permissionPage.folderInfo = CurentFolderInfo;
                PopupPanel.Content = permissionPage;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
        private void UploadCommand_Click(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                CurentFolderInfo = e.Parameter as FolderTree;
                if (CurentFolderInfo == null) { return; }
                InitDocumentFiles(CurentFolderInfo.FolderID.ToString());
                string WebFolderPath;
                if (CurentFolderInfo.FolderID != 0)
                {
                    WebFolderPath = CurentFolderInfo.ParentDiskPath.Substring(CurentFolderInfo.ParentDiskPath.LastIndexOf("BOOK"));
                    WebFolderPath = System.IO.Path.Combine(WebFolderPath, CurentFolderInfo.FolderName);
                }
                else
                {
                    WebFolderPath = string.Format("{0}", CurentFolderInfo.FolderName);
                }
                PopupPanel.Title = CurrentApp.GetLanguageInfo("FO3106005", "UpLoad");
                UCUpLoadPage upLoadPage = new UCUpLoadPage();
                upLoadPage.CurrentApp = CurrentApp;
                upLoadPage.ParentPage = this;
                upLoadPage.FolderInfo = CurentFolderInfo;
                upLoadPage.FilesName = FilesName;
                upLoadPage.WebFolderPath = WebFolderPath;
                PopupPanel.Content = upLoadPage;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        /* 选定该项,因为HierarchicalDataTemplate写在treeview里并非是treeviewitem中,获取到的父控件是treeview 所以该方法无效
        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }
        static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
            while (source != null && !(source is T))
                source = VisualTreeHelper.GetParent(source);
            return source;
        }
        **/

        #endregion

        #region  文件夹右键功能逻辑
        /// <summary>
        /// 新建、重命名文件夹
        /// </summary>
        public bool FolderDBO(string FolderName, int operationID)
        {
            try
            {
                WebRequest webRequest;
                Service31061Client client;
                WebReturn webReturn;
                FolderInfo Info;
                string NewFolderPath;//新的地址
                string OldFolderPath;//旧的地址
                if (operationID == 0)//新建
                {
                    #region Disk Operation
                    //OldFolderPath = System.IO.Path.Combine(CurentFolderInfo.ParentDiskPath, CurentFolderInfo.FolderName);//旧的地址
                    //NewFolderPath = System.IO.Path.Combine(OldFolderPath, FolderName);//新的地址
                    //if (!string.IsNullOrWhiteSpace(NewFolderPath) && (!Directory.Exists(NewFolderPath)))//已存在文件夹在DB中被删除，物理路径未删除而遗留下来，然后又命名一个同样名字的文件夹的处理
                    //{
                    //    Directory.CreateDirectory(NewFolderPath);
                    //}
                    //CurrentApp.WriteLog("New Disk Folder Sucessed!", string.Format("NewFolderPath : {0} ", NewFolderPath));
                    #endregion

                    #region DB Operation
                    webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)RequestCode.WSGetSerialID;
                    webRequest.ListData.Add("31");
                    webRequest.ListData.Add("361");
                    webRequest.ListData.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
                    client = new Service31061Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31061"));
                    //client = new Service31061Client();
                    webReturn = client.UMPTreeOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        CurrentApp.WriteLog("New DB Folder Falied", string.Format("Can't Create New FolderID"));
                        ShowException(string.Format("Can't Create New FolderID"));
                        return false;
                    }
                    string folderID = webReturn.Data;
                    if (string.IsNullOrWhiteSpace(folderID))
                    {
                        ShowException(string.Format("Can't Create New FolderID"));
                        return false;
                    }

                    Info = new FolderInfo();
                    Info.FolderID = Convert.ToInt64(folderID);
                    Info.FolderName = FolderName;
                    Info.TreeParentID = CurentFolderInfo.FolderID;
                    Info.TreeParentName = CurentFolderInfo.FolderName;
                    Info.CreatorId = CurrentApp.Session.UserID;
                    Info.CreatorName = CurrentApp.Session.UserInfo.UserName;
                    Info.UserID1 = CurrentApp.Session.UserID.ToString();//被分配人目前限定为管理员
                    Info.UserID2 = string.Empty;
                    Info.UserID3 = string.Empty;
                    webRequest = new WebRequest();
                    webRequest.ListData.Clear();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)S3106Codes.FolderDBO;
                    webRequest.ListData.Add(operationID.ToString());
                    OperationReturn optReturn = XMLHelper.SeriallizeObject(Info);//文件夹树信息
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return false;
                    }
                    webRequest.ListData.Add(optReturn.Data.ToString());
                    OldFolderPath = System.IO.Path.Combine(CurentFolderInfo.ParentDiskPath, CurentFolderInfo.FolderName);//旧的地址
                    NewFolderPath = System.IO.Path.Combine(OldFolderPath, FolderName);//新的地址
                    webRequest.ListData.Add(OldFolderPath);
                    webRequest.ListData.Add(NewFolderPath);
                    client = new Service31061Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31061"));
                    //client = new Service31061Client();
                    webReturn = client.UMPTreeOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        CurrentApp.WriteLog("New DB Folder Falied", string.Format("FolderName : {0} ", FolderName));
                        ShowException(CurrentApp.GetLanguageInfo("3106T00015", "Create New Folder Failed."));
                        return false;
                    }
                    CurrentApp.WriteLog("New DB Folder Sucessed!", string.Format("NewFolderID : {0} \t NewFolderName : {1}", folderID, FolderName));
                    #endregion
                }
                else if (operationID == 1)//修改
                {
                    #region Disk Operation
                    //NewFolderPath = System.IO.Path.Combine(CurentFolderInfo.ParentDiskPath, FolderName);//新的地址
                    //OldFolderPath = System.IO.Path.Combine(CurentFolderInfo.ParentDiskPath, CurentFolderInfo.FolderName);//旧的地址

                    //DirectoryInfo dir = new DirectoryInfo(OldFolderPath);//旧的地址
                    //dir.MoveTo(NewFolderPath);//新的地址  

                    //CurrentApp.WriteLog("Rename Disk Folder Sucessed!", string.Format("OldFolderPath : {0} \t  NewFolderPath : {1} ", OldFolderPath,NewFolderPath));
                    #endregion

                    #region DB Operation
                    Info = new FolderInfo();
                    Info.FolderID = CurentFolderInfo.FolderID;
                    Info.FolderName = FolderName;
                    Info.TreeParentID = CurentFolderInfo.TreeParentID;
                    Info.TreeParentName = CurentFolderInfo.TreeParentName;
                    Info.CreatorId = CurentFolderInfo.CreatorId;
                    Info.CreatorName = CurentFolderInfo.CreatorName;
                    Info.CreatorTime = CurentFolderInfo.CreatorTime;
                    Info.UserID1 = CurentFolderInfo.UserID1;
                    Info.UserID2 = CurentFolderInfo.UserID2;
                    Info.UserID3 = CurentFolderInfo.UserID3;
                    webRequest = new WebRequest();
                    webRequest.ListData.Clear();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)S3106Codes.FolderDBO;
                    webRequest.ListData.Add(operationID.ToString());
                    OperationReturn optReturn = XMLHelper.SeriallizeObject(Info);//文件夹树信息
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return false;
                    }
                    webRequest.ListData.Add(optReturn.Data.ToString());
                    NewFolderPath = System.IO.Path.Combine(CurentFolderInfo.ParentDiskPath, FolderName);//新的地址
                    OldFolderPath = System.IO.Path.Combine(CurentFolderInfo.ParentDiskPath, CurentFolderInfo.FolderName);//旧的地址
                    webRequest.ListData.Add(OldFolderPath);
                    webRequest.ListData.Add(NewFolderPath);

                    client = new Service31061Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31061"));
                    //client = new Service31061Client();
                    webReturn = client.UMPTreeOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        CurrentApp.WriteLog("Rename DB Folder Falied", string.Format("OldFolderName : {0} \t NewFolderName :{1}", CurentFolderInfo.FolderName, FolderName));
                        ShowException(CurrentApp.GetLanguageInfo("3106T00015", "Rename Folder Failed."));
                        return false;
                    }
                    CurrentApp.WriteLog("Rename DB Folder Sucessed!", string.Format("OldFolderName : {0} \t NewFolderName :{1}", CurentFolderInfo.FolderName, FolderName));

                    #endregion

                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return false;
            }
            finally
            {
                mListFolders.Clear();
                GetFolders(mCurentFolder, -1, FolderPath);
            }
            return true;
        }

        /// <summary>
        /// 删除
        /// 操作ID，0,删除文件夹及其目录下的文件; 1, 删除文件;  
        /// 主键
        /// </summary>
        private bool DeleteFolderDBO(int operationID, string KeyID, string FName)
        {
            string deletePath;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3106Codes.DeleteFolder;
                webRequest.ListData.Add(operationID.ToString());
                webRequest.ListData.Add(KeyID);
                deletePath = System.IO.Path.Combine(CurentFolderInfo.ParentDiskPath, CurentFolderInfo.FolderName);
                webRequest.ListData.Add(deletePath);
                Service31061Client client = new Service31061Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31061"));
                //Service31061Client client = new Service31061Client();
                WebReturn webReturn = client.UMPTreeOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    #region 写操作日志
                    string strLog = string.Format("{0} {1}", Utils.FormatOptLogString("FO3106003"), FName);
                    CurrentApp.WriteOperationLog(S3106Consts.OPT_Delete.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                    #endregion
                    CurrentApp.WriteLog("Delete DB Folder  Failed.", string.Format("FolderName :{0} \t Message :{1}", FName, webReturn.Message));
                    ShowException(string.Format("Can't Delete Folder"));
                    return false;
                }
                CurrentApp.WriteLog("Delete DB Folder  Sucessed!", string.Format("FolderName :{0}", FName));
                //if (operationID == 0)
                //{
                //    deletePath = System.IO.Path.Combine(CurentFolderInfo.ParentDiskPath, CurentFolderInfo.FolderName);
                //    Directory.Delete(deletePath, true);
                //    CurrentApp.WriteLog("Delete Disk Folder  Sucessed!", string.Format("FolderPath :{0}", deletePath));
                //}
            }
            catch (Exception ex)
            {
                #region 写操作日志
                string strLog = string.Format("{0} {1}",  Utils.FormatOptLogString("FO3106003"), FName);
                CurrentApp.WriteOperationLog(S3106Consts.OPT_Delete.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                #endregion
                ShowException(ex.Message);
                return false;
            }
            finally
            {
                mListFolders.Clear();
                GetFolders(mCurentFolder, -1, FolderPath);
            }
            #region 写操作日志
            string strLog1 = string.Format("{0} {1}", Utils.FormatOptLogString("FO3106003"), FName);
            CurrentApp.WriteOperationLog(S3106Consts.OPT_Delete.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog1);
            #endregion
            return true;
        }

        /// <summary>
        /// 修改权限
        /// </summary>
        public bool AllotPermission(FolderInfo Info)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3106Codes.AllotAgent;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(Info);//文件夹信息
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                Service31061Client client = new Service31061Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31061"));
                //Service31061Client client = new Service31061Client();
                WebReturn webReturn = client.UMPTreeOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    CurrentApp.WriteLog("Allot Folder Failed.", string.Format("FolderName : {0} \t  Message :{1}", Info.FolderName, webReturn.Message));
                    #region 写操作日志
                    string strLog = string.Format("{0} {1}", Utils.FormatOptLogString("FO3106004"), Info.FolderName);
                    CurrentApp.WriteOperationLog(S3106Consts.OPT_Allot.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                    #endregion
                    ShowException(string.Format("Allot Permission Failed."));
                    return false;
                }
                mListFolders.Clear();
                GetFolders(mCurentFolder, -1, FolderPath);
            }
            catch (Exception ex)
            {
                #region 写操作日志
                string strLog = string.Format("{0} {1}", Utils.FormatOptLogString("FO3106004"), Info.FolderName);
                CurrentApp.WriteOperationLog(S3106Consts.OPT_Allot.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                #endregion
                ShowException(ex.Message);
                return false;
            }
            #region 写操作日志
            string strLog1 = string.Format("{0} {1}{2}", Utils.FormatOptLogString("FO3106004"), Info.FolderName, Info.UserID1 + Info.UserID2 + Info.UserID3);
            CurrentApp.WriteOperationLog(S3106Consts.OPT_Allot.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog1);
            #endregion
            return true;
        }

        #endregion

        /*文件右键事件
        #region 文件右键事件
        /// <summary>
        /// 绑定右键事件
        /// </summary>
        private void BindCommands()
        {
            CommandBindings.Add(new CommandBinding(ListViewContenMenuCommands.DownLoadCommand,
                LDownLoadCommand_Click,
                (s, e) => e.CanExecute = true));
            CommandBindings.Add(new CommandBinding(ListViewContenMenuCommands.DeleteCommand,
                LDeleteCommand_Click,
                (s, e) => e.CanExecute = true));
            CommandBindings.Add(new CommandBinding(ListViewContenMenuCommands.RenameCommand,
                LRenameCommand_Click,
                (s, e) => e.CanExecute = true));
            CommandBindings.Add(new CommandBinding(ListViewContenMenuCommands.MoveCommand,
                LMoveCommand_Click,
                (s, e) => e.CanExecute = true));
        }

        private void LDownLoadCommand_Click(object sender, ExecutedRoutedEventArgs e)
        {
            
        }

        private void LDeleteCommand_Click(object sender, ExecutedRoutedEventArgs e)
        {
            
        }

        private void LRenameCommand_Click(object sender, ExecutedRoutedEventArgs e)
        {
            
        }

        private void LMoveCommand_Click(object sender, ExecutedRoutedEventArgs e)
        {
            
        }




        #endregion
         * */

        #region 文件右键功能逻辑



        #endregion

        #region 初始化 & 全局消息

        protected override void Init()
        {
            try
            {
                PageName = "Tutorial Repertory";
                StylePath = "UMPS3106/MainPageStyle.xmal";
                base.Init();

                CurrentApp.SendLoadedMessage();
                ChangeLanguage();

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


        #endregion

        #region 样式&语言
        public override void ChangeTheme()
        {
            base.ChangeTheme();

            bool bPage = false;
            if (AppServerInfo != null)
            {
                //优先从服务器上加载资源文件
                try
                {
                    string uri = string.Format("{0}://{1}:{2}/Themes/{3}/{4}",
                        AppServerInfo.Protocol,
                        AppServerInfo.Address,
                        AppServerInfo.Port,
                        ThemeInfo.Name
                        , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                    bPage = true;
                }
                catch (Exception)
                {
                    //ShowException("1" + ex.Message);
                }
            }
            if (!bPage)
            {
                //如果通过Url没有加载成功，就从已经编译到程序集的默认资源加载
                try
                {
                    string uri = string.Format("/UMPS3106;component/Themes/{0}/{1}",
                        "Default"
                        , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                }
                catch (Exception ex)
                {
                    //ShowException("2" + ex.Message);
                }
            }

            //固定资源(有些资源包含转换器，命令等自定义类型，
            //这些资源不能通过url来动态加载，他只能固定的编译到程序集中
            try
            {
                string uri = string.Format("/UMPS3106;component/Themes/Default/UMPS3106/AvalonDock.xaml");
                ResourceDictionary resource = new ResourceDictionary();
                resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                Resources.MergedDictionaries.Add(resource);
            }
            catch (Exception ex)
            {
                //ShowException("3" + ex.Message);
            }
            try
            {
                string uri = string.Format("/UMPS3106;component/Themes/Default/UMPS3106/MainPageStyle.xaml");
                ResourceDictionary resource = new ResourceDictionary();
                resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                Resources.MergedDictionaries.Add(resource);
            }
            catch (Exception ex)
            {
                //ShowException("4" + ex.Message);
            }
        }


        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                CurrentApp.AppTitle = CurrentApp.GetLanguageInfo(string.Format("FO{0}", CurrentApp.ModuleID),
                    "Tutorial Repertory");
                InitDocumentColumns();
                //mListFolders.Clear();
                //GetFolders(mCurentFolder, -1, FolderPath);
                PanelLearnDocument.Title = CurrentApp.GetLanguageInfo("3106T00002", "Learn Document");
                PanelPlayBox.Title = CurrentApp.GetLanguageInfo("3106T00034", "Play Box");
            }
            catch (Exception ex)
            {
            }

        }
        #endregion

        #region Laos
        private void LaosLinkSetting()
        {
            try
            {
                string xmlFileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), string.Format("UMP\\UMPS3106\\LaosLinkSetting.xml"));
                if (!File.Exists(xmlFileName))
                {
                    if (DownLaosXml().Equals(false))
                    {
                        if (File.Exists(xmlFileName)) { File.Delete(xmlFileName); }
                        mLaosUrl = string.Empty;
                        return;
                    }
                }
                XmlDocument laosXml = new XmlDocument();
                laosXml.Load(xmlFileName);
                //IsLinkToUrl=1 启用
                if (laosXml.SelectSingleNode(string.Format("root/Setting/IsLinkToUrl")).InnerText == "1")
                {
                    mLaosUrl = string.Format("{0}/{1}", laosXml.SelectSingleNode(string.Format("root/Setting/LaosURL")).InnerText, laosXml.SelectSingleNode(string.Format("root/Setting/Usersuffix")).InnerText);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private bool DownLaosXml()
        {
            System.Net.HttpWebRequest LHttpWebRequest = null;
            Stream LStreamResponse = null;
            System.IO.FileStream LStreamDownXml = null;
            string localPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), string.Format("UMP\\UMPS3106\\LaosLinkSetting.xml"));
            string ServerPath = string.Format("http://{0}:{1}/Components/LinkToLaosURL/LaosLinkSetting.xml", CurrentApp.Session.AppServerInfo.Address, CurrentApp.Session.AppServerInfo.Port - 1);
            //string path = string.Format("http://192.168.4.166:8081/Components/LinkToLaosURL/LaosLinkSetting.xml");
            try
            {
                LStreamDownXml = new FileStream(localPath, FileMode.Create);
                LHttpWebRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(ServerPath);
                long LContenctLength = LHttpWebRequest.GetResponse().ContentLength;
                LHttpWebRequest.AddRange(0);
                LStreamResponse = LHttpWebRequest.GetResponse().GetResponseStream();

                byte[] LbyteRead = new byte[1024];
                int LIntReadedSize = LStreamResponse.Read(LbyteRead, 0, 1024);
                while (LIntReadedSize > 0)
                {
                    LStreamDownXml.Write(LbyteRead, 0, LIntReadedSize);
                    LIntReadedSize = LStreamResponse.Read(LbyteRead, 0, 1024);
                }
                LStreamDownXml.Close(); LStreamDownXml.Dispose();
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("DownLoad Xml Failed.", string.Format("LocalPath : {0} \t   ServerPath :{1} \t  Message :{2}", localPath, ServerPath, ex.Message));
                return false;
            }
            finally
            {
                if (LHttpWebRequest != null) { LHttpWebRequest.Abort(); }
                if (LStreamResponse != null) { LStreamResponse.Close(); LStreamResponse.Dispose(); }
                if (LStreamDownXml != null) { LStreamDownXml.Close(); LStreamDownXml.Dispose(); }
            }
            return true;
        }

        private void LaosLinkCommand_Click(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var temp = e.Parameter as FilesItemInfo;

                System.Diagnostics.Process ie = new System.Diagnostics.Process();
                ie.StartInfo.FileName = "IEXPLORE.EXE";
                ie.StartInfo.Arguments = string.Format(@"{0}BID={1}&UID={2}", mLaosUrl, temp.FileName, CurrentApp.Session.UserInfo.Account);
                ie.Start();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion

        #region 老挝需求，添加教材浏览记录

        private void BrowseHistoryCommand_Click(object sender, ExecutedRoutedEventArgs e)
        {
            var fileInfo = e.Parameter as FilesItemInfo;
            if (fileInfo != null)
            {
                PopupPanel.Title = CurrentApp.GetLanguageInfo(" ", "Browse History");
                BrowseHistory browsePage = new BrowseHistory();
                browsePage.CurrentApp = CurrentApp;
                browsePage.ParentPage = this;
                browsePage.mFileInto = fileInfo;
                PopupPanel.Content = browsePage;
                PopupPanel.IsOpen = true;
            }
        }

        private void BrowseHistory(FilesItemInfo bookInfo)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3106Codes.WriteBrowseHistory;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(bookInfo);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserName);
                //Service31061Client client = new Service31061Client();
                Service31061Client client = new Service31061Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31061"));
                WebReturn webReturn = client.UMPTreeOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion

        public void OpenPasswordPanel(UMPUserControl content)
        {
            try
            {
                PopupPanel.Content = content;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        protected override void OnAppEvent(WebRequest webRequest)
        {
            base.OnAppEvent(webRequest);
            var code = webRequest.Code;
            switch (code)
            {
                case (int)RequestCode.ACPageHeadLeftPanel:
                    if (GridLeft.Width.Value == 0)
                    {
                        GridLeft.Width = new GridLength(200);
                    }
                    else
                    {
                        GridLeft.Width = new GridLength(0);
                    }
                    break;
            }
        }
    }
}
