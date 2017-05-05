using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UMP.Tools.PublicClasses;
using UMP.Tools.UMPWcfService00003;

namespace UMP.Tools.ThirdPartyApplications
{
    public partial class ThirdPartyASMSetting : Window, OperationsInterface
    {
        public event EventHandler<OperationEventArgs> IOperationEvent;

        //是否在处理的过程中
        private bool IBoolInDoing = false;

        //设置参数BackgroundWorker
        private BackgroundWorker IBackgroundWorkerA = null;

        private OperationDataArgs I00003OperationReturn = new OperationDataArgs();

        public ThirdPartyASMSetting()
        {
            InitializeComponent();
            this.Loaded += ThirdPartyASMSetting_Loaded;
            this.Closing += ThirdPartyASMSetting_Closing;
            this.MouseLeftButtonDown += ThirdPartyASMSetting_MouseLeftButtonDown;

            ButtonCloseThis.Click += WindowsButtonClicked;
            ButtonASMSetting.Click += WindowsButtonClicked;
            ButtonCloseWindow.Click += WindowsButtonClicked;
        }

        public void ShowThirdPartyAlreadSetting(DataRow ADataRowASMInfo)
        {
            try
            {
                TabItemASMSetting.Header = " " + ADataRowASMInfo["Attribute00"].ToString() + " ";

                if (ADataRowASMInfo["Attribute01"].ToString().ToLower() == "https://")
                {
                    ComboBoxPortol.SelectedIndex = 1;
                }
                else
                {
                    ComboBoxPortol.SelectedIndex = 0;
                }
                TextBoxServerName.Text = ADataRowASMInfo["Attribute02"].ToString();
                TextBoxPort.SetElementData(ADataRowASMInfo["Attribute03"].ToString());
                TextBoxArguments.Text = ADataRowASMInfo["Attribute11"].ToString();
            }
            catch { }
        }

        private void DisplayElementCharacters(bool ABoolLanguageChange)
        {
            LabelASMSettingTip.Content = App.GetDisplayCharater("M06005");
            LabelPortol.Content = App.GetDisplayCharater("M06001");
            LabelServerName.Content = App.GetDisplayCharater("M06002");
            LabelPort.Content = App.GetDisplayCharater("M06003");
            LabelArguments.Content = App.GetDisplayCharater("M06004");

            ButtonASMSetting.Content = App.GetDisplayCharater("M06006");
            ButtonCloseWindow.Content = App.GetDisplayCharater("M06007");
        }

