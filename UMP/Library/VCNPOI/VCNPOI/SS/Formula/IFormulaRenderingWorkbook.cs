//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    56291eb9-c047-4771-a8ef-03d828644c40
//        CLR Version:              4.0.30319.42000
//        Name:                     IFormulaRenderingWorkbook
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.SS.Formula
//        File Name:                IFormulaRenderingWorkbook
//
//        Created by Charley at 2016/9/30 16:42:55
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using VoiceCyber.NPOI.SS.Formula.PTG;


namespace VoiceCyber.NPOI.SS.Formula
{
    /**
      * Abstracts a workbook for the purpose of converting formula To text.<br/>
      * 
      * For POI internal use only
      * 
      * @author Josh Micich
      */
    public interface IFormulaRenderingWorkbook
    {

        /**
         * @return <c>null</c> if externSheetIndex refers To a sheet inside the current workbook
         */
        ExternalSheet GetExternalSheet(int externSheetIndex);
        //String GetSheetNameByExternSheet(int externSheetIndex);
        /**
         * @return the name of the (first) sheet referred to by the given external sheet index
         */
        String GetSheetFirstNameByExternSheet(int externSheetIndex);
        /**
         * @return the name of the (last) sheet referred to by the given external sheet index
         */
        String GetSheetLastNameByExternSheet(int externSheetIndex);
        String ResolveNameXText(NameXPtg nameXPtg);
        String GetNameText(NamePtg namePtg);
    }
}
