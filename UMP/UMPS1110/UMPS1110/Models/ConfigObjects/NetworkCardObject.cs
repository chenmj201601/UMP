//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6c629828-8aad-4bd6-a99c-4fb40ee3ade0
//        CLR Version:              4.0.30319.18444
//        Name:                     NetworkCardObject
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models
//        File Name:                NetworkCardObject
//
//        created by Charley at 2015/4/15 9:29:42
//        http://www.voicecyber.com 
//
//======================================================================

using VoiceCyber.UMP.Common11101;

namespace UMPS1110.Models.ConfigObjects
{
    public class NetworkCardObject : ConfigObject
    {
        public const int PRO_CARDID = 12;

        public string CardID { get; set; }
        public string CardName { get; set; }

        public override void GetNameAndDescription()
        {
            base.GetNameAndDescription();

            Name = string.Format("[{0}] {1}", ID, CardName);
            Description = string.Format("{0}({1})", Name, CardID);
        }

        public override void GetBasicPropertyValues()
        {
            base.GetBasicPropertyValues();

            ResourceProperty propertyValue;
            for (int i = 0; i < ListProperties.Count; i++)
            {
                propertyValue = ListProperties[i];
                switch (propertyValue.PropertyID)
                {
                    case PRO_CARDID:
                        CardID = propertyValue.Value;
                        if (propertyValue.ListOtherValues != null && propertyValue.ListOtherValues.Count > 0)
                        {
                            CardName = propertyValue.ListOtherValues[0];
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
                    case PRO_CARDID:
                        propertyValue.Value = CardID;
                        propertyValue.ListOtherValues.Clear();
                        propertyValue.ListOtherValues.Add(CardName);
                        break;
                    case S1110Consts.PROPERTYID_XMLKEY:
                        propertyValue.Value = ID.ToString();
                        break;
                }
            }
        }

        public override void SetPropertyValue(int propertyID, string value)
        {
            base.SetPropertyValue(propertyID, value);

            switch (propertyID)
            {
                case PRO_CARDID:
                    CardID = value;
                    break;
            }
        }
    }
}
