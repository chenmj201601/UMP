//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    0e54fdcb-6ec4-4799-9885-486e105d573d
//        CLR Version:              4.0.30319.18063
//        Name:                     StorageDeviceInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService03
//        File Name:                StorageDeviceInfo
//
//        created by Charley at 2015/11/2 17:49:09
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPService03
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
