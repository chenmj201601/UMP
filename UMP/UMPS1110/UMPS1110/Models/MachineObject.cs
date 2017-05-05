//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2385e2ec-ee6c-4cee-b6cf-6a7077257c24
//        CLR Version:              4.0.30319.18444
//        Name:                     MachineObject
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models
//        File Name:                MachineObject
//
//        created by Charley at 2015/4/13 14:48:43
//        http://www.voicecyber.com 
//
//======================================================================

using VoiceCyber.UMP.Common11101;

namespace UMPS1110.Models
{
    /// <summary>
    /// 物理机器
    /// </summary>
    public class MachineObject : ConfigObject
    {
        public string HostAddress { get; set; }
        public string HostName { get; set; }
        public int HostPort { get; set; }
        public string LogPath { get; set; }
        public string Continent { get; set; }
        public string Country { get; set; }

        public override void GetBasicPropertyValues()
        {
            base.GetBasicPropertyValues();

            ResourceProperty propertyValue;
            int intValue;
            for (int i = 0; i < ListProperties.Count; i++)
            {
                propertyValue = ListProperties[i];
                switch (propertyValue.PropertyID)
                {
                    case S1110Consts.PROPERTYID_HOSTADDRESS:
                        HostAddress = propertyValue.Value;
                        break;
                    case S1110Consts.PROPERTYID_HOSTNAME:
                        HostName = propertyValue.Value;
                        break;
                    case S1110Consts.PROPERTYID_HOSTPORT:
                        if (int.TryParse(propertyValue.Value, out intValue))
                        {
                            HostPort = intValue;
                        }
                        break;
                    case S1110Consts.PROPERTYID_CONTINENT:
                        Continent = propertyValue.Value;
                        break;
                    case S1110Consts.PROPERTYID_COUNTRY:
                        Country = propertyValue.Value;
                        break;
                    case 11:
                        LogPath = propertyValue.Value;
                        break;
                }
            }

            GetNameAndDescription();
        }

        public override void GetNameAndDescription()
        {
            base.GetNameAndDescription();

            Name = string.Format("[{0}] {1}", ID, HostAddress);
            Description = string.Format("{0}({1})", Name, ObjectID);
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
                    case S1110Consts.PROPERTYID_HOSTADDRESS:
                        propertyValue.Value = HostAddress;
                        break;
                    case S1110Consts.PROPERTYID_HOSTNAME:
                        propertyValue.Value = HostName;
                        break;
                    case S1110Consts.PROPERTYID_HOSTPORT:
                        propertyValue.Value = HostPort.ToString();
                        break;
                    case S1110Consts.PROPERTYID_CONTINENT:
                        propertyValue.Value = Continent;
                        break;
                    case S1110Consts.PROPERTYID_COUNTRY:
                        propertyValue.Value = Country;
                        break;
                    case 11:
                        propertyValue.Value = LogPath;
                        break;
                }
            }
        }
    }
}
