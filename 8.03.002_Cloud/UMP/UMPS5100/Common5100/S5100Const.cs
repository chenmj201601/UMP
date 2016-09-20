using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common5100
{
    public class S5100Const
    {
        #region bookmark等级的相关操作
        public const long OPT_BookmarkLevelAdd = 5101001;
        public const long OPT_BookmarkLevelModify = 5101002;
        public const long OPT_BookmarkLevelEnable= 5101003;
        public const long OPT_BookmarkLevelDisable = 5101004;
        public const long OPT_BookmarkLevelDelete = 5101005;
        #endregion

        #region KeyWorld的相关操作
        public const long OPT_KeyWorldAdd = 5102001;
        public const long OPT_KeyWroldModify = 5102002;
        public const long OPT_KeyWorldDelete = 5102005;
        #endregion

        #region 加密密钥
        public static string VCT_KEY128_LOW1 = "UvjCat20^q23df7X";					    // 低保密等级加密密钥1，仅可开放给VCC，用于数据库登录密码等第三方数据的加密。
        public static string VCT_KEY128_LOW2 = "&3jFs^%340KfncS_";					    // 低保密等级加密密钥2，仅可开放给VCC，用于数据库登录密码等第三方数据的加密。
        public static string VCT_KEY128_LOW3 = "!2&fnj*03JvcnRa$";					    // 低保密等级加密密钥3，仅可开放给VCC，用于数据库登录密码等第三方数据的加密。
        public static string VCT_KEY128_NORMAL1 = "^kgCtk%1K)ej4Afa";					// 一般保密等级加密密钥1，对VCT以外保密，用于数据传输过程中的加密
        public static string VCT_KEY128_NORMAL2 = "~cYs&50LKgm)2@Sh";					// 一般保密等级加密密钥2，对VCT以外保密，用于数据传输过程中的加密
        public static string VCT_KEY128_NORMAL3 = "%isA67VGf@c9>,+z";					// 一般保密等级加密密钥3，对VCT以外保密，用于数据传输过程中的加密
        public static string VCT_KEY128_HIGH1 = "4sYda3(24safafQ%";					    // 高保密等级加密密钥1，对VCT以外保密，用于客户数据、加密系统密钥的加密
        public static string VCT_KEY128_HIGH2 = "Fh*3,>)r7X_ag!39";					    // 高保密等级加密密钥2，对VCT以外保密，用于客户数据、加密系统密钥的加密
        public static string VCT_KEY128_HIGH3 = "kG56*20+&fc[3Jas";					    // 高保密等级加密密钥3，对VCT以外保密，用于客户数据、加密系统密钥的加密
        public static string VCT_KEY256_LOW1 = "74kjs&33$2240JsfkUgalaujYRDCasa#";	    // 低保密等级加密密钥1，仅可开放给VCC，用于数据库登录密码等第三方数据的加密。
        public static string VCT_KEY256_LOW2 = "4sfc[3J24safafQ%kG56*20+xfc[3Jas";	    // 低保密等级加密密钥2，仅可开放给VCC，用于数据库登录密码等第三方数据的加密。
        public static string VCT_KEY256_LOW3 = "724safafQ%kG56*2033YRDCr7X_(asa#";	    // 低保密等级加密密钥3，仅可开放给VCC，用于数据库登录密码等第三方数据的加密。
        public static string VCT_KEY256_NORMAL1 = "%^2kfgXCgtHk2%719Kf)1edj^4jAdfRa";	// 一般保密等级加密密钥1，对VCT以外保密，用于数据传输过程中的加密
        public static string VCT_KEY256_NORMAL2 = "$k2%fgcnRa$k2%4jA1edj^4at2ad#8d.";	// 一般保密等级加密密钥2，对VCT以外保密，用于数据传输过程中的加密
        public static string VCT_KEY256_NORMAL3 = "*tk%k2d*3cC1edBej4d%3#-x3()feL4a";	// 一般保密等级加密密钥3，对VCT以外保密，用于数据传输过程中的加密
        public static string VCT_KEY256_HIGH1 = "&4jskYBdfa%33()234'sjasfFabfTQA%";	    // 高保密等级加密密钥1，对VCT以外保密，用于客户数据、加密系统密钥的加密
        public static string VCT_KEY256_HIGH2 = "&$ofm*fM<dgta%z0%>*?hFa!24A)j*Sl";	    // 高保密等级加密密钥2，对VCT以外保密，用于客户数据、加密系统密钥的加密
        public static string VCT_KEY256_HIGH3 = "#$szL3sl(>!e3uAczX?!8)cc36+,a7h@";	    // 高保密等级加密密钥3，对VCT以外保密，用于客户数据、加密系统密钥的加密
        #endregion

        #region 加密向量
        public static string VCT_ENCRYPTION_AES_IVEC_OLD = "0000000000000000";	        // cbc模式加密向量(兼容以前老的程序)
        public static string VCT_ENCRYPTION_AES_IVEC_LOW = "4I!9V6X8Af98^5bC";	        // cbc模式加密向量(低保密等级,仅可开放给VCC)
        public static string VCT_ENCRYPTION_AES_IVEC_NORMAL = ")26V!7Cwh6i5Y1Qd";	    // cbc模式加密向量(一般保密等级,对VCT以外保密)
        public static string VCT_ENCRYPTION_AES_IVEC_HIGH = "ev04Z0VDLn!58}]o";         // cbc模式加密向量(高保密等级，对VCT以外保密)
        #endregion
    }

    public enum OperationType
    {
        Add = 0,
        Modify = 1,
        Delete = 2,
        Enable = 3,
        Disable = 4
    }
}
