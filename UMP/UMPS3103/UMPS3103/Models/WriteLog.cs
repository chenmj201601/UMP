using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace UMPS3103.Models
{
   public class WriteLog
    {
        public static string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                   string.Format("UMP/UMPS3103/UserInfo"));

        /// <summary>
        /// 将文本写入文件中保存
        /// </summary>
        private static void WriteTextToFile(string txt, string FileName)
        {

            if ((WriteLog.FilePath != string.Empty) && (!Directory.Exists(WriteLog.FilePath)))
            {
                Directory.CreateDirectory(WriteLog.FilePath);
            }

            StreamWriter sw = null;
            FileInfo finfo = new FileInfo(WriteLog.FilePath + "\\" + FileName);
            if (!finfo.Exists)
            {
                sw = File.CreateText(WriteLog.FilePath + "\\" + FileName);
            }
            else//如果存在该用户保存的文件就删掉再来一次 foolish
            {
                File.Delete(WriteLog.FilePath + "\\" + FileName);
                sw = File.CreateText(WriteLog.FilePath + "\\" + FileName);
            }

            sw.BaseStream.Seek(0, SeekOrigin.End);

            sw.WriteLine(txt);
            sw.Flush();
            sw.Close();
        }

        /// <summary>
        /// 安装日志
        /// </summary>
        public static void WriteLogToFile(string infohead, string msg)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\t" + infohead + "\t" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.Append("\t");
            sb.Append(msg);
            sb.Append(System.Environment.NewLine);
            try
            {
                //string FileName = "InstallCertificate_"+DateTime.Now.ToString("yyyyMMdd") + ".log";
                string FileName = "SaveColumnsInfo.log";
                WriteTextToFile(sb.ToString(), FileName);
            }
            catch (Exception ex)
            {
                //App.ShowExceptionMessage(ex.Message);
            }
            //finally
            //{
            //    DeleteOldLog();
            //}
        }

        /// <summary>
        /// 刪除7天后的日誌
        /// 日誌文件超過99刪除
        /// </summary>
        public static void DeleteOldLog()
        {
            if (WriteLog.FilePath != string.Empty)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(WriteLog.FilePath);

                foreach (FileInfo fileInfo in directoryInfo.GetFiles())
                {
                    if (fileInfo.Extension.ToLower() == ".log")
                    {
                        TimeSpan timespan = new TimeSpan(7, 0, 0, 0);
                        TimeSpan timeOld = new TimeSpan(fileInfo.LastWriteTime.Ticks);
                        TimeSpan timeNew = new TimeSpan(DateTime.Now.Ticks);
                        if (timeOld.Subtract(timeNew) > timespan)
                        {
                            fileInfo.Delete();
                        }
                    }
                }
                string[] filesLen = Directory.GetFiles(WriteLog.FilePath);
                if (filesLen.Length > 50)
                {
                    FileInfo fileInfo = directoryInfo.GetFiles().First();
                    fileInfo.Delete();
                }
            }
        }

        /// <summary>
        ///保存服务器配置信息
        /// </summary>
        public static void CreatColumnsInfoXml(string UserID,string Lans,string Cols, string Widths)
        {
            try
            {
                StringBuilder xmlSB = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n");
                xmlSB.Append("<ColumnsInfo>\r\n");
                xmlSB.AppendFormat("<Lans>{0}</Lans>\r\n", Lans);
                xmlSB.AppendFormat("<Cols>{0}</Cols>\r\n", Cols);
                xmlSB.AppendFormat("<Widths>{0}</Widths>\r\n", Widths);
                xmlSB.Append("</ColumnsInfo>");

                string FileName = string.Format("{0}.xml",UserID);
                WriteTextToFile(xmlSB.ToString(), FileName);
            }
            catch (Exception ex)
            {
                WriteLogToFile("Save Falied", "\t" + string.Format("UserID:{0}   Message:{1}", UserID,ex.Message));
            }
            WriteLogToFile("Save Sucessed", "\t" + string.Format("UserID:{0} ", UserID));
        }

       /// <summary>
       /// 读取保存列名的xml，文件名用用户ID代替
       /// </summary>
        public static List<string> ReadColumnsXml(string UserID)
        {
            List<string> columnsList = new List<string>();
            try
            {
                string xmlPath = Path.Combine(FilePath,string.Format("{0}.xml",UserID));
                if (!File.Exists(xmlPath)) { return null; }
                XmlDocument columnsXml = new XmlDocument();
                columnsXml.Load(xmlPath);
                columnsList.Add(columnsXml.SelectSingleNode("ColumnsInfo/Lans").InnerText);//lans
                columnsList.Add(columnsXml.SelectSingleNode("ColumnsInfo/Cols").InnerText);//cols
                columnsList.Add(columnsXml.SelectSingleNode("ColumnsInfo/Widths").InnerText);//widths
            }
            catch (Exception ex)
            {
                WriteLogToFile("LoadColums Falied", "\t" + string.Format("UserID:{0}   Message:{1}", UserID,ex.Message));
                return null;
            }
            return columnsList;
        }
    }
}
