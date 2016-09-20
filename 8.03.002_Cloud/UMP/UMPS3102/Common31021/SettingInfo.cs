//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    69a3c2da-93db-4251-9cef-cb9fbf7780b5
//        CLR Version:              4.0.30319.18444
//        Name:                     SettingInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common31021
//        File Name:                SettingInfo
//
//        created by Charley at 2014/11/19 17:06:58
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common31021
{
    public class SettingInfo
    {
        public long UserID { get; set; }
        public int ParamID { get; set; }
        public int GroupID { get; set; }
        public int SortID { get; set; }
        public int DataType { get; set; }
        public string Description { get; set; }
        public int IntValue { get; set; }
        public string StringValue { get; set; }
        public object ObjValue { get; set; }
    }
}
