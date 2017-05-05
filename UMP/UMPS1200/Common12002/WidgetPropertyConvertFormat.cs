//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    5972b850-3dcc-45e8-9d7c-6e205b71ec8f
//        CLR Version:              4.0.30319.18408
//        Name:                     WidgetPropertyConvertFormat
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common12002
//        File Name:                WidgetPropertyConvertFormat
//
//        created by Charley at 2016/5/3 15:55:21
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common12002
{
    public enum WidgetPropertyConvertFormat
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
        /// 时间（ 00:00:00 的形式）
        /// </summary>
        Time = 105,
        /// <summary>
        /// 日期时间（ yyyy-MM-dd HH:mm:ss 的形式）
        /// </summary>
        DateTime = 106,
        /// <summary>
        /// 是否单选
        /// </summary>
        YesNo = 200,
        /// <summary>
        /// 启用，禁用单选
        /// </summary>
        EnableDisable = 201,
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
    }
}
