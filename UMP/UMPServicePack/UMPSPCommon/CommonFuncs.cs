using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;

namespace UMPSPCommon
{
    public class CommonFuncs
    {
        /// <summary>
        /// 根据服务名获得服务状态
        /// </summary>
        /// <param name="AStrServiceName">服务名</param>
        /// <returns>0：不存在  1：未启动 2：启动</returns>
        public static string GetComputerServiceStatus(string AStrServiceName)
        {
            string LStrSatus = string.Empty;

            try
            {
                ManagementObjectCollection ObjectCollection;
                ObjectCollection = GetServiceCollection("SELECT * FROM Win32_Service WHERE Name = '" + AStrServiceName + "'");
                if (ObjectCollection.Count < 1)
                {
                    LStrSatus = "0";
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
            }
            catch (Exception ex)
            {
                LStrSatus = string.Empty;
            }
            return LStrSatus;
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
    }
}
