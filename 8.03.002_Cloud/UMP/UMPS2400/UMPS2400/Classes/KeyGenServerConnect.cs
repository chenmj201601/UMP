using Common2400;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UMPS2400.Service24011;
using VoiceCyber.UMP.Communications;

namespace UMPS2400.Classes
{
    public class KeyGenServerConnect
    {
        public static bool TryConnectToGeneratorServer(string AStrServerName, string AStrServerPort, ref string AStrReturn)
        {
            WebRequest webRequest = new WebRequest();
            webRequest.Code = (int)S2400RequestCode.TryConnToKeyGenServer;
            webRequest.Session =App.CurrentApp.Session;
            webRequest.ListData.Add(AStrServerName);
            webRequest.ListData.Add(AStrServerPort);
            Service24011Client client = new Service24011Client(WebHelper.CreateBasicHttpBinding(App.CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(App.CurrentApp.Session.AppServerInfo, "Service24011"));
            WebReturn webReturn = client.DoOperation(webRequest);
            App.CurrentApp.MonitorHelper.AddWebReturn(webReturn);
            client.Close();
            if (!webReturn.Result)
            {
                AStrReturn = string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message);
                return false;
            }
            return true;
        }
    }
}
