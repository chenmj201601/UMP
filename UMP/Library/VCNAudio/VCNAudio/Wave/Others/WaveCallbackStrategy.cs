//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b5334887-73e9-4d99-b665-54de035a2d53
//        CLR Version:              4.0.30319.18444
//        Name:                     WaveCallbackStrategy
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave
//        File Name:                WaveCallbackStrategy
//
//        created by Charley at 2014/12/8 15:53:47
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.NAudio.Wave
{
    /// <summary>
    /// Wave Callback Strategy
    /// </summary>
    public enum WaveCallbackStrategy
    {
        /// <summary>
        /// Use a function
        /// </summary>
        FunctionCallback,
        /// <summary>
        /// Create a new window (should only be done if on GUI thread)
        /// </summary>
        NewWindow,
        /// <summary>
        /// Use an existing window handle
        /// </summary>
        ExistingWindow,
        /// <summary>
        /// Use an event handle
        /// </summary>
        Event,
    }
}
