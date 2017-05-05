using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VoiceCyber.Common;
using VoiceCyber.SharpZips.Checksums;
using VoiceCyber.SharpZips.Zip;

namespace UMPServicePackCommon
{
    public class ZipHelper
    {
        #region 不可用的压缩代码 压缩出来的路径不对 
        /// <summary>   
        /// 递归压缩文件夹的内部方法   
        /// </summary>   
        /// <param name="folderToZip">要压缩的文件夹路径</param>   
        /// <param name="zipStream">压缩输出流</param>   
        /// <param name="parentFolderName">此文件夹的上级文件夹</param>   
        /// <returns></returns>   
        //private static OperationReturn ZipDirectory(string folderToZip, ZipOutputStream zipStream, string parentFolderName)
        //{
        //    OperationReturn optReturn = new OperationReturn();
        //    optReturn.Code = Defines.RET_SUCCESS;
        //    optReturn.Result = true;

        //    bool result = true;
        //    string[] folders, files;
        //    ZipEntry ent = null;
        //    FileStream fs = null;
        //    Crc32 crc = new Crc32();

        //    try
        //    {
        //        ent = new ZipEntry(Path.Combine(parentFolderName, Path.GetFileName(folderToZip) + "/"));
        //        zipStream.PutNextEntry(ent);
        //        zipStream.Flush();

        //        files = Directory.GetFiles(folderToZip);
        //        foreach (string file in files)
        //        {
        //            fs = File.OpenRead(file);

        //            byte[] buffer = new byte[fs.Length];
        //            fs.Read(buffer, 0, buffer.Length);
        //            ent = new ZipEntry(Path.Combine(parentFolderName, Path.GetFileName(folderToZip) + "/" + Path.GetFileName(file)));
        //            ent.DateTime = DateTime.Now;
        //            ent.Size = fs.Length;

        //            fs.Close();

        //            crc.Reset();
        //            crc.Update(buffer);

        //            ent.Crc = crc.Value;
        //            zipStream.PutNextEntry(ent);
        //            zipStream.Write(buffer, 0, buffer.Length);
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        optReturn.Result = false;
        //        optReturn.Code = ConstDefines.Compress_Exception;
        //        optReturn.Message = ex.Message;
        //    }
        //    finally
        //    {
        //        if (fs != null)
        //        {
        //            fs.Close();
        //            fs.Dispose();
        //        }
        //        if (ent != null)
        //        {
        //            ent = null;
        //        }
        //        GC.Collect();
        //        GC.Collect(1);
        //    }

        //    folders = Directory.GetDirectories(folderToZip);
        //    foreach (string folder in folders)
        //    {
        //        optReturn = ZipDirectory(folder, zipStream, folderToZip);
        //    }

        //    return optReturn;
        //}

        /// <summary>   
        /// 压缩文件夹    
        /// </summary>   
        /// <param name="folderToZip">要压缩的文件夹路径</param>   
        /// <param name="zipedFile">压缩文件完整路径</param>   
        /// <param name="password">密码</param>   
        ///// <returns>是否压缩成功</returns>   
        //public static OperationReturn ZipDirectory(string folderToZip, string zipedFile, string password)
        //{
        //    OperationReturn optReturn = new OperationReturn();
        //    try
        //    {
        //        optReturn.Result = true;
        //        optReturn.Code = Defines.RET_SUCCESS;

        //        bool result = false;
        //        if (!Directory.Exists(folderToZip))
        //        {
        //            optReturn.Result = false;
        //            optReturn.Code = ConstDefines.Compress_Src_Not_Found;
        //            optReturn.Message = folderToZip;
        //            return optReturn;
        //        }
        //        if(File.Exists(zipedFile))
        //        {
        //            //如果文件存在 先重命名
        //            string strNewName = zipedFile.Substring(0, zipedFile.LastIndexOf(".zip"));
        //            FileInfo fileInfo = new FileInfo(zipedFile);
        //            string strLastModifyTime = fileInfo.LastWriteTime.Year.ToString() + fileInfo.LastWriteTime.Month.ToString()
        //               + fileInfo.LastWriteTime.Day.ToString() + fileInfo.LastWriteTime.Hour.ToString()
        //               + fileInfo.LastWriteTime.Minute.ToString() + fileInfo.LastWriteTime.Second.ToString();
        //            strNewName += strLastModifyTime + ".zip";
        //            File.Move(zipedFile, strNewName);
        //        }
        //        ZipOutputStream zipStream = new ZipOutputStream(File.Create(zipedFile));
        //        zipStream.SetLevel(6);
        //        if (!string.IsNullOrEmpty(password)) zipStream.Password = password;

