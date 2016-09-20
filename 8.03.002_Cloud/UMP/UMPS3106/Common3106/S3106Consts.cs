using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common3106
{
    /// <summary>
    /// 常量、特定编码
    /// </summary>
    public static class S3106Consts
    {
        //权限
        public const long OPT_New = 3106001;
        public const long OPT_Rename = 3106002;
        public const long OPT_Delete = 3106003;
        public const long OPT_Allot = 3106004;
        public const long OPT_UpLoad = 3106005;
        public const long OPT_DownLoad= 3106006;
        public const long OPT_Play = 3106007;
        public const long OPT_BrowseHistory = 3106009;


        /*====================================================================
         * 
         * ****用户记住的解密密码信息***
         * 1、组编号：310210
         * 2、其后3位序号表示各个服务器
         * 3、范围是310210001 ~ 310210899
         * 4、从310210900到310210999保留特别使用
         * 5、310210900：所有服务器使用相同的密码
         * 参数值的规定：
         * M002(ServerAddress+char27+Password+char27+ExpireTime)
         * 注意：
         * 1、ServerAddress：服务器地址，可空（空表示应用于所有服务器）
         * 2、Password：密码，最大长度64个字符
         * 3、ExpireTime：密码有效截止时间（UTC，格式：yyyyMMddHHmmss）
         * 
         * ===================================================================
         */
        public const int USER_PARAM_GROUP_ENCRYPTINFO = 310210;


        public const int USER_PARAM_SKIPCONDITIONPANEL = 31020103;
        public const int USER_PARAM_QUERYVOICERECORD = 31020104;
        public const int USER_PARAM_QUERYSCREENRECORD = 31020105;
        public const int USER_PARAM_AUTORELATIVEPLAY = 31020106;
        public const int USER_PARAM_SKIPPASSWORDPANEL = 31020107;

        public const int USER_PARAM_EXPORTDATA_TYPE = 31020201;
        public const int USER_PARAM_EXPORTDATA_REMEMBER = 31020202;
        public const int USER_PARAM_EXPORTDATA_NOTSHOW = 31020203;

        public const int USER_PARAM_EXPORTRECORD_REMEMBER = 31020301;
        public const int USER_PARAM_EXPORTRECORD_NOTSHOW = 31020302;
        public const int USER_PARAM_EXPORTRECORD_SAVEDIR = 31020303;
        public const int USER_PARAM_EXPORTRECORD_PATHFORMAT = 31020304;
        public const int USER_PARAM_EXPORTRECORD_CONVERTPCM = 31020305;
        public const int USER_PARAM_EXPORTRECORD_DECRYPTFILE = 31020306;
        public const int USER_PARAM_EXPORTRECORD_GENERATEDB = 31020307;
        public const int USER_PARAM_EXPORTRECORD_REPLACEFILE = 31020308;
        public const int USER_PARAM_EXPORTRECORD_EXPORTVOICE = 31020309;
        public const int USER_PARAM_EXPORTRECORD_EXPORTSCREEN = 31020310;
        public const int USER_PARAM_EXPORTRECORD_IGNOREPATHFORMAT = 31020311;

        public const int USER_PARAM_PLAYSCREEN_NOPLAY = 31020401;
        public const int USER_PARAM_PLAYSCREEN_TOPMOST = 31020402;
        public const int USER_PARAM_PLAYSCREEN_SCALE = 31020403;

    }
}
