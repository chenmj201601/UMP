using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private GridTreeObject mRoot;
        private ObservableCollection<DataItem> mListDatas;

        public MainWindow()
        {
            InitializeComponent();

            mListDatas = new ObservableCollection<DataItem>();

        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            InitTreeObjects();
            InitDatas();
            //TvData.ItemsSource = mRoot.Children;
            LvData.ItemsSource = mListDatas;
            mRoot.Initialize();
        }

        private void InitTreeObjects()
        {
            mRoot = new GridTreeObject("");
            mRoot.Children.Add(new GridTreeObject("Root")
            {
                Children =
                {
                    new GridTreeObject("Sub1")
                    {
                        Children =
                        {
                            new GridTreeObject("Sub11")
                            {
                                Children =
                                {
                                    new GridTreeObject("Sub111"),
                                    new GridTreeObject("Sub112")
                                }
                            },
                            new GridTreeObject("Sub12")
                            {
                                Children =
                                {
                                    new GridTreeObject("Sub121"),
                                    new GridTreeObject("Sub122")
                                }
                            },
                            new GridTreeObject("Sub13")
                            {
                                Children =
                                {
                                    new GridTreeObject("Sub131")
                                }
                            }
                        }
                    },
                    new GridTreeObject("Sub2")
                    {
                        Children =
                        {
                            new GridTreeObject("21")
                            {
                                Children =
                                {
                                    new GridTreeObject("211")
                                }
                            },
                            new GridTreeObject("22")
                            {
                                Children =
                                {
                                    new GridTreeObject("221"),
                                    new GridTreeObject("222")
                                }
                            }
                        }
                    },
                    new GridTreeObject("Sub3")
                }
            });
        }

        private void InitDatas()
        {
            mListDatas.Clear();
            mListDatas.Add(new DataItem
            {
                Name = "user1",
                Age = 20,
                BirthDay = DateTime.Parse("1987-3-2"),
                Address = "Shanghai"
            });
            mListDatas.Add(new DataItem
            {
                Name = "user2",
                Age = 21,
                BirthDay = DateTime.Parse("1986-3-2"),
                Address = "Beijing"
            });
            mListDatas.Add(new DataItem
            {
                Name = "user3",
                Age = 17,
                BirthDay = DateTime.Parse("1992-3-2"),
                Address = "Wuxi"
            });
            mListDatas.Add(new DataItem
            {
                Name = "user4",
                Age = 25,
                BirthDay = DateTime.Parse("1982-3-2"),
                Address = "Nanjing"
            });
        }
    }

    public class DataItem
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime BirthDay { get; set; }
        public string Address { get; set; }
    }
}
