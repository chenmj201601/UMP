//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ac06deea-49f9-4b2b-9020-b32d9a4978a5
//        CLR Version:              4.0.30319.18063
//        Name:                     DownloadParamObject
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models.ConfigObjects
//        File Name:                DownloadParamObject
//
//        created by Charley at 2015/5/7 16:53:33
//        http://www.voicecyber.com 
//
//======================================================================

using System.Linq;
using VoiceCyber.UMP.Common11101;

namespace UMPS1110.Models.ConfigObjects
{
    public class DownloadParamObject : ConfigObject,IEnableDisableObject
    {
        public const int PRO_METHOD = 11;
        public const int PRO_VOICEID = 12;
        public const int PRO_ADDRESS = 13;
        public const int PRO_PORT = 14;
        public const int PRO_ROOTDIR = 15;
        public const int PRO_VOICEADDRESS = 16;

        private bool mIsEnabled;
        public bool IsEnabled
        {
            get { return mIsEnabled; }
            set { mIsEnabled = value; OnPropertyChanged("IsEnabled"); }
        }
        public int Method { get; set; }
        public int VoiceID { get; set; }
        public string Address { get; set; }
        public string RootDir { get; set; }
        public string StrMethod { get; set; }

        public override void GetNameAndDescription()
        {
            base.GetNameAndDescription();

            Name = string.Format("[{0}]{1} {2}\\{3}", ID, StrMethod, Address, RootDir);
            Description = string.Format("{0}\r\n{1}", Name, ObjectID);
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
                    case S1110Consts.PROPERTYID_ENABLEDISABLE:
                        IsEnabled = propertyValue.Value == "1";
                        break;
                    case PRO_METHOD:
                        if (int.TryParse(propertyValue.Value, out intValue))
                        {
                            Method = intValue;
                            StrMethod = intValue.ToString();
                            if (ListAllBasicInfos != null)
                            {
                                var info =
                                    ListAllBasicInfos.FirstOrDefault(
                                        b =>
                                            b.InfoID == S1110Consts.SOURCEID_DOWNLOADMETHOD &&
                                            b.Value == Method.ToString());
                                if (info != null)
                                {
                                    StrMethod =
                                      CurrentApp.GetLanguageInfo(
                                          string.Format("BID{0}{1}", S1110Consts.SOURCEID_DOWNLOADMETHOD, info.SortID.ToString("000")), info.Icon);
                                }
                            }
                        }
                        break;
                    case PRO_VOICEID:
                        if (int.TryParse(propertyValue.Value, out intValue))
                        {
                            VoiceID = intValue;
                        }
                        break;
                    case PRO_ADDRESS:
                        Address = propertyValue.Value;
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
                    case S1110Consts.PROPERTYID_ENABLEDISABLE:
                        propertyValue.Value = IsEnabled ? "1" : "0";
                        break;
                    case PRO_VOICEADDRESS:
                        if (ListAllObjects != null)
                        {
                            var voice =
                                ListAllObjects.FirstOrDefault(
                                    o => o.ObjectType == S1110Consts.RESOURCE_VOICESERVER && o.ID == VoiceID) as ServiceObject;
                            if (voice != null)
                            {
                                propertyValue.Value = voice.HostAddress;
                            }
                            else
                            {
                                propertyValue.Value = string.Empty;
                            }
                        }
                        break;
                }
            }
        }

        public override void SetPropertyValue(int propertyID, string value)
        {
            base.SetPropertyValue(propertyID, value);

            switch (propertyID)
            {
                case S1110Consts.PROPERTYID_ENABLEDISABLE:
                    IsEnabled = value == "1";
                    break;
            }
        }
    }
}
