//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6f4ffa03-c499-4799-83ca-fc08007a5629
//        CLR Version:              4.0.30319.18444
//        Name:                     CustomConditionItemType
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common31021
//        File Name:                CustomConditionItemType
//
//        created by Charley at 2014/11/6 17:23:00
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common31021
{
    /// <summary>
    /// 自定义查询条件项类型
    /// </summary>
    public enum CustomConditionItemType
    {
        Unkown = 0,
        SimpleText = 1,
        LikeText = 2,
        MultiText = 3,
        DropDownEnum = 4,
        RadioSelect = 5,
        CheckSelect = 6,
        TimeFromTo = 10,
        DurationFromTo = 11,
        TimeTypeFromTo = 12,

        //自动模糊查询
        AutoLikeText = 13,

        ButtonTreeAddMultiText = 14,
        ComboxboxSelect=15,
        CheckSelectText=16,
        RadionBoxCombox=17,
        RadionBoxCombox1=18,
        AutoMultiLikeText = 19

    }
}
