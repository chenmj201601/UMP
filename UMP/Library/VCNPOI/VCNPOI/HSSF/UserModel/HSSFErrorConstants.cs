//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    caf28651-893e-49b6-824c-2e0ef671d12c
//        CLR Version:              4.0.30319.42000
//        Name:                     HSSFErrorConstants
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NPOI.HSSF.UserModel
//        File Name:                HSSFErrorConstants
//
//        Created by Charley at 2016/9/30 15:51:02
//        http://www.voicecyber.com 
//
//======================================================================

using System;


namespace VoiceCyber.NPOI.HSSF.UserModel
{
    /// <summary>
    /// Contains raw Excel error codes (as defined in OOO's excelfileformat.pdf (2.5.6)
    /// @author  Michael Harhen
    /// </summary>
    public class HSSFErrorConstants
    {
        private HSSFErrorConstants()
        {
            // no instances of this class
        }

        /** <b>#NULL!</b>  - Intersection of two cell ranges is empty */
        public const int ERROR_NULL = 0x00;
        /** <b>#DIV/0!</b> - Division by zero */
        public const int ERROR_DIV_0 = 0x07;
        /** <b>#VALUE!</b> - Wrong type of operand */
        public const int ERROR_VALUE = 0x0F;
        /** <b>#REF!</b> - Illegal or deleted cell reference */
        public const int ERROR_REF = 0x17;
        /** <b>#NAME?</b> - Wrong function or range name */
        public const int ERROR_NAME = 0x1D;
        /** <b>#NUM!</b> - Value range overflow */
        public const int ERROR_NUM = 0x24;
        /** <b>#N/A</b> - Argument or function not available */
        public const int ERROR_NA = 0x2A;


        /// <summary>
        /// Gets standard Excel error literal for the specified error code.
        /// @throws ArgumentException if the specified error code is not one of the 7
        /// standard error codes
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <returns></returns>
        public static String GetText(int errorCode)
        {
            if (errorCode == ERROR_NULL)
            {
                return "#NULL!";
            }
            if (errorCode == ERROR_DIV_0)
            {
                return "#DIV/0!";
            }
            if (errorCode == ERROR_VALUE)
            {
                return "#VALUE!";
            }
            if (errorCode == ERROR_REF)
            {
                return "#REF!";
            }
            if (errorCode == ERROR_NAME)
            {
                return "#NAME?";
            }
            if (errorCode == ERROR_NUM)
            {
                return "#NUM!";
            }
            if (errorCode == ERROR_NA)
            {
                return "#N/A";
            }
            throw new ArgumentException("Bad error code (" + errorCode + ")");
        }

        /// <summary>
        /// Determines whether [is valid code] [the specified error code].
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <returns>
        /// 	<c>true</c> if the specified error code is a standard Excel error code.; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidCode(int errorCode)
        {
            // This method exists because it would be bad to force clients to catch 
            // ArgumentException if there were potential for passing an invalid error code.

            if (errorCode == ERROR_NULL
                || errorCode == ERROR_DIV_0
                || errorCode == ERROR_VALUE
                || errorCode == ERROR_REF
                || errorCode == ERROR_NAME
                || errorCode == ERROR_NUM
                || errorCode == ERROR_NA
                )
            {
                return true;
            }

            return false;
        }
    }
}
