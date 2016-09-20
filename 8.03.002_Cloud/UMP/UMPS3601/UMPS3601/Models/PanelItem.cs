//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    55b01965-cfaf-4675-a16e-36c5d5588ecc
//        CLR Version:              4.0.30319.18063
//        Name:                     PanelItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3101.Models
//        File Name:                PanelItem
//
//        created by Charley at 2015/11/5 11:16:53
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPS3601.Models
{
    public class PanelItem
    {
        public int PanelId { get; set; }
        public string Name { get; set; }
        public string ContentId { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }

        public bool IsVisible { get; set; }
        public bool CanClose { get; set; }
    }
}
