//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    211873a2-fd02-4178-b28a-84dc0e72a7d9
//        CLR Version:              4.0.30319.18444
//        Name:                     WaveFormatCustomMarshaler
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave
//        File Name:                WaveFormatCustomMarshaler
//
//        created by Charley at 2014/12/8 16:00:23
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Runtime.InteropServices;

namespace VoiceCyber.NAudio.Wave
{
    /// <summary>
    /// Custom marshaller for WaveFormat structures
    /// </summary>
    public sealed class WaveFormatCustomMarshaler : ICustomMarshaler
    {
        private static WaveFormatCustomMarshaler marshaler;

        /// <summary>
        /// Gets the instance of this marshaller
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static ICustomMarshaler GetInstance(string cookie)
        {
            if (marshaler == null)
            {
                marshaler = new WaveFormatCustomMarshaler();
            }
            return marshaler;
        }

        /// <summary>
        /// Clean up managed data
        /// </summary>
        public void CleanUpManagedData(object ManagedObj)
        {

        }

        /// <summary>
        /// Clean up native data
        /// </summary>
        /// <param name="pNativeData"></param>
        public void CleanUpNativeData(IntPtr pNativeData)
        {
            Marshal.FreeHGlobal(pNativeData);
        }

        /// <summary>
        /// Get native data size
        /// </summary>        
        public int GetNativeDataSize()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Marshal managed to native
        /// </summary>
        public IntPtr MarshalManagedToNative(object ManagedObj)
        {
            return WaveFormat.MarshalToPtr((WaveFormat)ManagedObj);
        }

        /// <summary>
        /// Marshal Native to Managed
        /// </summary>
        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            return WaveFormat.MarshalFromPtr(pNativeData);
        }
    }
}
