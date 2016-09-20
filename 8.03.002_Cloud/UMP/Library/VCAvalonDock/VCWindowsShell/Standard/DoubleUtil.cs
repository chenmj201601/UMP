//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    17dd1186-4bc9-47b9-b567-102579f57362
//        CLR Version:              4.0.30319.18444
//        Name:                     DoubleUtil
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Windows.Shells.Standard
//        File Name:                DoubleUtil
//
//        created by Charley at 2014/7/22 13:38:06
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace VoiceCyber.Windows.Shells.Standard
{
    /// <summary>
    /// DoubleUtil uses fixed eps to provide fuzzy comparison functionality for doubles.
    /// Note that FP noise is a big problem and using any of these compare 
    /// methods is not a complete solution, but rather the way to reduce 
    /// the probability of repeating unnecessary work.
    /// </summary>
    internal static class DoubleUtilities
    {
        /// <summary>
        /// Epsilon - more or less random, more or less small number.
        /// </summary>
        private const double Epsilon = 0.00000153;

        /// <summary>
        /// AreClose returns whether or not two doubles are "close".  That is, whether or 
        /// not they are within epsilon of each other.
        /// There are plenty of ways for this to return false even for numbers which
        /// are theoretically identical, so no code calling this should fail to work if this 
        /// returns false. 
        /// </summary>
        /// <param name="value1">The first double to compare.</param>
        /// <param name="value2">The second double to compare.</param>
        /// <returns>The result of the AreClose comparision.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool AreClose(double value1, double value2)
        {
            if (value1 == value2)
            {
                return true;
            }

            double delta = value1 - value2;
            return (delta < Epsilon) && (delta > -Epsilon);
        }

        /// <summary>
        /// LessThan returns whether or not the first double is less than the second double.
        /// That is, whether or not the first is strictly less than *and* not within epsilon of
        /// the other number.
        /// There are plenty of ways for this to return false even for numbers which
        /// are theoretically identical, so no code calling this should fail to work if this 
        /// returns false.
        /// </summary>
        /// <param name="value1">The first double to compare.</param>
        /// <param name="value2">The second double to compare.</param>
        /// <returns>The result of the LessThan comparision.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool LessThan(double value1, double value2)
        {
            return (value1 < value2) && !AreClose(value1, value2);
        }

        /// <summary>
        /// GreaterThan returns whether or not the first double is greater than the second double.
        /// That is, whether or not the first is strictly greater than *and* not within epsilon of
        /// the other number.
        /// There are plenty of ways for this to return false even for numbers which
        /// are theoretically identical, so no code calling this should fail to work if this 
        /// returns false.
        /// </summary>
        /// <param name="value1">The first double to compare.</param>
        /// <param name="value2">The second double to compare.</param>
        /// <returns>The result of the GreaterThan comparision.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool GreaterThan(double value1, double value2)
        {
            return (value1 > value2) && !AreClose(value1, value2);
        }

        /// <summary>
        /// LessThanOrClose returns whether or not the first double is less than or close to
        /// the second double.  That is, whether or not the first is strictly less than or within
        /// epsilon of the other number.
        /// There are plenty of ways for this to return false even for numbers which
        /// are theoretically identical, so no code calling this should fail to work if this 
        /// returns false.
        /// </summary>
        /// <param name="value1">The first double to compare.</param>
        /// <param name="value2">The second double to compare.</param>
        /// <returns>The result of the LessThanOrClose comparision.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool LessThanOrClose(double value1, double value2)
        {
            return (value1 < value2) || AreClose(value1, value2);
        }

        /// <summary>
        /// GreaterThanOrClose returns whether or not the first double is greater than or close to
        /// the second double.  That is, whether or not the first is strictly greater than or within
        /// epsilon of the other number.
        /// There are plenty of ways for this to return false even for numbers which
        /// are theoretically identical, so no code calling this should fail to work if this 
        /// returns false.
        /// </summary>
        /// <param name="value1">The first double to compare.</param>
        /// <param name="value2">The second double to compare.</param>
        /// <returns>The result of the GreaterThanOrClose comparision.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool GreaterThanOrClose(double value1, double value2)
        {
            return (value1 > value2) || AreClose(value1, value2);
        }

        /// <summary>
        /// Test to see if a double is a finite number (is not NaN or Infinity).
        /// </summary>
        /// <param name='value'>The value to test.</param>
        /// <returns>Whether or not the value is a finite number.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }

        /// <summary>
        /// Test to see if a double a valid size value (is finite and > 0).
        /// </summary>
        /// <param name='value'>The value to test.</param>
        /// <returns>Whether or not the value is a valid size value.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool IsValidSize(double value)
        {
            return IsFinite(value) && GreaterThanOrClose(value, 0);
        }
    }
}
