//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    beaa7167-1dd4-423a-89b1-a824572332ea
//        CLR Version:              4.0.30319.18063
//        Name:                     DownloadHelper
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Common
//        File Name:                DownloadHelper
//
//        created by Charley at 2014/3/22 17:55:05
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.IO;
using System.Net;
using System.Text;

namespace VoiceCyber.Common
{
    /// <summary>
    /// 文件下载帮助类，支持http和ftp下载 
    /// 这是一个静态类
    /// </summary>
    public static class DownloadHelper
    {
        /// <summary>
        /// FTP方式下载文件
        /// </summary>
        /// <param name="parameters">
        /// 下载参数
        /// 0：服务器地址
        /// 1：端口
        /// 2：登录名(如果为空，则匿名下载）
        /// 3：登录密码
        /// 4：源文件路径
        /// 5：存放路径
        /// 6: 如果目标文件已经存在，是否替换文件（可选）
        /// </param>
        /// <returns></returns>
        public static OperationReturn DownloadFileFTP(string[] parameters)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            try
            {
                if (parameters == null || parameters.Length < 6)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Download parameter is null or length invalid.");
                    return optReturn;
                }
                string strTargetFile = parameters[5];
                string strTargetDir = strTargetFile.Substring(0, strTargetFile.LastIndexOf("\\"));
                if (!Directory.Exists(strTargetDir))
                {
                    Directory.CreateDirectory(strTargetFile);
                }
                if (File.Exists(strTargetFile))
                {
                    if (parameters.Length > 6 && parameters[6] == "1")
                    {
                        File.Delete(strTargetFile);
                    }
                    else
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_FILE_ALREADY_EXIST;
                        optReturn.Message = string.Format("Target file already exist.\t{0}", strTargetFile);
                        return optReturn;
                    }
                }
                string strRequest = string.Format("ftp://{0}:{1}/{2}", parameters[0], parameters[1], parameters[4]);
                FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(strRequest);
                if (!string.IsNullOrEmpty(parameters[2]))
                {
                    ftpWebRequest.Credentials = new NetworkCredential(parameters[2], parameters[3]);
                    string code = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", parameters[2], parameters[3])));
                    ftpWebRequest.Headers.Add("Authorization", "Basic" + code);
                }
                WebResponse ftpWebResponse = ftpWebRequest.GetResponse();
                Stream stream = ftpWebResponse.GetResponseStream();
                if (stream == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Response stream is null");
                    return optReturn;
                }
                using (FileStream fileStream = new FileStream(strTargetFile, FileMode.Create, FileAccess.Write))
                {
                    byte[] buffer = new byte[1024];
                    int readed = stream.Read(buffer, 0, 1024);
                    while (readed > 0)
                    {
                        fileStream.Write(buffer, 0, readed);
                        readed = stream.Read(buffer, 0, 1024);
                    }
                    fileStream.Close();
                    stream.Close();
                }
                optReturn.StringValue = strTargetFile;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }
        /// <summary>
        /// HTTP方式下载文件
        /// </summary>
        /// <param name="parameters">
        /// 下载参数
        /// 0：服务器地址
        /// 1：端口
        /// 2：登录名(如果为空，则匿名下载）
        /// 3：登录密码
        /// 4：源文件路径
        /// 5：存放路径
        /// 6: 如果目标文件已经存在，是否替换文件（可选）
        /// </param>
        /// <returns></returns>
        public static OperationReturn DownloadFileHTTP(string[] parameters)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            try
            {
                if (parameters == null || parameters.Length < 6)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Download parameter is null or length invalid.");
                    return optReturn;
                }
                string strTargetFile = parameters[5];
                string strTargetDir = strTargetFile.Substring(0, strTargetFile.LastIndexOf("\\"));
                if (!Directory.Exists(strTargetDir))
                {
                    Directory.CreateDirectory(strTargetFile);
                }
                if (File.Exists(strTargetFile))
                {
                    if (parameters.Length > 6 && parameters[6] == "1")
                    {
                        File.Delete(strTargetFile);
                    }
                    else
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_FILE_ALREADY_EXIST;
                        optReturn.Message = string.Format("Target file already exist.\t{0}", strTargetFile);
                        return optReturn;
                    }
                }
                string strRequest = string.Format("http://{0}:{1}/{2}", parameters[0], parameters[1], parameters[4]);
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(strRequest);
                if (!string.IsNullOrEmpty(parameters[2]))
                {
                    httpWebRequest.Credentials = new NetworkCredential(parameters[2], parameters[3]);
                    string code = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", parameters[2], parameters[3])));
                    httpWebRequest.Headers.Add("Authorization", "Basic" + code);
                }
                WebResponse httpWebResponse = httpWebRequest.GetResponse();
                Stream stream = httpWebResponse.GetResponseStream();
                if (stream == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Response stream is null");
                    return optReturn;
                }
                using (FileStream fileStream = new FileStream(strTargetFile, FileMode.Create, FileAccess.Write))
                {
                    byte[] buffer = new byte[1024];
                    int readed = stream.Read(buffer, 0, 1024);
                    while (readed > 0)
                    {
                        fileStream.Write(buffer, 0, readed);
                        readed = stream.Read(buffer, 0, 1024);
                    }
                    fileStream.Close();
                    stream.Close();
                }
                optReturn.StringValue = strTargetFile;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }
        /// <summary>
        /// 从远程服务器下载文件
        /// </summary>
        /// <param name="config">下载参数</param>
        /// <returns></returns>
        public static OperationReturn DownloadFile(DownloadConfig config)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (config == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("DownloadConfig is null");
                    return optReturn;
                }
                string strTargetPath = config.SavePath;
                string strTargetDir = strTargetPath.Substring(0, strTargetPath.LastIndexOf("\\"));
                if (!Directory.Exists(strTargetDir))
                {
                    //如果目录不存在，创建目录
                    Directory.CreateDirectory(strTargetDir);
                }
                if (File.Exists(strTargetPath))
                {
                    if (config.IsReplace)
                    {
                        //如果文件存在，删除已经存在的文件
                        File.Delete(strTargetPath);
                    }
                    else
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_FILE_ALREADY_EXIST;
                        optReturn.Message = string.Format("Target file already exist.\t{0}", strTargetPath);
                        return optReturn;
                    }
                }
                WebRequest webRequest;
                string strRequest;
                switch (config.Method)
                {
                    case 0:
                    case 1:
                        strRequest = string.Format("http://{0}:{1}/{2}", config.Host, config.Port, config.RequestPath);
                        HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(strRequest);
                        if (!config.IsAnonymous)
                        {
                            //如果需要验证身份
                            httpRequest.Credentials = new NetworkCredential(config.LoginName, config.Password);
                            string code = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", config.LoginName, config.Password)));
                            httpRequest.Headers.Add("Authorization", "Basic" + code);
                        }
                        webRequest = httpRequest;
                        break;
                    case 2:
                        strRequest = string.Format("https://{0}:{1}/{2}", config.Host, config.Port, config.RequestPath);
                        HttpWebRequest httpsRequest = (HttpWebRequest)WebRequest.Create(strRequest);
                        if (!config.IsAnonymous)
                        {
                            httpsRequest.Credentials = new NetworkCredential(config.LoginName, config.Password);
                            string code = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", config.LoginName, config.Password)));
                            httpsRequest.Headers.Add("Authorization", "Basic" + code);
                        }
                        webRequest = httpsRequest;
                        break;
                    case 11:
                        strRequest = string.Format("ftp://{0}:{1}/{2}", config.Host, config.Port, config.RequestPath);
                        FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(strRequest);
                        if (!config.IsAnonymous)
                        {
                            ftpRequest.Credentials = new NetworkCredential(config.LoginName, config.Password);
                            string code = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", config.LoginName, config.Password)));
                            ftpRequest.Headers.Add("Authorization", "Basic" + code);
                        }
                        webRequest = ftpRequest;
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Method invalid\t{0}", config.Method);
                        return optReturn;
                }
                WebResponse httpWebResponse = webRequest.GetResponse();
                Stream stream = httpWebResponse.GetResponseStream();
                if (stream == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Response stream is null");
                    return optReturn;
                }
                using (FileStream fileStream = new FileStream(strTargetPath, FileMode.Create, FileAccess.Write))
                {
                    byte[] buffer = new byte[1024];
                    int readed = stream.Read(buffer, 0, 1024);
                    while (readed > 0)
                    {
                        fileStream.Write(buffer, 0, readed);
                        readed = stream.Read(buffer, 0, 1024);
                    }
                    fileStream.Close();
                    stream.Close();
                }
                optReturn.StringValue = strTargetPath;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }
    }

    /// <summary>
    /// 下载参数
    /// </summary>
    public class DownloadConfig
    {
        /// <summary>
        /// 下载方式
        /// 1       http
        /// 2       https
        /// 11      ftp
        /// </summary>
        public int Method { get; set; }
        /// <summary>
        /// 服务器地址
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 是否匿名下载，默认为True
        /// </summary>
        public bool IsAnonymous { get; set; }
        /// <summary>
        /// 登录名（IsAnonymouse为False才有效）
        /// </summary>
        public string LoginName { get; set; }
        /// <summary>
        /// 登录密码（IsAnonymouse为False才有效）
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 请求的路径
        /// </summary>
        public string RequestPath { get; set; }
        /// <summary>
        /// 存放的路径
        /// </summary>
        public string SavePath { get; set; }
        /// <summary>
        /// 如果文件存在，是否替换，默认True
        /// </summary>
        public bool IsReplace { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DownloadConfig()
        {
            IsAnonymous = true;
            IsReplace = true;
        }
    }
}
