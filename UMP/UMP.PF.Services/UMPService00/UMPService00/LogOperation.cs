using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.IO;
using System.Threading;
using System.Diagnostics;
using VoiceCyber.SharpZips.Zip;
using VoiceCyber.Common;

namespace UMPService00
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
            DateTime strLastModifyTime;
            int iLogSaveTime = 0;   //日志保存时长  默认30天
            int iLogCompressTime = 0;   //压缩多久前的日志  默认24小时
            try
            {
                while (true)
                {
                    //获得日志路径
                    string strLocalMachineDir = strProgramDataPath + "\\VoiceCyber\\UMP\\config";
                    DirectoryInfo dir = new DirectoryInfo(strLocalMachineDir);
                    if (!dir.Exists)
                    {
                        //如果没有C:\ProgramData\VoiceCyber\UMP\config文件夹  说明程序有误 因为这个文件夹在服务启动时生成
                        UMPService00.WriteLog(LogMode.Error, "Can not find the path : " + strLocalMachineDir);
                        break;
                    }
                    IniOperation ini = new IniOperation(strLocalMachineDir + "\\localmachine.ini");
                    string strLogPath = ini.IniReadValue("LocalMachine", "LogPath");
                    if (string.IsNullOrEmpty(strLogPath))
                    {
                        //如果没有配置日志路径 暂停5分钟
                        UMPService00.WriteLog(LogMode.Error, "No configuration log path , pause 5 minutes");
                        Thread.Sleep(5 * 60 * 1000);
                        continue;
                    }
                    //判断日志路径是否存在
                    dir = new DirectoryInfo(strLogPath);
                    if (!dir.Exists)
                    {
                        UMPService00.WriteLog(LogMode.Error, "Can not find the log file path : " + strLogPath + ", pause 5 minutes");
                        Thread.Sleep(5 * 60 * 1000);
                        continue;
                    }
                    GetLogParam(ref iLogSaveTime, ref iLogCompressTime);
                   // UMPService00.IEventLog.WriteEntry("iLogSaveTime = " + iLogSaveTime + " ; iLogCompressTime = " + iLogCompressTime, EventLogEntryType.Warning);
                    //获得当前时间
                    dtCurrTime = DateTime.Now;
                    LogFileOperate(dir, dtCurrTime, iLogSaveTime, iLogCompressTime);
                    UMPService00.WriteLog( "ok ,sleep 3 mins");
                    Thread.Sleep(3 * 60 * 1000);
                }
            }
            catch (Exception ex)
            {
                UMPService00.WriteLog(LogMode.Error, "LogCompressionAndDelete() error : " + ex.Message);
            }
           
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
                }
                catch (Exception ex)
                {
                    UMPService00.WriteLog("CompressFile() error : " + ex.Message);
                }
            }
        }

        private void GetLogParam(ref int iLogSaveTime, ref int iLogCompressTime)
        {
            try
            {
                string strProgramDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                //获得日志保存时间
                string strLogParamDirPath = strProgramDataPath + "\\VoiceServer\\UMP.Server";
                DirectoryInfo dir = new DirectoryInfo(strProgramDataPath);
                if (!dir.Exists)
                {
                    //在安装出错的情况下 会不存在 
                    //dir.Create();
                    iLogCompressTime = 24;
                    iLogSaveTime = 30;
                    return;
                }
                //从Args03.UMP.xml读取日志参数
                //XmlDocument xmlDoc = Common.CreateXmlDocumentIfNotExists(strLogParamDirPath, "Args03.UMP.xml", "Parameters03");
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(strLogParamDirPath + "\\Args03.UMP.xml");
                XMLOperator xmlOperator = new XMLOperator(xmlDoc);
                string strLogSaveTime = xmlOperator.SelectNodeText("Parameters03/LogConfig/SaveTime");
                if (string.IsNullOrEmpty(strLogSaveTime))
                {
                    iLogSaveTime = 30;
                }
                else
                {
                    int.TryParse(strLogSaveTime, out iLogSaveTime);
                    iLogSaveTime = iLogSaveTime == 0 ? 30 : iLogSaveTime;
                }
                string strLogCompressTime = xmlOperator.SelectNodeText("Parameters03/LogConfig/CompressTime");
                if (string.IsNullOrEmpty(strLogCompressTime))
                {
                    iLogCompressTime = 24;
                }
                else
                {
                    int.TryParse(strLogCompressTime, out iLogCompressTime);
                    iLogCompressTime = iLogCompressTime == 0 ? 24 : iLogCompressTime;
                }
            }
            catch (Exception ex)
            {
                iLogCompressTime = 24;
                iLogSaveTime = 30;
            }
        }

    }
}
