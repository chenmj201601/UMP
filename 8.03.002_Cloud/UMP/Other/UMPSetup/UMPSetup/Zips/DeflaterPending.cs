//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b27cbedc-6232-47e4-9d14-648708d16265
//        CLR Version:              4.0.30319.18063
//        Name:                     DeflaterPending
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPSetup.Zips
//        File Name:                DeflaterPending
//
//        created by Charley at 2015/12/29 9:52:19
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPSetup.Zips
{
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
