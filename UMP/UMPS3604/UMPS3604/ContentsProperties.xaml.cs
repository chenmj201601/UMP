using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Common3604;
using UMPS3604.Models;
using UMPS3604.Wcf36041;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS3604
{
    /// <summary>
    /// Interaction logic for ContentsProperties.xaml
    /// </summary>
    public partial class ContentsProperties
    {
        public MaterialLibraryView ParentPage;
        private readonly ContentsTree _mContentsTree;
        private readonly int _mOptContentsInfo;
        private ContentsParam _mContentsParam;

        public ContentsProperties()
        {
            _mContentsTree = new ContentsTree();
            _mContentsTree = S3604App.GContentsTree;
            _mOptContentsInfo = S3604App.GiOptContentsInfo;
            _mContentsParam = new ContentsParam();
            InitializeComponent();
            Loaded += UCCustomSetting_Loaded;
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            RbutOneContentsName.IsChecked = true;
            if (_mOptContentsInfo == (int)S3604Consts.OPT_Change)
            {
                RbutMoreContentsName.IsEnabled = false;
                TxtOneContentsName.Text = _mContentsTree.StrNodeName;
                RbutOneContentsName.Content = CurrentApp.GetLanguageInfo("3604T00010", "Change Contents");
            }
            else
            {
                RbutOneContentsName.Content = CurrentApp.GetLanguageInfo("3604T00012", "One Contents Name");
                TbNote.Visibility = Visibility.Visible;
                TbNote.Text = CurrentApp.GetLanguageInfo("3604T00014", "Note:") + "\r\n" +
                          CurrentApp.GetLanguageInfo("3604T00015", "1.One Contents: Name") + "\r\n" +
                          CurrentApp.GetLanguageInfo("3604T00016", "2.More Contents: Name1\\Name2\\Name3");
            }
            RbutMoreContentsName.Content = CurrentApp.GetLanguageInfo("3604T00013", "More Contents Name");

            BtnOk.Content = CurrentApp.GetLanguageInfo("3604T00017", "OK");
            BtnCancel.Content = CurrentApp.GetLanguageInfo("3604T00018", "Cancel");
        }

        void UCCustomSetting_Loaded(object sender, RoutedEventArgs e)
        {
            ChangeLanguage();
            SetRbutContents();
        }

        private void SetRbutContents()
        {
            if (RbutOneContentsName.IsChecked == true)
            {
                TxtOneContentsName.IsReadOnly = false;
                TxtOneContentsName.Background = Brushes.White;
                TxtMoreContentsName.IsReadOnly = true;
                TxtMoreContentsName.Background = Brushes.LightGray;
            }
            else
            {
                TxtMoreContentsName.IsReadOnly = false;
                TxtMoreContentsName.Background = Brushes.White;
                TxtOneContentsName.IsReadOnly = true;
                TxtOneContentsName.Background = Brushes.LightGray;
            }
        }

        private bool ChangeContents()
        {
            if (!string.IsNullOrWhiteSpace(TxtOneContentsName.Text))
            {
                if (string.Equals(TxtOneContentsName.Text, _mContentsTree.StrParentNodeName))
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3604T00019",
                           "Subfolder name can not be the same as the parent"));
                    return false;
                }
                if (_mContentsTree.LstChildInfos.Any(param => string.Equals(TxtOneContentsName.Text, param.StrNodeName)))
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3604T00019",
                        "Subfolder name can not be the same as the parent"));
                    return false;
                }
                if (_mContentsTree.LstNodeInfos.Any(param => string.Equals(TxtOneContentsName.Text, param.StrNodeName)))
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3604T00023",
                        "Subfolder name can not be the same as the parent"));
                    return false;
                }
            }
            else
            {
                ShowInformation(CurrentApp.GetLanguageInfo("3604T00021", "Please Input ContentsName"));
                return false;
            }

            _mContentsParam = new ContentsParam();
            _mContentsParam.LongNodeId = _mContentsTree.LongNodeId;
            _mContentsParam.StrNodeName = TxtOneContentsName.Text;
            _mContentsTree.StrNodeName = TxtOneContentsName.Text;
            
            WebRequest webRequest;
            Service36041Client client;
            WebReturn webReturn;
            webRequest = new WebRequest();
            webRequest.Session = CurrentApp.Session;
            webRequest.Code = (int)S3604Codes.OptChangeContents;
            OperationReturn optReturn = XMLHelper.SeriallizeObject(_mContentsParam);
            if (!optReturn.Result)
            {
                ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code,
                    optReturn.Message));
                return false;
            }
            webRequest.ListData.Add(optReturn.Data.ToString());
