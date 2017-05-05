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
using VoiceCyber.UMP.Communications;
using UMPS2400.Service24011;
using Common2400;
using System.Data;
using UMPS2400.Classes;
using VoiceCyber.Common;
using UMPS2400.MainUserControls;
using System.ComponentModel;
using UMPS2400.Entries;
using VoiceCyber.UMP.Common;

namespace UMPS2400.ChildUCs
{
    /// <summary>
    /// UC_AddKeyGenServer.xaml 的交互逻辑
    /// </summary>
    public partial class UC_AddKeyGenServer
    {
        #region 变量定义
        public UC_EncryptServersManagement mainPage = null;
        //添加或是修改操作
        public int iAddOrModify = 0;
        //正在被修改的项
        public KeyGenServerEntryInList keyGenServerModifying;

        //BackgroundWorker 用来做添加或修改的wcf交互 
        BackgroundWorker mBackgroundWorker;

        #endregion
        public UC_AddKeyGenServer()
        {
            InitializeComponent();
            Loaded += UC_AddKeyGenServer_Loaded;
            BtnApply.Click += BtnApply_Click;
            BtnCancel.Click += (s, de) =>
                {
                    mainPage.PopupPanel.IsOpen = false;
                };
        }

        void UC_AddKeyGenServer_Loaded(object sender, RoutedEventArgs e)
        {
            InitMachines();
            //如果是修改 则只能修改端口 并加载信息
            if (iAddOrModify == (int)OperationType.Modify)
            {
                cmbServers.IsEnabled = false;
                foreach (ComboBoxItem item in cmbServers.Items)
                {
                    StringValuePairs pairs = item.DataContext as StringValuePairs;
                    if (pairs.Key == keyGenServerModifying.ResourceID)
                    {
                        cmbServers.SelectedItem = item;
                    }
                }
                TxtPort.Text = keyGenServerModifying.HostPort;
            }
        }

        #region Init & Load
        private void InitMachines()
        {
            WebRequest webRequest = new WebRequest();
            webRequest.Code = (int)S2400RequestCode.GetAllMachines;
            webRequest.Session = CurrentApp.Session;
            webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
            webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
            Service24011Client client = new Service24011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service24011"));
            WebReturn webReturn = client.DoOperation(webRequest);
            CurrentApp.MonitorHelper.AddWebReturn(webReturn);
            client.Close();
            if (!webReturn.Result)
            {
                ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                return;
            }
            StringValuePairs obj = null;
            OperationReturn optReturn;
            ComboBoxItem item = null;
            for (int i = 0; i < webReturn.ListData.Count; i++)
            {
                optReturn = XMLHelper.DeserializeObject<StringValuePairs>(webReturn.ListData[i]);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                obj = optReturn.Data as StringValuePairs;
                obj.Value = S2400EncryptOperation.DecryptWithM004(S2400EncryptOperation.DecodeEncryptValue(obj.Value));
                item = new ComboBoxItem();
                item.Content = obj.Value;
                item.DataContext = obj;
                cmbServers.Items.Add(item);
            }
            if (cmbServers.Items.Count > 0)
            {
                cmbServers.SelectedIndex = 0;
            }
        }
        #endregion

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();
            LblServer.Content = CurrentApp.GetLanguageInfo("2401L002", "Machine Address");
            LblPort.Content = CurrentApp.GetLanguageInfo("2401L003", "Machine Port");
            BtnApply.Content = CurrentApp.GetLanguageInfo("2401B001", "Confirm");
            BtnCancel.Content = CurrentApp.GetLanguageInfo("2401B002", "Close");
        }


