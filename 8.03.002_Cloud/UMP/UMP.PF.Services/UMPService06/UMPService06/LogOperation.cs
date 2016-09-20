using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Threading;
using VoiceCyber.SharpZips.Zip;
using System.Configuration;


namespace UMPService06
{
    public class LogOperation
    {
        /// <summary>
        /// 日志压缩和回删
        /// </summary>
        public void LogCompressionAndDelete()
        {
            string strProgramDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            DateTime dtCurrTime;
            int iLogSaveTime = 0;   //日志保存时长  默认30天
            int iLogCompressTime = 0;   //压缩多久前的日志  默认24小时

            //得到配置文件里的压缩时间以及回删日志时间
            string strCompressLogHour = ConfigurationManager.AppSettings["CompressLogHour"] != null
       ? ConfigurationManager.AppSettings["CompressLogHour"] : "6";
            string strDeleteLogDay = ConfigurationManager.AppSettings["CompressLogDay"] != null
       ? ConfigurationManager.AppSettings["CompressLogDay"] : "15";

            try
            {
                while (true)
                {
                    //获得日志路径
                    string strLocalMachineDir = strProgramDataPath + "\\UMP\\UMPService06\\Log";
                    DirectoryInfo dir = new DirectoryInfo(strLocalMachineDir);
                    if (!dir.Exists)
                    {
                        Thread.Sleep(5 * 60 * 1000);
                        break;
                    }

                    //获得当前时间
                    dtCurrTime = DateTime.Now;

                    iLogCompressTime = IntParse(strCompressLogHour, 6);
                    iLogSaveTime = IntParse(strDeleteLogDay, 15);

                    LogFileOperate(dir, dtCurrTime, iLogSaveTime, iLogCompressTime);
                    Thread.Sleep(3 * 60 * 1000);
                }
            }
            catch (Exception ex)
            {
                FileLog.WriteError("LogCompressionAndDelete()", " error : " + ex.Message);
            }
        }

        public int IntParse(string str, int defaultValue)
        {
            int outRet = defaultValue;
            int.TryParse(str, out outRet);

            return outRet;
        }

        private void LogFileOperate(DirectoryInfo parentDir, DateTime dtCurrTime, int iLogSaveTime, int iLogCompressTime)
        {
            //处理parentDir下的文件（.txt和.zip）
            CompressFile(parentDir, iLogCompressTime, dtCurrTime);
            DeleteLogFile(parentDir, iLogSaveTime, dtCurrTime);

            //递归处理子文件夹
            foreach (DirectoryInfo childDir in parentDir.GetDirectories())
            {
                LogFileOperate(childDir, dtCurrTime, iLogSaveTime, iLogCompressTime);
            }
        }

        /// <summary>
        /// 删除到期的ZIP文件
        /// </summary>
        private void DeleteLogFile(DirectoryInfo dir, int iLogSaveTime, DateTime dtCurrTime)
        {
            List<FileInfo> lstFiles = dir.GetFiles("*.zip").ToList();
            if (lstFiles.Count > 0)
            {
                TimeSpan tspan;
                foreach (FileInfo file in lstFiles)
                {
                    tspan = dtCurrTime - file.LastWriteTime;
                    if (tspan.Days >= iLogSaveTime)
                    {
                        file.Delete();
                        Thread.Sleep(50);
                    }
                }
            }
        }

        /// <summary>
        /// 压缩到期的log文件 并删掉压缩完成对log文件
        /// </summary>
        private void CompressFile(DirectoryInfo dir, int iLogCompressTime, DateTime dtCurrTime)
        {
            List<FileInfo> lstFiles = dir.GetFiles("*.log").ToList();
            ZipOutputStream zipOutStream = null;
            string strZipFileName = string.Empty;
            TimeSpan tspan;
            foreach (FileInfo file in lstFiles)
            {
                try
                {
                    tspan = dtCurrTime - file.LastWriteTime;
                    if (tspan.TotalHours < iLogCompressTime)
                    {
                        continue;
                    }


                    strZipFileName = file.Name + ".zip";
                    zipOutStream = new ZipOutputStream(File.Create(System.IO.Path.Combine(file.DirectoryName, strZipFileName)));
                    zipOutStream.SetLevel(9);
                    byte[] LByteBuffer = new byte[1024];
                    ZipEntry LZipEntry = new ZipEntry(file.Name);
                    LZipEntry.DateTime = DateTime.UtcNow;
                    zipOutStream.PutNextEntry(LZipEntry);
                    using (FileStream LFileStream = File.OpenRead(file.FullName))
                    {
                        int LIntSourceBytes;
                        do
                        {
                            LIntSourceBytes = LFileStream.Read(LByteBuffer, 0, LByteBuffer.Length);
                            zipOutStream.Write(LByteBuffer, 0, LIntSourceBytes);
                        } while (LIntSourceBytes > 0);

                    }
                    zipOutStream.Finish(); zipOutStream.Close();
                    file.Delete();
                    Thread.Sleep(50);
                }
                catch (Exception ex)
                {
                  FileLog.WriteError("CompressFile()"," error : " + ex.Message);
                }
            }
        }

    }
}
