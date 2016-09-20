//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    86228119-2aec-472b-bf13-6746c7e2bf89
//        CLR Version:              4.0.30319.18063
//        Name:                     AlarmMonitorParamObject
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models.ConfigObjects
//        File Name:                AlarmMonitorParamObject
//
//        created by Charley at 2015/6/2 17:11:43
//        http://www.voicecyber.com 
//
//======================================================================

using System.Linq;
using VoiceCyber.UMP.Common11101;

namespace UMPS1110.Models.ConfigObjects
{
    public class AlarmMonitorParamObject : ConfigObject, IModuleObject
    {
        public const int PRO_SERVICEID = 11;

        public int ModuleNumber { get; set; }
        public int ServiceID { get; set; }

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
                    case S1110Consts.PROPERTYID_MODULENUMBER:
                        if (int.TryParse(propertyValue.Value, out intValue))
                        {
                            ModuleNumber = intValue;
                        }
                        break;
                    case PRO_SERVICEID:
                        if (int.TryParse(propertyValue.Value, out intValue))
                        {
                            ServiceID = intValue;
                        }
                        break;
                }
            }

            GetNameAndDescription();
        }

        public override void SetBasicPropertyValues()
        {
            base.SetBasicPropertyValues();

            //默认ModuleNumber由所在的服务的ModuleNumber决定
            if (ListAllObjects != null)
            {
                var service =
                    ListAllObjects.FirstOrDefault(
                        o => o.ObjectType == S1110Consts.RESOURCE_ALARMMONITOR && o.ID == ServiceID) as ServiceObject;
                if (service != null)
                {
                    ModuleNumber = service.ModuleNumber;
                }
            }
            ResourceProperty propertyValue;
            for (int i = 0; i < ListProperties.Count; i++)
            {
                propertyValue = ListProperties[i];
                switch (propertyValue.PropertyID)
                {
                    case S1110Consts.PROPERTYID_MODULENUMBER:
                        propertyValue.Value = ModuleNumber.ToString();
                        break;
                    case PRO_SERVICEID:
                        propertyValue.Value = ServiceID.ToString();
                        break;
                }
            }
        }

        public override void SetPropertyValue(int propertyID, string value)
        {
            base.SetPropertyValue(propertyID, value);

            int intValue;
            switch (propertyID)
            {
                case S1110Consts.PROPERTYID_MODULENUMBER:
                    if (int.TryParse(value, out intValue))
                    {
                        ModuleNumber = intValue;
                    }
                    break;
                case PRO_SERVICEID:
                    if (int.TryParse(value, out intValue))
                    {
                        ServiceID = intValue;

                        if (ListAllObjects != null)
                        {
                            var service =
                                ListAllObjects.FirstOrDefault(
                                    o => o.ObjectType == S1110Consts.RESOURCE_ALARMMONITOR && o.ID == ServiceID) as
                                    ServiceObject;
                            if (service != null)
                            {
                                SetPropertyValue(S1110Consts.PROPERTYID_MODULENUMBER, service.ModuleNumber.ToString());
                            }
                        }
                    }
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
            if (ListAllObjects != null)
            {
                var temp =
                    ListAllObjects.FirstOrDefault(
                        o => o.ObjectType == S1110Consts.RESOURCE_ALARMMONITOR && o.ID == ServiceID);
                if (temp == null)
                {
                    result.Result = false;
                    result.PropertyID = PRO_SERVICEID;
                    result.Code = CheckResult.RES_NOCONFIG;
                    result.Message = string.Format("ServiceID not configed");
                    return result;
                }
            }
            return result;
        }
    }
}
