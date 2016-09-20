//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    72903131-b906-468b-bf8f-4c27f4eb1722
//        CLR Version:              4.0.30319.18444
//        Name:                     Utils
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common
//        File Name:                Utils
//
//        created by Charley at 2014/12/1 10:23:27
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.IO;
using System.Text;
using System.Windows.Media;
using VoiceCyber.Common;

namespace VoiceCyber.UMP.Common
{
    public class Utils
    {
        /// <summary>
        /// 格式化操作日志
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string FormatOptLogString(string code)
        {
            return string.Format("{0}<{0}{1}{0}>{0}", ConstValue.SPLITER_CHAR_2, code);
        }
        /// <summary>
        /// 获取两个日期相差的月份数
        /// </summary>
        /// <param name="time1"></param>
        /// <param name="time2"></param>
        /// <returns></returns>
        public static int GetTimeMonthCount(DateTime time1, DateTime time2)
        {
            if (time1 > time2)
            {
                throw new ArgumentException(string.Format("Input datetime invalid"));
            }
            int year1, year2, month1, month2;
            int year, month;
            year1 = time1.Year;
            year2 = time2.Year;
            month1 = time1.Month;
            month2 = time2.Month;
            year = year2 - year1;
            month = month2 - month1;
            return year * 12 + month;
        }
        /// <summary>
        /// 从十六进制的Rgb字符串获取响应的颜色，如: FF44DD
        /// </summary>
        /// <param name="strColor"></param>
        /// <returns></returns>
        public static Color GetColorFromRgbString(string strColor)
        {
            strColor = strColor.ToUpper();
            if (strColor.StartsWith("#"))
            {
                strColor = strColor.Substring(1);
            }
            string strA = "FF";
            if (strColor.Length >= 8)
            {
                strA = strColor.Substring(0, 2);
                strColor = strColor.Substring(2);
            }
            string strR = strColor.Substring(0, 2);
            string strG = strColor.Substring(2, 2);
            string strB = strColor.Substring(4, 2);
            return Color.FromArgb((byte)Convert.ToInt32(strA, 16), (byte)Convert.ToInt32(strR, 16),
                (byte)Convert.ToInt32(strG, 16), (byte)Convert.ToInt32(strB, 16));
        }
        /// <summary>
        /// 下载MediaUtils文件夹中的文件，如果已经下载过（存在FileList.txt文件）则跳过
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static OperationReturn DownloadMediaUtils(SessionInfo session)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //添加Path环境变量
                optReturn = SetMeidaUtilsPath();
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                //下载到本应用程序域
                optReturn = DownloadMediaUtilsDomain(session);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                //下载到公共临时目录
                optReturn = DownloadMediaUtilsCommon(session);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
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
        /// 将MediaUtils下载到本应用程序所在目录（应用程序域）
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        private static OperationReturn DownloadMediaUtilsDomain(SessionInfo session)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string strFileList =
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                        ConstValue.TEMP_FILE_FILELIST);
                if (File.Exists(strFileList))
                {
                    optReturn.Message = string.Format("WaveUtils already downloaded");
                    return optReturn;
                }
                string dir = AppDomain.CurrentDomain.BaseDirectory;
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                optReturn = DownloadMediaUtilFile(dir, ConstValue.TEMP_FILE_FILELIST, session);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                if (!File.Exists(strFileList))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FILE_NOT_EXIST;
                    optReturn.Message = string.Format("FileList file not exist.\t{0}", strFileList);
                    return optReturn;
                }
                string[] listFiles = File.ReadAllLines(strFileList, Encoding.UTF8);
                for (int i = 0; i < listFiles.Length; i++)
                {
                    string file = listFiles[i];
                    optReturn = DownloadMediaUtilFile(dir, file, session);
                    if (!optReturn.Result)
                    {
                        optReturn.Message = string.Format("Download file fail.\t{0}\r\n{1}\t{2}", file,
                            optReturn.Code, optReturn.Message);
                        return optReturn;
                    }
                }
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
        /// 将MediaUtils下载到公共临时目录
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        private static OperationReturn DownloadMediaUtilsCommon(SessionInfo session)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string strFileList =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                        ConstValue.TEMP_PATH_FILELIST);
                if (File.Exists(strFileList))
                {
                    optReturn.Message = string.Format("WaveUtils already downloaded");
                    return optReturn;
                }
                string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    ConstValue.TEMP_PATH_MEDIAUTILS);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                optReturn = DownloadMediaUtilFile(dir, ConstValue.TEMP_FILE_FILELIST, session);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                if (!File.Exists(strFileList))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FILE_NOT_EXIST;
                    optReturn.Message = string.Format("FileList file not exist.\t{0}", strFileList);
                    return optReturn;
                }
                string[] listFiles = File.ReadAllLines(strFileList, Encoding.UTF8);
                for (int i = 0; i < listFiles.Length; i++)
                {
                    string file = listFiles[i];
                    optReturn = DownloadMediaUtilFile(dir, file, session);
                    if (!optReturn.Result)
                    {
                        optReturn.Message = string.Format("Download file fail.\t{0}\r\n{1}\t{2}", file,
                            optReturn.Code, optReturn.Message);
                        return optReturn;
                    }
                }
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
        /// 下载MediaUtils中的单个文件
        /// </summary>
        /// <param name="dir">存放路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="session"></param>
        /// <returns></returns>
        private static OperationReturn DownloadMediaUtilFile(string dir, string fileName, SessionInfo session)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                string strSavePath = Path.Combine(dir, fileName);
                DownloadConfig config = new DownloadConfig();
                config.Method = session.AppServerInfo.SupportHttps ? 2 : 1;
                config.Host = session.AppServerInfo.Address;
                config.Port = session.AppServerInfo.Port;
                config.IsAnonymous = true;
                config.RequestPath = string.Format("{0}/{1}", ConstValue.TEMP_DIR_MEDIAUTILS, fileName);
                config.SavePath = strSavePath;
                optReturn = DownloadHelper.DownloadFile(config);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
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
        /// 将MediaUtils目录添加到当前用户的Path环境变量中
        /// </summary>
        /// <returns></returns>
        private static OperationReturn SetMeidaUtilsPath()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //string regEnvironmentPath = string.Format(@"SYSTEM\CurrentControlSet\Control\Session Manager\Environment");
                //optReturn = RegistryHelper.GetItemData(RegistryType.LocalMerchine, regEnvironmentPath, "Path");
                //if (!optReturn.Result)
                //{
                //    return optReturn;
                //}
                //string strWaveUtilPath =
                //    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                //        ConstValue.TEMP_PATH_MEDIAUTILS);
                //string strPath = optReturn.Data.ToString();
                //if (!strPath.ToLower().Contains(strWaveUtilPath.ToLower()))
                //{
                //    if (!strPath.EndsWith(";"))
                //    {
                //        strPath += string.Format(";{0};", strWaveUtilPath);
                //    }
                //    else
                //    {
                //        strPath += string.Format("{0};", strWaveUtilPath);
                //    }
                //    optReturn = RegistryHelper.WriteItemData(RegistryType.LocalMerchine, regEnvironmentPath, "Path",
                //        strPath);
                //    if (!optReturn.Result)
                //    {
                //        return optReturn;
                //    }
                //}

                string strWaveUtilPath =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                        ConstValue.TEMP_PATH_MEDIAUTILS);
                string paths = Environment.GetEnvironmentVariable("PATH");
                if (string.IsNullOrEmpty(paths))
                {
                    paths = string.Empty;
                }
                bool isAdd = true;
                string[] arrPaths = paths.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < arrPaths.Length; i++)
                {
                    string path = arrPaths[i];
                    if (path.ToLower() == strWaveUtilPath.ToLower())
                    {
                        isAdd = false;
                        break;
                    }
                }
                if (isAdd)
                {
                    paths = string.Format("{0}{1}", paths, paths.EndsWith(";") ? strWaveUtilPath : ";" + strWaveUtilPath);
                    Environment.SetEnvironmentVariable("PATH", paths);
                }
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
        /// 计算验证码
        /// </summary>
        /// <param name="byteSource"></param>
        /// <returns></returns>
        public static byte[] GetVerificationCode(byte[] byteSource)
        {
            int offset = byteSource[0] & 0x001f;
            byte byteMask = (byte)(byteSource[offset] ^ offset);
            byte[] byteVerify = new byte[ConstValue.NET_AUTH_CODE_SIZE];
            for (int i = 0; i < ConstValue.NET_AUTH_CODE_SIZE; i++)
            {
                byteVerify[i] = (byte)(byteSource[i] ^ byteMask);
            }
            return byteVerify;
        }
        /// <summary>
        /// 计算验证码
        /// </summary>
        /// <param name="strSource"></param>
        /// <returns></returns>
        public static string GetVerificationCode(string strSource)
        {
            byte[] byteSession = Converter.Hex2Byte(strSource);
            byte[] byteVerify = GetVerificationCode(byteSession);
            return Converter.Byte2Hex(byteVerify);
        }
    }
}
