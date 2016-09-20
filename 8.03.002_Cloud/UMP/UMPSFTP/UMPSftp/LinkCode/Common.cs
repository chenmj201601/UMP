using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Net;

namespace UMPCommon
{
    class Common
    {
        private static object syncRoot = new object();
        #region 读写ini
        // 声明INI文件的写操作函数 WritePrivateProfileString()
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        // 声明INI文件的读操作函数 GetPrivateProfileString()
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, System.Text.StringBuilder retVal, int size, string filePath);

        static public void Writue(string section, string key, string value, string sPath)
        {
            lock (syncRoot)
            {
                // section=配置节，key=键名，value=键值，path=路径
                WritePrivateProfileString(section, key, value, sPath);
            }
        }
        static public string ReadValue(string section, string key, string sPath)
        {
            lock (syncRoot)
            {
                // 每次从ini中读取多少字节
                System.Text.StringBuilder temp = new System.Text.StringBuilder(255);
                // section=配置节，key=键名，temp=上面，path=路径
                GetPrivateProfileString(section, key, "", temp, 255, sPath);
                return temp.ToString();
            }
        }
        #endregion

        #region Communication
        static public string SendSSLMsg(string host, int port, string msg)
        {
            lock (syncRoot)
            {
                System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient(host, port);
                System.Net.Security.SslStream sslStream = new System.Net.Security.SslStream(
                    client.GetStream(),
                    false,
                    new System.Net.Security.RemoteCertificateValidationCallback(
                        (object sender,
                         System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                         System.Security.Cryptography.X509Certificates.X509Chain chain,
                         System.Net.Security.SslPolicyErrors sslPolicyErrors) =>
                        {
                            if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors
                                || sslPolicyErrors == System.Net.Security.SslPolicyErrors.None) { return true; }
                            return false;
                        }
                        ),
                    null);
                sslStream.WriteTimeout = 2000;
                sslStream.ReadTimeout = 2000;

                try
                {
                    sslStream.AuthenticateAsClient("VoiceCyber.PF", null, System.Security.Authentication.SslProtocols.Tls, false);
                    byte[] messsage = Encoding.UTF8.GetBytes(msg + "\r\n");
                    sslStream.Write(messsage);
                    sslStream.Flush();

                    byte[] buffer = new byte[2048];
                    StringBuilder messageData = new StringBuilder();
                    int bytes = -1;
                    try
                    {
                        do
                        {
                            bytes = sslStream.Read(buffer, 0, buffer.Length);
                            Decoder decoder = Encoding.UTF8.GetDecoder();
                            char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                            decoder.GetChars(buffer, 0, bytes, chars, 0);
                            messageData.Append(chars);
                            if (messageData.ToString().IndexOf("\r\n") != -1)
                            {
                                messageData.Replace("\r\n", "");
                                break;
                            }
                        } while (bytes != 0);
                    }
                    catch (System.Exception ex)
                    {
                        messageData.Clear();
                        throw ex;
                    }
                    return messageData.ToString();
                }
                catch (System.Security.Authentication.AuthenticationException e)
                {
                    client.Close();
                    throw e;
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
            }
        }
        #endregion

        static public string[] GetLocalIp(bool IpV6 = false)
        {
            lock (syncRoot)
            {
                string hostname = Dns.GetHostName();
                IPHostEntry localhost = Dns.GetHostEntry(hostname);
                List<string> addresses = new List<string>();
                foreach (var ipAddress in localhost.AddressList)
                {
                    if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        addresses.Add(ipAddress.ToString());
                        continue;
                    }
                    if (IpV6 && ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                        addresses.Add(ipAddress.ToString());
                }
                return addresses.ToArray();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ts">最后的时间点</param>
        /// <param name="minutes">间隔时间</param>
        /// <returns>间隔时间内为真，否则为假</returns>
        static public bool IntervalTime(ref TimeSpan ts, double minutes)
        {
            lock (syncRoot)
            {
                TimeSpan cur_ts = DateTime.Now.TimeOfDay;
                if (System.Math.Abs((cur_ts - ts).TotalMinutes) < minutes)
                    return true;

                ts = cur_ts;
                return false;
            }
        }

        /// <summary>
        /// 生成目录文件目录
        /// </summary>
        /// <param name="sourceFileName"></param>
        /// <param name="destFileName"></param>
        /// <param name="overwrite"></param>
        static public void CopyFileEx(string sourceFileName, string destFileName, bool overwrite)
        {
            lock (syncRoot)
            {
                if (!Directory.Exists(destFileName.Substring(0, destFileName.LastIndexOf("\\"))))
                {
                    Directory.CreateDirectory(destFileName.Substring(0, destFileName.LastIndexOf("\\")));
                }
                File.Copy(sourceFileName, destFileName, overwrite);
            }
        }

        /// <summary>
        /// Convert Bytes to Structure
        /// </summary>
        /// <param name="data">Data in byte</param>
        /// <param name="type">Structure type</param>
        /// <returns></returns>
        static public object BytesToStruct(byte[] data, Type type)
        {
            lock (syncRoot)
            {
                //得到结构体的大小
                int size = Marshal.SizeOf(type);
                //byte数组长度小于结构体的大小
                if (size > data.Length)
                {
                    //返回空
                    return null;
                }
                try
                {
                    //分配结构体大小的内存空间
                    IntPtr structPtr = Marshal.AllocHGlobal(size);
                    //将byte数组拷到分配好的内存空间
                    Marshal.Copy(data, 0, structPtr, size);
                    //将内存空间转换为目标结构体
                    object obj = Marshal.PtrToStructure(structPtr, type);
                    //释放内存空间
                    Marshal.FreeHGlobal(structPtr);
                    //返回结构体
                    return obj;
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
