//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7020f2d6-af38-4081-9369-6855e630f3a9
//        CLR Version:              4.0.30319.18444
//        Name:                     NtidrvPathObject
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models.ConfigObjects
//        File Name:                NtidrvPathObject
//
//        created by Charley at 2015/4/16 19:05:43
//        http://www.voicecyber.com 
//
//======================================================================

using VoiceCyber.UMP.Common11101;

namespace UMPS1110.Models.ConfigObjects
{
    public class NtidrvPathObject : ConfigObject
    {
        public override void SetBasicPropertyValues()
        {
            base.SetBasicPropertyValues();

            ResourceProperty propertyValue;
            for (int i = 0; i < ListProperties.Count; i++)
            {
                propertyValue = ListProperties[i];
                switch (propertyValue.PropertyID)
                {
                    case 14:
                        propertyValue.Value = ID.ToString();
                        break;
                }
            }
        }
    }
}
