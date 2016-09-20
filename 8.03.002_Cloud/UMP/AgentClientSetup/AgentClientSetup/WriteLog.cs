using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AgentClientSetup
{
    class WriteLog
    {
        public static string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                   string.Format("UMP/UMPS3104"));

        /// <summary>
        /// 将文本写入文件中保存
        /// </summary>
        private static void WriteTextToFile(string txt,string FileName)
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
            else
            {
                if (FileName.Substring(0, 1) == "A")
                {
                    File.Delete(WriteLog.FilePath + "\\" + FileName);
                    sw = File.CreateText(WriteLog.FilePath + "\\" + FileName);
                }
                else
                {
                    sw = new StreamWriter(finfo.OpenWrite());
                }                
            }

            sw.BaseStream.Seek(0, SeekOrigin.End);

            sw.WriteLine(txt);
            sw.Flush();
            sw.Close();
        }

        /// <summary>
        /// 安装日志
        /// </summary>
        public static void WriteLogToFile(string infohead,string msg)
        {
            StringBuilder sb=new StringBuilder();
            sb.Append("\t"+infohead + "\t" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.Append("\t");
            sb.Append(msg);
            sb.Append(System.Environment.NewLine);
            try
            {
                //string FileName = "InstallCertificate_"+DateTime.Now.ToString("yyyyMMdd") + ".log";
                string FileName = "InstallCertificate.log";
                WriteTextToFile(sb.ToString(), FileName);
            }
            catch (Exception ex)
            {
                //App.ShowExceptionMessage(ex.Message);
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

        /// <summary>
        ///保存服务器配置信息
        /// </summary>
        public static void CreatServerInfoXml(string serverIP,string port)
        {
            try
            {
                StringBuilder xmlSB = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n");
                xmlSB.Append("<ServerInfo>\r\n");
                xmlSB.AppendFormat("<ServerIP>{0}</ServerIP>\r\n", serverIP);
                xmlSB.AppendFormat("<Port>{0}</Port>\r\n", port);
                xmlSB.Append("</ServerInfo>");

                string FileName="AgentServerInfo.xml";
                WriteTextToFile(xmlSB.ToString(), FileName);
            }
            catch (Exception ex)
            {
                WriteLogToFile("Save Falied", "\t" + string.Format("{0} serverAdd:{1}  Port:{2}", ex.Message, serverIP, port));
            }
            WriteLogToFile("Save Sucessed", "\t" + string.Format("serverAdd:{0}  Port:{1}", serverIP, port));
        }

    }
}
