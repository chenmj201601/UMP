//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    08829e23-15c1-4743-b2c3-8d1d7333315b
//        CLR Version:              4.0.30319.42000
//        Name:                     BorderStyle
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.SS.UserModel
//        File Name:                BorderStyle
//
//        Created by Charley at 2016/9/30 15:29:20
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.NPOI.SS.UserModel
{
    /// <summary>
    /// The enumeration value indicating the line style of a border in a cell
    /// </summary>
    public enum BorderStyle : short
    {
        /// <summary>
        /// No border
        /// </summary>
        None = 0x0,

        /// <summary>
        /// Thin border
        /// </summary>
        Thin = 0x1,

        /// <summary>
        /// Medium border
        /// </summary>
        Medium = 0x2,

        /// <summary>
        /// dash border
        /// </summary>
        Dashed = 0x3,

        /// <summary>
        /// dot border
        /// </summary>
        Dotted = 0x4,

        /// <summary>
        /// Thick border
        /// </summary>
        Thick = 0x5,

        /// <summary>
        /// double-line border
        /// </summary>
        Double = 0x6,

        /// <summary>
        /// hair-line border
        /// </summary>
        Hair = 0x7,

        /// <summary>
        /// Medium dashed border
        /// </summary>
        MediumDashed = 0x8,

        /// <summary>
        /// dash-dot border
        /// </summary>
        DashDot = 0x9,

        /// <summary>
        /// medium dash-dot border
        /// </summary>
        MediumDashDot = 0xA,

        /// <summary>
        /// dash-dot-dot border
        /// </summary>
        DashDotDot = 0xB,

        /// <summary>
        /// medium dash-dot-dot border
        /// </summary>
        MediumDashDotDot = 0xC,

        /// <summary>
        /// slanted dash-dot border
        /// </summary>
        SlantedDashDot = 0xD
    }
}
