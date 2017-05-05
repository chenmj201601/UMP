//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    f41682ad-6ab9-4b63-8da5-382b4e22364d
//        CLR Version:              4.0.30319.18444
//        Name:                     ChannelObject
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models
//        File Name:                ChannelObject
//
//        created by Charley at 2015/4/14 16:36:44
//        http://www.voicecyber.com 
//
//======================================================================

using VoiceCyber.UMP.Common11101;

namespace UMPS1110.Models.ConfigObjects
{
    public class ChannelObject : ConfigObject
    {
        public const int PRO_CHANNAME = 11;
        public const int PRO_EXTENSION = 12;

        public string ChanName { get; set; }
        public string Extension { get; set; }

        public override void GetNameAndDescription()
        {
            base.GetNameAndDescription();

            Name = string.Format("[{0}] {1}", ID.ToString("00000"), Extension);
            Description = string.Format("{0}\r\n{1}", Name, ObjectID);
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
                    case PRO_CHANNAME:
                        ChanName = propertyValue.Value;
                        break;
                    case PRO_EXTENSION:
                        Extension = propertyValue.Value;
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
                    case PRO_CHANNAME:
                        propertyValue.Value = ChanName;
                        break;
                    case PRO_EXTENSION:
                        propertyValue.Value = Extension;
                        break;
                }
            }
        }

        public override void SetPropertyValue(int propertyID, string value)
        {
            base.SetPropertyValue(propertyID, value);

            switch (propertyID)
            {
                case PRO_CHANNAME:
                    ChanName = value;
                    break;
                case PRO_EXTENSION:
                    Extension = value;
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
            //分机号不能为空
            if (string.IsNullOrEmpty(Extension))
            {
                result.Result = false;
                result.Code = CheckResult.RES_CHAN_EXT_INVALID;
                result.Message = string.Format("Extension empty");
                result.PropertyID = PRO_EXTENSION;
                return result;
            }
            return result;
        }
    }
}
