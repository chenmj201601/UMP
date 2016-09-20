//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    f6ce0902-8825-4c84-b773-8168ecceb9a8
//        CLR Version:              4.0.30319.18444
//        Name:                     CheckValidResult
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets
//        File Name:                CheckValidResult
//
//        created by Charley at 2014/6/23 9:48:18
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.ScoreSheets
{
    /// <summary>
    /// 评分表检查结果
    /// </summary>
    public class CheckValidResult
    {
        /// <summary>
        /// 结果代码
        /// 0       通过检查
        /// 100     子标准类型不正确
        /// 110     总分不匹配
        /// 111     分制不匹配
        /// 201     总分设置无效
        /// 202     分制设置无效
        /// 300     值超出范围
        /// 999     发生异常
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 对应的评分对象
        /// </summary>
        public ScoreObject ScoreObject { get; set; }
        /// <summary>
        /// 文本消息
        /// </summary>
        public string Message { get; set; }
    }
    /// <summary>
    /// 错误码
    /// </summary>
    public class CheckValidResultCodes
    {
        /// <summary>
        /// 通过检查
        /// </summary>
        public const int SUCCESS = 0;
        /// <summary>
        /// 子标准类型不正确
        /// </summary>
        public const int STANDARDTYPE_INVALID = 100;
        /// <summary>
        /// 总分不匹配
        /// </summary>
        public const int TOTALSCORE_INEQUAL = 110;
        /// <summary>
        /// 分制不匹配
        /// </summary>
        public const int PONITSYSTEM_INEQUAL = 111;
        /// <summary>
        /// 总分设置无效
        /// </summary>
        public const int TOTALSCORE_INVALID = 201;
        /// <summary>
        /// 分制设置无效
        /// </summary>
        public const int PONITSYSTEM_INVALID = 202;
        /// <summary>
        /// 值超出范围
        /// </summary>
        public const int VALUE_OUT = 300;
        /// <summary>
        /// 标题不能为空
        /// </summary>
        public const int TITLE_EMPTY = 400;
        /// <summary>
        /// 发生异常
        /// </summary>
        public const int EXCEPTION = 999;
    }
}
