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
    /// UC_AvgScore.xaml 的交互逻辑
    /// </summary>
    public partial class UC_AvgScore
    {
        //统计时长（统计多长时间内的数据 由主界面传入）
        private int iRecTimeLength;

        public UC_AvgScore(int iTimeLength, int height, int width)
        {
            InitializeComponent();
            iRecTimeLength = iTimeLength;
            chartAvgScore.Width = width;
            chartAvgScore.Height = height;
            Loaded += UC_AvgScore_Loaded;
        }

        void UC_AvgScore_Loaded(object sender, RoutedEventArgs e)
        {
            InitLanguage();
            ShowChart();
        }

        #region Override
        public override void ChangeLanguage()
        {
            base.ChangeLanguage();
            InitLanguage();
        }
        #endregion

        #region Init
        private void InitLanguage()
        {
            chartAvgScore.Title = App.GetLanguageInfo("6106018", "Average quality inspection");
            lineAvgScore.Title = App.GetLanguageInfo("6106019", "Mean score");
        }
        #endregion

        private void ShowChart()
        {
            Service61061Client client = null;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S6106RequestCode.GetAvgScore;
                webRequest.Session = App.Session;
                webRequest.ListData.Add(iRecTimeLength.ToString());
                webRequest.ListData.Add(DateTime.Now.ToString());
                client = new Service61061Client(WebHelper.CreateBasicHttpBinding(App.Session),
                      WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service61061"));
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
                if (webReturn.ListData.Count <= 0)
                {
                    return;
                }
                List<string> lstRecords = webReturn.ListData as List<string>;
                OperationReturn optReturn = null;
                List<KeyValueEntry> lstk = new List<KeyValueEntry>();
                for (int i = 0; i < lstRecords.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<KeyValueEntry>(lstRecords[i]);
                    if (optReturn.Result)
                    {
                        lstk.Add(optReturn.Data as KeyValueEntry);
                    }
                }
                lineAvgScore.DataContext = lstk;
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
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
