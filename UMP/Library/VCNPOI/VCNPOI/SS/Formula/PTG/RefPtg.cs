//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    ffc00e86-48cb-42da-a99b-c83013f6f39d
//        CLR Version:              4.0.30319.42000
//        Name:                     RefPtg
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.SS.Formula.PTG
//        File Name:                RefPtg
//
//        Created by Charley at 2016/9/30 16:44:55
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using VoiceCyber.NPOI.SS.Util;
using VoiceCyber.NPOI.Util;


namespace VoiceCyber.NPOI.SS.Formula.PTG
{
    /**
     * ReferencePtg - handles references (such as A1, A2, IA4)
     * @author  Andrew C. Oliver (acoliver@apache.org)
     * @author Jason Height (jheight at chariot dot net dot au)
     */
    [Serializable]
    public class RefPtg : Ref2DPtgBase
    {
        public const byte sid = 0x24;

        /**
         * Takes in a String representation of a cell reference and Fills out the
         * numeric fields.
         */
        public RefPtg(String cellref)
            : base(new CellReference(cellref))
        {

        }

        public RefPtg(int row, int column, bool isRowRelative, bool isColumnRelative)
            : base(row, column, isRowRelative, isColumnRelative)
        {
            Row = row;
            Column = column;
            IsRowRelative = isRowRelative;
            IsColRelative = isColumnRelative;
        }

        public RefPtg(ILittleEndianInput in1)
            : base(in1)
        {

        }
        public RefPtg(CellReference cr)
            : base(cr)
        {

        }
        protected override byte Sid
        {
            get { return sid; }
        }

    }
}
