using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using VoiceCyber.Common;

namespace UMPService03
{
    public class FtpHelper
    {
        public static OperationReturn DownLoadFile_FTP(FtpHelperConfig ftpconfig)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;

            FtpWebRequest reqFTP;
            try
            {
                string sourceFile = string.Format("ftp://{0}:{1}/", ftpconfig.ServerHost, ftpconfig.ServerPort) + ftpconfig.SourceFilePath;

                FileStream outputStream = new FileStream(ftpconfig.TargetFilePath, FileMode.Create);
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(sourceFile));
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                reqFTP.KeepAlive = false;
                reqFTP.Credentials = new NetworkCredential(ftpconfig.LoginUser, ftpconfig.LoginPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                long cl = response.ContentLength;
                int bufferSize = 1024;
                int readCount;
                byte[] buffer = new byte[bufferSize];
                readCount = ftpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }
                ftpStream.Close();
                outputStream.Close();
                response.Close();
                return optReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                return optReturn;
            }
        }
    }

    /// <summary>
    /// Ftp参数配置
    /// </summary>
    public class FtpHelperConfig
    {
        /// <summary>
        /// ftp服务器地址
        /// </summary>
        public string ServerHost { get; set; }
        /// <summary>
        /// ftp服务器端口
        /// </summary>
        public int ServerPort { get; set; }
        /// <summary>
        /// 登录用户名
        /// </summary>
        public string LoginUser { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>
        public string LoginPassword { get; set; }
        /// <summary>
        /// 源文件路径 ftp://{0}:{1}/SourceFilePath
        /// </summary>
        public string SourceFilePath { get; set; }
        /// <summary>
        /// 录音流水号 C002
        /// </summary>
        public string RecordRef { get; set; }
        /// <summary>
        /// 本地存储路径
        /// </summary>
        public string TargetFilePath { get; set; }
    }
}