//             client = new Service36041Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
//                 WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36041"));
            client = new Service36041Client();
            webReturn = client.UmpTaskOperation(webRequest);
            client.Close();
            string strLog;
            if (!webReturn.Result)
            {
                #region 写操作日志
                strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3604T00010"), Utils.FormatOptLogString("3604T00024"), webReturn.Message);
                CurrentApp.WriteOperationLog(S3604Consts.OPT_Add.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                #endregion
                ShowException(string.Format("{0}: {1}",
                    CurrentApp.GetLanguageInfo("3604T00024", "Insert data failed"), webReturn.Message));
                return false;
            }
            #region 写操作日志
            strLog = string.Format("{0}", Utils.FormatOptLogString("3604T00010"));
            CurrentApp.WriteOperationLog(S3604Consts.OPT_Add.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
            #endregion
            CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3604T00010", "Change Contents!"));
            CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3604T00025", "Change Success!"));
            ParentPage.RefreshTree(_mContentsTree);
            return true;
        }

        private bool CreateContents()
        {
            WebRequest webRequest;
            Service36041Client client;
            WebReturn webReturn;
            string[] strContents = new string[10];
            long[] strResultId = new long[10];
            var lstContentsTree = new List<ContentsTree>();

            if (RbutOneContentsName.IsChecked == true)
            {
                if (!string.IsNullOrWhiteSpace(TxtOneContentsName.Text))
                {
                    strContents[0] = TxtOneContentsName.Text;
                    if (strContents[0] == _mContentsTree.StrNodeName)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3604T00019",
                            "Subfolder name can not be the same as the parent"));
                        return false;
                    }
                    foreach (var param in _mContentsTree.LstChildInfos)
                    {
                        if (string.Equals(TxtOneContentsName.Text, param.StrNodeName))
                        {
                            ShowInformation(CurrentApp.GetLanguageInfo("3604T00020",
                                "Contents has been created!"));
                            return false;
                        }
                    }
                }
                else
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3604T00021", "Please Input ContentsName"));
                    return false;
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(TxtMoreContentsName.Text))
                {
                    strContents = TxtMoreContentsName.Text.Split(new char[] { '\\' });
                    if (strContents[0] == S3604App.GContentsTree.StrNodeName)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3604T00019",
                            "Subfolder name can not be the same as the parent"));
                        return false;
                    }
                    for (int i = 0; i < strContents.Length - 1; i++)
                    {
                        if (strContents[i] == strContents[i + 1])
                        {
                            ShowInformation(CurrentApp.GetLanguageInfo("3604T00019",
                                "Subfolder name can not be the same as the parent"));
                            return false;
                        }
                        if (i + 2 > 10)
                        {
                            ShowInformation(CurrentApp.GetLanguageInfo("3604T00022",
                                "Create cannot exceed 10 more Contents"));
                            return false;
                        }
                    }
                }
                else
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3604T00021", "Please Input ContentsName"));
                    return false;
                }
            }

            for (int i = 0; i < strContents.Length; i++)
            {
                if (!S3604App.GQueryModify)
                {
                    //生成新的查询配置表主键
                    webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)RequestCode.WSGetSerialID;
                    webRequest.ListData.Add("36");
                    webRequest.ListData.Add("3604");
                    webRequest.ListData.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
//                     client = new Service36041Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
//                         WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36041"));
                    client = new Service36041Client();
                    webReturn = client.UmpTaskOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                        return false;
                    string strNewResultId = webReturn.Data;
                    if (string.IsNullOrEmpty(strNewResultId))
                        return false;
                    strResultId[i] = Convert.ToInt64(strNewResultId);
                }
            }

            for (int i = 0; i < strContents.Length; i++)
            {
                var contentsTree = new ContentsTree();
                if (string.IsNullOrEmpty(strContents[i]))
                {
                    break;
                }
                if (i == 0)
                {
                    _mContentsParam.LongParentNodeId = _mContentsTree.LongNodeId;
                    _mContentsParam.StrParentNodeName = _mContentsTree.StrNodeName;
                    contentsTree.LongParentNodeId = _mContentsTree.LongNodeId;
                }
                else
                {
                    _mContentsParam.LongParentNodeId = strResultId[i - 1];
                    _mContentsParam.StrParentNodeName = strContents[i - 1];
                    contentsTree.LongParentNodeId = strResultId[i - 1];
                }
                _mContentsParam.StrNodeName = strContents[i];
                _mContentsParam.LongNodeId = strResultId[i];
                _mContentsParam.StrDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                contentsTree.LongNodeId = strResultId[i];
                contentsTree.StrNodeName = strContents[i];

                webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3604Codes.OptCreateContents;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(_mContentsParam);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code,
                        optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                //client = new Service36041Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                //    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36041"));
                client = new Service36041Client();
                webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                string strLog;
                if (!webReturn.Result)
                {
                    #region 写操作日志
                    strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3604T00003"), Utils.FormatOptLogString("3604T00026"), webReturn.Message);
                    CurrentApp.WriteOperationLog(S3604Consts.OPT_Add.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                    #endregion

                    ShowException(string.Format("{0} : {1}",
                        CurrentApp.GetLanguageInfo("3604T00026", "Insert data failed"), webReturn.Message));
                    return false;
                }
                #region 写操作日志
                strLog = string.Format("{0}", Utils.FormatOptLogString("3604T00003"));
                CurrentApp.WriteOperationLog(S3604Consts.OPT_Add.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                #endregion
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3604T00003", "Add Contents!"));
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3604T00027", "Add Success!"));
                lstContentsTree.Add(contentsTree);
            }

            ParentPage.RefreshTree(lstContentsTree);
            return true;
        }

        private void ContentsName_OnClick(object sender, RoutedEventArgs e)
        {
            SetRbutContents();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool bEnable = false;
                var parent = Parent as PopupPanel;
                if (parent != null)
                    bEnable = _mOptContentsInfo == (int) S3604Consts.OPT_Change ? ChangeContents() : CreateContents();

                if (parent != null) parent.IsOpen = bEnable != true;
            }
            catch (Exception ex)
            {
                ShowInformation(ex.Message);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.IsOpen = false;
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}
