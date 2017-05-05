//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6a0983b8-2b0a-4178-b33b-dc1444d08a28
//        CLR Version:              4.0.30319.18063
//        Name:                     NMonWriter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.SDKs.NMon
//        File Name:                NMonWriter
//
//        created by Charley at 2015/6/18 16:09:06
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.IO;
using VoiceCyber.NAudio.Wave;

namespace VoiceCyber.SDKs.NMon
{
    /// <summary>
    /// Util class to write wave data to file;
    /// Remember call PrepareWrite function first for setting wave format
    /// 将音频数据写入音频文件
    /// </summary>
    class NMonWriter
    {
        public event Action<string> Debug;

        private EVLVoiceFormat mFormat;
        private WaveFormat mSrcWaveFormat;
        private WaveFormat mPcmWaveFormat;
        private WaveFileWriter mSrcWriter;
        private WaveFileWriter mPcmWriter;

        public void PrepareWrite(EVLVoiceFormat format, bool isSrcWriteFile, bool isPcmWriteFile, string wavePath)
        {
            SubDebug(string.Format("Call PrepareWrite,source wave format:{0}", format));
            Stop();
            mFormat = format;
            switch (mFormat)
            {
                case EVLVoiceFormat.MT_MSGSM:
                    mSrcWaveFormat = new Gsm610WaveFormat();
                    break;
                case EVLVoiceFormat.MT_MP3_32K_STEREO:
                    mSrcWaveFormat = new Mp3WaveFormat(8000, 2, 288, 32000);
                    break;
                default:
                    SubDebug(string.Format("Format not support,source wave format:{0}", format));
                    return;
            }
            if (mSrcWaveFormat.Channels == 2)
            {
                mPcmWaveFormat = WaveFormat.CreateCustomFormat(WaveFormatEncoding.Pcm, 8000, 2, 32000, 4, 16);
                //mPcmWaveFormat = WaveFormat.CreateCustomFormat(WaveFormatEncoding.Pcm, 8000, 1, 16000, 2, 16);
            }
            else
            {
                mPcmWaveFormat = WaveFormat.CreateCustomFormat(WaveFormatEncoding.Pcm, 8000, 1, 16000, 2, 16);
            }
            if (!Path.IsPathRooted(wavePath))
            {
                wavePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, wavePath);
            }
            if (!Directory.Exists(wavePath))
            {
                Directory.CreateDirectory(wavePath);
            }
            string srcPath, pcmPath;
            srcPath = Path.Combine(wavePath, "Src_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".wav");
            pcmPath = Path.Combine(wavePath, "Pcm_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".wav");
            if (isSrcWriteFile)
            {
                mSrcWriter = new WaveFileWriter(srcPath, mSrcWaveFormat);
            }
            if (isPcmWriteFile)
            {
                mPcmWriter = new WaveFileWriter(pcmPath, mPcmWaveFormat);
            }
            SubDebug(string.Format("Writer prepared,source wave format:{0},dest wave format:{1}", mSrcWaveFormat, mPcmWaveFormat));
        }
        public void WriteSrcWaveData(byte[] data, int length)
        {
            if (mSrcWaveFormat == null)
            {
                SubDebug(string.Format("Source wave format is null"));
                return;
            }
            if (mSrcWriter == null)
            {
                SubDebug(string.Format("Source wave file writer is null"));
                return;
            }
            mSrcWriter.Write(data, 0, length);
        }
        public void WritePcmWaveData(byte[] data, int length)
        {
            if (mPcmWaveFormat == null)
            {
                SubDebug(string.Format("Pcm wave format is null"));
                return;
            }
            if (mPcmWriter == null)
            {
                SubDebug(string.Format("Pcm wave file writer is null"));
                return;
            }
            mPcmWriter.Write(data, 0, length);
        }
        public void Stop()
        {
            try
            {
                if (mSrcWriter != null)
                {
                    mSrcWriter.Dispose();
                    mSrcWriter = null;
                }
                if (mPcmWriter != null)
                {
                    mPcmWriter.Dispose();
                    mPcmWriter = null;
                }
            }
            catch (Exception ex)
            {
                SubDebug(string.Format("Writer stop fail.\t{0}", ex.Message));
            }
        }
        private void SubDebug(string msg)
        {
            if (Debug != null)
            {
                Debug(msg);
            }
        }
    }
}
