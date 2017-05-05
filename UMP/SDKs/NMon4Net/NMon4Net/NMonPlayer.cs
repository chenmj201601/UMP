//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    180101e4-4098-4351-aa4a-0a0b61bc6a8f
//        CLR Version:              4.0.30319.18063
//        Name:                     NMonPlayer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.SDKs.NMon
//        File Name:                NMonPlayer
//
//        created by Charley at 2015/6/18 16:08:18
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Runtime.Serialization;
using VoiceCyber.NAudio.Wave;

namespace VoiceCyber.SDKs.NMon
{
    /// <summary>
    /// Play wave object;
    /// Remember,call Init function first for creating play device and setting wave format;
    /// This is a event driven object.
    /// 播放器，使用NAudio播放音频
    /// </summary>
    class NMonPlayer
    {
        public event Action<string> Debug;

        private WaveOut mWaveOut;
        private BufferedWaveProvider mBufferedWaveProvider;
        private EVLVoiceFormat mFormat;
        private WaveFormat mPcmWaveFormat;

        public void Init(EVLVoiceFormat format, float volume)
        {
            SubDebug(string.Format("Call NMonPlayer init,wave format:{0}", format));
            if (mWaveOut != null)
            {
                mWaveOut.Stop();
                mWaveOut.Dispose();
            }
            try
            {
                mWaveOut = new WaveOut();
                mFormat = format;
                switch (mFormat)
                {
                    case EVLVoiceFormat.MT_MSGSM:
                        mPcmWaveFormat = WaveFormat.CreateCustomFormat(WaveFormatEncoding.Pcm, 8000, 1, 16000, 2, 16);
                        break;
                    case EVLVoiceFormat.MT_MP3_32K_STEREO:
                        mPcmWaveFormat = WaveFormat.CreateCustomFormat(WaveFormatEncoding.Pcm, 8000, 2, 32000, 4, 16);
                        break;
                    default:
                        SubDebug(string.Format("Format not support,source wave format:{0}", mFormat));
                        return;
                }
                mBufferedWaveProvider = new BufferedWaveProvider(mPcmWaveFormat);
                mBufferedWaveProvider.DiscardOnBufferOverflow = true;
                mWaveOut.Init(mBufferedWaveProvider);
                if (volume >= 0 && volume <= 1)
                {
                    mWaveOut.Volume = volume;
                }
                else if (volume >= 2 && volume <= 3)
                {
                    volume = volume - 2;
                    mWaveOut.LeftVolume = volume;
                }
                else if (volume >= 4 && volume <= 5)
                {
                    volume = volume - 4;
                    mWaveOut.RightVolume = volume;
                }
                else
                {
                    SubDebug(string.Format("Volume invalid"));
                    mWaveOut.Volume = 1;
                }
                SubDebug(string.Format("WaveOut initialed,\tPlay wave format:{0}", mPcmWaveFormat));
            }
            catch (Exception ex)
            {
                SubDebug(string.Format("Wave out init fail.\t{0}", ex.Message));
            }
        }
        public void Play()
        {
            if (mWaveOut == null)
            {
                SubDebug(string.Format("Wave out is null"));
                return;
            }
            if (mPcmWaveFormat == null)
            {
                SubDebug(string.Format("Pcm wave format is null"));
                return;
            }
            SubDebug(string.Format("Call NMonPlayer play"));
            try
            {
                mWaveOut.Play();
            }
            catch (Exception ex)
            {
                SubDebug(string.Format("Wave out play fail.\t{0}", ex.Message));
            }
        }
        public void Stop()
        {
            try
            {
                SubDebug(string.Format("Call NMonPlayer stop"));
                if (mWaveOut == null)
                {
                    SubDebug(string.Format("Wave out is null"));
                    return;
                }
                mWaveOut.Stop();
                mWaveOut.Dispose();
                mWaveOut = null;
            }
            catch (Exception ex)
            {
                SubDebug(string.Format("Player stop fail.\t{0}", ex.Message));
            }
        }
        public void AddSample(byte[] data, int length)
        {
            mBufferedWaveProvider.AddSamples(data, 0, length);
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