        private void ThirdPartyASMSetting_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IBoolInDoing) { e.Cancel = true; return; }
        }

        private void ThirdPartyASMSetting_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ThirdPartyASMSetting_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                App.DrawWindowsBackGround(this);
                ImageASMSetting.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000046.ico"), UriKind.RelativeOrAbsolute));
                DisplayElementCharacters(false);
                TextBoxPort.SetMinMaxDefaultValue(1024, 65535, 8004);
            }
            catch { }
        }

        private void WindowsButtonClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                Button LButtonClicked = sender as Button;
                string LStrClickedName = LButtonClicked.Name;

                switch (LStrClickedName)
                {
                    case "ButtonCloseThis":
                        CloseThisWindow();
                        break;
                    case "ButtonASMSetting":
                        SetASMLinkArguments();
                        break;
                    case "ButtonCloseWindow":
                        CloseThisWindow();
                        break;
                    default:
                        break;
                }

            }
            catch { }
        }

        private void CloseThisWindow()
        {
            if (!IBoolInDoing) { this.Close(); }
        }

        #region 设置参数
        private List<string> IListStrArguments = new List<string>();            //0:应用名；1:协议；2:服务器；3：端口；4：参数
        private void SetASMLinkArguments()
        {
            try
            {
                IListStrArguments.Clear();

                IListStrArguments.Add("ASM");
                if (ComboBoxPortol.SelectedIndex == 0)
                {
                    IListStrArguments.Add("http://");
                }
                else
                {
                    IListStrArguments.Add("https://");
                }
                IListStrArguments.Add(TextBoxServerName.Text.Trim());
                IListStrArguments.Add(TextBoxPort.GetElementData());
                IListStrArguments.Add(TextBoxArguments.Text.Trim());

                MainPanel.IsEnabled = false;
                IBoolInDoing = true;
                App.ShowCurrentStatus(1, App.GetDisplayCharater("M06008"));
                if (IBackgroundWorkerA == null) { IBackgroundWorkerA = new BackgroundWorker(); }
                IBackgroundWorkerA.RunWorkerCompleted += IBackgroundWorkerA_RunWorkerCompleted;
                IBackgroundWorkerA.DoWork += IBackgroundWorkerA_DoWork;
                IBackgroundWorkerA.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                MainPanel.IsEnabled = true;
                IBoolInDoing = false;
                App.ShowCurrentStatus(int.MaxValue, string.Empty);
                if (IBackgroundWorkerA != null)
                {
                    IBackgroundWorkerA.Dispose(); IBackgroundWorkerA = null;
                }
                App.ShowExceptionMessage("" + ex.ToString());
            }
        }

        private void IBackgroundWorkerA_DoWork(object sender, DoWorkEventArgs e)
        {
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            Service00003Client LService00003Client = null;

            List<string> LListWcfArgs = new List<string>();

            bool LBoolCallReturn = true;
            string LStrCallReturn = string.Empty;

            try
            {
                List<string> LListStrDBProfile = App.GSystemMainWindow.GetCurrentDatabaseProfile();
                List<string> LListStrAppServer = App.GSystemMainWindow.GetCurrentAppServerConnection();

                LBasicHttpBinding = App.CreateBasicHttpBinding(true, 15);
                LEndpointAddress = App.CreateEndpointAddress(LListStrAppServer[0], LListStrAppServer[1], true, "Service00003");
                LService00003Client = new Service00003Client(LBasicHttpBinding, LEndpointAddress);

                foreach (string LStrSingleProfile in LListStrDBProfile) { LListWcfArgs.Add(LStrSingleProfile); }
                LListWcfArgs.Add("00000");
                foreach (string LStrSingleArgument in IListStrArguments) { LListWcfArgs.Add(LStrSingleArgument); }

                I00003OperationReturn = LService00003Client.OperationMethodA(11, LListWcfArgs);
            }
            catch (Exception ex)
            {
                I00003OperationReturn.BoolReturn = false;
                I00003OperationReturn.StringReturn = "UMP004E002" + App.GStrSpliterChar + ex.Message;
            }
            finally
            {
                if (LService00003Client != null)
                {
                    if (LService00003Client.State == CommunicationState.Opened) { LService00003Client.Close(); LService00003Client = null; }
                }
            }
        }

        private void IBackgroundWorkerA_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string LStrMessageBody = string.Empty;

            MainPanel.IsEnabled = true;
            IBoolInDoing = false;
            App.ShowCurrentStatus(int.MaxValue, string.Empty);
            MainPanel.IsEnabled = true;
            if (IBackgroundWorkerA != null)
            {
                IBackgroundWorkerA.Dispose(); IBackgroundWorkerA = null;
            }
            if (!I00003OperationReturn.BoolReturn)
            {
                string[] LStrOperationReturn = I00003OperationReturn.StringReturn.Split(App.GStrSpliterChar.ToCharArray());
                LStrMessageBody = App.GetDisplayCharater(LStrOperationReturn[0]);
                MessageBox.Show(LStrMessageBody, App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (IOperationEvent != null)
            {
                OperationEventArgs LOperationEventArgs = new OperationEventArgs();
                LOperationEventArgs.StrElementTag = "RSETTP";
                LOperationEventArgs.ObjSource = IListStrArguments;
                IOperationEvent(this, LOperationEventArgs);
            }
            MessageBox.Show(App.GetDisplayCharater("M06009"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Information);
            CloseThisWindow();
        }
        #endregion
    }
}
