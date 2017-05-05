//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    29d46888-b9a5-4635-b737-ad105174e580
//        CLR Version:              4.0.30319.18063
//        Name:                     EncryptInfoConfig
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Models
//        File Name:                EncryptInfoConfig
//
//        created by Charley at 2015/8/4 9:31:06
//        http://www.voicecyber.com 
//
//======================================================================

using VoiceCyber.UMP.Common31021;

namespace UMPS3102.Models
{
    public class EncryptInfoConfig
    {
        public long UserID { get; set; }
        public bool IsRemember { get; set; }
        public string ServerAddress { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Password { get; set; }

        public static EncryptInfoConfig CreateItem(RecordEncryptInfo info)
        {
            EncryptInfoConfig item=new EncryptInfoConfig();
            item.UserID = info.UserID;
            item.IsRemember = info.IsRemember;
            item.StartTime = S3102App.EncryptString(info.StartTime.ToString("yyyy-MM-dd HH:mm:ss"));
            item.EndTime = S3102App.EncryptString(info.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
            item.Password = S3102App.EncryptString(info.Password);
            return item;
        }
    }
}
