//======================================================================
//
//        Copyright © 2016-2017 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        GUID1:                    728de029-496a-49ce-8d86-d1849d5952e0
//        CLR Version:              4.0.30319.42000
//        Name:                     RequestParamInfo
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPService12
//        File Name:                RequestParamInfo
//
//        Created by Charley at 2016/9/14 18:12:43
//        http://www.voicecyber.com 
//
//======================================================================

using System.Net;


namespace UMPService12
{
    public class RequestParamInfo
    {
        public string Name { get; set; }
        public string Action { get; set; }
        public string Args { get; set; }
        public string Data { get; set; }
        public HttpListenerContext Context { get; set; }
        public HttpListenerRequest Request { get; set; }
        public HttpListenerResponse Response { get; set; }
    }
}
