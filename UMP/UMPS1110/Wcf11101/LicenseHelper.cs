using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoiceCyber.Common;
using VoiceCyber.SDKs.Licenses;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Encryptions;

namespace Wcf11101
{
    public class LicenseHelper : IEncryptable
    {

        #region Members

        private List<License> mListLicenses;
       
        private LicConnector mLicConnector;
        private string mHost;
        private int mPort;

        #endregion


        public LicenseHelper()
        {
            mListLicenses = new List<License>();
            mHost = string.Empty;
            mPort = 3070;
        }


        #region Public Members

        public string Host
        {
            get { return mHost; }
            set { mHost = value; }
        }

        public int Port
        {
            get { return mPort; }
            set { mPort = value; }
        }

        public List<License> ListLicenses
        {
            get { return mListLicenses; }
        }

        #endregion


        #region Public Functions

        public void Start()
        {
            try
            {
                if (mLicConnector != null)
                {
                    mLicConnector.Close();
                    mLicConnector = null;
                }
                mLicConnector = new LicConnector();
                mLicConnector.Debug += OnDebug;
                mLicConnector.ServerConnectionEvent += LicConnector_ServerConnectionEvent;
                mLicConnector.MessageReceivedEvent += LicConnector_MessageReceivedEvent;
                mLicConnector.EncryptObject = this;
                //mLicConnector.Client = string.Format("{0}:{1}", CurrentApp.Session.UserID, CurrentApp.Session.UserInfo.UserName);
                mLicConnector.Host = mHost;
                mLicConnector.Port = mPort;
                mLicConnector.BeginConnect();

                OnDebug(LogMode.Info, string.Format("LicenseHelper started.\t{0}\t{1}", mHost, mPort));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("LicenseHelper start fail.\t{0}", ex.Message));
            }
        }

