//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    f9b3ca8c-4e57-4764-b9a7-7e1a3d911a3c
//        CLR Version:              4.0.30319.18063
//        Name:                     DirectXPlayer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                NMon4NetDemo
//        File Name:                DirectXPlayer
//
//        created by Charley at 2015/6/18 16:14:06
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.IO;

namespace NMon4NetDemo
{
    class DirectXPlayer
    {
        private const int BUFFERSIZE = 1024000; //1000K

        public event Action<string> Debug;

        private IntPtr mHandler;
        //private Device I_PlayDevice;
        //private BufferDescription I_BufferDesc;
        private MemoryStream I_MemoryStream;
        private int I_WritePos;
        private int I_PlayPos;
        private int I_NotifySize;

        public DirectXPlayer(IntPtr handler)
        {
            mHandler = handler;
        }
        //public void Prepare(EVLVoiceFormat format)
        //{
        //    if (!CreatePlayDevice())
        //    {
        //        return;
        //    }
        //    PrepareBuffer(format);
        //}
        //public void Stop()
        //{
        //    if (I_PlayDevice != null)
        //    {
        //        I_PlayDevice.Dispose();
        //        I_PlayDevice = null;
        //    }
        //    if (I_BufferDesc != null)
        //    {
        //        I_BufferDesc.Dispose();
        //        I_BufferDesc = null;
        //    }
        //    if (I_MemoryStream != null)
        //    {
        //        I_MemoryStream.Dispose();
        //        I_MemoryStream = null;
        //    }
        //}
        //public void AddSamples(byte[] data, int length)
        //{
        //    GetVoiceData(length, data);
        //}
        //private void PrepareBuffer(EVLVoiceFormat format)
        //{
        //    I_BufferDesc = new BufferDescription();
        //    Microsoft.DirectX.DirectSound.WaveFormat mWavFormat = SetWaveFormat(format);
        //    I_BufferDesc.Format = mWavFormat;
        //    I_NotifySize = mWavFormat.AverageBytesPerSecond * 2;
        //    I_MemoryStream = new MemoryStream(BUFFERSIZE);
        //    I_WritePos = 0;
        //    I_PlayPos = 0;
        //}
        //private bool CreatePlayDevice()
        //{
        //    DevicesCollection dc = new DevicesCollection();
        //    Guid g;
        //    if (dc.Count > 0)
        //    {
        //        g = dc[0].DriverGuid;
        //    }
        //    else
        //    {
        //        Debug(string.Format("No play device."));
        //        return false;
        //    }
        //    I_PlayDevice = new Device(g);
        //    I_PlayDevice.SetCooperativeLevel(mHandler, CooperativeLevel.Normal);
        //    return true;
        //}
        //public void GetVoiceData(int length, byte[] data)
        //{
        //    //收到的数据写入内存流
        //    if (I_PlayPos - I_WritePos > 0 && I_PlayPos - I_WritePos < BUFFERSIZE / 2)
        //    {
        //        throw new Exception("Buffer is full");
        //    }
        //    if (I_WritePos + length <= BUFFERSIZE)
        //    {
        //        I_MemoryStream.Position = I_WritePos;
        //        I_MemoryStream.Write(data, 0, length);
        //        I_WritePos += length;
        //    }
        //    else
        //    {
        //        int rest = BUFFERSIZE - I_WritePos;
        //        I_MemoryStream.Position = I_WritePos;
        //        I_MemoryStream.Write(data, 0, rest);
        //        I_WritePos = length - rest;
        //        I_MemoryStream.Write(data, rest, I_WritePos);
        //    }

