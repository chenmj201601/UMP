using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NPOI.HSSF.UserModel;
using System.IO;
using NPOI.SS.UserModel;
using System.Reflection;
using NPOI.HPSF;
using NPOI.SS.Util;
using System.Collections;
using NPOI.HSSF.Util;
using NPOI.SS.Formula.Eval;
using System.Web.UI.WebControls;
using NPOI.XSSF.UserModel;

namespace UMPS1101.Models
{
    public class NPOIHelper
    {
        /// <summary>读取excel
        /// 默认第一行为表头
        /// </summary>
        /// <param name="strFileName">excel文档路径</param>
        /// <returns></returns>
        public static DataTable Import(string strFileName)
        {
            DataTable dt = new DataTable();

            HSSFWorkbook hssfworkbook;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }
            ISheet sheet = hssfworkbook.GetSheetAt(0);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

            //HSSFRow headerRow = sheet.GetRow(0);
            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;

            for (int j = 0; j < cellCount; j++)
            {
                ICell cell = headerRow.GetCell(j);
                dt.Columns.Add(cell.ToString());
            }

            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                DataRow dataRow = dt.NewRow();

                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.GetCell(j) != null)
                        dataRow[j] = row.GetCell(j).ToString();
                }

                dt.Rows.Add(dataRow);
            }
            return dt;
        }

        /// <summary>
        /// 合并单元格
        /// </summary>
        /// <param name="sheet">要合并单元格所在的sheet</param>
        /// <param name="rowstart">开始行的索引</param>
        /// <param name="rowend">结束行的索引</param>
        /// <param name="colstart">开始列的索引</param>
        /// <param name="colend">结束列的索引</param>
        public static void SetCellRangeAddress(ISheet sheet, int rowstart, int rowend, int colstart, int colend)
        {
            CellRangeAddress cellRangeAddress = new CellRangeAddress(rowstart, rowend, colstart, colend);
            sheet.AddMergedRegion(cellRangeAddress);
        }

        #region 从datatable中将数据导出到excel
        /// <summary>
        /// DataTable导出到Excel的MemoryStream
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="strHeaderText">表头文本</param>
        static MemoryStream ExportDT(DataTable dtSource, string strHeaderText)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            HSSFSheet sheet = workbook.CreateSheet() as HSSFSheet;

            #region 右击文件 属性信息

            //{
            //    DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            //    dsi.Company = "http://www.yongfa365.com/";
            //    workbook.DocumentSummaryInformation = dsi;

            //    SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            //    si.Author = "柳永法"; //填加xls文件作者信息
            //    si.ApplicationName = "NPOI测试程序"; //填加xls文件创建程序信息
            //    si.LastAuthor = "柳永法2"; //填加xls文件最后保存者信息
            //    si.Comments = "说明信息"; //填加xls文件作者信息
            //    si.Title = "NPOI测试"; //填加xls文件标题信息
            //    si.Subject = "NPOI测试Demo"; //填加文件主题信息
            //    si.CreateDateTime = DateTime.Now;
            //    workbook.SummaryInformation = si;
            //}

            #endregion

            HSSFCellStyle dateStyle = workbook.CreateCellStyle() as HSSFCellStyle;
            HSSFDataFormat format = workbook.CreateDataFormat() as HSSFDataFormat;
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

            //取得列宽
            int[] arrColWidth = new int[dtSource.Columns.Count];
            foreach (DataColumn item in dtSource.Columns)
            {
                arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
            }
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                for (int j = 0; j < dtSource.Columns.Count; j++)
                {
                    int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
                    if (intTemp > arrColWidth[j])
                    {
                        arrColWidth[j] = intTemp;
                    }
                }
            }
            int rowIndex = 0;

            foreach (DataRow row in dtSource.Rows)
            {
                #region 新建表，填充表头，填充列头，样式

                if (rowIndex == 65535 || rowIndex == 0)
                {
                    if (rowIndex != 0)
                    {
                        sheet = workbook.CreateSheet() as HSSFSheet;
                    }

                    #region 表头及样式

                    {
                        HSSFRow headerRow = sheet.CreateRow(0) as HSSFRow;
                        headerRow.HeightInPoints = 25;
                        headerRow.CreateCell(0).SetCellValue(strHeaderText);

                        HSSFCellStyle headStyle = workbook.CreateCellStyle() as HSSFCellStyle;
                        headStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                        HSSFFont font = workbook.CreateFont() as HSSFFont;
                        font.FontHeightInPoints = 20;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);

                        headerRow.GetCell(0).CellStyle = headStyle;

                        sheet.AddMergedRegion(new Region(0, 0, 0, dtSource.Columns.Count - 1));
                        //headerRow.Dispose();
                    }

                    #endregion


                    #region 列头及样式

                    {
                        HSSFRow headerRow = sheet.CreateRow(1) as HSSFRow;


                        HSSFCellStyle headStyle = workbook.CreateCellStyle() as HSSFCellStyle;
                        headStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                        HSSFFont font = workbook.CreateFont() as HSSFFont;
                        font.FontHeightInPoints = 10;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);


                        foreach (DataColumn column in dtSource.Columns)
                        {
                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                            headerRow.GetCell(column.Ordinal).CellStyle = headStyle;

                            //设置列宽
                            sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);

                        }
                        //headerRow.Dispose();
                    }

                    #endregion

                    rowIndex = 2;
                }

                #endregion

                #region 填充内容

                HSSFRow dataRow = sheet.CreateRow(rowIndex) as HSSFRow;
                foreach (DataColumn column in dtSource.Columns)
                {
                    HSSFCell newCell = dataRow.CreateCell(column.Ordinal) as HSSFCell;

                    string drValue = row[column].ToString();

                    switch (column.DataType.ToString())
                    {
                        case "System.String": //字符串类型
                            newCell.SetCellValue(drValue);
                            break;
                        case "System.DateTime": //日期类型
                            DateTime dateV;
                            DateTime.TryParse(drValue, out dateV);
                            newCell.SetCellValue(dateV);

                            newCell.CellStyle = dateStyle; //格式化显示
                            break;
                        case "System.Boolean": //布尔型
                            bool boolV = false;
                            bool.TryParse(drValue, out boolV);
                            newCell.SetCellValue(boolV);
                            break;
                        case "System.Int16": //整型
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            int intV = 0;
                            int.TryParse(drValue, out intV);
                            newCell.SetCellValue(intV);
                            break;
                        case "System.Decimal": //浮点型
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(drValue, out doubV);
                            newCell.SetCellValue(doubV);
                            break;
                        case "System.DBNull": //空值处理
                            newCell.SetCellValue("");
                            break;
                        default:
                            newCell.SetCellValue("");
                            break;
                    }

                }

                #endregion

                rowIndex++;
            }
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;

                //sheet.Dispose();
                //workbook.Clear();

                return ms;
            }
        }

        /// <summary>
        /// DataTable导出到Excel文件
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="strHeaderText">表头文本</param>
        /// <param name="strFileName">保存位置</param>
        public static void ExportDTtoExcel(DataTable dtSource, string strHeaderText, string strFileName)
        {
            using (MemoryStream ms = ExportDT(dtSource, strHeaderText))
            {
                using (FileStream fs = new FileStream(strFileName, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }
        }
        #endregion

        #region 从excel中将数据导出到datatable
        /// <summary>读取excel
        /// 默认第一行为标头
        /// </summary>
        /// <param name="strFileName">excel文档路径</param>
        /// <returns></returns>
        public static DataTable ImportExceltoDt(string strFileName)
        {
            DataTable dt = new DataTable();
            HSSFWorkbook hssfworkbook;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }
            HSSFSheet sheet = hssfworkbook.GetSheetAt(0) as HSSFSheet;
            dt = ImportDt(sheet, 0, true, null);
            return dt;
        }

        /// <summary>
        /// 读取excel
        /// </summary>
        /// <param name="strFileName">excel文件路径</param>
        /// <param name="sheet">需要导出的sheet</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <returns></returns>
        public static DataTable ImportExceltoDt(string strFileName, string SheetName, int HeaderRowIndex, Dictionary<string, PropertyInfo> dicTitleAndProperty)
        {
            HSSFWorkbook workbook;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                workbook = new HSSFWorkbook(file);
            }
            HSSFSheet sheet = workbook.GetSheet(SheetName) as HSSFSheet;
            DataTable table = new DataTable();
            table = ImportDt(sheet, HeaderRowIndex, true, dicTitleAndProperty);
            //ExcelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;
        }

        /// <summary>
        /// 读取excel
        /// </summary>
        /// <param name="strFileName">excel文件路径</param>
        /// <param name="sheet">需要导出的sheet序号</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <returns></returns>
        public static DataTable ImportExceltoDt(string strFileName, int SheetIndex, int HeaderRowIndex, Dictionary<string, PropertyInfo> dicTitleAndProperty)
        {
            HSSFWorkbook workbook;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                workbook = new HSSFWorkbook(file);
            }
            HSSFSheet sheet = workbook.GetSheetAt(SheetIndex) as HSSFSheet;
            DataTable table = new DataTable();
            table = ImportDt(sheet, HeaderRowIndex, true, dicTitleAndProperty);
            //ExcelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;
        }

        /// <summary>
        /// 读取excel
        /// </summary>
        /// <param name="strFileName">excel文件路径</param>
        /// <param name="sheet">需要导出的sheet</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <returns></returns>
        public static DataTable ImportExceltoDt(string strFileName, string SheetName, int HeaderRowIndex, bool needHeader, Dictionary<string, PropertyInfo> dicTitleAndProperty)
        {
            HSSFWorkbook workbook;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                workbook = new HSSFWorkbook(file);
            }
            HSSFSheet sheet = workbook.GetSheet(SheetName) as HSSFSheet;
            DataTable table = new DataTable();
            table = ImportDt(sheet, HeaderRowIndex, needHeader, dicTitleAndProperty);
            //ExcelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;
        }

        /// <summary>
        /// 读取excel
        /// </summary>
        /// <param name="strFileName">excel文件路径</param>
        /// <param name="sheet">需要导出的sheet序号</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <returns></returns>
        public static DataTable ImportExceltoDt(string strFileName, int SheetIndex, int HeaderRowIndex, bool needHeader, Dictionary<string, PropertyInfo> dicTitleAndProperty)
        {
            HSSFWorkbook workbook;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                workbook = new HSSFWorkbook(file);
            }
            HSSFSheet sheet = workbook.GetSheetAt(SheetIndex) as HSSFSheet;
            DataTable table = new DataTable();
            table = ImportDt(sheet, HeaderRowIndex, needHeader, dicTitleAndProperty);
            //ExcelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;
        }

        /// <summary>
        /// 将制定sheet中的数据导出到datatable中
        /// </summary>
        /// <param name="sheet">需要导出的sheet</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <returns></returns>
        static DataTable ImportDt(HSSFSheet sheet, int HeaderRowIndex, bool needHeader, Dictionary<string, PropertyInfo> dicTitleAndProperty)
        {
            DataTable table = new DataTable();
            HSSFRow headerRow;
            int cellCount;
            int columnCount = 0;
            try
            {
                if (HeaderRowIndex < 0 || !needHeader)
                {
                    headerRow = sheet.GetRow(0) as HSSFRow;
                    cellCount = headerRow.LastCellNum;

                    for (int i = headerRow.FirstCellNum; i <= cellCount; i++)
                    {
                        DataColumn column = new DataColumn(Convert.ToString(i));
                        table.Columns.Add(column); 
                        columnCount++;
                    }
                }
                else
                {
                    headerRow = sheet.GetRow(HeaderRowIndex) as HSSFRow;
                    cellCount = headerRow.LastCellNum;

                    for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                    {
                        if (i == 118)
                        {
                            i = i;
                        }
                        if (headerRow.GetCell(i) == null)
                        {
                            if (table.Columns.IndexOf(Convert.ToString(i)) > 0)
                            {
                                DataColumn column = new DataColumn(Convert.ToString("重复列名" + i));
                                table.Columns.Add(column); 
                                columnCount++;
                            }
                            else
                            {
                                DataColumn column = new DataColumn(Convert.ToString(i));
                                table.Columns.Add(column);
                                columnCount++;
                            }

                        }
                        else
                        {
                            string columnName = headerRow.GetCell(i).ToString();
                            Type columnType = typeof(System.String);
                            if (dicTitleAndProperty != null && dicTitleAndProperty.Count > 0 && dicTitleAndProperty.ContainsKey(headerRow.GetCell(i).ToString()))
                            {
                                PropertyInfo pi = dicTitleAndProperty.Where(p => p.Key == headerRow.GetCell(i).ToString()).First().Value;
                                columnName = pi.Name;//.Where(p => p.Key == headerRow.GetCell(i).ToString()).First().Value.PropertyName;

                                columnType = pi.PropertyType;
                                if (columnType.IsGenericType && columnType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                {
                                    columnType = columnType.GetGenericArguments()[0];
                                }
                            }
                            if (table.Columns.IndexOf(columnName) > 0)
                            {
                                DataColumn column = new DataColumn(Convert.ToString(columnName + i), columnType);
                                table.Columns.Add(column);
                                columnCount++;
                            }
                            else
                            {
                                DataColumn column = new DataColumn(columnName, columnType);
                                table.Columns.Add(column);
                                columnCount++;
                            }
                        }
                    }
                }
                int rowCount = sheet.LastRowNum;
                for (int i = (HeaderRowIndex + 1); i <= sheet.LastRowNum; i++)
                {
                    try
                    {
                        HSSFRow row;
                        if (sheet.GetRow(i) == null)
                        {
                            row = sheet.CreateRow(i) as HSSFRow;
                        }
                        else
                        {
                            row = sheet.GetRow(i) as HSSFRow;
                        }

                        DataRow dataRow = table.NewRow();

                        for (int j = row.FirstCellNum; j <= cellCount; j++)
                        {
                            int columnIndex = j - row.FirstCellNum;
                            if (columnIndex > columnCount - 1)
                            {
                                break;
                            }
                            try
                            {
                                if (row.GetCell(j) != null)
                                {
                                    switch (row.GetCell(j).CellType)
                                    {
                                        case CellType.String:
                                            string str = row.GetCell(j).StringCellValue;
                                            if (str != null && str.Length > 0)
                                            {
                                                dataRow[columnIndex] = str.ToString();
                                            }
                                            else
                                            {
                                                dataRow[columnIndex] = null;
                                            }
                                            break;
                                        case CellType.Numeric:
                                            DateTime dt;
                                            if (row.GetCell(j).CellStyle.DataFormat != 0)
                                            {
                                                if (DateTime.TryParse(row.GetCell(j).ToString(), out dt))
                                                {
                                                    dataRow[columnIndex] = dt;
                                                }
                                                else
                                                {
                                                    dataRow[columnIndex] = row.GetCell(j).ToString();
                                                }
                                            }
                                            else
                                                dataRow[columnIndex] = row.GetCell(j).NumericCellValue;
                                            break;
                                        case CellType.Boolean:
                                            dataRow[columnIndex] = row.GetCell(j).BooleanCellValue;
                                            break;
                                        case CellType.Error:
                                            dataRow[columnIndex] = ErrorEval.GetText(row.GetCell(j).ErrorCellValue);
                                            break;
                                        case CellType.Formula:
                                            HSSFFormulaEvaluator e = new HSSFFormulaEvaluator(sheet.Workbook);
                                            dataRow[columnIndex] = e.Evaluate(row.GetCell(j)).StringValue;
                                            break;
                                        default:
                                            dataRow[columnIndex] = "";
                                            break;
                                    }
                                }
                            }
                            catch (Exception exception)
                            {
                            }
                        }
                        table.Rows.Add(dataRow);
                    }
                    catch (Exception exception)
                    {
                    }
                }
            }
            catch (Exception exception)
            {
            }
            return table;
        }
        #endregion

        #region 更新excel中的数据
        /// <summary>
        /// 更新Excel表格
        /// </summary>
        /// <param name="outputFile">需更新的excel表格路径</param>
        /// <param name="sheetname">sheet名</param>
        /// <param name="updateData">需更新的数据</param>
        /// <param name="coluid">需更新的列号</param>
        /// <param name="rowid">需更新的开始行号</param>
        public static void UpdateExcel(string outputFile, string sheetname, string[] updateData, int coluid, int rowid)
        {
            FileStream readfile = new FileStream(outputFile, FileMode.Open, FileAccess.Read);

            HSSFWorkbook hssfworkbook = new HSSFWorkbook(readfile);
            ISheet sheet1 = hssfworkbook.GetSheet(sheetname);
            for (int i = 0; i < updateData.Length; i++)
            {
                try
                {
                    if (sheet1.GetRow(i + rowid) == null)
                    {
                        sheet1.CreateRow(i + rowid);
                    }
                    if (sheet1.GetRow(i + rowid).GetCell(coluid) == null)
                    {
                        sheet1.GetRow(i + rowid).CreateCell(coluid);
                    }

                    sheet1.GetRow(i + rowid).GetCell(coluid).SetCellValue(updateData[i]);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            try
            {
                readfile.Close();
                FileStream writefile = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
                hssfworkbook.Write(writefile);
                writefile.Close();
            }
            catch (Exception ex)
            {
            }

        }

        /// <summary>
        /// 更新Excel表格
        /// </summary>
        /// <param name="outputFile">需更新的excel表格路径</param>
        /// <param name="sheetname">sheet名</param>
        /// <param name="updateData">需更新的数据</param>
        /// <param name="coluids">需更新的列号</param>
        /// <param name="rowid">需更新的开始行号</param>
        public static void UpdateExcel(string outputFile, string sheetname, string[][] updateData, int[] coluids, int rowid)
        {
            FileStream readfile = new FileStream(outputFile, FileMode.Open, FileAccess.Read);

            HSSFWorkbook hssfworkbook = new HSSFWorkbook(readfile);
            readfile.Close();
            ISheet sheet1 = hssfworkbook.GetSheet(sheetname);
            for (int j = 0; j < coluids.Length; j++)
            {
                for (int i = 0; i < updateData[j].Length; i++)
                {
                    try
                    {
                        if (sheet1.GetRow(i + rowid) == null)
                        {
                            sheet1.CreateRow(i + rowid);
                        }
                        if (sheet1.GetRow(i + rowid).GetCell(coluids[j]) == null)
                        {
                            sheet1.GetRow(i + rowid).CreateCell(coluids[j]);
                        }
                        sheet1.GetRow(i + rowid).GetCell(coluids[j]).SetCellValue(updateData[j][i]);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            try
            {
                FileStream writefile = new FileStream(outputFile, FileMode.Create);
                hssfworkbook.Write(writefile);
                writefile.Close();
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 更新Excel表格
        /// </summary>
        /// <param name="outputFile">需更新的excel表格路径</param>
        /// <param name="sheetname">sheet名</param>
        /// <param name="updateData">需更新的数据</param>
        /// <param name="coluid">需更新的列号</param>
        /// <param name="rowid">需更新的开始行号</param>
        public static void UpdateExcel(string outputFile, string sheetname, double[] updateData, int coluid, int rowid)
        {
            FileStream readfile = new FileStream(outputFile, FileMode.Open, FileAccess.Read);

            HSSFWorkbook hssfworkbook = new HSSFWorkbook(readfile);
            ISheet sheet1 = hssfworkbook.GetSheet(sheetname);
            for (int i = 0; i < updateData.Length; i++)
            {
                try
                {
                    if (sheet1.GetRow(i + rowid) == null)
                    {
                        sheet1.CreateRow(i + rowid);
                    }
                    if (sheet1.GetRow(i + rowid).GetCell(coluid) == null)
                    {
                        sheet1.GetRow(i + rowid).CreateCell(coluid);
                    }

                    sheet1.GetRow(i + rowid).GetCell(coluid).SetCellValue(updateData[i]);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            try
            {
                readfile.Close();
                FileStream writefile = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
                hssfworkbook.Write(writefile);
                writefile.Close();
            }
            catch (Exception ex)
            {
            }

        }

        /// <summary>
        /// 更新Excel表格
        /// </summary>
        /// <param name="outputFile">需更新的excel表格路径</param>
        /// <param name="sheetname">sheet名</param>
        /// <param name="updateData">需更新的数据</param>
        /// <param name="coluids">需更新的列号</param>
        /// <param name="rowid">需更新的开始行号</param>
        public static void UpdateExcel(string outputFile, string sheetname, double[][] updateData, int[] coluids, int rowid)
        {
            FileStream readfile = new FileStream(outputFile, FileMode.Open, FileAccess.Read);

            HSSFWorkbook hssfworkbook = new HSSFWorkbook(readfile);
            readfile.Close();
            ISheet sheet1 = hssfworkbook.GetSheet(sheetname);
            for (int j = 0; j < coluids.Length; j++)
            {
                for (int i = 0; i < updateData[j].Length; i++)
                {
                    try
                    {
                        if (sheet1.GetRow(i + rowid) == null)
                        {
                            sheet1.CreateRow(i + rowid);
                        }
                        if (sheet1.GetRow(i + rowid).GetCell(coluids[j]) == null)
                        {
                            sheet1.GetRow(i + rowid).CreateCell(coluids[j]);
                        }
                        sheet1.GetRow(i + rowid).GetCell(coluids[j]).SetCellValue(updateData[j][i]);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            try
            {
                FileStream writefile = new FileStream(outputFile, FileMode.Create);
                hssfworkbook.Write(writefile);
                writefile.Close();
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 更新Excel表格
        /// </summary>
        /// <param name="outputFile">需更新的excel表格路径</param>
        /// <param name="sheetname">sheet名</param>
        /// <param name="dt">需更新的数据</param>
        /// <param name="coluids">需更新的列号</param>
        /// <param name="rowid">需更新的开始行号</param>
        public static void ExportDTtoExcel(string outputFile, string sheetname, DataTable dt)
        {
            FileStream readfile = new FileStream(outputFile, FileMode.Open, FileAccess.Read);

            HSSFWorkbook hssfworkbook = new HSSFWorkbook(readfile);
            readfile.Close();
            ISheet sheet1 = hssfworkbook.GetSheet(sheetname);
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    try
                    {
                        if (sheet1.GetRow(i) == null)
                        {
                            sheet1.CreateRow(i);
                        }
                        if (sheet1.GetRow(i).GetCell(j) == null)
                        {
                            sheet1.GetRow(i).CreateCell(j);
                        }
                        sheet1.GetRow(i).GetCell(j).SetCellValue(dt.Rows[i][j].ToString());
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            try
            {
                FileStream writefile = new FileStream(outputFile, FileMode.Create);
                hssfworkbook.Write(writefile);
                writefile.Close();
            }
            catch (Exception ex)
            {
            }
        }

        #endregion

        public static int GetSheetNumber(string outputFile)
        {
            int number = 0;
            try
            {
                FileStream readfile = new FileStream(outputFile, FileMode.Open, FileAccess.Read);

                HSSFWorkbook hssfworkbook = new HSSFWorkbook(readfile);
                number = hssfworkbook.NumberOfSheets;

            }
            catch (Exception exception)
            {
            }
            return number;
        }

        public List<string> GetSheetName(string outputFile,bool Is2007)
        {
            List<string> arrayList = new List<string>();

            FileStream readfile = new FileStream(outputFile, FileMode.Open, FileAccess.Read);
            
            try
            {
                if (Is2007)
                {
                    XSSFWorkbook xssfworkbook = new XSSFWorkbook(readfile);

                    for (int i = 0; i < xssfworkbook.NumberOfSheets; i++)
                    {
                        arrayList.Add(xssfworkbook.GetSheetName(i));
                    }
                }
                else
                {
                    HSSFWorkbook hssfworkbook = new HSSFWorkbook(readfile);

                    for (int i = 0; i < hssfworkbook.NumberOfSheets; i++)
                    {
                        arrayList.Add(hssfworkbook.GetSheetName(i));
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
           
            return arrayList;
        }
        /// <summary>
        /// DataTable导出到Excel的MemoryStream
        /// </summary>
        /// <param name="gv"></param>
        /// <param name="needHeader"></param>
        /// <param name="strHeaderText"></param>
        /// <param name="sheetName"></param>
        /// <param name="title">填加xls文件标题信息</param>
        /// <param name="subject">填加文件主题信息</param>
        /// <returns></returns>
        public HSSFWorkbook ExportToExcel(GridView gv, bool needHeader, string strHeaderText, string sheetName, string title, string subject)
        {
            try
            {
                DataTable dtSource = (DataTable)gv.DataSource;
                if (dtSource == null) return null;

                HSSFWorkbook workbook = new HSSFWorkbook();
                ISheet sheet = workbook.CreateSheet();

                #region 右击文件 属性信息
                {
                    DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                    dsi.Company = "I.S Engine";
                    workbook.DocumentSummaryInformation = dsi;

                    SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                    si.Author = "I.S Engine"; //填加xls文件作者信息
                    si.ApplicationName = "I.S Engine"; //填加xls文件创建程序信息
                    si.LastAuthor = "I.S Engine"; //填加xls文件最后保存者信息
                    si.Comments = "I.S Engine"; //填加xls文件作者信息
                    si.Title = title; //填加xls文件标题信息
                    si.Subject = subject;//填加文件主题信息
                    si.CreateDateTime = DateTime.Now;
                    workbook.SummaryInformation = si;
                }
                #endregion

                ICellStyle dateStyle = workbook.CreateCellStyle();
                IDataFormat format = workbook.CreateDataFormat();
                dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

                //取得列宽
                int[] arrColWidth = new int[dtSource.Columns.Count];
                foreach (DataColumn item in dtSource.Columns)
                {
                    arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
                }
                for (int i = 0; i < dtSource.Rows.Count; i++)
                {
                    for (int j = 0; j < dtSource.Columns.Count; j++)
                    {
                        int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
                        if (intTemp > arrColWidth[j])
                        {
                            arrColWidth[j] = intTemp;
                        }
                    }
                }
                int rowIndex = 0;
                foreach (DataRow row in dtSource.Rows)
                {
                    #region 新建表，填充表头，填充列头，样式
                    if (rowIndex == 65535 || rowIndex == 0)
                    {
                        if (rowIndex != 0)
                        {
                            sheet = workbook.CreateSheet(sheetName);
                        }
                        int header = 0;
                        if (needHeader)
                        {
                            #region 表头及样式
                            {
                                IRow headerRow = sheet.CreateRow(header);
                                headerRow.HeightInPoints = 25;
                                headerRow.CreateCell(0).SetCellValue(strHeaderText);

                                ICellStyle headStyle = workbook.CreateCellStyle();
                                headStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                                IFont font = workbook.CreateFont();
                                font.FontHeightInPoints = 20;
                                font.Boldweight = 700;
                                headStyle.SetFont(font);
                                headerRow.GetCell(0).CellStyle = headStyle;
                                sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, dtSource.Columns.Count - 1));
                                // headerRow.Dispose();
                                header++;
                            }
                            #endregion
                        }


                        #region 列头及样式
                        {
                            IRow headerRow = sheet.CreateRow(header);
                            ICellStyle headcellstyle = workbook.CreateCellStyle();
                            headcellstyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                            IFont font = workbook.CreateFont();
                            font.Color = HSSFColor.White.Index;
                            font.FontHeightInPoints = 10;
                            font.Boldweight = 700;
                            headcellstyle.SetFont(font);//设置字体
                            headcellstyle.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;
                            headcellstyle.FillBackgroundColor = HSSFColor.DarkBlue.Index;
                            headcellstyle.FillForegroundColor = HSSFColor.DarkBlue.Index;
                            headcellstyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;//标题水平居中
                            headcellstyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;//标题垂直居中
                            headcellstyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                            headcellstyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                            headcellstyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                            headcellstyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                            headcellstyle.TopBorderColor = HSSFColor.White.Index;
                            headcellstyle.LeftBorderColor = HSSFColor.White.Index;
                            headcellstyle.RightBorderColor = HSSFColor.White.Index;
                            headcellstyle.BottomBorderColor = HSSFColor.White.Index;
                            headcellstyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;


                            //右边框线样式
                            ICellStyle headcellstyleRight = workbook.CreateCellStyle();
                            headcellstyleRight.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                            headcellstyleRight.RightBorderColor = HSSFColor.White.Index;
                            //左边框线样式
                            ICellStyle headcellstyleLeft = workbook.CreateCellStyle();
                            headcellstyleLeft.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                            headcellstyleLeft.LeftBorderColor = HSSFColor.White.Index;


                            for (int i = 0; i < gv.Columns.Count; i++)
                            {
                                DataColumn column = dtSource.Columns[gv.Columns[i].HeaderText];
                                headerRow.CreateCell(column.Ordinal).SetCellValue(gv.Columns[i].HeaderText);
                                headerRow.GetCell(column.Ordinal).CellStyle = headcellstyle;
                                //设置列宽
                                sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);

                            }

                            //foreach (DataColumn column in dtSource.Columns)
                            //{
                            //    headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                            //    headerRow.GetCell(column.Ordinal).CellStyle = headcellstyle;

                            //    //设置列宽
                            //    sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
                            //}
                            //headerRow.Dispose();
                        }
                        #endregion

                        rowIndex = header + 1;
                    }
                    #endregion


                    #region 填充内容
                    IRow dataRow = sheet.CreateRow(rowIndex);
                    for (int i = 0; i < gv.Columns.Count; i++)
                    {
                        DataColumn column = dtSource.Columns[gv.Columns[i].HeaderText];//DataPropertyName
                        ICell newCell = dataRow.CreateCell(column.Ordinal);

                        string drValue = row[column].ToString();

                        switch (column.DataType.ToString())
                        {
                            case "System.String"://字符串类型
                                newCell.SetCellValue(drValue);
                                break;
                            case "System.DateTime"://日期类型
                                DateTime dateV;
                                DateTime.TryParse(drValue, out dateV);
                                newCell.SetCellValue(dateV);

                                newCell.CellStyle = dateStyle;//格式化显示
                                break;
                            case "System.Boolean"://布尔型
                                bool boolV = false;
                                bool.TryParse(drValue, out boolV);
                                newCell.SetCellValue(boolV);
                                break;
                            case "System.Int16"://整型
                            case "System.Int32":
                            case "System.Int64":
                            case "System.Byte":
                                int intV = 0;
                                int.TryParse(drValue, out intV);
                                newCell.SetCellValue(intV);
                                break;
                            case "System.Decimal"://浮点型
                            case "System.Double":
                                if (string.IsNullOrEmpty(drValue)) newCell.SetCellValue("-");
                                else
                                {
                                    double doubV = 0;
                                    double.TryParse(drValue, out doubV);
                                    newCell.SetCellValue(doubV);
                                }
                                break;
                            case "System.DBNull"://空值处理
                                newCell.SetCellValue("-");
                                break;
                            default:
                                newCell.SetCellValue("");
                                break;
                        }

                    }
                    #endregion

                    rowIndex++;
                }
                sheet.ForceFormulaRecalculation = true;
                return workbook;
                //using (MemoryStream ms = new MemoryStream())
                //{
                //    workbook.Write(ms);
                //    ms.Flush();
                //    ms.Position = 0;

                //    sheet.Dispose();
                //    //workbook.Dispose();//一般只用写这一个就OK了，他会遍历并释放所有资源，但当前版本有问题所以只释放sheet
                //    return ms;
                //}
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// DataTable导出到Excel的MemoryStream
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="strHeaderText">表头文本</param>
        public HSSFWorkbook ExportToExcel(DataTable dtSource, bool needHeader, string strHeaderText, string sheetName)
        {
            try
            {
                if (dtSource == null) return null;

                HSSFWorkbook workbook = new HSSFWorkbook();
                ISheet sheet = workbook.CreateSheet();

                #region 右击文件 属性信息
                {
                    DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                    dsi.Company = "I.S Engine";
                    workbook.DocumentSummaryInformation = dsi;

                    SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                    si.Author = "I.S Engine"; //填加xls文件作者信息
                    si.ApplicationName = "I.S Engine"; //填加xls文件创建程序信息
                    si.LastAuthor = "I.S Engine"; //填加xls文件最后保存者信息
                    si.Comments = "I.S Engine"; //填加xls文件作者信息
                    si.Title = "标准报表"; //填加xls文件标题信息
                    si.Subject = "标准报表";//填加文件主题信息
                    si.CreateDateTime = DateTime.Now;
                    workbook.SummaryInformation = si;
                }
                #endregion

                ICellStyle dateStyle = workbook.CreateCellStyle();
                IDataFormat format = workbook.CreateDataFormat();
                dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

                //取得列宽
                int[] arrColWidth = new int[dtSource.Columns.Count];
                foreach (DataColumn item in dtSource.Columns)
                {
                    arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
                }
                for (int i = 0; i < dtSource.Rows.Count; i++)
                {
                    for (int j = 0; j < dtSource.Columns.Count; j++)
                    {
                        int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
                        if (intTemp > arrColWidth[j])
                        {
                            arrColWidth[j] = intTemp;
                        }
                    }
                }
                int rowIndex = 0;
                foreach (DataRow row in dtSource.Rows)
                {
                    #region 新建表，填充表头，填充列头，样式
                    if (rowIndex == 65535 || rowIndex == 0)
                    {
                        if (rowIndex != 0)
                        {
                            sheet = workbook.CreateSheet(sheetName);
                        }
                        int header = 0;
                        if (needHeader)
                        {
                            #region 表头及样式
                            {
                                IRow headerRow = sheet.CreateRow(header);
                                headerRow.HeightInPoints = 25;
                                headerRow.CreateCell(0).SetCellValue(strHeaderText);

                                ICellStyle headStyle = workbook.CreateCellStyle();
                                headStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                                IFont font = workbook.CreateFont();
                                font.FontHeightInPoints = 20;
                                font.Boldweight = 700;
                                headStyle.SetFont(font);
                                headerRow.GetCell(0).CellStyle = headStyle;
                                sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, dtSource.Columns.Count - 1));
                                //headerRow.Dispose();
                                header++;
                            }
                            #endregion
                        }


                        #region 列头及样式
                        {
                            IRow headerRow = sheet.CreateRow(header);
                            ICellStyle headcellstyle = workbook.CreateCellStyle();
                            headcellstyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                            IFont font = workbook.CreateFont();
                            font.Color = HSSFColor.White.Index;
                            font.FontHeightInPoints = 10;
                            font.Boldweight = 700;
                            headcellstyle.SetFont(font);//设置字体
                            headcellstyle.FillPattern = FillPattern.SolidForeground;
                            headcellstyle.FillBackgroundColor = HSSFColor.DarkBlue.Index;
                            headcellstyle.FillForegroundColor = HSSFColor.DarkBlue.Index;
                            headcellstyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;//标题水平居中
                            headcellstyle.VerticalAlignment = VerticalAlignment.Center;//标题垂直居中
                            headcellstyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                            headcellstyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                            headcellstyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                            headcellstyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                            headcellstyle.TopBorderColor = HSSFColor.White.Index;
                            headcellstyle.LeftBorderColor = HSSFColor.White.Index;
                            headcellstyle.RightBorderColor = HSSFColor.White.Index;
                            headcellstyle.BottomBorderColor = HSSFColor.White.Index;
                            headcellstyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;


                            //右边框线样式
                            ICellStyle headcellstyleRight = workbook.CreateCellStyle();
                            headcellstyleRight.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                            headcellstyleRight.RightBorderColor = HSSFColor.White.Index;
                            //左边框线样式
                            ICellStyle headcellstyleLeft = workbook.CreateCellStyle();
                            headcellstyleLeft.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                            headcellstyleLeft.LeftBorderColor = HSSFColor.White.Index;

                            foreach (DataColumn column in dtSource.Columns)
                            {
                                headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                                headerRow.GetCell(column.Ordinal).CellStyle = headcellstyle;

                                //设置列宽
                                sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
                            }
                            //headerRow.Dispose();
                        }
                        #endregion

                        rowIndex = header + 1;
                    }
                    #endregion


                    #region 填充内容
                    IRow dataRow = sheet.CreateRow(rowIndex);
                    foreach (DataColumn column in dtSource.Columns)
                    {
                        ICell newCell = dataRow.CreateCell(column.Ordinal);

                        string drValue = row[column].ToString();

                        switch (column.DataType.ToString())
                        {
                            case "System.String"://字符串类型
                                newCell.SetCellValue(drValue);
                                break;
                            case "System.DateTime"://日期类型
                                DateTime dateV;
                                DateTime.TryParse(drValue, out dateV);
                                newCell.SetCellValue(dateV);

                                newCell.CellStyle = dateStyle;//格式化显示
                                break;
                            case "System.Boolean"://布尔型
                                bool boolV = false;
                                bool.TryParse(drValue, out boolV);
                                newCell.SetCellValue(boolV);
                                break;
                            case "System.Int16"://整型
                            case "System.Int32":
                            case "System.Int64":
                            case "System.Byte":
                                int intV = 0;
                                int.TryParse(drValue, out intV);
                                newCell.SetCellValue(intV);
                                break;
                            case "System.Decimal"://浮点型
                            case "System.Double":
                                if (string.IsNullOrEmpty(drValue)) newCell.SetCellValue("-");
                                else
                                {
                                    double doubV = 0;
                                    double.TryParse(drValue, out doubV);
                                    newCell.SetCellValue(doubV);
                                }
                                break;
                            case "System.DBNull"://空值处理
                                newCell.SetCellValue("-");
                                break;
                            default:
                                newCell.SetCellValue("");
                                break;
                        }

                    }
                    #endregion

                    rowIndex++;
                }
                sheet.ForceFormulaRecalculation = true;
                return workbook;
                //using (MemoryStream ms = new MemoryStream())
                //{
                //    workbook.Write(ms);
                //    ms.Flush();
                //    ms.Position = 0;

                //    sheet.Dispose();
                //    //workbook.Dispose();//一般只用写这一个就OK了，他会遍历并释放所有资源，但当前版本有问题所以只释放sheet
                //    return ms;
                //}
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region 导出DataSet到Excel

        /// <summary>
        /// DataSet导出到Excel的MemoryStream
        /// </summary>
        /// <param name="dtSet">源DataSet</param>
        /// <param name="AStrListHeaderText">表头文本</param>
        /// <param name="AStrListSheetName">表格名称</param>
        static MemoryStream ExportDS(DataSet dtSet, List<string> AStrListHeaderText, List<string> AStrListSheetName, Dictionary<string, string> dicNameAndTitle)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            if (dtSet.Tables.Count > 0)
            {
                #region 右击文件 属性信息

                //{
                //    DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                //    dsi.Company = "http://www.yongfa365.com/";
                //    workbook.DocumentSummaryInformation = dsi;

                //    SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                //    si.Author = "柳永法"; //填加xls文件作者信息
                //    si.ApplicationName = "NPOI测试程序"; //填加xls文件创建程序信息
                //    si.LastAuthor = "柳永法2"; //填加xls文件最后保存者信息
                //    si.Comments = "说明信息"; //填加xls文件作者信息
                //    si.Title = "NPOI测试"; //填加xls文件标题信息
                //    si.Subject = "NPOI测试Demo"; //填加文件主题信息
                //    si.CreateDateTime = DateTime.Now;
                //    workbook.SummaryInformation = si;
                //}

                #endregion

                HSSFCellStyle dateStyle = workbook.CreateCellStyle() as HSSFCellStyle;
                HSSFDataFormat format = workbook.CreateDataFormat() as HSSFDataFormat;
                dateStyle.DataFormat = format.GetFormat("yyyy-MM-dd");
                foreach (DataTable dtSource in dtSet.Tables)
                {
                    int tableIndex = dtSet.Tables.IndexOf(dtSource);
                    HSSFSheet sheet = null;
                    string sheetName = dtSource.TableName;
                    if (AStrListSheetName != null && AStrListSheetName.Count > tableIndex && !string.IsNullOrEmpty(AStrListSheetName[tableIndex]))
                    {
                        sheetName = AStrListSheetName[tableIndex];
                    }
                    sheet = workbook.CreateSheet(sheetName) as HSSFSheet;
                    //取得列宽
                    int[] arrColWidth = new int[dtSource.Columns.Count];
                    foreach (DataColumn item in dtSource.Columns)
                    {
                        arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
                    }
                    for (int i = 0; i < dtSource.Rows.Count; i++)
                    {
                        for (int j = 0; j < dtSource.Columns.Count; j++)
                        {
                            int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
                            if (intTemp > arrColWidth[j])
                            {
                                arrColWidth[j] = intTemp;
                            }
                        }
                    }
                    int rowIndex = 0;

                    foreach (DataRow row in dtSource.Rows)
                    {
                        #region 新建表，填充表头，填充列头，样式

                        if (rowIndex == 65535 || rowIndex == 0)
                        {
                            if (rowIndex != 0)
                            {
                                sheet = workbook.CreateSheet(sheetName + rowIndex) as HSSFSheet;
                            }

                            #region 表头及样式
                            if (AStrListHeaderText != null && AStrListHeaderText.Count > tableIndex && !string.IsNullOrEmpty(AStrListHeaderText[tableIndex]))
                            {
                                HSSFRow headerRow = sheet.CreateRow(0) as HSSFRow;
                                headerRow.HeightInPoints = 25;
                                headerRow.CreateCell(0).SetCellValue(AStrListHeaderText[tableIndex]);

                                HSSFCellStyle headStyle = workbook.CreateCellStyle() as HSSFCellStyle;
                                headStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                                HSSFFont font = workbook.CreateFont() as HSSFFont;
                                font.FontHeightInPoints = 20;
                                font.Boldweight = 700;
                                headStyle.SetFont(font);

                                headerRow.GetCell(0).CellStyle = headStyle;

                                sheet.AddMergedRegion(new Region(0, 0, 0, dtSource.Columns.Count - 1));
                                //headerRow.Dispose();

                                rowIndex++;
                            }

                            #endregion


                            #region 列头及样式
                            if(AStrListHeaderText == null || AStrListHeaderText.Count <= tableIndex)
                            {
                                HSSFRow headerRow = sheet.CreateRow(1) as HSSFRow;

                                HSSFCellStyle headStyle = workbook.CreateCellStyle() as HSSFCellStyle;
                                headStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                                HSSFFont font = workbook.CreateFont() as HSSFFont;
                                font.FontHeightInPoints = 10;
                                font.Boldweight = 700;
                                headStyle.SetFont(font);


                                foreach (DataColumn column in dtSource.Columns)
                                {
                                    string columnTitle = column.ColumnName;
                                    if (dicNameAndTitle != null && dicNameAndTitle.Count > 0)
                                    {
                                        if (dicNameAndTitle.ContainsKey(column.ColumnName))
                                        {
                                            columnTitle = dicNameAndTitle[column.ColumnName];
                                            headerRow.CreateCell(column.Ordinal).SetCellValue(columnTitle);
                                            headerRow.GetCell(column.Ordinal).CellStyle = headStyle;

                                            //设置列宽
                                            sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
                                        }
                                    }
                                    else
                                    {
                                        headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                                        headerRow.GetCell(column.Ordinal).CellStyle = headStyle;

                                        //设置列宽
                                        sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
                                    }

                                }
                                //headerRow.Dispose();
                            }

                            #endregion

                            rowIndex = 2;
                        }

                        #endregion

                        #region 填充内容

                        HSSFRow dataRow = sheet.CreateRow(rowIndex) as HSSFRow;
                        foreach (DataColumn column in dtSource.Columns)
                        {
                            if (dicNameAndTitle == null || dicNameAndTitle.Count == 0 || dicNameAndTitle.ContainsKey(column.ColumnName))
                            {
                                HSSFCell newCell = dataRow.CreateCell(column.Ordinal) as HSSFCell;
                                string drValue = row[column].ToString();

                                switch (column.DataType.ToString())
                                {
                                    case "System.String": //字符串类型
                                        newCell.SetCellValue(drValue);
                                        break;
                                    case "System.DateTime": //日期类型
                                        DateTime dateV;
                                        if (DateTime.TryParse(drValue, out dateV))
                                            newCell.SetCellValue(dateV);
                                        else
                                            newCell.SetCellValue(string.Empty);
                                        newCell.CellStyle = dateStyle; //格式化显示
                                        break;
                                    case "System.Boolean": //布尔型
                                        bool boolV = false;
                                        bool.TryParse(drValue, out boolV);
                                        newCell.SetCellValue(boolV);
                                        break;
                                    case "System.Int16": //整型
                                    case "System.Int32":
                                    case "System.Int64":
                                    case "System.Byte":
                                        int intV = 0;
                                        int.TryParse(drValue, out intV);
                                        if (intV != 0)
                                        {
                                            newCell.SetCellValue(intV);
                                        }
                                        else
                                        {
                                            newCell.SetCellValue(string.Empty);
                                        }
                                        break;
                                    case "System.Decimal": //浮点型
                                    case "System.Double":
                                        double doubV = 0;
                                        double.TryParse(drValue, out doubV);
                                        newCell.SetCellValue(doubV);
                                        break;
                                    case "System.DBNull": //空值处理
                                        newCell.SetCellValue("");
                                        break;
                                    default:
                                        newCell.SetCellValue("");
                                        break;
                                }

                            }
                        }

                        #endregion

                        rowIndex++;
                    }
                }
            }
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;

                //sheet.Dispose();
                //workbook.Clear();

                return ms;
            }
        }

        /// <summary>
        /// DataSet导出到Excel文件
        /// </summary>
        /// <param name="dtSet">源DataSet</param>
        /// <param name="AStrListHeaderText">表头文本</param>
        /// <param name="AStrListSheetName">表格名称</param>
        /// <param name="strFileName">保存位置</param>
        public static void ExportDStoExcel(DataSet dtSet, List<string> AStrListHeaderText, List<string> AStrListSheetName, string strFileName, Dictionary<string, string> dicNameAndTitle)
        {
            using (MemoryStream ms = ExportDS(dtSet, AStrListHeaderText, AStrListSheetName, dicNameAndTitle))
            {
                using (FileStream fs = new FileStream(strFileName, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }
        }
        #endregion

        #region NPOI Excel导出功能 +ExcelOut(List<T> arr, string path, List<string> Attries, List<string > headers)
        public static void ExcelOut<T>(List<T> arr, string path, List<string> Attries, List<string> headers)
        {
            try
            {
                HSSFWorkbook wb = new HSSFWorkbook();//创建一个工作薄
                HSSFSheet sheet = wb.CreateSheet() as HSSFSheet;//在工作薄中创建一个工作表
                for (int i = 0; i < headers.Count; i++) //循环一个表头来创建第一行的表头
                {
                    HSSFRow rw = sheet.CreateRow(0) as HSSFRow;
                    rw.CreateCell(i).SetCellValue(headers[i]);
                }
                Type t = typeof(T); //获取得泛型集合中的实体， 返回T的类型
                System.Reflection.PropertyInfo[] properties = t.GetProperties(); //返回当前获得实体后 实体类型中的所有公共属性
                for (int i = 0; i < arr.Count; i++)//循环实体泛型集合
                {
                    HSSFRow rw = sheet.CreateRow(i + 1) as HSSFRow;//创建一个新行，把传入集合中的每条数据创建一行
                    foreach (System.Reflection.PropertyInfo property in properties)//循环得到的所有属性（想要把里面指定的属性值导出到Excel文件中）
                    {
                        for (int j = 0; j < Attries.Count; j++)//循环需要导出属性值 的 属性名
                        {
                            string attry = Attries[j];//获得一个需要导入的属性名；
                            if (string.Compare(property.Name.ToUpper(), attry.ToUpper()) == 0)//如果需要导出的属性名和当前循环实体的属性名一样，
                            {
                                object result = property.GetValue(arr[i], null);//获取当前循环的实体属性在当前实体对象（arr[i]）的值
                                rw.CreateCell(j).SetCellValue((result == null) ? string.Empty : result.ToString());//创建单元格并进行赋值
                            }
                        }
                    }
                }
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    wb.Write(fs);
                }
            }
            catch (Exception ex)
            {

                throw new Exception("在导出Excel时文件出错啦=====" + ex.Message);
            }
        }
        #endregion

        #region 导入
        /// <summary>
        /// Excel导入DataSet
        /// </summary>
        /// <param name="excelPath"></param>
        /// <returns></returns>
        public DataSet ExcelToDataSet(string excelPath,string SheetName,bool Is2007)
        {
            return ExcelToDataSet(excelPath, true,SheetName, Is2007);
        }

        /// <summary>
        /// Excel导入DataSet
        /// </summary>
        /// <param name="excelPath"></param>
        /// <param name="firstRowAsHeader"></param>
        /// <returns></returns>
        public static DataSet ExcelToDataSet(string excelPath, bool firstRowAsHeader,string SheetName,bool Is2007)
        {
            int sheetCount;
            return ExcelToDataSet(excelPath, firstRowAsHeader, out sheetCount,SheetName, Is2007);
        }

        /// <summary>
        /// Excel导入DataSet
        /// </summary>
        /// <param name="excelPath"></param>
        /// <param name="firstRowAsHeader"></param>
        /// <param name="sheetCount"></param>
        /// <returns></returns>
        public static DataSet ExcelToDataSet(string excelPath, bool firstRowAsHeader, out int sheetCount,string SheetName,bool Is2007)
        {
            using (DataSet ds = new DataSet())
            {
                using (FileStream fileStream = new FileStream(excelPath, FileMode.Open, FileAccess.Read))
                {
                    if(Is2007)
                    {
                        XSSFWorkbook workbook = new XSSFWorkbook(fileStream);
                        XSSFFormulaEvaluator evaluator = new XSSFFormulaEvaluator(workbook);
                        sheetCount = workbook.NumberOfSheets;

                        //for (int i = 0; i < sheetCount; ++i)
                        //{
                            //ISheet sheet = workbook.GetSheetAt(i);// as ISheet;
                        ISheet sheet = workbook.GetSheet(SheetName);
                        DataTable dt = ExcelToDataTable(sheet, evaluator, firstRowAsHeader);
                        ds.Tables.Add(dt);
                        //}
                    }
                    else
                    {
                        HSSFWorkbook workbook = new HSSFWorkbook(fileStream);
                        HSSFFormulaEvaluator evaluator = new HSSFFormulaEvaluator(workbook);
                        sheetCount = workbook.NumberOfSheets;

                        //for (int i = 0; i < sheetCount; ++i)
                        //{
                            //ISheet sheet = workbook.GetSheetAt(i);// as ISheet;
                        ISheet sheet = workbook.GetSheet(SheetName);
                        DataTable dt = ExcelToDataTable(sheet, evaluator, firstRowAsHeader);
                        ds.Tables.Add(dt);
                        //}
                    }
                    return ds;
                }
            }
        }

        /// <summary>
        /// Excel导入DataTable
        /// </summary>
        /// <param name="excelPath">Excel路径</param>
        /// <param name="sheetName">工作表名</param>
        /// <returns></returns>
        public static DataTable ExcelToDataTable(string excelPath, string sheetName)
        {
            return ExcelToDataTable(excelPath, sheetName, true);
        }

        /// <summary>
        /// Excel导入DataTable,默认导入第一个工作表
        /// </summary>
        /// <param name="excelPath">Excel路径</param>
        /// <returns></returns>
        public static DataTable ExcelToDataTable(string excelPath)
        {
            return ExcelToDataTable(excelPath, null, true);
        }

        /// <summary>
        /// Excel导入DataTable
        /// </summary>
        /// <param name="excelPath">Excel路径</param>
        /// <param name="sheetName">工作</param>
        /// <param name="firstRowAsHeader">第一行是否为表头</param>
        /// <returns></returns>
        public static DataTable ExcelToDataTable(string excelPath, string sheetName, bool firstRowAsHeader)
        {
            using (FileStream fileStream = new FileStream(excelPath, FileMode.Open, FileAccess.Read))
            {
                XSSFWorkbook workbook = new XSSFWorkbook(fileStream);

                XSSFFormulaEvaluator evaluator = new XSSFFormulaEvaluator(workbook);
                ISheet sheet;
                if (sheetName != null)
                {
                    sheet = workbook.GetSheet(sheetName);
                }
                else
                {
                    sheet = workbook.GetSheetAt(0);
                }
                if (sheet == null)
                    throw new NullReferenceException("没有找到<" + sheetName + ">工作表");
                return ExcelToDataTable(sheet, evaluator, firstRowAsHeader);
            }
        }

        private static DataTable ExcelToDataTable(ISheet sheet, XSSFFormulaEvaluator evaluator, bool firstRowAsHeader)
        {
            if (firstRowAsHeader)
            {
                return ExcelToDataTableFirstRowAsHeader(sheet, evaluator);
            }
            else
            {
                return ExcelToDataTable(sheet, evaluator);
            }
        }
        private static DataTable ExcelToDataTable(ISheet sheet, HSSFFormulaEvaluator evaluator, bool firstRowAsHeader)
        {
            if (firstRowAsHeader)
            {
                return ExcelToDataTableFirstRowAsHeader(sheet, evaluator);
            }
            else
            {
                return ExcelToDataTable(sheet, evaluator);
            }
        }

        //第一行作为标题
        private static DataTable ExcelToDataTableFirstRowAsHeader(ISheet sheet, XSSFFormulaEvaluator evaluator)
        {
            using (DataTable dt = new DataTable())
            {
                IRow firstRow = sheet.GetRow(0) ;//第一行作为标题
                //IRow firstRow;
                //try
                //{
                //    firstRow = sheet.GetRow(3);//第三行做表头
                //}
                //catch (Exception)
                //{

                //    throw new NullReferenceException("表头设置错误");
                //}

                int cellCount = GetCellCount(sheet);


                for (int i = 0; i < cellCount; i++)//从第一列开始取数据
                {
                    if (firstRow.GetCell(i) != null)
                    {
                        dt.Columns.Add(firstRow.GetCell(i).StringCellValue.Trim() ?? string.Format("F{0}", i + 1), typeof(string));
                    }
                    else
                    {
                        dt.Columns.Add(string.Format("F{0}", i + 1), typeof(string));
                    }
                }
                 for (int i = 1; i <= sheet.LastRowNum; i++)//从第二行开始取数据
                //for (int i = 4; i <= sheet.LastRowNum; i++)//从第四行开始
                {
                    IRow row = sheet.GetRow(i);
                    DataRow dr = dt.NewRow();
                    FillDataRowByHSSFRow(row, evaluator, ref dr);
                    dt.Rows.Add(dr);
                }

                dt.TableName = sheet.SheetName;
                return dt;
            }
        }
        private static DataTable ExcelToDataTableFirstRowAsHeader(ISheet sheet, HSSFFormulaEvaluator evaluator)
        {
            using (DataTable dt = new DataTable())
            {
                IRow firstRow = sheet.GetRow(0);//第一行作为标题
                //IRow firstRow;
                //try
                //{
                //    firstRow = sheet.GetRow(3);//第三行做表头
                //}
                //catch (Exception)
                //{

                //    throw new NullReferenceException("表头设置错误");
                //}

                int cellCount = GetCellCount(sheet);


                for (int i = 0; i < cellCount; i++)//从第一列开始取数据
                {
                    if (firstRow.GetCell(i) != null)
                    {
                        dt.Columns.Add(firstRow.GetCell(i).StringCellValue.Trim() ?? string.Format("F{0}", i + 1), typeof(string));
                    }
                    else
                    {
                        dt.Columns.Add(string.Format("F{0}", i + 1), typeof(string));
                    }
                }

                for (int i = 1; i <= sheet.LastRowNum; i++)//从第二行开始取数据
                //for (int i = 4; i <= sheet.LastRowNum; i++)//从第四行开始
                {
                    IRow row = sheet.GetRow(i);
                    DataRow dr = dt.NewRow();
                    FillDataRowByHSSFRow(row, evaluator, ref dr);
                    dt.Rows.Add(dr);
                }

                dt.TableName = sheet.SheetName;
                return dt;
            }
        }

        //导入DataTable
        private static DataTable ExcelToDataTable(ISheet sheet, XSSFFormulaEvaluator evaluator)
        {
            using (DataTable dt = new DataTable())
            {
                if (sheet.LastRowNum != 0)
                {
                    int cellCount = GetCellCount(sheet);

                    for (int i = 0; i < cellCount; i++)
                    {
                        dt.Columns.Add(string.Format("F{0}", i), typeof(string));
                    }

                    for (int i = 0; i < sheet.FirstRowNum; ++i)
                    {
                        DataRow dr = dt.NewRow();
                        dt.Rows.Add(dr);
                    }

                    for (int i = sheet.FirstRowNum; i <= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        DataRow dr = dt.NewRow();
                        FillDataRowByHSSFRow(row, evaluator, ref dr);
                        dt.Rows.Add(dr);
                    }
                }

                dt.TableName = sheet.SheetName;
                return dt;
            }
        }
        private static DataTable ExcelToDataTable(ISheet sheet, HSSFFormulaEvaluator evaluator)
        {
            using (DataTable dt = new DataTable())
            {
                if (sheet.LastRowNum != 0)
                {
                    int cellCount = GetCellCount(sheet);

                    for (int i = 0; i < cellCount; i++)
                    {
                        dt.Columns.Add(string.Format("F{0}", i), typeof(string));
                    }

                    for (int i = 0; i < sheet.FirstRowNum; ++i)
                    {
                        DataRow dr = dt.NewRow();
                        dt.Rows.Add(dr);
                    }

                    for (int i = sheet.FirstRowNum; i <= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        DataRow dr = dt.NewRow();
                        FillDataRowByHSSFRow(row, evaluator, ref dr);
                        dt.Rows.Add(dr);
                    }
                }

                dt.TableName = sheet.SheetName;
                return dt;
            }
        }
        /// <summary>
        /// 通过IRow填充DataRow
        /// </summary>
        /// <param name="row"></param>
        /// <param name="evaluator"></param>
        /// <param name="dr"></param>
        private static void FillDataRowByHSSFRow(IRow row, XSSFFormulaEvaluator evaluator, ref DataRow dr)
        {
            if (row != null)
            {
                for (int j = 0; j < dr.Table.Columns.Count; j++)
                {
                    XSSFCell cell = row.GetCell(j) as XSSFCell;

                    if (cell != null && cell.GetRawValue() != string.Empty)
                    {
                        switch (cell.CellType)
                        {
                            case CellType.Blank:
                                dr[j] = DBNull.Value;
                                break;
                            case CellType.Boolean:
                                dr[j] = cell.BooleanCellValue;
                                break;
                            case CellType.Numeric:
                                if (DateUtil.IsCellDateFormatted(cell))
                                {
                                    dr[j] = cell.DateCellValue;
                                }
                                else
                                {
                                    dr[j] = cell.NumericCellValue;
                                }
                                break;
                            case CellType.String:
                                dr[j] = cell.StringCellValue;
                                break;
                            case CellType.Error:
                                dr[j] = cell.ErrorCellValue;
                                break;
                            case CellType.Formula:
                                cell = evaluator.EvaluateInCell(cell) as XSSFCell;
                                dr[j] = cell.ToString();
                                break;
                            default:
                                throw new NotSupportedException(string.Format("Catched unhandle CellType[{0}]", cell.CellType));
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        private static void FillDataRowByHSSFRow(IRow row, HSSFFormulaEvaluator evaluator, ref DataRow dr)
        {
            if (row != null)
            {
                for (int j = 0; j < dr.Table.Columns.Count; j++)
                {
                    HSSFCell cell = row.GetCell(j) as HSSFCell;

                    if (cell != null)
                    {
                        switch (cell.CellType)
                        {
                            case CellType.Blank:
                                dr[j] = DBNull.Value;
                                break;
                            case CellType.Boolean:
                                dr[j] = cell.BooleanCellValue;
                                break;
                            case CellType.Numeric:
                                if (DateUtil.IsCellDateFormatted(cell))
                                {
                                    dr[j] = cell.DateCellValue;
                                }
                                else
                                {
                                    dr[j] = cell.NumericCellValue;
                                }
                                break;
                            case CellType.String:
                                dr[j] = cell.StringCellValue;
                                break;
                            case CellType.Error:
                                dr[j] = cell.ErrorCellValue;
                                break;
                            case CellType.Formula:
                                cell = evaluator.EvaluateInCell(cell) as HSSFCell;
                                dr[j] = cell.ToString();
                                break;
                            default:
                                throw new NotSupportedException(string.Format("Catched unhandle CellType[{0}]", cell.CellType));
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 得到cell总数
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        private static int GetCellCount(ISheet sheet)
        {
            int firstRowNum = sheet.FirstRowNum;

            int cellCount = 0;

            for (int i = sheet.FirstRowNum; i <= sheet.LastRowNum; ++i)
            {
                IRow row = sheet.GetRow(i);

                if (row != null && row.LastCellNum > cellCount)
                {
                    cellCount = row.LastCellNum;
                }
            }

            return cellCount;
        }
        #endregion
    }
}