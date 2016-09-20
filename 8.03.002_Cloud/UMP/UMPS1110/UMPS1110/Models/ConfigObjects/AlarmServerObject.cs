//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    4dc44296-47e6-4591-949a-ab574da289b2
//        CLR Version:              4.0.30319.18444
//        Name:                     AlarmServerObject
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models.ConfigObjects
//        File Name:                AlarmServerObject
//
//        created by Charley at 2015/4/16 18:09:11
//        http://www.voicecyber.com 
//
//======================================================================

using VoiceCyber.UMP.Common11101;

namespace UMPS1110.Models.ConfigObjects
{
    public class AlarmServerObject : ServiceObject, IMasterSlaverObject
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
                    //MainFlag，由IsMaster决定
                    case 11:
                        propertyValue.Value = IsMaster ? "1" : "0";
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
