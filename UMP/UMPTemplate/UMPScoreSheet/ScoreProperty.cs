//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6d02b35f-70a6-4c1d-8ada-3b68bd21f62f
//        CLR Version:              4.0.30319.18444
//        Name:                     ScoreProperty
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets
//        File Name:                ScoreProperty
//
//        created by Charley at 2014/7/7 15:22:36
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections;
using System.Linq;

namespace VoiceCyber.UMP.ScoreSheets
{
    /// <summary>
    /// 评分表对象的属性
    /// </summary>
    public class ScoreProperty
    {
        /// <summary>
        /// 属性编号
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 显示名
        /// </summary>
        public string Display { get; set; }
        /// <summary>
        /// 属性名
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// 描述信息
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 类别ID
        /// 1       基本属性
        /// 2       扩展属性
        /// </summary>
        public int Category { get; set; }
        /// <summary>
        /// 标志
        /// </summary>
        public ScorePropertyFlag Flag { get; set; }
        /// <summary>
        /// 同组下的排列序号
        /// </summary>
        public int OrderID { get; set; }
        /// <summary>
        /// 属性数据类型
        /// </summary>
        public ScorePropertyDataType DataType { get; set; }
        /// <summary>
        /// 对于枚举类型，指定枚举的类型
        /// </summary>
        public Type ValueType { get; set; }
        /// <summary>
        /// 对于自定义对象列表类型，指定数据源
        /// </summary>
        public IEnumerable DataSource { get; set; }
        /// <summary>
        /// 对于自定义类型，指定显示字段
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 设定属性值
        /// </summary>
        /// <param name="scoreObject"></param>
        /// <param name="value"></param>
        public void SetPropertyValue(ScoreObject scoreObject, object value)
        {
            var properties =scoreObject.GetType().GetProperties();
            var proeprty = properties.FirstOrDefault(p => p.Name == PropertyName);
            if (proeprty != null)
            {
                proeprty.SetValue(scoreObject, value, null);
            }
        }
        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="scoreObject"></param>
        /// <returns></returns>
        public object GetPropertyValue(ScoreObject scoreObject)
        {
            var properties = scoreObject.GetType().GetProperties();
            var proeprty = properties.FirstOrDefault(p => p.Name == PropertyName);
            if (proeprty != null)
            {
                return proeprty.GetValue(scoreObject, null);
            }
            return null;
        }

        #region 属性定义

        //ScoreObject
        public const int PRO_ID = 1;
        public const int PRO_TYPE = 2;
        public const int PRO_DISPLAY = 3;

        //ScoreItem
        public const int PRO_ITEMID = 101;
        public const int PRO_ICONNAME = 102;
        public const int PRO_TITLE = 103;
        public const int PRO_VIEWCLASS = 104;
        public const int PRO_SCORETYPE = 105;
        public const int PRO_TOTALSCORE = 106;
        public const int PRO_SCORE = 107;
        public const int PRO_REALSCORE = 108;
        public const int PRO_ISABORTSCORE = 109;
        public const int PRO_CONTROLFLAG = 110;
        public const int PRO_ISSCORECONTROLED = 111;
        public const int PRO_ISKEYITEM = 112;
        public const int PRO_ISALLOWNA = 113;
        public const int PRO_ISNA = 114;
        public const int PRO_ISJUMPITEM = 115;
        public const int PRO_ISADDITIONITEM = 116;
        public const int PRO_USEPOINTSYSTEM = 117;
        public const int PRO_TITLESTYLE = 118;
        public const int PRO_PANELSTYLE = 119;
        public const int PRO_ORDERID = 120;
        public const int PRO_DESCRIPTION = 121;
        public const int PRO_TIP = 122;
        public const int PRO_LEVEL = 123;
        public const int PRO_ALLOWMODIFYSCORE = 124;

