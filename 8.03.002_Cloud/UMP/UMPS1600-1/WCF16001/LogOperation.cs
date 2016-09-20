using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace WCF16001
{
    public class LogOperation
    {
        public static void WriteLog(string strLog)
        {
            FileStream fs = null;
            StreamWriter sw = null;
            try
            {
                if (string.IsNullOrEmpty(strLog))
                {
                    strLog = " null";
                }
                strLog = string.Format("{0}    {1}", DateTime.Now.ToString(), strLog);
                string strFileName = DateTime.Now.Year.ToString() + DateTime.Now.Month + DateTime.Now.Day + ".log";
                string strDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\WCF1600";
                if (!Directory.Exists(strDir))
                {
                    Directory.CreateDirectory(strDir);
                }
                string strFilePath = strDir + "\\" + strFileName;
                if (File.Exists(strFilePath))
                {
                    fs = new FileStream(strFilePath, FileMode.Append);
                }
                else
                {
                    fs = new FileStream(strFilePath, FileMode.Create);
                }
                sw = new StreamWriter(fs);
                sw.WriteLine(strLog);
            }
            catch
            {

            }
            finally
            {
                sw.Flush();
                fs.Flush();
                sw.Close();
                fs.Close();
            }
        }
    }
}