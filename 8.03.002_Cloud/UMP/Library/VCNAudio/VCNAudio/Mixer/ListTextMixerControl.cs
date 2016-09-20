//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    8761e52c-6c07-4415-8622-de9adbfcc4d2
//        CLR Version:              4.0.30319.18444
//        Name:                     ListTextMixerControl
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Mixer
//        File Name:                ListTextMixerControl
//
//        created by Charley at 2014/12/8 14:49:23
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.NAudio.Mixer
{
    /// <summary>
    /// List text mixer control
    /// </summary>
    public class ListTextMixerControl : MixerControl
    {
        internal ListTextMixerControl(MixerInterop.MIXERCONTROL mixerControl, IntPtr mixerHandle, MixerFlags mixerHandleType, int nChannels)
        {
            this.mixerControl = mixerControl;
            this.mixerHandle = mixerHandle;
            this.mixerHandleType = mixerHandleType;
            this.nChannels = nChannels;
            mixerControlDetails = new MixerInterop.MIXERCONTROLDETAILS();

            GetControlDetails();

        }

        /// <summary>
        /// Get the details for this control
        /// </summary>
        /// <param name="pDetails">Memory location to read to</param>
        protected override void GetDetails(IntPtr pDetails)
        {
        }

        // TODO: provide a way of getting / setting data
    }
}
