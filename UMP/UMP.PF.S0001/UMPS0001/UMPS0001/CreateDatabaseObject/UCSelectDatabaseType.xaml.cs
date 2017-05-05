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

namespace UMPS0001.CreateDatabaseObject
{
    public partial class UCSelectDatabaseType : UserControl
    {
        public UCSelectDatabaseType()
        {
            InitializeComponent();
            this.Loaded += UCSelectDatabaseType_Loaded;
        }

        private void UCSelectDatabaseType_Loaded(object sender, RoutedEventArgs e)
        {
            ShowElementLanguage();
        }

        private void ShowElementLanguage()
        {
            TabItemDatabaseType.Header = " " + App.GetDisplayCharater("M02005") + " ";
            LabelSelectDatabase.Content = App.GetDisplayCharater("M02006");
            RadioButtonDBType2.Content = App.GetDisplayCharater("M02007");
            RadioButtonDBType3.Content = App.GetDisplayCharater("M02008");
        }

        public List<string> GetSettedData()
        {
            List<string> LListStrReturn = new List<string>();

            if (RadioButtonDBType2.IsChecked == true)
            {
                LListStrReturn.Add("2");
            }
            else
            {
                LListStrReturn.Add("3");
            }

            return LListStrReturn;
        }
    }
}
