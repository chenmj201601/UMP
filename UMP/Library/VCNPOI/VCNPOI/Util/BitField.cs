//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    db94c1c4-0f66-4e30-86c9-e041170ff097
//        CLR Version:              4.0.30319.42000
//        Name:                     BitField
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.Util
//        File Name:                BitField
//
//        Created by Charley at 2016/9/30 16:46:22
//        http://www.voicecyber.com 
//
//======================================================================

using System;


namespace VoiceCyber.NPOI.Util
{
    /// <summary>
    /// Manage operations dealing with bit-mapped fields.
    /// @author Marc Johnson (mjohnson at apache dot org)
    /// @author Andrew C. Oliver (acoliver at apache dot org)
    /// </summary>
    [Serializable]
    public class BitField
    {

        private int _mask;
        private int _shift_count;

        /// <summary>
        /// Create a <see cref="BitField"/> instance
        /// </summary>
        /// <param name="mask">
        /// the mask specifying which bits apply to this
        /// BitField. Bits that are set in this mask are the
        /// bits that this BitField operates on
        /// </param>
        public BitField(int mask)
        {
            this._mask = mask;
            int num = 0;
            int num2 = mask;
            if (num2 != 0)
            {
                while ((num2 & 1) == 0)
                {
                    num++;
                    num2 = num2 >> 1;
                }
            }
            this._shift_count = num;
        }
        /// <summary>
        /// Create a <see cref="BitField"/> instance
        /// </summary>
        /// <param name="mask">
        /// the mask specifying which bits apply to this
        /// BitField. Bits that are set in this mask are the
        /// bits that this BitField operates on
        /// </param>
        public BitField(uint mask)
            : this((int)mask)
        {

        }
        /// <summary>
        /// Clear the bits.
        /// </summary>
        /// <param name="holder">the int data containing the bits we're interested in</param>
        /// <returns>the value of holder with the specified bits cleared (set to 0)</returns>
        public int Clear(int holder)
        {
            return (holder & ~this._mask);
        }

        /// <summary>
        /// Clear the bits.
        /// </summary>
        /// <param name="holder">the short data containing the bits we're interested in</param>
        /// <returns>the value of holder with the specified bits cleared (set to 0)</returns>
        public short ClearShort(short holder)
        {
            return (short)this.Clear(holder);
        }

        /// <summary>
        /// Obtain the value for the specified BitField, appropriately
        /// shifted right. Many users of a BitField will want to treat the
        /// specified bits as an int value, and will not want to be aware
        /// that the value is stored as a BitField (and so shifted left so
        /// many bits)
        /// </summary>
        /// <param name="holder">the int data containing the bits we're interested in</param>
        /// <returns>the selected bits, shifted right appropriately</returns>
        public int GetRawValue(int holder)
        {
            return (holder & this._mask);
        }

        /// <summary>
        /// Obtain the value for the specified BitField, unshifted
        /// </summary>
        /// <param name="holder">the short data containing the bits we're interested in</param>
        /// <returns>the selected bits</returns>
        public short GetShortRawValue(short holder)
        {
            return (short)this.GetRawValue(holder);
        }

        /// <summary>
        /// Obtain the value for the specified BitField, appropriately
        /// shifted right, as a short. Many users of a BitField will want
        /// to treat the specified bits as an int value, and will not want
        /// to be aware that the value is stored as a BitField (and so
        /// shifted left so many bits)
        /// </summary>
        /// <param name="holder">the short data containing the bits we're interested in</param>
        /// <returns>the selected bits, shifted right appropriately</returns>
        public short GetShortValue(short holder)
        {
            return (short)this.GetValue(holder);
        }

        /// <summary>
        /// Obtain the value for the specified BitField, appropriately
        /// shifted right. Many users of a BitField will want to treat the
        /// specified bits as an int value, and will not want to be aware
        /// that the value is stored as a BitField (and so shifted left so
        /// many bits)
        /// </summary>
        /// <param name="holder">the int data containing the bits we're interested in</param>
        /// <returns>the selected bits, shifted right appropriately</returns>
        public int GetValue(int holder)
        {
            return Operator.UnsignedRightShift(this.GetRawValue(holder), this._shift_count);
        }

