using System;
using System.Diagnostics;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace ExcelHelper
{
    public class CExcelHelper
    {
        //创建文件路径 filePath
        //创建文件头 handlObjects
        ////设置列宽 int[] columnWidth
        public bool CreateExcel(string filePath, object[,] handlObjects, int[] columnWidth)
        {
            IWorkbook wb;
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
                Debug.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        public void ReadExcelFile(string filePath)
        {
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
                for (int i = 0; i <= sheet.LastRowNum; i++)
                {
                    row = sheet.GetRow(i);
                    if (row != null)
                    {
                        for (int j = 0; j < row.LastCellNum; j++)
                        {
                            string value = string.Empty;
                            if (GetCellValue(row.GetCell(j)) != null)
                            {
                                value = row.GetCell(j).ToString();
                            }
                            Console.Write(value.ToString() + " ");
                        }
                        Console.WriteLine("\n");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
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
    }
}
