//======================================================================
//
//        Copyright © 2016-2017 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        GUID1:                    e8e9b4f0-e5b8-459c-85e9-914a1fda29e4
//        CLR Version:              4.0.30319.42000
//        Name:                     StorageDeviceInfo
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPService91
//        File Name:                StorageDeviceInfo
//
//        Created by Charley at 2016/8/18 11:11:45
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPService91
{
    public class StorageDeviceInfo : ResourceConfigInfo
    {
        public const int RESOURCE_STORAGEDEVICE = 214;

        public const int PRO_DEVICETYPE = 11;
        public const int PRO_ADDRESS = 13;
        public const int PRO_ROOTDIR = 14;

        public int DeviceType { get; set; }
        public string Address { get; set; }
        public string RootDir { get; set; }

        public string AuthName { get; set; }
        public string AuthPassword { get; set; }

        public override string LogInfo()
        {
            string strInfo = base.LogInfo();
            strInfo =
                string.Format("{0};Type:{1};Address:{2};RootDir:{3};AuthName:{4};AuthPassword:***",
                    strInfo,
                    DeviceType,
                    Address,
                    RootDir,
                    AuthName);
            return strInfo;
        }
    }
}
