//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d647c14a-eb44-4ba4-a272-170b9e6f5d10
//        CLR Version:              4.0.30319.18408
//        Name:                     ComboItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdateTool.Models
//        File Name:                ComboItem
//
//        created by Charley at 2016/5/31 17:26:24
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPUpdateTool.Models
{
    public class ComboItem
    {
        public string Name { get; set; }
        public string Display { get; set; }
        public int IntValue { get; set; }
        public string StrValue { get; set; }

        public object Data { get; set; }

        public override string ToString()
        {
            return Display;
        }
    }
}
