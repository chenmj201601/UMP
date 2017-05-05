//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    696a42b6-342f-44b2-ac69-607618db58e3
//        CLR Version:              4.0.30319.42000
//        Name:                     UserDefinedFunction
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.SS.Formula
//        File Name:                UserDefinedFunction
//
//        Created by Charley at 2016/9/30 16:00:18
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using VoiceCyber.NPOI.SS.Formula.Eval;
using VoiceCyber.NPOI.SS.Formula.Function;


namespace VoiceCyber.NPOI.SS.Formula
{
    public class UserDefinedFunction : FreeRefFunction
    {

        public static FreeRefFunction instance = new UserDefinedFunction();

        private UserDefinedFunction()
        {
            // enforce Singleton
        }

        public ValueEval Evaluate(ValueEval[] args, OperationEvaluationContext ec)
        {
            int nIncomingArgs = args.Length;
            if (nIncomingArgs < 1)
            {
                throw new Exception("function name argument missing");
            }

            ValueEval nameArg = args[0];
            String functionName;
            if (nameArg is FunctionNameEval)
            {
                functionName = ((FunctionNameEval)nameArg).FunctionName;
            }
            else
            {
                throw new Exception("First argument should be a NameEval, but got ("
                        + nameArg.GetType().Name + ")");
            }
            FreeRefFunction targetFunc = ec.FindUserDefinedFunction(functionName);
            if (targetFunc == null)
            {
                throw new NotImplementedFunctionException(functionName);
            }
            int nOutGoingArgs = nIncomingArgs - 1;
            ValueEval[] outGoingArgs = new ValueEval[nOutGoingArgs];
            Array.Copy(args, 1, outGoingArgs, 0, nOutGoingArgs);
            return targetFunc.Evaluate(outGoingArgs, ec);
        }
    }
}
