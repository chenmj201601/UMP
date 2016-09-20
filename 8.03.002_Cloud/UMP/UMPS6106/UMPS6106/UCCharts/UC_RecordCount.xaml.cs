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
    /// UC_RecordCount.xaml 的交互逻辑
    /// </summary>
    public partial class UC_RecordCount 
    {
        //统计时长（统计多长时间内的数据 由主界面传入）
        private int iRecTimeLength;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="iTimeLength">统计时长 ( 统计多长时间内的数据 ) </param>
        public UC_RecordCount(int iTimeLength,int height,int width)
        {
            InitializeComponent();
            RecCountChart.Height = height;
            RecCountChart.Width = width;
            iRecTimeLength = iTimeLength;
            Loaded += UC_RecordCount_Loaded;
        }

        void UC_RecordCount_Loaded(object sender, RoutedEventArgs e)
        {
            PermissionFuncs.GetCotrlUser();
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
            CallInTitle.Title = App.GetLanguageInfo("6106002","Call In");
            CallOutTitle.Title = App.GetLanguageInfo("6106003", "Call Out");
            RecCountChart.Title = App.GetLanguageInfo("6106001", "Call volume statistics");
        }
        #endregion

        #region 获得数据 并整理
        private void ShowChart()
        {
            Service61061Client client = null;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S6106RequestCode.GetRecordCount;
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
                    if (webReturn.Code == (int)S6106WcfErrorCode.NoData)
                    {
                        App.ShowInfoMessage(App.GetLanguageInfo(((int)S6106WcfErrorCode.NoData).ToString(),"No data"));
                        return;
                    }
                    App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData.Count <= 0)
                {
                    return;
                }
                List<KeyValuePair<string, int>> CallInList = new List<KeyValuePair<string, int>>();
                List<KeyValuePair<string, int>> CallOutList = new List<KeyValuePair<string, int>>();
                RecordCountEntry recEntry = null;
                OperationReturn optReturn = null;
                foreach (string str in webReturn.ListData)
                {
                    optReturn = XMLHelper.DeserializeObject<RecordCountEntry>(str);
                    if (!optReturn.Result)
                    {
                        continue;
                    }
                    recEntry = optReturn.Data as RecordCountEntry;
                    string strDate = recEntry.RecDate.Substring(5);
                    KeyValuePair<string, int> item = new KeyValuePair<string, int>(strDate, int.Parse(recEntry.RecCount));
                    if (recEntry.RecDirection == "0")
                    {
                        CallOutList.Add(item);
                    }
                    else if (recEntry.RecDirection == "1")
                    {
                        CallInList.Add(item);
                    }
                }
                var LVarDataSource = new List<List<KeyValuePair<string, int>>>();
                LVarDataSource.Add(CallInList); LVarDataSource.Add(CallOutList);
                RecCountChart.DataContext = LVarDataSource;
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message); 
            }
            finally
            {
                if(client != null)
                {
                    if (client.State == System.ServiceModel.CommunicationState.Opened)
                    {
                        client.Close();
                    }
                }
            }
        }
        #endregion
    }
}
