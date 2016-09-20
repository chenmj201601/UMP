//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d1207705-27f9-4ab1-b242-b7897c94c1c4
//        CLR Version:              4.0.30319.18444
//        Name:                     DownloadHelper
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Codes
//        File Name:                DownloadHelper
//
//        created by Charley at 2015/3/30 17:48:58
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.IO;
using System.Net;
using System.Text;
using VoiceCyber.Common;

namespace UMPS3102.Codes
{
    /// <summary>
    /// 下载帮助类
    /// </summary>
    public class DownloadHelper
    {
        /// <summary>
        /// HTTP方式下载文件
        /// </summary>
        /// <param name="parameters">
        /// 下载参数
        /// 0：协议名（http或https）
        /// 1：服务器地址
        /// 2：端口
        /// 3：登录名(如果为空，则匿名下载）
        /// 4：登录密码
        /// 5：源文件路径
        /// 6：存放路径
        /// 7: 如果目标文件已经存在，是否替换文件（可选）
        /// </param>
        /// <returns></returns>
        public static OperationReturn DownloadFileHTTP(string[] parameters)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            try
            {
                if (parameters == null || parameters.Length < 7)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Download parameter is null or length invalid.");
                    return optReturn;
                }
                string strTargetFile = parameters[6];
                string strTargetDir = strTargetFile.Substring(0, strTargetFile.LastIndexOf("\\"));
                if (!Directory.Exists(strTargetDir))
                {
                    Directory.CreateDirectory(strTargetFile);
                }
                if (File.Exists(strTargetFile))
                {
                    if (parameters.Length > 7 && parameters[7] == "1")
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
                string strRequest = string.Format("{0}://{1}:{2}/{3}", parameters[0], parameters[1], parameters[2], parameters[5]);
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(strRequest);
                if (!string.IsNullOrEmpty(parameters[3]))
                {
                    httpWebRequest.Credentials = new NetworkCredential(parameters[3], parameters[4]);
                    string code = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", parameters[3], parameters[4])));
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
    }
}
