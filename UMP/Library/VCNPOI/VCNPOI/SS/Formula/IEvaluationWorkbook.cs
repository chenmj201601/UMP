//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    9bbbd799-c036-4d1f-a7b0-700a7910832d
//        CLR Version:              4.0.30319.42000
//        Name:                     IEvaluationWorkbook
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.SS.Formula
//        File Name:                IEvaluationWorkbook
//
//        Created by Charley at 2016/9/30 16:03:29
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoiceCyber.NPOI.SS.Formula.Udf;


namespace VoiceCyber.NPOI.SS.Formula
{
    public class ExternalSheet
    {
        private String _workbookName;
        private String _sheetName;

        public ExternalSheet(String workbookName, String sheetName)
        {
            _workbookName = workbookName;
            _sheetName = sheetName;
        }
        public String WorkbookName
        {
            get
            {
                return _workbookName;
            }
        }
        public String SheetName
        {
            get
            {
                return _sheetName;
            }
        }
    }

    public class ExternalSheetRange : ExternalSheet
    {
        private String _lastSheetName;
        public ExternalSheetRange(String workbookName, String firstSheetName, String lastSheetName)
            : base(workbookName, firstSheetName)
        {
            this._lastSheetName = lastSheetName;
        }

        public String FirstSheetName
        {
            get
            {
                return SheetName;
            }
        }
        public String LastSheetName
        {
            get
            {
                return _lastSheetName;
            }
        }
    }

    /**
     * Abstracts a workbook for the purpose of formula evaluation.<br/>
     * 
     * For POI internal use only
     * 
     * @author Josh Micich
     */
    public interface IEvaluationWorkbook
    {
        String GetSheetName(int sheetIndex);
        /**
         * @return -1 if the specified sheet is from a different book
         */
        int GetSheetIndex(IEvaluationSheet sheet);
        int GetSheetIndex(String sheetName);

        IEvaluationSheet GetSheet(int sheetIndex);

        /**
         * HSSF Only - fetch the external-style sheet details
         * <p>Return will have no workbook set if it's actually in our own workbook</p>
         */
        ExternalSheet GetExternalSheet(int externSheetIndex);
        /**
         * XSSF Only - fetch the external-style sheet details
         * <p>Return will have no workbook set if it's actually in our own workbook</p>
         */
        ExternalSheet GetExternalSheet(String firstSheetName, string lastSheetName, int externalWorkbookNumber);
        /**
         * HSSF Only - convert an external sheet index to an internal sheet index,
         *  for an external-style reference to one of this workbook's own sheets 
         */
        int ConvertFromExternSheetIndex(int externSheetIndex);
        /**
         * HSSF Only - fetch the external-style name details
         */
        ExternalName GetExternalName(int externSheetIndex, int externNameIndex);
        /**
         * XSSF Only - fetch the external-style name details
         */
        ExternalName GetExternalName(String nameName, String sheetName, int externalWorkbookNumber);

        IEvaluationName GetName(NamePtg namePtg);
        IEvaluationName GetName(String name, int sheetIndex);
        String ResolveNameXText(NameXPtg ptg);
        Ptg[] GetFormulaTokens(IEvaluationCell cell);
        UDFFinder GetUDFFinder();
    }

    public class ExternalName
    {
        private String _nameName;
        private int _nameNumber;
        private int _ix;

        public ExternalName(String nameName, int nameNumber, int ix)
        {
            _nameName = nameName;
            _nameNumber = nameNumber;
            _ix = ix;
        }
        public String Name
        {
            get
            {
                return _nameName;
            }
        }
        public int Number
        {
            get
            {
                return _nameNumber;
            }
        }
        public int Ix
        {
            get
            {
                return _ix;
            }
        }
    }
}