        /// <summary>
        /// Are all of the bits set or not? This is a stricter test than
        /// isSet, in that all of the bits in a multi-bit set must be set
        /// for this method to return true
        /// </summary>
        /// <param name="holder">the int data containing the bits we're interested in</param>
        /// <returns>
        /// 	<c>true</c> if all of the bits are set otherwise, <c>false</c>.
        /// </returns>
        public bool IsAllSet(int holder)
        {
            return ((holder & this._mask) == this._mask);
        }

        /// <summary>
        /// is the field set or not? This is most commonly used for a
        /// single-bit field, which is often used to represent a boolean
        /// value; the results of using it for a multi-bit field is to
        /// determine whether *any* of its bits are set
        /// </summary>
        /// <param name="holder">the int data containing the bits we're interested in</param>
        /// <returns>
        /// 	<c>true</c> if any of the bits are set; otherwise, <c>false</c>.
        /// </returns>
        public bool IsSet(int holder)
        {
            return ((holder & this._mask) != 0);
        }

        /// <summary>
        /// Set the bits.
        /// </summary>
        /// <param name="holder">the int data containing the bits we're interested in</param>
        /// <returns>the value of holder with the specified bits set to 1</returns>
        public int Set(int holder)
        {
            return (holder | this._mask);
        }

        /// <summary>
        /// Set a boolean BitField
        /// </summary>
        /// <param name="holder">the int data containing the bits we're interested in</param>
        /// <param name="flag">indicating whether to set or clear the bits</param>
        /// <returns>the value of holder with the specified bits set or cleared</returns>
        public int SetBoolean(int holder, bool flag)
        {
            return (!flag ? this.Clear(holder) : this.Set(holder));
        }

        /// <summary>
        /// Set the bits.
        /// </summary>
        /// <param name="holder">the short data containing the bits we're interested in</param>
        /// <returns>the value of holder with the specified bits set to 1</returns>
        public short SetShort(short holder)
        {
            return (short)this.Set(holder);
        }

        /// <summary>
        /// Set a boolean BitField
        /// </summary>
        /// <param name="holder">the short data containing the bits we're interested in</param>
        /// <param name="flag">indicating whether to set or clear the bits</param>
        /// <returns>the value of holder with the specified bits set or cleared</returns>
        public short SetShortBoolean(short holder, bool flag)
        {
            return (!flag ? this.ClearShort(holder) : this.SetShort(holder));
        }

        /// <summary>
        /// Obtain the value for the specified BitField, appropriately
        /// shifted right, as a short. Many users of a BitField will want
        /// to treat the specified bits as an int value, and will not want
        /// to be aware that the value is stored as a BitField (and so
        /// shifted left so many bits)
        /// </summary>
        /// <param name="holder">the short data containing the bits we're interested in</param>
        /// <param name="value">the new value for the specified bits</param>
        /// <returns>the selected bits, shifted right appropriately</returns>
        public short SetShortValue(short holder, short value)
        {
            return (short)this.SetValue(holder, value);
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="holder">the byte data containing the bits we're interested in</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public int SetValue(int holder, int value)
        {
            return ((holder & ~this._mask) | ((value << this._shift_count) & this._mask));
        }

        /// <summary>
        /// Set a boolean BitField
        /// </summary>
        /// <param name="holder"> the byte data containing the bits we're interested in</param>
        /// <param name="flag">indicating whether to set or clear the bits</param>
        /// <returns>the value of holder with the specified bits set or cleared</returns>
        public byte SetByteBoolean(byte holder, bool flag)
        {
            return (!flag ? this.ClearByte(holder) : this.SetByte(holder));
        }
        /// <summary>
        /// Clears the bits.
        /// </summary>
        /// <param name="holder">the byte data containing the bits we're interested in</param>
        /// <returns>the value of holder with the specified bits cleared</returns>
        public byte ClearByte(byte holder)
        {
            return (byte)this.Clear(holder);
        }
        /// <summary>
        /// Set the bits.
        /// </summary>
        /// <param name="holder">the byte data containing the bits we're interested in</param>
        /// <returns>the value of holder with the specified bits set to 1</returns>
        public byte SetByte(byte holder)
        {
            return (byte)this.Set(holder);
        }
    }
}
