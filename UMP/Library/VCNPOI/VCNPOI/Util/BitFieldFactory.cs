//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    03f26bc6-abb6-4f77-bc8a-2229f77eb9a0
//        CLR Version:              4.0.30319.42000
//        Name:                     BitFieldFactory
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.Util
//        File Name:                BitFieldFactory
//
//        Created by Charley at 2016/9/30 16:47:24
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections;


namespace VoiceCyber.NPOI.Util
{
    /// <summary>
    /// Returns immutable Btfield instances.
    /// @author Jason Height (jheight at apache dot org)
    /// </summary>
    public class BitFieldFactory
    {
        //use Hashtable to replace HashMap
        private static Hashtable instances = new Hashtable();

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <param name="mask">The mask.</param>
        /// <returns></returns>
        public static BitField GetInstance(int mask)
        {
            BitField f = (BitField)instances[mask];
            if (f == null)
            {
                f = new BitField(mask);
                instances[mask] = f;
            }
            return f;
        }
    }
}
