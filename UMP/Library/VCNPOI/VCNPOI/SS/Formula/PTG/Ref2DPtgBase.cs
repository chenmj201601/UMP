//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    13fd664b-b8f7-4ffc-8c0b-40f77c969ef0
//        CLR Version:              4.0.30319.42000
//        Name:                     Ref2DPtgBase
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.SS.Formula.PTG
//        File Name:                Ref2DPtgBase
//
//        Created by Charley at 2016/9/30 16:45:32
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Text;
using VoiceCyber.NPOI.SS.Util;
using VoiceCyber.NPOI.Util;


namespace VoiceCyber.NPOI.SS.Formula.PTG
{
    /**
      * @author Josh Micich
      */
    [Serializable]
    public abstract class Ref2DPtgBase : RefPtgBase
    {
        private const int SIZE = 5;

        /**
         * Takes in a String representation of a cell reference and fills out the
         * numeric fields.
         */
        protected Ref2DPtgBase(String cellref)
            : base(cellref)
        {

        }
        protected Ref2DPtgBase(CellReference cr)
            : base(cr)
        {

        }

        protected Ref2DPtgBase(int row, int column, bool isRowRelative, bool isColumnRelative)
        {
            Row = (row);
            Column = (column);
            IsRowRelative = (isRowRelative);
            IsColRelative = (isColumnRelative);
        }

        protected Ref2DPtgBase(ILittleEndianInput in1)
        {
            ReadCoordinates(in1);
        }
        public override void Write(ILittleEndianOutput out1)
        {
            out1.WriteByte(Sid + PtgClass);
            WriteCoordinates(out1);
        }
        public override String ToFormulaString()
        {
            return FormatReferenceAsString();
        }

        protected abstract byte Sid { get; }

        public override int Size
        {
            get
            {
                return SIZE;
            }
        }
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetType().Name);
            sb.Append(" [");
            sb.Append(FormatReferenceAsString());
            sb.Append("]");
            return sb.ToString();
        }
    }
}
