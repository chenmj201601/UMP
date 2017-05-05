//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    528fc0c4-6d25-416a-ad83-a109099511a3
//        CLR Version:              4.0.30319.18063
//        Name:                     Structs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.SDKs.NMon
//        File Name:                Structs
//
//        created by Charley at 2015/6/18 16:01:24
//        http://www.voicecyber.com 
//
//======================================================================

using System.Net;
using System.Runtime.InteropServices;

namespace VoiceCyber.SDKs.NMon
{
    /// <summary>
    /// /************ Voice format for record & play ***************/
    /// </summary>
    public enum EVLVoiceFormat
    {
        MT_PCM_uLaw = 0x100,   // u-Law 8 KHz 8-bit PCM 64000 bps
        MT_PCM_ALaw = 0x110,   // A-Law 8 KHz 8-bit PCM 64000 bps
        MT_PCM_Raw_8bit = 0x120,  // Raw 8 KHz 8-bit PCM 64000 bps
        MT_PCM_Raw_16bit = 0x130, // Raw 8 KHz 16-bit PCM 128000 bps
        MT_PCM_Raw_u8bit = 0x140,// Raw 8 KHz unsigned 8-bit PCM 64000 bps
        MT_PCM_Raw_u16bit = 0x150,   // Raw 8 KHz unsigned 16-bit PCM 128000 bps
        MT_PCM_Raw6K_16bit = 0x160,  // Raw 6 KHz 16-bit PCM 96000 bps
        MT_PCM_ALaw_Stereo = 0x180, // A-Law 8 KHz 8-bit stereo PCM 128000 bps (record only)
        // Old definition
        MT_PCM = MT_PCM_uLaw,
        MT_LINEAR = MT_PCM_Raw_16bit,
        MT_OKI_ADPCM_SR8 = 0x200,   // Dialogic ADPCM 32000 bps, 8 KHz sample rate
        MT_OKI_ADPCM_SR6 = 0x210,  // Dialogic ADPCM 24000 bps, 6 KHz sample rate
        MT_G729_8K = 0x300, // G.729 8000 bps, Not supported 10/08/2002
        MT_G729A_8K = 0x310,   // G.729A 8000 bps
        MT_GSM610 = 0x400,  // GSM 6.10 13000 bps recorded into 33 bytes per 20ms
        MT_MSGSM = 0x410, // Microsoft GSM
        MT_G726_16K = 0x500,// G.726 16000 bps
        MT_G726_16K_uLaw = 0x510,   // G.726 16000 bps for u-Law
        MT_G726_16K_ALaw = 0x520,  // G.726 16000 bps for A-Law
        MT_G726_24K = 0x540, // G.726 24000 bps
        MT_G726_24K_uLaw = 0x550,// G.726 24000 bps for u-Law
        MT_G726_24K_ALaw = 0x560,   // G.726 24000 bps for A-Law
        MT_G726_32K = 0x580,  // G.726 32000 bps
        MT_G726_32K_uLaw = 0x590, // G.726 32000 bps for u-Law
        MT_G726_32K_ALaw = 0x5A0,// G.726 32000 bps for A-Law
        MT_G726_40K = 0x5C0,  // G.726 40000 bps
        MT_G726_40K_uLaw = 0x5D0,  // G.726 40000 bps for u-Law
        MT_G726_40K_ALaw = 0x5E0,  // G.726 40000 bps for A-Law
        MT_DATA8BIT_LSB_1ST = 0x600,	// Data 8 bit LSB first
        MT_DATA8BIT_MSB_1ST = 0x610,	// Data 8 bit MSB first
        MT_MP3_32K_STEREO = 0x900, // MP3 32 KHz 8-bit stereo( Experimenting )
        MT_DSP_SPEECH = 0x910,  // DSP Group TrueSpeech(TM) 8.000 kHz, 1 Bit, Mono
    }
    /// <summary>
    /// Monitor param.Voice address,port and channel
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct NETMON_PARAM
    {
        /// <summary>
        /// Voice address,ip or machine name
        /// </summary>
        public string Host;

        /// <summary>
        /// Monitor port,default is 3002
        /// </summary>
        public int Port;

        /// <summary>
        /// Channel 
        /// </summary>
        public int Channel;

    }
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct NETMON_VOICECONNECTINFO
    {
        public bool Connected;
        public string Message;
        public NETMON_PARAM Param;
        public EndPoint LocalEndPoint;
    }
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct SNM_REQUEST
    {

        public ushort size;

        public ushort channel;

        public ushort flag;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct SNM_RESPONSE
    {

        public ushort size;

        public ushort channel;

        public ushort result;

        public ushort format;
    }
}
