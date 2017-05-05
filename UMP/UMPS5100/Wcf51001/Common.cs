using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using VoiceCyber.Common;
using Common5100;
using System.Threading;
using System.Net.Sockets;
using System.Net.Security;
using VoiceCyber.UMP.Common;
using System.Security.Authentication;
using System.Text;
using System.Security.Cryptography.X509Certificates;

namespace Wcf51001
{
    public class Common
    {
        /// <summary>
        /// 在文件不存在时创建xml文档 
        /// </summary>
        /// <param name="strPath">文件路径（如果没有 就创建）</param>
        /// <param name="strFileName">文件名</param>
        /// <param name="strRootEleName">根节点名</param>
        /// <returns></returns>
        public static OperationReturn CreateXmlDocumentIfNotExists(string strPath, string strFileName, string strRootEleName)
        {
            OperationReturn optReturn = new OperationReturn();
            DirectoryInfo dir = new DirectoryInfo(strPath);
            if (!dir.Exists)
            {
                dir.Create();
            }
            XmlDocument xmlDocument = null;
            try
            {
                List<string> lstFiles = Directory.GetFiles(strPath).ToList();
                string strXmlFilePath = strPath + @"\" + strFileName;
                if (lstFiles.Contains(strXmlFilePath))
                {
                    lstFiles.Remove(strXmlFilePath);
                }

                xmlDocument = new XmlDocument();
                XmlNode root = xmlDocument.CreateNode(XmlNodeType.XmlDeclaration, "", "");
                xmlDocument.AppendChild(root);
                XmlElement ele = xmlDocument.CreateElement(strRootEleName);
                xmlDocument.AppendChild(ele);
                xmlDocument.Save(strXmlFilePath);

                optReturn.Data = xmlDocument;
                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS; 
                optReturn.StringValue = strXmlFilePath;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = (int)S5100WcfErrorCode.CreateXmlError;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        /// <summary>
        /// 拷贝文件 尝试拷贝5次 通过比较最后修改时间来判断是否拷贝成功 每次拷贝间隔2秒
        /// </summary>
        /// <param name="strSrcDir">源文件路径</param>
        /// <param name="strTargetDir">目标文件路径</param>
        public static OperationReturn CopyFile(string strSrcDir, string strTargetDir)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            string strTarget = strTargetDir.Substring(0, strTargetDir.LastIndexOf('\\'));
            try
            {
                DirectoryInfo dir = new DirectoryInfo(strTarget);
                if (!dir.Exists)
                {
                    dir.Create();
                }

                FileInfo targetFile = null;
                FileInfo sourceFile = null;
                for (int i = 0; i < 5; i++)
                {
                    sourceFile = new FileInfo(strSrcDir);
                    targetFile = new FileInfo(strTargetDir);
                    if (targetFile.Exists)
                    {
                        if (sourceFile.LastWriteTime == targetFile.LastWriteTime)
                        {
                            break;
                        }
                        else
                        {
                            File.Copy(strSrcDir, strTargetDir, true);
                        }
                    }
                    else
                    {
                        File.Copy(strSrcDir, strTargetDir, true);
                    }
                    Thread.Sleep(2000);
                }

                sourceFile = new FileInfo(strSrcDir);
                targetFile = new FileInfo(strTargetDir);
                if (targetFile.Exists)
                {
                    if (targetFile.LastWriteTime == sourceFile.LastWriteTime)
                    {
                        File.Delete(strSrcDir);
                    }
                    else
                    {
                        File.Delete(strSrcDir);
                        optReturn.Result = false;
                        optReturn.Code = (int)S5100WcfErrorCode.UploadKeyWorldXmlFailed;
                        optReturn.Message = strTarget;
                    }
                }
                else
                {
                    File.Delete(strSrcDir);
                    optReturn.Result = false;
                    optReturn.Code = (int)S5100WcfErrorCode.UploadKeyWorldXmlFailed;
                    optReturn.Message = strTarget;
                }
            }
            catch 
            {
                optReturn.Code = (int)S5100WcfErrorCode.UploadKeyWorldXmlFailed;
                optReturn.Result = false;
                optReturn.Message = strTarget;
            }
            return optReturn;
        }

        public static OperationReturn UpLoadFile(string fileNamePath, string urlPath, string User, string Pwd)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;

            #region 用service00上传文件  失败
            //TcpClient LTcpClient = null;
            //SslStream LSslStream = null;
            //string LStrVerificationCode004 = string.Empty;
            //string LStrSendMessage = string.Empty;
            //string LStrReadMessage = string.Empty;
            //try
            //{
            //    LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
            //    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("E001", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
            //    LStrSendMessage += ConstValue.SPLITER_CHAR;
            //    LStrSendMessage += EncryptionAndDecryption.EncryptDecryptString(@"C:\ProgramData\VoiceCyber\UMP\KeyWorld\Keyword.xml", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
            //    LStrSendMessage += ConstValue.SPLITER_CHAR;
            //    LStrSendMessage += EncryptionAndDecryption.EncryptDecryptString(@"\\192.168.6.27\speech\pcm", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
            //    LStrSendMessage += ConstValue.SPLITER_CHAR;
            //    LStrSendMessage += EncryptionAndDecryption.EncryptDecryptString("administrator", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
            //    LStrSendMessage += ConstValue.SPLITER_CHAR;
            //    LStrSendMessage += EncryptionAndDecryption.EncryptDecryptString("voicecyber", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);

            //    LTcpClient = new TcpClient("127.0.0.1", 8009);
            //    LSslStream = new SslStream(LTcpClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
            //    LSslStream.AuthenticateAsClient("VoiceCyber.PF", null, SslProtocols.Default, false);
            //    byte[] LByteMesssage = Encoding.UTF8.GetBytes(LStrSendMessage + "\r\n");
            //    LSslStream.Write(LByteMesssage); LSslStream.Flush();
            //    if (ReadMessageFromServer(LSslStream, ref LStrReadMessage))
            //    {
            //        if (LStrReadMessage.TrimEnd("\r\n".ToCharArray()) == "OK")
            //        {
            //            optReturn.Code = Defines.RET_SUCCESS;
            //            optReturn.Result = true;
            //        }
            //        else
            //        {
            //            optReturn.Result = false;
            //            optReturn.Code = (int)S5100WcfErrorCode.UploadKeyWorldXmlFailed;
            //            optReturn.Message = LStrReadMessage;
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    optReturn.Result = false;
            //    optReturn.Code = (int)S5100WcfErrorCode.UploadKeyWorldXmlException;
            //    optReturn.Message = ex.Message;
            //}
            #endregion

            #region 用命令行上传文件  失败
            //bool status = false;
            ////连接共享文件夹
            //status = FileShare.connectState(urlPath,User,Pwd);
            //if (status)
            //{
            //    try
            //    {
            //        DirectoryInfo theFolder = new DirectoryInfo(urlPath);
            //        string filename = theFolder.ToString() + "\\Keyword.xml";
            //        optReturn = FileShare.WriteFiles(filename);
            //    }
            //    catch (Exception ex)
            //    {
            //        optReturn.Result = false;
            //        optReturn.Code = (int)S5100WcfErrorCode.UploadKeyWorldXmlFailed;
            //        optReturn.Message = ex.Message;
            //    }
            //}
            #endregion

            #region 用webclient上传文件 失败
            //string newFileName = fileNamePath.Substring(fileNamePath.LastIndexOf(@"\") + 1);//取文件名称
            //if (urlPath.EndsWith(@"\") == false) urlPath = urlPath + @"\";
            //if (!Directory.Exists(urlPath))
            //{
              
            //    try
            //    {
            //        Directory.CreateDirectory(urlPath);
            //    }
            //    catch (Exception ex)
            //    {
            //        optReturn.Result = false;
            //        optReturn.Code = 123456;
            //        optReturn.Message = ex.Message;
            //        return optReturn;
            //    }
            //}
            //urlPath = urlPath + newFileName;

            //System.Net.WebClient myWebClient = new System.Net.WebClient();
            //System.Net.NetworkCredential cread = new System.Net.NetworkCredential(User, Pwd, "Domain");
            //myWebClient.Credentials = cread;
            //byte[] bytes = myWebClient.UploadFile(urlPath, fileNamePath);
            //int icount = bytes.Length;

            //FileStream fs = new FileStream(fileNamePath, FileMode.Open, FileAccess.Read);
            //BinaryReader r = new BinaryReader(fs);

            //try
            //{
            //    byte[] postArray = r.ReadBytes((int)fs.Length);
            //    Stream postStream = myWebClient.OpenWrite(urlPath);
            //    if (postStream.CanWrite)
            //    {
            //        postStream.Write(postArray, 0, postArray.Length);
            //        optReturn.Result = true;
            //        optReturn.Code = Defines.RET_SUCCESS;
            //    }
            //    else
            //    {
            //        optReturn.Result = false;
            //        optReturn.Code = (int)S5100WcfErrorCode.UploadKeyWorldXmlFailed;
            //    }

            //    postStream.Close();
            //}
            //catch (Exception ex)
            //{
            //    optReturn.Result = false;
            //    optReturn.Code = (int)S5100WcfErrorCode.UploadKeyWorldXmlToShareException;
            //    optReturn.Message = ex.Message;
            //}
            #endregion

            #region mpr.dll上传文件
            UploadFile.NETRESOURCE[] res = new UploadFile.NETRESOURCE[1];
            res[0].dwType = UploadFile.RESOURCE_TYPE.RESOURCETYPE_DISK;
            res[0].lpLocalName = "";
            res[0].lpRemoteName=urlPath;
            int iResult = UploadFile.WNetAddConnection2A(res, Pwd, User, 1);
            if (iResult == (int)UploadFile.ERROR_ID.ERROR_SUCCESS)
            {
                string newFileName = fileNamePath.Substring(fileNamePath.LastIndexOf(@"\") + 1);//取文件名称
                if (urlPath.EndsWith(@"\") == false) urlPath = urlPath + @"\";
                urlPath = urlPath + newFileName;

                FileStream fs = new FileStream(fileNamePath, FileMode.Open, FileAccess.Read);
                BinaryReader r = new BinaryReader(fs);

                try
                {
                    byte[] postArray = r.ReadBytes((int)fs.Length);
                    Stream postStream = new FileStream(urlPath, FileMode.OpenOrCreate);
                    if (postStream.CanWrite)
                    {
                        postStream.Write(postArray, 0, postArray.Length);
                        optReturn.Result = true;
                        optReturn.Code = Defines.RET_SUCCESS;
                    }
                    else
                    {
                        optReturn.Result = false;
                        optReturn.Code = (int)S5100WcfErrorCode.WriteFileError;
                    }

                    postStream.Close();
                    fs.Close();
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = (int)S5100WcfErrorCode.UploadKeyWorldXmlToShareException;
                    optReturn.Message = ex.Message;
                }
              //  iResult = UploadFile.WNetCancelConnection2A("", 1, 1);
                //if (iResult != (int)UploadFile.ERROR_ID.ERROR_SUCCESS)
                //{
                //    optReturn.Result = false;
                //    optReturn.Code = (int)S5100WcfErrorCode.ConnectCancelError;
                //    optReturn.Message = "Result = " + iResult.ToString();
                //}
            }
            else
            {
                optReturn.Result = false;
                optReturn.Code = (int)S5100WcfErrorCode.UploadKeyWorldXmlFailed;
                optReturn.Message = "Result = " + iResult.ToString();
            }
            #endregion
            return optReturn;
        }
      

    }
}