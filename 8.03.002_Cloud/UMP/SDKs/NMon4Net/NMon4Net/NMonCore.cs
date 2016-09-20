//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    5b1276a5-64e3-4cee-ad94-ec21e66fa275
//        CLR Version:              4.0.30319.18063
//        Name:                     NMonCore
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.SDKs.NMon
//        File Name:                NMonCore
//
//        created by Charley at 2015/6/18 16:09:40
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using VoiceCyber.NAudio.Wave;

namespace VoiceCyber.SDKs.NMon
{
    /// <summary>
    /// Monitor object;
    /// Usage:NMonCore core=new NMonCore();
    /// Then do settings by its property,like core.IsPlayWave=true;
    /// You can call its StartMon function to monitor channel(assign monitor param); 
    /// This is an event driven object so you can binding event handler like DataReceived,HeadReceived,Debug; 
    /// Remember call StopMon function when stop monitor channel for dispose resources;
    /// 网络监听的核心类
    /// </summary>
    public class NMonCore
    {
        /// <summary>
        /// Debug message received
        /// </summary>
        public event Action<object, string> Debug;
        /// <summary>
        /// Voice head received which may contain voice format and so on
        /// </summary>
        public event Action<object, SNM_RESPONSE> HeadReceived;
        /// <summary>
        /// Voice data received
        /// </summary>
        public event Action<object, byte[], int> DataReceived;

        private object mUser;
        private NMonConnector mConnector;
        //private NMonConnector2 mConnector;
        private NMonPlayer mPlayer;
        private NMonConvertor mConvertor;
        private NMonWriter mWriter;

        private bool mIsConnectServer;
        private bool mIsPlayWave;
        private bool mIsDecodeData;
        private bool mIsSrcWriteFile;
        private bool mIsPcmWriteFile;
        private string mWaveDir;
        private float mVolume;

        /// <summary>
        /// Get or set user,user will return as first argument in each event
        /// </summary>
        public object User
        {
            get { return mUser; }
            set { mUser = value; }
        }
        /// <summary>
        /// Set connect to voice server or not
        /// </summary>
        public bool IsConnectServer
        {
            set { mIsConnectServer = value; }
        }
        /// <summary>
        /// Set play wave or not
        /// </summary>
        public bool IsPlayWave
        {
            set { mIsPlayWave = value; }
        }
        /// <summary>
        /// Set decoding voice data to pcm format or not,this format will be PCM 8kHz 16bit
        /// </summary>
        public bool IsDecodeData
        {
            set { mIsDecodeData = value; }
        }
        /// <summary>
        /// Set write source wave file or not
        /// </summary>
        public bool IsSourceWaveWriteFile
        {
            set { mIsSrcWriteFile = value; }
        }
        /// <summary>
        /// Set write pcm wave file or not
        /// </summary>
        public bool IsPcmWaveWriteFile
        {
            set { mIsPcmWriteFile = value; }
        }
        /// <summary>
        /// Set wave file save directory,relactively or absolutely,default is "WaveFiles" relactive to current domain path
        /// </summary>
        public string WaveDirectory
        {
            set { mWaveDir = value; }
        }
        /// <summary>
        /// Set the player's volume,for 0 ~ 1 indicate using mix channel,2 ~ 3 left channel,and 4 ~ 5 right channel
        /// </summary>
        public float Volume
        {
            set { mVolume = value; }
        }

        /// <summary>
        /// Get waveout device count of the system
        /// </summary>
        /// <returns></returns>
        public static int GetWaveOutDeviceCount()
        {
            return WaveOut.DeviceCount;
        }

        /// <summary>
        /// Create a NMonCore which will apply default setting.To change setting,please set its property
        /// </summary>
        public NMonCore()
        {
            mIsConnectServer = true;
            mIsDecodeData = true;
            mIsPlayWave = true;
            mIsSrcWriteFile = false;
            mIsPcmWriteFile = false;
            mWaveDir = "WaveFiles";
            mVolume = 1;

            mConnector = new NMonConnector();
            //mConnector = new NMonConnector2();
            mConnector.Debug += mConnector_Debug;
            mConnector.HeadReceived += mConnector_HeadReceived;
            mConnector.DataReceived += mConnector_DataReceived;

            mConvertor = new NMonConvertor();
            mConvertor.Debug += mConvertor_Debug;
            mConvertor.DataConverted += mConvertor_DataConverted;

            mPlayer = new NMonPlayer();
            mPlayer.Debug += mPlayer_Debug;

            mWriter = new NMonWriter();
            mWriter.Debug += mWriter_Debug;
        }
        /// <summary>
        /// Create a NMonCore with a user
        /// </summary>
        /// <param name="user"></param>
        public NMonCore(object user)
            : this()
        {
            mUser = user;
        }

