using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace UMPThemesDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<PersonInfo> mListPersonInfos;
        private DirInfo mRootDir;
        private ObservableCollection<string> mListThemes;
        private ObservableCollection<ColorInfo> mListColors;

        public MainWindow()
        {
            InitializeComponent();

            mListPersonInfos = new ObservableCollection<PersonInfo>();
            mListThemes = new ObservableCollection<string>();
            mListColors = new ObservableCollection<ColorInfo>();
            mRootDir = new DirInfo();
            this.Loaded += MainWindow_Loaded;
            ComboThemes.SelectionChanged += (s, e) => ChangeTheme();
            CbThemes.SelectionChanged += (s, e) => ChangeTheme();
            ListBoxColors.SelectionChanged += (s, e) => ChangeTheme();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitThemes();
            InitColors();
            InitPersonInfos();
            InitDirInfo();

            ComboThemes.ItemsSource = mListThemes;
            CbThemes.ItemsSource = mListThemes;
            ListBoxColors.ItemsSource = mListColors;
            ListBoxPerson.ItemsSource = mListPersonInfos;
            ComboPerson.ItemsSource = mListPersonInfos;
            LvPerson.ItemsSource = mListPersonInfos;
            TvPerson.ItemsSource = mRootDir.Children;
            DgPerson.ItemsSource = mListPersonInfos;

            if (mListThemes.Count > 0)
            {
                CbThemes.SelectedIndex = 0;
            }
            if (mListColors.Count > 0)
            {
                ListBoxColors.SelectedIndex = 0;
            }
        }

        private void InitPersonInfos()
        {
            mListPersonInfos.Clear();
            mListPersonInfos.Add(new PersonInfo
            {
                Name = "Charley",
                FullName = "Charley Chen",
                Age = 27,
                Birthday = DateTime.Parse("1988-01-14"),
                Job = "Enginer"
            });
            mListPersonInfos.Add(new PersonInfo
            {
                Name = "Charley1",
                FullName = "Charley Chen",
                Age = 4,
                Birthday = DateTime.Parse("1987-01-14"),
                Job = "Enginer"
            });
            mListPersonInfos.Add(new PersonInfo
            {
                Name = "Charley2",
                FullName = "Charley Chen",
                Age = 66,
                Birthday = DateTime.Parse("1986-01-14"),
                Job = "Enginer"
            });
            mListPersonInfos.Add(new PersonInfo
            {
                Name = "Charley3",
                FullName = "Charley Chen",
                Age = 3,
                Birthday = DateTime.Parse("1984-01-14"),
                Job = "Enginer"
            });
            mListPersonInfos.Add(new PersonInfo
            {
                Name = "Charley4",
                FullName = "Charley Chen",
                Age = 54,
                Birthday = DateTime.Parse("1988-01-14"),
                Job = "Enginer"
            });
            mListPersonInfos.Add(new PersonInfo
            {
                Name = "Charley5",
                FullName = "Charley Chen",
                Age = 23,
                Birthday = DateTime.Parse("1986-01-14"),
                Job = "Enginer"
            });
            mListPersonInfos.Add(new PersonInfo
            {
                Name = "Charley6",
                FullName = "Charley Chen",
                Age = 32,
                Birthday = DateTime.Parse("1989-01-14"),
                Job = "Enginer"
            });
            mListPersonInfos.Add(new PersonInfo
            {
                Name = "Charley7",
                FullName = "Charley Chen",
                Age = 5,
                Birthday = DateTime.Parse("1983-01-14"),
                Job = "Enginer"
            });
            mListPersonInfos.Add(new PersonInfo
            {
                Name = "Charley8",
                FullName = "Charley Chen",
                Age = 6,
                Birthday = DateTime.Parse("1984-01-14"),
                Job = "Enginer"
            });
            mListPersonInfos.Add(new PersonInfo
            {
                Name = "Charley8",
                FullName = "Charley Chen",
                Age = 55,
                Birthday = DateTime.Parse("1985-01-14"),
                Job = "Enginer"
            });
            mListPersonInfos.Add(new PersonInfo
            {
                Name = "Charley9",
                FullName = "Charley Chen",
                Age = 46,
                Birthday = DateTime.Parse("1983-01-14"),
                Job = "Enginer"
            });
            mListPersonInfos.Add(new PersonInfo
            {
                Name = "Charley10",
                FullName = "Charley Chen",
                Age = 27,
                Birthday = DateTime.Parse("1988-01-14"),
                Job = "Enginer"
            });
        }

        private void InitDirInfo()
        {
            if (mRootDir != null)
            {
                DirectoryInfo dir = new DirectoryInfo("F:\\VCLogIMPRelease");
                if (dir.Exists)
                {
                    mRootDir.Name = dir.Name;
                    mRootDir.FullPath = dir.FullName;
                    mRootDir.Children.Clear();
                    InitDirInfo(mRootDir, dir);
                }
            }
        }

        private void InitDirInfo(DirInfo parent, DirectoryInfo dir)
        {
            if (dir != null && parent != null)
            {
                DirectoryInfo[] children = dir.GetDirectories();
                for (int i = 0; i < children.Length; i++)
                {
                    DirInfo dirInfo = new DirInfo();
                    dirInfo.Name = children[i].Name;
                    dirInfo.FullPath = children[i].FullName;
                    InitDirInfo(dirInfo, children[i]);
                    parent.Children.Add(dirInfo);
                }
            }
        }

        private void InitThemes()
        {
            mListThemes.Clear();
            mListThemes.Add("Default");
            mListThemes.Add("MetroLight");
            mListThemes.Add("ExpLight");
        }

        private void InitColors()
        {
            mListColors.Clear();

            mListColors.Add(new ColorInfo
            {
                Name = "Brown",
                Value = "#632F00"
            });
            mListColors.Add(new ColorInfo
            {
                Name = "Blue",
                Value = "#4117A2"
            });
            mListColors.Add(new ColorInfo
            {
                Name = "Green",
                Value = "#036569"
            });
            mListColors.Add(new ColorInfo
            {
                Name = "Yellow",
                Value = "#486D05"
            });
            mListColors.Add(new ColorInfo
            {
                Name = "Gray",
                Value = "#FF9B9999"
            });
        }

        private void ChangeTheme()
        {
            var themeItem = CbThemes.SelectedItem;
            var colorItem = ListBoxColors.SelectedItem as ColorInfo;
            if (themeItem != null && colorItem != null)
            {
                var themeName = themeItem.ToString();
                var color = colorItem.Name;
                if (!string.IsNullOrEmpty(themeName) && !string.IsNullOrEmpty(color))
                {
                    App.Current.Resources.MergedDictionaries.Clear();
                    ResourceDictionary resource;
                    try
                    {
                        resource = new ResourceDictionary();
                        resource.Source = new Uri(string.Format("/UMPThemes;component/{0}/Colors/{1}.xaml", themeName, color), UriKind.RelativeOrAbsolute);
                        App.Current.Resources.MergedDictionaries.Add(resource);
                    }catch{}
                    try
                    {
                        resource = new ResourceDictionary();
                        resource.Source = new Uri(string.Format("/UMPThemes;component/{0}/Control.xaml", themeName), UriKind.RelativeOrAbsolute);
                        App.Current.Resources.MergedDictionaries.Add(resource);
                    }catch{}
                }
            }
        }
    }
}
