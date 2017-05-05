using Common6106;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UMPS6106.Service61061;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;

namespace UMPS6106
{
    /// <summary>
    /// UC_RecordLength.xaml 的交互逻辑
    /// </summary>
    public partial class UC_RecordLength
    {
        //统计时长（统计多长时间内的数据 由主界面传入）
        private int iRecTimeLength;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iTimeLength">统计时长 ( 统计多长时间内的数据 ) </param>
        public UC_RecordLength(int iTimeLength, int iHeight, int iWidth)
        {
            InitializeComponent();
            iRecTimeLength = iTimeLength;
            chartRecordLength.Width = iWidth;
            chartRecordLength.Height = iHeight;
            Loaded += UC_RecordLength_Loaded;
        }

        void UC_RecordLength_Loaded(object sender, RoutedEventArgs e)
        {
            InitLanguage();
            OperationReturn optReturn = PermissionFuncs.GetRecordMode();
            if (!optReturn.Result)
            {
                App.ShowExceptionMessage(App.GetLanguageInfo(optReturn.Code.ToString(), optReturn.Code.ToString()));
                return;
            }
            string strMode = optReturn.Data as string;
            ShowChart(strMode);
        }

        #region Override
        public override void ChangeLanguage()
        {
            App.WriteLog("UC_EncryptMailSetting ChangeLanguage");
            base.ChangeLanguage();
            InitLanguage();
        }
        #endregion

        #region Init
        private void InitLanguage()
        {
            LineAgent.Title = App.GetLanguageInfo("6106008", "Agent");
            LineExtension.Title = App.GetLanguageInfo("6106009", "Extension");
            chartRecordLength.Title = App.GetLanguageInfo("6106017", "Recording length statistics");
        }
        #endregion

        private void ShowChart(string strMode)
        {
            List<KeyValueEntry> lstExtensionDatas = new List<KeyValueEntry>();
            List<KeyValueEntry> lstAgentDatas = new List<KeyValueEntry>();

            if (strMode.Contains(ConstValue.SPLITER_CHAR))
            {
                LineAgent.Visibility = Visibility.Visible;
                LineExtension.Visibility = Visibility.Visible;
                lstExtensionDatas = GetDataByMode("E");
                lstAgentDatas = GetDataByMode("A");
            }
            else if (strMode == "E")
            {
                LineAgent.Visibility = Visibility.Collapsed;
                LineExtension.Visibility = Visibility.Visible;
                lstExtensionDatas = GetDataByMode("E");
            }
            else if (strMode == "A")
            {
                LineAgent.Visibility = Visibility.Visible;
                LineExtension.Visibility = Visibility.Visible;
                lstAgentDatas = GetDataByMode("A");
            }
            var lstSources = new List<List<KeyValueEntry>>();
            lstSources.Add(lstExtensionDatas);
            lstSources.Add(lstAgentDatas);
            chartRecordLength.DataContext = lstSources;

            
        }

        private List<KeyValueEntry> GetDataByMode(string strMode)
        {
            List<KeyValueEntry> lstReturn = new List<KeyValueEntry>();
            Service61061Client client = null;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S6106RequestCode.GetRecordLength;
                webRequest.Session = App.Session;
                client = new Service61061Client(WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service61061"));
                webRequest.ListData.Add(iRecTimeLength.ToString());
                webRequest.ListData.Add(DateTime.Now.ToString());
                webRequest.ListData.Add(strMode);
                webRequest.ListData.Add(App.OrgID);
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                App.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    if (webReturn.Code != Defines.RET_FAIL)
                    {
                        App.ShowExceptionMessage(App.GetLanguageInfo(webReturn.Code.ToString(), "WSFail.\t Error code:" + webReturn.Code.ToString()));
                    }
                    else
                    {
                        App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    }
                    return lstReturn;
                }
                OperationReturn optReturn = null;
                List<KeyValueEntry> lstk = new List<KeyValueEntry>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<KeyValueEntry>(webReturn.ListData[i]);
                    if (optReturn.Result)
                    {
                        lstk.Add(optReturn.Data as KeyValueEntry);
                    }
                }
                return lstk;
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(string.Format("WSFail.\t{0", ex.Message));
            }
            finally
            {
                if (client != null)
                {
                    if (client.State == System.ServiceModel.CommunicationState.Opened)
                    {
                        client.Close();
                    }
                }
            }
            return lstReturn;
        }
    }
}
