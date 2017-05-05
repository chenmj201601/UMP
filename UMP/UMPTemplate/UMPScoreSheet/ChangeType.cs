//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    e9b9ecaa-109f-4e53-9b35-cf152b2ed131
//        CLR Version:              4.0.30319.18444
//        Name:                     ChangeType
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets
//        File Name:                ChangeType
//
//        created by Charley at 2014/6/30 15:54:03
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.ScoreSheets
{
    /// <summary>
    /// 被控制项控制方式
    /// </summary>
    public enum ChangeType
    {
        /// <summary>
        /// =
        /// </summary>
        Equal = 1,
        /// <summary>
        /// +
        /// </summary>
        Sum = 2,
        /// <summary>
        /// -
        /// </summary>
        Sub = 3,
        /// <summary>
        /// *
        /// </summary>
        Multi = 4,
        /// <summary>
        /// /
        /// </summary>
        Div = 5,
        /// <summary>
        /// N/A
        /// </summary>
        NA = 6
    }
}
