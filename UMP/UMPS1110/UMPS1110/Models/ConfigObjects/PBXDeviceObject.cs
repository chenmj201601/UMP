//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    5b3b4298-c7d0-4290-976b-0e23ecd685a0
//        CLR Version:              4.0.30319.18444
//        Name:                     PBXDeviceObject
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models
//        File Name:                PBXDeviceObject
//
//        created by Charley at 2015/4/14 17:01:31
//        http://www.voicecyber.com 
//
//======================================================================

using System.Linq;
using VoiceCyber.UMP.Common11101;

namespace UMPS1110.Models.ConfigObjects
{
    public class PBXDeviceObject : DeviceObject
    {
        public const int PRO_CTITYPE = 12;
        public const int PRO_DEVICETYPE = 13;
        public const int PRO_MONITORMODE = 14;
        public const int PRO_DEVICENAME = 15;

        public int CTIType { get; set; }
        public int DeviceType { get; set; }
        public int MonitorMode { get; set; }
        public string DeviceName { get; set; }

        public string StrCTIType { get; set; }
        public string StrDeviceType { get; set; }
        public string StrMonitorMode { get; set; }

        public override void GetNameAndDescription()
        {
            base.GetNameAndDescription();

            Name = string.Format("[{0}] {1} {2}", ID, CTIType, DeviceName);
            Description = string.Format("{0}({1})", Name, ObjectID);

            if (ListAllBasicInfos == null) { return; }
            BasicInfoData info = ListAllBasicInfos.FirstOrDefault(b => b.InfoID == S1110Consts.SOURCEID_CTITYPE && b.Value == CTIType.ToString());
            if (info == null) { return; }
            Name = string.Format("[{0}] {1} {2}", ID, CurrentApp.GetLanguageInfo(string.Format("BID{0}{1}",S1110Consts.SOURCEID_CTITYPE, CTIType.ToString("000")), info.Icon), DeviceName);
            Description = string.Format("{0}({1})", Name, ObjectID);
        }

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
                    case PRO_CTITYPE:
                        if (int.TryParse(propertyValue.Value, out intValue))
                        {
                            CTIType = intValue;

                            StrCTIType = CTIType.ToString();
                            if (ListAllBasicInfos != null)
                            {
                                var info =
                                    ListAllBasicInfos.FirstOrDefault(
                                        b => b.InfoID == S1110Consts.SOURCEID_CTITYPE && b.Value == CTIType.ToString());
                                if (info != null)
                                {
                                    StrCTIType =
                                        CurrentApp.GetLanguageInfo(
                                            string.Format("BID{0}{1}",S1110Consts.SOURCEID_CTITYPE, info.SortID.ToString("000")), info.Icon);
                                }
                            }
                        }
                        break;
                    case PRO_DEVICETYPE:
                        if (int.TryParse(propertyValue.Value, out intValue))
                        {
                            DeviceType = intValue;

                            StrDeviceType = DeviceType.ToString();
                            if (ListAllBasicInfos != null)
                            {
                                var info =
                                    ListAllBasicInfos.FirstOrDefault(
                                        b => b.InfoID == S1110Consts.SOURCEID_PBX_DEVICETYPE && b.Value == DeviceType.ToString());
                                if (info != null)
                                {
                                    StrDeviceType =
                                        CurrentApp.GetLanguageInfo(
                                            string.Format("BID{0}{1}",S1110Consts.SOURCEID_PBX_DEVICETYPE, info.SortID.ToString("000")), info.Icon);
                                }
                            }
                        }
                        break;
                    case PRO_MONITORMODE:
                        if (int.TryParse(propertyValue.Value, out intValue))
                        {
                            MonitorMode = intValue;

                            StrMonitorMode = MonitorMode.ToString();
                            if (ListAllBasicInfos != null)
                            {
                                var info =
                                    ListAllBasicInfos.FirstOrDefault(
                                        b => b.InfoID == S1110Consts.SOURCEID_PBX_MONITORMODE && b.Value == MonitorMode.ToString());
                                if (info != null)
                                {
                                    StrMonitorMode =
                                        CurrentApp.GetLanguageInfo(
                                            string.Format("BID{0}{1}",S1110Consts.SOURCEID_PBX_MONITORMODE, info.SortID.ToString("000")), info.Icon);
                                }
                            }
                        }
                        break;
                    case PRO_DEVICENAME:
                        DeviceName = propertyValue.Value;
                        break;
                }
            }

            GetNameAndDescription();
        }

        public override void SetBasicPropertyValues()
        {
            //PBXDevice的Key值由CTIType，DeviceType，MonitorMode和DeviceName共同决定
            string strKey = string.Format("{0}-{1}-{2}-{3}", CTIType, DeviceType, MonitorMode, DeviceName);

            base.SetBasicPropertyValues();

            ResourceProperty propertyValue;
            for (int i = 0; i < ListProperties.Count; i++)
            {
                propertyValue = ListProperties[i];
                switch (propertyValue.PropertyID)
                {
                    case S1110Consts.PROPERTYID_XMLKEY:
                        propertyValue.Value = strKey;
                        break;
                    case PRO_CTITYPE:
                        propertyValue.Value = CTIType.ToString();
                        break;
                    case PRO_DEVICETYPE:
                        propertyValue.Value = DeviceType.ToString();
                        break;
                    case PRO_MONITORMODE:
                        propertyValue.Value = MonitorMode.ToString();
                        break;
                    case PRO_DEVICENAME:
                        propertyValue.Value = DeviceName;
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
                case PRO_CTITYPE:
                    if (int.TryParse(value, out intValue))
                    {
                        CTIType = intValue;
                    }
                    break;
                case PRO_DEVICETYPE:
                    if (int.TryParse(value, out intValue))
                    {
                        DeviceType = intValue;
                    }
                    break;
                case PRO_MONITORMODE:
                    if (int.TryParse(value, out intValue))
                    {
                        MonitorMode = intValue;
                    }
                    break;
                case PRO_DEVICENAME:
                    DeviceName = value;
                    break;
            }
        }
    }
}