        #region 事件
        void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            //在backgroundwork开始前 准备好数据
            BtnApply.IsEnabled = false;
            ComboBoxItem item = cmbServers.SelectedItem as ComboBoxItem;
            StringValuePairs obj = item.DataContext as StringValuePairs;
            switch (iAddOrModify)
            {
                case (int)OperationType.Add:
                    AddKeyGenServer(obj);
                    break;
                case (int)OperationType.Modify:
                    ModifyKeyGenServer(obj);
                    break;
            }
        }

        private void AddKeyGenServer(StringValuePairs machineInfo)
        {
            string strHost = machineInfo.Value;
            string strMachineID = machineInfo.Key;
            WebReturn webReturn = null;
            string strPort = TxtPort.Text;

            mBackgroundWorker = new BackgroundWorker();
            mBackgroundWorker.DoWork += (s, de) =>
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S2400RequestCode.AddKeyGenServer;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add(strMachineID);
                webRequest.ListData.Add(strHost);
                webRequest.ListData.Add(strPort);
                webRequest.ListData.Add("0");
                Service24011Client client = new Service24011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                        WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service24011"));
                webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
            };
            mBackgroundWorker.RunWorkerCompleted += (s, de) =>
            {
                if (!webReturn.Result)
                {
                    if (webReturn.Code == (int)S2400WcfErrorCode.KeyGenServerExists)
                    {
                        BtnApply.IsEnabled = true;
                        ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, CurrentApp.GetLanguageInfo(webReturn.Code.ToString(), webReturn.Message)));
                    }
                    else
                    {
                        BtnApply.IsEnabled = true;
                        ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    }
                    string msg = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO2401003")), strHost);
                    CurrentApp.WriteOperationLog("2401003", ConstValue.OPT_RESULT_FAIL, msg);
                }
                else
                {
                    string msg = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO2401003")), strHost);
                    CurrentApp.WriteOperationLog("2401003", ConstValue.OPT_RESULT_SUCCESS, msg);
                    //添加成功 关闭当前popupPanel 刷新父窗口list
                    BtnApply.IsEnabled = true;
                    mainPage.PopupPanel.IsOpen = false;
                    //mainPage.InitKeyGenServerList();
                    KeyGenServerEntryInList server = new KeyGenServerEntryInList();
                    server.HostAddress = strHost;
                    server.ResourceID = strMachineID;
                    server.HostPort = TxtPort.Text;
                    server.IsEnable = "0";
                    server.EnableIcon = "Images/00001.ico";
                    server.Status = true;
                    mainPage.UpdateKeyGenServerList(server, OperationType.Add);
                    ShowInformation(CurrentApp.GetLanguageInfo("COMN001", "Success"));
                    mainPage.lvGeneratorObject.SelectedIndex = 0;
                }
            };
            mBackgroundWorker.RunWorkerAsync();
        }

        private void ModifyKeyGenServer(StringValuePairs machineInfo)
        {
            string strHost = machineInfo.Value;
            string strMachineID = machineInfo.Key;
            WebReturn webReturn = null;
            string strPort = TxtPort.Text;

            string strStatusMsg = CurrentApp.GetLanguageInfo("2401022", "Is being revised, please later... .");
            mainPage.ShowStausMessage(strStatusMsg, true);
            mBackgroundWorker = new BackgroundWorker();
            mBackgroundWorker.DoWork += (s, de) =>
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S2400RequestCode.ModifyKeyGenServer;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(strMachineID);
                webRequest.ListData.Add(strHost);
                webRequest.ListData.Add(strPort);
                Service24011Client client = new Service24011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                        WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service24011"));
                webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
            };
            mBackgroundWorker.RunWorkerCompleted += (s, de) =>
            {
                mainPage.ShowStausMessage(string.Empty, false);
                if (!webReturn.Result)
                {
                    if (webReturn.Code == (int)S2400WcfErrorCode.KeyGenServerExists)
                    {
                        BtnApply.IsEnabled = true;
                        ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, CurrentApp.GetLanguageInfo(webReturn.Code.ToString(), webReturn.Message)));
                    }
                    else
                    {
                        BtnApply.IsEnabled = true;
                        ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    }
                    string msg = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO2401004")), strHost);
                    CurrentApp.WriteOperationLog("2401004", ConstValue.OPT_RESULT_FAIL, msg);
                }
                else
                {
                    string msg = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO2401004")), strHost);
                    CurrentApp.WriteOperationLog("2401004", ConstValue.OPT_RESULT_SUCCESS, msg);
                    //添加成功 关闭当前popupPanel 刷新父窗口list
                    BtnApply.IsEnabled = true;
                    mainPage.PopupPanel.IsOpen = false;
                    //mainPage.InitKeyGenServerList();
                    keyGenServerModifying.HostPort = TxtPort.Text;
                    keyGenServerModifying.Status = true;
                    mainPage.UpdateKeyGenServerList(keyGenServerModifying, OperationType.Modify);
                    ShowInformation(CurrentApp.GetLanguageInfo("COMN001", "Success"));
                }
            };
            mBackgroundWorker.RunWorkerAsync();
        }
        #endregion

    }
}
