//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    03117cf6-83a0-4a68-a415-fbfc2e148728
//        CLR Version:              4.0.30319.42000
//        Name:                     FuncVarPtg
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.SS.Formula.PTG
//        File Name:                FuncVarPtg
//
//        Created by Charley at 2016/9/30 16:40:29
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using VoiceCyber.NPOI.SS.Formula.Function;
using VoiceCyber.NPOI.Util;


namespace VoiceCyber.NPOI.SS.Formula.PTG
{
    /**
     *
     * @author Jason Height (jheight at chariot dot net dot au)
     */
    public class FuncVarPtg : AbstractFunctionPtg
    {
        public const byte sid = 0x22;
        private const int SIZE = 4;

        /**
 * Single instance of this token for 'sum() taking a single argument'
 */
        public static readonly OperationPtg SUM = Create("SUM", 1);

        private FuncVarPtg(int functionIndex, int returnClass, byte[] paramClasses, int numArgs)
            : base(functionIndex, returnClass, paramClasses, numArgs)
        {

        }

        /**Creates new function pointer from a byte array
 * usually called while reading an excel file.
 */
        public static FuncVarPtg Create(ILittleEndianInput in1)
        {
            return Create(in1.ReadByte(), in1.ReadShort());
        }

        /**
         * Create a function ptg from a string tokenised by the parser
         */
        public static FuncVarPtg Create(String pName, int numArgs)
        {
            return Create(numArgs, LookupIndex(pName));
        }

        private static FuncVarPtg Create(int numArgs, int functionIndex)
        {
            FunctionMetadata fm = FunctionMetadataRegistry.GetFunctionByIndex(functionIndex);
            if (fm == null)
            {
                // Happens only as a result of a call to FormulaParser.parse(), with a non-built-in function name
                return new FuncVarPtg(functionIndex, CLASS_VALUE, new[] { CLASS_VALUE }, numArgs);
            }
            return new FuncVarPtg(functionIndex, fm.ReturnClassCode, fm.ParameterClassCodes, numArgs);
        }


        public override void Write(ILittleEndianOutput out1)
        {
            out1.WriteByte(sid + PtgClass);
            out1.WriteByte(_numberOfArgs);
            out1.WriteShort(_functionIndex);
        }

        public override int Size
        {
            get { return SIZE; }
        }
    }
}
