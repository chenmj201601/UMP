//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    1e4ed12a-7ac7-446c-b6ee-3a1c2897d784
//        CLR Version:              4.0.30319.18444
//        Name:                     ConcurrentObject
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models.ConfigObjects
//        File Name:                ConcurrentObject
//
//        created by Charley at 2015/4/16 17:58:39
//        http://www.voicecyber.com 
//
//======================================================================

using System.Linq;
using VoiceCyber.UMP.Common11101;

namespace UMPS1110.Models.ConfigObjects
{
    public class ConcurrentObject : ConfigObject
    {
        public const int PRO_NUMBER = 11;
        public const int PRO_COUNT = 12;

        public int Number { get; set; }
        public int Count { get; set; }

        public override void GetNameAndDescription()
        {
            base.GetNameAndDescription();

            string name = string.Format("[{0}] {1}", ID, Number);

            if (ListAllBasicInfos != null)
            {
                BasicInfoData info =
                    ListAllBasicInfos.FirstOrDefault(b => b.InfoID == S1110Consts.SOURCEID_CONCURRENTNUMBER && b.Value == Number.ToString());
                if (info != null)
                {
                    name = string.Format("[{0}] {1}", ID,
                        CurrentApp.GetLanguageInfo(string.Format("BID{0}{1}",S1110Consts.SOURCEID_CONCURRENTNUMBER, info.SortID.ToString("000")), info.Icon));
                }
            }

            Name = name;
            Description = name;
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
                    case PRO_NUMBER:
                        if (int.TryParse(propertyValue.Value, out intValue))
                        {
                            Number = intValue;
                        }
                        break;
                    case PRO_COUNT:
                        if (int.TryParse(propertyValue.Value, out intValue))
                        {
                            Count = intValue;
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
                    case PRO_NUMBER:
                        propertyValue.Value = Number.ToString();
                        break;
                    case PRO_COUNT:
                        propertyValue.Value = Count.ToString();
                        break;
                    //XmlKey由Number决定
                    case S1110Consts.PROPERTYID_XMLKEY:
                        propertyValue.Value = Number.ToString();
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
                case PRO_NUMBER:
                    if (int.TryParse(value, out intValue))
                    {
                        Number = intValue;
                    }
                    break;
                case PRO_COUNT:
                    if (int.TryParse(value, out intValue))
                    {
                        Count = intValue;
                    }
                    break;
            }
        }
    }
}
