//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    dc2bb8b9-0b0b-4d01-b25e-bc97d6e3b309
//        CLR Version:              4.0.30319.18063
//        Name:                     CTIConnectionGroupObject
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models.ConfigObjects
//        File Name:                CTIConnectionGroupObject
//
//        created by Charley at 2015/4/20 13:28:33
//        http://www.voicecyber.com 
//
//======================================================================

using System.Linq;
using VoiceCyber.UMP.Common11101;

namespace UMPS1110.Models.ConfigObjects
{
    public class CTIConnectionGroupObject : ConfigObject, IEnableDisableObject
    {
        public const int PRO_CTITYPE = 11;

        private bool mIsEnabled;
        public bool IsEnabled
        {
            get { return mIsEnabled; }
            set { mIsEnabled = value; OnPropertyChanged("IsEnabled"); }
        }
        public int CTIType { get; set; }

        public override void GetNameAndDescription()
        {
            base.GetNameAndDescription();

            Name = string.Format("[{0}] [{1}]", ID, CTIType);
            Description = string.Format("{0} {1}", Name, ObjectID);
            if (TypeParam != null)
            {
                Name = string.Format("[{0}] [{1}]", ID,
                    CurrentApp.GetLanguageInfo(string.Format("OBJ{0}", ObjectType), TypeParam.Description));
                Description = string.Format("{0} {1}", Name, ObjectID);
                if (ListAllBasicInfos != null)
                {
                    BasicInfoData info =
                        ListAllBasicInfos.FirstOrDefault(b => b.InfoID == S1110Consts.SOURCEID_CTITYPE && b.Value == CTIType.ToString());
                    if (info != null)
                    {
                        Name = string.Format("[{0}] {1}", ID,
                            CurrentApp.GetLanguageInfo(string.Format("BID{0}{1}", S1110Consts.SOURCEID_CTITYPE, info.SortID.ToString("000")), info.Icon));
                        Description = string.Format("{0} {1}", Name, ObjectID);
                    }
                }
            }
        }

        public override void GetBasicPropertyValues()
        {
            base.GetBasicPropertyValues();

            int intValue;
            ResourceProperty propertyValue;
            for (int i = 0; i < ListProperties.Count; i++)
            {
                propertyValue = ListProperties[i];
                switch (propertyValue.PropertyID)
                {
                    case S1110Consts.PROPERTYID_ENABLEDISABLE:
                        IsEnabled = propertyValue.Value == "1";
                        break;
                    case PRO_CTITYPE:
                        if (int.TryParse(propertyValue.Value, out intValue))
                        {
                            CTIType = intValue;
                        }
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
                    case S1110Consts.PROPERTYID_XMLKEY:
                        //CTI组的XmlKey是ID
                        propertyValue.Value = ID.ToString();
                        break;
                }
            }
        }
    }
}
