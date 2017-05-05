//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    a376d647-04c5-4457-b946-573bffe146bf
//        CLR Version:              4.0.30319.42000
//        Name:                     IFont
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.SS.UserModel
//        File Name:                IFont
//
//        Created by Charley at 2016/9/30 15:24:44
//        http://www.voicecyber.com 
//
//======================================================================

using System;


namespace VoiceCyber.NPOI.SS.UserModel
{
    public enum FontUnderlineType : byte
    {
        /**
         * not underlined
         */
        None = 0,
        /**
         * single (normal) underline
         */
        Single = 1,
        /**
         * double underlined
         */
        Double = 2,
        /**
         * accounting style single underline
         */
        SingleAccounting = 0x21,
        /**
         * accounting style double underline
         */
        DoubleAccounting = 0x22
    }

    public enum FontSuperScript : short
    {

        /**
         * no type Offsetting (not super or subscript)
         */

        None = 0,

        /**
         * superscript
         */

        Super = 1,

        /**
         * subscript
         */

        Sub = 2,
    }

    public enum FontColor : short
    {
        /// <summary>
        /// Allow accessing the Initial value.
        /// </summary>
        None = 0,

        /**
         * normal type of black color.
         */

        Normal = 0x7fff,

        /**
         * Dark Red color
         */

        Red = 0xa,
    }

    public enum FontBoldWeight : short
    {
        /// <summary>
        /// Allow accessing the Initial value.
        /// </summary>
        None = 0,

        /**
         * Normal boldness (not bold)
         */

        Normal = 0x190,

        /**
         * Bold boldness (bold)
         */

        Bold = 0x2bc,
    }


    public interface IFont
    {

        /**
         * get the name for the font (i.e. Arial)
         * @return String representing the name of the font to use
         */

        String FontName { get; set; }

        /**
         * get the font height in unit's of 1/20th of a point.  Maybe you might want to
         * use the GetFontHeightInPoints which matches to the familiar 10, 12, 14 etc..
         * @return short - height in 1/20ths of a point
         * @see #GetFontHeightInPoints()
         */

        double FontHeight { get; set; }

        /**
         * get the font height
         * @return short - height in the familiar unit of measure - points
         * @see #GetFontHeight()
         */

        short FontHeightInPoints { get; set; }

        /**
         * get whether to use italics or not
         * @return italics or not
         */

        bool IsItalic { get; set; }


        /**
         * get whether to use a strikeout horizontal line through the text or not
         * @return strikeout or not
         */

        bool IsStrikeout { get; set; }

        /**
         * get the color for the font
         * @return color to use
         * @see #COLOR_NORMAL
         * @see #COLOR_RED
         * @see NPOI.HSSF.usermodel.HSSFPalette#GetColor(short)
         */
        short Color { get; set; }


        /**
         * get normal,super or subscript.
         * @return offset type to use (none,super,sub)
         */

        FontSuperScript TypeOffset { get; set; }

        /**
         * get type of text underlining to use
         * @return underlining type
         */

        FontUnderlineType Underline { get; set; }

        /**
         * get character-set to use.
         * @return character-set
         * @see #ANSI_CHARSET
         * @see #DEFAULT_CHARSET
         * @see #SYMBOL_CHARSET
         */
        short Charset { get; set; }

        /**
         * get the index within the XSSFWorkbook (sequence within the collection of Font objects)
         * 
         * @return unique index number of the underlying record this Font represents (probably you don't care
         *  unless you're comparing which one is which)
         */
        short Index { get; }

        short Boldweight { get; set; }

        bool IsBold { get; set; }
    }
}
