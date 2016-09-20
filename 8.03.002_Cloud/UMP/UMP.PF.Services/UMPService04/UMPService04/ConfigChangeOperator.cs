//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7807d59e-df98-4e18-8f07-13a33b83a413
//        CLR Version:              4.0.30319.18063
//        Name:                     ConfigChangeOperator
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService04
//        File Name:                ConfigChangeOperator
//
//        created by Charley at 2015/7/6 18:04:40
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;

/*
 * ======================================================================
 * 
 * ConfigChangeOperator 工作逻辑
 * 
 * ***启动***
 * （1）从LocalMachine.ini文件读取MachineID
 * （2）创建一个UDP连接，接收Service00的广播消息
 * （3）当接收到广播消息，解析广播消息的内容，取得MachineID
 * （4）与当前MachineID比较，如果相同，触发ConfigChanged事件
 * 
 * ======================================================================
 */

namespace UMPService04
{
    public class ConfigChangeOperator
    {

        #region Members

        public const string NODE_NAME_MACHINEID = "MachineID";
        public const string NODE_NAME_PARAMCHANGE = "ParamChange";
        public const string FILE_NAME_LOCALMACHINE = "localmachine.ini";

        private Socket mSocket;
        private byte[] mBufferedData;
        private int mDistHeadSize;
        private int mAppHeadSize;
        private string mLocalMachineID;

        #endregion
       

        public ConfigChangeOperator()
        {
            mBufferedData = new byte[ConstValue.NET_BUFFER_MAX_SIZE];
            mDistHeadSize = Marshal.SizeOf(typeof(DistHead));
            mAppHeadSize = Marshal.SizeOf(typeof(AppHead));
        }


        #region Start and Stop

        public void Start()
        {
            try
            {
                //读取LocalMachine文件获取MachineID
                if (!GetLocalMachineID()) { return; }

                //创建UDP Socket，接收广播消息
                IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 3789);
                IPAddress ipAddress = IPAddress.Parse("224.0.2.26");
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                }
                else if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    mSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
                }
                if (mSocket == null)
                {
                    OnDebug(LogMode.Error, string.Format("Socket is null"));
                    return;
                }
                mSocket.SetSocketOption(SocketOptionLevel.IP,
                                  SocketOptionName.AddMembership,
                           new MulticastOption(ipAddress, IPAddress.Any));
                mSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                mSocket.Bind(ipep);

