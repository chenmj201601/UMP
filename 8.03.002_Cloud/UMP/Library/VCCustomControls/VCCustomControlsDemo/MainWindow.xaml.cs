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

namespace VCCustomControlsDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        private List<PersonItem> mListPersons;  

        public MainWindow()
        {
            InitializeComponent();

            mListPersons = new List<PersonItem>();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            InitPersonInfos();
            DataContext = mListPersons;
        }

        private void InitPersonInfos()
        {
            mListPersons.Clear();
            mListPersons.Add(new PersonItem
            {
                ID = 1,
                FirstName = "Jian1",
                LastName = "Chen1",
                Age = 10,
                IsChecked = false
            });
            mListPersons.Add(new PersonItem
            {
                ID = 2,
                FirstName = "Jian2",
                LastName = "Chen2",
                Age = 20,
                IsChecked = false
            });
            mListPersons.Add(new PersonItem
            {
                ID = 3,
                FirstName = "Jian3",
                LastName = "Chen3",
                Age = 21,
                IsChecked = false
            });
            mListPersons.Add(new PersonItem
            {
                ID = 4,
                FirstName = "Jian4",
                LastName = "Chen4",
                Age = 43,
                IsChecked = false
            });
            mListPersons.Add(new PersonItem
            {
                ID = 5,
                FirstName = "Jian5",
                LastName = "Chen5",
                Age = 28,
                IsChecked = false
            });
            mListPersons.Add(new PersonItem
            {
                ID = 6,
                FirstName = "Jian6",
                LastName = "Chen6",
                Age = 76,
                IsChecked = false
            });
        }
    }
}
