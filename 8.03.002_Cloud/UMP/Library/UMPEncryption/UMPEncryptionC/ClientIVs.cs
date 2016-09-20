//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3ca08b20-0c99-40d9-a533-1e8e7c8c1267
//        CLR Version:              4.0.30319.18063
//        Name:                     ClientIVs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Encryptions
//        File Name:                ClientIVs
//
//        created by Charley at 2015/9/11 17:39:46
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Encryptions
{
    public class ClientIVs
    {
        /// <summary>
        /// 密钥长度
        /// </summary>
        public const int LENGTH_IV = 16;


        #region 向量

        internal const string VCT_ENCRYPTION_AES_IVEC_LOW = "4I!9V6X8Af98^5bC";

        #endregion
    }
}
