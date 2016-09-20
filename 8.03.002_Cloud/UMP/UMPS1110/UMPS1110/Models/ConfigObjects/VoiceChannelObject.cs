//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7f4ebb2c-0d61-48f1-8346-f8ff265ecbca
//        CLR Version:              4.0.30319.18444
//        Name:                     VoiceChannelObject
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models.ConfigObjects
//        File Name:                VoiceChannelObject
//
//        created by Charley at 2015/4/17 15:25:22
//        http://www.voicecyber.com 
//
//======================================================================

using System.Linq;
using VoiceCyber.UMP.Common11101;

namespace UMPS1110.Models.ConfigObjects
{
    /// <summary>
    /// 录音通道
    /// PropertyID
    /// 21      启动方式
    /// 26      是否Voip通道
    /// 50      使用板卡默认的能量启动参数
    /// 60      是否启用AGC
    /// 61      使用板卡默认的AGC参数
    /// </summary>
    public class VoiceChannelObject : ChannelObject
    {
        public const int PRO_PASSWORD = 18;
        public const int PRO_STARTTYPE = 21;
        public const int PRO_ISVOIPCHANNEL = 26;
        public const int PRO_ISBOARDDEFAULTACTIVITY = 50;
        public const int PRO_ISAGCENABLE = 60;
        public const int PRO_ISBOARDDEFAULTAGC = 61;
        public const int PRO_ALERTTONE = 75;
        public const int PRO_ALERTTYPE = 88;
        public const int PRO_RECORDDELAY = 89;

        public int StartType { get; set; }
        public bool IsVoipChannel { get; set; }
        public string StrStartType { get; set; }

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
                    //启动方式
                    case PRO_STARTTYPE:
                        if (int.TryParse(propertyValue.Value, out intValue))
                        {
                            StartType = intValue;
                            StrStartType = intValue.ToString();
                            if (ListAllBasicInfos != null)
                            {
                                var info =
                                    ListAllBasicInfos.FirstOrDefault(
                                        b => b.InfoID == S1110Consts.SOURCEID_VOICECHANNEL_STARTTYPE && b.Value == intValue.ToString());
                                if (info != null)
                                {
                                    StrStartType =
                                        CurrentApp.GetLanguageInfo(
                                            string.Format("BID{0}{1}", S1110Consts.SOURCEID_VOICECHANNEL_STARTTYPE, info.SortID.ToString("000")), info.Icon);
                                }
                            }
                        }
                        if (StartType == 1101
                            || StartType == 1102
                            || StartType == 1103
                            || StartType == 1107
                            || StartType == 1108
                            || StartType == 1113)
                        {
                            IsVoipChannel = true;
                        }
                        else
                        {
                            IsVoipChannel = false;
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
                    //启动方式
                    case PRO_STARTTYPE:
                        propertyValue.Value = StartType.ToString();
                        break;
                    //是否Voip通道
                    case PRO_ISVOIPCHANNEL:
                        if (StartType == 1101
                            || StartType == 1102
                            || StartType == 1103
                            || StartType == 1107
                            || StartType == 1108
                            || StartType == 1113)
                        {
                            propertyValue.Value = "1";
                        }
                        else
                        {
                            propertyValue.Value = "0";
                        }
                        break;
                    //提示音
                    case PRO_ALERTTONE:
                        int intValue = 0;
                        var value1 = ListProperties.FirstOrDefault(p => p.PropertyID == PRO_ALERTTYPE);
                        if (value1 != null)
                        {
                            int intValue1;
                            if (int.TryParse(value1.Value, out intValue1))
                            {
                                intValue = intValue1;
                                //AlertType为4的时候，取与RecordDelay的复合值
                                if (intValue1 == 2 || intValue == 4)
                                {
                                    var value2 = ListProperties.FirstOrDefault(p => p.PropertyID == PRO_RECORDDELAY);
                                    if (value2 != null)
                                    {
                                        int intValue2;
                                        if (int.TryParse(value2.Value, out intValue2))
                                        {
                                            intValue = intValue | ((intValue2 == 1) ? 8 : 0);
                                        }
                                    }
                                }
                            }
                        }
                        propertyValue.Value = intValue.ToString();
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
                case PRO_STARTTYPE:
                    if (int.TryParse(value, out intValue))
                    {
                        StartType = intValue;

                        bool bValue = StartType == 1101
                                      || StartType == 1102
                                      || StartType == 1103
                                      || StartType == 1107
                                      || StartType == 1108
                                      || StartType == 1113;
                        SetPropertyValue(26, bValue ? "1" : "0");
                    }
                    break;
                case PRO_ISVOIPCHANNEL:
                    IsVoipChannel = value == "1";
                    break;
            }
        }
    }
}
