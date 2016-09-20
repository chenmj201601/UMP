using PFShareClassesS;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace Wcf00001
{
    public class DatabaseType0Operation
    {
        /// <summary>
        /// 获取需要创建的对象
        /// </summary>
        /// <param name="AListStrArguments">
        /// 0-数据库类型
        /// 1-当前已经存在的版本
        /// </param>
        /// <returns></returns>
        public static OperationDataArgs ObtainCreateObjects(List<string> AListStrArguments)
        {
            OperationDataArgs LOperationDataArgsReturn = new OperationDataArgs();
            string LStrIISBaseFolder = string.Empty;
            string LStrObjectsFolder = string.Empty;

            string LStrGetVersion = string.Empty;
            string LStrGetObjectFolder = string.Empty;
            string LStrObjectFolderLeft1 = string.Empty;
            string LStrObjectFolderRight1 = string.Empty;
            string LStrGetObjectFileName = string.Empty;
            string LStrObjectName = string.Empty;
            int LIntLastSpliterPosition = 0;

            try
            {
                LStrIISBaseFolder = GetIISBaseDirectory();
                LStrObjectsFolder = System.IO.Path.Combine(LStrIISBaseFolder, @"MAMT\DBObjects");

                Version LVersionInArgument = new Version(AListStrArguments[1]);

                #region 创建对象保存的表
                DataTable LDataTableObjectsList = new DataTable();
                //对象名称
                LDataTableObjectsList.Columns.Add("C001", typeof(string));
                //对象类型
                LDataTableObjectsList.Columns.Add("C002", typeof(string));
                //所在相对路径 "MAMT\DBObjects"后面的路径
                LDataTableObjectsList.Columns.Add("C003", typeof(string));
                //版本
                LDataTableObjectsList.Columns.Add("C004", typeof(string));
                #endregion

                DirectoryInfo LDirectoryInfoDBObjects = new DirectoryInfo(LStrObjectsFolder);
                foreach (DirectoryInfo LDirectoryInfoVersion in LDirectoryInfoDBObjects.GetDirectories())
                {
                    LStrGetVersion = LDirectoryInfoVersion.Name;
                    Version LVersionGetted = new Version(LStrGetVersion);
                    if (LVersionInArgument >= LVersionGetted) { continue; }
                    foreach (DirectoryInfo LDirectoryInfoSingleObject in LDirectoryInfoVersion.GetDirectories())
                    {
                        LStrGetObjectFolder = LDirectoryInfoSingleObject.Name;
                        LStrObjectFolderLeft1 = LStrGetObjectFolder.Substring(0, 1);
                        if (LStrObjectFolderLeft1 == "0" || LStrObjectFolderLeft1 == AListStrArguments[0])
                        {
                            LStrObjectFolderRight1 = LStrGetObjectFolder.Substring(LStrGetObjectFolder.Length - 1, 1);
                            foreach (FileInfo LFileInfoSingleFile in LDirectoryInfoSingleObject.GetFiles())
                            {
                                LStrGetObjectFileName = LFileInfoSingleFile.Name;
                                LIntLastSpliterPosition = LStrGetObjectFileName.LastIndexOf('.');
                                LStrObjectName = LStrGetObjectFileName.Substring(0, LIntLastSpliterPosition);
                                DataRow LDataRowNew = LDataTableObjectsList.NewRow();
                                LDataRowNew.BeginEdit();
                                LDataRowNew["C001"] = LStrObjectName;
                                LDataRowNew["C002"] = LStrObjectFolderRight1;
                                LDataRowNew["C003"] = LStrGetVersion + @"\" + LStrGetObjectFolder + @"\" + LStrGetObjectFileName;
                                LDataRowNew["C004"] = LStrGetVersion;
                                LDataRowNew.EndEdit();
                                LDataTableObjectsList.Rows.Add(LDataRowNew);
                            }
                        }
                    }
                }
                LOperationDataArgsReturn.DataSetReturn.Tables.Add(LDataTableObjectsList);
            }
            catch (Exception ex)
            {
                LOperationDataArgsReturn.BoolReturn = false;
                LOperationDataArgsReturn.StringReturn = "ObtainCreateObjects()\n" + ex.Message;
            }

            return LOperationDataArgsReturn;
        }

        /// <summary>
        /// 获取需要初始化或更改的数据
        /// </summary>
        /// <param name="AListStrArguments">
        /// 0-当前已经存在的版本
        /// </param>
        /// <returns></returns>
        public static OperationDataArgs ObtainInitializationData(List<string> AListStrArguments)
        {
            OperationDataArgs LOperationDataArgsReturn = new OperationDataArgs();
            string LStrIISBaseFolder = string.Empty;
            string LStrObjectsFolder = string.Empty;

            string LStrGetVersion = string.Empty;
            string LStrGetObjectFolder = string.Empty;
            string LStrGetObjectFileName = string.Empty;
            string LStrObjectName = string.Empty;
            int LIntLastSpliterPosition = 0;

            try
            {
                LStrIISBaseFolder = GetIISBaseDirectory();
                LStrObjectsFolder = System.IO.Path.Combine(LStrIISBaseFolder, @"MAMT\DBObjects");

                Version LVersionInArgument = new Version(AListStrArguments[0]);

                #region 创建对象保存的表
                DataTable LDataTableObjectsList = new DataTable();
                //对象名称
                LDataTableObjectsList.Columns.Add("C001", typeof(string));
                //所在相对路径 "MAMT\DBObjects"后面的路径
                LDataTableObjectsList.Columns.Add("C002", typeof(string));
                //版本
                LDataTableObjectsList.Columns.Add("C003", typeof(string));
                #endregion

                DirectoryInfo LDirectoryInfoDBObjects = new DirectoryInfo(LStrObjectsFolder);
                foreach (DirectoryInfo LDirectoryInfoVersion in LDirectoryInfoDBObjects.GetDirectories())
                {
                    LStrGetVersion = LDirectoryInfoVersion.Name;
                    Version LVersionGetted = new Version(LStrGetVersion);
                    if (LVersionInArgument >= LVersionGetted) { continue; }

                    foreach (DirectoryInfo LDirectoryInfoSingleObject in LDirectoryInfoVersion.GetDirectories())
                    {
                        LStrGetObjectFolder = LDirectoryInfoSingleObject.Name;
                        if (LStrGetObjectFolder != "91-D") { continue; }
                        foreach (FileInfo LFileInfoSingleFile in LDirectoryInfoSingleObject.GetFiles())
                        {
                            LStrGetObjectFileName = LFileInfoSingleFile.Name;
                            LIntLastSpliterPosition = LStrGetObjectFileName.LastIndexOf('.');
                            LStrObjectName = LStrGetObjectFileName.Substring(0, LIntLastSpliterPosition);

                            DataRow LDataRowNew = LDataTableObjectsList.NewRow();
                            LDataRowNew.BeginEdit();
                            LDataRowNew["C001"] = LStrObjectName;
                            LDataRowNew["C002"] = LStrGetVersion + @"\" + LStrGetObjectFolder + @"\" + LStrGetObjectFileName;
                            LDataRowNew["C003"] = LStrGetVersion;
                            LDataRowNew.EndEdit();
                            LDataTableObjectsList.Rows.Add(LDataRowNew);
                        }
                    }
                }
                LOperationDataArgsReturn.DataSetReturn.Tables.Add(LDataTableObjectsList);
            }
            catch (Exception ex)
            {
                LOperationDataArgsReturn.BoolReturn = false;
                LOperationDataArgsReturn.StringReturn = "ObtainInitializationData()\n" + ex.Message;
            }

            return LOperationDataArgsReturn;
        }

        private static string CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType AKeyIVID)
        {
            string LStrReturn = string.Empty;
            int LIntRand = 0;
            string LStrTemp = string.Empty;

            try
            {
                Random LRandom = new Random();
                LStrReturn = DateTime.Now.ToString("yyyyMMddHHmmss");
                LIntRand = LRandom.Next(0, 14);
                LStrTemp = LIntRand.ToString("00");
                LStrReturn = LStrReturn.Insert(LIntRand, "VCT");
                LIntRand = LRandom.Next(0, 17);
                LStrTemp += LIntRand.ToString("00");
                LStrReturn = LStrReturn.Insert(LIntRand, "UMP");
                LIntRand = LRandom.Next(0, 20);
                LStrTemp += LIntRand.ToString("00");
                LStrReturn = LStrReturn.Insert(LIntRand, ((int)AKeyIVID).ToString("000"));

                LStrReturn = EncryptionAndDecryption.EncryptStringY(LStrTemp + LStrReturn);
            }
            catch { LStrReturn = string.Empty; }

            return LStrReturn;
        }

        /// <summary>
        /// 生成系统SpliterCharater,结果为字符char(27)
        /// </summary>
        /// <returns></returns>
        private static string CreateSpliterCharater()
        {
            string LStrSpliter = string.Empty;

            try
            {
                System.Text.ASCIIEncoding LAsciiEncoding = new System.Text.ASCIIEncoding();
                byte[] LByteArray = new byte[] { (byte)27 };
                string LStrCharacter = LAsciiEncoding.GetString(LByteArray);
                LStrSpliter = LStrCharacter;
            }
            catch { LStrSpliter = string.Empty; }

            return LStrSpliter;
        }

        /// <summary>
        /// 获取当前UMP安装的目录
        /// </summary>
        /// <returns></returns>
        private static string GetIISBaseDirectory()
        {
            string LStrBaseDirectory = string.Empty;

            try
            {
                LStrBaseDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                string[] LStrArrayDirectory = LStrBaseDirectory.Split(@"\".ToCharArray());
                LStrBaseDirectory = string.Empty;
                foreach (string LStrSingleDirectory in LStrArrayDirectory)
                {
                    LStrBaseDirectory += LStrSingleDirectory + @"\";
                    if (System.IO.Directory.Exists(LStrBaseDirectory + "GlobalSettings") && System.IO.Directory.Exists(LStrBaseDirectory + "Components") && System.IO.Directory.Exists(LStrBaseDirectory + "WcfServices")) { break; }
                }
            }
            catch { LStrBaseDirectory = string.Empty; }

            return LStrBaseDirectory;
        }
    }
}