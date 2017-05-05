//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    8a2ebc8b-fef2-4bf1-8ca3-2181da19e74e
//        CLR Version:              4.0.30319.42000
//        Name:                     NotImplementedFunctionException
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.SS.Formula.Eval
//        File Name:                NotImplementedFunctionException
//
//        Created by Charley at 2016/9/30 16:01:54
//        http://www.voicecyber.com 
//
//======================================================================

using System;


namespace VoiceCyber.NPOI.SS.Formula.Eval
{
    /**
      * An exception thrown by implementors of {@link FormulaEvaluator} when 
      *  attempting to evaluate a formula which requires a function that POI 
      *  does not (yet) support.
      */
    public class NotImplementedFunctionException : NotImplementedException
    {
        private String functionName;

        public NotImplementedFunctionException(string functionName)
            : base(functionName)
        {
            this.functionName = functionName;
        }
        public NotImplementedFunctionException(string functionName, NotImplementedException cause)
            : base(functionName, cause)
        {
            this.functionName = functionName;
        }

        public String FunctionName
        {
            get
            {
                return functionName;
            }
        }
    }
}
