using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UMPS3603.Models;
using VoiceCyber.UMP.Controls;

namespace UMPS3603
{
    /// <summary>
    /// SearchTestInfoPage.xaml 的交互逻辑
    /// </summary>
    public partial class SearchTestInfoPage
    {
        public ExamProductionView ParentPage;

        public SearchTestInfoPage()
        {
            InitializeComponent();
            Loaded += UCCustomSetting_Loaded;
        }

        void UCCustomSetting_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Init()
        {
            ComBoxUsedNum.SelectedIndex = 0;
            DtStartTime.Value = DateTime.Today;
            DtEndTime.Value = DateTime.Now;
            DttbTestTime.Text = DateTime.Now.ToString("yyyy-MM-dd");
            ChangeLanguage();
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();
            ChkTestNum.Content = CurrentApp.GetLanguageInfo("3603T00009", "Test Num");
            ChkPaperNum.Content = CurrentApp.GetLanguageInfo("3603T00010", "Paper Num");
            ChkPaperName.Content = CurrentApp.GetLanguageInfo("3603T00011", "Paper Name");
            ChkEditorNum.Content = CurrentApp.GetLanguageInfo("3603T00014", "Test Set Person Num");
            ChkEditorName.Content = CurrentApp.GetLanguageInfo("3603T00015", "Test Set Person Name");
            ChkTestState.Content = CurrentApp.GetLanguageInfo("3603T00017", "Test State");
            ChkTestTime.Content = CurrentApp.GetLanguageInfo("3603T00013", "Test Time");
            TbFrom.Text = CurrentApp.GetLanguageInfo("3603T00085", "Create Time");
            ButOk.Content = CurrentApp.GetLanguageInfo("3603T00051", "OK");
            ButClose.Content = CurrentApp.GetLanguageInfo("3603T00052", "Close");
            ComBoxUsedNum0.Content = CurrentApp.GetLanguageInfo("3603T00079", "All");
            ComBoxUsedNum1.Content = CurrentApp.GetLanguageInfo("3603T00086", "Y");
            ComBoxUsedNum2.Content = CurrentApp.GetLanguageInfo("3603T00087", "N");
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                if (!CheckText())
                {
                    return;
                }

                try
                {
                    TestInfoParamEx testInfoParamEx = new TestInfoParamEx();
                    if (ChkTestNum.IsChecked == true)
                    {
                        testInfoParamEx.LongTestNum = !string.IsNullOrEmpty(TxtTestNum.Text) ? Convert.ToInt64(TxtTestNum.Text) : 0;
                    }

                    if (ChkPaperNum.IsChecked == true)
                    {
                        testInfoParamEx.LongPaperNum = !string.IsNullOrEmpty(TxtPaperNum.Text) ? Convert.ToInt64(TxtPaperNum.Text) : 0;
                    }

                    if (ChkPaperName.IsChecked == true)
                    {
                        testInfoParamEx.StrPaperName = !string.IsNullOrEmpty(TxtPaperName.Text) ? TxtPaperName.Text : null;
                    }

                    if (ChkEditorNum.IsChecked == true)
                    {
                        testInfoParamEx.LongEditorId = !string.IsNullOrEmpty(TxtEditorNum.Text) ? Convert.ToInt64(TxtEditorNum.Text) : 0;
                    }

                    if (ChkEditorName.IsChecked == true)
                    {
                        testInfoParamEx.StrEditor = !string.IsNullOrEmpty(TxtEditorName.Text)
                            ? TxtEditorName.Text
                            : null;
                    }

                    if (ChkTestState.IsChecked == true)
                    {
                        switch (ComBoxUsedNum.SelectedIndex)
                        {
                            case 1:
                                testInfoParamEx.StrTestStatue = "Y";
                                break;
                            case 2:
                                testInfoParamEx.StrTestStatue = "N";
                                break;
                            default:
                                testInfoParamEx.StrTestStatue = null;
                                break;
                        }
                    }

                    if (ChkTestTime.IsChecked == true)
                    {
                        testInfoParamEx.StrTestTime = DttbTestTime.ToString();
                    }

                    if (ChkTestNum.IsChecked == false)
                    {
                        testInfoParamEx.StrStartTime = DtStartTime.Value.ToString();
                        testInfoParamEx.StrEndTime = DtEndTime.Value.ToString();
                    }

                    ParentPage.SearchTestInfo(testInfoParamEx);
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
                // ignored
            }
        }

        private bool CheckText()
        {
            try
            {
                if (TxtTestNum.Text.Length > 20)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3603T00098", "Test serial number is too long!"));
                    return false;
                }

                if (TxtPaperNum.Text.Length > 20)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3603T00099", "Paper serial number is too long!"));
                    return false;
                }

                if (TxtPaperName.Text.Length > 128)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3603T00100", "Paper name cannot exceed 128 characters!"));
                    return false;
                }