        //        optReturn = ZipDirectory(folderToZip, zipStream, "");

        //        zipStream.Finish();
        //        zipStream.Close();

        //    }
        //    catch (Exception ex)
        //    {
        //        optReturn.Result = false;
        //        optReturn.Code = ConstDefines.Compress_Exception;
        //        optReturn.Message = ex.Message;
        //    }
        //    return optReturn;
        //}

        ///// <summary>   
        ///// 压缩文件夹   
        ///// </summary>   
        ///// <param name="folderToZip">要压缩的文件夹路径</param>   
        ///// <param name="zipedFile">压缩文件完整路径</param>   
        ///// <returns>是否压缩成功</returns>   
        //public static OperationReturn ZipDirectory(string folderToZip, string zipedFile)
        //{
        //    OperationReturn optReturn = ZipDirectory(folderToZip, zipedFile, null);
        //    return optReturn;
        //}

        ///// <summary>   
        ///// 压缩文件   
        ///// </summary>   
        ///// <param name="fileToZip">要压缩的文件全名</param>   
        ///// <param name="zipedFile">压缩后的文件名</param>   
        ///// <param name="password">密码</param>   
        ///// <returns>压缩结果</returns>   
        //public static OperationReturn ZipFile(string fileToZip, string zipedFile, string password)
        //{
        //    OperationReturn optReturn = new OperationReturn();
        //    optReturn.Result = true;
        //    optReturn.Code = Defines.RET_SUCCESS;

        //    ZipOutputStream zipStream = null;
        //    FileStream fs = null;
        //    ZipEntry ent = null;

        //    if (!File.Exists(fileToZip))
        //    {
        //        optReturn.Result = false;
        //        optReturn.Code = ConstDefines.Compress_Src_Not_Found;
        //        return optReturn;
        //    }

        //    try
        //    {
        //        fs = File.OpenRead(fileToZip);
        //        byte[] buffer = new byte[fs.Length];
        //        fs.Read(buffer, 0, buffer.Length);
        //        fs.Close();

        //        fs = File.Create(zipedFile);
        //        zipStream = new ZipOutputStream(fs);
        //        if (!string.IsNullOrEmpty(password)) zipStream.Password = password;
        //        ent = new ZipEntry(Path.GetFileName(fileToZip));
        //        zipStream.PutNextEntry(ent);
        //        zipStream.SetLevel(6);

        //        zipStream.Write(buffer, 0, buffer.Length);

        //    }
        //    catch(Exception ex)
        //    {
        //        optReturn.Result = false;
        //        optReturn.Code = ConstDefines.Compress_Exception;
        //        optReturn.Message = ex.Message;
        //    }
        //    finally
        //    {
        //        if (zipStream != null)
        //        {
        //            zipStream.Finish();
        //            zipStream.Close();
        //        }
        //        if (ent != null)
        //        {
        //            ent = null;
        //        }
        //        if (fs != null)
        //        {
        //            fs.Close();
        //            fs.Dispose();
        //        }
        //    }
        //    GC.Collect();
        //    GC.Collect(1);

        //    return optReturn;
        //}

        ///// <summary>   
        ///// 压缩文件   
        ///// </summary>   
        ///// <param name="fileToZip">要压缩的文件全名</param>   
        ///// <param name="zipedFile">压缩后的文件名</param>   
        ///// <returns>压缩结果</returns>   
        //public static OperationReturn ZipFile(string fileToZip, string zipedFile)
        //{
        //    OperationReturn optReturn = ZipFile(fileToZip, zipedFile, null);
        //    return optReturn;
        //}

        ///// <summary>   
        ///// 压缩文件或文件夹   
        ///// </summary>   
        ///// <param name="fileToZip">要压缩的路径</param>   
        ///// <param name="zipedFile">压缩后的文件名</param>   
        ///// <param name="password">密码</param>   
        ///// <returns>压缩结果</returns>   
        //public static OperationReturn Zip(string fileToZip, string zipedFile, string password)
        //{
        //    OperationReturn optReturn = new OperationReturn();
        //    optReturn.Result = true;
        //    optReturn.Code = Defines.RET_SUCCESS;

