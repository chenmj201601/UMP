using System;
using System.IO;
using VoiceCyber.SharpZips.Checksums;
using VoiceCyber.SharpZips.Zip;

namespace VCSharpZipDemo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                string password = "voicecyber";

                Console.WriteLine("Starting...");

                //压缩单个文件
                string strFile = string.Format(@"E:\aaa.txt");
                string strZip = string.Format(@"E:\aaa.zip");
                ZipFile(strFile, strZip,password);

                ////压缩一个目录
                //string strPath = string.Format(@"D:\temp");
                //string strZip = string.Format(@"D:\temp.zip");
                //ZipFileDirectory(strPath, strZip, password);

                ////解压一个文件
                //string strZip = string.Format(@"D:\temp.zip");
                //string strPath = string.Format(@"D:\temp1");
                //UnZip(strZip, strPath, password, true);

                Console.WriteLine("End");
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            Console.ReadLine();
        }

        /// <summary>
        /// 压缩单个文件
        /// </summary>
        /// <param name="fileToZip">要进行压缩的文件名</param>
        /// <param name="zipedFile">压缩后生成的压缩文件名</param>
        /// <param name="password"></param>
        public static void ZipFile(string fileToZip, string zipedFile,string password)
        {
            //如果文件没有找到，则报错
            if (!File.Exists(fileToZip))
            {
                throw new FileNotFoundException("指定要压缩的文件: " + fileToZip + " 不存在!");
            }
            using (FileStream fs = File.OpenRead(fileToZip))
            {
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();
                using (FileStream ZipFile = File.Create(zipedFile))
                {
                    using (ZipOutputStream ZipStream = new ZipOutputStream(ZipFile))
                    {
                        ZipStream.Password = password;
                        ZipStream.SetLevel(9);
                        string fileName = fileToZip.Substring(fileToZip.LastIndexOf("\\") + 1);
                        ZipEntry ZipEntry = new ZipEntry(fileName);
                        ZipEntry.DateTime = File.GetLastWriteTime(fileToZip);
                        ZipStream.PutNextEntry(ZipEntry);
                        ZipStream.SetLevel(5);
                        ZipStream.Write(buffer, 0, buffer.Length);
                        ZipStream.Finish();
                        ZipStream.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 压缩多层目录
        /// </summary>
        /// <param name="strDirectory">The directory.</param>
        /// <param name="zipedFile">The ziped file.</param>
        /// <param name="password"></param>
        public static void ZipFileDirectory(string strDirectory, string zipedFile,string password)
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

        /// <summary>
        /// 解压缩一个 zip 文件。
        /// </summary>
        /// <param name="zipedFile">The ziped file.</param>
        /// <param name="strDirectory">The STR directory.</param>
        /// <param name="password">zip 文件的密码。</param>
        /// <param name="overWrite">是否覆盖已存在的文件。</param>
        public static void UnZip(string zipedFile, string strDirectory, string password, bool overWrite)
        {
            if (strDirectory == "")
                strDirectory = Directory.GetCurrentDirectory();
            if (!strDirectory.EndsWith("\\"))
                strDirectory = strDirectory + "\\";
            using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipedFile)))
            {
                s.Password = password;
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string directoryName = "";
                    string pathToZip;
                    pathToZip = theEntry.Name;
                    if (pathToZip != "")
                        directoryName = Path.GetDirectoryName(pathToZip) + "\\";
                    string fileName = Path.GetFileName(pathToZip);
                    Directory.CreateDirectory(strDirectory + directoryName);
                    if (fileName != "")
                    {
                        if ((File.Exists(strDirectory + directoryName + fileName) && overWrite) ||
                            (!File.Exists(strDirectory + directoryName + fileName)))
                        {
                            using (FileStream streamWriter = File.Create(strDirectory + directoryName + fileName))
                            {
                                int size;
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    size = s.Read(data, 0, data.Length);
                                    if (size > 0)
                                        streamWriter.Write(data, 0, size);
                                    else
                                        break;
                                }
                                streamWriter.Close();
                            }
                        }
                    }
                }
                s.Close();
            }
        }
    }
}
