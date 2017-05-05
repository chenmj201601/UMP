//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    3a304011-2c01-4d67-81ac-7d06b15165b8
//        CLR Version:              4.0.30319.42000
//        Name:                     FuncPtg
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.SS.Formula.PTG
//        File Name:                FuncPtg
//
//        Created by Charley at 2016/9/30 16:31:56
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using VoiceCyber.NPOI.Util;
using VoiceCyber.NPOI.SS.Formula.Function;


namespace VoiceCyber.NPOI.SS.Formula.PTG
{
    /**
     * @author aviks
     * @author Jason Height (jheight at chariot dot net dot au)
     * @author Danny Mui (dmui at apache dot org) (Leftover handling)
     */
    [Serializable]
    public class FuncPtg : AbstractFunctionPtg
    {

        public const byte sid = 0x21;
        public const int SIZE = 3;
        // not used: private int numParams = 0;
        public static FuncPtg Create(ILittleEndianInput in1)
        {
            return Create(in1.ReadUShort());
        }
        private FuncPtg(int funcIndex, FunctionMetadata fm) :
            base(funcIndex, fm.ReturnClassCode, fm.ParameterClassCodes, fm.MinParams)  // minParams same as max since these are not var-arg funcs {
        {
        }
        public static FuncPtg Create(int functionIndex)
        {
            FunctionMetadata fm = FunctionMetadataRegistry.GetFunctionByIndex(functionIndex);
            if (fm == null)
            {
                throw new Exception("Invalid built-in function index (" + functionIndex + ")");
            }
            return new FuncPtg(functionIndex, fm);
        }

        public override void Write(ILittleEndianOutput out1)
        {
            out1.WriteByte(sid + PtgClass);
            out1.WriteShort(_functionIndex);
        }
        public override int Size
        {
            get { return SIZE; }
        }
    }
}