        //    if (Directory.Exists(fileToZip))
        //    {
        //        optReturn = ZipDirectory(fileToZip, zipedFile, password);
        //    }
        //    else if (File.Exists(fileToZip))
        //    {
        //        optReturn = ZipFile(fileToZip, zipedFile, password);
        //    }

        //    return optReturn;
        //}

        ///// <summary>   
        ///// 压缩文件或文件夹   
        ///// </summary>   
        ///// <param name="fileToZip">要压缩的路径</param>   
        ///// <param name="zipedFile">压缩后的文件名</param>   
        ///// <returns>压缩结果</returns>   
        //public static OperationReturn Zip(string fileToZip, string zipedFile)
        //{
        //    OperationReturn optReturn= Zip(fileToZip, zipedFile, null);
        //    return optReturn;
        //}
        #endregion

        #region 正确的压缩代码
         /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="destinationZipFilePath"></param>
        public static OperationReturn CreateZip(string sourceFilePath, string destinationZipFilePath)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;

            ZipOutputStream zipStream=null;
            try
            {
                if (sourceFilePath[sourceFilePath.Length - 1] != System.IO.Path.DirectorySeparatorChar)
                    sourceFilePath += System.IO.Path.DirectorySeparatorChar;
                zipStream = new ZipOutputStream(File.Create(destinationZipFilePath));
                zipStream.SetLevel(6);  // 压缩级别 0-9
                CreateZipFiles(sourceFilePath, zipStream);
              
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            finally
            {
                if (zipStream != null)
                {
                    zipStream.Finish();
                    zipStream.Close();
                }
            }
            return optReturn;
        }
        /// <summary>
        /// 递归压缩文件
        /// </summary>
        /// <param name="sourceFilePath">待压缩的文件或文件夹路径</param>
        /// <param name="zipStream">打包结果的zip文件路径（类似 D:\WorkSpace\a.zip）,全路径包括文件名和.zip扩展名
        ///</param>
        /// <param name="staticFile"></param>
        private static void CreateZipFiles(string sourceFilePath, ZipOutputStream zipStream)
        {
            Crc32 crc = new Crc32();
            string[] filesArray = Directory.GetFileSystemEntries(sourceFilePath);
            foreach (string file in filesArray)
            {
                if (Directory.Exists(file))                     //如果当前是文件夹，递归
                {
                    CreateZipFiles(file, zipStream);
                }
                else                                            //如果是文件，开始压缩
                {
                    FileStream fileStream = File.OpenRead(file);
                    byte[] buffer = new byte[fileStream.Length];
                    fileStream.Read(buffer, 0, buffer.Length);
                    string tempFile = file.Substring(sourceFilePath.LastIndexOf("\\") + 1);
                    ZipEntry entry = new ZipEntry(tempFile);
                    entry.DateTime = DateTime.Now;
                    entry.Size = fileStream.Length;
                    fileStream.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    zipStream.PutNextEntry(entry);
                    zipStream.Write(buffer, 0, buffer.Length);
                }
            }
        }

        /// <summary>
        /// 压缩多层目录
        /// </summary>
        /// <param name="strDirectory">The directory.</param>
        /// <param name="zipedFile">The ziped file.</param>
        /// <param name="password"></param>
        public static OperationReturn ZipFileDirectory(string strDirectory, string zipedFile, string password)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            try
            {
                using (var ZipFile = File.Create(zipedFile))
                {
                    using (ZipOutputStream s = new ZipOutputStream(ZipFile))
                    {
                        s.Password = password;
                        s.SetLevel(9);
                        ZipSetp(strDirectory, s, "");
                    }
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = ConstDefines.Compress_Exception;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        /// <summary>
        /// 递归遍历目录
        /// </summary>
        /// <param name="strDirectory">The directory.</param>
        /// <param name="s">The ZipOutputStream Object.</param>
        /// <param name="parentPath">The parent path.</param>
        private static void ZipSetp(string strDirectory, ZipOutputStream s, string parentPath)
        {
            if (strDirectory[strDirectory.Length - 1] != Path.DirectorySeparatorChar)
            {
                strDirectory += Path.DirectorySeparatorChar;
            }
            Crc32 crc = new Crc32();
            string[] filenames = Directory.GetFileSystemEntries(strDirectory);
            foreach (string file in filenames) // 遍历所有的文件和目录
            {
                if (Directory.Exists(file)) // 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                {
                    string pPath = parentPath;
                    pPath += file.Substring(file.LastIndexOf("\\") + 1);
                    pPath += "\\";
                    ZipSetp(file, s, pPath);
                }
                else // 否则直接压缩文件
                {
                    //打开压缩文件
                    using (FileStream fs = File.OpenRead(file))
                    {
                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);
                        string fileName = parentPath + file.Substring(file.LastIndexOf("\\") + 1);
                        ZipEntry entry = new ZipEntry(fileName);
                        entry.DateTime = DateTime.Now;
                        entry.Size = fs.Length;
                        fs.Close();
                        crc.Reset();
                        crc.Update(buffer);
                        entry.Crc = crc.Value;
                        s.PutNextEntry(entry);
                        s.Write(buffer, 0, buffer.Length);
                    }
                }
            }
        }

