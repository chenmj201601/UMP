//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ea381ab5-adc9-4a42-b000-45aa7652a0b4
//        CLR Version:              4.0.30319.18408
//        Name:                     WidgetPropertyInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common12002
//        File Name:                WidgetPropertyInfo
//
//        created by Charley at 2016/5/3 16:02:23
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common12002
{
    public class WidgetPropertyInfo
    {
        public long WidgetID { get; set; }
        public int PropertyID { get; set; }
        public string Name { get; set; }
        public WidgetPropertyConvertFormat ConvertFormat { get; set; }
        public WidgetPropertyDataType DataType { get; set; }
        public int SortID { get; set; }
        public long SourceID { get; set; }
        public string DefaultValue { get; set; }
        public string MinValue { get; set; }
        public string MaxValue { get; set; }
    }
}
