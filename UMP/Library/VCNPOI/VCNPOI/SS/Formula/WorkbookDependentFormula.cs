//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    2b1af3e7-b6e2-439d-9751-4e1f0193d374
//        CLR Version:              4.0.30319.42000
//        Name:                     WorkbookDependentFormula
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.SS.Formula
//        File Name:                WorkbookDependentFormula
//
//        Created by Charley at 2016/9/30 16:42:36
//        http://www.voicecyber.com 
//
//======================================================================

using System;


namespace VoiceCyber.NPOI.SS.Formula
{
    /**
     * Should be implemented by any {@link Ptg} subclass that needs a workbook To render its formula.
     * <br/>
     * 
     * For POI internal use only
     * 
     * @author Josh Micich
     */
    public interface WorkbookDependentFormula
    {
        String ToFormulaString(IFormulaRenderingWorkbook book);
    }
}
