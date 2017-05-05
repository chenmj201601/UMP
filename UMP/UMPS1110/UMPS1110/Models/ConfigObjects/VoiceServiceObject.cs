//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    845f4057-3afb-40e1-8713-c5a0e4b7e0a1
//        CLR Version:              4.0.30319.18444
//        Name:                     VoiceServiceObject
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models.ConfigObjects
//        File Name:                VoiceServiceObject
//
//        created by Charley at 2015/4/17 15:22:22
//        http://www.voicecyber.com 
//
//======================================================================

using System.Linq;
using VoiceCyber.UMP.Common11101;

namespace UMPS1110.Models.ConfigObjects
{
    /// <summary>
    /// 录音服务对象
    /// PropertyID
    /// 12      StandByRole
    /// 13      MasterAddress
    /// 14      MasterPort
    /// 15      MainNtidrvPath
    /// 20      是否混合录音
    /// 44      Voip通道数
    /// 45      Voip最大通道数
    /// 180     是否启用按键登录功能
    /// </summary>
    public class VoiceServiceObject : ServiceObject
    {
        public const int PRO_STANDBYROLE = 12;
        public const int PRO_MASTERADDRESS = 13;
        public const int PRO_MASTERPORT = 14;
        public const int PRO_MAINNTIDRVPATH = 15;
        public const int PRO_ISMIXRECORD = 20;
        public const int PRO_VOIPCHANNELCOUNT = 44;
        public const int PRO_MAXVOIPCHANNELCOUNT = 45;
        public const int PRO_ISDTMFLOGIN = 180;

        public bool IsMixRecord { get; set; }
        public string MainNtidrvPath { get; set; }
        public int VoipChannelCount { get; set; }
        public int MaxVoipChannelCount { get; set; }

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
                    case PRO_ISMIXRECORD:
                        IsMixRecord = propertyValue.Value == "1";
                        break;
                    case PRO_MAINNTIDRVPATH:
                        MainNtidrvPath = propertyValue.Value;
                        break;
                    case PRO_MAXVOIPCHANNELCOUNT:
                        if (int.TryParse(propertyValue.Value, out intValue))
                        {
                            MaxVoipChannelCount = intValue;
                        }
                        break;
                }
            }
            //如果没有启用混合录音，MainNtidrvPath为空
            if (!IsMixRecord)
            {
                MainNtidrvPath = string.Empty;
            }
            VoipChannelCount = GetVoipChannelCount();

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
                    case PRO_ISMIXRECORD:
                        propertyValue.Value = IsMixRecord ? "1" : "0";
                        break;
                    case PRO_MAINNTIDRVPATH:
                        if (IsMixRecord)
                        {
                            propertyValue.Value = MainNtidrvPath;
                        }
                        else
                        {
                            propertyValue.Value = string.Empty;
                        }
                        break;
                    case PRO_VOIPCHANNELCOUNT:
                        propertyValue.Value = GetVoipChannelCount().ToString();
                        break;
                    case PRO_MAXVOIPCHANNELCOUNT:
                        propertyValue.Value = MaxVoipChannelCount.ToString();
                        break;
                }
            }
        }

        public override void SetPropertyValue(int propertyID, string value)
        {
            base.SetPropertyValue(propertyID, value);

            switch (propertyID)
            {
                case PRO_ISMIXRECORD:
                    IsMixRecord = value == "1";
                    SetPropertyValue(PRO_MAINNTIDRVPATH, MainNtidrvPath);
                    break;
                case PRO_MAINNTIDRVPATH:
                    if (IsMixRecord)
                    {
                        MainNtidrvPath = value;
                    }
                    else
                    {
                        MainNtidrvPath = string.Empty;
                    }
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
            //检查最大Voip通道数
            var count = GetVoipChannelCount();
            if (MaxVoipChannelCount > 0
                && MaxVoipChannelCount < count)
            {
                result.Result = false;
                result.PropertyID = PRO_MAXVOIPCHANNELCOUNT;
                result.Code = CheckResult.RES_VOICESERVER_MAXVOIPCHANNEL_INVALID;
                result.Message = string.Format("MaxVoipChannelCount invalid");
                return result;
            }
            return result;
        }

        private int GetVoipChannelCount()
        {
            int count = 0;
            if (ListAllObjects != null)
            {
                var channels =
                    ListAllObjects.Where(
                        c => c.ObjectType == S1110Consts.RESOURCE_CHANNEL && c.ParentID == ObjectID).ToList();
                for (int j = 0; j < channels.Count; j++)
                {
                    VoiceChannelObject channel = channels[j] as VoiceChannelObject;
                    if (channel != null)
                    {
                        channel.GetBasicPropertyValues();
                        if (channel.IsVoipChannel)
                        {
                            count++;
                        }
                    }
                }
            }
            return count;
        }

    }
}
