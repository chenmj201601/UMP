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
using YoungControlLibrary;

namespace UMPMainEntrance.BasicModule
{
    public partial class UMPMainPage : Page
    {
        public UMPMainPage()
        {
            InitializeComponent();
            this.Loaded += UMPMainPage_Loaded;
            //BorderUserInformation.MouseEnter += BorderUserInformation_MouseEnter;
            //BorderUserInformation.MouseLeave += BorderUserInformation_MouseLeave;

            ScrollViewerFeatureBody.PreviewMouseWheel += ScrollViewerFeatureBody_MouseWheel;
        }

        private void ScrollViewerFeatureBody_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta < 0)
            {
                ScrollViewerFeatureBody.LineRight();
                ScrollViewerFeatureBody.LineRight();
                ScrollViewerFeatureBody.LineRight();
                ScrollViewerFeatureBody.LineRight();
                ScrollViewerFeatureBody.LineRight();
                ScrollViewerFeatureBody.LineRight();
            }
            else
            {
                ScrollViewerFeatureBody.LineLeft();
                ScrollViewerFeatureBody.LineLeft();
                ScrollViewerFeatureBody.LineLeft();
                ScrollViewerFeatureBody.LineLeft();
                ScrollViewerFeatureBody.LineLeft();
                ScrollViewerFeatureBody.LineLeft();
            }
        }

        

        private void UMPMainPage_Loaded(object sender, RoutedEventArgs e)
        {
            App.LoadGloabalParameters();
            LoadUserDefaultHeadSculpture();
            ADDFeature(1);
            
        }

        private void LoadUserDefaultHeadSculpture()
        {
            ImageUser.Source = new BitmapImage(new Uri("http://192.168.4.186:8088/Themes/CommonComponents/UserNoFrame.png", UriKind.Absolute));
        }

        #region ======测试代码,以后删除
        private void ADDFeature(int AIntALl)
        {
            Random LRandom = new Random();

            for (int LIntLoop = 0; LIntLoop < 5; LIntLoop++)
            {
                int LIntSuijiID = LRandom.Next(1, 15);
                FeatureMagnetPanel LFeatureMagnetPanel01 = new FeatureMagnetPanel();
                LFeatureMagnetPanel01.Margin = new Thickness(30, 0, 0, 0);
                LFeatureMagnetPanel01.IOperationEvent += LFeatureMagnetPanel01_IOperationEvent;
                StackPanelFeatureBody.Children.Add(LFeatureMagnetPanel01);
                LFeatureMagnetPanel01.AddControlTest(LIntSuijiID);
            }
            
        }

        private void LFeatureMagnetPanel01_IOperationEvent(object sender, YoungClassesLibrary.OperationEventArgs e)
        {
            if (e.StrObjectTag == "LOAD")
            {
                FeatureContainerPage LFeatureContainerPage = new FeatureContainerPage();
                LFeatureContainerPage.StrFeatureImageSource = e.ObjectSource1.ToString();
                this.NavigationService.Navigate(LFeatureContainerPage);
            }
        }
        #endregion
    }
}
