using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using Common1600;
using UMPS1600.Service11012;

namespace UMPS1600
{
    /// <summary>
    /// 初始化数据的类 相当于app.cs 获取语言包等信息
    /// </summary>
    class InitApp
    {
        public static SessionInfo Session;
        public static List<LanguageInfo> ListLanguageInfos = new List<LanguageInfo>();

        /// <summary>
        /// 获得语言包
        /// </summary>
        /// <param name="strModuleID"></param>
        /// <returns></returns>
        public static OperationReturn InitLanguageInfos()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;

            try
            {
                if (Session == null || Session.LangTypeInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = (int)S1600WcfError.SessionNotAvailable;
                    return optReturn;
                }
                ListLanguageInfos.Clear();
                VoiceCyber.UMP.Communications.WebRequest webRequest = new VoiceCyber.UMP.Communications.WebRequest();
                webRequest.Code = (int)RequestCode.WSGetLangList;
                webRequest.Session = Session;
                webRequest.ListData.Clear();
                webRequest.ListData.Add(Session.LangTypeInfo.LangID.ToString());
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Format("0{0}16", ConstValue.SPLITER_CHAR));
                webRequest.ListData.Add(string.Format("0{0}1600", ConstValue.SPLITER_CHAR));
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Empty);
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(Session)
                    , WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    optReturn.Code = webReturn.Code;
                    optReturn.Result = false;
                    return optReturn;
                    //ShowExceptionMessage(string.Format("{0}\t{1}", webReturn.Code, webReturn.Message));
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<LanguageInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        //ShowExceptionMessage(string.Format("{0}\t{1}", optReturn.Code, optReturn.Message));
                        return optReturn;
                    }
                    LanguageInfo langInfo = optReturn.Data as LanguageInfo;
                    if (langInfo == null)
                    {
                        //ShowExceptionMessage(string.Format("LanguageInfo is null"));
                        return optReturn;
                    }
                    ListLanguageInfos.Add(langInfo);
                }
                return optReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                return optReturn;
            }
        }

        /// <summary>
        /// 获得显示文字
        /// </summary>
        /// <param name="strLanKey"></param>
        /// <param name="strDefault"></param>
        /// <returns></returns>
        public static string GetLanguage(string strLanKey, string strDefault)
        {
            string strContent = string.Empty;
            List<LanguageInfo> lstLanguage = ListLanguageInfos.Where(p => p.LangID == Session.LangTypeID && p.Name == strLanKey).ToList();
            if (lstLanguage.Count <= 0)
            {
                return strDefault;
            }
            strContent = lstLanguage.First().Display;
            return strContent;
        }
    }
}