                //接收消息
                mSocket.BeginReceive(mBufferedData, 0, ConstValue.NET_BUFFER_MAX_SIZE, SocketFlags.None,
                   CheckConfigChangeWorker, mSocket);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("Start fail.\t{0}", ex.Message));
            }
        }

        public void Stop()
        {
            try
            {
                if (mSocket != null)
                {
                    mSocket.Close();
                    mSocket.Dispose();
                    mSocket = null;
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("Stop fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region Others

        private bool GetLocalMachineID()
        {
            try
            {
                //获取LocalMachineID
                string path;
                //if (Program.IsDebug)
                //{
                //    path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FILE_NAME_LOCALMACHINE);
                //}
                //else
                //{
                //    path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                //    path = Path.Combine(path, "VoiceCyber\\UMP\\config");
                //    path = Path.Combine(path, FILE_NAME_LOCALMACHINE);
                //}
                path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                path = Path.Combine(path, "VoiceCyber\\UMP\\config");
                path = Path.Combine(path, FILE_NAME_LOCALMACHINE);
                if (!File.Exists(path))
                {
                    OnDebug(LogMode.Error, string.Format("LocalMachine file not exist.\t{0}", path));
                    return false;
                }
                string[] arrInfos = File.ReadAllLines(path, Encoding.Default);
                for (int i = 0; i < arrInfos.Length; i++)
                {
                    string strInfo = arrInfos[i];
                    if (strInfo.StartsWith(NODE_NAME_MACHINEID))
                    {
                        string strID = strInfo.Substring(NODE_NAME_MACHINEID.Length + 1);
                        mLocalMachineID = strID;
                    }
                }

                OnDebug(LogMode.Info, string.Format("LocalMachineID:{0}", mLocalMachineID));
                return true;
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("GetLocalMachineID fail.\t{0}", ex.Message));
                return false;
            }
        }

        private void CheckConfigChangeWorker(IAsyncResult result)
        {
            var socket = result.AsyncState as Socket;
            if (socket == null)
            {
                OnDebug(LogMode.Error, string.Format("Socket is null"));
                return;
            }
            int intSize;
            try
            {
                intSize = socket.EndReceive(result);
            }
            catch (ObjectDisposedException ex)
            {
                //Socket disposed
                return;
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("End receive fail.\t{0}", ex.Message));
                return;
            }
            if (intSize <= 0)
            {
                OnDebug(LogMode.Error, string.Format("Receive 0 length data"));
                return;
            }
            try
            {
                if (intSize < mDistHeadSize)
                {
                    OnDebug(LogMode.Error, string.Format("Receive length invalid"));
                    return;
                }
                //取出识别头
                byte[] temp = new byte[mDistHeadSize];
                Array.Copy(mBufferedData, 0, temp, 0, mDistHeadSize);
                DistHead distHead = (DistHead)Converter.Bytes2Struct(temp, typeof(DistHead));
                OnDebug(LogMode.Info, string.Format("DistHead:{0}\t{1}", distHead.BaseHead, distHead.DataSize));
                int baseHead = distHead.BaseHead;
                int dataSize = distHead.DataSize;
                if (baseHead != NETPACK_BASETYPE_APPLICATION_VER1)
                {
                    OnDebug(LogMode.Error, string.Format("BaseHead invalid"));
                }
                else
                {
                    if (intSize < mDistHeadSize + mAppHeadSize + dataSize)
                    {
                        OnDebug(LogMode.Error, string.Format("Receive length invalid"));
                        return;
                    }
                    //取出基本头
                    byte[] baseData = new byte[mAppHeadSize];
                    Array.Copy(mBufferedData, mDistHeadSize, baseData, 0, mAppHeadSize);
                    AppHead appHead = (AppHead)Converter.Bytes2Struct(baseData, typeof(AppHead));
                    OnDebug(LogMode.Info, string.Format("AppHead:{0}\t{1}", appHead.Encrypt, appHead.Format));
                    //取出数据区
                    byte[] dataData = new byte[dataSize];
                    Array.Copy(mBufferedData, mDistHeadSize + mAppHeadSize, dataData, 0, dataSize);
                    string strMsg = Encoding.UTF8.GetString(dataData);
                    OnDebug(LogMode.Info, string.Format("Data message:{0}", strMsg));
                    JsonObject jsonObject = new JsonObject(strMsg);
                    if (jsonObject[NODE_NAME_PARAMCHANGE] != null)
                    {
                        if (jsonObject[NODE_NAME_PARAMCHANGE][NODE_NAME_MACHINEID] != null)
                        {
                            //获取MachineID
                            string strID = jsonObject[NODE_NAME_PARAMCHANGE][NODE_NAME_MACHINEID].Value;
                            if (strID == mLocalMachineID)
                            {
                                OnDebug(LogMode.Info, string.Format("Begin change config"));
                                //Change config
                                OnConfigChanged();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("CheckConfigChangeWorker fail.\t{0}", ex.Message));
            }
            try
            {
                socket.BeginReceive(mBufferedData, 0, ConstValue.NET_BUFFER_MAX_SIZE, SocketFlags.None,
                    CheckConfigChangeWorker, socket);
            }
            catch (ObjectDisposedException ex)
            {
                //Socket disposed
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("Begin receive fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region Debug

        public event Action<LogMode, string, string> Debug;

        private void OnDebug(LogMode mode, string category, string msg)
        {
            if (Debug != null)
            {
                Debug(mode, category, msg);
            }
        }

        private void OnDebug(LogMode mode, string msg)
        {
            OnDebug(mode, "ConfigChangeOpt", msg);
        }

        #endregion


        #region ConfigChangedEvent

        public event Action ConfigChanged;

        private void OnConfigChanged()
        {
            if (ConfigChanged != null)
            {
                ConfigChanged();
            }
        }

        #endregion


        #region Struct

        public const ushort NETPACK_BASETYPE_APPLICATION_VER1 = 0x0040;	// 应用层数据描述第1版

        public struct DistHead
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] Head;
            public byte Sequence;
            public byte PackType;
            public ushort FollowSize;
            public ulong Source;
            public ulong Target;
            public ulong Timestamp;
            public ushort BaseHead;
            public ushort BaseSize;
            public ushort ExtHead;
            public ushort ExtSize;
            public ushort DataSize;
            public ushort State;
            public ushort ModuleID;
            public ushort Number;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] Message;
        }

        public struct AppHead
        {
            public byte Channel;
            public byte Encrypt;
            public byte Compress;
            public byte Format;
            public ushort CodePage;
            public ushort Identify;
            public ushort DataSize;
            public ushort ValidSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] Reserved;
        }

        #endregion
    }
}
