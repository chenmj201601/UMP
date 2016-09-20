//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b3efeedf-0e84-4701-b0be-5df9dd5bfe4d
//        CLR Version:              4.0.30319.18063
//        Name:                     NAudioPlayer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                NMon4NetDemo
//        File Name:                NAudioPlayer
//
//        created by Charley at 2015/6/18 16:16:23
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using VoiceCyber.NAudio.Wave;
using VoiceCyber.SDKs.NMon;

namespace NMon4NetDemo
{
    class NAudioPlayer
    {
        public event Action<string> Debug;

        private WaveOut mWaveOut;
        private BufferedWaveProvider mBufferedWaveProvider;

        public void Prepare(EVLVoiceFormat format)
        {
            InitWaveOut(format);
            Play();
        }
        public void Stop()
        {
            if (mWaveOut != null)
            {
                mWaveOut.Stop();
                mWaveOut.Dispose();
                mWaveOut = null;
            }
        }
        public void AddSamples(byte[] data, int length)
        {
            mBufferedWaveProvider.AddSamples(data, 0, length);
        }
        private void InitWaveOut(EVLVoiceFormat format)
        {
            if (mWaveOut != null)
            {
                Stop();
            }
            WaveFormat waveFormat;
            switch (format)
            {
                case EVLVoiceFormat.MT_MSGSM:
                    waveFormat = WaveFormat.CreateCustomFormat(WaveFormatEncoding.Pcm, 8000, 1, 16000, 2, 16);
                    break;
                case EVLVoiceFormat.MT_MP3_32K_STEREO:
                    waveFormat = WaveFormat.CreateCustomFormat(WaveFormatEncoding.Pcm, 8000, 2, 32000, 4, 16);
                    break;
                default:
                    Debug("Format not support");
                    return;
            }
            mWaveOut = new WaveOut();
            mBufferedWaveProvider = new BufferedWaveProvider(waveFormat);
            mWaveOut.Init(mBufferedWaveProvider);
        }
        private void Play()
        {
            if (mWaveOut != null)
            {
                mWaveOut.Play();
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
