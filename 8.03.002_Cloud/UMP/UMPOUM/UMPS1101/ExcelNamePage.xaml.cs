using System;
using System.Collections.Generic;
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
using VoiceCyber.UMP.Controls;

namespace UMPS1101
{
    /// <summary>
    /// ExcelNamePage.xaml 的交互逻辑
    /// </summary>
    public partial class ExcelNamePage
    {
        public List<string> ExcelName;
        public OUMMainView pageParent;
        string SheetName = string.Empty;
        public string ExcelPath = string.Empty;
        //public S1101App CurrentApp;
        public ExcelNamePage()
        {
            InitializeComponent();

            Loaded += ExcelNamePage_Loaded;
        }

        void ExcelNamePage_Loaded(object sender, RoutedEventArgs e)
        {
            this.ListExcelName.ItemsSource = ExcelName;
            this.BtnConfirm.Click += BtnConfirm_Click;
            this.BtnClose.Click += BtnClose_Click;
            this.ListExcelName.SelectionChanged += ListExcelName_SelectionChanged;

            ChangeLanguage();
        }

        void ListExcelName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SheetName = this.ListExcelName.SelectedItem as string;
        }

        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            PageClose();
        }

        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            pageParent.LoadData(SheetName, ExcelPath);
            PageClose();
        }

        private void PageClose()
        {
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }

        #region Languages

        public override void ChangeLanguage()
        {
            try
            {
                base.ChangeLanguage();

                BtnConfirm.Content = CurrentApp.GetLanguageInfo("110110", "Confirm");
                BtnClose.Content = CurrentApp.GetLanguageInfo("110111", "Close");

                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.Title = CurrentApp.GetLanguageInfo("1101T10217", "Excel Sheet Name Selection");
                }
            }
            catch (Exception ex)
            { }
        }

        #endregion
    }
}
