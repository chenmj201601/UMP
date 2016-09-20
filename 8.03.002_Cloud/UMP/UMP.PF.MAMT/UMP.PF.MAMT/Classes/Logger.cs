using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace UMP.PF.MAMT.Classes
{
    public class Logger
    {
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="strMsg"></param>
        public static void WriteLog(List<string> lstMsgs)
        {
            string strLogPath = GetLogPath();
            FileStream fs = new FileStream(strLogPath, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            for (int i = 0; i < lstMsgs.Count; i++)
            {
                sw.WriteLine(lstMsgs[i]);
            }
                
            sw.Close();
            fs.Close();
        }

        /// <summary>
        /// 创建日志文件
        /// </summary>
        /// <returns></returns>
        private static string GetLogPath()
        {
            string strLogPath = App.GStrLoginUserApplicationDataPath + "\\UMP.PF.MAMT\\Log" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + ".log";
            DirectoryInfo dir = new DirectoryInfo(App.GStrLoginUserApplicationDataPath);
            List<DirectoryInfo> dirChilds = dir.GetDirectories().ToList();
            DirectoryInfo dirConfig;
            //如果存在UMP.PF.MAMT文件夹
            if (dirChilds.Exists(p => p.Name == "UMP.PF.MAMT"))
            {
                dirConfig = new DirectoryInfo(App.GStrLoginUserApplicationDataPath + "\\UMP.PF.MAMT");
                List<FileInfo> files = dirConfig.GetFiles().ToList();
                //如果没有找到配置文件
                if (!files.Exists(p => p.Name == "Log"+DateTime.Now.Year.ToString() + DateTime.Now.Month + DateTime.Now.Day + ".log"))
                {
                    try
                    {
                        FileInfo fi = new FileInfo(strLogPath);
                        FileStream fs = fi.Create();
                        fs.Close();
                    }
                    catch(Exception ex)
                    {
                        string str = ex.Message;
                    }
                }
            }
            else
            {
                DirectoryInfo dirInfo = new DirectoryInfo(App.GStrLoginUserApplicationDataPath + "\\UMP.PF.MAMT");
                dirInfo.Create();
                FileStream fs = new FileStream(strLogPath, FileMode.Create);
                fs.Close();
            }
            return strLogPath;
        }
    }
}
