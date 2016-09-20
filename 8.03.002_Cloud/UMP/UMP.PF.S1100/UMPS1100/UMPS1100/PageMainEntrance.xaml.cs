using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using VoiceCyber.UMP.Communications;

namespace UMPS1100
{
    public partial class PageMainEntrance : Page,S1100ChangeLanguageInterface
    {
        public event EventHandler<OperationEventArgs> IChangeLanguageEvent;

        public PageMainEntrance()
        {
            InitializeComponent();
            this.Loaded += PageMainEntrance_Loaded;
        }

        private void PageMainEntrance_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                App.IPageMainOpend = this;
                WebRequest LWebRequestClientLoading = new WebRequest();
                LWebRequestClientLoading.Code = (int)RequestCode.CSModuleLoading;
                LWebRequestClientLoading.Session = App.GClassSessionInfo;
                LWebRequestClientLoading.Session.SessionID = App.GClassSessionInfo.SessionID;
                WebReturn LWebReturn = App.SendNetPipeMessage(LWebRequestClientLoading);
                if (LWebReturn.Result)
                {
                    App.GClassSessionInfo.AppServerInfo = LWebReturn.Session.AppServerInfo;
                    App.GClassSessionInfo.DatabaseInfo = LWebReturn.Session.DatabaseInfo;
                    App.GClassSessionInfo.DBConnectionString = LWebReturn.Session.DBConnectionString;
                    App.GClassSessionInfo.DBType = LWebReturn.Session.DBType;
                    App.GClassSessionInfo.LangTypeInfo = LWebReturn.Session.LangTypeInfo;
                    App.GClassSessionInfo.LocalMachineInfo = LWebReturn.Session.LocalMachineInfo;
                    App.GClassSessionInfo.RentInfo = LWebReturn.Session.RentInfo;
                    App.GClassSessionInfo.RoleInfo = LWebReturn.Session.RoleInfo;
                    App.GClassSessionInfo.ThemeInfo = LWebReturn.Session.ThemeInfo;
                    App.GClassSessionInfo.UserInfo = LWebReturn.Session.UserInfo;
                    if (!string.IsNullOrEmpty(LWebReturn.Data)) { App.GStrCurrentOperation = LWebReturn.Data; }
                    App.LoadStyleDictionary();
                    App.LoadApplicationLanguages();
                    App.Load11009Data();
                }
                DoingMainSendMessage(App.GStrCurrentOperation);

                WebRequest LWebRequestClientLoaded = new WebRequest();
                LWebRequestClientLoaded.Code = (int)RequestCode.CSModuleLoaded;
                LWebRequestClientLoaded.Session = App.GClassSessionInfo;
                LWebRequestClientLoaded.Session.SessionID = App.GClassSessionInfo.SessionID;
                App.SendNetPipeMessage(LWebRequestClientLoaded);

                GridMain.KeyDown += GridMain_KeyDown;
                GridMain.MouseMove += GridMain_MouseMove;
                ITimerSendMessage2Main.Elapsed += ITimerSendMessage2Main_Elapsed;
                ITimerSendMessage2Main.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }


        public void DoingMainSendMessage(string AStrData)
        {
            try
            {
                IChangeLanguageEvent = null;

                if (AStrData == "1104")
                {
                    App.Load11009Data();
                    GridDynamicObject.Children.Clear();
                    UCOrganizationMaintenance LUCOrgType = new UCOrganizationMaintenance();
                    LUCOrgType.IPageParent = this;
                    LUCOrgType.ShowAllOrgType();
                    GridDynamicObject.Children.Add(LUCOrgType);
                    GridDynamicObject.BringIntoView();
                    return;
                }
                if (AStrData == "1105")
                {
                    App.Load11009Data();
                    GridDynamicObject.Children.Clear();
                    UCSkillGroupMaintenance LUCSkillGroup = new UCSkillGroupMaintenance();
                    LUCSkillGroup.IPageParent = this;
                    LUCSkillGroup.ShowAllSkillGroup();
                    GridDynamicObject.Children.Add(LUCSkillGroup);
                    GridDynamicObject.BringIntoView();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void ApplicationLanguageChanged()
        {
            if (IChangeLanguageEvent != null)
            {
                OperationEventArgs LEventArgs = new OperationEventArgs();
                IChangeLanguageEvent(this, LEventArgs);
            }
        }

        

        #region 判断是否处于"忙"状态
        int IIntIdleCount = 0;
        private bool IBoolTimerBusy = false;
        private Timer ITimerSendMessage2Main = new Timer(1000);
        private delegate void IDelegateSendMessage2Main();

        private void GridMain_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                IIntIdleCount = 0;
            }
            catch { }
        }

        private void GridMain_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                IIntIdleCount = 0;
            }
            catch { }
        }

        private void ITimerSendMessage2Main_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, new IDelegateSendMessage2Main(SendMessage2MainFram));
        }

        private void SendMessage2MainFram()
        {
            try
            {
                IIntIdleCount += 1;
                if (IBoolTimerBusy) { return; }
                IBoolTimerBusy = true;
                WebRequest LWebRequestClientLoading = new WebRequest();
                LWebRequestClientLoading.Code = 91001;
                LWebRequestClientLoading.Data = IIntIdleCount.ToString();
                WebReturn LWebReturn = App.SendNetPipeMessage(LWebRequestClientLoading);
            }
            catch { }
            finally
            {
                IBoolTimerBusy = false;
            }
        }

        public void StartStopTimer(bool ABoolStart)
        {
            if (ABoolStart) { IIntIdleCount = 0; ITimerSendMessage2Main.Start(); } else { ITimerSendMessage2Main.Stop(); IIntIdleCount = 0; }
        }

        #endregion
    }
}
