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
using UMPS3102.Models;
using UMPS3102.Wcf31021;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31021;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS3102
{
    /// <summary>
    /// UCLibraryTree.xaml 的交互逻辑
    /// </summary>
    public partial class UCLibraryTree
    {

        public QMMainView MainPage;
        public RecordInfoItem SelectRecordInfoItem;
        public ObjectItem mRootItem;
        public ObjectItem mRootItem_Record;
        private BackgroundWorker mWorker;
        private List<LibraryContent> mListLibraryContent;

        private ObjectItem SelectedFolder;

        public UCLibraryTree()
        {
            mRootItem = new ObjectItem();
            SelectedFolder = new ObjectItem();
            mRootItem_Record = new ObjectItem();
            mListLibraryContent = new List<LibraryContent>();
            InitializeComponent();
            Loaded += Library_Loaded;
        }

        private void Library_Loaded(object sender, RoutedEventArgs e)
        {
            RecordingName.Content = SelectRecordInfoItem.RecordReference + ".wav";
            LibraryTree.ItemsSource = mRootItem.Children;
            mRootItem.Children.Clear();
            mWorker = new BackgroundWorker();
            mWorker.DoWork += (s, de) =>
            {
                InitLibraryFolder();
            };
            mWorker.RunWorkerCompleted += (s, re) =>
            {
                mWorker.Dispose();
                mRootItem.IsChecked = false;
                if (mRootItem.Children.Count > 0)
                {
                    mRootItem.Children[0].IsExpanded = true;
                }
            };
            mWorker.RunWorkerAsync();

            ChangeLanguage();
        }


        private void InitLibraryFolder()
        {
            InitLibraryFolder(mRootItem, "-1");
        }

        private void InitLibraryFolder(ObjectItem parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.GetLibraryFolder;
                //webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                Service31021Client client = new Service31021Client(
                     WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                     WebHelper.CreateEndpointAddress(
                         CurrentApp.Session.AppServerInfo,
                         "Service31021"));
                WebHelper.SetServiceClient(client);
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
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 3) { continue; }
                    string strFolderID = arrInfo[0];
                    string strParentFolderID = arrInfo[1];
                    string strFolderName = arrInfo[2];
                    string strParentFolderName = arrInfo[3];
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_ORG;
                    item.ObjID = Convert.ToInt64(strFolderID);
                    item.Name = strFolderName;
                    item.Data = strInfo;
                    item.Description = string.Format("{0}", strFolderName);
                    //如果为文件夹的话就在这里改其中的资源文件常量
                    if (strFolderID == ConstValue.ORG_ROOT.ToString())
                    {
                        item.Icon = "Images/rootorg.ico";
                    }
                    else
                    {
                        item.Icon = "Images/org.ico";
                    }
                    InitLibraryFolder(item, strFolderID);
                    //InitLibraryFolderContent(item, strFolderID);
                    AddChildObject(parentItem, item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitLibraryFolderContent(ObjectItem parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.GetLibraryFolderContent;
                //webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                Service31021Client client = new Service31021Client(
                     WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                     WebHelper.CreateEndpointAddress(
                         CurrentApp.Session.AppServerInfo,
                         "Service31021"));
                WebHelper.SetServiceClient(client);
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
                    OperationReturn optReturn = XMLHelper.DeserializeObject<LibraryContent>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    LibraryContent info = optReturn.Data as LibraryContent;
                    if (info == null)
                    {
                        ShowException(string.Format("Fail.\tLibraryContent is null"));
                        return;
                    }
                    mListLibraryContent.Add(info);
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_USER;
                    item.ObjID = Convert.ToInt64(info.BookID);
                    item.Name = info.BookName;
                    item.FullName = info.Path;
                    item.Data = info;
                    item.Description = string.Format("{0}({1})", item.Name, item.FullName);
                    item.Icon = "Images/inspector.ico";
                    AddChildObject(parentItem, item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #region 其他
        private void AddChildObject(ObjectItem parentItem, ObjectItem item)
        {
            Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));//这句话得理解下 
        }

        //写一个方法根据所选择的文件夹获得文件夹的路径  传入的初始path是SelectItem.Name
        private string GetPath(string path,ObjectItem SelectItem)
        {
            //item = LibraryTree.SelectedItem as ObjectItem;
            //path = SelectItem.Name;
            var itemParant = SelectItem.Parent as ObjectItem;
            if (itemParant != null)
            {
                path = itemParant.Name + @"\" + path;
                if (itemParant.Name == "BOOK")
                {
                    return path;
                }
                return GetPath(path,itemParant);
            }
            else 
            {
                return path;
            }

        }


        //写一个方法 然后将选中的录音 放到文件夹下面(首先我能够获得选中的录音,以及能够写的描述了)
        private bool AddRecordToLibrary()
        {
            try 
            {
                var tempitem = LibraryTree.SelectedItem as ObjectItem;
                if (SelectRecordInfoItem.MediaType != 1)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3102N051", "上传到教材库的不能有录屏"));
                    return false;
                }
                //文件夹路径
                if (tempitem == null)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3102N052", "必须选择一个文件夹"));
                    return false;
                }
                string path = GetPath(tempitem.Name, tempitem);

                //教材库文件夹主键 ,T_31_058_00000.C001
                string dirID = tempitem.ObjID.ToString();
                //if (string.IsNullOrWhiteSpace(dirID) || string.IsNullOrEmpty(dirID))
                //{
                //    ShowException("必须选择一个文件夹");
                //    return false;
                //}
                //教材名字  我这里就是录音ID 而不是录音流水号
                string learningName = SelectRecordInfoItem.SerialID.ToString();
                //录音流水号存放在教材库的路径
                string recordPath = path + @"\" + SelectRecordInfoItem.RecordReference + ".wav";
                //描述
                string tempDiscription =DiscriptionText.Text.ToString();
                if (tempDiscription.IndexOf("'") != -1)
                {
                    string tempa = tempDiscription.Replace("'", "''");
                    tempDiscription = tempa;
                }
                if (string.IsNullOrWhiteSpace(tempDiscription)||string.IsNullOrEmpty(tempDiscription))
                {
                    ShowException(CurrentApp.GetLanguageInfo("3102N053", "描述不能为空或者为空格"));
                    return false;
                }
                //该录音是否加密
                string isEncrytp = SelectRecordInfoItem.EncryptFlag=="0"?"0":"1"; 

                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.InsertLearningToLibrary;
                webRequest.ListData.Add(dirID);
                webRequest.ListData.Add(learningName);
                webRequest.ListData.Add(recordPath);
                webRequest.ListData.Add(tempDiscription);
                webRequest.ListData.Add(isEncrytp);
                //Service31021Client client = new Service31021Client();
                Service31021Client client = new Service31021Client(
                     WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                     WebHelper.CreateEndpointAddress(
                         CurrentApp.Session.AppServerInfo,
                         "Service31021"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                ShowException(ex.ToString());
                return false;
            }
        }
        #endregion

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (AddRecordToLibrary() == false)
            {
                //CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("3102N050", "加入失败"));
                return;
            }
            CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("3102N049", "加入成功"));
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
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

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.Title = CurrentApp.GetLanguageInfo("310211001", "录音加入教材库");
                    GroupBoxFolder.Header = CurrentApp.GetLanguageInfo("3102110011", "请选择文件夹");
                    Discription.Header = CurrentApp.GetLanguageInfo("3102110012", "描述");
                    BtnConfirm.Content = CurrentApp.GetLanguageInfo("31020", "Confirm");
                    BtnClose.Content = CurrentApp.GetLanguageInfo("31021", "Close");
                }

                //CreateDownloadColumns();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
    }
}
