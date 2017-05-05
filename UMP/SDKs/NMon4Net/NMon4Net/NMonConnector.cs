//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    5b5855a6-a4a2-4388-a8eb-485f26b9e5ee
//        CLR Version:              4.0.30319.18063
//        Name:                     NMonConnector
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.SDKs.NMon
//        File Name:                NMonConnector
//
//        created by Charley at 2015/6/18 16:04:05
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using VoiceCyber.Common;

namespace VoiceCyber.SDKs.NMon
{
    /// <summary>
    /// 建立与录音服务器的连接，并接收服务器发来的音频数据
    /// HeadReceived事件报告已经成功获取通道信息（编码格式等）
    /// DataReceived事件报告已经接收到一部分音频数据
    /// </summary>
    class NMonConnector
    {
        public event Action<string> Debug;
        public event Action<byte[], int> DataReceived;
        public event Action<SNM_RESPONSE> HeadReceived;

        private Thread mReceiveThread;
        private Socket mClient;
        private NETMON_PARAM mNetMonParam;
        private SNM_RESPONSE mHeader;
        private bool mConnected;
        private bool mInit;                         //For true,connector will send request to server.
        private bool mReceiveHead;                  //For true,connector will receive voice format info,else voice data.
        private int mBlockSize;                     //For each voice format,receive size per time not equal. 

        public NMonConnector()
        {
            mConnected = false;
            mInit = true;
            mReceiveHead = true;
            //mBlockSize = 325;
            mNetMonParam = new NETMON_PARAM();
            mNetMonParam.Host = string.Empty;
            mNetMonParam.Port = NMonDefines.CONNECTOR_SERVERPORT;
        }

        public bool StartMon(NETMON_PARAM param)
        {
            SubDebug(string.Format("Call NMonConnector StartMon,host:{0}:{1}\tchannel:{2}.", param.Host, param.Port, param.Channel));
            mNetMonParam = param;
            if (mReceiveThread == null)
            {
                mReceiveThread = new Thread(Worker);
                mReceiveThread.Start();
                SubDebug(string.Format("Receive Thread Start,Thread ID:{0}.", mReceiveThread.ManagedThreadId));
            }
            return true;
        }

        public bool StopMon()
        {
            try
            {
                SubDebug(string.Format("Call NMonConnector StopMon."));
                if (mReceiveThread != null)
                {
                    mReceiveThread.Abort();
                    mReceiveThread = null;
                    SubDebug(string.Format("Receive Thread aborted."));
                }
                if (mClient != null)
                {
                    mClient.Close();
                    mClient = null;
                    SubDebug(string.Format("Socket client closed."));
                }
                mConnected = false;
                mInit = true;
                mReceiveHead = true;
                SubDebug(string.Format("Monitor stopped."));
                return true;
            }
            catch (Exception ex)
            {
                SubDebug(string.Format("Connector stop fail.\t{0}", ex.Message));
                return false;
            }
        }

        private void MakeConnection()
        {
            try
            {
                mClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //mClient.Connect(new IPEndPoint(IPAddress.Parse(mNetMonParam.Host), mNetMonParam.Port));
                mClient.Connect(mNetMonParam.Host, mNetMonParam.Port);
                if (mClient.Connected)
                {
                    SubDebug(string.Format("Server connected.Host:{0}\tPort:{1}\tClient:{2}", mNetMonParam.Host, mNetMonParam.Port, mClient.LocalEndPoint));
                    mConnected = true;
                }
                else
                {
                    SubDebug(string.Format("Connect to server fail,host:{0}\tport:{1}", mNetMonParam.Host, mNetMonParam.Port));
                    mConnected = false;
                }
            }
            catch (Exception ex)
            {
                SubDebug(string.Format("Connect to server fail,host:{0}\tport:{1}.\r\n{2}", mNetMonParam.Host, mNetMonParam.Port, ex.Message));
                mConnected = false;
            }
        }