                if (TxtEditorNum.Text.Length > 20)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3603T00102", "Editor serial number is too long!"));
                    return false;
                }

                if (TxtEditorName.Text.Length > 128)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3603T00101", "Editor name cannot exceed 128 characters!"));
                    return false;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return false;
            }
            return true;
        }

        private void TxtTestNum_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtTestNum.Text.Length >= 20)
            {
                ShowException(CurrentApp.GetLanguageInfo("3603T00098", "Test serial number is too long!"));
            }
        }

        private void TxtPaperNum_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtPaperNum.Text.Length >= 20)
            {
                ShowException(CurrentApp.GetLanguageInfo("3603T00099", "Paper serial number is too long!"));
            }
        }

        private void TxtPaperName_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtPaperName.Text.Length >=128)
            {
                ShowException(CurrentApp.GetLanguageInfo("3603T00100", "Paper name cannot exceed 128 characters!"));
            }
        }

        private void TxtEditorNum_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtEditorNum.Text.Length >= 20)
            {
                ShowException(CurrentApp.GetLanguageInfo("3603T00102", "Editor serial number is too long!"));
            }
        }

        private void TxtEditorName_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtEditorName.Text.Length > 128)
            {
                ShowException(CurrentApp.GetLanguageInfo("3603T00101", "Editor name cannot exceed 128 characters!"));
            }
        }

        private void SetCheckBox(CheckBox checkBox, TextBox textBox)
        {
            if (checkBox.IsChecked == true)
            {
                textBox.IsReadOnly = false;
                textBox.Background = Brushes.White;
            }
            else
            {
                textBox.IsReadOnly = true;
                textBox.Background = Brushes.LightGray;
            }
        }

        private void SetCheckBox(CheckBox checkBox, ComboBox comboBox)
        {
            comboBox.IsEnabled = checkBox.IsChecked == true;
        }

        private void ChkTestNum_OnClick(object sender, RoutedEventArgs e)
        {
            SetCheckBox(ChkTestNum, TxtTestNum);
            DtStartTime.IsEnabled = ChkTestNum.IsChecked == false;
            DtEndTime.IsEnabled = ChkTestNum.IsChecked == false;

            ChkPaperNum.IsEnabled = ChkTestNum.IsChecked == false;
            ChkPaperNum.IsChecked = false;
            SetCheckBox(ChkPaperNum, TxtPaperNum);

            ChkPaperName.IsEnabled = ChkTestNum.IsChecked == false;
            ChkPaperName.IsChecked = false;
            SetCheckBox(ChkPaperName, TxtPaperName);

            ChkEditorNum.IsEnabled = ChkTestNum.IsChecked == false;
            ChkEditorNum.IsChecked = false;
            SetCheckBox(ChkEditorNum, TxtEditorNum);

            ChkEditorName.IsEnabled = ChkTestNum.IsChecked == false;
            ChkEditorName.IsChecked = false;
            SetCheckBox(ChkEditorName, TxtEditorName);

            ChkTestState.IsEnabled = ChkTestNum.IsChecked == false;
            ChkTestState.IsChecked = false;
            SetCheckBox(ChkTestState, ComBoxUsedNum);

            ChkTestTime.IsEnabled = ChkTestNum.IsChecked == false;
            ChkTestTime.IsChecked = false;
            DttbTestTime.IsEnabled = ChkTestTime.IsChecked == true;
        }

        private void ChkPaperNum_OnClick(object sender, RoutedEventArgs e)
        {
            SetCheckBox(ChkPaperNum, TxtPaperNum);
            ChkPaperName.IsChecked = false;
            SetCheckBox(ChkPaperName, TxtPaperName);
        }

        private void ChkPaperName_OnClick(object sender, RoutedEventArgs e)
        {
            SetCheckBox(ChkPaperName, TxtPaperName);
            ChkPaperNum.IsChecked = false;
            SetCheckBox(ChkPaperNum, TxtPaperNum);
        }

        private void ChkEditorNum_OnClick(object sender, RoutedEventArgs e)
        {
            SetCheckBox(ChkEditorNum, TxtEditorNum);
            ChkEditorName.IsChecked = false;
            SetCheckBox(ChkEditorName, TxtEditorName);
        }

        private void ChkEditorName_OnClick(object sender, RoutedEventArgs e)
        {
            SetCheckBox(ChkEditorName, TxtEditorName);
            ChkEditorNum.IsChecked = false;
            SetCheckBox(ChkEditorNum, TxtEditorNum);
        }

        private void ChkTestState_OnClick(object sender, RoutedEventArgs e)
        {
            SetCheckBox(ChkTestState, ComBoxUsedNum);
        }

        private void ChkTestTime_OnClick(object sender, RoutedEventArgs e)
        {
            DttbTestTime.IsEnabled = ChkTestTime.IsChecked == true;
        }
    }
}
