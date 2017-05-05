//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    e2e110d7-1567-4850-b69f-83eec31002b4
//        CLR Version:              4.0.30319.42000
//        Name:                     NamePtg
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.SS.Formula.PTG
//        File Name:                NamePtg
//
//        Created by Charley at 2016/9/30 16:41:26
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using VoiceCyber.NPOI.Util;


namespace VoiceCyber.NPOI.SS.Formula.PTG
{
    /**
        *
        * @author  andy
        * @author Jason Height (jheight at chariot dot net dot au)
        */
    [Serializable]
    public class NamePtg : OperandPtg, WorkbookDependentFormula
    {
        public const short sid = 0x23;
        private const int SIZE = 5;
        /** one-based index to defined name record */
        private int field_1_label_index;
        private short field_2_zero;   // reserved must be 0
        /**
 * @param nameIndex zero-based index to name within workbook
 */
        public NamePtg(int nameIndex)
        {
            field_1_label_index = 1 + nameIndex; // convert to 1-based
        }

        /** Creates new NamePtg */

        public NamePtg(ILittleEndianInput in1)
        {
            field_1_label_index = in1.ReadShort();
            field_2_zero = in1.ReadShort();
        }

        /**
         * @return zero based index to a defined name record in the LinkTable
         */
        public int Index
        {
            get { return field_1_label_index - 1; } // Convert to zero based
        }

        public override void Write(ILittleEndianOutput out1)
        {
            out1.WriteByte(sid + PtgClass);
            out1.WriteShort(field_1_label_index);
            out1.WriteShort(field_2_zero);
        }


        public override int Size
        {
            get { return SIZE; }
        }

        public String ToFormulaString(IFormulaRenderingWorkbook book)
        {
            return book.GetNameText(this);
        }
        public override String ToFormulaString()
        {
            throw new NotImplementedException("3D references need a workbook to determine formula text");
        }


        public override byte DefaultOperandClass
        {
            get { return Ptg.CLASS_REF; }
        }
    }
}
