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

namespace UMPS1110.Models.ConfigObjects
{
    /// <summary>
    /// 服务对象
    /// </summary>
    public class ServiceObject : ConfigObject, IModuleObject, IEnableDisableObject
    {


        public int ModuleNumber { get; set; }

        private bool mIsEnabled;
        public bool IsEnabled
        {
            get { return mIsEnabled; }
            set { mIsEnabled = value; OnPropertyChanged("IsEnabled"); }
        }
        public long MachineObjID { get; set; }
        public MachineObject Machine { get; set; }
        public string HostAddress { get; set; }
        public string HostName { get; set; }
        public int HostPort { get; set; }
        public string Continent { get; set; }
        public string Country { get; set; }

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
                            MachineObjID = longValue;
                            if (ListAllObjects != null)
                            {
                                var machine = ListAllObjects.FirstOrDefault(m => m.ObjectID == MachineObjID);
                                Machine = machine as MachineObject;
                                if (Machine != null)
                                {
                                    HostAddress = Machine.HostAddress;
                                    HostName = Machine.HostName;
                                    Continent = Machine.Continent;
                                    Country = Machine.Country;
                                }
                            }
                        }
                        break;
                    case S1110Consts.PROPERTYID_HOSTPORT:
                        if (int.TryParse(propertyValue.Value, out intValue))
                        {
                            HostPort = intValue;
                        }
                        break;
                }
            }

            GetNameAndDescription();
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
                        propertyValue.Value = MachineObjID.ToString();
                        break;
                }
            }
        }

        public override void GetNameAndDescription()
        {
            base.GetNameAndDescription();

            Name = string.Format("[{0}] {1}", ID, HostAddress);
            Description = string.Format("{0}\r\n{1}", Name, ObjectID);
        }

        public override void SetPropertyValue(int propertyID, string value)
        {
            base.SetPropertyValue(propertyID, value);

            int intValue;
            long longValue;
            switch (propertyID)
            {
                case S1110Consts.PROPERTYID_MODULENUMBER:
                    if (int.TryParse(value, out intValue))
                    {
                        ModuleNumber = intValue;
                    }
                    break;
                case S1110Consts.PROPERTYID_ENABLEDISABLE:
                    IsEnabled = value == "1";
                    break;
                case S1110Consts.PROPERTYID_MACHINE:
                    if (long.TryParse(value, out longValue))
                    {
                        MachineObjID = longValue;
                        if (ListAllObjects != null)
                        {
                            var machine =
                                ListAllObjects.FirstOrDefault(m => m.ObjectID == MachineObjID) as MachineObject;
                            if (machine != null)
                            {
                                Machine = machine;
                                if (Machine != null)
                                {
                                    SetPropertyValue(S1110Consts.PROPERTYID_HOSTADDRESS, machine.HostAddress);
                                    SetPropertyValue(S1110Consts.PROPERTYID_HOSTNAME, machine.HostName);
                                    SetPropertyValue(S1110Consts.PROPERTYID_CONTINENT, machine.Continent);
                                    SetPropertyValue(S1110Consts.PROPERTYID_COUNTRY, machine.Country);
                                }
                            }
                        }
                    }
                    break;
                case S1110Consts.PROPERTYID_HOSTADDRESS:
                    HostAddress = value;
                    break;
                case S1110Consts.PROPERTYID_HOSTNAME:
                    HostName = value;
                    break;
                case S1110Consts.PROPERTYID_HOSTPORT:
                    if (int.TryParse(value, out intValue))
                    {
                        HostPort = intValue;
                    }
                    break;
                case S1110Consts.PROPERTYID_CONTINENT:
                    Continent = value;
                    break;
                case S1110Consts.PROPERTYID_COUNTRY:
                    Country = value;
                    break;
            }
        }

        public override CheckResult CheckConfig()
        {
            var result = base.CheckConfig();
            if (!result.Result)
            {
                return result;
            }
            if (Machine == null)
            {
                result.Result = false;
                result.PropertyID = S1110Consts.PROPERTYID_MACHINE;
                result.Code = CheckResult.RES_NOMACHINE;
                result.Message = string.Format("Not specify a machine for the service object");
                return result;
            }
            if (ListAllObjects != null)
            {
                var temp = ListAllObjects.FirstOrDefault(m => m.ObjectID == Machine.ObjectID);
                if (temp == null)
                {
                    result.Result = false;
                    result.PropertyID = S1110Consts.PROPERTYID_MACHINE;
                    result.Code = CheckResult.RES_INVALIDMACHINE;
                    result.Message = string.Format("Specified machine invalid");
                    return result;
                }
            }
            return result;
        }
    }
}
