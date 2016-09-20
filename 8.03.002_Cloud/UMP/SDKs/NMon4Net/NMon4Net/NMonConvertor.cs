//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    88cb5e1a-ede3-40d0-b35b-54ad50481800
//        CLR Version:              4.0.30319.18063
//        Name:                     NMonConvertor
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.SDKs.NMon
//        File Name:                NMonConvertor
//
//        created by Charley at 2015/6/18 16:07:03
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using VoiceCyber.NAudio.Wave;

namespace VoiceCyber.SDKs.NMon
{
    /// <summary>
    /// Util class to convert wave data to pcm wave.
    /// Remember,call PrepareConvert function first for setting source and pcm wave format
    /// This is a event driven class.
    /// </summary>
    class NMonConvertor
    {
        public event Action<string> Debug;
        public event Action<byte[], int> DataConverted;

        private EVLVoiceFormat mFormat;
        private WaveFormat mPcmWaveFormat;
        private WaveFormat mSrcWaveFormat;
        private BufferedWaveStream mBufferedStream;
        private WaveFormatConversionStream mConvStream;

        public void PrepareConvert(EVLVoiceFormat format)
        {
            SubDebug(string.Format("Call PrepareConvert,source wave format:{0}", format));
            Stop();
            try
            {
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
                        SubDebug(string.Format("Format not support,source wave format:{0}", mFormat));
                        return;
                }
                if (mSrcWaveFormat.Channels == 2)
                {
                    mPcmWaveFormat = WaveFormat.CreateCustomFormat(WaveFormatEncoding.Pcm, 8000, 2, 32000, 4, 16);
                }
                else
                {
                    mPcmWaveFormat = WaveFormat.CreateCustomFormat(WaveFormatEncoding.Pcm, 8000, 1, 16000, 2, 16);
                }
                mBufferedStream = new BufferedWaveStream(mSrcWaveFormat);
                mConvStream = new WaveFormatConversionStream(mPcmWaveFormat, mBufferedStream);
                SubDebug(string.Format("Convertor prepared,source wave format:{0},dest wave format:{1}", mSrcWaveFormat, mPcmWaveFormat));
            }
            catch (Exception ex)
            {
                SubDebug(string.Format("PrepareConvert fail.\t{0}", ex.Message));
            }
        }

        public void ConvertData(byte[] data, int length)
        {
            if (mSrcWaveFormat == null)
            {
                SubDebug(string.Format("Source wave format is null"));
                return;
            }
            if (mPcmWaveFormat == null)
            {
                SubDebug(string.Format("Pcm wave format is null"));
                return;
            }
            if (mBufferedStream == null)
            {
                SubDebug(string.Format("Buffered wave stream is null"));
                return;
            }
            if (mConvStream == null)
            {
                SubDebug(string.Format("Conversion wave stream is null"));
                return;
            }

            try
            {
                mBufferedStream.Write(data, 0, length);
                byte[] buffer = new byte[mPcmWaveFormat.AverageBytesPerSecond / 5];
                while (true)
                {
                    int bytesRead = mConvStream.Read(buffer, 0, buffer.Length);
                    //Return 0 means we have read all avaliable data
                    if (bytesRead == 0)
                        break;
                    SubDataConverted(buffer, bytesRead);
                }
            }
            catch (Exception ex)
            {
                SubDebug(string.Format("Convert data fail.\t{0}", ex.Message));
            }
        }

        private void SubDebug(string msg)
        {
            if (Debug != null)
            {
                Debug(msg);
            }
        }
        private void SubDataConverted(byte[] data, int length)
        {
            if (DataConverted != null)
            {
                DataConverted(data, length);
            }
        }
        public void Stop()
        {
            try
            {
                if (mBufferedStream != null)
                {
                    mBufferedStream.Dispose();
                    mBufferedStream = null;
                }
                if (mConvStream != null)
                {
                    mConvStream.Dispose();
                    mConvStream = null;
                }
            }
            catch (Exception ex)
            {
                SubDebug(string.Format("Converter stop fail.\t{0}", ex.Message));
            }
        }
    }
}
