//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    38f67b8f-ca24-4795-8653-9c934ab2001d
//        CLR Version:              4.0.30319.18063
//        Name:                     ServerIVs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Encryptions
//        File Name:                ServerIVs
//
//        created by Charley at 2015/9/11 17:13:25
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Encryptions
{
    /// <summary>
    /// 加密向量
    /// </summary>
    public class ServerIVs
    {

        /// <summary>
        /// 向量长度
        /// </summary>
        public const int LENGTH_IV = 16;


        #region 向量

        //internal const string VCT_ENCRYPTION_AES_IVEC_LOW = "4I!9V6X8Af98^5bC";

        internal const string VCT_ENCRYPTION_AES_IVEC_LOW = "4dgf!ivh)oz%uxz6";

        internal const string VCT_ENCRYPTION_AES_IVEC_NORMAL = ")26V!7Cwh6i5Y1Qd";

        internal const string VCT_ENCRYPTION_AES_IVEC_HIGH = "ev04Z0VDLn!58}]o";

        #endregion

    }
}
