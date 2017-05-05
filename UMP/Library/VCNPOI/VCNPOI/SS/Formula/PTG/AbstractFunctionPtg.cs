//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    c0b0e1c5-11c7-43ed-bac2-c441520b5477
//        CLR Version:              4.0.30319.42000
//        Name:                     AbstractFunctionPtg
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.SS.Formula.PTG
//        File Name:                AbstractFunctionPtg
//
//        Created by Charley at 2016/9/30 16:32:24
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Text;
using VoiceCyber.NPOI.SS.Formula.Function;


namespace VoiceCyber.NPOI.SS.Formula.PTG
{
    /**
      * This class provides the base functionality for Excel sheet functions
      * There are two kinds of function Ptgs - tFunc and tFuncVar
      * Therefore, this class will have ONLY two subclasses
      * @author  Avik Sengupta
      * @author Andrew C. Oliver (acoliver at apache dot org)
      */
    [Serializable]
    public abstract class AbstractFunctionPtg : OperationPtg
    {

        /**
         * The name of the IF function (i.e. "IF").  Extracted as a constant for clarity.
         */
        public const string FUNCTION_NAME_IF = "IF";
        /** All external functions have function index 255 */
        private const short FUNCTION_INDEX_EXTERNAL = 255;

        protected byte returnClass;
        protected byte[] paramClass;

        protected byte _numberOfArgs;
        protected short _functionIndex;

        protected AbstractFunctionPtg(int functionIndex, int pReturnClass, byte[] paramTypes, int nParams)
        {
            _numberOfArgs = (byte)nParams;
            _functionIndex = (short)functionIndex;
            returnClass = (byte)pReturnClass;
            paramClass = paramTypes;
        }

        public override bool IsBaseToken
        {
            get { return false; }
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder(64);
            sb.Append(GetType().Name).Append(" [");
            sb.Append(LookupName(_functionIndex));
            sb.Append(" nArgs=").Append(_numberOfArgs);
            sb.Append("]");
            return sb.ToString();
        }

        public short FunctionIndex
        {
            get
            {
                return _functionIndex;
            }
        }
        public override int NumberOfOperands
        {
            get
            {
                return _numberOfArgs;
            }
        }

        public String Name
        {
            get { return LookupName(_functionIndex); }
        }
        /**
         * external functions Get some special Processing
         * @return <c>true</c> if this is an external function
         */
        public bool IsExternalFunction
        {
            get { return _functionIndex == FUNCTION_INDEX_EXTERNAL; }
        }

        public override String ToFormulaString()
        {
            return Name;
        }

        public override String ToFormulaString(String[] operands)
        {
            StringBuilder buf = new StringBuilder();

            if (IsExternalFunction)
            {
                buf.Append(operands[0]); // first operand is actually the function name
                AppendArgs(buf, 1, operands);
            }
            else
            {
                buf.Append(Name);
                AppendArgs(buf, 0, operands);
            }
            return buf.ToString();
        }

        private static void AppendArgs(StringBuilder buf, int firstArgIx, String[] operands)
        {
            buf.Append('(');
            for (int i = firstArgIx; i < operands.Length; i++)
            {
                if (i > firstArgIx)
                {
                    buf.Append(',');
                }
                buf.Append(operands[i]);
            }
            buf.Append(")");
        }

        /**
         * Used to detect whether a function name found in a formula is one of the standard excel functions
         * 
         * The name matching is case insensitive.
         * @return <c>true</c> if the name specifies a standard worksheet function,
         *  <c>false</c> if the name should be assumed to be an external function.
         */
        public static bool IsBuiltInFunctionName(String name)
        {
            short ix = FunctionMetadataRegistry.LookupIndexByName(name.ToUpper());
            return ix >= 0;
        }
        protected String LookupName(short index)
        {
            if (index == FunctionMetadataRegistry.FUNCTION_INDEX_EXTERNAL)
            {
                return "#external#";
            }
            FunctionMetadata fm = FunctionMetadataRegistry.GetFunctionByIndex(index);
            if (fm == null)
            {
                throw new Exception("bad function index (" + index + ")");
            }
            return fm.Name;
        }

        /**
         * Resolves internal function names into function indexes.
         * 
         * The name matching is case insensitive.
         * @return the standard worksheet function index if found, otherwise <c>FUNCTION_INDEX_EXTERNAL</c>
         */
        protected static short LookupIndex(String name)
        {
            short ix = FunctionMetadataRegistry.LookupIndexByName(name.ToUpper());
            if (ix < 0)
            {
                return FUNCTION_INDEX_EXTERNAL;
            }
            return ix;
        }

        public override byte DefaultOperandClass
        {
            get { return returnClass; }
        }

        public byte GetParameterClass(int index)
        {
            if (index >= paramClass.Length)
            {
                // For var-arg (and other?) functions, the metadata does not list all the parameter
                // operand classes.  In these cases, all extra parameters are assumed to have the 
                // same operand class as the last one specified.
                return paramClass[paramClass.Length - 1];
            }
            return paramClass[index];
        }
    }
}
