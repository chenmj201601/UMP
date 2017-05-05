using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Common3604;

namespace UMPS3604
{
    /// <summary>
    /// Interaction logic for UploadResourceFilesPage.xaml
    /// </summary>
    public partial class UploadResourceFilesPage
    {
        public MaterialLibraryView ParentPage;
        public List<FileProperty> MLstFileProperties;
        private ObservableCollection<FileProperty> _mLstFileProperties; 

        public UploadResourceFilesPage()
        {
            MLstFileProperties = new List<FileProperty>();
            _mLstFileProperties = new ObservableCollection<FileProperty>();
            InitializeComponent();
            Loaded += UCCustomSetting_Loaded;
        }

        private void UCCustomSetting_Loaded(object sender, RoutedEventArgs e)
        {
            InitList();
            UploadResourceFilesListView.ItemsSource = _mLstFileProperties;
            SetList();
            ChangeLanguage();
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            ButOk.Content = CurrentApp.GetLanguageInfo("3604T00043", "Upload");
            ButCancel.Content = CurrentApp.GetLanguageInfo("3604T00044", "Cancel");
        }

        private void InitList()
        {
            try
            {
                string[] lans = "3604T00041,3604T00037,3604T00040,3604T00042,3604T00038,3604T00039".Split(',');
                string[] cols = "StrImage,StrName,LSize,StrFileType,StrPath,RemotePath".Split(',');
                int[] colwidths = { 50, 200, 80, 80, 180, 180 };
                GridView columnGridView = new GridView();
                for (int i = 0; i < colwidths.Length; i++)
                {
                    DataTemplate dt;
                    var gvColumn = new GridViewColumn();
                    if (cols[i] == "StrImage")
                    {
                        dt = Resources["CellUploadStatusTemplate"] as DataTemplate;
                        if (dt != null)
                        {
                            gvColumn.CellTemplate = dt;
                        }
                        else
                        {
                            gvColumn.DisplayMemberBinding = new Binding(cols[0]);
                        }
                    }
                    else
                    {
                        gvColumn.DisplayMemberBinding = new Binding(cols[i]);
                    }
                    gvColumn.Header = CurrentApp.GetLanguageInfo(lans[i], cols[i]);
                    gvColumn.Width = colwidths[i];
                    columnGridView.Columns.Add(gvColumn);
                }
                UploadResourceFilesListView.View = columnGridView;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SetList()
        {
            _mLstFileProperties.Clear();
            foreach (var fileProperty in MLstFileProperties)
            {
                if (fileProperty.IState == 0)
                {
                    fileProperty.StrImage = "001.png";
                }
                if (fileProperty.IState == 1)
                {
                    fileProperty.StrImage = "002.png";
                }
                if (fileProperty.IState == 2)
                {
                    fileProperty.StrImage = "003.png";
                }
                _mLstFileProperties.Add(fileProperty);
            }
        }

        private void Ok_OnClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            
        }


    }
}
