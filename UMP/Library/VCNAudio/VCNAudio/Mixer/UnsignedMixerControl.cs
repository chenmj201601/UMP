//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b98d54a4-99ff-47f3-add4-d7ebed4c0df2
//        CLR Version:              4.0.30319.18444
//        Name:                     UnsignedMixerControl
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Mixer
//        File Name:                UnsignedMixerControl
//
//        created by Charley at 2014/12/8 14:48:45
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Runtime.InteropServices;

namespace VoiceCyber.NAudio.Mixer
{
    /// <summary>
    /// Represents an unsigned mixer control
    /// </summary>
    public class UnsignedMixerControl : MixerControl
    {
        private MixerInterop.MIXERCONTROLDETAILS_UNSIGNED[] unsignedDetails;

        internal UnsignedMixerControl(MixerInterop.MIXERCONTROL mixerControl, IntPtr mixerHandle, MixerFlags mixerHandleType, int nChannels)
        {
            this.mixerControl = mixerControl;
            this.mixerHandle = mixerHandle;
            this.mixerHandleType = mixerHandleType;
            this.nChannels = nChannels;
            mixerControlDetails = new MixerInterop.MIXERCONTROLDETAILS();
            GetControlDetails();
        }

        /// <summary>
        /// Gets the details for this control
        /// </summary>
        protected override void GetDetails(IntPtr pDetails)
        {
            unsignedDetails = new MixerInterop.MIXERCONTROLDETAILS_UNSIGNED[nChannels];
            for (int channel = 0; channel < nChannels; channel++)
            {
                unsignedDetails[channel] = (MixerInterop.MIXERCONTROLDETAILS_UNSIGNED)Marshal.PtrToStructure(mixerControlDetails.paDetails, typeof(MixerInterop.MIXERCONTROLDETAILS_UNSIGNED));
            }
        }

        /// <summary>
        /// The control value
        /// </summary>
        public uint Value
        {
            get
            {
                GetControlDetails();
                return unsignedDetails[0].dwValue;
            }
            set
            {
                int structSize = Marshal.SizeOf(unsignedDetails[0]);

                mixerControlDetails.paDetails = Marshal.AllocHGlobal(structSize * nChannels);
                for (int channel = 0; channel < nChannels; channel++)
                {
                    unsignedDetails[channel].dwValue = value;
                    long pointer = mixerControlDetails.paDetails.ToInt64() + (structSize * channel);
                    Marshal.StructureToPtr(unsignedDetails[channel], (IntPtr)pointer, false);
                }
                MmException.Try(MixerInterop.mixerSetControlDetails(mixerHandle, ref mixerControlDetails, MixerFlags.Value | mixerHandleType), "mixerSetControlDetails");
                Marshal.FreeHGlobal(mixerControlDetails.paDetails);
            }
        }

        /// <summary>
        /// The control's minimum value
        /// </summary>
        public UInt32 MinValue
        {
            get
            {
                return (uint)mixerControl.Bounds.minimum;
            }
        }

        /// <summary>
        /// The control's maximum value
        /// </summary>
        public UInt32 MaxValue
        {
            get
            {
                return (uint)mixerControl.Bounds.maximum;
            }
        }

        /// <summary>
        /// Value of the control represented as a percentage
        /// </summary>
        public double Percent
        {
            get
            {
                return 100.0 * (Value - MinValue) / (MaxValue - MinValue);
            }
            set
            {
                Value = (uint)(MinValue + (value / 100.0) * (MaxValue - MinValue));
            }
        }

        /// <summary>
        /// String Representation for debugging purposes
        /// </summary>
        public override string ToString()
        {
            return String.Format("{0} {1}%", base.ToString(), Percent);
        }
    }
}
