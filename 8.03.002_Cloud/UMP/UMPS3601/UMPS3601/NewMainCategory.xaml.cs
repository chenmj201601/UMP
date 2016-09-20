using System;
using System.Windows;
using System.Windows.Media;
using Common3601;
using UMPS3601.Models;
using UMPS3601.Wcf36011;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS3601
{
    /// <summary>
    /// NewMainCategory.xaml 的交互逻辑
    /// </summary>
    public partial class NewMainCategory
    {
        public ExamProductionView ParentPage;
        public PapersCategoryParam _mPapersCategoryParam;
        private CCategoryTree _mCategoryTree = new CCategoryTree();
        
        public NewMainCategory()
        {
            _mPapersCategoryParam = new PapersCategoryParam();
            _mCategoryTree = S3601App.GCategoryTreeNode;
            InitializeComponent();
            RbutOneCategroyName.IsChecked = true;
            if (S3601App.GMessageSource == S3601Codes.MessageUpdateCategory)
            {
                RbutMoreCategroyName.IsEnabled = false;
                TxtOneCategroyName.Text = _mCategoryTree.StrName;
            }
            
            this.Loaded += UCCustomSetting_Loaded;
        }

        void UCCustomSetting_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            SetRbutCategroy();
            ChangeLanguage();
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();
            if (S3601App.GMessageSource == S3601Codes.MessageUpdateCategory)
            {
                RbutOneCategroyName.Content = CurrentApp.GetLanguageInfo("3601T00153", "Change Name");
            }
            else
            {
                RbutOneCategroyName.Content = CurrentApp.GetLanguageInfo("3601T00123", "One Categroy Name");
                TbNote.Visibility = Visibility.Visible;
                TbNote.Text = CurrentApp.GetLanguageInfo("3601T00125", "Note:") + "\r\n" +
                          CurrentApp.GetLanguageInfo("3601T00126", "1.One Categroy: Name") + "\r\n" +
                          CurrentApp.GetLanguageInfo("3601T00127", "2.More Categroy: Name1\\Name2\\Name3");
            }
            RbutMoreCategroyName.Content = CurrentApp.GetLanguageInfo("3601T00124", "More Categroy Name");
            
            BtnOk.Content = CurrentApp.GetLanguageInfo("3601T00068", "OK");
            BtnCancel.Content = CurrentApp.GetLanguageInfo("3601T00069", "Cancel");
        }


        void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool bEnable = false;
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    if (S3601App.GMessageSource == S3601Codes.MessageUpdateCategory)
                    {
                       bEnable = UpdateCateGory();
                    }
                    else
                    {
                       bEnable = CreateCategory();
                    }

                    parent.IsOpen = bEnable != true;
                }

            }
            catch (Exception ex)
            {
                ShowInformation(ex.Message);
            }
        }

        void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var parent = Parent as PopupPanel;
                if(parent != null)
                {
                    parent.IsOpen = false;
                }
            }
            catch
            {

            }
        }

        private void CategroyName_OnClick(object sender, RoutedEventArgs e)
        {
            SetRbutCategroy();
        }

        private void SetRbutCategroy()
        {
            if (RbutOneCategroyName.IsChecked == true)
            {
                TxtOneCategroyName.IsReadOnly = false;
                TxtOneCategroyName.Background = Brushes.White;
                TxtMoreCategroyName.IsReadOnly = true;
                TxtMoreCategroyName.Background = Brushes.LightGray;
            }
            else
            {
                TxtMoreCategroyName.IsReadOnly = false;
                TxtMoreCategroyName.Background = Brushes.White;
                TxtOneCategroyName.IsReadOnly = true;
                TxtOneCategroyName.Background = Brushes.LightGray;
            }
        }

        private bool UpdateCateGory()
        {
            if (!string.IsNullOrWhiteSpace(TxtOneCategroyName.Text))
            {
                if (string.Equals(TxtOneCategroyName.Text, _mCategoryTree.StrParentNodeName))
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3601T00128",
                           "Subfolder name can not be the same as the parent"));
                    return false;
                }
                foreach (var categoryTree in _mCategoryTree.LstChildInfos)
                {
                    if ( string.Equals(TxtOneCategroyName.Text, categoryTree.StrName))
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3601T00128",
                            "Subfolder name can not be the same as the parent"));
                        return false;
                    }
                }
                foreach (var categoryTree in _mCategoryTree.LstNodeInfos)
                {
                    if (string.Equals(TxtOneCategroyName.Text, categoryTree.StrName))
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3601T00158",
                            "Subfolder name can not be the same as the parent"));
                        return false;
                    }
                }
            }
            else
            {
                ShowInformation(CurrentApp.GetLanguageInfo("3601T00129", "Please Input CategroyName"));
                return false;
            }

            _mPapersCategoryParam = new PapersCategoryParam();
            _mPapersCategoryParam.LongNum = _mCategoryTree.LongNum;
            _mCategoryTree.StrName = TxtOneCategroyName.Text;
            _mPapersCategoryParam.StrName = TxtOneCategroyName.Text;

            WebRequest webRequest;
            Service36011Client client;
            WebReturn webReturn;
            webRequest = new WebRequest();
            webRequest.Session = CurrentApp.Session;
            webRequest.Code = (int)S3601Codes.OperationUpdateCategory;
            OperationReturn optReturn = XMLHelper.SeriallizeObject(_mPapersCategoryParam);
            if (!optReturn.Result)
            {
                ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code,
                    optReturn.Message));
                return false;
            }
            webRequest.ListData.Add(optReturn.Data.ToString());
            client = new Service36011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36011"));
            //client = new Service36011Client();
            webReturn = client.UmpTaskOperation(webRequest);
            client.Close();
            string strLog;
            if (!webReturn.Result)
            {
                #region 写操作日志
                strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3601T00153"), Utils.FormatOptLogString("3601T00186"), webReturn.Message);
                CurrentApp.WriteOperationLog(S3601Consts.OPT_Add.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                #endregion
                ShowException(string.Format("{0}: {1}",
                    CurrentApp.GetLanguageInfo("3601T00015", "Insert data failed"), webReturn.Message));
                return false;
            }
            #region 写操作日志
            strLog = string.Format("{0}", Utils.FormatOptLogString("3601T00153"));
            CurrentApp.WriteOperationLog(S3601Consts.OPT_Add.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
            #endregion
            CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3601T00153", "Change Category!"));
            CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3601T00187", "Change Success!"));

            ParentPage.RefreshTree(_mCategoryTree);
            return true;
        }

        private bool CreateCategory()
        {
            WebRequest webRequest;
            Service36011Client client;
            WebReturn webReturn;
            string[] strCategroy = new string[10];
            long[] strResultId = new long[10];
            CCategoryTree cCategoryTree = new CCategoryTree();

            if (RbutOneCategroyName.IsChecked == true)
            {
                if (!string.IsNullOrWhiteSpace(TxtOneCategroyName.Text))
                {
                    strCategroy[0] = TxtOneCategroyName.Text;
                    if (strCategroy[0] == S3601App.GCategoryTree.StrName)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3601T00128",
                            "Subfolder name can not be the same as the parent"));
                        return false;
                    }
                    foreach (var categoryTree in _mCategoryTree.LstChildInfos)
                    {
                        if (string.Equals(TxtOneCategroyName.Text, categoryTree.StrName))
                        {
                            ShowInformation(CurrentApp.GetLanguageInfo("3601T00157",
                                "Category has been created!"));
                            return false;
                        }
                    }  
                }
                else
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3601T00129", "Please Input CategroyName"));
                    return false;
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(TxtMoreCategroyName.Text))
                {
                    strCategroy = TxtMoreCategroyName.Text.Split(new char[] {'\\'});
                    if (strCategroy[0] == S3601App.GCategoryTree.StrName)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3601T00128",
                            "Subfolder name can not be the same as the parent"));
                        return false;
                    }
                    for (int i = 0; i < strCategroy.Length - 1; i++)
                    {
                        if (strCategroy[i] == strCategroy[i + 1])
                        {
                            ShowInformation(CurrentApp.GetLanguageInfo("3601T00128",
                                "Subfolder name can not be the same as the parent"));
                            return false;
                        }
                        if (i + 2 > 10)
                        {
                            ShowInformation(CurrentApp.GetLanguageInfo("3601T00130",
                                "Create cannot exceed 10 more category"));
                            return false;
                        }
                    }
                }
                else
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3601T00129", "Please Input CategroyName"));
                    return false;
                }
            }

            for (int i = 0; i < strCategroy.Length; i++)
            {
                if (!S3601App.GQueryModify)
                {
                    //生成新的查询配置表主键
                    webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int) RequestCode.WSGetSerialID;
                    webRequest.ListData.Add("36");
                    webRequest.ListData.Add("3601");
                    webRequest.ListData.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
                    client = new Service36011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                        WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36011"));
                    //client = new Service36011Client();
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

            for (int i = 0; i < strCategroy.Length; i++)
            {
                if (string.IsNullOrEmpty(strCategroy[i]))
                {
                    break;
                }
                if (i == 0)
                {
                    _mPapersCategoryParam.LongParentNodeId = S3601App.GCategoryTree.LongNum;
                    _mPapersCategoryParam.StrParentNodeName = S3601App.GCategoryTree.StrName;
                    cCategoryTree.LongParentNodeId = S3601App.GCategoryTree.LongNum;
                }
                else
                {
                    _mPapersCategoryParam.LongParentNodeId = strResultId[i - 1];
                    _mPapersCategoryParam.StrParentNodeName = strCategroy[i - 1];
                    cCategoryTree.LongParentNodeId = strResultId[i - 1];
                }
                _mPapersCategoryParam.StrName = strCategroy[i];
                _mPapersCategoryParam.LongNum = strResultId[i];
                _mPapersCategoryParam.StrDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                cCategoryTree.LongNum = strResultId[i];

                webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int) S3601Codes.OperationCreateCategory;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(_mPapersCategoryParam);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code,
                        optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                client = new Service36011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36011"));
                //client = new Service36011Client();
                webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                string strLog;
                if (!webReturn.Result)
                {
                    #region 写操作日志
                    strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3601T00013"), Utils.FormatOptLogString("3601T00184"), webReturn.Message);
                    CurrentApp.WriteOperationLog(S3601Consts.OPT_Add.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                    #endregion

                    ShowException(CurrentApp.GetLanguageInfo("3601T00015", "Insert data failed"));
                    return false;
                }
                #region 写操作日志
                strLog = string.Format("{0}", Utils.FormatOptLogString("3601T00013"));
                CurrentApp.WriteOperationLog(S3601Consts.OPT_Add.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                #endregion
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3601T00013", "Add Category!"));
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3601T00185", "Add Success!"));
            }

            ParentPage.RefreshTree(cCategoryTree);
            return true;
        }
    }
}
