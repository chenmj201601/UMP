//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    2cbf6096-0f45-4010-affd-20465a7ca8a4
//        CLR Version:              4.0.30319.42000
//        Name:                     IClientAnchor
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.SS.UserModel
//        File Name:                IClientAnchor
//
//        Created by Charley at 2016/9/30 15:34:44
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.NPOI.SS.UserModel
{
    public enum AnchorType
    {
        /**
         * Move and Resize With Anchor Cells
         * <p>
         * Specifies that the current drawing shall move and
         * resize to maintain its row and column anchors (i.e. the
         * object is anchored to the actual from and to row and column)
         * </p>
         */
        MoveAndResize = 0,

        /**
         * Move With Cells but Do Not Resize
         * <p>
         * Specifies that the current drawing shall move with its
         * row and column (i.e. the object is anchored to the
         * actual from row and column), but that the size shall remain absolute.
         * </p>
         * <p>
         * If Additional rows/columns are Added between the from and to locations of the drawing,
         * the drawing shall move its to anchors as needed to maintain this same absolute size.
         * </p>
         */
        MoveDontResize = 2,

        /**
         * Do Not Move or Resize With Underlying Rows/Columns
         * <p>
         * Specifies that the current start and end positions shall
         * be maintained with respect to the distances from the
         * absolute start point of the worksheet.
         * </p>
         * <p>
         * If Additional rows/columns are Added before the
         * drawing, the drawing shall move its anchors as needed
         * to maintain this same absolute position.
         * </p>
         */
        DontMoveAndResize = 3

    }

    /**
     * A client anchor is attached to an excel worksheet.  It anchors against a
     * top-left and bottom-right cell.
     *
     * @author Yegor Kozlov
     */
    public interface IClientAnchor
    {

        /**
         * Returns the column (0 based) of the first cell.
         *
         * @return 0-based column of the first cell.
         */
        int Col1 { get; set; }

        /**
         * Returns the column (0 based) of the second cell.
         *
         * @return 0-based column of the second cell.
         */
        int Col2 { get; set; }

        /**
         * Returns the row (0 based) of the first cell.
         *
         * @return 0-based row of the first cell.
         */
        int Row1 { get; set; }


        /**
         * Returns the row (0 based) of the second cell.
         *
         * @return 0-based row of the second cell.
         */
        int Row2 { get; set; }


        /**
         * Returns the x coordinate within the first cell
         *
         * @return the x coordinate within the first cell
         */
        int Dx1 { get; set; }


        /**
         * Returns the y coordinate within the first cell
         *
         * @return the y coordinate within the first cell
         */
        int Dy1 { get; set; }


        /**
         * Sets the y coordinate within the second cell
         *
         * @return the y coordinate within the second cell
         */
        int Dy2 { get; set; }

        /**
         * Returns the x coordinate within the second cell
         *
         * @return the x coordinate within the second cell
         */
        int Dx2 { get; set; }


        /**
         * s the anchor type
         * <p>
         * 0 = Move and size with Cells, 2 = Move but don't size with cells, 3 = Don't move or size with cells.
         * </p>
         * @return the anchor type
         * @see #MOVE_AND_RESIZE
         * @see #MOVE_DONT_RESIZE
         * @see #DONT_MOVE_AND_RESIZE
         */
        AnchorType AnchorType { get; set; }

    }
}
