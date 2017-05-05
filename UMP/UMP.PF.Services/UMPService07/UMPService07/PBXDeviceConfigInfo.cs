//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    83c6510f-8fa5-4809-8be0-6646676e7093
//        CLR Version:              4.0.30319.18063
//        Name:                     PBXDeviceConfigInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService07
//        File Name:                PBXDeviceConfigInfo
//
//        created by Charley at 2015/11/24 19:55:18
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPService07
{
    public class PBXDeviceConfigInfo : ResourceConfigInfo
    {
        public string XmlKey { get; set; }
        public int CTIType { get; set; }
        public int DeviceType { get; set; }
        public int MonitorType { get; set; }
        public string DeviceName { get; set; }

        public override string LogInfo()
        {
            string strInfo = base.LogInfo();
            strInfo =
                string.Format(
                    "{0};XmlKey:{1};CTIType:{2};DeviceType:{3};MonitorType:{4};DeviceName:{5};",
                    strInfo,
                    XmlKey,
                    CTIType,
                    DeviceType,
                    MonitorType,
                    DeviceName);
            return strInfo;
        }
    }
}