        private void Worker()
        {
            int iInteval = NMonDefines.CONNECTOR_RECONINTEVAL * 100;
            int iRet;
            byte[] bBuffer = new byte[NMonDefines.CONNECTOR_BUFFERSIZE];
            while (true)
            {
                //Not connected
                if (!mConnected || mClient == null || !mClient.Connected)
                {
                    //reconnect to server per inteval
                    mInit = true;
                    mReceiveHead = true;
                    if (iInteval == NMonDefines.CONNECTOR_RECONINTEVAL * 100)
                    {
                        MakeConnection();
                        iInteval = 0;
                    }
                    else
                    {
                        iInteval++;
                    }
                }
                else
                {
                    //Not requested,so first send request to server
                    if (mInit)
                    {
                        SubDebug(string.Format("Send request start."));
                        SNM_REQUEST request = new SNM_REQUEST();
                        request.channel = (ushort)mNetMonParam.Channel;
                        request.flag = NMonDefines.NM_KEEP_ON_IDLE;
                        request.size = (ushort)Marshal.SizeOf(request);
                        if (SendData(Converter.Struct2Bytes(request)))
                        {
                            mInit = false;
                        }
                    }
                    else
                    {
                        //Check for disconnect
                        if (mClient.Poll(-1, SelectMode.SelectRead))
                        {
                            if (mClient.Available == 0)
                            {
                                SubDebug(string.Format("Server disconnected,Connector will reconnect later."));
                                mConnected = false;
                            }
                        }
                        while (mClient.Available > 0)
                        {
                            bBuffer.Initialize();
                            //Accept voice format info
                            if (mReceiveHead)
                            {
                                SubDebug(string.Format("Receive voice header start."));
                                int iHeadLenght = Marshal.SizeOf(typeof(SNM_RESPONSE));
                                iRet = mClient.Receive(bBuffer, iHeadLenght, SocketFlags.None);
                                if (iRet == iHeadLenght)
                                {
                                    byte[] temp = new byte[iRet];
                                    Array.Copy(bBuffer, temp, iRet);
                                    SNM_RESPONSE response = (SNM_RESPONSE)Converter.Bytes2Struct(temp, typeof(SNM_RESPONSE));
                                    PrepareReceive(response);
                                    SubHeadReceived(response);
                                    mReceiveHead = false;
                                }
                                else
                                {
                                    SubDebug(string.Format("Receive head data fail.Received length:{0}\tPossible length:{1}", iRet, iHeadLenght));
                                }
                            }
                            else
                            {
                                iRet = 0;
                                while (iRet < mBlockSize)
                                {
                                    int iTemp = mClient.Receive(bBuffer, iRet, mBlockSize - iRet, SocketFlags.None);
                                    iRet += iTemp;
                                    Thread.Sleep(10);
                                }
                                //iRet = mClient.Receive(bBuffer, mBlockSize, SocketFlags.None);
                                byte[] temp = new byte[iRet];
                                Array.Copy(bBuffer, temp, iRet);
                                SubDataReceived(temp, iRet);
                                //SubDataReceived(bBuffer, mBlockSize);
                            }
                        }
                    }
                }
                Thread.Sleep(10);
            }
        }

        private void PrepareReceive(SNM_RESPONSE response)
        {
            mHeader = response;
            SubDebug(string.Format("Call PrepareReceive,Current voice format:{0}.", (EVLVoiceFormat)response.format));
            switch (mHeader.format)
            {
                case (int)EVLVoiceFormat.MT_MSGSM:
                    mBlockSize = 325;
                    break;
                case (int)EVLVoiceFormat.MT_PCM_Raw_16bit:
                    mBlockSize = 3200;
                    break;
                case (int)EVLVoiceFormat.MT_PCM_Raw_u8bit:
                    mBlockSize = 1600;
                    break;
                case (int)EVLVoiceFormat.MT_OKI_ADPCM_SR8:
                    mBlockSize = 800;
                    break;
                case (int)EVLVoiceFormat.MT_G729A_8K:
                    mBlockSize = 200;
                    break;
                case (int)EVLVoiceFormat.MT_PCM_ALaw_Stereo:
                    mBlockSize = 3200;
                    break;
                case (int)EVLVoiceFormat.MT_MP3_32K_STEREO:
                    mBlockSize = 1608;
                    break;
                //Other voice format not impliment so on.

                default:
                    SubDebug(string.Format("Wave format not support,wave format:{0}", (EVLVoiceFormat)mHeader.format));
                    mBlockSize = 325;
                    return;
            }
        }

        private bool SendData(byte[] data)
        {
            try
            {
                if (!mConnected || mClient == null || !mClient.Connected)
                {
                    return false;
                }
                mClient.Send(data);
                SubDebug(string.Format("Send data to server,data length:{0}.", data.Length));
            }
            catch (Exception ex)
            {
                SubDebug(string.Format("Send data fail.\r\n{0}", ex.Message));
                return false;
            }
            return true;
        }

        private void SubDebug(string msg)
        {
            if (Debug != null)
            {
                Debug(msg);
            }
        }
        private void SubDataReceived(byte[] data, int length)
        {
            if (DataReceived != null)
            {
                DataReceived(data, length);
            }
        }
        private void SubHeadReceived(SNM_RESPONSE response)
        {
            if (HeadReceived != null)
            {
                HeadReceived(response);
            }
        }
    }
}
