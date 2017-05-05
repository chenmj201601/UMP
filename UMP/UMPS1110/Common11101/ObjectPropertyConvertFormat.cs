//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2471e70e-8d39-4396-9a31-f975a00addb4
//        CLR Version:              4.0.30319.18444
//        Name:                     ObjectPropertyConvertFormat
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                Common11101
//        File Name:                ObjectPropertyConvertFormat
//
//        created by Charley at 2014/12/19 10:39:01
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common11101
{
    /// <summary>
    /// 参数配置类型，指示在配置视图中以怎样的形式显示
    /// </summary>
    public enum ObjectPropertyConvertFormat
    {
        /// <summary>
        /// 不确定，使用TextBlock显示参数值
        /// </summary>
        Unkown = 0,
        /// <summary>
        /// 文本型，直接使用文本框配置参数值
        /// </summary>
        String = 100,
        /// <summary>
        /// 数值型，直接使用整形输入框（IntUpDown）
        /// </summary>
        Numeric = 101,
        /// <summary>
        /// 时间（ 0 的格式,单位小时表示秒。保存到数据库时，放大3600倍。）
        /// </summary>
        Numeric2 = 102,
        /// <summary>
        /// 网络端口，范围限制在0 - 65535
        /// </summary>
        NetPort = 103,
        /// <summary>
        /// IP地址
        /// </summary>
        NetIP = 104,
        /// <summary>
        /// 时间（ 00:00:00 的形式）
        /// </summary>
        Time = 105,
        /// <summary>
        /// 日期时间（ yyyy-MM-dd HH:mm:ss 的形式）
        /// </summary>
        DateTime = 106,
        /// <summary>
        /// 时间（ 00:00 的格式）
        /// </summary>
        Time2 = 107,
        /// <summary>
        /// 密码输入框
        /// </summary>
        Password = 110,
        /// <summary>
        /// 是否单选
        /// </summary>
        YesNo = 200,
        /// <summary>
        /// 启用，禁用单选
        /// </summary>
        EnableDisable = 201,
        /// <summary>
        /// 主机，备机单选
        /// </summary>
        MasterSlaver = 202,
        /// <summary>
        /// 禁用，主机，备机单选
        /// </summary>
        DisableMasterSlaver = 203,
        /// <summary>
        /// 基础信息列表中单选按钮单选
        /// </summary>
        BasicInfoSingleRadio = 210,
        /// <summary>
        /// 基础信息列表中下拉单选
        /// </summary>
        BasicInfoSingleSelect = 300,
        /// <summary>
        /// 下拉单选
        /// </summary>
        ComboSingleSelect = 301,
        /// <summary>
        /// 基础信息列表中下拉单选(可编辑)
        /// </summary>
        BasicInfoSingleEditSelect = 302,
        /// <summary>
        /// 下拉单选（可编辑）
        /// </summary>
        ComboSingleEditSelect = 303,
        /// <summary>
        /// 基础信息列表下拉多选（值按位）
        /// </summary>
        BasicInfoMultiSelect = 310,
        /// <summary>
        /// 基础信息列表下拉多选（值用分号隔开）
        /// </summary>
        BasicInfoMultiSelectSemicolon = 311,
        /// <summary>
        /// 下拉多选（值用分号隔开）
        /// </summary>
        ComboMultiSelect = 312,

        #region 其他特殊格式

        /// <summary>
        /// 目录选择
        /// </summary>
        DirBrowser = 400,
        /// <summary>
        /// CTI服务名称选择框
        /// </summary>
        CTIServiceName = 410,
        /// <summary>
        /// 网卡选择框
        /// </summary>
        NetworkCardName = 420

        #endregion

    }
}
