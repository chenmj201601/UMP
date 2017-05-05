//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    90d611d2-bece-4ce8-99df-003d3d49e9a2
//        CLR Version:              4.0.30319.42000
//        Name:                     ErrorEval
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.SS.Formula.Eval
//        File Name:                ErrorEval
//
//        Created by Charley at 2016/9/30 15:47:47
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Text;
using VoiceCyber.NPOI.SS.UserModel;
using VoiceCyber.NPOI.HSSF.UserModel;
using VoiceCyber.NPOI.Util;


namespace VoiceCyber.NPOI.SS.Formula.Eval
{
    /**
     * @author Amol S. Deshmukh &lt; amolweb at ya hoo dot com &gt;
     *
     */
    public class ErrorEval : ValueEval
    {
        private static Dictionary<FormulaError, ErrorEval> evals = new Dictionary<FormulaError, ErrorEval>();
        // convenient access to namespace
        private const HSSFErrorConstants EC = null;

        /** <b>#NULL!</b>  - Intersection of two cell ranges is empty */
        public static readonly ErrorEval NULL_INTERSECTION = new ErrorEval(FormulaError.NULL);
        /** <b>#DIV/0!</b> - Division by zero */
        public static readonly ErrorEval DIV_ZERO = new ErrorEval(FormulaError.DIV0);
        /** <b>#VALUE!</b> - Wrong type of operand */
        public static readonly ErrorEval VALUE_INVALID = new ErrorEval(FormulaError.VALUE);
        /** <b>#REF!</b> - Illegal or deleted cell reference */
        public static readonly ErrorEval REF_INVALID = new ErrorEval(FormulaError.REF);
        /** <b>#NAME?</b> - Wrong function or range name */
        public static readonly ErrorEval NAME_INVALID = new ErrorEval(FormulaError.NAME);
        /** <b>#NUM!</b> - Value range overflow */
        public static readonly ErrorEval NUM_ERROR = new ErrorEval(FormulaError.NUM);
        /** <b>#N/A</b> - Argument or function not available */
        public static readonly ErrorEval NA = new ErrorEval(FormulaError.NA);


        // POI internal error codes
        public static ErrorEval FUNCTION_NOT_IMPLEMENTED = new ErrorEval(FormulaError.FUNCTION_NOT_IMPLEMENTED);

        // Note - Excel does not seem to represent this condition with an error code
        public static ErrorEval CIRCULAR_REF_ERROR = new ErrorEval(FormulaError.CIRCULAR_REF);


        /**
         * Translates an Excel internal error code into the corresponding POI ErrorEval instance
         * @param errorCode
         */
        public static ErrorEval ValueOf(int errorCode)
        {
            FormulaError error = FormulaError.ForInt(errorCode);
            if (evals.ContainsKey(error))
                return evals[error];

            throw new RuntimeException("Unhandled error type  for code " + errorCode);
        }

        /**
         * Converts error codes to text.  Handles non-standard error codes OK.  
         * For debug/test purposes (and for formatting error messages).
         * @return the String representation of the specified Excel error code.
         */
        public static String GetText(int errorCode)
        {
            if (FormulaError.IsValidCode(errorCode))
            {
                return FormulaError.ForInt(errorCode).String;
            }
            // Give a special string, based on ~, to make clear this isn't a standard Excel error
            return "~non~std~err(" + errorCode + ")~";
        }

        private FormulaError _error;

        private ErrorEval(FormulaError error)
        {
            _error = error;
            if (!evals.ContainsKey(error))
                evals.Add(error, this);
        }
        public int ErrorCode
        {
            get { return _error.LongCode; }
        }
        public String ErrorString
        {
            get
            {
                return _error.String;
            }
        }
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder(64);
            sb.Append(GetType().Name).Append(" [");
            sb.Append(_error.String);
            sb.Append("]");
            return sb.ToString();
        }
    }
}