        //    while (true)
        //    {
        //        if (I_PlayPos <= I_WritePos)
        //        {
        //            //如果达到可以一次播放的数据长度时，就可以播放这段数据了
        //            if (I_PlayPos + I_NotifySize <= I_WritePos)
        //            {
        //                I_BufferDesc.BufferBytes = I_NotifySize;
        //                SecondaryBuffer buffer = new SecondaryBuffer(I_BufferDesc, I_PlayDevice);
        //                I_MemoryStream.Position = I_PlayPos;
        //                buffer.Write(0, I_MemoryStream, I_NotifySize, LockFlag.FromWriteCursor);
        //                buffer.Play(0, BufferPlayFlags.Default);
        //                I_PlayPos += I_NotifySize;
        //            }
        //            else
        //            {
        //                break;
        //            }
        //        }
        //        else
        //        {
        //            if (BUFFERSIZE - I_PlayPos + I_WritePos >= I_NotifySize)
        //            {
        //                byte[] temp = new byte[I_NotifySize];
        //                int rest = BUFFERSIZE - I_PlayPos;
        //                I_MemoryStream.Position = I_PlayPos;
        //                I_MemoryStream.Read(temp, 0, rest);
        //                I_MemoryStream.Position = 0;
        //                I_MemoryStream.Read(temp, rest, I_NotifySize - rest);
        //                I_BufferDesc.BufferBytes = I_NotifySize;
        //                SecondaryBuffer buffer = new SecondaryBuffer(I_BufferDesc, I_PlayDevice);
        //                buffer.Write(0, temp, LockFlag.FromWriteCursor);
        //                buffer.Play(0, BufferPlayFlags.Default);
        //                I_PlayPos = I_NotifySize - rest;
        //            }
        //            else
        //            {
        //                break;
        //            }
        //        }
        //    }
        //}
        //private Microsoft.DirectX.DirectSound.WaveFormat SetWaveFormat(EVLVoiceFormat format)
        //{
        //    switch (format)
        //    {
        //        case EVLVoiceFormat.MT_MSGSM:
        //            return SetMonoWaveFormat();
        //        case EVLVoiceFormat.MT_MP3_32K_STEREO:
        //            return SetSteroWaveFormat();
        //        default:
        //            Debug("Format not support");
        //            return SetMonoWaveFormat();
        //    }
        //}
        //private Microsoft.DirectX.DirectSound.WaveFormat SetSteroWaveFormat()
        //{
        //    Microsoft.DirectX.DirectSound.WaveFormat format = new Microsoft.DirectX.DirectSound.WaveFormat();
        //    format.FormatTag = WaveFormatTag.Pcm;//设置音频类型
        //    format.SamplesPerSecond = 8000;//采样率（单位：赫兹）典型值：11025、22050、44100Hz
        //    format.BitsPerSample = 16;//采样位数
        //    format.Channels = 2;//声道
        //    format.BlockAlign = (short)(format.Channels * (format.BitsPerSample / 8));//单位采样点的字节数
        //    format.AverageBytesPerSecond = format.BlockAlign * format.SamplesPerSecond;
        //    return format;
        //    //按照以上采样规格，可知采样1秒钟的字节数为22050*2=44100B 约为 43K
        //}
        //private Microsoft.DirectX.DirectSound.WaveFormat SetMonoWaveFormat()
        //{
        //    Microsoft.DirectX.DirectSound.WaveFormat format = new Microsoft.DirectX.DirectSound.WaveFormat();
        //    format.FormatTag = WaveFormatTag.Pcm;//设置音频类型
        //    format.SamplesPerSecond = 8000;//采样率（单位：赫兹）典型值：11025、22050、44100Hz
        //    format.BitsPerSample = 16;//采样位数
        //    format.Channels = 1;//声道
        //    format.BlockAlign = (short)(format.Channels * (format.BitsPerSample / 8));//单位采样点的字节数
        //    format.AverageBytesPerSecond = format.BlockAlign * format.SamplesPerSecond;
        //    return format;
        //    //按照以上采样规格，可知采样1秒钟的字节数为22050*2=44100B 约为 43K
        //}
        private void SubDebug(string msg)
        {
            if (Debug != null)
            {
                Debug(msg);
            }
        }
    }
}