        /// <summary>
        /// Start monitor channel
        /// </summary>
        /// <param name="param">Monitor param</param>
        public void StartMon(NETMON_PARAM param)
        {
            SubDebug(string.Format("Call NMonCore StartMon"));
            if (mIsConnectServer)
            {
                mConnector.StartMon(param);
            }
        }
        /// <summary>
        /// Stop monitor channel
        /// </summary>
        public void StopMon()
        {
            SubDebug(string.Format("Call NMonCore StopMon"));
            mConnector.StopMon();
            mPlayer.Stop();
            mConvertor.Stop();
            mWriter.Stop();
        }
        /// <summary>
        /// Received head info,call this function will prepare convert,play or write,must be called before ReceivedData
        /// </summary>
        /// <param name="response"></param>
        public void ReceiveHead(SNM_RESPONSE response)
        {
            SubDebug(string.Format("Head received,voice format:{0}\tchannel:{1}", (EVLVoiceFormat)response.format, response.channel));
            SubHeadReceived(response);
            if (mIsDecodeData)
            {
                PrepareConvert((EVLVoiceFormat)response.format);
            }
            if (mIsPlayWave)
            {
                PreparePlay((EVLVoiceFormat)response.format);
            }
            if (mIsSrcWriteFile || mIsPcmWriteFile)
            {
                PrepareWrite((EVLVoiceFormat)response.format);
            }
        }
        /// <summary>
        /// Receive voice data,call ReceiveHead before it
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        public void ReceiveData(byte[] data, int length)
        {
            if (!mIsDecodeData)
            {
                SubDataReceived(data, length);
                if (mIsSrcWriteFile)
                {
                    //mWriter.WriteSrcWaveData(data, length);
                    mWriter.WritePcmWaveData(data, length);
                }
                if (mIsPlayWave)
                {
                    mPlayer.AddSample(data, length);
                }
                return;
            }
            if (mIsSrcWriteFile)
            {
                mWriter.WriteSrcWaveData(data, length);
            }
            mConvertor.ConvertData(data, length);
        }
        private void PrepareConvert(EVLVoiceFormat format)
        {
            SubDebug(string.Format("Call PrepareConvert,voice format:{0}.", format));
            if (mIsDecodeData)
            {
                mConvertor.PrepareConvert(format);
            }
        }
        private void PreparePlay(EVLVoiceFormat format)
        {
            SubDebug(string.Format("Call PreparePlay,voice format:{0}.", format));
            if (mIsPlayWave)
            {
                mPlayer.Init(format, mVolume);
                mPlayer.Play();
            }
        }
        private void PrepareWrite(EVLVoiceFormat format)
        {
            SubDebug(string.Format("Call PrepareWrite,voice format:{0}.", format));
            if (mIsSrcWriteFile || mIsPcmWriteFile)
            {
                mWriter.PrepareWrite(format, mIsSrcWriteFile, mIsPcmWriteFile, mWaveDir);
            }
        }
        void mConnector_HeadReceived(SNM_RESPONSE response)
        {
            ReceiveHead(response);
        }
        void mConnector_DataReceived(byte[] data, int length)
        {
            ReceiveData(data, length);
        }
        void mConvertor_DataConverted(byte[] data, int length)
        {
            SubDataReceived(data, length);
            if (mIsPlayWave)
            {
                mPlayer.AddSample(data, length);
            }
            if (mIsPcmWriteFile)
            {
                mWriter.WritePcmWaveData(data, length);
            }
        }

        void mConnector_Debug(string msg)
        {
            SubDebug(string.Format("[NMonConnector]\t{0}", msg));
        }
        void mConvertor_Debug(string msg)
        {
            SubDebug(string.Format("[NMonConvertor]\t{0}", msg));
        }
        void mPlayer_Debug(string msg)
        {
            SubDebug(string.Format("[NMonPlayer ]\t{0}", msg));
        }
        void mWriter_Debug(string msg)
        {
            SubDebug(string.Format("[NMonWriter ]\t{0}", msg));
        }
        private void SubDebug(string msg)
        {
            if (Debug != null)
            {
                Debug(mUser, msg);
            }
        }
        private void SubHeadReceived(SNM_RESPONSE resonse)
        {
            if (HeadReceived != null)
            {
                HeadReceived(mUser, resonse);
            }
        }
        private void SubDataReceived(byte[] data, int length)
        {
            if (DataReceived != null)
            {
                DataReceived(mUser, data, length);
            }
        }
    }
}
