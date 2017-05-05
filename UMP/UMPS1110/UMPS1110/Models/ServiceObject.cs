//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d3d264c1-c890-4fcb-b72d-08877639e676
//        CLR Version:              4.0.30319.18444
//        Name:                     ServiceObject
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models
//        File Name:                ServiceObject
//
//        created by Charley at 2015/4/13 14:49:31
//        http://www.voicecyber.com 
//
//======================================================================

using System.Linq;
using VoiceCyber.UMP.Common11101;

namespace UMPS1110.Models
{
    /// <summary>
    /// 服务对象
    /// </summary>
    public class ServiceObject : ConfigObject, IModuleObject, IEnableDisableObject
    {
        public int ModuleNumber { get; set; }
        public bool IsEnabled { get; set; }
        public string HostAddress { get; set; }
        public string HostName { get; set; }
        public int HostPort { get; set; }
        public string Continent { get; set; }
        public string Country { get; set; }
        public long MachineID { get; set; }
        public MachineObject Machine { get; set; }

        public override void GetBasicPropertyValues()
        {
            base.GetBasicPropertyValues();

            ResourceProperty propertyValue;
            int intValue;
            long longValue;
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
                    case S1110Consts.PROPERTYID_MODULENUMBER:
                        if (int.TryParse(propertyValue.Value, out intValue))
                        {
                            ModuleNumber = intValue;
                        }
                        break;
                    case S1110Consts.PROPERTYID_ENABLEDISABLE:
                        IsEnabled = propertyValue.Value == "1";
                        break;
                    case S1110Consts.PROPERTYID_MACHINE:
                        if (long.TryParse(propertyValue.Value, out longValue))
                        {
                            MachineID = longValue;
                            if (ListAllObjects != null)
                            {
                                var machine = ListAllObjects.FirstOrDefault(m => m.ObjectID == MachineID);
                                Machine = machine as MachineObject;
                            }
                        }
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

            //默认ModuleNumber由ID决定
            ModuleNumber = ID;
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
                    case S1110Consts.PROPERTYID_MODULENUMBER:
                        propertyValue.Value = ModuleNumber.ToString();
                        break;
                    case S1110Consts.PROPERTYID_ENABLEDISABLE:
                        propertyValue.Value = IsEnabled ? "1" : "0";
                        break;
                    case S1110Consts.PROPERTYID_MACHINE:
                        propertyValue.Value = MachineID.ToString();
                        break;
                }
            }
        }
    }
}
