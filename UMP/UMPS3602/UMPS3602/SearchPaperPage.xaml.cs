using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Common3602;
using UMPS3602.Models;
using UMPS3602.Wcf36021;
using VoiceCyber.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS3602
{
    /// <summary>
    /// SearchPaperPage.xaml 的交互逻辑
    /// </summary>
    public partial class SearchPaperPage
    {
        public ExamProductionView ParentPage;
        private List<CPaperParam> _mlstPaperParamInfo;

        public SearchPaperPage()
        {
            _mlstPaperParamInfo = new List<CPaperParam>();
            InitializeComponent();
            
            this.Loaded += UCCustomSetting_Loaded;
        }

        void UCCustomSetting_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Init();
        }

        private void Init()
        {
            ComBoxUsedNum.SelectedIndex = 0;
            ChangeLanguage();
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();
            PaperName.Text = CurrentApp.GetLanguageInfo("3602T00003", "Paper");
            CourseNumberName.Text = CurrentApp.GetLanguageInfo("3602T00115", "Course Num");
            ScoreName.Text = CurrentApp.GetLanguageInfo("3602T00043", "Scores");
            PassMarkName.Text = CurrentApp.GetLanguageInfo("3602T00044", "Pass Mark");
            TimeName.Text = CurrentApp.GetLanguageInfo("3602T00045", "Time");
            UsedNumName.Text = CurrentApp.GetLanguageInfo("3602T00048", "Used");
            CreateName.Text = CurrentApp.GetLanguageInfo("3602T00116", "Create Time");
            ComBoxUsedNum0.Content = CurrentApp.GetLanguageInfo("3602T00154", "All");
            ComBoxUsedNum1.Content = CurrentApp.GetLanguageInfo("3602T00155", "Y");
            ComBoxUsedNum2.Content = CurrentApp.GetLanguageInfo("3602T00156", "N");
            BtnConfirm.Content = CurrentApp.GetLanguageInfo("3602T00068", "Confirm");
            BtnClose.Content = CurrentApp.GetLanguageInfo("3602T00069", "Close");
            FromTimeName.Value = DateTime.Today;
            ToTimeName.Value = DateTime.Now;
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            var parent = Parent as PopupPanel;
            int iMin = 0;
            int iMax = 0;
            if (parent != null)
            {
                if (!CheckUseNumText())
                    return;

                try
                {
                    CSearchPaperParam searchPaperParam = new CSearchPaperParam();
                    searchPaperParam.StrName = TbPaperName.Text;
                    searchPaperParam.IntScoresMax = Convert.ToInt32(TbScoreMax.Text);
                    searchPaperParam.IntScoresMin = Convert.ToInt32(TbScoreMax.Text.ToString());
                    searchPaperParam.IntPassMarkMax = Convert.ToInt32(TbPassMarkMax.Text.ToString());
                    searchPaperParam.IntPassMarkMin = Convert.ToInt32(TbPassMarkMin.Text.ToString());
                    searchPaperParam.IntTimeMax = Convert.ToInt32(TbTimeMarkMax.Text.ToString());
                    searchPaperParam.IntTimeMin = Convert.ToInt32(TbTimeMarkMin.Text.ToString());
                    if (ComBoxUsedNum.SelectedIndex > 0)
                    {
                        searchPaperParam.IntUsed = ComBoxUsedNum.SelectedIndex == 1 ? 1 : 0;
                    }
                    else
                    {
                        searchPaperParam.IntUsed = -1;
                    }
                    searchPaperParam.StrStartTime = FromTimeName.Value.ToString();
                    searchPaperParam.StrEndTime = ToTimeName.Value.ToString();
                    ParentPage.SearchPaper(searchPaperParam);
                    parent.IsOpen = false;
                }
                catch (Exception ex)
                {
                   ShowException(ex.Message);
                }
            }         
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
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

        private void TbScoreMin_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (Convert.ToInt64(TbScoreMin.Text) < 0 || Convert.ToInt64(TbScoreMin.Text) > 500)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3602T00177", "Test scores query in the range of 10-500"));
                }
            }
            catch
            {

            }
        }

        private void TbScoreMax_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (Convert.ToInt64(TbScoreMax.Text) < 0 || Convert.ToInt64(TbScoreMax.Text) > 500)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3602T00177", "Test scores query in the range of 10-500"));
                }
            }
            catch
            {

            }
        }

        private void TbPassMarkMin_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (Convert.ToInt64(TbPassMarkMin.Text) < 0 || Convert.ToInt64(TbPassMarkMin.Text) > 500)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3602T00178", "The passing score query in the range of 10-500"));
                }
            }
            catch
            {

            }
        }

        private void TbPassMarkMax_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (Convert.ToInt64(TbPassMarkMax.Text) < 0 || Convert.ToInt64(TbPassMarkMax.Text) > 500)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3602T00178", "The passing score query in the range of 10-500"));
                }
            }
            catch
            {

            }
        }

        private void TbTimeMarkMin_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (Convert.ToInt64(TbTimeMarkMin.Text) < 0 || Convert.ToInt64(TbTimeMarkMin.Text) > 1440)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3602T00179", "The test time query in the range of 10-1440"));
                }
            }
            catch
            {

            }
        }

        private void TbTimeMarkMax_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (Convert.ToInt64(TbTimeMarkMax.Text) < 0 || Convert.ToInt64(TbTimeMarkMax.Text) > 1440)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3602T00179", "The test time query in the range of 10-1440"));
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
                if (Convert.ToInt64(TbScoreMin.Text) < 0 || Convert.ToInt64(TbScoreMin.Text) > 500)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3602T00177", "Test scores query in the range of 10-500"));
                    return false;
                }
                if (Convert.ToInt64(TbPassMarkMin.Text) < 0 || Convert.ToInt64(TbPassMarkMin.Text) > 500)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3602T00178", "The passing score query in the range of 10-500"));
                    return false;
                }
                if (Convert.ToInt64(TbTimeMarkMin.Text) < 0 || Convert.ToInt64(TbTimeMarkMax.Text) > 1440)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3602T00179", "The test time query in the range of 10-1440"));
                    return false;
                }

                if (Convert.ToInt64(TbScoreMin.Text) > Convert.ToInt64(TbScoreMax.Text))
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3602T00180", "Test scores to fill in error! Please check the following fill out again!"));
                    return false;
                }

                if (Convert.ToInt64(TbPassMarkMin.Text) > Convert.ToInt64(TbPassMarkMax.Text))
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3602T00181", "Test Pass Mark to fill in error! Please check the following fill out again!"));
                    return false;
                }

                if (Convert.ToInt64(TbTimeMarkMin.Text) > Convert.ToInt64(TbTimeMarkMax.Text))
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3602T00182", "Test Time to fill in error! Please check the following fill out again!"));
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public void Connect(int connectionId, object target)
        {
            
        }
    }
}
