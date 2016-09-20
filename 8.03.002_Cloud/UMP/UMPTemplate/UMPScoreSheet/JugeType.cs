//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    bcb96db8-c91e-4fd6-a6d6-1ce7271e68aa
//        CLR Version:              4.0.30319.18444
//        Name:                     JugeType
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets
//        File Name:                JugeType
//
//        created by Charley at 2014/6/30 15:42:19
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.ScoreSheets
{
    /// <summary>
    /// 控制源判定方式
    /// </summary>
    public enum JugeType
    {
        /// <summary>
        /// =
        /// </summary>
        Equal = 1,
        /// <summary>
        /// >=
        /// </summary>
        UpperEqual = 2,
        /// <summary>
        /// >
        /// </summary>
        Upper = 3,
        /// <summary>
        /// LowerEqual
        /// </summary>
        LowerEqual = 4,
        /// <summary>
        /// Lower
        /// </summary>
        Lower = 5,
        /// <summary>
        /// Between
        /// </summary>
        Between = 6
    }
}
