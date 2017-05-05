//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    73108f22-03b3-4019-86ae-d09e4aefae51
//        CLR Version:              4.0.30319.42000
//        Name:                     FreeRefFunction
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.SS.Formula.Function
//        File Name:                FreeRefFunction
//
//        Created by Charley at 2016/9/30 15:58:30
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoiceCyber.NPOI.SS.Formula.Eval;


namespace VoiceCyber.NPOI.SS.Formula.Function
{
    /**
      * For most Excel functions, involving references ((cell, area), (2d, 3d)), the references are 
      * passed in as arguments, and the exact location remains fixed.  However, a select few Excel
      * functions have the ability to access cells that were not part of any reference passed as an
      * argument.<br/>
      * Two important functions with this feature are <b>INDIRECT</b> and <b>OFFSet</b><p/>
      *  
      * In POI, the <c>HSSFFormulaEvaluator</c> Evaluates every cell in each reference argument before
      * calling the function.  This means that functions using fixed references do not need access to
      * the rest of the workbook to execute.  Hence the <c>Evaluate()</c> method on the common
      * interface <c>Function</c> does not take a workbook parameter.  
      * 
      * This interface recognises the requirement of some functions to freely Create and Evaluate 
      * references beyond those passed in as arguments.
      * 
      * @author Josh Micich
      */
    public interface FreeRefFunction
    {
        /**
         * @param args the pre-Evaluated arguments for this function. args is never <code>null</code>,
         *             nor are any of its elements.
         * @param ec primarily used to identify the source cell Containing the formula being Evaluated.
         *             may also be used to dynamically create reference evals.
         * @return never <code>null</code>. Possibly an instance of <c>ErrorEval</c> in the case of
         * a specified Excel error (Exceptions are never thrown to represent Excel errors).
         */
        ValueEval Evaluate(ValueEval[] args, OperationEvaluationContext ec);
    }
}
