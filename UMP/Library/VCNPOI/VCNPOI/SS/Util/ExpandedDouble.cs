//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    0c4e26bf-103a-4132-9d43-068a82dfc467
//        CLR Version:              4.0.30319.42000
//        Name:                     ExpandedDouble
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.SS.Util
//        File Name:                ExpandedDouble
//
//        Created by Charley at 2016/9/30 16:15:25
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using VoiceCyber.NPOI.Util;


namespace VoiceCyber.NPOI.SS.Util
{
    /*
     * Represents a 64 bit IEEE double quantity expressed with both decimal and binary exponents
     * Does not handle negative numbers or zero
     * <p/>
     * The value of a {@link ExpandedDouble} is given by<br/>
     * <c> a &times; 2<sup>b</sup></c>
     * <br/>
     * where:<br/>
     *
     * <c>a</c> = <i>significand</i><br/>
     * <c>b</c> = <i>binaryExponent</i> - bitLength(significand) + 1<br/>
     *
     * @author Josh Micich
     */
    public class ExpandedDouble
    {
        private static readonly BigInteger BI_FRAC_MASK = new BigInteger(IEEEDouble.FRAC_MASK);
        private static readonly BigInteger BI_IMPLIED_FRAC_MSB = new BigInteger(IEEEDouble.FRAC_ASSUMED_HIGH_BIT);

        private static BigInteger GetFrac(long rawBits)
        {
            return (new BigInteger(rawBits) & BI_FRAC_MASK | BI_IMPLIED_FRAC_MSB) << 11;
        }


        public static ExpandedDouble FromRawBitsAndExponent(long rawBits, int exp)
        {
            return new ExpandedDouble(GetFrac(rawBits), exp);
        }

        /**
         * Always 64 bits long (MSB, bit-63 is '1')
         */
        private BigInteger _significand;
        private int _binaryExponent;

        public ExpandedDouble(long rawBits)
        {
            int biasedExp = (int)(rawBits >> 52);
            if (biasedExp == 0)
            {
                // sub-normal numbers
                BigInteger frac = new BigInteger(rawBits) & BI_FRAC_MASK;
                int expAdj = 64 - frac.BitLength();
                _significand = frac << expAdj;
                _binaryExponent = (biasedExp & 0x07FF) - 1023 - expAdj;
            }
            else
            {
                BigInteger frac = GetFrac(rawBits);
                _significand = frac;
                _binaryExponent = (biasedExp & 0x07FF) - 1023;
            }
        }

        public ExpandedDouble(BigInteger frac, int binaryExp)
        {
            if (frac.BitLength() != 64)
            {
                throw new ArgumentException("bad bit length");
            }
            _significand = frac;
            _binaryExponent = binaryExp;
        }


        /**
         * Convert to an equivalent {@link NormalisedDecimal} representation having 15 decimal digits of precision in the
         * non-fractional bits of the significand.
         */
        public NormalisedDecimal NormaliseBaseTen()
        {
            return NormalisedDecimal.Create(_significand, _binaryExponent);
        }

        /**
         * @return the number of non-fractional bits after the MSB of the significand
         */
        public int GetBinaryExponent()
        {
            return _binaryExponent;
        }

        public BigInteger GetSignificand()
        {
            return _significand;
        }
    }
}
