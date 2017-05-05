//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    4045bf01-a25c-4750-8a68-7cb6519a7b7c
//        CLR Version:              4.0.30319.18444
//        Name:                     ExportDataHelper
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Codes
//        File Name:                ExportDataHelper
//
//        created by Charley at 2014/11/20 13:15:26
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Win32;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Controls;

namespace UMPS3102.Codes
{
    public class ExportDataHelper
    {
        public OperationReturn ExportDataToExecel<T>(List<T> listData, List<ViewColumnInfo> listColumns, UMPApp currentApp)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listData == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("ListData is null");
                    return optReturn;
                }
                if (listData.Count <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_NOT_EXIST;
                    optReturn.Message = string.Format("No Record Data");
                    return optReturn;
                }
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.DefaultExt = "xls";
                dialog.Filter = "Excel Files  (*.xls)|*.xls";
                dialog.FilterIndex = 1;
                dialog.RestoreDirectory = true;
                if (dialog.ShowDialog() != true)
                {
                    optReturn.Code = Defines.RET_ALREADY_EXIST;
                    return optReturn;
                }
                StringBuilder sb = new StringBuilder();
                List<string> listFields = new List<string>();
                int intColCount = listColumns.Count;
                //列标题
                for (int i = 0; i < intColCount; i++)
                {
                    string strTitle =
                        currentApp.GetLanguageInfo(
                            string.Format("COL{0}{1}", listColumns[i].ViewID, listColumns[i].ColumnName),
                            listColumns[i].ColumnName);
                    listFields.Add(FormatField(strTitle));
                }
                BuildStringToRow(sb, listFields);
                //列数据
                PropertyInfo[] listProperty;
                PropertyInfo property;
                for (int i = 0; i < listData.Count; i++)
                {
                    T item = listData[i];
                    listFields.Clear();
                    listProperty = item.GetType().GetProperties();
                    for (int j = 0; j < intColCount; j++)
                    {
                        ViewColumnInfo column = listColumns[j];
                        property = listProperty.FirstOrDefault(p => p.Name == column.ColumnName);
                        if (property != null)
                        {
                            var temp = property.GetValue(item, null);
                            if (temp != null)
                            {
                                string strValue;
                                switch (column.ColumnName)
                                {
                                    case "Duration":
                                        strValue = Converter.Second2Time(Convert.ToDouble(temp));
                                        break;
                                    case "StartRecordTime":
                                    case "StopRecordTime":
                                        strValue = Convert.ToDateTime(temp).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                                        break;
                                    case "Direction":
                                        strValue = currentApp.GetLanguageInfo(string.Format("3102TIP001{0}", temp),temp.ToString());
                                        break;
                                    default:
                                        strValue = temp.ToString();
                                        break;
                                }
                                listFields.Add(FormatField(strValue));
                            }
                            else
                            {
                                listFields.Add(FormatField(String.Empty));
                            }
                        }
                        else
                        {
                            listFields.Add(FormatField(string.Empty));
                        }
                    }
                    BuildStringToRow(sb, listFields);
                }

                StreamWriter streamWriter = new StreamWriter(dialog.OpenFile());
                streamWriter.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                streamWriter.WriteLine("<?mso-application progid=\"Excel.Sheet\"?>");
                streamWriter.WriteLine("<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\">");
                streamWriter.WriteLine("<DocumentProperties xmlns=\"urn:schemas-microsoft-com:office:office\">");
                streamWriter.WriteLine("<Author>" + currentApp.Session.UserInfo.Account + "</Author>");
                streamWriter.WriteLine("<Created>" + DateTime.Now.ToLocalTime().ToLongDateString() + "</Created>");
                streamWriter.WriteLine("<LastSaved>" + DateTime.Now.ToLocalTime().ToLongDateString() + "</LastSaved>");
                streamWriter.WriteLine("<Company>" + "VoiceCyber" + "</Company>");
                streamWriter.WriteLine("<Version>1.00</Version>");
                streamWriter.WriteLine("</DocumentProperties>");
                streamWriter.WriteLine("<Worksheet ss:Name=\"UMP Export\" xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\">");
                streamWriter.WriteLine("<Table>");
                streamWriter.WriteLine(sb);
                streamWriter.WriteLine("</Table>");
                streamWriter.WriteLine("</Worksheet>");
                streamWriter.WriteLine("</Workbook>");

                streamWriter.Close();
                optReturn.Data = dialog.FileName;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private string FormatField(string strData)
        {
            return String.Format("<Cell><Data ss:Type=\"String\">{0}</Data></Cell>", strData);
        }

        private void BuildStringToRow(StringBuilder stringBuilder, List<string> listFields)
        {
            stringBuilder.AppendLine("<Row>");
            stringBuilder.AppendLine(String.Join("\r\n", listFields.ToArray()));
            stringBuilder.AppendLine("</Row>");
        }
    }
}
