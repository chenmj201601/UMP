//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3341564b-5306-4d61-a13d-975757acba5f
//        CLR Version:              4.0.30319.18444
//        Name:                     PropertyValueChangedEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models
//        File Name:                PropertyValueChangedEventArgs
//
//        created by Charley at 2015/1/19 11:19:51
//        http://www.voicecyber.com 
//
//======================================================================

using VoiceCyber.UMP.Common11101;

namespace UMPS1110.Models
{
    public class PropertyValueChangedEventArgs
    {
        public ConfigObject ConfigObject { get; set; }
        public ResourcePropertyInfoItem PropertyItem { get; set; }
        public ObjectPropertyInfo PropertyInfo { get; set; }
        public ResourceProperty PropetyValue { get; set; }
        public string Value { get; set; }
        public PropertyValueEnumItem ValueItem { get; set; }
        public bool IsInit { get; set; }
    }
}
