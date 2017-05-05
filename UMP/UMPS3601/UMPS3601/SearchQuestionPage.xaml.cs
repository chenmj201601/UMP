using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
    /// SearchQuestionPage.xaml 的交互逻辑
    /// </summary>
    public partial class SearchQuestionPage
    {
        public ExamProductionView ParentPage;
        private CCategoryTree _mcCategoryTree;
        private List<CQuestionsParam> _mlqQuestionsParams;
        private List<long> _mlstCategoryNum = new List<long>();
        private CCategoryTree _mCategoryNode;
        private List<CCategoryTree> _mListCategoryTreeParam;
        private List<CCategoryTree> _mlstSearchCategoryNode;

        public SearchQuestionPage()
        {
            _mcCategoryTree = new CCategoryTree();
            _mlqQuestionsParams = new List<CQuestionsParam>();
            _mCategoryNode = new CCategoryTree();
            _mListCategoryTreeParam = S3601App.GLstCCategoryTrees;
            _mlstSearchCategoryNode = new List<CCategoryTree>();
            InitializeComponent();
            Loaded += UCCustomSetting_Loaded;
            CategoryTree.SelectedItemChanged += OrgCategoryTree_SelectedItemChanged;
        }

        void UCCustomSetting_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Init();
        }

        private void Init()
        {
            ChangeLanguage();
            ButSearchCategroy.Children.Clear();
            Button btn;
            OperationInfo opt;
            btn = new Button();
            btn.Click += SearchCategory_Click;
            opt = new OperationInfo();
            opt.Icon = "Images/search.png";
            TbSearchCategroy.Text = CurrentApp.GetLanguageInfo("3601T00154", "Search Category");
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            ButSearchCategroy.Children.Add(btn);
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();
            SearchCategoryName.Text = CurrentApp.GetLanguageInfo("3601T00116", "Choice Category");
            SearchInfoName.Text = CurrentApp.GetLanguageInfo("3601T00117","Question Infornation");
            TypeName.Text = CurrentApp.GetLanguageInfo("3601T00028", "Question Type");
            UseNumName.Text = CurrentApp.GetLanguageInfo("3601T00027", "used Number");
            //ToName.Text = CurrentApp.GetLanguageInfo("3601T00119", "To");
            TrueOrFalseName.Content = CurrentApp.GetLanguageInfo("3601T00098", "True Or False");
            SingleChioceName.Content = CurrentApp.GetLanguageInfo("3601T00097", "Single Chioce");
            MultipleChoiceName.Content = CurrentApp.GetLanguageInfo("3601T00099", "Multiple Choice");
            OkButton.Content = CurrentApp.GetLanguageInfo("3601T00068", "OK");
            BtnCancel.Content = CurrentApp.GetLanguageInfo("3601T00069", "Cancel");
            CreateTime.Text = CurrentApp.GetLanguageInfo("3601T00032", "Create Time");
            BoxItem1.Content = AccessoryType.PictureGif;
            BoxItem2.Content = AccessoryType.PictureJpeg;
            BoxItem3.Content = AccessoryType.PictureJpg;
            BoxItem4.Content = AccessoryType.PicturePng;
            BoxItem5.Content = AccessoryType.VoiceWav;
            ComboBoxName.SelectedIndex = -1;
            FromTimeName.Value = DateTime.Today;
            ToTimeNum.Value = DateTime.Now;
        }

        private void SearchCategory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if( string.IsNullOrEmpty( TxtSearchCategroy.Text) ) return;
                string strSql = string.Format("SELECT * FROM T_36_021_{0} WHERE C002 like '%{1}%'", CurrentApp.Session.RentInfo.Token, TxtSearchCategroy.Text);
                SearchCategoryTreeInfo(strSql);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void SearchCategoryTreeInfo(string strSql)
        {
            try
            {
                _mlstSearchCategoryNode = new List<CCategoryTree>();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3601Codes.OperationGetQuestionCategory;
                Service36011Client client = new Service36011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36011"));
                //var client = new Service36011Client();
                OperationReturn optReturn = XMLHelper.SeriallizeObject(strSql);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                WebReturn webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                    return;

                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<PapersCategoryParam>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    PapersCategoryParam param = optReturn.Data as PapersCategoryParam;
                    if (param == null)
                    {
                        ShowException(string.Format("Fail. queryItem is null"));
                        return;
                    }

                    CCategoryTree tempTree = new CCategoryTree();
                    tempTree.LongNum = param.LongNum;
                    tempTree.StrName = param.StrName;
                    tempTree.LongParentNodeId = param.LongParentNodeId;
                    tempTree.StrParentNodeName = param.StrParentNodeName;
                    tempTree.LongFounderId = param.LongFounderId;
                    tempTree.StrFounderName = param.StrFounderName;
                    tempTree.StrDateTime = param.StrDateTime;
                    _mlstSearchCategoryNode.Add(tempTree);
                }

                _mlstCategoryNum = new List<long>();
                foreach (var categoryTree in _mlstSearchCategoryNode)
                {
                    GetCategoryNum(categoryTree.LongNum);
                }

                var distinctNames = _mlstCategoryNum.Distinct();
                _mlstCategoryNum = new List<long>(distinctNames.ToList());

                _mCategoryNode.Children.Clear();
                CategoryTree.ItemsSource = _mCategoryNode.Children;
                InitCategoryTree(_mListCategoryTreeParam, 0, _mCategoryNode);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void GetCategoryNum(long longCategoryNum)
        {
            if (longCategoryNum == 0)
            {
                return;
            }
            long lNum = new long();
            foreach (var categoryTree in _mListCategoryTreeParam)
            {
                if (categoryTree.LongNum == longCategoryNum)
                {
                    lNum = categoryTree.LongParentNodeId;
                    GetCategoryNum(lNum);
                    _mlstCategoryNum.Add(lNum);
                    return;
                }
            }
            return;
        }

        public void InitCategoryTree(List<CCategoryTree> listPapersCategoryParam, long longParentNodeId, CCategoryTree categoryNodes)
        {
            CCategoryTree nodeTemp = new CCategoryTree();
            foreach (CCategoryTree param in listPapersCategoryParam)
            {
                if (param.LongParentNodeId == longParentNodeId)
                {
                    CCategoryTree tempNode = new CCategoryTree();
                    nodeTemp = GetCategoryNodeInfo(categoryNodes, param);
                    InitCategoryTree(listPapersCategoryParam, param.LongNum, nodeTemp);
                }
            }
        }

        public CCategoryTree GetCategoryNodeInfo(CCategoryTree parentInfo, CCategoryTree param)
        {
            CCategoryTree temp = new CCategoryTree();
            try
            {
                temp.StrName = param.StrName;
                temp.Icon = "/UMPS3601;component/Themes/Default/UMPS3601/Images/document.ico";
                temp.LongNum = param.LongNum;
                temp.StrName = param.LongParentNodeId == 0 ? CurrentApp.GetLanguageInfo("3601T00017", "Category") : param.StrName;
                temp.LongParentNodeId = param.LongParentNodeId;
                if (_mlstCategoryNum.Count <= 0)
                {
                    if (param.LongParentNodeId == 0) temp.IsExpanded = true;
                }
                else
                {
                    int iCount = 0;
                    foreach (var num in _mlstCategoryNum)
                    {
                        iCount++;
                        if (param.LongNum == num)
                        {
                            temp.IsExpanded = true;
                            if (iCount == _mlstCategoryNum.Count)
                                temp.IsChecked = true;
                        }
                    }
                    foreach (var categoryTree in _mlstSearchCategoryNode)
                    {
                        if (param.LongNum == categoryTree.LongNum)
                        {
                            temp.ChangeBrush = Brushes.Gold;
                        }
                    }
                }
                temp.StrParentNodeName = param.StrParentNodeName;
                temp.LongFounderId = param.LongFounderId;
                temp.StrFounderName = param.StrFounderName;
                temp.StrDateTime = param.StrDateTime;
                AddChildNode(parentInfo, temp);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
            return temp;
        }

        private void AddChildNode(CCategoryTree parentItem, CCategoryTree item)
        {
            Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
        }

        private void OrgCategoryTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                _mcCategoryTree = new CCategoryTree();
                CCategoryTree nodeInfo = CategoryTree.SelectedItem as CCategoryTree;
                if (nodeInfo == null) { return; }
                _mcCategoryTree = nodeInfo;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                if (!CheckUseNumText())
                    return;

                try
                {
                    string strTemp = string.Empty;
                    CSearchQuestionParam questionsParam = new CSearchQuestionParam();
                    questionsParam.LongCategoryNum = _mcCategoryTree.LongNum;
                    questionsParam.StrQuestionsContect = TxtSearchInfo.Text;
                    questionsParam.IntType = ComboBoxName.SelectedIndex;
                    questionsParam.IntUseMax = Convert.ToInt32(TbUseNumMax.Text.ToString());
                    questionsParam.IntUseMin = Convert.ToInt32(TbUseNumMin.Text.ToString());
                    if (ComboBoxName.SelectedIndex != -1)
                    {
                        switch (ComboBoxName.SelectedIndex)
                        {
                            case 0:
                                questionsParam.StrAccessoryType =  BoxItem1.Content.ToString();
                                break;
                            case 1:
                                questionsParam.StrAccessoryType =  BoxItem2.Content.ToString();
                                break;
                            case 2:
                                questionsParam.StrAccessoryType = BoxItem3.Content.ToString();
                                break;
                            case 3:
                                questionsParam.StrAccessoryType = BoxItem4.Content.ToString();
                                break;
                            case 4:
                                questionsParam.StrAccessoryType = BoxItem5.Content.ToString();
                                break;
                        }
                    }
                    else
                    {
                        questionsParam.StrAccessoryType = null;
                    }
                    
                    if (TrueOrFalseName.IsChecked == true)
                    {
                        strTemp = ((int)QuestionType.TrueOrFalse).ToString();
                    }
                    if (SingleChioceName.IsChecked == true)
                    {
                        if (!string.IsNullOrEmpty(strTemp))
                        {
                            strTemp += (char)27 + ((int)QuestionType.SingleChoice).ToString();
                        }
                        else
                        {
                            strTemp = ((int)QuestionType.SingleChoice).ToString();
                        }
                    }
                    if (MultipleChoiceName.IsChecked == true)
                    {
                        if (!string.IsNullOrEmpty(strTemp))
                        {
                            strTemp += (char)27 + ((int)QuestionType.MultipleChioce).ToString();
                        }
                        else
                        {
                            strTemp = ((int)QuestionType.MultipleChioce).ToString();
                        }
                    }
                    questionsParam.StrQuestionType = strTemp;
                    questionsParam.StrStartTime = FromTimeName.Value.ToString();
                    questionsParam.StrEndTime = ToTimeNum.Value.ToString();

                    ParentPage.SearchQuestions(questionsParam);
                    parent.IsOpen = false;
                }
                catch (Exception ex)
                {
                    ShowException(ex.Message);
                }
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

            }
        }

        private void TbUseNumMin_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (Convert.ToInt64(TbUseNumMin.Text) < 0 || Convert.ToInt64(TbUseNumMin.Text) > 10000)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3601T00193", "Numerical range of 0-10000"));
                }
            }
            catch
            {

            }

        }

        private void TbUseNumMax_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (Convert.ToInt64(TbUseNumMax.Text) < 0 || Convert.ToInt64(TbUseNumMax.Text) > 10000)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3601T00193", "Numerical range of 0-10000"));
                }
            }
            catch
            {

            }
        }

        private bool CheckUseNumText()
        {
            try
            {
                if (Convert.ToInt64(TbUseNumMin.Text) < 0 || Convert.ToInt64(TbUseNumMin.Text) > 10000)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3601T00193", "Numerical range of 0-10000"));
                    return false;
                }
                if (Convert.ToInt64(TbUseNumMax.Text) < 0 || Convert.ToInt64(TbUseNumMax.Text) > 10000)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3601T00193", "Numerical range of 0-10000"));
                    return false;
                }
                if (Convert.ToInt64(TbUseNumMin.Text) > Convert.ToInt64(TbUseNumMax.Text))
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3601T00194", "Use the number fill in error! Please check the following fill out again!"));
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private void TxtSearchInfo_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtSearchInfo.Text.Length > 128)
            {
                ShowInformation(CurrentApp.GetLanguageInfo("3601T00195", "The subject content query cannot exceed 128 characters."));
            }
        }
    }
}
