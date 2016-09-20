using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using VoiceCyber.Common;
using VoiceCyber.SDKs.Licenses;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Encryptions;
using License = VoiceCyber.SDKs.Licenses.License;

namespace License4NetDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : IEncryptable
    {

        #region Members

        private LicConnector mLicConnector;
        private LicenseHelper mLicHelper;
        private LicConnector mLicChecker;

        private long mLicID;

        #endregion


        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            BtnTest.Click += BtnTest_Click;
            BtnGetLic.Click += BtnGetLic_Click;
            BtnQueryLic.Click += BtnQueryLic_Click;
            BtnShowInfo.Click += BtnShowInfo_Click;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (mLicConnector != null)
            {
                mLicConnector.Close();
                mLicConnector = null;
            }
            if (mLicChecker != null)
            {
                mLicChecker.Close();
                mLicChecker = null;
            }
            if (mLicHelper != null)
            {
                mLicHelper.Stop();
                mLicHelper = null;
            }
        }


        #region EventHandlers

        void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //if (mLicConnector != null)
                //{
                //    mLicConnector.Close();
                //    mLicConnector = null;
                //}
                //mLicConnector = new LicConnector();
                //mLicConnector.Debug += (client, mode, msg) => AppendMessage(string.Format("{0}\t{1}", client, msg));
                //mLicConnector.ServerConnectionEvent += LicConnector_ConnectionEvent;
                //mLicConnector.MessageReceivedEvent += LicConnector_MessageReceiveEvent;
                //mLicConnector.EncryptObject = this;
                //mLicConnector.Client = "LicDemo - 0";
                //mLicConnector.Host = "192.168.4.182";
                //mLicConnector.Port = 3070;
                //mLicConnector.Connect();
                ////mLicConnector.BeginConnect();
                //AppendMessage("End");

                List<License> listLicenses = new List<License>();
                License lic = new License();
                lic.Name = "UserNumber";
                lic.SerialNo = 1100001;
                lic.Expiration = LicDefines.KEYWORD_LICENSE_EXPIRATION_UNLIMITED;
                lic.Type = LicOwnerType.Mono;
                lic.DataType = LicDataType.Number;
                lic.MajorID = 0;
                lic.MinorID = 1;
                lic.RequestValue = "100";
                lic.Value = "0";
                listLicenses.Add(lic);
                lic = new License();
                lic.Name = "OnlineUserNumber";
                lic.SerialNo = 1100002;
                lic.Expiration = LicDefines.KEYWORD_LICENSE_EXPIRATION_UNLIMITED;
                lic.Type = LicOwnerType.Mono;
                lic.DataType = LicDataType.Number;
                lic.MajorID = 0;
                lic.MinorID = 1;
                lic.RequestValue = "1000";
                lic.Value = "0";
                listLicenses.Add(lic);

                if (mLicHelper != null)
                {
                    mLicHelper.Stop();
                    mLicHelper = null;
                }
                mLicHelper = new LicenseHelper();
                mLicHelper.Debug += (mode, cat, msg) => AppendMessage(string.Format("{0}\t{1}", cat, msg));
                mLicHelper.LicInfoChanged += mLicHelper_LicInfoChanged;
                mLicHelper.Host = "192.168.4.182";
                mLicHelper.Port = 3070;
                mLicHelper.ClearLicense();
                for (int i = 0; i < listLicenses.Count; i++)
                {
                    mLicHelper.ListLicenses.Add(listLicenses[i]);
                }
                mLicHelper.Start();
                AppendMessage("End");

            }
            catch (Exception ex)
            {
                AppendMessage(string.Format("Fail.\t{0}", ex.Message));
            }
        }

        void BtnShowInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (mLicHelper != null)
                {
                    var lics = mLicHelper.ListLicenses;
                    for (int i = 0; i < lics.Count; i++)
                    {
                        var lic = lics[i];

                        AppendMessage(string.Format("LicInfo:\t{0}", lic));
                    }
                }
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
            }
        }

        void BtnGetLic_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<License> listLicenses = new List<License>();
                License lic = new License();
                lic.Name = "UserNumber";
                lic.SerialNo = 1100001;
                lic.Expiration = LicDefines.KEYWORD_LICENSE_EXPIRATION_UNLIMITED;
                lic.Type = LicOwnerType.Mono;
                lic.DataType = LicDataType.Number;
                lic.MajorID = 0;
                lic.MinorID = 1;
                lic.RequestValue = "100";
                lic.Value = "0";
                listLicenses.Add(lic);
                lic = new License();
                lic.Name = "OnlineUserNumber";
                lic.SerialNo = 1100002;
                lic.Expiration = LicDefines.KEYWORD_LICENSE_EXPIRATION_UNLIMITED;
                lic.Type = LicOwnerType.Mono;
                lic.DataType = LicDataType.Number;
                lic.MajorID = 0;
                lic.MinorID = 1;
                lic.RequestValue = "1000";
                lic.Value = "0";
                listLicenses.Add(lic);

                if (mLicHelper != null)
                {
                    mLicHelper.Stop();
                    mLicHelper = null;
                }
                mLicHelper = new LicenseHelper();
                mLicHelper.Debug += (mode, cat, msg) => AppendMessage(string.Format("{0}\t{1}", cat, msg));
                mLicHelper.LicInfoChanged += mLicHelper_LicInfoChanged;
                mLicHelper.Host = "192.168.4.182";
                mLicHelper.Port = 3070;
                mLicHelper.ClearLicense();
                for (int i = 0; i < listLicenses.Count; i++)
                {
                    mLicHelper.ListLicenses.Add(listLicenses[i]);
                }
                mLicHelper.Start();
                AppendMessage("End");

            }
            catch (Exception ex)
            {
                AppendMessage(string.Format("Fail.\t{0}", ex.Message));
            }
        }

        void BtnQueryLic_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string strLicID = TxtLicID.Text;
                long licID;
                if (!long.TryParse(strLicID, out licID))
                {
                    AppendMessage(string.Format("LicID invalid"));
                    return;
                }
                mLicID = licID;
                if (mLicChecker != null)
                {
                    mLicChecker.Close();
                    mLicChecker = null;
                }
                mLicChecker = new LicConnector();
                mLicChecker.Debug += (client, mode, msg) => AppendMessage(string.Format("{0}\t{1}", client, msg));
                mLicChecker.ServerConnectionEvent += LicChecker_ConnectionEvent;
                mLicChecker.MessageReceivedEvent += LicChecker_MessageReceiveEvent;
                mLicChecker.EncryptObject = this;
                mLicChecker.Client = "LicChecker - 0";
                mLicChecker.Host = "192.168.4.182";
                mLicChecker.Port = 3070;
                mLicChecker.Connect();
            }
            catch (Exception ex)
            {
                AppendMessage(string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void LicChecker_MessageReceiveEvent(object sender, MessageReceivedEventArgs e)
        {
            try
            {
                var json = new JsonObject(e.StringData);
                int classid = (int)json[LicDefines.KEYWORD_MSG_COMMON_CLASSID].Number;
                int messageid = (int)json[LicDefines.KEYWORD_MSG_COMMON_MESSAGEID].Number;
                switch (classid)
                {
                    case LicDefines.LICENSE_MSG_CLASS_REQRES:
                        switch (messageid)
                        {
                            case LicDefines.LICENSE_MSG_RESPONSE_QUERY_SPECIFIC_LICENSE:
                                ProcessQuerySpecialLicense(json);
                                break;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                AppendMessage(string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void LicChecker_ConnectionEvent(int code, string client, string msg)
        {
            switch (code)
            {
                case Defines.EVT_NET_CONNECTED:
                    AppendMessage(string.Format("{0}\tServer connected.\t{1}", client, msg));
                    break;
                case Defines.EVT_NET_DISCONNECTED:
                    AppendMessage(string.Format("{0}\tServer disconnected.\t{1}", client, msg));
                    break;
                case Defines.EVT_NET_AUTHED:
                    AppendMessage(string.Format("{0}\tAuthenticate.\t{1}", client, msg));

                    QueryLicInfos();
                    break;
                default:
                    AppendMessage(string.Format("{0}\tConnectionEvent\t{1}\t{2}", client, code, msg));
                    break;
            }
        }

        private void mLicHelper_LicInfoChanged(List<License> listLics)
        {
            try
            {
                AppendMessage(string.Format("LicInfo changed"));

                if (listLics != null)
                {
                    for (int i = 0; i < listLics.Count; i++)
                    {
                        AppendMessage(string.Format("LicInfo:\t{0}", listLics[i]));
                    }
                }
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
            }
        }

        #endregion


        #region Operations

        private void QueryLicInfos()
        {
            try
            {
                List<License> listLics = new List<License>();
                License lic = new License();
                lic.SerialNo = mLicID;
                lic.DataType = LicDataType.Number;
                listLics.Add(lic);

                JsonObject json = new JsonObject();
                json[LicDefines.KEYWORD_MSG_COMMON_CLASSID] =
                    new JsonProperty(LicDefines.LICENSE_MSG_CLASS_REQRES);
                json[LicDefines.KEYWORD_MSG_COMMON_CLASSDESC] =
                    new JsonProperty(LicUtils.GetClassDesc(LicDefines.LICENSE_MSG_CLASS_REQRES));
                json[LicDefines.KEYWORD_MSG_COMMON_MESSAGEID] =
                    new JsonProperty(LicDefines.LICENSE_MSG_REQUEST_QUERY_SPECIFIC_LICENSE);
                json[LicDefines.KEYWORD_MSG_COMMON_MESSAGEDESC] =
                    new JsonProperty(LicUtils.GetMessageDesc(LicDefines.LICENSE_MSG_CLASS_REQRES,
                        LicDefines.LICENSE_MSG_REQUEST_QUERY_SPECIFIC_LICENSE));
                json[LicDefines.KEYWORD_MSG_COMMON_CURRENTTIME] =
                    new JsonProperty(DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                json[LicDefines.KEYWORD_MSG_COMMON_DATA] = new JsonProperty(new JsonObject());
                json[LicDefines.KEYWORD_MSG_COMMON_DATA][LicDefines.KEYWORD_MSG_APPINFO_LICENSES] = new JsonProperty();
                for (int i = 0; i < listLics.Count; i++)
                {
                    lic = listLics[i];
                    lic.ResetValue();

                    JsonObject jsonLic = new JsonObject();
                    jsonLic[LicDefines.KEYWORD_MSG_LICENSE_DISPLAY] = new JsonProperty(lic.Name);
                    jsonLic[LicDefines.KEYWORD_MSG_LICENSE_EXPIRATION] = new JsonProperty(lic.Expiration);
                    jsonLic[LicDefines.KEYWORD_MSG_LICENSE_LICENSEID] = new JsonProperty(lic.SerialNo);
                    jsonLic[LicDefines.KEYWORD_MSG_LICENSE_MODULEMARJORTYPEID] = new JsonProperty(lic.MajorID);
                    jsonLic[LicDefines.KEYWORD_MSG_LICENSE_MODULEMINJORTYPEID] = new JsonProperty(lic.MinorID);
                    jsonLic[LicDefines.KEYWORD_MSG_LICENSE_OWNERTYPE] = new JsonProperty((int)lic.Type);
                    jsonLic[LicDefines.KEYWORD_MSG_LICENSE_VALUETYPE] = new JsonProperty((int)lic.DataType);
                    if (lic.DataType == LicDataType.Number)
                    {
                        jsonLic[LicDefines.KEYWORD_MSG_LICENSE_VALUE] = new JsonProperty(lic.RequestValue);
                    }
                    else
                    {
                        jsonLic[LicDefines.KEYWORD_MSG_LICENSE_VALUE] = new JsonProperty(string.Format("\"{0}\"", lic.RequestValue));
                    }
                    json[LicDefines.KEYWORD_MSG_COMMON_DATA][LicDefines.KEYWORD_MSG_APPINFO_LICENSES].Add(jsonLic);
                }
                string strMsg = json.ToString();
                if (mLicChecker != null)
                {
                    mLicChecker.SendMessage(strMsg);

                    AppendMessage(string.Format("Send:\tClass: {0};\tMsg: {1}",
                            LicUtils.GetClassDesc(LicDefines.LICENSE_MSG_CLASS_REQRES),
                            LicUtils.GetMessageDesc(LicDefines.LICENSE_MSG_CLASS_REQRES,
                                LicDefines.LICENSE_MSG_REQUEST_QUERY_SPECIFIC_LICENSE)));
                }
            }
            catch (Exception ex)
            {
                AppendMessage(string.Format("QueryLicInfos fail.\t{0}", ex.Message));
            }
        }

        private void ProcessQuerySpecialLicense(JsonObject json)
        {
            try
            {
                if (json[LicDefines.KEYWORD_MSG_COMMON_DATA] != null)
                {
                    if (json[LicDefines.KEYWORD_MSG_COMMON_DATA][LicDefines.KEYWORD_MSG_SERVER_LICPOOL_FREE] != null)
                    {
                        var
                            lics = json[LicDefines.KEYWORD_MSG_COMMON_DATA][LicDefines.KEYWORD_MSG_SERVER_LICPOOL_FREE];
                        for (int i = 0; i < lics.Count; i++)
                        {
                            var lic = lics[i];

                            var temp = new License();
                            temp.SerialNo = (long) lic[LicDefines.KEYWORD_MSG_LICENSE_LICENSEID].Number;
                            temp.Name = lic[LicDefines.KEYWORD_MSG_LICENSE_DISPLAY].Value;
                            temp.Type = (LicOwnerType)(int)lic[LicDefines.KEYWORD_MSG_LICENSE_OWNERTYPE].Number;
                            temp.DataType = (LicDataType)(int)lic[LicDefines.KEYWORD_MSG_LICENSE_VALUETYPE].Number;
                            temp.Expiration = lic[LicDefines.KEYWORD_MSG_LICENSE_EXPIRATION].Value;
                            temp.MajorID = (int)lic[LicDefines.KEYWORD_MSG_LICENSE_MODULEMARJORTYPEID].Number;
                            temp.MinorID = (int)lic[LicDefines.KEYWORD_MSG_LICENSE_MODULEMINJORTYPEID].Number;
                            temp.Value = lic[LicDefines.KEYWORD_MSG_LICENSE_VALUE].Value;

                            AppendMessage(string.Format("QueryResult Free: {0}", temp));
                        }
                    }
                    if (json[LicDefines.KEYWORD_MSG_COMMON_DATA][LicDefines.KEYWORD_MSG_SERVER_LICPOOL_TOTAL] != null)
                    {
                        var
                            lics = json[LicDefines.KEYWORD_MSG_COMMON_DATA][LicDefines.KEYWORD_MSG_SERVER_LICPOOL_TOTAL];
                        for (int i = 0; i < lics.Count; i++)
                        {
                            var lic = lics[i];

                            var temp = new License();
                            temp.SerialNo = (long)lic[LicDefines.KEYWORD_MSG_LICENSE_LICENSEID].Number;
                            temp.Name = lic[LicDefines.KEYWORD_MSG_LICENSE_DISPLAY].Value;
                            temp.Type = (LicOwnerType)(int)lic[LicDefines.KEYWORD_MSG_LICENSE_OWNERTYPE].Number;
                            temp.DataType = (LicDataType)(int)lic[LicDefines.KEYWORD_MSG_LICENSE_VALUETYPE].Number;
                            temp.Expiration = lic[LicDefines.KEYWORD_MSG_LICENSE_EXPIRATION].Value;
                            temp.MajorID = (int)lic[LicDefines.KEYWORD_MSG_LICENSE_MODULEMARJORTYPEID].Number;
                            temp.MinorID = (int)lic[LicDefines.KEYWORD_MSG_LICENSE_MODULEMINJORTYPEID].Number;
                            temp.Value = lic[LicDefines.KEYWORD_MSG_LICENSE_VALUE].Value;

                            AppendMessage(string.Format("QueryResult Total: {0}", temp));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppendMessage(string.Format("ProcessQuerySpecial fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region Encrypt and Decrypt

        public string DecryptString(string source, int mode)
        {
            //return source;
            return ServerAESEncryption.DecryptString(source, (EncryptionMode)mode, Encoding.ASCII);
        }

        public string DecryptString(string source)
        {
            return source;
        }

        public string EncryptString(string source, int mode)
        {
            //return source;
            return ServerAESEncryption.EncryptString(source, (EncryptionMode)mode, Encoding.ASCII);
        }

        public string EncryptString(string source)
        {
            return source;
        }

        public string DecryptString(string source, int mode, Encoding encoding)
        {
            return ServerAESEncryption.DecryptString(source, (EncryptionMode)mode, encoding);
        }

        public string EncryptString(string source, int mode, Encoding encoding)
        {
            return ServerAESEncryption.EncryptString(source, (EncryptionMode)mode, encoding);
        }

        public byte[] DecryptBytes(byte[] source, int mode)
        {
            return ServerAESEncryption.DecryptBytes(source, (EncryptionMode)mode);
        }

        public byte[] DecryptBytes(byte[] source)
        {
            return source;
        }

        public byte[] EncryptBytes(byte[] source, int mode)
        {
            return ServerAESEncryption.EncryptBytes(source, (EncryptionMode)mode);
        }

        public byte[] EncryptBytes(byte[] source)
        {
            return source;
        }

        #endregion


        #region Others

        private void AppendMessage(string msg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                TxtMsg.AppendText(string.Format("{0}\t{1}\r\n", DateTime.Now.ToString("HH:mm:ss"), msg));
                TxtMsg.ScrollToEnd();
            }));
        }

        #endregion

    }
}
