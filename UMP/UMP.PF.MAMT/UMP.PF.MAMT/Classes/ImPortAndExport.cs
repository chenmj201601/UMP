using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoiceCyber.EncryptionPrivate;
using System.Xml;
using System.IO;

namespace UMP.PF.MAMT.Classes
{
    public class ImPortAndExport
    {
        /// <summary>
        /// 尝试解密语言包文件(导入时用)
        /// </summary>
        /// <param name="AStrSourceFile"></param>
        /// <param name="AStrPassword"></param>
        /// <param name="AStrReturn"></param>
        /// <returns></returns>
        public static bool TryDecryptionLanguagePackage(string AStrSourceFile, string AStrPassword, ref string AStrReturn)
        {
            bool LBoolReturn = true;
            string LStrSourceBody = string.Empty;

            try
            {
                AStrReturn = string.Empty;
                LBoolReturn = VoiceCyberPrivate.DecryptFileToString(AStrSourceFile, AStrPassword, ref AStrReturn);
                if (LBoolReturn)
                {
                    LBoolReturn = IsXMLTypeFileOrString("", AStrReturn);
                    if (LBoolReturn)
                    {
                        LStrSourceBody = AStrReturn;
                        LBoolReturn = SourceIsLanguagePackage(LStrSourceBody, ref AStrReturn);
                        if (!LBoolReturn)
                        {
                            AStrReturn = "N013";
                        }
                        else
                        {
                        }
                    }
                    else
                    {
                        AStrReturn = "N012";
                    }
                }
                else
                {
                    AStrReturn = "N011";
                }
            }
            catch
            {
                LBoolReturn = false;
                AStrReturn = "N011";
            }
            return LBoolReturn;
        }

        private static bool IsXMLTypeFileOrString(string AStrSourceFile, string AStrSoureBody)
        {
            bool LBoolReturn = true;

            try
            {
                XmlDocument LXmlDocTableDataLoaded = new XmlDocument();

                if (!string.IsNullOrEmpty(AStrSoureBody))
                {
                    Stream LStreamXmlBody = new MemoryStream(Encoding.UTF8.GetBytes(AStrSoureBody));
                    LXmlDocTableDataLoaded.Load(LStreamXmlBody);
                }
            }
            catch { LBoolReturn = false; }
            return LBoolReturn;
        }

        private static bool SourceIsLanguagePackage(string AStrSoureBody, ref string AStrReturn)
        {
            bool LBoolReturn = true;
            string LStrTableName = string.Empty;
            string LStrLanguageCode = string.Empty;

            try
            {
                XmlDocument LXmlDocTableDataLoaded = new XmlDocument();
                Stream LStreamXmlBody = new MemoryStream(Encoding.UTF8.GetBytes(AStrSoureBody));
                LXmlDocTableDataLoaded.Load(LStreamXmlBody);
                XmlNode LXMLNodeTableDataRowsList = LXmlDocTableDataLoaded.SelectSingleNode("LanguageDataRowsList");
                LStrTableName = LXMLNodeTableDataRowsList.Attributes["TableName"].Value;
                LStrLanguageCode = LXMLNodeTableDataRowsList.Attributes["LanguageCode"].Value;
                AStrReturn = LStrLanguageCode;
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturn = ex.Message;
            }

            return LBoolReturn;
        }
    }
}
