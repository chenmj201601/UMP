//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    8a7395de-4feb-426c-8cb8-7b429268e671
//        CLR Version:              4.0.30319.42000
//        Name:                     IEvaluationSheet
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.SS.Formula
//        File Name:                IEvaluationSheet
//
//        Created by Charley at 2016/9/30 16:04:19
//        http://www.voicecyber.com 
//
//======================================================================


namespace VoiceCyber.NPOI.SS.Formula
{
    /**
      * Abstracts a sheet for the purpose of formula evaluation.<br/>
      * 
      * For POI internal use only
      * 
      * @author Josh Micich
      */
    public interface IEvaluationSheet
    {

        /**
         * @return <c>null</c> if there is no cell at the specified coordinates
         */
        IEvaluationCell GetCell(int rowIndex, int columnIndex);
    }
}
