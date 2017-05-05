using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UMPService00
{
    class IniOperation
    {
        private string strPath = string.Empty;

        public IniOperation(string strFilePath)
        {
            strPath = strFilePath;
        }

        #region 导入操作Ini的dll
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defVal, Byte[] retVal, int size, string filePath);
        #endregion

        /// <summary>
        /// 写INI
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="iValue"></param>
        public void IniWriteValue(string section, string key, string iValue)
        {
            WritePrivateProfileString(section, key, iValue, this.strPath);
        }

        public string IniReadValue(string section, string key)
        {
            byte[] byteTemp = new byte[255];
            int i = GetPrivateProfileString(section, key, "", byteTemp, 255, this.strPath);
            return  Encoding.Default.GetString(byteTemp).Trim('\0');
        }

         public byte[] IniReadValues(string section, string key)
        {
            byte[] temp = new byte[255];
            int i = GetPrivateProfileString(section, key, "", temp, 255, this.strPath);
            return temp;
        }
    }
}
