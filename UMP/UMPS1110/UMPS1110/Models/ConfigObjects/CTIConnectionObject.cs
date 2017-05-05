//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    465e08b5-c2de-4751-8abc-ac6f85e19696
//        CLR Version:              4.0.30319.18063
//        Name:                     CTIConnectionObject
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models.ConfigObjects
//        File Name:                CTIConnectionObject
//
//        created by Charley at 2015/4/20 15:59:15
//        http://www.voicecyber.com 
//
//======================================================================

using System.Linq;
using VoiceCyber.UMP.Common11101;

namespace UMPS1110.Models.ConfigObjects
{
    public class CTIConnectionObject : ConfigObject, IEnableDisableObject
    {
        public const int PRO_CTIHOST = 11;
        public const int PRO_CTIPORT = 12;
        public const int PRO_CTISERVERNAME = 14;
        public const int PRO_USERDMCC = 16;
        public const int PRO_VERSIONPROTOCOL = 18;
        public const int PRO_PROTOCOL = 19;
        public const int PRO_VERSION = 20;
        public const int PRO_FILENAME = 24;
        public const int PRO_CTIHOSTA = 31;
        public const int PRO_CTIPORTA = 32;
        public const int PRO_CTIHOSTB = 33;
        public const int PRO_CTIPORTB = 34;
        public const int PRO_LOGICALLINK = 41;

        private bool mIsEnabled;
        public bool IsEnabled
        {
            get { return mIsEnabled; }
            set { mIsEnabled = value; OnPropertyChanged("IsEnabled"); }
        }
        public int CTIType { get; set; }
        public string CTIServerName { get; set; }
        public string CTIHost { get; set; }
        public int CTIPort { get; set; }
        public string FileName { get; set; }
        public string LogicalLink { get; set; }
        public string CTIHostA { get; set; }
        public int CTIPortA { get; set; }
        public string CTIHostB { get; set; }
        public int CTIPortB { get; set; }

        public override void GetBasicPropertyValues()
        {
            base.GetBasicPropertyValues();

            int intValue;
            long longValue;
            ResourceProperty propertyValue;
            long parentID = 0;
            for (int i = 0; i < ListProperties.Count; i++)
            {
                propertyValue = ListProperties[i];
                switch (propertyValue.PropertyID)
                {
                    case S1110Consts.PROPERTYID_ENABLEDISABLE:
                        IsEnabled = propertyValue.Value == "1";
                        break;
                    case S1110Consts.PROPERTYID_PARENTOBJID:
                        if (long.TryParse(propertyValue.Value, out longValue))
                        {
                            parentID = longValue;
                        }
                        break;
                    case PRO_CTIHOST:
                        CTIHost = propertyValue.Value;
                        break;
                    case PRO_CTIPORT:
                        if (int.TryParse(propertyValue.Value, out intValue))
                        {
                            CTIPort = intValue;
                        }
                        break;
                    case PRO_CTISERVERNAME:
                        CTIServerName = propertyValue.Value;
                        break;
                    case PRO_FILENAME:
                        FileName = propertyValue.Value;
                        break;
                    case PRO_LOGICALLINK:
                        LogicalLink = propertyValue.Value;
                        break;
                    case PRO_CTIHOSTA:
                        CTIHostA = propertyValue.Value;
                        break;
                    case PRO_CTIPORTA:
                        if (int.TryParse(propertyValue.Value, out intValue))
                        {
                            CTIPortA = intValue;
                        }
                        break;
                    case PRO_CTIHOSTB:
                        CTIHostB = propertyValue.Value;
                        break;
                    case PRO_CTIPORTB:
                        if (int.TryParse(propertyValue.Value, out intValue))
                        {
                            CTIPortB = intValue;
                        }
                        break;
                }
            }
            if (parentID > 0)
            {
                if (ListAllObjects != null)
                {
                    var group = ListAllObjects.FirstOrDefault(o => o.ObjectID == parentID) as CTIConnectionGroupObject;
                    if (group != null)
                    {
                        CTIType = group.CTIType;
                    }
                }
            }

            GetNameAndDescription();
        }

        public override void SetBasicPropertyValues()
        {
            base.SetBasicPropertyValues();

            string strKey;
            switch (CTIType)
            {
                //CTC
                case 1:
                    //Key=SHA256(ANSI(CTIHost+LogicalLink))
                    strKey = string.Format("{0}{1}", CTIHost, LogicalLink);
                    break;
                //CVCT
                case 4:
                //AES
                case 5:
                    //Key=SHA(UNICODE(ServerName))
                    strKey = string.Format("{0}", CTIServerName);
                    break;
                //AIC
                case 6:
                    //Key=SHA(UNICODE(FileName))
                    strKey = string.Format("{0}", FileName);
                    break;
                //CTI OS
                case 8:
                    //Key=SHA(UNICODE(CTIHostA)+UNICODE(":")+UNICODE(CTIPortA)
                    strKey = string.Format("{0}:{1}", CTIHostA, CTIPortA);
                    break;
                default:
                    //Key=SHA(UNICODE(CTIHost)+UNICODE(":")+UNICODE(CTIPort)
                    strKey = string.Format("{0}:{1}", CTIHost, CTIPort);
                    break;
            }
            ResourceProperty propertyValue;
            for (int i = 0; i < ListProperties.Count; i++)
            {
                propertyValue = ListProperties[i];
                switch (propertyValue.PropertyID)
                {
                    case S1110Consts.PROPERTYID_XMLKEY:
                        propertyValue.Value = strKey;
                        break;
                }
            }
        }
    }
}
