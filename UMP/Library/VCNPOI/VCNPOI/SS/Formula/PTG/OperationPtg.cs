//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    df67d5a6-bc09-4eac-aea1-c83b94f96113
//        CLR Version:              4.0.30319.42000
//        Name:                     OperationPtg
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.SS.Formula.PTG
//        File Name:                OperationPtg
//
//        Created by Charley at 2016/9/30 16:32:48
//        http://www.voicecyber.com 
//
//======================================================================

using System;


namespace VoiceCyber.NPOI.SS.Formula.PTG
{
    /**
     * defines a Ptg that is an operation instead of an operand
     * @author  andy
     */
    [Serializable]
    public abstract class OperationPtg : Ptg
    {
        public const int TYPE_UNARY = 0;
        public const int TYPE_BINARY = 1;
        public const int TYPE_FUNCTION = 2;

        /**
         *  returns a string representation of the operations
         *  the Length of the input array should equal the number returned by 
         *  @see #GetNumberOfOperands
         *  
         */
        public abstract String ToFormulaString(String[] operands);

        /**
         * The number of operands expected by the operations
         */
        public abstract int NumberOfOperands { get; }

        public override byte DefaultOperandClass
        {
            get { return Ptg.CLASS_VALUE; }
        }
    }
}
