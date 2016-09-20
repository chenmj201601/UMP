//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d2190a2e-c53c-481d-a5e5-f2b299bc5cf5
//        CLR Version:              4.0.30319.18444
//        Name:                     EncryptionMode
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common
//        File Name:                EncryptionMode
//
//        created by Charley at 2015/3/16 15:08:29
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common
{
    /// <summary>
    /// 加密模式
    /// 由三部分组成
    /// 1、加密类型（1位），可以是AES，SHA等
    /// 2、加密版本（2位），即密钥向量对，仅用于AES加密方式
    /// 3、编码方式（1位），可以是Hex，Base64等
    /// </summary>
    public enum EncryptionMode
    {
        /// <summary>
        /// 无加密
        /// </summary>
        None = 0,
        /// <summary>
        /// AES256 加密方式
        /// 编码方式为Hex
        /// </summary>
        AES256V00Hex = 1001,
        /// <summary>
        /// AES256 加密方式
        /// M001加密，M101解密
        /// 编码方式为Hex
        /// </summary>
        AES256V01Hex = 1011,
        /// <summary>
        /// AES256 加密方式
        /// M002加密，M102解密
        /// 编码方式为Hex
        /// </summary>
        AES256V02Hex = 1021,
        /// <summary>
        /// AES256 加密方式
        /// M003加密，M103解密
        /// 编码方式为Hex
        /// </summary>
        AES256V03Hex = 1031,
        /// <summary>
        /// AES256 加密方式
        /// M004加密，M104解密
        /// 编码方式为Hex
        /// </summary>
        AES256V04Hex = 1041,
        /// <summary>
        /// AES256 加密方式
        /// M005加密，M105解密
        /// 编码方式为Hex
        /// </summary>
        AES256V05Hex = 1051,
        /// <summary>
        /// AES256 加密方式
        /// M006加密，M106解密
        /// 编码方式为Hex
        /// </summary>
        AES256V06Hex = 1061,
        /// <summary>
        /// AES256 加密方式
        /// M007加密，M107解密
        /// 编码方式为Hex
        /// </summary>
        AES256V07Hex = 1071,
        /// <summary>
        /// AES256 加密方式
        /// M008加密，M108解密
        /// 编码方式为Hex
        /// </summary>
        AES256V08Hex = 1081,
        /// <summary>
        /// AES256 加密方式
        /// M009加密，M109解密
        /// 编码方式为Hex
        /// </summary>
        AES256V09Hex = 1091,
        /// <summary>
        /// AES256 加密方式
        /// M010加密，M110解密
        /// 编码方式为Hex
        /// </summary>
        AES256V10Hex = 1101,
        /// <summary>
        /// AES256 加密方式
        /// M011加密，M111解密
        /// 编码方式为Hex
        /// </summary>
        AES256V11Hex = 1111,
        /// <summary>
        /// AES256 加密方式
        /// M012加密，M112解密
        /// 编码方式为Hex
        /// </summary>
        AES256V12Hex = 1121,
        /// <summary>
        /// AES256 加密方式
        /// M013加密，M113解密
        /// 编码方式为Hex
        /// </summary>
        AES256V13Hex = 1131,
        /// <summary>
        /// AES256 加密方式
        /// M014加密，M114解密
        /// 编码方式为Hex
        /// </summary>
        AES256V14Hex = 1141,
        /// <summary>
        /// AES256 加密方式
        /// M015加密，M115解密
        /// 编码方式为Hex
        /// </summary>
        AES256V15Hex = 1151,
        /// <summary>
        /// AES256 加密方式
        /// M016加密，M116解密
        /// 编码方式为Hex
        /// </summary>
        AES256V16Hex = 1161,
        /// <summary>
        /// AES256 加密方式
        /// M017加密，M117解密
        /// 编码方式为Hex
        /// </summary>
        AES256V17Hex = 1171,
        /// <summary>
        /// AES256 加密方式
        /// M018加密，M118解密
        /// 编码方式为Hex
        /// </summary>
        AES256V18Hex = 1181,
        /// <summary>
        /// AES256 加密方式
        /// M019加密，M119解密
        /// 编码方式为Hex
        /// </summary>
        AES256V19Hex = 1191,
        /// <summary>
        /// AES256 加密方式
        /// M020加密，M120解密
        /// 编码方式为Hex
        /// </summary>
        AES256V20Hex = 1201,
        /// <summary>
        /// AES256 加密方式
        /// M021加密，M121解密
        /// 编码方式为Hex
        /// </summary>
        AES256V21Hex = 1211,
        /// <summary>
        /// AES256 加密方式
        /// M022加密，M122解密
        /// 编码方式为Hex
        /// </summary>
        AES256V22Hex = 1221,
        /// <summary>
        /// AES256 加密方式
        /// M023加密，M123解密
        /// 编码方式为Hex
        /// </summary>
        AES256V23Hex = 1231,
        /// <summary>
        /// AES256 加密方式
        /// M024加密，M124解密
        /// 编码方式为Hex
        /// </summary>
        AES256V24Hex = 1241,
        /// <summary>
        /// AES256 加密方式
        /// M025加密，M125解密
        /// 编码方式为Hex
        /// </summary>
        AES256V25Hex = 1251,
        /// <summary>
        /// AES256 加密方式
        /// M026加密，M126解密
        /// 编码方式为Hex
        /// </summary>
        AES256V26Hex = 1261,
        /// <summary>
        /// AES256 加密方式
        /// M027加密，M127解密
        /// 编码方式为Hex
        /// </summary>
        AES256V27Hex = 1271,
        /// <summary>
        /// SHA256 加密方式
        /// 编码方式为Hex
        /// </summary>
        SHA256V00Hex = 2001,
        /// <summary>
        /// SHA512 加密方式
        /// 编码方式为Hex
        /// </summary>
        SHA512V00Hex = 3001,

        /// <summary>
        /// AES256 加密方式
        /// 编码方式为Base64
        /// </summary>
        AES256V00B64 = 1002,
        /// <summary>
        /// AES256 加密方式
        /// M001加密，M101解密
        /// 编码方式为Base64
        /// </summary>
        AES256V01B64 = 1012,
        /// <summary>
        /// AES256 加密方式
        /// M002加密，M102解密
        /// 编码方式为Base64
        /// </summary>
        AES256V02B64 = 1022,
        /// <summary>
        /// AES256 加密方式
        /// M003加密，M103解密
        /// 编码方式为Base64
        /// </summary>
        AES256V03B64 = 1032,
        /// <summary>
        /// AES256 加密方式
        /// M004加密，M104解密
        /// 编码方式为Base64
        /// </summary>
        AES256V04B64 = 1042,
        /// <summary>
        /// AES256 加密方式
        /// M005加密，M105解密
        /// 编码方式为Base64
        /// </summary>
        AES256V05B64 = 1052,
        /// <summary>
        /// AES256 加密方式
        /// M006加密，M106解密
        /// 编码方式为Base64
        /// </summary>
        AES256V06B64 = 1062,
        /// <summary>
        /// AES256 加密方式
        /// M007加密，M107解密
        /// 编码方式为Base64
        /// </summary>
        AES256V07B64 = 1072,
        /// <summary>
        /// AES256 加密方式
        /// M008加密，M108解密
        /// 编码方式为Base64
        /// </summary>
        AES256V08B64 = 1082,
        /// <summary>
        /// AES256 加密方式
        /// M009加密，M109解密
        /// 编码方式为Base64
        /// </summary>
        AES256V09B64 = 1092,
        /// <summary>
        /// AES256 加密方式
        /// M010加密，M110解密
        /// 编码方式为Base64
        /// </summary>
        AES256V10B64 = 1102,
        /// <summary>
        /// AES256 加密方式
        /// M011加密，M111解密
        /// 编码方式为Base64
        /// </summary>
        AES256V11B64 = 1112,
        /// <summary>
        /// AES256 加密方式
        /// M012加密，M112解密
        /// 编码方式为Base64
        /// </summary>
        AES256V12B64 = 1122,
        /// <summary>
        /// AES256 加密方式
        /// M013加密，M113解密
        /// 编码方式为Base64
        /// </summary>
        AES256V13B64 = 1132,
        /// <summary>
        /// AES256 加密方式
        /// M014加密，M114解密
        /// 编码方式为Base64
        /// </summary>
        AES256V14B64 = 1142,
        /// <summary>
        /// AES256 加密方式
        /// M015加密，M115解密
        /// 编码方式为Base64
        /// </summary>
        AES256V15B64 = 1152,
        /// <summary>
        /// AES256 加密方式
        /// M016加密，M116解密
        /// 编码方式为Base64
        /// </summary>
        AES256V16B64 = 1162,
        /// <summary>
        /// AES256 加密方式
        /// M017加密，M117解密
        /// 编码方式为Base64
        /// </summary>
        AES256V17B64 = 1172,
        /// <summary>
        /// AES256 加密方式
        /// M018加密，M118解密
        /// 编码方式为Base64
        /// </summary>
        AES256V18B64 = 1182,
        /// <summary>
        /// AES256 加密方式
        /// M019加密，M119解密
        /// 编码方式为Base64
        /// </summary>
        AES256V19B64 = 1192,
        /// <summary>
        /// AES256 加密方式
        /// M020加密，M120解密
        /// 编码方式为Base64
        /// </summary>
        AES256V20B64 = 1202,
        /// <summary>
        /// AES256 加密方式
        /// M021加密，M121解密
        /// 编码方式为Base64
        /// </summary>
        AES256V21B64 = 1212,
        /// <summary>
        /// AES256 加密方式
        /// M022加密，M122解密
        /// 编码方式为Base64
        /// </summary>
        AES256V22B64 = 1222,
        /// <summary>
        /// AES256 加密方式
        /// M023加密，M123解密
        /// 编码方式为Base64
        /// </summary>
        AES256V23B64 = 1232,
        /// <summary>
        /// AES256 加密方式
        /// M024加密，M124解密
        /// 编码方式为Base64
        /// </summary>
        AES256V24B64 = 1242,
        /// <summary>
        /// AES256 加密方式
        /// M025加密，M125解密
        /// 编码方式为Base64
        /// </summary>
        AES256V25B64 = 1252,
        /// <summary>
        /// AES256 加密方式
        /// M026加密，M126解密
        /// 编码方式为Base64
        /// </summary>
        AES256V26B64 = 1262,
        /// <summary>
        /// AES256 加密方式
        /// M027加密，M127解密
        /// 编码方式为Base64
        /// </summary>
        AES256V27B64 = 1272,
        /// <summary>
        /// SHA256 加密方式
        /// 编码方式为Base64
        /// </summary>
        SHA256V00B64 = 2002,
        /// <summary>
        /// SHA512 加密方式
        /// 编码方式为Base64
        /// </summary>
        SHA512V00B64 = 3002,
    }
}
