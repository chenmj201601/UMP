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
using YoungClassesLibrary;

namespace UMPS1101
{
    public partial class S1101MainPage : Page
    {
        public S1101MainPage()
        {
            InitializeComponent();
            this.Loaded += S1101MainPage_Loaded;
        }

        private void S1101MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            UMPFeatureHeader.InitClientHost(string.Empty);
            UMPFeatureHeader.IOperationEvent += UMPFeatureHeader_IOperationEvent;
        }

        private void UMPFeatureHeader_IOperationEvent(object sender, OperationEventArgs e)
        {
            if (e.StrObjectTag == "INIT") { LoadStyleResources(e); return; }
        }

        #region 加载style 资源
        private void LoadStyleResources(OperationEventArgs AEventArgs)
        {
            string LStrStyleUrl = string.Empty;
            ShareClassesInterface LClassInterface = new ShareClassesInterface();

            LClassInterface.StrObjectTag = "INITED";
            try
            {
                IntermediateSingleFeature LIntermediateParam = AEventArgs.ObjectSource0 as IntermediateSingleFeature;
                LStrStyleUrl = LIntermediateParam.StrUseProtol + "://" + LIntermediateParam.StrServerHost + ":" + LIntermediateParam.StrServerPort + "/Themes/" + LIntermediateParam.StrUseStyle + ".xaml";
                ResourceDictionary LResourceDictionary = new ResourceDictionary();
                LResourceDictionary.Source = new Uri(LStrStyleUrl, UriKind.Absolute);
                Application.Current.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add(LResourceDictionary);
                LClassInterface.ObjObjectSource0 = "1";
            }
            catch (Exception ex)
            {
                LClassInterface.ObjObjectSource0 = "0";
                LClassInterface.ObjObjectSource1 = ex.ToString();
            }
            finally
            {
                UMPFeatureHeader.SendMessageToParent(LClassInterface);
            }
        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(UMPFeatureHeader.IIntermediateParam.StrUseProtol + "://" + UMPFeatureHeader.IIntermediateParam.StrServerHost + ":" + UMPFeatureHeader.IIntermediateParam.StrServerPort + "/Themes/" + UMPFeatureHeader.IIntermediateParam.StrUseStyle + ".xaml");
        }
    }
}
