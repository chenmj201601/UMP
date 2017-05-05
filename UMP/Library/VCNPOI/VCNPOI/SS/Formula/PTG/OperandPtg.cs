//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    64dd5432-4595-4d68-be99-4ca2a8f5876c
//        CLR Version:              4.0.30319.42000
//        Name:                     OperandPtg
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.SS.Formula.PTG
//        File Name:                OperandPtg
//
//        Created by Charley at 2016/9/30 16:41:59
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using VoiceCyber.NPOI.Util;


namespace VoiceCyber.NPOI.SS.Formula.PTG
{
    /**
     * @author Josh Micich
     */
    [Serializable]
    public abstract class OperandPtg : Ptg
    {

        /**
         * All Operand <c>Ptg</c>s are classifed ('relative', 'value', 'array')  
         */
        public override bool IsBaseToken
        {
            get { return false; }
        }
        public OperandPtg Copy()
        {
            try
            {
                return (OperandPtg)Clone();
            }
            catch (NotSupportedException e)
            {
                throw new RuntimeException(e);
            }
        }
    }
}
