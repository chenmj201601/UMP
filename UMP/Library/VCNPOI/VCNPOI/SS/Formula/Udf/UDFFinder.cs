//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    4660aa24-9e84-4e0b-a03f-a14b07e8dd4b
//        CLR Version:              4.0.30319.42000
//        Name:                     UDFFinder
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.SS.Formula.Udf
//        File Name:                UDFFinder
//
//        Created by Charley at 2016/9/30 15:56:50
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace VoiceCyber.NPOI.SS.Formula.Udf
{

    /**
     * Common interface for "Add-in" libraries and user defined function libraries.
     *
     * @author PUdalau
     */
    public abstract class UDFFinder
    {
        public static readonly UDFFinder DEFAULT = new AggregatingUDFFinder(AnalysisToolPak.instance);

        /**
         * Returns executor by specified name. Returns <code>null</code> if the function name is unknown.
         *
         * @param name Name of function.
         * @return Function executor.
         */
        public abstract FreeRefFunction FindFunction(String name);
    }
}
