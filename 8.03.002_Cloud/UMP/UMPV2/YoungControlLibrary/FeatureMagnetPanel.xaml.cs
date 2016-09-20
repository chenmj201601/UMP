using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using YoungClassesLibrary;

namespace YoungControlLibrary
{
    public partial class FeatureMagnetPanel : UserControl, OperationsInterface
    {
        public event EventHandler<OperationEventArgs> IOperationEvent;

        public FeatureMagnetPanel()
        {
            InitializeComponent();
            this.SizeChanged += FeatureMagnetPanel_SizeChanged;
        }

        #region 界面大小发生变化时重新设置 WrapPanel 的宽度
        
        private void FeatureMagnetPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResetWrapPanelWidth();
        }

        private void ResetWrapPanelWidth()
        {
            try
            {
                double LDoubleActualHeight = this.ActualHeight - 30;
                int LIntAllChildren = WrapPanelFeatureMagnets.Children.Count;

                if (LIntAllChildren <= 0) { return; }

                int LIntCanContainRows = (int)(LDoubleActualHeight / 106);
                int LIntCanContainCols = (int)Math.Ceiling(Convert.ToDouble(LIntAllChildren) / Convert.ToDouble(LIntCanContainRows));

                //if (LIntCanContainCols < 2) { LIntCanContainCols = 2; }
                if (LIntCanContainCols % 2 == 1 && LIntAllChildren > 1 && LIntCanContainRows > 1) { LIntCanContainCols += 1; }
                WrapPanelFeatureMagnets.Width = LIntCanContainCols * 106;
            }
            catch { }
        }

        #endregion

        #region 将FeatureMagnet传递过来的消息转发至UMPMainPage

        private void LFeatureMagnet_IOperationEvent(object sender, OperationEventArgs e)
        {
            IOperationEvent(this, e);
        }

        #endregion
        
        public void AddControlTest(int AIntAllButton)
        {
            Random LRandom = new Random();

            for (int LIntLoop = 0; LIntLoop < AIntAllButton; LIntLoop++)
            {
                int LIntSuijiID = LRandom.Next(0, 20);

                FeatureMagnet LFeatureMagnet = new FeatureMagnet();
                LFeatureMagnet.StrFeatureImageSource = "http://192.168.4.186:8088/Themes/CommonComponents/suiji" + LIntSuijiID.ToString("00") + ".png";
                LFeatureMagnet.StrFeatureContent = "Demo " + LIntLoop.ToString("00");
                LFeatureMagnet.DoubleLabelHeight = 0;
                LFeatureMagnet.Width = 96;
                LFeatureMagnet.Height = 96;
                LFeatureMagnet.Margin = new Thickness(5);
                LFeatureMagnet.IOperationEvent += LFeatureMagnet_IOperationEvent;
                
                WrapPanelFeatureMagnets.Children.Add(LFeatureMagnet);
            }
        }
    }
}
