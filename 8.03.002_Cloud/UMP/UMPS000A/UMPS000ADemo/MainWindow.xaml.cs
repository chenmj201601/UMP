using System;
using System.ServiceModel;
using System.Windows;
using UMPS000ADemo.Wcf000A1;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common000A1;

namespace UMPS000ADemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            BtnTest.Click += BtnTest_Click;
            BtnUpdateRecordInfo.Click += BtnUpdateRecordInfo_Click;
            BtnGetRecordUrl.Click += BtnGetRecordUrl_Click;
            BtnQueryRecord.Click += BtnQueryRecord_Click;
            BtnGetResource.Click += BtnGetResource_Click;
            BtnGetOpt.Click += BtnGetOpt_Click;
            BtnGetLang.Click += BtnGetLang_Click;
            BtnInsertRecord.Click += BtnInsertRecord_Click;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                JsonObject json = new JsonObject();
                json["E"] = new JsonProperty();
                json["E"].Add("8  g");
                json["E"].Add("7  ");
                string str = json.ToString();
                AppendMessage(str);
                JsonObject temp = new JsonObject(str);
                int count = temp["E"].Count;
                AppendMessage(count.ToString());
            }
            catch (Exception ex)
            {
                AppendMessage(string.Format("Fail.\t{0}", ex.Message));
            }
        }

        void BtnGetLang_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SDKRequest request = new SDKRequest();
                request.Code = (int)S000ACodes.GetLangList;  //获取语言列表
                request.ListData.Add("2052");
                request.ListData.Add(string.Empty);
                request.ListData.Add("11");
                request.ListData.Add("1110");
                request.ListData.Add(string.Empty);
                request.ListData.Add(string.Empty);
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.MaxBufferSize = int.MaxValue;
                binding.MaxBufferPoolSize = int.MaxValue;
                binding.SendTimeout = new TimeSpan(0, 10, 0);
                binding.ReceiveTimeout = new TimeSpan(0, 20, 0);
                string url = string.Format("{0}://{1}:{2}/Wcf2Client/{3}.svc",
                    "http",
                    "192.168.6.7",
                    8081,
                    "Service000A1");
                EndpointAddress address = new EndpointAddress(new Uri(url, UriKind.RelativeOrAbsolute));
                Service000A1Client client = new Service000A1Client(binding, address);
                SDKReturn sdkReturn = client.DoOperation(request);
                client.Close();
                if (!sdkReturn.Result)
                {
                    AppendMessage(string.Format("WSFail.\t{0}\t{1}", sdkReturn.Code, sdkReturn.Message));
                    return;
                }
                if (sdkReturn.ListData == null)
                {
                    AppendMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < sdkReturn.ListData.Count; i++)
                {
                    AppendMessage(string.Format("ListData{0}:{1}", i, sdkReturn.ListData[i]));
                }
                AppendMessage("End");
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
            }
        }

        void BtnGetOpt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SDKRequest request = new SDKRequest();
                request.Code = (int)S000ACodes.GetUserOptList;  //获取操作权限
                request.ListData.Add("1060000000000000001");
                request.ListData.Add("44");
                request.ListData.Add("4401");
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.MaxBufferSize = int.MaxValue;
                binding.MaxBufferPoolSize = int.MaxValue;
                binding.SendTimeout = new TimeSpan(0, 10, 0);
                binding.ReceiveTimeout = new TimeSpan(0, 20, 0);
                string url = string.Format("{0}://{1}:{2}/Wcf2Client/{3}.svc",
                    "http",
                    "192.168.6.7",
                    8081,
                    "Service000A1");
                EndpointAddress address = new EndpointAddress(new Uri(url, UriKind.RelativeOrAbsolute));
                Service000A1Client client = new Service000A1Client(binding, address);
                SDKReturn sdkReturn = client.DoOperation(request);
                client.Close();
                if (!sdkReturn.Result)
                {
                    AppendMessage(string.Format("WSFail.\t{0}\t{1}", sdkReturn.Code, sdkReturn.Message));
                    return;
                }
                if (sdkReturn.ListData == null)
                {
                    AppendMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < sdkReturn.ListData.Count; i++)
                {
                    AppendMessage(string.Format("ListData{0}:{1}", i, sdkReturn.ListData[i]));
                }
                AppendMessage("End");
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
            }
        }

        void BtnGetResource_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SDKRequest request = new SDKRequest();
                request.Code = (int)S000ACodes.GetUserCtlObjList;  //获取管理对象列表
                request.ListData.Add("1020000000000000001");
                request.ListData.Add("0");
                request.ListData.Add("102");
                request.ListData.Add("1010000000000000001");
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.MaxBufferSize = int.MaxValue;
                binding.MaxBufferPoolSize = int.MaxValue;
                binding.SendTimeout = new TimeSpan(0, 10, 0);
                binding.ReceiveTimeout = new TimeSpan(0, 20, 0);
                string url = string.Format("{0}://{1}:{2}/Wcf2Client/{3}.svc",
                    "http",
                    "192.168.6.7",
                    8081,
                    "Service000A1");
                EndpointAddress address = new EndpointAddress(new Uri(url, UriKind.RelativeOrAbsolute));
                Service000A1Client client = new Service000A1Client(binding, address);
                SDKReturn sdkReturn = client.DoOperation(request);
                client.Close();
                if (!sdkReturn.Result)
                {
                    AppendMessage(string.Format("WSFail.\t{0}\t{1}", sdkReturn.Code, sdkReturn.Message));
                    return;
                }
                if (sdkReturn.ListData == null)
                {
                    AppendMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < sdkReturn.ListData.Count; i++)
                {
                    AppendMessage(string.Format("ListData{0}:{1}", i, sdkReturn.ListData[i]));
                }
                AppendMessage("End");
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
            }
        }

        void BtnQueryRecord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                JsonObject json = new JsonObject();

                //json["Extension"] = new JsonProperty(string.Format("\"{0}\"", "8021"));       //Single

                json["Extension"] = new JsonProperty();                                         //Multi
                json["Extension"].Add("8021");
                json["Extension"].Add("8022");

                SDKRequest request = new SDKRequest();
                request.Code = (int)S000ACodes.GetLogRecordData;  //获取录音记录信息
                request.ListData.Add("1020000000000000001");
                request.ListData.Add("0");
                request.ListData.Add("0");
                request.ListData.Add(DateTime.Parse("2014/1/1").ToString("yyyy-MM-dd HH:mm:ss"));
                request.ListData.Add(DateTime.Parse("2199/12/31").ToString("yyyy-MM-dd HH:mm:ss"));
                request.ListData.Add("10");
                request.ListData.Add("2");
                request.ListData.Add(json.ToString());
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.MaxBufferSize = int.MaxValue;
                binding.MaxBufferPoolSize = int.MaxValue;
                binding.SendTimeout = new TimeSpan(0, 10, 0);
                binding.ReceiveTimeout = new TimeSpan(0, 20, 0);
                string url = string.Format("{0}://{1}:{2}/Wcf2Client/{3}.svc",
                    "http",
                    "192.168.6.15",
                    8081,
                    "Service000A1");
                EndpointAddress address = new EndpointAddress(new Uri(url, UriKind.RelativeOrAbsolute));
                Service000A1Client client = new Service000A1Client(binding, address);
                SDKReturn sdkReturn = client.DoOperation(request);
                client.Close();
                if (!sdkReturn.Result)
                {
                    AppendMessage(string.Format("WSFail.\t{0}\t{1}", sdkReturn.Code, sdkReturn.Message));
                    return;
                }
                if (sdkReturn.ListData == null)
                {
                    AppendMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < sdkReturn.ListData.Count; i++)
                {
                    AppendMessage(string.Format("ListData{0}:{1}", i, sdkReturn.ListData[i]));
                }
                AppendMessage(string.Format("Sql:{0}", sdkReturn.Message));
                AppendMessage("End");
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
            }
        }

        void BtnGetRecordUrl_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                JsonObject jsonOption = new JsonObject();
                jsonOption[S000AConsts.OPTION_FIELD_CONVERTWAVEFORMAT] = new JsonProperty(string.Format("3"));

                SDKRequest request = new SDKRequest();
                request.Code = (int)S000ACodes.GetLogRecordUrl;  //获取录音下载地址
                request.ListData.Add("1020000000000000001");
                request.ListData.Add("0");
                request.ListData.Add("1604010700000000001");
                request.ListData.Add(string.Empty);
                request.ListData.Add("a");
                //request.ListData.Add(string.Empty);
                request.ListData.Add(jsonOption.ToString());
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.MaxBufferSize = int.MaxValue;
                binding.MaxBufferPoolSize = int.MaxValue;
                binding.SendTimeout = new TimeSpan(0, 10, 0);
                binding.ReceiveTimeout = new TimeSpan(0, 20, 0);
                string url = string.Format("{0}://{1}:{2}/Wcf2Client/{3}.svc",
                    "http",
                    "192.168.4.166",
                    8081,
                    "Service000A1");
                EndpointAddress address = new EndpointAddress(new Uri(url, UriKind.RelativeOrAbsolute));
                Service000A1Client client = new Service000A1Client(binding, address);
                SDKReturn sdkReturn = client.DoOperation(request);
                client.Close();
                if (!sdkReturn.Result)
                {
                    AppendMessage(string.Format("WSFail.\t{0}\t{1}", sdkReturn.Code, sdkReturn.Message));
                    return;
                }
                if (sdkReturn.ListData == null)
                {
                    AppendMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < sdkReturn.ListData.Count; i++)
                {
                    AppendMessage(string.Format("ListData{0}:{1}", i, sdkReturn.ListData[i]));
                }
                AppendMessage("End");
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
            }
        }

        void BtnUpdateRecordInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                XmlMappingItem condition = new XmlMappingItem();
                condition.Name = "Agent";
                condition.Column = "C039";
                condition.DataType = (int)DBDataType.NVarchar;
                condition.Value = "8021";
                JsonObject json = new JsonObject();
                json[condition.Name] = new JsonProperty(string.Format("\"{0}\"", condition.Value));

                SDKRequest request = new SDKRequest();
                request.Code = (int)S000ACodes.UpdateLogRecordInfo;  //获取记录信息
                request.ListData.Add("1020000000000000001");
                request.ListData.Add("0");
                request.ListData.Add("1510220600000000001");
                request.ListData.Add(json.ToString());
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.MaxBufferSize = int.MaxValue;
                binding.MaxBufferPoolSize = int.MaxValue;
                binding.SendTimeout = new TimeSpan(0, 10, 0);
                binding.ReceiveTimeout = new TimeSpan(0, 20, 0);
                string url = string.Format("{0}://{1}:{2}/Wcf2Client/{3}.svc",
                    "http",
                    "192.168.6.7",
                    8081,
                    "Service000A1");
                EndpointAddress address = new EndpointAddress(new Uri(url, UriKind.RelativeOrAbsolute));
                Service000A1Client client = new Service000A1Client(binding, address);
                SDKReturn sdkReturn = client.DoOperation(request);
                client.Close();
                if (!sdkReturn.Result)
                {
                    AppendMessage(string.Format("WSFail.\t{0}\t{1}", sdkReturn.Code, sdkReturn.Message));
                    return;
                }
                if (sdkReturn.ListData == null)
                {
                    AppendMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < sdkReturn.ListData.Count; i++)
                {
                    AppendMessage(string.Format("ListData{0}:{1}", i, sdkReturn.ListData[i]));
                }
                AppendMessage("End");
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
            }
        }

        void BtnInsertRecord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime now = DateTime.Now;

                SDKRequest request = new SDKRequest();
                request.Code = (int)S000ACodes.InsertLogRecord;  //插入记录信息
                request.ListData.Add("1020000000000000001");
                request.ListData.Add(string.Format("ASCHN0000000000030000{0}001",
                    DateTime.Now.ToString("yyyyMMddHHmmss")));
                request.ListData.Add(now.AddSeconds(-20).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                request.ListData.Add(now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                request.ListData.Add("3");
                request.ListData.Add("0");
                request.ListData.Add("8021");
                request.ListData.Add("8021");
                request.ListData.Add(string.Empty);
                request.ListData.Add(string.Empty);
                request.ListData.Add("0");
                request.ListData.Add("20");
                request.ListData.Add("0");
                request.ListData.Add("192.168.6.112");
                request.ListData.Add("0");
                request.ListData.Add("AS");
                request.ListData.Add("CHN");
                request.ListData.Add("0");
                request.ListData.Add("00000");
                request.ListData.Add("71015add-8dd7-478a-b4ca-bfd660bf020e");
                request.ListData.Add(string.Empty);
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.MaxBufferSize = int.MaxValue;
                binding.MaxBufferPoolSize = int.MaxValue;
                binding.SendTimeout = new TimeSpan(0, 10, 0);
                binding.ReceiveTimeout = new TimeSpan(0, 20, 0);
                string url = string.Format("{0}://{1}:{2}/Wcf2Client/{3}.svc",
                    "http",
                    "192.168.6.15",
                    8081,
                    "Service000A1");
                EndpointAddress address = new EndpointAddress(new Uri(url, UriKind.RelativeOrAbsolute));
                Service000A1Client client = new Service000A1Client(binding, address);
                SDKReturn sdkReturn = client.DoOperation(request);
                client.Close();
                if (!sdkReturn.Result)
                {
                    AppendMessage(string.Format("WSFail.\t{0}\t{1}", sdkReturn.Code, sdkReturn.Message));
                    return;
                }
                if (sdkReturn.ListData == null)
                {
                    AppendMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < sdkReturn.ListData.Count; i++)
                {
                    AppendMessage(string.Format("ListData{0}:{1}", i, sdkReturn.ListData[i]));
                }
                AppendMessage("End");
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
            }
        }

        private void AppendMessage(string msg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                TxtMsg.AppendText(string.Format("{0}\t{1}\r\n", DateTime.Now.ToString("HH:mm:ss.f"), msg));
                TxtMsg.ScrollToEnd();
            }));
        }
    }
}
