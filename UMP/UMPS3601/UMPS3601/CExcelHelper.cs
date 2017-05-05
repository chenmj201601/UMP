using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common3601;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace UMPS3601
{
    class CExcelHelper
    {
        private List<string[]> _mLstStr;
        private string[] _mString = new string[32];

        public CExcelHelper()
        {
            _mString = new string[32];
            _mLstStr = new List<string[]>();
        }

        //创建文件路径 filePath
        //创建文件头 handlObjects
        ////设置列宽 int[] columnWidth
        public bool CreateExcel(string filePath, object[,] handlObjects, int[] columnWidth, out string error)
        {
            IWorkbook wb;
            error = string.Empty;
            string extension = Path.GetExtension(filePath);
            if (extension != null && extension.Equals(".xls"))
            {
                wb = new HSSFWorkbook();
            }
            else
            {
                wb = new XSSFWorkbook();
            }
            ICellStyle style1 = wb.CreateCellStyle();
            style1.Alignment = HorizontalAlignment.Left;
            style1.VerticalAlignment = VerticalAlignment.Center;
            style1.BorderBottom = BorderStyle.Thin;
            style1.BorderLeft = BorderStyle.Thin;
            style1.BorderRight = BorderStyle.Thin;
            style1.BorderTop = BorderStyle.Thin;
            style1.WrapText = true;
            ICellStyle style2 = wb.CreateCellStyle();
            IFont font1 = wb.CreateFont();
            font1.FontName = "Arial Black";
            font1.FontHeightInPoints = 12;
            font1.Color = HSSFColor.Red.Index;
            font1.Boldweight = (short)FontBoldWeight.Normal;
            style2.SetFont(font1);

            ISheet sheet = wb.CreateSheet("Sheet0");
            int rowCount = handlObjects.Length / columnWidth.Length, columnCount = columnWidth.Length;

            for (int i = 0; i < columnWidth.Length; i++)
            {
                sheet.SetColumnWidth(i, 256 * columnWidth[i]);
            }

            IRow row;
            ICell cell;
            for (int i = 0; i < rowCount; i++)
            {
                row = sheet.CreateRow(i);
                for (int j = 0; j < columnCount; j++)
                {
                    cell = row.CreateCell(j);
                         cell.CellStyle = i != 0 ? style1 : style2;
                    cell.SetCellValue(handlObjects[i, j].ToString());
                }
            }

            //CellRangeAddress(0, 2, 0, 0)，合并0-2行，0-0列的单元格
            CellRangeAddress region = new CellRangeAddress(0, 0, 0, columnWidth.Length - 1);
            sheet.AddMergedRegion(region);

            try
            {
                FileStream fs = File.OpenWrite(filePath);
                wb.Write(fs); 
                fs.Close();
            }
            catch (Exception e)
            {
                error = e.ToString();
                return false;
            }
            return true;
        }

        public bool ReadExcelFile(string filePath, out string error)
        {
            error = string.Empty;
            IWorkbook wk = null;
            string extension = System.IO.Path.GetExtension(filePath);
            try
            {
                FileStream fs = File.OpenRead(filePath);
                if (extension != null && extension.Equals(".xls"))
                    wk = new HSSFWorkbook(fs);
                else
                    wk = new XSSFWorkbook(fs);

                fs.Close();
                ISheet sheet = wk.GetSheetAt(0);

                IRow row = sheet.GetRow(0);
                int offset = 0;
                _mLstStr = new List<string[]>();
                for (int i = 0; i <= sheet.LastRowNum; i++)
                {
                    row = sheet.GetRow(i);
                    if (row != null)
                    {
                        _mString = new string[12];
                        for (int j = 0; j < row.LastCellNum; j++)
                        {
                            string value = string.Empty;
                            if (GetCellValue(row.GetCell(j)) != null)
                            {
                                value = row.GetCell(j).ToString();
                            }
                            _mString[j] = value.ToString();
                        }
                        _mLstStr.Add(_mString);
                    }
                }
            }
            catch (Exception e)
            {
                error = e.Message;
                return false;
            }
            return true;
        }

        public bool SetExcelInfo(string filePath, out string error, List<CQuestionsParam> _mLstExcelInfoErr)
        {
            error = string.Empty;
            IWorkbook wk = null;
            string extension = System.IO.Path.GetExtension(filePath);
            try
            {
                FileStream fs = File.OpenRead(filePath);
                if (extension != null && extension.Equals(".xls"))
                    wk = new HSSFWorkbook(fs);
                else
                    wk = new XSSFWorkbook(fs);

                fs.Close();
                ISheet sheet = wk.GetSheetAt(0);

                IRow row = sheet.GetRow(0);
                int offset = 0;
                
                _mLstStr = new List<string[]>();
                for (int i = 0; i <= sheet.LastRowNum; i++)
                {
                    bool bEnable = true;
                    row = sheet.GetRow(i);
                    if (row != null)
                    {
                        _mString = new string[12];
                        string value = string.Empty;
                        if (GetCellValue(row.GetCell(1)) != null)
                        {
                            value = row.GetCell(1).ToString();
                        }
                        if (!string.IsNullOrEmpty(value) && i >= 2)
                        {
                            foreach (var question in _mLstExcelInfoErr)
                            {
                                if (question.IntExcelNum == Convert.ToInt32(value))
                                {
                                    sheet.GetRow(i).GetCell(0).SetCellValue("N");
                                    bEnable = false;
                                }
                            }
                            if (bEnable)
                            {
                                sheet.GetRow(i).GetCell(0).SetCellValue("Y");
                            }
                        }                       
                    }
                }
                fs = File.OpenWrite(filePath);
                wk.Write(fs);
                fs.Close();
            }
            catch (Exception e)
            {
                error = e.Message;
                return false;
            }
            return true;
        }

        public object GetCellValue(ICell cell)
        {
            object value = null;
            try
            {
                if (cell.CellType != CellType.Blank)
                {
                    switch (cell.CellType)
                    {
                        case CellType.Numeric:
                            if (DateUtil.IsCellDateFormatted(cell))
                                value = cell.DateCellValue;
                            else
                                value = cell.NumericCellValue;
                            break;
                        case CellType.Boolean:
                            value = cell.BooleanCellValue;
                            break;
                        default:
                            value = cell.StringCellValue;
                            break;
                    }
                }
            }
            catch (Exception)
            {
                value = null;
            }

            return value;
        }

        public List<string[]> GetImportInfo()
        {
            return _mLstStr;
        }

        public bool CheckSerialNum()
        {
            List<int> lstNewNum = new List<int>();
            lstNewNum.Add(0);
            int iCount = 0;
            foreach (var str in _mLstStr)
            {
                if (iCount > 1)
                {
                    string strStr = str[1];
                    if (!string.IsNullOrEmpty(strStr))
                    {
                        if (lstNewNum.Any(newNum => Convert.ToInt32(strStr) == newNum))
                        {
                            return false;
                        }
                        lstNewNum.Add(Convert.ToInt32(str[1]));
                    }
                }
                iCount ++;
            }
            return true;
        }
    }
}
