using Common6106;
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
using UMPS6106.Service61061;
using VoiceCyber.Common;
using VoiceCyber.UMP.Communications;

namespace UMPS6106.UCCharts
{
    /// <summary>
    /// UC_AppealCount.xaml 的交互逻辑
    /// </summary>
    public partial class UC_AppealCount 
    {
        //统计时长（统计多长时间内的数据 由主界面传入）
        private int iRecTimeLength;

        public UC_AppealCount(int iTimeLength, int Iheight, int Iwidth)
        {
            InitializeComponent();
            iRecTimeLength = iTimeLength;
            chartApplealCount.Height = Iheight;
            chartApplealCount.Width = Iwidth;
            Loaded += UC_AppealCount_Loaded;
        }

        void UC_AppealCount_Loaded(object sender, RoutedEventArgs e)
        {
            InitLanguage();
            ShowChart();
        }

        #region Init
        private void InitLanguage()
        {
            chartApplealCount.Title = App.GetLanguageInfo("6106012", "The number of statistical representations");
            colAppeal.Title = App.GetLanguageInfo("6106013", "Count");
        }
        #endregion

        #region Overried
        public override void ChangeLanguage()
        {
            base.ChangeLanguage();
            InitLanguage();
        }
        #endregion

        private void ShowChart()
        {
            List<KeyValueEntry> lstReturn = new List<KeyValueEntry>();
            Service61061Client client = null;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S6106RequestCode.GetAppealCount;
                webRequest.Session = App.Session;
                client = new Service61061Client(WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service61061"));
                webRequest.ListData.Add(iRecTimeLength.ToString());
                webRequest.ListData.Add(DateTime.Now.ToString());
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
                    return;
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
                chartApplealCount.DataContext = lstk;
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
        }
    }
}
