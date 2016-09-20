//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    f18e65d9-8aea-4209-aa9a-77a4e22e1267
//        CLR Version:              4.0.30319.18444
//        Name:                     CustomMixerControl
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Mixer
//        File Name:                CustomMixerControl
//
//        created by Charley at 2014/12/8 14:50:02
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.NAudio.Mixer
{
    /// <summary>
    /// Custom Mixer control
    /// </summary>
    public class CustomMixerControl : MixerControl
    {
        internal CustomMixerControl(MixerInterop.MIXERCONTROL mixerControl, IntPtr mixerHandle, MixerFlags mixerHandleType, int nChannels)
        {
            this.mixerControl = mixerControl;
            this.mixerHandle = mixerHandle;
            this.mixerHandleType = mixerHandleType;
            this.nChannels = nChannels;
            mixerControlDetails = new MixerInterop.MIXERCONTROLDETAILS();
            GetControlDetails();
        }

        /// <summary>
        /// Get the data for this custom control
        /// </summary>
        /// <param name="pDetails">pointer to memory to receive data</param>
        protected override void GetDetails(IntPtr pDetails)
        {
        }

        // TODO: provide a way of getting / setting data
    }
}