        public void Stop()
        {
            try
            {
                mListLicenses.Clear();

                if (mLicConnector != null)
                {
                    mLicConnector.Close();
                    mLicConnector = null;
                }

                OnDebug(LogMode.Info, string.Format("LicenseHelper stopped."));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("LicenseHelper stop fail.\t{0}", ex.Message));
            }
        }

        public void ClearLicense()
        {
            try
            {
                mListLicenses.Clear();
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("ClearLicense fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region EventHandlers

        void LicConnector_MessageReceivedEvent(object sender, MessageReceivedEventArgs e)
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
                                //ProcessGetLicenses(json);
                                OnDebug(LogMode.Debug, json.ToString());
                                ProcessQuerySpecificLicense(json);
                                break;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, ex.Message);
            }
        }

        void LicConnector_ServerConnectionEvent(int code, string client, string msg)
        {
            try
            {
                switch (code)
                {
                    case Defines.EVT_NET_CONNECTED:
                        OnDebug(LogMode.Info, client, string.Format("Server connected.\t{0}", msg));
                        //QuerySpecificLicense();
                        break;
                    case Defines.EVT_NET_DISCONNECTED:
                        OnDebug(LogMode.Info, client, string.Format("Server disconnected.\t{0}", msg));
                        break;
                    case Defines.EVT_NET_AUTHED:
                        //OnDebug(LogMode.Info, client, string.Format("*Authenticate.\t{0}", msg));
                        //登录成功后，请求获取License
                        //GetLicenses();
                        QuerySpecificLicense();
                        break;
                    default:
                        OnDebug(LogMode.Info, client, string.Format("ConnectionEvent\t{0}\t{1}", code, msg));
                        break;
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, ex.Message);
            }
        }

        #endregion


        #region Operations

        private void GetLicenses()
        {
            try
            {
                var lics = mListLicenses;
                if (lics.Count <= 0)
                {
                    OnDebug(LogMode.Error, string.Format("No license to get."));
                    return;
                }
                JsonObject json = new JsonObject();
                json[LicDefines.KEYWORD_MSG_COMMON_CLASSID] =
                    new JsonProperty(LicDefines.LICENSE_MSG_CLASS_REQRES);
                json[LicDefines.KEYWORD_MSG_COMMON_CLASSDESC] =
                    new JsonProperty(LicUtils.GetClassDesc(LicDefines.LICENSE_MSG_CLASS_REQRES));
                json[LicDefines.KEYWORD_MSG_COMMON_MESSAGEID] =
                    new JsonProperty(LicDefines.LICENSE_MSG_REQUEST_GET_LICENSE);
                json[LicDefines.KEYWORD_MSG_COMMON_MESSAGEDESC] =
                    new JsonProperty(LicUtils.GetMessageDesc(LicDefines.LICENSE_MSG_CLASS_REQRES,
                        LicDefines.LICENSE_MSG_REQUEST_GET_LICENSE));
                json[LicDefines.KEYWORD_MSG_COMMON_CURRENTTIME] =
                    new JsonProperty(DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                json[LicDefines.KEYWORD_MSG_COMMON_DATA] = new JsonProperty(new JsonObject());
                json[LicDefines.KEYWORD_MSG_COMMON_DATA][LicDefines.KEYWORD_MSG_APPINFO_LICENSES] = new JsonProperty();
                for (int i = 0; i < lics.Count; i++)
                {
                    var lic = lics[i];
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
                if (mLicConnector != null)
                {
                    mLicConnector.SendMessage(strMsg);

                    OnDebug(LogMode.Info,
                        string.Format("Send:\tClass: {0};\tMsg: {1}",
                            LicUtils.GetClassDesc(LicDefines.LICENSE_MSG_CLASS_REQRES),
                            LicUtils.GetMessageDesc(LicDefines.LICENSE_MSG_CLASS_REQRES,
                                LicDefines.LICENSE_MSG_REQUEST_GET_LICENSE)));
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("GetLicenses fail.\t{0}", ex.Message));
            }
        }

        private void ProcessGetLicenses(JsonObject json)
        {
            try
            {
                if (json[LicDefines.KEYWORD_MSG_COMMON_DATA] != null
                    && json[LicDefines.KEYWORD_MSG_COMMON_DATA][LicDefines.KEYWORD_MSG_APPINFO_CLIENTLICPOOL] != null
                    &&
                    json[LicDefines.KEYWORD_MSG_COMMON_DATA][LicDefines.KEYWORD_MSG_APPINFO_CLIENTLICPOOL][
                        LicDefines.KEYWORD_MSG_APPINFO_LICENSES] != null)
                {
                    var lics =
                        json[LicDefines.KEYWORD_MSG_COMMON_DATA][LicDefines.KEYWORD_MSG_APPINFO_CLIENTLICPOOL][
                            LicDefines.KEYWORD_MSG_APPINFO_LICENSES];
                    for (int i = 0; i < lics.Count; i++)
                    {
                        var lic = lics[i];
                        string strNo = lic[LicDefines.KEYWORD_MSG_LICENSE_LICENSEID].Value;
                        var temp = mListLicenses.FirstOrDefault(l => l.SerialNo.ToString() == strNo);
                        if (temp != null)
                        {
                            temp.Name = lic[LicDefines.KEYWORD_MSG_LICENSE_DISPLAY].Value;
                            temp.Type = (LicOwnerType)(int)lic[LicDefines.KEYWORD_MSG_LICENSE_OWNERTYPE].Number;
                            temp.DataType = (LicDataType)(int)lic[LicDefines.KEYWORD_MSG_LICENSE_VALUETYPE].Number;
                            temp.Expiration = lic[LicDefines.KEYWORD_MSG_LICENSE_EXPIRATION].Value;
                            temp.MajorID = (int)lic[LicDefines.KEYWORD_MSG_LICENSE_MODULEMARJORTYPEID].Number;
                            temp.MinorID = (int)lic[LicDefines.KEYWORD_MSG_LICENSE_MODULEMINJORTYPEID].Number;
                            temp.Value = lic[LicDefines.KEYWORD_MSG_LICENSE_VALUE].Value;
                        }
                    }

                    //通知License信息改变了
                    OnLicInfoChanged();
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("ProcessGetLicenses fail.\t{0}", ex.Message));
            }
        }

        private void QuerySpecificLicense()
        {
            try
            {
                var lics = mListLicenses;
                if (lics.Count <= 0)
                {
                    OnDebug(LogMode.Error, string.Format("No license to get."));
                    return;
                }
                JsonObject json = new JsonObject();
                json[LicDefines.KEYWORD_MSG_COMMON_CLASSID] =
                    new JsonProperty(LicDefines.LICENSE_MSG_CLASS_REQRES);
                json[LicDefines.KEYWORD_MSG_COMMON_CLASSDESC] =
                    new JsonProperty(LicUtils.GetClassDesc(LicDefines.LICENSE_MSG_CLASS_REQRES));
                json[LicDefines.KEYWORD_MSG_COMMON_MESSAGEID] =
                    new JsonProperty(LicDefines.LICENSE_MSG_REQUEST_QUERY_SPECIFIC_LICENSE);
                json[LicDefines.KEYWORD_MSG_COMMON_MESSAGEDESC] =
                    new JsonProperty(string.Format("\"Query special license\""));
                json[LicDefines.KEYWORD_MSG_COMMON_CURRENTTIME] =
                    new JsonProperty(DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                json[LicDefines.KEYWORD_MSG_REQRES_REQUESTID] =
                    new JsonProperty(LicDefines.LICENSE_MSG_NOTIFY_LICESNE_POOL);
                json[LicDefines.KEYWORD_MSG_COMMON_DATA] = new JsonProperty(new JsonObject());
                json[LicDefines.KEYWORD_MSG_COMMON_DATA][LicDefines.KEYWORD_MSG_APPINFO_LICENSES] = new JsonProperty();
                for (int i = 0; i < lics.Count; i++)
                {
                    var lic = lics[i];
                    lic.ResetValue();

                    JsonObject jsonLic = new JsonObject();
                    jsonLic[LicDefines.KEYWORD_MSG_LICENSE_DISPLAY] = new JsonProperty(lic.Name);
                    jsonLic[LicDefines.KEYWORD_MSG_LICENSE_LICENSEID] = new JsonProperty(lic.SerialNo);
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
                if (mLicConnector != null)
                {
                    mLicConnector.SendMessage(strMsg);
                    OnDebug(LogMode.Info,
                        string.Format("Send:\tClass: {0};\tMsg: {1}",
                            LicUtils.GetClassDesc(LicDefines.LICENSE_MSG_CLASS_REQRES),
                            LicUtils.GetMessageDesc(LicDefines.LICENSE_MSG_CLASS_REQRES,
                                LicDefines.LICENSE_MSG_REQUEST_GET_LICENSE)));
                }
            }
            catch (Exception ex)
            {
                //CurrentApp.WriteLog(string.Format("QuerySpecificLicense fail.\t{0}", ex.Message));
            }
        }

        private void ProcessQuerySpecificLicense(JsonObject json)
        {
            try
            {
                if (json[LicDefines.KEYWORD_MSG_COMMON_DATA] != null)
                {
                    var lics =
                        json[LicDefines.KEYWORD_MSG_COMMON_DATA][LicDefines.KEYWORD_MSG_SERVER_LICPOOL_TOTAL];
                    for (int i = 0; i < lics.Count; i++)
                    {
                        var lic = lics[i];
                        if (lic[LicDefines.KEYWORD_MSG_LICENSE_LICENSEID].Value != null)
                        {
                            string strNo = lic[LicDefines.KEYWORD_MSG_LICENSE_LICENSEID].Value;
                            var temp = mListLicenses.FirstOrDefault(l => l.SerialNo.ToString() == strNo);
                            if (temp != null)
                            {
                                temp.Name = lic[LicDefines.KEYWORD_MSG_LICENSE_DISPLAY].Value;
                                temp.Type = (LicOwnerType)(int)lic[LicDefines.KEYWORD_MSG_LICENSE_OWNERTYPE].Number;
                                temp.DataType = (LicDataType)(int)lic[LicDefines.KEYWORD_MSG_LICENSE_VALUETYPE].Number;
                                temp.Expiration = lic[LicDefines.KEYWORD_MSG_LICENSE_EXPIRATION].Value;
                                temp.MajorID = (int)lic[LicDefines.KEYWORD_MSG_LICENSE_MODULEMARJORTYPEID].Number;
                                temp.MinorID = (int)lic[LicDefines.KEYWORD_MSG_LICENSE_MODULEMINJORTYPEID].Number;
                                temp.Value = lic[LicDefines.KEYWORD_MSG_LICENSE_VALUE].Value;
                                //S1110App.mListLicenseRev.Add(temp);
                            }
                        }
                    }

                    //通知License信息改变了
                    OnLicInfoChanged();
                    //App.GetLicenseList();
                    //CurrentApp.WriteLog(string.Format("ProcessQuerySpecificLicense.ListCount:{1}\t{0}", json, lics.Count));
                }
            }
            catch (Exception ex)
            {
                //OnDebug(LogMode.Error, string.Format("ProcessGetLicenses fail.\t{0}", ex.Message));
                //CurrentApp.WriteLog(string.Format("ProcessQuerySpecificLicense fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region Debug

        public Action<LogMode, string, string> Debug;

        private void OnDebug(LogMode mode, string msg)
        {
            //CurrentApp.WriteLog(string.Format("mode={0},\t{1},\tmsg:{2}", mode, "LicHelper", msg));
        }

        private void OnDebug(LogMode mode, string category, string msg)
        {
            if (Debug != null)
            {
                //Debug(mode, category, msg);
                //CurrentApp.WriteLog(string.Format("mode={0},\tcategory:{1},\tmsg:{2}", mode, category, msg));
            }
        }

        #endregion


        #region License info changed

        public event Action<List<License>> LicInfoChanged;

        private void OnLicInfoChanged()
        {
            if (LicInfoChanged != null)
            {
                LicInfoChanged(mListLicenses);
                //CurrentApp.ShowInfoMessage(mListLicenses.Count.ToString());
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

    }
}