        //ScoreGroup
        public const int PRO_ISAVG = 131;
        public const int PRO_ITEMS = 132;
        public const int PRO_HASAUTOSTANDARD = 133;

        //Standard
        public const int PRO_SCORECLASSIC = 141;
        public const int PRO_STANDARDTYPE = 142;
        public const int PRO_POINTSYSTEM = 143;
        public const int PRO_ISAUTOSTANDARD = 144;
        public const int PRO_STATISTICALID = 145;

        //ScoreSheet
        public const int PRO_CREATOR = 151;
        public const int PRO_CREATETIME = 152;
        public const int PRO_STATUS = 153;
        public const int PRO_DATEFROM = 154;
        public const int PRO_DATETO = 155;
        public const int PRO_USETAG = 156;
        public const int PRO_QUALIFIEDLINE = 157;
        public const int PRO_CALADDITIONALITEM = 158;
        public const int PRO_CONTROLITEMS = 159;

        //NumericStandard
        public const int PRO_N_MINVALUE = 161;
        public const int PRO_N_MAXVALUE = 162;
        public const int PRO_N_DEFAULTVALUE = 163;

        //YesNoStandard
        public const int PRO_Y_DEFAULTVALUE = 166;

        //SliderStandard
        public const int PRO_S_MINVALUE = 171;
        public const int PRO_S_MAXVALUE = 172;
        public const int PRO_S_DEFAULTVALUE = 173;
        public const int PRO_S_INTEVAL = 174;

        //ItemStandard
        public const int PRO_I_DEFAULTVALUE = 176;
        public const int PRO_I_DEFAULTINDEX = 177;
        public const int PRO_I_SELECTVALUE = 178;
        public const int PRO_I_VALUEITEMS = 179;

        //ScoreSheet 扩展
        public const int PRO_SCOREWIDTH = 191;
        public const int PRO_TIPWIDTH = 192;

        //StandardItem
        public const int PRO_SI_DISPLAY = 201;
        public const int PRO_SI_VALUE = 202;

        //Comment
        public const int PRO_C_STYLE = 211;
        public const int PRO_C_TITLE = 212;
        public const int PRO_C_DESCRIPTION = 213;
        public const int PRO_C_ORDERID = 214;
        public const int PRO_C_TITLESTYLE = 215;
        public const int PRO_C_PANELSTYLE = 216;
       
        //TextComment
        public const int PRO_C_DEFAULTTEXT = 221;
        public const int PRO_C_TEXT = 222;

        //ItemComment
        public const int PRO_C_DEFAULTINDEX = 226;
        public const int PRO_C_DEFAULTITEM = 227;
        public const int PRO_C_SELECTVALUE = 228;
        public const int PRO_C_VALUEITEMS = 229;

        //CommentItem
        public const int PRO_CI_TEXT = 231;

        //ControlItem
        public const int PRO_CTL_TITLE = 241;
        public const int PRO_CTL_JUGETYPE = 242;
        public const int PRO_CTL_JUGEVALUE1 = 243;
        public const int PRO_CTL_JUGEVALUE2 = 244;
        public const int PRO_CTL_SOURCE = 245;
        public const int PRO_CTL_SOURCEID = 246;
        public const int PRO_CTL_TARGET = 247;
        public const int PRO_CTL_TARGETID = 248;
        public const int PRO_CTL_CHANGETYPE = 249;
        public const int PRO_CTL_CHANGEVALUE = 250;
        public const int PRO_CTL_ORDERID = 251;

        //VisualStyle
        public const int PRO_VS_FONTSIZE = 261;
        public const int PRO_VS_FONTWEIGHT = 262;
        public const int PRO_VS_FONTFAMILY = 263;
        public const int PRO_VS_FORECOLOR = 264;
        public const int PRO_VS_BACKCOLOR = 265;
        public const int PRO_VS_WIDTH = 266;
        public const int PRO_VS_HEIGHT = 267;

        #endregion
    }

}
