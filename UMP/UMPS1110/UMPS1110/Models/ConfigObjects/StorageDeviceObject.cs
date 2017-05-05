//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b4917311-3b42-4ed4-b247-66a25cf68e93
//        CLR Version:              4.0.30319.18444
//        Name:                     StorageDeviceObject
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models
//        File Name:                StorageDeviceObject
//
//        created by Charley at 2015/4/13 18:24:40
//        http://www.voicecyber.com 
//
//======================================================================

using System.Linq;
using VoiceCyber.UMP.Common11101;

namespace UMPS1110.Models.ConfigObjects
{
    /// <summary>
    /// 存储设备
    /// PropertyID
    /// 11      设备类型
    /// 12      机器的ID
    /// 13      地址
    /// 14      主目录
    /// </summary>
    public class StorageDeviceObject : DeviceObject
    {
        public const int PRO_TYPE = 11;
        public const int PRO_SERVERID = 12;
        public const int PRO_HOSTADDRESS = 13;
        public const int PRO_ROOTDIR = 14;

        public int DeviceType { get; set; }
        public string StrDeviceType { get; set; }
        public int ServerID { get; set; }
        public string HostAddress { get; set; }
        public string RootDir { get; set; }

        public override void GetNameAndDescription()
        {
            base.GetNameAndDescription();

            Name = string.Format("[{0}] {1}\\\\{2}", ID, HostAddress, RootDir);
            if (DeviceType == 1)
            {
                Name = string.Format("{0}", RootDir);
            }
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
                    case PRO_TYPE:
                        if (int.TryParse(propertyValue.Value, out intValue))
                        {
                            DeviceType = intValue;

                            StrDeviceType = intValue.ToString();
                            if (ListAllBasicInfos != null)
                            {
                                var info =
                                    ListAllBasicInfos.FirstOrDefault(
                                        b => b.InfoID == S1110Consts.SOURCEID_STORAGE_DEVICETYPE && b.Value == intValue.ToString());
                                if (info != null)
                                {
                                    StrDeviceType =
                                        CurrentApp.GetLanguageInfo(
                                            string.Format("BID{0}{1}",S1110Consts.SOURCEID_STORAGE_DEVICETYPE, info.SortID.ToString("000")), info.Icon);
                                }
                            }
                        }
                        break;
                    case PRO_SERVERID:
                        if (int.TryParse(propertyValue.Value, out intValue))
                        {
                            ServerID = intValue;
                            if (DeviceType == 1)
                            {
                                ServerID = -1;
                            }
                            if (ListAllObjects != null)
                            {
                                var machine =
                                    ListAllObjects.FirstOrDefault(
                                        m => m.ObjectType == S1110Consts.RESOURCE_MACHINE && m.ID == ServerID) as
                                        MachineObject;
                                if (machine != null)
                                {
                                    HostAddress = machine.HostAddress;
                                }
                            }
                        }
                        break;
                    case PRO_ROOTDIR:
                        RootDir = propertyValue.Value;
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
                    case PRO_TYPE:
                        propertyValue.Value = DeviceType.ToString();
                        break;
                    case PRO_SERVERID:
                        propertyValue.Value = ServerID.ToString();
                        break;
                    case PRO_HOSTADDRESS:
                        propertyValue.Value = HostAddress;
                        break;
                    case PRO_ROOTDIR:
                        propertyValue.Value = RootDir;
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
                case PRO_TYPE:
                    if (int.TryParse(value, out intValue))
                    {
                        DeviceType = intValue;
                    }
                    break;
                case PRO_SERVERID:
                    if (int.TryParse(value, out intValue))
                    {
                        ServerID = intValue;
                        if (DeviceType == 1)
                        {
                            ServerID = -1;
                            SetPropertyValue(PRO_HOSTADDRESS, string.Empty);
                        }
                        if (ListAllObjects != null)
                        {
                            var machine =
                                ListAllObjects.FirstOrDefault(
                                    m => m.ObjectType == S1110Consts.RESOURCE_MACHINE && m.ID == ServerID) as
                                    MachineObject;
                            if (machine != null)
                            {
                                SetPropertyValue(PRO_HOSTADDRESS, machine.HostAddress);
                            }
                        }
                    }
                    break;
                case PRO_HOSTADDRESS:
                    HostAddress = value;
                    break;
                case PRO_ROOTDIR:
                    RootDir = value;
                    break;
            }
        }
    }
}
