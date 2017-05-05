//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    12d9f2bc-4ae8-4830-b0cc-4dc282a5fd68
//        CLR Version:              4.0.30319.42000
//        Name:                     Operator
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.Util
//        File Name:                Operator
//
//        Created by Charley at 2016/9/30 16:16:50
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.NPOI.Util
{
    public static class Operator
    {
        public static int UnsignedRightShift(int operand, int val)
        {
            if (operand > 0)
                return operand >> val;
            return (int)(((uint)operand) >> val);
        }

        public static long UnsignedRightShift(long operand, int val)
        {
            if (operand > 0)
                return operand >> val;
            return (long)(((ulong)operand) >> val);
        }

        public static short UnsignedRightShift(short operand, int val)
        {
            if (operand > 0)
                return (short)(operand >> val);
            return (short)(((ushort)operand) >> val);
        }

        public static sbyte UnsignedRightShift(sbyte operand, int val)
        {
            if (operand > 0)
                return (sbyte)(operand >> val);
            return (sbyte)(((byte)operand) >> val);
        }
    }
}
