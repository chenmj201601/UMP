//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    711d7197-1017-40a1-a7db-93234bc75fac
//        CLR Version:              4.0.30319.42000
//        Name:                     IEvaluationCell
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.SS.Formula
//        File Name:                IEvaluationCell
//
//        Created by Charley at 2016/9/30 16:04:39
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using VoiceCyber.NPOI.SS.UserModel;


namespace VoiceCyber.NPOI.SS.Formula
{
    /**
      * Abstracts a cell for the purpose of formula evaluation.  This interface represents both formula
      * and non-formula cells.<br/>
      * 
      * Implementors of this class must implement {@link #HashCode()} and {@link #Equals(Object)}
      * To provide an <em>identity</em> relationship based on the underlying HSSF or XSSF cell <p/>
      * 
      * For POI internal use only
      * 
      * @author Josh Micich
      */
    public interface IEvaluationCell
    {
        // consider method Object GetUnderlyingCell() To reduce memory consumption in formula cell cache
        IEvaluationSheet Sheet { get; }
        int RowIndex { get; }
        int ColumnIndex { get; }
        CellType CellType { get; }

        double NumericCellValue { get; }
        String StringCellValue { get; }
        bool BooleanCellValue { get; }
        int ErrorCellValue { get; }
        Object IdentityKey { get; }
        CellType CachedFormulaResultType { get; }
    }
}
