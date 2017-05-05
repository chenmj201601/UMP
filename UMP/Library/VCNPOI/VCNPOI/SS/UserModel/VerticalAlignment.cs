//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    209363c8-2ca5-407a-8c45-b1fcc22d4168
//        CLR Version:              4.0.30319.42000
//        Name:                     VerticalAlignment
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.SS.UserModel
//        File Name:                VerticalAlignment
//
//        Created by Charley at 2016/9/30 15:28:34
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.NPOI.SS.UserModel
{
    /**
     * This enumeration value indicates the type of vertical alignment for a cell, i.e.,
     * whether it is aligned top, bottom, vertically centered, justified or distributed.
     */
    public enum VerticalAlignment
    {
        None = -1,
        /**
         * The vertical alignment is aligned-to-top.
         */
        Top = 0,

        /**
         * The vertical alignment is centered across the height of the cell.
         */
        Center = 1,

        /**
         * The vertical alignment is aligned-to-bottom.
         */
        Bottom = 2,

        /**
         * <p>
         * When text direction is horizontal: the vertical alignment of lines of text is distributed vertically,
         * where each line of text inside the cell is evenly distributed across the height of the cell,
         * with flush top and bottom margins.
         * </p>
         * <p>
         * When text direction is vertical: similar behavior as horizontal justification.
         * The alignment is justified (flush top and bottom in this case). For each line of text, each
         * line of the wrapped text in a cell is aligned to the top and bottom (except the last line).
         * If no single line of text wraps in the cell, then the text is not justified.
         *  </p>
         */
        Justify = 3,

        /** 
         * <p>
         * When text direction is horizontal: the vertical alignment of lines of text is distributed vertically,
         * where each line of text inside the cell is evenly distributed across the height of the cell,
         * with flush top
         * </p>
         * <p>
         * When text direction is vertical: behaves exactly as distributed horizontal alignment.
         * The first words in a line of text (appearing at the top of the cell) are flush
         * with the top edge of the cell, and the last words of a line of text are flush with the bottom edge of the cell,
         * and the line of text is distributed evenly from top to bottom.
         * </p>
         */
        Distributed = 4
    }
}
