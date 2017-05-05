//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    c9d1e75a-c820-443b-a8e1-76020380a72a
//        CLR Version:              4.0.30319.42000
//        Name:                     FillPattern
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.SS.UserModel
//        File Name:                FillPattern
//
//        Created by Charley at 2016/9/30 15:30:00
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.NPOI.SS.UserModel
{
    /**
      * The enumeration value indicating the style of fill pattern being used for a cell format.
      * 
      */
    public enum FillPattern : short
    {

        /**  No background */
        NoFill = 0,

        /**  Solidly Filled */
        SolidForeground = 1,

        /**  Small fine dots */
        FineDots = 2,

        /**  Wide dots */
        AltBars = 3,

        /**  Sparse dots */
        SparseDots = 4,

        /**  Thick horizontal bands */
        ThickHorizontalBands = 5,

        /**  Thick vertical bands */
        ThickVerticalBands = 6,

        /**  Thick backward facing diagonals */
        ThickBackwardDiagonals = 7,

        /**  Thick forward facing diagonals */
        ThickForwardDiagonals = 8,

        /**  Large spots */
        BigSpots = 9,

        /**  Brick-like layout */
        Bricks = 10,

        /**  Thin horizontal bands */
        ThinHorizontalBands = 11,

        /**  Thin vertical bands */
        ThinVerticalBands = 12,

        /**  Thin backward diagonal */
        ThinBackwardDiagonals = 13,

        /**  Thin forward diagonal */
        ThinForwardDiagonals = 14,

        /**  Squares */
        Squares = 15,

        /**  Diamonds */
        Diamonds = 16,

        /**  Less Dots */
        LessDots = 17,

        /**  Least Dots */
        LeastDots = 18
    }
}
