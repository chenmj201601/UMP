//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3d238913-7f7e-4eb7-bd5b-c349abaffaa8
//        CLR Version:              4.0.30319.18444
//        Name:                     VoipProtocalObject
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models.ConfigObjects
//        File Name:                VoipProtocalObject
//
//        created by Charley at 2015/4/16 17:45:30
//        http://www.voicecyber.com 
//
//======================================================================

using System.Linq;
using VoiceCyber.UMP.Common11101;

namespace UMPS1110.Models.ConfigObjects
{
    /// <summary>
    /// Voip协议
    /// PropertyID
    /// 11      Voip协议代码
    /// </summary>
    public class VoipProtocalObject : ConfigObject
    {
        public const int PRO_CODE = 11;
        public const int PRO_PBXMAINPORT = 22;

        public int Code { get; set; }

        public override void GetNameAndDescription()
        {
            base.GetNameAndDescription();

            string name = string.Format("[{0}] {1}", ID, Code);

            if (ListAllBasicInfos != null)
            {
                BasicInfoData info =
                    ListAllBasicInfos.FirstOrDefault(b => b.InfoID == S1110Consts.SOURCEID_VOIPPROTOCOL_TYPE && b.Value == Code.ToString());
                if (info != null)
                {
                    name = string.Format("[{0}] {1}", ID,
                        CurrentApp.GetLanguageInfo(string.Format("BID{0}{1}",S1110Consts.SOURCEID_VOIPPROTOCOL_TYPE, info.SortID.ToString("000")), info.Icon));
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
                    case PRO_CODE:
                        if (int.TryParse(propertyValue.Value, out intValue))
                        {
                            Code = intValue;
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
                    case PRO_CODE:
                        propertyValue.Value = Code.ToString();
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
                case PRO_CODE:
                    if (int.TryParse(value, out intValue))
                    {
                        Code = intValue;
                    }
                    break;
            }
        }
    }
}
