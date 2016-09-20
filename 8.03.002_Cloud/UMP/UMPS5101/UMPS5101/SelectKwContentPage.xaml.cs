using System;
using System.Windows;
using System.Windows.Controls;
using VoiceCyber.UMP.Controls;

namespace UMPS5101
{
    public struct SelectKwContentParam
    {
        public bool BEnableContent;
        public string StrContent;
        public bool BEnable;
        public int IntEnable;
        public bool BDelete;
        public int IntDelete;
        public string StrStartTime;
        public string StrEndTime;
    }
    /// <summary>
    /// SelectKwContentPage.xaml 的交互逻辑
    /// </summary>
    public partial class SelectKwContentPage
    {
        public MainView ParentPage;

        public SelectKwContentPage()
        {
            InitializeComponent();
            Loaded += UCCustomSetting_Loaded;
        }

        private void UCCustomSetting_Loaded(object sender, RoutedEventArgs e)
        {
            ChbKwContent.IsChecked = false;
            ChbEnableState.IsChecked = false;
            ChbDeleteState.IsChecked = true;
            Init();
            ChangeLanguage();
        }

        private void Init()
        {
            SetKwContentEnable();
            SetRbEnable();
            SetRbDelete();
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();
            ChbKwContent.Content = CurrentApp.GetLanguageInfo("5101T00037", "Content");
            ChbEnableState.Content = CurrentApp.GetLanguageInfo("5101T00010", "Enable");
            ChbDeleteState.Content = CurrentApp.GetLanguageInfo("5101T00051", "Delete");
            GpbCreateTime.Header = CurrentApp.GetLanguageInfo("5101T00012", "Add Time");
            DtStartTime.Value = DateTime.Today;
            DtEndTime.Value = DateTime.Now;
            RbYes.IsChecked = true;
            RbYes.Content = CurrentApp.GetLanguageInfo("5101T00005", "Enable");
            RbNo.Content = CurrentApp.GetLanguageInfo("5101T00048", "Close");
            RbDeleteYes.Content = CurrentApp.GetLanguageInfo("5101T00052", "Yes");
            RbDeleteNo.Content = CurrentApp.GetLanguageInfo("5101T00053", "No");
            RbDeleteNo.IsChecked = true;
            ButOk.Content = CurrentApp.GetLanguageInfo("5101T00031", "OK");
            ButCancel.Content = CurrentApp.GetLanguageInfo("5101T00027", "Cancel");
        }

        private void SetKwContentEnable()
        {
            TxtKwContent.IsReadOnly = ChbKwContent.IsChecked != true;
        }

        private void SetRbEnable()
        {
            RbYes.IsEnabled = ChbEnableState.IsChecked == true;
            RbNo.IsEnabled = ChbEnableState.IsChecked == true;
        }

        private void SetRbDelete()
        {
            RbDeleteYes.IsEnabled = ChbDeleteState.IsChecked == true;
            RbDeleteNo.IsEnabled = ChbDeleteState.IsChecked == true;
        }

        private void ButOk_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TxtKwContent.Text.Length > 120)
                {
                    MessageBox.Show(CurrentApp.GetLanguageInfo("5101T00066",
                    "Add content should not exceed 120 characters."),
                    CurrentApp.GetLanguageInfo("5101T00050", "Warning"),
                    MessageBoxButton.OK);
                    return;
                }

                SelectKwContentParam selectKwContent =new SelectKwContentParam();
                selectKwContent.BEnableContent = ChbKwContent.IsChecked == true;
                if (selectKwContent.BEnableContent)
                {
                    selectKwContent.StrContent = TxtKwContent.Text;
                }
                selectKwContent.BEnable = ChbEnableState.IsChecked == true;
                if (selectKwContent.BEnable)
                {
                    selectKwContent.IntEnable = RbYes.IsChecked == true ? 1 : 0;
                }
                selectKwContent.BDelete = ChbDeleteState.IsChecked == true;
                if (selectKwContent.BDelete)
                {
                    selectKwContent.IntDelete = RbDeleteYes.IsChecked == true ? 1 : 0;
                }
                selectKwContent.StrStartTime = DtStartTime.Value.ToString();
                selectKwContent.StrEndTime = DtEndTime.Value.ToString();

                ParentPage.SelectKwContent(selectKwContent);
                var parant = Parent as PopupPanel;
                if (parant != null)
                {
                    parant.IsOpen = false;
                }

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ButCancel_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var parant = Parent as PopupPanel;
                if (parant != null)
                {
                    parant.IsOpen = false;
                }
            }
            catch
            {
                // ignored
            }
        }

        private void TbKwContent_OnClick(object sender, RoutedEventArgs e)
        {
            SetKwContentEnable();
        }

        private void TbEnableState_OnClick(object sender, RoutedEventArgs e)
        {
            SetRbEnable();
        }

        private void TbEnableDelete_OnClick(object sender, RoutedEventArgs e)
        {
            SetRbDelete();
        }

        private void TxtKwContent_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (TxtKwContent.Text.Length > 120)
                {
                    MessageBox.Show(CurrentApp.GetLanguageInfo("5101T00066",
                    "Add content should not exceed 120 characters."),
                    CurrentApp.GetLanguageInfo("5101T00050", "Warning"),
                    MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
    }
}
