//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    9bc77926-47c4-4764-986b-db8f163294cb
//        CLR Version:              4.0.30319.42000
//        Name:                     IWidgetView
//        Computer:                 DESKTOP-AH05P0E
//        Organization:             VoiceCyber
//        Namespace:                UMPS1206
//        File Name:                IWidgetView
//
//        created by Charley at 2016/3/9 14:06:08
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using UMPS1206.Models;
using VoiceCyber.UMP.Common;


namespace UMPS1206
{
    public interface IWidgetView
    {
        IList<BasicDataInfo> ListBasicDataInfos { get; set; }
        WidgetItem WidgetItem { get; set; }
        bool IsCenter { get; set; }
        bool IsFull { get; set; }

        void Refresh();
    }
}
