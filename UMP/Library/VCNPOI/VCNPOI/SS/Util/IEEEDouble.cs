//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    8b269d1d-cd56-4bc5-be9a-447159aad33d
//        CLR Version:              4.0.30319.42000
//        Name:                     IEEEDouble
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.SS.Util
//        File Name:                IEEEDouble
//
//        Created by Charley at 2016/9/30 16:27:06
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.NPOI.SS.Util
{
    public class IEEEDouble
    {
        private const long EXPONENT_MASK = 0x7FF0000000000000L;
        private const int EXPONENT_SHIFT = 52;
        public const long FRAC_MASK = 0x000FFFFFFFFFFFFFL;
        public const int EXPONENT_BIAS = 1023;
        public const long FRAC_ASSUMED_HIGH_BIT = (1L << EXPONENT_SHIFT);
        /**
         * The value the exponent field Gets for all <i>NaN</i> and <i>InfInity</i> values
         */
        public const int BIASED_EXPONENT_SPECIAL_VALUE = 0x07FF;

        /**
         * @param rawBits the 64 bit binary representation of the double value
         * @return the top 12 bits (sign and biased exponent value)
         */
        public static int GetBiasedExponent(long rawBits)
        {
            return (int)((rawBits & EXPONENT_MASK) >> EXPONENT_SHIFT);
        }
    }
}
