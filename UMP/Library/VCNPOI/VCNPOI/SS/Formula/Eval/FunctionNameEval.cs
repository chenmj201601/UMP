//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    c200f6ed-43b2-4851-99a3-7ce1c345e543
//        CLR Version:              4.0.30319.42000
//        Name:                     FunctionNameEval
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.SS.Formula.Eval
//        File Name:                FunctionNameEval
//
//        Created by Charley at 2016/9/30 16:01:02
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Text;


namespace VoiceCyber.NPOI.SS.Formula.Eval
{
    /**
     * @author Josh Micich
     */
    public class FunctionNameEval : ValueEval
    {

        private String _functionName;

        /**
         * Creates a NameEval representing a function name
         */
        public FunctionNameEval(String functionName)
        {
            _functionName = functionName;
        }

        public String FunctionName
        {
            get
            {
                return _functionName;
            }
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder(64);
            sb.Append(GetType().Name).Append(" [");
            sb.Append(_functionName);
            sb.Append("]");
            return sb.ToString();
        }
    }
}
