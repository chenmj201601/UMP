using System;
using System.Collections.Generic;
using System.Data;
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
using UMP.Tools.PublicClasses;

namespace UMP.Tools.ThirdPartyApplications
{
    public partial class UCThirdPartyASM : UserControl, OperationsInterface
    {
        public event EventHandler<OperationEventArgs> IOperationEvent;

        #region 控件初始化
        public UCThirdPartyASM()
        {
            InitializeComponent();
            this.Loaded += UCThirdPartyASM_Loaded;
            this.Unloaded += UCThirdPartyASM_Unloaded;
        }

        private void UCThirdPartyASM_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayElementCharacters(false);
            App.GSystemMainWindow.IOperationEvent += GSystemMainWindow_IOperationEvent;
        }

        private void UCThirdPartyASM_Unloaded(object sender, RoutedEventArgs e)
        {
            App.GSystemMainWindow.IOperationEvent -= GSystemMainWindow_IOperationEvent;
        }
        #endregion

        #region 显示界面语言/语言改变
        public void DisplayElementCharacters(bool ABoolLanguageChange)
        {
            LabelProtol.Text = App.GetDisplayCharater("M06001");
            LabelServerName.Text = App.GetDisplayCharater("M06002");
            LabelServerPort.Text = App.GetDisplayCharater("M06003");
            LabelArguments.Text = App.GetDisplayCharater("M06004");
        }
        #endregion

        public void WriteData2View(DataRow ADataRowASMInfo)
        {
            try
            {
                LabelProfileTitle.Text = ADataRowASMInfo["Attribute00"].ToString();

                TextBoxProtol.Text = ADataRowASMInfo["Attribute01"].ToString();
                TextBoxServerName.Text = ADataRowASMInfo["Attribute02"].ToString();
                TextBoxServerPort.Text = ADataRowASMInfo["Attribute03"].ToString();
                TextBoxArguments.Text = ADataRowASMInfo["Attribute11"].ToString();
            }
            catch { }
        }

        private void GSystemMainWindow_IOperationEvent(object sender, PublicClasses.OperationEventArgs e)
        {
            if (e.StrElementTag == "CLID") { DisplayElementCharacters(true); return; }
        }

    }
}
