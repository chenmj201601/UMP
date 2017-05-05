﻿//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    998f315e-44f3-4c5b-884a-f117147f7a3d
//        CLR Version:              4.0.30319.42000
//        Name:                     ArrayPtg
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.SS.Formula.PTG
//        File Name:                ArrayPtg
//
//        Created by Charley at 2016/9/30 16:08:28
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Text;
using VoiceCyber.NPOI.Util;
using VoiceCyber.NPOI.SS.Formula.Constant;
using VoiceCyber.NPOI.SS.Util;


namespace VoiceCyber.NPOI.SS.Formula.PTG
{
    /**
     * ArrayPtg - handles arrays
     * 
     * The ArrayPtg is a little weird, the size of the Ptg when parsing initially only
     * includes the Ptg sid and the reserved bytes. The next Ptg in the expression then follows.
     * It is only after the "size" of all the Ptgs is met, that the ArrayPtg data is actually
     * held after this. So Ptg.CreateParsedExpression keeps track of the number of 
     * ArrayPtg elements and need to Parse the data upto the FORMULA record size.
     *  
     * @author Jason Height (jheight at chariot dot net dot au)
     */
    public class ArrayPtg : Ptg
    {
        public const byte sid = 0x20;

        private const int RESERVED_FIELD_LEN = 7;
        /** 
 * The size of the plain tArray token written within the standard formula tokens
 * (not including the data which comes after all formula tokens)
 */
        public const int PLAIN_TOKEN_SIZE = 1 + RESERVED_FIELD_LEN;

        //private static byte[] DEFAULT_RESERVED_DATA = new byte[RESERVED_FIELD_LEN];

        // 7 bytes of data (stored as an int, short and byte here)
        private int _reserved0Int;
        private int _reserved1Short;
        private int _reserved2Byte;

        // data from these fields comes after the Ptg data of all tokens in current formula
        private int _nColumns;
        private int _nRows;
        private Object[] _arrayValues;

        ArrayPtg(int reserved0, int reserved1, int reserved2, int nColumns, int nRows, Object[] arrayValues)
        {
            _reserved0Int = reserved0;
            _reserved1Short = reserved1;
            _reserved2Byte = reserved2;
            _nColumns = nColumns;
            _nRows = nRows;
            _arrayValues = arrayValues;
        }
        /**
         * @param values2d array values arranged in rows
         */
        public ArrayPtg(Object[][] values2d)
        {
            int nColumns = values2d[0].Length;
            int nRows = values2d.Length;
            // convert 2-d to 1-d array (row by row according to getValueIndex())
            _nColumns = (short)nColumns;
            _nRows = (short)nRows;

            Object[] vv = new Object[_nColumns * _nRows];
            for (int r = 0; r < nRows; r++)
            {
                Object[] rowData = values2d[r];
                for (int c = 0; c < nColumns; c++)
                {
                    vv[GetValueIndex(c, r)] = rowData[c];
                }
            }

            _arrayValues = vv;
            _reserved0Int = 0;
            _reserved1Short = 0;
            _reserved2Byte = 0;
        }
        public Object[][] GetTokenArrayValues()
        {
            if (_arrayValues == null)
            {
                throw new InvalidOperationException("array values not read yet");
            }
            Object[][] result = new Object[_nRows][];
            for (int r = 0; r < _nRows; r++)
            {
                result[r] = new object[_nColumns];
                for (int c = 0; c < _nColumns; c++)
                {
                    result[r][c] = _arrayValues[GetValueIndex(c, r)];
                }
            }
            return result;

        }

        public override bool IsBaseToken
        {
            get { return false; }
        }

        public override String ToString()
        {
            StringBuilder buffer = new StringBuilder("[ArrayPtg]\n");

            buffer.Append("columns = ").Append(ColumnCount).Append("\n");
            buffer.Append("rows = ").Append(RowCount).Append("\n");
            for (int x = 0; x < ColumnCount; x++)
            {
                for (int y = 0; y < RowCount; y++)
                {
                    Object o = _arrayValues.GetValue(GetValueIndex(x, y));
                    buffer.Append("[").Append(x).Append("][").Append(y).Append("] = ").Append(o).Append("\n");
                }
            }
            return buffer.ToString();
        }

        /**
         * Note - (2D) array elements are stored column by column 
         * @return the index into the internal 1D array for the specified column and row
         */
        /* package */
        public int GetValueIndex(int colIx, int rowIx)
        {
            if (colIx < 0 || colIx >= _nColumns)
            {
                throw new ArgumentException("Specified colIx (" + colIx
                        + ") is outside the allowed range (0.." + (_nColumns - 1) + ")");
            }
            if (rowIx < 0 || rowIx >= _nRows)
            {
                throw new ArgumentException("Specified rowIx (" + rowIx
                        + ") is outside the allowed range (0.." + (_nRows - 1) + ")");
            }
            return rowIx * _nColumns + colIx;
        }