        #endregion

        #region 解压
        /// <summary>   
        /// 解压功能(解压压缩文件到指定目录)   
        /// </summary>   
        /// <param name="fileToUnZip">待解压的文件</param>   
        /// <param name="zipedFolder">指定解压目标目录</param>   
        /// <param name="password">密码</param>   
        /// <returns>解压结果</returns>   
        public static OperationReturn UnZip(string fileToUnZip, string zipedFolder, string password)
        {
            OperationReturn optReturn  = new OperationReturn();
            if (File.Exists(fileToUnZip))
            {
                FileStream fs = File.OpenRead(fileToUnZip);
                optReturn = UnZip(fs, zipedFolder, password);
            }
            else
            {
                optReturn.Result = false;
                optReturn.Code = ConstDefines.Upgrade_File_Not_Found;
            }
            return optReturn;
        }

        /// <summary>   
        /// 解压功能(解压压缩文件到指定目录)   
        /// </summary>   
        /// <param name="fileToUnZip">待解压的文件</param>   
        /// <param name="zipedFolder">指定解压目标目录</param>   
        /// <returns>解压结果</returns>   
        public static OperationReturn UnZip(string fileToUnZip, string zipedFolder)
        {
            OperationReturn optReturn= UnZip(fileToUnZip, zipedFolder, null);
            return optReturn;
        }

        /// <summary>
        /// 解压功能(解压压缩文件到指定目录)   
        /// </summary>
        /// <param name="zipStream">待解压的文件流</param>
        /// <param name="zipedFolder">指定解压目标目录</param>
        /// <returns>解压结果</returns>
        public static OperationReturn UnZip(Stream zipStream, string zipedFolder)
        {
            OperationReturn optReturn = UnZip(zipStream, zipedFolder, null);
            return optReturn;
        }

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="zipStream"></param>
        /// <param name="zipedFolder"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static OperationReturn UnZip(Stream zipInputStream, string zipedFolder, string password)
        {
            OperationReturn optReturn= new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;

            bool result = true;
            FileStream fs = null;
            ZipInputStream zipStream = null;
            ZipEntry ent = null;
            string fileName;

            try
            {
                if (!Directory.Exists(zipedFolder))
                    Directory.CreateDirectory(zipedFolder);

                zipStream = new ZipInputStream(zipInputStream);
                if (!string.IsNullOrEmpty(password)) zipStream.Password = password;
                while ((ent = zipStream.GetNextEntry()) != null)
                {
                    if (!string.IsNullOrEmpty(ent.Name))
                    {
                        fileName = Path.Combine(zipedFolder, ent.Name);
                        fileName = fileName.Replace('/', '\\');//change by Mr.HopeGi   

                        if (fileName.EndsWith("\\"))
                        {
                            Directory.CreateDirectory(fileName);
                            continue;
                        }
                        fs = File.Create(fileName);
                        int size = 2048;
                        byte[] data = new byte[size];
                        while (true)
                        {
                            size = zipStream.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                fs.Write(data, 0, size);
                                zipStream.Flush();
                            }
                            else
                                break;
                        }
                        if (fs != null)
                        {
                            fs.Close();
                            fs.Dispose();
                        }
                        FileInfo fileInfo = new FileInfo(fileName);
                        fileInfo.LastWriteTime = ent.DateTime;
                    }
                }
            }
            catch(Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
                if (zipStream != null)
                {
                    zipStream.Close();
                    zipStream.Dispose();
                }
                if (ent != null)
                {
                    ent = null;
                }
                GC.Collect();
                GC.Collect(1);
            }
            return optReturn;
        }

        #endregion
    }

}
