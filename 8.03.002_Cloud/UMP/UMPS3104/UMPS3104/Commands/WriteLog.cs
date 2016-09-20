using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UMPS3104.Commands
{
    class WriteLog
    {
        public static string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                   string.Format("UMP/UMPS3104/Logs"));
        public static string FileName = string.Empty;//日志文件名

        /// <summary>
        /// 将文本写入日志文件
        /// </summary>
        /// <param name="txt"></param>
        private static void WriteTextToFile(string txt)
        {

            if ((WriteLog.FilePath != string.Empty) && (!Directory.Exists(WriteLog.FilePath)))
            {
                Directory.CreateDirectory(WriteLog.FilePath);
            }

            if (WriteLog.FileName == string.Empty)
                WriteLog.FileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".log";

            StreamWriter sw = null;
            FileInfo finfo = new FileInfo(WriteLog.FilePath + "\\" + WriteLog.FileName);
            if (!finfo.Exists)
            {
                sw = File.CreateText(WriteLog.FilePath + "\\" + WriteLog.FileName);
            }
            else
            {
                sw = new StreamWriter(finfo.OpenWrite());
            }

            sw.BaseStream.Seek(0, SeekOrigin.End);

            sw.WriteLine(txt);
            sw.Flush();
            sw.Close();
        }

        public static void WriteLogToFile(string infohead,string msg)
        {
            StringBuilder sb=new StringBuilder();
            sb.Append("\t"+infohead + "\t" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.Append("\t");
            sb.Append(msg);
            sb.Append(System.Environment.NewLine);
            try
            {
                WriteTextToFile(sb.ToString());
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
            finally
            {
                DeleteOldLog();
            }
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
                        TimeSpan timeOld=new TimeSpan(fileInfo.LastWriteTime.Ticks);
                        TimeSpan timeNew=new TimeSpan(DateTime.Now.Ticks);
                        if (timeOld.Subtract(timeNew)> timespan)
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
    }
}
