//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3043fd23-8204-43ea-9f17-79949de8cfab
//        CLR Version:              4.0.30319.18063
//        Name:                     DeflaterPending
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.SharpZips.Zip.Compression
//        File Name:                DeflaterPending
//
//        created by Charley at 2015/7/22 16:53:43
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.SharpZips.Zip.Compression
{
    /// <summary>
    /// This class stores the pending output of the Deflater.
    /// 
    /// author of the original java version : Jochen Hoenicke
    /// </summary>
    public class DeflaterPending : PendingBuffer
    {
        /// <summary>
        /// Construct instance with default buffer size
        /// </summary>
        public DeflaterPending()
            : base(DeflaterConstants.PENDING_BUF_SIZE)
        {
        }
    }
}
