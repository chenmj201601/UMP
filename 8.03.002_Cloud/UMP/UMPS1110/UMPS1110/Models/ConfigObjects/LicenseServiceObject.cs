//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b2652818-8c0e-4a96-b9f4-ebbfaec73623
//        CLR Version:              4.0.30319.18444
//        Name:                     LicenseServiceObject
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models
//        File Name:                LicenseServiceObject
//
//        created by Charley at 2015/4/13 17:37:03
//        http://www.voicecyber.com 
//
//======================================================================

using VoiceCyber.UMP.Common11101;

namespace UMPS1110.Models.ConfigObjects
{
    public class LicenseServiceObject : ServiceObject, IMasterSlaverObject
    {
        public bool IsMaster { get; set; }

        public override void GetNameAndDescription()
        {
            base.GetNameAndDescription();

            //[主/备] HostAddress
            if (IsMaster)
            {
                Name = string.Format("[{0}] {1}", CurrentApp.GetLanguageInfo("1110110", "Master"), HostAddress);
                Description = string.Format("[{0}] {1}\r\n{2}", CurrentApp.GetLanguageInfo("1110110", "Master"), HostAddress, ObjectID);
            }
            else
            {
                Name = string.Format("[{0}] {1}", CurrentApp.GetLanguageInfo("1110111", "Slaver"), HostAddress);
                Description = string.Format("[{0}] {1}\r\n{2}", CurrentApp.GetLanguageInfo("1110111", "Slaver"), HostAddress, ObjectID);
            }
        }

        public override void GetBasicPropertyValues()
        {
            base.GetBasicPropertyValues();

            ResourceProperty propertyValue;
            for (int i = 0; i < ListProperties.Count; i++)
            {
                propertyValue = ListProperties[i];
                switch (propertyValue.PropertyID)
                {
                    case S1110Consts.PROPERTYID_MASTERSLAVER:
                        IsMaster = propertyValue.Value == "1";
                        break;
                }
            }

            GetNameAndDescription();
        }

        public override void SetBasicPropertyValues()
        {
            base.SetBasicPropertyValues();

            ResourceProperty propertyValue;
            for (int i = 0; i < ListProperties.Count; i++)
            {
                propertyValue = ListProperties[i];
                switch (propertyValue.PropertyID)
                {
                    case S1110Consts.PROPERTYID_MASTERSLAVER:
                        propertyValue.Value = IsMaster ? "1" : "2";
                        break;
                }
            }
        }

        public override void SetPropertyValue(int propertyID, string value)
        {
            base.SetPropertyValue(propertyID, value);

            switch (propertyID)
            {
                case S1110Consts.PROPERTYID_MASTERSLAVER:
                    IsMaster = value == "1";
                    break;
            }
        }
    }
}
