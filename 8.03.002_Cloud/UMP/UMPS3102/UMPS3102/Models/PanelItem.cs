//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    35316fa2-7f0c-4c34-9869-266eda3a1376
//        CLR Version:              4.0.30319.18444
//        Name:                     PanelItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Models
//        File Name:                PanelItem
//
//        created by Charley at 2014/11/5 16:18:16
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPS3102.Models
{
    public class PanelItem
    {
        public string Name { get; set; }
        public string ContentID { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }

        public bool IsVisible { get; set; }
        public bool CanClose { get; set; }
    }
}
