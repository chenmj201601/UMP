using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using VoiceCyber.Common;

namespace UMPServicePackCommon
{
    public class CommonFuncs
    {
        /// <summary>
        /// 根据服务名获得服务状态
        /// </summary>
        /// <param name="AStrServiceName">服务名</param>
        /// <returns>0：不存在  1：未启动 2：启动</returns>
        public static OperationReturn GetComputerServiceStatus(string AStrServiceName)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;

            ServiceEnty service = null;
            string LStrSatus = string.Empty;

            try
            {
                ManagementObjectCollection ObjectCollection;
                ObjectCollection = GetServiceCollection("SELECT * FROM Win32_Service WHERE Name = '" + AStrServiceName + "'");
                if (ObjectCollection.Count < 1)
                {
                    LStrSatus = "0";
                    service = new ServiceEnty();
                    service.ServiceName = AStrServiceName;
                    service.ServiceStatus = int.Parse(LStrSatus);
                    optReturn.Data = service;
                    return optReturn;
                }
                foreach (ManagementObject ObjectSingleReturn in ObjectCollection)
                {
                    try
                    {
                        if (ObjectSingleReturn["Started"].Equals(true))
                            LStrSatus = "2";
                        else
                            LStrSatus = "1";
                        break;
                    }
                    catch (Exception ex)
                    {
                        LStrSatus = string.Empty;
                    }
                }
                service = new ServiceEnty();
                service.ServiceName = AStrServiceName;
                service.ServiceStatus = int.Parse(LStrSatus);
                optReturn.Data = service;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        static ManagementObjectCollection GetServiceCollection(string AStrQuery)
        {
            ConnectionOptions ComputerConnect = new ConnectionOptions();

            ManagementScope ComputerManagement = new ManagementScope("\\\\localhost\\root\\cimv2", ComputerConnect);
            ComputerConnect.Timeout = new TimeSpan(0, 10, 0);
            ComputerManagement.Connect();
            ObjectQuery VoiceServiceQuery = new ObjectQuery(AStrQuery);
            ManagementObjectSearcher ObjectSearcher = new ManagementObjectSearcher(ComputerManagement, VoiceServiceQuery);
            ManagementObjectCollection ReturnCollection = ObjectSearcher.Get();
            return ReturnCollection;
        }

        /// <summary>
        /// 拷贝文件夹(用于备份UMP系统)
        /// </summary>
        /// 备份系统时 如果文件夹存在 尝试删除 如果无法删除 则重命名 
        /// <param name="strFromPath">源路径</param>
        /// <param name="strToPath">目标路径</param>
        public static OperationReturn CopyFolder(string strFromPath, string strToPath)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;


            //如果源文件夹不存在，则返回错误
            if (!Directory.Exists(strFromPath))
            {
                optReturn.Result = false;
                optReturn.Code = ConstDefines.Copy_Res_Not_Found;
                return optReturn;
            }
            try
            {
                //取得要拷贝的文件夹名
                string strFolderName = strFromPath.Substring(strFromPath.LastIndexOf("\\") +
                1, strFromPath.Length - strFromPath.LastIndexOf("\\") - 1);
                //如果文件夹存在 就重命名
                if (Directory.Exists(strToPath + "\\" + strFolderName))
                {
                    try
                    {
                        Directory.Delete(strToPath + "\\" + strFolderName);
                    }
                    catch
                    {
                        string strOldName = strToPath + "\\" + strFolderName;
                        string strLastModifyTime = string.Empty;
                        DirectoryInfo dirTempInfo = new DirectoryInfo(strOldName);
                        strLastModifyTime = dirTempInfo.LastWriteTime.Year.ToString() + dirTempInfo.LastWriteTime.Month.ToString()
                            + dirTempInfo.LastWriteTime.Day.ToString() + dirTempInfo.LastWriteTime.Hour.ToString()
                            + dirTempInfo.LastWriteTime.Minute.ToString() + dirTempInfo.LastWriteTime.Second.ToString();
                        string strNewName = strToPath + "\\" + strFolderName + strLastModifyTime;
                        Directory.Move(strOldName, strNewName);
                    }
                }
                //if (!Directory.Exists(strToPath + "\\" + strFolderName))
                //{
                //如果目标文件夹中没有源文件夹则在目标文件夹中创建源文件夹
                Directory.CreateDirectory(strToPath + "\\" + strFolderName);
                //}
                //创建数组保存源文件夹下的文件名
                string[] strFiles = Directory.GetFiles(strFromPath);
                //循环拷贝文件
                for (int i = 0; i < strFiles.Length; i++)
                {
                    //FileIsInUse(strFiles[i]);
                    //取得拷贝的文件名，只取文件名，地址截掉。
                    string strFileName = strFiles[i].Substring(strFiles[i].LastIndexOf("\\") + 1, strFiles[i].Length - strFiles[i].LastIndexOf("\\") - 1);
                    //FileIsInUse(strToPath + "\\" + strFolderName + "\\" + strFileName);
                    //开始拷贝文件,true表示覆盖同名文件
                    File.Copy(strFiles[i], strToPath + "\\" + strFolderName + "\\" + strFileName, true);
                }
                //创建DirectoryInfo实例
                DirectoryInfo dirInfo = new DirectoryInfo(strFromPath);
                //取得源文件夹下的所有子文件夹名称
                DirectoryInfo[] ZiPath = dirInfo.GetDirectories();
                for (int j = 0; j < ZiPath.Length; j++)
                {
                    //获取所有子文件夹名
                    string strZiPath = strFromPath + "\\" + ZiPath[j].ToString();
                    //把得到的子文件夹当成新的源文件夹，从头开始新一轮的拷贝
                    optReturn = CopyFolder(strZiPath, strToPath + "\\" + strFolderName);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = ConstDefines.Copy_File_Exception;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        /// <summary>
        /// 拷贝文件夹（用于升级时）
        /// </summary>
        /// 拷贝文件夹时，如果文件夹存在 就保留原文件结构 增加或修改文件夹中的文件
        /// <param name="strFromPath"></param>
        /// <param name="strToPath"></param>
        /// <returns></returns>
        //public static OperationReturn CopyFolderInUpgrade(string strFromPath, string strToPath)
        //{
        //    OperationReturn optReturn = new OperationReturn();
        //    optReturn.Result = true;
        //    optReturn.Code = Defines.RET_SUCCESS;
        //    try
        //    {
        //        //如果源文件夹不存在，则返回错误
        //        if (!Directory.Exists(strFromPath))
        //        {
        //            optReturn.Result = false;
        //            optReturn.Code = ConstDefines.Copy_Res_Not_Found;
        //            return optReturn;
        //        }
        //        //取得要拷贝的文件夹名
        //        string strFolderName = strFromPath.Substring(strFromPath.LastIndexOf("\\") +
        //        1, strFromPath.Length - strFromPath.LastIndexOf("\\") - 1);
        //        if (!Directory.Exists(strToPath + "\\" + strFolderName))
        //        {
        //            //如果目标文件夹不存在 就创建
        //            Directory.CreateDirectory(strToPath + "\\" + strFolderName);
        //        }
        //        //如果文件夹存在 更新其中的文件和子文件夹
        //        //创建数组保存源文件夹下的文件名
        //        string[] strFiles = Directory.GetFiles(strFromPath);
        //        //循环拷贝文件
        //        for (int i = 0; i < strFiles.Length; i++)
        //        {
        //            //FileIsInUse(strFiles[i]);
        //            //取得拷贝的文件名，只取文件名，地址截掉。
        //            string strFileName = strFiles[i].Substring(strFiles[i].LastIndexOf("\\") + 1, strFiles[i].Length - strFiles[i].LastIndexOf("\\") - 1);
        //            //FileIsInUse(strToPath + "\\" + strFolderName + "\\" + strFileName);
        //            //开始拷贝文件,true表示覆盖同名文件
        //            string strFilePath = strToPath + "\\" + strFolderName + "\\" + strFileName;
        //            FileIsInUse(strFiles[i]);
        //            if (File.Exists(strFilePath))
        //            {
        //                FileIsInUse(strFilePath);
        //                if ((File.GetAttributes(strFilePath) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
        //                {
        //                    // 如果是将文件的属性设置为Normal 
        //                    File.SetAttributes(strFilePath, FileAttributes.Normal);
        //                }
        //                for (int iCopyCount = 0; iCopyCount < 3; iCopyCount++)
        //                {
        //                    try
        //                    {
        //                        File.Copy(strFiles[i], strToPath + "\\" + strFolderName + "\\" + strFileName, true);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        if()
        //                    }
        //                }
        //            }
                    
        //        }
        //        //创建DirectoryInfo实例
        //        DirectoryInfo dirInfo = new DirectoryInfo(strFromPath);
        //        //取得源文件夹下的所有子文件夹名称
        //        DirectoryInfo[] ZiPath = dirInfo.GetDirectories();
        //        for (int j = 0; j < ZiPath.Length; j++)
        //        {
        //            //获取所有子文件夹名
        //            string strZiPath = strFromPath + "\\" + ZiPath[j].ToString();
        //            //把得到的子文件夹当成新的源文件夹，从头开始新一轮的拷贝
        //            optReturn = CopyFolderInUpgrade(strZiPath, strToPath.TrimEnd('\\') + "\\" + strFolderName);
        //            if (!optReturn.Result)
        //            {
        //                return optReturn;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        optReturn.Result = false;
        //        optReturn.Code = ConstDefines.Copy_File_Exception;
        //        optReturn.Message = ex.Message;
        //    }
        //    return optReturn;
        //}


        /// <summary>
        /// 判断文件是否被占用
        /// </summary>
        /// <param name="AStrFileName"></param>
        /// <returns></returns>
        public static bool FileIsInUse(string AStrFileName)
        {
            bool LBoolReturn = true;
            FileStream LocalFileStream = null;

            try
            {
                LocalFileStream = new FileStream(AStrFileName, FileMode.Open, FileAccess.Read, FileShare.None);
                LBoolReturn = false;
            }
            catch { }
            finally
            {
                if (LocalFileStream != null) { LocalFileStream.Close(); }
            }

            return LBoolReturn;
        }

        public static OperationReturn StopServiceByName(string strServiceName)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;

            try
            {
                InvokeMethodOptions OperationMethodOptions = new InvokeMethodOptions(null, new TimeSpan(0, 10, 0));
                ManagementBaseObject LOutStopService;

                ManagementObjectCollection ObjectCollection;
                ObjectCollection = GetServiceCollection(string.Format("SELECT * FROM Win32_Service WHERE Name = '{0}'", strServiceName));
                int i = ObjectCollection.Count;
                string LStrProcessId = string.Empty;
                string LStrDisplayName = string.Empty;
                int LIntReturnValue = -1;
                string strStatus = string.Empty;

                foreach (ManagementObject SingleCollenction in ObjectCollection)
                {
                    LStrProcessId = SingleCollenction["ProcessId"].ToString();
                    LStrDisplayName = SingleCollenction["DisplayName"].ToString();
                    strStatus = SingleCollenction["State"].ToString();
                    if (strStatus == "Stopped")
                    {
                        return optReturn;
                    }
                    LOutStopService = SingleCollenction.InvokeMethod("StopService", null, OperationMethodOptions);
                    LIntReturnValue = int.Parse(LOutStopService["ReturnValue"].ToString());
                    if (LIntReturnValue != 0)
                    {
                        int LIntWaitCount = 0;
                        string LStrServiceCurrentStatus = string.Empty;
                        optReturn = GetComputerServiceStatus(strServiceName);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        LStrServiceCurrentStatus = optReturn.Data as string;
                        while (LStrServiceCurrentStatus != "0" && LIntWaitCount < 30)
                        {
                            System.Threading.Thread.Sleep(1000);
                            LIntWaitCount += 1;
                            optReturn = GetComputerServiceStatus(strServiceName);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            LStrServiceCurrentStatus = optReturn.Data as string;
                            if (LStrServiceCurrentStatus == "0")
                            {
                                break;
                            }
                        }
                        if (LStrServiceCurrentStatus == "0") { LIntReturnValue = 0; }
                        else { KillProcess(LStrProcessId); LIntReturnValue = 0; }
                    }
                    //判断服务状态 直到变成Stopped为止 30s不停止 杀掉进程
                    optReturn = GetComputerServiceStatus(strServiceName);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    int status = (optReturn.Data as ServiceEnty).ServiceStatus;
                    int iCount = 0;
                    while (status != 1 && iCount < 30)
                    {
                        System.Threading.Thread.Sleep(1000);
                        optReturn = GetComputerServiceStatus(strServiceName);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        status = (optReturn.Data as ServiceEnty).ServiceStatus;
                        iCount++;
                        if (status == 1)
                        {
                            break;
                        }
                    }
                    if (status != 1)
                    {
                        KillProcess(LStrProcessId);
                    }
                }
                //保险起见 再杀一次进程
                if (!string.IsNullOrEmpty(LStrProcessId))
                {
                    KillProcess(LStrProcessId);
                }
                return optReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = ConstDefines.Stop_Service_Exception;
                optReturn.Message = ex.Message;
                return optReturn;
            }
        }

        /// <summary>
        /// 杀进程 
        /// </summary>
        /// <param name="AStrProcessId"></param>
        private static OperationReturn KillProcess(string AStrProcessId)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;

            try
            {
                ManagementObjectCollection ObjectCollection;
                ObjectCollection = GetServiceCollection("Select * from win32_process where ProcessID = '" + AStrProcessId + "'");
                foreach (ManagementObject SingleCollenction in ObjectCollection)
                {
                    SingleCollenction.InvokeMethod("Terminate", null);
                }
                return optReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = ConstDefines.Kill_Process_Exception;
                optReturn.Message = ex.Message;
                return optReturn;
            }
        }

        /// <summary>
        /// 读出脚本文件中的sql脚本
        /// </summary>
        /// <param name="strSqlFilePath"></param>
        /// <returns></returns>
        public static OperationReturn GetScriptFile(string strSqlFilePath)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            try
            {
                if (!File.Exists(strSqlFilePath))
                {
                    optReturn.Result = false;
                    optReturn.Code = ConstDefines.Script_File_Not_Exists;
                    return optReturn;
                }
                StreamReader sr = new StreamReader(strSqlFilePath);
                string strContent = sr.ReadToEnd();
                optReturn.Data = strContent;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = ConstDefines.Kill_Process_Exception;
                optReturn.Message = ex.Message;
                return optReturn;
            }
            return optReturn;
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="strServiceName"></param>
        /// <returns></returns>
        public static OperationReturn StartServiceByNmae(string strServiceName)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;

            try
            {
                InvokeMethodOptions OperationMethodOptions = new InvokeMethodOptions(null, new TimeSpan(0, 10, 0));
                ManagementBaseObject LOutStopService;

                ManagementObjectCollection ObjectCollection;
                ObjectCollection = GetServiceCollection(string.Format("SELECT * FROM Win32_Service WHERE Name = '{0}'", strServiceName));
                int i = ObjectCollection.Count;
                string LStrProcessId = string.Empty;
                string LStrDisplayName = string.Empty;
                int LIntReturnValue = -1;

                foreach (ManagementObject SingleCollenction in ObjectCollection)
                {
                    for (int iStartCount = 0; iStartCount < 30; iStartCount++)
                    {
                        LOutStopService = SingleCollenction.InvokeMethod("StartService", null, OperationMethodOptions);
                        LIntReturnValue = int.Parse(LOutStopService["ReturnValue"].ToString());
                        if (LIntReturnValue == 0)
                        {
                            return optReturn;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = ConstDefines.Start_Service_Exception;
                optReturn.Message = ex.Message;
                return optReturn;
            }
            return optReturn;
        }

        /// <summary>
        /// 调用cmd 安装、卸载服务、
        /// </summary>
        /// <param name="strCommand">命令</param>
        /// <returns></returns>
        public static OperationReturn CmdOperator(string strCommand)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;

            try
            {
                Process p = new Process();  //创建并实例化一个操作进程的类：Process  
                p.StartInfo.FileName = "cmd.exe";    //设置要启动的应用程序  
                p.StartInfo.UseShellExecute = false;   //设置是否使用操作系统shell启动进程  
                p.StartInfo.RedirectStandardInput = true;  //指示应用程序是否从StandardInput流中读取  
                p.StartInfo.RedirectStandardOutput = true; //将应用程序的输入写入到StandardOutput流中  
                p.StartInfo.RedirectStandardError = true;  //将应用程序的错误输出写入到StandarError流中  
                p.StartInfo.CreateNoWindow = true;    //是否在新窗口中启动进程  
                string strOutput = null;
                try
                {
                    p.Start();
                    p.StandardInput.WriteLine(strCommand);    //将CMD命令写入StandardInput流中  
                    p.StandardInput.WriteLine("exit");         //将 exit 命令写入StandardInput流中  
                    strOutput = p.StandardOutput.ReadToEnd();   //读取所有输出的流的所有字符  
                    p.WaitForExit();                           //无限期等待，直至进程退出  
                    p.Close();                                  //释放进程，关闭进程  
                }
                catch (Exception e)
                {
                    strOutput = e.Message;
                }
                return optReturn;

            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = ConstDefines.Start_Service_Exception;
                optReturn.Message = ex.Message;
                return optReturn;
            }
        }

        public static OperationReturn StartMAMT(string strMAMTPath)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;

            try
            {
                string strRoot = strMAMTPath.Substring(0, strMAMTPath.IndexOf(@"\"));
                string strFolder = strMAMTPath + "\\ManagementMaintenance";
                string strFileName = "UMP.MAMT.exe";
                
                Process p = new Process();  //创建并实例化一个操作进程的类：Process  
                p.StartInfo.FileName = "cmd.exe";    //设置要启动的应用程序  
                p.StartInfo.UseShellExecute = false;   //设置是否使用操作系统shell启动进程  
                p.StartInfo.RedirectStandardInput = true;  //指示应用程序是否从StandardInput流中读取  
                p.StartInfo.RedirectStandardOutput = true; //将应用程序的输入写入到StandardOutput流中  
                p.StartInfo.RedirectStandardError = true;  //将应用程序的错误输出写入到StandarError流中  
                p.StartInfo.CreateNoWindow = true;    //是否在新窗口中启动进程  
                string strOutput = null;
                try
                {
                    p.Start();
                    p.StandardInput.WriteLine(strRoot);    //将CMD命令写入StandardInput流中  
                    p.StandardInput.WriteLine("cd " + strFolder);
                    p.StandardInput.WriteLine(strFileName);
                    p.StandardInput.WriteLine("exit");         //将 exit 命令写入StandardInput流中  
                    strOutput = p.StandardOutput.ReadToEnd();   //读取所有输出的流的所有字符  
                    p.WaitForExit();                           //无限期等待，直至进程退出  
                    p.Close();                                  //释放进程，关闭进程  
                }
                catch (Exception e)
                {
                    strOutput = e.Message;
                }
                return optReturn;

            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = ConstDefines.Start_Service_Exception;
                optReturn.Message = ex.Message;
                return optReturn;
            }
        }


    }
}
