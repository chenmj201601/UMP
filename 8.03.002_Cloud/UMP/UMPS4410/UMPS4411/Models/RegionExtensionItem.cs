//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    445db028-78e3-4eb6-a2c8-692512e765f8
//        CLR Version:              4.0.30319.18408
//        Name:                     RegionExtensionItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4411.Models
//        File Name:                RegionExtensionItem
//
//        created by Charley at 2016/7/17 14:52:15
//        http://www.voicecyber.com 
//
//======================================================================

using VoiceCyber.UMP.Common44101;
using VoiceCyber.UMP.CommonService10;


namespace UMPS4411.Models
{
    public class RegionExtensionItem
    {
        public long ChildRegionID { get; set; }
        public long RegionID { get; set; }
        public long SeatID { get; set; }
        public string SeatName { get; set; }
        public string Extension { get; set; }
        public string MonID { get; set; }
        
        public SeatInfo SeatInfo;
        public ExtensionInfo ExtInfo;
        public MonitorObject MonObject;

        public override string ToString()
        {
            return string.Format("[{0}][{1}][{2}][{3}]", ChildRegionID, RegionID, Extension, MonID);
        }
    }
}