        public override void Write(ILittleEndianOutput out1)
        {
            out1.WriteByte(sid + PtgClass);
            out1.WriteInt(_reserved0Int);
            out1.WriteShort(_reserved1Short);
            out1.WriteByte(_reserved2Byte);
        }

        public int WriteTokenValueBytes(ILittleEndianOutput out1)
        {

            out1.WriteByte(_nColumns - 1);
            out1.WriteShort(_nRows - 1);
            ConstantValueParser.Encode(out1, _arrayValues);
            return 3 + ConstantValueParser.GetEncodedSize(_arrayValues);
        }

        public int RowCount
        {
            get
            {
                return _nRows;
            }
        }

        public int ColumnCount
        {
            get
            {
                return _nColumns;
            }
        }

        /** This size includes the size of the array Ptg plus the Array Ptg Token value size*/
        public override int Size
        {
            get
            {
                int size = 1 + 7 + 1 + 2;
                size += ConstantValueParser.GetEncodedSize(_arrayValues);
                return size;
            }
        }

        public override String ToFormulaString()
        {
            StringBuilder b = new StringBuilder();
            b.Append("{");
            for (int y = 0; y < RowCount; y++)
            {
                if (y > 0)
                {
                    b.Append(";");
                }
                for (int x = 0; x < ColumnCount; x++)
                {
                    if (x > 0)
                    {
                        b.Append(",");
                    }
                    Object o = _arrayValues.GetValue(GetValueIndex(x, y));
                    b.Append(GetConstantText(o));
                }
            }
            b.Append("}");
            return b.ToString();
        }

        private static String GetConstantText(Object o)
        {

            if (o == null)
            {
                return ""; // TODO - how is 'empty value' represented in formulas?
            }
            if (o is String)
            {
                return "\"" + (String)o + "\"";
            }
            if (o is Double)
            {
                return NumberToTextConverter.ToText((Double)o);
            }
            if (o is bool)
            {
                return ((bool)o).ToString().ToUpper();
            }
            if (o is ErrorConstant)
            {
                return ((ErrorConstant)o).Text;
            }
            throw new ArgumentException("Unexpected constant class (" + o.GetType().Name + ")");
        }

        public override byte DefaultOperandClass
        {
            get { return CLASS_ARRAY; }
        }


        /**
 * Represents the initial plain tArray token (without the constant data that trails the whole
 * formula).  Objects of this class are only temporary and cannot be used as {@link Ptg}s.
 * These temporary objects get converted to {@link ArrayPtg} by the
 * {@link #finishReading(LittleEndianInput)} method.
 */
        public class Initial : Ptg
        {
            private int _reserved0;
            private int _reserved1;
            private int _reserved2;

            public Initial(ILittleEndianInput in1)
            {
                _reserved0 = in1.ReadInt();
                _reserved1 = in1.ReadUShort();
                _reserved2 = in1.ReadUByte();
            }
            private static Exception Invalid()
            {
                throw new InvalidOperationException("This object is a partially initialised tArray, and cannot be used as a Ptg");
            }
            public override byte DefaultOperandClass
            {
                get
                {
                    throw Invalid();
                }
            }
            public override int Size
            {
                get
                {
                    return PLAIN_TOKEN_SIZE;
                }
            }
            public override bool IsBaseToken
            {
                get
                {
                    return false;
                }
            }
            public override String ToFormulaString()
            {
                throw Invalid();
            }
            public override void Write(ILittleEndianOutput out1)
            {
                throw Invalid();
            }
            /**
             * Read in the actual token (array) values. This occurs
             * AFTER the last Ptg in the expression.
             * See page 304-305 of Excel97-2007BinaryFileFormat(xls)Specification.pdf
             */
            public ArrayPtg FinishReading(ILittleEndianInput in1)
            {
                int nColumns = in1.ReadUByte();
                short nRows = in1.ReadShort();
                //The token_1_columns and token_2_rows do not follow the documentation.
                //The number of physical rows and columns is actually +1 of these values.
                //Which is not explicitly documented.
                nColumns++;
                nRows++;

                int totalCount = nRows * nColumns;
                Object[] arrayValues = ConstantValueParser.Parse(in1, totalCount);

                ArrayPtg result = new ArrayPtg(_reserved0, _reserved1, _reserved2, nColumns, nRows, arrayValues);
                result.PtgClass = PtgClass;
                return result;
            }
        }
    }
}
