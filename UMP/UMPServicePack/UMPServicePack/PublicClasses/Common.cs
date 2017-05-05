using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VoiceCyber.Common;
using UMPServicePackCommon;
using System.Net;
using System.Net.Sockets;

namespace UMPServicePack.PublicClasses
{
    public class Common
    {
        public static OperationReturn GetMachineID()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            try
            {
                bool bIsExist = true;
                string strProgramDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\VoiceCyber\\UMP\\config";
                DirectoryInfo dir = new DirectoryInfo(strProgramDataPath);
                if (!dir.Exists)
                {
                    dir.Create();
                }
                string strIniPath = strProgramDataPath + "\\localmachine.ini";
                FileInfo fi = new FileInfo(strIniPath);
                if (!fi.Exists)
                {
                    FileStream fs = fi.Create();
                    fs.Close();
                    bIsExist = false;
                }
                IniOperation ini = new IniOperation(strIniPath);

                if (!bIsExist)
                {
                    string strHostName = Dns.GetHostName(); //得到本机的主机名
                    IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
                    if (ipEntry.AddressList.Count() > 0)
                    {
                        foreach (IPAddress adress in ipEntry.AddressList)
                        {
                            if (adress.AddressFamily == AddressFamily.InterNetwork)
                            {
                                ini.IniWriteValue("LocalMachine", "SubscribeAddress", "224.0.2.26,3789");
                                break;
                            }
                            //else if (adress.AddressFamily == AddressFamily.InterNetworkV6)
                            //{
                            //    ini.IniWriteValue("LocalMachine", "SubscribeAddress", "ff01::0226,3789");
                            //    break;
                            //}
                            continue;
                        }
                    }
                }
                string strMachineID = ini.IniReadValue("LocalMachine", "MachineID");
                if (string.IsNullOrEmpty(strMachineID))
                {
                    strMachineID = Guid.NewGuid().ToString();
                    ini.IniWriteValue("LocalMachine", "MachineID", strMachineID);
                }
                optReturn.Data = strMachineID;
                App.gStrMachineID = strMachineID;
            }
            catch (Exception ex)
            {
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Result = false;
                optReturn.Message = ex.Message;
            }

            return optReturn;
        }
    }
}
