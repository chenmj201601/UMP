//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2889c1e6-0108-4891-a210-a6fafd1567f6
//        CLR Version:              4.0.30319.18444
//        Name:                     WaveOutChannelMode
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave
//        File Name:                WaveOutChannelMode
//
//        created by Charley at 2014/12/8 16:13:02
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.NAudio.Wave
{
    /// <summary>
    /// WaveOut channel mode
    /// Added by Charley at 2013/4/9
    /// </summary>
    public enum WaveOutChannelMode
    {
        /// <summary>
        /// Not change each channel data.
        /// </summary>
        Default = 0,
        /// <summary>
        /// Will replace right channel by Zero.
        /// </summary>
        Left = 1,
        /// <summary>
        ///  Will replace left channel by Zero.
        /// </summary>
        Right = 2
    }
}
