//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3ea071aa-92f6-49c8-a9f5-19a96bcf52ac
//        CLR Version:              4.0.30319.18063
//        Name:                     DECServerInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService04
//        File Name:                DECServerInfo
//
//        created by Charley at 2015/6/25 12:55:08
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPService04
{
    public class DECServerInfo : ServiceConfigInfo
    {
        public const int RESOURCE_DATAEXCHANGECENTER = 212;

        public override string LogInfo()
        {
            string strInfo = base.LogInfo();
            strInfo = string.Format("{0};",
                strInfo);
            return strInfo;
        }
    }
}
